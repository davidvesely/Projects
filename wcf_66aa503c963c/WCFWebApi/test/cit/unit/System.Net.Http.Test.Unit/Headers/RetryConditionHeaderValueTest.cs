using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class RetryConditionHeaderValueTest
    {
        [TestMethod]
        public void Ctor_EntityTagOverload_MatchExpectation()
        {
            RetryConditionHeaderValue retryCondition = new RetryConditionHeaderValue(new TimeSpan(0, 0, 3));
            Assert.AreEqual(new TimeSpan(0, 0, 3), retryCondition.Delta, "Delta");
            Assert.IsNull(retryCondition.Date, "Date");

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
            { new RetryConditionHeaderValue(new TimeSpan(1234567, 0, 0)); }, "'delta' seconds must be in 0..2^31");
        }

        [TestMethod]
        public void Ctor_DateOverload_MatchExpectation()
        {
            RetryConditionHeaderValue retryCondition = new RetryConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            Assert.IsNull(retryCondition.Delta, "Delta");
            Assert.AreEqual(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero), retryCondition.Date, "Date");
        }

        [TestMethod]
        public void ToString_UseDifferentRetryConditions_AllSerializedCorrectly()
        {
            RetryConditionHeaderValue retryCondition = new RetryConditionHeaderValue(new TimeSpan(0, 0, 50000000));
            Assert.AreEqual("50000000", retryCondition.ToString());

            retryCondition = new RetryConditionHeaderValue(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            Assert.AreEqual("Thu, 15 Jul 2010 12:33:57 GMT", retryCondition.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRetryConditions_SameOrDifferentHashCodes()
        {
            RetryConditionHeaderValue retryCondition1 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 1000000));
            RetryConditionHeaderValue retryCondition2 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 1000000));
            RetryConditionHeaderValue retryCondition3 = new RetryConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition4 = new RetryConditionHeaderValue(
                new DateTimeOffset(2008, 8, 16, 13, 44, 10, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition5 = new RetryConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition6 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 2000000));

            Assert.AreEqual(retryCondition1.GetHashCode(), retryCondition2.GetHashCode(), "delta vs. delta");
            Assert.AreNotEqual(retryCondition1.GetHashCode(), retryCondition3.GetHashCode(), "delta vs. date");
            Assert.AreNotEqual(retryCondition3.GetHashCode(), retryCondition4.GetHashCode(), "date vs. different date");
            Assert.AreEqual(retryCondition3.GetHashCode(), retryCondition5.GetHashCode(), "date vs. date");
            Assert.AreNotEqual(retryCondition1.GetHashCode(), retryCondition6.GetHashCode(), "delta vs. different delta");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRetrys_EqualOrNotEqualNoExceptions()
        {
            RetryConditionHeaderValue retryCondition1 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 1000000));
            RetryConditionHeaderValue retryCondition2 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 1000000));
            RetryConditionHeaderValue retryCondition3 = new RetryConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition4 = new RetryConditionHeaderValue(
                new DateTimeOffset(2008, 8, 16, 13, 44, 10, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition5 = new RetryConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RetryConditionHeaderValue retryCondition6 = new RetryConditionHeaderValue(new TimeSpan(0, 0, 2000000));

            Assert.IsFalse(retryCondition1.Equals(null), "delta vs. <null>");
            Assert.IsTrue(retryCondition1.Equals(retryCondition2), "delta vs. delta");
            Assert.IsFalse(retryCondition1.Equals(retryCondition3), "delta vs. date");
            Assert.IsFalse(retryCondition3.Equals(retryCondition1), "date vs. delta");
            Assert.IsFalse(retryCondition3.Equals(retryCondition4), "date vs. different date");
            Assert.IsTrue(retryCondition3.Equals(retryCondition5), "date vs. date");
            Assert.IsFalse(retryCondition1.Equals(retryCondition6), "delta vs. different delta");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            RetryConditionHeaderValue source = new RetryConditionHeaderValue(new TimeSpan(0, 0, 123456789));
            RetryConditionHeaderValue clone = (RetryConditionHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.Delta, clone.Delta, "Delta");
            Assert.IsNull(clone.Date, "Date");

            source = new RetryConditionHeaderValue(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            clone = (RetryConditionHeaderValue)((ICloneable)source).Clone();

            Assert.IsNull(clone.Delta, "Delta");
            Assert.AreEqual(source.Date, clone.Date, "Date");
        }

        [TestMethod]
        public void GetRetryConditionLength_DifferentValidScenarios_AllReturnNonZero()
        {
            RetryConditionHeaderValue result = null;

            CallGetRetryConditionLength(" 1234567890 ", 1, 11, out result);
            Assert.AreEqual(new TimeSpan(0, 0, 1234567890), result.Delta, "Delta");
            Assert.IsNull(result.Date);

            CallGetRetryConditionLength("1", 0, 1, out result);
            Assert.AreEqual(new TimeSpan(0, 0, 1), result.Delta, "Delta");
            Assert.IsNull(result.Date);

            CallGetRetryConditionLength("001", 0, 3, out result);
            Assert.AreEqual(new TimeSpan(0, 0, 1), result.Delta, "Delta");
            Assert.IsNull(result.Date);

            CallGetRetryConditionLength("Wed, 09 Nov 1994 08:49:37 GMT", 0, 29, out result);
            Assert.IsNull(result.Delta);
            Assert.AreEqual(new DateTimeOffset(1994, 11, 9, 8, 49, 37, TimeSpan.Zero), result.Date, "Date");

            CallGetRetryConditionLength("Sun, 06 Nov 1994 08:49:37 GMT     ", 0, 34, out result);
            Assert.IsNull(result.Delta);
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), result.Date, "Date");
        }

        [TestMethod]
        public void GetRetryConditionLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetRetryConditionLength(" 1", 0); // no leading whitespaces allowed
            CheckInvalidGetRetryConditionLength(" Wed 09 Nov 1994 08:49:37 GMT", 0);
            CheckInvalidGetRetryConditionLength("-5", 0);

            // Even though the first char is a valid 'delta', GetRetryConditionLength() expects the whole string to be
            // a valid 'delta'.
            CheckInvalidGetRetryConditionLength("1.5", 0);
            CheckInvalidGetRetryConditionLength("5123,", 0);

            CheckInvalidGetRetryConditionLength("123456789012345678901234567890", 0); // >>Int32.MaxValue
            CheckInvalidGetRetryConditionLength("9999999999", 0); // >Int32.MaxValue but same amount of digits

            CheckInvalidGetRetryConditionLength("Wed, 09 Nov", 0);
            CheckInvalidGetRetryConditionLength("W/Wed 09 Nov 1994 08:49:37 GMT", 0);
            CheckInvalidGetRetryConditionLength("Wed 09 Nov 1994 08:49:37 GMT,", 0);

            CheckInvalidGetRetryConditionLength("", 0);
            CheckInvalidGetRetryConditionLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("  123456789 ", new RetryConditionHeaderValue(new TimeSpan(0, 0, 123456789)));
            CheckValidParse("  Sun, 06 Nov 1994 08:49:37 GMT ", 
                new RetryConditionHeaderValue(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero)));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("123 ,"); // no delimiter allowed
            CheckInvalidParse("Sun, 06 Nov 1994 08:49:37 GMT ,"); // no delimiter allowed
            CheckInvalidParse("123 Sun, 06 Nov 1994 08:49:37 GMT");
            CheckInvalidParse("Sun, 06 Nov 1994 08:49:37 GMT \"x\"");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("  123456789 ", new RetryConditionHeaderValue(new TimeSpan(0, 0, 123456789)));
            CheckValidTryParse("  Sun, 06 Nov 1994 08:49:37 GMT ",
                new RetryConditionHeaderValue(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero)));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("123 ,"); // no delimiter allowed
            CheckInvalidTryParse("Sun, 06 Nov 1994 08:49:37 GMT ,"); // no delimiter allowed
            CheckInvalidTryParse("123 Sun, 06 Nov 1994 08:49:37 GMT");
            CheckInvalidTryParse("Sun, 06 Nov 1994 08:49:37 GMT \"x\"");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
        }

        #region Helper methods

        private void CheckValidParse(string input, RetryConditionHeaderValue expectedResult)
        {
            RetryConditionHeaderValue result = RetryConditionHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => RetryConditionHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, RetryConditionHeaderValue expectedResult)
        {
            RetryConditionHeaderValue result = null;
            Assert.IsTrue(RetryConditionHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            RetryConditionHeaderValue result = null;
            Assert.IsFalse(RetryConditionHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetRetryConditionLength(string input, int startIndex, int expectedLength,
            out RetryConditionHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, RetryConditionHeaderValue.GetRetryConditionLength(input, startIndex,
                out temp), "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as RetryConditionHeaderValue;
        }

        private static void CheckInvalidGetRetryConditionLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, RetryConditionHeaderValue.GetRetryConditionLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
