using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Diagnostics.Contracts;
using System.Collections;

namespace Microsoft.Net.Http.Test.Headers
{
    [TestClass]
    public class Int64NumberHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void Parse_ValidValue_ReturnsLongValue()
        {
            // This test verifies that Parse() correctly calls TryParse().
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            int index = 2;
            Assert.AreEqual((long)15, parser.ParseValue("  15", null, ref index), "Value length.");
            Assert.AreEqual(4, index, "Returned index (startIndex: 2).");

            index = 0;
            Assert.AreEqual((long)15, parser.ParseValue("  15", null, ref index), "Value length.");
            Assert.AreEqual(4, index, "Returned index (startIndex: 0).");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_InvalidValue_Throw()
        {
            // This test verifies that Parse() correctly calls TryParse().
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            int index = 0;
            parser.ParseValue("a", null, ref index);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_NullValue_Throw()
        {
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            int index = 0;
            parser.ParseValue(null, null, ref index);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("123456789012345", 0, 123456789012345, 15);
            CheckValidParsedValue("0", 0, 0, 1);
            CheckValidParsedValue("000015", 0, 15, 6);
            CheckValidParsedValue(" 123 \t\r\n ", 0, 123, 9);
            CheckValidParsedValue("a 5 \r\n ", 1, 5, 7);
            CheckValidParsedValue(" 987", 0, 987, 4);
            CheckValidParsedValue("987 ", 0, 987, 4);
            CheckValidParsedValue("a456", 1, 456, 4);
            CheckValidParsedValue("a456 ", 1, 456, 5);
            CheckValidParsedValue("9223372036854775807", 0, long.MaxValue, 19);            
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("", 0);
            CheckInvalidParsedValue("  ", 2);
            CheckInvalidParsedValue("a", 0);
            CheckInvalidParsedValue(".123", 0);
            CheckInvalidParsedValue(".", 0);
            CheckInvalidParsedValue("12a", 0);
            CheckInvalidParsedValue("a12b", 1);
            CheckInvalidParsedValue("123 1", 0);
            CheckInvalidParsedValue("123.1", 0);
            CheckInvalidParsedValue(" 123 1", 0);
            CheckInvalidParsedValue("a 123 1", 1);
            CheckInvalidParsedValue("a 123 1 ", 1);
            CheckInvalidParsedValue("-123.1", 0);
            CheckInvalidParsedValue("-123", 0);
            CheckInvalidParsedValue("123456789012345678901234567890", 0); // value >> Int64.MaxValue
            CheckInvalidParsedValue("9223372036854775808", 0); // value = Int64.MaxValue + 1
        }

        [TestMethod]
        public void ToString_UseDifferentValues_MatchExpectation()
        {
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;

            Assert.AreEqual("123456789012345", parser.ToString((long)123456789012345));
            Assert.AreEqual("0", parser.ToString((long)0));
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, long expectedResult, int expectedIndex)
        {
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            object result = 0;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedResult, result, "Parsed value.");
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            Int64NumberHeaderParser parser = Int64NumberHeaderParser.Parser;
            object result = 0;
            int newIndex = startIndex;
            Assert.IsFalse(parser.TryParseValue(input, null, ref newIndex, out result), "TryParse returned true: {0}",
                input);
            Assert.AreEqual(null, result, "Parsed value.");
            Assert.AreEqual(startIndex, newIndex, "Returned index.");
        }

        #endregion
    }
}
