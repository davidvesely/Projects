//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Versioning;
    using System.Security;
    using Microsoft.Server.Common.Diagnostics;
    using Microsoft.Server.Common.Interop;

    public class ExceptionTrace
    {
        const ushort FailFastEventLogCategory = 6;
        string eventSourceName;

        public ExceptionTrace()
        {
        }

        public ExceptionTrace(string eventSourceName)
        {
            this.eventSourceName = eventSourceName;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        public void AsInformation(Exception exception)
        {
            //Traces an informational trace message
            TraceCore.HandledException(Fx.Trace, exception);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        public void AsWarning(Exception exception)
        {
            //Traces a warning trace message
            TraceCore.HandledExceptionWarning(Fx.Trace, exception);
        }

        public Exception AsError(Exception exception)
        {
            // AggregateExceptions are automatically unwrapped.
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return AsError<Exception>(aggregateException);
            }

            // TargetInvocationExceptions are automatically unwrapped.
            TargetInvocationException targetInvocationException = exception as TargetInvocationException;
            if (targetInvocationException != null && targetInvocationException.InnerException != null)
            {
                return AsError(targetInvocationException.InnerException);
            }

            return TraceException<Exception>(exception);
        }

        public Exception AsError(Exception exception, string eventSource)
        {
            // AggregateExceptions are automatically unwrapped.
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return AsError<Exception>(aggregateException, eventSource);
            }

            // TargetInvocationExceptions are automatically unwrapped.
            TargetInvocationException targetInvocationException = exception as TargetInvocationException;
            if (targetInvocationException != null && targetInvocationException.InnerException != null)
            {
                return AsError(targetInvocationException.InnerException, eventSource);
            }

            return TraceException<Exception>(exception, eventSource);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This overload is deliberate")]
        public Exception AsError(TargetInvocationException targetInvocationException, string eventSource)
        {
            Fx.Assert(targetInvocationException != null, "targetInvocationException cannot be null.");

            // A non-null inner exception could require further unwrapping in AsError.
            Exception innerException = targetInvocationException.InnerException;
            if (innerException != null)
            {
                return AsError(innerException, eventSource);
            }

            // A null inner exception is unlikely but possible.
            // In this case, trace and return the targetInvocationException itself.
            return TraceException<Exception>(targetInvocationException, eventSource);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Acceptable to have only this generic form")]
        public Exception AsError<TPreferredException>(AggregateException aggregateException)
        {
            return AsError<TPreferredException>(aggregateException, this.eventSourceName);
        }

        /// <summary>
        /// Extracts the first inner exception of type <typeparamref name="TPreferredException"/>
        /// from the <see cref="AggregateException"/> if one is present.
        /// </summary>
        /// <remarks>
        /// If no <typeparamref name="TPreferredException"/> inner exception is present, this
        /// method returns the first inner exception.   All inner exceptions will be traced,
        /// including the one returned.   The containing <paramref name="aggregateException"/>
        /// will not be traced unless there are no inner exceptions.
        /// </remarks>
        /// <typeparam name="TPreferredException">The preferred type of inner exception to extract.   
        /// Use <c>typeof(Exception)</c> to extract the first exception regardless of type.</typeparam>
        /// <param name="aggregateException">The <see cref="AggregateException"/> to examine.</param>
        /// <param name="eventSource">The event source to trace.</param>
        /// <returns>The extracted exception.  It will not be <c>null</c> 
        /// but it may not be of type <typeparamref name="TPreferredException"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Acceptable to have only this generic form")]
        public Exception AsError<TPreferredException>(AggregateException aggregateException, string eventSource)
        {
            Fx.Assert(aggregateException != null, "aggregateException cannot be null.");

            // Collapse possibly nested graph into a flat list.
            // Empty inner exception list is unlikely but possible via public api.
            ReadOnlyCollection<Exception> innerExceptions = aggregateException.Flatten().InnerExceptions;
            if (innerExceptions.Count == 0)
            {
                return TraceException(aggregateException, eventSource);
            }

            // Find the first inner exception, giving precedence to TPreferredException
            Exception favoredException = null;
            foreach (Exception nextInnerException in innerExceptions)
            {
                // AggregateException may wrap TargetInvocationException, so unwrap those as well
                TargetInvocationException targetInvocationException = nextInnerException as TargetInvocationException;

                Exception innerException = (targetInvocationException != null && targetInvocationException.InnerException != null)
                                                ? targetInvocationException.InnerException
                                                : nextInnerException;

                if (innerException is TPreferredException && favoredException == null)
                {
                    favoredException = innerException;
                }

                // All inner exceptions are traced
                TraceException<Exception>(innerException, eventSource);
            }

            if (favoredException == null)
            {
                Fx.Assert(innerExceptions.Count > 0, "InnerException.Count is known to be > 0 here.");
                favoredException = innerExceptions[0];
            }

            return favoredException;
        }

        public ArgumentException Argument(string parameterName, string message)
        {
            return TraceException<ArgumentException>(new ArgumentException(message, parameterName));
        }

        public ArgumentNullException ArgumentNull(string parameterName)
        {
            return TraceException<ArgumentNullException>(new ArgumentNullException(parameterName));
        }

        public ArgumentNullException ArgumentNull(string parameterName, string message)
        {
            return TraceException<ArgumentNullException>(new ArgumentNullException(parameterName, message));
        }

        public ArgumentException ArgumentNullOrEmpty(string parameterName)
        {
            return this.Argument(parameterName, SR.ArgumentNullOrEmpty(parameterName));
        }

        public ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName, object actualValue, string message)
        {
            return TraceException<ArgumentOutOfRangeException>(new ArgumentOutOfRangeException(parameterName, actualValue, message));
        }

        // When throwing ObjectDisposedException, it is highly recommended that you use this ctor
        // [C#]
        // public ObjectDisposedException(string objectName, string message);
        // And provide null for objectName but meaningful and relevant message for message. 
        // It is recommended because end user really does not care or can do anything on the disposed object, commonly an internal or private object.
        public ObjectDisposedException ObjectDisposed(string message)
        {
            // pass in null, not disposedObject.GetType().FullName as per the above guideline
            return TraceException<ObjectDisposedException>(new ObjectDisposedException(null, message));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        public void TraceUnhandledException(Exception exception)
        {
            TraceCore.UnhandledException(Fx.Trace, exception);
        }

        TException TraceException<TException>(TException exception)
            where TException : Exception
        {
            return TraceException<TException>(exception, this.eventSourceName);
        }

        [ResourceConsumption(ResourceScope.Process)]
        [Fx.Tag.SecurityNote(Critical = "Calls 'System.Runtime.Interop.UnsafeNativeMethods.IsDebuggerPresent()' which is a P/Invoke method",
            Safe = "Does not leak any resource, needed for debugging")]
        [SecuritySafeCritical]
        TException TraceException<TException>(TException exception, string eventSource)
            where TException : Exception
        {
            TraceCore.ThrowingException(Fx.Trace, eventSource, exception);
            BreakOnException(exception);

            return exception;
        }

        [Fx.Tag.SecurityNote(Critical = "Calls into critical method UnsafeNativeMethods.IsDebuggerPresent and UnsafeNativeMethods.DebugBreak",
        Safe = "Safe because it's a no-op in retail builds.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Existing API")]
        [SecuritySafeCritical]
        void BreakOnException(Exception exception)
        {
#if DEBUG
            if (Fx.BreakOnExceptionTypes != null)
            {
                foreach (Type breakType in Fx.BreakOnExceptionTypes)
                {
                    if (breakType.IsAssignableFrom(exception.GetType()))
                    {
                        // This is intended to "crash" the process so that a debugger can be attached.  If a managed
                        // debugger is already attached, it will already be able to hook these exceptions.  We don't
                        // want to simulate an unmanaged crash (DebugBreak) in that case.
                        if (!Debugger.IsAttached && !UnsafeNativeMethods.IsDebuggerPresent())
                        {
                            UnsafeNativeMethods.DebugBreak();
                        }
                    }
                }
            }
#endif
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void TraceFailFast(string message)
        {
            EventLogger logger = null;            
#pragma warning disable 618
            logger = new EventLogger(this.eventSourceName, Fx.Trace);
#pragma warning restore 618
            TraceFailFast(message, logger);            
        }

        // Generate an event Log entry for failfast purposes
        // To force a Watson on a dev machine, do the following:
        // 1. Set \HKLM\SOFTWARE\Microsoft\PCHealth\ErrorReporting ForceQueueMode = 0 
        // 2. In the command environment, set COMPLUS_DbgJitDebugLaunchSetting=0
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Existing API")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is logged and FailFast will follow")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void TraceFailFast(string message, EventLogger logger)
        {
            if (logger != null)
            {
                try
                {
                    string stackTrace = null;
                    try
                    {
                        stackTrace = new StackTrace().ToString();
                    }
                    catch (Exception exception)
                    {
                        stackTrace = exception.Message;
                    }
                    finally
                    {
                        logger.LogEvent(TraceEventType.Critical,
                            FailFastEventLogCategory,
                            (uint)Microsoft.Server.Common.Diagnostics.EventLogEventId.FailFast,
                            message,
                            stackTrace);
                    }
                }
                catch (Exception ex)
                {                 
                    logger.LogEvent(TraceEventType.Critical,
                        FailFastEventLogCategory,
                        (uint)Microsoft.Server.Common.Diagnostics.EventLogEventId.FailFastException,
                        ex.ToString());
                }
            }        
        }
    }
}
