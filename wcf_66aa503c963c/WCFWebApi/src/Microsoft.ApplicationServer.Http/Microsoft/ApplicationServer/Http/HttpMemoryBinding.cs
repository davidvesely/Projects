// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Configuration;
    using Microsoft.Server.Common;

    /// <summary>
    /// A binding used with endpoints for web services that use strongly-type HTTP request 
    /// and response messages for in-memory communication.
    /// </summary>
    public class HttpMemoryBinding : Binding, IBindingRuntimePreferences
    {
        internal const string CollectionElementName = "httpMemoryBinding";

        private HttpMemoryTransportBindingElement httpMemoryTransportBindingElement;
        private HttpMessageHandlerBindingElement httpHandlerBindingElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryBinding"/> class.
        /// </summary>
        public HttpMemoryBinding() : base()
        {
            this.httpMemoryTransportBindingElement = new HttpMemoryTransportBindingElement();
            this.httpHandlerBindingElement = new HttpMessageHandlerBindingElement();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryBinding"/> class with a 
        /// binding specified by its configuration name.
        /// </summary>
        /// <param name="configurationName">
        /// The binding configuration name.
        /// </param>
        public HttpMemoryBinding(string configurationName) : this()
        {
            if (configurationName == null)
            {
                throw Fx.Exception.ArgumentNull("configurationName");
            }

            this.ApplyConfiguration(configurationName);
        }

        /// <summary>
        /// Gets the envelope version that is used by endpoints that are configured to use an 
        /// <see cref="HttpMemoryBinding"/> binding.  Always returns <see cref="System.ServiceModel.EnvelopeVersion.None"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is existing public API")]
        public EnvelopeVersion EnvelopeVersion
        {
            get 
            { 
                return EnvelopeVersion.None; 
            }
        }

        /// <summary>
        /// Gets or sets a value representing the <see cref="HttpMessageHandlerFactory"/> to use for
        /// instantiating <see cref="HttpMessageHandlerChannel"/> instances.
        /// </summary>
        [DefaultValue(null)]
        public HttpMessageHandlerFactory MessageHandlerFactory 
        {
            get
            {
                return this.httpHandlerBindingElement.MessageHandlerFactory;
            }

            set
            {
                this.httpHandlerBindingElement.MessageHandlerFactory = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the hostname is used to reach the 
        /// service when matching the URI.
        /// </summary>
        /// <remarks>Only the value <see cref="F:HostNameComparisonMode.StrongWildcard"/> is supported by the <see cref="HttpMemoryTransportBindingElement"/>.</remarks>
        public HostNameComparisonMode HostNameComparisonMode
        {
            get 
            { 
                return this.httpMemoryTransportBindingElement.HostNameComparisonMode; 
            }
        }

        /// <summary>
        /// Gets the URI transport scheme for the channels and listeners that are configured 
        /// with this binding. (Overrides <see cref="Binding.Scheme"/>)
        /// </summary>
        public override string Scheme
        { 
            get 
            { 
                return this.httpMemoryTransportBindingElement.Scheme; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether incoming requests can be handled more efficiently synchronously or asynchronously.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is the pattern used by all standard bindings.")]
        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an implementation of an <see cref="System.Net.Http.HttpMessageHandler"/> for this <see cref="HttpMemoryBinding"/> instance which can be 
        /// accessed directly or from an <see cref="System.Net.Http.HttpClient"/> for in-memory communication.
        /// </summary>
        /// <returns>The <see cref="HttpMemoryHandler"/> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The result of the method may not be available immediately upon calling and so a method is more appropriate.")]
        public HttpMemoryHandler GetHttpMemoryHandler()
        {
            return this.httpMemoryTransportBindingElement.GetHttpMemoryHandler(TimeSpan.MaxValue);
        }

        /// <summary>
        /// Returns an ordered collection of binding elements contained in the current binding. 
        /// (Overrides <see cref="System.ServiceModel.Channels.Binding.CreateBindingElements">
        /// Binding.CreateBindingElements</see>.)
        /// </summary>
        /// <returns>
        /// An ordered collection of binding elements contained in the current binding.
        /// </returns>
        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = new BindingElementCollection();

            bindingElements.Add(this.httpHandlerBindingElement);
            bindingElements.Add(this.httpMemoryTransportBindingElement);

            return bindingElements.Clone();
        }

        private void ApplyConfiguration(string configurationName)
        {
            HttpMemoryBindingCollectionElement section = HttpMemoryBindingCollectionElement.GetBindingCollectionElement();

            HttpMemoryBindingElement element = null;
            if (section != null && section.Bindings.ContainsKey(configurationName))
            {
                element = section.Bindings[configurationName];
            }

            if (element == null)
            {
                throw Fx.Exception.AsError(
                   new ConfigurationErrorsException(
                       SR.ConfigInvalidBindingConfigurationName(
                           configurationName,
                           CollectionElementName)));
            }

            element.ApplyConfiguration(this);
        }
    }
}
