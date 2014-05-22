using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers
{
    [TestClass]
    public class MediaTypeHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            MediaTypeHeaderParser parser = MediaTypeHeaderParser.SingleValueParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = MediaTypeHeaderParser.MultipleValuesParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void Parse_ValidValue_ReturnsMediaTypeHeaderValue()
        {
            // This test verifies that Parse() correctly calls TryParse().
            MediaTypeHeaderParser parser = MediaTypeHeaderParser.SingleValueParser;
            int index = 2;

            MediaTypeHeaderValue expected = new MediaTypeHeaderValue("text/plain");
            expected.CharSet = "utf-8";
            Assert.IsTrue(expected.Equals(parser.ParseValue("   text / plain ; charset = utf-8 ", null, ref index)));
            Assert.AreEqual(34, index, "Returned index.");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_InvalidValue_Throw()
        {
            // This test verifies that Parse() correctly calls TryParse().
            MediaTypeHeaderParser parser = MediaTypeHeaderParser.SingleValueParser;
            int index = 0;
            parser.ParseValue("text/plain; charset=utf-8, next/mediatype", null, ref index); // only one value allowed.
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_NullValue_Throw()
        {
            MediaTypeHeaderParser parser = MediaTypeHeaderParser.SingleValueParser;
            int index = 0;
            parser.ParseValue(null, null, ref index);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            MediaTypeHeaderValue expected = new MediaTypeHeaderValue("text/plain");
            CheckValidParsedValue("\r\n text/plain  ", 0, expected, 15, false);
            CheckValidParsedValue("text/plain", 0, expected, 10, false);
            CheckValidParsedValue("\r\n text/plain  ", 0, expected, 15, true);
            CheckValidParsedValue("text/plain", 0, expected, 10, true);
            CheckValidParsedValue("\r\n text/plain  , next/mediatype", 0, expected, 17, true);
            CheckValidParsedValue("text/plain, next/mediatype", 0, expected, 12, true);
            CheckValidParsedValue("  ", 0, null, 2, true);
            CheckValidParsedValue("", 0, null, 0, true);
            CheckValidParsedValue(null, 0, null, 0, true);
            CheckValidParsedValue("  ,,", 0, null, 4, true);

            // Note that even if the whole string is invalid, the first media-type value is valid. When the parser
            // gets called again using the result-index (13), then it fails: I.e. we have 1 valid media-type and an
            // invalid one.
            CheckValidParsedValue("text/plain , invalid", 0, expected, 13, true);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // media-type parser.
            expected.CharSet = "utf-8";
            CheckValidParsedValue("\r\n text   /  plain ;  charset =   utf-8 ", 0, expected, 40, false);
            CheckValidParsedValue("  text/plain;charset=utf-8", 2, expected, 26, false);
            CheckValidParsedValue("\r\n text   /  plain ;  charset =   utf-8 ", 0, expected, 40, true);
            CheckValidParsedValue("  text/plain;charset=utf-8", 2, expected, 26, true);
            CheckValidParsedValue("\r\n text   /  plain ;  charset =   utf-8  , next/mediatype", 0, expected, 43, true);
            CheckValidParsedValue("  text/plain;charset=utf-8, next/mediatype", 2, expected, 28, true);
        }
        
        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("", 0, false);
            CheckInvalidParsedValue("  ", 0, false);
            CheckInvalidParsedValue(null, 0, false);
            CheckInvalidParsedValue("text/plain会", 0, true);
            CheckInvalidParsedValue("text/plain会", 0, false);
            CheckInvalidParsedValue("text/plain ,", 0, false);
            CheckInvalidParsedValue("text/plain,", 0, false);
            CheckInvalidParsedValue("text/plain; charset=utf-8 ,", 0, false);
            CheckInvalidParsedValue("text/plain; charset=utf-8,", 0, false);
            CheckInvalidParsedValue("textplain", 0, true);
            CheckInvalidParsedValue("textplain", 0, false);
            CheckInvalidParsedValue("text/", 0, true);
            CheckInvalidParsedValue("text/", 0, false);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, MediaTypeHeaderValue expectedResult,
            int expectedIndex, bool supportsMultipleValues)
        {
            MediaTypeHeaderParser parser = null;
            if (supportsMultipleValues)
            {
                parser = MediaTypeHeaderParser.MultipleValuesParser;
            }
            else 
            {
                parser = MediaTypeHeaderParser.SingleValueParser;
            }

            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}', Index: {1}", input, startIndex);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex, bool supportsMultipleValues)
        {
            MediaTypeHeaderParser parser = null;
            if (supportsMultipleValues)
            {
                parser = MediaTypeHeaderParser.MultipleValuesParser;
            }
            else
            {
                parser = MediaTypeHeaderParser.SingleValueParser;
            }

            object result = null;
            int newIndex = startIndex;
            Assert.IsFalse(parser.TryParseValue(input, null, ref newIndex, out result),
                "TryParse returned true. Input: '{0}', Index: {1}", input, startIndex);
            Assert.AreEqual(null, result, "Parsed value");
            Assert.AreEqual(startIndex, newIndex, "Returned index");
        }

        #endregion
    }
}
