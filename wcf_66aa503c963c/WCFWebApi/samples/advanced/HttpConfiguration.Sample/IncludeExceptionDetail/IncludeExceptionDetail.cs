// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class IncludeExceptionDetail
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Setting to include exception detail when the request fails
                config.IncludeExceptionDetail = true;

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request to get an item by id, if the id is less than zero the service operation will throw an exception
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/item/-1").Result;

                if (response.IsSuccessStatusCode)
                {
                    throw new ApplicationException("The request should not be successful");
                }

                string responseString = response.Content.ReadAsStringAsync().Result;
                if (!responseString.Contains("The exception stack trace"))
                {
                    throw new ApplicationException("The response should show the exception stack trace since IncludeExceptionDetail is set to true.");
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
