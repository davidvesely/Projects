// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Json;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class for handling HTML form URL-ended data, also known as <c>application/x-www-form-urlencoded</c>. 
    /// </summary>
    public class FormUrlEncodedMediaTypeFormatter : MediaTypeFormatter
    {
        private const int MinBufferSize = 256;
        private const int DefaultBufferSize = 32 * 1024;

        private static readonly Type FormUrlEncodedMediaTypeFormatterType = typeof(FormUrlEncodedMediaTypeFormatter);
        private static readonly MediaTypeHeaderValue[] supportedMediaTypes = new MediaTypeHeaderValue[]
        {
            MediaTypeConstants.ApplicationFormUrlEncodedMediaType
        };

        private bool isModified;
        private int readBufferSize = DefaultBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormUrlEncodedMediaTypeFormatter"/> class.
        /// </summary>
        public FormUrlEncodedMediaTypeFormatter()
        {
            foreach (MediaTypeHeaderValue value in supportedMediaTypes)
            {
                this.SupportedMediaTypes.Add(value);
            }
        }

        /// <summary>
        /// Gets the default media type for HTML Form URL encoded data, namely <c>application/x-www-form-urlencoded</c>.
        /// </summary>
        /// <value>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return MediaTypeConstants.ApplicationFormUrlEncodedMediaType;
            }
        }

        /// <summary>
        /// Gets or sets the size of the buffer when reading the incoming stream.
        /// </summary>
        /// <value>
        /// The size of the read buffer.
        /// </value>
        public int ReadBufferSize
        {
            get
            {
                return this.readBufferSize;
            }

            set
            {
                if (value < MinBufferSize)
                {
                    throw new ArgumentException(SR.MinParameterSize(MinBufferSize), "value");
                }

                this.isModified = true;
                this.readBufferSize = value;
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
                    this.MediaTypeMappings.Count > 0 ||
                    !FormattingUtilities.ValidateCollection(this.SupportedMediaTypes, supportedMediaTypes) ||
                    this.GetType().IsSubclassOf(FormUrlEncodedMediaTypeFormatterType);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="FormUrlEncodedMediaTypeFormatter"/> can read objects
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

            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="FormUrlEncodedMediaTypeFormatter"/> can write objects
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

            return false;
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

            IEnumerable<Tuple<string, string>> nameValuePairs = ReadFormUrlEncoded(stream, this.ReadBufferSize);
            JsonObject jsonObject = FormUrlEncodedJson.Parse(nameValuePairs);
            return FormattingUtilities.IsJsonValueType(type) ? jsonObject : jsonObject.ReadAsType(type);
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
            throw new NotImplementedException(SR.MediaTypeFormatterWriteUnsupported(FormUrlEncodedMediaTypeFormatterType));
        }

        /// <summary>
        /// Reads all name-value pairs encoded as HTML Form URL encoded data and add them to 
        /// a collection as UNescaped URI strings.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="bufferSize">Size of the buffer used to read the contents.</param>
        /// <returns>Collection of name-value pairs.</returns>
        private static IEnumerable<Tuple<string, string>> ReadFormUrlEncoded(Stream input, int bufferSize)
        {
            Contract.Assert(input != null, "input stream cannot be null");
            Contract.Assert(bufferSize >= MinBufferSize, "buffer size cannot be less than MinBufferSize");

            byte[] data = new byte[bufferSize];
            int bytesConsumed = 0;
            int bytesRead;
            bool isFinal = false;
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            FormUrlEncodedParser parser = new FormUrlEncodedParser(result, long.MaxValue);
            ParserState state;

            while (true)
            {
                try
                {
                    bytesRead = input.Read(data, 0, data.Length);
                    if (bytesRead == 0)
                    {
                        isFinal = true;
                    }
                }
                catch (Exception e)
                {
                    throw new IOException(SR.ErrorReadingFormUrlEncodedStream, e);
                }

                state = parser.ParseBuffer(data, bytesRead, ref bytesConsumed, isFinal);
                if (state != ParserState.NeedMoreData && state != ParserState.Done)
                {
                    throw new IOException(SR.FormUrlEncodedParseError(bytesConsumed));
                }

                if (isFinal)
                {
                    return result;
                }
            }
        }
    }
}