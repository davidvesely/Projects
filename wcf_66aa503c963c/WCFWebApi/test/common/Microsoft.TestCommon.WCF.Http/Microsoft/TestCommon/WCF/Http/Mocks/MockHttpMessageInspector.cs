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

    public class MockHttpMessageInspector : HttpMessageInspector
    {
        public Func<HttpRequestMessage, object> OnAfterReceiveRequestCallback { get; set; }
        public Action<HttpResponseMessage, object> OnBeforeSendReplyCallback { get; set; }

        protected override object OnAfterReceiveRequest(HttpRequestMessage request)
        {
            Assert.IsNotNull(this.OnAfterReceiveRequestCallback, "Set OnAfterReceiveRequestCallback first.");
            return this.OnAfterReceiveRequestCallback(request);
        }

        protected override void OnBeforeSendReply(HttpResponseMessage response, object correlationState)
        {
            Assert.IsNotNull(this.OnBeforeSendReplyCallback, "Set OnBeforeSendReplyCallback first.");
            this.OnBeforeSendReplyCallback(response, correlationState);
        }
    }
}
