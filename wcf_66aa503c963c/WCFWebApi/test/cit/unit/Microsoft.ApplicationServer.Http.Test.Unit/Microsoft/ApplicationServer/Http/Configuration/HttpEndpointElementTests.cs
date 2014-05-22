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

    [TestClass]
    public class HttpEndpointElementTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpEndpointElement ctor initializes all properties to known defaults")]
        public void Endpoint_Ctor_Initializes_All_Properties()
        {
            HttpEndpointElement el = new HttpEndpointElement();

            Assert.AreEqual(HttpBehavior.DefaultHelpEnabled, el.HelpEnabled, "HelpEnabled wrong");
            Assert.AreEqual(HttpBehavior.DefaultTestClientEnabled, el.TestClientEnabled, "TestClientEnabled wrong");
            Assert.AreEqual(HttpBehavior.DefaultTrailingSlashMode, el.TrailingSlashMode, "TrailingSlashMode wrong");
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "HttpOperationHandlerFactory should default to empty");

            HostNameComparisonMode mode = el.HostNameComparisonMode;
            Assert.AreEqual(HostNameComparisonMode.StrongWildcard, mode, "HostNameComparisonMode failed");

            long maxBufferPoolSize = el.MaxBufferPoolSize;
            long expectedMaxBufferPoolSize = 0x80000L;
            Assert.AreEqual(expectedMaxBufferPoolSize, maxBufferPoolSize, "MaxBufferPoolSize failed");

            Assert.AreEqual(0x10000, el.MaxBufferSize, "MaxBufferSize failed");
            Assert.AreEqual(0x10000L, el.MaxReceivedMessageSize, "MaxReceivedMessageSize failed");

            HttpBindingSecurityElement secElement = el.Security;
            Assert.IsNotNull(secElement, "Security failed");

            TransferMode xferMode = el.TransferMode;
            Assert.AreEqual(TransferMode.Buffered, xferMode, "TransferMode failed");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpEndpointElement set and test all mutable properties")]
        public void Endpoint_Set_Properties()
        {
            HttpEndpointElement el = new HttpEndpointElement();

            el.HelpEnabled = false;
            Assert.IsFalse(el.HelpEnabled, "HelpEnabled false");
            el.HelpEnabled = true;
            Assert.IsTrue(el.HelpEnabled, "HelpEnabled true");

            el.TestClientEnabled = false;
            Assert.IsFalse(el.TestClientEnabled, "TestClientEnabled false");
            el.TestClientEnabled = true;
            Assert.IsTrue(el.TestClientEnabled, "TestClientEnabled true");

            el.TrailingSlashMode = TrailingSlashMode.AutoRedirect;
            Assert.AreEqual(TrailingSlashMode.AutoRedirect, el.TrailingSlashMode, "Autoredirect failed");
            el.TrailingSlashMode = TrailingSlashMode.Ignore;
            Assert.AreEqual(TrailingSlashMode.Ignore, el.TrailingSlashMode, "Ignore failed");

            el.OperationHandlerFactory = "hello";
            Assert.AreEqual("hello", el.OperationHandlerFactory, "HttpOperationHandlerFactory failed");
            el.OperationHandlerFactory = null;
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "Null handler provider failed");
            el.OperationHandlerFactory = "  ";
            Assert.AreEqual(string.Empty, el.OperationHandlerFactory, "whitespace handler provider failed");

            el.HostNameComparisonMode = HostNameComparisonMode.Exact;
            Assert.AreEqual(HostNameComparisonMode.Exact, el.HostNameComparisonMode, "Exact failed");
            el.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            Assert.AreEqual(HostNameComparisonMode.StrongWildcard, el.HostNameComparisonMode, "StrongWildcard failed");
            el.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
            Assert.AreEqual(HostNameComparisonMode.WeakWildcard, el.HostNameComparisonMode, "WeakWildcard failed");

            el.MaxBufferPoolSize = long.MaxValue;
            Assert.AreEqual(long.MaxValue, el.MaxBufferPoolSize, "MaxBufferPoolSize max failed");
            el.MaxBufferPoolSize = 0L;
            Assert.AreEqual(0L, el.MaxBufferPoolSize, "MaxBufferPoolSize min failed");

            el.MaxBufferSize = int.MaxValue;
            Assert.AreEqual(int.MaxValue, el.MaxBufferSize, "MaxBufferSize max failed");
            el.MaxBufferSize = 1;
            Assert.AreEqual(1, el.MaxBufferSize, "MaxBufferSize 1 failed");

            el.MaxReceivedMessageSize = long.MaxValue;
            Assert.AreEqual(long.MaxValue, el.MaxReceivedMessageSize, "MaxReceivedMessageSize max failed");
            el.MaxReceivedMessageSize = 1;
            Assert.AreEqual(1, el.MaxReceivedMessageSize, "MaxReceivedMessageSize 1 failed");

            el.TransferMode = TransferMode.Buffered;
            Assert.AreEqual(TransferMode.Buffered, el.TransferMode, "Buffered failed");
            el.TransferMode = TransferMode.Streamed;
            Assert.AreEqual(TransferMode.Streamed, el.TransferMode, "Streamed failed");
            el.TransferMode = TransferMode.StreamedRequest;
            Assert.AreEqual(TransferMode.StreamedRequest, el.TransferMode, "StreamedRequest failed");
            el.TransferMode = TransferMode.StreamedResponse;
            Assert.AreEqual(TransferMode.StreamedResponse, el.TransferMode, "StreamedResponse failed");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpEndpointElement.OnApplyConfiguration throws for client entry point")]
        public void Endpoint_Throws_OnApplyConfiguration_Client()
        {
            HttpEndpointElement el = new HttpEndpointElement();
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
                (e) => Assert.AreEqual(Http.SR.HttpEndpointNotSupported("HttpEndpoint", "HttpClient"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpEndpointElement.OnInitializeAndValidate throws for client entry point")]
        public void Endpoint_Throws_OnInitializeAndValidate_Client()
        {
            HttpEndpointElement el = new HttpEndpointElement();
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
                (e) => Assert.AreEqual(Http.SR.HttpEndpointNotSupported("HttpEndpoint","HttpClient"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpEndpointElement.OnInitializeAndValidate throws wrong binding set")]
        public void Endpoint_Throws_OnInitializeAndValidate_Wrong_Binding()
        {
            HttpEndpointElement el = new HttpEndpointElement();
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
                (e) => Assert.AreEqual(Http.SR.HttpEndpointRequiredBinding(typeof(HttpEndpoint).Name, "httpBinding"), e.InnerException.Message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpServiceHost gets configured endpoint from host")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpEndpointWithServiceTest.config")]
        public void Endpoint_Configured_Endpoint_From_ServiceHost()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpEndpointWithServiceTest.config", () =>
            {
                HttpEndpoint[] endPoints = GetEndpointsFromServiceHost(typeof(CustomerService), new Uri("http://somehost"));
                Assert.IsTrue(endPoints.Length > 0, "No HttpEndpoints");
                HttpEndpoint endPoint = endPoints[0];
                Assert.AreEqual("HttpBinding_CustomerService", endPoint.Name, "Should have had this name");
                Assert.AreEqual(HostNameComparisonMode.Exact, endPoint.HostNameComparisonMode, "HostNameComparisonMode was not set");

                Assert.IsFalse(endPoint.HelpEnabled, "HelpEnabled wrong");
                Assert.IsFalse(endPoint.TestClientEnabled, "TestClientEnabled wrong");
                Assert.AreEqual(TransferMode.Streamed, endPoint.TransferMode, "TransferMode wrong");
                Assert.AreEqual(HostNameComparisonMode.Exact, endPoint.HostNameComparisonMode, "HostNameComparisonMode wrong");
                Assert.AreEqual(1, endPoint.MaxBufferPoolSize, "MaxBufferPoolSize wrong");
                Assert.AreEqual(2, endPoint.MaxBufferSize, "MaxBufferSize wrong");
                Assert.AreEqual(3, endPoint.MaxReceivedMessageSize, "MaxReceivedMessageSize wrong");
            });
        }

        private static HttpEndpoint[] GetEndpointsFromServiceHost(Type serviceType, Uri uri)
        {
            HttpServiceHost host = new HttpServiceHost(serviceType, uri);
            Assert.AreEqual(1, host.Description.Endpoints.Count, "Expected one endpoint added from config");
            return host.Description.Endpoints.OfType<HttpEndpoint>().ToArray();
        }

        private static HttpEndpoint[] GetEndpointsFromServiceHostOpen(Type serviceType, Uri uri)
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

            return host.Description.Endpoints.OfType<HttpEndpoint>().ToArray();
        }
    }
}
