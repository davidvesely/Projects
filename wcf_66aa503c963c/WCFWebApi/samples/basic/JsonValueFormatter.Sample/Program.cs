// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace JsonValueFormatter.Sample
{
    using System;
    using System.Json;
    using System.Net.Http;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Configuration;

    /// <summary>
    /// This sample illustrates how to use <see cref="JsonValue"/> both on the client side
    /// and the server side of a Web API.
    /// </summary>
    public class Program
    {
        private static int maxSize = 4 * 1024 * 1024;
        private static string sampleServiceAddress = "http://localhost:8080/sample";

        public static HttpServiceHost OpenHost()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.TransferMode = TransferMode.Buffered;
            config.MaxBufferSize = maxSize;
            config.MaxReceivedMessageSize = maxSize;

            HttpServiceHost host = new HttpServiceHost(typeof(SampleService), config, sampleServiceAddress);
            host.Open();

            Console.WriteLine("Service '{0}' listening on '{1}'", typeof(SampleService).Name, sampleServiceAddress);
            return host;
        }

        public static void CloseHost(HttpServiceHost host)
        {
            if (host != null)
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
        }

        private static void RunClient()
        {
            // Create handler so that we can set max request message size
            WebRequestHandler handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = maxSize;

            // Create http client and set max response message size
            HttpClient client = new HttpClient(handler);
            client.MaxResponseContentBufferSize = maxSize;

            // Create a big JsonValue object
            int size = 32 * 1024;
            JsonPrimitive[] hello = new JsonPrimitive[size];
            for (int index = 0; index < size; index++)
            {
                hello[index] = new JsonPrimitive("hello world hello world hello world hello world hello world hello world hello world");
            }
            JsonValue bigValue = new JsonArray(hello);

            // Issue GET request to retrieve JsonValue hitting the "/message" path
            using (HttpResponseMessage response = client.GetAsync(sampleServiceAddress + "/message").Result)
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Received response from server: {0}", response);
            }

            // Issue POST request containing our big value hitting the "/message" path
            using (HttpResponseMessage response = client.PostAsync(sampleServiceAddress + "/message", new ObjectContent<JsonValue>(bigValue, "application/json")).Result)
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Received response from server: {0}", response);
            }

            // Issue GET request against server hitting the "/object" path
            using (HttpResponseMessage response = client.GetAsync(sampleServiceAddress + "/object").Result)
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Received response from server: {0}", response);
            }

            // Issue POST request against server hitting the "/object" path
            using (HttpResponseMessage response = client.PostAsync(sampleServiceAddress + "/object", new ObjectContent<JsonValue>(bigValue, "application/json")).Result)
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Received response from server: {0}", response);
            }

            // Done with the client
            client.Dispose();
        }

        public static void Main()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost();

                RunClient();
            }
            finally
            {
                CloseHost(host);
            }
        }
    }
}