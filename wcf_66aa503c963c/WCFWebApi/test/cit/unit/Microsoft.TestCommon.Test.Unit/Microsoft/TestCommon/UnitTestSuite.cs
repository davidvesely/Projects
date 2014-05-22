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
    using Microsoft.TestCommon.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [UnitTestSuiteConfiguration(
        PublicTypeMinimumCoverage = 100, 
        NonPublicTypeMinimumCoverage = 0,
        PublicMemberMinimumCoverage = 100,
        NonPublicMemberMinimumCoverage = 0
        )]

    [UnitTestExclude(
        // Common
        typeof(TestData),
        typeof(TestDataAssert),
        typeof(TestDataVariations),
        typeof(RefTypeTestData<object>),
        typeof(TimeoutConstant),
        typeof(ValueTypeTestData<int>),

        // Types
        typeof(FlagsEnum),
        typeof(IGenericValueContainer),
        typeof(INameAndIdContainer),
        typeof(ISerializableType),
        typeof(LongEnum),
        typeof(SimpleEnum)
        )]
    public class UnitTestSuite : Microsoft.TestCommon.Base.UnitTestSuite
    {
        [TestMethod]
        public override void UnitTestSuiteIsCorrect()
        {
            this.ValidateUnitTestSuite();
        }
    }
}