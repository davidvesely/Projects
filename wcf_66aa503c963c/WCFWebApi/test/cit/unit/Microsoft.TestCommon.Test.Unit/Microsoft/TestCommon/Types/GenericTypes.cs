// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit.Types
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UnitTest = Microsoft.TestCommon.Base.UnitTest;
    using UnitTestSuite = Microsoft.TestCommon.Base.UnitTestSuite;

    internal class GenericTypeTestClass : UnitTest<GenericProductType<object>>
    {
        [MockTestMethod]
        [Description("GenericProductType type is a public, abstract and not sealed class")]
        public void TypeIsCorrect() { }

        [MockTestMethod]
        [Description("GenericProductType(TClass) constructor")]
        public void Constructor() { }

        [MockTestMethod]
        [Description("GenericProductType<T>(T) constructor")]
        public void ConstructorAliased() { }

        [MockTestMethod]
        [Description("MethodUsesTypeGeneric(TClass) succeeds")]
        public void MethodUsesTypeGeneric() { }

        [MockTestMethod]
        [Description("MethodUsesGeneric<TMethod>(TMethod) succeeds")]
        public void MethodUsesGeneric() { }

        [MockTestMethod]
        [Description("MethodUsesGeneric<T>(T) succeeds")]
        public void MethodUsesAliasedGeneric() { }

        [MockTestMethod]
        [Description("MethodUsesNestedGeneric(IDictionary<string, IEnumerable<int>>) succeeds")]
        public void MethodUsesNestedGeneric() { }

        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class GenericTypeTest2Class : UnitTest<GenericProductType2<object, object>>
    {
        [MockTestMethod]
        [Description("GenericProductType type is a public, abstract and not sealed class")]
        public void TypeIsCorrect() { }

        [MockTestMethod]
        [Description("GenericProductType constructor")]
        public void Constructor() { }

        [MockTestMethod]
        [Description("MethodUsesTypeGeneric(TClass1, TClass2) succeeds")]
        public void MethodUsesTypeGeneric() { }

        [MockTestMethod]
        [Description("MethodUsesGeneric<TMethod1, TMethod2>(TMethod1, TMethod2) succeeds")]
        public void MethodUsesGeneric() { }

        public override void UnitTestClassIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    [UnitTestSuiteConfiguration()]
    internal class GenericTestSuite : UnitTestSuite
    {
        public override void UnitTestSuiteIsCorrect()
        {
            throw new NotImplementedException();
        }
    }

    internal class GenericProductType<TClass>
    {
        public GenericProductType(TClass tClass)
        {
        }

        public void MethodUsesTypeGeneric(TClass tClass) { }

        public TMethod MethodUsesGeneric<TMethod>(TMethod t) { return t; }

        public void MethodUsesNestedGeneric(IDictionary<string, IEnumerable<int>> value) { }
    }

    internal class GenericProductType2<TClass1, TClass2>
    {
        public GenericProductType2()
        {
        }

        public void MethodUsesTypeGeneric(TClass1 tMethod1, TClass2 tMethod2) { }

        public TMethod2 MethodUsesGeneric<TMethod1, TMethod2>(TMethod1 tMethod1, TMethod2 tMethod2) { return tMethod2; }
    }
}
