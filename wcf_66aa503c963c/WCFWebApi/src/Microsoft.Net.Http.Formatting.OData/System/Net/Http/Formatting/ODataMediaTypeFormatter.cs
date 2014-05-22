// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "There are other types in this namespace within other assemblies", Scope = "namespace", Target = "System.Net.Http.Formatting")]
namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http.Formatting.OData;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Data.OData;

    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle OData.
    /// </summary>
    public class ODataMediaTypeFormatter : MediaTypeFormatter
    {
        internal const string DefaultApplicationODataMediaType = "application/atom+xml";
        internal const int DefaultMaxReferenceDepth = 5;
        internal const string ODataServiceVersion = "DataServiceVersion";
        internal const string ODataMaxServiceVersion = "MaxDataServiceVersion";

        private static readonly MediaTypeHeaderValue DefaultApplicationAtomXmlMediaType = new MediaTypeHeaderValue(DefaultApplicationODataMediaType);
        private static readonly MediaTypeHeaderValue DefaultApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");    
        private static readonly XsdDataContractExporter xsdDataContractExporter = new XsdDataContractExporter();
        private static readonly ODataVersion defaultODataVersion = ODataVersion.V3;
        private static readonly Type ODataMediaTypeFormatterType = typeof(ODataMediaTypeFormatter);

        private int maxReferenceDepth = DefaultMaxReferenceDepth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMediaTypeFormatter"/> class.
        /// </summary>
        public ODataMediaTypeFormatter()
            : base()
        {
            this.Encoding = Encoding.UTF8;
            this.SupportedMediaTypes.Add(ODataMediaTypeFormatter.ApplicationAtomXmlMediaType);
            this.SupportedMediaTypes.Add(ODataMediaTypeFormatter.ApplicationJsonMediaType);
        }

        /// <summary>
        /// Gets the default media type for atom, namely "application/atom+xml".
        /// </summary>
        /// <value>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return ODataMediaTypeFormatter.ApplicationAtomXmlMediaType;
            }
        }

        /// <summary>
        /// Gets or sets the MaxReferenceDepth for serializing entity types.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Parametername passed in is correct")]
        public int MaxReferenceDepth
        {
            get
            {
                return this.maxReferenceDepth;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MaxReferenceDepth", value, OData.SR.MaxReferenceDepthLessThanZero);
                }

                this.maxReferenceDepth = value;
            }
        }

        private static MediaTypeHeaderValue ApplicationAtomXmlMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultApplicationAtomXmlMediaType).Clone();
            }
        }

        private static MediaTypeHeaderValue ApplicationJsonMediaType
        {
            get
            {
                return (MediaTypeHeaderValue)((ICloneable)DefaultApplicationJsonMediaType).Clone();
            }
        }

        /// <summary>
        /// Allows one or more members of a type to be specified as the key members for
        /// the given type when serialized by the <see cref="ODataMediaTypeFormatter"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the key memebers are being specified.</param>
        /// <param name="memberNames">One or more names of members on the <paramref name="type"/> that should be considered key members by the <see cref="ODataMediaTypeFormatter"/>.</param>
        public static void SetKeyMembers(Type type, params string[] memberNames)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (memberNames == null)
            {
                throw new ArgumentNullException("memberNames");
            }

            DefaultODataSerializerProvider.Instance.SetKeyMembers(type, memberNames);
        }

        /// <summary>
        ///  Retrieve the ContentHeaders by calling into the internal ODataSerializer
        /// </summary>
        /// <param name="objectType"> The type of object to be serialized.</param>
        /// <param name="mediaType"> mediatType to determine the ODataFormat.</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> instance to retrieve the ODataVersion.</param>
        /// <returns>A the headers set by the ODataserializer.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> GetODataContentHeaders(Type objectType, string mediaType, HttpResponseMessage response)
        {
            ODataFormat format = GetODataFormat(mediaType);
            ODataVersion version = GetODataVersion(response);
            return GetResponseMessageHeaders(objectType, format, version);
        }

        /// <summary>
        /// Called from the base class to retrieve the response headers.
        /// </summary>
        /// <param name="objectType">The type of the object.  See <see cref="ObjectContent"/>.</param>
        /// <param name="mediaType">The media type.</param>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/>.</param>
        /// <returns>The collection of response header key value pairs.</returns>
        protected override IEnumerable<KeyValuePair<string, string>> OnGetResponseHeaders(Type objectType, string mediaType, HttpResponseMessage responseMessage)
        {
            return GetODataContentHeaders(objectType, mediaType, responseMessage);
        }

        /// <summary>
        /// Determines whether this <see cref="ODataMediaTypeFormatter"/> can read objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be read.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            // ODataSerializer does not support deserialization
            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="ODataMediaTypeFormatter"/> can write objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be written.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception translates to false.")]
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            MediaTypeFormatter.TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type);

            if (IsKnownUnserializableType(type))
            {
                return false;
            }

            try
            {
                IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(type);
                return serializer != null;
            }
            catch (Exception)
            {
                return false;
            }
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
            throw new NotSupportedException(OData.SR.CanNotDeserialize(ODataMediaTypeFormatterType.Name));
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

            if (MediaTypeFormatter.TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type))
            {
                value = MediaTypeFormatter.GetTypeRemappingConstructor(type).Invoke(new object[] { value });
            }

            // Format and version has already been copied to the response content headers
            ODataVersion version = GetODataVersion(contentHeaders);
            ODataFormat odataFormat = GetODataFormat(contentHeaders);

            // TODO, 202270, Will lead to InvalidOperationException when xmlQualifiedName is null since XmlDataContract is not supported. Fix the error message and the
            // logic when xmlQualifiedName is null.
            XmlQualifiedName xmlQualifiedName = xsdDataContractExporter.GetRootElementName(type);
            string typeName = xmlQualifiedName != null ? xmlQualifiedName.Name : type.Name;

            IODataResponseMessage responseMessage = new ResponseStreamOnlyODataResponseMessage(stream);
            ODataResponseContext responseContext = new ODataResponseContext(responseMessage, odataFormat, version, new Uri(ODataConstants.DefaultNamespace), typeName);

            IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(type);
            if (serializer == null)
            {
                throw new InvalidOperationException(OData.SR.TypeCannotBeSerialized(type.FullName));
            }

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings()
            {
                BaseUri = responseContext.BaseAddress,
                Version = responseContext.ODataVersion,
                Indent = responseContext.IsIndented,
                DisableMessageStreamDisposal = true,
            };
            writerSettings.SetContentType(responseContext.ODataFormat);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(responseContext.ODataResponseMessage, writerSettings))
            {
                ODataSerializerWriteContext writeContext = new ODataSerializerWriteContext(this.maxReferenceDepth, responseContext);
                serializer.WriteObject(value, messageWriter, writeContext);
            }
        }

        private static bool IsKnownUnserializableType(Type type)
        {
            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    if (type.GetMethod("Add") == null)
                    {
                        return true;
                    }

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

        private static ODataFormat GetODataFormat(HttpContentHeaders contentHeaders)
        {
            Contract.Assert(contentHeaders.ContentType != null, "ContentType should be populated when ODataFormatter is selected");
            return GetODataFormat(contentHeaders.ContentType.MediaType);
        }

        private static ODataFormat GetODataFormat(string mediaType)
        {
            ODataFormat format = ODataFormat.Default;
            if (mediaType == null)
            {
                throw new ArgumentNullException("mediaType");
            }

            if (String.Equals(mediaType, DefaultApplicationODataMediaType, StringComparison.OrdinalIgnoreCase))
            {
                format = ODataFormat.Atom;
            }
            else if (String.Equals(mediaType, ODataMediaTypeFormatter.ApplicationJsonMediaType.MediaType, StringComparison.OrdinalIgnoreCase)) 
            {
                format = ODataFormat.Json;
            }

            return format;
        }

        private static ODataVersion GetODataVersion(HttpResponseMessage response)
        {
            ODataVersion version = defaultODataVersion;
            if (response != null)
            {
                IEnumerable<string> values;

                // if the requestMessage does not contain DataServiceVersion header, look for the 
                // MaxDataServiceVersion header.
                if (response.RequestMessage != null)
                {
                    if (response.RequestMessage.Headers.TryGetValues(ODataServiceVersion, out values) ||
                        response.RequestMessage.Headers.TryGetValues(ODataMaxServiceVersion, out values))
                    {
                        foreach (string value in values)
                        {
                            string trimmedValue = value.Trim(' ', ';');
                            version = GetODataVersion(trimmedValue);
                        }
                    }
                }
            }
           
            return version;
        }

        private static ODataVersion GetODataVersion(HttpContentHeaders contentHeaders)
        {
            ODataVersion version = defaultODataVersion;
            if (contentHeaders == null)
            {
                throw new ArgumentNullException("contentHeaders");
            }

            IEnumerable<string> values;

            if (contentHeaders.TryGetValues(ODataServiceVersion, out values))
            {
                foreach (string value in values)
                {
                    string trimmedValue = value.Trim(' ', ';');
                    version = GetODataVersion(trimmedValue);
                }
            }

            return version;
        }

        private static ODataVersion GetODataVersion(string versionString)
        {
            // TODO, DevDiv 180528, We have requested the ODataTeam to provide a helper method to do this, remove 
            // this method once we get this.
            ODataVersion version = defaultODataVersion;
            const string Version1NumberString = "1.0";
            const string Version2NumberString = "2.0";
            const string Version3NumberString = "3.0";
            if (String.Equals(versionString, Version1NumberString, StringComparison.OrdinalIgnoreCase))
            {
                version = ODataVersion.V1;
            }
            else if (String.Equals(versionString, Version2NumberString, StringComparison.OrdinalIgnoreCase))
            {
                version = ODataVersion.V2;
            }
            else if (String.Equals(versionString, Version3NumberString, StringComparison.OrdinalIgnoreCase))
            {
                version = ODataVersion.V3;
            }

            return version;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "TODO: review ODataLib handling of object.")]
        private static IEnumerable<KeyValuePair<string, string>> GetResponseMessageHeaders(Type graphType, ODataFormat odataFormat, ODataVersion version)
        {
            IODataResponseMessage responseMessage = new HeadersOnlyODataResponseMessage();

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings()
            {
                BaseUri = new Uri(ODataConstants.DefaultNamespace),
                Version = version,
                Indent = false
            };
            writerSettings.SetContentType(odataFormat);
            ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage, writerSettings);
            IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(graphType);

            // get the OData specific headers for the payloadkind
            ODataUtils.SetHeadersForPayload(messageWriter, serializer.ODataPayloadKind);
            return responseMessage.Headers;
        }

        private class HeadersOnlyODataResponseMessage : IODataResponseMessage
        {
            private Dictionary<string, string> headers = new Dictionary<string, string>();

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return this.headers; }
            }

            public int StatusCode
            {
                get
                {
                    return 0;
                }

                set
                {
                    // no op
                }
            }

            public string GetHeader(string headerName)
            {
                return null;
            }

            public Stream GetStream()
            {
                return null;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                this.headers[headerName] = headerValue;
            }
        }

        private class ResponseStreamOnlyODataResponseMessage : IODataResponseMessageAsync
        {
            private readonly Stream stream;

            public ResponseStreamOnlyODataResponseMessage(Stream stream)
            {
                Contract.Assert(stream != null, "The 'stream' parameter should never be null.");
                this.stream = stream;
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get
                {
                    return null;
                }
            }

            public int StatusCode
            {
                get
                {
                    return 0;
                }

                set
                {
                   // no op
                }
            }

            public string GetHeader(string headerName)
            {
                return null;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                // no op
            }

            public Stream GetStream()
            {
                return this.stream;
            }

            public Task<Stream> GetStreamAsync()
            {
                TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
                completionSource.SetResult(this.stream);
                return completionSource.Task;
            }
        }
    }
}
