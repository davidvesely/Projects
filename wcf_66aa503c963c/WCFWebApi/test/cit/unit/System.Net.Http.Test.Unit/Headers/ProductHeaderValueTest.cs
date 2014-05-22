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
    public class ProductHeaderValueTest
    {
        [TestMethod]
        public void Ctor_SetValidHeaderValues_InstanceCreatedCorrectly()
        {
            ProductHeaderValue product = new ProductHeaderValue("HTTP", "2.0");
            Assert.AreEqual("HTTP", product.Name, "Name");
            Assert.AreEqual("2.0", product.Version, "Version");

            product = new ProductHeaderValue("HTTP");
            Assert.AreEqual("HTTP", product.Name, "Name");
            Assert.IsNull(product.Version, "Version");

            product = new ProductHeaderValue("HTTP", ""); // null and string.Empty are equivalent
            Assert.AreEqual("HTTP", product.Name, "Name");
            Assert.IsNull(product.Version, "Version");
        }

        [TestMethod]
        public void Ctor_UseInvalidValues_Throw()
        {
            ExceptionAssert.Throws<ArgumentException>(() => { new ProductHeaderValue(null); }, "null");
            ExceptionAssert.Throws<ArgumentException>(() => { new ProductHeaderValue(string.Empty); }, "empty string");
            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue(" x"); }, "leading space");
            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue("x "); }, "trailing space");
            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue("x y"); }, "invalid token");

            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue("x", " y"); }, "leading space (version)");
            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue("x", "y "); }, "trailing space (version)");
            ExceptionAssert.ThrowsFormat(() => { new ProductHeaderValue("x", "y z"); }, "invalid token (version)");
        }

        [TestMethod]
        public void ToString_UseDifferentRanges_AllSerializedCorrectly()
        {
            ProductHeaderValue product = new ProductHeaderValue("IRC", "6.9");
            Assert.AreEqual("IRC/6.9", product.ToString());

            product = new ProductHeaderValue("product");
            Assert.AreEqual("product", product.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentRanges_SameOrDifferentHashCodes()
        {
            ProductHeaderValue product1 = new ProductHeaderValue("custom", "1.0");
            ProductHeaderValue product2 = new ProductHeaderValue("custom");
            ProductHeaderValue product3 = new ProductHeaderValue("CUSTOM", "1.0");
            ProductHeaderValue product4 = new ProductHeaderValue("RTA", "x11");
            ProductHeaderValue product5 = new ProductHeaderValue("rta", "X11");

            Assert.AreNotEqual(product1.GetHashCode(), product2.GetHashCode(), "custom/1.0 vs. custom");
            Assert.AreEqual(product1.GetHashCode(), product3.GetHashCode(), "custom/1.0 vs. CUSTOM/1.0");
            Assert.AreNotEqual(product1.GetHashCode(), product4.GetHashCode(), "custom/1.0 vs. rta/X11");
            Assert.AreEqual(product4.GetHashCode(), product5.GetHashCode(), "RTA/x11 vs. rta/X11");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            ProductHeaderValue product1 = new ProductHeaderValue("custom", "1.0");
            ProductHeaderValue product2 = new ProductHeaderValue("custom");
            ProductHeaderValue product3 = new ProductHeaderValue("CUSTOM", "1.0");
            ProductHeaderValue product4 = new ProductHeaderValue("RTA", "x11");
            ProductHeaderValue product5 = new ProductHeaderValue("rta", "X11");

            Assert.IsFalse(product1.Equals(null), "custom/1.0 vs. <null>");
            Assert.IsFalse(product1.Equals(product2), "custom/1.0 vs. custom");
            Assert.IsFalse(product2.Equals(product1), "custom/1.0 vs. custom");
            Assert.IsTrue(product1.Equals(product3), "custom/1.0 vs. CUSTOM/1.0");
            Assert.IsFalse(product1.Equals(product4), "custom/1.0 vs. rta/X11");
            Assert.IsTrue(product4.Equals(product5), "RTA/x11 vs. rta/X11");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            ProductHeaderValue source = new ProductHeaderValue("SHTTP", "1.3");
            ProductHeaderValue clone = (ProductHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.Name, clone.Name, "Name");
            Assert.AreEqual(source.Version, clone.Version, "Version");

            source = new ProductHeaderValue("SHTTP", null);
            clone = (ProductHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.Name, clone.Name, "Name");
            Assert.IsNull(clone.Version);
        }

        [TestMethod]
        public void GetProductLength_DifferentValidScenarios_AllReturnNonZero()
        {
            ProductHeaderValue result = null;

            CallGetProductLength(" custom", 1, 6, out result);
            Assert.AreEqual("custom", result.Name, "Name");
            Assert.IsNull(result.Version, "Version");

            CallGetProductLength(" custom, ", 1, 6, out result);
            Assert.AreEqual("custom", result.Name, "Name");
            Assert.IsNull(result.Version, "Version");

            CallGetProductLength(" custom / 5.6 ", 1, 13, out result);
            Assert.AreEqual("custom", result.Name, "Name");
            Assert.AreEqual("5.6", result.Version, "Version");

            CallGetProductLength("RTA / x58 ,", 0, 10, out result);
            Assert.AreEqual("RTA", result.Name, "Name");
            Assert.AreEqual("x58", result.Version, "Version");

            CallGetProductLength("RTA / x58", 0, 9, out result);
            Assert.AreEqual("RTA", result.Name, "Name");
            Assert.AreEqual("x58", result.Version, "Version");

            CallGetProductLength("RTA / x58 XXX", 0, 10, out result);
            Assert.AreEqual("RTA", result.Name, "Name");
            Assert.AreEqual("x58", result.Version, "Version");
        }

        [TestMethod]
        public void GetProductLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetProductLength(" custom", 0); // no leading whitespaces allowed
            CheckInvalidGetProductLength("custom/", 0);
            CheckInvalidGetProductLength("custom/[", 0);
            CheckInvalidGetProductLength("=", 0);

            CheckInvalidGetProductLength("", 0);
            CheckInvalidGetProductLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse(" y/1 ", new ProductHeaderValue("y", "1"));
            CheckValidParse(" custom / 1.0 ", new ProductHeaderValue("custom", "1.0"));
            CheckValidParse("custom / 1.0 ", new ProductHeaderValue("custom", "1.0"));
            CheckValidParse("custom / 1.0", new ProductHeaderValue("custom", "1.0"));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("product/version="); // only delimiter ',' allowed after last product
            CheckInvalidParse("product otherproduct");
            CheckInvalidParse("product[");
            CheckInvalidParse("=");

            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse(" y/1 ", new ProductHeaderValue("y", "1"));
            CheckValidTryParse(" custom / 1.0 ", new ProductHeaderValue("custom", "1.0"));
            CheckValidTryParse("custom / 1.0 ", new ProductHeaderValue("custom", "1.0"));
            CheckValidTryParse("custom / 1.0", new ProductHeaderValue("custom", "1.0"));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("product/version="); // only delimiter ',' allowed after last product
            CheckInvalidTryParse("product otherproduct");
            CheckInvalidTryParse("product[");
            CheckInvalidTryParse("=");

            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
        }

        #region Helper methods

        private void CheckValidParse(string input, ProductHeaderValue expectedResult)
        {
            ProductHeaderValue result = ProductHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => ProductHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, ProductHeaderValue expectedResult)
        {
            ProductHeaderValue result = null;
            Assert.IsTrue(ProductHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            ProductHeaderValue result = null;
            Assert.IsFalse(ProductHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetProductLength(string input, int startIndex, int expectedLength,
            out ProductHeaderValue result)
        {
            Assert.AreEqual(expectedLength, ProductHeaderValue.GetProductLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
        }

        private static void CheckInvalidGetProductLength(string input, int startIndex)
        {
            ProductHeaderValue result = null;
            Assert.AreEqual(0, ProductHeaderValue.GetProductLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
