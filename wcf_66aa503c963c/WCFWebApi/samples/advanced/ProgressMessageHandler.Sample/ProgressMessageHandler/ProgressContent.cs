// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class ProgressContent : HttpContent
    {
        private HttpContent innerContent;
        private ProgressMessageHandler handler;
        private HttpRequestMessage request;

        public ProgressContent(HttpContent innerContent, ProgressMessageHandler handler, HttpRequestMessage request)
        {
            Contract.Assert(innerContent != null, "inner content cannot be null");
            Contract.Assert(handler != null, "handler cannot be null");
            Contract.Assert(request != null, "request cannot be null");

            this.innerContent = innerContent;
            this.handler = handler;
            this.request = request;

            foreach (KeyValuePair<string, IEnumerable<string>> header in innerContent.Headers)
            {
                this.Headers.AddWithoutValidation(header.Key, header.Value);
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            ProgressStream progressStream = new ProgressStream(stream, this.handler, this.request);
            return this.innerContent.CopyToAsync(progressStream);
        }

        protected override bool TryComputeLength(out long length)
        {
            long? contentLength = this.innerContent.Headers.ContentLength;
            if (contentLength.HasValue)
            {
                length = contentLength.Value;
                return true;
            }

            length = -1;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.innerContent.Dispose();
            }
        }
    }
}