// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// A custom HttpOperationHandlerFactory that takes delegates and call them during OnCreateRequestHandlers and OnCreateResponseHandlers
    /// </summary>
    internal class HttpConfigurationOperationHandlerFactory : HttpOperationHandlerFactory
    {
        /// <summary>
        /// Gets or sets the request handler delegate.
        /// </summary>
        /// <value>
        /// The request handler delegate.
        /// </value>
        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> RequestHandlerDelegate { get; set; }

        /// <summary>
        /// Gets or sets the response handler delegate.
        /// </summary>
        /// <value>
        /// The response handler delegate.
        /// </value>
        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> ResponseHandlerDelegate { get; set; }

        /// <summary>
        /// Called when the ordered collection of <see cref="HttpOperationHandler"/> instances is being created for
        /// the given <paramref name="operation"/>.  Can be overridden in a derived class to customize the
        /// collection of <see cref="HttpOperationHandler"/> instances returned.
        /// </summary>
        /// <param name="endpoint">The service endpoint.</param>
        /// <param name="operation">The description of the service operation.</param>
        /// <returns>
        /// The ordered collection of <see cref="HttpOperationHandler"/> instances to use when handling
        /// <see cref="System.Net.Http.HttpRequestMessage"/> instances for the given operation.
        /// </returns>
        protected override Collection<HttpOperationHandler> OnCreateRequestHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            Collection<HttpOperationHandler> requestHandlers = base.OnCreateRequestHandlers(endpoint, operation);
            if (this.RequestHandlerDelegate != null)
            {
                this.RequestHandlerDelegate(requestHandlers, endpoint, operation);
            }

            return requestHandlers;
        }

        /// <summary>
        /// Called when the ordered collection of <see cref="HttpOperationHandler"/> instances is being created for
        /// the given <paramref name="operation"/>.  Can be overridden in a derived class to customize the
        /// collection of <see cref="HttpOperationHandler"/> instances returned.
        /// </summary>
        /// <param name="endpoint">The service endpoint.</param>
        /// <param name="operation">The <see cref="HttpOperationDescription"/> for the given operation that the <see cref="HttpOperationHandler"/>
        /// instances will be associated with.</param>
        /// <returns>
        /// The ordered collection of <see cref="HttpOperationHandler"/> instances to use when handling
        /// <see cref="System.Net.Http.HttpRequestMessage"/> instances for the given operation.
        /// </returns>
        protected override Collection<HttpOperationHandler> OnCreateResponseHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            Collection<HttpOperationHandler> responseHandlers = base.OnCreateResponseHandlers(endpoint, operation);
            if (this.ResponseHandlerDelegate != null)
            {
                this.ResponseHandlerDelegate(responseHandlers, endpoint, operation);
            }

            return responseHandlers;
        }
    }
}
