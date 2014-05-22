// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using System;

    /// <summary>
    /// This sample WCF service provides up- and download data for demonstrating the <see cref="ProgressMessageHandler"/>.
    /// </summary>
    internal class SampleService
    {
        private static readonly string rootPath = Path.Combine(Path.GetTempPath(), "ProgressMessageHandlerSample");

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public List<FileResult> UploadFile(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Create a stream provider for setting up output streams
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(rootPath);

            // Read the MIME multipart content using the stream provider we just created.
            IEnumerable<HttpContent> bodyparts = request.Content.ReadAsMultipart(streamProvider);

            // The submitter field is the entity with a Content-Disposition header field with a "name" parameter with value "submitter"
            string submitter;
            if (!bodyparts.TryGetFormFieldValue("submitter", out submitter))
            {
                submitter = "unknown";
            }

            // Get list of local file names from stream provider
            IDictionary<string, string> bodyPartFileNames = streamProvider.BodyPartFileNames;

            // Create response
            return bodyPartFileNames.Select(kv =>
                {
                    FileInfo fileinfo = new FileInfo(kv.Value);
                    return new FileResult
                    {
                        FileName = kv.Key,
                        LocalPath = fileinfo.FullName,
                        LastModifiedTime = fileinfo.LastWriteTimeUtc,
                        Length = fileinfo.Length,
                        Submitter = submitter
                    };
                }).ToList();
        }

        [WebGet(UriTemplate = "?file={name}")]
        public Stream DownloadFile(string name)
        {
            string filename = Path.GetFileName(name);
            string filepath = Path.Combine(rootPath, filename);
            if (!File.Exists(filepath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return File.OpenRead(filepath);
        }
    }
}