using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class ContentRangeParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.ContentRangeParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Only verify parser functionality (i.e. ContentRangeHeaderParser.TryParse()). We don't need to validate
            // all possible range values (verification done by tests for ContentRangeHeaderValue.GetContentRangeLength()).
            CheckValidParsedValue("X bytes 1-2/3 ", 1, new ContentRangeHeaderValue(1, 2, 3), 14);
            CheckValidParsedValue("bytes  *  /  3", 0, new ContentRangeHeaderValue(3), 14);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("bytes 1-2/3,", 0); // no character after 'length' allowed
            CheckInvalidParsedValue("x bytes 1-2/3", 0);
            CheckInvalidParsedValue("bytes 1-2/3.4", 0);
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, ContentRangeHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.ContentRangeParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}'", input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.ContentRangeParser;
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
