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
    /// This sample WCF service reads the contents of multiple individual HTML file upload requests and 
    /// returns a list of all of then when done. This illustrates how to do upload of spanned archives.
    /// </summary>
    internal class SplitFileService
    {
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public List<FileResult> UploadFile(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Get location where we store the various parts and create stream provider
            string rootPath = Path.Combine(Path.GetTempPath(), "SplitFileService");
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(rootPath);

            // Read the MIME multipart content
            IEnumerable<HttpContent> bodyparts = request.Content.ReadAsMultipart(streamProvider);

            // The radio button is the entity with a Content-Disposition header field with a "name" parameter with value "final"
            bool isFinal = false;
            string radioButtonValue;
            if (bodyparts.TryGetFormFieldValue("final", out radioButtonValue))
            {
                isFinal = radioButtonValue.Equals("yes", System.StringComparison.OrdinalIgnoreCase);
            }

            // Get list of files written to the rootPath
            string[] partPaths = Directory.GetFiles(rootPath);
            return partPaths.Select(path =>
                {
                    FileInfo fileinfo = new FileInfo(path);
                    return new FileResult
                    {
                        LocalPath = fileinfo.FullName,
                        LastModifiedTime = fileinfo.LastWriteTimeUtc,
                        Length = fileinfo.Length,
                        Partial = !isFinal
                    };
                }).ToList();
        }
    }
}