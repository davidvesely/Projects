// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;

    public delegate bool TryComputeLengthDelegate(out long length);

    public class MockHttpContent : HttpContent
    {
        public MockHttpContent()
        {
        }

        public MockHttpContent(HttpContent innerContent)
        {
            this.InnerContent = innerContent;
            this.Headers.ContentType = innerContent.Headers.ContentType;
        }

        public MockHttpContent(MediaTypeHeaderValue contentType)
        {
            Assert.IsNotNull(contentType, "ContentType cannot be null.");
            this.Headers.ContentType = contentType;
        }

        public MockHttpContent(string contentType)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(contentType), "ContentType cannot be null.");
            this.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        public HttpContent InnerContent { get; set; }

        public Action<bool> DisposeCallback { get; set; }
        public TryComputeLengthDelegate TryComputeLengthCallback { get; set; }
        public Action<Stream, TransportContext> SerializeToStreamCallback { get; set; }
        public Func<Stream, TransportContext, Task> SerializeToStreamAsyncCallback { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (this.DisposeCallback != null)
            {
                this.DisposeCallback(disposing);
            }

            base.Dispose(disposing);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (this.SerializeToStreamAsyncCallback != null)
            {
                return this.SerializeToStreamAsyncCallback(stream, context);
            }
            else if (this.InnerContent != null)
            {
                return this.InnerContent.CopyToAsync(stream, context);
            }
            else
            {
                Assert.Fail("Construct with inner HttpContent or set SerializeToStreamCallback first.");
            }

            return null;
        }

        protected override bool TryComputeLength(out long length)
        {
            if (this.TryComputeLengthCallback != null)
            {
                return this.TryComputeLengthCallback(out length);
            }

            if (this.InnerContent != null)
            {
                long? len = this.InnerContent.Headers.ContentLength;
                length = len.HasValue ? len.Value : 0L;
                return len.HasValue;
            }

            length = 0L;
            return false;
        }
    }
}
