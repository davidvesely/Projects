// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.TestCases
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using System.Net.Http;

    [TestClass]
    public class CustomHttpOperationSelector
    {
        const string address = "http://localhost/HelloService";

        [TestMethod]
        public void TestCustomOperationSelector()
        {
            using (HttpServiceHost host = new HttpServiceHost(typeof(MyHelloService), address))
            {
                host.AddDefaultEndpoints();

                foreach (HttpEndpoint endpoint in host.Description.Endpoints.OfType<HttpEndpoint>())
                {
                    HttpBehavior behavior = endpoint.Behaviors.Remove<HttpBehavior>();
                    endpoint.Behaviors.Add(new MyHttpBehavior());
                }

                host.Open();

                HttpClient proxy = new HttpClient();
                Assert.AreEqual("Hello2", proxy.GetAsync(address + "/Hello").Result.Content.ReadAsAsync<string>().Result);
            }
        }
    }

    public class MyHttpBehavior : HttpBehavior
    {
        protected override void OnApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            base.OnApplyDispatchBehavior(endpoint, endpointDispatcher);
            endpointDispatcher.DispatchRuntime.OperationSelector = new MyOperationSelector(endpoint.Address.Uri, endpoint.Contract.Operations.Select(od => od.ToHttpOperationDescription()));
        }
    }

    public class MyOperationSelector : UriAndMethodOperationSelector
    {
        public MyOperationSelector(Uri baseAddress, IEnumerable<HttpOperationDescription> httpOperations)
            : base(baseAddress, httpOperations)
        {
        }

        protected override bool OnTrySelectOperation(System.Net.Http.HttpRequestMessage request, out string operationName, out bool matchDifferByTrailingSlash)
        {
            bool found = base.OnTrySelectOperation(request, out operationName, out matchDifferByTrailingSlash);

            if (operationName == "Hello")
            {
                // add some logic to customize here
                operationName = "Hello2";
            }

            return true;
        }
    }

    [ServiceContract]
    public class MyHelloService
    {
        [WebGet(UriTemplate="/Hello")]
        public string Hello()
        {
            return "Hello";
        }

        [WebGet(UriTemplate = "/Hello2")]
        public string Hello2()
        {
            return "Hello2";
        }

        [WebInvoke(Method="POST", UriTemplate = "/Hello3")]
        public string Hello3(string input)
        {
            return "Hello3";
        }
    }
}
