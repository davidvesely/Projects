// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class AddRequestHandler
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration
                {
                    // Adding a request handler that validates the input - SampleItem.Id should be greater than zero
                    RequestHandlers = (handlers, endpoint, operation) =>
                    {
                        // Add it only to the operations with parameter of type SampleItem
                        var sampleItemParameter = operation.InputParameters.FirstOrDefault(p => p.ParameterType == typeof(SampleItem));
                        if (sampleItemParameter != null)
                        {
                            handlers.Add(new SampleItemRequestValidationHandler(sampleItemParameter.Name));
                        }

                    }
                };

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a POST request with valid input
                HttpClient client = new HttpClient();
                var input = new ObjectContent<SampleItem>(
                    new SampleItem
                    {
                        Id = 4, 
                        StringValue = "four"
                    }, "text/xml");
                var response = client.PostAsync(baseAddress, input).Result;
                Console.WriteLine("\nresponse is {0}\ncontent is {1}", response, response.Content);

                // The request should be successful
                response.EnsureSuccessStatusCode();

                // Create a POST request with invalid input
                input = new ObjectContent<SampleItem>(
                    new SampleItem
                    {
                        Id = -1,
                        StringValue = "invalid"
                    }, "text/xml");
                response = client.PostAsync(baseAddress, input).Result;
                Console.WriteLine("\nresponse is {0}\ncontent is {1}", response, response.Content);

                // The request should fail
                if (response.StatusCode != HttpStatusCode.BadRequest)
                {
                    throw new ApplicationException("The request has invalid input.");
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
