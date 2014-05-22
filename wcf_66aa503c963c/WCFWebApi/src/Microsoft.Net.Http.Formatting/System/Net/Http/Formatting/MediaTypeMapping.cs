// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// An abstract base class used to create an association between <see cref="HttpRequestMessage"/> or 
    /// <see cref="HttpResponseMessage"/> instances that have certain characteristics 
    /// and a specific <see cref="MediaTypeHeaderValue"/>. 
    /// </summary>
    public abstract class MediaTypeMapping
    {
        private static readonly Type httpRequestMessageType = typeof(HttpRequestMessage);
        private static readonly Type httpResponseMessageType = typeof(HttpResponseMessage);

        /// <summary>
        /// Initializes a new instance of a <see cref="MediaTypeMapping"/> with the
        /// given <paramref name="mediaType"/> value.
        /// </summary>
        /// <param name="mediaType">
        /// The <see cref="MediaTypeHeaderValue"/> that is associated with <see cref="HttpRequestMessage"/> or 
        /// <see cref="HttpResponseMessage"/> instances that have the given characteristics of the 
        /// <see cref="MediaTypeMapping"/>.
        /// </param>
        protected MediaTypeMapping(MediaTypeHeaderValue mediaType)
        {
            if (mediaType == null)
            {
                throw new ArgumentNullException("mediaType");
            }

            this.MediaType = mediaType;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="MediaTypeMapping"/> with the
        /// given <paramref name="mediaType"/> value.
        /// </summary>
        /// <param name="mediaType">
        /// The <see cref="string"/> that is associated with <see cref="HttpRequestMessage"/> or 
        /// <see cref="HttpResponseMessage"/> instances that have the given characteristics of the 
        /// <see cref="MediaTypeMapping"/>.
        /// </param>
        protected MediaTypeMapping(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            this.MediaType = new MediaTypeHeaderValue(mediaType);
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeHeaderValue"/> that is associated with <see cref="HttpRequestMessage"/> or 
        /// <see cref="HttpResponseMessage"/> instances that have the given characteristics of the 
        /// <see cref="MediaTypeMapping"/>.
        /// </summary>
        public MediaTypeHeaderValue MediaType { get; private set; }

        /// <summary>
        /// Returns the quality of the match of the <see cref="MediaTypeHeaderValue"/>
        /// associated with <paramref name="request"/>.
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequestMessage"/> to evaluate for the characteristics 
        /// associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>.
        /// </param> 
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        public double TryMatchMediaType(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return this.OnTryMatchMediaType(request);
        }

        /// <summary>
        /// Returns the quality of the match of the <see cref="MediaTypeHeaderValue"/>
        /// associated with <paramref name="response"/>.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to evaluate for the characteristics 
        /// associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>.
        /// </param> 
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        public double TryMatchMediaType(HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (response.RequestMessage == null)
            {
                throw new InvalidOperationException(
                    SR.ResponseMustReferenceRequest(
                        httpResponseMessageType.Name, 
                        "response", 
                        httpRequestMessageType.Name, 
                        "RequestMessage"));
            }

            return this.OnTryMatchMediaType(response);
        }

        /// <summary>
        /// Implemented in a derived class to determine if the <see cref="HttpRequestMessage"/> 
        /// should be associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>. 
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequestMessage"/> to evaluate for the characteristics 
        /// associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>.
        /// </param> 
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        protected abstract double OnTryMatchMediaType(HttpRequestMessage request);

        /// <summary>
        /// Implemented in a derived class to determine if the <see cref="HttpResponseMessage"/> 
        /// should be associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>. 
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to evaluate for the characteristics 
        /// associated with the <see cref="MediaTypeHeaderValue"/>
        /// of the <see cref="MediaTypeMapping"/>.
        /// </param> 
        /// <returns>
        /// The quality of the match. It must be between <c>0.0</c> and <c>1.0</c>.
        /// A value of <c>0.0</c> signifies no match.
        /// A value of <c>1.0</c> signifies a complete match.
        /// </returns>
        protected abstract double OnTryMatchMediaType(HttpResponseMessage response);
    }
}