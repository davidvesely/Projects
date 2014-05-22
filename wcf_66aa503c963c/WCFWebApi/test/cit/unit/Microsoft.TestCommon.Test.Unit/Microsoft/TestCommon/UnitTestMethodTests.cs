// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.CIT.Unit.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTestMethodTests : UnitTest<UnitTestMethod>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("UnitTestMethod type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct ctor that uses generic type from class definition.")]
        public void MemberUnderTestWithCtorUsingTypesGenericSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("Constructor");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find ctor method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.IsTrue(memberInfo is ConstructorInfo, "Should have found ctor member kind.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct ctor that uses generic type aliased from description.")]
        public void MemberUnderTestWithCtorUsingAliasedGenericSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("ConstructorAliased");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find ctor method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.IsTrue(memberInfo is ConstructorInfo, "Should have found ctor member kind.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method that uses generic type from class definition.")]
        public void MemberUnderTestUsingTypesGenericSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesTypeGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesTypeGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesTypeGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method for a generic method.")]
        public void MemberUnderTestUsingGenericMethodSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method for a generic method aliased as T in description.")]
        public void MemberUnderTestUsingAliasedGenericMethodSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesAliasedGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method for a method accepting a nested generic parameter.")]
        public void MemberUnderTestUsingMethodWithNestedGenericsSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType<>);
            Type unitTestType = typeof(GenericTypeTestClass);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesNestedGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesNestedGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesNestedGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method that uses multiple generic type from class definition.")]
        public void MemberUnderTestUsingTypesMultipleGenericSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType2<,>);
            Type unitTestType = typeof(GenericTypeTest2Class);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesTypeGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesTypeGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesTypeGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MemberUnderTest returns correct method for a generic method with multiple generic arguments.")]
        public void MemberUnderTestUsingGenericMethodMultipleSucceeds()
        {
            Type typeUnderTest = typeof(GenericProductType2<,>);
            Type unitTestType = typeof(GenericTypeTest2Class);
            UnitTest.Asserters.Context.Execute(new Type[] { unitTestType }, new Type[] { typeUnderTest }, (context) =>
            {
                UnitTestClass testClass = new UnitTestClass(context, unitTestType, typeUnderTest, new UnitTestLevelAttribute(UnitTestLevel.InProgress));
                MethodInfo methodInfo = unitTestType.GetMethod("MethodUsesGeneric");
                Assert.IsNotNull(methodInfo, "Test failure -- could not find MethodUsesGeneric method");
                UnitTestMethod testMethod = new UnitTestMethod(testClass, methodInfo);
                MemberInfo memberInfo = testMethod.MemberUnderTest;
                Assert.IsNotNull(memberInfo, "Failed to find MemberInfo");
                Assert.AreEqual("MethodUsesGeneric", memberInfo.Name, "Found the wrong MemberInfo.");
            });
        }


        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
