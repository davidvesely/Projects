// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.TestCommon;
using Microsoft.TestCommon.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.TestCommon.CIT.Unit
{
    public class MockUnitTestContext : UnitTestContext
    {
        public MockUnitTestContext(IEnumerable<Type> unitTestTypes, IEnumerable<Type> productTypes) : base(unitTestTypes.First().Assembly)
        {
            this.MockUnitTestTypes = unitTestTypes;
            this.MockProductTypes = productTypes;
        }

        private IEnumerable<Type> MockProductTypes { get; set; }
        private IEnumerable<Type> MockUnitTestTypes { get; set; }

        protected override IEnumerable<Type> OnGetProductTypes()
        {
            return this.MockProductTypes;
        }

        protected override IEnumerable<Type> OnGetUnitTestTypes()
        {
            return this.MockUnitTestTypes;
        }

        protected override IEnumerable<MethodInfo> OnGetUnitTestClassMethods(Type unitTestType)
        {
            IEnumerable<MethodInfo> methods = unitTestType.GetMethods(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where((m) => m.DeclaringType == unitTestType && 
                                    (m.GetCustomAttributes(typeof(TestMethodAttribute), false).Any() || 
                                     (m.GetCustomAttributes(typeof(MockTestMethodAttribute), false).Any())));

            return methods;
        }
    }
}
