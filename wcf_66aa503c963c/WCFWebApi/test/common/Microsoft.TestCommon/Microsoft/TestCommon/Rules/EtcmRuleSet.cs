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

    public static class EtcmRuleSet
    {
        public const string MethodsRequireTestCategoryRuleName = "UTTE001";
        public const string MethodsRequireOwnerRuleName = "UTTE002";
        public const string MethodsRequireTimeoutRuleName = "UTTE003";

        public const string MethodsRequireTestCategoryDescription = "The [TestCategory] attribute is required on all unit test methods.";
        public const string MethodsRequireOwnerDescription = "The [Owner] attribute is required on all unit test methods.";
        public const string MethodsRequireTimeoutDescription = "The [Timeout] attribute is required on all unit test methods.";

        private const string EtcmRuleSetClassCategory = "ETCM Class Rule";

        [UnitTestRule(UnitTestLevel.InProgress, Category = EtcmRuleSetClassCategory, Name = MethodsRequireTestCategoryRuleName, Description = MethodsRequireTestCategoryDescription)]
        public static UnitTestRuleResult MethodsRequireTestCategoryAttribute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            string categories = string.Join(",", unitTestContext.ValidTestCategories);
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                IEnumerable<TestCategoryAttribute> testCategoryAttributes = method.MethodInfo.GetCustomAttributes(typeof(TestCategoryAttribute), false).Cast<TestCategoryAttribute>();
               
                if (!testCategoryAttributes.Any())
                {
                    sb.AppendLine(string.Format("    Missing from '{0}'.", unitTestClass.UnitTestType.Name, method.Name));
                }
                else
                {
                    foreach (TestCategoryAttribute testCategoryAttribute in testCategoryAttributes)
                    {
                        foreach (string category in testCategoryAttribute.TestCategories)
                        {
                            bool foundCategory = false;
                            foreach (string validTestCategory in unitTestContext.ValidTestCategories)
                            {
                                if (string.Equals(category, validTestCategory))
                                {
                                    foundCategory = true;
                                    break;
                                }
                            }

                            if (!foundCategory)
                            {
                                sb.AppendLine(
                                    string.Format(
                                        "    '{0}' specified category '{1}' but is allowed to specify only one of these: {2}",
                                        method.Name,
                                        category,
                                        categories));

                            }
                        }
                    }
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = EtcmRuleSetClassCategory, Name = MethodsRequireOwnerRuleName, Description = MethodsRequireOwnerDescription)]
        public static UnitTestRuleResult MethodsRequireOwnerAttribute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            string categories = string.Join(",", unitTestContext.ValidTestCategories);
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                OwnerAttribute ownerAttribute = method.MethodInfo.GetCustomAttributes(typeof(OwnerAttribute), false).Cast<OwnerAttribute>().SingleOrDefault();
                if (ownerAttribute == null)
                {
                    sb.AppendLine(string.Format("    Missing from '{0}'.", method.Name));
                }
                else if (string.IsNullOrWhiteSpace(ownerAttribute.Owner))
                {
                    sb.AppendLine(string.Format("    Cannot be blank in '{0}'.", method.Name));
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = EtcmRuleSetClassCategory, Name = MethodsRequireTimeoutRuleName, Description = MethodsRequireTimeoutDescription)]
        public static UnitTestRuleResult MethodsRequireTimeoutAttribute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            string categories = string.Join(",", unitTestContext.ValidTestCategories);
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                TimeoutAttribute timeoutAttribute = method.MethodInfo.GetCustomAttributes(typeof(TimeoutAttribute), false).Cast<TimeoutAttribute>().SingleOrDefault();
                if (timeoutAttribute == null)
                {
                    sb.AppendLine(string.Format("    Missing from '{0}'.", method.Name));
                }
                else
                {
                    if (timeoutAttribute.Timeout <= 0 || timeoutAttribute.Timeout > unitTestContext.MaxTimeoutMs)
                    {
                        sb.AppendLine(
                            string.Format(
                                "    '{0}' specifies a timeout of {1} ms, but it must be between 1 and {2}.", 
                                method.Name, 
                                timeoutAttribute.Timeout, 
                                unitTestContext.MaxTimeoutMs));
                    }
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }
    }
}
