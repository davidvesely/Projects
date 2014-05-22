// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class that validates all types in a unit test assembly
    /// have correct unit test classes.
    /// </summary>
    /// <remarks>
    /// Unit test suite-level classes should derive from this class and call the
    /// <see cref="UnitTestSuiteIsCorrect"/> to self-validate all the unit test
    /// classes in the same assembly.
    /// </remarks>
    [TestClass]
    public abstract class UnitTestSuite
    {
        /// <summary>
        /// Subclasses are required to implement this, and should call <see cref="ValidateUnitTestSuite"/>.
        /// </summary>
        public abstract void UnitTestSuiteIsCorrect();

        /// <summary>
        /// Validates all product types in this suite's assembly adhere to a common set of conventions.
        /// </summary>
        protected void ValidateUnitTestSuite()
        {
            Type testSuiteType = this.GetType();
            UnitTestContext context = UnitTestContext.GetOrCreateUnitTextContext(testSuiteType.Assembly);
            context.ReportUnitTestIssues(context.ExecuteSuiteRules());
        }
    }
}
