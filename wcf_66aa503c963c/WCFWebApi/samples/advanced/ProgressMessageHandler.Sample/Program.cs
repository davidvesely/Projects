// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http;

    /// <summary>
    /// This sample illustrates the <see cref="ProgressMessageHandler"/> which introduces progress notifications
    /// for upload and download of data.
    /// </summary>
    public class Program
    {
        private static string fileServiceAddress = "http://localhost:8081/fileservice";
        private static HttpServiceHost fileServiceHost = null;

        /// <summary>
        /// Handles the <see cref="M:ProgressMessageHandler.HttpUploadProgress"/> event of the <see cref="ProgressMessageHandler"/> 
        /// HTTP message handler.
        /// </summary>
        /// <param name="sender">The <see cref="HttpRequestMessage"/> source of the event.</param>
        /// <param name="e">The <see cref="HttpProgressEventArgs"/> instance containing the event data.</param>
        private static void progressHandler_HttpUploadProgress(object sender, HttpProgressEventArgs e)
        {
            Console.WriteLine("Uploaded: {0}% ({1} of {2} bytes)", e.ProgressPercentage, e.BytesExchanged, e.TotalBytesExchanged);
        }

        /// <summary>
        /// Handles the <see cref="M:ProgressMessageHandler.HttpDownloadProgress"/> event of the <see cref="ProgressMessageHandler"/> 
        /// HTTP message handler.
        /// </summary>
        /// <param name="sender">The <see cref="HttpRequestMessage"/> source of the event.</param>
        /// <param name="e">The <see cref="HttpProgressEventArgs"/> instance containing the event data.</param>
        private static void progressHandler_HttpDownloadProgress(object sender, HttpProgressEventArgs e)
        {
            Console.WriteLine("Downloaded: {0}% ({1} of {2} bytes)", e.ProgressPercentage, e.BytesExchanged, e.TotalBytesExchanged);
        }

        /// <summary>
        /// This method illustrates using the <see cref="ProgressMessageHandler"/> for an HTML File Upload request to 
        /// show how progress notifications are generated both for upload and download of data.
        /// </summary>
        private static void UploadData(HttpClient client)
        {
            Console.WriteLine("Running sample showing uploading data to service...");

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
                // Progress notifications will be sent to the event handlers on every read and write
                // to the network reporting the number of bytes read or written respectively.
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

            Console.WriteLine();
        }

        /// <summary>
        /// This method illustrates using the <see cref="ProgressMessageHandler"/> for an HTTP GET Request to 
        /// show how progress notifications are generated for download of data.
        /// </summary>
        private static void DownloadData(HttpClient client)
        {
            Console.WriteLine("Running sample showing downloading data from service...");

            // Submit GET request to download the file.
            // Progress notifications will be sent to the event handlers on every read and write
            // to the network reporting the number of bytes read or written respectively.
            using (HttpResponseMessage response = client.GetAsync(fileServiceAddress + "?file=SampleData.random").Result)
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine("File downloaded with status code {0}", response.StatusCode);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// This opens the sample service which we communicate with throughout this sample.
        /// </summary>
        private static void OpenHost()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.TransferMode = TransferMode.StreamedRequest;

            // We set the max receive size to 10G but note that many browsers have trouble getting the content-length
            // right for files over 2G. As a result it is safer to chop up files bigger than 2G in smaller pieces and
            // assemble them once received.
            // NOTE: If you are hosted in ASP then you also must set the maxRequestLength property in the web.config file
            // so that it matches the MaxReceivedMessageSize value.
            config.MaxReceivedMessageSize = 10L * 1024 * 1024 * 1024;

            fileServiceHost = new HttpServiceHost(typeof(SampleService), config, fileServiceAddress);
            fileServiceHost.Open();
            Console.WriteLine("Service '{0}' listening on '{1}'", typeof(SampleService).Name, fileServiceAddress);
        }

        /// <summary>
        /// This closes the sample service which we communicate with throughout this sample.
        /// </summary>
        private static void CloseHost()
        {
            if (fileServiceHost != null)
            {
                try
                {
                    fileServiceHost.Close();
                }
                catch
                {
                    fileServiceHost.Abort();
                }
                fileServiceHost = null;
            }
        }

        /// <summary>
        /// This sample illustrates the <see cref="ProgressMessageHandler"/> which introduces progress notifications
        /// for upload and download of data.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                // Configure progress notification
                ProgressMessageHandler progressHandler = new ProgressMessageHandler(new WebRequestHandler());
                progressHandler.HttpDownloadProgress += new HttpProgressEventHandler(progressHandler_HttpDownloadProgress);
                progressHandler.HttpUploadProgress += new HttpProgressEventHandler(progressHandler_HttpUploadProgress);

                // Create HTTP client for submitting requests
                HttpClient client = new HttpClient(progressHandler);
                
                // Open our sample service host
                OpenHost();

                // Upload data using our HttpClient
                UploadData(client);

                // Download data using our HttpClient
                DownloadData(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            finally
            {
                // Close our sample service host
                CloseHost();
            }
        }
    }
}