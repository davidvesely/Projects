// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MimeMultipart.Sample
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// This sample WCF service reads the contents of an HTML file upload and 
    /// deserializes the contents into a local object which is then returned 
    /// in the response. 
    /// </summary>
    internal class TypeService
    {
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public List<Contact> UploadContacts(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Read the MIME multipart content
            IEnumerable<HttpContent> bodyparts = request.Content.ReadAsMultipart();

            // The actual data is the entity with a Content-Disposition header field 
            // with a "name" parameter with value "data"
            HttpContent dataContent = bodyparts.FirstDispositionNameOrDefault("data");
            if (dataContent != null)
            {
                // Read contents which deserializes the content based on the content type.
                // The browser typically sets the content type if it knows about the file
                // extension. However, if it doesn't set it then it is possible to set 
                // here.
                var contacts = dataContent.ReadAsAsync<List<Contact>>().Result;
                return contacts;
            }

            return new List<Contact>();
        }
    }
}