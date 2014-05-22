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
    public class ClassDataContractTests
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ClassDataContract is an internal, non-abstract class.")]
        public void ClassDataContract_Is_An_Internal_Non_Abstract_Class()
        {
            Type type = typeof(ClassDataContract);
            Assert.IsTrue(type.IsNotPublic, "ClassDataContract should not be public.");
            Assert.IsFalse(type.IsVisible, "ClassDataContract should not be visible.");
            Assert.IsFalse(type.IsAbstract, "ClassDataContract should not be abstract.");
            Assert.IsTrue(type.IsClass, "ClassDataContract should be a class.");
        }

        #endregion Type Tests

        #region Factory Creation Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DataContract.GetDataContract returns a ClassDataContract instance for the SimpleDataContractClass type.")]
        public void GetDataContract_Returns_ClassDataContract_For_SimpleDataContractClass()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsNotNull(contract, "GetDataContract should have returned an instance of ClassDataContract.");
        }

        #endregion Factory Creation Tests

        #region BaseContract Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract BaseContract is null.")]
        public void SimpleDataContractClass_BaseContract_Is_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsNull(contract.BaseContract, "The data contract BaseContract should have been null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract BaseContract is SimpleDataContract.")]
        public void DerivedDataContractClass_BaseContract_Is_SimpleDataContract()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreSame(DataContract.GetDataContract(typeof(SimpleDataContractClass)), contract.BaseContract, "The data contract BaseContract of DerivedDataContractClass should have been the data contract of SimpleDataContractClass.");
        }

        #endregion BaseContract Property Tests

        #region DeserializationExceptionMessage Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract DeserializationExceptionMessage is null.")]
        public void SimpleDataContractClass_DeserializationExceptionMessage_Is_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsNull(contract.DeserializationExceptionMessage, "The data contract DeserializationExceptionMessage should have been null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract DeserializationExceptionMessage is not null.")]
        public void NoSetterDataContractClass_DeserializationExceptionMessage_Is_Not_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.IsNotNull(contract.DeserializationExceptionMessage, "The data contract DeserializationExceptionMessage should not have been null.");
        }

        #endregion DeserializationExceptionMessage Property Tests

        #region SerializationExceptionMessage Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract SerializationExceptionMessage is null.")]
        public void SimpleDataContractClass_SerializationExceptionMessage_Is_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsNull(contract.SerializationExceptionMessage, "The data contract SerializationExceptionMessage should have been null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract SerializationExceptionMessage is not null.")]
        public void NoSetterDataContractClass_SerializationExceptionMessage_Is_Not_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.IsNotNull(contract.SerializationExceptionMessage, "The data contract SerializationExceptionMessage should not have been null.");
        }

        #endregion SerializationExceptionMessage Property Tests

        #region HasDataContract Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract HasDataContract is true.")]
        public void SimpleDataContractClass_HasDataContract_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsTrue(contract.HasDataContract, "The data contract HasDataContract should have been true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract HasDataContract is false.")]
        public void PocoDataContractClass_HasDataContract_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.IsFalse(contract.HasDataContract, "The data contract HasDataContract should have been false.");
        }

        #endregion HasDataContract Property Tests

        #region HasExtensionData Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract HasExtensionData is false.")]
        public void SimpleDataContractClass_HasExtensionData_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsFalse(contract.HasExtensionData, "The data contract HasExtensionData should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ExtensionDataContractClass type's data contract HasExtensionData is true.")]
        public void ExtensionDataContractClass_HasExtensionData_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.IsTrue(contract.HasExtensionData, "The data contract HasExtensionData should have been true.");
        }

        #endregion HasExtensionData Property Tests

        #region IsNonAttributedType Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract IsNonAttributedType is false.")]
        public void SimpleDataContractClass_IsNonAttributedType_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.IsFalse(contract.IsNonAttributedType, "The data contract IsNonAttributedType should have been false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract IsNonAttributedType is true.")]
        public void PocoDataContractClass_IsNonAttributedType_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.IsTrue(contract.IsNonAttributedType, "The data contract IsNonAttributedType should have been true.");
        }

        #endregion IsNonAttributedType Property Tests

        #region MemberNames Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNames length is two.")]
        public void SimpleDataContractClass_MemberNames_Length_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.MemberNames.Length, "The data contract should have had two DataMember names.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNames length is three.")]
        public void DerivedDataContractClass_MemberNames_Length_Is_Three()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(3, contract.MemberNames.Length, "The data contract should have had three DataMember names.");
        }
        
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNames length is three.")]
        public void PocoDataContractClass_MemberNames_Length_Is_Three()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(3, contract.MemberNames.Length, "The data contract should have had three DataMember names.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNames length is two.")]
        public void ExtensionDataContractClass_MemberNames_Length_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.MemberNames.Length, "The data contract should have had two DataMember names.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract MemberNames length is one.")]
        public void NoSetterDataContractClass_MemberNames_Length_Is_One()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.AreEqual(1, contract.MemberNames.Length, "The data contract should have had one DataMember names.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNames[0] is property name.")]
        public void SimpleDataContractClass_MemberNames_Zero_Is_Property_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember1", contract.MemberNames[0].Value, "The data contract should have had DataMemeberName 'DataMember1'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNames[0] is property name.")]
        public void DerivedDataContractClass_MemberNames_Zero_Is_Property_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember1", contract.MemberNames[0].Value, "The data contract should have had DataMemeberName 'DataMember1'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ExtensionDataContractClass type's data contract MemberNames[0] is property name.")]
        public void ExtensionDataContractClass_MemberNames_Zero_Is_Property_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember1", contract.MemberNames[0].Value, "The data contract should have had DataMemeberName 'DataMember1'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract MemberNames[0] is property name.")]
        public void NoSetterDataContractClass_MemberNames_Zero_Is_Property_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember1", contract.MemberNames[0].Value, "The data contract should have had DataMemeberName 'DataMember1'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNames[0] is property name.")]
        public void PocoDataContractClass_MemberNames_Zero_Is_Property_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual("BoolProperty", contract.MemberNames[0].Value, "The data contract should have had DataMemeberName 'BoolProperty'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNames[1] is the attribute name.")]
        public void SimpleDataContractClass_MemberNames_One_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual("RenamedDataMember", contract.MemberNames[1].Value, "The data contract should have had DataMemeberName 'RenamedDataMember'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNames[1] is the attribute name.")]
        public void DerivedDataContractClass_MemberNames_One_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual("RenamedDataMember", contract.MemberNames[1].Value, "The data contract should have had DataMemeberName 'RenamedDataMember'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ExtensionDataContractClass type's data contract MemberNames[1] is the attribute name.")]
        public void ExtensionDataContractClass_MemberNames_One_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual("RenamedDataMember", contract.MemberNames[1].Value, "The data contract should have had DataMemeberName 'RenamedDataMember'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNames[1] is the property name.")]
        public void PocoDataContractClass_MemberNames_One_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember1", contract.MemberNames[1].Value, "The data contract should have had DataMemeberName 'DataMember1'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNames[2] is the attribute name.")]
        public void DerivedDataContractClass_MemberNames_Two_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember3", contract.MemberNames[2].Value, "The data contract should have had DataMemeberName 'DataMember3'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNames[2] is the property name.")]
        public void PocoDataContractClass_MemberNames_Two_Is_Attribute_Name()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual("DataMember2", contract.MemberNames[2].Value, "The data contract should have had DataMemeberName 'DataMember2'.");
        }

        #endregion MemberNames Property Tests

        #region MemberNamespaces Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNamespaces length is two.")]
        public void SimpleDataContractClass_MemberNamespaces_Length_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.MemberNamespaces.Length, "The data contract should have had two DataMember namespaces.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNamespaces length is three.")]
        public void DerivedDataContractClass_MemberNamespaces_Length_Is_Three()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(3, contract.MemberNamespaces.Length, "The data contract should have had three DataMember namespaces.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNamespaces length is three.")]
        public void PocoDataContractClass_MemberNamespaces_Length_Is_Three()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(3, contract.MemberNamespaces.Length, "The data contract should have had three DataMember namespaces.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNamespaces length is two.")]
        public void ExtensionDataContractClass_MemberNamespaces_Length_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.MemberNamespaces.Length, "The data contract should have had two DataMember namespaces.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract MemberNames length is one.")]
        public void NoSetterDataContractClass_MemberNamespaces_Length_Is_One()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.AreEqual(1, contract.MemberNamespaces.Length, "The data contract should have had one DataMember namespaces.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNamespaces[0] is data contract namespace.")]
        public void SimpleDataContractClass_MemberNamespaces_Zero_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[0].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNamespaces[0] is data contract namespace.")]
        public void DerivedDataContractClass_MemberNamespaces_Zero_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[0].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ExtensionDataContractClass type's data contract MemberNamespaces[0] is data contract namespace.")]
        public void ExtensionDataContractClass_MemberNamespaces_Zero_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[0].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract MemberNamespaces[0] is data contract namespace.")]
        public void NoSetterDataContractClass_MemberNamespaces_Zero_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[0].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNamespaces[0] is data contract namespace.")]
        public void PocoDataContractClass_MemberNamespaces_Zero_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[0].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNamespaces[1] is the data contract namespace.")]
        public void SimpleDataContractClass_MemberNamespaces_One_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[1].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNamespaces[1] is the data contract namespace.")]
        public void DerivedDataContractClass_MemberNamespaces_One_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[1].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("ExtensionDataContractClass type's data contract MemberNamespaces[1] is the data contract namespace.")]
        public void ExtensionDataContractClass_MemberNamespaces_One_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[1].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNamespaces[1] is the data contract namespace.")]
        public void PocoDataContractClass_MemberNamespaces_One_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[1].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract MemberNamespaces[2] is the data contract namespace.")]
        public void DerivedDataContractClass_MemberNamespaces_Two_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[2].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract MemberNamespaces[2] is the data contract namespace.")]
        public void PocoDataContractClass_MemberNamespaces_Two_Is_Contract_Namespace()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(contract.Namespace.Value, contract.MemberNamespaces[2].Value, "The data contract should have had DataMemeberNamespace same as the data contract namespace.");
        }

        #endregion MemberNamespaces Property Tests

        #region Member Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract Members count is two.")]
        public void SimpleDataContractClass_Members_Count_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.Members.Count, "The data contract should have had two DataMembers.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("DerivedDataContractClass type's data contract Members count is one.")]
        public void DerivedDataContractClass_Members_Count_Is_One()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(DerivedDataContractClass)) as ClassDataContract;
            Assert.AreEqual(1, contract.Members.Count, "The data contract should have had one DataMembers.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("PocoDataContractClass type's data contract Members count is three.")]
        public void PocoDataContractClass_Members_Count_Is_Three()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(PocoDataContractClass)) as ClassDataContract;
            Assert.AreEqual(3, contract.Members.Count, "The data contract should have had three DataMembers.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("SimpleDataContractClass type's data contract MemberNames count is two.")]
        public void ExtensionDataContractClass_Members_Count_Is_Two()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ExtensionDataContractClass)) as ClassDataContract;
            Assert.AreEqual(2, contract.Members.Count, "The data contract should have had two DataMembers.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ClassDataContract)]
        [Description("NoSetterDataContractClass type's data contract MemberNames count is one.")]
        public void NoSetterDataContractClass_Members_Count_Is_One()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NoSetterDataContractClass)) as ClassDataContract;
            Assert.AreEqual(1, contract.Members.Count, "The data contract should have had one DataMembers.");
        }

        #endregion Member Property Tests
    }
}
