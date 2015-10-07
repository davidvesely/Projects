using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CarRental.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "availablecars",
            //    routeTemplate: "api/reservation/availablecars",
            //    defaults: new { controller = "ReservationApi", action = "GetAvailableCars", id = UrlParameter.Optional }

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
