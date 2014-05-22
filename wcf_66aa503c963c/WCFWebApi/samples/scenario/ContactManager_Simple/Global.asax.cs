// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Simple
{
    using System;
    using System.Web.Routing;
    using Microsoft.ApplicationServer.Http;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.SetDefaultHttpConfiguration(new WebApiConfiguration() { EnableTestClient = true });
            
            RouteTable.Routes.MapServiceRoute<ContactApi>("Contact");
            RouteTable.Routes.MapServiceRoute<ContactsApi>("Contacts");
        }
    }
}