using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class AuthenticationHeaderValueTest
    {
        [TestMethod]
        public void Ctor_SetBothSchemeAndParameters_MatchExpectation()
        {
            AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\"");
            Assert.AreEqual("Basic", auth.Scheme, "Scheme");
            Assert.AreEqual("realm=\"alexandc-tst7\"", auth.Parameter, "Parameter");

            ExceptionAssert.Throws<ArgumentException>(() => { new AuthenticationHeaderValue(null, "x"); }, "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => { new AuthenticationHeaderValue("", "x"); }, "empty string");
            ExceptionAssert.ThrowsFormat(() => { new AuthenticationHeaderValue(" x", "x"); }, "leading space");
            ExceptionAssert.ThrowsFormat(() => { new AuthenticationHeaderValue("x ", "x"); }, "trailing space");
            ExceptionAssert.ThrowsFormat(() => { new AuthenticationHeaderValue("x y", "x"); }, "invalid token");
        }

        [TestMethod]
        public void Ctor_SetSchemeOnly_MatchExpectation()
        {
            // Just verify that this ctor forwards the call to the overload taking 2 parameters.
            AuthenticationHeaderValue auth = new AuthenticationHeaderValue("NTLM");
            Assert.AreEqual("NTLM", auth.Scheme, "Scheme");
            Assert.IsNull(auth.Parameter, "Parameter");
        }

        [TestMethod]
        public void ToString_UseBothNoParameterAndSetParameter_AllSerializedCorrectly()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string input = string.Empty;

            AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Digest",                 
                "qop=\"auth\",algorithm=MD5-sess,nonce=\"+Upgraded+v109e309640b\",charset=utf-8,realm=\"Digest\"");

            Assert.AreEqual(
                "Digest qop=\"auth\",algorithm=MD5-sess,nonce=\"+Upgraded+v109e309640b\",charset=utf-8,realm=\"Digest\"", 
                auth.ToString());
            response.Headers.ProxyAuthenticate.Add(auth);
            input += auth.ToString();

            auth = new AuthenticationHeaderValue("Negotiate");
            Assert.AreEqual("Negotiate", auth.ToString());
            response.Headers.ProxyAuthenticate.Add(auth);
            input += ", " + auth.ToString();

            auth = new AuthenticationHeaderValue("Custom", ""); // empty string should be treated like 'null'.
            Assert.AreEqual("Custom", auth.ToString());
            response.Headers.ProxyAuthenticate.Add(auth);
            input += ", " + auth.ToString();

            string result = response.Headers.ProxyAuthenticate.ToString();
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Parse_GoodValues_Success()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            string input = " Digest qop=\"auth\",algorithm=MD5-sess,nonce=\"+Upgraded+v109e309640b\",charset=utf-8 ";

            request.Headers.Authorization = AuthenticationHeaderValue.Parse(input);
            Assert.AreEqual(input.Trim(), request.Headers.Authorization.ToString());
        }

        [TestMethod]
        public void TryParse_GoodValues_Success()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            string input = " Digest qop=\"auth\",algorithm=MD5-sess,nonce=\"+Upgraded+v109e309640b\",realm=\"Digest\" ";

            AuthenticationHeaderValue parsedValue;
            Assert.IsTrue(AuthenticationHeaderValue.TryParse(input, out parsedValue));
            request.Headers.Authorization = parsedValue;            
            Assert.AreEqual(input.Trim(), request.Headers.Authorization.ToString());            
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_BadValues_Throws()
        {
            string input = "D\rigest qop=\"auth\",algorithm=MD5-sess,charset=utf-8,realm=\"Digest\"";
            
            AuthenticationHeaderValue.Parse(input);
        }

        [TestMethod]
        public void TryParse_BadValues_False()
        {
            string input = ", Digest qop=\"auth\",nonce=\"+Upgraded+v109e309640b\",charset=utf-8,realm=\"Digest\"";
            
            AuthenticationHeaderValue parsedValue;
            Assert.IsFalse(AuthenticationHeaderValue.TryParse(input, out parsedValue));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Add_BadValues_Throws()
        {
            string input = "Digest algorithm=MD5-sess,nonce=\"+Upgraded+v109e309640b\",charset=utf-8,realm=\"Digest\", ";

            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add(HttpKnownHeaderNames.Authorization, input);
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentAuth_SameOrDifferentHashCodes()
        {
            AuthenticationHeaderValue auth1 = new AuthenticationHeaderValue("A", "b");
            AuthenticationHeaderValue auth2 = new AuthenticationHeaderValue("a", "b");
            AuthenticationHeaderValue auth3 = new AuthenticationHeaderValue("A", "B");
            AuthenticationHeaderValue auth4 = new AuthenticationHeaderValue("A");
            AuthenticationHeaderValue auth5 = new AuthenticationHeaderValue("A", "");
            AuthenticationHeaderValue auth6 = new AuthenticationHeaderValue("X", "b");

            Assert.AreEqual(auth1.GetHashCode(), auth2.GetHashCode(), "'A b' vs. 'a b'");
            Assert.AreNotEqual(auth1.GetHashCode(), auth3.GetHashCode(), "'A b' vs. 'A B'");
            Assert.AreNotEqual(auth1.GetHashCode(), auth4.GetHashCode(), "'A b' vs. 'A'");
            Assert.AreEqual(auth4.GetHashCode(), auth5.GetHashCode(), "'A' vs. 'A <empty_string>'");
            Assert.AreNotEqual(auth1.GetHashCode(), auth6.GetHashCode(), "'A b' vs. 'X b'");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentAuth_EqualOrNotEqualNoExceptions()
        {
            AuthenticationHeaderValue auth1 = new AuthenticationHeaderValue("A", "b");
            AuthenticationHeaderValue auth2 = new AuthenticationHeaderValue("a", "b");
            AuthenticationHeaderValue auth3 = new AuthenticationHeaderValue("A", "B");
            AuthenticationHeaderValue auth4 = new AuthenticationHeaderValue("A");
            AuthenticationHeaderValue auth5 = new AuthenticationHeaderValue("A", "");
            AuthenticationHeaderValue auth6 = new AuthenticationHeaderValue("X", "b");

            Assert.IsFalse(auth1.Equals(null), "bytes=1-2 vs. <null>");
            Assert.IsTrue(auth1.Equals(auth2), "'A b' vs. 'a b'");
            Assert.IsFalse(auth1.Equals(auth3), "'A b' vs. 'A B'");
            Assert.IsFalse(auth1.Equals(auth4), "'A b' vs. 'A'");
            Assert.IsFalse(auth4.Equals(auth1), "'A' vs. 'A b'");
            Assert.IsFalse(auth1.Equals(auth5), "'A b' vs. 'A <empty_string>'");
            Assert.IsFalse(auth5.Equals(auth1), "'A <empty_string>' vs. 'A b'");
            Assert.IsTrue(auth4.Equals(auth5), "'A' vs. 'A <empty_string>'");
            Assert.IsTrue(auth5.Equals(auth4), "'A <empty_string>' vs. 'A'");
            Assert.IsFalse(auth1.Equals(auth6), "'A b' vs. 'X b'");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            AuthenticationHeaderValue source = new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
            AuthenticationHeaderValue clone = (AuthenticationHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Scheme, clone.Scheme, "Scheme");
            Assert.AreEqual(source.Parameter, clone.Parameter, "Parameter");

            source = new AuthenticationHeaderValue("Kerberos");
            clone = (AuthenticationHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Scheme, clone.Scheme, "Scheme");
            Assert.IsNull(clone.Parameter, "Parameter");
        }

        [TestMethod]
        public void GetAuthenticationLength_DifferentValidScenarios_AllReturnNonZero()
        {
            CallGetAuthenticationLength(" Basic  QWxhZGRpbjpvcGVuIHNlc2FtZQ==  ", 1, 37,
                new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ=="));
            CallGetAuthenticationLength(" Basic  QWxhZGRpbjpvcGVuIHNlc2FtZQ==  , ", 1, 37,
                new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ=="));
            CallGetAuthenticationLength(" Basic realm=\"example.com\"", 1, 25,
                new AuthenticationHeaderValue("Basic", "realm=\"example.com\""));
            CallGetAuthenticationLength(" Basic realm=\"exam,,ple.com\",", 1, 27,
                new AuthenticationHeaderValue("Basic", "realm=\"exam,,ple.com\""));
            CallGetAuthenticationLength(" Basic realm=\"exam,ple.com\",", 1, 26,
                new AuthenticationHeaderValue("Basic", "realm=\"exam,ple.com\""));
            CallGetAuthenticationLength("NTLM   ", 0, 7, new AuthenticationHeaderValue("NTLM"));
            CallGetAuthenticationLength("Digest", 0, 6, new AuthenticationHeaderValue("Digest"));
            CallGetAuthenticationLength("Digest,,", 0, 6, new AuthenticationHeaderValue("Digest"));
            CallGetAuthenticationLength("Digest a=b, c=d,,", 0, 15, new AuthenticationHeaderValue("Digest", "a=b, c=d"));
            CallGetAuthenticationLength("Kerberos,", 0, 8, new AuthenticationHeaderValue("Kerberos"));
            CallGetAuthenticationLength("Basic,NTLM", 0, 5, new AuthenticationHeaderValue("Basic"));
            CallGetAuthenticationLength("Digest a=b,c=\"d\", e=f, NTLM", 0, 21,
                new AuthenticationHeaderValue("Digest", "a=b,c=\"d\", e=f"));
            CallGetAuthenticationLength("Digest a = b , c = \"d\" ,  e = f ,NTLM", 0, 32,
                new AuthenticationHeaderValue("Digest", "a = b , c = \"d\" ,  e = f"));
            CallGetAuthenticationLength("Digest a = b , c = \"d\" ,  e = f , NTLM AbCdEf==", 0, 32,
                new AuthenticationHeaderValue("Digest", "a = b , c = \"d\" ,  e = f"));
            CallGetAuthenticationLength("Digest a = \"b\", c= \"d\" ,  e = f,NTLM AbC=,", 0, 31,
                new AuthenticationHeaderValue("Digest", "a = \"b\", c= \"d\" ,  e = f"));
            CallGetAuthenticationLength("Digest a=\"b\", c=d", 0, 17,
                new AuthenticationHeaderValue("Digest", "a=\"b\", c=d"));
            CallGetAuthenticationLength("Digest a=\"b\", c=d,", 0, 17,
                new AuthenticationHeaderValue("Digest", "a=\"b\", c=d"));
            CallGetAuthenticationLength("Digest a=\"b\", c=d ,", 0, 18,
                new AuthenticationHeaderValue("Digest", "a=\"b\", c=d"));
            CallGetAuthenticationLength("Digest a=\"b\", c=d  ", 0, 19,
                new AuthenticationHeaderValue("Digest", "a=\"b\", c=d"));
            CallGetAuthenticationLength("Custom \"blob\", c=d,Custom2 \"blob\"", 0, 18,
                new AuthenticationHeaderValue("Custom", "\"blob\", c=d"));
            CallGetAuthenticationLength("Custom \"blob\", a=b,,,c=d,Custom2 \"blob\"", 0, 24,
                new AuthenticationHeaderValue("Custom", "\"blob\", a=b,,,c=d"));
            CallGetAuthenticationLength("Custom \"blob\", a=b,c=d,,,Custom2 \"blob\"", 0, 22,
                new AuthenticationHeaderValue("Custom", "\"blob\", a=b,c=d"));
            CallGetAuthenticationLength("Custom a=b, c=d,,,InvalidNextScheme服", 0, 15,
                new AuthenticationHeaderValue("Custom", "a=b, c=d"));
        }

        [TestMethod]
        public void GetAuthenticationLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetAuthenticationLength(" NTLM", 0); // no leading whitespaces allowed
            CheckInvalidGetAuthenticationLength("Basic=", 0);
            CheckInvalidGetAuthenticationLength("=Basic", 0);
            CheckInvalidGetAuthenticationLength("Digest a=b, 服", 0);
            CheckInvalidGetAuthenticationLength("Digest a=b, c=d, 服", 0);
            CheckInvalidGetAuthenticationLength("Digest a=b, c=", 0);
            CheckInvalidGetAuthenticationLength("Digest a=\"b, c", 0);
            CheckInvalidGetAuthenticationLength("Digest a=\"b", 0);
            CheckInvalidGetAuthenticationLength("Digest a=b, c=服", 0);

            CheckInvalidGetAuthenticationLength("", 0);
            CheckInvalidGetAuthenticationLength(null, 0);
        }

        #region Helper methods

        private static void CallGetAuthenticationLength(string input, int startIndex, int expectedLength,
            AuthenticationHeaderValue expectedResult)
        {
            object result = null;
            Assert.AreEqual(expectedLength, AuthenticationHeaderValue.GetAuthenticationLength(input, startIndex, 
                out result), "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.AreEqual(expectedResult, result, "Input: '{0}', Start index: {1}", input, startIndex);
        }

        private static void CheckInvalidGetAuthenticationLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, AuthenticationHeaderValue.GetAuthenticationLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
