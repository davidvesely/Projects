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
    using Microsoft.TestCommon.CIT.Unit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Asserter that creates a <see cref="UnitTestContext"/> for a given closure of types
    /// and invokes an <see cref="Action"/> to use it.
    /// </summary>
    public class ContextAssert
    {
        private static readonly ContextAssert singleton = new ContextAssert();

        public static ContextAssert Singleton { get { return singleton; } }

        public void Execute(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Action<UnitTestContext> contextCallback)
        {
            UnitTestContext unitTestContext = new MockUnitTestContext(unitTestTypes, productTypes);
            contextCallback(unitTestContext);
        }

        public void Execute(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes, Action<UnitTestClass, UnitTestContext> contextCallback)
        {
            UnitTestContext unitTestContext = new MockUnitTestContext(unitTestTypes, productTypes);
            Assert.AreEqual(1, unitTestContext.UnitTestClasses.Count(), "Only one UnitTestClass is permitted in the unit test types.");
            UnitTestClass unitTestClass = unitTestContext.UnitTestClasses.First();
            contextCallback(unitTestClass, unitTestContext);
        }
    }
}