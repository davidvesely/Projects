// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Custom attribute to declare which types should not be considered when
    /// determining the type coverage for a unit test suite.
    /// </summary>
    /// <remarks>
    /// This attribute can be used to exclude types that are impossible to test
    /// or for legacy unit tests that know these types are adequately tested.
    /// This attribute should be applied to the <see cref="UnitTestSuite"/>
    /// class in the unit test project.
    /// </remarks>
    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UnitTestExcludeAttribute : Attribute
    {
        public UnitTestExcludeAttribute(params Type[] excludedProductTypes)
        {
            this.ExcludedProductTypes = excludedProductTypes == null 
                ? (IEnumerable<Type>) Enumerable.Empty<Type>() 
                : (IEnumerable<Type>) excludedProductTypes.ToList();
        }

        public IEnumerable<Type> ExcludedProductTypes { get; private set; }
    }
}
