// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpBindingSecurityTests
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity is a public, non-abstract class.")]
        public void HttpBindingSecurity_Is_A_Public_Non_Abstract_Class()
        {
            UnitTest.Asserters.Type.HasProperties<HttpBindingSecurity>(TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsSealed);
        }

        #endregion Type Tests

        #region Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Mode is HttpBindingSecurityMode.None by default.")]
        public void Mode_Is_None_By_Default()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.AreEqual(HttpBindingSecurityMode.None, security.Mode, "HttpBindingSecurity.Mode should have been HttpBindingSecurityMode.None by default.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Mode can be set.")]
        public void Mode_Can_Be_Set()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            security.Mode = HttpBindingSecurityMode.Transport;
            Assert.AreEqual(HttpBindingSecurityMode.Transport, security.Mode, "HttpBindingSecurity.Mode should have been HttpBindingSecurityMode.Transport.");

            security.Mode = HttpBindingSecurityMode.TransportCredentialOnly;
            Assert.AreEqual(HttpBindingSecurityMode.TransportCredentialOnly, security.Mode, "HttpBindingSecurity.Mode should have been HttpBindingSecurityMode.TransportCredentialOnly.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Mode throws with an invalid HttpBindingSecurityMode value.")]
        public void Mode_Throws_With_Invalid_HttpBindingSecurityMode_Value()
        {
            UnitTest.Asserters.Exception.ThrowsInvalidEnumArgument("value", 99, typeof(HttpBindingSecurityMode),
                () =>
                {
                    HttpBindingSecurity security = new HttpBindingSecurity();
                    security.Mode = (HttpBindingSecurityMode)99;
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport is not null.")]
        public void Transport_Is_Not_Null()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.IsNotNull(security.Transport, "HttpBindingSecurity.Transport should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport can be set.")]
        public void Transport_Can_Be_Set()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            HttpTransportSecurity transport = new HttpTransportSecurity();
            transport.ClientCredentialType = HttpClientCredentialType.Basic;
            security.Transport = transport;
            Assert.AreEqual(HttpClientCredentialType.Basic, security.Transport.ClientCredentialType, "HttpBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.Basic.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport is reset to a new HttpTransportSecurity instance if Transport is set to null.")]
        public void Transport_Resets_To_New_Instance_If_Set_To_Null()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            HttpTransportSecurity transport = security.Transport;
            transport.ClientCredentialType = HttpClientCredentialType.Basic;
            security.Transport = null;
            Assert.AreNotSame(transport, security.Transport, "HttpBindingSecurity.Transport should have been a new instance of HttpTransportSecurity.");
            Assert.AreEqual(HttpClientCredentialType.None, security.Transport.ClientCredentialType, "HttpBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.None.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport.ClientCredentialType is HttpClientCredentialType.None by default.")]
        public void Transport_ClientCredentialType_Is_None_By_Default()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.AreEqual(HttpClientCredentialType.None, security.Transport.ClientCredentialType, "HttpBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.None by default.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport.HttpProxyCredentialType is HttpProxyCredentialType.None by default.")]
        public void Transport_HttpProxyCredentialType_Is_None_By_Default()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.AreEqual(HttpProxyCredentialType.None, security.Transport.ProxyCredentialType, "HttpBindingSecurity.Transport.ProxyCredentialType should have been HttpProxyCredentialType.None by default.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport.ExtendedProtectionPolicy.PolicyEnforcement is PolicyEnforcement.Never by default.")]
        public void Transport_ExtendedProtectionPolicy_PolicyEnforcement_Is_Never_By_Default()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.AreEqual(PolicyEnforcement.Never, security.Transport.ExtendedProtectionPolicy.PolicyEnforcement, "HttpBindingSecurity.Transport.ExtendedProtectionPolicy.PolicyEnforcement should have been PolicyEnforcement.Never by default.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpBindingSecurity)]
        [Description("HttpBindingSecurity.Transport.Realm is empty string by default.")]
        public void Transport_Realm_Is_Empty_String_By_Default()
        {
            HttpBindingSecurity security = new HttpBindingSecurity();
            Assert.AreEqual(string.Empty, security.Transport.Realm, "HttpBindingSecurity.Transport.Realm should have been empty string.");
        }

        #endregion Property Tests
    }
}