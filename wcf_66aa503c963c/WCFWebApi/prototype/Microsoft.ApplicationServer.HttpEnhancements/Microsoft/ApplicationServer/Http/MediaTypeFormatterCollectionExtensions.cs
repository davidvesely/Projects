// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Net.Http.Formatting;

    public static class MediaTypeFormatterCollectionExtensions
    {
        public static void AddRange(this MediaTypeFormatterCollection formatters, params MediaTypeFormatter[] formattersToAdd) 
        {
            foreach (var formatter in formattersToAdd)
            {
                formatters.Add(formatter);
            }
        }
    }
}
