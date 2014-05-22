// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;

    public static class SetTrailingSlashMode
    {
        public static void Run()
        {
            RunWithIgnore();
            RunWithAutoRedirect();
        }

        public static void RunWithIgnore()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";

            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Ignore trailing slash
                config.TrailingSlashMode = TrailingSlashMode.Ignore;

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request without trailing slash
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/item/1").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("The response should return Http status 200 (OK) because the TrailingSlashMode is set to Ignore");
                }

                // Create a GET request with trailing slash
                client = new HttpClient();
                response = client.GetAsync(baseAddress + "/item/1/").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("The response should return Http status 200 (OK) because the TrailingSlashMode is set to Ignore");
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

        public static void RunWithAutoRedirect()
        {
            string baseAddress = "http://localhost:8080/HttpConfigurationSample";

            HttpServiceHost host = null;
            try
            {
                var config = new HttpConfiguration();

                // Set to AutoRedirect
                config.TrailingSlashMode = TrailingSlashMode.AutoRedirect;

                host = new HttpServiceHost(typeof(SampleService), config, baseAddress);
                host.Open();
                Console.WriteLine("Service listening at: " + baseAddress);

                // Create a GET request without trailing slash
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/item/1").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("The response should return Http status 200 (OK) because the TrailingSlashMode is set to AutoRedirect");
                }

                // Create a GET request with trailing slash
                client = new HttpClient();
                response = client.GetAsync(baseAddress + "/item/1/").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("The response should return Http status 200 (OK) because the TrailingSlashMode is set to AutoRedirect");
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
