// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Microsoft.ApplicationServer.Http;

    public static class SetSerializerOnXmlFormatter
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // use DataContractSerializer for SampleItem type
                config.Formatters.XmlFormatter.SetSerializer<SampleItem>(new DataContractSerializer(typeof(SampleItem)));

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: {0}", baseAddress);

                // Create a GET request to get an item by id
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/item/1").Result;
                response.EnsureSuccessStatusCode();
                string responseStr = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Printing response:\r\n{0}", responseStr);

                // If DataContractSerializer was used to serialize SampleItem on the server, the response should contain the data contract name DataContractSampleItem.
                if (!responseStr.Contains("DataContractSampleItem"))
                {
                    throw new ApplicationException("XmlMediaTypeFormatter did not use DataContractSerializer to serialize SampleItem on the server.");
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
