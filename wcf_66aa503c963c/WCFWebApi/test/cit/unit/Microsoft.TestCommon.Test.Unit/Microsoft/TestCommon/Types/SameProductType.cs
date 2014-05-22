// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // Negative test: two test classes test same product type

    internal class SameProductTypeTestClass1 : UnitTest<string>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class SameProductTypeTestClass2 : UnitTest<string>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration()]
    internal class SameProductTypeTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }
}
