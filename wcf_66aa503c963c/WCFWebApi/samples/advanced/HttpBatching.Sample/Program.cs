// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpBatching.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;

    /// <summary>
    /// This sample illustrates use of batching
    /// </summary>
    public class Program
    {
        private static TimeSpan timeout = TimeSpan.FromSeconds(30);
        private static string sampleServiceAddress = "http://localhost:8080/sample";
        private static string memorySampleServiceAddress = "http://memoryhost/sample";

        private static ServiceHost OpenServiceHost()
        {
            // Add our MIME batching handler plus a sample message handler operating on the outer request
            BatchingMessageHandler batchingMessageHandler = new BatchingMessageHandler();
            HttpConfiguration config = new HttpConfiguration
            {
                MessageHandlerFactory = () =>
                {
                    return new DelegatingHandler[]
                    {
                        batchingMessageHandler,
                        new SampleMessageHandler()
                    };
                }
            };

            HttpServiceHost serviceHost = new HttpServiceHost(typeof(SampleService), config, sampleServiceAddress);

            // Add default endpoint
            IEnumerable<ServiceEndpoint> defaultEndpoint = serviceHost.AddDefaultEndpoints();

            // Create memory binding endpoint
            HttpMemoryConfiguration memoryConfiguration = new HttpMemoryConfiguration();
            memoryConfiguration.MessageHandlers.Add(typeof(SampleMemoryMessageHandler));
            HttpMemoryEndpoint memoryEndpoint = serviceHost.AddHttpMemoryEndpoint(typeof(SampleService), memorySampleServiceAddress, memoryConfiguration);

            // Open host
            serviceHost.Open();

            // Wire up BatchingMessageHandler with the HttpMemoryBinding
            batchingMessageHandler.Initialize(memoryEndpoint);

            return serviceHost;
        }

        private static HttpRequestMessage CreateBatchedRequest()
        {
            MultipartContent multipart = new MultipartContent("batching");

            // Create a sample GET request for contacts
            HttpRequestMessage httpRequestContacts = new HttpRequestMessage(HttpMethod.Get, sampleServiceAddress + "/contacts");
            HttpMessageContent httpRequestContactsContent = new HttpMessageContent(httpRequestContacts);
            multipart.Add(httpRequestContactsContent);

            // Create a sample GET request for orders
            HttpRequestMessage httpRequestOrders = new HttpRequestMessage(HttpMethod.Get, sampleServiceAddress + "/orders");
            HttpMessageContent httpRequestOrdersContent = new HttpMessageContent(httpRequestOrders);
            multipart.Add(httpRequestOrdersContent);

            // Create a sample POST request for adding a contact
            HttpRequestMessage httpRequestAddContact = new HttpRequestMessage(HttpMethod.Post, sampleServiceAddress + "/contacts");
            httpRequestAddContact.Content = new ObjectContent<Contact>(new Contact { Age = 10, Avatar = "http://example.com/10", Id = 10, Name = "10" });
            HttpMessageContent httpRequestAddContactContent = new HttpMessageContent(httpRequestAddContact);
            multipart.Add(httpRequestAddContactContent);

            // Create a sample POST request for adding an order
            HttpRequestMessage httpRequestAddOrder = new HttpRequestMessage(HttpMethod.Post, sampleServiceAddress + "/orders");
            httpRequestAddOrder.Content = new ObjectContent<Order>(new Order { Category = "10", Product = "10", Quantity = 10 });
            HttpMessageContent httpRequestAddOrdersContent = new HttpMessageContent(httpRequestAddOrder);
            multipart.Add(httpRequestAddOrdersContent);

            // Create the HTTP request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, sampleServiceAddress + BatchingMessageHandler.BatchingUriPostfix);
            request.Content = multipart;

            Console.WriteLine("Created batched request with {0} inner requests: ", multipart.Count());
            foreach (var bodypart in multipart)
            {
                HttpMessageContent content = bodypart as HttpMessageContent;
                if (content != null)
                {
                    Console.WriteLine("\n{0}\n", content.HttpRequestMessage.ToString());
                }
            }

            return request;
        }

        private static void ReadBatchedResponse(HttpResponseMessage response)
        {
            Console.WriteLine("Received batched response {0}", response);
            if (response.Content.IsMimeMultipartContent())
            {
                foreach (HttpContent bodypart in response.Content.ReadAsMultipart())
                {
                    if (bodypart.IsHttpResponseMessageContent())
                    {
                        HttpResponseMessage batchedResponse = bodypart.ReadAsHttpResponseMessage();
                        batchedResponse.EnsureSuccessStatusCode();
                        Console.WriteLine("\n{0}\n", batchedResponse.ToString());
                    }
                    else
                    {
                        throw new Exception("Unexpected MIME body part -- expected a batched HTTP Response Message.");
                    }
                }
            }
            else
            {
                throw new Exception("Unexpected HTTP response -- expected a MIME Multipart batching response.");
            }
        }

        public static void Main()
        {
            ServiceHost host = null;
            try
            {
                host = OpenServiceHost();
                HttpClient client = new HttpClient();

                List<Task<HttpResponseMessage>> requestTasks = new List<Task<HttpResponseMessage>>();
                HttpRequestMessage request = CreateBatchedRequest();
                requestTasks.Add(client.SendAsync(request));

                Task.WaitAll(requestTasks.ToArray(), timeout);
                foreach (Task<HttpResponseMessage> task in requestTasks)
                {
                    ReadBatchedResponse(task.Result);
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
