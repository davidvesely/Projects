using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class MailAddressParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MailAddressParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // We don't need to validate all possible date values, since they're already tested MailAddress.
            // Just make sure the parser calls MailAddressParser with correct parameters (like startIndex must be
            // honored).

            // Note that we still have trailing whitespaces since we don't do the parsing of the email address.
            CheckValidParsedValue("!!      info@example.com   ", 2, "info@example.com   ", 27);
            CheckValidParsedValue("\r\n \"My name\" info@example.com", 0,
                "\"My name\" info@example.com", 29);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("[info@example.com", 0);
            CheckInvalidParsedValue("info@example.com\r\nother", 0);
            CheckInvalidParsedValue("info@example.com\r\n other", 0);
            CheckInvalidParsedValue("info@example.com\r\n", 0);
            CheckInvalidParsedValue("info@example.com,", 0);
            CheckInvalidParsedValue("\r\ninfo@example.com", 0);
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
            CheckInvalidParsedValue("  ", 2);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, string expectedResult,
            int expectedIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MailAddressParser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            HttpHeaderParser parser = GenericHeaderParser.MailAddressParser;
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
