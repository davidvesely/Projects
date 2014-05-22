// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Collection class that contains <see cref="MediaTypeFormatter"/> instances.
    /// </summary>
    public class MediaTypeFormatterCollection : Collection<MediaTypeFormatter>
    {
        private static readonly Type mediaTypeFormatterType = typeof(MediaTypeFormatter);

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeFormatterCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This collection will be initialized to contain default <see cref="MediaTypeFormatter"/>
        /// instances for Xml, JsonValue and Json.
        /// </remarks>
        public MediaTypeFormatterCollection()
            : this(CreateDefaultFormatters())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeFormatterCollection"/> class.
        /// </summary>
        /// <param name="formatters">A collection of <see cref="MediaTypeFormatter"/> instances to place in the collection.</param>
        public MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter> formatters)
            : base()
        {
            this.VerifyAndSetFormatters(formatters);
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeFormatter"/> to use for Xml.
        /// </summary>
        public XmlMediaTypeFormatter XmlFormatter
        {
            get
            {
                return this.Items.OfType<XmlMediaTypeFormatter>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeFormatter"/> to use for Json.
        /// </summary>
        public JsonMediaTypeFormatter JsonFormatter
        {
            get
            {
                return this.Items.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeFormatter"/> to use for <see cref="System.Json.JsonValue"/>.
        /// </summary>
        public JsonValueMediaTypeFormatter JsonValueFormatter
        {
            get
            {
                return this.Items.OfType<JsonValueMediaTypeFormatter>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaTypeFormatter"/> to use for <c>application/x-www-form-urlencoded</c> data.
        /// </summary>
        public FormUrlEncodedMediaTypeFormatter FormUrlEncodedFormatter
        {
            get
            {
                return this.Items.OfType<FormUrlEncodedMediaTypeFormatter>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Creates a collection of new instances of the default <see cref="MediaTypeFormatter"/>s.
        /// </summary>
        /// <returns>The collection of default <see cref="MediaTypeFormatter"/> instances.</returns>
        private static IEnumerable<MediaTypeFormatter> CreateDefaultFormatters()
        {
            return new MediaTypeFormatter[]
            {
                new XmlMediaTypeFormatter(),
                new JsonValueMediaTypeFormatter(),
                new JsonMediaTypeFormatter(),
                new FormUrlEncodedMediaTypeFormatter()
            };
        }

        private void VerifyAndSetFormatters(IEnumerable<MediaTypeFormatter> formatters)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException("formatters");
            }

            foreach (MediaTypeFormatter formatter in formatters)
            {
                if (formatter == null)
                {
                    throw new ArgumentException(SR.CannotHaveNullInList(mediaTypeFormatterType.Name), "formatters");
                }

                this.Add(formatter);
            }
        }
    }
}
