// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace CustomFormatter.Sample
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;

    public class CustomXmlMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue fooMediaType = new MediaTypeHeaderValue("application/foo");
        
        public CustomXmlMediaTypeFormatter()
        {
            this.SupportedMediaTypes.Add(fooMediaType);

            // This is match anything with "?$format=foo" 
            this.MediaTypeMappings.Add(new QueryStringMapping("$format", "foo", fooMediaType));
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);
            serializer.WriteObject(stream, value);
        }
    }
}
