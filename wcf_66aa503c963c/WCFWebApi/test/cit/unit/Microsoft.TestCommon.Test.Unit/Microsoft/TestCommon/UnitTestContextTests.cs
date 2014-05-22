// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.CIT.Unit.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTestContextTests : UnitTest<UnitTestContext>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("UnitTestContext type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [Description("UnitTestContext(Assembly) throws with a null assembly")]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        public void ConstructorThrowsWithNullAssembly()
        {
            Asserters.Exception.ThrowsArgumentNull("testAssembly", () => new UnitTestContext((Assembly)null));
        }

        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ExecuteSuiteRules() returns an empty set of UnitTestIssues")]
        public void ExecuteSuiteRulesReturnsIssues()
        {
            UnitTestContext context = UnitTestContext.GetOrCreateUnitTextContext(this.GetType().Assembly);
            IEnumerable<UnitTestIssue> issues = context.ExecuteSuiteRules();
            Assert.IsNotNull(issues, "Null issues from ExecuteSuiteRules.");
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ExecuteSuiteRules() returns an empty set of UnitTestIssues")]
        public void ExecuteSuiteRulesReturnsIssuesFromMock()
        {
            UnitTestContext context = new MockUnitTestContext(
                                        new Type[] { typeof(SimpleTestClass), typeof(SimpleTestSuite) },
                                        new Type[] { typeof(SimpleProductType) });

            IEnumerable<UnitTestIssue> issues = context.ExecuteSuiteRules();
            Assert.IsNotNull(issues, "Null issues from ExecuteSuiteRules.");

        }

        #endregion Methods
    }
}
