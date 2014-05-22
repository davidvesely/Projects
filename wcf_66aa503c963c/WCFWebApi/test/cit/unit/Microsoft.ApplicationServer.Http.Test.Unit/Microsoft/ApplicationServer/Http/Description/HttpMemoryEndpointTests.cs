// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Channels.Mocks;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMemoryEndpointTests: UnitTest<HttpMemoryEndpoint>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMemoryEndpoint is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("HttpMemoryEndpoint(ContractDescription) works as expected.")]
        public void HttpMemoryEndpointTestsContractCtor()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);
            Assert.IsNotNull(endpoint, "endpoint should not be null.");

            Assert.IsNotNull(endpoint.Binding, "endpoint binding should not be null.");
            Assert.IsInstanceOfType(endpoint.Binding, typeof(HttpMemoryBinding), "endpoint binding should be of type HttpMemoryBinding");

            HttpBehavior behavior = endpoint.Behaviors.Find<HttpBehavior>();
            Assert.IsNotNull(behavior, "HttpBehavior should be present.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("HttpMemoryEndpoint(ContractDescription, EndpointAddress) works as expected.")]
        public void HttpMemoryEndpointTestsContractAddressCtor()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            EndpointAddress endpointAddress = new EndpointAddress("http://some/path");
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract, endpointAddress);
            Assert.IsNotNull(endpoint, "endpoint should not be null.");

            Assert.IsNotNull(endpoint.Address, "endpoint address should not be null.");
            Assert.AreSame(endpointAddress, endpoint.Address, "endpoint address should be identical.");
        }

        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("MessageHandlerFactory is passed to binding.")]
        public void MessageHandlerFactory()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);

            HttpMessageHandlerFactory messageHandlerFactory = new HttpMessageHandlerFactory(typeof(MockValidMessageHandler));
            endpoint.MessageHandlerFactory = messageHandlerFactory;
            HttpMemoryBinding binding = endpoint.Binding as HttpMemoryBinding;
            Assert.AreSame(messageHandlerFactory, binding.MessageHandlerFactory, "Message handler factory should be passed to binding.");
            Assert.AreSame(binding.MessageHandlerFactory, endpoint.MessageHandlerFactory, "Message handler factory should come from binding.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("OperationHandlerFactory is passed to behavior.")]
        public void OperationHandlerFactory()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);

            HttpOperationHandlerFactory operationHandlerFactory = new HttpOperationHandlerFactory();
            endpoint.OperationHandlerFactory = operationHandlerFactory;

            HttpBehavior behavior = endpoint.Behaviors.Find<HttpBehavior>();
            Assert.AreSame(operationHandlerFactory, behavior.OperationHandlerFactory, "Operation handler factory should be passed to behavior.");
            Assert.AreSame(behavior.OperationHandlerFactory, endpoint.OperationHandlerFactory, "Operation handler factory should come from behavior.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("HelpEnabled is passed to behavior.")]
        public void HelpEnabled()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);

            endpoint.HelpEnabled = true;

            HttpBehavior behavior = endpoint.Behaviors.Find<HttpBehavior>();
            Assert.AreEqual(true, behavior.HelpEnabled, "HelpEnabled should be passed to behavior.");
            Assert.AreEqual(behavior.HelpEnabled, endpoint.HelpEnabled, "HelpEnabled should come from behavior.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("TestClientEnabled is passed to behavior.")]
        public void TestClientEnabled()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);

            endpoint.TestClientEnabled = true;

            HttpBehavior behavior = endpoint.Behaviors.Find<HttpBehavior>();
            Assert.AreEqual(true, behavior.TestClientEnabled, "TestClientEnabled should be passed to behavior.");
            Assert.AreEqual(behavior.TestClientEnabled, endpoint.TestClientEnabled, "TestClientEnabled should come from behavior.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("TrailingSlashMode is passed to behavior.")]
        public void TrailingSlashMode()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(ContactsService));
            HttpMemoryEndpoint endpoint = new HttpMemoryEndpoint(contract);

            endpoint.TrailingSlashMode = Http.TrailingSlashMode.Ignore;

            HttpBehavior behavior = endpoint.Behaviors.Find<HttpBehavior>();
            Assert.AreEqual(Http.TrailingSlashMode.Ignore, behavior.TrailingSlashMode, "TrailingSlashMode should be passed to behavior.");
            Assert.AreEqual(behavior.TrailingSlashMode, endpoint.TrailingSlashMode, "TrailingSlashMode should come from behavior.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("derik")]
        [Description("GetHttpMemoryHandler succeeds.")]
        public void GetHttpMemoryHandlerSucceeds()
        {
            Uri serviceAddress = new Uri("http://localhost:8080/contactsservice");
            Uri memoryServiceAddress = new Uri("http://memoryhost/contactsservice");
            HttpServiceHost serviceHost = new HttpServiceHost(typeof(ContactsService), serviceAddress);
            try
            {
                // Add default endpoint
                IEnumerable<ServiceEndpoint> defaultEndpoint = serviceHost.AddDefaultEndpoints();
                ContractDescription contract = defaultEndpoint.ElementAt(0).Contract;
                HttpMemoryEndpoint memoryEndpoint = new HttpMemoryEndpoint(contract, new EndpointAddress(memoryServiceAddress));
                serviceHost.AddServiceEndpoint(memoryEndpoint);
                serviceHost.Open();

                HttpMemoryHandler memoryHandler = memoryEndpoint.GetHttpMemoryHandler();
            }
            finally
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
            }
        }

        #endregion
    }
}