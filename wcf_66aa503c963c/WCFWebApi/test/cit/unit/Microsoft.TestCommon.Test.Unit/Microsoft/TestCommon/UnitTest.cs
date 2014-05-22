// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class UnitTest : Microsoft.TestCommon.Base.UnitTest
    {
        private static readonly UnitTestAsserters unitTestAsserters = new UnitTestAsserters();

        public static new UnitTestAsserters Asserters { get { return unitTestAsserters; } }

        [TestMethod]
        public override void UnitTestClassIsCorrect()
        {
            this.ValidateUnitTestClass();
        }
    }

    [TestClass]
    public abstract class UnitTest<T> : Microsoft.TestCommon.Base.UnitTest<T>
    {
        public static new UnitTestAsserters Asserters { get { return UnitTest.Asserters; } }

        [TestMethod]
        public override void UnitTestClassIsCorrect()
        {
            this.ValidateUnitTestClass();
        }
    }
}