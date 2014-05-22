// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiKeyHandler: DelegatingHandler
    {
        private const string APIKey = "apikey";
        private string key;

        public string Key 
        { 
            get
            {
                if (this.key == null)
                {
                    this.key = "default";
                }

                return this.key;
            }

            set
            {
                this.key = value;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            // Let us try header first
            if (request.Headers.Contains(APIKey))
            {   
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
                {
                    if (header.Key == APIKey && header.Value.Contains<string>(Key))
                    {
                        // we have a match from header!
                        return base.SendAsync(request, cancellationToken);
                    }
                }
            }

            // let us try request uri next
            string requestUri = request.RequestUri.Query;
            string[] queries = requestUri.Split('&');

            foreach (string s in queries)
            {
                if (s.Contains(APIKey))
                {
                    int index = s.IndexOf('=');
                    string receivedKey = s.Substring(index + 1, s.Length - index - 1);

                    if (receivedKey == Key)
                    {
                        // we have a match from Uri!
                        return base.SendAsync(request, cancellationToken);
                    }
                }
            }

            // here we should send back a validation failure response
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden)
                        {
                            Content = new StringContent("Not authorized! Please provide your credential.")
                        };
                });
        }
    }
}