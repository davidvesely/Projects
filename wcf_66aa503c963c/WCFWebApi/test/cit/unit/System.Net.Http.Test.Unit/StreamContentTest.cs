using System.IO;
using System.Net.Test.Common.Logging;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class StreamContentTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullStream_Throw()
        {
            StreamContent content = new StreamContent(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_ZeroBufferSize_Throw()
        {
            StreamContent content = new StreamContent(new MemoryStream(), 0);
        }

        [TestMethod]
        public void ContentLength_SetStreamSupportingSeeking_StreamLengthMatchesHeaderValue()
        {
            MockStream source = new MockStream(new byte[10], true, true); // supports seeking.
            StreamContent content = new StreamContent(source);

            Assert.AreEqual(source.Length, content.Headers.ContentLength, "Expected 'Content-Length' value.");
        }

        [TestMethod]
        public void ContentLength_SetStreamNotSupportingSeeking_NullReturned()
        {
            MockStream source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            StreamContent content = new StreamContent(source);

            Assert.IsNull(content.Headers.ContentLength,
                "No 'Content-Length' expected for stream that doesn't support seeking.");
        }

        [TestMethod]
        public void Dispose_UseMockStreamSourceAndDisposeContent_MockStreamGotDisposed()
        {
            MockStream source = new MockStream(new byte[10]);
            StreamContent content = new StreamContent(source);
            content.Dispose();

            Assert.AreEqual(1, source.DisposeCount, "No. of times Dispose(bool) was called.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyTo_NullDestination_Throw()
        {
            MockStream source = new MockStream(new byte[10]);
            StreamContent content = new StreamContent(source);
            content.CopyTo(null);
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamSupportingSeeking_ContentIsSerializedMultipleTimes()
        {
            MockStream source = new MockStream(new byte[10], true, true); // supports seeking.
            StreamContent content = new StreamContent(source);

            MemoryStream destination1 = new MemoryStream();
            content.CopyTo(destination1);
            Assert.AreEqual(source.Length, destination1.Length, "Length of first destination stream.");

            MemoryStream destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(source.Length, destination2.Length, "Length of second destination stream.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyTo_CallMultipleTimesWithStreamNotSupportingSeeking_Throw()
        {
            MockStream source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            StreamContent content = new StreamContent(source);

            MemoryStream destination1 = new MemoryStream();
            try
            {
                content.CopyTo(destination1);
            }
            catch (Exception e)
            {
                Assert.Fail("No exception expected, but got: {0}", e.ToString());
            }
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable)
            Assert.AreEqual(10, destination1.Length, "Length of first destination stream.");

            MemoryStream destination2 = new MemoryStream();
            content.CopyTo(destination2); // This will throw since stream can't seek.
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamNotSupportingSeekingButBufferedStream_ContentSerializedOnceToBuffer()
        {
            MockStream source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            StreamContent content = new StreamContent(source);

            // After loading the content into a buffer, we should be able to copy the content to a destination stream
            // multiple times, even though the stream doesn't support seeking.
            content.LoadIntoBufferAsync().Wait();

            MemoryStream destination1 = new MemoryStream();
            content.CopyTo(destination1);
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable)
            Assert.AreEqual(10, destination1.Length, "Length of first destination stream.");

            MemoryStream destination2 = new MemoryStream();
            content.CopyTo(destination2); // This will NOT throw since stream is buffered
            Assert.AreEqual(10, destination2.Length, "Length of second destination stream.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyToAsync_NullDestination_Throw()
        {
            MockStream source = new MockStream(new byte[10]);
            StreamContent content = new StreamContent(source);
            Task t = content.CopyToAsync(null);
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_CallMultipleTimesWithStreamSupportingSeeking_ContentIsSerializedMultipleTimes()
        {
            MockStream source = new MockStream(new byte[10], true, true); // supports seeking.
            StreamContent content = new StreamContent(source);

            MemoryStream destination1 = new MemoryStream();
            Task t1 = content.CopyToAsync(destination1);
            t1.Wait();
            Assert.AreEqual(source.Length, destination1.Length, "Length of first destination stream.");

            MemoryStream destination2 = new MemoryStream();
            Task t2 = content.CopyToAsync(destination2);
            t2.Wait();
            Assert.AreEqual(source.Length, destination2.Length, "Length of second destination stream.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyToAsync_CallMultipleTimesWithStreamNotSupportingSeeking_Throw()
        {
            MockStream source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            StreamContent content = new StreamContent(source);

            MemoryStream destination1 = new MemoryStream();
            try
            {
                Task t1 = content.CopyToAsync(destination1);
                t1.Wait();
            }
            catch (Exception e)
            {
                Assert.Fail("No exception expected, but got: {0}", e.ToString());
            }
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable)
            Assert.AreEqual(10, destination1.Length, "Length of first destination stream.");

            // Note that the InvalidOperationException is thrown in CopyToAsync(). It is not thrown inside the task.
            MemoryStream destination2 = new MemoryStream();
            Task t2 = content.CopyToAsync(destination2);
            t2.Wait();
        }

        [TestMethod]
        public void CopyToAsync_CallMultipleTimesWithStreamNotSupportingSeekingButBufferedStream_ContentSerializedOnceToBuffer()
        {
            MockStream source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            StreamContent content = new StreamContent(source);

            // After loading the content into a buffer, we should be able to copy the content to a destination stream
            // multiple times, even though the stream doesn't support seeking.
            content.LoadIntoBufferAsync().Wait();

            MemoryStream destination1 = new MemoryStream();
            Task t1 = content.CopyToAsync(destination1);
            t1.Wait();
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable)
            Assert.AreEqual(10, destination1.Length, "Length of first destination stream.");

            MemoryStream destination2 = new MemoryStream();
            Task t2 = content.CopyToAsync(destination2);
            t2.Wait();
            Assert.AreEqual(10, destination2.Length, "Length of second destination stream.");
        }

        [TestMethod]
        public void ContentReadStream_GetProperty_ReturnOriginalStream()
        {
            MockStream source = new MockStream(new byte[10]);
            StreamContent content = new StreamContent(source);

            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "ContentReadStream should not be writable.");
            Assert.AreEqual(source.Length, content.ReadAsStreamAsync().Result.Length,
                "ContentReadStream length doesn't match stream length.");
            Assert.AreEqual(0, source.ReadCount,
                "Getting ContentReadStream should not require read operations on the stream.");
            Assert.AreNotSame(source, content.ReadAsStreamAsync().Result,
                "Expected ContentReadStream to be different than original stream.");
        }

        [TestMethod]
        public void ContentReadStream_CheckResultProperties_ValuesRepresentReadOnlyStream()
        {
            byte[] data = new byte[10];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            MockStream source = new MockStream(data);

            StreamContent content = new StreamContent(source);
            Stream contentReadStream = content.ReadAsStreamAsync().Result;

            // The following checks verify that the stream returned passes all read-related properties to the 
            // underlying MockStream and throws when using write-related members.

            Assert.IsFalse(contentReadStream.CanWrite, "CanWrite");
            Assert.IsTrue(contentReadStream.CanRead, "CanRead");
            Assert.AreEqual(source.Length, contentReadStream.Length, "Length");

            Assert.AreEqual(0, source.CanSeekCount, "No calls to CanSeek() expected.");
            Log.Info(contentReadStream.CanSeek.ToString());
            Assert.AreEqual(1, source.CanSeekCount, "Calls to CanSeek() expected.");

            contentReadStream.Position = 3; // no exception
            Assert.AreEqual(3, contentReadStream.Position, "Position");
          
            byte byteOnIndex3 = (byte)contentReadStream.ReadByte();
            Assert.AreEqual(data[3], byteOnIndex3, "Byte value on current position.");

            byte[] byteOnIndex4 = new byte[1];
            IAsyncResult ar = contentReadStream.BeginRead(byteOnIndex4, 0, 1, null, null);
            Assert.AreEqual(1, contentReadStream.EndRead(ar), "Expected 1 byte to be read.");
            Assert.AreEqual(data[4], byteOnIndex4[0], "Byte value on index 4.");

            byte[] byteOnIndex5 = new byte[1];
            Assert.AreEqual(1, contentReadStream.Read(byteOnIndex5, 0, 1), "Expected 1 byte to be read.");
            Assert.AreEqual(data[5], byteOnIndex5[0], "Byte value on index 5.");

            contentReadStream.ReadTimeout = 123;
            Assert.AreEqual(123, source.ReadTimeout, "ReadTimeout not written to source stream.");
            Assert.AreEqual(123, contentReadStream.ReadTimeout, "ReadTimeout not retrieved from source stream.");

            Assert.AreEqual(0, source.CanTimeoutCount, "No calls to CanTimeout() expected.");
            Log.Info(contentReadStream.CanTimeout.ToString());
            Assert.AreEqual(1, source.CanTimeoutCount, "Calls to CanTimeout() expected.");

            Assert.AreEqual(0, source.SeekCount, "No calls to Seek() expected.");
            contentReadStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(1, source.SeekCount, "Calls to Seek() expected.");

            CheckException(() => { contentReadStream.WriteTimeout = 5; }, typeof(NotSupportedException));
            CheckException(() => { Log.Info(contentReadStream.WriteTimeout.ToString()); },
                typeof(NotSupportedException));
            CheckException(() => { contentReadStream.Flush(); }, typeof(NotSupportedException));
            CheckException(() => { contentReadStream.SetLength(1); }, typeof(NotSupportedException));
            CheckException(() => { contentReadStream.BeginWrite(null, 0, 0, null, null); },
                typeof(NotSupportedException));
            CheckException(() => { contentReadStream.EndWrite(null); }, typeof(NotSupportedException));
            CheckException(() => { contentReadStream.Write(null, 0, 0); }, typeof(NotSupportedException));
            CheckException(() => { contentReadStream.WriteByte(1); }, typeof(NotSupportedException));

            Assert.AreEqual(0, source.DisposeCount, "No calls to Dispose() expected.");
            contentReadStream.Dispose();
            Assert.AreEqual(1, source.DisposeCount, "Calls to Dispose() expected.");
        }

        #region Helper methods

        private void CheckException(Action action, Type exceptionType)
        {
            try
            {
                action();
                Assert.Fail("Exception expected.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType(), "Expected exception type.");
            }
        }

        private class MockStream : MemoryStream
        {
            private bool canSeek;
            private bool canRead;
            private int readTimeout;

            public int DisposeCount { get; private set; }
            public int BufferSize { get; private set; }
            public int ReadCount { get; private set; }
            public int CanSeekCount { get; private set; }
            public int CanTimeoutCount { get; private set; }
            public int SeekCount { get; private set; }

            public override bool CanSeek
            {
                get
                {
                    CanSeekCount++;
                    return canSeek;
                }
            }

            public override bool CanRead
            {
                get { return canRead; }
            }

            public override int ReadTimeout
            {
                get { return readTimeout; }
                set { readTimeout = value; }
            }

            public override bool CanTimeout
            {
                get
                {
                    CanTimeoutCount++;
                    return base.CanTimeout;
                }
            }

            public MockStream(byte[] data)
                : this(data, true, true)
            {
            }

            public MockStream(byte[] data, bool canSeek, bool canRead)
                : base(data)
            {
                this.canSeek = canSeek;
                this.canRead = canRead;
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                SetBufferSize(count);
                return base.BeginRead(buffer, offset, count, callback, state);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ReadCount++;
                SetBufferSize(count);
                return base.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin loc)
            {
                SeekCount++;
                return base.Seek(offset, loc);
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }

            private void SetBufferSize(int count)
            {
                if (BufferSize == 0)
                {
                    BufferSize = count;
                }
                else
                {
                    Assert.AreEqual(BufferSize, count, "Expected same buffer size to be used for all operations.");
                }
            }
        }

        #endregion
    }
}
