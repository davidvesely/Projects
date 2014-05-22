// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // Positive and negative test: member coverage is complete or not depending which unit test classes are exposed

    [UnitTestLevel(UnitTestLevel.Complete, PublicMemberMinimumCoverage=100)]
    internal class PublicMemberCoverageCompleteTestClass : UnitTest<PublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }

        [MockTestMethod]
        [Description("PublicMemberCoverageProductType constructor is covered")]
        public void Constructor()
        {
        }

        [MockTestMethod]
        [Description("PublicMethod is covered")]
        public void PublicMethodIsCovered()
        {
        }
    }

    [UnitTestLevel(UnitTestLevel.Complete, PublicMemberMinimumCoverage = 100)]
    internal class PublicMemberCoverageIncompleteTestClass : UnitTest<PublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestLevel(UnitTestLevel.InProgress, PublicMemberMinimumCoverage = 100)]
    internal class PublicMemberCoverageIncompleteInProgressTestClass : UnitTest<PublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(PublicMemberMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.Complete)]
    internal class PublicMemberCoverageCompleteTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration]
    internal class PublicMemberCoverageTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    public class PublicMemberCoverageProductType {
        public void PublicMethod() { }
    }
}
