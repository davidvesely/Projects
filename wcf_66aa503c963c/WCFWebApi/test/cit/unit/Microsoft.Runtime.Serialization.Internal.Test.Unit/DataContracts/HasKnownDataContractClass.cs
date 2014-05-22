// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [KnownType(typeof(DerivedHasKnownDataContractClass))]
    [DataContract]
    public class HasKnownDataContractClass
    {
        [DataMember]
        public int DataMember1 { get; set; }
    }
}
