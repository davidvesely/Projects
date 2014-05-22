// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OrderedDataContractClass
    {
        [DataMember]
        public int DataMember1 { get; set; }

        [DataMember(Name = "RenamedDataMember", Order=2)]
        public string DataMember2 { get; set; }

        [DataMember(Order = 1)]
        public bool DataMember3 { get; set; }
    }
}
