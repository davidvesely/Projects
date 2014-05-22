// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MimeMultipart.Sample
{
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// This sample WCF service that produces MIME multipart in response to a 
    /// GET request. 
    /// </summary>
    internal class MultipartGeneratorService
    {
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage GetMultipart(HttpRequestMessage request)
        {
            MultipartContent multipart = new MultipartContent();
            multipart.Add(new StringContent("This is part 1"));
            multipart.Add(new StringContent("This is part 2"));
            multipart.Add(new StringContent("This is part 3"));

            HttpResponseMessage response = new HttpResponseMessage();
            response.RequestMessage = request;
            response.Content = multipart;
            return response;
        }
    }
}