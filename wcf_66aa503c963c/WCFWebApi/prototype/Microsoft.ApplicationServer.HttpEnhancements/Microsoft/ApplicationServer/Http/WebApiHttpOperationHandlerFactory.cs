// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net.Http.Formatting;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class WebApiHttpOperationHandlerFactory : HttpOperationHandlerFactory
    {
        public WebApiHttpOperationHandlerFactory() 
        { 
        }
        
        public WebApiHttpOperationHandlerFactory(IEnumerable<MediaTypeFormatter> formatters) : base(formatters) 
        { 
        }

        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> RequestHandlerDelegate { get; set; }

        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> ResponseHandlerDelegate { get; set; }

        protected override Collection<HttpOperationHandler> OnCreateRequestHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            var requestHandlers = base.OnCreateRequestHandlers(endpoint, operation);
            if (this.RequestHandlerDelegate != null)
            {
                this.RequestHandlerDelegate(requestHandlers, endpoint, operation);
            }

            return requestHandlers;
        }

        protected override Collection<HttpOperationHandler> OnCreateResponseHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            var responseHandlers = base.OnCreateResponseHandlers(endpoint, operation);
            responseHandlers.Add(new JsonpHttpResponseHandler());
            if (this.ResponseHandlerDelegate != null)
            {
                this.ResponseHandlerDelegate(responseHandlers, endpoint, operation);
            }

            return responseHandlers;
        }
    }
}
