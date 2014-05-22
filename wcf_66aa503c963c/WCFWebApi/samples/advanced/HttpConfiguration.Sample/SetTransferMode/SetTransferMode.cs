// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http;

    public static class SetTransferMode
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            string customText = "HelloStream";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Enable streaming
                config.TransferMode = TransferMode.Streamed;
                
                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);

                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request to /stream and provide a custom text which will be returned as part of response stream
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/stream/" + customText).Result;
                Stream contentStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader reader = new StreamReader(contentStream);
                if (!reader.ReadToEnd().Contains(customText))
                {
                    throw new ApplicationException("The customText should be part of the response stream.");
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
