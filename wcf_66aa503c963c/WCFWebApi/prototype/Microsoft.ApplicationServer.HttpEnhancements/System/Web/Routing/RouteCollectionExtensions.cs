// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Web.Routing
{
    using System;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Activation;

    public static class RouteCollectionExtensions
    {
        private static object lockObject = new object();
        private static HttpConfiguration defaultConfiguration;

        //default config
        static RouteCollectionExtensions()
        {
            lock(lockObject) {
                defaultConfiguration = new WebApiConfiguration(true);
            }
        }

        public static void SetDefaultHttpConfiguration(this RouteCollection routes, HttpConfiguration configuration)
        {
            defaultConfiguration = configuration;
        }

        public static HttpConfiguration GetDefaultHttpConfiguration(this RouteCollection routes)
        {
            return defaultConfiguration;
        }

        public static void MapServiceRoute<TService>(this RouteCollection routes, string routePrefix, HttpConfiguration configuration = null, object constraints = null, bool useMethodPrefixForHttpMethod = true)
        {
            if (configuration == null)
            {
                configuration = defaultConfiguration;
            }
            
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            var route = new WebApiRoute(routePrefix, new HttpServiceHostFactory() {Configuration = configuration},
                                        typeof (TService)) {Constraints = new RouteValueDictionary(constraints)};
            routes.Add(route);
        }
    }
}
