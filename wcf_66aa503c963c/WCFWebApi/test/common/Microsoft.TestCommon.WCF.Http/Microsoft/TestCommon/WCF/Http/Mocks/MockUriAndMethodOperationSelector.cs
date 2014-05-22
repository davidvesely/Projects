// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public delegate bool OnTrySelectOperationDelegate(HttpRequestMessage request, out string operationName, out bool matchDifferByTrailingSlash);

    public class MockUriAndMethodOperationSelector : UriAndMethodOperationSelector
    {
        public bool CallBase { get; set; }
        public OnTrySelectOperationDelegate OnTrySelectOperationCallback { get; set; }

        public MockUriAndMethodOperationSelector(Uri baseAddress, IEnumerable<HttpOperationDescription> operations)
            : base(baseAddress, operations)
        {
        }

        public MockUriAndMethodOperationSelector(Uri baseAddress, IEnumerable<HttpOperationDescription> operations, TrailingSlashMode trailingSlashMode)
            : base(baseAddress, operations, trailingSlashMode)
        {
        }

        protected override bool OnTrySelectOperation(HttpRequestMessage request, out string operationName, out bool matchDifferByTrailingSlash)
        {
            Assert.IsTrue(this.CallBase || this.OnTrySelectOperationCallback != null, "Set CallBase or OnTrySelectOperationCallback first.");

            return this.OnTrySelectOperationCallback != null
                    ? this.OnTrySelectOperationCallback(request, out operationName, out matchDifferByTrailingSlash)
                    : base.OnTrySelectOperation(request, out operationName, out matchDifferByTrailingSlash);
        }
    }
}
