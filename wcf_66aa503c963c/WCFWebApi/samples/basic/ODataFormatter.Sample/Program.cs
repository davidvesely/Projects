// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ODataFormatter.Sample
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;

    public static class Program
    {
        /// <summary>
        /// Demonstrates how to get application/atom+xml from the OData formatter.
        /// </summary>
        /// <returns>The response as a string.</returns>
        public static string GetAtomXml()
        {
            Console.WriteLine("\n\nOData response for application/atom+xml is:");
            string result = null;

            // Register a new OData formatter to run before the default formatters
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Insert(0, new ODataMediaTypeFormatter());

            using (HttpServiceHost host = Util.CreateServiceHost(configuration))
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = Util.BaseAddressUri;
                HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(Util.BaseAddress + "GetPerson"));
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));
                requestMessage.Headers.Add("DataServiceVersion", "2.0");
                requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Demonstrates how to get application/json from the OData formatter.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationJson()
        {
            Console.WriteLine("\n\nOData response for application/json is:");
            string result = null;

            // Register a new OData formatter to run before the default formatters
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Insert(0, new ODataMediaTypeFormatter());

            using (HttpServiceHost host = Util.CreateServiceHost(configuration))
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = Util.BaseAddressUri;
                HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(Util.BaseAddress + "GetPerson"));
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Util.ApplicationJsonMediaType.MediaType));
                requestMessage.Headers.Add("DataServiceVersion", "2.0");
                requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Demonstrates how to make the <see cref="ODataMediaTypeFormatter"/> support only application/atom+xml,
        /// allowing the normal <see cref="JsonMediaTypeFormatter"/> to respond to application/json.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationJsonFromJsonFormatter()
        {
            Console.WriteLine("\n\nResponse to application/json when OData formatter is present but does not support that media type is:");
            string result = null;

            // Create a new OData formatter, but remove support for application/json
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter();
            odataFormatter.SupportedMediaTypes.Remove(Util.ApplicationJsonMediaType);

            // Register a new OData formatter to run before the default formatters
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Insert(0, odataFormatter);

            using (HttpServiceHost host = Util.CreateServiceHost(configuration))
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = Util.BaseAddressUri;

                HttpRequestMessage messageWithJsonHeader = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(Util.BaseAddress + "GetPerson"));
                messageWithJsonHeader.Headers.Accept.Add(Util.ApplicationJsonMediaTypeWithQuality);
                messageWithJsonHeader.Headers.Add("DataServiceVersion", "2.0");
                messageWithJsonHeader.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Demonstrates how ODataMediaTypeFormatter would conditionally support application/atom+xml and application/json only if format=odata is present in the QueryString
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationJsonWithFormat()
        {
            Console.WriteLine("\n\nOData response to application/json when 'format=odata' is in the Uri is:");
            string result = null;

            // Create a new OData formatter
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter();

            // Tell the OData formatter not to respond to its normal media types
            odataFormatter.SupportedMediaTypes.Clear();

            // And instead provide a MediaTypeMapping that conditionally responds based on the format specified in the Uri
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationAtomMediaTypeWithQuality));
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationJsonMediaTypeWithQuality));

            // Register a new OData formatter to run before the default formatters
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Insert(0, odataFormatter);

            using (HttpServiceHost host = Util.CreateServiceHost(configuration))
            {
                HttpEndpoint endpoint = host.Description.Endpoints.OfType<HttpEndpoint>().Single();
            
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = Util.BaseAddressUri;

                // Placing "?format=odata" in the Uri selects the OData formatter
                HttpRequestMessage messageWithJsonHeader = Util.GenerateRequestMessage(false, new Uri(Util.BaseAddress + "GetPerson?format=odata"));
                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Demonstrates how ODataMediaTypeFormatter would conditionally support application/atom+xml and application/json only if format=odata is present in the QueryString
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationJsonWithoutFormat()
        {
            Console.WriteLine("\n\nOData response to application/json when format=odata is *not* in the Uri is:");
            string result = null;

            // Create a new OData formatter
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter();

            // Tell the OData formatter not to respond to its normal media types
            odataFormatter.SupportedMediaTypes.Clear();

            // And instead provide a MediaTypeMapping that conditionally responds based on the format specified in the Uri
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationAtomMediaTypeWithQuality));
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationJsonMediaTypeWithQuality));

            // Register a new OData formatter to run before the default formatters
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Insert(0, odataFormatter);

            using (HttpServiceHost host = Util.CreateServiceHost(configuration))
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = Util.BaseAddressUri;

                // When "?format=odata" is not in the Uri, the OData formatter declines, and the normal Json formatter is used
                HttpRequestMessage messageWithJsonHeader = Util.GenerateRequestMessage(false, new Uri(Util.BaseAddress + "GetPerson"));

                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return result;
        }

        public static void Main(string[] args)
        {
            try
            {
                GetAtomXml();

                GetApplicationJson();

                GetApplicationJsonFromJsonFormatter();

                GetApplicationJsonWithFormat();

                GetApplicationJsonWithoutFormat();

                Console.WriteLine("\n\nHit ENTER to exit\n");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
