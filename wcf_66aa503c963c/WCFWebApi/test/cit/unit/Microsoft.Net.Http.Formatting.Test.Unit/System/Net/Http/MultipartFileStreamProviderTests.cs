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
    public class MultipartFileStreamProviderTests : UnitTest<MultipartFileStreamProvider>
    {
        const int defaultBufferSize = 0x1000;
        const string validPath = @"c:\some\path";

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFileStreamProvider is public, visible type.")]
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
        [Description("MultipartFileStreamProvider default ctor.")]
        public void DefaultConstructor()
        {
            MultipartFileStreamProvider instance = new MultipartFileStreamProvider();
            Assert.IsNotNull(instance, "instance should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFileStreamProvider ctor with invalid root paths.")]
        public void ConstructorInvalidRootPath()
        {
            Asserters.Exception.ThrowsArgumentNull("rootPath", () => { new MultipartFileStreamProvider(null); });

            foreach (string path in TestData.NotSupportedFilePaths)
            {
                Asserters.Exception.Throws<NotSupportedException>(
                    () => { new MultipartFileStreamProvider(path, defaultBufferSize); },
                    (exception) => { });
            }

            foreach (string path in TestData.InvalidNonNullFilePaths)
            {
                // Note: Path.GetFileName doesn't set the argument name when throwing.
                Asserters.Exception.ThrowsArgument(null, () => { new MultipartFileStreamProvider(path, defaultBufferSize); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartFileStreamProvider ctor with null path.")]
        public void ConstructorInvalidBufferSize()
        {
            Asserters.Exception.ThrowsArgumentOutOfRange("bufferSize", () => { new MultipartFileStreamProvider(validPath, -1); });
            Asserters.Exception.ThrowsArgumentOutOfRange("bufferSize", () => { new MultipartFileStreamProvider(validPath, 0); });
        }
        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("BodyPartFileNames empty.")]
        public void EmptyBodyPartFileNames()
        {
            MultipartFileStreamProvider instance = new MultipartFileStreamProvider();
            Assert.IsNotNull(instance.BodyPartFileNames);
            Assert.AreEqual(0, instance.BodyPartFileNames.Count());
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) throws on null.")]
        public void GetStreamThrowsOnNull()
        {
            MultipartFileStreamProvider instance = new MultipartFileStreamProvider();
            Asserters.Exception.ThrowsArgumentNull("headers", () => { instance.GetStream(null); });
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

                MultipartFileStreamProvider instance = new MultipartFileStreamProvider();
                stream0 = instance.GetStream(content.ElementAt(0).Headers);
                Assert.IsInstanceOfType(stream0, typeof(FileStream), "GetSteam should return a FileStream");
                stream1 = instance.GetStream(content.ElementAt(1).Headers);
                Assert.IsInstanceOfType(stream1, typeof(FileStream), "GetSteam should return a FileStream");

                Assert.AreEqual(2, instance.BodyPartFileNames.Count(), "There should be two elements in the file list.");
                Asserters.String.Contains(instance.BodyPartFileNames.ElementAt(0), "BodyPart", "Filename should contain with 'BodyPart'");
                Asserters.String.EndsWith(instance.BodyPartFileNames.ElementAt(1), "filename", "Filename should end with 'filename'");
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
