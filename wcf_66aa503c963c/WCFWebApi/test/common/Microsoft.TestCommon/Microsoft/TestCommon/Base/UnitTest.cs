// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class that does basic unit test class validation.
    /// </summary>
    /// <remarks>
    /// Unit test classes should derive from this class and call the
    /// <see cref="ValidateUnitTestClass"/> to self-validate using MSTest.
    /// </remarks>
    [TestClass]
    public abstract class UnitTest
    {
        private static readonly UnitTestAsserters unitTestAsserters = new UnitTestAsserters();
        private static readonly UnitTestDataSets unitTestDataSets = new UnitTestDataSets();

        /// <summary>
        /// Gets the <see cref="UnitTestAsserters"/> class that exposes all common assert classes.
        /// </summary>
        public static UnitTestAsserters Asserters { get { return unitTestAsserters; } }

        /// <summary>
        /// Gets the <see cref="UnitTestDataSets"/> class that exposes all common data set classes.
        /// </summary>
        public static UnitTestDataSets DataSets { get { return unitTestDataSets; } }

        /// <summary>
        /// Gets the product type tested by this unit test.
        /// </summary>
        /// <remarks>
        /// This value is found either from the "T" for classes derived from <see cref="UnitTest{T}"/>
        /// or from the <see cref="UnitTestTypeAttribute"/> attached to the unit test class.
        /// </remarks>
        public Type TypeUnderTest { get { return UnitTestContext.GetTypeUnderTest(this.GetType()); } }

        /// <summary>
        /// Subclasses are required to implement this, and should call <see cref="ValidateUnitTestClass"/>.
        /// </summary>
        public abstract void UnitTestClassIsCorrect();

        /// <summary>
        /// Method that invokes all rules applicable to this unit test class and asserts when
        /// there are errors.
        /// </summary>
        protected void ValidateUnitTestClass()
        {
            Type testClassType = this.GetType();
            UnitTestContext context = UnitTestContext.GetOrCreateUnitTextContext(testClassType.Assembly);
            context.ReportUnitTestIssues(context.ExecuteClassRules(testClassType));
        }
    }

    /// <summary>
    /// Generic form of <see cref="UnitTest"/> where the generic parameter <typeparamref name="T"/>
    /// describes the type under test.
    /// </summary>
    /// <typeparam name="T">The type tested by this class.</typeparam>
    public abstract class UnitTest<T> : UnitTest
    {
    }
}
