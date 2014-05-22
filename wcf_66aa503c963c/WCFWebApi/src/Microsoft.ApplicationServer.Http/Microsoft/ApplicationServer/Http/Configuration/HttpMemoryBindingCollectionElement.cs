// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using Microsoft.ServiceModel.Activation;

    /// <summary>
    /// Represents a configuration element that contains sub-elements that specify settings for using 
    /// the <see cref="HttpMemoryBinding"/> binding.
    /// </summary>
    public class HttpMemoryBindingCollectionElement : StandardBindingCollectionElement<HttpMemoryBinding, HttpMemoryBindingElement>
    {
        private const string SectionPath = "system.serviceModel/bindings";

        internal static HttpMemoryBindingCollectionElement GetBindingCollectionElement()
        {
            BindingsSection bindings = AspNetEnvironment.Current.GetConfigurationSection(SectionPath) as BindingsSection;
            if (bindings != null)
            {            
                foreach (BindingCollectionElement bindingCollection in bindings.BindingCollections)
                {
                    if (bindingCollection.BindingName == HttpMemoryBinding.CollectionElementName)
                    {
                        return bindingCollection as HttpMemoryBindingCollectionElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a default <see cref="HttpMemoryBinding"/> binding.
        /// </summary>
        /// <returns>
        /// A default <see cref="HttpMemoryBinding"/> binding.
        /// </returns>
        protected override Binding GetDefault()
        {
            return new HttpMemoryBinding();
        }
    }
}
