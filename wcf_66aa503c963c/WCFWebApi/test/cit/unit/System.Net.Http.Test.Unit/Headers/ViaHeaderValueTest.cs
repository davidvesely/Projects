using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class ViaHeaderValueTest
    {
        [TestMethod]
        public void Ctor_ProtocolVersionAndReceivedByOnlyOverload_CallForwardedToOtherCtor()
        {
            ViaHeaderValue via = new ViaHeaderValue("1.1", ".token");
            Assert.AreEqual("1.1", via.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual(".token", via.ReceivedBy, "ReceivedBy");
            Assert.IsNull(via.ProtocolName, "ProtocolName");
            Assert.IsNull(via.Comment, "Comment");

            via = new ViaHeaderValue("x11", "[::1]:1818");
            Assert.AreEqual("x11", via.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual("[::1]:1818", via.ReceivedBy, "ReceivedBy");
            
            ExceptionAssert.Throws<ArgumentException>(() => new ViaHeaderValue(null, "host"), "ProtocolVersion: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new ViaHeaderValue("", "host"), "ProtocolVersion: \"\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("x y", "h"), "ProtocolVersion: \"x y\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("x ", "h"), "ProtocolVersion: \"x \"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue(" x", "h"), "ProtocolVersion: \" x\"");
            ExceptionAssert.Throws<ArgumentException>(() => new ViaHeaderValue("1.1", null), "ReceivedBy: <null>");
            ExceptionAssert.Throws<ArgumentException>(() => new ViaHeaderValue("1.1", ""), "ReceivedBy: \"\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "x y"), "ReceivedBy: \"x y\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "x "), "ReceivedBy: \"x \"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", " x"), "ReceivedBy: \" x\"");
        }

        [TestMethod]
        public void Ctor_ProtocolVersionReceivedByAndProtocolNameOnlyOverload_CallForwardedToOtherCtor()
        {
            ViaHeaderValue via = new ViaHeaderValue("1.1", "host", "HTTP");
            Assert.AreEqual("1.1", via.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual("host", via.ReceivedBy, "ReceivedBy");
            Assert.AreEqual("HTTP", via.ProtocolName, "ProtocolName");
            Assert.IsNull(via.Comment, "Comment");

            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "h", "x y"), "ProtocolName: \"x y\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "h", "x "), "ProtocolName: \"x \"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "h", " x"), "ProtocolName: \" x\"");
        }

        [TestMethod]
        public void Ctor_AllParams_AllFieldsInitializedCorrectly()
        {
            ViaHeaderValue via = new ViaHeaderValue("1.1", "host", "HTTP", "(comment)");
            Assert.AreEqual("1.1", via.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual("host", via.ReceivedBy, "ReceivedBy");
            Assert.AreEqual("HTTP", via.ProtocolName, "ProtocolName");
            Assert.AreEqual("(comment)", via.Comment, "Comment");

            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "h", "p", "(x"), "Comment: \"(x\"");
            ExceptionAssert.ThrowsFormat(() => new ViaHeaderValue("v", "h", "p", "x)"), "Comment: \"x)\"");
        }

        [TestMethod]
        public void ToString_UseDifferentRanges_AllSerializedCorrectly()
        {
            ViaHeaderValue via = new ViaHeaderValue("1.1", "host:80");
            Assert.AreEqual("1.1 host:80", via.ToString());

            via = new ViaHeaderValue("1.1", "[::1]", "HTTP");
            Assert.AreEqual("HTTP/1.1 [::1]", via.ToString());

            via = new ViaHeaderValue("1.0", "www.example.com", "WS", "(comment)");
            Assert.AreEqual("WS/1.0 www.example.com (comment)", via.ToString());

            via = new ViaHeaderValue("1.0", "www.example.com:80", null, "(comment)");
            Assert.AreEqual("1.0 www.example.com:80 (comment)", via.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRanges_SameOrDifferentHashCodes()
        {
            ViaHeaderValue via1 = new ViaHeaderValue("x11", "host");
            ViaHeaderValue via2 = new ViaHeaderValue("x11", "HOST");
            ViaHeaderValue via3 = new ViaHeaderValue("X11", "host");
            ViaHeaderValue via4 = new ViaHeaderValue("x11", "host", "HTTP");
            ViaHeaderValue via5 = new ViaHeaderValue("x11", "host", "http");
            ViaHeaderValue via6 = new ViaHeaderValue("x11", "host", null, "(comment)");
            ViaHeaderValue via7 = new ViaHeaderValue("x11", "host", "HTTP", "(comment)");
            ViaHeaderValue via8 = new ViaHeaderValue("x11", "host", "HTTP", "(COMMENT)");
            ViaHeaderValue via9 = new ViaHeaderValue("x12", "host");
            ViaHeaderValue via10 = new ViaHeaderValue("x11", "host2");
            ViaHeaderValue via11 = new ViaHeaderValue("x11", "host", "WS");
            ViaHeaderValue via12 = new ViaHeaderValue("x11", "host", string.Empty, string.Empty);

            Assert.AreEqual(via1.GetHashCode(), via2.GetHashCode(), "x11 host vs. x11 HOST");
            Assert.AreEqual(via1.GetHashCode(), via3.GetHashCode(), "x11 host vs. X11 host");
            Assert.AreNotEqual(via1.GetHashCode(), via4.GetHashCode(), "x11 host vs. HTTP/x11 host");
            Assert.AreNotEqual(via1.GetHashCode(), via6.GetHashCode(), "x11 host vs. HTTP/x11 (comment)");
            Assert.AreNotEqual(via1.GetHashCode(), via7.GetHashCode(), "x11 host vs. HTTP/x11 host (comment)");
            Assert.AreNotEqual(via1.GetHashCode(), via9.GetHashCode(), "x11 host vs. x12 host");
            Assert.AreNotEqual(via1.GetHashCode(), via10.GetHashCode(), "x11 host vs. x11 host2");
            Assert.AreNotEqual(via4.GetHashCode(), via11.GetHashCode(), "HTTP/x11 host vs. WS/x11 host");
            Assert.AreEqual(via4.GetHashCode(), via5.GetHashCode(), "HTTP/x11 host vs. http/x11 host");
            Assert.AreNotEqual(via4.GetHashCode(), via6.GetHashCode(), "HTTP/x11 host vs. x11 host (comment)");
            Assert.AreNotEqual(via6.GetHashCode(), via7.GetHashCode(), "x11 host (comment) vs. HTTP/x11 host (comment)");
            Assert.AreNotEqual(via7.GetHashCode(), via8.GetHashCode(), "HTTP/x11 host (comment) vs. HTTP/x11 host (COMMENT)");
            Assert.AreEqual(via1.GetHashCode(), via12.GetHashCode(), "x11 host vs. x11 host <empty> <empty>");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            ViaHeaderValue via1 = new ViaHeaderValue("x11", "host");
            ViaHeaderValue via2 = new ViaHeaderValue("x11", "HOST");
            ViaHeaderValue via3 = new ViaHeaderValue("X11", "host");
            ViaHeaderValue via4 = new ViaHeaderValue("x11", "host", "HTTP");
            ViaHeaderValue via5 = new ViaHeaderValue("x11", "host", "http");
            ViaHeaderValue via6 = new ViaHeaderValue("x11", "host", null, "(comment)");
            ViaHeaderValue via7 = new ViaHeaderValue("x11", "host", "HTTP", "(comment)");
            ViaHeaderValue via8 = new ViaHeaderValue("x11", "host", "HTTP", "(COMMENT)");
            ViaHeaderValue via9 = new ViaHeaderValue("x12", "host");
            ViaHeaderValue via10 = new ViaHeaderValue("x11", "host2");
            ViaHeaderValue via11 = new ViaHeaderValue("x11", "host", "WS");
            ViaHeaderValue via12 = new ViaHeaderValue("x11", "host", string.Empty, string.Empty);

            Assert.IsFalse(via1.Equals(null), "x11 host vs. <null>");
            Assert.IsTrue(via1.Equals(via2), "x11 host vs. x11 HOST");
            Assert.IsTrue(via1.Equals(via3), "x11 host vs. X11 host");
            Assert.IsFalse(via1.Equals(via4), "x11 host vs. HTTP/x11 host");
            Assert.IsFalse(via4.Equals(via1), "HTTP/x11 host vs. x11 host");
            Assert.IsFalse(via1.Equals(via6), "x11 host vs. HTTP/x11 (comment)");
            Assert.IsFalse(via6.Equals(via1), "HTTP/x11 (comment) vs. x11 host");
            Assert.IsFalse(via1.Equals(via7), "x11 host vs. HTTP/x11 host (comment)");
            Assert.IsFalse(via7.Equals(via1), "HTTP/x11 host (comment) vs. x11 host");
            Assert.IsFalse(via1.Equals(via9), "x11 host vs. x12 host");
            Assert.IsFalse(via1.Equals(via10), "x11 host vs. x11 host2");
            Assert.IsFalse(via4.Equals(via11), "HTTP/x11 host vs. WS/x11 host");
            Assert.IsTrue(via4.Equals(via5), "HTTP/x11 host vs. http/x11 host");
            Assert.IsFalse(via4.Equals(via6), "HTTP/x11 host vs. x11 host (comment)");
            Assert.IsFalse(via6.Equals(via4), "x11 host (comment) vs. HTTP/x11 host");
            Assert.IsFalse(via6.Equals(via7), "x11 host (comment) vs. HTTP/x11 host (comment)");
            Assert.IsFalse(via7.Equals(via6), "HTTP/x11 host (comment) vs. x11 host (comment)");
            Assert.IsFalse(via7.Equals(via8), "HTTP/x11 host (comment) vs. HTTP/x11 host (COMMENT)");
            Assert.IsTrue(via1.Equals(via12), "x11 host vs. x11 host <empty> <empty>");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            ViaHeaderValue source = new ViaHeaderValue("1.1", "host");
            ViaHeaderValue clone = (ViaHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.ProtocolVersion, clone.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual(source.ReceivedBy, clone.ReceivedBy, "ReceivedBy");
            Assert.AreEqual(source.ProtocolName, clone.ProtocolName, "ProtocolName");
            Assert.AreEqual(source.Comment, clone.Comment, "Comment");

            source = new ViaHeaderValue("1.1", "host", "HTTP");
            clone = (ViaHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.ProtocolVersion, clone.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual(source.ReceivedBy, clone.ReceivedBy, "ReceivedBy");
            Assert.AreEqual(source.ProtocolName, clone.ProtocolName, "ProtocolName");
            Assert.AreEqual(source.Comment, clone.Comment, "Comment");

            source = new ViaHeaderValue("1.1", "host", "HTTP", "(comment)");
            clone = (ViaHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.ProtocolVersion, clone.ProtocolVersion, "ProtocolVersion");
            Assert.AreEqual(source.ReceivedBy, clone.ReceivedBy, "ReceivedBy");
            Assert.AreEqual(source.ProtocolName, clone.ProtocolName, "ProtocolName");
            Assert.AreEqual(source.Comment, clone.Comment, "Comment");
        }

        [TestMethod]
        public void GetViaLength_DifferentValidScenarios_AllReturnNonZero()
        {
            CheckGetViaLength(" HTTP  /  1.1   .host \t (comment)  ", 1, 34, 
                new ViaHeaderValue("1.1", ".host", "HTTP", "(comment)"));
            CheckGetViaLength("x11x [FE18:AB64::156]:80 (comment,) other", 0, 36,
                new ViaHeaderValue("x11x", "[FE18:AB64::156]:80", null, "(comment,)"));
         
            // The parser reads until it reaches an invalid/unexpected character. If until then it was able to create
            // a valid ViaHeaderValue, it will return the length of the parsed string. Therefore a string like 
            // "1.1 host," is considered valid (until ','), whereas "1.1 host (invalid" is considered invalid, since
            // the comment is in an invalid format.
            CheckGetViaLength("WS/version example.com,next", 0, 22, new ViaHeaderValue("version", "example.com", "WS"));
            
            // Note that since 'HTTP1.1' is a valid token, it is considered to be the protocol version.
            CheckGetViaLength("HTTP1.1 host", 0, 12, new ViaHeaderValue("HTTP1.1", "host"));

            CheckGetViaLength(" v h", 1, 3, new ViaHeaderValue("v", "h"));
            CheckGetViaLength(" p/v h", 1, 5, new ViaHeaderValue("v", "h", "p"));
            CheckGetViaLength(" p/v h (c)", 1, 9, new ViaHeaderValue("v", "h", "p", "(c)"));
            CheckGetViaLength(" v h,,", 1, 3, new ViaHeaderValue("v", "h"));
            CheckGetViaLength(" p/v h,,", 1, 5, new ViaHeaderValue("v", "h", "p"));
            CheckGetViaLength(" p/v h (c),,", 1, 9, new ViaHeaderValue("v", "h", "p", "(c)"));
            CheckGetViaLength(" v h ", 1, 4, new ViaHeaderValue("v", "h"));
            CheckGetViaLength(" p/v h ", 1, 6, new ViaHeaderValue("v", "h", "p"));
            CheckGetViaLength(" p/v h (c) ", 1, 10, new ViaHeaderValue("v", "h", "p", "(c)"));

            CheckGetViaLength(null, 0, 0, null);
            CheckGetViaLength(string.Empty, 0, 0, null);
            CheckGetViaLength("  ", 0, 0, null);
        }

        [TestMethod]
        public void GetViaLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetViaLength(" 1.1 host", 0); // no leading whitespaces allowed
            CheckInvalidGetViaLength("1=host", 0);
            CheckInvalidGetViaLength("1.1 host (invalid_comment", 0);
            CheckInvalidGetViaLength("=", 0);
            CheckInvalidGetViaLength("HTTP=1.1 host", 0);
            CheckInvalidGetViaLength("HTTP=/1.1 host", 0);
            CheckInvalidGetViaLength("HTTP/1.1[ host", 0);
            CheckInvalidGetViaLength("HTTP/ = host", 0);
            CheckInvalidGetViaLength("HTTP/务 host", 0);
            CheckInvalidGetViaLength("HTTP/  ", 0);
            CheckInvalidGetViaLength("HTTP  ", 0);
            CheckInvalidGetViaLength("HTTP/1.1 /  ", 0);
            CheckInvalidGetViaLength("1.1 ", 0);
            CheckInvalidGetViaLength("  ", 0);
            CheckInvalidGetViaLength("WS/version example.com[", 0);
            CheckInvalidGetViaLength("HTTP/test[::1]", 0);
            CheckInvalidGetViaLength("HTTP/test [::1]:80(comment)", 0);
            CheckInvalidGetViaLength("1.1 http://example.com", 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse(" 1.1   host ", new ViaHeaderValue("1.1", "host"));
            CheckValidParse(" HTTP  /  x11   192.168.0.1\r\n (comment) ", 
                new ViaHeaderValue("x11", "192.168.0.1", "HTTP", "(comment)"));
            CheckValidParse(" HTTP/1.1 [::1]", new ViaHeaderValue("1.1", "[::1]", "HTTP"));
            CheckValidParse("1.1 host", new ViaHeaderValue("1.1", "host"));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("HTTP/1.1 host (comment)invalid");
            CheckInvalidParse("HTTP/1.1 host (comment)=");
            CheckInvalidParse("HTTP/1.1 host (comment) invalid");
            CheckInvalidParse("HTTP/1.1 host (comment) =");
            CheckInvalidParse("HTTP/1.1 host invalid");
            CheckInvalidParse("HTTP/1.1 host =");
            CheckInvalidParse("1.1 host invalid");
            CheckInvalidParse("1.1 host =");
            CheckInvalidParse("会");
            CheckInvalidParse("HTTP/test [::1]:80\r(comment)");
            CheckInvalidParse("HTTP/test [::1]:80\n(comment)");

            CheckInvalidParse("X , , 1.1   host, ,next");
            CheckInvalidParse("X HTTP  /  x11   192.168.0.1\r\n (comment) , ,next");
            CheckInvalidParse(" ,HTTP/1.1 [::1]");

            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse(" 1.1   host ", new ViaHeaderValue("1.1", "host"));
            CheckValidTryParse(" HTTP  /  x11   192.168.0.1\r\n (comment) ",
                new ViaHeaderValue("x11", "192.168.0.1", "HTTP", "(comment)"));
            CheckValidTryParse(" HTTP/1.1 [::1]", new ViaHeaderValue("1.1", "[::1]", "HTTP"));
            CheckValidTryParse("1.1 host", new ViaHeaderValue("1.1", "host"));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("HTTP/1.1 host (comment)invalid");
            CheckInvalidTryParse("HTTP/1.1 host (comment)=");
            CheckInvalidTryParse("HTTP/1.1 host (comment) invalid");
            CheckInvalidTryParse("HTTP/1.1 host (comment) =");
            CheckInvalidTryParse("HTTP/1.1 host invalid");
            CheckInvalidTryParse("HTTP/1.1 host =");
            CheckInvalidTryParse("1.1 host invalid");
            CheckInvalidTryParse("1.1 host =");
            CheckInvalidTryParse("会");
            CheckInvalidTryParse("HTTP/test [::1]:80\r(comment)");
            CheckInvalidTryParse("HTTP/test [::1]:80\n(comment)");

            CheckInvalidTryParse("X , , 1.1   host, ,next");
            CheckInvalidTryParse("X HTTP  /  x11   192.168.0.1\r\n (comment) , ,next");
            CheckInvalidTryParse(" ,HTTP/1.1 [::1]");

            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
        }

        #region Helper methods

        private void CheckValidParse(string input, ViaHeaderValue expectedResult)
        {
            ViaHeaderValue result = ViaHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => ViaHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, ViaHeaderValue expectedResult)
        {
            ViaHeaderValue result = null;
            Assert.IsTrue(ViaHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            ViaHeaderValue result = null;
            Assert.IsFalse(ViaHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CheckGetViaLength(string input, int startIndex, int expectedLength,
            ViaHeaderValue expectedResult)
        {
            object result = null;
            Assert.AreEqual(expectedLength, ViaHeaderValue.GetViaLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.AreEqual(expectedResult, result, "Input: '{0}', Start index: {1}", input, startIndex);
        }

        private static void CheckInvalidGetViaLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, ViaHeaderValue.GetViaLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
