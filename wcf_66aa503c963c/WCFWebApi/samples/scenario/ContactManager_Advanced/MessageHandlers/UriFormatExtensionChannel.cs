// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Linq;
    using System.ComponentModel.Composition;
    
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(DelegatingHandler))]
    public class UriFormatExtensionMessageHandler : DelegatingHandler
    {
        [ImportingConstructor]
        public UriFormatExtensionMessageHandler(IEnumerable<UriExtensionMapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                extensionMappings[mapping.Extension] = mapping.MediaType;
            }
        }

        private static Dictionary<string, MediaTypeWithQualityHeaderValue> extensionMappings = new Dictionary<string, MediaTypeWithQualityHeaderValue>();

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var segments = request.RequestUri.Segments;
            var lastSegment = segments.LastOrDefault();
            MediaTypeWithQualityHeaderValue mediaType;
            var found = extensionMappings.TryGetValue(lastSegment, out mediaType);
            
            if (found)
            {
                var newUri = request.RequestUri.OriginalString.Replace("/" + lastSegment, "");
                request.RequestUri = new Uri(newUri, UriKind.Absolute);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(mediaType);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class UriExtensionMapping
    {
        public string Extension { get; set; }

        public MediaTypeWithQualityHeaderValue MediaType { get; set; }

    }

    public static class UriExtensionMappingExtensions
    {
        public static void AddMapping(this IList<UriExtensionMapping> mappings, string extension, string mediaType)
        {
            mappings.Add(new UriExtensionMapping { Extension = extension, MediaType = new MediaTypeWithQualityHeaderValue(mediaType) });
        }
    }

}