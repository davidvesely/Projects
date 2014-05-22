// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Class that provides <see cref="MediaTypeHeaderValue"/>s for a request or response
    /// from a media range.
    /// </summary>
    public sealed class MediaRangeMapping : MediaTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRangeMapping"/> class.
        /// </summary>
        /// <param name="mediaRange">The <see cref="MediaTypeHeaderValue"/> that provides a description
        /// of the media range.</param>
        /// <param name="mediaType">The <see cref="MediaTypeHeaderValue"/> to return on a match.</param>
        public MediaRangeMapping(MediaTypeHeaderValue mediaRange, MediaTypeHeaderValue mediaType)
            : base(mediaType)
        {
            this.Initialize(mediaRange);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRangeMapping"/> class.
        /// </summary>
        /// <param name="mediaRange">The description of the media range.</param>
        /// <param name="mediaType">The media type to return on a match.</param>
        public MediaRangeMapping(string mediaRange, string mediaType)
            : base(mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaRange))
            {
                throw new ArgumentNullException("mediaRange");
            }

            this.Initialize(new MediaTypeHeaderValue(mediaRange));
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeHeaderValue"/>
        /// describing the known media range.
        /// </summary>
        public MediaTypeHeaderValue MediaRange { get; private set; }

        /// <summary>
        /// Returns a value indicating whether this <see cref="MediaRangeMapping"/>
        /// instance can provide a <see cref="MediaTypeHeaderValue"/> for the <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to check.</param>
        /// <returns>This method always returns <c>0.0</c>.</returns>
        protected override sealed double OnTryMatchMediaType(HttpRequestMessage request)
        {
            return MediaTypeMatch.NoMatch;
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="MediaRangeMapping"/>
        /// instance can provide a <see cref="MediaTypeHeaderValue"/> for the <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> to check.</param>
        /// <returns>If this instance can match <paramref name="response"/>
        /// it returns the quality of the match otherwise <c>0.0</c>.</returns>
        protected override sealed double OnTryMatchMediaType(HttpResponseMessage response)
        {
            Contract.Assert(response != null, "Base class ensures that the 'response' parameter will never be null.");
            Contract.Assert(response.RequestMessage != null, "Base class ensures that the 'response.RequestMessage' will never be null.");

            ICollection<MediaTypeWithQualityHeaderValue> acceptHeader = response.RequestMessage.Headers.Accept;
            if (acceptHeader != null)
            {
                foreach (MediaTypeWithQualityHeaderValue mediaType in acceptHeader)
                {
                    if (mediaType != null && MediaTypeHeaderValueEqualityComparer.EqualityComparer.Equals(this.MediaRange, mediaType))
                    {
                        return mediaType.Quality.HasValue ? mediaType.Quality.Value : MediaTypeMatch.Match;
                    }
                }
            }

            return MediaTypeMatch.NoMatch;
        }

        private void Initialize(MediaTypeHeaderValue mediaRange)
        {
            if (mediaRange == null)
            {
                throw new ArgumentNullException("mediaRange");
            }

            if (!mediaRange.IsMediaRange())
            {
                throw new InvalidOperationException(SR.InvalidMediaRange(mediaRange.ToString()));
            }

            this.MediaRange = mediaRange;
        }
    }
}