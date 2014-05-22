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

    internal class PublicTypeCoverageTestClass1 : UnitTest<PublicTypeCoverageProductType1>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class PublicTypeCoverageTestClass2 : UnitTest<PublicTypeCoverageProductType1>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(PublicTypeMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.Complete)]
    internal class PublicTypeCoverageCompleteTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(PublicTypeMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.InProgress)]
    internal class PublicTypeCoverageInProgressTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    public class PublicTypeCoverageProductType1 { }

    public class PublicTypeCoverageProductType2 { }

}
