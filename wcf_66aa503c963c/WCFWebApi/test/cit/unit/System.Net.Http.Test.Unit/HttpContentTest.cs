using System.IO;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using System.Net.Test.Common.Logging;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class HttpContentTest
    {
        [TestMethod]
        public void CopyToAsync_CallWithMockContent_MockContentMethodCalled()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);
            MemoryStream m = new MemoryStream();

            Task t = content.CopyToAsync(m);
            t.Wait();

            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "SerializeToStreamAsync() count.");
            CollectionAssert.AreEqual(content.GetMockData(), m.ToArray(), "Copied content data doesn't match.");
        }

        [TestMethod]
        public void CopyToAsync_ThrowCustomExceptionInOverriddenMethod_ExceptionExposedByTask()
        {
            MockContent content = new MockContent(new MockException(), MockOptions.ThrowInSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(MockException), 
                    "Unexpected exception type.");
            });
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_ThrowObjectDisposedExceptionInOverriddenMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new ObjectDisposedException(""), MockOptions.ThrowInSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(ObjectDisposedException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_ThrowIOExceptionInOverriddenMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new IOException(), MockOptions.ThrowInSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void CopyToAsync_ThrowCustomExceptionInOverriddenAsyncMethod_ExceptionBubblesUp()
        {
            MockContent content = new MockContent(new MockException(), MockOptions.ThrowInAsyncSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(MockException),
                    "Unexpected exception type.");
            });
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_ThrowObjectDisposedExceptionInOverriddenAsyncMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new ObjectDisposedException(""), 
                MockOptions.ThrowInAsyncSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(ObjectDisposedException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_ThrowIOExceptionInOverriddenAsyncMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new IOException(), MockOptions.ThrowInAsyncSerializeMethods);

            MemoryStream m = new MemoryStream();
            Task t = content.CopyToAsync(m).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyToAsync_MockContentReturnsNull_Throw()
        {
            // return 'null' when CopyToAsync() is called.
            MockContent content = new MockContent(MockOptions.ReturnNullInCopyToAsync); 
            MemoryStream m = new MemoryStream();
            
            // The HttpContent derived class (MockContent in our case) must return a Task object when WriteToAsync()
            // is called. If not, HttpContent will throw.
            Task t = content.CopyToAsync(m);
            t.Wait();
        }

        [TestMethod]
        public void CopyToAsync_BufferContentFirst_UseBufferedStreamAsSource()
        {
            byte[] data = new byte[10];
            MockContent content = new MockContent(data);
            content.LoadIntoBufferAsync().Wait();

            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "SerializeToStream() wasn't called while buffering.");
            MemoryStream destination = new MemoryStream();
            Task t = content.CopyToAsync(destination);
            t.Wait();

            // Our MockContent should not be called for the CopyTo() operation since the buffered stream should be 
            // used.
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "Expected buffered content to be used.");
            Assert.AreEqual(data.Length, destination.Length, "Destination expected to have same length as content.");
        }

        [TestMethod]
        public void TryComputeLength_RetrieveContentLength_ComputeLengthShouldBeCalled()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);

            Assert.AreEqual(content.GetMockData().Length, content.Headers.ContentLength, "Content-Length");
            Assert.AreEqual(1, content.TryComputeLengthCount, "ComputeLength count.");
        }

        [TestMethod]
        public void TryComputeLength_RetrieveContentLengthFromBufferedContent_ComputeLengthIsNotCalled()
        {
            MockContent content = new MockContent();
            content.LoadIntoBufferAsync().Wait();

            Assert.AreEqual(content.GetMockData().Length, content.Headers.ContentLength, "Content-Length");
            
            // Called once to determine the size of the buffer.
            Assert.AreEqual(1, content.TryComputeLengthCount, "ComputeLength count."); 
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TryComputeLength_ThrowCustomExceptionInOverriddenMethod_ExceptionBubblesUpToCaller()
        {
            MockContent content = new MockContent(MockOptions.ThrowInTryComputeLength); 

            MemoryStream m = new MemoryStream();
            Log.Info(content.Headers.ContentLength.ToString());
        }

        [TestMethod]
        public void ReadAsStreamAsync_GetFromUnbufferedContent_CreateContentReadStreamCalledOnce()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);

            // Call multiple times: CreateContentReadStreamAsync() should be called only once.
            Stream stream = content.ReadAsStreamAsync().Result;
            stream = content.ReadAsStreamAsync().Result;
            stream = content.ReadAsStreamAsync().Result;

            Assert.AreEqual(1, content.CreateContentReadStreamCount, "CreateContentReadStream count.");
            Assert.AreEqual(content.GetMockData().Length, stream.Length,
                "Buffered stream length doesn't match content length.");
            Assert.AreSame(stream, content.ReadAsStreamAsync().Result,
                "Expected same instance when calling ContentReadStream multiple times.");
        }

        [TestMethod]
        public void ReadAsStreamAsync_GetFromBufferedContent_CreateContentReadStreamCalled()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);
            content.LoadIntoBufferAsync().Wait();

            Stream stream = content.ReadAsStreamAsync().Result;

            Assert.AreEqual(0, content.CreateContentReadStreamCount, "CreateContentReadStream count.");
            Assert.AreEqual(content.GetMockData().Length, stream.Length, 
                "Buffered stream length doesn't match content length.");
            Assert.AreSame(stream, content.ReadAsStreamAsync().Result, 
                "Expected same instance when calling ContentReadStream multiple times.");
        }

        [TestMethod]
        public void ReadAsStreamAsync_FirstGetFromUnbufferedContentThenGetFromBufferedContent_SameStream()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);

            Stream before = content.ReadAsStreamAsync().Result;
            Assert.AreEqual(1, content.CreateContentReadStreamCount, "CreateContentReadStream count before buffering.");

            content.LoadIntoBufferAsync().Wait();

            Stream after = content.ReadAsStreamAsync().Result;
            Assert.AreEqual(1, content.CreateContentReadStreamCount, "CreateContentReadStream count after buffering.");

            // Note that ContentReadStream returns always the same stream. If the user gets the stream, buffers content,
            // and gets the stream again, the same instance is returned. Returning a different instance could be 
            // confusing, even though there shouldn't be any real world scenario for retrieving the read stream both
            // before and after buffering content.
            Assert.AreEqual(before, after, 
                "Expected same stream when calling ContentReadStream before and after buffering.");
        }

        [TestMethod]
        public void ReadAsStreamAsync_UseBaseImplementation_ContentGetsBufferedThenMemoryStreamReturned()
        {
            MockContent content = new MockContent(MockOptions.DontOverrideCreateContentReadStream);
            Stream stream = content.ReadAsStreamAsync().Result;

            Assert.IsNotNull(stream, "ContentReadStream is null.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "Content was not buffered.");
            Assert.AreSame(stream, content.ReadAsStreamAsync().Result,
                "Expected same instance when calling ContentReadStream multiple times.");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_BufferSizeSmallerThanContentSizeWithCalculatedContentLength_Throw()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);
            Task t = content.LoadIntoBufferAsync(content.GetMockData().Length - 1).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "LoadIntoBufferAsync() did not fail.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(HttpRequestException),
                    "Unexpected exception type.");
            });
            
            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_BufferSizeSmallerThanContentSizeWithNullContentLength_Throw()
        {
            MockContent content = new MockContent();
            Task t = content.LoadIntoBufferAsync(content.GetMockData().Length - 1).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "LoadIntoBufferAsync() did not fail.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(HttpRequestException),
                    "Unexpected exception type.");
            });

            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_CallOnMockContentWithCalculatedContentLength_CopyToAsyncMemoryStreamCalled()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);
            Assert.IsNotNull(content.Headers.ContentLength, "Content length expected to be not null before buffering.");
            Task t = content.LoadIntoBufferAsync();
            t.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "Task failed.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "No. of calls to SerializeToStreamAsync().");
            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "Buffered stream should not be writable.");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_CallOnMockContentWithNullContentLength_CopyToAsyncMemoryStreamCalled()
        {
            MockContent content = new MockContent();
            Assert.IsNull(content.Headers.ContentLength, "No content length expected before buffering.");
            Task t = content.LoadIntoBufferAsync();
            t.Wait();
            Assert.IsNotNull(content.Headers.ContentLength, "Content length expected to be not null after buffering.");
            Assert.AreEqual(content.MockData.Length, content.Headers.ContentLength,
                "Expected content length after buffering.");

            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "Task failed.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "No. of calls to SerializeToStreamAsync().");
            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "Buffered stream should not be writable.");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_CallOnMockContentWithLessLengthThanContentLengthHeader_BufferedStreamLengthMatchesActualLengthNotContentLengthHeaderValue()
        {
            byte[] data = Encoding.ASCII.GetBytes("16 bytes of data");
            MockContent content = new MockContent(data);
            content.Headers.ContentLength = 32; // set the Content-Length header to a value > actual data length
            Assert.AreEqual(32, content.Headers.ContentLength, "Expected content length to reflect custom value.");

            Task t = content.LoadIntoBufferAsync();
            t.Wait();

            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "No. of calls to SerializeToStreamAsync().");
            Assert.IsNotNull(content.Headers.ContentLength, "Content length expected to be not null after buffering.");
            Assert.AreEqual(32, content.Headers.ContentLength, "Expected content length to not change after buffering.");
            Assert.AreEqual(data.Length, content.ReadAsStreamAsync().Result.Length, "ContentReadStream.Length");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_CallMultipleTimesWithCalculatedContentLength_CopyToAsyncMemoryStreamCalledOnce()
        {
            MockContent content = new MockContent(MockOptions.CanCalculateLength);
            Task t1 = content.LoadIntoBufferAsync();
            t1.Wait();
            Task t2 = content.LoadIntoBufferAsync();
            t2.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, t1.Status, "Task failed.");
            Assert.AreEqual(TaskStatus.RanToCompletion, t2.Status, "Task failed.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "No. of calls to SerializeToStreamAsync().");
            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "Buffered stream should not be writable.");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_CallMultipleTimesWithNullContentLength_CopyToAsyncMemoryStreamCalledOnce()
        {
            MockContent content = new MockContent();
            Task t1 = content.LoadIntoBufferAsync();
            t1.Wait();
            Task t2 = content.LoadIntoBufferAsync();
            t2.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, t1.Status, "Task failed.");
            Assert.AreEqual(TaskStatus.RanToCompletion, t2.Status, "Task failed.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "No. of calls to SerializeToStreamAsync().");
            Assert.IsFalse(content.ReadAsStreamAsync().Result.CanWrite, "Buffered stream should not be writable.");
        }

        [TestMethod]
        public void LoadIntoBufferAsync_ThrowCustomExceptionInOverriddenMethod_ExceptionExposedByTask()
        {
            MockContent content = new MockContent(new MockException(), MockOptions.ThrowInSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(MockException),
                    "Unexpected exception type.");
            });
            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_ThrowObjectDisposedExceptionInOverriddenMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new ObjectDisposedException(""), MockOptions.ThrowInSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(ObjectDisposedException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_ThrowIOExceptionInOverriddenMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new IOException(), MockOptions.ThrowInSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void LoadIntoBufferAsync_ThrowCustomExceptionInOverriddenAsyncMethod_ExceptionBubblesUpToCaller()
        {
            MockContent content = new MockContent(new MockException(), MockOptions.ThrowInAsyncSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.InnerException, typeof(MockException),
                    "Unexpected exception type.");
            });
            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_ThrowObjectDisposedExceptionInOverriddenAsyncMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new ObjectDisposedException(""), 
                MockOptions.ThrowInAsyncSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(ObjectDisposedException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void LoadIntoBufferAsync_ThrowIOExceptionInOverriddenAsyncMethod_ExceptionWrappedInHttpRequestException()
        {
            MockContent content = new MockContent(new IOException(), MockOptions.ThrowInAsyncSerializeMethods);

            Task t = content.LoadIntoBufferAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task is not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), "Expected exception");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_BufferContentThenDisposeContent_BufferedStreamGetsDisposed()
        {
            MockContent content = new MockContent();
            content.LoadIntoBufferAsync().Wait();

            FieldInfo bufferedContentField = typeof(HttpContent).GetField("bufferedContent", 
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(bufferedContentField, "bufferedContent field doesn't exist.");

            MemoryStream bufferedContentStream = bufferedContentField.GetValue(content) as MemoryStream;
            Assert.IsNotNull(bufferedContentStream, "bufferedContent field can't be casted to MemoryStream.");
            
            content.Dispose();

            // The following line will throw an ObjectDisposedException if the buffered-stream was correctly disposed.
            Log.Info(bufferedContentStream.Length.ToString());
        }

        [TestMethod]
        public void Dispose_GetReadStreamThenDispose_ReadStreamGetsDisposed()
        {
            MockContent content = new MockContent();
            MockMemoryStream s = content.ReadAsStreamAsync().Result as MockMemoryStream;
            Assert.AreEqual(1, content.CreateContentReadStreamCount, "CreateContentReadStream count.");

            Assert.AreEqual(0, s.DisposeCount, "Read stream DisposeCount before disposing content.");
            content.Dispose();
            Assert.AreEqual(1, s.DisposeCount, "Read stream DisposeCount after disposing content.");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_DisposeContentThenAccessContentLength_Throw()
        {
            MockContent content = new MockContent();

            // This is not really typical usage of the type, but let's make sure we consider also this case: The user
            // keeps a reference to the Headers property before disposing the content. Then after disposing, the user
            // accesses the ContentLength property.
            var headers = content.Headers;
            content.Dispose();
            Log.Info(headers.ContentLength.ToString());            
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void CopyToAsync_UseStreamWriteByteWithBufferSizeSmallerThanContentSize_Throw()
        {
            // MockContent uses stream.WriteByte() rather than stream.Write(): Verify that the max. buffer size
            // is also checked when using WriteByte().
            MockContent content = new MockContent(MockOptions.UseWriteByteInCopyTo);
            try
            {
                content.LoadIntoBufferAsync(content.GetMockData().Length - 1).Wait();
            }
            catch (AggregateException ae)
            {
                throw ae.GetBaseException();
            }
        }
        
        [TestMethod]
        public void ReadAsStringAsync_EmptyContent_EmptyString()
        {
            MockContent content = new MockContent(new byte[0]);
            Assert.AreEqual(string.Empty, content.ReadAsStringAsync().Result, "Expected empty string.");
        }

        [TestMethod]
        public void ReadAsStringAsync_SetSpecificCharset_CorrectCharsetUsed()
        {
            // Use content with turkish dot-less lower-case i and uppercase I with dot.
            string sourceString = "i\u0131I\u0130"; 
            Encoding encoding = Encoding.GetEncoding("ISO-8859-9");
            byte[] contentBytes = encoding.GetBytes(sourceString); // 69 fd 49 dd

            MockContent content = new MockContent(contentBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            content.Headers.ContentType.CharSet = "ISO-8859-9";

            // Reading the string should consider the charset of the 'Content-Type' header.
            string result = content.ReadAsStringAsync().Result;

            Assert.AreEqual(sourceString, result, "Expected string.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadAsStringAsync_SetInvalidCharset_Throw()
        {
            string sourceString = "some string";
            byte[] contentBytes = Encoding.ASCII.GetBytes(sourceString);

            MockContent content = new MockContent(contentBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            content.Headers.ContentType.CharSet = "invalid";

            // This will throw because we have an invalid charset.
            try
            {
                content.ReadAsStringAsync().Wait();
                Assert.Fail();
            }
            catch (AggregateException ae)
            {
                throw ae.GetBaseException();
            }
        }

        [TestMethod]
        public void ReadAsStringAsync_SetNoCharset_DefaultCharsetUsed()
        {
            // Use content with umlaut characters.
            string sourceString = "ÄäüÜ"; // c4 e4 fc dc
            Encoding defaultEncoding = Encoding.GetEncoding("utf-8");
            byte[] contentBytes = defaultEncoding.GetBytes(sourceString);

            MockContent content = new MockContent(contentBytes);

            // Reading the string should consider the charset of the 'Content-Type' header.
            string result = content.ReadAsStringAsync().Result;

            Assert.AreEqual(sourceString, result, "Expected string.");
        }

        [TestMethod]
        public void ReadAsByteArrayAsync_EmptyContent_EmptyArray()
        {
            MockContent content = new MockContent(new byte[0]);
            Assert.AreEqual(0, content.ReadAsByteArrayAsync().Result.Length, "Expected empty string.");
        }

        [TestMethod]
        public void Dispose_DisposedObjectThenAccessMembers_ObjectDisposedException()
        {
            MockContent content = new MockContent();
            content.Dispose();

            MemoryStream m = new MemoryStream();

            ExceptionAssert.ThrowsObjectDisposed(() => { Task t = content.CopyToAsync(m); t.Wait(); }, "CopyToAsync");
            ExceptionAssert.ThrowsObjectDisposed(() => { Log.Info(content.ReadAsStreamAsync().Result.ToString()); }, 
                "get_ContentReadStream");
            ExceptionAssert.ThrowsObjectDisposed(() => { Task t = content.LoadIntoBufferAsync(); t.Wait(); }, 
                "LoadIntoBufferAsync");
            ExceptionAssert.ThrowsObjectDisposed(() => { content.ReadAsStringAsync(); }, "ReadAsString");
            ExceptionAssert.ThrowsObjectDisposed(() => { content.ReadAsByteArrayAsync(); }, "ReadAsByteArray");

            // Note that we don't throw when users access the Headers property. This is useful e.g. to be able to 
            // read the headers of a content, even though the content is already disposed. Note that the FX guidelines
            // only require members to throw ObjectDisposedExcpetion for members "that cannot be used after the object 
            // has been disposed of".
            //ExceptionAssert.ThrowsObjectDisposed(() => { Log.Info(content.Headers.ToString()); }, "get_Headers");
        }

        #region Helper methods

        public class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        [Flags]
        private enum MockOptions
        {
            None = 0x0,
            ThrowInSerializeMethods = 0x1,
            ReturnNullInCopyToAsync = 0x2,
            UseWriteByteInCopyTo = 0x4,
            DontOverrideCreateContentReadStream = 0x8,
            CanCalculateLength = 0x10,
            ThrowInTryComputeLength = 0x20,
            ThrowInAsyncSerializeMethods = 0x40
        }

        private class MockContent : HttpContent
        {
            private byte[] mockData;
            private MockOptions options;
            private Exception customException;
            
            public int TryComputeLengthCount { get; private set; }
            public int SerializeToStreamAsyncCount { get; private set; }
            public int CreateContentReadStreamCount { get; private set; }
            public int DisposeCount { get; private set; }

            public byte[] MockData
            {
                get { return mockData; }
            }

            public MockContent()
                : this((byte[])null, MockOptions.None)
            { 
            }

            public MockContent(byte[] mockData)
                : this(mockData, MockOptions.None)
            {
            }

            public MockContent(MockOptions options)
                : this((byte[])null, options)
            { 
            }

            public MockContent(Exception customException, MockOptions options)
                : this((byte[])null, options)
            {
                this.customException = customException;
            }

            public MockContent(byte[] mockData, MockOptions options)
            {
                this.options = options;

                if (mockData == null)
                {
                    this.mockData = Encoding.ASCII.GetBytes("data");
                }
                else
                {
                    this.mockData = mockData;
                }
            }

            public byte[] GetMockData()
            {
                return mockData;
            }

            protected internal override bool TryComputeLength(out long length)
            {
                TryComputeLengthCount++;

                if ((options & MockOptions.ThrowInTryComputeLength) != 0)
                {
                    throw new MockException();
                }

                if ((options & MockOptions.CanCalculateLength) != 0)
                {
                    length = mockData.Length;
                    return true;
                }
                else
                {
                    length = 0;
                    return false;
                }
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                SerializeToStreamAsyncCount++;

                if ((options & MockOptions.ReturnNullInCopyToAsync) != 0)
                {
                    return null;
                }

                if ((options & MockOptions.ThrowInAsyncSerializeMethods) != 0)
                {
                    throw customException;
                }

                return Task.Factory.StartNew(() =>
                {
                    CheckThrow();
                    IAsyncResult ar = stream.BeginWrite(mockData, 0, mockData.Length, null, null);
                    stream.EndWrite(ar);
                });
            }

            protected override Task<Stream> CreateContentReadStreamAsync()
            {
                CreateContentReadStreamCount++;

                if ((options & MockOptions.DontOverrideCreateContentReadStream) != 0)
                {
                    return base.CreateContentReadStreamAsync();
                }
                else
                {
                    Task<Stream> task = new Task<Stream>(() => new MockMemoryStream(mockData, 0, mockData.Length, false));
                    task.RunSynchronously();
                    return task;
                }
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }

            private void CheckThrow()
            {
                if ((options & MockOptions.ThrowInSerializeMethods) != 0)
                {
                    throw customException;
                }
            }
        }

        private class MockMemoryStream : MemoryStream
        {
            public int DisposeCount { get; private set; }

            public MockMemoryStream(byte[] buffer, int index, int count, bool writable)
                : base(buffer, index, count, writable)
            {
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }
        }
        
        #endregion
    }
}
