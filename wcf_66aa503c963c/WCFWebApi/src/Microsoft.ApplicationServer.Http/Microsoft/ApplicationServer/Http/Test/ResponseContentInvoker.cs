// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Server.Common;

    internal class ResponseContentInvoker : IOperationInvoker
    {
        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design. Any uncaught exception will be turned into an internal server error response")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            try
            {
                string content = request.Content.ReadAsStringAsync().Result;
                if (content == null)
                {
                    return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
                }

                string format = (string)inputs[1];

                return HttpTestUtils.CreateFormattingResponse(request, format, content, false);
            }
            catch (Exception e)
            {
                return HttpTestUtils.CreateErrorResponseMessage(request, e);
            }
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
    }
}