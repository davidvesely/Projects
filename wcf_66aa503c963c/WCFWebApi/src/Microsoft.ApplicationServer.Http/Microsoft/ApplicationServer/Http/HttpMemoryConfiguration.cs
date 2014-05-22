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

    /// <summary>
    /// The configuration class for Http Services with an <see cref="HttpMemoryEndpoint"/> for 
    /// in-memory communication.
    /// </summary>
    public class HttpMemoryConfiguration
    {
        // TODO: CSDMain 238509: Introduce abstract HttpConfigurationBase class which is the union of HttpConfiguration and HttpMemoryConfiguration
        private static readonly Type httpMemoryConfigurationType = typeof(HttpMemoryConfiguration);
        private static readonly Type httpServiceHostType = typeof(HttpServiceHost);
       
        private bool isReadOnly;
        private HttpConfigurationInstanceProvider instanceProvider;
        private HttpConfigurationOperationHandlerFactory operationHandlerFactory;
        private HttpMessageHandlerFactory messageHandlerFactory;
        private Action<Collection<HttpErrorHandler>, ServiceEndpoint, IEnumerable<HttpOperationDescription>> errorHandlerAction;
        private TrailingSlashMode trailingSlashMode;
        private Func<IEnumerable<DelegatingHandler>> messageHandlerDelegate;
        private Collection<Type> messageHandlerTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryConfiguration"/> class.
        /// </summary>
        public HttpMemoryConfiguration()
        {
            this.instanceProvider = new HttpConfigurationInstanceProvider();
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
        /// Internal method called to configure endpoint settings.
        /// </summary>
        /// <param name="endpoint">Http endpoint.</param>
        internal void ConfigureEndpoint(HttpMemoryEndpoint endpoint)
        {
            this.isReadOnly = true;
            this.OnConfigureEndpoint(endpoint);
        }

        /// <summary>
        /// Called to apply the configuration on the endpoint level.
        /// </summary>
        /// <param name="endpoint">Http endpoint.</param>
        protected virtual void OnConfigureEndpoint(HttpMemoryEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw Fx.Exception.ArgumentNull("endpoint");
            }

            if (this.ReleaseInstance != null && this.CreateInstance == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR.CannotSetOnlyOneProperty("ReleaseInstance", "CreateInstance", httpMemoryConfigurationType.Name)));
            }

            // At this point the endpoint has the default HttpBehavior so we can get it in order to configure it.
            HttpBehavior httpBehavior = endpoint.HttpBehavior;

            // Configure the HttpBehavior
            httpBehavior.OperationHandlerFactory = this.operationHandlerFactory;
            httpBehavior.ErrorHandlerDelegate = this.errorHandlerAction;
            httpBehavior.InstanceProvider = this.instanceProvider.CreateInstanceDelegate != null ? this.instanceProvider : null;
            httpBehavior.TrailingSlashMode = this.TrailingSlashMode;

            if (this.MessageHandlerFactory != null && this.MessageHandlers.Count > 0)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR.CannotSetBothProperties("MessageHandlerFactory", "MessageHandlers", httpMemoryConfigurationType.Name)));
            }

            if (this.MessageHandlerFactory != null)
            {
                this.messageHandlerFactory = new HttpMessageHandlerFactory(this.MessageHandlerFactory);
            }

            if (this.MessageHandlers.Count > 0)
            {
                this.messageHandlerFactory = new HttpMessageHandlerFactory(this.MessageHandlers.ToArray());
            }

            HttpMemoryBinding httpBinding = endpoint.HttpMemoryBinding;
            httpBinding.MessageHandlerFactory = this.messageHandlerFactory;
        }

        private void CheckReadOnly()
        {
            if (this.isReadOnly)
            {
                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Microsoft.ApplicationServer.Http.SR.HttpConfigurationIsReadOnly(
                            httpMemoryConfigurationType.Name, 
                            httpServiceHostType.Name)));
            }
        }
    }
}
