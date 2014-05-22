// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MimeMultipart.Sample
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// This sample WCF service reads the contents of an HTML file upload and writes one or more body parts to a local file.
    /// </summary>
    internal class FileService
    {
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public List<FileResult> UploadFile(HttpRequestMessage request)
        {
            // Verify that this is an HTML Form file upload request
            if (!request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Create a stream provider for setting up output streams
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider();

            // Read the MIME multipart content using the stream provider we just created.
            IEnumerable<HttpContent> bodyparts = request.Content.ReadAsMultipart(streamProvider);

            // The submitter field is the entity with a Content-Disposition header field with a "name" parameter with value "submitter"
            string submitter;
            if (!bodyparts.TryGetFormFieldValue("submitter", out submitter))
            {
                submitter = "unknown";
            }

            // Get a dictionary of local file names from stream provider.
            // The filename parameters provided in Content-Disposition header fields are the keys.
            // The local file names where the files are stored are the values.
            IDictionary<string, string> bodyPartFileNames = streamProvider.BodyPartFileNames;

            // Create response containing information about the stored files.
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
    }
}