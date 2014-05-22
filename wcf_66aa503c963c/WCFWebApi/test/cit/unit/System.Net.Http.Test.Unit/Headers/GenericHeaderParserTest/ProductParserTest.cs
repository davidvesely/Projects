using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class ProductParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueProductParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = GenericHeaderParser.SingleValueProductParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X y/1 ", 1, new ProductHeaderValue("y", "1"), 6);
            CheckValidParsedValue(", , custom / 1.0 ,,Y", 0, new ProductHeaderValue("custom", "1.0"), 19);
            CheckValidParsedValue(", , custom / 1.0 ,,", 0, new ProductHeaderValue("custom", "1.0"), 19);
            CheckValidParsedValue(", , custom / 1.0", 0, new ProductHeaderValue("custom", "1.0"), 16);

            CheckValidParsedValue(null, 0, null, 0);
            CheckValidParsedValue(string.Empty, 0, null, 0);
            CheckValidParsedValue("  ", 0, null, 2);
            CheckValidParsedValue("  ,,", 0, null, 4);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("product/version=", 0); // only delimiter ',' allowed after last product
            CheckInvalidParsedValue("product otherproduct", 0);
            CheckInvalidParsedValue("product[", 0);
            CheckInvalidParsedValue("=", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, ProductHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueProductParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}'", input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueProductParser;
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
