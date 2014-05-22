using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using JsonpSample.Apis;
using Microsoft.ApplicationServer.Http;

namespace JsonpSample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = new WebApiConfiguration() { EnableTestClient = true };
            RouteTable.Routes.MapServiceRoute<HelloWorldApi>("api", config);
        }
    }
}