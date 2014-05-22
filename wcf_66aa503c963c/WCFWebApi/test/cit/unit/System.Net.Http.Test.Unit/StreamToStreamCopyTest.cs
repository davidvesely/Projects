using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class StreamToStreamCopyTest
    {
        [TestMethod]
        public void Execute_AsyncReadSyncWrite_CopiedCorrectly()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.None); // async completion
            MockStream destination = new MockStream(MockOption.CompleteSync);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_SyncReadAsyncWrite_CopiedCorrectly()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.CompleteSync);
            MockStream destination = new MockStream(MockOption.None); // async completion

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_SyncReadAsyncWriteWithDisposeSourceNotSet_CopiedCorrectlySourceNotDisposed()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.CompleteSync);
            MockStream destination = new MockStream(MockOption.None); // async completion

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, false);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(0, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperations_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockStream source = new MockStream(data, MockOption.CompleteSync);
            MockStream destination = new MockStream(MockOption.CompleteSync);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperationsBesidesFirstRead_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockStream source = new MockStream(data, MockOption.CompleteSyncButFirst);
            MockStream destination = new MockStream(MockOption.CompleteSync);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperationsWithDestinationMemoryStream_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockStream source = new MockStream(data, MockOption.CompleteSync);
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperationsBesidesFirstReadWithDestinationMemoryStream_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockStream source = new MockStream(data, MockOption.CompleteSyncButFirst);
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperationsWithSourceMemoryStream_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockMemoryStream source = new MockMemoryStream(data);
            MockStream destination = new MockStream(MockOption.CompleteSync);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_OnlyCompletedSyncAsyncOperationsWithSourceMemoryStreamAndFirstWriteAsync_NoStackOverflow()
        {
            // Use 1,000 read and 1,000 write operations. All operations complete synchronously: Make sure we don't get
            // a stack overflow exception.
            byte[] data = CreateSourceData(1000);
            MockMemoryStream source = new MockMemoryStream(data);
            MockStream destination = new MockStream(MockOption.CompleteSyncButFirst);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_ReadingSourceThrowsInEndRead_TaskIsFaultedNoExceptionInCtor()
        {
            MockStream source = new MockStream(MockOption.ThrowInEnd);
            MockStream destination = new MockStream(MockOption.None); // async completion

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected MockException.");
            });

            t.Wait();
        }

        [TestMethod]
        public void Execute_ReadingSourceThrowsInBeginRead_TaskIsFaultedNoExceptionInCtor()
        {
            MockStream source = new MockStream(MockOption.ThrowInBegin);
            MockStream destination = new MockStream(MockOption.None); // async completion

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected MockException.");
            });

            t.Wait();
        }

        [TestMethod]
        public void Execute_WritingDestinationThrowsInEndWrite_TaskIsFaultedNoExceptionInCtor()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.None); // async completion
            MockStream destination = new MockStream(MockOption.ThrowInEnd);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected MockException.");
            });

            t.Wait();
        }

        [TestMethod]
        public void Execute_WritingDestinationThrowsInBeginWrite_TaskIsFaultedNoExceptionInCtor()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.None); // async completion
            MockStream destination = new MockStream(MockOption.ThrowInBegin);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected MockException.");
            });

            t.Wait();
        }

        [TestMethod]
        public void Execute_SourceMemoryStreamToNonMemoryStream_MemoryStreamReadSyncNonMemoryStreamWriteAsync()
        {
            byte[] data = CreateSourceData(10);
            MockMemoryStream source = new MockMemoryStream(data);
            MockStream destination = new MockStream(MockOption.None);

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            Assert.AreEqual(11, source.ReadCount, "source.ReadCount");
            Assert.AreEqual(10, destination.BeginWriteCount, "destination.BeginWriteCount");
            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_SourceNonMemoryStreamToMemoryStream_NonMemoryStreamReadAsyncMemoryStreamWriteSync()
        {
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.None);
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            Assert.AreEqual(11, source.BeginReadCount, "source.BeginReadCount");
            Assert.AreEqual(10, destination.WriteCount, "destination.WriteCount");
            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_SourceMemoryStreamToMemoryStream_WholeStreamCopiedWithOneWrite()
        {
            byte[] data = CreateSourceData(10);
            MockMemoryStream source = new MockMemoryStream(data);
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync();
            t.Wait();

            Assert.AreEqual(0, source.ReadCount, "source.ReadCount");
            Assert.AreEqual(1, destination.WriteCount, "destination.WriteCount");
            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        [TestMethod]
        public void Execute_SourceMemoryStreamThrows_TaskSetToFaultedNoExceptionInCtor()
        {
            byte[] data = CreateSourceData(10);
            MockMemoryStream source = new MockMemoryStream(data, true); // throw
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected exception.");
            });
            t.Wait();
        }

        [TestMethod]
        public void Execute_SourceMemoryStreamToTargetMemoryStreamThrows_TaskSetToFaultedNoExceptionInCtor()
        {
            byte[] data = CreateSourceData(10);
            MockMemoryStream source = new MockMemoryStream(data);
            MockMemoryStream destination = new MockMemoryStream(true); // throw

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException),
                    "Expected exception.");
            });
            t.Wait();
        }

        [TestMethod]
        public void Execute_SourceStreamThrowsOnDispose_ExceptionLoggedAndCaught()
        {
            // This test just makes sure that exceptions in Dispose() get caught and don't result in unhandled exceptions.
            byte[] data = CreateSourceData(10);
            MockStream source = new MockStream(data, MockOption.ThrowInDispose);
            MockMemoryStream destination = new MockMemoryStream();

            StreamToStreamCopy streamCopy = new StreamToStreamCopy(source, destination, 1, true);
            Task t = streamCopy.StartAsync().ContinueWith(task =>
            {
                // Exceptions thrown in Dispose() have no impact on the result. 
                Assert.IsFalse(task.IsFaulted, "Not expected task to be faulted.");
            });
            t.Wait();

            CheckResult(data, destination);
            Assert.AreEqual(1, source.DisposeCount, "DisposeCount");
        }

        #region Helper methods

        private static byte[] CreateSourceData(int length)
        {
            byte[] data = new byte[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)(i % 256);
            }
            return data;
        }

        private static void CheckResult(byte[] sourceData, Stream destination)
        {
            destination.Position = 0;
            byte[] destinationData = new byte[destination.Length];
            int read = destination.Read(destinationData, 0, destinationData.Length);

            Assert.AreEqual(sourceData.Length, destination.Length, "Expected amount of copied data.");
            CollectionAssert.AreEqual(sourceData, destinationData, "Destination content.");            
        }

        [Serializable]
        public class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        private enum MockOption
        {
            None,
            ThrowInEnd,
            ThrowInBegin,
            ThrowInDispose,
            CompleteSync,
            CompleteSyncButFirst
        }

        private class MockStream : Stream
        {
            private MockOption option;
            private MemoryStream dataStream;
            private bool firstCall;
            private bool firstPending;

            public int ReadCount { get; private set; }
            public int WriteCount { get; private set; }
            public int BeginReadCount { get; private set; }
            public int BeginWriteCount { get; private set; }
            public int DisposeCount { get; private set; }

            public override bool CanRead
            {
                get { return dataStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return dataStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return dataStream.CanWrite; }
            }

            public override long Length
            {
                get { return dataStream.Length; }
            }

            public override long Position
            {
                get { return dataStream.Position; }
                set { dataStream.Position = value; }
            }

            public MockStream(MockOption option)
                : this(null, option)
            {
            }
            
            public MockStream(byte[] data, MockOption option)
            {
                this.option = option;
                this.firstCall = true;

                if (data == null)
                {
                    this.dataStream = new MemoryStream();
                }
                else
                {
                    this.dataStream = new MemoryStream(data);
                }
            }

            public override void Flush()
            {
                dataStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ReadCount++;
                return dataStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return dataStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                dataStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                WriteCount++;
                dataStream.Write(buffer, offset, count);
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                BeginReadCount++;

                if (option == MockOption.ThrowInBegin)
                {
                    throw new MockException();
                }

                if ((option == MockOption.CompleteSync) || ((option == MockOption.CompleteSyncButFirst) && !IsFirstCall()))
                {
                    int readBytes = dataStream.Read(buffer, offset, count);
                    return new CompletedAsyncResult(state, readBytes);
                }
                return base.BeginRead(buffer, offset, count, callback, state);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                if (option == MockOption.ThrowInEnd)
                {
                    throw new MockException();
                }

                if ((option == MockOption.CompleteSync) || ((option == MockOption.CompleteSyncButFirst) && !IsFirstPending()))
                {
                    CompletedAsyncResult completedAR = asyncResult as CompletedAsyncResult;
                    Assert.IsNotNull(completedAR, "Parameter was not an instance of IAsyncResult.");
                    return completedAR.Size;
                }

                return base.EndRead(asyncResult);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                BeginWriteCount++;

                if (option == MockOption.ThrowInBegin)
                {
                    throw new MockException();
                }

                if ((option == MockOption.CompleteSync) || ((option == MockOption.CompleteSyncButFirst) && !IsFirstCall()))
                {
                    dataStream.Write(buffer, offset, count);
                    return new CompletedAsyncResult(state, 0);
                }
                return base.BeginWrite(buffer, offset, count, callback, state);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                if (option == MockOption.ThrowInEnd)
                {
                    throw new MockException();
                }

                if ((option == MockOption.CompleteSync) || ((option == MockOption.CompleteSyncButFirst) && !IsFirstPending()))
                {
                    return;
                }

                base.EndWrite(asyncResult);
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;

                if (option == MockOption.ThrowInDispose)
                {
                    throw new MockException("Exception from Dispose()");
                }

                base.Dispose(disposing);
            }

            private bool IsFirstCall()
            {
                if (firstCall)
                {
                    firstCall = false;
                    firstPending = true;
                    return true;
                }
                return false;
            }

            private bool IsFirstPending()
            {
                if (firstPending)
                {
                    firstPending = false;
                    return true;
                }
                return false;
            }

            private class CompletedAsyncResult : IAsyncResult
            {
                private object state;
                private int size;

                public int Size
                {
                    get { return size; }
                }

                public CompletedAsyncResult(object state, int size)
                {
                    this.state = state;
                    this.size = size;
                }

                #region IAsyncResult Members

                public object AsyncState
                {
                    get { return state; }
                }

                public WaitHandle AsyncWaitHandle
                {
                    get { return new AutoResetEvent(true); }
                }

                public bool CompletedSynchronously
                {
                    get { return true; }
                }

                public bool IsCompleted
                {
                    get { return true; }
                }

                #endregion
            }
        }

        private class MockMemoryStream : MemoryStream
        {
            private bool alwaysThrow;

            public int ReadCount { get; private set; }
            public int WriteCount { get; private set; }
            public int DisposeCount { get; private set; }

            public MockMemoryStream()
                : this(false)
            {
            }

            public MockMemoryStream(bool alwaysThrow)
                : base()
            {
                this.alwaysThrow = alwaysThrow;
            }

            public MockMemoryStream(byte[] data)
                : this(data, false)
            {
            }

            public MockMemoryStream(byte[] data, bool alwaysThrow)
                : base(data)
            {
                this.alwaysThrow = alwaysThrow;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ReadCount++;

                if (alwaysThrow)
                {
                    throw new MockException();
                }

                return base.Read(buffer, offset, count);
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                Assert.Fail("Async BeginWrite() should never be called on a MemoryStream.");
                return null;
            }

            public override byte[] ToArray()
            {
                if (alwaysThrow)
                {
                    throw new MockException();
                }
                return base.ToArray();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                WriteCount++;

                if (alwaysThrow)
                {
                    throw new MockException();
                }
                base.Write(buffer, offset, count);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                Assert.Fail("Async BeginWrite() should never be called on a MemoryStream.");
                return null;
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
