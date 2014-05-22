// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class SetMaxBufferSizeAndMaxReceivedMessageSize
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Limiting MaxBufferSize
                config.MaxBufferSize = 600;
                // When TransferMode = Buffered, MaxBufferSize and MaxReceivedMessageSize should be the same
                config.MaxReceivedMessageSize = 600;

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a small POST request
                HttpClient client = new HttpClient();
                var input = new ObjectContent<SampleItem>(
                    new SampleItem
                    {
                        Id = 4,
                        StringValue = "four"
                    }, "text/xml");
                var response = client.PostAsync(baseAddress, input).Result;

                // The request should be successful
                response.EnsureSuccessStatusCode();

                // Create a larger POST request, exceeding MaxBufferSize 
                string str = new string('y', 1000);
                input = new ObjectContent<SampleItem>(
                    new SampleItem
                    {
                        Id = -1,
                        StringValue = str
                    }, "text/xml");
                response = client.PostAsync(baseAddress, input).Result;

                // The request should fail
                if (response.StatusCode != HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.RequestEntityTooLarge)
                {
                    throw new ApplicationException("The request should fail because the MaxBufferSize was set to 600.");
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
