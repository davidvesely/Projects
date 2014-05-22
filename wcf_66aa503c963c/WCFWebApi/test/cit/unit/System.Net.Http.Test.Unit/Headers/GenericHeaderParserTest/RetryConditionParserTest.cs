using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class RetryConditionParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.RetryConditionParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X  123456789 ", 1, new RetryConditionHeaderValue(new TimeSpan(0, 0, 123456789)), 13);
            CheckValidParsedValue("  Sun, 06 Nov 1994 08:49:37 GMT ", 0,
                new RetryConditionHeaderValue(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero)), 32);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("123 ,", 0); // no delimiter allowed
            CheckInvalidParsedValue("Sun, 06 Nov 1994 08:49:37 GMT ,", 0); // no delimiter allowed
            CheckInvalidParsedValue("123 Sun, 06 Nov 1994 08:49:37 GMT", 0);
            CheckInvalidParsedValue("Sun, 06 Nov 1994 08:49:37 GMT \"x\"", 0);
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, RetryConditionHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.RetryConditionParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}'", input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.RetryConditionParser;
            object result = null;
            int newIndex = startIndex;
            Assert.IsFalse(parser.TryParseValue(input, null, ref newIndex, out result),
                "TryParse returned true. Input: '{0}'", input);
            Assert.AreEqual(null, result, "Parsed value");
            Assert.AreEqual(startIndex, newIndex, "Returned index");
        }

        #endregion
    }
}
