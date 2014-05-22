// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // Positive and negative test: type coverage is complete or not depending which unit test classes are exposed

    internal class NonPublicTypeCoverageTestClass1 : UnitTest<NonPublicTypeCoverageInternalProductType1>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class NonPublicTypeCoverageTestClass2 : UnitTest<NonPublicTypeCoverageInternalProductType1>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(NonPublicTypeMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.Complete)]
    internal class NonPublicTypeCoverageCompleteTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(NonPublicTypeMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.InProgress)]
    internal class NonPublicTypeCoverageInProgressTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class NonPublicTypeCoverageInternalProductType1 { }

    internal class NonPublicTypeCoverageInternalProductType2 { }
}
