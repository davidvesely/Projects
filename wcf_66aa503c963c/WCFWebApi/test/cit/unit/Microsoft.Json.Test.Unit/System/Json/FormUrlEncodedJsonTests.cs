// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(FormUrlEncodedJson))]
    public class FormUrlEncodedJsonTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedJson is public and static.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Parse(IEnumerable<Tuple<string,string>>) throws on null.")]
        public void ParseThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull(null, () => FormUrlEncodedJson.Parse(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Parse(IEnumerable<Tuple<string,string>>, int) throws on invalid MaxDepth.")]
        public void ParseThrowsInvalidMaxDepth()
        {
            Asserters.Exception.ThrowsArgument("maxDepth", () => FormUrlEncodedJson.Parse(CreateQuery(), -1));
            Asserters.Exception.ThrowsArgument("maxDepth", () => FormUrlEncodedJson.Parse(CreateQuery(), 0));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Parse(IEnumerable<Tuple<string,string>>, int) throws on MaxDepth exceeded.")]
        public void ParseThrowsMaxDepthExceeded()
        {
            // Depth of 'a[b]=1' is 3
            IEnumerable<Tuple<string, string>> query = CreateQuery(new Tuple<string, string>("a[b]", "1"));
            Asserters.Exception.ThrowsArgument(null, () => { FormUrlEncodedJson.Parse(query, 2); });
            
            // This should succeed
            Assert.IsNotNull(FormUrlEncodedJson.Parse(query, 3), "Expected non-null JsonObject instance");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryParse(IEnumerable<Tuple<string,string>>, out JsonObject) throws on null.")]
        public void TryParseThrowsOnNull()
        {
            JsonObject value;
            Asserters.Exception.ThrowsArgumentNull(null, () => FormUrlEncodedJson.TryParse(null, out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryParse(IEnumerable<Tuple<string,string>>, int, out JsonObject) throws on invalid MaxDepth.")]
        public void TryParseThrowsInvalidMaxDepth()
        {
            JsonObject value;
            Asserters.Exception.ThrowsArgument("maxDepth", () => FormUrlEncodedJson.TryParse(CreateQuery(), -1, out value));
            Asserters.Exception.ThrowsArgument("maxDepth", () => FormUrlEncodedJson.TryParse(CreateQuery(), 0, out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryParse(IEnumerable<Tuple<string,string>>, int, out JsonObject) returns false on MaxDepth exceeded.")]
        public void TryParseReturnsFalseMaxDepthExceeded()
        {
            JsonObject value;

            // Depth of 'a[b]=1' is 3
            IEnumerable<Tuple<string, string>> query = CreateQuery(new Tuple<string, string>("a[b]", "1"));
            Assert.IsFalse(FormUrlEncodedJson.TryParse(query, 2, out value), "Parse should have failed due to too high depth.");

            // This should succeed
            Assert.IsTrue(FormUrlEncodedJson.TryParse(query, 3, out value), "Expected non-null JsonObject instance");
            Assert.IsNotNull(value, "Expected non-null JsonObject instance");
        }

        #endregion

        #region Helpers

        private IEnumerable<Tuple<string, string>> CreateQuery(params Tuple<string, string>[] namevaluepairs)
        {
            return new List<Tuple<string, string>>(namevaluepairs);
        }

        #endregion
    }
}