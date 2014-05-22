// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Web.Routing;
    using Microsoft.ApplicationServer.Http;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Formatting;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // use MEF for providing instances
            var catalog = new AssemblyCatalog(typeof(Global).Assembly);
            var container = new CompositionContainer(catalog);
            var config = new MefConfiguration(container);
            config.EnableTestClient = true;
            RouteTable.Routes.SetDefaultHttpConfiguration(config);

            //remove support for OData json
            var odataFormatter = new ODataMediaTypeFormatter();
            odataFormatter.SupportedMediaTypes.Clear();
            odataFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/atom+xml"));

            config.Formatters.AddRange(
                    odataFormatter,
                    new ContactPngFormatter(),
                    new VCardFormatter(),
                    new CalendarFormatter());

            config.MessageHandlerFactory = () => container.GetExportedValues<DelegatingHandler>();
                                
            RouteTable.Routes.MapServiceRoute<ContactApi>("Contact");
            RouteTable.Routes.MapServiceRoute<ContactsApi>("Contacts");
        }
    }


}