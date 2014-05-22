// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System.Net.Http.Formatting;

namespace Microsoft.ApplicationServer.HttpEnhancements.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;

    public class TestMediaTypeFormatter : MediaTypeFormatter
    {
        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            throw new NotImplementedException();
        }
    }
}