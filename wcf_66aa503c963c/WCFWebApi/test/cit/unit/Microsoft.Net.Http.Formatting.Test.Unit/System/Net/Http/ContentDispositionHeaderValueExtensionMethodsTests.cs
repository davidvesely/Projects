// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Collections.Generic;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Http.Headers;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(ContentDispositionHeaderValueExtensionMethods))]
    public class ContentDispositionHeaderValueExtensionMethodsTests : UnitTest
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ContentDispositionHeaderValueExtensionMethods is internal static type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsStatic);
        }
        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExtractLocalFileName throws on null")]
        public void ExtractLocalFileNameThrowsOnNull()
        {
            ContentDispositionHeaderValue test = null;
            Asserters.Exception.ThrowsArgumentNull("contentDisposition",
                () =>
                {
                    ContentDispositionHeaderValueExtensionMethods.ExtractLocalFileName(test);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExtractLocalFileName throws on quoted empty strings")]
        public void ExtractLocalFileNameThrowsOnQuotedEmpty()
        {
            foreach (var empty in TestData.NonNullEmptyStrings)
            {
                Asserters.Exception.ThrowsArgument("contentDisposition",
                    () =>
                    {
                        ContentDispositionHeaderValue contentDisposition = null;
                        ContentDispositionHeaderValue.TryParse(string.Format("formdata; filename=\"{0}\"", empty), out contentDisposition);
                        Assert.IsNotNull(contentDisposition.FileName, "Filename should not be null");
                        ContentDispositionHeaderValueExtensionMethods.ExtractLocalFileName(contentDisposition);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExtractLocalFileName picks filename* over filename.")]
        public void ExtractLocalFileNamePicksFileNameStarFirst()
        {
            ContentDispositionHeaderValue contentDisposition = null;
            ContentDispositionHeaderValue.TryParse("formdata; filename=\"aaa\"; filename*=utf-8''%e2BBB", out contentDisposition);
            string localFilename = ContentDispositionHeaderValueExtensionMethods.ExtractLocalFileName(contentDisposition);
            Assert.AreEqual("�BBB", localFilename, "ExtractLocalFileName did not pick expected value");
        }

        #endregion
    }
}
