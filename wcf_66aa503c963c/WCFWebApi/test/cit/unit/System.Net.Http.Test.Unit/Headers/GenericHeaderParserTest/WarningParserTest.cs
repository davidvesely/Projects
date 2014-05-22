using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class WarningParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueWarningParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = GenericHeaderParser.SingleValueWarningParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X , , 123   host \"text\", ,next", 1,
                new WarningHeaderValue(123, "host", "\"text\""), 26);
            CheckValidParsedValue("X 50  192.168.0.1  \"text  \"  \"Tue, 20 Jul 2010 01:02:03 GMT\" , ,next", 1,
                new WarningHeaderValue(50, "192.168.0.1", "\"text  \"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)), 64);
            CheckValidParsedValue(" ,123 h \"t\",", 0, new WarningHeaderValue(123, "h", "\"t\""), 12);
            CheckValidParsedValue("1 h \"t\"", 0, new WarningHeaderValue(1, "h", "\"t\""), 7);
            CheckValidParsedValue("1 h \"t\" \"Tue, 20 Jul 2010 01:02:03 GMT\"", 0,
                new WarningHeaderValue(1, "h", "\"t\"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)), 39);
            CheckValidParsedValue("1 会 \"t\" ,,", 0, new WarningHeaderValue(1, "会", "\"t\""), 10);

            CheckValidParsedValue(null, 0, null, 0);
            CheckValidParsedValue(string.Empty, 0, null, 0);
            CheckValidParsedValue("  ", 0, null, 2);
            CheckValidParsedValue("  ,,", 0, null, 4);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("1.1 host \"text\"", 0);
            CheckInvalidParsedValue("11 host text", 0);
            CheckInvalidParsedValue("11 host \"text\" Tue, 20 Jul 2010 01:02:03 GMT", 0);
            CheckInvalidParsedValue("11 host \"text\" 123 next \"text\"", 0);
            CheckInvalidParsedValue("会", 0);
            CheckInvalidParsedValue("123 会", 0);
            CheckInvalidParsedValue("111 [::1]:80\r(comment) \"text\"", 0);
            CheckInvalidParsedValue("111 [::1]:80\n(comment) \"text\"", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, WarningHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueWarningParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueWarningParser;
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
