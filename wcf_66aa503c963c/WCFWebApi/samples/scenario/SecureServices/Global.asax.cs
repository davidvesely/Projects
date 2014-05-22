using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http;
using SecureServices.Apis;
using SecureServices.Infrastructure;

namespace SecureServices
{
    public class Global : System.Web.HttpApplication
    {
        private const string FACEBOOK_APP_ID = "YOUR FB APP ID";
        private const string FACEBOOK_APP_SECRET = "YOUR FB APP SECRET";

        protected void Application_Start(object sender, EventArgs e)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.EnableTestClient = true;

            List<Person> myPeople = new List<Person>() 
            { 
                new Person("howard", 34),
                new Person("jennifer", 35),
                new Person("grace", 4)
            };

            config.CreateInstance = (t, ic, rm) =>
            {
                if (t == typeof(GreetingService))
                    return new GreetingService(myPeople);
                return Activator.CreateInstance(t);
            };

            config.RegisterFacebookOAuth(new FacebookOAuthClient(FACEBOOK_APP_ID, FACEBOOK_APP_SECRET));

            RouteTable.Routes.MapServiceRoute<GreetingService>("greeting", config);
        }
    }
}