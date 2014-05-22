
// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common
{

    public class UnitTestAsserters : Microsoft.TestCommon.UnitTestAsserters
    {
        public DiagnosticTraceAssert DiagnosticTrace { get { return DiagnosticTraceAssert.Singleton; } }
    }
}