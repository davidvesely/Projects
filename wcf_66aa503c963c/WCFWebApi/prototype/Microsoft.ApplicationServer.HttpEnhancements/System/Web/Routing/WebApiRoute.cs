// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Web.Routing
{
    using System;
    using ServiceModel.Activation;

    public class WebApiRoute : ServiceRoute
    {
        public WebApiRoute(string routePrefix, ServiceHostFactoryBase serviceHostFactory, Type serviceType)
            : base(routePrefix, serviceHostFactory, serviceType)
        {                
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }
}
