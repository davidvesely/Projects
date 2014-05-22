// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Server.Common;

    /// <summary>
    /// Class that provides <see cref="IEndpointBehavior"/> for the <see cref="HttpMemoryBinding"/> binding.
    /// </summary>
    internal class HttpMemoryBehavior : IEndpointBehavior
    {
        private static readonly Type httpMemoryBehaviorType = typeof(HttpMemoryBehavior);

        /// <summary>
        /// Passes data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (bindingParameters == null)
            {
                throw Fx.Exception.ArgumentNull("bindingParameters");
            }
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw Fx.Exception.AsError(
                new NotSupportedException(SR.ApplyClientBehaviorNotSupportedByBehavior(httpMemoryBehaviorType.Name)));
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            if (endpointDispatcher == null)
            {
                throw new ArgumentNullException("endpointDispatcher");
            }

            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                OperationBehaviorAttribute operationBehavior = operationDescription.Behaviors.Find<OperationBehaviorAttribute>();
                if (operationBehavior == null)
                {
                    operationDescription.Behaviors.Add(new OperationBehaviorAttribute { AutoDisposeParameters = false });
                }
                else
                {
                    operationBehavior.AutoDisposeParameters = false;
                }
            }
        }

        /// <summary>
        /// Confirms that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }
        }
    }
}
