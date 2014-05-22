// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskDispatcherBehavior.Sample
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http;

    public class Program
    {
        public static string BackendServiceAddress = "http://localhost:8081/backend";
        public static string AggregatorServiceAddress = "http://localhost:8082/aggregator";

        public static void Main()
        {
            HttpServiceHost backendHost = null;
            HttpServiceHost aggregatorHost = null;

            try
            {
                backendHost = new HttpServiceHost(typeof(BackendService), BackendServiceAddress);
                backendHost.Open();

                aggregatorHost = new HttpServiceHost(typeof(AggregatorService), AggregatorServiceAddress);
                aggregatorHost.Open();

                HttpClient client = new HttpClient();
                Task<HttpResponseMessage>[] requestTasks = new Task<HttpResponseMessage>[] 
                {
                    client.GetAsync(AggregatorServiceAddress),
                    client.GetAsync(AggregatorServiceAddress + "/contacts"),
                    client.GetAsync(AggregatorServiceAddress + "/empty")
                };

                Task t = Task.Factory.ContinueWhenAll(requestTasks,
                    (Task<HttpResponseMessage>[] result) =>
                    {
                        foreach (Task<HttpResponseMessage> task in result)
                        {
                            HttpResponseMessage response = task.Result;
                            Console.WriteLine("Received response from frontend server: {0}", response);
                        }
                    });

                t.Wait();
            }
            finally
            {
                if (backendHost != null)
                {
                    backendHost.Close();
                }

                if (aggregatorHost != null)
                {
                    aggregatorHost.Close();
                }
            }
        }
    }
}