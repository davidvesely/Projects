// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.Text;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(TestCommon.UnitTestLevel.InProgress)]
    public class HttpEndpointTests: UnitTest<HttpEndpoint>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpEndpoint is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("dravva")]
        [Description("HttpEndpoint(ContractDescription, EndpointAddress) supports default ctor with Https Binding")]
        public void HttpEndpointTests_Default_Ctor()
        {
            using (HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), "https://localhost:8085/ServerScenario1"))
            {
                host.Open();

                Assert.AreEqual(1, host.Description.Endpoints.Count);
                Assert.IsInstanceOfType(host.Description.Endpoints[0], typeof(HttpEndpoint));
                Assert.AreEqual(HttpBindingSecurityMode.Transport, ((HttpEndpoint)host.Description.Endpoints[0]).Security.Mode);
            }
        }

        #endregion Constructors
    }
}