// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.ApplicationServer.Http.Test;
    using Microsoft.Server.Common;

    /// <summary>
    /// Class that provides <see cref="IEndpointBehavior"/> for the <see cref="HttpBinding"/> binding.
    /// </summary>
    public class HttpBehavior : IEndpointBehavior
    {
        internal const string WildcardAction = "*";
        internal const string WildcardMethod = "*";
        internal const bool DefaultHelpEnabled = false;
        internal const bool DefaultTestClientEnabled = false;
        internal const TrailingSlashMode DefaultTrailingSlashMode = TrailingSlashMode.AutoRedirect;

        private static readonly Type xmlSerializerFormatAttributeType = typeof(XmlSerializerFormatAttribute);
        private static readonly Type messageContractAttributeType = typeof(MessageContractAttribute);
        private static readonly Type httpMessageEncodingBindingElementType = typeof(HttpMessageEncodingBindingElement);
        private static readonly Type httpMemoryBindingElementType = typeof(HttpMemoryTransportBindingElement);
        private static readonly Type transportBindingElementType = typeof(TransportBindingElement);
        private static readonly Type messageEncodingBindingElementType = typeof(MessageEncodingBindingElement);
        private static readonly Type httpBehaviorType = typeof(HttpBehavior);

        private TrailingSlashMode trailingSlashMode;
        private HttpOperationHandlerFactory httpOperationHandlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpBehavior"/> class.
        /// </summary>
        public HttpBehavior()
        {
            this.HelpEnabled = DefaultHelpEnabled;
            this.TestClientEnabled = DefaultTestClientEnabled;
            this.TrailingSlashMode = DefaultTrailingSlashMode;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an automatic help page will be
        /// available.
        /// </summary>
        public bool HelpEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a web-based test client will be
        /// available.
        /// </summary>
        public bool TestClientEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value specifying how trailing slashes will be handled
        /// with URI-based operation selection.
        /// </summary>
        public TrailingSlashMode TrailingSlashMode
        {
            get
            {
                return this.trailingSlashMode;
            }

            set
            {
                TrailingSlashModeHelper.Validate(value, "value");
                this.trailingSlashMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="OperationHandlerFactory"/>.
        /// </summary>
        /// <value>
        /// The value returned will never be <c>null</c>.  
        /// If no <see cref="HttpOperationHandlerFactory"/> has been set,
        /// the default factory will be returned.
        /// </value>
        public HttpOperationHandlerFactory OperationHandlerFactory
        {
            get
            {
                if (this.httpOperationHandlerFactory == null)
                {
                    this.httpOperationHandlerFactory = new HttpOperationHandlerFactory();
                }

                return this.httpOperationHandlerFactory;
            }

            set
            {
                this.httpOperationHandlerFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the error handler delegate.
        /// </summary>
        /// <value>
        /// The error handler delegate.
        /// </value>
        internal Action<Collection<HttpErrorHandler>, ServiceEndpoint, IEnumerable<HttpOperationDescription>> ErrorHandlerDelegate { get; set; }

        /// <summary>
        /// Gets or sets the http service instance provider
        /// </summary>
        /// <value>
        /// The instance provider.
        /// </value>
        internal HttpInstanceProvider InstanceProvider { get; set; }

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

            this.OnAddBindingParameters(endpoint, bindingParameters);
        }

        /// <summary>
        /// Implements a modification or extension of the client across and endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized</param>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is the pattern used by all behaviors.")]
        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw Fx.Exception.AsError(
                new NotSupportedException(SR.ApplyClientBehaviorNotSupportedByBehavior(httpBehaviorType.Name)));
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

            this.OnValidate(endpoint);
        }

        /// <summary>
        /// Allows for extension or modification of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (endpointDispatcher == null)
            {
                throw Fx.Exception.ArgumentNull("endpointDispatcher");
            }

            this.OnApplyDispatchBehavior(endpoint, endpointDispatcher);
        }

        /// <summary>
        /// Generates a ContractDescription that can be used for generating the help page.
        /// </summary>
        /// <param name="originalContract"><see cref="ContractDescription"/> of the Service endpoint for which the help page contract description is generated.</param>
        /// <returns><see cref="ContractDescription"/> that should be used for the HelpPage generation.</returns>
        internal static ContractDescription GenerateClientContractDescription(ContractDescription originalContract)
        {
            if (originalContract == null)
            {
                return null;
            }

            ContractDescription contract = new ContractDescription(originalContract.Name, originalContract.Namespace)
            {
                ProtectionLevel = originalContract.ProtectionLevel,
                SessionMode = originalContract.SessionMode,
                CallbackContractType = originalContract.CallbackContractType,
                ConfigurationName = originalContract.ConfigurationName,
                ContractType = originalContract.ContractType, // this will point to the original contract
            };

            // add contract behaviors
            foreach (IContractBehavior behavior in originalContract.Behaviors)
            {
                contract.Behaviors.Add(behavior);
            }

            // add operations
            foreach (OperationDescription operationDescription in originalContract.Operations)
            {
                contract.Operations.Add(ClientContractDescriptionHelper.GetEquivalentOperationDescription(operationDescription));
            }

            return contract;
        }

        /// <summary>
        /// Override in a derived class to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <remarks>This base implementation does nothing.</remarks>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        protected virtual void OnAddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // do nothing
        }

        /// <summary>
        /// Override in a derived class to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <remarks>
        /// This base implementation provides a some standard validation for the endpoint and the service operations, that
        /// derived implementations should generally leverage.
        /// </remarks>
        /// <param name="endpoint">The endpoint to validate.</param>
        protected virtual void OnValidate(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            ValidateEndpoint(endpoint);

            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                ValidateOperationDescription(operationDescription);
            }
        }

        /// <summary>
        /// Override in a derived class to extend or modify the behavior of the service across an endpoint.
        /// </summary>
        /// <remarks>
        /// This base implementation sets up the proper operation dispatcher, formatter, and error handler.
        /// Derived implementations should always call the base.
        /// </remarks>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This behavior is the core of web api and that is why it has quite some coupling here.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This behavior is the core of web api and that is why it has quite some complex logic here.")]
        protected virtual void OnApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (endpointDispatcher == null)
            {
                throw Fx.Exception.ArgumentNull("endpointDispatcher");
            }

            Uri helpUri = null;
            OperationDescription[] helpOperations = null;
            if (this.HelpEnabled)
            {
                helpUri = new UriTemplate(HelpPage.OperationListHelpPageUriTemplate).BindByPosition(endpoint.ListenUri);
                helpOperations = HelpPage.AddHelpOperations(endpoint.Contract, endpointDispatcher.DispatchRuntime);
            }

            OperationDescription[] testOperations = null;
            HttpEndpoint httpEndpoint = endpoint as HttpEndpoint;
            if (this.TestClientEnabled && httpEndpoint != null)
            {
                testOperations = HttpTestUtils.AddHttpTestOperations(httpEndpoint, endpointDispatcher.DispatchRuntime);
            }

            List<HttpOperationDescription> httpOperations = new List<HttpOperationDescription>();
            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();
                httpOperations.Add(httpOperationDescription);
            }

            // endpoint filter
            endpointDispatcher.AddressFilter = new PrefixEndpointAddressMessageFilter(endpoint.Address);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();

            // operation selector
            endpointDispatcher.DispatchRuntime.OperationSelector = this.OnGetOperationSelector(endpoint, httpOperations);
            UriAndMethodOperationSelector httpOperationSelector = endpointDispatcher.DispatchRuntime.OperationSelector as UriAndMethodOperationSelector;
            if (httpOperationSelector != null)
            {
                httpOperationSelector.HelpPageUri = helpUri;
            }

            // unhandled operation
            string actionStarOperationName = null;
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                if (operation.Messages[0].Direction == MessageDirection.Input &&
                    operation.Messages[0].Action == WildcardAction)
                {
                    actionStarOperationName = operation.Name;
                    break;
                }
            }

            if (actionStarOperationName != null)
            {
                endpointDispatcher.DispatchRuntime.Operations.Add(
                    endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation);
            }

            // clean up the httpoperations so it does not contain the test client operations or help page operations
            List<HttpOperationDescription> systemOperations = new List<HttpOperationDescription>();
            foreach (HttpOperationDescription httpOperation in httpOperations)
            {
                OperationDescription operationDescription = httpOperation.ToOperationDescription();
                if ((testOperations != null && testOperations.Contains<OperationDescription>(operationDescription)) ||
                     (helpOperations != null && helpOperations.Contains<OperationDescription>(operationDescription)))
                {
                    systemOperations.Add(httpOperation);
                }
            }

            foreach (HttpOperationDescription httpOperation in systemOperations)
            {
                httpOperations.Remove(httpOperation);
            }

            // message formatters
            this.ApplyMessageFormatters(endpoint, endpointDispatcher, systemOperations, true);
            this.ApplyMessageFormatters(endpoint, endpointDispatcher, httpOperations, false);

            HttpErrorHandlerPipeline errorHandlerPipeline = new HttpErrorHandlerPipeline();

            // add any user error handlers
            IEnumerable<HttpErrorHandler> errorHandlers = this.OnGetHttpErrorHandlers(endpoint, httpOperations);
            if (errorHandlers != null)
            {
                foreach (HttpErrorHandler errorHandler in errorHandlers)
                {
                    errorHandlerPipeline.HttpErrorHandlers.Add(errorHandler);
                }
            }

            // add the default error handler
            HttpErrorHandler defaultErrorHandler = new HttpResponseErrorHandler(
                this.OperationHandlerFactory.Formatters,
                helpUri,
                endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults);
            errorHandlerPipeline.HttpErrorHandlers.Add(defaultErrorHandler);

            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(errorHandlerPipeline);

            // remove the help operations from the contract if they were added
            if (helpOperations != null)
            {
                foreach (OperationDescription helpOperation in helpOperations)
                {
                    if (endpoint.Contract.Operations.Contains(helpOperation))
                    {
                        endpoint.Contract.Operations.Remove(helpOperation);
                    }
                }
            }

            // remove the test operations from the contract if they were added
            if (testOperations != null)
            {
                foreach (OperationDescription testOperation in testOperations)
                {
                    if (endpoint.Contract.Operations.Contains(testOperation))
                    {
                        endpoint.Contract.Operations.Remove(testOperation);
                    }
                }
            }

            if (this.InstanceProvider != null)
            {
                endpointDispatcher.DispatchRuntime.InstanceProvider = this.InstanceProvider;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="HttpErrorHandler"/> instances for the given
        /// set of <paramref name="operations"/> for the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        /// <param name="endpoint">The endpoint whose error handlers are required.</param>
        /// <param name="operations">The set of <see cref="HttpOperationDescription"/>s.</param>
        /// <returns>The set of error handlers. The default is an empty collection.</returns>
        protected virtual Collection<HttpErrorHandler> OnGetHttpErrorHandlers(ServiceEndpoint endpoint, IEnumerable<HttpOperationDescription> operations)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            Collection<HttpErrorHandler> errorHandlers = new Collection<HttpErrorHandler>();
            if (this.ErrorHandlerDelegate != null)
            {
                this.ErrorHandlerDelegate(errorHandlers, endpoint, operations);
            }

            return errorHandlers;
        }

        /// <summary>
        /// Gets the <see cref="HttpOperationSelector"/> to use for the given set
        /// of <paramref name="operations"/> for the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <remarks>The base implementation returns the default <see cref="UriAndMethodOperationSelector"/>.
        /// </remarks>
        /// <param name="endpoint">The endpoint exposing the operations.</param>
        /// <param name="operations">The set of <see cref="HttpOperationDescription"/>.</param>
        /// <returns>The <see cref="HttpOperationSelector"/> to use.</returns>
        protected virtual HttpOperationSelector OnGetOperationSelector(ServiceEndpoint endpoint, IEnumerable<HttpOperationDescription> operations)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            return new UriAndMethodOperationSelector(endpoint.Address.Uri, operations, this.TrailingSlashMode);
        }

        /// <summary>
        /// Gets the <see cref="HttpMessageFormatter"/> to use for the given 
        /// <paramref name="operation"/> for the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <remarks>
        /// The base implementation returns an <see cref="HttpMessageFormatter"/> with the
        /// <see cref="HttpOperationHandler"/> instances applied to the given operation.
        /// </remarks>
        /// <param name="endpoint">The endpoint exposing the operations.</param>
        /// <param name="operation">The <see cref="HttpOperationDescription"/>.</param>
        /// <returns>The <see cref="HttpMessageFormatter"/> to use for the <paramref name="operation"/>.</returns>
        protected virtual HttpMessageFormatter OnGetMessageFormatter(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (operation == null)
            {
                throw Fx.Exception.ArgumentNull("operation");
            }

            OperationHandlerPipeline pipeline = null;
            Collection<HttpOperationHandler> requestHandlers = this.OperationHandlerFactory.CreateRequestHandlers(endpoint, operation);
            Collection<HttpOperationHandler> responseHandlers = this.OperationHandlerFactory.CreateResponseHandlers(endpoint, operation);
            pipeline = new OperationHandlerPipeline(requestHandlers, responseHandlers, operation);
            OperationHandlerFormatter formatter = new OperationHandlerFormatter(pipeline);
            return formatter;
        }

        private static void ValidateEndpoint(ServiceEndpoint endpoint)
        {
            Fx.Assert(endpoint != null, "The 'endpoint' parameter should not be null.");

            // Ensure no message headers
            if (endpoint.Address != null && endpoint.Address.Headers.Count > 0)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.HttpServiceEndpointCannotHaveMessageHeaders(
                            endpoint.Address)));
            }

            // Ensure http(s) scheme
            Binding binding = endpoint.Binding;
            if (binding == null)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(SR.HttpBehaviorBindingRequired(httpBehaviorType.Name)));
            }

            if (binding.Scheme != Uri.UriSchemeHttp && binding.Scheme != Uri.UriSchemeHttps)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidUriScheme(
                            endpoint.Address.Uri.AbsoluteUri)));
            }

            // Ensure MessageVersion.None
            if (binding.MessageVersion != MessageVersion.None)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidMessageVersion(
                            endpoint.Address.Uri.AbsoluteUri)));
            }

            // Ensure manual addressing
            TransportBindingElement transportBindingElement = binding.CreateBindingElements().Find<TransportBindingElement>();
            Fx.Assert(transportBindingElement != null, "The MessageVersion check would have failed if there is not a transportBindingElement.");
            if (!transportBindingElement.ManualAddressing)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidManualAddressingValue(
                            endpoint.Address.Uri.AbsoluteUri)));
            }

            // Ensure HttpMessageEncodingBindingElement or HttpMemoryBindingElement
            BindingElementCollection bindingElementCollection = binding.CreateBindingElements();
            HttpMessageEncodingBindingElement httpMessageEncodingBindingElement = bindingElementCollection.Find<HttpMessageEncodingBindingElement>();
            HttpMemoryTransportBindingElement httpMemoryBindingElement = bindingElementCollection.Find<HttpMemoryTransportBindingElement>();
            if (httpMessageEncodingBindingElement == null && httpMemoryBindingElement == null)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidMessageEncodingBindingElement(
                            endpoint.Address.Uri.AbsoluteUri,
                            messageEncodingBindingElementType.Name,
                            httpMessageEncodingBindingElementType.Name,
                            transportBindingElementType.Name,
                            httpMemoryBindingElementType.Name)));
            }
        }

        private static void ValidateOperationDescription(OperationDescription operation)
        {
            Fx.Assert(operation != null, "The 'operationDescription' parameter should not be null.");

            // Ensure no operations with XmlSerializer Rpc style
            XmlSerializerOperationBehavior xmlSerializerOperationBehavior = operation.Behaviors.Find<XmlSerializerOperationBehavior>();
            if (xmlSerializerOperationBehavior != null &&
                (xmlSerializerOperationBehavior.XmlSerializerFormatAttribute.Style == OperationFormatStyle.Rpc))
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidXmlSerializerFormatAttribute(
                            operation.Name,
                            operation.DeclaringContract.Name,
                            xmlSerializerFormatAttributeType.Name)));
            }

            // Ensure operations don't have message headers
            if (operation.Messages[0].Headers.Count > 0 ||
                (operation.Messages.Count > 1 &&
                 operation.Messages[1].Headers.Count > 0))
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidOperationWithMessageHeaders(
                            operation.Name,
                            operation.DeclaringContract.Name)));
            }

            // Ensure operations don't have typed messages
            if (operation.Messages[0].MessageType != null)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidMessageContractParameter(
                            operation.Name,
                            operation.DeclaringContract.Name,
                            messageContractAttributeType.Name,
                            operation.Messages[0].MessageType.Name)));
            }

            if (operation.Messages.Count > 1 &&
                operation.Messages[1].MessageType != null)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        SR.InvalidMessageContractParameter(
                            operation.Name,
                            operation.DeclaringContract.Name,
                            messageContractAttributeType.Name,
                            operation.Messages[1].MessageType.Name)));
            }
        }

        /// <summary>
        /// Applies the message formatters.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher.</param>
        /// <param name="httpOperations">The HTTP operations.</param>
        /// <param name="isSystemOperation">true if the operation is help page operation or test client operation</param>
        private void ApplyMessageFormatters(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher, List<HttpOperationDescription> httpOperations, bool isSystemOperation)
        {
            Fx.Assert(endpoint != null, "endpoint cannot be null.");
            Fx.Assert(endpointDispatcher != null, "endpointDispatcher cannot be null");
            Fx.Assert(httpOperations != null, "httpOperations cannot be null.");

            foreach (HttpOperationDescription httpOperationDescription in httpOperations)
            {
                DispatchOperation dispatchOperation = null;
                if (endpointDispatcher.DispatchRuntime.Operations.Contains(httpOperationDescription.Name))
                {
                    dispatchOperation = endpointDispatcher.DispatchRuntime.Operations[httpOperationDescription.Name];
                }
                else if (endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Name == httpOperationDescription.Name)
                {
                    dispatchOperation = endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation;
                }

                if (dispatchOperation != null)
                {
                    if (isSystemOperation)
                    {
                        // for special operation such as help page or test client
                        HttpOperationHandlerFactory factory = new HttpOperationHandlerFactory();
                        Collection<HttpOperationHandler> requestHandlers = factory.CreateRequestHandlers(endpoint, httpOperationDescription);
                        Collection<HttpOperationHandler> responseHandlers = factory.CreateResponseHandlers(endpoint, httpOperationDescription);
                        dispatchOperation.Formatter = new OperationHandlerFormatter(new OperationHandlerPipeline(requestHandlers, responseHandlers, httpOperationDescription));
                    }
                    else
                    {
                        // for normal application operation
                        dispatchOperation.Formatter = this.OnGetMessageFormatter(endpoint, httpOperationDescription);
                    }

                    dispatchOperation.DeserializeRequest = true;
                    dispatchOperation.SerializeReply = !dispatchOperation.IsOneWay;
                }
            }
        }
    }
}