using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class ContentRangeHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_LengthOnlyOverloadUseInvalidValues_Throw()
        {
            ContentRangeHeaderValue v = new ContentRangeHeaderValue(-1);
        }

        [TestMethod]
        public void Ctor_LengthOnlyOverloadValidValues_ValuesCorrectlySet()
        {
            ContentRangeHeaderValue range = new ContentRangeHeaderValue(5);

            Assert.IsFalse(range.HasRange, "HasRange");
            Assert.IsTrue(range.HasLength, "HasLength");
            Assert.AreEqual("bytes", range.Unit, "Unit");
            Assert.IsNull(range.From, "From");
            Assert.IsNull(range.To, "To");
            Assert.AreEqual(5, range.Length, "Length");
        }

        [TestMethod]
        public void Ctor_FromAndToOverloadUseInvalidValues_Throw()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(-1, 1),
                "Negative 'from'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(0, -1),
                "Negative 'to'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(2, 1),
                "'from' > 'to'");
        }

        [TestMethod]
        public void Ctor_FromAndToOverloadValidValues_ValuesCorrectlySet()
        {
            ContentRangeHeaderValue range = new ContentRangeHeaderValue(0, 1);

            Assert.IsTrue(range.HasRange, "HasRange");
            Assert.IsFalse(range.HasLength, "HasLength");
            Assert.AreEqual("bytes", range.Unit, "Unit");
            Assert.AreEqual(0, range.From, "From");
            Assert.AreEqual(1, range.To, "To");
            Assert.IsNull(range.Length, "Length");
        }

        [TestMethod]
        public void Ctor_FromToAndLengthOverloadUseInvalidValues_Throw()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(-1, 1, 2),
                "Negative 'from'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(0, -1, 2),
                "Negative 'to'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(0, 1, -1),
                "Negative 'length'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(2, 1, 3),
                "'from' > 'to'");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new ContentRangeHeaderValue(1, 2, 1),
                "'to' > 'length'");
        }

        [TestMethod]
        public void Ctor_FromToAndLengthOverloadValidValues_ValuesCorrectlySet()
        {
            ContentRangeHeaderValue range = new ContentRangeHeaderValue(0, 1, 2);

            Assert.IsTrue(range.HasRange, "HasRange");
            Assert.IsTrue(range.HasLength, "HasLength");
            Assert.AreEqual("bytes", range.Unit, "Unit");
            Assert.AreEqual(0, range.From, "From");
            Assert.AreEqual(1, range.To, "To");
            Assert.AreEqual(2, range.Length, "Length");
        }

        [TestMethod]
        public void Unit_GetAndSetValidAndInvalidValues_MatchExpectation()
        {
            ContentRangeHeaderValue range = new ContentRangeHeaderValue(0);
            range.Unit = "myunit";
            Assert.AreEqual("myunit", range.Unit, "Unit (custom value)");

            ExceptionAssert.Throws<ArgumentException>(() => range.Unit = null, "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => range.Unit = "", "empty string");
            ExceptionAssert.ThrowsFormat(() => range.Unit = " x", "leading space");
            ExceptionAssert.ThrowsFormat(() => range.Unit = "x ", "trailing space");
            ExceptionAssert.ThrowsFormat(() => range.Unit = "x y", "invalid token");
        }

        [TestMethod]
        public void ToString_UseDifferentRanges_AllSerializedCorrectly()
        {
            ContentRangeHeaderValue range = new ContentRangeHeaderValue(1, 2, 3);
            range.Unit = "myunit";
            Assert.AreEqual("myunit 1-2/3", range.ToString(), "Range with all fields set");

            range = new ContentRangeHeaderValue(123456789012345678, 123456789012345679);
            Assert.AreEqual("bytes 123456789012345678-123456789012345679/*", range.ToString(), "Only range, no length");

            range = new ContentRangeHeaderValue(150);
            Assert.AreEqual("bytes */150", range.ToString(), "Only length, no range");
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRanges_SameOrDifferentHashCodes()
        {
            ContentRangeHeaderValue range1 = new ContentRangeHeaderValue(1, 2, 5);
            ContentRangeHeaderValue range2 = new ContentRangeHeaderValue(1, 2);
            ContentRangeHeaderValue range3 = new ContentRangeHeaderValue(5);
            ContentRangeHeaderValue range4 = new ContentRangeHeaderValue(1, 2, 5);
            range4.Unit = "BYTES";
            ContentRangeHeaderValue range5 = new ContentRangeHeaderValue(1, 2, 5);
            range5.Unit = "myunit";

            Assert.AreNotEqual(range1.GetHashCode(), range2.GetHashCode(), "bytes 1-2/5 vs. bytes 1-2/*");
            Assert.AreNotEqual(range1.GetHashCode(), range3.GetHashCode(), "bytes 1-2/5 vs. bytes */5");
            Assert.AreNotEqual(range2.GetHashCode(), range3.GetHashCode(), "bytes 1-2/* vs. bytes */5");
            Assert.AreEqual(range1.GetHashCode(), range4.GetHashCode(), "bytes 1-2/5 vs. BYTES 1-2/5");
            Assert.AreNotEqual(range1.GetHashCode(), range5.GetHashCode(), "bytes 1-2/5 vs. myunit 1-2/5");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            ContentRangeHeaderValue range1 = new ContentRangeHeaderValue(1, 2, 5);
            ContentRangeHeaderValue range2 = new ContentRangeHeaderValue(1, 2);
            ContentRangeHeaderValue range3 = new ContentRangeHeaderValue(5);
            ContentRangeHeaderValue range4 = new ContentRangeHeaderValue(1, 2, 5);
            range4.Unit = "BYTES";
            ContentRangeHeaderValue range5 = new ContentRangeHeaderValue(1, 2, 5);
            range5.Unit = "myunit";
            ContentRangeHeaderValue range6 = new ContentRangeHeaderValue(1, 3, 5);
            ContentRangeHeaderValue range7 = new ContentRangeHeaderValue(2, 2, 5);
            ContentRangeHeaderValue range8 = new ContentRangeHeaderValue(1, 2, 6);

            Assert.IsFalse(range1.Equals(null), "bytes 1-2/5 vs. <null>");
            Assert.IsFalse(range1.Equals(range2), "bytes 1-2/5 vs. bytes 1-2/*");
            Assert.IsFalse(range1.Equals(range3), "bytes 1-2/5 vs. bytes */5");
            Assert.IsFalse(range2.Equals(range3), "bytes 1-2/* vs. bytes */5");
            Assert.IsTrue(range1.Equals(range4), "bytes 1-2/5 vs. BYTES 1-2/5");
            Assert.IsTrue(range4.Equals(range1), "BYTES 1-2/5 vs. bytes 1-2/5");
            Assert.IsFalse(range1.Equals(range5), "bytes 1-2/5 vs. myunit 1-2/5");
            Assert.IsFalse(range1.Equals(range6), "bytes 1-2/5 vs. bytes 1-3/5");
            Assert.IsFalse(range1.Equals(range7), "bytes 1-2/5 vs. bytes 2-2/5");
            Assert.IsFalse(range1.Equals(range8), "bytes 1-2/5 vs. bytes 1-2/6");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            ContentRangeHeaderValue source = new ContentRangeHeaderValue(1, 2, 5);
            source.Unit = "custom";
            ContentRangeHeaderValue clone = (ContentRangeHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.Unit, clone.Unit, "Unit");
            Assert.AreEqual(source.From, clone.From, "From");
            Assert.AreEqual(source.To, clone.To, "To");
            Assert.AreEqual(source.Length, clone.Length, "Length");
        }

        [TestMethod]
        public void GetContentRangeLength_DifferentValidScenarios_AllReturnNonZero()
        {
            ContentRangeHeaderValue result = null;

            CallGetContentRangeLength("bytes 1-2/3", 0, 11, out result);
            Assert.AreEqual("bytes", result.Unit, "Unit");
            Assert.AreEqual(1, result.From, "From");
            Assert.AreEqual(2, result.To, "To");
            Assert.AreEqual(3, result.Length, "Length");
            Assert.IsTrue(result.HasRange, "HasRange");
            Assert.IsTrue(result.HasLength, "HasLength");

            CallGetContentRangeLength(" custom 1234567890123456789-1234567890123456799/*", 1, 48, out result);
            Assert.AreEqual("custom", result.Unit, "Unit");
            Assert.AreEqual(1234567890123456789, result.From, "From");
            Assert.AreEqual(1234567890123456799, result.To, "To");
            Assert.IsNull(result.Length, "Length");
            Assert.IsTrue(result.HasRange, "HasRange");
            Assert.IsFalse(result.HasLength, "HasLength");

            // Note that the final space should be skipped by GetContentRangeLength() and be considered by the returned
            // value.            
            CallGetContentRangeLength(" custom * / 123 ", 1, 15, out result);
            Assert.AreEqual("custom", result.Unit, "Unit");
            Assert.IsNull(result.From, "From");
            Assert.IsNull(result.To, "To");
            Assert.AreEqual(123, result.Length, "Length");
            Assert.IsFalse(result.HasRange, "HasRange");
            Assert.IsTrue(result.HasLength, "HasLength");

            // Note that we don't have a public constructor for value 'bytes */*' since the RFC doesn't mentione a 
            // scenario for it. However, if a server returns this value, we're flexible and accept it.
            CallGetContentRangeLength("bytes */*", 0, 9, out result);
            Assert.AreEqual("bytes", result.Unit, "Unit");
            Assert.IsNull(result.From, "From");
            Assert.IsNull(result.To, "To");
            Assert.IsNull(result.Length, "Length");
            Assert.IsFalse(result.HasRange, "HasRange");
            Assert.IsFalse(result.HasLength, "HasLength");
        }

        [TestMethod]
        public void GetContentRangeLength_DifferentInvalidScenarios_AllReturnZero()
        {

            CheckInvalidGetContentRangeLength(" bytes 1-2/3", 0);
            CheckInvalidGetContentRangeLength("bytes 3-2/5", 0);
            CheckInvalidGetContentRangeLength("bytes 6-6/5", 0);
            CheckInvalidGetContentRangeLength("bytes 1-6/5", 0);
            CheckInvalidGetContentRangeLength("bytes 1-2/", 0);
            CheckInvalidGetContentRangeLength("bytes 1-2", 0);
            CheckInvalidGetContentRangeLength("bytes 1-/", 0);
            CheckInvalidGetContentRangeLength("bytes 1-", 0);
            CheckInvalidGetContentRangeLength("bytes 1", 0);
            CheckInvalidGetContentRangeLength("bytes ", 0);
            CheckInvalidGetContentRangeLength("bytes a-2/3", 0);
            CheckInvalidGetContentRangeLength("bytes 1-b/3", 0);
            CheckInvalidGetContentRangeLength("bytes 1-2/c", 0);
            CheckInvalidGetContentRangeLength("bytes1-2/3", 0);
            
            // More than 19 digits >>Int64.MaxValue
            CheckInvalidGetContentRangeLength("bytes 1-12345678901234567890/3", 0);
            CheckInvalidGetContentRangeLength("bytes 12345678901234567890-3/3", 0);
            CheckInvalidGetContentRangeLength("bytes 1-2/12345678901234567890", 0);

            // Exceed Int64.MaxValue, but use 19 digits
            CheckInvalidGetContentRangeLength("bytes 1-9999999999999999999/3", 0);
            CheckInvalidGetContentRangeLength("bytes 9999999999999999999-3/3", 0);
            CheckInvalidGetContentRangeLength("bytes 1-2/9999999999999999999", 0);

            CheckInvalidGetContentRangeLength(string.Empty, 0);
            CheckInvalidGetContentRangeLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Only verify parser functionality (i.e. ContentRangeHeaderParser.TryParse()). We don't need to validate
            // all possible range values (verification done by tests for ContentRangeHeaderValue.GetContentRangeLength()).
            CheckValidParse(" bytes 1-2/3 ", new ContentRangeHeaderValue(1, 2, 3));
            CheckValidParse("bytes  *  /  3", new ContentRangeHeaderValue(3));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("bytes 1-2/3,"); // no character after 'length' allowed
            CheckInvalidParse("x bytes 1-2/3");
            CheckInvalidParse("bytes 1-2/3.4");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Only verify parser functionality (i.e. ContentRangeHeaderParser.TryParse()). We don't need to validate
            // all possible range values (verification done by tests for ContentRangeHeaderValue.GetContentRangeLength()).
            CheckValidTryParse(" bytes 1-2/3 ", new ContentRangeHeaderValue(1, 2, 3));
            CheckValidTryParse("bytes  *  /  3", new ContentRangeHeaderValue(3));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("bytes 1-2/3,"); // no character after 'length' allowed
            CheckInvalidTryParse("x bytes 1-2/3");
            CheckInvalidTryParse("bytes 1-2/3.4");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
        }

        #region Helper methods

        private void CheckValidParse(string input, ContentRangeHeaderValue expectedResult)
        {
            ContentRangeHeaderValue result = ContentRangeHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => ContentRangeHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, ContentRangeHeaderValue expectedResult)
        {
            ContentRangeHeaderValue result = null;
            Assert.IsTrue(ContentRangeHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            ContentRangeHeaderValue result = null;
            Assert.IsFalse(ContentRangeHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetContentRangeLength(string input, int startIndex, int expectedLength,
            out ContentRangeHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, ContentRangeHeaderValue.GetContentRangeLength(input, startIndex, 
                out temp), "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as ContentRangeHeaderValue;
        }

        private static void CheckInvalidGetContentRangeLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, ContentRangeHeaderValue.GetContentRangeLength(input, startIndex,
                out result), "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
