using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Web;
using Microsoft.TestCommon;

namespace System.Net.Http.Test
{
    [TestClass]
    public class FormUrlEncodedContentTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullSource_Throw()
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(null);
        }

        [TestMethod]
        public void Ctor_EmptySource_Succeed()
        {
            // No exception should be thrown for empty collections.
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>());
            Assert.AreEqual(0, content.ReadAsStreamAsync().Result.Length, "Length");
        }

        [TestMethod]
        public void Ctor_OneEntry_SeperatedByEquals()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("key", "value");
            FormUrlEncodedContent content = new FormUrlEncodedContent(data);

            Assert.AreEqual(9, content.ReadAsStreamAsync().Result.Length, "Length");
            string result = new StreamReader(content.ReadAsStreamAsync().Result).ReadToEnd();
            Assert.AreEqual("key=value", result);
        }

        [TestMethod]
        public void Ctor_OneUnicodeEntry_Encoded()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("key", "valueク");
            FormUrlEncodedContent content = new FormUrlEncodedContent(data);

            Assert.AreEqual(18, content.ReadAsStreamAsync().Result.Length, "Length");
            string result = new StreamReader(content.ReadAsStreamAsync().Result).ReadToEnd();
            Assert.AreEqual("key=value%E3%82%AF", result);
        }

        [TestMethod]
        public void Ctor_TwoEntries_SperatedByAnd()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("key1", "value1");
            data.Add("key2", "value2");
            FormUrlEncodedContent content = new FormUrlEncodedContent(data);

            Assert.AreEqual(23, content.ReadAsStreamAsync().Result.Length, "Length");
            string result = new StreamReader(content.ReadAsStreamAsync().Result).ReadToEnd();
            Assert.AreEqual("key1=value1&key2=value2", result);
        }

        [TestMethod]
        public void Ctor_WithSpaces_EncodedAsPlus()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("key 1", "val%20ue 1"); // %20 is a percent-encoded space, make sure it survives
            data.Add("key 2", "val%ue 2");
            FormUrlEncodedContent content = new FormUrlEncodedContent(data);

            Assert.AreEqual(35, content.ReadAsStreamAsync().Result.Length, "Length");
            string result = new StreamReader(content.ReadAsStreamAsync().Result).ReadToEnd();
            Assert.AreEqual("key+1=val%2520ue+1&key+2=val%25ue+2", result);
        }

        [TestMethod]
        public void Ctor_AllAsciiChars_EncodingMatchesHttpUtilty()
        {
            // List every char 0 - 127
            StringBuilder builder = new StringBuilder();
            for (int ch = 0; ch < 128; ch++)
            {
                builder.Append((char)ch);
            }
            string testString = builder.ToString();

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("key", testString);
            FormUrlEncodedContent content = new FormUrlEncodedContent(data);
            
            // HttpUtility encodes using lower case hex.
            string result = new StreamReader(content.ReadAsStreamAsync().Result).ReadToEnd().ToLowerInvariant();
            string expectedResult = "key=" + HttpUtility.UrlEncode(testString).ToLowerInvariant();

            // Uri.EscapeDataString behaves differently in 4.5 to conform with RFC 3986
            string knownDiscrepancies = RuntimeEnvironment.IsVersion45Installed ? "~!*()" : "~'";

            int discrepancies = 0;
            for (int i = 0; i < result.Length && i < expectedResult.Length; i++)
            {
                if (result[i] != expectedResult[i])
                {
                    Assert.IsTrue((result[i] == '%' || expectedResult[i] == '%'),
                        "Non-Escaping mis-match: " + i);

                    if (result[i] == '%')
                    {
                        Assert.IsTrue(knownDiscrepancies.Contains(expectedResult[i]), 
                            "Escaped when it shouldn't be: " + expectedResult[i]);
                        result = result.Substring(i + 3);
                        expectedResult = expectedResult.Substring(i + 1);
                    }
                    else
                    {
                        Assert.IsTrue(knownDiscrepancies.Contains(result[i]), 
                            "Not escaped when it should be: " + result[i]);
                        result = result.Substring(i + 1);
                        expectedResult = expectedResult.Substring(i + 3);
                    }
                    i = -1;
                    discrepancies++;
                }
            }
            Assert.AreEqual(knownDiscrepancies.Length, discrepancies);
        }
    }
}
