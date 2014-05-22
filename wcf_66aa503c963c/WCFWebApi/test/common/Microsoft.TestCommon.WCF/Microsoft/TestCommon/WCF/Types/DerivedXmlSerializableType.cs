// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Types
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Microsoft.TestCommon.Types;

    [KnownType(typeof(DerivedWcfPocoType))]
    [XmlInclude(typeof(DerivedWcfPocoType))]
    public class DerivedXmlSerializableType : XmlSerializableType, INotJsonSerializable
    {
        private WcfPocoType reference;

        public DerivedXmlSerializableType()
        {
        }

        public DerivedXmlSerializableType(int id, string name, WcfPocoType reference) 
            : base(id, name)
        {
            this.reference = reference;
        }

        [XmlElement]
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

        [XmlIgnore]
        public bool ReferenceSet { get; private set; }

        public static new IEnumerable<DerivedXmlSerializableType> GetTestData()
        {
            return new DerivedXmlSerializableType[] { 
                new DerivedXmlSerializableType(), 
                new DerivedXmlSerializableType(1, "SomeName", new WcfPocoType(2, "SomeOtherName")) };
        }

        public static IEnumerable<DerivedXmlSerializableType> GetKnownTypeTestData()
        {
            return new DerivedXmlSerializableType[] { 
                new DerivedXmlSerializableType(), 
                new DerivedXmlSerializableType(1, "SomeName", null), 
                new DerivedXmlSerializableType(1, "SomeName", new DerivedWcfPocoType(2, "SomeOtherName", null))};
        }
    }
}
