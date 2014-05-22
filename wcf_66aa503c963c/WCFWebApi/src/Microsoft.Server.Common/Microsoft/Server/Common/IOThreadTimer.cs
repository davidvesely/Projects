//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.Threading;
    using Microsoft.Server.Common.Interop;
    using Microsoft.Win32.SafeHandles;

    // IOThreadTimer has several characterstics that are important for performance:
    // - Timers that expire benefit from being scheduled to run on IO threads using IOThreadScheduler.Schedule.
    // - The timer "waiter" thread thread is only allocated if there are set timers.
    // - The timer waiter thread itself is an IO thread, which allows it to go away if there is no need for it,
    //   and allows it to be reused for other purposes.
    // - After the timer count goes to zero, the timer waiter thread remains active for a bounded amount
    //   of time to wait for additional timers to be set.
    // - Timers are stored in an array-based priority queue to reduce the amount of time spent in updates, and
    //   to always provide O(1) access to the minimum timer (the first one that will expire).
    // - The standard textbook priority queue data structure is extended to allow efficient Delete in addition to 
    //   DeleteMin for efficient handling of canceled timers.
    // - Timers that are typically set, then immediately canceled (such as a retry timer, 
    //   or a flush timer), are tracked separately from more stable timers, to avoid having 
    //   to update the waitable timer in the typical case when a timer is canceled.  Whether 
    //   a timer instance follows this pattern is specified when the timer is constructed.
    // - Extending a timer by a configurable time delta (maxSkew) does not involve updating the
    //   waitable timer, or taking a lock.
    // - Timer instances are relatively cheap.  They share "heavy" resources like the waiter thread and 
    //   waitable timer handle.
    // - Setting or canceling a timer does not typically involve any allocations.

    class IOThreadTimer
    {
        const int maxSkewInMillisecondsDefault = 100;
        static long systemTimeResolutionTicks = -1;
        Action<object> callback;
        object callbackState;
        long dueTime;

        int index;
        long maxSkew;
        TimerGroup timerGroup;

        public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
            : this(callback, callbackState, isTypicallyCanceledShortlyAfterBeingSet, maxSkewInMillisecondsDefault)
        {
        }

        public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet, int maxSkewInMilliseconds)
        {
            this.callback = callback;
            this.callbackState = callbackState;
            this.maxSkew = Ticks.FromMilliseconds(maxSkewInMilliseconds);
            this.timerGroup =
                (isTypicallyCanceledShortlyAfterBeingSet ? TimerManager.Value.VolatileTimerGroup : TimerManager.Value.StableTimerGroup);
        }

        public static long SystemTimeResolutionTicks
        {
            get
            {
                if (IOThreadTimer.systemTimeResolutionTicks == -1)
                {
                    IOThreadTimer.systemTimeResolutionTicks = GetSystemTimeResolution();
                }
                return IOThreadTimer.systemTimeResolutionTicks;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Calls critical method GetSystemTimeAdjustment", Safe = "method is a SafeNativeMethod")]
        [SecuritySafeCritical]
        static long GetSystemTimeResolution()
        {
            int dummyAdjustment;
            uint increment;
            uint dummyAdjustmentDisabled;

            if (UnsafeNativeMethods.GetSystemTimeAdjustment(out dummyAdjustment, out increment, out dummyAdjustmentDisabled) != 0)
            {
                return (long)increment;
            }

            // Assume the default, which is around 15 milliseconds.
            return 15 * TimeSpan.TicksPerMillisecond;
        }

        public bool Cancel()
        {
            return TimerManager.Value.Cancel(this);
        }

        public void Set(TimeSpan timeFromNow)
        {
            if (timeFromNow != TimeSpan.MaxValue)
            {
                SetAt(Ticks.Add(Ticks.Now, Ticks.FromTimeSpan(timeFromNow)));
            }
        }

        public void Set(int millisecondsFromNow)
        {
            SetAt(Ticks.Add(Ticks.Now, Ticks.FromMilliseconds(millisecondsFromNow)));
        }

        public void SetAt(long time)
        {
            TimerManager.Value.Set(this, time);
        }

        [Fx.Tag.SynchronizationObject(Blocking = false, Scope = Fx.Tag.Strings.AppDomain)]
        class TimerManager
        {
            const long maxTimeToWaitForMoreTimers = 1000 * TimeSpan.TicksPerMillisecond;

            [Fx.Tag.Queue(typeof(IOThreadTimer), Scope = Fx.Tag.Strings.AppDomain, StaleElementsRemovedImmediately = true)]
            static TimerManager value = new TimerManager();

            Action<object> onWaitCallback;
            TimerGroup stableTimerGroup;
            TimerGroup volatileTimerGroup;
            [Fx.Tag.SynchronizationObject(Blocking = false)]
            WaitableTimer[] waitableTimers;

            bool waitScheduled;

            public TimerManager()
            {
                this.onWaitCallback = new Action<object>(OnWaitCallback);
                this.stableTimerGroup = new TimerGroup();
                this.volatileTimerGroup = new TimerGroup();
                this.waitableTimers = new WaitableTimer[] { this.stableTimerGroup.WaitableTimer, this.volatileTimerGroup.WaitableTimer };
            }

            object ThisLock
            {
                get { return this; }
            }

            public static TimerManager Value
            {
                get
                {
                    return TimerManager.value;
                }
            }

            public TimerGroup StableTimerGroup
            {
                get
                {
                    return this.stableTimerGroup;
                }
            }
            public TimerGroup VolatileTimerGroup
            {
                get
                {
                    return this.volatileTimerGroup;
                }
            }

            public void Set(IOThreadTimer timer, long time)
            {
                long timeDiff = time - timer.dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }

                if (timeDiff > timer.maxSkew)
                {
                    lock (ThisLock)
                    {
                        TimerGroup timerGroup = timer.timerGroup;
                        TimerQueue timerQueue = timerGroup.TimerQueue;

                        if (timer.index > 0)
                        {
                            if (timerQueue.UpdateTimer(timer, time))
                            {
                                UpdateWaitableTimer(timerGroup);
                            }
                        }
                        else
                        {
                            if (timerQueue.InsertTimer(timer, time))
                            {
                                UpdateWaitableTimer(timerGroup);

                                if (timerQueue.Count == 1)
                                {
                                    EnsureWaitScheduled();
                                }
                            }
                        }
                    }
                }
            }

            public bool Cancel(IOThreadTimer timer)
            {
                lock (ThisLock)
                {
                    if (timer.index > 0)
                    {
                        TimerGroup timerGroup = timer.timerGroup;
                        TimerQueue timerQueue = timerGroup.TimerQueue;

                        timerQueue.DeleteTimer(timer);

                        if (timerQueue.Count > 0)
                        {
                            UpdateWaitableTimer(timerGroup);
                        }
                        else
                        {
                            TimerGroup otherTimerGroup = GetOtherTimerGroup(timerGroup);
                            if (otherTimerGroup.TimerQueue.Count == 0)
                            {
                                long now = Ticks.Now;
                                long thisGroupRemainingTime = timerGroup.WaitableTimer.DueTime - now;
                                long otherGroupRemainingTime = otherTimerGroup.WaitableTimer.DueTime - now;
                                if (thisGroupRemainingTime > maxTimeToWaitForMoreTimers &&
                                    otherGroupRemainingTime > maxTimeToWaitForMoreTimers)
                                {
                                    timerGroup.WaitableTimer.Set(Ticks.Add(now, maxTimeToWaitForMoreTimers));
                                }
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            void EnsureWaitScheduled()
            {
                if (!this.waitScheduled)
                {
                    ScheduleWait();
                }
            }

            TimerGroup GetOtherTimerGroup(TimerGroup timerGroup)
            {
                if (object.ReferenceEquals(timerGroup, this.volatileTimerGroup))
                {
                    return this.stableTimerGroup;
                }
                else
                {
                    return this.volatileTimerGroup;
                }
            }

            void OnWaitCallback(object state)
            {
                WaitHandle.WaitAny(this.waitableTimers);
                long now = Ticks.Now;
                lock (ThisLock)
                {
                    this.waitScheduled = false;
                    ScheduleElapsedTimers(now);
                    ReactivateWaitableTimers();
                    ScheduleWaitIfAnyTimersLeft();
                }
            }

            void ReactivateWaitableTimers()
            {
                ReactivateWaitableTimer(this.stableTimerGroup);
                ReactivateWaitableTimer(this.volatileTimerGroup);
            }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
            void ReactivateWaitableTimer(TimerGroup timerGroup)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;

                if (timerQueue.Count > 0)
                {
                    timerGroup.WaitableTimer.Set(timerQueue.MinTimer.dueTime);
                }
                else
                {
                    timerGroup.WaitableTimer.Set(long.MaxValue);
                }
            }

            void ScheduleElapsedTimers(long now)
            {
                ScheduleElapsedTimers(this.stableTimerGroup, now);
                ScheduleElapsedTimers(this.volatileTimerGroup, now);
            }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
            void ScheduleElapsedTimers(TimerGroup timerGroup, long now)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;
                while (timerQueue.Count > 0)
                {
                    IOThreadTimer timer = timerQueue.MinTimer;
                    long timeDiff = timer.dueTime - now;
                    if (timeDiff <= timer.maxSkew)
                    {
                        timerQueue.DeleteMinTimer();
                        ActionItem.Schedule(timer.callback, timer.callbackState);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            void ScheduleWait()
            {
                ActionItem.Schedule(this.onWaitCallback, null);
                this.waitScheduled = true;
            }

            void ScheduleWaitIfAnyTimersLeft()
            {
                if (this.stableTimerGroup.TimerQueue.Count > 0 ||
                    this.volatileTimerGroup.TimerQueue.Count > 0)
                {
                    ScheduleWait();
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
            void UpdateWaitableTimer(TimerGroup timerGroup)
            {
                WaitableTimer waitableTimer = timerGroup.WaitableTimer;
                IOThreadTimer minTimer = timerGroup.TimerQueue.MinTimer;
                long timeDiff = waitableTimer.DueTime - minTimer.dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }
                if (timeDiff > minTimer.maxSkew)
                {
                    waitableTimer.Set(minTimer.dueTime);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ported from WCF")]
        class TimerGroup
        {
            TimerQueue timerQueue;
            WaitableTimer waitableTimer;

            public TimerGroup()
            {
                this.waitableTimer = new WaitableTimer();
                this.waitableTimer.Set(long.MaxValue);
                this.timerQueue = new TimerQueue();
            }

            public TimerQueue TimerQueue
            {
                get
                {
                    return this.timerQueue;
                }
            }
            public WaitableTimer WaitableTimer
            {
                get
                {
                    return this.waitableTimer;
                }
            }
        }

        class TimerQueue
        {
            int count;
            IOThreadTimer[] timers;

            public TimerQueue()
            {
                this.timers = new IOThreadTimer[4];
            }

            public int Count
            {
                get { return count; }
            }

            public IOThreadTimer MinTimer
            {
                get
                {
                    Fx.Assert(this.count > 0, "Should have at least one timer in our queue.");
                    return timers[1];
                }
            }
            public void DeleteMinTimer()
            {
                IOThreadTimer minTimer = this.MinTimer;
                DeleteMinTimerCore();
                minTimer.index = 0;
                minTimer.dueTime = 0;
            }
            public void DeleteTimer(IOThreadTimer timer)
            {
                int index = timer.index;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= this.count, "");

                IOThreadTimer[] localTimers = this.timers;

                for (;;)
                {
                    int parentIndex = index / 2;

                    if (parentIndex >= 1)
                    {
                        IOThreadTimer parentTimer = localTimers[parentIndex];
                        localTimers[index] = parentTimer;
                        parentTimer.index = index;
                    }
                    else
                    {
                        break;
                    }

                    index = parentIndex;
                }

                timer.index = 0;
                timer.dueTime = 0;
                localTimers[1] = null;
                DeleteMinTimerCore();
            }

            public bool InsertTimer(IOThreadTimer timer, long dueTime)
            {
                Fx.Assert(timer.index == 0, "Timer should not have an index.");

                IOThreadTimer[] localTimers = this.timers;

                int index = this.count + 1;

                if (index == localTimers.Length)
                {
                    localTimers = new IOThreadTimer[localTimers.Length * 2];
                    Array.Copy(this.timers, localTimers, this.timers.Length);
                    this.timers = localTimers;
                }

                this.count = index;

                if (index > 1)
                {
                    for (;;)
                    {
                        int parentIndex = index / 2;

                        if (parentIndex == 0)
                        {
                            break;
                        }

                        IOThreadTimer parent = localTimers[parentIndex];

                        if (parent.dueTime > dueTime)
                        {
                            localTimers[index] = parent;
                            parent.index = index;
                            index = parentIndex;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                localTimers[index] = timer;
                timer.index = index;
                timer.dueTime = dueTime;
                return index == 1;
            }

            public bool UpdateTimer(IOThreadTimer timer, long dueTime)
            {
                int index = timer.index;

                IOThreadTimer[] localTimers = this.timers;
                int localCount = this.count;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= localCount, "");

                int parentIndex = index / 2;
                if (parentIndex == 0 ||
                    localTimers[parentIndex].dueTime <= dueTime)
                {
                    int leftChildIndex = index * 2;
                    if (leftChildIndex > localCount ||
                        localTimers[leftChildIndex].dueTime >= dueTime)
                    {
                        int rightChildIndex = leftChildIndex + 1;
                        if (rightChildIndex > localCount ||
                            localTimers[rightChildIndex].dueTime >= dueTime)
                        {
                            timer.dueTime = dueTime;
                            return index == 1;
                        }
                    }
                }

                DeleteTimer(timer);
                InsertTimer(timer, dueTime);
                return true;
            }

            void DeleteMinTimerCore()
            {
                int localCount = this.count;

                if (localCount == 1)
                {
                    this.count = 0;
                    this.timers[1] = null;
                }
                else
                {
                    IOThreadTimer[] localTimers = this.timers;
                    IOThreadTimer lastTimer = localTimers[localCount];
                    this.count = --localCount;

                    int index = 1;
                    for (;;)
                    {
                        int leftChildIndex = index * 2;

                        if (leftChildIndex > localCount)
                        {
                            break;
                        }

                        int childIndex;
                        IOThreadTimer child;

                        if (leftChildIndex < localCount)
                        {
                            IOThreadTimer leftChild = localTimers[leftChildIndex];
                            int rightChildIndex = leftChildIndex + 1;
                            IOThreadTimer rightChild = localTimers[rightChildIndex];

                            if (rightChild.dueTime < leftChild.dueTime)
                            {
                                child = rightChild;
                                childIndex = rightChildIndex;
                            }
                            else
                            {
                                child = leftChild;
                                childIndex = leftChildIndex;
                            }
                        }
                        else
                        {
                            childIndex = leftChildIndex;
                            child = localTimers[childIndex];
                        }

                        if (lastTimer.dueTime > child.dueTime)
                        {
                            localTimers[index] = child;
                            child.index = index;
                        }
                        else
                        {
                            break;
                        }

                        index = childIndex;

                        if (leftChildIndex >= localCount)
                        {
                            break;
                        }
                    }

                    localTimers[index] = lastTimer;
                    lastTimer.index = index;
                    localTimers[localCount + 1] = null;
                }
            }
        }

        [Fx.Tag.SynchronizationPrimitive(Fx.Tag.BlocksUsing.NonBlocking)]
        class WaitableTimer : WaitHandle
        {

            long dueTime;

            [Fx.Tag.SecurityNote(Critical = "Call the critical CreateWaitableTimer method in TimerHelper",
                Safe = "Doesn't leak information or resources")]
            [SecuritySafeCritical]
            public WaitableTimer()
            {
                this.SafeWaitHandle = TimerHelper.CreateWaitableTimer();
            }

            public long DueTime
            {
                get { return this.dueTime; }
            }

            [Fx.Tag.SecurityNote(Critical = "Call the critical Set method in TimerHelper",
                Safe = "Doesn't leak information or resources")]
            [SecuritySafeCritical]
            public void Set(long time)
            {
                this.dueTime = TimerHelper.Set(this.SafeWaitHandle, time);
            }

            [Fx.Tag.SecurityNote(Critical = "Provides a set of unsafe methods used to work with the WaitableTimer")]
            [SecurityCritical]
            static class TimerHelper
            {
                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Code ported from WCF.")]
                public static unsafe SafeWaitHandle CreateWaitableTimer()
                {
                    SafeWaitHandle handle = UnsafeNativeMethods.CreateWaitableTimer(IntPtr.Zero, false, null);
                    if (handle.IsInvalid)
                    {
                        Exception exception = new Win32Exception();
                        handle.SetHandleAsInvalid();
                        throw Fx.Exception.AsError(exception);
                    }
                    return handle;
                }
                public static unsafe long Set(SafeWaitHandle timer, long dueTime)
                {
                    if (!UnsafeNativeMethods.SetWaitableTimer(timer, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false))
                    {
                        throw Fx.Exception.AsError(new Win32Exception());
                    }
                    return dueTime;
                }
            }
        }
    }
}

