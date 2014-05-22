// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpBatching.Sample
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class SampleMemoryMessageHandler : DelegatingHandler
    {
        private static int offset;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    var headerValue = Interlocked.Increment(ref offset);
                    HttpResponseMessage response = task.Result;
                    response.Headers.Add("SampleMemoryHeader", headerValue.ToString());
                    return response;
                });
        }
    }
}
