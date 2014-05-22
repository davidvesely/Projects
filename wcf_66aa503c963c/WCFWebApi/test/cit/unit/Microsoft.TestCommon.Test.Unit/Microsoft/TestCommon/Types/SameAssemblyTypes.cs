// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // Positive test: multiple tested product types live in same assembly

    internal class SameAssemblyTestClass1 : UnitTest<string>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class SameAssemblyTestClass2 : UnitTest<int>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration()]
    internal class SameAssemblyTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }
}
