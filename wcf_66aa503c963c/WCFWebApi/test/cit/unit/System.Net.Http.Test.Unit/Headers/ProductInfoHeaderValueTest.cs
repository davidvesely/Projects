using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Test.Common;
using System.Net.Http.Headers;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class ProductInfoHeaderValueTest
    {
        [TestMethod]
        public void Ctor_ProductOverload_MatchExpectation()
        {
            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue(new ProductHeaderValue("product"));
            Assert.AreEqual(new ProductHeaderValue("product"), productInfo.Product, "Product");
            Assert.IsNull(productInfo.Comment, "Comment");

            ProductHeaderValue input = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => { new ProductInfoHeaderValue(input); }, "<null>");
        }

        [TestMethod]
        public void Ctor_ProductStringOverload_MatchExpectation()
        {
            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue("product", "1.0");
            Assert.AreEqual(new ProductHeaderValue("product", "1.0"), productInfo.Product, "Product");
            Assert.IsNull(productInfo.Comment, "Comment");
        }

        [TestMethod]
        public void Ctor_CommentOverload_MatchExpectation()
        {
            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue("(this is a comment)");
            Assert.IsNull(productInfo.Product, "Product");
            Assert.AreEqual("(this is a comment)", productInfo.Comment, "Comment");

            ExceptionAssert.Throws<ArgumentException>(() => { new ProductInfoHeaderValue((string)null); }, "<null>");
            ExceptionAssert.ThrowsFormat(() => { new ProductInfoHeaderValue("invalid comment"); }, "<null>");
            ExceptionAssert.ThrowsFormat(() => { new ProductInfoHeaderValue(" (leading space)"); }, "leading space");
            ExceptionAssert.ThrowsFormat(() => { new ProductInfoHeaderValue("(trailing space) "); }, "trailing space");
        }

        [TestMethod]
        public void ToString_UseDifferentProductInfos_AllSerializedCorrectly()
        {
            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue("product", "1.0");
            Assert.AreEqual("product/1.0", productInfo.ToString());

            productInfo = new ProductInfoHeaderValue("(comment)");
            Assert.AreEqual("(comment)", productInfo.ToString());
        }

        [TestMethod]
        public void ToString_Aggregate_AllSerializedCorrectly()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            string input = string.Empty;

            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue("product", "1.0");
            Assert.AreEqual("product/1.0", productInfo.ToString());

            input += productInfo.ToString();
            request.Headers.UserAgent.Add(productInfo);

            productInfo = new ProductInfoHeaderValue("(comment)");
            Assert.AreEqual("(comment)", productInfo.ToString());

            input += " " + productInfo.ToString(); // Space delineated
            request.Headers.UserAgent.Add(productInfo);

            Assert.AreEqual(input, request.Headers.UserAgent.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentProductInfos_SameOrDifferentHashCodes()
        {
            ProductInfoHeaderValue productInfo1 = new ProductInfoHeaderValue("product", "1.0");
            ProductInfoHeaderValue productInfo2 = new ProductInfoHeaderValue(new ProductHeaderValue("product", "1.0"));
            ProductInfoHeaderValue productInfo3 = new ProductInfoHeaderValue("(comment)");
            ProductInfoHeaderValue productInfo4 = new ProductInfoHeaderValue("(COMMENT)");

            Assert.AreEqual(productInfo1.GetHashCode(), productInfo2.GetHashCode(), "product/1.0 vs. product/1.0");
            Assert.AreNotEqual(productInfo1.GetHashCode(), productInfo3.GetHashCode(), "product/1.0 vs. (comment)");
            Assert.AreNotEqual(productInfo3.GetHashCode(), productInfo4.GetHashCode(), "(comment) vs. (COMMENT)");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentRanges_EqualOrNotEqualNoExceptions()
        {
            ProductInfoHeaderValue productInfo1 = new ProductInfoHeaderValue("product", "1.0");
            ProductInfoHeaderValue productInfo2 = new ProductInfoHeaderValue(new ProductHeaderValue("product", "1.0"));
            ProductInfoHeaderValue productInfo3 = new ProductInfoHeaderValue("(comment)");
            ProductInfoHeaderValue productInfo4 = new ProductInfoHeaderValue("(COMMENT)");

            Assert.IsFalse(productInfo1.Equals(null), "product/1.0 vs. <null>");
            Assert.IsTrue(productInfo1.Equals(productInfo2), "product/1.0 vs. product/1.0");
            Assert.IsFalse(productInfo1.Equals(productInfo3), "product/1.0 vs. (comment)");
            Assert.IsFalse(productInfo3.Equals(productInfo4), "(comment) vs. (COMMENT)");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            ProductInfoHeaderValue source = new ProductInfoHeaderValue("product", "1.0");
            ProductInfoHeaderValue clone = (ProductInfoHeaderValue)((ICloneable)source).Clone();

            Assert.AreEqual(source.Product, clone.Product, "Product");
            Assert.IsNull(clone.Comment, "Comment");

            source = new ProductInfoHeaderValue("(comment)");
            clone = (ProductInfoHeaderValue)((ICloneable)source).Clone();

            Assert.IsNull(clone.Product, "Product");
            Assert.AreEqual(source.Comment, clone.Comment, "Comment");
        }

        [TestMethod]
        public void GetProductInfoLength_DifferentValidScenarios_AllReturnNonZero()
        {
            ProductInfoHeaderValue result = null;

            CallGetProductInfoLength(" product / 1.0 ", 1, 14, out result);
            Assert.AreEqual(new ProductHeaderValue("product", "1.0"), result.Product, "Product");
            Assert.IsNull(result.Comment);

            CallGetProductInfoLength("p/1.0", 0, 5, out result);
            Assert.AreEqual(new ProductHeaderValue("p", "1.0"), result.Product, "Product");
            Assert.IsNull(result.Comment);

            CallGetProductInfoLength(" (this is a comment)  , ", 1, 21, out result);
            Assert.IsNull(result.Product);
            Assert.AreEqual("(this is a comment)", result.Comment, "Comment");

            CallGetProductInfoLength("(c)", 0, 3, out result);
            Assert.IsNull(result.Product);
            Assert.AreEqual("(c)", result.Comment, "Comment");

            CallGetProductInfoLength("(comment/1.0)[", 0, 13, out result);
            Assert.IsNull(result.Product);
            Assert.AreEqual("(comment/1.0)", result.Comment, "Comment");
        }

        [TestMethod]
        public void GetRangeLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetProductInfoLength(" p/1.0", 0); // no leading whitespaces allowed
            CheckInvalidGetProductInfoLength(" (c)", 0); // no leading whitespaces allowed
            CheckInvalidGetProductInfoLength("(invalid", 0);
            CheckInvalidGetProductInfoLength("product/", 0);
            CheckInvalidGetProductInfoLength("product/(1.0)", 0);

            CheckInvalidGetProductInfoLength("", 0);
            CheckInvalidGetProductInfoLength(null, 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("product", new ProductInfoHeaderValue("product", null));
            CheckValidParse(" product ", new ProductInfoHeaderValue("product", null));

            CheckValidParse(" (comment)   ", new ProductInfoHeaderValue("(comment)"));

            CheckValidParse(" Mozilla/5.0 ", new ProductInfoHeaderValue("Mozilla", "5.0"));
            CheckValidParse(" (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0) ",
                new ProductInfoHeaderValue("(compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)"));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("p/1.0,");
            CheckInvalidParse("p/1.0\r\n"); // for \r\n to be a valid whitespace, it must be followed by space/tab
            CheckInvalidParse("p/1.0(comment)");
            CheckInvalidParse("(comment)[");

            // TODO: Needs a single value parser
            CheckInvalidParse(" Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0) ");
            CheckInvalidParse("p/1.0 =");

            // "User-Agent" and "Server" don't allow empty values (unlike most other headers supporting lists of values)
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("\t");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("product", new ProductInfoHeaderValue("product", null));
            CheckValidTryParse(" product ", new ProductInfoHeaderValue("product", null));

            CheckValidTryParse(" (comment)   ", new ProductInfoHeaderValue("(comment)"));

            CheckValidTryParse(" Mozilla/5.0 ", new ProductInfoHeaderValue("Mozilla", "5.0"));
            CheckValidTryParse(" (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0) ",
                new ProductInfoHeaderValue("(compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)"));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("p/1.0,");
            CheckInvalidTryParse("p/1.0\r\n"); // for \r\n to be a valid whitespace, it must be followed by space/tab
            CheckInvalidTryParse("p/1.0(comment)");
            CheckInvalidTryParse("(comment)[");
            
            // TODO: Needs a single value parser
            CheckInvalidTryParse(" Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0) ");
            CheckInvalidTryParse("p/1.0 =");

            // "User-Agent" and "Server" don't allow empty values (unlike most other headers supporting lists of values)
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("\t");
        }

        #region Helper methods

        private void CheckValidParse(string input, ProductInfoHeaderValue expectedResult)
        {
            ProductInfoHeaderValue result = ProductInfoHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => ProductInfoHeaderValue.Parse(input), input);
        }

        private void CheckValidTryParse(string input, ProductInfoHeaderValue expectedResult)
        {
            ProductInfoHeaderValue result = null;
            Assert.IsTrue(ProductInfoHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            ProductInfoHeaderValue result = null;
            Assert.IsFalse(ProductInfoHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetProductInfoLength(string input, int startIndex, int expectedLength,
            out ProductInfoHeaderValue result)
        {
            Assert.AreEqual(expectedLength, ProductInfoHeaderValue.GetProductInfoLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
        }

        private static void CheckInvalidGetProductInfoLength(string input, int startIndex)
        {
            ProductInfoHeaderValue result = null;
            Assert.AreEqual(0, ProductInfoHeaderValue.GetProductInfoLength(input, startIndex, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
