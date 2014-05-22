//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Server.Common.Diagnostics
{
    using System.Diagnostics;

    public class DiagnosticTraceSource : TraceSource
    {
        const string PropagateActivityValue = "propagateActivity";
        public DiagnosticTraceSource(string name)
            : base(name)
        {
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { DiagnosticTraceSource.PropagateActivityValue };
        }

        public bool PropagateActivity
        {
            get
            {
                bool retval = false;
                string attributeValue = this.Attributes[DiagnosticTraceSource.PropagateActivityValue];
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    if (!bool.TryParse(attributeValue, out retval))
                    {
                        retval = false;
                    }
                }
                return retval;
            }
            set
            {
                this.Attributes[DiagnosticTraceSource.PropagateActivityValue] = value.ToString();
            }
        }
    }
}
