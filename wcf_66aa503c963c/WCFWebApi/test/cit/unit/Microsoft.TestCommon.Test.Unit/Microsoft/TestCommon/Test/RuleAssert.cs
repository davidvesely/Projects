// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class RuleAssert
    {
        private static readonly RuleAssert singleton = new RuleAssert();

        public static RuleAssert Singleton { get { return singleton; } }

        public void Succeeds(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(context);
                Assert.AreEqual(UnitTestRuleResult.Success, unitTestRuleResult, "Expected UnitTestRuleResult.Success.");
            });
        }

        public void Succeeds(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestClass, UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (testClass, context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(testClass, context);
                Assert.AreEqual(UnitTestRuleResult.Success, unitTestRuleResult, "Expected UnitTestRuleResult.Success.");
            });
        }

        public void Fails(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(context);
                Assert.AreNotEqual(UnitTestRuleResult.Success, "Expected Error but actual was Success.");
                Assert.IsNotNull(unitTestRuleResult, "Expected non-null UnitTestRuleResult.");
                Assert.AreEqual(UnitTestIssueLevel.Error, unitTestRuleResult.Level, "Expected Error.");
            });
        }

        public void Fails(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestClass, UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (testClass, context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(testClass, context);
                Assert.AreNotEqual(UnitTestRuleResult.Success, "Expected Error but actual was Success.");
                Assert.IsNotNull(unitTestRuleResult, "Expected non-null UnitTestRuleResult.");
                Assert.AreEqual(UnitTestIssueLevel.Error, unitTestRuleResult.Level, "Expected Error.");
            });
        }

        public void Warns(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(context);
                Assert.AreNotEqual(UnitTestRuleResult.Success, "Expected Warning but actual was Success.");
                Assert.IsNotNull(unitTestRuleResult, "Expected non-null UnitTestRuleResult.");
                Assert.AreEqual(UnitTestIssueLevel.Warning, unitTestRuleResult.Level, "Expected Warning.");
            });
        }

        public void Warns(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Func<UnitTestClass, UnitTestContext, UnitTestRuleResult> ruleCallback)
        {
            UnitTest.Asserters.Context.Execute(unitTestTypes, productTypes, (testClass, context) =>
            {
                UnitTestRuleResult unitTestRuleResult = ruleCallback(testClass, context);
                Assert.AreNotEqual(UnitTestRuleResult.Success, "Expected Warning but actual was Success.");
                Assert.IsNotNull(unitTestRuleResult, "Expected non-null UnitTestRuleResult.");
                Assert.AreEqual(UnitTestIssueLevel.Warning, unitTestRuleResult.Level, "Expected Warning.");
            });
        }
    }
}