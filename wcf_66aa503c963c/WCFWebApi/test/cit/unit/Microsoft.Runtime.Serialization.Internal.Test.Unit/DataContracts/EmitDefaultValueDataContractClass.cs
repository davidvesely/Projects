// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Serialization.CIT.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmitDefaultValueDataContractClass
    {
        [DataMember(EmitDefaultValue=true)]
        public int DataMember1 { get; set; }

        [DataMember(Name="RenamedDataMember", EmitDefaultValue=false)]
        public string DataMember2 { get; set; }
    }
}
