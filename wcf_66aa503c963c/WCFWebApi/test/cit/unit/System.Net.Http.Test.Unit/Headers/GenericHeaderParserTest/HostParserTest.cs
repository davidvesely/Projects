using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class HostParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.HostParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.AreEqual(HeaderUtilities.CaseInsensitiveStringComparer, parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("host", 0, "host", 4);
            CheckValidParsedValue(" host ", 0, "host", 6);
            CheckValidParsedValue("\r\n host\r\n ", 0, "host", 10);
            CheckValidParsedValue("!host", 1, "host", 5);
            CheckValidParsedValue("!host:50", 1, "host:50", 8);
            CheckValidParsedValue("//host会", 2, "host会", 7);
            CheckValidParsedValue("192.168.0.1", 0, "192.168.0.1", 11);
            CheckValidParsedValue(" 192.168.0.1:80 ", 0, "192.168.0.1:80", 16);
            CheckValidParsedValue("[::1]", 0, "[::1]", 5);
            CheckValidParsedValue(" [::1] ", 0, "[::1]", 7);
            CheckValidParsedValue("[::1]:1234", 0, "[::1]:1234", 10);
            CheckValidParsedValue(" [::1]:1234 ", 0, "[::1]:1234", 12);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
            CheckInvalidParsedValue("  ", 2);
            CheckInvalidParsedValue("  ", 0);
            CheckInvalidParsedValue("host:xx", 0);
            CheckInvalidParsedValue("host/", 0);
            CheckInvalidParsedValue(".host", 0); // Even though this is a valid token, "Host" expects valid URI hosts.
            CheckInvalidParsedValue("host/path", 0);
            CheckInvalidParsedValue("host: 80", 0);
            CheckInvalidParsedValue("host:123456789", 0);
            CheckInvalidParsedValue("[FE80::12]: 80", 0);
            CheckInvalidParsedValue("[FE80 ::12]", 0);
            CheckInvalidParsedValue("host host", 0);
            CheckInvalidParsedValue("host,", 0);
            CheckInvalidParsedValue("host ,", 0);
            CheckInvalidParsedValue("/", 0);
            CheckInvalidParsedValue(" , ", 0);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, string expectedResult, int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.HostParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.HostParser;
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
