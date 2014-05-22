// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Main unit test base class for all Http unit tests in this assembly.
    /// This class exists solely to bridge to <see cref="UnitTest"/> because
    /// MSTest cannot inherit test methods across project boundaries.
    /// </summary>
    [TestClass]
    public class UnitTest : Microsoft.TestCommon.Base.UnitTest
    {
        private static readonly Microsoft.TestCommon.WCF.Http.UnitTestAsserters unitTestAsserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();
        private static readonly System.Net.Http.Formatting.OData.UnitTestDataSets unitTestDataSets = new System.Net.Http.Formatting.OData.UnitTestDataSets();

        public static new Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters { get { return unitTestAsserters; } }
        public static new System.Net.Http.Formatting.OData.UnitTestDataSets DataSets { get { return unitTestDataSets; } }

        [TestMethod]
        [Description("Microsoft.Net.Http.Formatting.OData unit test is correct")]
        public override void UnitTestClassIsCorrect()
        {
            // Don't validate the base UnitTest class
            if (this.GetType() != typeof(UnitTest))
            {
                this.ValidateUnitTestClass();
            }
        }
    }

    [TestClass]
    public abstract class UnitTest<T> : Microsoft.TestCommon.Base.UnitTest<T>
    {
        public static new Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters { get { return UnitTest.Asserters; } }
        public static new System.Net.Http.Formatting.OData.UnitTestDataSets DataSets { get { return UnitTest.DataSets; } }

        [TestMethod]
        public override void UnitTestClassIsCorrect()
        {
            // Don't validate the base UnitTest class
            if (this.GetType() != typeof(UnitTest<>))
            {
                this.ValidateUnitTestClass();
            }
        }
    }
}
