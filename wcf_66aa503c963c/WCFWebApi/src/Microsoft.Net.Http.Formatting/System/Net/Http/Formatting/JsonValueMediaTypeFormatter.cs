// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Json;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;

    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle <see cref="JsonValue"/>.
    /// </summary>
    public class JsonValueMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly Type JsonValueMediaTypeFormatterType = typeof(JsonValueMediaTypeFormatter);

        private static readonly MediaTypeHeaderValue[] supportedMediaTypes = new MediaTypeHeaderValue[]
        {
            MediaTypeConstants.ApplicationJsonMediaType,
            MediaTypeConstants.TextJsonMediaType
        };

        private bool isModified;
        private JsonSaveOptions jsonWriteOptions = JsonSaveOptions.None;
        private RequestHeaderMapping requestHeaderMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonValueMediaTypeFormatter"/> class.
        /// </summary>
        public JsonValueMediaTypeFormatter()
        {
            this.Encoding = Encoding.UTF8;
            foreach (MediaTypeHeaderValue value in supportedMediaTypes)
            {
                this.SupportedMediaTypes.Add(value);
            }

            this.requestHeaderMapping = new RequestHeaderMapping(
                FormattingUtilities.HttpRequestedWithHeader,
                FormattingUtilities.HttpRequestedWithHeaderValue,
                StringComparison.OrdinalIgnoreCase,
                true,
                MediaTypeConstants.ApplicationJsonMediaType);
            this.MediaTypeMappings.Add(this.requestHeaderMapping);
        }

        /// <summary>
        /// Gets the default media type for <see cref="JsonValue"/>, namely "application/json".
        /// </summary>
        /// <value>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return MediaTypeConstants.ApplicationJsonMediaType;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="JsonSaveOptions"/> to use when writing out the contents.
        /// </summary>
        /// <value>
        /// The <see cref="JsonSaveOptions"/> options.
        /// </value>
        public JsonSaveOptions JsonWriteOptions
        {
            get
            {
                return this.jsonWriteOptions;
            }

            set
            {
                JsonSaveOptionsHelper.Validate(value, "value");
                this.isModified = true;
                this.jsonWriteOptions = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is modified from the default settings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        internal override bool IsModified
        {
            get
            {
                return this.isModified ||
                    this.MediaTypeMappings.Count != 1 ||
                    this.MediaTypeMappings[0] != this.requestHeaderMapping ||
                    !FormattingUtilities.ValidateCollection(this.SupportedMediaTypes, supportedMediaTypes) ||
                    this.GetType().IsSubclassOf(JsonValueMediaTypeFormatterType);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="JsonValueMediaTypeFormatter"/> can read objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be read.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return FormattingUtilities.IsJsonValueType(type);
        }

        /// <summary>
        /// Determines whether this <see cref="JsonValueMediaTypeFormatter"/> can write objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be written.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return FormattingUtilities.IsJsonValueType(type);
        }

        /// <summary>
        /// Called during deserialization to read an object of the specified <paramref name="type"/>
        /// from the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="type">The type of object to read.</param>
        /// <param name="stream">The <see cref="Stream"/> from which to read.</param>
        /// <param name="contentHeaders">The content headers associated with the request or response.</param>
        /// <returns>The object instance that has been read.</returns>
        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            return JsonValue.Load(stream);
        }

        /// <summary>
        /// Called during serialization to write an object of the specified <paramref name="type"/>
        /// to the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="contentHeaders">The content headers associated with the request or response.</param>
        /// <param name="context">The <see cref="TransportContext"/>.</param>
        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (value != null)
            {
                JsonValue jsonValue = (JsonValue)value;
                jsonValue.Save(stream, this.jsonWriteOptions);
            }
        }
    }
}
