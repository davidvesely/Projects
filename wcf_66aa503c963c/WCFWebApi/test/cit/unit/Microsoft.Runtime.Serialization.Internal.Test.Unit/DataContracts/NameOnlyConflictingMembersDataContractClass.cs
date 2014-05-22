// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class NameOnlyConflictingMembersDataContractClass : SimpleDataContractClass 
    {
        [DataMember(Name="DataMember1", IsRequired=true, Order=2)]
        public int DataMember3 { get; set; }

        [DataMember(Name = "RenamedDataMember", EmitDefaultValue=true, Order=1)]
        public string DataMember4 { get; set; }
    }
}
