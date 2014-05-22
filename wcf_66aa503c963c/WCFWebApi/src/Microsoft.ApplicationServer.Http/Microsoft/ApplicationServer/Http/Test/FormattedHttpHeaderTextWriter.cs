// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class FormattedHttpHeaderTextWriter : IDisposable
    {
        internal FormattedHttpHeaderTextWriter(TextWriter innerWriter, IEnumerable<HttpTestUtils.HttpHeaderInfo> headers)
        {
            this.InnerWriter = innerWriter;
            this.Headers = headers;
        }

        private TextWriter InnerWriter { get; set; }

        private IEnumerable<HttpTestUtils.HttpHeaderInfo> Headers { get; set; }

        public void WriteAllHeaders()
        {
            foreach (HttpTestUtils.HttpHeaderInfo header in this.Headers)
            {
                this.InnerWriter.WriteLine("{0}:{1}", header.Key, header.Value);
            }
        }

        public void Dispose()
        {
            // nop, don't dispose InnerWriter
            GC.SuppressFinalize(this);
        }
    }
}
