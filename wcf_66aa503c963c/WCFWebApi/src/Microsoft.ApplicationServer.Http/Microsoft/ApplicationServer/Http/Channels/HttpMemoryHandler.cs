// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Server.Common;

    /// <summary>
    /// Defines an implementation of an <see cref="HttpMessageHandler"/> which can be 
    /// used to submit HTTP requests to bindings using the <see cref="HttpMemoryTransportBindingElement"/> 
    /// </summary>
    public class HttpMemoryHandler : HttpMessageHandler
    {
        private bool disposed;
        private InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryHandler"/> class.
        /// </summary>
        /// <param name="inputQueue">The <see cref="InputQueue{T}"/> to use for enqueing messages.</param>
        internal HttpMemoryHandler(InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue)
        {
            Fx.Assert(inputQueue != null, "input queue cannot be null");
            this.inputQueue = inputQueue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryHandler"/> class.
        /// </summary>
        protected HttpMemoryHandler()
        {
        }

        /// <summary>
        /// Enqueues the request into an <see cref="InputQueue{T}"/> and dispatches to Service Model.
        /// </summary>
        /// <param name="request"><see cref="HttpRequestMessage"/> to submit</param>
        /// <param name="cancellationToken">Token used to cancel operation.</param>
        /// <returns>A Task for handling the request.</returns>
        public Task<HttpResponseMessage> SubmitRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return this.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                // Note: We don't dispose this.inputQueue but rather leave it to the caller constructing this class to dispose.
                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Enqueues the request into an <see cref="InputQueue{T}"/> and dispatches to Service Model.
        /// </summary>
        /// <param name="request"><see cref="HttpRequestMessage"/> to submit</param>
        /// <param name="cancellationToken">Token used to cancel operation.</param>
        /// <remarks>Requests submitted to the <see cref="HttpMemoryChannel"/> can only be cancelled 
        /// from the WCF Service Model side, not from the caller side.</remarks>
        /// <returns>A Task for handling the request.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            HttpMemoryChannel.HttpMemoryRequestContext context = new HttpMemoryChannel.HttpMemoryRequestContext(request);
            this.inputQueue.EnqueueAndDispatch(context, null, false);
            return context.HttpRequestTask;
        }
    }
}