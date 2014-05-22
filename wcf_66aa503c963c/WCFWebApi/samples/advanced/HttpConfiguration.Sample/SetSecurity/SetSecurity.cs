// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http;

    public static class SetSecurity
    {
        public static void Run()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";
            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration
                {
                    // Configuring security
                    Security = (address, security) =>
                        {
                            security.Mode = HttpBindingSecurityMode.TransportCredentialOnly;
                            security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                        }
                };

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress).Result;

                // The request should fail with Http status 401 (Unauthorized) because of the security configuration
                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw new ApplicationException("The request should fail with Http status 401 (Unauthorized) because of the security configuration");
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
