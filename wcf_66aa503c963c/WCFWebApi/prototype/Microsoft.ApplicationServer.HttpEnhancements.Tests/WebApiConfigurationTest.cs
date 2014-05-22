// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.HttpEnhancements.Tests
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebApiConfigurationTest
    {
        private HttpServiceHost host;
        private WebApiConfiguration config;

        [TestInitialize]
        public void Initialize()
        {
            config = new WebApiConfiguration();
            host = new HttpServiceHost(typeof(DummyWebApi), config, "http://localhost/test");
            host.Open();
        }

        [TestCleanup]
        public void Cleanup()
        {
            host.Close();
        }

        [TestMethod]
        public void WhenWebGetAttributeHasUriTemplateOfNullIsSetToEmpty()
        {
            var contract = GetContract();
            var webGet = GetWebGetAttributeForOperation(contract, "Get1");
            Assert.AreEqual(string.Empty, webGet.UriTemplate);
            
        }

        [TestMethod]
        public void WhenWebGetAttributeHasUriTemplateOfNonNullIsNotChanged()
        {
            var contract = GetContract();
            var webGet = GetWebGetAttributeForOperation(contract, "Get2");
            Assert.AreEqual("test1", webGet.UriTemplate);

        }

        [TestMethod]
        public void WhenWebInvokeAttributeMethodIsNullAndOperationIsStartsWithPostThenMethodIsPost()
        {
            var contract = GetContract();
            var webInvoke = GetWebInvokeAttributeForOperation(contract, "PostSomething");
            Assert.AreEqual("POST", webInvoke.Method);
        }

        [TestMethod]
        public void WhenWebInvokeAttributeMethodIsNullAndOperationIsPostThenMethodIsPost()
        {
            var contract = GetContract();
            var webInvoke = GetWebInvokeAttributeForOperation(contract, "Post");
            Assert.AreEqual("POST", webInvoke.Method);
        }


        [TestMethod]
        public void WhenWebInvokeAttributeMethodIsNullAndOperationIsSetThenIsNotChanged()
        {
            var contract = GetContract();
            var webInvoke = GetWebInvokeAttributeForOperation(contract, "PostFoo");
            Assert.AreEqual("FOO", webInvoke.Method);
        }

        [TestMethod]
        public void WhenWebInvokeMethodIsNullAndCannotDetermineMethodFromOperationNameThenMethodIsNull()
        {
            var contract = GetContract();
            var webInvoke = GetWebInvokeAttributeForOperation(contract, "FooFoo");
            Assert.AreEqual(null, webInvoke.Method);
        }

        [TestMethod]
        public void DefaultJsonMediaTypeFormatterSupportsJsonp()
        {
            var config = new WebApiConfiguration();
            Assert.IsTrue(config.Formatters.JsonFormatter is JsonpMediaTypeFormatter);
        }

        [TestMethod]
        public void DefaultJsonValueMediaTypeFormatterSupportsJsonp()
        {
            var config = new WebApiConfiguration();
            Assert.IsTrue(config.Formatters.JsonValueFormatter is JsonpValueMediaTypeFormatter);
        }

        [TestMethod]
        public void EndpointsHaveWebApiHttpOperationHandlerFactory()
        {
            WebApiHttpOperationHandlerFactory factory = null;
            foreach (var endpoint in host.Description.Endpoints)
            {
                var httpEndpoint = endpoint as HttpEndpoint;
                if (httpEndpoint != null)
                {
                    if (factory != null)
                    {
                        Assert.AreSame(factory, httpEndpoint.OperationHandlerFactory);
                    }
                    else
                    {
                        Assert.IsTrue(httpEndpoint.OperationHandlerFactory is WebApiHttpOperationHandlerFactory);
                        factory = httpEndpoint.OperationHandlerFactory as WebApiHttpOperationHandlerFactory;
                        Assert.AreSame(factory.RequestHandlerDelegate, config.RequestHandlers);
                        Assert.AreSame(factory.ResponseHandlerDelegate, config.ResponseHandlers);
                        Assert.AreEqual(factory.Formatters.Count, config.Formatters.Count);
                        for (int i = 0; i < factory.Formatters.Count; i++)
                        {
                            Assert.AreSame(factory.Formatters[i], config.Formatters[i]);
                        }
                    }
                }
            }
        }

        private ContractDescription GetContract()
        {
            return host.Description.Endpoints[0].Contract;
        }

        private WebGetAttribute GetWebGetAttributeForOperation(ContractDescription description, string operationName)
        {
            var operation = description.Operations.Single(o => o.Name == operationName);
            return operation.Behaviors.Find<WebGetAttribute>();
        }

        private WebInvokeAttribute GetWebInvokeAttributeForOperation(ContractDescription description, string operationName)
        {
            var operation = description.Operations.Single(o => o.Name == operationName);
            return operation.Behaviors.Find<WebInvokeAttribute>();
        }


        [ServiceContract]
        public class DummyWebApi {
            [WebGet]
            public string Get1()
            {
                return null;
            }

            [WebGet(UriTemplate="test1")]
            public string Get2()
            {
                return null;
            }

            [WebInvoke]
            public void PostSomething()
            {
            }

            [WebInvoke(UriTemplate="test2")] //dummy uri template necessary for the test
            public void Post()
            {
            }

            [WebInvoke(Method = "FOO", UriTemplate = "test3")] //dummy uri template necessary for the test
            public void PostFoo()
            {
            }

            [WebInvoke(UriTemplate="test4")] //dummy uri template necessary for the test
            public void FooFoo()
            {
            }
        }

    }
}
