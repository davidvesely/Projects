// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Mocks
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.Server.Common;

    public class MockHttpMemoryHandler : HttpMemoryHandler
    {
        public static MockHttpMemoryHandler Create()
        {
            InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue = new InputQueue<HttpMemoryChannel.HttpMemoryRequestContext>();
            return new MockHttpMemoryHandler(inputQueue);
        }

        internal MockHttpMemoryHandler(InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> inputQueue)
            : base(inputQueue)
        {
            this.InputQueue = inputQueue;
        }

        internal InputQueue<HttpMemoryChannel.HttpMemoryRequestContext> InputQueue { get; private set; }

        new public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}