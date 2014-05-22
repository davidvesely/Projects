using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class HttpMethodTest
    {
        [TestMethod]
        public void StaticProperties_VerifyValues_PropertyNameMatchesHttpMethodName()
        {
            Assert.AreEqual("GET", HttpMethod.Get.Method);
            Assert.AreEqual("PUT", HttpMethod.Put.Method);
            Assert.AreEqual("POST", HttpMethod.Post.Method);
            Assert.AreEqual("DELETE", HttpMethod.Delete.Method);
            Assert.AreEqual("HEAD", HttpMethod.Head.Method);
            Assert.AreEqual("OPTIONS", HttpMethod.Options.Method);
            Assert.AreEqual("TRACE", HttpMethod.Trace.Method);
        }

        [TestMethod]
        public void Ctor_ValidMethodToken_Success()
        {
            new HttpMethod("GET");
            new HttpMethod("custom");

            // Note that '!' is the first ASCII char after CTLs and '~' is the last character before DEL char.
            new HttpMethod("validtoken!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz^_`|~");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullMethod_Exception()
        {
            new HttpMethod(null);
        }

        [TestMethod]
        public void Ctor_SeparatorInMethod_Exception()
        {
            char[] separators = new char[] { '(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '/', '[', ']', 
                '?', '=', '{', '}', ' ', '\t' };

            for (int i = 0; i < separators.Length; i++)
            {
                try
                {
                    new HttpMethod("Get" + separators[i]);
                    Assert.Fail("Expected 'FormatException' for method string with separator character '{0}' ({1})",
                        separators[i], (int)separators[i]);
                }
                catch (FormatException) { }
            }
        }

        [TestMethod]
        public void Equals_DifferentComparisonMethodsForSameMethods_MethodsConsideredEqual()
        {
            // Positive test cases
            Assert.IsTrue(new HttpMethod("GET") == HttpMethod.Get, "'==': Expected 'GET' to be equal to 'GET'.");
            Assert.IsFalse(new HttpMethod("GET") != HttpMethod.Get, "'!=': Expected 'GET' to be equal to 'GET'");
            Assert.IsTrue((new HttpMethod("GET")).Equals(HttpMethod.Get), 
                "Equals(): Expected 'GET' to be equal to 'GET'.");

            Assert.IsTrue(new HttpMethod("get") == HttpMethod.Get, "'==': Expected 'get' to be equal to 'GET'.");
            Assert.IsFalse(new HttpMethod("get") != HttpMethod.Get, "'!=': Expected 'get' to be equal to 'GET'");
            Assert.IsTrue((new HttpMethod("get")).Equals(HttpMethod.Get), 
                "Equals(): Expected 'get' to be equal to 'GET'.");
        }

        [TestMethod]
        public void Equals_CompareWithMethodCastedToObject_ReturnsTrue()
        {
            object other = new HttpMethod("GET");
            Assert.IsTrue(HttpMethod.Get.Equals(other), "Equals(object): Expected 'GET' to be equal to 'GET'.");

            // Note that we considered adding an implicit operator to be able to compare string with HttpMethod. 
            // NetFX API review rejected it, since implicit operators must never throw. In our case it would throw
            // when trying to create a HttpMethod instance from an invalid string.
            Assert.IsFalse(HttpMethod.Get.Equals("GET"), "Equals(object): String 'GET' should not be equal to HttpMethod 'GET'.");
        }

        [TestMethod]
        public void Equals_NullComparand_ReturnsFalse()
        {
            Assert.IsFalse(null == HttpMethod.Options);
            Assert.IsFalse(HttpMethod.Trace == null);
        }

        [TestMethod]
        public void GetHashCode_UseCustomStringMethod_SameAsStringHashCode()
        {
            string custom = "CUSTOM";
            HttpMethod method = new HttpMethod(custom);
            Assert.AreEqual(custom.GetHashCode(), method.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_DiferentlyCasedMethod_SameHashCode()
        {
            string input = "GeT";
            HttpMethod method = new HttpMethod(input);
            Assert.AreEqual(HttpMethod.Get.GetHashCode(), method.GetHashCode());
        }

        [TestMethod]
        public void ToString_UseCustomStringMethod_SameAsString()
        {
            string custom = "custom";
            HttpMethod method = new HttpMethod(custom);
            Assert.AreEqual(custom, method.ToString());
        }

        [TestMethod]
        public void Method_AccessProperty_MatchesCtorString()
        {
            HttpMethod method = new HttpMethod("custom");
            Assert.AreEqual("custom", method.Method);
        }
    }
}
