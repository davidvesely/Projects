// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class AddErrorHandler
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration
                {
                    // Adding an error handler that always returns 200 with the error message in the body 
                    ErrorHandlers = (handlers, endpoint, descriptions) =>
                    {
                        handlers.Add(new AlwaysOkErrorHandler());
                    }
                };

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request to get an item by id, if the id is less than zero the service operation will throw an exception
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/item/-1").Result;

                // The status code for the response should be 200
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("The AlwaysOkErrorHandler should've been added and the status code should be 200 (OK).");
                }

                Console.WriteLine("Printing response: " + response.Content.ReadAsStringAsync().Result);
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
