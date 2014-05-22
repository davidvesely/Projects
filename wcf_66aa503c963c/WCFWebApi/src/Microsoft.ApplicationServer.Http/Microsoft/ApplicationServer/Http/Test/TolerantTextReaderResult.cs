// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    internal class TolerantTextReaderResult
    {
        [DataMember(Name = "autoCompleteList")]
        public IEnumerable<string> AutoCompleteList { get; set; }
    }
}