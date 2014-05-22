// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public class IsReferenceDataContractClass
    {
        [DataMember]
        public int DataMember1 { get; set; }

        [DataMember]
        public IsReferenceDataContractClass DataMember2 { get; set; }
    }
}
