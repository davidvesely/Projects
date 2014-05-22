// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class UnitTest : Microsoft.TestCommon.Base.UnitTest
    {
        private static readonly Microsoft.TestCommon.WCF.UnitTestAsserters asserters = new Microsoft.TestCommon.WCF.UnitTestAsserters();

        public static new Microsoft.TestCommon.WCF.UnitTestAsserters Asserters { get { return asserters; } }

        [TestMethod]
        public override void UnitTestClassIsCorrect()
        {
            this.ValidateUnitTestClass();
        }
    }

    [TestClass]
    public abstract class UnitTest<T> : Microsoft.TestCommon.Base.UnitTest<T>
    {
        public static new Microsoft.TestCommon.WCF.UnitTestAsserters Asserters { get { return UnitTest.Asserters; } }

        [TestMethod]
        public override void UnitTestClassIsCorrect()
        {
            this.ValidateUnitTestClass();
        }
    }
}