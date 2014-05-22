using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Test
{
    [TestClass]
    public class MessageProcessingHandlerTest
    {
        [TestMethod]
        public void Ctor_CreateDispose_Success()
        {
            MockHandler handler = new MockHandler();
            Assert.IsNull(handler.InnerHandler);
            handler.Dispose();
        }

        [TestMethod]
        public void Ctor_CreateWithHandlerDispose_Success()
        {
            MockHandler handler = new MockHandler(new MockTransportHandler());
            Assert.IsNotNull(handler.InnerHandler);
            handler.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_CreateWithNullHandler_Throws()
        {
            MockHandler handler = new MockHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendAsync_NullRequest_Throw()
        {
            MockTransportHandler transport = new MockTransportHandler();
            MockHandler handler = new MockHandler(transport);

            handler.SendAsync(null, CancellationToken.None);
        }

        [TestMethod]
        public void SendAsync_CallMethod_ProcessRequestAndProcessResponseCalled()
        {
            MockTransportHandler transport = new MockTransportHandler();
            MockHandler handler = new MockHandler(transport);

            Task<HttpResponseMessage> t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
            t.Wait();

            Assert.IsNotNull(t.Result, "Response message expected.");
            Assert.AreEqual(1, handler.ProcessRequestCount, "Expected calls to ProcessRequestCount().");
            Assert.AreEqual(1, handler.ProcessResponseCount, "Expected calls to ProcessRequestCount().");
        }

        [TestMethod]
        public void SendAsync_InnerHandlerThrows_ThrowWithoutCallingProcessRequest()
        {
            MockTransportHandler transport = new MockTransportHandler(true); // throw if Send/SendAsync() is called
            MockHandler handler = new MockHandler(transport);

            Task t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to fail.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException));
            });
            t.Wait();

            Assert.AreEqual(1, handler.ProcessRequestCount, "Expected calls to ProcessRequestCount().");
            Assert.AreEqual(0, handler.ProcessResponseCount, "Expected no calls to ProcessRequestCount().");
        }

        [TestMethod]
        public void SendAsync_InnerHandlerReturnsNullResponse_ThrowInvalidOperationExceptionWithoutCallingProcessRequest()
        {
            MockTransportHandler transport = new MockTransportHandler(() => { return null; });
            MockHandler handler = new MockHandler(transport);

            Task t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to fail.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(InvalidOperationException));
            });
            t.Wait();

            Assert.AreEqual(1, handler.ProcessRequestCount, "Expected calls to ProcessRequestCount().");
            Assert.AreEqual(0, handler.ProcessResponseCount, "Expected no calls to ProcessRequestCount().");
        }

        [TestMethod]
        public void SendAsync_ProcessRequestThrows_ThrowWithoutCallingProcessRequestNorInnerHandler()
        {
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessRequest() throws exception.
            MockHandler handler = new MockHandler(transport, true, () => { throw new MockException(); }); 

            // Note that ProcessRequest() is called by SendAsync(). However, the exception is not thrown
            // by SendAsync(). Instead, the returned Task is marked as faulted and contains the exception. 
            Task t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to fail.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException));
            });
            t.Wait();

            Assert.AreEqual(0, transport.SendAsyncCount, "Expected no calls to inner handler.");
            Assert.AreEqual(1, handler.ProcessRequestCount, "Expected calls to ProcessRequestCount().");
            Assert.AreEqual(0, handler.ProcessResponseCount, "Expected no calls to ProcessRequestCount().");
        }

        [TestMethod]
        public void SendAsync_ProcessResponseThrows_TaskIsFaulted()
        {
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessResponse() throws exception.
            MockHandler handler = new MockHandler(transport, false, () => { throw new MockException(); }); 

            // Throwing an exception in ProcessResponse() will cause the Task to complete as 'faulted'.
            Task t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to fail.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException));
            });
            t.Wait();

            Assert.AreEqual(1, transport.SendAsyncCount, "Expected calls to inner handler.");
            Assert.AreEqual(1, handler.ProcessRequestCount, "Expected calls to ProcessRequestCount().");
            Assert.AreEqual(1, handler.ProcessResponseCount, "Expected calls to ProcessRequestCount().");
        }

        [TestMethod]
        public void SendAsync_OperationCanceledWhileInnerHandlerIsWorking_TaskSetToIsCanceled()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            // We simulate a cancellation happening while the inner handler was processing the request, by letting
            // the inner mock handler call Cancel() and behave like if another thread called cancel while it was
            // processing.
            MockTransportHandler transport = new MockTransportHandler(cts); // inner handler will cancel.
            MockHandler handler = new MockHandler(transport);

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "Expected Task to be canceled.");
                Assert.AreEqual(0, handler.ProcessResponseCount, "Expected ProcessResponse() to not be called.");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_OperationCanceledWhileProcessRequestIsExecuted_TaskSetToIsCanceled()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessRequest will cancel.
            MockHandler handler = new MockHandler(transport, true,
                () => { cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); });

            // Note that even ProcessMessage() is called on the same thread, we don't expect SendAsync() to throw.
            // SendAsync() must complete successfully, but the Task will be canceled. 
            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "Expected Task to be canceled.");
                Assert.AreEqual(0, handler.ProcessResponseCount, "Expected ProcessResponse() to not be called.");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_OperationCanceledWhileProcessResponseIsExecuted_TaskSetToIsCanceled()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessResponse will cancel.
            MockHandler handler = new MockHandler(transport, false,
                () => { cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); });

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "Expected Task to be canceled.");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_ProcessRequestThrowsOperationCanceledExceptionNotBoundToCts_TaskSetToIsFaulted()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessRequest will throw a random OperationCanceledException() not related to cts. We also cancel
            // the cts to make sure the code behaves correctly even if cts is canceled & an OperationCanceledException
            // was thrown.
            MockHandler handler = new MockHandler(transport, true,
                () => { cts.Cancel(); throw new OperationCanceledException("custom"); });

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be canceled.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                    "Expected exception.");
            });
            t.Wait();

            Assert.AreEqual(0, handler.ProcessResponseCount, "Expected ProcessResponse to not be called.");
        }

        [TestMethod]
        public void SendAsync_ProcessResponseThrowsOperationCanceledExceptionNotBoundToCts_TaskSetToIsFaulted()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessResponse will throw a random OperationCanceledException() not related to cts. We also cancel
            // the cts to make sure the code behaves correctly even if cts is canceled & an OperationCanceledException
            // was thrown.
            MockHandler handler = new MockHandler(transport, false,
                () => { cts.Cancel(); throw new OperationCanceledException("custom"); });

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be canceled.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                    "Expected exception.");
            });
            t.Wait();

            Assert.AreEqual(1, handler.ProcessResponseCount, "Expected ProcessResponse to be called.");
        }
        
        [TestMethod]
        public void SendAsync_ProcessRequestThrowsOperationCanceledExceptionUsingOtherCts_TaskSetToIsFaulted()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationTokenSource otherCts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessRequest will throw a random OperationCanceledException() not related to cts. We also cancel
            // the cts to make sure the code behaves correctly even if cts is canceled & an OperationCanceledException
            // was thrown.
            MockHandler handler = new MockHandler(transport, true,
                () => { cts.Cancel(); throw new OperationCanceledException("custom", otherCts.Token); });

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be canceled.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                    "Expected exception.");
            });
            t.Wait();

            Assert.AreEqual(0, handler.ProcessResponseCount, "Expected ProcessResponse to not be called.");
        }

        [TestMethod]
        public void SendAsync_ProcessResponseThrowsOperationCanceledExceptionUsingOtherCts_TaskSetToIsFaulted()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationTokenSource otherCts = new CancellationTokenSource();
            MockTransportHandler transport = new MockTransportHandler();
            // ProcessResponse will throw a random OperationCanceledException() not related to cts. We also cancel
            // the cts to make sure the code behaves correctly even if cts is canceled & an OperationCanceledException
            // was thrown.
            MockHandler handler = new MockHandler(transport, false,
                () => { cts.Cancel(); throw new OperationCanceledException("custom", otherCts.Token); });

            Task t = handler.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected Task to be canceled.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                    "Expected exception.");
            });
            t.Wait();

            Assert.AreEqual(1, handler.ProcessResponseCount, "Expected ProcessResponse to be called.");
        }

        #region Helper methods

        [Serializable]
        public class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        private class MockHandler : MessageProcessingHandler
        {
            private bool callInProcessRequest;
            private Action customAction;

            public int ProcessRequestCount { get; private set; }
            public int ProcessResponseCount { get; private set; }

            public MockHandler()
                : base()
            {
            }

            public MockHandler(HttpMessageHandler innerHandler)
                : this(innerHandler, true, null)
            {
            }

            public MockHandler(HttpMessageHandler innerHandler, bool callInProcessRequest, Action customAction)
                : base(innerHandler)
            {
                this.customAction = customAction;
                this.callInProcessRequest = callInProcessRequest;
            }

            protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                ProcessRequestCount++;
                Assert.IsNotNull(request, "Expected request message.");

                if (callInProcessRequest && (customAction != null))
                {
                    customAction();
                }

                return request;
            }

            protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response,
                CancellationToken cancellationToken)
            {
                ProcessResponseCount++;
                Assert.IsNotNull(response, "Expected response message.");

                if (!callInProcessRequest && (customAction != null))
                {
                    customAction();
                }

                return response;
            }
        }

        private class MockTransportHandler : HttpMessageHandler
        {
            private bool alwaysThrow;
            private CancellationTokenSource cts;
            private Func<HttpResponseMessage> mockResultDelegate;

            public int SendAsyncCount { get; private set; }

            public MockTransportHandler(Func<HttpResponseMessage> mockResultDelegate)
            {
                this.mockResultDelegate = mockResultDelegate;
            }

            public MockTransportHandler()
            {
            }

            public MockTransportHandler(bool alwaysThrow)
            {
                this.alwaysThrow = alwaysThrow;
            }

            public MockTransportHandler(CancellationTokenSource cts)
            {
                this.cts = cts;
            }

            protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                SendAsyncCount++;

                TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();

                if (cts != null)
                {
                    cts.Cancel();
                    tcs.TrySetCanceled();
                }

                if (alwaysThrow)
                {
                    tcs.TrySetException(new MockException());
                }
                else
                {
                    if (mockResultDelegate == null)
                    {
                        tcs.TrySetResult(new HttpResponseMessage());
                    }
                    else
                    {
                        tcs.TrySetResult(mockResultDelegate());
                    }
                }

                return tcs.Task;
            }
        }

        #endregion
    }
}
