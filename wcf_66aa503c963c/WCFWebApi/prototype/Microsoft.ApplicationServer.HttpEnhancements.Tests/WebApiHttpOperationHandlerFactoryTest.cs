// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.HttpEnhancements.Tests
{
    using System.Linq;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebApiHttpOperationHandlerFactoryTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var factory = new WebApiHttpOperationHandlerFactory();
            var endpoint = new ServiceEndpoint(new ContractDescription("test"));
            var operation = new HttpOperationDescription();
            var responseHandlers = factory.CreateResponseHandlers(endpoint, operation);
            Assert.IsTrue(responseHandlers.Any(handler => handler is JsonpHttpResponseHandler));
        }
    }
}
