// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Server.Common;   

    internal class RequestHeadersInvoker : IOperationInvoker
    {
        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            string headers = request.Content.ReadAsStringAsync().Result;
            int cursorPos = (int)inputs[1];

            if (cursorPos < 0 || cursorPos > headers.Length)
            {
                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
            }

            return RequestHeadersInvoker.CreateHeadersIntellisenseResponse(request, headers, cursorPos);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw Fx.Exception.AsError(new NotSupportedException());
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw Fx.Exception.AsError(new NotSupportedException());
        }

        public object[] AllocateInputs()
        {
            return new object[2];
        }

        private static HttpResponseMessage CreateHeadersIntellisenseResponse(HttpRequestMessage request, string headers, int cursorPos)
        {
            headers = HttpTestUtils.CleanString(headers, ref cursorPos);

            using (TolerantHttpHeaderTextReader reader = new TolerantHttpHeaderTextReader(headers, cursorPos))
            {
                return HttpTestUtils.CreateIntellisenseResponse(reader, request);
            }
        }
    }
}