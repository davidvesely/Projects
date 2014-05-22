using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class ByteArrayContentTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullSourceArray_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(null);
        }

        [TestMethod]
        public void Ctor_EmptySourceArray_Succeed()
        {
            // No exception should be thrown for empty arrays.
            ByteArrayContent content = new ByteArrayContent(new byte[0]);
            Assert.AreEqual(0, content.ReadAsStreamAsync().Result.Length, "Length");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullSourceArrayWithRange_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(null, 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_EmptySourceArrayWithRange_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[0], 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_StartIndexTooBig_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[5], 5, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_StartIndexNegative_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[5], -1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_LengthTooBig_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[5], 1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_LengthPlusOffsetCauseIntOverflow_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[5], 1, int.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_LengthNegative_Throw()
        {
            ByteArrayContent content = new ByteArrayContent(new byte[5], 0, -1);
        }

        [TestMethod]
        public void ContentLength_UseWholeSourceArray_LengthMatchesArrayLength()
        {
            byte[] contentData = new byte[10];
            ByteArrayContent content = new ByteArrayContent(contentData);

            Assert.AreEqual(contentData.Length, content.Headers.ContentLength, "Expected 'Content-Length' value.");
        }

        [TestMethod]
        public void ContentLength_UsePartialSourceArray_LengthMatchesArrayLength()
        {
            byte[] contentData = new byte[10];
            ByteArrayContent content = new ByteArrayContent(contentData, 5, 3);

            Assert.AreEqual(3, content.Headers.ContentLength, "Expected 'Content-Length' value.");
        }

        [TestMethod]
        public void ContentReadStream_CallProperty_MemoryStreamWrappingByteArrayReturned()
        {
            byte[] contentData = new byte[10];
            MockByteArrayContent content = new MockByteArrayContent(contentData, 5, 3);

            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "ContentReadStream should not be writable.");
            Assert.AreEqual(3, content.ReadAsStreamAsync().Result.Length, 
                "ContentReadStream length doesn't match segment length.");
            Assert.AreEqual(0, content.CopyToCount, "Getting ContentReadStream should not call CopyTo().");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyTo_NullDestination_Throw()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData);

            content.CopyTo(null);
        }

        [TestMethod]
        public void CopyTo_UseWholeSourceArray_WholeContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData);

            MemoryStream destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(contentData.Length, destination.Length, "Expected length of destination stream.");
            CheckResult(destination, 0);
        }

        [TestMethod]
        public void CopyTo_UsePartialSourceArray_PartialContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData, 3, 5);

            MemoryStream destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(5, destination.Length, "Expected length of destination stream.");
            CheckResult(destination, 3);
        }

        [TestMethod]
        public void CopyTo_UseEmptySourceArray_NothingCopied()
        {
            byte[] contentData = new byte[0];
            ByteArrayContent content = new ByteArrayContent(contentData);

            MemoryStream destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(0, destination.Length, "Expected length of destination stream.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyToAsync_NullDestination_Throw()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData);

            Task t = content.CopyToAsync(null);
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_UseWholeSourceArray_WholeContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData);

            MemoryStream destination = new MemoryStream();
            Task t = content.CopyToAsync(destination);
            t.Wait();

            Assert.AreEqual(contentData.Length, destination.Length, "Expected length of destination stream.");
            CheckResult(destination, 0);
        }

        [TestMethod]
        public void CopyToAsync_UsePartialSourceArray_PartialContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            ByteArrayContent content = new ByteArrayContent(contentData, 3, 5);

            MemoryStream destination = new MemoryStream();
            Task t = content.CopyToAsync(destination);
            t.Wait();

            Assert.AreEqual(5, destination.Length, "Expected length of destination stream.");
            CheckResult(destination, 3);
        }

        [TestMethod]
        public void CopyToAsync_UseEmptySourceArray_NothingCopied()
        {
            byte[] contentData = new byte[0];
            ByteArrayContent content = new ByteArrayContent(contentData, 0, 0);

            MemoryStream destination = new MemoryStream();
            Task t = content.CopyToAsync(destination);
            t.Wait();

            Assert.AreEqual(0, destination.Length, "Expected length of destination stream.");
        }

        #region Helper methods

        private static byte[] CreateSourceArray()
        {
            byte[] contentData = new byte[10];
            for (int i = 0; i < contentData.Length; i++)
            {
                contentData[i] = (byte)(i % 256);
            }
            return contentData;
        }

        private static void CheckResult(Stream destination, byte firstValue)
        {
            destination.Position = 0;
            byte[] destinationData = new byte[destination.Length];
            int read = destination.Read(destinationData, 0, destinationData.Length);

            Assert.AreEqual(destinationData.Length, read, "Expected amount of copied data.");
            Assert.AreEqual(firstValue, destinationData[0], 
                "First value in destination stream doesn't match expectations.");

            for (int i = 1; i < read; i++)
            {
                Assert.IsTrue((destinationData[i] == (destinationData[i - 1] + 1)) ||
                    ((destinationData[i] == 0) && (destinationData[i - 1] != 0)),
                    "Order of values incorrect: [i-1]: {0}, [i]: {1}", destinationData[i - 1].ToString(),
                    destinationData[i].ToString());
            }
        }

        private class MockByteArrayContent : ByteArrayContent
        {
            public int CopyToCount { get; private set; }

            public MockByteArrayContent(byte[] content, int offset, int count)
                : base(content, offset, count)
            {
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                CopyToCount++;
                return base.CopyToAsync(stream, context);
            }
        }

        #endregion
    }
}
