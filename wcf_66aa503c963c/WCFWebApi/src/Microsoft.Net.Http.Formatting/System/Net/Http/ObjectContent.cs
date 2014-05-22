// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Derived <see cref="HttpContent"/> class that contains a strongly typed object.
    /// </summary>
    public class ObjectContent : HttpContent
    {
        private const string HeadersContentTypeName = "Headers.ContentType";

        private static readonly Type ObjectContentType = typeof(ObjectContent);
        private static readonly Type HttpContentType = typeof(HttpContent);
        private static readonly Type MediaTypeHeaderValueType = typeof(MediaTypeHeaderValue);
        private static readonly Type MediaTypeFormatterType = typeof(MediaTypeFormatter);
        private static readonly Type NullableType = typeof(Nullable<>);

        private MediaTypeFormatterCollection formatters;
        private HttpRequestMessage requestMessage;
        private HttpResponseMessage responseMessage;
        private object defaultValue;
        private MediaTypeFormatter defaultFormatter;
        private MediaTypeFormatter selectedWriteFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        public ObjectContent(Type type, object value)
            : this(type)
        {
            this.VerifyAndSetObject(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        public ObjectContent(Type type, object value, string mediaType)
            : this(type)
        {
            this.VerifyAndSetObjectAndMediaType(value, mediaType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        public ObjectContent(Type type, object value, MediaTypeHeaderValue mediaType)
            : this(type)
        {
            this.VerifyAndSetObjectAndMediaType(value, mediaType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        public ObjectContent(Type type, HttpContent content)
            : this(type)
        {
            this.VerifyAndSetHttpContent(content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to use for serialization or deserialization.</param>
        public ObjectContent(Type type, object value, IEnumerable<MediaTypeFormatter> formatters)
            : this(type, value)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException("formatters");
            }

            this.formatters = new MediaTypeFormatterCollection(formatters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to use for serialization or deserialization.</param>
        public ObjectContent(Type type, object value, string mediaType, IEnumerable<MediaTypeFormatter> formatters)
            : this(type, value, mediaType)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException("formatters");
            }

            this.formatters = new MediaTypeFormatterCollection(formatters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to use for serialization or deserialization.</param>
        public ObjectContent(Type type, object value, MediaTypeHeaderValue mediaType, IEnumerable<MediaTypeFormatter> formatters)
            : this(type, value, mediaType)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException("formatters");
            }

            this.formatters = new MediaTypeFormatterCollection(formatters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent"/> class.
        /// </summary>
        /// <param name="type">The type of object this instance will contain.</param>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to use for serialization or deserialization.</param>
        public ObjectContent(Type type, HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
            : this(type, content)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException("formatters");
            }

            this.formatters = new MediaTypeFormatterCollection(formatters);
        }

        private ObjectContent(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (HttpContentType.IsAssignableFrom(type))
            {
                throw new ArgumentException(SR.CannotUseThisParameterType(HttpContentType.Name, ObjectContentType.Name), "type");
            }

            this.ObjectType = type;
        }

        /// <summary>
        /// Gets the type of object managed by this <see cref="ObjectContent"/> instance.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the mutable collection of <see cref="MediaTypeFormatter"/> instances used to
        /// serialize or deserialize the value of this <see cref="ObjectContent"/>.
        /// </summary>
        public MediaTypeFormatterCollection Formatters
        {
            get
            {
                if (this.formatters == null)
                {
                    this.formatters = new MediaTypeFormatterCollection();
                }

                return this.formatters;
            }
        }

        internal MediaTypeFormatter DefaultFormatter
        {
            get
            {
                if (this.defaultFormatter == null)
                {
                    this.defaultFormatter = this.Formatters.XmlFormatter;
                    if (this.defaultFormatter == null)
                    {
                        this.defaultFormatter = this.Formatters.JsonFormatter;
                    }
                }

                return this.defaultFormatter;
            }

            set
            {
                this.defaultFormatter = value;
            }
        }

        internal HttpRequestMessage HttpRequestMessage
        {
            get
            {
                return this.requestMessage != null && object.ReferenceEquals(this.requestMessage.Content, this)
                        ? this.requestMessage
                        : null;
            }

            set
            {
                this.requestMessage = value;

                // Pairing to a request unpairs from response
                if (value != null)
                {
                    this.HttpResponseMessage = null;
                }
            }
        }

        internal HttpResponseMessage HttpResponseMessage
        {
            get
            {
                return this.responseMessage != null && object.ReferenceEquals(this.responseMessage.Content, this)
                        ? this.responseMessage
                        : null;
            }

            set
            {
                this.responseMessage = value;

                // pairing to a response unpairs from a request
                if (value != null)
                {
                    this.HttpRequestMessage = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the inner <see cref="HttpContent"/> wrapped by
        /// by the current <see cref="ObjectContent"/>.
        /// </summary>
        protected HttpContent HttpContent { get; set; }

        /// <summary>
        /// Gets or sets the value of the current <see cref="ObjectContent"/>.
        /// </summary>
        protected object Value { get; set; }

        private object DefaultValue
        {
            get
            {
                if (this.defaultValue == null)
                {
                    this.defaultValue = GetDefaultValueForType(this.ObjectType);
                }

                return this.defaultValue;
            }
        }

        private bool IsValueCached
        {
            get
            {
                return this.HttpContent == null;
            }
        }

        private MediaTypeHeaderValue MediaType
        {
            get
            {
                return this.Headers.ContentType;
            }

            set
            {
                this.Headers.ContentType = value;
            }
        }

        /// <summary>
        /// Asynchronously returns the object instance for this <see cref="ObjectContent"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance that will yield the object instance.</returns>
        public Task<object> ReadAsAsync()
        {
            return this.ReadAsAsyncInternal(allowDefaultIfNoFormatter: false);
        }

        /// <summary>
        /// Asynchronously returns the object instance for this <see cref="ObjectContent"/>
        /// or the default value for the type if content is not available.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance that will yield the object instance.</returns>
        public Task<object> ReadAsOrDefaultAsync()
        {
            return this.ReadAsAsyncInternal(allowDefaultIfNoFormatter: true);
        }

        /// <summary>
        /// Forces selection of the write <see cref="MediaTypeFormatter"/> and content-type.
        /// </summary>
        internal void DetermineWriteSerializerAndContentType()
        {
            this.selectedWriteFormatter = this.SelectAndValidateWriteFormatter();
        }

        internal void SetFormatters(IEnumerable<MediaTypeFormatter> list)
        {
            this.formatters = new MediaTypeFormatterCollection(list);
        }

        /// <summary>
        /// Asynchronously serializes the object's content to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="context">The associated <see cref="TransportContext"/>.</param>
        /// <returns>A <see cref="Task"/> instance that is asynchronously serializing the object's content.</returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return this.WriteToStreamAsyncInternal(stream, context);
        }

        /// <summary>
        /// Asynchronously creates the content read stream.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance that will yield a stream intended for reading.</returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            if (this.HttpContent != null)
            {
                return this.HttpContent.ReadAsStreamAsync();
            }
            else
            {
                return base.CreateContentReadStreamAsync();
            }
        }

        /// <summary>
        /// Computes the length of the stream if possible.
        /// </summary>
        /// <param name="length">The computed length of the stream.</param>
        /// <returns><c>true</c> if the length has been computed; otherwise <c>false</c>.</returns>
        protected override bool TryComputeLength(out long length)
        {
            HttpContent httpContent = this.HttpContent;
            if (httpContent != null)
            {
                long? contentLength = httpContent.Headers.ContentLength;
                if (contentLength.HasValue)
                {
                    length = contentLength.Value;
                    return true;
                }
            }

            length = -1;
            return false;
        }

        /// <summary>
        /// Selects the appropriate <see cref="MediaTypeFormatter"/> to read the object content.
        /// </summary>
        /// <returns>The selected <see cref="MediaTypeFormatter"/> or null.</returns>
        protected MediaTypeFormatter SelectReadFormatter()
        {
            HttpRequestMessage request = this.HttpRequestMessage;
            HttpResponseMessage response = this.HttpResponseMessage;
            Type type = this.ObjectType;

            if (request != null)
            {
                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    if (formatter.CanReadAs(type, request))
                    {
                        return formatter;
                    }
                }
            }
            else if (response != null)
            {
                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    if (formatter.CanReadAs(type, response))
                    {
                        return formatter;
                    }
                }
            }
            else
            {
                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    if (formatter.CanReadAs(type, this))
                    {
                        return formatter;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Selects the appropriate <see cref="MediaTypeFormatter"/> to write the object content.
        /// </summary>
        /// <param name="mediaType">The <see cref="MediaTypeHeaderValue"/> to use to describe the object's content type.</param>
        /// <returns>The selected <see cref="MediaTypeFormatter"/> or null.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Out parameter is fine here.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is acceptable for now.  A larger refactor is planned.")]
        protected MediaTypeFormatter SelectWriteFormatter(out MediaTypeHeaderValue mediaType)
        {
            mediaType = null;

            // We are paired with a request, or a response, or neither.
            HttpRequestMessage request = this.HttpRequestMessage;
            HttpResponseMessage response = this.HttpResponseMessage;
            Type type = this.ObjectType;

            if (request != null)
            {
                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    if (formatter.CanWriteAs(type, request, out mediaType))
                    {
                        return formatter;
                    }
                }
            }
            else if (response != null)
            {
                MediaTypeFormatter formatterMatchOnType = null;
                ResponseMediaTypeMatch mediaTypeMatchOnType = null;

                MediaTypeFormatter formatterMatchOnAcceptHeader = null;
                ResponseMediaTypeMatch mediaTypeMatchOnAcceptHeader = null;

                MediaTypeFormatter formatterMatchOnAcceptHeaderWithMapping = null;
                ResponseMediaTypeMatch mediaTypeMatchOnAcceptHeaderWithMapping = null;

                MediaTypeFormatter formatterMatchOnRequestContentType = null;
                ResponseMediaTypeMatch mediaTypeMatchOnRequestContentType = null;

                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(type, response);
                    if (match == null)
                    {
                        // Null signifies no match
                        continue;
                    }

                    ResponseFormatterSelectionResult matchResult = match.ResponseFormatterSelectionResult;
                    switch (matchResult)
                    {
                        case ResponseFormatterSelectionResult.MatchOnCanWriteType:

                            // First match by type trumps all other type matches
                            if (formatterMatchOnType == null)
                            {
                                formatterMatchOnType = formatter;
                                mediaTypeMatchOnType = match;
                            }

                            break;

                        case ResponseFormatterSelectionResult.MatchOnResponseContentType:

                            // Match on response content trumps all other choices
                            return formatter;

                        case ResponseFormatterSelectionResult.MatchOnRequestAcceptHeader:

                            // Matches on accept headers must choose the highest quality match
                            double thisQuality = match.MediaTypeMatch.Quality;
                            if (formatterMatchOnAcceptHeader != null)
                            {
                                double bestQualitySeen = mediaTypeMatchOnAcceptHeader.MediaTypeMatch.Quality;
                                if (thisQuality <= bestQualitySeen)
                                {
                                    continue;
                                }
                            }

                            formatterMatchOnAcceptHeader = formatter;
                            mediaTypeMatchOnAcceptHeader = match;

                            break;

                        case ResponseFormatterSelectionResult.MatchOnRequestAcceptHeaderWithMediaTypeMapping:

                            // Matches on accept headers using mappings must choose the highest quality match
                            double thisMappingQuality = match.MediaTypeMatch.Quality;
                            if (mediaTypeMatchOnAcceptHeaderWithMapping != null)
                            {
                                double bestMappingQualitySeen = mediaTypeMatchOnAcceptHeaderWithMapping.MediaTypeMatch.Quality;
                                if (thisMappingQuality <= bestMappingQualitySeen)
                                {
                                    continue;
                                }
                            }

                            formatterMatchOnAcceptHeaderWithMapping = formatter;
                            mediaTypeMatchOnAcceptHeaderWithMapping = match;

                            break;

                        case ResponseFormatterSelectionResult.MatchOnRequestContentType:

                            // First match on request content type trumps other request content matches
                            if (formatterMatchOnRequestContentType == null)
                            {
                                formatterMatchOnRequestContentType = formatter;
                                mediaTypeMatchOnRequestContentType = match;
                            }

                            break;
                    }
                }

                // If we received matches based on both supported media types and from media type mappings,
                // we want to give precedence to the media type mappings, but only if their quality is >= that of the supported media type.
                // We do this because media type mappings are the user's extensibility point and must take precedence over normal
                // supported media types in the case of a tie.   The 99% case is where both have quality 1.0.
                if (mediaTypeMatchOnAcceptHeaderWithMapping != null && mediaTypeMatchOnAcceptHeader != null)
                {
                    if (mediaTypeMatchOnAcceptHeader.MediaTypeMatch.Quality > mediaTypeMatchOnAcceptHeaderWithMapping.MediaTypeMatch.Quality)
                    {
                        formatterMatchOnAcceptHeaderWithMapping = null;
                    }
                }

                // now select the formatter and media type
                // A MediaTypeMapping is highest precedence -- it is an extensibility point
                // allowing the user to override normal accept header matching
                if (formatterMatchOnAcceptHeaderWithMapping != null)
                {
                    mediaType = mediaTypeMatchOnAcceptHeaderWithMapping.MediaTypeMatch.MediaType;
                    return formatterMatchOnAcceptHeaderWithMapping;
                }
                else if (formatterMatchOnAcceptHeader != null)
                {
                    mediaType = mediaTypeMatchOnAcceptHeader.MediaTypeMatch.MediaType;
                    return formatterMatchOnAcceptHeader;
                }
                else if (formatterMatchOnRequestContentType != null)
                {
                    mediaType = mediaTypeMatchOnRequestContentType.MediaTypeMatch.MediaType;
                    return formatterMatchOnRequestContentType;
                }
                else if (formatterMatchOnType != null)
                {
                    mediaType = mediaTypeMatchOnType.MediaTypeMatch.MediaType;
                    return formatterMatchOnType;
                }
            }
            else
            {
                foreach (MediaTypeFormatter formatter in this.Formatters)
                {
                    if (formatter.CanWriteAs(type, this, out mediaType))
                    {
                        return formatter;
                    }
                }
            }

            mediaType = null;
            return null;
        }

        /// <summary>
        /// Determines if the given <paramref name="value"/> is an instance of
        /// <see cref="HttpContent"/> or is some type we automatically wrap inside
        /// <see cref="HttpContent"/>.
        /// </summary>
        /// <param name="value">The object value to test.</param>
        /// <returns>A non-null <see cref="HttpContent"/> if the <paramref name="value"/>
        /// was an instance of <see cref="HttpContent"/> or needed to be wrapped
        /// inside one.  A <c>null</c> indicates the <paramref name="value"/> is not
        /// <see cref="HttpContent"/> or needed to be wrapped in one.</returns>
        private static HttpContent WrapOrCastAsHttpContent(object value)
        {
            Stream stream = value as Stream;
            return stream == null ? value as HttpContent : new StreamContent(stream);
        }

        private static object GetDefaultValueForType(Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }

            if (type.IsEnum)
            {
                Array enumValues = Enum.GetValues(type);
                if (enumValues.Length > 0)
                {
                    return enumValues.GetValue(0);
                }
            }

            return Activator.CreateInstance(type);
        }

        private static bool IsTypeNullable(Type type)
        {
            return !type.IsValueType ||
                    (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == NullableType);
        }

        private void CacheValueAndDisposeWrappedHttpContent(object value)
        {
            this.Value = value;

            if (this.HttpContent != null)
            {
                this.HttpContent.Dispose();
                this.HttpContent = null;
            }

            Contract.Assert(this.IsValueCached, "IsValueCached must be true.");
        }

        private object ReadAsInternal(bool allowDefaultIfNoFormatter)
        {
            if (this.IsValueCached)
            {
                return this.Value;
            }

            object value;
            HttpContent httpContent = this.HttpContent;

            Contract.Assert(httpContent.Headers != null, "HttpContent headers are never null.");
            if (httpContent.Headers.ContentLength == 0)
            {
                value = this.DefaultValue;
            }
            else
            {
                MediaTypeFormatter formatter = this.SelectAndValidateReadFormatter(acceptNullFormatter: allowDefaultIfNoFormatter);
                if (formatter == null)
                {
                    Contract.Assert(allowDefaultIfNoFormatter, "allowDefaultIfNoFormatter should always be true here.");
                    value = this.DefaultValue;
                }
                else
                {
                    // Delegate to the wrapped HttpContent for the stream
                    value = formatter.ReadFromStream(this.ObjectType, httpContent.ReadAsStreamAsync().Result, this.Headers);
                }
            }

            this.CacheValueAndDisposeWrappedHttpContent(value);
            return value;
        }

        private Task<object> ReadAsAsyncInternal(bool allowDefaultIfNoFormatter)
        {
            if (this.IsValueCached)
            {
                return Task.Factory.StartNew<object>(() => this.Value);
            }

            HttpContent httpContent = this.HttpContent;

            Contract.Assert(httpContent.Headers != null, "HttpContent headers are never null.");
            if (httpContent.Headers.ContentLength == 0)
            {
                object defaultValue = this.DefaultValue;
                this.CacheValueAndDisposeWrappedHttpContent(defaultValue);
                return Task.Factory.StartNew<object>(() => defaultValue);
            }

            MediaTypeFormatter formatter = this.SelectAndValidateReadFormatter(acceptNullFormatter: allowDefaultIfNoFormatter);
            if (formatter == null)
            {
                Contract.Assert(allowDefaultIfNoFormatter, "allowDefaultIfNoFormatter should always be true here.");
                object defaultValue = this.DefaultValue;
                this.CacheValueAndDisposeWrappedHttpContent(defaultValue);
                return Task.Factory.StartNew<object>(() => defaultValue);
            }

            // If we wrap an HttpContent, delegate to its stream..
            return formatter.ReadFromStreamAsync(
                this.ObjectType,
                httpContent.ReadAsStreamAsync().Result,
                this.Headers)
                    .ContinueWith<object>((task) =>
                    {
                        object value = task.Result;
                        this.CacheValueAndDisposeWrappedHttpContent(value);
                        return value;
                    });
        }

        private MediaTypeFormatter SelectAndValidateReadFormatter(bool acceptNullFormatter)
        {
            MediaTypeFormatter formatter = this.SelectReadFormatter();
            if (formatter == null)
            {
                if (!acceptNullFormatter)
                {
                    MediaTypeHeaderValue mediaType = this.Headers.ContentType;
                    string mediaTypeAsString = mediaType != null ? mediaType.MediaType : SR.UndefinedMediaType;
                    throw new InvalidOperationException(
                        SR.NoReadSerializerAvailable(MediaTypeFormatterType.Name, this.ObjectType.Name, mediaTypeAsString));
                }
            }

            return formatter;
        }

        private Task WriteToStreamAsyncInternal(Stream stream, TransportContext context)
        {
            if (this.HttpContent != null)
            {
                return this.HttpContent.CopyToAsync(stream, context);
            }

            MediaTypeFormatter formatter = this.selectedWriteFormatter ?? this.SelectAndValidateWriteFormatter();
            return formatter.WriteToStreamAsync(this.ObjectType, this.Value, stream, this.Headers, context);
        }

        private MediaTypeFormatter SelectAndValidateWriteFormatter()
        {
            MediaTypeHeaderValue mediaType = null;
            MediaTypeFormatter formatter = this.SelectWriteFormatter(out mediaType);

            if (formatter == null)
            {
                if (this.DefaultFormatter != null &&
                    this.DefaultFormatter.SupportedMediaTypes.Count > 0)
                {
                    formatter = this.DefaultFormatter;
                    mediaType = this.DefaultFormatter.SupportedMediaTypes[0];
                }
                else
                {
                    string errorMessage = this.MediaType == null
                                            ? SR.MediaTypeMustBeSetBeforeWrite(HeadersContentTypeName, ObjectContentType.Name)
                                            : SR.NoWriteSerializerAvailable(MediaTypeFormatterType.Name, this.ObjectType.Name, this.MediaType.ToString());
                    throw new InvalidOperationException(errorMessage);
                }
            }

            // Update our MediaType based on what the formatter said it would produce
            // TODO: 228498 MediaType should provide an extensibility point here so that we could avoid special casing 
            // ODataMediaTypeFormatter here
            if (mediaType != null)
            {
                IEnumerable<KeyValuePair<string, string>> headers = formatter.GetResponseHeaders(this.ObjectType, mediaType.MediaType, this.responseMessage);
                if (headers == null)
                {
                    this.MediaType = mediaType;
                }
                else
                {
                    // we need to set the content type based on the headers set by the serializer
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        this.Headers.AddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            return formatter;
        }

        private void VerifyAndSetObject(object value)
        {
            Contract.Assert(this.ObjectType != null, "this.Type cannot be null");

            if (value == null)
            {
                // Null may not be assigned to value types (unless Nullable<T>)
                // We allow an ObjectContent of type void and value null as a special case
                if (this.ObjectType != typeof(void) && !IsTypeNullable(this.ObjectType))
                {
                    throw new InvalidOperationException(SR.CannotUseNullValueType(ObjectContentType.Name, this.ObjectType.Name));
                }
            }
            else
            {
                // It is possible to pass HttpContent as object and arrive at this
                // code path.  Detect and redirect.
                HttpContent objAsHttpContent = WrapOrCastAsHttpContent(value);
                if (objAsHttpContent != null)
                {
                    this.VerifyAndSetHttpContent(objAsHttpContent);
                    return;
                }
                else
                {
                    // Non-null objects must be a type assignable to this.Type
                    Type objectType = value.GetType();
                    if (!this.ObjectType.IsAssignableFrom(objectType))
                    {
                        throw new ArgumentException(
                                SR.ObjectAndTypeDisagree(objectType.Name, this.ObjectType.Name),
                                "value");
                    }
                }
            }

            this.Value = value;
        }

        private void VerifyAndSetObjectAndMediaType(object value, MediaTypeHeaderValue mediaType)
        {
            Contract.Assert(this.ObjectType != null, "this.ObjectType cannot be null");

            // It is possible to pass HttpContent as object and arrive at this
            // code path.  Detect and redirect.  We do not use the media type
            // specified unless the given HttpContent's media type is null.
            HttpContent objAsHttpContent = WrapOrCastAsHttpContent(value);
            if (objAsHttpContent != null)
            {
                this.VerifyAndSetHttpContent(objAsHttpContent);
                if (objAsHttpContent.Headers.ContentType == null)
                {
                    this.VerifyAndSetMediaType(mediaType);
                }
            }
            else
            {
                this.VerifyAndSetObject(value);
                this.VerifyAndSetMediaType(mediaType);
            }
        }

        private void VerifyAndSetObjectAndMediaType(object value, string mediaType)
        {
            Contract.Assert(this.ObjectType != null, "this.ObjectType cannot be null");

            // It is possible to pass HttpContent as object and arrive at this
            // code path.  Detect and redirect.  We do not use the media type
            // specified unless the given HttpContent's media type is null.
            HttpContent objAsHttpContent = WrapOrCastAsHttpContent(value);

            if (objAsHttpContent != null)
            {
                this.VerifyAndSetHttpContent(objAsHttpContent);
                if (objAsHttpContent.Headers.ContentType == null)
                {
                    this.VerifyAndSetMediaType(mediaType);
                }
            }
            else
            {
                this.VerifyAndSetObject(value);
                this.VerifyAndSetMediaType(mediaType);
            }
        }

        private void VerifyAndSetHttpContent(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this.HttpContent = content;
            content.Headers.CopyTo(this.Headers);
        }

        private void VerifyAndSetMediaType(MediaTypeHeaderValue mediaType)
        {
            if (mediaType == null)
            {
                throw new ArgumentNullException("mediaType");
            }

            if (mediaType.IsMediaRange())
            {
                throw new InvalidOperationException(
                        SR.MediaTypeCanNotBeMediaRange(mediaType.MediaType));
            }

            this.MediaType = mediaType;
        }

        private void VerifyAndSetMediaType(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            MediaTypeHeaderValue mediaTypeHeaderValue = null;
            try
            {
                mediaTypeHeaderValue = new MediaTypeHeaderValue(mediaType);
            }
            catch (FormatException formatException)
            {
                throw new ArgumentException(
                    SR.InvalidMediaType(mediaType, MediaTypeHeaderValueType.Name),
                    "mediaType",
                    formatException);
            }

            this.VerifyAndSetMediaType(mediaTypeHeaderValue);
        }
    }

    /// <summary>
    /// Generic form of <see cref="ObjectContent"/>.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="ObjectContent"/> class will contain.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Class contains generic forms")]
    public class ObjectContent<T> : ObjectContent
    {
        private static readonly Type MediaTypeFormatterType = typeof(MediaTypeFormatter);

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        public ObjectContent(T value)
            : base(typeof(T), value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        public ObjectContent(T value, string mediaType)
            : base(typeof(T), value, mediaType)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        public ObjectContent(T value, MediaTypeHeaderValue mediaType)
            : base(typeof(T), value, mediaType)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        public ObjectContent(HttpContent content)
            : base(typeof(T), content)
        {
            this.HttpContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to serialize or deserialize the object content.</param>
        public ObjectContent(T value, IEnumerable<MediaTypeFormatter> formatters)
            : base(typeof(T), value, formatters)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to serialize or deserialize the object content.</param>
        public ObjectContent(T value, string mediaType, IEnumerable<MediaTypeFormatter> formatters)
            : base(typeof(T), value, mediaType, formatters)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="mediaType">The media type to associate with this object.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to serialize or deserialize the object content.</param>
        public ObjectContent(T value, MediaTypeHeaderValue mediaType, IEnumerable<MediaTypeFormatter> formatters)
            : base(typeof(T), value, mediaType, formatters)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances
        /// to serialize or deserialize the object content.</param>
        public ObjectContent(HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
            : base(typeof(T), content, formatters)
        {
            this.HttpContent = content;
        }

        private MediaTypeHeaderValue MediaType
        {
            get
            {
                return this.Headers.ContentType;
            }

            set
            {
                this.Headers.ContentType = value;
            }
        }

        private bool IsValueCached
        {
            get
            {
                return this.HttpContent == null;
            }
        }

        /// <summary>
        /// Returns a <see cref="Task"/> instance to yield the object instance for this <see cref="ObjectContent"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that will yield the object instance.</returns>
        public new Task<T> ReadAsAsync()
        {
            return this.ReadAsAsyncInternal(allowDefaultIfNoFormatter: false);
        }

        /// <summary>
        /// Returns a <see cref="Task"/> instance to yield the object instance for this <see cref="ObjectContent"/>
        /// or the default value for the type if content is not available.
        /// </summary>
        /// <returns>A <see cref="Task"/> that will yield the object instance.</returns>
        public new Task<T> ReadAsOrDefaultAsync()
        {
            return this.ReadAsAsyncInternal(allowDefaultIfNoFormatter: true);
        }

        private void CacheValueAndDisposeWrappedHttpContent(T value)
        {
            this.Value = value;

            if (this.HttpContent != null)
            {
                this.HttpContent.Dispose();
                this.HttpContent = null;
            }

            Contract.Assert(this.IsValueCached, "IsValueCached must be true.");
        }

        private Task<T> ReadAsAsyncInternal(bool allowDefaultIfNoFormatter)
        {
            if (this.IsValueCached)
            {
                return Task.Factory.StartNew<T>(() => (T)this.Value);
            }

            HttpContent httpContent = this.HttpContent;

            Contract.Assert(httpContent.Headers != null, "HttpContent headers are never null.");
            if (httpContent.Headers.ContentLength == 0)
            {
                T defaultValue = default(T);
                this.CacheValueAndDisposeWrappedHttpContent(defaultValue);
                return Task.Factory.StartNew<T>(() => defaultValue);
            }

            MediaTypeFormatter formatter = this.SelectAndValidateReadFormatter(acceptNullFormatter: allowDefaultIfNoFormatter);
            if (formatter == null)
            {
                Contract.Assert(allowDefaultIfNoFormatter, "allowDefaultIfNoFormatter must be true to execute this code path.");
                T defaultValue = default(T);
                this.CacheValueAndDisposeWrappedHttpContent(defaultValue);
                return Task.Factory.StartNew<T>(() => defaultValue);
            }

            // If we wrap an HttpContent, delegate to its stream.
            return formatter.ReadFromStreamAsync(
                    this.ObjectType,
                    httpContent.ReadAsStreamAsync().Result,
                    this.Headers)
                        .ContinueWith<T>((task) =>
                            {
                                T value = (T)task.Result;
                                this.CacheValueAndDisposeWrappedHttpContent(value);
                                return value;
                            });
        }

        private MediaTypeFormatter SelectAndValidateReadFormatter(bool acceptNullFormatter)
        {
            MediaTypeFormatter formatter = this.SelectReadFormatter();

            if (formatter == null)
            {
                if (!acceptNullFormatter)
                {
                    MediaTypeHeaderValue mediaType = this.Headers.ContentType;
                    string mediaTypeAsString = mediaType != null ? mediaType.MediaType : SR.UndefinedMediaType;
                    throw new InvalidOperationException(
                        SR.NoReadSerializerAvailable(MediaTypeFormatterType.Name, this.ObjectType.Name, mediaTypeAsString));
                }
            }

            return formatter;
        }
    }
}
