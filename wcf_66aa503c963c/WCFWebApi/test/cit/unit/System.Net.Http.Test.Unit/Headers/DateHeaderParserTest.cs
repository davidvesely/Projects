using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class DateHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            DateHeaderParser parser = DateHeaderParser.Parser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // We don't need to validate all possible date values, since they're already tested in HttpRuleParserTest.
            // Just make sure the parser calls HttpRuleParser methods correctly.
            CheckValidParsedValue("Tue, 15 Nov 1994 08:12:31 GMT", 0, 
                new DateTimeOffset(1994, 11, 15, 8, 12, 31, TimeSpan.Zero), 29);
            CheckValidParsedValue("!!      Sunday, 06-Nov-94 08:49:37 GMT   ", 2,
                new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), 41);
            CheckValidParsedValue("\r\n Tue,\r\n 15 Nov\r\n 1994 08:12:31 GMT   ", 2,
                new DateTimeOffset(1994, 11, 15, 8, 12, 31, TimeSpan.Zero), 39);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue(null, 0);
            CheckInvalidParsedValue(string.Empty, 0);
            CheckInvalidParsedValue("  ", 2);
            CheckInvalidParsedValue("!!Sunday, 06-Nov-94 08:49:37 GMT", 0);
        }

        [TestMethod]
        public void ToString_UseDifferentValues_MatchExpectation()
        {
            DateHeaderParser parser = DateHeaderParser.Parser;
            
            Assert.AreEqual("Sat, 31 Jul 2010 15:38:57 GMT", 
                parser.ToString(new DateTimeOffset(2010, 7, 31, 15, 38, 57, TimeSpan.Zero)));

            Assert.AreEqual("Fri, 01 Jan 2010 01:01:01 GMT",
                parser.ToString(new DateTimeOffset(2010, 1, 1, 1, 1, 1, TimeSpan.Zero)));
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, DateTimeOffset expectedResult, 
            int expectedIndex)
        {
            DateHeaderParser parser = DateHeaderParser.Parser;
            object result = null;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");
            Assert.AreEqual(expectedResult, result, "Result doesn't match expected object");
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            DateHeaderParser parser = DateHeaderParser.Parser;
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
