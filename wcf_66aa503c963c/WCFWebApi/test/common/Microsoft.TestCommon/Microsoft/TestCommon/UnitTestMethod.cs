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
    /// Class that describes a unit test method (marked with <see cref="TestMethodAttribute"/> that belongs to
    /// a <see cref="UnitTestClass"/>.
    /// </summary>
    public class UnitTestMethod
    {
        private static readonly string typeIsCorrectMethodName = "TypeIsCorrect";
        private static readonly string[] genericSuffixes = new string[] { "OfT", "[T]", "<T>", "Generic" };

        private const BindingFlags publicDeclaredInstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        private const BindingFlags publicOrPrivateDeclaredInstanceBindingFlags = publicDeclaredInstanceBindingFlags | BindingFlags.NonPublic;
        private const BindingFlags publicOrPrivateStaticOrInstance = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags publicOrPrivateInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private MemberInfo memberUnderTest;
        private bool triedToExtractMemberUnderTest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestMethod"/> class.
        /// </summary>
        /// <param name="unitTestClass">The <see cref="UnitTestClass"/> containing this method.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> of this method.</param>
        public UnitTestMethod(UnitTestClass unitTestClass, MethodInfo methodInfo)
        {
            if (unitTestClass == null)
            {
                throw new ArgumentNullException("unitTestClass");
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            this.UnitTestClass = unitTestClass;
            this.MethodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the <see cref="UnitTestClass"/> that declared this method.
        /// </summary>
        public UnitTestClass UnitTestClass { get; private set; }

        /// <summary>
        /// Gets the type being tested by this method's unit test class.
        /// </summary>
        public Type TypeUnderTest { get { return this.UnitTestClass.TypeUnderTest; } }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of this method.
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> of the product type's member
        /// tested by this method.
        /// </summary>
        /// <remarks>
        /// This value may be <c>null</c> is the unit test method incorrectly specifies
        /// the member.  In this case, <see cref="ErrorMessages"/> contains more detailed
        /// information about the possible issues.
        /// </remarks>
        public MemberInfo MemberUnderTest
        {
            get
            {
                if (this.memberUnderTest == null && !this.triedToExtractMemberUnderTest)
                {
                    this.triedToExtractMemberUnderTest = true;
                    this.memberUnderTest = this.ExtractMemberUnderTest();
                }

                return this.memberUnderTest;
            }
        }

        /// <summary>
        /// Gets the friendly name of the member tested by this method.
        /// </summary>
        public string MemberUnderTestName
        {
            get
            {
                return this.UnitTestClass.DisplayNameOfMemberInfo(this.MemberUnderTest);
            }
        }

        /// <summary>
        /// Gets the friendly name of the type tested by the class containing this method.
        /// </summary>
        public string TypeUnderTestName
        {
            get
            {
                return UnitTestClass.DisplayNameOfType(this.TypeUnderTest);
            }
        }

        /// <summary>
        /// Gets the friendly name of this method.
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0}.{1}", this.MethodInfo.DeclaringType.Name, this.MethodInfo.Name);
            }
        }

        /// <summary>
        /// Gets the list of errors encountered attempting to discover <see cref="MemberUnderTesst"/>.
        /// </summary>
        public List<string> ErrorMessages { get; private set; }

        private void AddErrorMessage(string message)
        {
            List<string> errorMessages = this.ErrorMessages;
            if (errorMessages == null)
            {
                errorMessages = new List<string>();
                this.ErrorMessages = errorMessages;
            }

            errorMessages.Add(message);
        }

        private MemberInfo ExtractMemberUnderTest()
        {
            MemberInfo memberInfo = null;
            DescriptionAttribute descriptionAttribute = this.MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
            if (descriptionAttribute == null)
            {
                this.AddErrorMessage(
                    string.Format(
                        "Test method {0} must have a [Description] attribute to specify the member under test.",
                        this.Name));
            }
            else
            {
                memberInfo = this.GetMemberInfoFromDescription(descriptionAttribute.Description);
            }
            return memberInfo;
        }

        private MemberInfo GetMemberInfoFromDescription(string description)
        {
            // Open paren is taken as end of method name
            int openParen = description.IndexOf('(');
            int endOfName = (openParen > 0)
                                ? openParen
                                : description.IndexOf(' ');

            if (endOfName < 0)
            {
                endOfName = description.Length - 1;
            }

            bool hasParameterList = endOfName == openParen;

            string parametersString = null;
            string memberName = description.Substring(0, endOfName).Trim();

            string baseName;
            IEnumerable<string> genericArguments;
            bool isGenericName = TryMatchAsGenericName(memberName, out baseName, out genericArguments);
            if (isGenericName)
            {
                memberName = baseName;
            }

            // Normalize generic type names to "Xxx<T>" form to match ctors below
            string normalizedTypeName = GetTypeNameWithoutGeneric(this.TypeUnderTest);

            // Special case TypeIsCorrect to return the TypeInfo
            if (this.MethodInfo.Name.Equals(typeIsCorrectMethodName))
            {
                if (!normalizedTypeName.Equals(memberName, StringComparison.OrdinalIgnoreCase))
                {
                    this.AddErrorMessage(string.Format("The {0} [Description] must begin with the type under test.", this.Name));
                }

                return this.TypeUnderTest;
            }

            // If we had a parameter list, extract it
            if (hasParameterList)
            {
                int closeParen = description.IndexOf(')', openParen + 1);
                if (closeParen < 0)
                {
                    this.AddErrorMessage(string.Format("Missing ')' in signature in [Description] for {0}", this.Name));
                    return null;
                }

                parametersString = description.Substring(openParen + 1, closeParen - openParen - 1);
                parametersString = parametersString.Trim();
            }

            UnitTestTypeReference[] parameterList = string.IsNullOrWhiteSpace(parametersString)
                                                        ? new UnitTestTypeReference[0]
                                                        : UnitTestTypeReference.ParseList(parametersString).ToArray();

            // If it matches the type name, it is treated as a ctor
            if (string.Equals(normalizedTypeName, memberName, StringComparison.OrdinalIgnoreCase))
            {
                ConstructorInfo constructorInfo = null;
                if (this.TryMatchAsConstructor(parameterList, genericArguments, out constructorInfo))
                {
                    return constructorInfo;
                }

                this.AddErrorMessage(
                    string.Format(
                        "Type '{0}' has no ctor with the signature '{1}' specified in [Description] for '{2}'.",
                        this.TypeUnderTestName,
                        string.IsNullOrWhiteSpace(parametersString) ? "<empty>" : parametersString,
                        this.Name));

                string availableOverloads = GenerateListOfExistingMethods(this.TypeUnderTestName, this.TypeUnderTest.GetConstructors(publicOrPrivateInstance).Cast<MethodBase>());
                if (!string.IsNullOrWhiteSpace(availableOverloads))
                {
                    this.AddErrorMessage(string.Format("  Available overloads include:{0}{1}", Environment.NewLine, availableOverloads));
                }

                return null;
            }

            // Try to match as a method, even if it has no parameter list
            MethodInfo methodInfo = null;
            if (this.TryMatchAsMethod(memberName, isGenericName, parameterList, hasParameterList, genericArguments, out methodInfo))
            {
                return methodInfo;
            }

            // Not a ctor or method -- if it has no parameter list, try as a property
            if (!hasParameterList)
            {
                PropertyInfo propertyInfo = null;
                if (this.TryMatchAsProperty(memberName, out propertyInfo))
                {
                    return propertyInfo;
                }

                this.AddErrorMessage(
                    string.Format(
                        "Type'{0}' has no property named '{1}' as specified in the [Description] for '{2}'.",
                        this.TypeUnderTestName,
                        memberName,
                        this.Name));
            }

            return null;
        }

        private static bool TryMatchAsGenericName(string name, out string baseName, out IEnumerable<string> genericArguments)
        {
            baseName = name;
            genericArguments = null;
            int pos = name.IndexOf('<');
            if (pos >= 0)
            {
                baseName = name.Substring(0, pos);
                int endPos = name.LastIndexOf('>');
                if (endPos > pos)
                {
                    string generics = name.Substring(pos+1, endPos - pos - 1);
                    string[] genericNames = generics.Split(',');
                    genericArguments = genericNames.Select((s) => s.Trim());
                    return true;
                }
            }

            return false;
        }

        private static string GetTypeNameWithoutGeneric(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                int genericPos = name.IndexOf('`');
                if (genericPos >= 0)
                {
                    name = name.Substring(0, genericPos);
                }
            }

            return name;
        }

        private bool TryMatchAsConstructor(UnitTestTypeReference[] parameterList, IEnumerable<string> genericArguments, out ConstructorInfo constructorInfo)
        {
            constructorInfo = null;

            // Static ctors ignored in current implementation
            ConstructorInfo[] constructors = this.TypeUnderTest.GetConstructors(publicOrPrivateInstance);

            // Accept empty parameter list if only overload
            if (constructors.Length == 1 && parameterList.Length == 0)
            {
                constructorInfo = constructors[0];
                return true;
            }

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == parameterList.Length)
                {
                    int matchCount = 0;
                    for (int i = 0; i < parameters.Length; ++i)
                    {
                        // We prefer an exact type match from the ctor parameters
                        if (parameterList[i].DoesTypeMatch(parameters[i].ParameterType))
                        {
                            ++matchCount;
                            continue;
                        }

                        // Lacking that, we accept a generic type that came from the description,
                        // such as "MyMethod<T>(T, int)" will accept "T"
                        if ((parameters[i].ParameterType.IsGenericParameter || parameters[i].ParameterType == typeof(object)) &&
                            genericArguments != null &&
                            genericArguments.Any((s) => string.Equals(s, parameterList[i].Name, StringComparison.Ordinal)))
                        {
                            ++matchCount;
                        }
                    }

                    if (matchCount == parameters.Length)
                    {
                        constructorInfo = constructor;
                        return true;
                    }
                }
            }

            return false;
        }

        private MethodInfo[] GetAvailableMethods(string memberName, bool isGenericName)
        {
            Func<MethodInfo, bool> predicate = (m) =>
                {
                    if (m.IsGenericMethod != isGenericName)
                    {
                        return false;
                    }

                    string methodName = m.Name;

                    // Allow for full match of dotted names to include explicit interface methods
                    if (string.Equals(methodName, memberName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    // But also allow match of the base method name without the explicit interface prefix
                    methodName = methodName.Substring(methodName.LastIndexOf('.') + 1);
                    return string.Equals(methodName, memberName, StringComparison.OrdinalIgnoreCase);
                };

            HashSet<MethodInfo> hashedMethods = new HashSet<MethodInfo>();
            foreach (MethodInfo method in this.TypeUnderTest.GetMethods(publicOrPrivateStaticOrInstance).Where(predicate))
            {
                hashedMethods.Add(method);
            }

            for (Type t = this.TypeUnderTest; t != typeof(object); t = t.BaseType)
            {
                // Add the methods for interfaces as well to cover explicit and implicit interface implementations
                foreach (Type implementedInterface in t.GetInterfaces())
                {
                    // Add all interfaces' methods
                    foreach (MethodInfo method in implementedInterface.GetMethods(publicOrPrivateStaticOrInstance).Where(predicate))
                    {
                        hashedMethods.Add(method);
                    }
                }
            }

            return hashedMethods.ToArray();
        }

        private bool TryMatchAsMethod(string memberName, bool isGenericName, UnitTestTypeReference[] parameterList, bool errorIfNoMatch, IEnumerable<string> genericArguments, out MethodInfo methodInfo)
        {
            methodInfo = null;
            MethodInfo[] methods = this.GetAvailableMethods(memberName, isGenericName);
            if (methods.Length == 0)
            {
                if (errorIfNoMatch)
                {
                    this.AddErrorMessage(string.Format(
                        "The type '{0}' has no method call '{1}'.",
                        this.TypeUnderTestName,
                        memberName));
                }

                return false;
            }

            // Accept empty parameter list if only overload
            if (methods.Length == 1 && parameterList.Length == 0)
            {
                methodInfo = methods[0];
                return true;
            }

            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == parameterList.Length)
                {
                    int matchCount = 0;
                    for (int i = 0; i < parameters.Length; ++i)
                    {
                        if (parameterList[i].DoesTypeMatch(parameters[i].ParameterType))
                        {
                            ++matchCount;
                            continue;
                        }

                        // Lacking that, we accept a generic type that came from the description,
                        // such as "MyMethod<T>(T, int)" will accept "T"
                        if ((parameters[i].ParameterType.IsGenericParameter || parameters[i].ParameterType == typeof(object)) &&
                            genericArguments != null &&
                            genericArguments.Any((s) => string.Equals(s, parameterList[i].Name, StringComparison.Ordinal)))
                        {
                            ++matchCount;
                        }
                    }

                    if (matchCount == parameters.Length)
                    {
                        methodInfo = method;
                        return true;
                    }
                }
            }

            if (errorIfNoMatch)
            {
                this.AddErrorMessage(string.Format(
                    "No overload of '{0}.{1}' has the parameter list '{2}' specified in [Description] for '{3}'.",
                    this.TypeUnderTestName,
                    memberName,
                    UnitTestTypeReference.FormatList(parameterList),
                    this.Name));

                string availableOverloads = GenerateListOfExistingMethods(memberName, this.GetAvailableMethods(memberName, isGenericName).Cast<MethodBase>());
                if (!string.IsNullOrWhiteSpace(availableOverloads))
                {
                    this.AddErrorMessage(string.Format("  Available overloads include:{0}{1}", Environment.NewLine, availableOverloads));
                }
            }

            return false;
        }

        private bool TryMatchAsProperty(string memberName, out PropertyInfo propertyInfo)
        {
            propertyInfo = null;

            foreach (PropertyInfo property in this.TypeUnderTest.GetProperties(publicOrPrivateStaticOrInstance))
            {
                if (memberName.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    propertyInfo = property;
                    return true;
                }
            }

            return false;
        }

        private static string GenerateListOfExistingMethods(string memberName, IEnumerable<MethodBase> methods)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MethodBase methodBase in methods)
            {
                string parameters = UnitTestClass.DisplayNameOfParameterList(methodBase);
                sb.AppendLine(string.Format("            {0}({1})", memberName, parameters));
            }

            return sb.ToString();
        }
    }
}
