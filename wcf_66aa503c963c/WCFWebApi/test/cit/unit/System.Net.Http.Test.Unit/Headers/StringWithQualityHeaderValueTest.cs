using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class StringWithQualityHeaderValueTest
    {
        [TestMethod]
        public void Ctor_StringOnlyOverload_MatchExpectation()
        {
            StringWithQualityHeaderValue value = new StringWithQualityHeaderValue("token");
            Assert.AreEqual("token", value.Value, "Value");
            Assert.IsNull(value.Quality, "Quality");

            ExceptionAssert.Throws<ArgumentException>(() => { new StringWithQualityHeaderValue(null); }, "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => { new StringWithQualityHeaderValue(""); }, "empty string");
            ExceptionAssert.ThrowsFormat(() => { new StringWithQualityHeaderValue("in valid"); }, "invalid");
        }

        [TestMethod]
        public void Ctor_StringWithQualityOverload_MatchExpectation()
        {
            StringWithQualityHeaderValue value = new StringWithQualityHeaderValue("token", 0.5);
            Assert.AreEqual("token", value.Value, "Value");
            Assert.AreEqual(0.5, value.Quality, "Quality");

            ExceptionAssert.Throws<ArgumentException>(() => { new StringWithQualityHeaderValue(null, 0.1); }, "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => { new StringWithQualityHeaderValue("", 0.1); }, "empty string");
            ExceptionAssert.ThrowsFormat(() => { new StringWithQualityHeaderValue("in valid", 0.1); }, "invalid");

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => { new StringWithQualityHeaderValue("t", 1.1); },
                "1.1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => { new StringWithQualityHeaderValue("t", -0.1); },
                "-0.00001");
        }

        [TestMethod]
        public void ToString_UseDifferentValues_AllSerializedCorrectly()
        {
            StringWithQualityHeaderValue value = new StringWithQualityHeaderValue("token");
            Assert.AreEqual("token", value.ToString());

            value = new StringWithQualityHeaderValue("token", 0.1);
            Assert.AreEqual("token; q=0.1", value.ToString());

            value = new StringWithQualityHeaderValue("token", 0);
            Assert.AreEqual("token; q=0.0", value.ToString());

            value = new StringWithQualityHeaderValue("token", 1);
            Assert.AreEqual("token; q=1.0", value.ToString());

            // Note that the quality value gets rounded
            value = new StringWithQualityHeaderValue("token", 0.56789);
            Assert.AreEqual("token; q=0.568", value.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentValues_SameOrDifferentHashCodes()
        {
            StringWithQualityHeaderValue value1 = new StringWithQualityHeaderValue("t", 0.123);
            StringWithQualityHeaderValue value2 = new StringWithQualityHeaderValue("t", 0.123);
            StringWithQualityHeaderValue value3 = new StringWithQualityHeaderValue("T", 0.123);
            StringWithQualityHeaderValue value4 = new StringWithQualityHeaderValue("t");
            StringWithQualityHeaderValue value5 = new StringWithQualityHeaderValue("x", 0.123);
            StringWithQualityHeaderValue value6 = new StringWithQualityHeaderValue("t", 0.5);
            StringWithQualityHeaderValue value7 = new StringWithQualityHeaderValue("t", 0.1234);
            StringWithQualityHeaderValue value8 = new StringWithQualityHeaderValue("T");
            StringWithQualityHeaderValue value9 = new StringWithQualityHeaderValue("x");

            Assert.AreEqual(value1.GetHashCode(), value2.GetHashCode(), "t; q=0.123 vs. t; q=0.123");
            Assert.AreEqual(value1.GetHashCode(), value3.GetHashCode(), "t; q=0.123 vs. T; q=0.123");
            Assert.AreNotEqual(value1.GetHashCode(), value4.GetHashCode(), "t; q=0.123 vs. t");
            Assert.AreNotEqual(value1.GetHashCode(), value5.GetHashCode(), "t; q=0.123 vs. x; q=0.123");
            Assert.AreNotEqual(value1.GetHashCode(), value6.GetHashCode(), "t; q=0.123 vs. t; q=0.5");
            Assert.AreNotEqual(value1.GetHashCode(), value7.GetHashCode(), "t; q=0.123 vs. t; q=0.1234");
            Assert.AreEqual(value4.GetHashCode(), value8.GetHashCode(), "t vs. T");
            Assert.AreNotEqual(value4.GetHashCode(), value9.GetHashCode(), "t vs. T");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            StringWithQualityHeaderValue value1 = new StringWithQualityHeaderValue("t", 0.123);
            StringWithQualityHeaderValue value2 = new StringWithQualityHeaderValue("t", 0.123);
            StringWithQualityHeaderValue value3 = new StringWithQualityHeaderValue("T", 0.123);
            StringWithQualityHeaderValue value4 = new StringWithQualityHeaderValue("t");
            StringWithQualityHeaderValue value5 = new StringWithQualityHeaderValue("x", 0.123);
            StringWithQualityHeaderValue value6 = new StringWithQualityHeaderValue("t", 0.5);
            StringWithQualityHeaderValue value7 = new StringWithQualityHeaderValue("t", 0.1234);
            StringWithQualityHeaderValue value8 = new StringWithQualityHeaderValue("T");
            StringWithQualityHeaderValue value9 = new StringWithQualityHeaderValue("x");

            Assert.IsFalse(value1.Equals(null), "t; q=0.123 vs. <null>");
            Assert.IsTrue(value1.Equals(value2), "t; q=0.123 vs. t; q=0.123");
            Assert.IsTrue(value1.Equals(value3), "t; q=0.123 vs. T; q=0.123");
            Assert.IsFalse(value1.Equals(value4), "t; q=0.123 vs. t");
            Assert.IsFalse(value4.Equals(value1), "t vs. t; q=0.123");
            Assert.IsFalse(value1.Equals(value5), "t; q=0.123 vs. x; q=0.123");
            Assert.IsFalse(value1.Equals(value6), "t; q=0.123 vs. t; q=0.5");
            Assert.IsFalse(value1.Equals(value7), "t; q=0.123 vs. t; q=0.1234");
            Assert.IsTrue(value4.Equals(value8), "t vs. T");
            Assert.IsFalse(value4.Equals(value9), "t vs. T");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            StringWithQualityHeaderValue source = new StringWithQualityHeaderValue("token", 0.123);
            StringWithQualityHeaderValue clone = (StringWithQualityHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Value, clone.Value, "Value");
            Assert.AreEqual(source.Quality, clone.Quality, "Quality");

            source = new StringWithQualityHeaderValue("token");
            clone = (StringWithQualityHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Value, clone.Value, "Value");
            Assert.IsNull(source.Quality, "Quality");
        }

        [TestMethod]
        public void GetStringWithQualityLength_DifferentValidScenarios_AllReturnNonZero()
        {
            StringWithQualityHeaderValue result = null;

            CallGetStringWithQualityLength(" token ; q = 0.123 ,", 1, 18, out result);
            Assert.AreEqual("token", result.Value, "Value");
            Assert.AreEqual(0.123, result.Quality, "Quality");

            CallGetStringWithQualityLength("token;q=1 , x", 0, 10, out result);
            Assert.AreEqual("token", result.Value, "Value");
            Assert.AreEqual(1, result.Quality, "Quality");

            CallGetStringWithQualityLength("*", 0, 1, out result);
            Assert.AreEqual("*", result.Value, "Value");
            Assert.IsNull(result.Quality, "Quality");

            CallGetStringWithQualityLength("t;q=0.", 0, 6, out result);
            Assert.AreEqual("t", result.Value, "Value");
            Assert.AreEqual(0, result.Quality, "Quality");

            CallGetStringWithQualityLength("t;q=1.,", 0, 6, out result);
            Assert.AreEqual("t", result.Value, "Value");
            Assert.AreEqual(1, result.Quality, "Quality");

            CallGetStringWithQualityLength("t ;  q  =   0X", 0, 13, out result);
            Assert.AreEqual("t", result.Value, "Value");
            Assert.AreEqual(0, result.Quality, "Quality");

            CallGetStringWithQualityLength("t ;  q  =   0,", 0, 13, out result);
            Assert.AreEqual("t", result.Value, "Value");
            Assert.AreEqual(0, result.Quality, "Quality");
        }

        [TestMethod]
        public void GetStringWithQualityLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetStringWithQualityLength(" t", 0); // no leading whitespaces allowed
            CheckInvalidGetStringWithQualityLength("t;q=", 0);
            CheckInvalidGetStringWithQualityLength("t;q=-1", 0);
            CheckInvalidGetStringWithQualityLength("t;q=1.00001", 0);
            CheckInvalidGetStringWithQualityLength("t;q", 0);
            CheckInvalidGetStringWithQualityLength("t;", 0);
            CheckInvalidGetStringWithQualityLength("t;;q=1", 0);
            CheckInvalidGetStringWithQualityLength("t;q=a", 0);
            CheckInvalidGetStringWithQualityLength("t;qa", 0);
            CheckInvalidGetStringWithQualityLength("t;q1", 0);

            CheckInvalidGetStringWithQualityLength("", 0);
            CheckInvalidGetStringWithQualityLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("text", new StringWithQualityHeaderValue("text"));
            CheckValidParse("text;q=0.5", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidParse("text ; q = 0.5", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidParse("\r\n text ; q = 0.5 ", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidParse("  text  ", new StringWithQualityHeaderValue("text"));
            CheckValidParse(" \r\n text \r\n ; \r\n q = 0.123", new StringWithQualityHeaderValue("text", 0.123));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("text,");
            CheckInvalidParse("\r\n text ; q = 0.5, next_text  ");
            CheckInvalidParse("  text,next_text  ");
            CheckInvalidParse(" ,, text, , ,next");
            CheckInvalidParse(" ,, text, , ,");
            CheckInvalidParse(", \r\n text \r\n ; \r\n q = 0.123");
            CheckInvalidParse("teäxt");
            CheckInvalidParse("text会");
            CheckInvalidParse("会");
            CheckInvalidParse("t;q=会");
            CheckInvalidParse("t;q=");
            CheckInvalidParse("t;q");
            CheckInvalidParse("t;会=1");
            CheckInvalidParse("t;q会=1");
            CheckInvalidParse("t y");
            CheckInvalidParse("t;q=1 y");

            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("text", new StringWithQualityHeaderValue("text"));
            CheckValidTryParse("text;q=0.5", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidTryParse("text ; q = 0.5", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidTryParse("\r\n text ; q = 0.5 ", new StringWithQualityHeaderValue("text", 0.5));
            CheckValidTryParse("  text  ", new StringWithQualityHeaderValue("text"));
            CheckValidTryParse(" \r\n text \r\n ; \r\n q = 0.123", new StringWithQualityHeaderValue("text", 0.123));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("text,");
            CheckInvalidTryParse("\r\n text ; q = 0.5, next_text  ");
            CheckInvalidTryParse("  text,next_text  ");
            CheckInvalidTryParse(" ,, text, , ,next");
            CheckInvalidTryParse(" ,, text, , ,");
            CheckInvalidTryParse(", \r\n text \r\n ; \r\n q = 0.123");
            CheckInvalidTryParse("teäxt");
            CheckInvalidTryParse("text会");
            CheckInvalidTryParse("会");
            CheckInvalidTryParse("t;q=会");
            CheckInvalidTryParse("t;q=");
            CheckInvalidTryParse("t;q");
            CheckInvalidTryParse("t;会=1");
            CheckInvalidTryParse("t;q会=1");
            CheckInvalidTryParse("t y");
            CheckInvalidTryParse("t;q=1 y");

            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
        }

        #region Helper methods

        private void CheckValidParse(string input, StringWithQualityHeaderValue expectedResult)
        {
            StringWithQualityHeaderValue result = StringWithQualityHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => StringWithQualityHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, StringWithQualityHeaderValue expectedResult)
        {
            StringWithQualityHeaderValue result = null;
            Assert.IsTrue(StringWithQualityHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            StringWithQualityHeaderValue result = null;
            Assert.IsFalse(StringWithQualityHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetStringWithQualityLength(string input, int startIndex, int expectedLength,
            out StringWithQualityHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, StringWithQualityHeaderValue.GetStringWithQualityLength(input, 
                startIndex, out temp), "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as StringWithQualityHeaderValue;
        }

        private static void CheckInvalidGetStringWithQualityLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, StringWithQualityHeaderValue.GetStringWithQualityLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
