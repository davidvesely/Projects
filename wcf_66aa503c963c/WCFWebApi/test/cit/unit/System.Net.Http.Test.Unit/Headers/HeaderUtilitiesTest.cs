using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Net.Test.Common;

namespace Microsoft.Net.Http.Test.Headers
{
    [TestClass]
    public class HeaderUtilitiesTest
    {
        [TestMethod]
        public void AreEqualCollections_UseSetOfEqualCollections_ReturnsTrue()
        {
            ICollection<NameValueHeaderValue> x = new List<NameValueHeaderValue>();
            ICollection<NameValueHeaderValue> y = new List<NameValueHeaderValue>();

            Assert.IsTrue(HeaderUtilities.AreEqualCollections(x, y), "<empty> == <empty>");

            x.Add(new NameValueHeaderValue("a"));
            x.Add(new NameValueHeaderValue("c"));
            x.Add(new NameValueHeaderValue("b"));
            x.Add(new NameValueHeaderValue("c"));

            y.Add(new NameValueHeaderValue("c"));
            y.Add(new NameValueHeaderValue("c"));
            y.Add(new NameValueHeaderValue("b"));
            y.Add(new NameValueHeaderValue("a"));

            Assert.IsTrue(HeaderUtilities.AreEqualCollections(x, y), "Expected 'acbc' == 'ccba'");
            Assert.IsTrue(HeaderUtilities.AreEqualCollections(y, x), "Expected 'ccba' == 'acbc'");
        }

        [TestMethod]
        public void AreEqualCollections_UseSetOfNotEqualCollections_ReturnsFalse()
        {
            ICollection<NameValueHeaderValue> x = new List<NameValueHeaderValue>();
            ICollection<NameValueHeaderValue> y = new List<NameValueHeaderValue>();

            Assert.IsTrue(HeaderUtilities.AreEqualCollections(x, y), "Expected '<empty>' == '<empty>'");

            x.Add(new NameValueHeaderValue("a"));
            x.Add(new NameValueHeaderValue("c"));
            x.Add(new NameValueHeaderValue("b"));
            x.Add(new NameValueHeaderValue("c"));

            y.Add(new NameValueHeaderValue("a"));
            y.Add(new NameValueHeaderValue("b"));
            y.Add(new NameValueHeaderValue("c"));
            y.Add(new NameValueHeaderValue("d"));

            Assert.IsFalse(HeaderUtilities.AreEqualCollections(x, y), "Expected 'acbc' != 'abcd'");
            Assert.IsFalse(HeaderUtilities.AreEqualCollections(y, x), "Expected 'abcd' != 'acbc'");

            y.Clear();
            y.Add(new NameValueHeaderValue("a"));
            y.Add(new NameValueHeaderValue("b"));
            y.Add(new NameValueHeaderValue("b"));
            y.Add(new NameValueHeaderValue("c"));

            Assert.IsFalse(HeaderUtilities.AreEqualCollections(x, y), "Expected 'acbc' != 'abbc'");
            Assert.IsFalse(HeaderUtilities.AreEqualCollections(y, x), "Expected 'abbc' != 'acbc'");
        }

        [TestMethod]
        public void GetNextNonEmptyOrWhitespaceIndex_UseDifferentInput_MatchExpectation()
        {
            CheckGetNextNonEmptyOrWhitespaceIndex("x , , ,,  ,\t\r\n , ,x", 1, true, 18, true);
            CheckGetNextNonEmptyOrWhitespaceIndex("x , ,   ", 1, false, 4, true); // stop at the second ','
            CheckGetNextNonEmptyOrWhitespaceIndex("x , ,   ", 1, true, 8, true);
            CheckGetNextNonEmptyOrWhitespaceIndex(" x", 0, true, 1, false);
            CheckGetNextNonEmptyOrWhitespaceIndex(" ,x", 0, true, 2, true);
            CheckGetNextNonEmptyOrWhitespaceIndex(" ,x", 0, false, 2, true);
            CheckGetNextNonEmptyOrWhitespaceIndex(" ,,x", 0, true, 3, true);
            CheckGetNextNonEmptyOrWhitespaceIndex(" ,,x", 0, false, 2, true);
        }

        [TestMethod]
        public void CheckValidQuotedString_ValidAndInvalidvalues_MatchExpectation()
        {
            // No exception expected for the following input.
            HeaderUtilities.CheckValidQuotedString("\"x\"", "param");
            HeaderUtilities.CheckValidQuotedString("\"x y\"", "param");

            ExceptionAssert.Throws<ArgumentException>(() => HeaderUtilities.CheckValidQuotedString(null, "param"),
                "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => HeaderUtilities.CheckValidQuotedString("", "param"),
                "<empty>");
            ExceptionAssert.ThrowsFormat(() => HeaderUtilities.CheckValidQuotedString("\"x", "param"), "\"x");
            ExceptionAssert.ThrowsFormat(() => HeaderUtilities.CheckValidQuotedString("\"x\"y", "param"), "\"x\"y");
        }

        [TestMethod]
        public void CaseInsensitiveStringEqualityComparer_CompareDifferentStrings_CaseInsensitiveComparison()
        {
            HeaderUtilities.CaseInsensitiveStringEqualityComparer comparer = 
                HeaderUtilities.CaseInsensitiveStringComparer;

            Assert.IsTrue(comparer.Equals("value", "VALUE"), "value == VALUE");
            Assert.IsTrue(comparer.Equals("value", "value"), "value == value");
            Assert.IsFalse(comparer.Equals("value", "value1"), "value == value1");
            Assert.IsFalse(comparer.Equals((string)null, "value"), "<null> == value");
            Assert.IsFalse(comparer.Equals("value", (string)null), "value == <null>");
            Assert.IsFalse(comparer.Equals("", "value"), "<empty> == value");
            Assert.IsFalse(comparer.Equals("value", ""), "value == <empty>");
            
            Assert.IsTrue(comparer.Equals((object)"value", (object)"VALUE"), "object: value == VALUE");
            Assert.IsTrue(comparer.Equals((object)"value", (object)"value"), "object: value == value");
            Assert.IsFalse(comparer.Equals((object)"value", (object)"value1"), "object: value == value1");
            Assert.IsFalse(comparer.Equals((object)null, (object)"value"), "object: <null> == value");
            Assert.IsFalse(comparer.Equals((object)"value", (object)null), "object: value == <null>");
            Assert.IsFalse(comparer.Equals((object)"", (object)"value"), "object: <empty> == value");
            Assert.IsFalse(comparer.Equals((object)"value", (object)""), "object: value == <empty>");
        }

        [TestMethod]
        public void CaseInsensitiveStringEqualityComparer_GetHashCodeForDifferentStrings_SameHashForStringsDifferingOnlyInCase()
        {
            HeaderUtilities.CaseInsensitiveStringEqualityComparer comparer =
                HeaderUtilities.CaseInsensitiveStringComparer;

            Assert.AreEqual(comparer.GetHashCode("value"), comparer.GetHashCode("VALUE"), "value == VALUE");
            Assert.AreEqual(comparer.GetHashCode("value"), comparer.GetHashCode("value"), "value == value");
            Assert.AreNotEqual(comparer.GetHashCode("value"), comparer.GetHashCode("value1"), "value == value1");
            Assert.AreNotEqual(comparer.GetHashCode((string)null), comparer.GetHashCode("value"), "<null> == value");
            Assert.AreNotEqual(comparer.GetHashCode("value"), comparer.GetHashCode((string)null), "value == <null>");
            Assert.AreNotEqual(comparer.GetHashCode(""), comparer.GetHashCode("value"), "<empty> == value");
            Assert.AreNotEqual(comparer.GetHashCode("value"), comparer.GetHashCode(""), "value == <empty>");

            Assert.AreEqual(comparer.GetHashCode((object)"value"), comparer.GetHashCode((object)"VALUE"), 
                "value == VALUE");
            Assert.AreEqual(comparer.GetHashCode((object)"value"), comparer.GetHashCode((object)"value"),
                "value == value");
            Assert.AreNotEqual(comparer.GetHashCode((object)"value"), comparer.GetHashCode((object)"value1"), 
                "value == value1");
            Assert.AreNotEqual(comparer.GetHashCode((object)null), comparer.GetHashCode((object)"value"),
                "<null> == value");
            Assert.AreNotEqual(comparer.GetHashCode((object)"value"), comparer.GetHashCode((object)null), 
                "value == <null>");
            Assert.AreNotEqual(comparer.GetHashCode((object)""), comparer.GetHashCode((object)"value"),
                "<empty> == value");
            Assert.AreNotEqual(comparer.GetHashCode((object)"value"), comparer.GetHashCode((object)""), 
                "value == <empty>");
        }

        #region Helper methods

        private static void CheckGetNextNonEmptyOrWhitespaceIndex(string input, int startIndex, 
            bool supportsEmptyValues, int expectedIndex, bool expectedSeparatorFound)
        {
            bool separatorFound = false;
            Assert.AreEqual(expectedIndex, HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex,
                supportsEmptyValues, out separatorFound), "Input: '{0}', Index: {1}", input, startIndex);
            Assert.AreEqual(expectedSeparatorFound, separatorFound, "Separator - Input: '{0}', Index: {1}", 
                input, startIndex);
        }

        #endregion
    }
}
