// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.Integration
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Web;
    
    /// <summary>
    /// Class that provides <see cref="MediaTypeHeaderValue"/>s for OData from query strings.
    /// </summary>
    internal class ODataMediaTypeMapping : MediaTypeMapping
    {
        internal const string QueryStringFormatParameter = "format";
        internal const string QueryFormatODataValue = "odata";
        private static readonly Type typeODataMediaTypeMapping = typeof(ODataMediaTypeMapping);

        /// <summary>
        /// ODataMediaTypeMapping constructor.
        /// </summary>
        /// <param name="mediaType">The media type to use if the query parameter for OData is present </param>
        public ODataMediaTypeMapping(string mediaType) : base(mediaType)
        {
        }

        /// <summary>
        /// ODataMediaTypeMapping constructor.
        /// </summary>
        /// <param name="mediaType">The media type to use if the query parameter for OData is present </param>
        public ODataMediaTypeMapping(MediaTypeHeaderValue mediaType)
            : base(mediaType)
        {
        }

        /// <summary>
        /// Returns a value indicating the quality of the media type match for the current <see cref="ODataMediaTypeMapping"/>
        /// instance for <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to check.</param>
        /// <returns>This base class unconditionally returns <c>0.0</c>.</returns>
        protected override double OnTryMatchMediaType(HttpRequestMessage request)
        {
            return 0.0;
        }

        /// <summary>
        /// Returns a value indicating the quality of the media type match for the current <see cref="ODataMediaTypeMapping"/>
        /// instance for <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> to check.</param>
        /// <returns>If this instance can produce a <see cref="MediaTypeHeaderValue"/> from <paramref name="response"/>
        /// it returns <c>1.0</c> otherwise <c>false</c>.</returns>
        protected override sealed double OnTryMatchMediaType(HttpResponseMessage response)
        {
            NameValueCollection queryString = GetQueryString(response.RequestMessage.RequestUri);
            MediaTypeWithQualityHeaderValue responseMediaType = response.RequestMessage.Headers.Accept.FirstOrDefault();
            if (responseMediaType == null)
            {
                return 0.0;
            }

            double quality = responseMediaType.Quality.HasValue ? responseMediaType.Quality.Value : 1.0;

            return String.Equals(responseMediaType.ToString(), MediaType.ToString(), StringComparison.OrdinalIgnoreCase) && this.DoesQueryStringMatch(queryString)
                        ? quality
                        : 0.0;
        }

        private static NameValueCollection GetQueryString(Uri uri)
        {
            if (uri == null)
            {
                throw new InvalidOperationException(String.Format("Uri cannot be null for {0}", typeODataMediaTypeMapping.Name));
            }

            return HttpUtility.ParseQueryString(uri.Query);
        }

        private bool DoesQueryStringMatch(NameValueCollection queryString)
        {
            if (queryString != null)
            {
                foreach (string queryParameter in queryString.AllKeys)
                {
                    if (string.Equals(queryParameter, ODataMediaTypeMapping.QueryStringFormatParameter, StringComparison.OrdinalIgnoreCase))
                    {
                        string queryValue = queryString[queryParameter];
                        if (string.Equals(queryValue, ODataMediaTypeMapping.QueryFormatODataValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }   
    }
}
