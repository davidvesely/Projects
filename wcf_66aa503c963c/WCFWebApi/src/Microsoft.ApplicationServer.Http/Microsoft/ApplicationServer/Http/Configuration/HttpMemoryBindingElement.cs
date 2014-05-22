// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using Microsoft.ServiceModel;
    using Microsoft.ServiceModel.Channels;
    using Microsoft.ServiceModel.Configuration;

    /// <summary>
    /// A configuration element for the <see cref="HttpMemoryBinding"/> binding. 
    /// </summary>
    public class HttpMemoryBindingElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryBindingElement"/> class and specifies 
        /// the name of the element. 
        /// </summary>
        /// <param name="name">The name that is used for this binding configuration element</param>
        public HttpMemoryBindingElement(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryBindingElement"/> class.
        /// </summary>
        public HttpMemoryBindingElement()
            : this(null)
        {
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of binding that this configuration element represents. 
        /// </summary>
        /// <returns>A <see cref="Type"/> object that represents the custom binding type.</returns>
        protected override Type BindingElementType
        {
            get
            {
                return typeof(HttpMemoryBinding);
            }
        }

        /// <summary>
        /// Gets a <see cref="ConfigurationPropertyCollection"/> instance that contains a collection of <see cref="ConfigurationProperty"/> 
        /// objects that can be attributes or <see cref="ConfigurationElement"/> objects of this configuration element.
        /// </summary>
        /// <returns>A <see cref="ConfigurationPropertyCollection"/> instance that contains a collection of <see cref="ConfigurationProperty"/> 
        /// objects that can be attributes or <see cref="ConfigurationElement"/> objects of this configuration element.</returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection baseProperties = base.Properties;
                    baseProperties.Add(new ConfigurationProperty(ConfigurationStrings.HostNameComparisonMode, typeof(HostNameComparisonMode), HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    this.properties = baseProperties;
                }

                return this.properties;
            }
        }

        /// <summary>
        /// Applies the configuration of the the <see cref="HttpMemoryBindingElement"/> to the given
        /// <see cref="HttpMemoryBinding"/> instance.
        /// </summary>
        /// <param name="binding">The <see cref="HttpMemoryBinding"/> 
        /// instance to which the <see cref="HttpMemoryBindingElement"/> configuration will be applied.
        /// </param>
        protected override void OnApplyConfiguration(Binding binding)
        {
        }
    }
}