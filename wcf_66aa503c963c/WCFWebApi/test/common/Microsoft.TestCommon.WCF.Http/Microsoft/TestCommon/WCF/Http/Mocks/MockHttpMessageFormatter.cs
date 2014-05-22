// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

    public class MockHttpMessageFormatter : HttpMessageFormatter
    {
        public Action<HttpRequestMessage, object[]> OnDeserializeRequestCallback { get; set; }
        public Func<object[], object, HttpResponseMessage> OnSerializeReplyCallback { get; set; }

        protected override void OnDeserializeRequest(HttpRequestMessage message, object[] parameters)
        {
            Assert.IsNotNull(this.OnDeserializeRequestCallback, "Set OnDeserializeRequestCallback first.");
            this.OnDeserializeRequestCallback(message, parameters);
        }

        protected override HttpResponseMessage OnSerializeReply(object[] parameters, object result)
        {
            Assert.IsNotNull(this.OnSerializeReplyCallback, "Set OnSerializeReplyCallback first.");
            return this.OnSerializeReplyCallback(parameters, result);
        }
    }
}
