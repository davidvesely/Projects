// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;

    public class EventTraceActivity
    {
        // This field is public because it needs to be passed by reference for P/Invoke
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Must be public for P/Invoke")]
        public Guid ActivityId;
        static EventTraceActivity empty;

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Approved public API.")]
        public EventTraceActivity(bool setOnThread = false)
            : this(Guid.NewGuid(), setOnThread)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Approved public API.")]
        public EventTraceActivity(Guid activityId, bool setOnThread = false)
        {
            this.ActivityId = activityId;
            if (setOnThread)
            {
                SetActivityIdOnThread();
            }
        }

        public static EventTraceActivity Empty
        {
            get 
            {
                if (empty == null)
                {
                    empty = new EventTraceActivity(Guid.Empty);
                }

                return empty;
            }
        }

        public static string Name
        {
            get { return "E2EActivity"; }
        }

        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
            Safe = "We do not leak security data.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Not necessary to make another method for all defaults here")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Approved public API.")]
        [SecuritySafeCritical]
        public static EventTraceActivity GetFromThreadOrCreate(bool clearIdOnThread = false)
        {
            Guid guid = Trace.CorrelationManager.ActivityId;
            if (guid == Guid.Empty)
            {
                guid = Guid.NewGuid();
            }
            else if (clearIdOnThread)
            {
                // Reset the ActivityId on the thread to avoid using the same Id again
                Trace.CorrelationManager.ActivityId = Guid.Empty;
            }

            return new EventTraceActivity(guid);
        }

        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
            Safe = "We do not leak security data.")]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Ported from WCF")]
        public static Guid GetActivityIdFromThread()
        {
            return Trace.CorrelationManager.ActivityId;
        }

        public void SetActivityId(Guid activityId)
        {
            this.ActivityId = activityId;
        }

        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
            Safe = "We do not leak security data.")]
        [SecuritySafeCritical]
        void SetActivityIdOnThread()
        {
            Trace.CorrelationManager.ActivityId = this.ActivityId;
        }
    }
}
