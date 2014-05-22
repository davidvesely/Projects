// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle Xml.
    /// </summary>
    public class XmlMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly Type xmlSerializerType = typeof(XmlSerializer);
        private static readonly Type dataContractSerializerType = typeof(DataContractSerializer);
        private static readonly Type xmlMediaTypeFormatterType = typeof(XmlMediaTypeFormatter);
        private static readonly MediaTypeHeaderValue[] supportedMediaTypes = new MediaTypeHeaderValue[]
        {
            MediaTypeConstants.ApplicationXmlMediaType,
            MediaTypeConstants.TextXmlMediaType
        };

        // Encoders used for reading data based on charset parameter and default encoder doesn't match
        private readonly Dictionary<string, Encoding> decoders = new Dictionary<string, Encoding>(StringComparer.OrdinalIgnoreCase)
        {
            { Encoding.UTF8.WebName, new UTF8Encoding(false, true) },
            { Encoding.Unicode.WebName, new UnicodeEncoding(false, true, true) },
        };

        private bool isModified;
        private ConcurrentDictionary<Type, object> serializerCache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMediaTypeFormatter"/> class.
        /// </summary>
        public XmlMediaTypeFormatter()
            : base()
        {
            this.Encoding = new UTF8Encoding(false, true);
            foreach (MediaTypeHeaderValue value in supportedMediaTypes)
            {
                this.SupportedMediaTypes.Add(value);
            }
        }

        /// <summary>
        /// Gets the default media type for xml, namely "application/xml".
        /// </summary>
        /// <value>
        /// <remarks>
        /// The default media type does not have any <c>charset</c> parameter as 
        /// the <see cref="Encoding"/> can be configured on a per <see cref="XmlMediaTypeFormatter"/> 
        /// instance basis.
        /// </remarks>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return MediaTypeConstants.ApplicationXmlMediaType;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use <see cref="DataContractSerializer"/> by default.
        /// </summary>
        /// <value>
        ///     <c>true</c> if use <see cref="DataContractSerializer"/> by default; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool UseDataContractSerializer { get; set; }
        
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
                    SR.UnsupportedEncoding(xmlMediaTypeFormatterType.Name, FormattingUtilities.Utf8EncodingType.Name, FormattingUtilities.Utf16EncodingType.Name), "value");
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
                    this.GetType().IsSubclassOf(xmlMediaTypeFormatterType);
            }
        }

        /// <summary>
        /// Registers the <see cref="XmlObjectSerializer"/> to use to read or write
        /// the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</param>
        /// <param name="serializer">The <see cref="XmlObjectSerializer"/> instance to use.</param>
        public void SetSerializer(Type type, XmlObjectSerializer serializer)
        {
            this.isModified = true;
            this.VerifyAndSetSerializer(type, serializer);
        }

        /// <summary>
        /// Registers the <see cref="XmlObjectSerializer"/> to use to read or write
        /// the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</typeparam>
        /// <param name="serializer">The <see cref="XmlObjectSerializer"/> instance to use.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents a Type parameter.")]
        public void SetSerializer<T>(XmlObjectSerializer serializer)
        {
            this.isModified = true;
            this.SetSerializer(typeof(T), serializer);
        }

        /// <summary>
        /// Registers the <see cref="XmlSerializer"/> to use to read or write
        /// the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of objects for which <paramref name="serializer"/> will be used.</param>
        /// <param name="serializer">The <see cref="XmlSerializer"/> instance to use.</param>
        public void SetSerializer(Type type, XmlSerializer serializer)
        {
            this.isModified = true;
            this.VerifyAndSetSerializer(type, serializer);
        }

        /// <summary>
        /// Registers the <see cref="XmlSerializer"/> to use to read or write
        /// the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</typeparam>
        /// <param name="serializer">The <see cref="XmlSerializer"/> instance to use.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents a Type parameter.")]
        public void SetSerializer<T>(XmlSerializer serializer)
        {
            this.isModified = true;
            this.SetSerializer(typeof(T), serializer);
        }

        /// <summary>
        /// Unregisters the serializer currently associated with the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// Unless another serializer is registered for the <paramref name="type"/>, a default one will be created.
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

            object value = null;
            return this.serializerCache.TryRemove(type, out value);
        }

        internal bool ContainsSerializerForType(Type type)
        {
            return this.serializerCache.ContainsKey(type);
        }

        /// <summary>
        /// Determines whether this <see cref="XmlMediaTypeFormatter"/> can read objects
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
            // Otherwise attempt to create the default serializer.
            object serializer = this.serializerCache.GetOrAdd(
                                    type,
                                    (t) => this.CreateDefaultSerializer(t, throwOnError: false));

            // Null means we tested it before and know it is not supported
            return serializer != null;
        }

        /// <summary>
        /// Determines whether this <see cref="XmlMediaTypeFormatter"/> can write objects
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

            if (this.UseDataContractSerializer)
            {
                MediaTypeFormatter.TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);
            }
            else
            {
                MediaTypeFormatter.TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type);
            }

            // If there is a registered non-null serializer, we can support this type.
            object serializer = this.serializerCache.GetOrAdd(
                                    type,
                                    (t) => this.CreateDefaultSerializer(t, throwOnError: false));

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

            object serializer = this.GetSerializerForType(type);

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

            using (XmlReader reader = XmlDictionaryReader.CreateTextReader(stream, effectiveEncoding, XmlDictionaryReaderQuotas.Max, null))
            {
                XmlSerializer xmlSerializer = serializer as XmlSerializer;
                if (xmlSerializer != null)
                {
                    return xmlSerializer.Deserialize(reader);
                }
                else
                {
                    XmlObjectSerializer xmlObjectSerializer = (XmlObjectSerializer)serializer;
                    return xmlObjectSerializer.ReadObject(reader);
                }
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

            bool isRemapped = false;
            if (this.UseDataContractSerializer)
            {
                isRemapped = MediaTypeFormatter.TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);
            }
            else
            {
                isRemapped = MediaTypeFormatter.TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type);
            }

            if (isRemapped)
            {
                value = MediaTypeFormatter.GetTypeRemappingConstructor(type).Invoke(new object[] { value });
            }

            object serializer = this.GetSerializerForType(type);

            // TODO: CSDMain 235508: Should formatters close write stream on completion or leave that to somebody else?
            using (XmlWriter writer = XmlDictionaryWriter.CreateTextWriter(stream, this.Encoding, ownsStream: false))
            {
                XmlSerializer xmlSerializer = serializer as XmlSerializer;
                if (xmlSerializer != null)
                {
                    xmlSerializer.Serialize(writer, value);
                }
                else
                {
                    XmlObjectSerializer xmlObjectSerializer = (XmlObjectSerializer)serializer;
                    xmlObjectSerializer.WriteObject(writer, value);
                }
            }
        }

        private object CreateDefaultSerializer(Type type, bool throwOnError)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Exception exception = null;
            object serializer = null;

            try
            {
                if (this.UseDataContractSerializer)
                {
                    serializer = new DataContractSerializer(type);
                }
                else
                {
                    serializer = new XmlSerializer(type);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                exception = invalidOperationException;
            }
            catch (NotSupportedException notSupportedException)
            {
                exception = notSupportedException;
            }

            // The serializer throws one of the exceptions above if it cannot
            // support this type.
            if (exception != null)
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException(
                            SR.SerializerCannotSerializeType(
                                this.UseDataContractSerializer ?
                                    dataContractSerializerType.Name :
                                    xmlSerializerType.Name,
                                type.Name),
                            exception);
                }
            }

            return serializer;
        }

        private void VerifyAndSetSerializer(Type type, object serializer)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.SetSerializerInternal(type, serializer);
        }

        private void SetSerializerInternal(Type type, object serializer)
        {
            Contract.Assert(type != null, "type cannot be null.");
            Contract.Assert(serializer != null, "serializer cannot be null.");

            this.serializerCache.AddOrUpdate(type, serializer, (key, value) => value);
        }

        private object GetSerializerForType(Type type)
        {
            Contract.Assert(type != null, "Type cannot be null");
            object serializer = this.serializerCache.GetOrAdd(type, (t) => this.CreateDefaultSerializer(t, throwOnError: true));

            if (serializer == null)
            {
                // A null serializer indicates the type has already been tested
                // and found unsupportable.
                throw new InvalidOperationException(
                    SR.SerializerCannotSerializeType(
                        this.UseDataContractSerializer ?
                            dataContractSerializerType.Name :
                            xmlSerializerType.Name,
                        type.Name));
            }

            Contract.Assert(serializer is XmlSerializer || serializer is XmlObjectSerializer, "Only XmlSerializer or XmlObjectSerializer are supported.");
            return serializer;
        }
    }
}
