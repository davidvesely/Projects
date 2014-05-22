// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Types
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    public class DerivedWcfPocoType : WcfPocoType
    {
        private WcfPocoType reference;

        public DerivedWcfPocoType()
        {
        }

        public DerivedWcfPocoType(int id, string name, WcfPocoType reference)
            : base(id, name)
        {
            this.reference = reference;
        }

        public WcfPocoType Reference
        {
            get
            {
                return this.reference;
            }

            set
            {
                this.ReferenceSet = true;
                this.reference = value;
            }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public bool ReferenceSet { get; private set; }
    }
}
