//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common.Diagnostics
{
    using System.Collections;
    using System.Xml;

    class DictionaryTraceRecord : TraceRecord
    {
        IDictionary dictionary;

        public DictionaryTraceRecord(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public override string EventId { get { return TraceRecord.EventIdBase + "Dictionary" + TraceRecord.NamespaceSuffix; } }

        public override void WriteTo(XmlWriter xml)
        {
            if (this.dictionary != null)
            {
                foreach (object key in this.dictionary.Keys)
                {
                    object value = this.dictionary[key];
                    xml.WriteElementString(key.ToString(), value == null ? string.Empty : value.ToString());
                }
            }
        }
    }
}