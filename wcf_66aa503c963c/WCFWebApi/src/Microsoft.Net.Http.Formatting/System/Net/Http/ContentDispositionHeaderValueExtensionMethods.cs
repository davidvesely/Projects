// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.IO;
    using System.Net.Http.Headers;

    /// <summary>
    /// Extension methods for <see cref="ContentDispositionHeaderValue"/>.
    /// </summary>
    internal static class ContentDispositionHeaderValueExtensionMethods
    {
        private static readonly Type contentDispositionHeaderValueType = typeof(ContentDispositionHeaderValue);

        /// <summary>
        /// Returns a file name suitable for use on the local file system. The file name is extracted from 
        /// <see cref="ContentDispositionHeaderValue.FileNameStar"/> and <see cref="ContentDispositionHeaderValue.FileName"/>
        /// in that order.
        /// </summary>
        /// <param name="contentDisposition">The content disposition to extract a local file name from.</param>
        /// <returns>A file name (without any path components) suitable for use on local file system.</returns>
        public static string ExtractLocalFileName(this ContentDispositionHeaderValue contentDisposition)
        {
            if (contentDisposition == null)
            {
                throw new ArgumentNullException("contentDisposition");
            }

            string candidate = contentDisposition.FileNameStar;
            if (string.IsNullOrEmpty(candidate))
            {
                candidate = contentDisposition.FileName;
            }

            if (string.IsNullOrWhiteSpace(candidate))
            {
                throw new ArgumentException(
                    SR.ContentDispositionInvalidFileName(contentDispositionHeaderValueType.Name, candidate),
                    "contentDisposition");
            }

            string unquotedFileName = FormattingUtilities.UnquoteToken(candidate);
            if (string.IsNullOrWhiteSpace(unquotedFileName))
            {
                throw new ArgumentException(
                    SR.ContentDispositionInvalidFileName(contentDispositionHeaderValueType.Name, unquotedFileName),
                    "contentDisposition");
            }

            // Get rid of all path components
            return Path.GetFileName(unquotedFileName);
        }
    }
}
