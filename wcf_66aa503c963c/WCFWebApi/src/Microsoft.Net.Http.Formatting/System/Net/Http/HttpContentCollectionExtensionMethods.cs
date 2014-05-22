// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http.Headers;

    /// <summary>
    /// Extension methods to provide convenience methods for finding <see cref="HttpContent"/> items  
    /// within a <see cref="IEnumerable{HttpContent}"/> collection.
    /// </summary>
    public static class HttpContentCollectionExtensionMethods
    {
        private const string ContentID = @"Content-ID";

        /// <summary>
        /// Returns the first <see cref="HttpContent"/> in a sequence that has a <see cref="ContentDispositionHeaderValue"/> header field
        /// with a <see cref="ContentDispositionHeaderValue.DispositionType"/> property equal to <paramref name="dispositionType"/>.
        /// </summary>
        /// <param name="contents">The contents to evaluate</param>
        /// <param name="dispositionType">The disposition type to look for.</param>
        /// <returns>The first <see cref="HttpContent"/> in the sequence with a matching disposition type.</returns>
        public static HttpContent FirstDispositionType(this IEnumerable<HttpContent> contents, string dispositionType)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(dispositionType))
            {
                throw new ArgumentNullException("dispositionType");
            }

            return contents.First((item) =>
                {
                    return HttpContentCollectionExtensionMethods.FirstDispositionType(item, dispositionType);
                });
        }

        /// <summary>
        /// Returns the first <see cref="HttpContent"/> in a sequence that has a <see cref="ContentDispositionHeaderValue"/> header field
        /// with a <see cref="ContentDispositionHeaderValue.DispositionType"/> property equal to <paramref name="dispositionType"/>.
        /// </summary>
        /// <param name="contents">The contents to evaluate</param>
        /// <param name="dispositionType">The disposition type to look for.</param>
        /// <returns>null if source is empty or if no element matches; otherwise the first <see cref="HttpContent"/> in 
        /// the sequence with a matching disposition type.</returns>
        public static HttpContent FirstDispositionTypeOrDefault(this IEnumerable<HttpContent> contents, string dispositionType)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(dispositionType))
            {
                throw new ArgumentNullException("dispositionType");
            }

            return contents.FirstOrDefault((item) =>
            {
                return HttpContentCollectionExtensionMethods.FirstDispositionType(item, dispositionType);
            });
        }

        /// <summary>
        /// Returns the first <see cref="HttpContent"/> in a sequence that has a <see cref="ContentDispositionHeaderValue"/> header field
        /// with a <see cref="ContentDispositionHeaderValue.Name"/> property equal to <paramref name="dispositionName"/>.
        /// </summary>
        /// <param name="contents">The contents to evaluate</param>
        /// <param name="dispositionName">The disposition name to look for.</param>
        /// <returns>The first <see cref="HttpContent"/> in the sequence with a matching disposition name.</returns>
        public static HttpContent FirstDispositionName(this IEnumerable<HttpContent> contents, string dispositionName)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(dispositionName))
            {
                throw new ArgumentNullException("dispositionName");
            }

            return contents.First((item) =>
            {
                return HttpContentCollectionExtensionMethods.FirstDispositionName(item, dispositionName);
            });
        }

        /// <summary>
        /// Returns the first <see cref="HttpContent"/> in a sequence that has a <see cref="ContentDispositionHeaderValue"/> header field
        /// with a <see cref="ContentDispositionHeaderValue.Name"/> property equal to <paramref name="dispositionName"/>.
        /// </summary>
        /// <param name="contents">The contents to evaluate</param>
        /// <param name="dispositionName">The disposition name to look for.</param>
        /// <returns>null if source is empty or if no element matches; otherwise the first <see cref="HttpContent"/> in 
        /// the sequence with a matching disposition name.</returns>
        public static HttpContent FirstDispositionNameOrDefault(this IEnumerable<HttpContent> contents, string dispositionName)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(dispositionName))
            {
                throw new ArgumentNullException("dispositionName");
            }

            return contents.FirstOrDefault((item) =>
            {
                return HttpContentCollectionExtensionMethods.FirstDispositionName(item, dispositionName);
            });
        }

        /// <summary>
        /// Returns the <c>start</c> multipart body part. The <c>start</c> is used to identify the main body 
        /// in <c>multipart/related</c> content (see RFC 2387).
        /// </summary>
        /// <param name="contents">The contents to evaluate.</param>
        /// <param name="start">The <c>start</c> value to look for. 
        /// A match is found if a <see cref="HttpContent"/> has a <c>Content-ID</c> 
        /// header field with the given value.</param>
        /// <returns>The first <see cref="HttpContent"/> in the sequence with a matching value.</returns>
        public static HttpContent FirstStart(this IEnumerable<HttpContent> contents, string start)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(start))
            {
                throw new ArgumentNullException("start");
            }

            return contents.First((item) =>
            {
                return HttpContentCollectionExtensionMethods.FirstStart(item, start);
            });
        }

        /// <summary>
        /// Returns the first <see cref="HttpContent"/> in a sequence that has a <see cref="ContentDispositionHeaderValue"/> header field
        /// parameter equal to <paramref name="start"/>. This parameter is typically used in connection with <c>multipart/related</c>
        /// content (see RFC 2387).
        /// </summary>
        /// <param name="contents">The contents to evaluate.</param>
        /// <param name="start">The start value to look for. A match is found if a <see cref="HttpContent"/> has a <c>Content-ID</c> 
        /// header field with the given value.</param>
        /// <returns>null if source is empty or if no element matches; otherwise the first <see cref="HttpContent"/> in 
        /// the sequence with a matching value.</returns>
        public static HttpContent FirstStartOrDefault(this IEnumerable<HttpContent> contents, string start)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrWhiteSpace(start))
            {
                throw new ArgumentNullException("start");
            }

            return contents.FirstOrDefault((item) =>
            {
                return HttpContentCollectionExtensionMethods.FirstStart(item, start);
            });
        }

        /// <summary>
        /// Tries to get the contents of the first <see cref="HttpContent"/> that has a 
        /// <see cref="ContentDispositionHeaderValue"/> header field with a 
        /// <see cref="ContentDispositionHeaderValue.Name"/> property equal 
        /// to <paramref name="dispositionName"/>.
        /// </summary>
        /// <param name="contents">The contents to evaluate.</param>
        /// <param name="dispositionName">The disposition name to look for.</param>
        /// <param name="formFieldValue">The form field value as a string.</param>
        /// <returns>The string content of the form field with the given disposition name if found; otherwise null.</returns>
        public static bool TryGetFormFieldValue(this IEnumerable<HttpContent> contents, string dispositionName, out string formFieldValue)
        {
            formFieldValue = null;
            HttpContent content = HttpContentCollectionExtensionMethods.FirstDispositionNameOrDefault(contents, dispositionName);
            if (content != null)
            {
                formFieldValue = content.ReadAsStringAsync().Result;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all instances of <see cref="HttpContent"/> in a sequence that has a <see cref="MediaTypeHeaderValue"/> header field
        /// with a <see cref="MediaTypeHeaderValue.MediaType"/> property equal to the provided <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contents">The content to evaluate</param>
        /// <param name="contentType">The media type to look for.</param>
        /// <returns>null if source is empty or if no element matches; otherwise the first <see cref="HttpContent"/> in 
        /// the sequence with a matching media type.</returns>
        public static IEnumerable<HttpContent> FindAllContentType(this IEnumerable<HttpContent> contents, string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException("contentType");
            }

            return HttpContentCollectionExtensionMethods.FindAllContentType(contents, new MediaTypeHeaderValue(contentType));
        }

        /// <summary>
        /// Returns all instances of <see cref="HttpContent"/> in a sequence that has a <see cref="MediaTypeHeaderValue"/> header field
        /// with a <see cref="MediaTypeHeaderValue.MediaType"/> property equal to the provided <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contents">The content to evaluate</param>
        /// <param name="contentType">The media type to look for.</param>
        /// <returns>null if source is empty or if no element matches; otherwise the first <see cref="HttpContent"/> in 
        /// the sequence with a matching media type.</returns>
        public static IEnumerable<HttpContent> FindAllContentType(this IEnumerable<HttpContent> contents, MediaTypeHeaderValue contentType)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            return contents.Where((item) =>
            {
                return HttpContentCollectionExtensionMethods.FindAllContentType(item, contentType);
            });
        }

        private static bool FirstStart(HttpContent content, string start)
        {
            Contract.Assert(content != null, "content cannot be null");
            Contract.Assert(start != null, "start cannot be null");
            if (content.Headers != null)
            {
                IEnumerable<string> values;
                if (content.Headers.TryGetValues(ContentID, out values))
                {
                    return string.Equals(
                        FormattingUtilities.UnquoteToken(values.ElementAt(0)),
                        FormattingUtilities.UnquoteToken(start),
                        StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;
        }

        private static bool FirstDispositionType(HttpContent content, string dispositionType)
        {
            Contract.Assert(content != null, "content cannot be null");
            Contract.Assert(dispositionType != null, "dispositionType cannot be null");
            return content.Headers != null && content.Headers.ContentDisposition != null &&
                string.Equals(
                FormattingUtilities.UnquoteToken(content.Headers.ContentDisposition.DispositionType),
                FormattingUtilities.UnquoteToken(dispositionType), 
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool FirstDispositionName(HttpContent content, string dispositionName)
        {
            Contract.Assert(content != null, "content cannot be null");
            Contract.Assert(dispositionName != null, "dispositionName cannot be null");
            return content.Headers != null && content.Headers.ContentDisposition != null &&
                string.Equals(
                FormattingUtilities.UnquoteToken(content.Headers.ContentDisposition.Name),
                FormattingUtilities.UnquoteToken(dispositionName), 
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool FindAllContentType(HttpContent content, MediaTypeHeaderValue contentType)
        {
            Contract.Assert(content != null, "content cannot be null");
            Contract.Assert(contentType != null, "contentType cannot be null");
            return content.Headers != null && content.Headers.ContentType != null &&
                string.Equals(
                content.Headers.ContentType.MediaType, 
                contentType.MediaType, 
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
