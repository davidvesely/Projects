using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class TokenListParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.TokenListParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.AreEqual(HeaderUtilities.CaseInsensitiveStringComparer, parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("text", 0, "text", 4);
            CheckValidParsedValue("text,", 0, "text", 5);
            CheckValidParsedValue("\r\n text , next_text  ", 0, "text", 10);
            CheckValidParsedValue("  text,next_text  ", 2, "text", 7);
            CheckValidParsedValue(" ,, text, , ,next", 0, "text", 13);
            CheckValidParsedValue(" ,, text, , ,", 0, "text", 13);

            CheckValidParsedValue(null, 0, null, 0);
            CheckValidParsedValue(string.Empty, 0, null, 0);
            CheckValidParsedValue("   ", 0, null, 3);
            CheckValidParsedValue("  ,,", 0, null, 4);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("teäxt", 0);
            CheckInvalidParsedValue("text会", 0);
            CheckInvalidParsedValue("会", 0);            
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, string expectedResult, int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.TokenListParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.TokenListParser;
            object result = null;
            int newIndex = startIndex;
            Assert.IsFalse(parser.TryParseValue(input, null, ref newIndex, out result), "TryParse returned true: {0}", 
                input);
            Assert.AreEqual(null, result, "Parsed value");
            Assert.AreEqual(startIndex, newIndex, "Returned index");
        }

        #endregion
    }
}
