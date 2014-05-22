using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace System.Net.Http.Test
{
    [TestClass]
    public class DelegatingHandlerTest
    {
        [TestMethod]
        public void Ctor_CreateDispose_Success()
        {
            MockHandler handler = new MockHandler();
            Assert.IsNull(handler.InnerHandler);
            handler.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Ctor_CreateDisposeAssign_Throws()
        {
            MockHandler handler = new MockHandler();
            Assert.IsNull(handler.InnerHandler);
            handler.Dispose();
            handler.InnerHandler = new MockTransportHandler();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullInnerHandler_Throw()
        {
            MockHandler handler = new MockHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_SetNullInnerHandler_Throw()
        {
            MockHandler handler = new MockHandler();
            handler.InnerHandler = null;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_WithoutSettingInnerHandlerCallMethod_Throw()
        {
            MockHandler handler = new MockHandler();

            HttpResponseMessage response = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).Result;
        }

        [TestMethod]
        public void SendAsync_SetInnerHandlerCallMethod_InnerHandlerSendIsCalled()
        {
            MockHandler handler = new MockHandler();
            MockTransportHandler transport = new MockTransportHandler();
            handler.InnerHandler = transport;

            HttpResponseMessage response = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).Result;

            Assert.IsNotNull(response, "Response message expected.");
            Assert.AreEqual(1, handler.SendAsyncCount, "Expected calls to handler.Send().");
            Assert.AreEqual(1, transport.SendAsyncCount, "Expected calls to transport.Send().");
        }

        [TestMethod]
        public void SendAsync_SetInnerHandlerTwiceCallMethod_SecondInnerHandlerSendIsCalled()
        {
            MockHandler handler = new MockHandler();
            MockTransportHandler transport1 = new MockTransportHandler();
            MockTransportHandler transport2 = new MockTransportHandler();
            handler.InnerHandler = transport1;
            handler.InnerHandler = transport2;

            HttpResponseMessage response = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None).Result;

            Assert.IsNotNull(response, "Response message expected.");
            Assert.AreEqual(1, handler.SendAsyncCount, "Expected calls to handler.Send().");
            Assert.AreEqual(0, transport1.SendAsyncCount, "Expected calls to transport.Send().");
            Assert.AreEqual(1, transport2.SendAsyncCount, "Expected calls to transport.Send().");
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
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SendAsync_Disposed_Throw()
        {
            MockTransportHandler transport = new MockTransportHandler();
            MockHandler handler = new MockHandler(transport);
            handler.Dispose();

            handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
        }

        [TestMethod]
        public void SendAsync_CallMethod_InnerHandlerSendAsyncIsCalled()
        {
            MockTransportHandler transport = new MockTransportHandler();
            MockHandler handler = new MockHandler(transport);

            Task<HttpResponseMessage> t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
            t.Wait();

            Assert.IsNotNull(t.Result, "Response message expected.");
            Assert.AreEqual(1, handler.SendAsyncCount, "Expected calls to handler.Send().");
            Assert.AreEqual(1, transport.SendAsyncCount, "Expected calls to transport.Send().");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_CallMethodWithoutSettingInnerHandler_Throws()
        {
            MockHandler handler = new MockHandler();

            Task<HttpResponseMessage> t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_SetInnerHandlerAfterCallMethod_Throws()
        {
            MockTransportHandler transport = new MockTransportHandler();
            MockHandler handler = new MockHandler(transport);

            Task<HttpResponseMessage> t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
            t.Wait();

            Assert.IsNotNull(t.Result, "Response message expected.");
            Assert.AreEqual(1, handler.SendAsyncCount, "Expected calls to handler.Send().");
            Assert.AreEqual(1, transport.SendAsyncCount, "Expected calls to transport.Send().");

            handler.InnerHandler = transport;
        }

        [TestMethod]
        public void Dispose_CallDispose_OverriddenDisposeMethodCalled()
        {
            MockTransportHandler innerHandler = new MockTransportHandler();
            MockHandler handler = new MockHandler(innerHandler);
            handler.Dispose();

            Assert.AreEqual(1, handler.DisposeCount, "Expected Dispose() to be called.");
            Assert.AreEqual(1, innerHandler.DisposeCount, "Expected Dispose() to be called.");
        }

        [TestMethod]
        public void Dispose_CallDisposeMultipleTimes_OverriddenDisposeMethodCalled()
        {
            MockTransportHandler innerHandler = new MockTransportHandler();
            MockHandler handler = new MockHandler(innerHandler);
            handler.Dispose();
            handler.Dispose();
            handler.Dispose();

            Assert.AreEqual(3, handler.DisposeCount, "Expected Dispose() to be called.");
            Assert.AreEqual(1, innerHandler.DisposeCount, "Expected Dispose() to be called.");
        }

        #region Helper methods

        private class MockHandler : DelegatingHandler
        {
            public int SendAsyncCount { get; private set; }
            public int DisposeCount { get; private set; }

            public MockHandler()
                : base()
            {
            }

            public MockHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected internal override Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Threading.CancellationToken cancellationToken)
            {
                SendAsyncCount++;
                return base.SendAsync(request, cancellationToken);
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }
        }

        private class MockTransportHandler : HttpMessageHandler
        {
            public int SendAsyncCount { get; private set; }
            public int DisposeCount { get; private set; }

            protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Threading.CancellationToken cancellationToken)
            {
                SendAsyncCount++;
                TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.TrySetResult(new HttpResponseMessage());
                return tcs.Task;
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
