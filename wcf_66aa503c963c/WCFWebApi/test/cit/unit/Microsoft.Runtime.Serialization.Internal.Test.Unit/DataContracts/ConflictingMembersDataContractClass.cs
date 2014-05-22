// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ConflictingMembersDataContractClass : SimpleMembersDataContractClass 
    {
        [DataMember(Name="DataMember1")]
        public string DataMember3 { get; set; }

        [DataMember(Name = "RenamedDataMember")]
        public int DataMember4 { get; set; }
    }
}
