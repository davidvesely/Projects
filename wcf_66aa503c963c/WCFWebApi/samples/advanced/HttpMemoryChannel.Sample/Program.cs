// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpMemoryChannel.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Description;

    /// <summary>
    /// This sample illustrates use of the <see cref="HttpMemoryBinding"/> class.
    /// </summary>
    public class Program
    {
        private static TimeSpan timeout = TimeSpan.FromSeconds(20);
        private static string sampleServiceAddress = "http://localhost:8080/sample";
        private static string memorySampleServiceAddress = "http://memoryhost/sample";

        /// <summary>
        /// Creates a <see cref="HttpServiceHost"/>, adds an <see cref="HttpMemoryEndpoint"/>, and then
        /// submits an <see cref="HttpRequest"/> which is processed by the <see cref="SampleService"/>.
        /// </summary>
        private static void RunHostWithAddHttpMemoryEndpoint()
        {
            HttpServiceHost serviceHost = new HttpServiceHost(typeof(SampleService), sampleServiceAddress);
            try
            {
                // Add default endpoint
                IEnumerable<ServiceEndpoint> defaultEndpoint = serviceHost.AddDefaultEndpoints();

                // Create memory endpoint for the same service 
                HttpMemoryConfiguration memoryConfig = new HttpMemoryConfiguration();
                memoryConfig.MessageHandlers.Add(typeof(SampleMessageHandler));

                // Add memory endpoint to host
                HttpMemoryEndpoint memoryEndpoint = serviceHost.AddHttpMemoryEndpoint(typeof(SampleService), memorySampleServiceAddress, memoryConfig);

                // Open host
                serviceHost.Open();

                // Get HttpMemoryHandler from HttpMemoryEndpoint
                HttpMemoryHandler memoryHandler = memoryEndpoint.GetHttpMemoryHandler();

                // Create HttpClient around handler
                HttpClient client = new HttpClient(memoryHandler);

                // Submit request and wait for response
                Task<HttpResponseMessage> getTask = client.GetAsync(memorySampleServiceAddress + "/contacts");
                getTask.Wait(timeout);
                HttpResponseMessage response = getTask.Result;
                response.EnsureSuccessStatusCode();

                // Note that we receive an ObjectContent and that a header field 
                // was inserted by the sample message handler.
                Console.WriteLine(response);

                List<Contact> contacts = response.Content.ReadAsAsync<List<Contact>>().Result;
                Console.WriteLine("Received {0} contacts from service", contacts.Count);
            }
            finally
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="HttpServiceHost"/>, adds an <see cref="HttpMemoryEndpoint"/>, and then
        /// submits an <see cref="HttpRequest"/> which is processed by the <see cref="SampleService"/>.
        /// </summary>
        private static void RunHostWithHttpMemoryEndpoint()
        {
            HttpServiceHost serviceHost = new HttpServiceHost(typeof(SampleService), sampleServiceAddress);
            try
            {
                // Add default endpoint
                IEnumerable<ServiceEndpoint> defaultEndpoint = serviceHost.AddDefaultEndpoints();

                // Create memory endpoint for the same service 
                ContractDescription contract = defaultEndpoint.ElementAt(0).Contract;
                HttpMemoryEndpoint memoryEndpoint = new HttpMemoryEndpoint(contract,
                    new EndpointAddress(memorySampleServiceAddress));

                // Add a sample message handler to the memory endpoint.
                memoryEndpoint.MessageHandlerFactory = new HttpMessageHandlerFactory(typeof(SampleMessageHandler));

                // Add memory endpoint to host
                serviceHost.AddServiceEndpoint(memoryEndpoint);

                // Open host
                serviceHost.Open();

                // Get HttpMemoryHandler from HttpMemoryEndpoint
                HttpMemoryHandler memoryHandler = memoryEndpoint.GetHttpMemoryHandler();

                // Create HttpClient around handler
                HttpClient client = new HttpClient(memoryHandler);

                // Submit request and wait for response
                Task<HttpResponseMessage> getTask = client.GetAsync(memorySampleServiceAddress + "/contacts");
                getTask.Wait(timeout);
                HttpResponseMessage response = getTask.Result;
                response.EnsureSuccessStatusCode();

                // Note that we receive an ObjectContent and that a header field 
                // was inserted by the sample message handler.
                Console.WriteLine(response);

                List<Contact> contacts = response.Content.ReadAsAsync<List<Contact>>().Result;
                Console.WriteLine("Received {0} contacts from service", contacts.Count);
            }
            finally
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="HttpServiceHost"/>, adds an <see cref="HttpMemoryBinding"/>, and then
        /// submits an <see cref="HttpRequest"/> which is processed by the <see cref="SampleService"/>.
        /// </summary>
        private static void RunHostWithHttpMemoryBinding()
        {
            HttpServiceHost serviceHost = new HttpServiceHost(typeof(SampleService), sampleServiceAddress);
            try
            {
                // In this scenario we do NOT add a default endpoint meaning that this service ONLY listens
                // on the memory channel

                // Add new endpoint using HttpMemoryBinding
                HttpMemoryBinding memoryBinding = new HttpMemoryBinding();
                serviceHost.AddServiceEndpoint(typeof(SampleService), memoryBinding, memorySampleServiceAddress);

                // Add a sample message handler to the memory binding.
                memoryBinding.MessageHandlerFactory = new HttpMessageHandlerFactory(typeof(SampleMessageHandler));

                // Open host
                serviceHost.Open();

                // Get HttpMemoryHandler from HttpMemoryBinding
                HttpMemoryHandler memoryHandler = memoryBinding.GetHttpMemoryHandler();

                // Create HttpClient around handler
                HttpClient client = new HttpClient(memoryHandler);

                // Submit request and wait for response
                Task<HttpResponseMessage> getTask = client.GetAsync(memorySampleServiceAddress + "/contacts");
                getTask.Wait(timeout);
                HttpResponseMessage response = getTask.Result;
                response.EnsureSuccessStatusCode();

                // Note that we receive an ObjectContent and that a header field 
                // was inserted by the sample message handler.
                Console.WriteLine(response);

                List<Contact> contacts = response.Content.ReadAsAsync<List<Contact>>().Result;
                Console.WriteLine("Received {0} contacts from service", contacts.Count);
            }
            finally
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
            }
        }

        public static void Main()
        {
            Program.RunHostWithAddHttpMemoryEndpoint();

            Program.RunHostWithHttpMemoryEndpoint();

            Program.RunHostWithHttpMemoryBinding();
        }
    }
}
