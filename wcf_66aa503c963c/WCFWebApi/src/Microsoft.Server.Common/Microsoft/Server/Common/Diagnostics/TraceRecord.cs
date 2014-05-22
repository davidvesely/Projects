//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    [Serializable]
    public class TraceRecord
    {
        protected const string EventIdBase = "http://schemas.microsoft.com/2006/08/ServiceModel/";
        protected const string NamespaceSuffix = "TraceRecord";

        public virtual string EventId { get { return BuildEventId("Empty"); } }

        public virtual void WriteTo(XmlWriter writer) 
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        protected string BuildEventId(string eventId)
        {
            return TraceRecord.EventIdBase + eventId + TraceRecord.NamespaceSuffix;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ported from WCF")]
        protected string XmlEncode(string text)
        {
            return DiagnosticTraceBase.XmlEncode(text);
        }
    }
}