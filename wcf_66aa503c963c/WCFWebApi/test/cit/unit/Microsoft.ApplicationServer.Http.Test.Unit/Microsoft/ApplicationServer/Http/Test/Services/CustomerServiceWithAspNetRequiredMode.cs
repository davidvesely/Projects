// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Services
{
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CustomerServiceWithAspNetRequiredMode
    {
        [WebGet(UriTemplate = "GetCustomer/{name}")]
        [DataContractFormat]
        public Customer GetCustomer(string name)
        {
            return new Customer() { Name = name };
        }

        [WebGet(UriTemplate = "GetCustomerAsJson/{name}", ResponseFormat = WebMessageFormat.Json)]
        [DataContractFormat]
        public Customer GetCustomerAsJson(string name)
        {
            return new Customer() { Name = name };
        }

        [WebGet(UriTemplate = "GetCustomerAsXml/{name}", ResponseFormat = WebMessageFormat.Xml)]
        [DataContractFormat]
        public Customer GetCustomerAsXml(string name)
        {
            return new Customer() { Name = name };
        }

        [WebGet(UriTemplate = "GetCustomerAsXmlWithXmlSerializer/{name}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        public Customer GetCustomerAsXmlWithXmlSerializer(string name)
        {
            return new Customer() { Name = name };
        }
    }
}
