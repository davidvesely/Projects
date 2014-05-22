// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;

    /// <summary>
    /// Custom attribute used to indicate the type tested by
    /// a unit test class.
    /// </summary>
    /// <remarks>
    /// This custom attribute provides an alternate way to specify the product type
    /// tested by a unit test class when it cannot derive from <see cref="UnitTest{T}"/>.
    /// </remarks>
    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class UnitTestTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestTypeAttribute"/> class.
        /// </summary>
        /// <param name="unitTestLevel"></param>
        public UnitTestTypeAttribute(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the product type tested by the unit test using this attribute.
        /// </summary>
        public Type Type { get; private set; }
    }
}
