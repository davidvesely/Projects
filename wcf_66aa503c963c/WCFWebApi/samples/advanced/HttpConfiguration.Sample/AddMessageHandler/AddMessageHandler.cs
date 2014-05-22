// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class AddMessageHandler
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";           
            HttpServiceHost host = null;
            try
            {
                string apiKey = "P@$$w0rd";
                var config = new HttpConfiguration
                {
                    // Adding an API key message handler
                    MessageHandlerFactory = () => 
                        new List<DelegatingHandler> { new ApiKeyHandler() { Key = apiKey } }
                };
                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                UriBuilder uriBuilder = new UriBuilder(baseAddress);
                uriBuilder.Query = "apikey=" + apiKey;

                // Create a GET request with the API key
                HttpClient client = new HttpClient();
                var response = client.GetAsync(uriBuilder.Uri.ToString()).Result;
                // The request should complete successfully
                response.EnsureSuccessStatusCode();
                Console.WriteLine(response.ToString());
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                // Create a GET request without the API key
                response = client.GetAsync(baseAddress).Result;
                Console.WriteLine(response.ToString());
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.StatusCode != HttpStatusCode.Forbidden)
                {
                    throw new ApplicationException("The request should've failed because the API key is required.");
                }
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }  
        }
    }
}
