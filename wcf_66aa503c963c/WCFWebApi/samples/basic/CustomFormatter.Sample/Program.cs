// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace CustomFormatter.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using System.Net.Http;

    /// <summary>
    /// This program will show how to add a <see cref="CustomXmlMediaTypeFormatter"/> and how that affects the 
    /// content negotiation.
    /// </summary>
    public class Program
    {
        static Uri baseAddress = new Uri("http://localhost:8080/");

        public static HttpServiceHost ConfigureAndOpenHost()
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            // insert custom xml media type formatter at the first position
            //httpConfig.Formatters.Insert(0, new CustomXmlMediaTypeFormatter());

            httpConfig.Formatters.Add(new CustomXmlMediaTypeFormatter());
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), httpConfig, baseAddress);
            host.AddDefaultEndpoints();
            host.Open();

            return host;
        }

        public static HttpResponseMessage SendRequest(string path, int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StringContent("Id = " + id.ToString()+ "; Name = NewCustomer");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            PrintOutRequest(request);
            return client.SendAsync(request).Result;
        }

        public static HttpResponseMessage SendRequestWithAcceptHeader(string path, string acceptHeader)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));
            PrintOutRequest(request);
            return client.SendAsync(request).Result;
        }

        public static HttpResponseMessage SendRequestWithRelativeAddress(string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            PrintOutRequest(request);
            return client.SendAsync(request).Result;
        }

        public static HttpResponseMessage SendRequestWithContentType(string path, int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StringContent("Id = " + id.ToString() + "; Name = NewCustomer");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            PrintOutRequest(request);
            return client.SendAsync(request).Result;
        }

        public static void RemoveCustomer(string path, int id)
        {
            // Put server back in original state
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress; 
            client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, path + "?id=" + id.ToString())).Wait();
        }

        public static void CloseHost(HttpServiceHost host)
        {
            try
            {
                host.Close();
            }
            catch
            {
                host.Abort();
            }
        }

        static void PrintOutResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Response content type is " + response.Content.Headers.ContentType.MediaType);
            Console.WriteLine("\n{0}\n", response.ToString());
            Console.WriteLine("\n{0}\n", response.Content.ReadAsStringAsync().Result);
        }

        static void PrintOutRequest(HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                Console.WriteLine("request content type is " + request.Content.Headers.ContentType.MediaType);
            }

            Console.WriteLine("\n{0}\n", request.ToString());
        }

        public static void Main()
        {
            HttpServiceHost host = null;

            try
            {
                host = ConfigureAndOpenHost();
                HttpResponseMessage result = null;

                // first of all, we try to map on the response's content type
                // custom xml formatter get selected because it matches the response content type set by user in the service operation
                result = SendRequest("CustomersWithCustomContent", 7);
                PrintOutResponse(result); // we are expecting the "application/foo" in the response, and the response is actually processed by the MyMediaTypeFormatter user added
                RemoveCustomer("CustomersWithCustomContent", 7);

                // built in json formatter get selected because it adds the supported media type that matches the response header
                result = SendRequest("CustomersWithDynamicCustomContent", 8);
                PrintOutResponse(result); // we are expecting the "application/bar" in the response, and response is actually processed by json formatter
                RemoveCustomer("CustomersWithDynamicCustomContent", 8);

                // secondly, we tried to match on the accept header. This is the most common case
                result = SendRequestWithAcceptHeader("/1", "application/foo");
                PrintOutResponse(result); // we are expecting the "application/foo" in the response

                // thirdly, we match by the media type mapping: querystringmapping, mediaRangeMapping, uriPathExtensionMapping
                result = SendRequestWithRelativeAddress("/2?$format=foo");
                PrintOutResponse(result);

                // fourth option: we use the request's content type header
                result = SendRequestWithContentType("/Customers", 9);
                PrintOutResponse(result);
                RemoveCustomer("/Customers", 9);

                // last: we default it to default formatter
                result = SendRequestWithRelativeAddress("/2");
                PrintOutResponse(result);
            }
            finally
            {
                if (host != null)
                {
                    CloseHost(host);
                }
            }
        }
    }
}
