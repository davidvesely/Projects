using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class ViaParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueViaParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = GenericHeaderParser.SingleValueViaParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X , , 1.1   host, ,next", 1, new ViaHeaderValue("1.1", "host"), 19);
            CheckValidParsedValue("X HTTP  /  x11   192.168.0.1\r\n (comment) , ,next", 1,
                new ViaHeaderValue("x11", "192.168.0.1", "HTTP", "(comment)"), 44);
            CheckValidParsedValue(" ,HTTP/1.1 [::1]", 0, new ViaHeaderValue("1.1", "[::1]", "HTTP"), 16);
            CheckValidParsedValue("1.1 host", 0, new ViaHeaderValue("1.1", "host"), 8);

            CheckValidParsedValue(null, 0, null, 0);
            CheckValidParsedValue(string.Empty, 0, null, 0);
            CheckValidParsedValue("  ", 0, null, 2);
            CheckValidParsedValue("  ,,", 0, null, 4);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("HTTP/1.1 host (comment)invalid", 0);
            CheckInvalidParsedValue("HTTP/1.1 host (comment)=", 0);
            CheckInvalidParsedValue("HTTP/1.1 host (comment) invalid", 0);
            CheckInvalidParsedValue("HTTP/1.1 host (comment) =", 0);
            CheckInvalidParsedValue("HTTP/1.1 host invalid", 0);
            CheckInvalidParsedValue("HTTP/1.1 host =", 0);
            CheckInvalidParsedValue("1.1 host invalid", 0);
            CheckInvalidParsedValue("1.1 host =", 0);
            CheckInvalidParsedValue("会", 0);
            CheckInvalidParsedValue("HTTP/test [::1]:80\r(comment)", 0);
            CheckInvalidParsedValue("HTTP/test [::1]:80\n(comment)", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, ViaHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueViaParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueViaParser;
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
