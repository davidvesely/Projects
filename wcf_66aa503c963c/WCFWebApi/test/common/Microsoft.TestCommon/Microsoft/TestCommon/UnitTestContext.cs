// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.TestCommon.Base;
    using Microsoft.TestCommon.Rules;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class containing all the runtime state and context describing a unit test suite.
    /// </summary>
    public class UnitTestContext
    {
        private static ConcurrentDictionary<Assembly, UnitTestContext> ContextsByAssembly = new ConcurrentDictionary<Assembly, UnitTestContext>();
        private static AssertFailedException StandardAssertFailedException = null;

        private Type unitTestSuiteType;
        private UnitTestSuiteConfigurationAttribute unitTestSuiteConfigurationAttribute;
        private Dictionary<Type, UnitTestClass> unitTestClassesByTestType;
        private IEnumerable<Type> excludedProductTypes;
        private List<UnitTestSuiteRule> unitTestSuiteRules;
        private List<UnitTestClassRule> unitTestClassRules;
        private Assembly assemblyUnderTest;
        private List<Type> unitTestTypes;
        private List<Type> productTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestContext"/> class.
        /// </summary>
        /// <param name="testAssembly"></param>
        public UnitTestContext(Assembly testAssembly)
        {
            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            this.TestAssembly = testAssembly;
        }

        /// <summary>
        /// Gets the list of test assemblies within the current <see cref="AppDomain"/>
        /// which have opened a <see cref="UnitTestContext"/>.
        /// </summary>
        public static IEnumerable<Assembly> TestAssemblies
        {
            get
            {
                return ContextsByAssembly.Keys;
            }
        }

        /// <summary>
        /// Gets the unit test assembly used by this context.
        /// </summary>
        public Assembly TestAssembly { get; private set; }

        /// <summary>
        /// Gets the product assembly tested by this context's unit test assembly.
        /// </summary>
        public Assembly AssemblyUnderTest
        {
            get
            {
                if (this.assemblyUnderTest == null)
                {
                    this.assemblyUnderTest = this.FindAssemblyUnderTest();
                }

                return this.assemblyUnderTest;
            }
        }

        /// <summary>
        /// Gets or sets the minimum percentage of public types the unit test suite must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicTypeMinimumCoverage { get { return this.UnitTestSuiteConfigurationAttribute.PublicTypeMinimumCoverage; } }

        /// <summary>
        /// Gets or sets the minimum percentage of non-public types the unit test suite must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicTypeMinimumCoverage { get { return this.UnitTestSuiteConfigurationAttribute.NonPublicTypeMinimumCoverage; } }

        /// <summary>
        /// Gets or sets the default minimum percentage of public members each unit test class
        /// must test before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicMemberMinimumCoverage { get { return this.UnitTestSuiteConfigurationAttribute.PublicMemberMinimumCoverage; } }

        /// <summary>
        /// Gets or sets the default minimum percentage of non-public members each unit test class
        /// must test before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicMemberMinimumCoverage { get { return this.UnitTestSuiteConfigurationAttribute.NonPublicMemberMinimumCoverage; } }

        /// <summary>
        /// Gets or sets the maximum timeout (milliseconds) any unit test in this
        /// suite may declare.
        /// </summary>
        public int MaxTimeoutMs { get { return this.UnitTestSuiteConfigurationAttribute.MaxTimeoutMs; } }

        /// <summary>
        /// Gets or sets the legal categories for the unit test classes within this
        /// suite.
        /// </summary>
        /// <remarks>
        /// Unit tests declare their category in a <see cref="CategoryAttribute"/>, and
        /// the value they provide must be one from this list.
        /// </remarks>
        public IEnumerable<string> ValidTestCategories { get { return this.UnitTestSuiteConfigurationAttribute.ValidTestCategories; } }

        /// <summary>
        /// Gets or sets the <see cref="UnitTestLevel"/> of the entire suite of unit tests.
        /// </summary>
        public UnitTestLevel UnitTestLevel { get { return this.UnitTestSuiteConfigurationAttribute.UnitTestLevel; } }

        /// <summary>
        /// Gets or sets the default<see cref="UnitTestLevel"/> for every unit test class
        /// which does not supply its own.
        /// </summary>
        public UnitTestLevel DefaultUnitTestClassLevel { get { return this.UnitTestSuiteConfigurationAttribute.DefaultUnitTestClassLevel; } }

        /// <summary>
        /// Gets the set of <see cref="UnitTestClass"/> instances declared by this context's unit test assembly.
        /// </summary>
        public IEnumerable<UnitTestClass> UnitTestClasses
        {
            get
            {
                if (this.unitTestClassesByTestType == null)
                {
                    this.unitTestClassesByTestType = this.GetUnitTestClasses();
                }

                return this.unitTestClassesByTestType.Values;
            }
        }

        /// <summary>
        /// Gets the list of types excluded from test via <see cref="UnitTestExcludedAttribute"/>.
        /// </summary>
        public IEnumerable<Type> ExcludedProductTypes
        {
            get
            {
                if (this.excludedProductTypes == null)
                {
                    this.excludedProductTypes = this.GetExcludedProductTypes();
                }

                return this.excludedProductTypes;
            }
        }

        public IEnumerable<Type> UnitTestTypes
        {
            get
            {
                if (this.unitTestTypes == null)
                {
                    this.unitTestTypes = this.OnGetUnitTestTypes().ToList();
                }

                return this.unitTestTypes;
            }
        }

        public IEnumerable<Type> ProductTypes
        {
            get
            {
                if (this.productTypes == null)
                {
                    this.productTypes = this.OnGetProductTypes().ToList();
                }

                return this.productTypes;
            }
        }

        private static AssertFailedException CachedAssertFailedException
        {
            get
            {
                if (StandardAssertFailedException == null)
                {
                    try
                    {
                        Assert.Fail();
                    }
                    catch (AssertFailedException exception)
                    {
                        StandardAssertFailedException = exception;
                    }
                }

                return StandardAssertFailedException;
            }
        }

        private UnitTestClass this[Type type]
        {
            get
            {
                Assert.IsNotNull(type, "UnitTestContext[Type] requires a non-null type.");

                if (this.unitTestClassesByTestType == null)
                {
                    this.unitTestClassesByTestType = this.GetUnitTestClasses();
                }

                UnitTestClass classInfo = null;
                if (!this.unitTestClassesByTestType.TryGetValue(type, out classInfo))
                {
                    Assert.Fail(string.Format("Type {0} is not one of the unit test classes in this suite.", type.Name));
                }

                return classInfo;
            }
        }

        private Type UnitTestSuiteType
        {
            get
            {
                if (this.unitTestSuiteType == null)
                {
                    this.unitTestSuiteType = this.FindUnitTestSuiteType();
                }

                return this.unitTestSuiteType;
            }
        }

        private UnitTestSuiteConfigurationAttribute UnitTestSuiteConfigurationAttribute
        {
            get
            {
                if (this.unitTestSuiteConfigurationAttribute == null)
                {
                    this.unitTestSuiteConfigurationAttribute = this.FindUnitTestSuiteLevelAttribute();
                }

                return this.unitTestSuiteConfigurationAttribute;
            }
        }

        private List<UnitTestSuiteRule> UnitTestSuiteRules
        {
            get
            {
                if (this.unitTestSuiteRules == null)
                {
                    this.LoadRuleSets();
                }

                return this.unitTestSuiteRules;
            }
        }

        private List<UnitTestClassRule> UnitTestClassRules
        {
            get
            {
                if (this.unitTestClassRules == null)
                {
                    this.LoadRuleSets();
                }

                return this.unitTestClassRules;
            }
        }

        protected virtual IEnumerable<Type> OnGetUnitTestTypes()
        {
            return this.TestAssembly.GetTypes().Where((t) => t.IsPublic && !t.IsAbstract);
        }

        protected virtual IEnumerable<Type> OnGetProductTypes()
        {
            return this.AssemblyUnderTest == null
                    ? Enumerable.Empty<Type>()
                    : this.AssemblyUnderTest.GetTypes();
        }

        protected virtual IEnumerable<MethodInfo> OnGetUnitTestClassMethods(Type unitTestType)
        {
            IEnumerable<MethodInfo> methods = unitTestType.GetMethods(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where((m) => m.DeclaringType == unitTestType && m.GetCustomAttributes(typeof(TestMethodAttribute), false).Any());

            return methods;
        }

        public IEnumerable<MethodInfo> GetUnitTestClassMethods(Type unitTestType)
        {
            if (unitTestType == null)
            {
                throw new ArgumentNullException("unitTestType");
            }

            return this.OnGetUnitTestClassMethods(unitTestType);
        }

        public static UnitTestContext GetOrCreateUnitTextContext(Assembly assembly)
        {
            return ContextsByAssembly.GetOrAdd(assembly, (a) => { return new UnitTestContext(a); });
        }

        public static void ValidateUnitTestClass(Type testClassType)
        {
            UnitTestContext context = GetOrCreateUnitTextContext(testClassType.Assembly);
            context.ExecuteClassRules(testClassType);
        }

        public static void ValidateUnitTestSuite(Type testSuiteType)
        {
            UnitTestContext context = GetOrCreateUnitTextContext(testSuiteType.Assembly);
            context.ExecuteSuiteRules();
        }

        public static Type GetTypeUnderTest(Type unitTestType)
        {
            Type typeUnderTest = null;

            if (unitTestType.BaseType.IsGenericType)
            {
                typeUnderTest = unitTestType.BaseType.GetGenericArguments()[0];
            }
            else
            {
                UnitTestTypeAttribute unitTestTypeAttribute = unitTestType.GetCustomAttributes(typeof(UnitTestTypeAttribute), false).Cast<UnitTestTypeAttribute>().SingleOrDefault();
                if (unitTestTypeAttribute != null)
                {
                    typeUnderTest = unitTestTypeAttribute.Type;
                }
            }

            return typeUnderTest;
        }

        public IEnumerable<UnitTestIssue> ExecuteSuiteRules()
        {
            List<UnitTestIssue> issues = new List<UnitTestIssue>();

            foreach (UnitTestSuiteRule rule in this.UnitTestSuiteRules)
            {
                UnitTestRuleResult result = rule.Execute(this);
                if (result != UnitTestRuleResult.Success)
                {
                    issues.Add(new UnitTestIssue(rule.UnitTestRuleAttribute, result));
                }
            }

            return issues;
        }

        public IEnumerable<UnitTestIssue> ExecuteClassRules(Type testType)
        {
            List<UnitTestIssue> issues = new List<UnitTestIssue>();
            UnitTestClass classInfo = this[testType];

            foreach (UnitTestClassRule rule in this.UnitTestClassRules)
            {
                UnitTestRuleResult result = rule.Execute(classInfo, this);
                if (result != UnitTestRuleResult.Success)
                {
                    issues.Add(new UnitTestIssue(rule.UnitTestRuleAttribute, result));
                }
            }

            return issues;
        }

        public void ReportUnitTestIssues(IEnumerable<UnitTestIssue> issues)
        {
            if (issues.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (UnitTestIssue issue in issues.Where((i) => i.RuleResult.Level == UnitTestIssueLevel.Warning))
                {
                    string message = string.Format("{0} : {1}{2}{3}", issue.UnitTestRuleAttribute.Name, issue.UnitTestRuleAttribute.Description, Environment.NewLine, issue.RuleResult.Message);
                    sb.Append(message);
                }

                if (sb.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unit test validation warning:{0}{1}", Environment.NewLine, sb.ToString()));
                }

                sb.Clear();
                foreach (UnitTestIssue issue in issues.Where((i) => i.RuleResult.Level == UnitTestIssueLevel.Error))
                {
                    string message = string.Format("{0} : {1}{2}{3}", issue.UnitTestRuleAttribute.Name, issue.UnitTestRuleAttribute.Description, Environment.NewLine, issue.RuleResult.Message);
                    sb.Append(message);
                }

                if (sb.Length > 0)
                {
                    Assert.Fail(string.Format("Unit test validation error:{0}{1}", Environment.NewLine, sb.ToString()));
                }

                sb.Clear();
                foreach (UnitTestIssue issue in issues.Where((i) => i.RuleResult.Level == UnitTestIssueLevel.Inconclusive))
                {
                    string message = string.Format("{0} : {1}{2}{3}", issue.UnitTestRuleAttribute.Name, issue.UnitTestRuleAttribute.Description, Environment.NewLine, issue.RuleResult.Message);
                    sb.Append(message);
                }

                if (sb.Length > 0)
                {
                    Assert.Inconclusive(string.Format("Unit test validation inconclusive:{0}{1}", Environment.NewLine, sb.ToString()));
                }
            }
        }

        private List<Tuple<UnitTestRuleAttribute, Action<Type>>> GetRulesToExecute()
        {
            List<Tuple<UnitTestRuleAttribute, Action<Type>>> rules = new List<Tuple<UnitTestRuleAttribute, Action<Type>>>();

            foreach (MethodInfo methodInfo in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                UnitTestRuleAttribute unitTestRuleAttribute = null;
                if (IsMethodAnExecutableRule(methodInfo, out unitTestRuleAttribute))
                {
                    Action<Type> rule = (t) => { methodInfo.Invoke(this, new object[] { t }); };
                    Tuple<UnitTestRuleAttribute, Action<Type>> tuple = new Tuple<UnitTestRuleAttribute, Action<Type>>(unitTestRuleAttribute, rule);
                    rules.Add(tuple);
                }
            }

            return rules;
        }

        private bool IsMethodAnExecutableRule(MethodInfo methodInfo, out UnitTestRuleAttribute unitTestRuleAttribute)
        {
            unitTestRuleAttribute = methodInfo.GetCustomAttributes(typeof(UnitTestRuleAttribute), false).Cast<UnitTestRuleAttribute>().SingleOrDefault();
            if (unitTestRuleAttribute == null)
            {
                return false;
            }

            if (methodInfo.ReturnType != typeof(UnitTestRuleResult))
            {
                return false;
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            return (parameterInfos.Length == 1 &&
                    parameterInfos[0].ParameterType == typeof(Type) &&
                    unitTestRuleAttribute.UnitTestLevel <= this.UnitTestSuiteConfigurationAttribute.UnitTestLevel);
        }

        private static string RemoveStandardAssertFailedPrefix(string message)
        {
            string standardMessage = CachedAssertFailedException.Message;
            if (message.StartsWith(standardMessage, StringComparison.OrdinalIgnoreCase))
            {
                message = message.Substring(standardMessage.Length);
            }

            return message;
        }

        private static UnitTestLevelAttribute GetUnitTestLevelAttributeOfTest(Type testType, UnitTestSuiteConfigurationAttribute suiteLevelAttribute)
        {
            UnitTestLevelAttribute attribute = testType.GetCustomAttributes(typeof(UnitTestLevelAttribute), false).Cast<UnitTestLevelAttribute>().SingleOrDefault();
            if (attribute == null)
            {
                attribute = new UnitTestLevelAttribute(suiteLevelAttribute);
            }

            return attribute;
        }

        private Type FindUnitTestSuiteType()
        {
            IEnumerable<Type> unitTestSuiteTypes = this.UnitTestTypes.Where((t) => typeof(UnitTestSuite).IsAssignableFrom(t));
            if (!unitTestSuiteTypes.Any())
            {
                Assert.Fail("This project must have a single [TestClass] that derives from UnitTestSuite.");
            }

            if (unitTestSuiteTypes.Count() > 1)
            {
                string message = string.Join(Environment.NewLine + "    ", unitTestSuiteTypes.Select<Type, string>((t) => t.Name));
                Assert.Fail(
                    string.Format(
                        "The assembly must contain only one [TestClass] that derives from UnitTestSuite but multiple were found:{0}", 
                        message));
            }

            return unitTestSuiteTypes.First();
        }

        private UnitTestSuiteConfigurationAttribute FindUnitTestSuiteLevelAttribute()
        {
            Type unitTestSuiteType = this.UnitTestSuiteType;
            UnitTestSuiteConfigurationAttribute attribute = unitTestSuiteType.GetCustomAttributes(typeof(UnitTestSuiteConfigurationAttribute), false).Cast<UnitTestSuiteConfigurationAttribute>().SingleOrDefault();
            Assert.IsNotNull(attribute, string.Format("The unit test suite type {0} must have a [{1}] custom attribute.", unitTestSuiteType.Name, typeof(UnitTestSuiteConfigurationAttribute).Name));
            return attribute;
        }

        private void LoadRuleSets()
        {
            this.unitTestSuiteRules = new List<UnitTestSuiteRule>();
            this.unitTestClassRules = new List<UnitTestClassRule>();

            Type[] ruleSetTypes = this.UnitTestSuiteConfigurationAttribute.RuleSetTypes;
            List<Type> ruleSets = new List<Type>();
            if (ruleSetTypes != null)
            {
                ruleSets.AddRange(ruleSetTypes);
            }

            if (!ruleSets.Contains(typeof(StandardRuleSet)))
            {
                ruleSets.Add(typeof(StandardRuleSet));
            }

            foreach (Type ruleSetType in ruleSets)
            {
                this.LoadRuleSet(ruleSetType);
            }
        }

        private void LoadRuleSet(Type ruleSetType)
        {
            IEnumerable<MethodInfo> methods = ruleSetType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where((m) => m.ReturnType == typeof(UnitTestRuleResult));
            foreach (MethodInfo method in methods)
            {
                UnitTestRuleAttribute attribute = method.GetCustomAttributes(typeof(UnitTestRuleAttribute), false).Cast<UnitTestRuleAttribute>().SingleOrDefault();
                if (attribute != null)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(UnitTestContext))
                    {
                        this.unitTestSuiteRules.Add(new UnitTestSuiteRule(method, attribute));
                    }
                    else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(UnitTestClass) && parameters[1].ParameterType == typeof(UnitTestContext))
                    {
                        this.unitTestClassRules.Add(new UnitTestClassRule(method, attribute));
                    }
                }
            }
        }

        private Dictionary<Type, UnitTestClass> GetUnitTestClasses()
        {
            Dictionary<Type, UnitTestClass> classInfos = new Dictionary<Type, UnitTestClass>();
            IEnumerable<Type> unitTestTypes = this.UnitTestTypes.Where((t) => typeof(UnitTest).IsAssignableFrom(t));
            StringBuilder sb = new StringBuilder();

            foreach (Type unitTestType in unitTestTypes)
            {
                Type typeUnderTest = UnitTestContext.GetTypeUnderTest(unitTestType);
                if (typeUnderTest == null)
                {
                    sb.AppendLine(string.Format("The unit test class {0} must declare the type it tests with [{1}].", unitTestType.Name, typeof(UnitTestTypeAttribute).Name));
                    continue;
                }

                UnitTestLevelAttribute unitTestLevelAttribute = unitTestType.GetCustomAttributes(typeof(UnitTestLevelAttribute), false).Cast<UnitTestLevelAttribute>().SingleOrDefault();
                if (unitTestLevelAttribute == null)
                {
                    unitTestLevelAttribute = new UnitTestLevelAttribute(this.DefaultUnitTestClassLevel)
                    { 
                        PublicMemberMinimumCoverage = this.PublicMemberMinimumCoverage, 
                        NonPublicMemberMinimumCoverage = this.NonPublicMemberMinimumCoverage 
                    };
                }

                classInfos.Add(unitTestType, new UnitTestClass(this, unitTestType, typeUnderTest, unitTestLevelAttribute));
            }

            return classInfos;
        }

        private Assembly FindAssemblyUnderTest()
        {
            foreach (UnitTestClass unitTestClass in this.UnitTestClasses)
            {
                return unitTestClass.TypeUnderTest.Assembly;
            }

            Assert.Fail(string.Format("This project requires at least one [TestClass] that derives from {0} to determine the assembly under test.", typeof(UnitTest).Name));
            return null;
        }

        private IEnumerable<Type> GetExcludedProductTypes()
        {
            Type unitTestSuiteType = this.UnitTestSuiteType;
            HashSet<Type> excludedProductTypes = new HashSet<Type>();
            IEnumerable<UnitTestExcludeAttribute> excludeAttributes = unitTestSuiteType.GetCustomAttributes(typeof(UnitTestExcludeAttribute), false).Cast<UnitTestExcludeAttribute>();
            foreach (UnitTestExcludeAttribute excludeAttribute in excludeAttributes)
            {
                foreach (Type t in excludeAttribute.ExcludedProductTypes)
                {
                    excludedProductTypes.Add(t);
                }
            }

            return excludedProductTypes;
        }

        private class UnitTestRule
        {
            public UnitTestRule(MethodInfo methodInfo, UnitTestRuleAttribute unitTestRuleAttribute)
            {
                this.MethodInfo = methodInfo;
                this.UnitTestRuleAttribute = unitTestRuleAttribute;
            }

            protected MethodInfo MethodInfo { get; private set; }

            public UnitTestRuleAttribute UnitTestRuleAttribute { get; private set; }
        }

        private class UnitTestSuiteRule : UnitTestRule
        {
            public UnitTestSuiteRule(MethodInfo methodInfo, UnitTestRuleAttribute unitTestRuleAttribute)
                : base(methodInfo, unitTestRuleAttribute)
            {
            }

            public UnitTestRuleResult Execute(UnitTestContext unitTestContext)
            {   
                try
                {
                    return (UnitTestRuleResult) this.MethodInfo.Invoke(null, new object[] { unitTestContext });
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
            }
        }

        private class UnitTestClassRule : UnitTestRule
        {
            public UnitTestClassRule(MethodInfo methodInfo, UnitTestRuleAttribute unitTestRuleAttribute)
                : base(methodInfo, unitTestRuleAttribute)
            {
            }

            public UnitTestRuleResult Execute(UnitTestClass unitTestClass, UnitTestContext unitTestContext)
            {
                try
                {
                    return (UnitTestRuleResult) this.MethodInfo.Invoke(null, new object[] { unitTestClass, unitTestContext });
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
            }
        }
    }
}
