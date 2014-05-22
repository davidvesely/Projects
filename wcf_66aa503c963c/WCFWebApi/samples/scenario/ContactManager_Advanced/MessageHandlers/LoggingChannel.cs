// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced
{
    using System.Net.Http;
    using System.ComponentModel.Composition;

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(DelegatingHandler))]
    public class LoggingMessageHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            System.Diagnostics.Trace.TraceInformation("Begin Request: {0} {1}", request.Method, request.RequestUri);
            return base.SendAsync(request, cancellationToken);
        }
    }
}