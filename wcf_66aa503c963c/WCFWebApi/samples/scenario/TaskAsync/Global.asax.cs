using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http;

namespace TaskAsync
{
    public class Global : System.Web.HttpApplication
    {
        
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.SetDefaultHttpConfiguration(new WebApiConfiguration() { EnableTestClient = true });

            // setting up contacts services
            RouteTable.Routes.MapServiceRoute<AggregatorApi>("aggregator");
            RouteTable.Routes.MapServiceRoute<BackendApi>("backend");
        }

    }
}