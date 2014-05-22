// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(MediaTypeConstants))]
    public class MediaTypeConstantsTests : UnitTest
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Class is internal static type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsStatic);
        }
        #endregion

        #region Helpers
        private static void ValidateClones(MediaTypeHeaderValue clone1, MediaTypeHeaderValue clone2, string charset)
        {
            Assert.IsNotNull(clone1, "clone should not be null");
            Assert.IsNotNull(clone2, "clone should not be null");
            Assert.AreNotSame(clone1, clone2, "Should not return the same instance twice");
            Assert.AreEqual(clone1.MediaType, clone2.MediaType, "Media types should be identical");
            Assert.AreEqual(charset, clone1.CharSet, "Unexpected charset.");
            Assert.AreEqual(charset, clone2.CharSet, "Unexpected charset.");
        }

        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HtmlMediaType returns clone")]
        public void HtmlMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.HtmlMediaType, MediaTypeConstants.HtmlMediaType, Encoding.UTF8.WebName);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ApplicationXmlMediaType returns clone")]
        public void ApplicationXmlMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.ApplicationXmlMediaType, MediaTypeConstants.ApplicationXmlMediaType, null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ApplicationJsonMediaType returns clone")]
        public void ApplicationJsonMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.ApplicationJsonMediaType, MediaTypeConstants.ApplicationJsonMediaType, null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TextXmlMediaType returns clone")]
        public void TextXmlMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.TextXmlMediaType, MediaTypeConstants.TextXmlMediaType, null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TextJsonMediaType returns clone")]
        public void TextJsonMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.TextJsonMediaType, MediaTypeConstants.TextJsonMediaType, null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ApplicationFormUrlEncodedMediaType returns clone")]
        public void ApplicationFormUrlEncodedMediaTypeReturnsClone()
        {
            ValidateClones(MediaTypeConstants.ApplicationFormUrlEncodedMediaType, MediaTypeConstants.ApplicationFormUrlEncodedMediaType, null);
        }

        #endregion
    }
}
