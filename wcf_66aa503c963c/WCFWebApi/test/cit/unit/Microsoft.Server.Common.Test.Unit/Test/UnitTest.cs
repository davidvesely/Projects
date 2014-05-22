// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common
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
        private static readonly Microsoft.Server.Common.UnitTestAsserters unitTestAsserters = new Microsoft.Server.Common.UnitTestAsserters();
        public static new Microsoft.Server.Common.UnitTestAsserters Asserters { get { return unitTestAsserters; } }
        
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
        public static new Microsoft.Server.Common.UnitTestAsserters Asserters { get { return UnitTest.Asserters; } }

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
