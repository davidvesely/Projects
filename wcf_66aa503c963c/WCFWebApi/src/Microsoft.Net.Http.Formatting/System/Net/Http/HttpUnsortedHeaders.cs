// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Net.Http.Headers;

    /// <summary>
    /// All of the existing non-abstract <see cref="HttpHeaders"/> implementations, namely
    /// <see cref="HttpRequestHeaders"/>, <see cref="HttpResponseHeaders"/>, and <see cref="HttpContentHeaders"/>
    /// enforce strict rules on what kinds of HTTP header fields can be added to each collection.
    /// When parsing the "application/http" media type we need to just get the unsorted list. It
    /// will get sorted later.
    /// </summary>
    internal class HttpUnsortedHeaders : HttpHeaders
    {
    }
}