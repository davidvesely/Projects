// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Services
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// This class illustrates how a POCO type can be treated as a service contract,
    /// despite not using <see cref="ServiceContractAttribute"/>
    /// </summary>
    public class PocoServiceContract
    {
        [WebGet]
        public PocoServiceItem GetItem(string name)
        {
            return new PocoServiceItem() { Name = name };
        }
    }

    public class PocoServiceItem
    {
        public string Name { get; set; }
    }
}
