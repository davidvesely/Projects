using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.Web.Mvc;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace SecureServices.Apis
{
    public class GreetingService
    {
        private List<Person> _people;

        public GreetingService(IEnumerable<Person> people)
        {
            _people = new List<Person>(people);
        }

        [WebGet(UriTemplate = "")]
        public string GetGreeting() 
        {
            return "Hello";
        }

        [WebGet(UriTemplate = "{name}")]
        [Authorize]
        public string GetPersonalizedGreeting(string name)
        {
            var person = _people.FirstOrDefault(p => p.Name == name);
            if (person == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            return string.Format("Hello {0}! You are {1} years old today.", name, person.Age);
        }
    }
}