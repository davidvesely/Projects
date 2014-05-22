using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers.GenericHeaderParserTest
{
    [TestClass]
    public class AuthenticationParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            HttpHeaderParser parser = GenericHeaderParser.MultipleValueAuthenticationParser;
            Assert.IsTrue(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");

            parser = GenericHeaderParser.SingleValueAuthenticationParser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Note that there is no difference between setting "SupportMultipleValues" to true or false: The parser
            // is only able to parse one authentication information per string. Setting "SupportMultipleValues" just
            // tells the caller (HttpHeaders) that parsing multiple strings is allowed.
            CheckValidParsedValue("X NTLM ", 1, new AuthenticationHeaderValue("NTLM"), 7, true);
            CheckValidParsedValue("X NTLM ", 1, new AuthenticationHeaderValue("NTLM"), 7, false);
            CheckValidParsedValue("custom x=y", 0, new AuthenticationHeaderValue("Custom", "x=y"), 10, true);
            CheckValidParsedValue("custom x=y", 0, new AuthenticationHeaderValue("Custom", "x=y"), 10, false);
            CheckValidParsedValue("C x=y, other", 0, new AuthenticationHeaderValue("C", "x=y"), 7, true);

            CheckValidParsedValue("  ", 0, null, 2, true);
            CheckValidParsedValue(null, 0, null, 0, true);
            CheckValidParsedValue("", 0, null, 0, true);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("NTLM[", 0, true); // only delimiter ',' allowed after last range
            CheckInvalidParsedValue("NTLM[", 0, false); // only delimiter ',' allowed after last range
            CheckInvalidParsedValue("]NTLM", 0, true);
            CheckInvalidParsedValue("]NTLM", 0, false);
            CheckInvalidParsedValue("C x=y, other", 0, false);
            CheckInvalidParsedValue("C x=y,", 0, false);
            CheckInvalidParsedValue("  ", 0, false);
            CheckInvalidParsedValue(null, 0, false);
            CheckInvalidParsedValue(string.Empty, 0, false);
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, AuthenticationHeaderValue expectedResult,
            int expectedIndex, bool supportMultipleValues)
        {
            HttpHeaderParser parser = null;
            if (supportMultipleValues)
            {
                parser = GenericHeaderParser.MultipleValueAuthenticationParser;
            }
            else
            {
                parser = GenericHeaderParser.SingleValueAuthenticationParser;
            }

            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result),
                "TryParse returned false. Input: '{0}'", input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index. Input: {0}", input);
            Assert.AreEqual(result, expectedResult, "Result doesn't match expected object. Input: '{0}'", input);
        }

        private void CheckInvalidParsedValue(string input, int startIndex, bool supportMultipleValues)
        {
            HttpHeaderParser parser = null;
            if (supportMultipleValues)
            {
                parser = GenericHeaderParser.MultipleValueAuthenticationParser;
            }
            else
            {
                parser = GenericHeaderParser.SingleValueAuthenticationParser;
            }

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
