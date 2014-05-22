// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [UnitTestSuiteConfiguration()]
    public class UnitTestSuite : Microsoft.TestCommon.Base.UnitTestSuite
    {
        [TestMethod]
        public override void UnitTestSuiteIsCorrect()
        {
            this.ValidateUnitTestSuite();
        }
    }
}