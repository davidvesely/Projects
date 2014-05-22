// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.IO;
    using System.Linq;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Http.Headers;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class MultipartFormDataStreamProviderTests : UnitTest<MultipartFormDataStreamProvider>
    {
        const int defaultBufferSize = 0x1000;
        const string validPath = @"c:\some\path";

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFormDataStreamProvider is public, visible type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass,
                typeof(IMultipartStreamProvider));
        }
        #endregion

        #region Constructors
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFormDataStreamProvider default ctor.")]
        public void DefaultConstructor()
        {
            MultipartFormDataStreamProvider instance = new MultipartFormDataStreamProvider();
            Assert.IsNotNull(instance, "instance should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFormDataStreamProvider ctor with invalid root paths.")]
        public void ConstructorInvalidRootPath()
        {
            Asserters.Exception.ThrowsArgumentNull("rootPath", () => { new MultipartFormDataStreamProvider(null); });

            foreach (string path in TestData.NotSupportedFilePaths)
            {
                Asserters.Exception.Throws<NotSupportedException>(
                    () => { new MultipartFormDataStreamProvider(path, defaultBufferSize); },
                    (exception) => { });
            }

            foreach (string path in TestData.InvalidNonNullFilePaths)
            {
                // Note: Path.GetFileName doesn't set the argument name when throwing.
                Asserters.Exception.ThrowsArgument(null, () => { new MultipartFormDataStreamProvider(path, defaultBufferSize); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFormDataStreamProvider ctor with null path.")]
        public void ConstructorInvalidBufferSize()
        {
            Asserters.Exception.ThrowsArgumentOutOfRange("bufferSize", () => { new MultipartFormDataStreamProvider(validPath, -1); });
            Asserters.Exception.ThrowsArgumentOutOfRange("bufferSize", () => { new MultipartFormDataStreamProvider(validPath, 0); });
        }
        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("BodyPartFileNames empty.")]
        public void EmptyBodyPartFileNames()
        {
            MultipartFormDataStreamProvider instance = new MultipartFormDataStreamProvider();
            Assert.IsNotNull(instance.BodyPartFileNames);
            Assert.AreEqual(0, instance.BodyPartFileNames.Count);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) throws on null.")]
        public void GetStreamThrowsOnNull()
        {
            MultipartFormDataStreamProvider instance = new MultipartFormDataStreamProvider();
            Asserters.Exception.ThrowsArgumentNull("headers", () => { instance.GetStream(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) throws on no Content-Disposition header.")]
        public void GetStreamThrowsOnNoContentDisposition()
        {
            MultipartFormDataStreamProvider instance = new MultipartFormDataStreamProvider();
            HttpContent content = new StringContent("text");
            Asserters.Exception.Throws<IOException>(
                SR.MultipartFormDataStreamProviderNoContentDisposition("Content-Disposition"),
                () => { instance.GetStream(content.Headers); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) validation.")]
        public void GetStreamValidation()
        {
            Stream stream0 = null;
            Stream stream1 = null;

            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent("Not a file"), "notafile");
                content.Add(new StringContent("This is a file"), "file", "filename");

                MultipartFormDataStreamProvider instance = new MultipartFormDataStreamProvider();
                stream0 = instance.GetStream(content.ElementAt(0).Headers);
                Assert.IsInstanceOfType(stream0, typeof(MemoryStream), "GetSteam should return a MemoryStream");
                stream1 = instance.GetStream(content.ElementAt(1).Headers);
                Assert.IsInstanceOfType(stream1, typeof(FileStream), "GetSteam should return a FileStream");

                Assert.AreEqual(1, instance.BodyPartFileNames.Count, "There should be one element in the file list.");
                Assert.AreEqual(content.ElementAt(1).Headers.ContentDisposition.FileName, instance.BodyPartFileNames.Keys.ElementAt(0));
            }
            finally
            {
                if (stream0 != null)
                {
                    stream0.Close();
                }

                if (stream1 != null)
                {
                    stream1.Close();
                }
            }
        }

        #endregion
    }
}
