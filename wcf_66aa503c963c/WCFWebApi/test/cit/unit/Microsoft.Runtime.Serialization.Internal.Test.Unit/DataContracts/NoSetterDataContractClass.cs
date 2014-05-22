// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class NoSetterDataContractClass
    {
        private int dataMember1 = 0;

        [DataMember]
        public int DataMember1 { get { return this.dataMember1; } }
    }
}
