// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Formatting.Test.Sample
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests for <see cref="Microsoft.Net.Http.Scenarios.ProgressMessageHandlerSample.Program"/> sample.
    /// </summary>
    [TestClass]
    public class ProgressMessageHandlerTests
    {
        private static int iterations = 2;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that ProgressMessageHandlerSample runs without exceptions.")]
        [DeploymentItem("Web\\SampleData.random", "Web")]
        public void ProgressMessageHandler_Basic()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                string[] cmdArgs = null;
                ProgressMessageHandler.Sample.Program.Main(cmdArgs);
            }
        }
    }
}