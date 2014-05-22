// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MimeMultipart.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;

    /// <summary>
    /// This sample illustrates several scenarios around MIME multipart file upload from browsers. To run the sample,
    /// copy the contents of the Web folder to a place where it can be served by IIS, for example c:\inetpub\wwwroot\multipart\*
    /// and access the default.html page, for example http://localhost/multipart/default.html in your browser of choice.
    /// </summary>
    public class Program
    {
        private static string typeServiceAddress = "http://localhost:8080/typeservice";
        private static string fileServiceAddress = "http://localhost:8081/fileservice";
        private static string dotNetZipServiceAddress = "http://localhost:8082/dotnetzipservice";
        private static string splitfileServiceAddress = "http://localhost:8083/splitfileservice";
        private static string multipartGeneratorServiceAddress = "http://localhost:8084/multipartgenerator";

        private static HttpServiceHost typeServiceHost = null;
        private static HttpServiceHost fileServiceHost = null;
        private static HttpServiceHost dotnetzipServiceHost = null;
        private static HttpServiceHost splitfileServiceHost = null;
        private static HttpServiceHost multipartGeneratorServiceHost = null;

        private static void LogOpenHost<TService>(string address)
        {
            Console.WriteLine("Service '{0}' listening on '{1}'", typeof(TService).Name, address);
        }

        public static void OpenHosts()
        {
            HttpConfiguration config = new HttpConfiguration();

            // Set transfer mode to streamed to avoid keeping the whole thing in memory
            config.TransferMode = TransferMode.Streamed;

            // We set the max receive size to 1M. Note that many browsers have trouble getting the content-length
            // right for files over 2G. As a result it is safer to chop up files bigger than 2G in smaller pieces and
            // assemble them once received.
            // NOTE: If you are hosted in ASP then you also must set the maxRequestLength property in the web.config file
            // so that it matches the MaxReceivedMessageSize value.
            config.MaxReceivedMessageSize = 1L * 1024 * 1024;

            // NOTE: For very big uploads you may have to set ReceiveTimeout on the binding. The default value
            // is 10 minutes which may not be sufficient for large uploads.

            typeServiceHost = new HttpServiceHost(typeof(TypeService), config, typeServiceAddress);
            typeServiceHost.Open();
            LogOpenHost<TypeService>(typeServiceAddress);

            fileServiceHost = new HttpServiceHost(typeof(FileService), config, fileServiceAddress);
            fileServiceHost.Open();
            LogOpenHost<FileService>(fileServiceAddress);

            dotnetzipServiceHost = new HttpServiceHost(typeof(DotNetZipService), config, dotNetZipServiceAddress);
            dotnetzipServiceHost.Open();
            LogOpenHost<DotNetZipService>(dotNetZipServiceAddress);

            splitfileServiceHost = new HttpServiceHost(typeof(SplitFileService), config, splitfileServiceAddress);
            splitfileServiceHost.Open();
            LogOpenHost<SplitFileService>(splitfileServiceAddress);

            multipartGeneratorServiceHost = new HttpServiceHost(typeof(MultipartGeneratorService), config, multipartGeneratorServiceAddress);
            multipartGeneratorServiceHost.Open();
            LogOpenHost<MultipartGeneratorService>(multipartGeneratorServiceAddress);
        }

        private static void Close(ServiceHost host)
        {
            try
            {
                host.Close(TimeSpan.FromSeconds(5));
            }
            catch
            {
                host.Abort();
            }
        }

        public static void CloseHosts()
        {
            if (typeServiceHost != null)
            {
                Close(typeServiceHost);
                typeServiceHost = null;
            }

            if (fileServiceHost != null)
            {
                Close(fileServiceHost);
                fileServiceHost = null;
            }

            if (dotnetzipServiceHost != null)
            {
                Close(dotnetzipServiceHost);
                dotnetzipServiceHost = null;
            }

            if (splitfileServiceHost != null)
            {
                Close(splitfileServiceHost);
                splitfileServiceHost = null;
            }

            if (multipartGeneratorServiceHost != null)
            {
                Close(multipartGeneratorServiceHost);
                multipartGeneratorServiceHost = null;
            }
        }

        public static void ReadMultipartMemory()
        {
            // Create HTTP client for submitting requests
            HttpClient client = new HttpClient();

            // Submit an HTTP GET request 
            using (HttpResponseMessage response = client.GetAsync(multipartGeneratorServiceAddress).Result)
            {
                if (response.Content.IsMimeMultipartContent())
                {
                    // Read response as MIME multipart
                    IEnumerable<HttpContent> parts = response.Content.ReadAsMultipart();
                    foreach (var content in parts)
                    {
                        Console.WriteLine("Received MIME multipart body part: {0}",
                            content.ReadAsStringAsync().Result);
                    }
                }
                else
                {
                    throw new Exception("Response was not MIME multipart");
                }
            }
        }

        public static void ReadMultipartFileAsync()
        {
            // Create HTTP client for submitting requests
            HttpClient client = new HttpClient();

            // Submit an asynchronous HTTP GET request 
            Task task = client.GetAsync(multipartGeneratorServiceAddress).ContinueWith(
                (responseTask) => 
                {
                    // Check that the response is MIME multipart
                    HttpResponseMessage response = responseTask.Result;
                    if (response.Content.IsMimeMultipartContent())
                    {
                        // Read the response asynchronously. We use FileStreamProvider 
                        // for writing each part asynchronously to local disk.
                        MultipartFileStreamProvider fileStreamProvider = new MultipartFileStreamProvider();
                        response.Content.ReadAsMultipartAsync(fileStreamProvider).ContinueWith(
                            contentTask =>
                            {
                                IEnumerable<HttpContent> result = contentTask.Result;
                                Console.WriteLine("Received {0} parts", result.Count());
                                foreach (var filename in fileStreamProvider.BodyPartFileNames)
                                {
                                    Console.WriteLine("Wrote file '{0}'", filename);
                                }
                            });
                    }
                    else
                    {
                        throw new Exception("Response was not MIME multipart: " + response.ToString());
                    }
                });

            task.Wait();
        }

        public static void SubmitMultipartFormData()
        {
            // Create HTTP client for submitting requests
            HttpClient client = new HttpClient();

            // Create MIME Multipart content using HTML File Upload containing a local file
            MultipartFormDataContent formdata = new MultipartFormDataContent();

            // Add submitter field as content
            formdata.Add(new StringContent("Name of Submitter"), "submitter");

            // Add file as content. Note that there will be a HttpContent that
            // encapsulates a file so the File.* code below will go away.
            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead("Web\\SampleData.random");
                HttpContent fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formdata.Add(fileContent, "data", "SampleData.random");

                // Submit an HTTP POST request with the MIME multipart.
                using (HttpResponseMessage response = client.PostAsync(fileServiceAddress, formdata).Result)
                {
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("File uploaded with status code {0}", response.StatusCode);
                    List<FileResult> result = response.Content.ReadAsAsync<List<FileResult>>().Result;
                    Console.WriteLine("File was saved as '{0}' by '{1}'", result[0].LocalPath, result[0].Submitter);
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                OpenHosts();

                ReadMultipartMemory();

                ReadMultipartFileAsync();

                SubmitMultipartFormData();

                Console.WriteLine("\n\nHit ENTER to exit\n");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            finally
            {
                CloseHosts();
            }
        }
    }
}