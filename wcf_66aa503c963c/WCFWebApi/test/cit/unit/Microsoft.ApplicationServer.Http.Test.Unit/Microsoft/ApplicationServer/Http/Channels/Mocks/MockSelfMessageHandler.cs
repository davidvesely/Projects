// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels.Mocks
{
    using System.Net.Http;

    public class MockSelfMessageHandler : DelegatingHandler
    {
        public MockSelfMessageHandler()
            : base()
        {
            this.InnerHandler = this;
        }

        public MockSelfMessageHandler(HttpMessageHandler innerChannel)
            : base(innerChannel)
        {
            this.InnerHandler = this;
        }
    }
}
