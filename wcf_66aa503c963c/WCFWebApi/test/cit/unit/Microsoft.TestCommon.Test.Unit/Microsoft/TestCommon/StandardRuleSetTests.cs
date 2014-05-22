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
    using Microsoft.TestCommon.Rules;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(StandardRuleSet))]
    public class StandardRuleSetTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("StandardRuleSet type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublic | TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsStatic | TypeAssert.TypeProperties.IsVisible);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        #region ClassesTestSameAssembly

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ClassesTestSameAssembly() returns success when product types are in same assembly.")]
        public void ClassesTestSameAssemblyReturnsSuccessWithTypesInSameAssembly()
        {
            Asserters.Rule.Succeeds(new Type[] { typeof(SameAssemblyTestClass1), typeof(SameAssemblyTestClass2), typeof(SameAssemblyTestSuite) },
                                    new Type[] { typeof(string), typeof(int) },
                                    (context) => StandardRuleSet.ClassesTestSameAssembly(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ClassesTestSameAssembly() returns error when product types are in different assemblies.")]
        public void ClassesTestSameAssemblyReturnsErrorWithTypesFromDifferentAssemblies()
        {
            Asserters.Rule.Fails(new Type[] { typeof(MultipleAssembliesTestClass1), typeof(MultipleAssembliesTestClass2), typeof(MultipleAssembliesTestSuite) },
                                 new Type[] { typeof(string), typeof(MultipleAssembliesTestClass1) },
                                 (context) => StandardRuleSet.ClassesTestSameAssembly(context));
        }

        #endregion ClassesTestSameAssembly

        #region OneTestClassPerProductType

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("OneTestClassPerProductType() returns success when tests don't test same product type.")]
        public void OneTestClassPerProductTypeReturnsSuccessWithTestingDifferentTypes()
        {
            Asserters.Rule.Succeeds(new Type[] { typeof(SameAssemblyTestClass1), typeof(SameAssemblyTestClass2), typeof(SameAssemblyTestSuite) },
                                    new Type[] { typeof(string), typeof(int) },
                                    (context) => StandardRuleSet.OneTestClassPerProductType(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("OneTestClassPerProductType() returns error when 2 tests test the same product type.")]
        public void OneTestClassPerProductTypeReturnsErrorWithTestingSameType()
        {
            Asserters.Rule.Fails(new Type[] { typeof(SameProductTypeTestClass1), typeof(SameProductTypeTestClass2), typeof(SameProductTypeTestSuite) },
                                 new Type[] { typeof(string) },
                                 (context) => StandardRuleSet.OneTestClassPerProductType(context));
        }

        #endregion OneTestClassPerProductType

        #region MinimumPublicTypesTested

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicTypesTested() returns success when tests reach 100% type coverage.")]
        public void MinimumPublicTypesTestedReturnsSuccessWithCompleteCoverage()
        {
            Asserters.Rule.Succeeds(new Type[] { typeof(PublicTypeCoverageTestClass1), typeof(PublicTypeCoverageTestClass2), typeof(PublicTypeCoverageCompleteTestSuite) },
                                    new Type[] { typeof(PublicTypeCoverageProductType1), typeof(PublicTypeCoverageProductType2) },
                                    (context) => StandardRuleSet.MinimumPublicTypesTested(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicTypesTested() returns error when tests have < 100% type coverage.")]
        public void MinimumPublicTypesTestedReturnsErrorWithIncompleteCoverage()
        {
            // Omit test for TypeCoverageProductType2
            Asserters.Rule.Fails(new Type[] { typeof(PublicTypeCoverageTestClass1), typeof(PublicTypeCoverageCompleteTestSuite) },
                                    new Type[] { typeof(PublicTypeCoverageProductType1), typeof(PublicTypeCoverageProductType2) },
                                    (context) => StandardRuleSet.MinimumPublicTypesTested(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicTypesTested() returns warning when tests have < 100% type coverage but are InProgress.")]
        public void MinimumPublicTypesTestedReturnsWarningWithIncompleteCoverage()
        {
            // Omit test for TypeCoverageProductType2
            Asserters.Rule.Warns(new Type[] { typeof(PublicTypeCoverageTestClass1), typeof(PublicTypeCoverageInProgressTestSuite) },
                                    new Type[] { typeof(PublicTypeCoverageProductType1), typeof(PublicTypeCoverageProductType2) },
                                    (context) => StandardRuleSet.MinimumPublicTypesTested(context));
        }

        #endregion MinimumPublicTypesTested

        #region MinimumNonPublicTypesTested

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicTypesTested() returns success when tests reach 100% type coverage.")]
        public void MinimumNonPublicTypesTestedReturnsSuccessWithCompleteCoverage()
        {
            // PrivateProductType is included to prove private types are not counted
            Asserters.Rule.Succeeds(new Type[] { typeof(NonPublicTypeCoverageTestClass1), typeof(NonPublicTypeCoverageTestClass2), typeof(NonPublicTypeCoverageCompleteTestSuite) },
                                    new Type[] { typeof(NonPublicTypeCoverageInternalProductType1), 
                                                 typeof(NonPublicTypeCoverageInternalProductType2),
                                                 typeof(PrivateProductType)},
                                    (context) => StandardRuleSet.MinimumNonPublicTypesTested(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicTypesTested() returns error when tests have < 100% type coverage.")]
        public void MinimumNonPublicTypesTestedReturnsErrorWithIncompleteCoverage()
        {
            // Omit test for NonPublicTypeCoverageProductType2
            // PrivateProductType is included to prove private types are not counted
            Asserters.Rule.Fails(new Type[] { typeof(NonPublicTypeCoverageTestClass1), typeof(NonPublicTypeCoverageCompleteTestSuite) },
                                    new Type[] { typeof(NonPublicTypeCoverageInternalProductType1), 
                                                 typeof(NonPublicTypeCoverageInternalProductType2),
                                                 typeof(PrivateProductType)},
                                    (context) => StandardRuleSet.MinimumNonPublicTypesTested(context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicTypesTested() returns warning when tests have < 100% type coverage but are InProgress.")]
        public void MinimumNonPublicTypesTestedReturnsWarningWithIncompleteCoverage()
        {
            // Omit test for NonPublicTypeCoverageProductType2
            // PrivateProductType is included to prove private types are not counted
            Asserters.Rule.Warns(new Type[] { typeof(NonPublicTypeCoverageTestClass1), typeof(NonPublicTypeCoverageInProgressTestSuite) },
                                    new Type[] { typeof(NonPublicTypeCoverageInternalProductType1), 
                                                 typeof(NonPublicTypeCoverageInternalProductType2),
                                                 typeof(PrivateProductType)},
                                    (context) => StandardRuleSet.MinimumNonPublicTypesTested(context));
        }

        #endregion MinimumNonPublicTypesTested

        #region MinimumPublicMembersTested

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicMembersTested() returns success when tests reach 100% member coverage.")]
        public void MinimumPublicMembersTestedReturnsSuccessWithCompleteCoverage()
        {
            Asserters.Rule.Succeeds(new Type[] { typeof(PublicMemberCoverageCompleteTestClass), typeof(PublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(PublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumPublicMembersTested(testClass, context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicMembersTested() returns error when tests have < 100% member coverage and ask for UnitTestLevel.Complete.")]
        public void MinimumPublicMembersTestedReturnsErrorWithIncompleteCoverage()
        {
            Asserters.Rule.Fails(new Type[] { typeof(PublicMemberCoverageIncompleteTestClass), typeof(PublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(PublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumPublicMembersTested(testClass, context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumPublicMembersTested() returns warning when tests have < 100% type coverage but are InProgress.")]
        public void MinimumPublicMembersTestedReturnsWarningWithIncompleteCoverage()
        {
            Asserters.Rule.Warns(new Type[] { typeof(PublicMemberCoverageIncompleteInProgressTestClass), typeof(PublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(PublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumPublicMembersTested(testClass, context));
        }

        #endregion MinimumPublicMembersTested

        #region MinimumNonPublicMembersTested

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicMembersTested() returns success when tests reach 100% member coverage.")]
        public void MinimumNonPublicMembersTestedReturnsSuccessWithCompleteCoverage()
        {
            Asserters.Rule.Succeeds(new Type[] { typeof(NonPublicMemberCoverageCompleteTestClass), typeof(NonPublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(NonPublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumNonPublicMembersTested(testClass, context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicMembersTested() returns error when tests have < 100% member coverage and ask for UnitTestLevel.Complete.")]
        public void MinimumNonPublicMembersTestedReturnsErrorWithIncompleteCoverage()
        {
            Asserters.Rule.Fails(new Type[] { typeof(NonPublicMemberCoverageIncompleteTestClass), typeof(NonPublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(NonPublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumNonPublicMembersTested(testClass, context));
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("MinimumNonPublicMembersTested() returns warning when tests have < 100% type coverage but are InProgress.")]
        public void MinimumNonPublicMembersTestedReturnsWarningWithIncompleteCoverage()
        {
            Asserters.Rule.Warns(new Type[] { typeof(NonPublicMemberCoverageIncompleteInProgressTestClass), typeof(NonPublicMemberCoverageTestSuite) },
                                    new Type[] { typeof(NonPublicMemberCoverageProductType) },
                                    (testClass, context) => StandardRuleSet.MinimumNonPublicMembersTested(testClass, context));
        }

        #endregion MinimumNonPublicMembersTested

        #endregion Methods

        #region Helper Types

        private class PrivateProductType { }

        #endregion Helper Types
    }
}
