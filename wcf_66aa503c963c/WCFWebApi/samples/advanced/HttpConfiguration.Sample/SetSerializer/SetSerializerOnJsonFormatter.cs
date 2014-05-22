// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using Microsoft.ApplicationServer.Http;

    public static class SetSerializerOnJsonFormatter
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // use DataContractJsonSerializer with the surrogate for SampleItem type
                var surrogate = new DataContractJsonSurrogate();
                var jsonSerializer = new DataContractJsonSerializer(typeof(SampleItem), null, int.MaxValue, false, surrogate, false);
                config.Formatters.JsonFormatter.SetSerializer<SampleItem>(jsonSerializer);

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: {0}", baseAddress);

                // Create a GET request to get an item by id with accept: text/json
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseAddress + "/item/1");
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/json"));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                string responseStr = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Printing response:\r\n{0}", responseStr);

                // If the provided instance of DataContractJsonSerializer was used to serialize SampleItem on the server, the surrogate should've been called.
                if (!surrogate.WasCalled)
                {
                    throw new ApplicationException("JsonMediaTypeFormatter did not use the provided instance of DataContractJsonSerializer to serialize SampleItem on the server.");
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
