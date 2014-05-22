// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.IO;
    using System.Net.Http.Formatting;

    public class PlainTextFormatter : MediaTypeFormatter
    {
        public PlainTextFormatter()
        {
            this.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain"));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders, System.Net.TransportContext context)
        {
            var output = value.ToString();
            var writer = new StreamWriter(stream);
            writer.Write(output);
            writer.Flush();
        }

        protected override object OnReadFromStream(Type type, Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
