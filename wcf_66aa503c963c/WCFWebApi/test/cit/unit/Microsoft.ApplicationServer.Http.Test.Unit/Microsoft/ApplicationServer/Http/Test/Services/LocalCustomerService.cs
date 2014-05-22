// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Services
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class LocalCustomerService
    {
        [WebGet(UriTemplate = "{name}")]
        public LocalCustomer GetLocalCustomer(string name) { return null; }
    }

    [DataContract]
    public class LocalCustomer
    {
        [DataMember]
        public string Name { get; set; }
    }
}
