// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System.Net.Http.Headers;
    using System.Text;

    /// <summary>
    /// Constants related to media types.
    /// </summary>
    internal static class MediaTypeConstants
    {
        private static readonly MediaTypeHeaderValue DefaultApplicationXmlMediaType = new MediaTypeHeaderValue("application/xml");
        private static readonly MediaTypeHeaderValue DefaultTextXmlMediaType = new MediaTypeHeaderValue("text/xml");
        private static readonly MediaTypeHeaderValue DefaultApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeHeaderValue DefaultTextJsonMediaType = new MediaTypeHeaderValue("text/json");
        private static readonly MediaTypeHeaderValue DefaultTextHtmlMediaType = new MediaTypeHeaderValue("text/html") { CharSet = Encoding.UTF8.WebName };
        private static readonly MediaTypeHeaderValue DefaultApplicationFormUrlEncodedMediaType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/html</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/html</c>.
        /// </value>
        public static MediaTypeHeaderValue HtmlMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultTextHtmlMediaType).Clone();
            }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/xml</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/xml</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationXmlMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultApplicationXmlMediaType).Clone();
            }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationJsonMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultApplicationJsonMediaType).Clone();
            }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/xml</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/xml</c>.
        /// </value>
        public static MediaTypeHeaderValue TextXmlMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultTextXmlMediaType).Clone();
            }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </value>
        public static MediaTypeHeaderValue TextJsonMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultTextJsonMediaType).Clone();
            }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/x-www-form-urlencoded</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/x-www-form-urlencoded</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationFormUrlEncodedMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultApplicationFormUrlEncodedMediaType).Clone();
            }
        }
    }
}
