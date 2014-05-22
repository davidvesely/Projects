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
    /// Class that describes a type reference composed from a string such as "int".
    /// This class also represents ref/out types and generic types.
    /// </summary>
    public class UnitTestTypeReference
    {
        private static Dictionary<Type, string> typesToAliases;

        /// <summary>
        /// Gets the name of the type reference
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this reference was preceded by "ref" or "out"
        /// </summary>
        public bool IsByRef { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this type reference had generic arguments
        /// </summary>
        public bool IsGenericType { get; private set; }

        /// <summary>
        /// Gets the generic arguments when <see cref="IsGenericType"/> is <c>true</c>.
        /// </summary>
        public IEnumerable<UnitTestTypeReference> GenericTypes { get; private set; }

        /// <summary>
        /// Determines whether the current <see cref="UnitTestTypeReference"/> is a reference
        /// to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><c>true</c> if it matches</returns>
        public bool DoesTypeMatch(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            bool isTypeByRef = type.IsByRef;
            if (isTypeByRef && type.HasElementType)
            {
                type = type.GetElementType();
            }

            if (isTypeByRef != this.IsByRef || this.IsGenericType != type.IsGenericType)
            {
                return false;
            }

            // We accept name match on short name, full name or C# alias name.
            // Generic names need to strip off the hashed generic suffix
            string alias = GetAliasForType(type);
            string genericBaseName = type.IsGenericType ? type.Name.Substring(0, type.Name.IndexOf('`')) : string.Empty;
            if (!string.Equals(this.Name, type.Name, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Name, type.FullName, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Name, genericBaseName, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Name, alias, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (this.IsGenericType)
            {
                UnitTestTypeReference[] genericReferences = this.GenericTypes.ToArray();
                Type[] genericArguments = type.GetGenericArguments();
                if (genericArguments.Length != genericReferences.Length)
                {
                    return false;
                }

                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    if (!genericReferences[i].DoesTypeMatch(genericArguments[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a single <see cref="UnitTestTypeReference"/> from the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The <see cref="UnitTestTypeReference"/> parsed from <paramref name="value"/></returns>
        public static UnitTestTypeReference Parse(string value)
        {
            int parsePosition = 0;
            return Parse(value, ref parsePosition);
        }

        /// <summary>
        /// Creates a single <see cref="UnitTestTypeReference"/> from the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="parsePosition">The starting position in the string to begin parsing.</param>
        /// <returns>The <see cref="UnitTestTypeReference"/> parsed from <paramref name="value"/></returns>
        public static UnitTestTypeReference Parse(string value, ref int parsePosition)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (parsePosition < 0)
            {
                throw new ArgumentOutOfRangeException("parsePosition");
            }

            UnitTestTypeReference reference = new UnitTestTypeReference();
            reference.ParseName(value, ref parsePosition);
            reference.ParseGenericsIfPresent(value, ref parsePosition);
            return reference;
        }

        /// <summary>
        /// Parses a potential list of <see cref="UnitTestTypeReference"/> objects.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>The collection of <see cref="UnitTestTypeReference"/> instances</returns>
        public static IEnumerable<UnitTestTypeReference> ParseList(string value)
        {
            int parsePosition = 0;
            return ParseList(value, ref parsePosition);
        }

        /// <summary>
        /// Parses a potential list of <see cref="UnitTestTypeReference"/> objects.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="parsePosition">The starting parse position in the string.</param>
        /// <returns>The collection of <see cref="UnitTestTypeReference"/> instances</returns>
        public static IEnumerable<UnitTestTypeReference> ParseList(string value, ref int parsePosition)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (parsePosition < 0)
            {
                throw new ArgumentOutOfRangeException("parsePosition");
            }

            List<UnitTestTypeReference> references = new List<UnitTestTypeReference>();
            int len = value.Length;
            while (parsePosition < len)
            {
                SkipWhitespace(value, ref parsePosition);
                if (parsePosition >= len)
                {
                    break;
                }

                char c = value[parsePosition];
                if (c == ',')
                {
                    ++parsePosition;
                    continue;
                }

                if (c == '>' || c == ')')
                {
                    ++parsePosition;
                    break;
                }

                UnitTestTypeReference reference = Parse(value, ref parsePosition);
                references.Add(reference);
            }

            return references;
        }

        /// <summary>
        /// Formats the given collection of <see cref="UnitTestTypeReference"/> instances to a string.
        /// </summary>
        /// <param name="references">The collection of <see cref="UnitTestTypeReference"/> instances.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatList(IEnumerable<UnitTestTypeReference> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException("references");
            }

            bool first = true;
            StringBuilder sb = new StringBuilder();

            foreach (UnitTestTypeReference reference in references)
            {
                if (!first)
                {
                    sb.Append(", ");
                }

                first = false;
                sb.Append(reference.Format());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats the current <see cref="UnitTestTypeReference"/> to a string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string Format()
        {
            StringBuilder sb = new StringBuilder();
            if (this.IsByRef)
            {
                sb.Append("ref ");
            }

            sb.Append(this.Name);

            if (this.IsGenericType)
            {
                sb.Append("<");
                sb.Append(FormatList(this.GenericTypes));
                sb.Append(">");
            }

            return sb.ToString();
        }

        private static void SkipWhitespace(string value, ref int parsePosition)
        {
            int len = value.Length;
            for (; parsePosition < len; ++parsePosition)
            {
                char c = value[parsePosition];
                if (!char.IsWhiteSpace(c))
                {
                    break;
                }
            }
        }

        private static string GetAliasForType(Type type)
        {
            if (typesToAliases == null)
            {
                typesToAliases = new Dictionary<Type, string>();
                typesToAliases[typeof(Boolean)] = "bool";
                typesToAliases[typeof(Byte)] = "byte";
                typesToAliases[typeof(SByte)] = "sbyte";
                typesToAliases[typeof(Char)] = "char";
                typesToAliases[typeof(Decimal)] = "decimal";
                typesToAliases[typeof(Double)] = "double";
                typesToAliases[typeof(Single)] = "single";
                typesToAliases[typeof(Int32)] = "int";
                typesToAliases[typeof(UInt32)] = "uint";
                typesToAliases[typeof(Int64)] = "long";
                typesToAliases[typeof(UInt64)] = "ulong";
                typesToAliases[typeof(Int16)] = "short";
                typesToAliases[typeof(UInt16)] = "ushort";
            }

            string alias = null;
            typesToAliases.TryGetValue(type, out alias);
            return alias;
        }

        private void ParseName(string value, ref int parsePosition)
        {
            SkipWhitespace(value, ref parsePosition);
            int startPosition = parsePosition;
            int len = value.Length;
            for (; parsePosition < len; ++parsePosition)
            {
                char c = value[parsePosition];
                if (char.IsWhiteSpace(c) || c == ',' || c == '<' || c == '>' || c == ')')
                {
                    break;
                }
            }

            // If we encounter "ref Xyz", we indicate this is a ref and recurse for the name
            string candidateName = value.Substring(startPosition, parsePosition - startPosition).Trim();
            if (candidateName.Equals("ref", StringComparison.OrdinalIgnoreCase) || candidateName.Equals("out", StringComparison.OrdinalIgnoreCase))
            {
                this.IsByRef = true;
                this.ParseName(value, ref parsePosition);
            }
            else
            {
                this.Name = candidateName;
            }
        }

        private void ParseGenericsIfPresent(string value, ref int parsePosition)
        {
            SkipWhitespace(value, ref parsePosition);
            if (parsePosition < value.Length)
            {
                if (value[parsePosition] == '<')
                {
                    this.IsGenericType = true;
                    ++parsePosition;
                    this.GenericTypes = ParseList(value, ref parsePosition);
                }
            }
        }
    }
}