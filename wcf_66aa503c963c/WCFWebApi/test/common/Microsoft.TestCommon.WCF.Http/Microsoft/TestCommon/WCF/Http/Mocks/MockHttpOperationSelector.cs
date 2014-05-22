// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpOperationSelector : HttpOperationSelector
    {
        public Func<HttpRequestMessage, string> OnSelectOperationCallback { get; set; }

        protected override string OnSelectOperation(HttpRequestMessage request)
        {
            Assert.IsNotNull(this.OnSelectOperationCallback, "Set OnSelectOperationCallback first.");
            return this.OnSelectOperationCallback(request);
        }
    }
}
