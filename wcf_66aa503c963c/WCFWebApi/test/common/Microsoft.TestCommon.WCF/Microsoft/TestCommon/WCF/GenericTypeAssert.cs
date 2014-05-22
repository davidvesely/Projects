// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
namespace Microsoft.TestCommon.WCF
{
    using System;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test assertion class to provide convenience and assert methods for generic types
    /// whose type parameters are not known at compile time.
    /// </summary>
    public class GenericTypeAssert : Microsoft.TestCommon.GenericTypeAssert
    {
        private static readonly Type genericDataContractType = typeof(GenericDataContractType<>);
        private static readonly Type genericXmlSerializableType = typeof(GenericXmlSerializableType<>);
        private static readonly GenericTypeAssert singleton = new GenericTypeAssert();

        public static new GenericTypeAssert Singleton { get { return singleton; } }

        /// <summary>
        /// Asserts <paramref name="instance"/> is a <c>GenericDataContractType</c> and returns
        /// the value of its "Value" property.
        /// </summary>
        /// <param name="instance">The instance to test.</param>
        /// <param name="failureMessage">The error message to prefix any test assertions.</param>
        /// <returns>The value returned from the "Value" property.</returns>
        public object HasGenericDataContractTypeValue(object instance, string failureMessage)
        {
            Type expectedType = instance.GetType();
            bool isGenericDataContractType = expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == genericDataContractType;

            Assert.IsTrue(
                isGenericDataContractType,
                string.Format("{0}: object was type '{1}' but should have been GenericDataContractType.", failureMessage, instance.GetType().Name));

            return GetProperty(instance, "Value", failureMessage);
        }

        /// <summary>
        /// Asserts <paramref name="instance"/> is a <c>GenericXmlSerializerType</c> and returns
        /// the value of its "Value" property.
        /// </summary>
        /// <param name="instance">The instance to test.</param>
        /// <param name="failureMessage">The error message to prefix any test assertions.</param>
        /// <returns>The value returned from the "Value" property.</returns>
        public object HasGenericXmlSerializableTypeValue(object instance, string failureMessage)
        {
            Type expectedType = instance.GetType();
            bool isXmlSerializerType = expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == genericXmlSerializableType;

            Assert.IsTrue(
                isXmlSerializerType,
                string.Format("{0}: object was type '{1}' but should have been GenericXmlSerializableType.", failureMessage, instance.GetType().Name));

            return GetProperty(instance, "Value", failureMessage);
        }
    }
}
