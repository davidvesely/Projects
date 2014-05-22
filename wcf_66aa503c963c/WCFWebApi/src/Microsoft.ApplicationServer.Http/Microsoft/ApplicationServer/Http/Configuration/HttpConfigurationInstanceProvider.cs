// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System;
    using System.Net.Http;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.Server.Common;

    /// <summary>
    /// A custom HttpInstanceProvider that takes delegates and call them during OnGetInstance and OnReleaseInstance
    /// </summary>
    internal class HttpConfigurationInstanceProvider : HttpInstanceProvider
    {
        /// <summary>
        /// Gets or sets an action to release instance.
        /// </summary>
        /// <value>
        /// The release instance.
        /// </value>
        /// <remarks>This is optional.</remarks>
        public Action<InstanceContext, object> ReleaseInstanceDelegate { get; set; }

        /// <summary>
        /// Gets or sets the action used to create instance.
        /// </summary>
        /// <value>
        /// The create instance.
        /// </value>
        public Func<Type, InstanceContext, HttpRequestMessage, object> CreateInstanceDelegate { get; set; }

        /// <summary>
        /// Called by <see cref="HttpInstanceProvider.GetInstance(InstanceContext)"/> to get the instance.
        /// Derived classes must implement this.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext"/> object.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        protected override object OnGetInstance(InstanceContext instanceContext)
        {
            return this.OnGetInstance(instanceContext, null);
        }

        /// <summary>
        /// Gets an instance of the service
        /// </summary>
        /// <param name="instanceContext">Instance context</param>
        /// <param name="request">Request message</param>
        /// <returns>
        /// Instance object
        /// </returns>
        protected override object OnGetInstance(InstanceContext instanceContext, HttpRequestMessage request)
        {
            Fx.Assert(this.CreateInstanceDelegate != null, "CreateInstanceDelegate property should not be null.");
            Type serviceType = instanceContext.Host.Description.ServiceType;
            return this.CreateInstanceDelegate(serviceType, instanceContext, request);
        }

        /// <summary>
        /// Release instance
        /// </summary>
        /// <param name="instanceContext">Instance context</param>
        /// <param name="instance">Instance object</param>
        protected override void OnReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (this.ReleaseInstanceDelegate != null)
            {
                this.ReleaseInstanceDelegate(instanceContext, instance);
            }
        }
    }
}
