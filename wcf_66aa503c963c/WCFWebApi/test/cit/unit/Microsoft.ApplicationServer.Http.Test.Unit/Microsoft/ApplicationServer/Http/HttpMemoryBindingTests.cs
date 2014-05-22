// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Channels.Mocks;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMemoryBindingTests: UnitTest<HttpMemoryBinding>
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
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBinding() ctor")]
        public void HttpMemoryBinding()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            Assert.IsNotNull(binding, "Binding should not be null");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBinding(string) throws on null")]
        public void HttpMemoryBindingNullString()
        {
            Asserters.Exception.ThrowsArgumentNull("configurationName",
                () =>
                {
                    HttpMemoryBinding binding = new HttpMemoryBinding(null);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBinding(string) throws on unknown config name")]
        public void HttpMemoryBindingUnknownString()
        {
            Asserters.Exception.Throws<ConfigurationErrorsException>(
                SR.ConfigInvalidBindingConfigurationName("unknown", "httpMemoryBinding"),
                () =>
                {
                    HttpMemoryBinding binding = new HttpMemoryBinding("unknown");
                });
        }

        #endregion

        #region Members
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("EnvelopeVersion returns None.")]
        public void EnvelopeVersionTest()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            Assert.AreSame(EnvelopeVersion.None, binding.EnvelopeVersion, "Envelope version should be None");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("MessageHandlerFactory is passed to binding element.")]
        public void MessageHandlerFactoryTest()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(typeof(MockValidMessageHandler));
            binding.MessageHandlerFactory = factory;

            BindingElementCollection bindingElements = binding.CreateBindingElements();
            HttpMessageHandlerBindingElement httpMessageHandlerBindingElement = bindingElements.Find<HttpMessageHandlerBindingElement>();
            Assert.AreSame(factory, httpMessageHandlerBindingElement.MessageHandlerFactory, "Message handler factory should be passed to message handler.");
            Assert.AreSame(httpMessageHandlerBindingElement.MessageHandlerFactory, binding.MessageHandlerFactory, "Message handler should come from message handler.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HostNameComparisonMode returns strong wildcard.")]
        public void HostNameComparisonModeTest()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            Assert.AreEqual(HostNameComparisonMode.StrongWildcard, binding.HostNameComparisonMode, "Unexpected host comparison mode");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Scheme returns 'http'.")]
        public void SchemeTest()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            Assert.AreEqual(Uri.UriSchemeHttp, binding.Scheme, "Unexpected scheme.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("CreateBindingElements() returns valid stack.")]
        public void CreateBindingElementsTest()
        {
            HttpMemoryBinding binding = new HttpMemoryBinding();
            BindingElementCollection collection = binding.CreateBindingElements();
            Assert.AreEqual(2, collection.Count, "Unexpected number of binding elements.");
            Assert.IsInstanceOfType(collection[0], typeof(HttpMessageHandlerBindingElement), "Unexpected binding element type");
            Assert.IsInstanceOfType(collection[1], typeof(HttpMemoryTransportBindingElement), "Unexpected transport binding element type");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetHttpMemoryHandler succeeds.")]
        public void GetHttpMemoryHandlerSucceeds()
        {
            Uri serviceAddress = new Uri("http://localhost:8080/contactsservice");
            Uri memoryServiceAddress = new Uri("http://memoryhost/contactsservice");
            HttpServiceHost serviceHost = new HttpServiceHost(typeof(ContactsService), serviceAddress);
            try
            {
                HttpMemoryBinding memoryBinding = new HttpMemoryBinding();
                serviceHost.AddServiceEndpoint(typeof(ContactsService), memoryBinding, memoryServiceAddress);
                serviceHost.Open();
                HttpMemoryHandler memoryHandler = memoryBinding.GetHttpMemoryHandler();
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