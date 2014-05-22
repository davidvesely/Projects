using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;

namespace Microsoft.Net.Http.Test.Headers
{
    [TestClass]
    public class ByteArrayHeaderParserTest
    {
        [TestMethod]
        public void Properties_ReadValues_MatchExpectation()
        {
            ByteArrayHeaderParser parser = ByteArrayHeaderParser.Parser;
            Assert.IsFalse(parser.SupportsMultipleValues, "SupportsMultipleValues");
            Assert.IsNull(parser.Comparer, "Comparer");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_NullValue_Throw()
        {
            ByteArrayHeaderParser parser = ByteArrayHeaderParser.Parser;
            int index = 0;
            parser.ParseValue(null, null, ref index);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParsedValue("X  A/b+CQ== ", 1, new byte[] { 3, 246, 254, 9 }, 12);
            CheckValidParsedValue("AQ==", 0, new byte[] { 1 }, 4);
            
            // Note that Convert.FromBase64String() is tolerant with whitespace characters in the middle of the Base64
            // string:
            CheckValidParsedValue(" AbCdE fGhI  jKl+/Mn \r\n \t", 0, 
                new byte[] { 1, 176, 157, 17, 241, 161, 34, 50, 165, 251, 243, 39 }, 25);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidParsedValue("", 0);
            CheckInvalidParsedValue("  ", 2);
            CheckInvalidParsedValue("a", 0);
            CheckInvalidParsedValue("AQ", 0);
            CheckInvalidParsedValue("AQ== X", 0);
            CheckInvalidParsedValue("AQ==,", 0);
            CheckInvalidParsedValue("AQ==A", 0);
            CheckInvalidParsedValue("AQ== ,", 0);
            CheckInvalidParsedValue(", AQ==", 0);
            CheckInvalidParsedValue(" ,AQ==", 0);
            CheckInvalidParsedValue("=", 0);            
        }

        [TestMethod]
        public void ToString_UseDifferentValues_MatchExpectation()
        {
            ByteArrayHeaderParser parser = ByteArrayHeaderParser.Parser;
            Assert.AreEqual("A/b+CQ==", parser.ToString(new byte[] { 3, 246, 254, 9 }));
        }

        #region Helper methods

        private void CheckValidParsedValue(string input, int startIndex, byte[] expectedResult, int expectedIndex)
        {
            ByteArrayHeaderParser parser = ByteArrayHeaderParser.Parser;
            object result = 0;
            Assert.IsTrue(parser.TryParseValue(input, null, ref startIndex, out result), "TryParse returned false: {0}",
                input);
            Assert.AreEqual(expectedIndex, startIndex, "Returned index.");

            if (result == null)
            {
                Assert.IsNull(expectedResult, "Result was 'null' but non-null was expected. Input: '{0}'", input);
            }
            else
            {
                byte[] arrayResult = (byte[])result;                
                CollectionAssert.AreEqual(expectedResult, arrayResult, "Collections are not equal. Input: '{0}'",
                    input);
            }
        }

        private void CheckInvalidParsedValue(string input, int startIndex)
        {
            ByteArrayHeaderParser parser = ByteArrayHeaderParser.Parser;
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
