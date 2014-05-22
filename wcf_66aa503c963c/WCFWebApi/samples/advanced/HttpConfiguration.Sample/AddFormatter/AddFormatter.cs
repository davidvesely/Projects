// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http;

    public static class AddFormatter
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // adding the PlainTextFormatter to handle 'text/plain'
                config.Formatters.Add(new PlainTextFormatter());

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request with accept header = "text/plain"
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                var response = client.GetAsync(baseAddress).Result;
                response.EnsureSuccessStatusCode();

                try
                {
                    string responseStr = response.Content.ReadAsAsync<string>(new List<MediaTypeFormatter> { new PlainTextFormatter() }).Result;
                    Console.WriteLine("Printing response:\r\n" + responseStr);
                }
                catch
                {
                    Console.WriteLine("The 'text/plain' format was not handled by the service because PlainTextFormatter was not added correctly.");
                    throw;
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
