// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DerivedHasKnownDataContractClass : HasKnownDataContractClass
    {
        [DataMember]
        public int DataMember2 { get; set; }
    }
}
