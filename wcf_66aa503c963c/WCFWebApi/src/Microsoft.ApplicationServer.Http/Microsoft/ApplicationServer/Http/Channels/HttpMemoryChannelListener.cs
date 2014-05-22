// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading;
    using Microsoft.Server.Common;

    /// <summary>
    /// Defines a <see cref="IChannelListener"/> for creating <see cref="HttpMemoryChannel"/> instances.
    /// </summary>
    internal class HttpMemoryChannelListener : ChannelListenerBase<IReplyChannel>
    {
        private volatile InputQueue<IReplyChannel> channelQueue = new InputQueue<IReplyChannel>();
        private InputQueue<HttpMemoryHandler> handlerQueue;
        private int channelCount;
        private Uri listeningAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryChannelListener"/> class.
        /// </summary>
        /// <param name="bindingElement">The <see cref="BindingElement"/> using this <see cref="HttpMemoryChannelListener"/>.</param>
        /// <param name="context">The <see cref="BindingContext"/> used to create this <see cref="HttpMemoryChannelListener"/>.</param>
        /// <param name="handlerQueue">Input queue where <see cref="HttpMemoryHandler"/> instances will be sent.</param>
        public HttpMemoryChannelListener(HttpMemoryTransportBindingElement bindingElement, BindingContext context, InputQueue<HttpMemoryHandler> handlerQueue)
            : base(context.Binding)
        {
            Fx.Assert(bindingElement != null, "Binding element cannot be null");
            Fx.Assert(context != null, "Context cannot be null");
            Fx.Assert(handlerQueue != null, "handlerQueue cannot be null.");
            this.listeningAddress = InitializeUri(bindingElement, context);
            this.handlerQueue = handlerQueue;
        }

        /// <summary>
        /// Gets the URI on which this channel listener listens for an incoming channel.
        /// </summary>
        public override Uri Uri
        {
            get { return this.listeningAddress; }
        }

        /// <summary>
        /// Returns a typed object requested, if present, from the appropriate layer in the channel stack.
        /// </summary>
        /// <typeparam name="T">The typed object for which the method is querying.</typeparam>
        /// <returns>The typed object T requested if it is present or null if it is not.</returns>
        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageVersion.None;
            }

            return base.GetProperty<T>();
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions to the closing state due to the invocation of a synchronous abort operation. 
        /// </summary>
        protected override void OnAbort()
        {
            this.Cleanup();
        }

        /// <summary>
        /// Inserts processing on a communication object after it transitions to the closing state due to the invocation of a synchronous close operation. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on close operation has to complete before timing out.</param>
        protected override void OnClose(TimeSpan timeout)
        {
            this.Cleanup();
        }

        /// <summary>
        /// Begins an asynchronous operation to close a communication object with a specified timeout.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the close operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives notification of the completion of the asynchronous close operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous close operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous close operation.</returns>
        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnClose(timeout);
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
        /// Inserts processing on a communication object after it transitions into the opening state which must complete within a specified interval of time. 
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on open operation has to complete before timing out.</param>
        protected override void OnOpen(TimeSpan timeout)
        {
            this.EnqueueChannel();
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
        /// <param name="result">The <see cref="IAsyncResult"/> that is returned by a call to <see cref="M:BeginOpen"/>.</param>
        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        /// <summary>
        /// When implemented in a derived class, provides an extensibility point when accepting a channel.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the accept channel operation has to complete before timing out.</param>
        /// <returns>The <see cref="IReplyChannel"/> accepted.</returns>
        protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
        {
            return this.channelQueue.Dequeue(timeout);
        }

        /// <summary>
        /// Provides an asynchronous extensibility point when beginning to accept a channel.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the accept channel operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous completion of the accept channel operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous accept channel operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous accept channel operation.</returns>
        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.channelQueue.BeginDequeue(timeout, callback, state);
        }

        /// <summary>
        /// Provides an asynchronous extensibility point when completing the acceptance a channel.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:OnBeginAcceptChannel"/>.</param>
        /// <returns>The <see cref="IReplyChannel"/> accepted by the listener.</returns>
        protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return this.channelQueue.EndDequeue(result);
        }

        /// <summary>
        /// Provides a point of extensibility when starting to wait for a channel to arrive.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on begin wait operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate that receives the notification of the asynchronous operation on begin wait completion.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous on begin wait operation.</param>
        /// <returns>The <see cref="IAsyncResult"/> that references the asynchronous on begin wait operation. </returns>
        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.channelQueue.BeginWaitForItem(timeout, callback, state);
        }

        /// <summary>
        /// Provides a point of extensibility when ending the waiting for a channel to arrive.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> returned by a call to <see cref="M:OnBeginWaitForChannel"/>.</param>
        /// <returns>true if the method completed before the interval of time specified by the timeout expired; otherwise false.</returns>
        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.channelQueue.EndWaitForItem(result);
        }

        /// <summary>
        /// Provides a point of extensibility when waiting for a channel to arrive.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> that specifies how long the on wait for a channel operation has to complete before timing out.</param>
        /// <returns>true if the method completed before the interval of time specified by the timeout expired; otherwise false.</returns>
        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return this.channelQueue.WaitForItem(timeout);
        }

        /// <summary>
        /// Initializes the listening URI
        /// </summary>
        /// <param name="binding">The binding for which we are generating a listening URI.</param>
        /// <param name="context">The context used to create the listening <see cref="Uri"/>.</param>
        /// <returns>The listening <see cref="Uri"/>.</returns>
        private static Uri InitializeUri(HttpMemoryTransportBindingElement binding, BindingContext context)
        {
            Fx.Assert(binding != null, "Binding cannot be null");
            Fx.Assert(context != null, "Context cannot be null");
            Uri baseAddress = context.ListenUriBaseAddress;
            string relativeAddress = context.ListenUriRelativeAddress;

            if (context.ListenUriMode != ListenUriMode.Explicit)
            {
                throw Fx.Exception.Argument(
                    "context",
                     Http.SR.HttpMemoryChannelExplicitListenUriMode(Enum.GetName(typeof(ListenUriMode), ListenUriMode.Explicit), "ListenUriMode", typeof(HttpMemoryChannel).Name));
            }

            if (baseAddress == null)
            {
                throw Fx.Exception.Argument(
                    "context",
                    Http.SR.HttpMemoryChannelNullListenUri(typeof(BindingContext).Name, "ListenUriBaseAddress"));
            }

            if (!baseAddress.IsAbsoluteUri)
            {
                throw Fx.Exception.Argument(
                    "context",
                    Http.SR.HttpMemoryChannelRelativeListenUri(baseAddress, typeof(BindingContext).Name, "ListenUriBaseAddress"));
            }

            if (string.Compare(baseAddress.Scheme, binding.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw Fx.Exception.Argument(
                    "context",
                    Http.SR.HttpMemoryChannelBadListenUriScheme(baseAddress.Scheme, typeof(BindingContext).Name, "ListenUriBaseAddress", Uri.UriSchemeHttp));
            }

            if (relativeAddress == null)
            {
                throw Fx.Exception.Argument(
                    "context",
                    Http.SR.HttpMemoryChannelNullListenUri(typeof(BindingContext).Name, "ListenUriRelativeAddress"));
            }

            return new Uri(baseAddress, relativeAddress);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        private void EnqueueChannel()
        {
            if (!this.IsDisposed)
            {
                if (Interlocked.CompareExchange(ref this.channelCount, 1, 0) == 0)
                {
                    HttpMemoryChannel channel = new HttpMemoryChannel(this);
                    channel.Opened += new EventHandler(this.OnChannelOpened);
                    channel.Closed += new EventHandler(this.OnChannelClosed);
                    this.channelQueue.EnqueueAndDispatch(channel);
                }
            }
        }

        private void OnChannelOpened(object sender, EventArgs e)
        {
            HttpMemoryChannel channel = sender as HttpMemoryChannel;
            Fx.Assert(channel != null, "Channel null or invalid type");
            this.handlerQueue.EnqueueAndDispatch(channel.HttpMemoryHandler);
        }

        private void OnChannelClosed(object sender, EventArgs e)
        {
            int count = Interlocked.Decrement(ref this.channelCount);
            Fx.Assert(count >= 0, "Channel count cannot be less than zero");
            this.EnqueueChannel();
        }

        private void Cleanup()
        {
            if (this.channelQueue != null)
            {
                lock (this.ThisLock)
                {
                    if (this.channelQueue != null)
                    {
                        this.channelQueue.Dispose();
                        this.channelQueue = null;
                    }
                }
            }
        }
    }
}
