// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DerivedDataContractClass : SimpleDataContractClass
    {
        [DataMember]
        public int DataMember3 { get; set; }
    }
}
