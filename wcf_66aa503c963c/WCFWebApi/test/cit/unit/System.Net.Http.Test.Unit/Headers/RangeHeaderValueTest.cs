using System.Linq;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class RangeHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_InvalidRange_Throw()
        {
            new RangeHeaderValue(5, 2);

            // TODO AC 0 remove!!!
            HttpClient client = new HttpClient();
            StringContent content = new StringContent("huuuuge string");
            
        }

        [TestMethod]
        public void Unit_GetAndSetValidAndInvalidValues_MatchExpectation()
        {
            RangeHeaderValue range = new RangeHeaderValue();
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
            RangeHeaderValue range = new RangeHeaderValue();
            range.Unit = "myunit";
            range.Ranges.Add(new RangeItemHeaderValue(1, 3));
            Assert.AreEqual("myunit=1-3", range.ToString());
            
            range.Ranges.Add(new RangeItemHeaderValue(5, null));
            range.Ranges.Add(new RangeItemHeaderValue(null, 17));
            Assert.AreEqual("myunit=1-3, 5-, -17", range.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRanges_SameOrDifferentHashCodes()
        {
            RangeHeaderValue range1 = new RangeHeaderValue(1, 2);
            RangeHeaderValue range2 = new RangeHeaderValue(1, 2);
            range2.Unit = "BYTES";
            RangeHeaderValue range3 = new RangeHeaderValue(1, null);
            RangeHeaderValue range4 = new RangeHeaderValue(null, 2);
            RangeHeaderValue range5 = new RangeHeaderValue();
            range5.Ranges.Add(new RangeItemHeaderValue(1, 2));
            range5.Ranges.Add(new RangeItemHeaderValue(3, 4));
            RangeHeaderValue range6 = new RangeHeaderValue();
            range6.Ranges.Add(new RangeItemHeaderValue(3, 4)); // reverse order of range5
            range6.Ranges.Add(new RangeItemHeaderValue(1, 2));
            
            Assert.AreEqual(range1.GetHashCode(), range2.GetHashCode(), "bytes=1-2 vs. BYTES=1-2");
            Assert.AreNotEqual(range1.GetHashCode(), range3.GetHashCode(), "bytes=1-2 vs. bytes=1-");
            Assert.AreNotEqual(range1.GetHashCode(), range4.GetHashCode(), "bytes=1-2 vs. bytes=-2");
            Assert.AreNotEqual(range1.GetHashCode(), range5.GetHashCode(), "bytes=1-2 vs. bytes=1-2,3-4");
            Assert.AreEqual(range5.GetHashCode(), range6.GetHashCode(), "bytes=1-2,3-4 vs. bytes=3-4,1-2");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            RangeHeaderValue range1 = new RangeHeaderValue(1, 2);
            RangeHeaderValue range2 = new RangeHeaderValue(1, 2);
            range2.Unit = "BYTES";
            RangeHeaderValue range3 = new RangeHeaderValue(1, null);
            RangeHeaderValue range4 = new RangeHeaderValue(null, 2);
            RangeHeaderValue range5 = new RangeHeaderValue();
            range5.Ranges.Add(new RangeItemHeaderValue(1, 2));
            range5.Ranges.Add(new RangeItemHeaderValue(3, 4));
            RangeHeaderValue range6 = new RangeHeaderValue();
            range6.Ranges.Add(new RangeItemHeaderValue(3, 4)); // reverse order of range5
            range6.Ranges.Add(new RangeItemHeaderValue(1, 2));
            RangeHeaderValue range7 = new RangeHeaderValue(1, 2);
            range7.Unit = "other";

            Assert.IsFalse(range1.Equals(null), "bytes=1-2 vs. <null>");
            Assert.IsTrue(range1.Equals(range2), "bytes=1-2 vs. BYTES=1-2");
            Assert.IsFalse(range1.Equals(range3), "bytes=1-2 vs. bytes=1-");
            Assert.IsFalse(range1.Equals(range4), "bytes=1-2 vs. bytes=-2");
            Assert.IsFalse(range1.Equals(range5), "bytes=1-2 vs. bytes=1-2,3-4");
            Assert.IsTrue(range5.Equals(range6), "bytes=1-2,3-4 vs. bytes=3-4,1-2");
            Assert.IsFalse(range1.Equals(range7), "bytes=1-2 vs. other=1-2");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            RangeHeaderValue source = new RangeHeaderValue(1, 2);            
            RangeHeaderValue clone = (RangeHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(1, source.Ranges.Count, "source.Ranges.Count");
            Assert.AreEqual(source.Unit, clone.Unit, "Unit");
            Assert.AreEqual(source.Ranges.Count, clone.Ranges.Count, "Ranges.Count");
            Assert.AreEqual(source.Ranges.ElementAt(0), clone.Ranges.ElementAt(0), "Ranges.ElementAt(0)");
                        
            source.Unit = "custom";
            source.Ranges.Add(new RangeItemHeaderValue(3, null));
            source.Ranges.Add(new RangeItemHeaderValue(null, 4));
            clone = (RangeHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(3, source.Ranges.Count, "source.Ranges.Count");
            Assert.AreEqual(source.Unit, clone.Unit, "Unit");
            Assert.AreEqual(source.Ranges.Count, clone.Ranges.Count, "Ranges.Count");
            Assert.AreEqual(source.Ranges.ElementAt(0), clone.Ranges.ElementAt(0), "Ranges.ElementAt(0)");
            Assert.AreEqual(source.Ranges.ElementAt(1), clone.Ranges.ElementAt(1), "Ranges.ElementAt(1)");
            Assert.AreEqual(source.Ranges.ElementAt(2), clone.Ranges.ElementAt(2), "Ranges.ElementAt(2)");
        }

        [TestMethod]
        public void GetRangeLength_DifferentValidScenarios_AllReturnNonZero()
        {
            RangeHeaderValue result = null;

            CallGetRangeLength(" custom = 1 - 2", 1, 14, out result);
            Assert.AreEqual("custom", result.Unit, "Unit");
            Assert.AreEqual(1, result.Ranges.Count, "Ranges.Count");
            Assert.AreEqual(new RangeItemHeaderValue(1, 2), result.Ranges.First(), "Ranges.First()");

            CallGetRangeLength("bytes =1-2,,3-, , ,-4,,", 0, 23, out result);
            Assert.AreEqual("bytes", result.Unit, "Unit");
            Assert.AreEqual(3, result.Ranges.Count, "Ranges.Count");
            Assert.AreEqual(new RangeItemHeaderValue(1, 2), result.Ranges.ElementAt(0), "Ranges.ElementAt(0)");
            Assert.AreEqual(new RangeItemHeaderValue(3, null), result.Ranges.ElementAt(1), "Ranges.ElementAt(1)");
            Assert.AreEqual(new RangeItemHeaderValue(null, 4), result.Ranges.ElementAt(2), "Ranges.ElementAt(2)");
        }

        [TestMethod]
        public void GetRangeLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetRangeLength(" bytes=1-2", 0); // no leading whitespaces allowed
            CheckInvalidGetRangeLength("bytes=1", 0);
            CheckInvalidGetRangeLength("bytes=", 0);
            CheckInvalidGetRangeLength("bytes", 0);
            CheckInvalidGetRangeLength("bytes 1-2", 0);
            CheckInvalidGetRangeLength("bytes=1-2.5", 0);
            CheckInvalidGetRangeLength("bytes= ,,, , ,,", 0);
            
            CheckInvalidGetRangeLength("", 0);
            CheckInvalidGetRangeLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse(" bytes=1-2 ", new RangeHeaderValue(1, 2));

            RangeHeaderValue expected = new RangeHeaderValue();
            expected.Unit = "custom";
            expected.Ranges.Add(new RangeItemHeaderValue(null, 5));
            expected.Ranges.Add(new RangeItemHeaderValue(1, 4));
            CheckValidParse("custom = -  5 , 1 - 4 ,,", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("bytes=1-2x"); // only delimiter ',' allowed after last range
            CheckInvalidParse("x bytes=1-2");
            CheckInvalidParse("bytes=1-2.4");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse(" bytes=1-2 ", new RangeHeaderValue(1, 2));

            RangeHeaderValue expected = new RangeHeaderValue();
            expected.Unit = "custom";
            expected.Ranges.Add(new RangeItemHeaderValue(null, 5));
            expected.Ranges.Add(new RangeItemHeaderValue(1, 4));
            CheckValidTryParse("custom = -  5 , 1 - 4 ,,", expected);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("bytes=1-2x"); // only delimiter ',' allowed after last range
            CheckInvalidTryParse("x bytes=1-2");
            CheckInvalidTryParse("bytes=1-2.4");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
        }

        #region Helper methods

        private void CheckValidParse(string input, RangeHeaderValue expectedResult)
        {
            RangeHeaderValue result = RangeHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => RangeHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, RangeHeaderValue expectedResult)
        {
            RangeHeaderValue result = null;
            Assert.IsTrue(RangeHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            RangeHeaderValue result = null;
            Assert.IsFalse(RangeHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetRangeLength(string input, int startIndex, int expectedLength,
            out RangeHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, RangeHeaderValue.GetRangeLength(input, startIndex, out temp), 
                "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as RangeHeaderValue;
        }

        private static void CheckInvalidGetRangeLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, RangeHeaderValue.GetRangeLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
