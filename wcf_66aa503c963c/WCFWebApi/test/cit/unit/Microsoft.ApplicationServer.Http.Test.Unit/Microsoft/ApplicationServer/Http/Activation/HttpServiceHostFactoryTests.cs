// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Mocks;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Activation;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpServiceHostFactoryTests : UnitTest<HttpServiceHostFactory>
    {
        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost is public, concrete and disposable.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass, typeof(ServiceHostFactory));
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHostFactory()")]
        public void DefaultCtor()
        {
            HttpServiceHostFactory hostFactory = new HttpServiceHostFactory();
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Configuration returns correct instance.")]
        public void Configuration()
        {
            HttpServiceHostFactory hostFactory = new HttpServiceHostFactory();

            // Get default config object
            HttpConfiguration configuration1 = hostFactory.Configuration;
            Assert.IsNotNull(configuration1, "configuration should not be null");

            // Set and get the same config object
            HttpConfiguration configuration2 = new HttpConfiguration();
            hostFactory.Configuration = configuration2;
            Assert.AreSame(configuration2, hostFactory.Configuration, "Expected the same configuration.");

            // Set config to null
            hostFactory.Configuration = null;
            HttpConfiguration configuration3 = hostFactory.Configuration;
            Assert.IsNotNull(configuration3, "configuration should have been reinitialized.");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("CreateServiceHost(Type, Uri[]) throws on null")]
        public void CreateServiceHostThrowsOnNull()
        {
            MockHttpServiceHostFactory hostFactory = MockHttpServiceHostFactory.Create();
            Uri[] baseAddresses = new Uri[] { new Uri("http://some.host") };

            Asserters.Exception.ThrowsArgumentNull("serviceType", () => { hostFactory.CreateServiceHost((Type)null, baseAddresses); });
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { hostFactory.CreateServiceHost(typeof(CustomerService), null); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("CreateServiceHost(Type, Uri[]) succeeds")]
        public void CreateServiceHost()
        {
            Uri[] baseAddresses = new Uri[] { new Uri("http://some.host") };

            // Test without config
            MockHttpServiceHostFactory hostFactoryWithoutConfig = MockHttpServiceHostFactory.Create();
            ServiceHost host1 = hostFactoryWithoutConfig.CreateServiceHost(typeof(CustomerService), baseAddresses);
            Assert.IsNotNull(host1, "Host should not be null");

            // Test with config
            MockHttpServiceHostFactory hostFactoryWithConfig = MockHttpServiceHostFactory.Create();
            hostFactoryWithConfig.Configuration = new HttpConfiguration
            {
                TransferMode = TransferMode.StreamedResponse
            };

            ServiceHost host2 = hostFactoryWithConfig.CreateServiceHost(typeof(CustomerService), baseAddresses);
            Assert.IsNotNull(host2, "Host should not be null");

            // Validate the configuration made it all the way through
            IEnumerable<ServiceEndpoint> endpoints = host2.AddDefaultEndpoints();
            Assert.IsNotNull(endpoints, "Endpoints should not be null");
            Assert.AreEqual(1, endpoints.Count(), "Expected 1 default endpoints");

            HttpTransportBindingElement transport = endpoints.ElementAt(0).Binding.CreateBindingElements().Last() as HttpTransportBindingElement;
            Assert.IsNotNull(transport, "Transport should not be null");
            Assert.AreEqual(TransferMode.StreamedResponse, transport.TransferMode, "Unexpected transfer mode.");
        }
    }
}