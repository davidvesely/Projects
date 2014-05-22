// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.IO;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(MultipartMemoryStreamProvider))]
    public class MultipartMemoryStreamProviderTests : UnitTest
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartMemoryStreamProvider is internal type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsClass,
                typeof(IMultipartStreamProvider));
        }
        #endregion

        #region Members
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MultipartMemoryStreamProvider default ctor.")]
        public void DefaultConstructor()
        {
            MultipartMemoryStreamProvider instance = MultipartMemoryStreamProvider.Instance;
            Assert.IsNotNull(instance, "instance should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) throws on null.")]
        public void GetStreamThrowsOnNull()
        {
            MultipartMemoryStreamProvider instance = MultipartMemoryStreamProvider.Instance;
            Asserters.Exception.ThrowsArgumentNull("headers", () => { instance.GetStream(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetStream(HttpContentHeaders) throws on no Content-Disposition header.")]
        public void GetStreamReturnsMemoryStream()
        {
            MultipartMemoryStreamProvider instance = MultipartMemoryStreamProvider.Instance;
            HttpContent content = new StringContent("text");
            
            Stream stream = instance.GetStream(content.Headers);
            Assert.IsNotNull(stream, "GetStream should return non-null.");
            
            MemoryStream memStream = stream as MemoryStream;
            Assert.IsNotNull(stream, "GetStream should return a memory stream.");

            Assert.AreEqual(0, stream.Length, "GetStream should return empty MemoryStream.");
            Assert.AreEqual(0, stream.Position, "GetStream should return MemoryStream at the start position.");

            Assert.AreNotSame(memStream, instance.GetStream(content.Headers), "GetStream should not return the same MemoryStream instance twice.");
        }
        #endregion
    }
}
