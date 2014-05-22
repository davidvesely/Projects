// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Json.Test.Sample
{
    using JsonValue.Sample;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Verifies that the <see cref="JsonValue.Sample.Program"/> sample runs correctly.
    /// </summary>
    [TestClass]
    public class JsonValueTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that JsonValue.Samples runs without exceptions.")]
        public void JsonValue_Basic()
        {
            JsonValue.Sample.Program.Main();
        }
    }
}