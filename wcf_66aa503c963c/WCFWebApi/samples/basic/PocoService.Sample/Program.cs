// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace PocoService.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;

    /// <summary>
    /// This program will show that a service type does not require <see cref="ServiceContractAttribute"/>
    /// to be used as a service.
    /// </summary>
    public class Program
    {
        static Uri baseAddress = new Uri("http://localhost:8080/");

        public static HttpServiceHost ConfigureAndOpenHost()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(PocoService), baseAddress);
            host.AddDefaultEndpoints();
            host.Open();

            return host;
        }

        public static HttpResponseMessage SendRequest(string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = baseAddress;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            return client.SendAsync(request).Result;
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
                HttpResponseMessage result = SendRequest("Item/SomeName");
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
