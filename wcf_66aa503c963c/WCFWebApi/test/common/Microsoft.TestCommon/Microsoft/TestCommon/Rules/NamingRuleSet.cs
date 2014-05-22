// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class NamingRuleSet
    {
        public const string TestAssemblyStartsWithProductAssemblyRuleName = "UTNAME001";
        public const string TestClassStartsWithProductTypeRuleName = "UTNAME500";

        public const string TestAssemblyStartsWithProductAssemblyDescription = "Test assembly names must begin with the name of the product assembly they test.";
        public const string TestClassStartsWithProductTypeDescription = "Test class names must begin with the name of the product type they test.";

        private const string NamingRuleSetSuiteCategory = "Naming Suite Rule";
        private const string NamingUnitTestRuleSetClassCategory = "Naming Class Rule";

        [UnitTestRule(UnitTestLevel.NotReady, Category = NamingRuleSetSuiteCategory, Name = TestAssemblyStartsWithProductAssemblyRuleName, Description = TestAssemblyStartsWithProductAssemblyDescription)]
        public static UnitTestRuleResult TestAssemblyStartsWithProductAssembly(UnitTestContext unitTestContext)
        {
            string assemblyUnderTestName = unitTestContext.AssemblyUnderTest.GetName().Name;
            string testAssemblyName = unitTestContext.TestAssembly.GetName().Name;
            if (testAssemblyName.StartsWith(assemblyUnderTestName, StringComparison.OrdinalIgnoreCase))
            {
                return UnitTestRuleResult.Success;
            }

            string message = string.Format(
                                "    The name of the assembly '{0}' must start with '{1}'.",
                                testAssemblyName,
                                assemblyUnderTestName);

            return new UnitTestRuleResult(message, UnitTestIssueLevel.Error);
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = NamingRuleSetSuiteCategory, Name = TestClassStartsWithProductTypeRuleName, Description = TestClassStartsWithProductTypeDescription)]
        public static UnitTestRuleResult ClassRequiresTypeIsCorrectMethod(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            string preferredTestClassName = unitTestClass.TypeUnderTest.Name;
            if (unitTestClass.TypeUnderTest.IsGenericType)
            {
                int pos = preferredTestClassName.IndexOf('`');
                if (pos >= 0)
                {
                    preferredTestClassName = preferredTestClassName.Substring(0, pos);
                }

                preferredTestClassName += "OfT";
            }

            preferredTestClassName += "Tests";

            string unitTestClassName = unitTestClass.UnitTestType.Name;

            if (string.Equals(unitTestClassName, preferredTestClassName, StringComparison.OrdinalIgnoreCase))
            {
                return UnitTestRuleResult.Success;
            }

            string message = string.Format(
                                "    The unit test class '{0}' must be named '{1}' because it tests '{2}'.",
                                unitTestClassName,
                                preferredTestClassName,
                                UnitTestClass.DisplayNameOfType(unitTestClass.TypeUnderTest));

            return new UnitTestRuleResult(message, UnitTestIssueLevel.Error);
        }
    }
}
