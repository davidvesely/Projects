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
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Ionic.Zlib;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// This sample WCF service reads the contents of an HTML file upload and writes one or more body parts to a local file
    /// while decompressing the contents on the fly using a deflate stream from the DotNetZip library.
    /// </summary>
    internal class DotNetZipService : MultipartFormDataStreamProvider
    {
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public List<FileResult> UploadFile(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Read the MIME multipart content using this as our stream provider
            IEnumerable<HttpContent> bodyparts = request.Content.ReadAsMultipart(this);

            // Get the list of local file names from the base class and create response
            return this.BodyPartFileNames.Select(kv =>
            {
                FileInfo fileinfo = new FileInfo(kv.Value);
                return new FileResult
                {
                    FileName = kv.Key,
                    LocalPath = fileinfo.FullName,
                    LastModifiedTime = fileinfo.LastWriteTimeUtc,
                    Length = fileinfo.Length
                };
            }).ToList();
        }

        /// <summary>
        /// This <see cref="IMultipartStreamProvider"/> implementation is based on <see cref="MultipartFormDataStreamProvider"/> but 
        /// inserts a compression stream to decompress the body part as it is written to disk.
        /// </summary>
        /// <param name="headers">Header fields describing the body part</param>
        /// <returns>The <see cref="Stream"/> instance where the message body part is written to.</returns>
        protected override Stream OnGetStream(HttpContentHeaders headers)
        {
            Stream output = base.OnGetStream(headers);
            if (output is MemoryStream)
            {
                // If memory stream then just write straight to output
                return output;
            }
            else
            {
                // Plug in deflate stream to decompress while writing to disk                   
                return new Ionic.Zlib.DeflateStream(output, CompressionMode.Decompress);
            }
        }
    }
}