// <copyright>
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Runtime
{
    using System.Threading.Tasks;
    using Microsoft.Server.Common;
    using TaskDispatcherBehavior;

    internal static class TaskExtensions
    {
        private static readonly Type taskType = typeof(Task);

        public static IAsyncResult AsAsyncResult<T>(this Task<T> task, AsyncCallback callback, object state)
        {
            if (task == null)
            {
                throw Fx.Exception.ArgumentNull("task");
            }

            if (task.Status == TaskStatus.Created)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxTaskNotStarted));
            }

            var tcs = new TaskCompletionSource<T>(state);

            task.ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    }
                    else if (t.IsCanceled)
                    {
                        // the use of Task.ContinueWith(,TaskContinuationOptions.OnlyOnRanToCompletion)
                        // can give us a cancelled Task here with no t.Exception.
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(t.Result);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                }, 
                TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static IAsyncResult AsAsyncResult(this Task task, AsyncCallback callback, object state)
        {
            if (task == null)
            {
                throw Fx.Exception.ArgumentNull("task");
            }

            if (task.Status == TaskStatus.Created)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR2.SFxTaskNotStarted));
            }

            var tcs = new TaskCompletionSource<object>(state);

            task.ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    }
                    else if (t.IsCanceled)
                    {
                        // the use of Task.ContinueWith(,TaskContinuationOptions.OnlyOnRanToCompletion)
                        // can give us a cancelled Task here with no t.Exception.
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                }, 
                TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}