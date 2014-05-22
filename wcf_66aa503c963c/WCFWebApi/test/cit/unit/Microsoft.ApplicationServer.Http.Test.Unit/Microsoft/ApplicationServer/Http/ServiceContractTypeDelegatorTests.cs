// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(ServiceContractTypeDelegator)), UnitTestLevel(UnitTestLevel.Complete)]
    public class ServiceContractTypeDelegatorTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ServiceContractTypeDelegator is internal.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsClass,
                typeof(TypeDelegator));
        }

        #endregion

        #region Helpers

        [TestCustomAttribute]
        [ServiceContract]
        public class TestClassBase
        {
            public int Number { get; set; }
        }

        public class TestClass : TestClassBase
        {
        }

        public class TestCustomAttribute : Attribute
        {
            public string Name { get; set; }
        }

        #endregion

        #region Constructor

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ServiceContractTypeDelegator(Type) throws on null")]
        public void ServiceContractTypeDelegatorThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull("delegatingType", () => new ServiceContractTypeDelegator(null));
        }

        #endregion

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("IsDefined(Type, bool) throws on null")]
        public void IsDefinedThrowsOnNull()
        {
            ServiceContractTypeDelegator serviceContractTypeDelegator = new ServiceContractTypeDelegator(typeof(TestClass));
            Asserters.Exception.ThrowsArgumentNull("attributeType", () => serviceContractTypeDelegator.IsDefined(null, false));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("IsDefined(Type, bool) follows inherit")]
        public void IsDefinedFollowsInherit()
        {
            ServiceContractTypeDelegator serviceContractTypeDelegator = new ServiceContractTypeDelegator(typeof(TestClass));

            Assert.IsFalse(serviceContractTypeDelegator.IsDefined(typeof(TestCustomAttribute), false), "Custom attribute should not be defined without following hierarchy");
            Assert.IsTrue(serviceContractTypeDelegator.IsDefined(typeof(TestCustomAttribute), true), "Custom attribute should be defined while following hierarchy");

            Assert.IsTrue(serviceContractTypeDelegator.IsDefined(typeof(ServiceContractAttribute), false), "ServiceContract should be defined without following hierarchy");
            Assert.IsTrue(serviceContractTypeDelegator.IsDefined(typeof(ServiceContractAttribute), true), "ServiceContract should be defined while following hierarchy");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetCustomAttributes(Type, bool) throws on null")]
        public void GetCustomAttributesThrowsOnNull()
        {
            ServiceContractTypeDelegator serviceContractTypeDelegator = new ServiceContractTypeDelegator(typeof(TestClass));
            Asserters.Exception.ThrowsArgumentNull("attributeType", () => serviceContractTypeDelegator.GetCustomAttributes(null, false));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetCustomAttributes(Type, bool) follows inherit")]
        public void GetCustomAttributesFollowsInherit()
        {
            object[] customAttributes;
            ServiceContractTypeDelegator serviceContractTypeDelegator = new ServiceContractTypeDelegator(typeof(TestClass));

            customAttributes = serviceContractTypeDelegator.GetCustomAttributes(typeof(TestCustomAttribute), false);
            Assert.IsNotNull(customAttributes, "Custom attribute should not be null");
            Assert.AreEqual(0, customAttributes.Length, "Expected zero custom attribute");

            customAttributes = serviceContractTypeDelegator.GetCustomAttributes(typeof(TestCustomAttribute), true);
            Assert.IsNotNull(customAttributes, "Custom attribute should not be null");
            Assert.AreEqual(1, customAttributes.Length, "Expected one custom attribute");

            customAttributes = serviceContractTypeDelegator.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            Assert.IsNotNull(customAttributes, "Custom attribute should not be null");
            Assert.AreEqual(1, customAttributes.Length, "Expected one service contract attribute");

            customAttributes = serviceContractTypeDelegator.GetCustomAttributes(typeof(ServiceContractAttribute), true);
            Assert.IsNotNull(customAttributes, "Custom attribute should not be null");
            Assert.AreEqual(1, customAttributes.Length, "Expected one service contract attribute");
        }

        #endregion

    }
}
