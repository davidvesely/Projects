// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Web.Script.Serialization;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;

    internal class ServiceResourceInvoker : IOperationInvoker
    {
        private HttpEndpoint endpoint;
        private HttpTestUtils.ServiceInfo serviceInfo;

        public ServiceResourceInvoker(HttpEndpoint endpoint)
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

                // will return url with internal host name, such as http://ComputerName/servicename
                // in Azure case, the internal host name is not accessible from external browser clients
                // so web api test client must fix the host name to the real one in browser address bar
                return this.serviceInfo;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            outputs = new object[0];
            return HttpTestUtils.CreateStringResponse(request, this.GetSerializedServiceInfo(), MediaTypeConstants.ApplicationJsonMediaType);
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
            return new object[1];
        }

        private string GetSerializedServiceInfo()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(this.ServiceInfo);
        }
    }
}
