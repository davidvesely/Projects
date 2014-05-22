// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http.Headers;

    /// <summary>
    /// Provides a special <see cref="MediaTypeHeaderValue"/> comparer function
    /// </summary>
    internal class MediaTypeHeaderValueEqualityComparer : IEqualityComparer<MediaTypeHeaderValue>
    {
        private static readonly MediaTypeHeaderValueEqualityComparer mediaTypeEqualityComparer = new MediaTypeHeaderValueEqualityComparer();

        private MediaTypeHeaderValueEqualityComparer()
        {
        }

        public static MediaTypeHeaderValueEqualityComparer EqualityComparer
        {
            get
            {
                return mediaTypeEqualityComparer;
            }
        }

        /// <summary>
        /// Determines whether two <see cref="MediaTypeHeaderValue"/> instances match. The instance
        /// <paramref name="mediaType1"/> is said to match <paramref name="mediaType2"/> if and only if
        /// <paramref name="mediaType1"/> is a strict subset of the values and parameters of <paramref name="mediaType2"/>. 
        /// That is, if the media type and media type parameters of <paramref name="mediaType1"/> are all present 
        /// and match those of <paramref name="mediaType2"/> then it is a match even though <paramref name="mediaType2"/> may have additional
        /// parameters.
        /// </summary>
        /// <param name="mediaType1">The first media type.</param>
        /// <param name="mediaType2">The second media type.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is part of implementing IEqualityComparer.")]
        public bool Equals(MediaTypeHeaderValue mediaType1, MediaTypeHeaderValue mediaType2)
        {
            Contract.Assert(mediaType1 != null, "The 'mediaType1' parameter should not be null.");
            Contract.Assert(mediaType2 != null, "The 'mediaType2' parameter should not be null.");

            if (!string.Equals(mediaType1.MediaType, mediaType2.MediaType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            foreach (NameValueHeaderValue parameter1 in mediaType1.Parameters)
            {
                if (mediaType2.Parameters.FirstOrDefault(
                    (parameter2) =>
                    {
                        return
                            string.Equals(parameter1.Name, parameter2.Name, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(parameter1.Value, parameter2.Value, StringComparison.OrdinalIgnoreCase);
                    }) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(MediaTypeHeaderValue mediaType)
        {
            return mediaType.MediaType.ToUpperInvariant().GetHashCode();
        }
    }
}