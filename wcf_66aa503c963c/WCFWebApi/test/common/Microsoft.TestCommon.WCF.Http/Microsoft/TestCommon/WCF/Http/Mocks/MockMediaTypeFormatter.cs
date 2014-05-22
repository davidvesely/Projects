// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockMediaTypeFormatter : MediaTypeFormatter
    {
        public bool CallBase { get; set; }
        public Func<Type, bool> CanReadTypeCallback { get; set; }
        public Func<Type, bool> CanWriteTypeCallback { get; set; }
        public Func<Type, Stream, HttpContentHeaders, object> OnReadFromStreamCallback { get; set; }
        public Func<Type, Stream, HttpContentHeaders, Task<object>> OnReadFromStreamAsyncCallback { get; set; }
        public Action<Type, object, Stream, HttpContentHeaders, TransportContext> OnWriteToStreamCallback { get; set; }
        public Func<Type, object, Stream, HttpContentHeaders, TransportContext, Task> OnWriteToStreamAsyncCallback { get; set; }
        public Func<Type, string, HttpResponseMessage, IEnumerable<KeyValuePair<string, string>>> OnGetResponseHeadersCallback { get; set; }

        protected override bool CanReadType(Type type)
        {
            Assert.IsTrue(this.CallBase || this.CanReadTypeCallback != null, "CallBase or CanReadTypeCallback must be set first.");
            return this.CanReadTypeCallback != null ? this.CanReadTypeCallback(type) : base.CanReadType(type);
        }

        protected override bool CanWriteType(Type type)
        {
            Assert.IsTrue(this.CallBase || this.CanWriteTypeCallback != null, "CallBase or CanWriteTypeCallback must be set first.");
            return this.CanWriteTypeCallback != null ? this.CanWriteTypeCallback(type) : base.CanReadType(type);
        }

        protected override IEnumerable<KeyValuePair<string, string>> OnGetResponseHeaders(Type objectType, string mediaType, HttpResponseMessage responseMessage)
        {
            Assert.IsTrue(this.CallBase || this.OnGetResponseHeadersCallback != null, "CallBase or GetResponseHeadersCallback must be set first.");
            return this.OnGetResponseHeadersCallback != null
                    ? this.OnGetResponseHeadersCallback(objectType, mediaType, responseMessage)
                    : base.OnGetResponseHeaders(objectType, mediaType, responseMessage);
        }
 
        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            Assert.IsNotNull(this.OnReadFromStreamCallback, "OnReadFromStreamCallback must be set first.");
            return this.OnReadFromStreamCallback(type, stream, contentHeaders);
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            Assert.IsTrue(this.CallBase || this.OnReadFromStreamAsyncCallback != null, "CallBase or OnReadFromStreamCallback must be set first.");
            return this.OnReadFromStreamAsyncCallback != null
                    ? this.OnReadFromStreamAsyncCallback(type, stream, contentHeaders)
                    : base.OnReadFromStreamAsync(type, stream, contentHeaders);
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            Assert.IsNotNull(this.OnWriteToStreamCallback, "OnWriteToStreamCallback must be set first.");
            this.OnWriteToStreamCallback(type, value, stream, contentHeaders, context);
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            Assert.IsTrue(this.CallBase || this.OnWriteToStreamAsyncCallback != null, "CallBase or OnWriteToStreamAsyncCallback must be set first.");
            return this.OnWriteToStreamAsyncCallback != null
                    ? this.OnWriteToStreamAsyncCallback(type, value, stream, contentHeaders, context)
                    : base.OnWriteToStreamAsync(type, value, stream, contentHeaders, context);
        }
    }
}
