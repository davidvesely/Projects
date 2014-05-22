// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(UnitTestTypeReference))]
    public class UnitTestTypeReferenceTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("UnitTestTypeReference type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        #region ParseList(string, ref int)

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ParseList(string) returns single item with single type")]
        public void ParseListWithSingleTypeSucceeds()
        {
            string value = "MyType";
            IEnumerable<UnitTestTypeReference> refs = UnitTestTypeReference.ParseList(value);
            Assert.IsNotNull(refs, "ParseList should never return null.");
            UnitTestTypeReference[] array = refs.ToArray();
            Assert.AreEqual(1, array.Length, "Expected one type in list.");
            UnitTestTypeReference reference = array[0];
            Assert.AreEqual(value, reference.Name, "Name was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
            Assert.IsFalse(reference.IsByRef, "IsRef should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ParseList(string) returns single item with single ref type")]
        public void ParseListWithSingleRefTypeSucceeds()
        {
            string value = "ref MyType";
            IEnumerable<UnitTestTypeReference> refs = UnitTestTypeReference.ParseList(value);
            Assert.IsNotNull(refs, "ParseList should never return null.");
            UnitTestTypeReference[] array = refs.ToArray();
            Assert.AreEqual(1, array.Length, "Expected one type in list.");
            UnitTestTypeReference reference = array[0];
            Assert.AreEqual("MyType", reference.Name, "Name was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
            Assert.IsTrue(reference.IsByRef, "IsRef should have been true.");
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ParseList(string) returns single item with single generic type")]
        public void ParseListWithSingleGenericTypeSucceeds()
        {
            string value = "IEnumerable<int>";
            IEnumerable<UnitTestTypeReference> refs = UnitTestTypeReference.ParseList(value);
            Assert.IsNotNull(refs, "ParseList should never return null.");
            UnitTestTypeReference[] array = refs.ToArray();
            Assert.AreEqual(1, array.Length, "Expected one type in list.");
            UnitTestTypeReference reference = array[0];
            Assert.AreEqual("IEnumerable", reference.Name, "Name was incorrect.");
            Assert.IsTrue(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNotNull(reference.GenericTypes, "GenericTypes should not be null.");

            array = reference.GenericTypes.ToArray();
            Assert.AreEqual(1, array.Length, "Expected 1 generic type.");
            reference = array[0];
            Assert.AreEqual("int", reference.Name, "Name for generic was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric for generic was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ParseList(string) returns single item with nested generic type")]
        public void ParseListWithNestedGenericTypeSucceeds()
        {
            string value = "IDictionary<string, IEnumerable<int>>";
            IEnumerable<UnitTestTypeReference> refs = UnitTestTypeReference.ParseList(value);
            Assert.IsNotNull(refs, "ParseList should never return null.");
            UnitTestTypeReference[] array = refs.ToArray();
            Assert.AreEqual(1, array.Length, "Expected one type in list.");
            UnitTestTypeReference reference = array[0];
            Assert.AreEqual("IDictionary", reference.Name, "Name was incorrect.");
            Assert.IsTrue(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNotNull(reference.GenericTypes, "GenericTypes should not be null.");

            array = reference.GenericTypes.ToArray();
            Assert.AreEqual(2, array.Length, "Expected 2 generic types.");
            reference = array[0];
            Assert.AreEqual("string", reference.Name, "Name for 1st generic was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric for 1st generic was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");

            reference = array[1];
            Assert.AreEqual("IEnumerable", reference.Name, "Name for generic was incorrect.");
            Assert.IsTrue(reference.IsGenericType, "IsGeneric for 2nd generic was incorrect.");
            Assert.IsNotNull(reference.GenericTypes, "GenericTypes for nested generic should not be null.");

            array = reference.GenericTypes.ToArray();
            Assert.AreEqual(1, array.Length, "Expected 1 nested generic type.");
            reference = array[0];
            Assert.AreEqual("int", reference.Name, "Name for generic was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric for generic was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("ParseList(string) returns correct list from multiple comma-separated types")]
        public void ParseListWithMultipleTypesSucceeds()
        {
            string value = "Type1, out Type2";
            IEnumerable<UnitTestTypeReference> refs = UnitTestTypeReference.ParseList(value);
            Assert.IsNotNull(refs, "ParseList should never return null.");
            UnitTestTypeReference[] array = refs.ToArray();
            Assert.AreEqual(2, array.Length, "Expected 2 types in list.");
            UnitTestTypeReference reference = array[0];
            Assert.AreEqual("Type1", reference.Name, "Name was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
            Assert.IsFalse(reference.IsByRef, "IsRef should have been false.");

            reference = array[1];
            Assert.AreEqual("Type2", reference.Name, "Name was incorrect.");
            Assert.IsFalse(reference.IsGenericType, "IsGeneric was incorrect.");
            Assert.IsNull(reference.GenericTypes, "GenericTypes should be null.");
            Assert.IsTrue(reference.IsByRef, "IsRef should have been true.");
        }

        #endregion ParseList(string, ref int)

        #region DoesTypeMatch(Type)

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("DoesTypeMatch(Type) returns true with all known simple types")]
        public void DoesTypeMatchReturnsTrueWithSimpleTypes()
        {
            Asserters.Data.Execute(
                DataSets.ValueAndRefTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "DoesTypeMatch failed",
                (t, obj) =>
                {
                    UnitTestTypeReference reference = UnitTestTypeReference.Parse(t.Name);
                    Assert.IsTrue(reference.DoesTypeMatch(t), string.Format("DoesTypeMatch failed for type '{0}'.", t.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("DoesTypeMatch(Type) returns true with nested generic")]
        public void DoesTypeMatchReturnsTrueWithNestedGeneric()
        {
            UnitTestTypeReference reference = UnitTestTypeReference.Parse("IDictionary<string, IEnumerable<int>>");
            Assert.IsTrue(reference.DoesTypeMatch(typeof(IDictionary<string, IEnumerable<int>>)), "DoesTypeMatch failed for nested generic");
        }

        #endregion DoesTypeMatch(Type)

        #endregion Methods
    }
}
