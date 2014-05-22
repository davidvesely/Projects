// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockObjectContent : ObjectContent
    {
        public MockObjectContent(Type type, object value) : base(type, value) { }
        public MockObjectContent(Type type, object value, IEnumerable<MediaTypeFormatter> formatters) : base(type, value, formatters) { }
        public MockObjectContent(Type type, HttpContent value) : base(type, value) { }
        public MockObjectContent(Type type, object value, string mediaType) : base(type, value, mediaType) { }
        public MockObjectContent(Type type, object value, string mediaType, IEnumerable<MediaTypeFormatter> formatters) : base(type, value, mediaType, formatters) { }
        public MockObjectContent(Type type, object value, MediaTypeHeaderValue mediaType) : base(type, value, mediaType) { }
        public MockObjectContent(Type type, object value, MediaTypeHeaderValue mediaType, IEnumerable<MediaTypeFormatter> formatters) : base(type, value, mediaType, formatters) { }

        public bool CallBase { get; set; }
        public Func<Stream, TransportContext, Task> SerializeToStreamAsyncCallback { get; set; }

        public object ValueProperty
        {
            get
            {
                return GetValueProperty(this);
            }
        }

        public HttpContent HttpContentProperty
        {
            get
            {
                return GetHttpContentProperty(this);
            }
        }

        public static object GetValueProperty(ObjectContent content)
        {
            PropertyInfo property = content.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return property.GetValue(content, null);
        }

        public static HttpContent GetHttpContentProperty(ObjectContent content)
        {
            PropertyInfo property = content.GetType().GetProperty("HttpContent", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return property.GetValue(content, null) as HttpContent;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Assert.IsTrue(this.CallBase || this.SerializeToStreamAsyncCallback != null, "CallBase or SerializeToStreamAsyncCallback lambda must be set first.");
            return this.SerializeToStreamAsyncCallback != null ? this.SerializeToStreamAsyncCallback(stream, context) : base.SerializeToStreamAsync(stream, context);
        }
    }
}
