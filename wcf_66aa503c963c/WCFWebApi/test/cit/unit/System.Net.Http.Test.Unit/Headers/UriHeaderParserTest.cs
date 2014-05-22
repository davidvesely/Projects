using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class UriHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            UriHeaderParser parser = UriHeaderParser.RelativeOrAbsoluteUriParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // We don't need to validate all possible Uri values, since we use Uri.TryParse().
            // Just make sure the parser calls Uri.TryParse() correctly.
            CheckValidParsedValue("/this/is/a/rel/uri", 0, new Uri("/this/is/a/rel/uri", UriKind.Relative), 18);
            CheckValidParsedValue("!!  http://example.com/path,/ ", 2, new Uri("http://example.com/path,/"), 30);
            
            // Note that Uri.TryParse(.., UriKind.Relative) doesn't remove whitespaces
            CheckValidParsedValue("!!  /path/x,/  ", 2, new Uri("  /path/x,/  ", UriKind.Relative), 15);
            CheckValidParsedValue("  http://example.com/path/?query=value   ", 2, new Uri("http://example.com/path/?query=value"), 41);
            CheckValidParsedValue("  http://example.com/path/?query=value \r\n  ", 2, new Uri("http://example.com/path/?query=value"), 43);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("http://example.com,", 0);

            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
            CheckInvalidParsedValue(string.Empty, 0);
            CheckInvalidParsedValue("  ", 2);
            CheckInvalidParsedValue("  ", 2);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, Uri expectedResult, int expectedIndex)
        {
            UriHeaderParser parser = UriHeaderParser.RelativeOrAbsoluteUriParser;

            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            UriHeaderParser parser = UriHeaderParser.RelativeOrAbsoluteUriParser;

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
