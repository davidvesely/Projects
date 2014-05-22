// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http.Headers;

    internal static class HttpHeaderExtensionMethods
    {
        public static void CopyTo(this HttpContentHeaders fromHeaders, HttpContentHeaders toHeaders)
        {
            Contract.Assert(fromHeaders != null, "fromHeaders cannot be null.");
            Contract.Assert(toHeaders != null, "toHeaders cannot be null.");

            foreach (KeyValuePair<string, IEnumerable<string>> header in fromHeaders)
            {
                toHeaders.AddWithoutValidation(header.Key, header.Value);
            }
        }

        public static void CopyTo(this HttpRequestHeaders fromHeaders, HttpRequestHeaders toHeaders)
        {
            Contract.Assert(fromHeaders != null, "fromHeaders cannot be null.");
            Contract.Assert(toHeaders != null, "toHeaders cannot be null.");

            foreach (KeyValuePair<string, IEnumerable<string>> header in fromHeaders)
            {
                toHeaders.AddWithoutValidation(header.Key, header.Value);
            }
        }

        public static void CopyTo(this HttpResponseHeaders fromHeaders, HttpResponseHeaders toHeaders)
        {
            Contract.Assert(fromHeaders != null, "fromHeaders cannot be null.");
            Contract.Assert(toHeaders != null, "toHeaders cannot be null.");

            foreach (KeyValuePair<string, IEnumerable<string>> header in fromHeaders)
            {
                toHeaders.AddWithoutValidation(header.Key, header.Value);
            }
        }
    }
}