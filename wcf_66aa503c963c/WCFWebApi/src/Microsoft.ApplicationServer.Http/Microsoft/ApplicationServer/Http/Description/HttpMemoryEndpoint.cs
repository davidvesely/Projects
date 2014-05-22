// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.Server.Common;

    /// <summary>
    /// Class that provides a <see cref="ServiceEndpoint"/> for the <see cref="HttpMemoryBinding"/> binding.
    /// </summary>
    public class HttpMemoryEndpoint : ServiceEndpoint
    {
        internal const string Kind = "httpMemoryEndpoint";

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryEndpoint"/> class.
        /// </summary>
        /// <param name="contract">The <see cref="ContractDescription"/> for the service endpoint.</param>
        public HttpMemoryEndpoint(ContractDescription contract)
            : this(contract, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryEndpoint"/> class.
        /// </summary>
        /// <param name="contract">The <see cref="ContractDescription"/> for the service endpoint.</param>
        /// <param name="address">The <see cref="EndpointAddress"/> for the service endpoint.</param>
        public HttpMemoryEndpoint(ContractDescription contract, EndpointAddress address)
            : base(contract, new HttpMemoryBinding(), address)
        {
            this.Behaviors.Add(new HttpBehavior());
            this.Behaviors.Add(new HttpMemoryBehavior());
        }

        /// <summary>
        /// Gets or sets the default <see cref="HttpMessageHandlerFactory"/> to use for
        /// the <see cref="HttpMemoryEndpoint"/>.
        /// </summary>
        public HttpMessageHandlerFactory MessageHandlerFactory
        {
            get { return this.HttpMemoryBinding.MessageHandlerFactory; }
            set { this.HttpMemoryBinding.MessageHandlerFactory = value; }
        }

        /// <summary>
        /// Gets or sets the default <see cref="HttpOperationHandlerFactory"/> to use for
        /// the <see cref="HttpMemoryEndpoint"/>.
        /// </summary>
        public HttpOperationHandlerFactory OperationHandlerFactory
        {
            get { return this.HttpBehavior.OperationHandlerFactory; }
            set { this.HttpBehavior.OperationHandlerFactory = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an automatic help page will be generated.
        /// </summary>
        public bool HelpEnabled
        {
            get { return this.HttpBehavior.HelpEnabled; }
            set { this.HttpBehavior.HelpEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a web-based test client will be generated.
        /// </summary>
        public bool TestClientEnabled
        {
            get { return this.HttpBehavior.TestClientEnabled; }
            set { this.HttpBehavior.TestClientEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value specifying how trailing slashes on <see cref="Uri"/>s should be treated.
        /// </summary>
        public TrailingSlashMode TrailingSlashMode
        {
            get { return this.HttpBehavior.TrailingSlashMode; }
            set { this.HttpBehavior.TrailingSlashMode = value; }
        }

        internal HttpMemoryBinding HttpMemoryBinding
        {
            get
            {
                HttpMemoryBinding localHttpMemoryBinding = this.Binding as HttpMemoryBinding;
                if (localHttpMemoryBinding == null)
                {
                    throw Fx.Exception.AsError(
                        new InvalidOperationException(
                            Http.SR.HttpEndpointRequiresHttpBinding(
                                typeof(HttpMemoryEndpoint).Name,
                                typeof(HttpMemoryBinding).Name)));
                }

                return localHttpMemoryBinding;
            }
        }

        internal HttpBehavior HttpBehavior
        {
            get
            {
                HttpBehavior localHttpBehavior = this.Behaviors.Find<HttpBehavior>();
                if (localHttpBehavior == null)
                {
                    throw Fx.Exception.AsError(
                        new InvalidOperationException(
                            Http.SR.HttpBehaviorNotFoundWithEndpoint(
                                typeof(HttpMemoryEndpoint).Name,
                                typeof(HttpBehavior).Name)));
                }

                return localHttpBehavior;
            }
        }

        /// <summary>
        /// Gets an implementation of an <see cref="System.Net.Http.HttpMessageHandler"/> for this <see cref="HttpMemoryEndpoint"/> instance which can be 
        /// either accessed directly or from an <see cref="System.Net.Http.HttpClient"/> for in-memory communication.
        /// </summary>
        /// <returns>The <see cref="HttpMemoryHandler"/> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The result of the method may not be available immediately upon calling and so a method is more appropriate.")]
        public HttpMemoryHandler GetHttpMemoryHandler()
        {
            return this.HttpMemoryBinding.GetHttpMemoryHandler();
        }
    }
}
