// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System.Linq;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(MediaTypeFormatterExtensionMethods))]
    public class MediaTypeFormatterExtensionMethodsTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterExtensionMethods is public and static.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Methods

        #region AddQueryStringMapping

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddQueryStringMapping(MediaTypeFormatter, string, string, MediaTypeHeaderValue) throws for null 'this'.")]
        public void AddQueryStringMappingThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddQueryStringMapping("name", "value", new MediaTypeHeaderValue("application/xml")));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddQueryStringMapping(MediaTypeFormatter, string, string, string) throws for null 'this'.")]
        public void AddQueryStringMapping1ThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddQueryStringMapping("name", "value", "application/xml"));
        }

        #endregion AddQueryStringMapping

        #region AddUriPathExtensionMapping

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddUriPathExtensionMapping(MediaTypeFormatter, string, MediaTypeHeaderValue) throws for null 'this'.")]
        public void AddUriPathExtensionMappingThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddUriPathExtensionMapping("xml", new MediaTypeHeaderValue("application/xml")));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddUriPathExtensionMapping(MediaTypeFormatter, string, string) throws for null 'this'.")]
        public void AddUriPathExtensionMapping1ThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddUriPathExtensionMapping("xml", "application/xml"));
        }

        #endregion AddUriPathExtensionMapping

        #region AddMediaRangeMapping

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddMediaRangeMapping(MediaTypeFormatter, MediaTypeHeaderValue, MediaTypeHeaderValue) throws for null 'this'.")]
        public void AddMediaRangeMappingThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddMediaRangeMapping(new MediaTypeHeaderValue("application/*"), new MediaTypeHeaderValue("application/xml")));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddMediaRangeMapping(MediaTypeFormatter, string, string) throws for null 'this'.")]
        public void AddMediaRangeMapping1ThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddMediaRangeMapping("application/*", "application/xml"));
        }

        #endregion AddMediaRangeMapping

        #region AddRequestHeaderMapping

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddRequestHeaderMapping(MediaTypeFormatter, string, string, StringComparison, bool, MediaTypeHeaderValue) throws for null 'this'.")]
        public void AddRequestHeaderMappingThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddRequestHeaderMapping("name", "value", StringComparison.CurrentCulture, true, new MediaTypeHeaderValue("application/xml")));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddRequestHeaderMapping(MediaTypeFormatter, string, string, StringComparison, bool, MediaTypeHeaderValue) adds formatter on 'this'.")]
        public void AddRequestHeaderMappingAddsSuccessfully()
        {
            MediaTypeFormatter formatter = new TestMediaTypeFormatter();
            Assert.AreEqual(0, formatter.MediaTypeMappings.Count, "Expected 0 media type mappings.");
            formatter.AddRequestHeaderMapping("name", "value", StringComparison.CurrentCulture, true, new MediaTypeHeaderValue("application/xml"));
            IEnumerable<RequestHeaderMapping> mappings = formatter.MediaTypeMappings.OfType<RequestHeaderMapping>();
            Assert.AreEqual(1, mappings.Count(), "Expected 1 RequestHeader mapping");
            RequestHeaderMapping mapping = mappings.ElementAt(0);
            Assert.AreEqual("name", mapping.HeaderName, "Expected header name didn't match.");
            Assert.AreEqual("value", mapping.HeaderValue, "Expected header value didn't match.");
            Assert.AreEqual(StringComparison.CurrentCulture, mapping.HeaderValueComparison, "Expected comparison mode didn't match");
            Assert.AreEqual(true, mapping.IsValueSubstring, "Expected substring value didn't match");
            Assert.AreEqual(new MediaTypeHeaderValue("application/xml"), mapping.MediaType, "Expected media type value didn't match");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddRequestHeaderMapping(MediaTypeFormatter, string, string, StringComparison, bool, string) throws for null 'this'.")]
        public void AddRequestHeaderMapping1ThrowsWithNullThis()
        {
            MediaTypeFormatter formatter = null;
            Asserters.Exception.ThrowsArgumentNull("formatter", () => formatter.AddRequestHeaderMapping("name", "value", StringComparison.CurrentCulture, true, "application/xml"));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("AddRequestHeaderMapping(MediaTypeFormatter, string, string, StringComparison, bool, string) adds formatter on 'this'.")]
        public void AddRequestHeaderMapping1AddsSuccessfully()
        {
            MediaTypeFormatter formatter = new TestMediaTypeFormatter();
            Assert.AreEqual(0, formatter.MediaTypeMappings.Count, "Expected 0 media type mappings.");
            formatter.AddRequestHeaderMapping("name", "value", StringComparison.CurrentCulture, true, "application/xml");
            IEnumerable<RequestHeaderMapping> mappings = formatter.MediaTypeMappings.OfType<RequestHeaderMapping>();
            Assert.AreEqual(1, mappings.Count(), "Expected 1 RequestHeader mapping");
            RequestHeaderMapping mapping = mappings.ElementAt(0);
            Assert.AreEqual("name", mapping.HeaderName, "Expected header name didn't match.");
            Assert.AreEqual("value", mapping.HeaderValue, "Expected header value didn't match.");
            Assert.AreEqual(StringComparison.CurrentCulture, mapping.HeaderValueComparison, "Expected comparison mode didn't match");
            Assert.AreEqual(true, mapping.IsValueSubstring, "Expected substring value didn't match");
            Assert.AreEqual(new MediaTypeHeaderValue("application/xml"), mapping.MediaType, "Expected media type value didn't match");
        }

        #endregion AddRequestHeaderMapping

        #endregion Methods

        internal class TestMediaTypeFormatter : MediaTypeFormatter
        {
            protected override object OnReadFromStream(Type type, IO.Stream stream, HttpContentHeaders contentHeaders)
            {
                throw new NotImplementedException();
            }

            protected override void OnWriteToStream(Type type, object value, IO.Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
