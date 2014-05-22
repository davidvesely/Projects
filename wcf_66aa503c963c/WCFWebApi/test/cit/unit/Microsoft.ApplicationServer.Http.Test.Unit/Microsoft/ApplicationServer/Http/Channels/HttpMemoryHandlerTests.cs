// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Common.Test.Mocks;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Server.Common;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpMemoryHandlerTests : UnitTest<HttpMemoryHandler>
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryHandler is public non-abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<HttpMemoryHandler, HttpMessageHandler>(
                TypeAssert.TypeProperties.IsPublicVisibleClass |
                TypeAssert.TypeProperties.IsDisposable);
        }

        #endregion Type Tests

        #region Helpers
        private static HttpMemoryHandler CreateHttpMemoryHandler()
        {
            InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue = new InputQueue<HttpMemoryChannel.HttpMemoryRequestContext>();
            HttpMemoryHandler handler = new HttpMemoryHandler(inputQueue);
            return handler;
        }
        #endregion

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryHandler constructor.")]
        public void HttpMemoryHandlerConstructor()
        {
            HttpMemoryHandler handler = CreateHttpMemoryHandler();
            Assert.IsNotNull(handler, "handler should not be null");
        }
        #endregion Constructor Tests

        #region Members

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Dispose does not dispose input queue.")]
        public void HttpMemoryHandlerShouldNotDisposeInputQueue()
        {
            HttpMemoryChannel.HttpMemoryRequestContext context = new HttpMemoryChannel.HttpMemoryRequestContext(new HttpRequestMessage());

            // Create input queue and start dequeing first item 
            InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue = new InputQueue<HttpMemoryChannel.HttpMemoryRequestContext>();
            inputQueue.BeginDequeue(TimeSpan.MaxValue, (ar) => { }, null);
            bool beforeResult = inputQueue.EnqueueWithoutDispatch(context, () => { });
            Assert.IsTrue(beforeResult, "Could not enqueue context as expected before disposing memory handler.");

            HttpMemoryHandler handler = new HttpMemoryHandler(inputQueue);
            handler.Dispose();

            // Start dequeing second item 
            inputQueue.BeginDequeue(TimeSpan.MaxValue, (ar) => { }, null);
            bool afterResult = inputQueue.EnqueueWithoutDispatch(context, () => { });
            Assert.IsTrue(afterResult, "Could not enqueue context as expected after disposing memory handler.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("SubmitRequestAsync(HttpRequestMessage, CancellationToken) throws on null request.")]
        public void HttpMemoryHandlerSubmitRequestAsyncThrowsOnNullRequest()
        {
            HttpMemoryHandler handler = CreateHttpMemoryHandler();
            handler.Dispose();
            Asserters.Exception.ThrowsArgumentNull("request",
                () =>
                {
                    handler.SubmitRequestAsync(null, CancellationToken.None);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("SubmitRequestAsync(HttpRequestMessage, CancellationToken) throws after dispose.")]
        public void HttpMemoryHandlerSubmitRequestAsyncThrowsAfterDispose()
        {
            HttpMemoryHandler handler = CreateHttpMemoryHandler();
            handler.Dispose();
            Asserters.Exception.ThrowsObjectDisposed(typeof(HttpMemoryHandler).FullName,
                () =>
                {
                    handler.SubmitRequestAsync(new HttpRequestMessage(), CancellationToken.None);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("SubmitRequestAsync(HttpRequestMessage, CancellationToken) works as expected.")]
        public void HttpMemoryHandlerSubmitRequestAsync()
        {
            InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue = new InputQueue<HttpMemoryChannel.HttpMemoryRequestContext>();
            HttpMemoryHandler handler = new HttpMemoryHandler(inputQueue);
            HttpRequestMessage request = new HttpRequestMessage();

            Task<HttpResponseMessage> responseTask = handler.SubmitRequestAsync(request, CancellationToken.None);
            Assert.IsNotNull(responseTask, "response task should not be null.");
            Assert.IsFalse(responseTask.IsCompleted, "response task should not have been completed");
            Assert.IsFalse(responseTask.IsFaulted, "response task should not have been faulted");
            Assert.IsFalse(responseTask.IsCanceled, "response task should not have been cancelled");

            // Check that we can get the context out from the input queue and that it contains our request
            Assert.AreEqual(1, inputQueue.PendingCount, "Expected one enqueued context");
            HttpMemoryChannel.HttpMemoryRequestContext context = inputQueue.Dequeue(TimeSpan.MaxValue);
            Assert.IsNotNull(context, "Expected to get context from input queue.");
            
            Message message = context.RequestMessage;
            Assert.IsNotNull(message, "WCF message should not be null");
            
            HttpMessage httpMessage = message as HttpMessage;
            Assert.IsNotNull(httpMessage, "WCF message is not of expected type.");
            Assert.AreSame(request, httpMessage.GetHttpRequestMessage(false), "Didn't find expected HttpRequestMessage");
        }

        #endregion
    }
}