// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Server.Common;

    /// <summary>
    /// This <see cref="IReplyChannel"/> implementation provides a transport binding element for submitting
    /// <see cref="HttpRequestMessage"/> instances for in-memory processing.
    /// </summary>
    internal class HttpMemoryChannel : ChannelBase, IReplyChannel, IDisposable
    {
        private bool disposed;
        private InputQueue<HttpMemoryRequestContext> inputQueue = new InputQueue<HttpMemoryRequestContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryChannel"/> class.
        /// </summary>
        /// <param name="listener">The <see cref="HttpMemoryChannelListener"/> that provides default timeouts for the channel operations (send, receive, open, and close).</param>
        public HttpMemoryChannel(HttpMemoryChannelListener listener)
            : base(listener)
        {
            Fx.Assert(listener != null, "listener cannot be null");
            Fx.Assert(listener.Uri != null, "listener URI cannot be null");
            this.LocalAddress = new EndpointAddress(listener.Uri);
            this.HttpMemoryHandler = new HttpMemoryHandler(this.inputQueue);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="HttpMemoryChannel"/> is reclaimed by garbage collection.
        /// </summary>
        ~HttpMemoryChannel()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets an implementation of an <see cref="HttpMemoryHandler"/> for this <see cref="HttpMemoryChannel"/> instance which can be 
        /// either accessed directly or from an <see cref="HttpClient"/> for in-memory communication.
        /// </summary>
        public HttpMemoryHandler HttpMemoryHandler { get; private set; }

        /// <summary>
        /// Gets the address on which this reply channel receives messages.
        /// </summary>
        public EndpointAddress LocalAddress { get; private set; }

        /// <summary>
        /// Begins an asynchronous operation to receive an available request with a specified timeout.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies the interval of time to wait for the reception of an available request.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous reception of the request.</returns>
        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ReceiveAsyncResult(this, timeout, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to receive an available request with a specified timeout.
        /// </summary>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous reception of the request.</returns>
        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return this.BeginReceiveRequest(this.DefaultReceiveTimeout, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to receive a request message that has a specified time out and state object associated with it. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive request operation has to complete before timing out and returning false.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous reception of the request.</returns>
        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new TryReceiveAsyncResult(this, timeout, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous request operation that has a specified time out and state object associated with it. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies the interval of time to wait for the reception of an available request.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous operation to wait for a request message to arrive.</returns>
        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.inputQueue.BeginWaitForItem(timeout, callback, state);
        }

        /// <summary>
        /// Completes an asynchronous operation to receive an available request. 
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:BeginReceive"/>.</param>
        /// <returns>The <see cref="RequestContext"/> used to construct a reply to the request. </returns>
        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            return ReceiveAsyncResult.End(result);
        }

        /// <summary>
        /// Completes the specified asynchronous operation to receive a request message.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:BeginTryReceiveRequest"/>.</param>
        /// <param name="context">The <see cref="RequestContext"/> received.</param>
        /// <returns>true if a request message is received before the specified interval of time elapses; otherwise false.</returns>
        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            return TryReceiveAsyncResult.End(result, out context);
        }

        /// <summary>
        /// Completes the specified asynchronous wait-for-a-request message operation.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> that identifies the <see cref="BeginWaitForRequest"/> operation to finish, and from which to retrieve an end result.</param>
        /// <returns>true if a request is received before the specified interval of time elapses; otherwise false.</returns>
        public bool EndWaitForRequest(IAsyncResult result)
        {
            return this.inputQueue.EndWaitForItem(result);
        }

        /// <summary>
        /// Returns the context of the request received, if one is available. If a context is not available, waits until there is one available. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive of a request operation has to complete before timing out.</param>
        /// <returns>The <see cref="RequestContext"/> used to construct replies. </returns>
        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            IAsyncResult result = new ReceiveAsyncResult(this, timeout, null, null);
            return ReceiveAsyncResult.End(result);
        }

        /// <summary>
        /// Returns the context of the request received, if one is available. If a context is not available, waits until there is one available. 
        /// </summary>
        /// <returns>The <see cref="RequestContext"/> used to construct replies.</returns>
        public RequestContext ReceiveRequest()
        {
            return this.ReceiveRequest(this.DefaultReceiveTimeout);
        }

        /// <summary>
        /// Returns a value that indicates whether a request is received before a specified interval of time elapses. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive of a request operation has to complete before timing out and returning false.</param>
        /// <param name="context">The <see cref="RequestContext"/> received.</param>
        /// <returns>true if a request message is received before the specified interval of time elapses; otherwise false.</returns>
        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            IAsyncResult result = new TryReceiveAsyncResult(this, timeout, null, null);
            return TryReceiveAsyncResult.End(result, out context);
        }

        /// <summary>
        /// Returns a value that indicates whether a request message is received before a specified interval of time elapses.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long a request operation has to complete before timing out and returning false.</param>
        /// <returns>true if a request is received before the specified interval of time elapses; otherwise false.</returns>
        public bool WaitForRequest(TimeSpan timeout)
        {
            return this.inputQueue.WaitForItem(timeout);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Cleanup();
                }

                // shared cleanup logic
                this.disposed = true;
            }
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions to the closing state due to the invocation of a synchronous close operation.
        /// </summary>
        /// <param name="timeout">The time span that specifies how long the on close operation has to complete before timing out.</param>
        protected override void OnClose(TimeSpan timeout)
        {
            this.Cleanup();
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions into the opening state which must complete within a specified interval of time.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on open operation has to complete before timing out.</param>
        protected override void OnOpen(TimeSpan timeout)
        {
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions to the opening state due to the invocation of an asynchronous open operation. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on open operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives notification of the completion of the asynchronous on open operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous on open operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous on open operation. </returns>
        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        /// <summary>
        /// Completes an asynchronous operation on the open of a communication object.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> that is returned by a call to <see cref="M:OnBeginOpen"/>.</param>
        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the closing state.
        /// </summary>
        protected override void OnClosed()
        {
            base.OnClosed();
            this.Cleanup();
        }

        /// <summary>
        /// Inserts processing after a communication object transitions to the closing state due to the invocation of an asynchronous close operation.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on close operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives notification of the completion of the asynchronous on close operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous on close operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous on close operation. </returns>
        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.Cleanup();
            return new CompletedAsyncResult(callback, state);
        }

        /// <summary>
        /// Completes an asynchronous operation on the close of a communication object.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> that is returned by a call to <see cref="M:OnBeginClose"/>.</param>
        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions to the closing state due to the invocation of a synchronous abort operation.
        /// </summary>
        protected override void OnAbort()
        {
            this.Cleanup();
        }

        private void Cleanup()
        {
            if (this.inputQueue != null)
            {
                lock (this.ThisLock)
                {
                    if (this.inputQueue != null)
                    {
                        this.inputQueue.Dispose();
                        this.inputQueue = null;
                    }
                }
            }

            if (this.HttpMemoryHandler != null)
            {
                lock (this.ThisLock)
                {
                    if (this.HttpMemoryHandler != null)
                    {
                        this.HttpMemoryHandler.Dispose();
                        this.HttpMemoryHandler = null;
                    }
                }
            }
        }

        /// <summary>
        /// Provides a reply that is correlated to an incoming request.
        /// </summary>
        public class HttpMemoryRequestContext : RequestContext
        {
            private Message request;
            private TaskCompletionSource<HttpResponseMessage> httpTaskSource;

            /// <summary>
            /// Initializes a new instance of the <see cref="HttpMemoryRequestContext"/> class.
            /// </summary>
            /// <param name="httpRequest">The <see cref="HttpRequestMessage"/> instance.</param>
            public HttpMemoryRequestContext(HttpRequestMessage httpRequest)
            {
                this.request = httpRequest.ToMessage();
                this.httpTaskSource = new TaskCompletionSource<HttpResponseMessage>();
            }

            /// <summary>
            /// Gets the message that contains the request.
            /// </summary>
            public override Message RequestMessage
            {
                get { return this.request; }
            }

            /// <summary>
            /// Gets the pending task awaiting completion.
            /// </summary>
            public Task<HttpResponseMessage> HttpRequestTask
            {
                get { return this.httpTaskSource.Task; }
            }

            /// <summary>
            /// Aborts processing the request associated with the context. 
            /// </summary>
            public override void Abort()
            {
                this.httpTaskSource.TrySetCanceled();
                this.Dispose(true);
            }

            /// <summary>
            /// Closes the operation that is replying to the request context associated with the current context.
            /// </summary>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies the interval of time within which the reply operation associated with the current context must close.</param>
            public override void Close(TimeSpan timeout)
            {
                this.Dispose(true);
            }

            /// <summary>
            /// Closes the operation that is replying to the request context associated with the current context.
            /// </summary>
            public override void Close()
            {
                this.Dispose(true);
            }

            /// <summary>
            /// Begins an asynchronous operation to reply to the request associated with the current context.
            /// </summary>
            /// <param name="message">The incoming <see cref="Message"/> that contains the request.</param>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies the interval of time to wait for the reply to an available request.</param>
            /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous reply operation completion.</param>
            /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous reply operation.</param>
            /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous reply operation.</returns>
            public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return this.BeginReply(message, callback, state);
            }

            /// <summary>
            /// Begins an asynchronous operation to reply to the request associated with the current context.
            /// </summary>
            /// <param name="message">The incoming <see cref="Message"/> that contains the request.</param>
            /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous reply operation completion.</param>
            /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous reply operation.</param>
            /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous reply operation.</returns>
            public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
            {
                this.SetResult(message);
                return new CompletedAsyncResult(callback, state);
            }

            /// <summary>
            /// Completes an asynchronous operation to reply to a request message.
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:BeginReply"/>.</param>
            public override void EndReply(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }

            /// <summary>
            /// Replies to a request message within a specified interval of time.
            /// </summary>
            /// <param name="message">The incoming <see cref="Message"/> that contains the request.</param>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies the interval of time to wait for the reply to a request.</param>
            public override void Reply(Message message, TimeSpan timeout)
            {
                this.Reply(message);
            }

            /// <summary>
            /// Replies to a request message within a specified interval of time.
            /// </summary>
            /// <param name="message">The incoming <see cref="Message"/> that contains the request.</param>
            public override void Reply(Message message)
            {
                this.SetResult(message);
            }

            private void SetResult(Message message)
            {
                Fx.Assert(message != null, "message cannot be null");
                HttpResponseMessage httpResponse = message.ExtractHttpResponseMessage();
                if (httpResponse.Content != null)
                {
                    ObjectContent objectContent = httpResponse.Content as ObjectContent;
                    if (objectContent != null)
                    {
                        objectContent.DetermineWriteSerializerAndContentType();
                    }
                }

                this.httpTaskSource.SetResult(httpResponse);
            }
        }

        /// <summary>
        /// Encapsulates the results of <see cref="BeginTryReceiveRequest"/>.
        /// </summary>
        private sealed class TryReceiveAsyncResult : ReceiveAsyncResultBase
        {
            private static AsyncCallback onReceive = new AsyncCallback(TryReceiveAsyncResult.OnReceive);
            private bool receiveSuccess;

            /// <summary>
            /// Initializes a new instance of the <see cref="TryReceiveAsyncResult"/> class.
            /// </summary>
            /// <param name="channel">The <see cref="HttpMemoryChannel"/> within which we are operating.</param>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive request operation has to complete before timing out and returning false.</param>
            /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
            /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
            public TryReceiveAsyncResult(HttpMemoryChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(channel, timeout, callback, state)
            {
                if (channel.IsDisposed)
                {
                    this.Context = null;
                    this.receiveSuccess = true;
                    this.Complete(true);
                }
                else
                {
                    this.StartReceive();
                }
            }

            /// <summary>
            /// Completes the specified asynchronous operation to receive a request.
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> instance.</param>
            /// <param name="context">The <see cref="RequestContext"/> received.</param>
            /// <returns>true if a request message is received before the specified interval of time elapses; otherwise false.</returns>
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
            public static bool End(IAsyncResult result, out RequestContext context)
            {
                TryReceiveAsyncResult thisPtr = AsyncResult.End<TryReceiveAsyncResult>(result);
                context = thisPtr.Context;
                return thisPtr.receiveSuccess;
            }

            /// <summary>
            /// Start receiving a request from the inner channel
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
            protected override void StartReceive()
            {
                if (this.IsCompleted)
                {
                    return;
                }

                try
                {
                    IAsyncResult result = this.Channel.inputQueue.BeginDequeue(this.Timeout, TryReceiveAsyncResult.onReceive, this);
                    if (result.CompletedSynchronously)
                    {
                        this.CompleteReceive(result);
                    }
                }
                catch (TimeoutException e)
                {
                    Fx.Exception.AsError(e);
                    this.Complete(true);
                }
                catch (Exception e)
                {
                    this.Complete(true, e);
                }
            }

            /// <summary>
            /// Complete receiving a request from the inner channel
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:BeginTryReceiveRequest"/>.</param>
            protected override void CompleteReceive(IAsyncResult result)
            {
                HttpMemoryRequestContext context;
                if (this.receiveSuccess = this.Channel.inputQueue.EndDequeue(result, out context))
                {
                    if (context != null)
                    {
                        this.Context = context;
                        this.Complete(result.CompletedSynchronously);
                    }
                    else
                    {
                        this.Context = null;
                        this.Complete(result.CompletedSynchronously);
                    }
                }
                else
                {
                    this.Context = null;
                    this.Complete(result.CompletedSynchronously);
                }
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
            private static void OnReceive(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                TryReceiveAsyncResult thisPtr = (TryReceiveAsyncResult)result.AsyncState;
                try
                {
                    thisPtr.CompleteReceive(result);
                }
                catch (TimeoutException e)
                {
                    Fx.Exception.AsError(e);
                    thisPtr.Complete(false);
                }
                catch (Exception e)
                {
                    thisPtr.Complete(false, e);
                }
            }
        }

        /// <summary>
        /// Encapsulates the results of <see cref="M:BeginReceiveRequest"/>.
        /// </summary>
        private sealed class ReceiveAsyncResult : ReceiveAsyncResultBase
        {
            private static AsyncCallback onReceive = new AsyncCallback(ReceiveAsyncResult.OnReceive);

            /// <summary>
            /// Initializes a new instance of the <see cref="ReceiveAsyncResult"/> class.
            /// </summary>
            /// <param name="channel">The <see cref="HttpMemoryChannel"/> within which we are operating.</param>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive request operation has to complete before timing out.</param>
            /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
            /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
            public ReceiveAsyncResult(HttpMemoryChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(channel, timeout, callback, state)
            {
                if (channel.IsDisposed)
                {
                    this.Context = null;
                    this.Complete(true);
                }
                else
                {
                    this.StartReceive();
                }
            }

            /// <summary>
            /// Completes the specified asynchronous operation to receive a request.
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> instance.</param>
            /// <returns>The <see cref="RequestContext"/> used to construct a reply to the request. </returns>
            public static RequestContext End(IAsyncResult result)
            {
                ReceiveAsyncResult thisPtr = AsyncResult.End<ReceiveAsyncResult>(result);
                return thisPtr.Context;
            }

            /// <summary>
            /// Start receiving a request from the inner channel
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
            protected override void StartReceive()
            {
                if (this.IsCompleted)
                {
                    return;
                }

                try
                {
                    IAsyncResult result = this.Channel.inputQueue.BeginDequeue(this.Timeout, ReceiveAsyncResult.onReceive, this);
                    if (result.CompletedSynchronously)
                    {
                        this.CompleteReceive(result);
                    }
                }
                catch (TimeoutException e)
                {
                    Fx.Exception.AsInformation(e);
                    this.Complete(true, e);
                }
                catch (Exception e)
                {
                    this.Complete(true, e);
                }
            }

            /// <summary>
            /// Complete receiving a request from the inner channel
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:BeginReceiveRequest"/>.</param>
            protected override void CompleteReceive(IAsyncResult result)
            {
                HttpMemoryRequestContext context = this.Channel.inputQueue.EndDequeue(result);
                if (context != null)
                {
                    this.Context = context;
                    this.Complete(result.CompletedSynchronously);
                }
                else
                {
                    this.Context = null;
                    this.Complete(result.CompletedSynchronously);
                }
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
            private static void OnReceive(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReceiveAsyncResult thisPtr = (ReceiveAsyncResult)result.AsyncState;
                try
                {
                    thisPtr.CompleteReceive(result);
                }
                catch (TimeoutException e)
                {
                    Fx.Exception.AsInformation(e);
                    thisPtr.Complete(false, e);
                }
                catch (Exception e)
                {
                    thisPtr.Complete(false, e);
                }
            }
        }

        /// <summary>
        /// Base class encapsulating the shared results of <see cref="M:BeginTryReceiveRequest"/> and <see cref="M:BeginReceiveRequest"/>.
        /// </summary>
        private abstract class ReceiveAsyncResultBase : AsyncResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReceiveAsyncResultBase"/> class.
            /// </summary>
            /// <param name="channel">The <see cref="HttpMemoryChannel"/> within which we are operating.</param>
            /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the receive request operation has to complete before timing out.</param>
            /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous receive that a request operation completes.</param>
            /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous receive of a request operation.</param>
            protected ReceiveAsyncResultBase(HttpMemoryChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Channel = channel;
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the <see cref="HttpMemoryChannel"/> within which we are operating.
            /// </summary>
            protected HttpMemoryChannel Channel { get; private set; }

            /// <summary>
            /// Gets the <see cref="TimeSpan"/> that specifies how long the receive request operation has to complete before timing out.
            /// </summary>
            protected TimeSpan Timeout { get; private set; }

            /// <summary>
            /// Gets or sets the <see cref="RequestContext"/> representing the reply.
            /// </summary>
            protected HttpMemoryRequestContext Context { get; set; }

            /// <summary>
            /// Start receiving a request from the inner channel
            /// </summary>
            protected abstract void StartReceive();

            /// <summary>
            /// Complete receiving a request from the inner channel
            /// </summary>
            /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to BeginReceiveRequest or BeginTryReceiveRequest.</param>
            protected abstract void CompleteReceive(IAsyncResult result);
        }
    }
}
