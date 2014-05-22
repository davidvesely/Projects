// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.TestCommon.WCF.Types;

    public class UnitTestDataSets
    {
        public ValueTypeTestData<DataContractEnum> DataContractEnums { get { return WcfTestData.DataContractEnumTestData; } } 

        public RefTypeTestData<string> UriStrings { get { return WcfTestData.UriTestDataStrings; } }

        public RefTypeTestData<Uri> Uris { get { return WcfTestData.UriTestData; } }

        public RefTypeTestData<DataContractType> DataContractTypes { get { return WcfTestData.DataContractTypeTestData; } }

        public RefTypeTestData<DerivedDataContractType> DerivedDataContractTypes { get { return WcfTestData.DerivedDataContractTypeTestData; } }

        public RefTypeTestData<ReferenceDataContractType> ReferenceDataContractTypes { get { return WcfTestData.ReferenceDataContractTypeTestData; } }

        public RefTypeTestData<XmlSerializableType> XmlSerializableTypes { get { return WcfTestData.XmlSerializableTypeTestData; } }

        public RefTypeTestData<DerivedXmlSerializableType> DerivedXmlSerializableTypes { get { return WcfTestData.DerivedXmlSerializableTypeTestData; } }

        public RefTypeTestData<WcfPocoType> PocoTypes { get { return WcfTestData.WcfPocoTypeTestData; } }

        public RefTypeTestData<WcfPocoType> PocoTypesWithNull { get { return WcfTestData.WcfPocoTypeTestDataWithNull; } }

        public ReadOnlyCollection<TestData> RefTypes { get { return WcfTestData.RefTypeTestDataCollection; } }

        public ReadOnlyCollection<TestData> ValueAndRefTypes { get { return WcfTestData.ValueAndRefTypeTestDataCollection; } }

        public ReadOnlyCollection<TestData> RepresentativeValueAndRefTypes { get { return WcfTestData.RepresentativeValueAndRefTypeTestDataCollection; } }
    }
}