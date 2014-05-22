// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests for <see cref="HttpBatching.Sample.Program"/> sample.
    /// </summary>
    [TestClass]
    public class HttpBatchingTests
    {
        private static int iterations = 4;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpBatching Sample runs without exceptions.")]
        public void HttpBatching_Basic()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpBatching.Sample.Program.Main();
            }
        }
    }
}