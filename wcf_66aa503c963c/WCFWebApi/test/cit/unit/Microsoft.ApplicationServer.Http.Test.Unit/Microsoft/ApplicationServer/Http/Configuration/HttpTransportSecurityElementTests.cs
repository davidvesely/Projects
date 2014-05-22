// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System.ServiceModel;
    using System.ServiceModel.Configuration;
    using Microsoft.ServiceModel.Configuration;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpTransportSecurityElementTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpTransportSecurity)]
        [Description("HttpTransportSecurityElement.ApplyConfiguration sets HttpTransportSecurity")]
        public void HttpTransportSecurityElement_ApplyConfiguration()
        {
            HttpTransportSecurityElement element = new HttpTransportSecurityElement()
            {
                ClientCredentialType = HttpClientCredentialType.Basic,
                ProxyCredentialType = HttpProxyCredentialType.Basic,
                Realm = "MyRealm"
            };
            HttpTransportSecurity security = new HttpTransportSecurity();

            element.ApplyConfiguration(security);

            Assert.AreEqual(element.ClientCredentialType, security.ClientCredentialType, "ClientCredentialType failed");
            Assert.AreEqual(element.ProxyCredentialType, security.ProxyCredentialType, "ProxyCredentialType failed");
            Assert.AreEqual(element.Realm, security.Realm, "Realm failed");
            Assert.IsNotNull(security.ExtendedProtectionPolicy, "ExtendedProtectionPolicy failed");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpTransportSecurity)]
        [Description("HttpTransportSecurityElementInitializeFrom works properly")]
        public void HttpTransportSecurityElement_InitializeFrom()
        {
            HttpTransportSecurityElement element = new HttpTransportSecurityElement()
            {
                ClientCredentialType = HttpClientCredentialType.Basic,
                ProxyCredentialType = HttpProxyCredentialType.Basic,
                Realm = "MyRealm"
            };
            HttpTransportSecurity security = new HttpTransportSecurity();

            // first initialize the transport security
            element.ApplyConfiguration(security);

            Assert.AreEqual(element.ClientCredentialType, security.ClientCredentialType, "ClientCredentialType failed");
            Assert.AreEqual(element.ProxyCredentialType, security.ProxyCredentialType, "ProxyCredentialType failed");
            Assert.AreEqual(element.Realm, security.Realm, "Realm failed");
            Assert.IsNotNull(security.ExtendedProtectionPolicy, "ExtendedProtectionPolicy failed");

            // now initialize a new instance from the security
            HttpTransportSecurityElement element2 = new HttpTransportSecurityElement();
            element2.InitializeFrom(security);

            Assert.AreEqual(element.ClientCredentialType, element2.ClientCredentialType, "ClientCredentialType failed");
            Assert.AreEqual(element.ProxyCredentialType, element2.ProxyCredentialType, "ProxyCredentialType failed");
            Assert.AreEqual(element.Realm, element2.Realm, "Realm failed");
            Assert.IsNotNull(element2.ExtendedProtectionPolicy, "ExtendedProtectionPolicy failed");
        }
    }
}
