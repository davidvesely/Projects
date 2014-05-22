// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Net.Http;
    using System.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class JsonpHttpResponseHandler : HttpOperationHandler<HttpResponseMessage, HttpResponseMessage>
    {
        private string callbackQueryParameter;

        public JsonpHttpResponseHandler() : base("response") 
        { 
        }

        public string CallbackQueryParameter
        {
            get { return this.callbackQueryParameter ?? "callback"; }
            set { this.callbackQueryParameter = value; }
        }

        protected override HttpResponseMessage OnHandle(HttpResponseMessage response)
        {
            string callback;
            if (this.IsJsonpRequest(response.RequestMessage, out callback))
            {
                response.Content.Headers.Add("jsonp-callback", callback);
            }

            return response;
        }

        private bool IsJsonpRequest(HttpRequestMessage request, out string callback)
        {
            callback = null;
            if (request.Method != HttpMethod.Get)
            {
                return false;
            }

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            callback = query[this.CallbackQueryParameter];
            return !string.IsNullOrEmpty(callback);
        }
    }
}
