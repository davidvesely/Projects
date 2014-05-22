// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http.Headers;

    internal static class MediaTypeHeaderValueExtensionMethods
    {
        public static bool IsMediaRange(this MediaTypeHeaderValue mediaType)
        {
            Contract.Assert(mediaType != null, "The 'mediaType' parameter should not be null.");
            return new ParsedMediaTypeHeaderValue(mediaType).IsSubTypeMediaRange;
        }

        public static bool IsWithinMediaRange(this MediaTypeHeaderValue mediaType, MediaTypeHeaderValue mediaRange)
        {
            Contract.Assert(mediaType != null, "The 'mediaType' parameter should not be null.");
            Contract.Assert(mediaRange != null, "The 'mediaRange' parameter should not be null.");

            ParsedMediaTypeHeaderValue parsedMediaType = new ParsedMediaTypeHeaderValue(mediaType);
            ParsedMediaTypeHeaderValue parsedMediaRange = new ParsedMediaTypeHeaderValue(mediaRange);

            if (!string.Equals(parsedMediaType.Type, parsedMediaRange.Type, StringComparison.OrdinalIgnoreCase))
            {
                return parsedMediaRange.IsAllMediaRange;
            }
            else if (!string.Equals(parsedMediaType.SubType, parsedMediaRange.SubType, StringComparison.OrdinalIgnoreCase))
            {
                return parsedMediaRange.IsSubTypeMediaRange;
            }

            if (!string.IsNullOrWhiteSpace(parsedMediaRange.CharSet))
            {
                return string.Equals(parsedMediaRange.CharSet, parsedMediaType.CharSet, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}