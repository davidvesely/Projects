// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Configuration
{
    using System.Configuration;
    using System.ServiceModel;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMemoryBindingElementTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBindingElement ctor initializes all properties to known defaults")]
        public void HttpMemoryBindingElement_Ctor_Initializes_All_Properties()
        {
            HttpMemoryBindingElement element = new HttpMemoryBindingElement();
            HttpMemoryBinding binding = new HttpMemoryBinding();

            Assert.AreEqual(binding.CloseTimeout, element.CloseTimeout, "The HttpMemoryBinding and HttpMemoryBindingElement should have the same default CloseTimeout");
            Assert.AreEqual(binding.OpenTimeout, element.OpenTimeout, "The HttpMemoryBinding and HttpMemoryBindingElement should have the same default OpenTimeout");
            Assert.AreEqual(binding.ReceiveTimeout, element.ReceiveTimeout, "The HttpMemoryBinding and HttpMemoryBindingElement should have the same default ReceiveTimeout");
            Assert.AreEqual(binding.SendTimeout, element.SendTimeout, "The HttpMemoryBinding and HttpMemoryBindingElement should have the same default ReceiveTimeout");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("The HttpMemoryBinding constructor throws with an unknown configurationName parameter.")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config")]
        public void HttpMemoryBinding_Throws_With_Unknown_ConfigurationName_Parameter()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config", () =>
            {
                UnitTest.Asserters.Exception.Throws<ConfigurationErrorsException>(
                    null,
                    () =>
                    {
                        new HttpMemoryBinding("noSuchConfiguredBinding");
                    });
            });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("The HttpMemoryBinding constructor is correctly configured when the binding configuration exists.")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config")]
        public void HttpMemoryBinding_Correctly_Configured_With_Name_Configuration()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config", () =>
            {
                HttpMemoryBinding binding = new HttpMemoryBinding("configuredBinding");
                Assert.AreEqual(HostNameComparisonMode.StrongWildcard, binding.HostNameComparisonMode, "Binding.HostNameComparisonMode should have been HostNameComparisonMode.StrongWildcard.");
            });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("The HttpMemoryBinding constructor is correctly configured when the binding configuration exists but has an empty string name.")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config")]
        public void HttpMemoryBinding_Correctly_Configured_With_Empty_Name_Configuration()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.ConfiguredHttpMemoryBindingTest.config", () =>
            {
                HttpMemoryBinding binding = new HttpMemoryBinding("");
                Assert.AreEqual(HostNameComparisonMode.StrongWildcard, binding.HostNameComparisonMode, "Binding.HostNameComparisonMode should have been HostNameComparisonMode.StrongWildcard.");
            });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("The HttpMemoryBinding constructor throws when there is no config file for the AppDomain.")]
        public void HttpMemoryBinding_Throws_When_No_Config_File()
        {
            UnitTest.Asserters.Exception.Throws<ConfigurationErrorsException>(
                Http.SR.ConfigInvalidBindingConfigurationName("configuredBinding", "httpMemoryBinding"),
                () => new HttpMemoryBinding("configuredBinding"));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("The HttpMemoryBinding constructor throws when there is no binding section in the config file.")]
        [DeploymentItem("ConfigFiles\\Microsoft.ApplicationServer.Http.CIT.Unit.HttpMemoryEmptyConfigurationTest.config")]
        public void HttpMemoryBinding_Throws_When_No_Binding_Section()
        {
            UnitTest.Asserters.Config.Execute("Microsoft.ApplicationServer.Http.CIT.Unit.HttpMemoryEmptyConfigurationTest.config", () =>
            {
                UnitTest.Asserters.Exception.Throws<ConfigurationErrorsException>(null, () => new HttpMemoryBinding("configuredBinding"));
            });
        }
    }
}
