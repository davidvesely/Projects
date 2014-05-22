// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for testing that all product types in the entire
    /// assembly have correct unit tests.
    /// </summary>
    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    [UnitTestSuiteConfiguration]
    public class UnitTestSuite : Microsoft.TestCommon.Base.UnitTestSuite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestSuite"/> class.
        /// </summary>
        public UnitTestSuite()
        {
        }

        /// <summary>
        /// Called to validate all the tests in the assembly.
        /// </summary>
        [TestMethod]
        public override void UnitTestSuiteIsCorrect()
        {
            this.ValidateUnitTestSuite();
        }
    }
}
