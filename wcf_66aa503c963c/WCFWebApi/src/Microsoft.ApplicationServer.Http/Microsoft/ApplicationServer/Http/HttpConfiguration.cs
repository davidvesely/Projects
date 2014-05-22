// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Configuration;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.Server.Common;
    using Microsoft.ServiceModel.Channels;

    /// <summary>
    /// The configuration class for Http Services
    /// </summary>
    public class HttpConfiguration
    {
        private static readonly Type httpConfigurationType = typeof(HttpConfiguration);
        private static readonly Type httpServiceHostType = typeof(HttpServiceHost);
       
        private bool isReadOnly;
        private HttpConfigurationInstanceProvider instanceProvider;
        private HttpConfigurationOperationHandlerFactory operationHandlerFactory;
        private HttpMessageHandlerFactory messageHandlerFactory;
        private Action<Collection<HttpErrorHandler>, ServiceEndpoint, IEnumerable<HttpOperationDescription>> errorHandlerAction;
        private Action<Uri, HttpBindingSecurity> security;
        private TransferMode transferMode;
        private bool enableHelpPage;
        private bool enableTestClient;
        private TrailingSlashMode trailingSlashMode;
        private bool includeExceptionDetail;
        private int maxBufferSize;
        private long maxReceivedMessageSize;
        private Func<IEnumerable<DelegatingHandler>> messageHandlerDelegate;
        private Collection<Type> messageHandlerTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpConfiguration"/> class.
        /// </summary>
        public HttpConfiguration()
        {
            this.maxBufferSize = TransportDefaults.MaxBufferSize;
            this.maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
            this.instanceProvider = new HttpConfigurationInstanceProvider();
        }

        /// <summary>
        /// Gets or sets the transfer mode.
        /// </summary>
        /// <value>
        /// The transfer mode.
        /// </value>
        [DefaultValue(TransferMode.Buffered)]
        public TransferMode TransferMode 
        { 
            get
            {
                return this.transferMode;
            }

            set
            {
                this.CheckReadOnly();
                this.transferMode = value;
            }
        }

        /// <summary>
        /// Gets the media type formatters.
        /// </summary>
        public MediaTypeFormatterCollection Formatters 
        { 
            get
            {
                if (this.OperationHandlerFactory.Formatters == null)
                {
                    this.OperationHandlerFactory.Formatters = new MediaTypeFormatterCollection();
                }

                return this.OperationHandlerFactory.Formatters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable help page.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableHelpPage
        {
            get
            {
                return this.enableHelpPage;
            }

            set
            {
                this.CheckReadOnly();
                this.enableHelpPage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable test client.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableTestClient
        {
            get
            {
                return this.enableTestClient;
            }

            set
            {
                this.CheckReadOnly();
                this.enableTestClient = value;
            }
        }

        /// <summary>
        /// Gets or sets the trailing slash mode.
        /// </summary>
        /// <value>
        /// The trailing slash mode.
        /// </value>
        [DefaultValue(TrailingSlashMode.AutoRedirect)]
        public TrailingSlashMode TrailingSlashMode 
        { 
            get
            {
                return this.trailingSlashMode;
            }

            set
            {
                this.CheckReadOnly();
                TrailingSlashModeHelper.Validate(value, "value");
                this.trailingSlashMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max buffer.
        /// </summary>
        /// <value>
        /// The size of the max buffer.
        /// </value>
        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                return this.maxBufferSize;
            }

            set
            {
                this.CheckReadOnly();
                this.maxBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max received message.
        /// </summary>
        /// <value>
        /// The size of the max received message.
        /// </value>
        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get
            {
                return this.maxReceivedMessageSize;
            }

            set
            {
                this.CheckReadOnly();
                this.maxReceivedMessageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include exception detail.
        /// </summary>
        [DefaultValue(false)]
        public bool IncludeExceptionDetail
        {
            get
            {
                return this.includeExceptionDetail;
            }

            set
            {
                this.CheckReadOnly();
                this.includeExceptionDetail = value;
            }
        }

        /// <summary>
        /// Gets or sets an action to configure request handlers.
        /// </summary>
        /// <value>
        /// The request handlers action.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required in order to provide a plug in model instead of having to define a new factory.")]
        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> RequestHandlers
        {
            get
            {
                return this.OperationHandlerFactory.RequestHandlerDelegate;
            }

            set
            {
                this.CheckReadOnly();
                this.OperationHandlerFactory.RequestHandlerDelegate = value;
            }
        }

        /// <summary>
        /// Gets or sets an action to configure response handlers.
        /// </summary>
        /// <value>
        /// The response handlers action.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required in order to provide a plug in model instead of having to define a new factory.")]
        public Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> ResponseHandlers
        {
            get
            {
                return this.OperationHandlerFactory.ResponseHandlerDelegate;
            }

            set
            {
                this.CheckReadOnly();
                this.OperationHandlerFactory.ResponseHandlerDelegate = value;
            }
        }

        /// <summary>
        /// Gets or sets a delegate to create service instance
        /// </summary>
        /// <value>
        /// The create service instance delegate.
        /// </value>
        public Func<Type, InstanceContext, HttpRequestMessage, object> CreateInstance
        {
            get
            {
                return this.instanceProvider.CreateInstanceDelegate;
            }

            set
            {
                this.CheckReadOnly();
                this.instanceProvider.CreateInstanceDelegate = value;
            }
        }

        /// <summary>
        /// Gets or sets a delegate to release service instance
        /// </summary>
        /// <value>
        /// The release service instance delegate.
        /// </value>
        public Action<InstanceContext, object> ReleaseInstance
        {
            get
            {
                return this.instanceProvider.ReleaseInstanceDelegate;
            }

            set
            {
                this.CheckReadOnly();
                this.instanceProvider.ReleaseInstanceDelegate = value;
            }
        }

        /// <summary>
        /// Gets or sets the delegate to create message handlers.
        /// </summary>
        /// <value>
        /// The message handler delegate.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required in order to provide a plug in model instead of having to define a new factory.")]
        public Func<IEnumerable<DelegatingHandler>> MessageHandlerFactory
        {
            get
            {
                return this.messageHandlerDelegate;
            }

            set
            {
                this.CheckReadOnly();
                this.messageHandlerDelegate = value;
            }
        }
        
        /// <summary>
        /// Gets the types of message handlers to be created.
        /// </summary>
        /// <value>
        /// The message handler types.
        /// </value>
        public Collection<Type> MessageHandlers
        {
            get
            {
                if (this.messageHandlerTypes == null)
                {
                    this.messageHandlerTypes = new Collection<Type>();
                }

                return this.messageHandlerTypes;
            }
        }

        /// <summary>
        /// Gets or sets an action to configure http error handlers.
        /// </summary>
        /// <value>
        /// The error handlers action.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required in order to provide a plug in model instead of having to define a new factory.")]
        public Action<Collection<HttpErrorHandler>, ServiceEndpoint, IEnumerable<HttpOperationDescription>> ErrorHandlers 
        {
            get
            {
                return this.errorHandlerAction;
            }

            set
            {
                this.CheckReadOnly();
                this.errorHandlerAction = value;
            }
        }

        /// <summary>
        /// Gets or sets an action to configure security.
        /// </summary>
        /// <value>
        /// The security action.
        /// </value>
        public Action<Uri, HttpBindingSecurity> Security
        {
            get
            {
                return this.security;
            }

            set
            {
                this.CheckReadOnly();
                this.security = value;
            }
        }

        private HttpConfigurationOperationHandlerFactory OperationHandlerFactory
        {
            get
            {
                if (this.operationHandlerFactory == null)
                {
                    this.operationHandlerFactory = new HttpConfigurationOperationHandlerFactory();
                }

                return this.operationHandlerFactory;
            }
        }

        /// <summary>
        /// Internal method called to configure host settings.
        /// </summary>
        /// <param name="serviceHost">Service host.</param>
        internal void ConfigureServiceHost(HttpServiceHost serviceHost)
        {
            this.isReadOnly = true;
            this.OnConfigureServiceHost(serviceHost);
        }

        /// <summary>
        /// Internal method called to configure endpoint settings.
        /// </summary>
        /// <param name="endpoint">Http endpoint.</param>
        internal void ConfigureEndpoint(HttpEndpoint endpoint)
        {
            this.OnConfigureEndpoint(endpoint);
        }

        /// <summary>
        /// Called to apply the configuration on the host level.
        /// </summary>
        /// <param name="serviceHost">Http service host.</param>
        protected virtual void OnConfigureServiceHost(HttpServiceHost serviceHost)
        {
            if (serviceHost == null)
            {
                throw Fx.Exception.ArgumentNull("serviceHost");
            }

            ServiceDescription serviceDescription = serviceHost.Description;
            if (serviceDescription != null)
            {
                ServiceDebugBehavior debugBehavior = serviceDescription.Behaviors.Find<ServiceDebugBehavior>();
                if (debugBehavior == null)
                {
                    if (this.IncludeExceptionDetail)
                    {
                        debugBehavior = new ServiceDebugBehavior();
                        serviceDescription.Behaviors.Add(debugBehavior);
                        debugBehavior.IncludeExceptionDetailInFaults = true;
                    }
                }
                else
                {
                    debugBehavior.IncludeExceptionDetailInFaults = this.IncludeExceptionDetail;
                }
            }
        }

        /// <summary>
        /// Called to apply the configuration on the endpoint level.
        /// </summary>
        /// <param name="endpoint">Http endpoint.</param>
        protected virtual void OnConfigureEndpoint(HttpEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (this.ReleaseInstance != null && this.CreateInstance == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR.CannotSetOnlyOneProperty("ReleaseInstance", "CreateInstance", httpConfigurationType.Name)));
            }

            // At this point the endpoint has the default HttpBehavior so we can get it in order to configure it.
            HttpBehavior httpBehavior = endpoint.HttpBehavior;

            // Configure the HttpBehavior
            httpBehavior.OperationHandlerFactory = this.operationHandlerFactory;
            httpBehavior.ErrorHandlerDelegate = this.errorHandlerAction;
            httpBehavior.InstanceProvider = this.instanceProvider.CreateInstanceDelegate != null ? this.instanceProvider : null;
            httpBehavior.HelpEnabled = this.EnableHelpPage;
            httpBehavior.TestClientEnabled = this.EnableTestClient;
            httpBehavior.TrailingSlashMode = this.TrailingSlashMode;

            if (this.MessageHandlerFactory != null && this.MessageHandlers.Count > 0)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR.CannotSetBothProperties("MessageHandlerFactory", "MessageHandlers", httpConfigurationType.Name)));
            }

            if (this.MessageHandlerFactory != null)
            {
                this.messageHandlerFactory = new HttpMessageHandlerFactory(this.MessageHandlerFactory);
            }

            if (this.MessageHandlers.Count > 0)
            {
                this.messageHandlerFactory = new HttpMessageHandlerFactory(this.MessageHandlers.ToArray());
            }

            HttpBinding httpBinding = endpoint.HttpBinding;
            httpBinding.MessageHandlerFactory = this.messageHandlerFactory;
            httpBinding.MaxBufferSize = this.MaxBufferSize;
            httpBinding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            httpBinding.TransferMode = this.TransferMode;
            if (this.security != null)
            {
                this.security(endpoint.Address.Uri, httpBinding.Security);
            }
        }

        private void CheckReadOnly()
        {
            if (this.isReadOnly)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Microsoft.ApplicationServer.Http.SR.HttpConfigurationIsReadOnly(
                            httpConfigurationType.Name,
                            httpServiceHostType.Name)));
            }
        }
    }
}
