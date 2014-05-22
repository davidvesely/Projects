// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.ApplicationServer.Http;

    public static class SetInstanceProvider
    {
        /// <summary>
        /// This type is provided to the container when resolving SampleItems
        /// </summary>
        [Export(typeof(SampleItems))]
        public class DefaultSampleItems : SampleItems
        {
            public DefaultSampleItems()
            {
                // Adding 10 default items
                for (int i = 0; i < 10; i++)
                {
                    this.Add(new SampleItem { Id = i, StringValue = "My default item " + i });
                }
            }
        }

        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            var container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration
                {
                    // Use MEF to provide instances
                    CreateInstance = (type, context, message) =>
                    {
                        // Return an instance of SampleService - constructed using the constructor marked with [ImportingConstructor].
                        // The constructor parameter should resolve to an instance of DefaultSampleItems
                        return container.GetExportedValue<object>(AttributedModelServices.GetContractName(type));
                    }
                };

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request to verify that the service is initialized with an instance of DefaultSampleItems which contain 10 items
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress).Result;
                response.EnsureSuccessStatusCode();
                SampleItems items = response.Content.ReadAsAsync<SampleItems>().Result;
                if (items.Count != 10)
                {
                    throw new ApplicationException("The service instance was not initialized with DefaultSampleItems.");
                }

                // Printing out the items
                Console.WriteLine(items);
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
