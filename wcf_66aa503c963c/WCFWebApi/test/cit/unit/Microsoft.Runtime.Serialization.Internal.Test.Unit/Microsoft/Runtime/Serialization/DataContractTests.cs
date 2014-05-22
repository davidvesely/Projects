// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Runtime.Serialzition
{
    using System;
    using Microsoft.ApplicationServer.Serialization.CIT.DataContracts;
    using Microsoft.Runtime.Serialization;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataContractTests
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("DataContract is an internal, abstract class.")]
        public void DataContract_Is_An_Internal_Abstract_Class()
        {
            Type type = typeof(DataContract);
            Assert.IsTrue(type.IsNotPublic, "DataContract should not be public.");
            Assert.IsFalse(type.IsVisible, "DataContract should not be visible.");
            Assert.IsTrue(type.IsAbstract, "DataContract should be abstract.");
            Assert.IsTrue(type.IsClass, "DataContract should be a class.");
        }

        #endregion Type Tests

        #region Factory Creation Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("DataContract.GetDataContract returns a DataContract instance for the SimpleDataContractClass type.")]
        public void GetDataContract_Returns_DataContract_For_SimpleDataContractClass()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsNotNull(contract, "GetDataContract should have returned an instance of DataContract.");
        }

        #endregion Factory Creation Tests

        #region Name Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract name should be the type name.")]
        public void SimpleDataContractClass_Name_Is_Class_Name()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual("SimpleDataContractClass", contract.Name.Value, "The data contract name should have been the class name.");
        }

        #endregion Name Property Tests

        #region Namespace Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract namespace should be the type namespace.")]
        public void SimpleDataContractClass_Namespace_Is_Class_Namespace()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual("http://schemas.datacontract.org/2004/07/Microsoft.ApplicationServer.Serialization.CIT.DataContracts", contract.Namespace.Value, "The data contract namesapce should have been the class namespace.");
        }

        #endregion Namespace Property Tests

        #region StableName Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract StableName should be the type namespace:name.")]
        public void SimpleDataContractClass_StableName_Is_Class_Namespace_And_Name()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual("http://schemas.datacontract.org/2004/07/Microsoft.ApplicationServer.Serialization.CIT.DataContracts:SimpleDataContractClass", contract.StableName.ToString(), "The data contract StableName should have been the class namespace:name.");   
        }

        #endregion StableName Property Tests

        #region UnderlyingType Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract UnderlyingType should be the class itself.")]
        public void SimpleDataContractClass_UnderlyingType_Is_SimpleDataContractClass()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual(typeof(SimpleDataContractClass), contract.UnderlyingType, "The data contract UnderlyingType should have been the class itself.");
        }

        #endregion UnderlyingType Property Tests

        #region TypeForInitialization Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract TypeForInitialization should be the class itself.")]
        public void SimpleDataContractClass_TypeForInitialization_Is_SimpleDataContractClass()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual(typeof(SimpleDataContractClass), contract.TypeForInitialization, "The data contract TypeForInitialization should have been the class itself.");
        }

        #endregion TypeForInitialization Property Tests

        #region TopLevelElementName Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract TopLevelElementName should be the type name.")]
        public void SimpleDataContractClass_TopLevelElementName_Is_Class_Name()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual("SimpleDataContractClass", contract.TopLevelElementName.Value, "The data contract TopLevelElementName should have been the class name.");
        }

        #endregion TopLevelElementName Property Tests

        #region TopLevelElementNamespace Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract TopLevelElementNamespace should be the type namespace.")]
        public void SimpleDataContractClass_TopLevelElementNamespace_Is_Class_Namespace()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.AreEqual("http://schemas.datacontract.org/2004/07/Microsoft.ApplicationServer.Serialization.CIT.DataContracts", contract.TopLevelElementNamespace.Value, "The data contract TopLevelElementNamespace should have been the class namespace.");
        }

        #endregion TopLevelElementNamespace Property Tests

        #region KnownDataContract Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract KnownDataContracts is null.")]
        public void SimpleDataContractClass_KnownDataContracts_Is_Null()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsNull(contract.KnownDataContracts, "The data contract KnownDataContracts should have been null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("HasKnownDataContractClass type's data contract KnownDataContracts is the data contract of DerivedHasKnownDataContractClass.")]
        public void HasKnownDataContractClass_KnownDataContracts_Includes_DerivedHasKnownDataContractClass()
        {
            DataContract contract = DataContract.GetDataContract(typeof(HasKnownDataContractClass));
            DataContract derivedContract = DataContract.GetDataContract(typeof(DerivedHasKnownDataContractClass));
            Assert.AreSame(derivedContract, contract.KnownDataContracts[derivedContract.StableName], "The data contract KnownDataContracts should included DerivedHasKnownDataContractClass");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("HasKnownDataContractClass type's data contract KnownDataContracts count is one.")]
        public void HasKnownDataContractClass_KnownDataContracts_Count_Is_One()
        {
            DataContract contract = DataContract.GetDataContract(typeof(HasKnownDataContractClass));
            Assert.AreEqual(1, contract.KnownDataContracts.Count, "The data contract KnownDataContracts count should have been one.");
        }

        #endregion KnownDataContract Property Tests

        #region IsISerializable Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract IsISerializable is false.")]
        public void SimpleDataContractClass_IsISerializable_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsFalse(contract.IsISerializable, "The data contract IsISerializable should have been false.");
        }

        #endregion IsISerializable Property Tests

        #region IsValueType Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract IsValueType is false.")]
        public void SimpleDataContractClass_IsValueType_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsFalse(contract.IsValueType, "The data contract IsValueType should have been false.");
        }

        #endregion IsValueType Property Tests

        #region IsReference Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract IsReference is false.")]
        public void SimpleDataContractClass_IsReference_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsFalse(contract.IsReference, "The data contract IsReference should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("IsReferenceDataContractClass type's data contract IsReference is true.")]
        public void IsReferenceDataContractClass_IsReference_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(IsReferenceDataContractClass));
            Assert.IsTrue(contract.IsReference, "The data contract IsReference should have been true.");
        }

        #endregion IsReference Property Tests

        #region IsPrimitive Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract IsPrimitive is false.")]
        public void SimpleDataContractClass_IsPrimitive_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsFalse(contract.IsPrimitive, "The data contract IsPrimitive should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("Int type's data contract IsPrimitive is true.")]
        public void Int_IsPrimitive_Is_True()
        {
            DataContract contract = DataContract.GetDataContract(typeof(int));
            Assert.IsTrue(contract.IsPrimitive, "The data contract IsPrimitive should have been true.");
        }

        #endregion IsPrimitive Property Tests

        #region IsBuiltInDataContract Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract IsBuiltInDataContract is false.")]
        public void SimpleDataContractClass_IsBuiltInDataContract_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsFalse(contract.IsBuiltInDataContract, "The data contract IsBuiltInDataContract should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("String type's data contract IsBuiltInDataContract is true.")]
        public void String_IsBuiltInDataContract_Is_True()
        {
            DataContract contract = DataContract.GetDataContract(typeof(string));
            Assert.IsTrue(contract.IsBuiltInDataContract, "The data contract IsBuiltInDataContract should have been true.");
        }

        #endregion IsBuiltInDataContract Property Tests

        #region HasRoot Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract HasRoot is true.")]
        public void SimpleDataContractClass_HasRoot_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsTrue(contract.HasRoot, "The data contract HasRoot should have been true.");
        }

        #endregion HasRoot Property Tests

        #region CanContainReferences Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract CanContainReferences is true.")]
        public void SimpleDataContractClass_CanContainReferences_Is_True()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsTrue(contract.CanContainReferences, "The data contract CanContainReferences should have been true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("Bool type's data contract CanContainReferences is false.")]
        public void Bool_CanContainReferences_Is_False()
        {
            DataContract contract = DataContract.GetDataContract(typeof(bool));
            Assert.IsFalse(contract.CanContainReferences, "The data contract CanContainReferences should have been false.");
        }

        #endregion CanContainReferences Property Tests

        #region ParseMethod Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract ParseMethod is null.")]
        public void SimpleDataContractClass_ParseMethod_Is_Null()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsNull(contract.ParseMethod, "The data contract ParseMethod should have been null.");
        }

        #endregion ParseMethod Property Tests

        #region GenericInfo Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataContract)]
        [Description("SimpleDataContractClass type's data contract GenericInfo is null.")]
        public void SimpleDataContractClass_GenericInfo_Is_Null()
        {
            DataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass));
            Assert.IsNull(contract.GenericInfo, "The data contract GenericInfo should have been null.");
        }

        #endregion GenericInfo Property Tests
    }
}
