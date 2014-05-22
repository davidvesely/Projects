// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WindowsCredentialsFlowTests
    {
        private static void VerifySecurityProperty(HttpRequestMessage request, bool verifyOperationContext)
        {
            Assert.IsNotNull(request, "Request can not be null");
            object messageSecurityAsObj = request.Properties["Security"];
            Assert.IsNotNull(messageSecurityAsObj, "Request did not contain a property with name 'Security'");
            SecurityMessageProperty securityProperty = messageSecurityAsObj as SecurityMessageProperty;
            Assert.IsNotNull(securityProperty, "Request property with name 'Security' was not of type SecurityMessageProperty");

            if (verifyOperationContext)
            {
                ServiceSecurityContext securityContextFromProperty = securityProperty.ServiceSecurityContext;
                Assert.IsNotNull(OperationContext.Current, "Operation context should not be null");
                ServiceSecurityContext securityContextFromContext = OperationContext.Current.ServiceSecurityContext;
                Assert.AreSame(securityContextFromProperty, securityContextFromContext, "Security contexts should be identical.");
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.WindowsCredentials)]
        public void WindowsCredentialsFlowTest1()
        {
            this.WindowsCredentialsFlowTest(false, false);
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
         [Owner(TestOwner.WindowsCredentials)]
        public void WindowsCredentialsFlowTest2()
        {
            this.WindowsCredentialsFlowTest(true, false);
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
         [Owner(TestOwner.WindowsCredentials)]
        public void WindowsCredentialsFlowTest3()
        {
            this.WindowsCredentialsFlowTest(false, true);
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
         [Owner(TestOwner.WindowsCredentials)]
        public void WindowsCredentialsFlowTest4()
        {
            this.WindowsCredentialsFlowTest(true, true);
        }

        public void WindowsCredentialsFlowTest(bool asynchronousSendEnabled, bool streamed)
        {
            HttpBindingSecurity bindingSecurity = new HttpBindingSecurity();
            bindingSecurity.Mode = HttpBindingSecurityMode.TransportCredentialOnly;
            bindingSecurity.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            bindingSecurity.Transport.ProxyCredentialType = HttpProxyCredentialType.Windows;

            using (var host = TestServiceHost.CreateWebHost<TestService>(
                asynchronousSendEnabled,
                false,
                streamed,
                new HttpMessageHandlerFactory(typeof(TestHandler)),
                bindingSecurity))
            {
                using (var client = TestServiceClient.CreateClient(true, TestServiceCommon.ServiceAddress, CredentialCache.DefaultCredentials))
                {
                    var result = TestServiceClient.RunClient(client, TestHeaderOptions.InsertRequest | TestHeaderOptions.ValidateResponse);
                    foreach (var httpResponse in result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
                        Assert.AreEqual(TestWebServiceBase.HttpReasonPhrase, httpResponse.ReasonPhrase);
                        Assert.AreEqual("text/plain", httpResponse.Content.Headers.ContentType.MediaType);
                        Assert.AreEqual("utf-8", httpResponse.Content.Headers.ContentType.CharSet);
                        var body = httpResponse.Content.ReadAsStringAsync().Result;
                        Assert.AreEqual(body, TestWebServiceBase.HttpResponseContent);
                    }
                }
            }
        }

        public class TestService : TestWebServiceBase
        {
            public override HttpResponseMessage BasicOperation(HttpRequestMessage request)
            {
                VerifySecurityProperty(request, true);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.ReasonPhrase = TestWebServiceBase.HttpReasonPhrase;
                response.Content = new StringContent(TestWebServiceBase.HttpResponseContent, Encoding.UTF8);
                TestServiceCommon.CopyTestHeader(request, response);
                return response;
            }
        }

        public class TestHandler : DelegatingHandler
        {
            public TestHandler() { }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
            {
                VerifySecurityProperty(httpRequest, false);
                TestServiceCommon.DetectRequestTestHeader(httpRequest);
                return base.SendAsync(httpRequest, cancellationToken).ContinueWith<HttpResponseMessage>(
                    task =>
                    {
                        VerifySecurityProperty(httpRequest, false);
                        TestServiceCommon.DetectResponseTestHeader(task.Result);
                        return task.Result;
                    },
                    cancellationToken);
            }
        }
    }
}