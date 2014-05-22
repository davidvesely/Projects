using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class NameValueParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueNameValueParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = GenericHeaderParser.SingleValueNameValueParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X , , name = value  ,  ,next", 1, new NameValueHeaderValue("name", "value"), 24);
            CheckValidParsedValue("X name,", 1, new NameValueHeaderValue("name"), 7);
            CheckValidParsedValue(" ,name=\"value\"", 0, new NameValueHeaderValue("name", "\"value\""), 14);
            CheckValidParsedValue("name=value", 0, new NameValueHeaderValue("name", "value"), 10);

            CheckValidParsedValue(null, 0, null, 0);
            CheckValidParsedValue(string.Empty, 0, null, 0);
            CheckValidParsedValue("  ", 0, null, 2);
            CheckValidParsedValue("  ,,", 0, null, 4);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("name[value", 0);
            CheckInvalidParsedValue("name=value=", 0);
            CheckInvalidParsedValue("name=会", 0);
            CheckInvalidParsedValue("name==value", 0);
            CheckInvalidParsedValue("=value", 0);
            CheckInvalidParsedValue("name value", 0);
            CheckInvalidParsedValue("name=,value", 0);
            CheckInvalidParsedValue("会", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, NameValueHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueNameValueParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueNameValueParser;
            object result = null;
            int newIndex = startIndex;
            Assert.IsFalse(parser.TryParseValue(input, null, ref newIndex, out result), "TryParse returned true: {0}",
                input);
            Assert.AreEqual(null, result, "Parsed value. Input: '{0}'", input);
            Assert.AreEqual(startIndex, newIndex, "Returned index");
        }

        #endregion
    }
}
