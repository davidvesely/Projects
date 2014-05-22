// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// This class provides a mapping from an arbitrary HTTP request header field to a <see cref="MediaTypeHeaderValue"/>
    /// used to select <see cref="MediaTypeFormatter"/> instances for handling the entity body of an <see cref="HttpRequestMessage"/>
    /// or <see cref="HttpResponseMessage"/>.
    /// <remarks>This class only checks header fields associated with <see cref="M:HttpRequestMessage.Headers"/> for a match. It does
    /// not check header fields associated with <see cref="M:HttpResponseMessage.Headers"/> or <see cref="M:HttpContent.Headers"/> instances.</remarks>
    /// </summary>
    public class RequestHeaderMapping : MediaTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderMapping"/> class.
        /// </summary>
        /// <param name="headerName">Name of the header to match.</param>
        /// <param name="headerValue">The header value to match.</param>
        /// <param name="valueComparison">The value comparison to use when matching <paramref name="headerValue"/>.</param>
        /// <param name="isValueSubstring">if set to <c>true</c> then <paramref name="headerValue"/> is 
        /// considered a match if it matches a substring of the actual header value.</param>
        /// <param name="mediaType">The media type to use if <paramref name="headerName"/> and <paramref name="headerValue"/> 
        /// is considered a match.</param>
        public RequestHeaderMapping(string headerName, string headerValue, StringComparison valueComparison, bool isValueSubstring, string mediaType)
            : base(mediaType)
        {
            this.Initialize(headerName, headerValue, valueComparison, isValueSubstring);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderMapping"/> class.
        /// </summary>
        /// <param name="headerName">Name of the header to match.</param>
        /// <param name="headerValue">The header value to match.</param>
        /// <param name="valueComparison">The <see cref="StringComparison"/> to use when matching <paramref name="headerValue"/>.</param>
        /// <param name="isValueSubstring">if set to <c>true</c> then <paramref name="headerValue"/> is 
        /// considered a match if it matches a substring of the actual header value.</param>
        /// <param name="mediaType">The <see cref="MediaTypeHeaderValue"/> to use if <paramref name="headerName"/> and <paramref name="headerValue"/> 
        /// is considered a match.</param>
        public RequestHeaderMapping(string headerName, string headerValue, StringComparison valueComparison, bool isValueSubstring, MediaTypeHeaderValue mediaType)
            : base(mediaType)
        {
            this.Initialize(headerName, headerValue, valueComparison, isValueSubstring);
        }

        /// <summary>
        /// Gets the name of the header to match.
        /// </summary>
        public string HeaderName { get; private set; }

        /// <summary>
        /// Gets the header value to match.
        /// </summary>
        public string HeaderValue { get; private set; }

        /// <summary>
        /// Gets the <see cref="StringComparison"/> to use when matching <see cref="M:HeaderValue"/>.
        /// </summary>
        public StringComparison HeaderValueComparison { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="M:HeaderValue"/> is 
        /// a matched as a substring of the actual header value.
        /// this instance is value substring.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="HeaderValue"/> is to be matched as a substring; otherwise <c>false</c>.
        /// </value>
        public bool IsValueSubstring { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the current <see cref="RequestHeaderMapping"/>
        /// instance can return a <see cref="MediaTypeHeaderValue"/> from <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to check.</param>
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        protected override double OnTryMatchMediaType(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return MatchHeaderValue(request, this.HeaderName, this.HeaderValue, this.HeaderValueComparison, this.IsValueSubstring);
        }

        /// <summary>
        /// Returns a value indicating whether the current <see cref="RequestHeaderMapping"/>
        /// instance can return a <see cref="MediaTypeHeaderValue"/> from <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> to check.</param>
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        protected override double OnTryMatchMediaType(HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (response.RequestMessage != null)
            {
                return MatchHeaderValue(response.RequestMessage, this.HeaderName, this.HeaderValue, this.HeaderValueComparison, this.IsValueSubstring);
            }

            return MediaTypeMatch.NoMatch;
        }

        private static double MatchHeaderValue(HttpRequestMessage request, string headerName, string headerValue, StringComparison valueComparison, bool isValueSubstring)
        {
            Contract.Assert(request != null, "request should not be null");
            Contract.Assert(headerName != null, "header name should not be null");
            Contract.Assert(headerValue != null, "header value should not be null");

            IEnumerable<string> values;
            if (request.Headers.TryGetValues(headerName, out values))
            {
                foreach (string value in values)
                {
                    if (isValueSubstring)
                    {
                        if (value.IndexOf(headerValue, valueComparison) != -1)
                        {
                            return MediaTypeMatch.Match;
                        }
                    }
                    else
                    {
                        if (value.Equals(headerValue, valueComparison))
                        {
                            return MediaTypeMatch.Match;
                        }
                    }
                }
            }

            return MediaTypeMatch.NoMatch;
        }

        private void Initialize(string headerName, string headerValue, StringComparison valueComparison, bool isValueSubstring)
        {
            if (string.IsNullOrWhiteSpace(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            if (string.IsNullOrWhiteSpace(headerValue))
            {
                throw new ArgumentNullException("headerValue");
            }

            StringComparisonHelper.Validate(valueComparison, "valueComparison");

            this.HeaderName = headerName;
            this.HeaderValue = headerValue;
            this.HeaderValueComparison = valueComparison;
            this.IsValueSubstring = isValueSubstring;
        }
    }
}