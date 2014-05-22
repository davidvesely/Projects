// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class EnableHelpPage
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            string helpPageUrl = baseAddress + "/help";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Enabling help page
                config.EnableHelpPage = true;

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request to the help page
                HttpClient client = new HttpClient();
                var response = client.GetAsync(helpPageUrl).Result;

                // The request should complete successfully because the help page is enabled
                response.EnsureSuccessStatusCode();

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
