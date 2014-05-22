//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Preserving WCF compat with 4.0 and 4.5")]
    public sealed class EtwDiagnosticTrace : DiagnosticTraceBase
    {
        //Diagnostics trace
        const int WindowsVistaMajorNumber = 6;
        const string EventSourceVersion = "4.0.0.0";
        const ushort TracingEventLogCategory = 4;

        // The guid for System.ServiceModel.Internals is {c651f5f6-1c0d-492e-8ae1-b4efd7c9d503}.
        // This guid is deliberately different to avoid a conflict
        public static readonly Guid ImmutableDefaultEtwProviderId = new Guid("{87FDB7C4-163B-4FF0-B43A-2BB31CCE5E19}");

        [Fx.Tag.SecurityNote(Critical = "provider Id to create EtwProvider, which is SecurityCritical")]
        [SecurityCritical]
        static Guid defaultEtwProviderId = ImmutableDefaultEtwProviderId;
        static Hashtable etwProviderCache = new Hashtable();
        static bool isVistaOrGreater = Environment.OSVersion.Version.Major >= WindowsVistaMajorNumber;
        static Func<string> traceAnnotation;

        [Fx.Tag.SecurityNote(Critical = "Stores object created by a critical c'tor")]
        [SecurityCritical]
        EtwProvider etwProvider;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Ported from WCF")]
        Guid etwProviderId;

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SecurityCritical]
        static EventDescriptor transferEventDescriptor = new EventDescriptor(499, 0, (byte)TraceChannel.Analytic, (byte)TraceEventLevel.LogAlways, (byte)TraceEventOpcode.Info, 0x0, 0x20000000001A0065);

        //Compiler will add all static initializers into the static constructor.  Adding an explicit one to mark SecurityCritical.
        [Fx.Tag.SecurityNote(Critical = "setting critical field defaultEtwProviderId")]
        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
                        Justification = "SecurityCriticial method")]
        static EtwDiagnosticTrace()
        {
            // In Partial Trust, initialize to Guid.Empty to disable ETW Tracing.
            if (!PartialTrustHelpers.HasEtwPermissions())
            {
                defaultEtwProviderId = Guid.Empty;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider, eventSourceName field")]
        [SecurityCritical]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ported from WCF")]
        public EtwDiagnosticTrace(string traceSourceName, Guid etwProviderId)
            : base(traceSourceName)
        {
            try
            {
                this.TraceSourceName = traceSourceName;
                this.EventSourceName = string.Concat(this.TraceSourceName, " ", EventSourceVersion);
                CreateTraceSource();
            }
            catch (Exception exception)
            {

#pragma warning disable 618
                EventLogger logger = new EventLogger(this.EventSourceName, null);
                logger.LogEvent(TraceEventType.Error, TracingEventLogCategory, (uint)Microsoft.Server.Common.Diagnostics.EventLogEventId.FailedToSetupTracing, false,
                    exception.ToString());
#pragma warning restore 618
            }

            try
            {
                CreateEtwProvider(etwProviderId);
            }
            catch (Exception exception)
            {

#pragma warning disable 618
                EventLogger logger = new EventLogger(this.EventSourceName, null);
                logger.LogEvent(TraceEventType.Error, TracingEventLogCategory, (uint)Microsoft.Server.Common.Diagnostics.EventLogEventId.FailedToSetupTracing, false,
                    exception.ToString());
#pragma warning restore 618

            }

            if (this.TracingEnabled || this.EtwTracingEnabled)
            {
#pragma warning disable 618
                this.AddDomainEventHandlersForCleanup();
#pragma warning restore 618
            }
        }

        static public Guid DefaultEtwProviderId
        {
            [Fx.Tag.SecurityNote(Critical = "reading critical field defaultEtwProviderId", Safe = "Doesn't leak info\\resources")]
            [SecuritySafeCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                Justification = "SecuritySafeCriticial method")]
            get
            {
                return EtwDiagnosticTrace.defaultEtwProviderId;
            }
            [Fx.Tag.SecurityNote(Critical = "setting critical field defaultEtwProviderId")]
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                Justification = "SecurityCriticial method")]
            set
            {
                EtwDiagnosticTrace.defaultEtwProviderId = value;
            }
        }

        public EtwProvider EtwProvider
        {
            [Fx.Tag.SecurityNote(Critical = "Exposes the critical etwProvider field")]
            [SecurityCritical]
            get
            {
                return this.etwProvider;
            }
        }

        public bool IsEtwProviderEnabled
        {
            [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
                Safe = "Doesn't leak info\\resources")]
            [SecuritySafeCritical]
            get
            {
                return (this.EtwTracingEnabled && this.etwProvider.IsEnabled());
            }
        }

        public Action RefreshState
        {
            [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
            Safe = "Doesn't leak resources or information")]
            [SecuritySafeCritical]
            get
            {
                return this.EtwProvider.ControllerCallBack;
            }

            [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
            Safe = "Doesn't leak resources or information")]
            [SecuritySafeCritical]
            set
            {
                this.EtwProvider.ControllerCallBack = value;
            }
        }

        bool EtwTracingEnabled
        {
            [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
                Safe = "Doesn't leak info\\resources")]
            [SecuritySafeCritical]
            get
            {
                return (this.etwProvider != null);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        public void SetAnnotation(Func<string> annotation)
        {
            EtwDiagnosticTrace.traceAnnotation = annotation;
        }

        public override bool ShouldTrace(TraceEventLevel level)
        {
            return base.ShouldTrace(level) || ShouldTraceToEtw(level);
        }

        [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
            Safe = "Doesn't leak information\\resources")]
        [SecuritySafeCritical]
        public bool ShouldTraceToEtw(TraceEventLevel level)
        {
            return (this.EtwProvider != null && this.EtwProvider.IsEnabled((byte)level, 0));
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand",
            Safe = "Doesn't leak information\\resources")]
        [SecuritySafeCritical]
        public void Event(int eventId, TraceEventLevel traceEventLevel, TraceChannel channel, string description)
        {
            if (this.TracingEnabled)
            {
                EventDescriptor eventDescriptor = EtwDiagnosticTrace.GetEventDescriptor(eventId, channel, traceEventLevel);
                this.Event(ref eventDescriptor, description);
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SecurityCritical]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Ported from WCF")]
        public void Event(ref EventDescriptor eventDescriptor, string description)
        {
            if (this.TracingEnabled)
            {
                TracePayload tracePayload = this.GetSerializedPayload(null, null, null);
                this.WriteTraceSource(ref eventDescriptor, description, tracePayload);
            }
        }

        public void SetAndTraceTransfer(Guid newId, bool emitTransfer)
        {
            if (emitTransfer)
            {
                TraceTransfer(newId);
            }
            EtwDiagnosticTrace.ActivityId = newId;
        }

        [Fx.Tag.SecurityNote(Critical = "Access critical transferEventDescriptor field, as well as other critical methods",
            Safe = "Doesn't leak information or resources")]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ported from WCF")]
        public void TraceTransfer(Guid newId)
        {
            Guid oldId = EtwDiagnosticTrace.ActivityId;
            if (newId != oldId)
            {
                try
                {
                    if (this.HaveListeners)
                    {
                        this.TraceSource.TraceTransfer(0, null, newId);
                    }
                    //also emit to ETW
                    if (this.IsEtwEventEnabled(ref EtwDiagnosticTrace.transferEventDescriptor))
                    {
                        this.etwProvider.WriteTransferEvent(ref EtwDiagnosticTrace.transferEventDescriptor, new EventTraceActivity(oldId), newId,
                            EtwDiagnosticTrace.traceAnnotation == null ? string.Empty : EtwDiagnosticTrace.traceAnnotation(),
                            DiagnosticTraceBase.AppDomainFriendlyName);
                    }
                }
                catch (Exception e)
                {
                    LogTraceFailure(null, e);
                }
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SecurityCritical]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ported from WCF")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Ported from WCF")]
        public void WriteTraceSource(ref EventDescriptor eventDescriptor, string description, TracePayload payload)
        {
            if (this.TracingEnabled)
            {
                XPathNavigator navigator = null;
                try
                {
                    string traceString = BuildTrace(ref eventDescriptor, description, payload);
                    XmlDocument traceDocument = new XmlDocument();
                    traceDocument.LoadXml(traceString);
                    navigator = traceDocument.CreateNavigator();
                    this.TraceSource.TraceData(TraceLevelHelper.GetTraceEventType(eventDescriptor.Level, eventDescriptor.Opcode), (int)eventDescriptor.EventId, navigator);

                    if (this.CalledShutdown)
                    {
                        this.TraceSource.Flush();
                    }
                }
                catch (Exception exception)
                {
                    LogTraceFailure(navigator == null ? string.Empty : navigator.ToString(), exception);
                }
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed by outer")]
        [SecurityCritical]
        static string BuildTrace(ref EventDescriptor eventDescriptor, string description, TracePayload payload)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb, CultureInfo.CurrentCulture)))
            {

                writer.WriteStartElement(DiagnosticStrings.TraceRecordTag);
                writer.WriteAttributeString(DiagnosticStrings.NamespaceTag, EtwDiagnosticTrace.TraceRecordVersion);
                writer.WriteAttributeString(DiagnosticStrings.SeverityTag,
                    TraceLevelHelper.LookupSeverity((TraceEventLevel)eventDescriptor.Level, (TraceEventOpcode)eventDescriptor.Opcode));
                writer.WriteAttributeString(DiagnosticStrings.ChannelTag, EtwDiagnosticTrace.LookupChannel((TraceChannel)eventDescriptor.Channel));

                writer.WriteElementString(DiagnosticStrings.TraceCodeTag, EtwDiagnosticTrace.GenerateTraceCode(ref eventDescriptor));
                writer.WriteElementString(DiagnosticStrings.DescriptionTag, description);
                writer.WriteElementString(DiagnosticStrings.AppDomain, payload.AppDomainFriendlyName);

                if (!string.IsNullOrEmpty(payload.EventSource))
                {
                    writer.WriteElementString(DiagnosticStrings.SourceTag, payload.EventSource);
                }

                if (!string.IsNullOrEmpty(payload.ExtendedData))
                {
                    writer.WriteRaw(payload.ExtendedData);
                }

                if (!string.IsNullOrEmpty(payload.SerializedException))
                {
                    writer.WriteRaw(payload.SerializedException);
                }

                writer.WriteEndElement();

                return sb.ToString();
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SecurityCritical]
        static string GenerateTraceCode(ref EventDescriptor eventDescriptor)
        {
            return eventDescriptor.EventId.ToString(CultureInfo.InvariantCulture);
        }

        static string LookupChannel(TraceChannel traceChannel)
        {
            string channelName;
            switch (traceChannel)
            {
                case TraceChannel.Admin:
                    channelName = "Admin";
                    break;
                case TraceChannel.Analytic:
                    channelName = "Analytic";
                    break;
                case TraceChannel.Application:
                    channelName = "Application";
                    break;
                case TraceChannel.Debug:
                    channelName = "Debug";
                    break;
                case TraceChannel.Operational:
                    channelName = "Operational";
                    break;
                case TraceChannel.Perf:
                    channelName = "Perf";
                    break;
                default:
                    channelName = traceChannel.ToString();
                    break;
            }

            return channelName;
        }

        public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception)
        {
            return this.GetSerializedPayload(source, traceRecord, exception, false);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed by outer")]
        public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception, bool getServiceReference)
        {
            string eventSource = null;
            string extendedData = null;
            string serializedException = null;

            if (source != null)
            {
                eventSource = CreateSourceString(source);
            }

            if (traceRecord != null)
            {
                StringBuilder sb = new StringBuilder();
                using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb, CultureInfo.CurrentCulture)))
                {

                    writer.WriteStartElement(DiagnosticStrings.ExtendedDataTag);
                    traceRecord.WriteTo(writer);
                    writer.WriteEndElement();

                    extendedData = sb.ToString();
                }
            }

            if (exception != null)
            {
                serializedException = DiagnosticTraceBase.ExceptionToTraceString(exception);
            }

            if (getServiceReference && (EtwDiagnosticTrace.traceAnnotation != null))
            {
                return new TracePayload(serializedException, eventSource, DiagnosticTraceBase.AppDomainFriendlyName, extendedData, EtwDiagnosticTrace.traceAnnotation());
            }

            return new TracePayload(serializedException, eventSource, DiagnosticTraceBase.AppDomainFriendlyName, extendedData, string.Empty);
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand",
            Safe = "Only queries the status of the provider - does not modify the state")]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Ported from WCF")]
        public bool IsEtwEventEnabled(ref EventDescriptor eventDescriptor)
        {
            return (this.EtwTracingEnabled && this.etwProvider.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords));
        }

        [Fx.Tag.SecurityNote(Critical = "Access the critical Listeners property",
            Safe = "Only Removes the default listener of the local source")]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "SecuritySafeCriticial method")]
        void CreateTraceSource()
        {
            if (!string.IsNullOrEmpty(this.TraceSourceName))
            {
                SetTraceSource(new DiagnosticTraceSource(this.TraceSourceName));
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Sets this.etwProvider and calls EtwProvider constructor, which are Security Critical")]
        [SecurityCritical]
        void CreateEtwProvider(Guid etwProviderIdValue)
        {
            if (etwProviderIdValue != Guid.Empty && EtwDiagnosticTrace.isVistaOrGreater)
            {
                //Pick EtwProvider from cache, add to cache if not found
                this.etwProvider = (EtwProvider)etwProviderCache[etwProviderIdValue];
                if (this.etwProvider == null)
                {
                    lock (etwProviderCache)
                    {
                        this.etwProvider = (EtwProvider)etwProviderCache[etwProviderIdValue];
                        if (this.etwProvider == null)
                        {
                            this.etwProvider = new EtwProvider(etwProviderIdValue);
                            etwProviderCache.Add(etwProviderIdValue, this.etwProvider);
                        }
                    }
                }

                this.etwProviderId = etwProviderIdValue;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Usage of EventDescriptor, which is protected by a LinkDemand")]
        [SecurityCritical]
        static EventDescriptor GetEventDescriptor(int eventId, TraceChannel channel, TraceEventLevel traceEventLevel)
        {
            unchecked
            {
                //map channel to keywords
                long keyword = (long)0x0;
                if (channel == TraceChannel.Admin)
                {
                    keyword = keyword | (long)0x8000000000000000;
                }
                else if (channel == TraceChannel.Operational)
                {
                    keyword = keyword | 0x4000000000000000;
                }
                else if (channel == TraceChannel.Analytic)
                {
                    keyword = keyword | 0x2000000000000000;
                }
                else if (channel == TraceChannel.Debug)
                {
                    keyword = keyword | 0x100000000000000;
                }
                else if (channel == TraceChannel.Perf)
                {
                    keyword = keyword | 0x0800000000000000;
                }
                return new EventDescriptor(eventId, 0x0, (byte)channel, (byte)traceEventLevel, 0x0, 0x0, (long)keyword);
            }
        }

        protected override void OnShutdownTracing()
        {
            ShutdownTraceSource();
            ShutdownEtwProvider();             
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ported from WCF")]
        void ShutdownTraceSource()
        {            
            try
            {
                if (TraceCore.AppDomainUnloadIsEnabled(this))
                {
                    TraceCore.AppDomainUnload(this, AppDomain.CurrentDomain.FriendlyName,
                        DiagnosticTraceBase.ProcessName, DiagnosticTraceBase.ProcessId.ToString(CultureInfo.CurrentCulture));
                }
                
                this.TraceSource.Flush();
            }
            catch (Exception exception)
            {
                //log failure
                LogTraceFailure(null, exception);
            }         
        }

        [Fx.Tag.SecurityNote(Critical = "Access critical etwProvider field",
            Safe = "Doesn't leak info\\resources")]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ported from WCF")]
        void ShutdownEtwProvider()
        {            
            try
            {                
                if (this.etwProvider != null)
                {
                    this.etwProvider.Dispose();
                    //no need to set this.etwProvider as null as Dispose() provides the necessary guard
                    //leaving it non-null protects trace calls from NullReferenceEx, CSDMain Bug 136228
                }                
            }
            catch (Exception exception)
            {
                //log failure
                LogTraceFailure(null, exception);
            }            
        }

        public override void TraceHandledException(Exception exception)
        {
            if (TraceCore.HandledExceptionIsEnabled(this))
            {
                TraceCore.HandledException(this, exception);
            }
        }

        public override bool IsEnabled()
        {
            return TraceCore.TraceCodeEventLogCriticalIsEnabled(this)
                || TraceCore.TraceCodeEventLogVerboseIsEnabled(this)
                || TraceCore.TraceCodeEventLogInfoIsEnabled(this)
                || TraceCore.TraceCodeEventLogWarningIsEnabled(this)
                || TraceCore.TraceCodeEventLogErrorIsEnabled(this);  
        }

        public override void TraceEventLogEvent(TraceEventType type, TraceRecord traceRecord)
        {
            switch (type)
            {
                case TraceEventType.Critical:
                    TraceCore.TraceCodeEventLogCritical(this, traceRecord);
                    break;

                case TraceEventType.Verbose:
                    TraceCore.TraceCodeEventLogVerbose(this, traceRecord);
                    break;

                case TraceEventType.Information:
                    TraceCore.TraceCodeEventLogInfo(this, traceRecord);
                    break;

                case TraceEventType.Warning:
                    TraceCore.TraceCodeEventLogWarning(this, traceRecord);
                    break;

                case TraceEventType.Error:
                    TraceCore.TraceCodeEventLogError(this, traceRecord);
                    break;
            }
        }

        protected override void OnUnhandledException(Exception exception)
        {
            TraceCore.UnhandledException(this, exception);
        }
    }
}
