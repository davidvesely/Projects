// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using UnitTest=Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [UnitTestType(typeof(SimpleProductType))]
    internal class SimpleTestClass : UnitTest
    {
        [MockTestMethod]
        [Description("SimpleProductType type is a public, abstract and not sealed class")]
        public void TypeIsCorrect() { }

        [MockTestMethod]
        [Description("SimpleProductType constructor")]
        public void Constructor() { }

        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration()]
    internal class SimpleTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class SimpleProductType
    {
        public SimpleProductType()
        {
        }
    }
}
