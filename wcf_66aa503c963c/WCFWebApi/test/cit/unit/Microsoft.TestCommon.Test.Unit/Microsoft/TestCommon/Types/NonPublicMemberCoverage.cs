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

    [UnitTestLevel(UnitTestLevel.Complete, PublicMemberMinimumCoverage = 100)]
    internal class NonPublicMemberCoverageCompleteTestClass : UnitTest<NonPublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }

        [MockTestMethod]
        [Description("NonPublicMemberCoverageProductType constructor is covered")]
        public void Constructor()
        {
        }

        [MockTestMethod]
        [Description("PublicMethod is covered")]
        public void PublicMethodIsCovered()
        {
        }
    }

    [UnitTestLevel(UnitTestLevel.Complete, NonPublicMemberMinimumCoverage = 100)]
    internal class NonPublicMemberCoverageIncompleteTestClass : UnitTest<NonPublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestLevel(UnitTestLevel.InProgress, NonPublicMemberMinimumCoverage = 100)]
    internal class NonPublicMemberCoverageIncompleteInProgressTestClass : UnitTest<NonPublicMemberCoverageProductType>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration(NonPublicMemberMinimumCoverage = 100, UnitTestLevel = UnitTestLevel.Complete)]
    internal class NonPublicMemberCoverageCompleteTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration]
    internal class NonPublicMemberCoverageTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    public class NonPublicMemberCoverageProductType
    {
        internal void InternalMethod() { }

        // Demonstrates private members do not penalize coverage
        private void PrivateMethod() { }
    }
}
