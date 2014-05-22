// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.ApplicationServer.Http;

    public static class SetInstanceProviderMultipleHost
    {
        /// <summary>
        /// This type is provided to the container when resolving List<Customer>
        /// </summary>
        [Export(typeof(List<Customer>))]
        public class DefaultCustomerList : List<Customer>
        {
            public DefaultCustomerList()
            {
                // Adding 20 customers to the default list
                for (int i = 0; i < 20; i++)
                {
                    this.Add(new Customer { Id = i,  Name = "Existing customer " + i });
                }
            }
        }

        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            var container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            HttpServiceHost sampleServiceHost = null;
            HttpServiceHost customerServiceHost = null;
            try
            {
                var config = new HttpConfiguration
                {
                    // Use MEF to provide instances
                    CreateInstance = (serviceType, context, message) =>
                    {
                        // Return an instance of SampleService or CustomerService depending on the 'serviceType' - constructed using the constructor marked with [ImportingConstructor].
                        // The constructor parameter should resolve to an instance of DefaultSampleItems or DefaultCustomerList depending on the 'serviceType'.
                        return container.GetExportedValue<object>(AttributedModelServices.GetContractName(serviceType));
                    }
                };

                string sampleServiceAddress = new UriBuilder(baseAddress) { Path = "sample" }.ToString();
                sampleServiceHost = new HttpServiceHost(typeof(SampleService), config, sampleServiceAddress);
                sampleServiceHost.Open();
                Console.WriteLine("Sample Service listening at: " + sampleServiceAddress);

                string customerServiceAddress = new UriBuilder(baseAddress) { Path = "customer" }.ToString();
                customerServiceHost = new HttpServiceHost(typeof(CustomerService), config, customerServiceAddress);
                customerServiceHost.Open();
                Console.WriteLine("Customer Service listening at: " + customerServiceAddress);

                // Create a GET request to verify that the service is initialized with an instance of DefaultSampleItems which contain 10 items
                HttpClient client = new HttpClient();
                var response = client.GetAsync(sampleServiceAddress).Result;
                response.EnsureSuccessStatusCode();
                SampleItems items = response.Content.ReadAsAsync<SampleItems>().Result;
                if (items.Count != 10)
                {
                    throw new ApplicationException("The service instance was not initialized with DefaultSampleItems.");
                }

                // Printing out the items
                Console.WriteLine(items);

                // Create a GET request to verify that the service is initialized with an instance of DefaultCustomerList which contain 20 customers
                client = new HttpClient();
                response = client.GetAsync(customerServiceAddress).Result;
                response.EnsureSuccessStatusCode();
                List<Customer> customers = response.Content.ReadAsAsync<List<Customer>>().Result;
                if (customers.Count != 20)
                {
                    throw new ApplicationException("The service instance was not initialized with DefaultCustomerList.");
                }

                // Printing out each customer
                foreach (var customer in customers)
                {
                    Console.WriteLine("{0},{1}", customer.Id, customer.Name);
                }
            }
            finally
            {
                if (sampleServiceHost != null)
                {
                    sampleServiceHost.Close();
                }
            }  
        }
    }
}
