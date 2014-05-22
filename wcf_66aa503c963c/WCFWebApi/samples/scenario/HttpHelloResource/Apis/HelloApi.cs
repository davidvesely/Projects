// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpHelloResource
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Json;

    // Illustrates how to create a service that works with raw http messages

    public class HelloApi
    {
        //for testing
        public static void Initialize(IList<string> peopleToSayHelloTo)
        {
            HelloApi.peopleToSayHelloTo = peopleToSayHelloTo;
        }

        private static IList<string> peopleToSayHelloTo = new List<string>();

        [WebGet]
        public HttpResponseMessage Get()
        {
            var body = string.Format("Hello {0}", string.Join(",", peopleToSayHelloTo));
            var response = new HttpResponseMessage();
            response.Content = new StringContent(body, Encoding.UTF8, "text/plain");
            return response;
        }

        //The post method works directly with raw http requests and responses
        [WebInvoke]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            dynamic formContent = request.Content.ReadAsAsync<JsonValue>().Result;
            var person = (string)formContent.person;
            peopleToSayHelloTo.Add(person);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(string.Format("Added {0}", person), Encoding.UTF8, "text/plain");
            response.StatusCode = HttpStatusCode.Created;
            return response;
        }
    }
}