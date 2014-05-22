// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Activation
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    /// <summary>
    /// <see cref="ServiceHostFactory"/> derived class that can create <see cref="HttpServiceHost"/> instances.
    /// </summary>
    public class HttpServiceHostFactory : ServiceHostFactory
    {
        private HttpConfiguration configuration;

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public HttpConfiguration Configuration 
        {
            get
            {
                if (this.configuration == null)
                {
                    this.configuration = new HttpConfiguration();    
                }

                return this.configuration;
            }

            set
            {
                this.configuration = value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="HttpServiceHost"/> instance.
        /// </summary>
        /// <param name="serviceType">Specifies the type of service to host.</param>
        /// <param name="baseAddresses">The base addresses for the service hosted.</param>
        /// <returns>A new <see cref="HttpServiceHost"/> instance.</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return this.configuration != null ? 
                new HttpServiceHost(serviceType, this.configuration, baseAddresses) :
                new HttpServiceHost(serviceType, baseAddresses);
        }
    }
}
