// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;

    internal interface ITolerantTextReader
    {
        Exception Exception { get; }

        bool Read();

        IEnumerable<string> GetExpectedItems();
    }
}