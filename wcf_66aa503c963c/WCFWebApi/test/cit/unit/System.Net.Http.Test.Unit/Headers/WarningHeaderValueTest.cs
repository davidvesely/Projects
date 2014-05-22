using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class WarningHeaderValueTest
    {
        [TestMethod]
        public void Ctor_3ParamsOverload_AllFieldsInitializedCorrectly()
        {
            WarningHeaderValue warning = new WarningHeaderValue(111, ".host", "\"Revalidation failed\"");
            Assert.AreEqual(111, warning.Code, "Code");
            Assert.AreEqual(".host", warning.Agent, "Agent");
            Assert.AreEqual("\"Revalidation failed\"", warning.Text, "Text");
            Assert.IsNull(warning.Date, "Date");

            warning = new WarningHeaderValue(112, "[::1]", "\"Disconnected operation\"");
            Assert.AreEqual(112, warning.Code, "Code");
            Assert.AreEqual("[::1]", warning.Agent, "Agent");
            Assert.AreEqual("\"Disconnected operation\"", warning.Text, "Text");
            Assert.IsNull(warning.Date, "Date");

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new WarningHeaderValue(-1, "host", "\"\""), 
                "Code: -1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new WarningHeaderValue(1000, "host", "\"\""), 
                "Code: 1000");
            
            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, null, "\"\""), "Agent: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, "", "\"\""), "Agent: \"\"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "x y", "\"\""), "Agent: \"x y\"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "x ", "\"\""), "Agent: \"x \"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, " x", "\"\""), "Agent: \" x\"");

            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, null, "\"\""), "Text: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, "", "\"\""), "Text: \"\"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "h", "x"), "Text: x");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "h", "\"x"), "Text: \"x");
        }

        [TestMethod]
        public void Ctor_4ParamsOverload_AllFieldsInitializedCorrectly()
        {
            WarningHeaderValue warning = new WarningHeaderValue(111, "host", "\"Revalidation failed\"", 
                new DateTimeOffset(2010, 7, 19, 17, 9, 15, TimeSpan.Zero));
            Assert.AreEqual(111, warning.Code, "Code");
            Assert.AreEqual("host", warning.Agent, "Agent");
            Assert.AreEqual("\"Revalidation failed\"", warning.Text, "Text");
            Assert.AreEqual(new DateTimeOffset(2010, 7, 19, 17, 9, 15, TimeSpan.Zero), warning.Date, "Date");

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new WarningHeaderValue(-1, "host", "\"\""),
                "Code: -1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new WarningHeaderValue(1000, "host", "\"\""),
                "Code: 1000");

            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, null, "\"\""), "Agent: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, "", "\"\""), "Agent: \"\"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "[::1]:80(x)", "\"\""), "Agent: [::1]:80(x)");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "host::80", "\"\""), "Agent: host::80");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "192.168.0.1=", "\"\""), "Agent: 192.168.0.1=");

            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, null, "\"\""), "Text: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new WarningHeaderValue(100, "", "\"\""), "Text: \"\"");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "h", "(x)"), "Text: (x)");
            ExceptionAssert.ThrowsFormat(() => new WarningHeaderValue(100, "h", "\"x\"y"), "Text: \"x\"y");
        }

        [TestMethod]
        public void ToString_UseDifferentRanges_AllSerializedCorrectly()
        {
            WarningHeaderValue warning = new WarningHeaderValue(113, "host:80", "\"Heuristic expiration\"");
            Assert.AreEqual("113 host:80 \"Heuristic expiration\"", warning.ToString());

            warning = new WarningHeaderValue(199, "[::1]", "\"Miscellaneous warning\"", 
                new DateTimeOffset(2010, 7, 19, 18, 31, 27, TimeSpan.Zero));
            Assert.AreEqual("199 [::1] \"Miscellaneous warning\" \"Mon, 19 Jul 2010 18:31:27 GMT\"", warning.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRanges_SameOrDifferentHashCodes()
        {
            WarningHeaderValue warning1 = new WarningHeaderValue(214, "host", "\"Transformation applied\"");
            WarningHeaderValue warning2 = new WarningHeaderValue(214, "HOST", "\"Transformation applied\"");
            WarningHeaderValue warning3 = new WarningHeaderValue(215, "host", "\"Transformation applied\"");
            WarningHeaderValue warning4 = new WarningHeaderValue(214, "other", "\"Transformation applied\"");
            WarningHeaderValue warning5 = new WarningHeaderValue(214, "host", "\"TRANSFORMATION APPLIED\"");
            WarningHeaderValue warning6 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2010, 7, 19, 18, 31, 27, TimeSpan.Zero));
            WarningHeaderValue warning7 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2011, 7, 19, 18, 31, 27, TimeSpan.Zero));
            WarningHeaderValue warning8 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2010, 7, 19, 18, 31, 27, TimeSpan.Zero));

            Assert.AreEqual(warning1.GetHashCode(), warning2.GetHashCode(), "214 host \"t.a.\" vs. 214 HOST \"t.a.\"");
            Assert.AreNotEqual(warning1.GetHashCode(), warning3.GetHashCode(), "214 host \"t.a.\" vs. 215 host \"t.a.\"");
            Assert.AreNotEqual(warning1.GetHashCode(), warning4.GetHashCode(), "214 host \"t.a.\" vs. 214 other \"t.a.\"");
            Assert.AreNotEqual(warning1.GetHashCode(), warning6.GetHashCode(), "214 host \"t.a.\" vs. 214 host \"T.A.\"");
            Assert.AreNotEqual(warning1.GetHashCode(), warning7.GetHashCode(), 
                "214 host \"t.a.\" vs. 214 host \"t.a.\" \"D\"");
            Assert.AreNotEqual(warning6.GetHashCode(), warning7.GetHashCode(), 
                "214 host \"t.a.\" \"D\" vs. 214 host \"t.a.\" \"other D\"");
            Assert.AreEqual(warning6.GetHashCode(), warning8.GetHashCode(),
                "214 host \"t.a.\" \"D\" vs. 214 host \"t.a.\" \"D\"");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            WarningHeaderValue warning1 = new WarningHeaderValue(214, "host", "\"Transformation applied\"");
            WarningHeaderValue warning2 = new WarningHeaderValue(214, "HOST", "\"Transformation applied\"");
            WarningHeaderValue warning3 = new WarningHeaderValue(215, "host", "\"Transformation applied\"");
            WarningHeaderValue warning4 = new WarningHeaderValue(214, "other", "\"Transformation applied\"");
            WarningHeaderValue warning5 = new WarningHeaderValue(214, "host", "\"TRANSFORMATION APPLIED\"");
            WarningHeaderValue warning6 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2010, 7, 19, 18, 31, 27, TimeSpan.Zero));
            WarningHeaderValue warning7 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2011, 7, 19, 18, 31, 27, TimeSpan.Zero));
            WarningHeaderValue warning8 = new WarningHeaderValue(214, "host", "\"Transformation applied\"",
                new DateTimeOffset(2010, 7, 19, 18, 31, 27, TimeSpan.Zero));

            Assert.IsFalse(warning1.Equals(null), "214 host \"t.a.\" vs. <null>");
            Assert.IsTrue(warning1.Equals(warning2), "214 host \"t.a.\" vs. 214 HOST \"t.a.\"");
            Assert.IsFalse(warning1.Equals(warning3), "214 host \"t.a.\" vs. 215 host \"t.a.\"");
            Assert.IsFalse(warning1.Equals(warning4), "214 host \"t.a.\" vs. 214 other \"t.a.\"");
            Assert.IsFalse(warning1.Equals(warning6), "214 host \"t.a.\" vs. 214 host \"T.A.\"");
            Assert.IsFalse(warning1.Equals(warning7), "214 host \"t.a.\" vs. 214 host \"t.a.\" \"D\"");
            Assert.IsFalse(warning7.Equals(warning1), "214 host \"t.a.\" \"D\" vs. 214 host \"t.a.\"");
            Assert.IsFalse(warning6.Equals(warning7), "214 host \"t.a.\" \"D\" vs. 214 host \"t.a.\" \"other D\"");
            Assert.IsTrue(warning6.Equals(warning8), "214 host \"t.a.\" \"D\" vs. 214 host \"t.a.\" \"D\"");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            WarningHeaderValue source = new WarningHeaderValue(299, "host", "\"Miscellaneous persistent warning\"");
            WarningHeaderValue clone = (WarningHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Code, clone.Code, "Code");
            Assert.AreEqual(source.Agent, clone.Agent, "Agent");
            Assert.AreEqual(source.Text, clone.Text, "Text");
            Assert.AreEqual(source.Date, clone.Date, "Date");

            source = new WarningHeaderValue(110, "host", "\"Response is stale\"", 
                new DateTimeOffset(2010, 7, 31, 15, 37, 05, TimeSpan.Zero));
            clone = (WarningHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Code, clone.Code, "Code");
            Assert.AreEqual(source.Agent, clone.Agent, "Agent");
            Assert.AreEqual(source.Text, clone.Text, "Text");
            Assert.AreEqual(source.Date, clone.Date, "Date");
        }

        [TestMethod]
        public void GetWarningLength_DifferentValidScenarios_AllReturnNonZero()
        {
            CheckGetWarningLength(" 199 .host \r\n \"Miscellaneous warning\"  ", 1, 38,
                new WarningHeaderValue(199, ".host", "\"Miscellaneous warning\""));
            CheckGetWarningLength("987 [FE18:AB64::156]:80 \"\" \"Tue, 20 Jul 2010 01:02:03 GMT\"", 0, 58,
                new WarningHeaderValue(987, "[FE18:AB64::156]:80", "\"\"", 
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));

            // The parser reads until it reaches an invalid/unexpected character. If until then it was able to create
            // a valid WarningHeaderValue, it will return the length of the parsed string. Therefore a string like 
            // the following is considered valid (until '[')
            CheckGetWarningLength("1 h \"t\"[", 0, 7, new WarningHeaderValue(1, "h", "\"t\""));
            CheckGetWarningLength("1 h \"t\" \"Tue, 20 Jul 2010 01:02:03 GMT\"[", 0, 39, 
                new WarningHeaderValue(1, "h", "\"t\"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));

            CheckGetWarningLength(null, 0, 0, null);
            CheckGetWarningLength(string.Empty, 0, 0, null);
            CheckGetWarningLength("  ", 0, 0, null);
        }

        [TestMethod]
        public void GetWarningLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidWarningViaLength(" 123 host", 0); // no leading whitespaces allowed

            // No delimiter between two values
            CheckInvalidWarningViaLength("123host \"t\"", 0);
            CheckInvalidWarningViaLength("123 host\"t\"", 0);
            CheckInvalidWarningViaLength("123 host \"t\"\"Tue, 20 Jul 2010 01:02:03 GMT\"", 0);
            CheckInvalidWarningViaLength("123 http://host \"t\"", 0);

            CheckInvalidWarningViaLength("1=host \"t\"", 0);
            CheckInvalidWarningViaLength("1.1 host \"invalid_quoted_string", 0);
            CheckInvalidWarningViaLength("=", 0);
            CheckInvalidWarningViaLength("121 host=\"t\"", 0);
            CheckInvalidWarningViaLength("121 host= \"t\"", 0);
            CheckInvalidWarningViaLength("121 host =\"t\"", 0);
            CheckInvalidWarningViaLength("121 host = \"t\"", 0);
            CheckInvalidWarningViaLength("121 host =", 0);
            CheckInvalidWarningViaLength("121 = \"t\"", 0);
            CheckInvalidWarningViaLength("123", 0);
            CheckInvalidWarningViaLength("123 host", 0);
            CheckInvalidWarningViaLength("  ", 0);
            CheckInvalidWarningViaLength("123 example.com[ \"t\"", 0);
            CheckInvalidWarningViaLength("123 / \"t\"", 0);
            CheckInvalidWarningViaLength("123 host::80 \"t\"", 0);
            CheckInvalidWarningViaLength("123 host/\"t\"", 0);
            CheckInvalidWarningViaLength("123 host \"t\" \"Tue, 20 Jul 2010 01:02:03 GMT", 0);
            CheckInvalidWarningViaLength("123 host \"t\" \"Tue, 200 Jul 2010 01:02:03 GMT\"", 0);
            CheckInvalidWarningViaLength("123 host \"t\" \"\"", 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse(" 123   host \"text\"", new WarningHeaderValue(123, "host", "\"text\""));
            CheckValidParse(" 50  192.168.0.1  \"text  \"  \"Tue, 20 Jul 2010 01:02:03 GMT\" ",
                new WarningHeaderValue(50, "192.168.0.1", "\"text  \"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));
            CheckValidParse(" 123 h \"t\"", new WarningHeaderValue(123, "h", "\"t\""));
            CheckValidParse("1 h \"t\"", new WarningHeaderValue(1, "h", "\"t\""));
            CheckValidParse("1 h \"t\" \"Tue, 20 Jul 2010 01:02:03 GMT\"", 
                new WarningHeaderValue(1, "h", "\"t\"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));
            CheckValidParse("1 会 \"t\" ", new WarningHeaderValue(1, "会", "\"t\""));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("1.1 host \"text\"");
            CheckInvalidParse("11 host text");
            CheckInvalidParse("11 host \"text\" Tue, 20 Jul 2010 01:02:03 GMT");
            CheckInvalidParse("11 host \"text\" 123 next \"text\"");
            CheckInvalidParse("会");
            CheckInvalidParse("123 会");
            CheckInvalidParse("111 [::1]:80\r(comment) \"text\"");
            CheckInvalidParse("111 [::1]:80\n(comment) \"text\"");

            CheckInvalidParse("X , , 123   host \"text\", ,next");
            CheckInvalidParse("X 50  192.168.0.1  \"text  \"  \"Tue, 20 Jul 2010 01:02:03 GMT\" , ,next");
            CheckInvalidParse(" ,123 h \"t\",");
            CheckInvalidParse("1 会 \"t\" ,,");

            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse(" 123   host \"text\"", new WarningHeaderValue(123, "host", "\"text\""));
            CheckValidTryParse(" 50  192.168.0.1  \"text  \"  \"Tue, 20 Jul 2010 01:02:03 GMT\" ",
                new WarningHeaderValue(50, "192.168.0.1", "\"text  \"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));
            CheckValidTryParse(" 123 h \"t\"", new WarningHeaderValue(123, "h", "\"t\""));
            CheckValidTryParse("1 h \"t\"", new WarningHeaderValue(1, "h", "\"t\""));
            CheckValidTryParse("1 h \"t\" \"Tue, 20 Jul 2010 01:02:03 GMT\"",
                new WarningHeaderValue(1, "h", "\"t\"",
                    new DateTimeOffset(2010, 7, 20, 1, 2, 3, TimeSpan.Zero)));
            CheckValidTryParse("1 会 \"t\" ", new WarningHeaderValue(1, "会", "\"t\""));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("1.1 host \"text\"");
            CheckInvalidTryParse("11 host text");
            CheckInvalidTryParse("11 host \"text\" Tue, 20 Jul 2010 01:02:03 GMT");
            CheckInvalidTryParse("11 host \"text\" 123 next \"text\"");
            CheckInvalidTryParse("会");
            CheckInvalidTryParse("123 会");
            CheckInvalidTryParse("111 [::1]:80\r(comment) \"text\"");
            CheckInvalidTryParse("111 [::1]:80\n(comment) \"text\"");

            CheckInvalidTryParse("X , , 123   host \"text\", ,next");
            CheckInvalidTryParse("X 50  192.168.0.1  \"text  \"  \"Tue, 20 Jul 2010 01:02:03 GMT\" , ,next");
            CheckInvalidTryParse(" ,123 h \"t\",");
            CheckInvalidTryParse("1 会 \"t\" ,,");

            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
        }

        #region Helper methods

        private void CheckValidParse(string input, WarningHeaderValue expectedResult)
        {
            WarningHeaderValue result = WarningHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => WarningHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, WarningHeaderValue expectedResult)
        {
            WarningHeaderValue result = null;
            Assert.IsTrue(WarningHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            WarningHeaderValue result = null;
            Assert.IsFalse(WarningHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CheckGetWarningLength(string input, int startIndex, int expectedLength,
            WarningHeaderValue expectedResult)
        {
            object result = null;
            Assert.AreEqual(expectedLength, WarningHeaderValue.GetWarningLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.AreEqual(expectedResult, result, "Input: '{0}', Start index: {1}", input, startIndex);
        }

        private static void CheckInvalidWarningViaLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, WarningHeaderValue.GetWarningLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
