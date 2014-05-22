using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class CacheControlHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = CacheControlHeaderParser.Parser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Just verify parser is implemented correctly. Don't try to test syntax parsed by CacheControlHeaderValue.
            CacheControlHeaderValue expected = new CacheControlHeaderValue();
            expected.NoStore = true;
            expected.MinFresh = new TimeSpan(0, 2, 3);
            CheckValidParsedValue("X , , no-store, min-fresh=123", 1, expected, 29);

            expected = new CacheControlHeaderValue();
            expected.MaxStale = true;
            expected.NoCache = true;
            expected.NoCacheHeaders.Add("t");
            CheckValidParsedValue("max-stale, no-cache=\"t\", ,,", 0, expected, 27);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("no-cache,=", 0);
            CheckInvalidParsedValue("max-age=123x", 0);
            CheckInvalidParsedValue("=no-cache", 0);
            CheckInvalidParsedValue("no-cache no-store", 0);
            CheckInvalidParsedValue("invalid =", 0);
            CheckInvalidParsedValue("会", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, CacheControlHeaderValue expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = CacheControlHeaderParser.Parser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: '{0}'", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = CacheControlHeaderParser.Parser;
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
