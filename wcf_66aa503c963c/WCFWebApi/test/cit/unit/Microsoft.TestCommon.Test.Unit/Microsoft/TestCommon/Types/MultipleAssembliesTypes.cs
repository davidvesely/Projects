// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // Negative test: multiple tested product types live in different assemblies

    internal class MultipleAssembliesTestClass1 : UnitTest<string>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class MultipleAssembliesTestClass2 : UnitTest<MultipleAssembliesTestClass1>
    {
        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration()]
    internal class MultipleAssembliesTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }
}
