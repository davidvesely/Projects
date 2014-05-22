// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels.Mocks
{
    using System.Net.Http;

    public class MockValidMessageHandler : DelegatingHandler
    {
        public MockValidMessageHandler()
            : base()
        {
        }

        public MockValidMessageHandler(HttpMessageHandler innerChannel)
            : base(innerChannel)
        { 
        }

        public int Index { get; set; }
    }
}
