// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
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
        private static readonly System.Net.Http.Test.UnitTestDataSets unitTestDataSets = new System.Net.Http.Test.UnitTestDataSets();

        public static new Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters { get { return unitTestAsserters; } }
        public static new System.Net.Http.Test.UnitTestDataSets DataSets { get { return unitTestDataSets; } }

        [TestMethod]
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
        public static new System.Net.Http.Test.UnitTestDataSets DataSets { get { return UnitTest.DataSets; } }

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
