using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class RangeItemHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_BothValuesNull_Throw()
        {
            new RangeItemHeaderValue(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_FromValueNegative_Throw()
        {
            new RangeItemHeaderValue(-1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_FromGreaterThanToValue_Throw()
        {
            new RangeItemHeaderValue(2, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_ToValueNegative_Throw()
        {
            new RangeItemHeaderValue(null, -1);
        }

        [TestMethod]
        public void Ctor_ValidFormat_SuccessfullyCreated()
        {
            RangeItemHeaderValue rangeItem = new RangeItemHeaderValue(1, 2);
            Assert.AreEqual(1, rangeItem.From, "From");
            Assert.AreEqual(2, rangeItem.To, "To");
        }

        [TestMethod]
        public void ToString_UseDifferentRangeItems_AllSerializedCorrectly()
        {
            // Make sure ToString() doesn't add any separators.
            RangeItemHeaderValue rangeItem = new RangeItemHeaderValue(1000000000, 2000000000);
            Assert.AreEqual("1000000000-2000000000", rangeItem.ToString());

            rangeItem = new RangeItemHeaderValue(5, null);
            Assert.AreEqual("5-", rangeItem.ToString());

            rangeItem = new RangeItemHeaderValue(null, 10);
            Assert.AreEqual("-10", rangeItem.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRangeItems_SameOrDifferentHashCodes()
        {
            RangeItemHeaderValue rangeItem1 = new RangeItemHeaderValue(1, 2);
            RangeItemHeaderValue rangeItem2 = new RangeItemHeaderValue(1, null);
            RangeItemHeaderValue rangeItem3 = new RangeItemHeaderValue(null, 2);
            RangeItemHeaderValue rangeItem4 = new RangeItemHeaderValue(2, 2);
            RangeItemHeaderValue rangeItem5 = new RangeItemHeaderValue(1, 2);

            Assert.AreNotEqual(rangeItem1.GetHashCode(), rangeItem2.GetHashCode(), "1-2 vs. 1-.");
            Assert.AreNotEqual(rangeItem1.GetHashCode(), rangeItem3.GetHashCode(), "1-2 vs. -2.");
            Assert.AreNotEqual(rangeItem1.GetHashCode(), rangeItem4.GetHashCode(), "1-2 vs. 2-2.");
            Assert.AreEqual(rangeItem1.GetHashCode(), rangeItem5.GetHashCode(), "1-2 vs. 1-2.");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            RangeItemHeaderValue rangeItem1 = new RangeItemHeaderValue(1, 2);
            RangeItemHeaderValue rangeItem2 = new RangeItemHeaderValue(1, null);
            RangeItemHeaderValue rangeItem3 = new RangeItemHeaderValue(null, 2);
            RangeItemHeaderValue rangeItem4 = new RangeItemHeaderValue(2, 2);
            RangeItemHeaderValue rangeItem5 = new RangeItemHeaderValue(1, 2);

            Assert.IsFalse(rangeItem1.Equals(rangeItem2), "1-2 vs. 1-.");
            Assert.IsFalse(rangeItem2.Equals(rangeItem1), "1- vs. 1-2.");
            Assert.IsFalse(rangeItem1.Equals(null), "1-2 vs. null.");
            Assert.IsFalse(rangeItem1.Equals(rangeItem3), "1-2 vs. -2.");
            Assert.IsFalse(rangeItem3.Equals(rangeItem1), "-2 vs. 1-2.");
            Assert.IsFalse(rangeItem1.Equals(rangeItem4), "1-2 vs. 2-2.");
            Assert.IsTrue(rangeItem1.Equals(rangeItem5), "1-2 vs. 1-2.");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            RangeItemHeaderValue source = new RangeItemHeaderValue(1, 2);
            RangeItemHeaderValue clone = (RangeItemHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.From, clone.From, "From");
            Assert.AreEqual(source.To, clone.To, "To");

            source = new RangeItemHeaderValue(1, null);
            clone = (RangeItemHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.From, clone.From, "From");
            Assert.AreEqual(source.To, clone.To, "To");

            source = new RangeItemHeaderValue(null, 2);
            clone = (RangeItemHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.From, clone.From, "From");
            Assert.AreEqual(source.To, clone.To, "To");
        }

        [TestMethod]
        public void GetRangeItemLength_DifferentValidScenarios_AllReturnNonZero()
        {
            CheckValidGetRangeItemLength("1-2", 0, 3, 1, 2);
            CheckValidGetRangeItemLength("0-0", 0, 3, 0, 0);
            CheckValidGetRangeItemLength(" 1-", 1, 2, 1, null);
            CheckValidGetRangeItemLength(" -2", 1, 2, null, 2);

            // Note that the parser will only parse '1-' as a valid range and ignore '-2'. It is the callers 
            // responsibility to determine if this is indeed a valid range
            CheckValidGetRangeItemLength(" 1--2", 1, 2, 1, null);

            CheckValidGetRangeItemLength(" 684684 - 123456789012345 !", 1, 25, 684684, 123456789012345);

            // The separator doesn't matter. It only parses until the first non-whitespace
            CheckValidGetRangeItemLength(" 1 - 2 ,", 1, 6, 1, 2);
        }

        [TestMethod]
        public void GetRangeItemLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetRangeItemLength(" 1-2", 0); // no leading spaces.
            CheckInvalidGetRangeItemLength("2-1", 0);
            CheckInvalidGetRangeItemLength("-", 0);
            CheckInvalidGetRangeItemLength("1", 0);
            CheckInvalidGetRangeItemLength(null, 0);
            CheckInvalidGetRangeItemLength(string.Empty, 0);
            CheckInvalidGetRangeItemLength("12345678901234567890123-", 0); // >>Int64.MaxValue
            CheckInvalidGetRangeItemLength("-12345678901234567890123", 0); // >>Int64.MaxValue
            CheckInvalidGetRangeItemLength("9999999999999999999-", 0); // 19-digit numbers outside the Int64 range.
            CheckInvalidGetRangeItemLength("-9999999999999999999", 0); // 19-digit numbers outside the Int64 range.
        }

        [TestMethod]
        public void GetRangeItemListLength_DifferentValidScenarios_AllReturnNonZero()
        {
            CheckValidGetRangeItemListLength("x,,1-2, 3 -  , , -6 , ,,", 1, 23,
                new Tuple<long?, long?>(1, 2), new Tuple<long?, long?>(3, null), new Tuple<long?, long?>(null, 6));
            CheckValidGetRangeItemListLength("1-2,", 0, 4, new Tuple<long?, long?>(1, 2));
            CheckValidGetRangeItemListLength("1-", 0, 2, new Tuple<long?, long?>(1, null));
        }

        [TestMethod]
        public void GetRangeItemListLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetRangeItemListLength(null, 0);
            CheckInvalidGetRangeItemListLength(string.Empty, 0);
            CheckInvalidGetRangeItemListLength("1-2", 3);
            CheckInvalidGetRangeItemListLength(",,", 0);
            CheckInvalidGetRangeItemListLength("1", 0);
            CheckInvalidGetRangeItemListLength("1-2,3", 0);
            CheckInvalidGetRangeItemListLength("1--2", 0);
            CheckInvalidGetRangeItemListLength("1,-2", 0);
            CheckInvalidGetRangeItemListLength("-", 0);
            CheckInvalidGetRangeItemListLength("--", 0);
        }

        #region Helper methods

        private static void AssertFormatException(string tag)
        {
            ExceptionAssert.ThrowsFormat(() => { new EntityTagHeaderValue(tag); },
                "name: '" + (tag == null ? "<null>" : tag) + "'");
        }

        private static void CheckValidGetRangeItemLength(string input, int startIndex, int expectedLength, 
            long? expectedFrom, long? expectedTo)
        { 
            RangeItemHeaderValue result = null;
            Assert.AreEqual(expectedLength, RangeItemHeaderValue.GetRangeItemLength(input, startIndex, out result),
                "Input: '{0}', Index: {1}", input, startIndex);
            Assert.AreEqual(expectedFrom, result.From, "From - Input: '{0}', Index: {1}", input, startIndex);
            Assert.AreEqual(expectedTo, result.To, "To - Input: '{0}', Index: {1}", input, startIndex);
        }

        private static void CheckInvalidGetRangeItemLength(string input, int startIndex)
        {
            RangeItemHeaderValue result = null;
            Assert.AreEqual(0, RangeItemHeaderValue.GetRangeItemLength(input, startIndex, out result),
                "Input: '{0}', Index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        private static void CheckValidGetRangeItemListLength(string input, int startIndex, int expectedLength,
            params Tuple<long?, long?>[] expectedRanges)
        {
            List<RangeItemHeaderValue> ranges = new List<RangeItemHeaderValue>();
            Assert.AreEqual(expectedLength, RangeItemHeaderValue.GetRangeItemListLength(input, startIndex, ranges),
                "Input: '{0}', Index: {1}", input, startIndex);

            Assert.AreEqual<int>(expectedRanges.Length, ranges.Count, "Count - Input: '{0}', Index: {1}", 
                input, startIndex);

            for (int i = 0; i < expectedRanges.Length; i++)
            {
                Assert.AreEqual(expectedRanges[i].Item1, ranges[i].From, "From - Input: '{0}', Index: {1}, i: {2}",
                    input, startIndex, i);
                Assert.AreEqual(expectedRanges[i].Item2, ranges[i].To, "To - Input: '{0}', Index: {1}, i: {2}",
                    input, startIndex, i);
            }
        }

        private static void CheckInvalidGetRangeItemListLength(string input, int startIndex)
        {
            List<RangeItemHeaderValue> ranges = new List<RangeItemHeaderValue>();
            Assert.AreEqual(0, RangeItemHeaderValue.GetRangeItemListLength(input, startIndex, ranges),
                "Input: '{0}', Index: {1}", input, startIndex);
        }

        #endregion
    }
}
