// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class AddResponseHandler
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            Counter counter = new Counter();
            try
            {
                var config = new HttpConfiguration
                {
                    // Adding a response handler that counts how many times the operations have been invoked
                    ResponseHandlers = (handlers, endpoint, operation) =>
                    {
                        handlers.Add(new OperationCountResponseHandler("response", counter));
                    }
                };

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Make a Post request
                HttpClient client = new HttpClient();
                var input = new ObjectContent<SampleItem>(
                    new SampleItem
                    {
                        Id = 4, 
                        StringValue = "four"
                    }, "text/xml");
                var response = client.PostAsync(baseAddress, input).Result;
                response.EnsureSuccessStatusCode();
                Console.WriteLine("\nresponse is " + response.ToString() + "\ncontent is " + response.Content.ReadAsStringAsync().Result);

                // Make a Get request
                response = client.GetAsync(baseAddress).Result;
                response.EnsureSuccessStatusCode();
                Console.WriteLine("\nresponse is " + response.ToString() + "\ncontent is " + response.Content.ReadAsStringAsync().Result);

                Console.WriteLine("counter.Count = " + counter.GetCount());

                // Verify count
                if (counter.GetCount() != 2)
                {
                    throw new ApplicationException("The OperationCountResponseHandler should've been added and the count should be 2.");
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
