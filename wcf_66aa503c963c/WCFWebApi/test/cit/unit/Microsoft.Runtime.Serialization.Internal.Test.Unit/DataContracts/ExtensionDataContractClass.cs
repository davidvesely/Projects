// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ExtensionDataContractClass : IExtensibleDataObject
    {
        [DataMember]
        public int DataMember1 { get; set; }

        [DataMember(Name = "RenamedDataMember")]
        public string DataMember2 { get; set; }

        public bool BoolProperty { get; set; }

        private ExtensionDataObject extensionDataObject_value;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return extensionDataObject_value;
            }
            set
            {
                extensionDataObject_value = value;
            }
        }
    }
}
