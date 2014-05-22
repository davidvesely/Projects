// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class ReadOnlyCollectionDataContractClass
    {
        [DataMember]
        public List<int> DataMember1 { get { return null; } }

        [DataMember]
        public List<int> DataMember2 { get; set; }

        [DataMember]
        public List<int> DataMember3 { get; private set; }
    }
}
