// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TaskDispatcherBehavior;
    using TaskDispatcherBehavior.Sample;

    /// <summary>
    /// Integration tests for <see cref="Program"/> sample.
    /// </summary>
    [TestClass]
    public class TaskDispatcherBehaviorTests
    {
        private static int iterations = 2;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that TaskDispatcherBehavior Sample runs without exceptions.")]
        public void TaskDispatcherBehavior()
        {
            // This sample does not apply to 4.5 because task invocation is supported in WCF 4.5
            if (!RuntimeEnvironment.IsVersion45Installed) 
            {
                for (int cnt = 0; cnt < iterations; cnt++)
                {
                    Program.Main();
                }
            }
        }
    }
}