// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using System.ServiceModel;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpInstanceProvider : HttpInstanceProvider
    {
        public Func<InstanceContext, object> OnGetInstanceCallback { get; set; }
        public Func<InstanceContext, HttpRequestMessage, object> OnGetInstanceHttpRequestMessageCallback { get; set; }
        public Action<InstanceContext, object> OnReleaseInstanceCallback { get; set; }

        protected override object OnGetInstance(InstanceContext instanceContext)
        {
            Assert.IsNotNull(this.OnGetInstanceCallback, "Set OnGetInstanceCallback first.");
            return this.OnGetInstanceCallback(instanceContext);
        }

        protected override object OnGetInstance(InstanceContext instanceContext, HttpRequestMessage request)
        {
            Assert.IsNotNull(this.OnGetInstanceHttpRequestMessageCallback, "Set OnGetInstanceHttpRequestMessageCallback first.");
            return this.OnGetInstanceHttpRequestMessageCallback(instanceContext, request);
        }

        protected override void OnReleaseInstance(InstanceContext instanceContext, object instance)
        {
            Assert.IsNotNull(this.OnReleaseInstanceCallback, "Set OnReleaseInstanceCallback first.");
            this.OnReleaseInstanceCallback(instanceContext, instance);
        }
    }
}
