// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.TestCases
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CustomOperationHandlerTest
    {
        const string address = "http://localhost/HelloService";

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("Test adding a custom operation handler")]
        public void TestCustomOperationHandler()
        {
            using (HttpServiceHost host = new HttpServiceHost(typeof(MyGridService), address))
            {
                host.AddDefaultEndpoints();

                foreach (HttpEndpoint endpoint in host.Description.Endpoints.OfType<HttpEndpoint>())
                {
                    endpoint.OperationHandlerFactory = new CustomOperationHandlerFactory();
                    endpoint.HelpEnabled = true;
                }

                host.Open();

                HttpClient proxy = new HttpClient();
                proxy.Timeout = TimeSpan.FromHours(1);
                HttpResponseMessage response = proxy.GetAsync(address + "/blah/3/4").Result;
                Assert.IsTrue(response.Content.ReadAsStringAsync().Result.Contains("7"));
            }
        }
    }

    [ServiceContract]
    public class MyGridService
    {
        [WebGet(UriTemplate = "/blah/{input1}/{input2}")]
        int Method(GridPosition gridPosition)
        {
            return gridPosition.X + gridPosition.Y;
        }
    }

    public class GridHandler : HttpOperationHandler<int, int, GridPosition>
    {
        public GridHandler() 
            : base("gridPostition")
        { }

        protected override GridPosition OnHandle(int input1, int input2)
        {
            return new GridPosition(input1, input2);
        }
    }

    public class GridPosition
    {
        public GridPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }

    public class CustomOperationHandlerFactory : HttpOperationHandlerFactory
    {
        protected override Collection<HttpOperationHandler> OnCreateRequestHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            var handlers = base.OnCreateRequestHandlers(endpoint, operation);
            handlers.Add(new GridHandler());
            return handlers;
        }
    }
}
