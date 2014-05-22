// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="ProgressMessageHandler"/> provides a mechanism for getting progress event notifications
    /// when submitting <see cref="HttpRequestMessage"/> requests and reading <see cref="HttpResponseMessage"/> responses.
    /// Register event handlers for the events <see cref="M:HttpUploadProgress"/> and <see cref="M:HttpDownloadProgress"/>
    /// to see events for uploading and downloading of data respectively.
    /// </summary>
    public class ProgressMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Occurs every HTTP request entity body data is sent to the network.
        /// </summary>
        public event HttpProgressEventHandler HttpUploadProgress;

        /// <summary>
        /// Occurs every time HTTP response entity data is read from the network.
        /// </summary>
        public event HttpProgressEventHandler HttpDownloadProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressMessageHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler to which this handler submits requests.</param>
        public ProgressMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Sends the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddWriteProgress(this, request);
            return base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    AddReadProgress(this, request, task.Result);
                    return task.Result;
                });
        }

        private static void AddWriteProgress(ProgressMessageHandler handler, HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                HttpContent progressContent = new ProgressContent(request.Content, handler, request);
                request.Content = progressContent;
            }
        }

        private static void AddReadProgress(ProgressMessageHandler handler, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                ProgressStream progressStream = new ProgressStream(response.Content.ReadAsStreamAsync().Result, handler, request, response);
                HttpContent progressContent = new StreamContent(progressStream);
                CopyContentHeaders(response.Content, progressContent);
                response.Content = progressContent;
            }
        }

        /// <summary>
        /// Raises the <see cref="M:HttpDownloadProgress"/> event.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="e">The <see cref="HttpProgressEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnHttpDownloadProgress(HttpRequestMessage request, HttpProgressEventArgs e)
        {
            if (this.HttpDownloadProgress != null)
            {
                this.HttpDownloadProgress(request, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="M:HttpUploadProgress"/> event.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="e">The <see cref="HttpProgressEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnHttpUploadProgress(HttpRequestMessage request, HttpProgressEventArgs e)
        {
            if (this.HttpUploadProgress != null)
            {
                this.HttpUploadProgress(request, e);
            }
        }

        private static void CopyContentHeaders(HttpContent from, HttpContent to)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in from.Headers)
            {
                to.Headers.AddWithoutValidation(header.Key, header.Value);
            }
        }
    }
}