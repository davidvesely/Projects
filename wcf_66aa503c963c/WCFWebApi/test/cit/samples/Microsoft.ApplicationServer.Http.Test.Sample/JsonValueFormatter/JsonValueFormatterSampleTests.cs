// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using JsonValueFormatter.Sample;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests for <see cref="JsonValueFormatter.Sample.Program"/> sample.
    /// </summary>
    [TestClass]
    public class JsonValueFormatterSampleTests
    {
        private static int iterations = 2;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that JsonValueFormatterSample runs without exceptions.")]
        public void JsonValueFormatter_Basic()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                JsonValueFormatter.Sample.Program.Main();
            }
        }
    }
}