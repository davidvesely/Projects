// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public delegate bool OnTryProvideResponseDelegate(Exception e, ref HttpResponseMessage message);

    public class MockHttpErrorHandler : HttpErrorHandler
    {
        public OnTryProvideResponseDelegate OnTryProvideResponseCallback { get; set; }

        protected override bool OnTryProvideResponse(Exception exception, ref HttpResponseMessage message)
        {
            Assert.IsNotNull(this.OnTryProvideResponseCallback, "Set OnTryProvideResponseCallback first.");
            return this.OnTryProvideResponseCallback(exception, ref message);
        }
    }
}
