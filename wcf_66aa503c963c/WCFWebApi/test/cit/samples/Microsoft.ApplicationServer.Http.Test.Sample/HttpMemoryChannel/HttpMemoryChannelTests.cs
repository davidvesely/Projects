// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests for <see cref="HttpMemoryChannel.Sample.Program"/> sample.
    /// </summary>
    [TestClass]
    public class HttpMemoryChannelTests
    {
        private static int iterations = 4;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMemoryChannel.Program runs without exceptions.")]
        public void HttpMemoryChannel_Basic()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpMemoryChannel.Sample.Program.Main();
            }
        }
    }
}