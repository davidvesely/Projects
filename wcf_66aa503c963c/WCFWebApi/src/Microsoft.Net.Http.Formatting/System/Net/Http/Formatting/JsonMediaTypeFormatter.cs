// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle Json.
    /// </summary>
    public class JsonMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly Type dataContractJsonSerializerType = typeof(DataContractJsonSerializer);
        private static readonly Type jsonMediaTypeFormatterType = typeof(JsonMediaTypeFormatter);
        private static readonly MediaTypeHeaderValue[] supportedMediaTypes = new MediaTypeHeaderValue[]
        {
            MediaTypeConstants.ApplicationJsonMediaType,
            MediaTypeConstants.TextJsonMediaType
        };

        // Encoders used for reading data based on charset parameter and default encoder doesn't match
        private readonly Dictionary<string, Encoding> decoders = new Dictionary<string, Encoding>(StringComparer.OrdinalIgnoreCase)
        {
            { Encoding.UTF8.WebName, new UTF8Encoding(false, true) },
            { Encoding.Unicode.WebName, new UnicodeEncoding(false, true, true) },
        };

        private bool isModified;
        private ConcurrentDictionary<Type, DataContractJsonSerializer> serializerCache = new ConcurrentDictionary<Type, DataContractJsonSerializer>();
        private RequestHeaderMapping requestHeaderMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMediaTypeFormatter"/> class.
        /// </summary>
        public JsonMediaTypeFormatter()
            : base()
        {
            this.Encoding = new UTF8Encoding(false, true);
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
        /// Gets the default media type for Json, namely "application/json".
        /// </summary>
        /// <remarks>
        /// The default media type does not have any <c>charset</c> parameter as 
        /// the <see cref="Encoding"/> can be configured on a per <see cref="JsonMediaTypeFormatter"/> 
        /// instance basis.
        /// </remarks>
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
        /// Gets or sets the <see cref="Encoding"/> to use when writing data.
        /// </summary>
        /// <remarks>The default encoding is <see cref="UTF8Encoding"/>.</remarks>
        /// <value>
        /// The <see cref="Encoding"/> to use when writing data.
        /// </value>
        public Encoding CharacterEncoding
        {
            get
            {
                return this.Encoding;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                Type valueType = value.GetType();
                if (FormattingUtilities.Utf8EncodingType.IsAssignableFrom(valueType) || FormattingUtilities.Utf16EncodingType.IsAssignableFrom(valueType))
                {
                    this.Encoding = value;
                    return;
                }

                throw new ArgumentException(
                    SR.UnsupportedEncoding(jsonMediaTypeFormatterType.Name, FormattingUtilities.Utf8EncodingType.Name, FormattingUtilities.Utf16EncodingType.Name), "value");
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
                    this.GetType().IsSubclassOf(jsonMediaTypeFormatterType);
            }
        }

        /// <summary>
        /// Registers the <see cref="DataContractJsonSerializer"/> to use to read or write
        /// the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be serialized or deserialized with the <paramref name="serializer"/>.</param>
        /// <param name="serializer">The <see cref="DataContractJsonSerializer"/> instance to use.</param>
        public void SetSerializer(Type type, DataContractJsonSerializer serializer)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.isModified = true;
            this.serializerCache.AddOrUpdate(type, serializer, (key, value) => value);
        }

        /// <summary>
        /// Registers the <see cref="DataContractJsonSerializer"/> to use to read or write
        /// the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type of object that will be serialized or deserialized with the <paramref name="serializer"/>.</typeparam>
        /// <param name="serializer">The <see cref="DataContractJsonSerializer"/> instance to use.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents a Type parameter.")]
        public void SetSerializer<T>(DataContractJsonSerializer serializer)
        {
            this.isModified = true;
            this.SetSerializer(typeof(T), serializer);
        }

        /// <summary>
        /// Unregisters the serializer currently associated with the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// Unless another serializer is registered for the <paramref name="type"/>, a default one will be created
        /// the next time an instance of the type needs to be serialized or deserialized.
        /// </remarks>
        /// <param name="type">The type of object whose serializer should be removed.</param>
        /// <returns><c>true</c> if a serializer was registered for the <paramref name="type"/>; otherwise <c>false</c>.</returns>
        public bool RemoveSerializer(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.isModified = true;

            DataContractJsonSerializer value = null;
            return this.serializerCache.TryRemove(type, out value);
        }

        internal bool ContainsSerializerForType(Type type)
        {
            return this.serializerCache.ContainsKey(type);
        }

        /// <summary>
        /// Determines whether this <see cref="JsonMediaTypeFormatter"/> can read objects
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

            if (FormattingUtilities.IsJsonValueType(type))
            {
                return false;
            }

            // If there is a registered non-null serializer, we can support this type.
            DataContractJsonSerializer serializer =
                this.serializerCache.GetOrAdd(type, (t) => JsonMediaTypeFormatter.CreateDefaultSerializer(t));

            // Null means we tested it before and know it is not supported
            return serializer != null;
        }

        /// <summary>
        /// Determines whether this <see cref="JsonMediaTypeFormatter"/> can write objects
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

            if (FormattingUtilities.IsJsonValueType(type))
            {
                return false;
            }

            MediaTypeFormatter.TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);

            // If there is a registered non-null serializer, we can support this type.
            DataContractJsonSerializer serializer =
                this.serializerCache.GetOrAdd(type, (t) => JsonMediaTypeFormatter.CreateDefaultSerializer(t));

            // Null means we tested it before and know it is not supported
            return serializer != null;
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

            DataContractJsonSerializer serializer = this.GetSerializerForType(type);

            Encoding effectiveEncoding = this.Encoding;
            if (contentHeaders != null && contentHeaders.ContentType != null)
            {
                string charset = contentHeaders.ContentType.CharSet;
                if (!string.IsNullOrWhiteSpace(charset) &&
                    !string.Equals(charset, this.Encoding.WebName) &&
                    !this.decoders.TryGetValue(charset, out effectiveEncoding))
                {
                    effectiveEncoding = this.Encoding;
                }
            }

            using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(stream, effectiveEncoding, XmlDictionaryReaderQuotas.Max, null))
            {
                return serializer.ReadObject(reader);
            }
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

            if (MediaTypeFormatter.TryGetDelegatingTypeForIQueryableGenericOrSame(ref type))
            {
                value = MediaTypeFormatter.GetTypeRemappingConstructor(type).Invoke(new object[] { value });
            }

            DataContractJsonSerializer serializer = this.GetSerializerForType(type);

            // TODO: CSDMain 235508: Should formatters close write stream on completion or leave that to somebody else?
            using (XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream, this.Encoding, ownsStream: false))
            {
                serializer.WriteObject(writer, value);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        private static DataContractJsonSerializer CreateDefaultSerializer(Type type)
        {
            Contract.Assert(type != null, "type cannot be null.");
            DataContractJsonSerializer serializer = null;

            try
            {
                //// TODO: CSDMAIN 211321 -- determine the correct algorithm to know what is serializable.
                serializer = IsKnownUnserializableType(type) ? null : new DataContractJsonSerializer(type);
            }
            catch (Exception)
            {
                //// TODO: CSDMain 232171 -- review and fix swallowed exception
            }

            return serializer;
        }

        private static bool IsKnownUnserializableType(Type type)
        {
            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return IsKnownUnserializableType(type.GetGenericArguments()[0]);
                }
            }

            if (!type.IsVisible)
            {
                return true;
            }

            if (type.HasElementType && IsKnownUnserializableType(type.GetElementType()))
            {
                return true;
            }

            return false;
        }

        private DataContractJsonSerializer GetSerializerForType(Type type)
        {
            Contract.Assert(type != null, "Type cannot be null");

            DataContractJsonSerializer serializer =
                this.serializerCache.GetOrAdd(type, (t) => JsonMediaTypeFormatter.CreateDefaultSerializer(type));

            if (serializer == null)
            {
                // A null serializer means the type cannot be serialized
                throw new InvalidOperationException(
                        SR.SerializerCannotSerializeType(dataContractJsonSerializerType.Name, type.Name));
            }

            return serializer;
        }
    }
}
