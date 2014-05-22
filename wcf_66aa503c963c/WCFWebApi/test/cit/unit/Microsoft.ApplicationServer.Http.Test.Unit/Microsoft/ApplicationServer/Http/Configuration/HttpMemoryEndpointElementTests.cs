// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.TestCommon.WCF;

    [TestClass]
    public class HttpMemoryEndpointElementTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryEndpointElement ctor initializes all properties to known defaults")]
        public void Endpoint_Ctor_Initializes_All_Properties()
        {
            HttpMemoryEndpointElement el = new HttpMemoryEndpointElement();

            Assert.AreEqual(HttpBehavior.DefaultHelpEnabled, el.HelpEnabled, "HelpEnabled wrong");
            Assert.AreEqual(HttpBehavior.DefaultTestClientEnabled, el.TestClientEnabled, "TestClientEnabled wrong");
            Assert.AreEqual(HttpBehavior.DefaultTrailingSlashMode, el.TrailingSlashMode, "TrailingSlashMode wrong");
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "HttpOperationHandlerFactory should default to empty");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryEndpointElement set and test all mutable properties")]
        public void Endpoint_Set_Properties()
        {
            HttpMemoryEndpointElement el = new HttpMemoryEndpointElement();

            el.HelpEnabled = false;
            Assert.IsFalse(el.HelpEnabled, "HelpEnabled false");
            el.HelpEnabled = true;
            Assert.IsTrue(el.HelpEnabled, "HelpEnabled true");

            el.TestClientEnabled = false;
            Assert.IsFalse(el.TestClientEnabled, "TestClientEnabled false");
            el.TestClientEnabled = true;
            Assert.IsTrue(el.TestClientEnabled, "TestClientEnabled true");

            el.TrailingSlashMode = TrailingSlashMode.AutoRedirect;
            Assert.AreEqual(TrailingSlashMode.AutoRedirect, el.TrailingSlashMode, "Auto-redirect failed");
            el.TrailingSlashMode = TrailingSlashMode.Ignore;
            Assert.AreEqual(TrailingSlashMode.Ignore, el.TrailingSlashMode, "Ignore failed");

            el.OperationHandlerFactory = "hello";
            Assert.AreEqual("hello", el.OperationHandlerFactory, "HttpOperationHandlerFactory failed");
            el.OperationHandlerFactory = null;
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "Null handler provider failed");
            el.OperationHandlerFactory = "  ";
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "whitespace handler provider failed");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryEndpointElement.OnApplyConfiguration throws for client entry point")]
        public void Endpoint_Throws_OnApplyConfiguration_Client()
        {
            HttpMemoryEndpointElement el = new HttpMemoryEndpointElement();
            ContractDescription cd = ContractDescription.GetContract(typeof(CustomerService));
            ServiceEndpoint serviceEndpoint = new ServiceEndpoint(cd);
            ChannelEndpointElement channelEndpoint = new ChannelEndpointElement();

            // Protected method in sealed class requires reflection to invoke
            MethodInfo methodInfo = el.GetType().GetMethod(
                                                "OnApplyConfiguration",
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                null,
                                                new Type[] { typeof(ServiceEndpoint), typeof(ChannelEndpointElement) },
                                                null);

            UnitTest.Asserters.Exception.Throws<TargetInvocationException>(
                "ApplyConfiguration throws for client channel entry point",
                () => methodInfo.Invoke(el, new object[] { serviceEndpoint, channelEndpoint }),
                (e) => Assert.AreEqual(Http.SR.HttpEndpointNotSupported("HttpMemoryEndpoint", "HttpClient"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryEndpointElement.OnInitializeAndValidate throws for client entry point")]
        public void Endpoint_Throws_OnInitializeAndValidate_Client()
        {
            HttpMemoryEndpointElement el = new HttpMemoryEndpointElement();
            ChannelEndpointElement channelEndpoint = new ChannelEndpointElement();

            // Protected method in sealed class requires reflection to invoke
            MethodInfo methodInfo = el.GetType().GetMethod(
                                                "OnInitializeAndValidate",
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                null,
                                                new Type[] { typeof(ChannelEndpointElement) },
                                                null);

            UnitTest.Asserters.Exception.Throws<TargetInvocationException>(
                "ApplyConfiguration throws for client channel entry point",
                () => methodInfo.Invoke(el, new object[] { channelEndpoint }),
                (e) => Assert.AreEqual(Http.SR.HttpEndpointNotSupported("HttpMemoryEndpoint","HttpClient"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryEndpointElement.OnInitializeAndValidate throws wrong binding set")]
        public void Endpoint_Throws_OnInitializeAndValidate_Wrong_Binding()
        {
            HttpMemoryEndpointElement el = new HttpMemoryEndpointElement();
            ServiceEndpointElement endpointElement = new ServiceEndpointElement()
            {
                Binding = "bindingThatDoesntMatchExpected"
            };

            // Protected method in sealed class requires reflection to invoke
            MethodInfo methodInfo = el.GetType().GetMethod(
                                                "OnInitializeAndValidate",
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                null,
                                                new Type[] { typeof(ServiceEndpointElement) },
                                                null);

            UnitTest.Asserters.Exception.Throws<TargetInvocationException>(
                "ApplyConfiguration throws for wrong binding",
                () => methodInfo.Invoke(el, new object[] { endpointElement }),
                (e) => Assert.AreEqual(Http.SR.HttpEndpointRequiredBinding(typeof(HttpMemoryEndpoint).Name, "httpMemoryBinding"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpServiceHost gets configured endpoint from host")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryEndpointWithServiceTest.config")]
        public void Endpoint_Configured_Endpoint_From_ServiceHost()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryEndpointWithServiceTest.config", () =>
            {
                HttpMemoryEndpoint[] endPoints = GetEndpointsFromServiceHost(typeof(CustomerService), new Uri("http://somehost"));
                Assert.IsTrue(endPoints.Length > 0, "No HttpMemoryEndpoints");
                HttpMemoryEndpoint endPoint = endPoints[0];
                Assert.AreEqual("HttpMemoryBinding_CustomerService", endPoint.Name, "Should have had this name");
                Assert.IsFalse(endPoint.HelpEnabled, "HelpEnabled wrong");
                Assert.IsFalse(endPoint.TestClientEnabled, "TestClientEnabled wrong");
            });
        }

        private static HttpMemoryEndpoint[] GetEndpointsFromServiceHost(Type serviceType, Uri uri)
        {
            HttpServiceHost host = new HttpServiceHost(serviceType, uri);
            Assert.AreEqual(1, host.Description.Endpoints.Count, "Expected one endpoint added from config");
            return host.Description.Endpoints.OfType<HttpMemoryEndpoint>().ToArray();
        }

        private static HttpMemoryEndpoint[] GetEndpointsFromServiceHostOpen(Type serviceType, Uri uri)
        {
            HttpServiceHost host = new HttpServiceHost(serviceType, uri);
            try
            {
                host.Open();
            }
            catch (AddressAlreadyInUseException)
            {
                // currently necessary to recover from failed attempt to open port 80 again
            }

            return host.Description.Endpoints.OfType<HttpMemoryEndpoint>().ToArray();
        }
    }
}
