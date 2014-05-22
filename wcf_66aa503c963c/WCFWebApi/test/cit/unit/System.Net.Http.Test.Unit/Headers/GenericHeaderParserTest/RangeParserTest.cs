using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class RangeParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.RangeParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X bytes=1-2 ", 1, new RangeHeaderValue(1, 2), 12);

            RangeHeaderValue expected = new RangeHeaderValue();
            expected.Unit = "custom";
            expected.Ranges.Add(new RangeItemHeaderValue(null, 5));
            expected.Ranges.Add(new RangeItemHeaderValue(1, 4));
            CheckValidParsedValue("custom = -  5 , 1 - 4 ,,", 0, expected, 24);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("bytes=1-2x", 0); // only delimiter ',' allowed after last range
            CheckInvalidParsedValue("x bytes=1-2", 0);
            CheckInvalidParsedValue("bytes=1-2.4", 0);
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, RangeHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.RangeParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}'", input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.RangeParser;
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
