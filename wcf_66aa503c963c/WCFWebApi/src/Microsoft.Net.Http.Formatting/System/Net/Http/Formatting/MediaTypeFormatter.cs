// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class to handle serializing and deserializing strongly-typed objects using <see cref="ObjectContent"/>.
    /// </summary>
    public abstract class MediaTypeFormatter
    {
        private static ConcurrentDictionary<Type, Type> delegatingEnumerableCache = new ConcurrentDictionary<Type, Type>();
        private static ConcurrentDictionary<Type, ConstructorInfo> delegatingEnumerableConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeFormatter"/> class.
        /// </summary>
        protected MediaTypeFormatter()
        {
            this.SupportedMediaTypes = new MediaTypeHeaderValueCollection();
            this.MediaTypeMappings = new Collection<MediaTypeMapping>();
        }

        /// <summary>
        /// Gets the mutable collection of <see cref="MediaTypeHeaderValue"/> elements supported by
        /// this <see cref="MediaTypeFormatter"/> instance.
        /// </summary>
        public Collection<MediaTypeHeaderValue> SupportedMediaTypes { get; private set; }

        /// <summary>
        /// Gets the mutable collection of <see cref="MediaTypeMapping"/> elements used
        /// by this <see cref="MediaTypeFormatter"/> instance to determine the 
        /// <see cref="MediaTypeHeaderValue"/> of requests or responses.
        /// </summary>
        public Collection<MediaTypeMapping> MediaTypeMappings { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is modified from the default settings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        internal virtual bool IsModified
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to use when reading and writing data.
        /// </summary>
        /// <value>
        /// The <see cref="Encoding"/> to use when reading and writing data.
        /// </value>
        protected Encoding Encoding { get; set; }

        // If the type is IEnumerable<T> or an interface type implementing it, a DelegatingEnumerable<T> type is cached for use at serialization time.
        internal static bool TryGetDelegatingTypeForIEnumerableGenericOrSame(ref Type type)
        {
            if (type != null
             && type.IsInterface
             && type.IsGenericType
             && (type.GetInterface(FormattingUtilities.EnumerableInterfaceGenericType.FullName) != null
                 || type.GetGenericTypeDefinition().Equals(FormattingUtilities.EnumerableInterfaceGenericType)))
            {
                type = GetOrAddDelegatingType(type);
                return true;
            }

            return false;
        }

        // If the type is IQueryable<T> or an interface type implementing it, a DelegatingEnumerable<T> type is cached for use at serialization time.
        internal static bool TryGetDelegatingTypeForIQueryableGenericOrSame(ref Type type)
        {
            if (type != null
             && type.IsInterface
             && type.IsGenericType
             && (type.GetInterface(FormattingUtilities.QueryableInterfaceGenericType.FullName) != null
                 || type.GetGenericTypeDefinition().Equals(FormattingUtilities.QueryableInterfaceGenericType)))
            {
                type = GetOrAddDelegatingType(type);
                return true;
            }

            return false;
        }

        internal static ConstructorInfo GetTypeRemappingConstructor(Type type)
        {
            ConstructorInfo constructorInfo = null;
            delegatingEnumerableConstructorCache.TryGetValue(type, out constructorInfo);
            return constructorInfo;
        }

        internal bool CanReadAs(Type type, HttpContent content)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (!this.CanReadType(type))
            {
                return false;
            }

            // Content type must be set and must be supported
            MediaTypeHeaderValue mediaType = content.Headers.ContentType;
            MediaTypeMatch mediaTypeMatch = null;
            return mediaType == null ? false : this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch);
        }

        internal bool CanWriteAs(Type type, HttpContent content, out MediaTypeHeaderValue mediaType)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (!this.CanWriteType(type))
            {
                mediaType = null;
                return false;
            }

            // Content type must be set and must be supported
            mediaType = content.Headers.ContentType;
            MediaTypeMatch mediaTypeMatch = null;
            return mediaType != null && this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch);
        }

        internal bool CanReadAs(Type type, HttpRequestMessage request)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!this.CanReadType(type))
            {
                return false;
            }

            // Content type must be set and must be supported
            MediaTypeHeaderValue mediaType = request.Content.Headers.ContentType;
            MediaTypeMatch mediaTypeMatch = null;
            return mediaType != null && this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch);
        }

        internal bool CanWriteAs(Type type, HttpRequestMessage request, out MediaTypeHeaderValue mediaType)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            mediaType = null;

            if (!this.CanWriteType(type))
            {
                return false;
            }

            mediaType = request.Content.Headers.ContentType;
            MediaTypeMatch mediaTypeMatch = null;
            if (mediaType != null)
            {
                if (this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch))
                {
                    return true;
                }
            }
            else
            {
                if (this.TryMatchMediaTypeMapping(request, out mediaTypeMatch))
                {
                    return true;
                }
            }

            mediaType = null;
            return false;
        }

        internal bool CanReadAs(Type type, HttpResponseMessage response)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (!this.CanReadType(type))
            {
                return false;
            }

            // Content type must be set and must be supported
            MediaTypeHeaderValue mediaType = response.Content.Headers.ContentType;
            MediaTypeMatch mediaTypeMatch = null;
            return mediaType != null && this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch);
        }

        internal ResponseMediaTypeMatch SelectResponseMediaType(Type type, HttpResponseMessage response)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            MediaTypeHeaderValue mediaType = null;
            MediaTypeMatch mediaTypeMatch = null;

            if (!this.CanWriteType(type))
            {
                return null;
            }

            mediaType = response.Content == null ? null : response.Content.Headers.ContentType;
            if (mediaType != null && this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch))
            {
                return new ResponseMediaTypeMatch(
                            mediaTypeMatch, 
                            ResponseFormatterSelectionResult.MatchOnResponseContentType);
            }

            HttpRequestMessage request = response.RequestMessage;
            if (request != null)
            {
                IEnumerable<MediaTypeWithQualityHeaderValue> acceptHeaderMediaTypes = request.Headers.Accept.OrderBy((m) => m, MediaTypeHeaderValueComparer.Comparer);

                if (this.TryMatchSupportedMediaType(acceptHeaderMediaTypes, out mediaTypeMatch))
                {
                    return new ResponseMediaTypeMatch(
                                mediaTypeMatch, 
                                ResponseFormatterSelectionResult.MatchOnRequestAcceptHeader);
                }

                if (this.TryMatchMediaTypeMapping(response, out mediaTypeMatch))
                {
                    return new ResponseMediaTypeMatch(
                                mediaTypeMatch,
                                ResponseFormatterSelectionResult.MatchOnRequestAcceptHeaderWithMediaTypeMapping);
                }

                HttpContent requestContent = request.Content;
                if (requestContent != null)
                {
                    MediaTypeHeaderValue requestContentType = requestContent.Headers.ContentType;
                    if (requestContentType != null && this.TryMatchSupportedMediaType(requestContentType, out mediaTypeMatch))
                    {
                        return new ResponseMediaTypeMatch(
                                    mediaTypeMatch,
                                    ResponseFormatterSelectionResult.MatchOnRequestContentType);
                    }
                }
            }

            mediaType = this.SupportedMediaTypes.FirstOrDefault();
            if (mediaType != null && this.Encoding != null)
            {
                mediaType = (MediaTypeHeaderValue)((ICloneable)mediaType).Clone();
                mediaType.CharSet = this.Encoding.WebName;
            }

            return new ResponseMediaTypeMatch(
                            new MediaTypeMatch(mediaType), 
                            ResponseFormatterSelectionResult.MatchOnCanWriteType);
        }

        internal Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Contract.Assert(stream != null, "stream cannot be null.");
            Contract.Assert(contentHeaders != null, "contentHeaders cannot be null.");

            return this.OnReadFromStreamAsync(type, stream, contentHeaders);
        }

        internal Task WriteToStreamAsync(Type type, object instance, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Contract.Assert(stream != null, "stream cannot be null.");
            Contract.Assert(contentHeaders != null, "contentHeaders cannot be null.");

            return this.OnWriteToStreamAsync(type, instance, stream, contentHeaders, context);
        }

        internal object ReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Contract.Assert(stream != null, "stream cannot be null.");
            Contract.Assert(contentHeaders != null, "contentHeaders cannot be null.");

            // TODO: CSDMain 235646 Introduce new MediaTypeFormatter exception that should be thrown from MediaTypeFormatter.WriteToStream and MediaTypeFormatter.ReadFromStream
            return this.OnReadFromStream(type, stream, contentHeaders);
        }

        internal void WriteToStream(Type type, object instance, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Contract.Assert(stream != null, "stream cannot be null.");
            Contract.Assert(contentHeaders != null, "contentHeaders cannot be null.");

            // TODO: CSDMain 235646 Introduce new MediaTypeFormatter exception that should be thrown from MediaTypeFormatter.WriteToStream and MediaTypeFormatter.ReadFromStream
            this.OnWriteToStream(type, instance, stream, contentHeaders, context);
        }

        internal bool TryMatchSupportedMediaType(MediaTypeHeaderValue mediaType, out MediaTypeMatch mediaTypeMatch)
        {
            Contract.Assert(mediaType != null, "mediaType cannot be null.");

            foreach (MediaTypeHeaderValue supportedMediaType in this.SupportedMediaTypes)
            {
                if (MediaTypeHeaderValueEqualityComparer.EqualityComparer.Equals(supportedMediaType, mediaType))
                {
                    // If the incoming media type had an associated quality factor, propagate it to the match
                    MediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue = mediaType as MediaTypeWithQualityHeaderValue;
                    double quality = mediaTypeWithQualityHeaderValue != null && mediaTypeWithQualityHeaderValue.Quality.HasValue
                                        ? mediaTypeWithQualityHeaderValue.Quality.Value
                                        : MediaTypeMatch.Match;

                    MediaTypeHeaderValue effectiveMediaType = supportedMediaType;
                    if (this.Encoding != null)
                    {
                        effectiveMediaType = (MediaTypeHeaderValue)((ICloneable)supportedMediaType).Clone();
                        effectiveMediaType.CharSet = this.Encoding.WebName;
                    }

                    mediaTypeMatch = new MediaTypeMatch(effectiveMediaType, quality);
                    return true;
                }
            }

            mediaTypeMatch = null;
            return false;
        }

        internal bool TryMatchSupportedMediaType(IEnumerable<MediaTypeHeaderValue> mediaTypes, out MediaTypeMatch mediaTypeMatch)
        {
            Contract.Assert(mediaTypes != null, "mediaTypes cannot be null.");
            foreach (MediaTypeHeaderValue mediaType in mediaTypes)
            {
                if (this.TryMatchSupportedMediaType(mediaType, out mediaTypeMatch))
                {
                    return true;
                }
            }

            mediaTypeMatch = null;
            return false;
        }

        internal bool TryMatchMediaTypeMapping(HttpRequestMessage request, out MediaTypeMatch mediaTypeMatch)
        {
            Contract.Assert(request != null, "request cannot be null.");

            foreach (MediaTypeMapping mapping in this.MediaTypeMappings)
            {
                // Collection<T> is not protected against null, so avoid them
                double quality;
                if (mapping != null && ((quality = mapping.TryMatchMediaType(request)) > 0.0))
                {
                    mediaTypeMatch = new MediaTypeMatch(mapping.MediaType, quality);
                    return true;
                }
            }

            mediaTypeMatch = null;
            return false;
        }

        internal bool TryMatchMediaTypeMapping(HttpResponseMessage response, out MediaTypeMatch mediaTypeMatch)
        {
            Contract.Assert(response != null, "response cannot be null.");

            foreach (MediaTypeMapping mapping in this.MediaTypeMappings)
            {
                // Collection<T> is not protected against null, so avoid them
                double quality;
                if (mapping != null && ((quality = mapping.TryMatchMediaType(response)) > 0.0))
                {
                    mediaTypeMatch = new MediaTypeMatch(mapping.MediaType, quality);
                    return true;
                }
            }

            mediaTypeMatch = null;
            return false;
        }

        internal IEnumerable<KeyValuePair<string, string>> GetResponseHeaders(Type objectType, string mediaType, HttpResponseMessage responseMessage)
        {
            return this.OnGetResponseHeaders(objectType, mediaType, responseMessage);
        }

        /// <summary>
        /// Called from <see cref="GetResponseHeaders"/> to retrieve the response headers.
        /// </summary>
        /// <param name="objectType">The type of the object.  See <see cref="ObjectContent"/>.</param>
        /// <param name="mediaType">The media type.</param>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/>.</param>
        /// <returns>The collection of response header key value pairs.</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> OnGetResponseHeaders(Type objectType, string mediaType, HttpResponseMessage responseMessage)
        {
            return null;
        }

        /// <summary>
        /// Determines whether this <see cref="MediaTypeFormatter"/> can deserialize
        /// an object of the specified type.
        /// </summary>
        /// <remarks>
        /// The base class unconditionally returns <c>true</c>.  Derived classes must
        /// override this to exclude types they cannot deserialize.
        /// </remarks>
        /// <param name="type">The type of object that will be deserialized.</param>
        /// <returns><c>true</c> if this <see cref="MediaTypeFormatter"/> can deserialize an object of that type; otherwise <c>false</c>.</returns>
        protected virtual bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="MediaTypeFormatter"/> can serialize
        /// an object of the specified type.
        /// </summary>
        /// <remarks>
        /// The base class unconditionally returns <c>true</c>.  Derived classes must
        /// override this to exclude types they cannot serialize.
        /// </remarks>
        /// <param name="type">The type of object that will be serialized.</param>
        /// <returns><c>true</c> if this <see cref="MediaTypeFormatter"/> can serialize an object of that type; otherwise <c>false</c>.</returns>
        protected virtual bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Called to read an object from the <paramref name="stream"/> asynchronously.
        /// Derived classes may override this to do custom deserialization.
        /// </summary>
        /// <param name="type">The type of the object to read.</param>
        /// <param name="stream">The <see cref="Stream"/> from which to read.</param>
        /// <param name="contentHeaders">The content headers from the respective request or response.</param>
        /// <returns>A <see cref="Task"/> that will yield an object instance when it completes.</returns>
        protected virtual Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            // Base implementation provides only a Task wrapper over the synchronous operation.
            // More intelligent derived formatters should override.
            return Task.Factory.StartNew<object>(() => this.OnReadFromStream(type, stream, contentHeaders));
        }

        /// <summary>
        /// Called to write an object to the <paramref name="stream"/> asynchronously.
        /// </summary>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object instance to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="contentHeaders">The content headers from the respective request or response.</param>
        /// <param name="context">The <see cref="TransportContext"/>.</param>
        /// <returns>A <see cref="Task"/> that will write the object to the stream asynchronously.</returns>
        protected virtual Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            // Base implementation provides only a Task wrapper over the synchronous operation.
            // More intelligent derived formatters should override.
            return Task.Factory.StartNew(() => this.OnWriteToStream(type, value, stream, contentHeaders, context));
        }

        /// <summary>
        /// Called to read an object from the <paramref name="stream"/>.
        /// Derived classes may override this to do custom deserialization.
        /// </summary>
        /// <param name="type">The type of the object to read.</param>
        /// <param name="stream">The <see cref="Stream"/> from which to read.</param>
        /// <param name="contentHeaders">The content headers from the respective request or response.</param>
        /// <returns>The object instance read from the <paramref name="stream"/>.</returns>
        protected abstract object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders);

        /// <summary>
        /// Called to write an object to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object instance to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="contentHeaders">The content headers from the respective request or response.</param>
        /// <param name="context">The <see cref="TransportContext"/>.</param>
        protected abstract void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context);

        private static Type GetOrAddDelegatingType(Type type)
        {
            return delegatingEnumerableCache.GetOrAdd(
                type,
                (typeToRemap) =>
                {
                    // The current method is called by methods that already checked the type for is not null, is generic and is or implements IEnumerable<T>
                    // This retrieves the T type of the IEnumerable<T> interface.
                    Type elementType;
                    if (typeToRemap.GetGenericTypeDefinition().Equals(FormattingUtilities.EnumerableInterfaceGenericType))
                    {
                        elementType = typeToRemap.GetGenericArguments()[0];
                    }
                    else
                    {
                        elementType = typeToRemap.GetInterface(FormattingUtilities.EnumerableInterfaceGenericType.FullName).GetGenericArguments()[0];
                    }

                    Type delegatingType = FormattingUtilities.DelegatingEnumerableGenericType.MakeGenericType(elementType);
                    ConstructorInfo delegatingConstructor = delegatingType.GetConstructor(new Type[] { FormattingUtilities.EnumerableInterfaceGenericType.MakeGenericType(elementType) });
                    delegatingEnumerableConstructorCache.TryAdd(delegatingType, delegatingConstructor);

                    return delegatingType;
                });
        }

        /// <summary>
        /// Collection class that validates it contains only <see cref="MediaTypeHeaderValue"/> instances
        /// that are not null and not media ranges.
        /// </summary>
        internal class MediaTypeHeaderValueCollection : Collection<MediaTypeHeaderValue>
        {
            private static readonly Type mediaTypeHeaderValueType = typeof(MediaTypeHeaderValue);

            /// <summary>
            /// Inserts the <paramref name="item"/> into the collection at the specified <paramref name="index"/>.
            /// </summary>
            /// <param name="index">The zero-based index at which item should be inserted.</param>
            /// <param name="item">The object to insert. It cannot be <c>null</c>.</param>
            protected override void InsertItem(int index, MediaTypeHeaderValue item)
            {
                ValidateMediaType(item);
                base.InsertItem(index, item);
            }

            /// <summary>
            /// Replaces the element at the specified <paramref name="index"/>.
            /// </summary>
            /// <param name="index">The zero-based index of the item that should be replaced.</param>
            /// <param name="item">The new value for the element at the specified index.  It cannot be <c>null</c>.</param>
            protected override void SetItem(int index, MediaTypeHeaderValue item)
            {
                ValidateMediaType(item);
                base.SetItem(index, item);
            }

            private static void ValidateMediaType(MediaTypeHeaderValue item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException("item");
                }

                ParsedMediaTypeHeaderValue parsedMediaType = new ParsedMediaTypeHeaderValue(item);
                if (parsedMediaType.IsAllMediaRange || parsedMediaType.IsSubTypeMediaRange)
                {
                    throw new ArgumentException(
                            SR.CannotUseMediaRangeForSupportedMediaType(mediaTypeHeaderValueType.Name, item.MediaType),
                            "item");
                }
            }
        }
    }
}