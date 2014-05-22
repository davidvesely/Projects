// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;

    /// <summary>
    /// Class that provides a <see cref="ServiceHost"/> for the <see cref="HttpBinding"/> binding.
    /// </summary>
    public class HttpServiceHost : ServiceHost
    {
        private static readonly string httpServiceHostTypeName = typeof(HttpServiceHost).Name;
       
        private static readonly string[] httpEndpointUriSchemes = new string[] 
        {
            Uri.UriSchemeHttp,
            Uri.UriSchemeHttps
        };

        private static readonly string[] httpMemoryEndpointUriSchemes = new string[] 
        {
            Uri.UriSchemeHttp
        };

        private HttpConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="singletonInstance">The instance of the hosted service.</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, ValidateBaseAddresses(baseAddresses))
        {
            this.SetAspNetCompatibilityRequirements();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="singletonInstance">The instance of the hosted service.</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(object singletonInstance, HttpConfiguration configuration, params Uri[] baseAddresses)
            : base(singletonInstance, ValidateBaseAddresses(baseAddresses))
        {
            if (configuration == null)
            {
                throw Fx.Exception.ArgumentNull("configuration");
            }

            this.SetAspNetCompatibilityRequirements();
            this.InitializeConfiguration(configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, ValidateBaseAddresses(baseAddresses))
        {
            this.SetAspNetCompatibilityRequirements();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(Type serviceType, HttpConfiguration configuration, params Uri[] baseAddresses)
            : base(serviceType, ValidateBaseAddresses(baseAddresses))
        {
            if (configuration == null)
            {
                throw Fx.Exception.ArgumentNull("configuration");
            }

            this.SetAspNetCompatibilityRequirements();
            this.InitializeConfiguration(configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="singletonInstance">The instance of the hosted service.</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(object singletonInstance, params string[] baseAddresses)
            : base(singletonInstance, CreateUriBaseAddresses(baseAddresses))
        {
            this.SetAspNetCompatibilityRequirements();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="singletonInstance">The instance of the hosted service.</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(object singletonInstance, HttpConfiguration configuration, params string[] baseAddresses)
            : base(singletonInstance, CreateUriBaseAddresses(baseAddresses))
        {
            if (configuration == null)
            {
                throw Fx.Exception.ArgumentNull("configuration");
            }

            this.SetAspNetCompatibilityRequirements();
            this.InitializeConfiguration(configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(Type serviceType, params string[] baseAddresses)
            : base(serviceType, CreateUriBaseAddresses(baseAddresses))
        {
            this.SetAspNetCompatibilityRequirements();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        public HttpServiceHost(Type serviceType, HttpConfiguration configuration, params string[] baseAddresses)
            : base(serviceType, CreateUriBaseAddresses(baseAddresses))
        {
            if (configuration == null)
            {
                throw Fx.Exception.ArgumentNull("configuration");
            }

            this.SetAspNetCompatibilityRequirements();
            this.InitializeConfiguration(configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        protected HttpServiceHost()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceHost"/> class.
        /// </summary>
        /// <param name="configuration">The configuration</param>
        protected HttpServiceHost(HttpConfiguration configuration)
            : base()
        {
            this.InitializeConfiguration(configuration);
        }

        /// <summary>
        /// Adds service endpoints for all base addresses in each contract found in the service host
        /// with the default binding.
        /// </summary>
        /// <returns>A read-only collection of default endpoints.</returns>
        public override ReadOnlyCollection<ServiceEndpoint> AddDefaultEndpoints()
        {
            if (this.Description != null &&
                this.BaseAddresses.Count > 0 &&
                (this.Description.Endpoints == null || this.Description.Endpoints.Count == 0))
            {
                List<ServiceEndpoint> defaultEndpoints = new List<ServiceEndpoint>();

                if (this.ImplementedContracts.Count != 1)
                {
                    throw new InvalidOperationException(SR.DefaultEndpointsServiceWithMultipleContracts(this.Description.Name, httpServiceHostTypeName));
                }

                ContractDescription contractDescription = this.ImplementedContracts.Values.First();
                ServiceDebugBehavior debugBehavior = this.Description.Behaviors.Find<ServiceDebugBehavior>();
                foreach (Uri baseAddress in this.BaseAddresses)
                {
                    if (Object.ReferenceEquals(baseAddress.Scheme, Uri.UriSchemeHttp) || Object.ReferenceEquals(baseAddress.Scheme, Uri.UriSchemeHttps))
                    {
                        HttpEndpoint endpoint = new HttpEndpoint(contractDescription, new EndpointAddress(baseAddress));
                        BindingElementCollection bindingElementCollection = endpoint.Binding.CreateBindingElements();

                        InitializeEndpointBehavior(bindingElementCollection, endpoint, debugBehavior);

                        if (this.configuration != null)
                        {
                            this.configuration.ConfigureEndpoint(endpoint);
                        }

                        defaultEndpoints.Add(endpoint);
                        this.AddServiceEndpoint(endpoint);
                    }
                }

                return new ReadOnlyCollection<ServiceEndpoint>(defaultEndpoints);
            }

            throw Fx.Exception.AsError(new InvalidOperationException(SR.DefaultEndpointsMustBeAddedFirst));
        }

        /// <summary>
        /// Adds the HTTP endpoint with configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <returns>The http endpoint.</returns>
        /// <exception cref="UriFormatException" />
        /// <exception cref="ArgumentNullException" />
        public HttpEndpoint AddHttpEndpoint(Type implementedContract, string address)
        {
            if (address == null)
            {
                throw Fx.Exception.ArgumentNull("address");
            }

            return this.AddHttpEndpoint(implementedContract, new Uri(address, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Adds the HTTP endpoint with configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <returns>The http endpoint.</returns>
        public HttpEndpoint AddHttpEndpoint(Type implementedContract, Uri address)
        {
            if (implementedContract == null)
            {
                throw Fx.Exception.ArgumentNull("implementedContract");
            }

            if (address == null)
            {
                throw Fx.Exception.ArgumentNull("address");
            }

            Uri absoluteAddress = CreateAbsoluteEndpointAddress(address, httpEndpointUriSchemes, this.BaseAddresses);
            HttpEndpoint endpoint = new HttpEndpoint(this.GetContract(implementedContract), new EndpointAddress(absoluteAddress));
            if (this.configuration != null)
            {
                this.configuration.ConfigureEndpoint(endpoint);
            }

            this.AddServiceEndpoint(endpoint);
            return endpoint;
        }

        /// <summary>
        /// Adds an <see cref="HttpMemoryEndpoint"/> endpoint with default configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <returns>The configured <see cref="HttpMemoryEndpoint"/>.</returns>
        public HttpMemoryEndpoint AddHttpMemoryEndpoint(Type implementedContract, string address)
        {
            if (address == null)
            {
                throw Fx.Exception.ArgumentNull("address");
            }

            return this.AddHttpMemoryEndpoint(implementedContract, new Uri(address, UriKind.RelativeOrAbsolute), null);
        }

        /// <summary>
        /// Adds an <see cref="HttpMemoryEndpoint"/> endpoint with configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <param name="httpMemoryConfiguration">The <see cref="HttpMemoryConfiguration"/> to apply to this endpoint.</param>
        /// <returns>The configured <see cref="HttpMemoryEndpoint"/>.</returns>
        public HttpMemoryEndpoint AddHttpMemoryEndpoint(Type implementedContract, string address, HttpMemoryConfiguration httpMemoryConfiguration)
        {
            if (address == null)
            {
                throw Fx.Exception.ArgumentNull("address");
            }

            return this.AddHttpMemoryEndpoint(implementedContract, new Uri(address, UriKind.RelativeOrAbsolute), httpMemoryConfiguration);
        }

        /// <summary>
        /// Adds an <see cref="HttpMemoryEndpoint"/> endpoint with default configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <returns>The configured <see cref="HttpMemoryEndpoint"/>.</returns>
        public HttpMemoryEndpoint AddHttpMemoryEndpoint(Type implementedContract, Uri address)
        {
            return this.AddHttpMemoryEndpoint(implementedContract, address, null);
        }

        /// <summary>
        /// Adds an <see cref="HttpMemoryEndpoint"/> endpoint with configuration.
        /// </summary>
        /// <param name="implementedContract">The implemented contract.</param>
        /// <param name="address">The address.</param>
        /// <param name="httpMemoryConfiguration">The <see cref="HttpMemoryConfiguration"/> to apply to this endpoint.</param>
        /// <returns>The configured <see cref="HttpMemoryEndpoint"/>.</returns>
        public HttpMemoryEndpoint AddHttpMemoryEndpoint(Type implementedContract, Uri address, HttpMemoryConfiguration httpMemoryConfiguration)
        {
            if (implementedContract == null)
            {
                throw Fx.Exception.ArgumentNull("implementedContract");
            }

            if (address == null)
            {
                throw Fx.Exception.ArgumentNull("address");
            }

            Uri absoluteAddress = CreateAbsoluteEndpointAddress(address, httpMemoryEndpointUriSchemes, this.BaseAddresses);
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(this.GetContract(implementedContract), new EndpointAddress(absoluteAddress));
            if (httpMemoryConfiguration != null)
            {
                httpMemoryConfiguration.ConfigureEndpoint(endpoint);
            }

            this.AddServiceEndpoint(endpoint);
            return endpoint;
        }

        /// <summary>
        /// Creates a description of the service hosted.
        /// </summary>
        /// <param name="implementedContracts">The <see cref="T:System.Collections.Generic.IDictionary`2"/> with key pairs of type (<see cref="T:System.String"/>, <see cref="T:System.ServiceModel.Description.ContractDescription"/>) that contains the keyed-contracts of the hosted service that have been implemented.</param>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.Description.ServiceDescription"/> of the hosted service.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The type of service hosted is null.</exception>
        protected override ServiceDescription CreateDescription(out IDictionary<string, ContractDescription> implementedContracts)
        {
            ServiceDescription serviceDescription = base.CreateDescription(out implementedContracts);
            if (serviceDescription != null && implementedContracts != null)
            {
                if (implementedContracts.Count == 0)
                {
                    ContractDescription contractDescription = ContractDescription.GetContract(new ServiceContractTypeDelegator(serviceDescription.ServiceType));
                    contractDescription.ContractType = serviceDescription.ServiceType;
                    implementedContracts[contractDescription.ConfigurationName] = contractDescription;
                }
            }

            return serviceDescription;
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the opening state.
        /// </summary>
        protected override void OnOpening()
        {
            if (this.Description == null)
            {
                return;
            }

            DisableServiceDebugAndMetadataBehaviors(this.Description);
            ServiceDebugBehavior debugBehavior = this.Description.Behaviors.Find<ServiceDebugBehavior>();
            foreach (ServiceEndpoint serviceEndpoint in this.Description.Endpoints)
            {
                if (serviceEndpoint.Binding != null)
                {
                    BindingElementCollection bindingElementCollection = serviceEndpoint.Binding.CreateBindingElements();
                    InitializeEndpointBehavior(bindingElementCollection, serviceEndpoint, debugBehavior);

                    if (bindingElementCollection.Find<HttpMessageEncodingBindingElement>() != null ||
                        bindingElementCollection.Find<HttpMemoryTransportBindingElement>() != null)
                    {
                        if (serviceEndpoint.Behaviors.Find<HttpBehavior>() == null)
                        {
                            serviceEndpoint.Behaviors.Add(new HttpBehavior());
                        }
                    }

                    if (serviceEndpoint.Binding is HttpMemoryBinding)
                    {
                        if (serviceEndpoint.Behaviors.Find<HttpMemoryBehavior>() == null)
                        {
                            serviceEndpoint.Behaviors.Add(new HttpMemoryBehavior());
                        }
                    }
                }
            }

            base.OnOpening();
        }

        private static void InitializeEndpointBehavior(BindingElementCollection bindingElementCollection, ServiceEndpoint serviceEndpoint, ServiceDebugBehavior debugBehavior)
        {
            Fx.Assert(bindingElementCollection != null, "bindingElementCollection cannot be null");
            Fx.Assert(serviceEndpoint != null, "serviceEndpoint cannot be null");
            if (bindingElementCollection.Find<HttpMessageHandlerBindingElement>() != null)
            {
                DispatcherSynchronizationBehavior synchronizationBehavior = serviceEndpoint.Behaviors.Find<DispatcherSynchronizationBehavior>();
                if (synchronizationBehavior == null)
                {
                    synchronizationBehavior = new DispatcherSynchronizationBehavior() { AsynchronousSendEnabled = true };
                    serviceEndpoint.Behaviors.Add(synchronizationBehavior);
                }

                if (serviceEndpoint.Behaviors.Find<HttpBindingParameterBehavior>() == null)
                {
                    serviceEndpoint.Behaviors.Add(new HttpBindingParameterBehavior(debugBehavior, synchronizationBehavior));
                }
            }
        }

        private static Uri CreateAbsoluteEndpointAddress(Uri address, string[] allowedUriSchemes, IEnumerable<Uri> baseAddresses)
        {
            Fx.Assert(address != null, "endpoint address cannot be null");
            Fx.Assert(allowedUriSchemes != null, "allowed URI schemes cannot be null");
            if (address.IsAbsoluteUri)
            {
                if (!allowedUriSchemes.Contains(address.Scheme))
                {
                    throw Fx.Exception.AsError(new ArgumentException(SR.InvalidUriScheme(address.AbsoluteUri), "address"));
                }

                return address;
            }

            if (baseAddresses != null)
            {
                foreach (string uriScheme in allowedUriSchemes)
                {
                    foreach (Uri baseAddress in baseAddresses)
                    {
                        if (baseAddress.Scheme == uriScheme)
                        {
                            return new Uri(baseAddress, address);
                        }
                    }
                }
            }

            throw Fx.Exception.AsError(new ArgumentException(SR.InvalidUriScheme(address.ToString()), "address"));
        }

        private static Uri[] CreateUriBaseAddresses(string[] baseAddresses)
        {
            if (baseAddresses == null)
            {
                throw Fx.Exception.ArgumentNull("baseAddresses");
            }

            List<Uri> uris = new List<Uri>();

            foreach (string baseAddress in baseAddresses)
            {
                if (!string.IsNullOrWhiteSpace(baseAddress))
                {
                    uris.Add(new Uri(baseAddress, UriKind.RelativeOrAbsolute));
                }
            }

            return ValidateBaseAddresses(uris.ToArray());
        }

        private static Uri[] ValidateBaseAddresses(Uri[] baseAddresses)
        {
            if (baseAddresses == null)
            {
                throw Fx.Exception.ArgumentNull("baseAddresses");
            }

            List<Uri> uris = new List<Uri>();

            foreach (Uri baseAddress in baseAddresses)
            {
                if (!baseAddress.IsAbsoluteUri)
                {
                    throw Fx.Exception.Argument("baseAddresses", SR.InvalidUriNotAbsolute(baseAddress.ToString()));
                }

                if (!string.Equals(baseAddress.Scheme, Uri.UriSchemeHttp) && !string.Equals(baseAddress.Scheme, Uri.UriSchemeHttps))
                {
                    throw Fx.Exception.Argument("baseAddresses", SR.InvalidBaseUriScheme(baseAddress.Scheme, Uri.UriSchemeHttp, Uri.UriSchemeHttps));
                }

                if (baseAddress != null)
                {
                    uris.Add(baseAddress);
                }
            }

            return uris.ToArray();
        }

        private static void DisableServiceDebugAndMetadataBehaviors(ServiceDescription serviceDescription)
        {
            Fx.Assert(serviceDescription != null, "The 'serviceDescription' parameter should not be null.");

            ServiceDebugBehavior serviceDebugBehavior = serviceDescription.Behaviors.Find<ServiceDebugBehavior>();
            if (serviceDebugBehavior != null)
            {
                serviceDebugBehavior.HttpHelpPageEnabled = false;
                serviceDebugBehavior.HttpsHelpPageEnabled = false;
            }

            ServiceMetadataBehavior serviceMetadataBehavior = serviceDescription.Behaviors.Find<ServiceMetadataBehavior>();
            if (serviceMetadataBehavior != null)
            {
                serviceMetadataBehavior.HttpGetEnabled = false;
                serviceMetadataBehavior.HttpsGetEnabled = false;
            }
        }

        private void InitializeConfiguration(HttpConfiguration config)
        {
            this.configuration = config;
            if (this.configuration != null)
            {
                this.configuration.ConfigureServiceHost(this);
            }
        }

        private void SetAspNetCompatibilityRequirements()
        {
            AspNetCompatibilityRequirementsAttribute aspNetCompatibilityBehavior = this.Description.Behaviors.Find<AspNetCompatibilityRequirementsAttribute>();
            if (aspNetCompatibilityBehavior == null)
            {
                aspNetCompatibilityBehavior = new AspNetCompatibilityRequirementsAttribute() { RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed };
                this.Description.Behaviors.Add(aspNetCompatibilityBehavior);
            }

            if (aspNetCompatibilityBehavior.RequirementsMode == AspNetCompatibilityRequirementsMode.NotAllowed)
            {
                aspNetCompatibilityBehavior.RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed;
            }
        }

        /// <summary>
        /// Returns either a new <see cref="ContractDescription"/> or an already existing instance associated with
        /// another <see cref="HttpMemoryEndpoint"/> or <see cref="HttpEndpoint"/> endpoint.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>A new or existing <see cref="ContractDescription"/> instance.</returns>
        private ContractDescription GetContract(Type contractType)
        {
            Fx.Assert(contractType != null, "contract type cannot be null");

            // First look in implemented contracts
            ContractDescription contractDescription;            
            if (this.ImplementedContracts.TryGetValue(contractType.FullName, out contractDescription))
            {
                return contractDescription;
            }

            return ContractDescription.GetContract(new ServiceContractTypeDelegator(contractType));
        }
    }
}
