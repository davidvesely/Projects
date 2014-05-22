using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class RangeConditionHeaderValueTest
    {
        [TestMethod]
        public void Ctor_EntityTagOverload_MatchExpectation()
        {
            RangeConditionHeaderValue rangeCondition = new RangeConditionHeaderValue(new EntityTagHeaderValue("\"x\""));
            Assert.AreEqual(new EntityTagHeaderValue("\"x\""), rangeCondition.EntityTag, "EntityTag");
            Assert.IsNull(rangeCondition.Date, "Date");

            EntityTagHeaderValue input = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => { new RangeConditionHeaderValue(input); }, "<null>");
        }

        [TestMethod]
        public void Ctor_EntityTagStringOverload_MatchExpectation()
        {
            RangeConditionHeaderValue rangeCondition = new RangeConditionHeaderValue("\"y\"");
            Assert.AreEqual(new EntityTagHeaderValue("\"y\""), rangeCondition.EntityTag, "EntityTag");
            Assert.IsNull(rangeCondition.Date, "Date");

            ExceptionAssert.Throws<ArgumentException>(() => { new RangeConditionHeaderValue((string)null); }, "<null>");
        }

        [TestMethod]
        public void Ctor_DateOverload_MatchExpectation()
        {
            RangeConditionHeaderValue rangeCondition = new RangeConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            Assert.IsNull(rangeCondition.EntityTag, "EntityTag");
            Assert.AreEqual(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero), rangeCondition.Date, "Date");
        }

        [TestMethod]
        public void ToString_UseDifferentrangeConditions_AllSerializedCorrectly()
        {
            RangeConditionHeaderValue rangeCondition = new RangeConditionHeaderValue(new EntityTagHeaderValue("\"x\""));
            Assert.AreEqual("\"x\"", rangeCondition.ToString());

            rangeCondition = new RangeConditionHeaderValue(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            Assert.AreEqual("Thu, 15 Jul 2010 12:33:57 GMT", rangeCondition.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentrangeConditions_SameOrDifferentHashCodes()
        {
            RangeConditionHeaderValue rangeCondition1 = new RangeConditionHeaderValue("\"x\"");
            RangeConditionHeaderValue rangeCondition2 = new RangeConditionHeaderValue(new EntityTagHeaderValue("\"x\""));
            RangeConditionHeaderValue rangeCondition3 = new RangeConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition4 = new RangeConditionHeaderValue(
                new DateTimeOffset(2008, 8, 16, 13, 44, 10, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition5 = new RangeConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition6 = new RangeConditionHeaderValue(
                new EntityTagHeaderValue("\"x\"", true));

            Assert.AreEqual(rangeCondition1.GetHashCode(), rangeCondition2.GetHashCode(), "\"x\" vs. \"x\"");
            Assert.AreNotEqual(rangeCondition1.GetHashCode(), rangeCondition3.GetHashCode(), "\"x\" vs. date");
            Assert.AreNotEqual(rangeCondition3.GetHashCode(), rangeCondition4.GetHashCode(), "date vs. different date");
            Assert.AreEqual(rangeCondition3.GetHashCode(), rangeCondition5.GetHashCode(), "date vs. date");
            Assert.AreNotEqual(rangeCondition1.GetHashCode(), rangeCondition6.GetHashCode(), "\"x\" vs. W/\"x\"");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            RangeConditionHeaderValue rangeCondition1 = new RangeConditionHeaderValue("\"x\"");
            RangeConditionHeaderValue rangeCondition2 = new RangeConditionHeaderValue(new EntityTagHeaderValue("\"x\""));
            RangeConditionHeaderValue rangeCondition3 = new RangeConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition4 = new RangeConditionHeaderValue(
                new DateTimeOffset(2008, 8, 16, 13, 44, 10, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition5 = new RangeConditionHeaderValue(
                new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            RangeConditionHeaderValue rangeCondition6 = new RangeConditionHeaderValue(
                new EntityTagHeaderValue("\"x\"", true));

            Assert.IsFalse(rangeCondition1.Equals(null), "\"x\" vs. <null>");
            Assert.IsTrue(rangeCondition1.Equals(rangeCondition2), "\"x\" vs. \"x\"");
            Assert.IsFalse(rangeCondition1.Equals(rangeCondition3), "\"x\" vs. date");
            Assert.IsFalse(rangeCondition3.Equals(rangeCondition1), "date vs. \"x\"");
            Assert.IsFalse(rangeCondition3.Equals(rangeCondition4), "date vs. different date");
            Assert.IsTrue(rangeCondition3.Equals(rangeCondition5), "date vs. date");
            Assert.IsFalse(rangeCondition1.Equals(rangeCondition6), "\"x\" vs. W/\"x\"");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            RangeConditionHeaderValue source = new RangeConditionHeaderValue(new EntityTagHeaderValue("\"x\""));
            RangeConditionHeaderValue clone = (RangeConditionHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.EntityTag, clone.EntityTag, "EntityTag");
            Assert.IsNull(clone.Date, "Date");

            source = new RangeConditionHeaderValue(new DateTimeOffset(2010, 7, 15, 12, 33, 57, TimeSpan.Zero));
            clone = (RangeConditionHeaderValue)((ICloneable)source).Clone();

            Assert.IsNull(clone.EntityTag, "EntityTag");
            Assert.AreEqual(source.Date, clone.Date, "Date");
        }

        [TestMethod]
        public void GetRangeConditionLength_DifferentValidScenarios_AllReturnNonZero()
        {
            RangeConditionHeaderValue result = null;

            CallGetRangeConditionLength(" W/ \"tag\" ", 1, 9, out result);
            Assert.AreEqual(new EntityTagHeaderValue("\"tag\"", true), result.EntityTag, "EntityTag");
            Assert.IsNull(result.Date);

            CallGetRangeConditionLength(" w/\"tag\"", 1, 7, out result);
            Assert.AreEqual(new EntityTagHeaderValue("\"tag\"", true), result.EntityTag, "EntityTag");
            Assert.IsNull(result.Date);

            CallGetRangeConditionLength("\"tag\"", 0, 5, out result);
            Assert.AreEqual(new EntityTagHeaderValue("\"tag\""), result.EntityTag, "EntityTag");
            Assert.IsNull(result.Date);

            CallGetRangeConditionLength("Wed, 09 Nov 1994 08:49:37 GMT", 0, 29, out result);
            Assert.IsNull(result.EntityTag);
            Assert.AreEqual(new DateTimeOffset(1994, 11, 9, 8, 49, 37, TimeSpan.Zero), result.Date, "Date");

            CallGetRangeConditionLength("Sun, 06 Nov 1994 08:49:37 GMT", 0, 29, out result);
            Assert.IsNull(result.EntityTag);
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), result.Date, "Date");
        }

        [TestMethod]
        public void GetRangeConditionLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetRangeConditionLength(" \"x\"", 0); // no leading whitespaces allowed
            CheckInvalidGetRangeConditionLength(" Wed 09 Nov 1994 08:49:37 GMT", 0);
            CheckInvalidGetRangeConditionLength("\"x", 0);
            CheckInvalidGetRangeConditionLength("Wed, 09 Nov", 0);
            CheckInvalidGetRangeConditionLength("W/Wed 09 Nov 1994 08:49:37 GMT", 0);
            CheckInvalidGetRangeConditionLength("\"x\",", 0);
            CheckInvalidGetRangeConditionLength("Wed 09 Nov 1994 08:49:37 GMT,", 0);

            CheckInvalidGetRangeConditionLength("", 0);
            CheckInvalidGetRangeConditionLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("  \"x\" ", new RangeConditionHeaderValue("\"x\""));
            CheckValidParse("  Sun, 06 Nov 1994 08:49:37 GMT ",
                new RangeConditionHeaderValue(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero)));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("\"x\" ,"); // no delimiter allowed
            CheckInvalidParse("Sun, 06 Nov 1994 08:49:37 GMT ,"); // no delimiter allowed
            CheckInvalidParse("\"x\" Sun, 06 Nov 1994 08:49:37 GMT");
            CheckInvalidParse("Sun, 06 Nov 1994 08:49:37 GMT \"x\"");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("  \"x\" ", new RangeConditionHeaderValue("\"x\""));
            CheckValidTryParse("  Sun, 06 Nov 1994 08:49:37 GMT ",
                new RangeConditionHeaderValue(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero)));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("\"x\" ,"); // no delimiter allowed
            CheckInvalidTryParse("Sun, 06 Nov 1994 08:49:37 GMT ,"); // no delimiter allowed
            CheckInvalidTryParse("\"x\" Sun, 06 Nov 1994 08:49:37 GMT");
            CheckInvalidTryParse("Sun, 06 Nov 1994 08:49:37 GMT \"x\"");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
        }

        #region Helper methods

        private void CheckValidParse(string input, RangeConditionHeaderValue expectedResult)
        {
            RangeConditionHeaderValue result = RangeConditionHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => RangeConditionHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, RangeConditionHeaderValue expectedResult)
        {
            RangeConditionHeaderValue result = null;
            Assert.IsTrue(RangeConditionHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            RangeConditionHeaderValue result = null;
            Assert.IsFalse(RangeConditionHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetRangeConditionLength(string input, int startIndex, int expectedLength,
            out RangeConditionHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, RangeConditionHeaderValue.GetRangeConditionLength(input, startIndex, 
                out temp), "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as RangeConditionHeaderValue;
        }

        private static void CheckInvalidGetRangeConditionLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, RangeConditionHeaderValue.GetRangeConditionLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
