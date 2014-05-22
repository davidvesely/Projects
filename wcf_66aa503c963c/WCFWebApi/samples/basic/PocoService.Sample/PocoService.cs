// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace PocoService.Sample
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// This type can be used as a service type though it has not been marked with <see cref="ServiceContractAttribute"/>. 
    /// </summary>
    internal class PocoService
    {
        [WebGet(UriTemplate="Item/{name}")]
        public PocoServiceItem GetItem(string name)
        {
            return new PocoServiceItem() { Name = name };
        }
    }
}
