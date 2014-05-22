// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class that encapsulates the information about a single unit test class
    /// (marked with <see cref="TestClassAttribute"/>) within a unit test suite.
    /// </summary>
    public class UnitTestClass
    {
        private List<UnitTestMethod> testMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestClass"/> class.
        /// </summary>
        /// <param name="unitTestType">The type of the unit test class.</param>
        /// <param name="typeUnderTest">The type tested by that unit test class.</param>
        /// <param name="unitTestLevelAttribute">The <see cref="UnitTestLevelAttribute"/> describing that unit test class's validation level.</param>
        public UnitTestClass(UnitTestContext unitTestContext, Type unitTestType, Type typeUnderTest, UnitTestLevelAttribute unitTestLevelAttribute)
        {
            if (unitTestContext == null)
            {
                throw new ArgumentNullException("unitTestContext");
            }

            if (unitTestType == null)
            {
                throw new ArgumentNullException("unitTestType");
            }

            if (typeUnderTest == null)
            {
                throw new ArgumentNullException("typeUnderTest");
            }

            if (unitTestLevelAttribute == null)
            {
                throw new ArgumentNullException("unitTestLevelAttribute");
            }

            this.UnitTestContext = unitTestContext;
            this.UnitTestType = unitTestType;
            this.TypeUnderTest = typeUnderTest;
            this.UnitTestLevelAttribute = unitTestLevelAttribute;
        }

        public UnitTestContext UnitTestContext { get; private set; }
 
        /// <summary>
        /// Gets the type of the unit test class.
        /// </summary>
        public Type UnitTestType { get; private set; }

        /// <summary>
        /// Gets the type being tested by the unit test class.
        /// </summary>
        public Type TypeUnderTest { get; private set; }

        /// <summary>
        /// Gets the <see cref="UnitTestLevelAttribute"/> associated with this unit test class.
        /// </summary>
        public UnitTestLevelAttribute UnitTestLevelAttribute { get; private set; }

        /// <summary>
        /// Gets the <see cref="UnitTestLevel"/> for the unit test class.
        /// </summary>
        public UnitTestLevel UnitTestLevel { get { return this.UnitTestLevelAttribute.UnitTestLevel; } }

        /// <summary>
        /// Gets the minimum percentage of public members the unit test class must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicMinimumMemberCoverage { get { return this.UnitTestLevelAttribute.PublicMemberMinimumCoverage; } }

        /// <summary>
        /// Gets the minimum percentage of non-public members the unit test class must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicMinimumMemberCoverage { get { return this.UnitTestLevelAttribute.NonPublicMemberMinimumCoverage; } }

        /// <summary>
        /// Gets the collection of <see cref="UnitTestMethod"/> instances used by this unit test class.
        /// </summary>
        public IEnumerable<UnitTestMethod> TestMethods
        {
            get
            {
                if (this.testMethods == null)
                {
                    IEnumerable<MethodInfo> methods = this.UnitTestContext.GetUnitTestClassMethods(this.UnitTestType);
                    this.testMethods = methods.Select<MethodInfo, UnitTestMethod>((m) => new UnitTestMethod(this, m)).ToList();
                }

                return this.testMethods;
            }
        }

        /// <summary>
        /// Returns the friendly name of the specified type.
        /// </summary>
        /// <param name="type">The type whose friendly name is required.</param>
        /// <returns>The name of that type, expanded to look like what the developer
        /// would type.</returns>
        public static string DisplayNameOfType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string typeName = type.Name;
            if (type.IsByRef)
            {
                type = type.HasElementType ? type.GetElementType() : type;
                typeName = "out " + typeName;
            }

            if (!type.IsGenericType)
            {
                return typeName;
            }

            Type[] argumentTypes = type.GetGenericArguments();
            List<string> argumentTypeNames = argumentTypes.Select<Type, string>((t) => DisplayNameOfType(t)).ToList();

            string baseName = type.Name.Substring(0, type.Name.IndexOf('`'));
            return string.Format("{0}<{1}>", baseName, string.Join(",", argumentTypeNames));
        }

        /// <summary>
        /// Returns a formatted set of parameter declarations for the given
        /// method suitable for display to the user.
        /// </summary>
        /// <param name="methodBase">The method or constructor to use.</param>
        /// <returns>A comma-separated list of parameter types formatted for display.</returns>
        public static string DisplayNameOfParameterList(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException("methodBase");
            }

            StringBuilder sb = new StringBuilder();

            ParameterInfo[] parameters = methodBase.GetParameters();
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(DisplayNameOfType(parameters[i].ParameterType));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the friendly name of the given member.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> whose name is required..</param>
        /// <returns>The name of that member as a developer would type it.</returns>
        public string DisplayNameOfMemberInfo(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return "<none>";
            }

            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.Name;
            }

            MethodBase methodBase = memberInfo as MethodBase;
            if (methodBase != null)
            {
                return this.GenerateMethodOrConstructorName(methodBase);
            }

            return "<Unknown>";
        }

        private string GenerateMethodOrConstructorName(MethodBase methodBase)
        {
            string name = (methodBase is ConstructorInfo) ? this.TypeUnderTest.Name : methodBase.Name;

            return string.Format("{0}({1})", name, DisplayNameOfParameterList(methodBase));
        }
    }
}
