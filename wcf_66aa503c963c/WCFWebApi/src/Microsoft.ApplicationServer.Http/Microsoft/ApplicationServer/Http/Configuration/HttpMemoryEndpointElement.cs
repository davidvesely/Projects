// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;
    using Microsoft.ServiceModel.Configuration;

    /// <summary>
    /// Class that provides an endpoint element for the <see cref="HttpMemoryBinding"/> binding.
    /// </summary>
    public sealed class HttpMemoryEndpointElement : StandardEndpointElement
    {
        private const string HttpBindingCollectionElementName = "httpMemoryBinding";
        private const string NameAsKeyPropertyName = "name";
        private static readonly Type httpClientType = typeof(HttpClient);
        private static readonly Type httpMemoryEndpointType = typeof(HttpMemoryEndpoint);
        private ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryEndpoint"/> class.
        /// </summary>
        public HttpMemoryEndpointElement()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the automatic help page will be available.
        /// </summary>
        [ConfigurationProperty(HttpConfigurationStrings.HelpEnabled, DefaultValue = HttpBehavior.DefaultHelpEnabled)]
        public bool HelpEnabled
        {
            get { return (bool)base[HttpConfigurationStrings.HelpEnabled]; }
            set { base[HttpConfigurationStrings.HelpEnabled] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the web-based test client will be available.
        /// </summary>
        [ConfigurationProperty(HttpConfigurationStrings.TestClientEnabled, DefaultValue = HttpBehavior.DefaultTestClientEnabled)]
        public bool TestClientEnabled
        {
            get { return (bool)base[HttpConfigurationStrings.TestClientEnabled]; }
            set { base[HttpConfigurationStrings.TestClientEnabled] = value; }
        }

        /// <summary>
        /// Gets or sets a value specifying how trailing slashes in the <see cref="Uri"/> will be handled.
        /// </summary>
        [ConfigurationProperty(HttpConfigurationStrings.TrailingSlashMode, DefaultValue = HttpBehavior.DefaultTrailingSlashMode)]
        [ServiceModelEnumValidator(typeof(TrailingSlashModeHelper))]
        public TrailingSlashMode TrailingSlashMode
        {
            get { return (TrailingSlashMode)base[HttpConfigurationStrings.TrailingSlashMode]; }
            set { base[HttpConfigurationStrings.TrailingSlashMode] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="OperationHandlerFactory"/>.
        /// </summary>
        [ConfigurationProperty(HttpConfigurationStrings.OperationHandlerFactory, DefaultValue = "")]
        [StringValidator(MinLength = 0)]
        public string OperationHandlerFactory
        {
            get
            {
                return (string)base[HttpConfigurationStrings.OperationHandlerFactory];
            }

            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    value = String.Empty;
                }

                base[HttpConfigurationStrings.OperationHandlerFactory] = value;
            }
        }

        /// <summary>
        /// Gets the type of the endpoint.
        /// </summary>
        protected override Type EndpointType
        {
            get 
            {
                return httpMemoryEndpointType;
            }
        }

        /// <summary>
        /// Gets the collection of properties for the current endpoint element.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection localProperties = new ConfigurationPropertyCollection();

                    localProperties.Add(new ConfigurationProperty(NameAsKeyPropertyName, typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey));
                    localProperties.Add(new ConfigurationProperty(HttpConfigurationStrings.HelpEnabled, typeof(bool), HttpBehavior.DefaultHelpEnabled, null, null, ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(HttpConfigurationStrings.TestClientEnabled, typeof(bool), HttpBehavior.DefaultTestClientEnabled, null, null, ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(HttpConfigurationStrings.TrailingSlashMode, typeof(TrailingSlashMode), HttpBehavior.DefaultTrailingSlashMode, null, new ServiceModelEnumValidator(typeof(TrailingSlashModeHelper)), ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(HttpConfigurationStrings.OperationHandlerFactory, typeof(string), string.Empty, null, new System.Configuration.StringValidator(0), System.Configuration.ConfigurationPropertyOptions.None));
                    this.properties = localProperties;
                }

                return this.properties;
            }
        }

        /// <summary>
        /// When implemented in a derived class, creates a service endpoint from message information
        /// contained in the specified <see cref="ContractDescription"/>.
        /// </summary>
        /// <param name="contractDescription">Information about what is contained in messages
        /// being sent from an endpoint.</param>
        /// <returns>A service endpoint.</returns>
        protected override ServiceEndpoint CreateServiceEndpoint(ContractDescription contractDescription)
        {
            return new HttpMemoryEndpoint(contractDescription);
        }

        /// <summary>
        /// When called from a derived class, initializes and verifies the format of the
        /// specified service endpoint element in a service application configuration file.
        /// </summary>
        /// <param name="channelEndpointElement">An endpoint element that defineds the physical transfer
        /// of messages back and forth between the client and the service.</param>
        protected override void OnInitializeAndValidate(ChannelEndpointElement channelEndpointElement)
        {
            throw Fx.Exception.AsError(
                new NotSupportedException(
                        Http.SR.HttpEndpointNotSupported(
                        httpMemoryEndpointType.Name,
                        httpClientType.Name)));
        }

        /// <summary>
        /// When called from a derived class, initializes and verifies the format of the
        /// specified service endpoint element in a service application configuration file.
        /// </summary>
        /// <param name="serviceEndpointElement">A service endpoint element that enables clients to find and
        /// communicate with a service.</param>
        protected override void OnInitializeAndValidate(ServiceEndpointElement serviceEndpointElement)
        {
            if (string.IsNullOrEmpty(serviceEndpointElement.Binding))
            {
                serviceEndpointElement.Binding = HttpBindingCollectionElementName;
            }
            else if (!string.Equals(serviceEndpointElement.Binding, HttpBindingCollectionElementName, StringComparison.Ordinal))
            {
                throw Fx.Exception.AsError(
                    new NotSupportedException(
                        Http.SR.HttpEndpointRequiredBinding(
                            typeof(HttpMemoryEndpoint).Name,
                            HttpBindingCollectionElementName)));              
            }
        }

        /// <summary>
        /// When called from a derived class, loads the service description information
        /// from the configuration file and applies it to the runtime being constructed.
        /// </summary>
        /// <param name="endpoint">And endpoint that enables clients to find and communicate with a service.</param>
        /// <param name="serviceEndpointElement">A service endpoint element of a service application.</param>
        protected override void OnApplyConfiguration(ServiceEndpoint endpoint, ServiceEndpointElement serviceEndpointElement)
        {
            this.InternalOnApplyConfiguration(endpoint);
        }

        /// <summary>
        /// When called from a derived class, loads the service description information
        /// from the configuration file and applies it to the runtime being constructed.
        /// </summary>
        /// <param name="endpoint">And endpoint that enables clients to find and communicate with a service.</param>
        /// <param name="channelEndpointElement">An endpoint element that defined the physical
        /// transfer of messages ack and forth between the client and the service.</param>
        protected override void OnApplyConfiguration(ServiceEndpoint endpoint, ChannelEndpointElement channelEndpointElement)
        {
            throw Fx.Exception.AsError(
                new NotSupportedException(
                    Http.SR.HttpEndpointNotSupported(
                        httpMemoryEndpointType.Name,
                        httpClientType.Name)));
        }

        private void InternalOnApplyConfiguration(ServiceEndpoint endpoint)
        {
            HttpMemoryEndpoint httpMemoryEndpoint = endpoint as HttpMemoryEndpoint;
            Fx.Assert(httpMemoryEndpoint != null, "The endpoint should be of type HttpMemoryEndpoint since this is what was returned with CreateServiceEndpoint().");

            if (this.IsSet(HttpConfigurationStrings.HelpEnabled))
            {
                httpMemoryEndpoint.HelpEnabled = this.HelpEnabled;
            }

            if (this.IsSet(HttpConfigurationStrings.TestClientEnabled))
            {
                httpMemoryEndpoint.TestClientEnabled = this.TestClientEnabled;
            }

            if (this.IsSet(HttpConfigurationStrings.TrailingSlashMode))
            {
                httpMemoryEndpoint.TrailingSlashMode = this.TrailingSlashMode;
            }

            if (this.IsSet(HttpConfigurationStrings.OperationHandlerFactory))
            {
                httpMemoryEndpoint.OperationHandlerFactory = HttpBehaviorElement.GetHttpOperationHandlerFactory(this.OperationHandlerFactory);
            }
        }

        private bool IsSet(string propertyName)
        {
            return this.ElementInformation.Properties[propertyName].IsModified;
        }
    }
}
