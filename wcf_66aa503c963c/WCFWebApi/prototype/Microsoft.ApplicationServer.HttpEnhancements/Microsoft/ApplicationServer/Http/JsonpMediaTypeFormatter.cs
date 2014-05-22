// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public JsonpMediaTypeFormatter()
        {
            MediaTypeHeaderValue jsonpMediaType = new MediaTypeHeaderValue("application/javascript");

            SupportedMediaTypes.Add(jsonpMediaType);
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", jsonpMediaType));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            IEnumerable<string> callbackValues = null;
            if (contentHeaders.TryGetValues("jsonp-callback", out callbackValues))
            {
                var callback = callbackValues.First<string>();
                var writer = new StreamWriter(stream);
                writer.Write(callback + "(");
                writer.Flush();
                base.OnWriteToStream(type, value, stream, contentHeaders, context);
                writer.Write(")");
                writer.Flush();
            }
            else
            {
                base.OnWriteToStream(type, value, stream, contentHeaders, context);
            }
        }
    }
}
