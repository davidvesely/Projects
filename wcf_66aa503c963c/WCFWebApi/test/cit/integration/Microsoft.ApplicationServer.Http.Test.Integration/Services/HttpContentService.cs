// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    public class HttpContentService
    {
        [WebGet(UriTemplate = "GetHttpContent/{value}")]
        public HttpContent GetHttpContent(string value)
        {
            return new StringContent(value);
        }

        [WebGet(UriTemplate = "GetObjectContent/{value}")]
        public ObjectContent GetObjectContent(string value)
        {
            return new ObjectContent<int>(int.Parse(value));
        }

        [WebGet(UriTemplate = "GetObjectContentOfT/{value}")]
        public ObjectContent<int> GetObjectContentOfT(string value)
        {
            return new ObjectContent<int>(int.Parse(value));
        }

        [WebInvoke(Method = "POST")]
        public HttpContent PostHttpContent(HttpContent content)
        {
            return content;
        }

        [WebInvoke(Method = "POST")]
        public ObjectContent<int> PostObjectContentOfT(ObjectContent<int> content)
        {
            int value = content.ReadAsAsync().Result;
            return new ObjectContent<int>(value);
        }
    }
}
