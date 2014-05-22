// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Collections.Generic;
    using System.Json;
    using System.Linq;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(FormattingUtilities))]
    public class FormattingUtilitiesTests : UnitTest
    {
        private static Dictionary<string, string> quotedStrings = new Dictionary<string, string>()
        {
            { @"""""", @"" },
            { @"""string""", @"string" },
            { @"string", @"string" },
            { @"""str""ing""", @"str""ing" },
        };

        private static List<string> notQuotedStrings = new List<string>()
        {
            { @" """ },
            { @" """"" },
            { @"string" },
            { @"str""ing" },
            { @"s""trin""g" },
        };

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Utilities is internal static type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsStatic);
        }
        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsJsonValueType returns true")]
        public void IsJsonValueTypeReturnsTrue()
        {
            Assert.IsTrue(FormattingUtilities.IsJsonValueType(typeof(JsonValue)), "Should return true");
            Assert.IsTrue(FormattingUtilities.IsJsonValueType(typeof(JsonPrimitive)), "Should return true");
            Assert.IsTrue(FormattingUtilities.IsJsonValueType(typeof(JsonObject)), "Should return true");
            Assert.IsTrue(FormattingUtilities.IsJsonValueType(typeof(JsonArray)), "Should return true");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CreateEmptyContentHeaders returns empty headers")]
        public void CreateEmptyContentHeadersReturnsEmptyHeaders()
        {
            HttpContentHeaders headers = FormattingUtilities.CreateEmptyContentHeaders();
            Assert.IsNotNull(headers, "headers should not be null");
            Assert.AreEqual(0, headers.Count(), "Content headers should be empty");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UnquoteToken returns same string on null, empty strings")]
        public void UnquoteTokenReturnsSameRefOnEmpty()
        {
            foreach (string empty in TestData.EmptyStrings)
            {
                string result = FormattingUtilities.UnquoteToken(empty);
                Assert.AreSame(empty, result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UnquoteToken returns unquoted strings")]
        public void UnquoteTokenReturnsSameRefNonQuotedStrings()
        {
            foreach (var test in notQuotedStrings)
            {
                string result = FormattingUtilities.UnquoteToken(test);
                Assert.AreEqual(test, result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UnquoteToken returns unquoted strings")]
        public void UnquoteTokenReturnsUnquotedStrings()
        {
            foreach (var test in quotedStrings)
            {
                string result = FormattingUtilities.UnquoteToken(test.Key);
                Assert.AreEqual(test.Value, result);
            }
        }

        #endregion
    }
}
