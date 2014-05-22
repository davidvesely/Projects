// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;

    internal class RequestUriInvoker : IOperationInvoker
    {
        private HttpEndpoint endpoint;
        private HttpTestUtils.ServiceInfo serviceInfo;

        public RequestUriInvoker(HttpEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        private HttpTestUtils.ServiceInfo ServiceInfo
        {
            get
            {
                if (this.serviceInfo == null)
                {
                    this.serviceInfo = HttpTestUtils.GetServiceInfo(this.endpoint);
                }

                return this.serviceInfo;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            string uriStr = request.Content.ReadAsStringAsync().Result;

            string uriTemplateStr = (string)inputs[1];
            string method = (string)inputs[2];
            string operation = (string)inputs[3];

            if (string.Equals(operation, "autocomplete", StringComparison.OrdinalIgnoreCase))
            {
                int cursorPos = (int)inputs[4];
                return this.CreateParameterIntellisenseResponse(request, uriStr, method, uriTemplateStr, cursorPos);
            }
            else if (string.Equals(operation, "validate", StringComparison.OrdinalIgnoreCase))
            {
                return this.CreateValidationResponse(request, uriTemplateStr, uriStr);
            }
            else
            {
                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
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
            return new object[5];
        }

        private HttpResponseMessage CreateValidationResponse(HttpRequestMessage request, string uriTemplateStr, string uriStr)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriStr, UriKind.Absolute);
            }
            catch (UriFormatException)
            {
                return HttpTestUtils.CreateBoolResponse(request, false);
            }

            // uriTemplateStr is in absolute address used to retreive the resource
            HttpTestUtils.ResourceInfo resource = this.ServiceInfo.GetResourceInfo(uriTemplateStr);
            if (resource != null)
            {
                UriTemplate template = resource.UriTemplate;
                if (template.Match(resource.Endpoint.Address.Uri.GetHostNormalizedUri(request), uri) != null)
                {
                    return HttpTestUtils.CreateBoolResponse(request, true);
                }
                else
                {
                    return HttpTestUtils.CreateBoolResponse(request, false);
                }
            }
            else
            {
                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private HttpResponseMessage CreateParameterIntellisenseResponse(HttpRequestMessage request, string uriStr, string method, string resourceUriTemplate, int cursorPos)
        {
            HttpTestUtils.OperationInfo operation = this.ServiceInfo.GetOperationInfo(resourceUriTemplate, method);
            if (operation != null)
            {
                using (TolerantUriTextReader reader = new TolerantUriTextReader(uriStr, cursorPos, operation.ResourceInfo.Endpoint.Address.Uri.GetHostNormalizedUri(request), operation.ResourceInfo.UriTemplate, operation.Description))
                {
                    try
                    {
                        return HttpTestUtils.CreateIntellisenseResponse(reader, request);
                    }
                    catch (TolerantUriTextReader.UriReaderException e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
        }
    }
}
