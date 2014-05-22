// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Net.Http.Formatting;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(HttpContentExtensionMethods))]
    public class HttpContentExtensionMethodsTests : UnitTest
    {
        private const string helloString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><string>hello</string>";

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpContentExtensionMethods is public and static.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Methods

        #region ReadAsAsync(HttpContent, Type)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type) throws with null 'this'.")]
        public void ReadAsAsyncThrowsWithNullThis()
        {
            StringContent content = null;
            Asserters.Exception.ThrowsArgumentNull("content", () => content.ReadAsAsync(typeof(string)));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type) throws with null Type.")]
        public void ReadAsAsyncThrowsWithNullType()
        {
            StringContent content = new StringContent(string.Empty);
            Asserters.Exception.ThrowsArgumentNull("type", () => content.ReadAsAsync(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type) works with string content.")]
        public void ReadAsAsyncString()
        {
            StringContent content = new StringContent(helloString, new Text.UTF8Encoding(), "application/xml");
            string result = (string)content.ReadAsAsync(typeof(string)).Result;
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type) works with object content.")]
        public void ReadAsAsyncWithObjectContent()
        {
            ObjectContent content = new ObjectContent(typeof(string), "hello");
            string result = (string)content.ReadAsAsync(typeof(string)).Result;
            Assert.AreEqual("hello", result);
        }

        #endregion ReadAsAsync(HttpContent, Type)

        #region ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null 'this'.")]
        public void ReadAsAsync1ThrowsWithNullThis()
        {
            StringContent content = null;
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[0];
            Asserters.Exception.ThrowsArgumentNull("content", () => content.ReadAsAsync(typeof(string), formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null Type.")]
        public void ReadAsAsync1ThrowsWithNullType()
        {
            StringContent content = new StringContent(string.Empty);
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[0];
            Asserters.Exception.ThrowsArgumentNull("type", () => content.ReadAsAsync(null, formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null Formatters.")]
        public void ReadAsAsync1ThrowsWithNullFormatters()
        {
            StringContent content = new StringContent(string.Empty);
            MediaTypeFormatter[] formatters = null;
            Asserters.Exception.ThrowsArgumentNull("formatters", () => content.ReadAsAsync(typeof(string), formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) works with string content.")]
        public void ReadAsAsync1String()
        {
            StringContent content = new StringContent(helloString, new Text.UTF8Encoding(), "application/xml");
            string result = (string)content.ReadAsAsync(typeof(string), new MediaTypeFormatterCollection()).Result;
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) works with object content.")]
        public void ReadAsAsync1WithObjectContent()
        {
            ObjectContent content = new ObjectContent(typeof(string), "hello");
            string result = (string)content.ReadAsAsync(typeof(string), new MediaTypeFormatterCollection()).Result;
            Assert.AreEqual("hello", result);
        }


        #endregion ReadAsAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>)

        #region ReadAsOrDefaultAsync(HttpContent, Type)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type) throws with null 'this'.")]
        public void ReadAsOrDefaultAsyncThrowsWithNullThis()
        {
            StringContent content = null;
            Asserters.Exception.ThrowsArgumentNull("content", () => content.ReadAsOrDefaultAsync(typeof(string)));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type) throws with null Type.")]
        public void ReadAsOrDefaultAsyncThrowsWithNullType()
        {
            StringContent content = new StringContent(string.Empty);
            Asserters.Exception.ThrowsArgumentNull("type", () => content.ReadAsOrDefaultAsync(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type) works with string content.")]
        public void ReadAsOrDefaultAsyncString()
        {
            StringContent content = new StringContent(helloString, new Text.UTF8Encoding(), "application/xml");
            string result = (string)content.ReadAsOrDefaultAsync(typeof(string)).Result;
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type) works with object content.")]
        public void ReadAsOrDefaultAsyncWithObjectContent()
        {
            ObjectContent content = new ObjectContent(typeof(string), "hello");
            string result = (string)content.ReadAsOrDefaultAsync(typeof(string)).Result;
            Assert.AreEqual("hello", result);
        }

        #endregion ReadAsOrDefaultAsync(HttpContent, Type)

        #region ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null 'this'.")]
        public void ReadAsOrDefaultAsync1ThrowsWithNullThis()
        {
            StringContent content = null;
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[0];
            Asserters.Exception.ThrowsArgumentNull("content", () => content.ReadAsOrDefaultAsync(typeof(string), formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null Type.")]
        public void ReadAsOrDefaultAsync1ThrowsWithNullType()
        {
            StringContent content = new StringContent(string.Empty);
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[0];
            Asserters.Exception.ThrowsArgumentNull("type", () => content.ReadAsOrDefaultAsync(null, formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) throws with null Formatters.")]
        public void ReadAsOrDefaultAsync1ThrowsWithNullFormatters()
        {
            StringContent content = new StringContent(string.Empty);
            MediaTypeFormatter[] formatters = null;
            Asserters.Exception.ThrowsArgumentNull("formatters", () => content.ReadAsOrDefaultAsync(typeof(string), formatters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) works with string content.")]
        public void ReadAsOrDefault1AsyncString()
        {
            StringContent content = new StringContent(helloString, new Text.UTF8Encoding(), "application/xml");
            string result = (string)content.ReadAsOrDefaultAsync(typeof(string), new MediaTypeFormatterCollection()).Result;
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>) works with object content.")]
        public void ReadAsOrDefault1AsyncWithObjectContent()
        {
            ObjectContent content = new ObjectContent(typeof(string), "hello");
            string result = (string)content.ReadAsOrDefaultAsync(typeof(string), new MediaTypeFormatterCollection()).Result;
            Assert.AreEqual("hello", result);
        }

        #endregion ReadAsOrDefaultAsync(HttpContent, Type, IEnumerable<MediaTypeFormatter>)

        #endregion Methods
    }
}
