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

    public static class StandardRuleSet
    {
        public const string ClassesTestSameAssemblyRuleName = "UTSTD001";
        public const string OneTestAssemblyPerProductAssemblyRuleName = "UTSTD002";
        public const string OneTestClassPerProductTypeRuleName = "UTSTD003";
        public const string MininumPublicTypesRuleName = "UTSTD004";
        public const string MininumNonPublicTypesRuleName = "UTSTD005";

        public const string ClassesTestSameAssemblyDescription = "All classes in a unit test suite must test types from the same assembly.";
        public const string OneTestAssemblyPerProductAssemblyDescription = "Only one test assembly may test a single product assembly.";
        public const string OneTestClassPerProductTypeDescription = "Only one test class may test a single product type.";
        public const string MinimumPublicTypesDescription = "This test suite does not test the minimum required number of public types.";
        public const string MinimumNonPublicTypesDescription = "This test suite does not test the minimum required number of non-public types.";

        public const string ClassRequiresTypeIsCorrectMethodRuleName = "UTSTD500";
        public const string MethodsRequireDescriptionRuleName = "UTSTD501";
        public const string MethodsRequireMemberInDescriptionRuleName = "UTSTD502";
        public const string MininumPublicMembersRuleName = "UTSTD503";
        public const string MininumNonPublicMembersRuleName = "UTSTD504";
        public const string NotReadyTestsFailRuleName = "UTSTD504";

        public const string ClassRequiresTypeIsCorrectMethodDescription = "All test classes must have a test method called 'TypeIsCorrect' to verify visibility and inheritance.";
        public const string MethodsRequireDescriptionDescription = "The [Description] attribute is required on all unit test methods.";
        public const string MethodsRequireMemberInDescriptionDescription = "The [Description] attribute must begin with the name of the member it tests.";
        public const string MinimumPublicMembersDescription = "This test class does not test the minimum required number of public members.";
        public const string MinimumNonPublicMembersDescription = "This test class does not test the minimum required number of non-public members.";
        public const string NotReadyTestsFailDescription = "Test classes marked UnitTestLevel.NotReady assert inconclusive even if error free.";

        private const string StandardRuleSetSuiteCategory = "Standard Suite Rule";
        private const string StandardUnitTestRuleSetClassCategory = "Standard Class Rule";

        [UnitTestRule(UnitTestLevel.NotReady, Category = StandardRuleSetSuiteCategory, Name = ClassesTestSameAssemblyRuleName, Description = ClassesTestSameAssemblyDescription)]
        public static UnitTestRuleResult ClassesTestSameAssembly(UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            UnitTestClass firstClassInfo = null;
            foreach (UnitTestClass info in unitTestContext.UnitTestClasses)
            {
                if (firstClassInfo == null)
                {
                    firstClassInfo = info;
                }
                else
                {
                    if (info.TypeUnderTest.Assembly != firstClassInfo.TypeUnderTest.Assembly)
                    {
                        sb.AppendLine(
                            string.Format(
                                "    Test class '{0}' tests type '{1}' in assembly '{2}', but '{3}' tests type '{4}' in assembly '{5}'.",
                                UnitTestClass.DisplayNameOfType(firstClassInfo.UnitTestType),
                                UnitTestClass.DisplayNameOfType(firstClassInfo.TypeUnderTest),
                                firstClassInfo.TypeUnderTest.Assembly.GetName().Name,
                                UnitTestClass.DisplayNameOfType(info.UnitTestType),
                                UnitTestClass.DisplayNameOfType(info.TypeUnderTest),
                                info.TypeUnderTest.Assembly.GetName().Name));
                    }
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardRuleSetSuiteCategory, Name = OneTestAssemblyPerProductAssemblyRuleName, Description = OneTestAssemblyPerProductAssemblyDescription)]
        public static UnitTestRuleResult OneTestAssemblyPerProductAssembly(UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<Assembly> otherTestAssemblies = UnitTestContext.TestAssemblies.Where((a) => a != unitTestContext.TestAssembly);
            foreach (Assembly testAssembly in otherTestAssemblies)
            {
                UnitTestContext otherContext = UnitTestContext.GetOrCreateUnitTextContext(testAssembly);
                if (otherContext.AssemblyUnderTest == unitTestContext.AssemblyUnderTest)
                {
                    sb.AppendLine(
                        string.Format(
                            "    Test assemblies '{0}' and '{1}' both test the same '{2}' product assembly.",
                            unitTestContext.TestAssembly.GetName().Name,
                            otherContext.TestAssembly.GetName().Name,
                            unitTestContext.AssemblyUnderTest.GetName().Name));
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }


        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardRuleSetSuiteCategory, Name = OneTestClassPerProductTypeRuleName, Description = OneTestClassPerProductTypeDescription)]
        public static UnitTestRuleResult OneTestClassPerProductType(UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Type, UnitTestClass> testClassesByType = new Dictionary<Type, UnitTestClass>();
            foreach (UnitTestClass testClass in unitTestContext.UnitTestClasses)
            {
                UnitTestClass otherClass = null;
                Type typeUnderTest = testClass.TypeUnderTest;
                if (testClassesByType.TryGetValue(typeUnderTest, out otherClass))
                {
                    sb.AppendLine(
                        string.Format(
                            "    Test classes '{0}' and '{1}' both test the same product type '{2}'.",
                            UnitTestClass.DisplayNameOfType(testClass.UnitTestType),
                            UnitTestClass.DisplayNameOfType(otherClass.UnitTestType),
                            UnitTestClass.DisplayNameOfType(typeUnderTest)));

                }
                else
                {
                    testClassesByType[typeUnderTest] = testClass;
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }


        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardRuleSetSuiteCategory, Name = MininumPublicTypesRuleName, Description = MinimumPublicTypesDescription)]
        public static UnitTestRuleResult MinimumPublicTypesTested(UnitTestContext unitTestContext)
        {
            return EvaluateTypeCoverage(unitTestContext, isPublic: true);
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardRuleSetSuiteCategory, Name = MininumPublicTypesRuleName, Description = MinimumNonPublicTypesDescription)]
        public static UnitTestRuleResult MinimumNonPublicTypesTested(UnitTestContext unitTestContext)
        {
            return EvaluateTypeCoverage(unitTestContext, isPublic: false);
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardUnitTestRuleSetClassCategory, Name = MethodsRequireDescriptionRuleName, Description = MethodsRequireDescriptionDescription)]
        public static UnitTestRuleResult MethodsRequireDescriptionAttribute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                DescriptionAttribute descriptionAttribute = method.MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
                if (descriptionAttribute == null)
                {
                    sb.AppendLine(string.Format("    Missing from '{0}'.", method.Name));
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardUnitTestRuleSetClassCategory, Name = ClassRequiresTypeIsCorrectMethodRuleName, Description = ClassRequiresTypeIsCorrectMethodDescription)]
        public static UnitTestRuleResult ClassRequiresTypeIsCorrectMethod(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                if (string.Equals("TypeIsCorrect", method.MethodInfo.Name, StringComparison.Ordinal))
                {
                    return UnitTestRuleResult.Success;
                }
            }

            string message = string.Format("    The test class '{0}' does not contain a test method called TypeIsCorrect.", UnitTestClass.DisplayNameOfType(unitTestClass.UnitTestType));
            return new UnitTestRuleResult(message, UnitTestIssueLevel.Error);
        }

        [UnitTestRule(UnitTestLevel.InProgress, Category = StandardUnitTestRuleSetClassCategory, Name = MethodsRequireMemberInDescriptionRuleName, Description = MethodsRequireMemberInDescriptionDescription)]
        public static UnitTestRuleResult MethodsRequireMemberInDescriptionAttribute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UnitTestMethod method in unitTestClass.TestMethods)
            {
                DescriptionAttribute descriptionAttribute = method.MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
                if (descriptionAttribute != null)
                {
                    MemberInfo memberUnderTest = method.MemberUnderTest;
                    if (memberUnderTest == null)
                    {
                        sb.AppendLine(string.Format("    '{0}' incorrectly specified its member using [Description(\"{1}\")].", method.Name, descriptionAttribute.Description));
                        if (method.ErrorMessages != null)
                        {
                            foreach (string error in method.ErrorMessages)
                            {
                                sb.AppendLine(string.Format("        {0}", error));
                            }
                        }
                    }
                }
            }

            return (sb.Length > 0) ? new UnitTestRuleResult(sb.ToString(), UnitTestIssueLevel.Error) : UnitTestRuleResult.Success;
        }

        [UnitTestRule(UnitTestLevel.NotReady, Category = StandardRuleSetSuiteCategory, Name = MininumPublicMembersRuleName, Description = MinimumPublicMembersDescription)]
        public static UnitTestRuleResult MinimumPublicMembersTested(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            return EvaluateMemberCoverage(unitTestClass, unitTestContext, isPublic: true);
        }

        [UnitTestRule(UnitTestLevel.NotReady, Category = StandardRuleSetSuiteCategory, Name = MininumNonPublicMembersRuleName, Description = MinimumNonPublicMembersDescription)]
        public static UnitTestRuleResult MinimumNonPublicMembersTested(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            return EvaluateMemberCoverage(unitTestClass, unitTestContext, isPublic: false);
        }

        [UnitTestRule(UnitTestLevel.NotReady, Category = StandardRuleSetSuiteCategory, Name = NotReadyTestsFailRuleName, Description = NotReadyTestsFailDescription)]
        public static UnitTestRuleResult NotReadyClassesFail(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
        {
            if (unitTestClass.UnitTestLevel == UnitTestLevel.NotReady)
            {
                string message = string.Format("    Test class '{0}' is marked UnitTestLevel.NotReady.", UnitTestClass.DisplayNameOfType(unitTestClass.UnitTestType));
                return new UnitTestRuleResult(message, UnitTestIssueLevel.Inconclusive);
            }

            return UnitTestRuleResult.Success;
        }

        private static string FormatMemberInfoList(UnitTestClass unitTestClass, IEnumerable<MemberInfo> availableMembers)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> ctors = availableMembers.OfType<ConstructorInfo>()
                            .Select<ConstructorInfo, string>((c) => unitTestClass.DisplayNameOfMemberInfo(c))
                            .OrderBy((s) => s);

            if (ctors.Any())
            {
                sb.AppendLine("        Constructors:");
                foreach (string ctor in ctors)
                {
                    sb.AppendLine(string.Format("            {0}", ctor));
                }
            }

            IEnumerable<string> properties = availableMembers.OfType<PropertyInfo>()
                    .Select<PropertyInfo, string>((c) => unitTestClass.DisplayNameOfMemberInfo(c))
                    .OrderBy((s) => s);

            if (properties.Any())
            {
                sb.AppendLine("        Properties:");
                foreach (string property in properties)
                {
                    sb.AppendLine(string.Format("            {0}", property));
                }
            }

            IEnumerable<string> methods = availableMembers.OfType<MethodInfo>()
           .Select<MethodInfo, string>((c) => unitTestClass.DisplayNameOfMemberInfo(c))
           .OrderBy((s) => s);

            if (methods.Any())
            {
                sb.AppendLine("        Methods:");
                foreach (string method in methods)
                {
                    sb.AppendLine(string.Format("            {0}", method));
                }
            }

            return sb.ToString();
        }

        private static UnitTestRuleResult EvaluateTypeCoverage(UnitTestContext unitTestContext, bool isPublic)
        {
            int minCoverage = isPublic ? unitTestContext.PublicTypeMinimumCoverage : unitTestContext.NonPublicTypeMinimumCoverage;

            if (minCoverage > 0)
            {
                Func<Type, bool> predicate;
                string publicOrNonPublicString;
                if (isPublic)
                {
                    predicate = (t) => t.IsPublic;
                    publicOrNonPublicString = "public";
                }
                else
                {
                    predicate = (t) => t.IsNotPublic;
                    publicOrNonPublicString = "non-public";
                }

                List<Type> availableTypes = unitTestContext.ProductTypes.Where(predicate).ToList();
                int totalAvailable = availableTypes.Count;

                // Exclude from consideration any explicitly excluded with [UnitTestExclude]
                int countExcluded = 0;
                foreach (Type excludedType in unitTestContext.ExcludedProductTypes)
                {
                    if (availableTypes.Remove(excludedType))
                    {
                        ++countExcluded;
                    }
                }

                if (countExcluded > 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format(
                            "      {0} of {1} {2} types ({3}%) were excluded from coverage validation with [UnitTestExclude]",
                            countExcluded,
                            totalAvailable,
                            publicOrNonPublicString,
                            (countExcluded * 100) / totalAvailable));
                }

                if (availableTypes.Count > 0)
                {
                    List<Type> testedTypes = unitTestContext.UnitTestClasses.Select<UnitTestClass, Type>((c) => c.TypeUnderTest).Where(predicate).ToList();

                    int available = availableTypes.Count;
                    int tested = testedTypes.Count;
                    int percentCovered = (tested * 100) / available;

                    if (percentCovered < minCoverage)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Type t in availableTypes)
                        {
                            if (!testedTypes.Contains(t))
                            {
                                sb.AppendLine(string.Format("      {0}", UnitTestClass.DisplayNameOfType(t)));
                            }
                        }

                        return new UnitTestRuleResult(
                                string.Format(
                                "    Coverage of {0} types is currently {1}% which is below the minimum required of {2}%.  Untested types are:{3}{4}",
                                    publicOrNonPublicString,
                                    percentCovered,
                                    minCoverage,
                                    Environment.NewLine,
                                    sb.ToString()),
                               unitTestContext.UnitTestLevel == UnitTestLevel.Complete ? UnitTestIssueLevel.Error : UnitTestIssueLevel.Warning);
                    }
                }
            }

            return UnitTestRuleResult.Success;
        }

        private static UnitTestRuleResult EvaluateMemberCoverage(UnitTestClass unitTestClass, UnitTestContext unitTestContext, bool isPublic)
        {
            int minCoverage = isPublic ? unitTestClass.PublicMinimumMemberCoverage : unitTestClass.NonPublicMinimumMemberCoverage;

            if (minCoverage > 0)
            {
                Func<MethodBase, bool> isMethodTestable;
                string publicOrNonPublicString;

                if (isPublic)
                {
                    isMethodTestable = (m) => m != null && m.IsPublic && !m.IsAbstract && m.DeclaringType == unitTestClass.TypeUnderTest;
                    publicOrNonPublicString = "public";
                }
                else
                {
                    isMethodTestable = (m) => m != null && !m.IsPublic && !m.IsPrivate && !m.IsAbstract && m.DeclaringType == unitTestClass.TypeUnderTest;
                    publicOrNonPublicString = "non-public";
                };

                Func<MemberInfo, bool> predicate = (m) =>
                {
                    PropertyInfo propertyInfo = m as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        MethodInfo getMethod = propertyInfo.GetGetMethod();
                        MethodInfo setMethod = propertyInfo.GetSetMethod();

                        return (!propertyInfo.IsSpecialName && (isMethodTestable(getMethod) || isMethodTestable(setMethod)));
                    }

                    MethodBase methodBase = m as MethodBase;
                    if (methodBase != null)
                    {
                        if (!isMethodTestable(methodBase))
                        {
                            return false;
                        }

                        return !methodBase.IsSpecialName || methodBase is ConstructorInfo;
                    }

                    return false;
                };

                StringBuilder sb = new StringBuilder();
                int tested = 0;

                BindingFlags bindingFlags = isPublic
                    ? BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
                    : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
                List<MemberInfo> availableMembers = unitTestClass.TypeUnderTest.GetMembers(bindingFlags).Where(predicate).ToList();
                int available = availableMembers.Count;

                if (available > 0)
                {
                    List<MemberInfo> testedMembers = new List<MemberInfo>();
                    foreach (UnitTestMethod method in unitTestClass.TestMethods)
                    {
                        MemberInfo memberInfo = method.MemberUnderTest;
                        if (memberInfo != null)
                        {
                            if (predicate(memberInfo))
                            {
                                if (availableMembers.Remove(memberInfo))
                                {
                                    ++tested;
                                }
                            }
                        }
                    }

                    int percentCovered = (tested * 100) / available;

                    if (percentCovered < minCoverage)
                    {
                        string formattedList = FormatMemberInfoList(unitTestClass, availableMembers);
                        if (formattedList.Length > 0)
                        {
                            return new UnitTestRuleResult(
                                string.Format(
                                    "    Coverage of {0} members of type '{1}' by '{2}' is currently {3}% which is below the minimum required of {4}%.  Untested members are:{5}{6}",
                                    publicOrNonPublicString,
                                    UnitTestClass.DisplayNameOfType(unitTestClass.TypeUnderTest),
                                    UnitTestClass.DisplayNameOfType(unitTestClass.UnitTestType),
                                    percentCovered,
                                    minCoverage,
                                    Environment.NewLine,
                                    formattedList),
                                unitTestClass.UnitTestLevel == UnitTestLevel.Complete ? UnitTestIssueLevel.Error : UnitTestIssueLevel.Warning);
                        }
                    }
                }
            }

            return UnitTestRuleResult.Success;
        }
    }
}
