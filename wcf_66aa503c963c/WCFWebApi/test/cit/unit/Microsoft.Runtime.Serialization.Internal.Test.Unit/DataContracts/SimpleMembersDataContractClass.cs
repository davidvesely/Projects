// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SimpleMembersDataContractClass
    {
        [DataMember]
        public int DataMember1 { get; set; }

        [DataMember(Name="RenamedDataMember")]
        public string DataMember2 { get; set; }
    }
}
