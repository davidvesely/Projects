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
    public class DataMemeberTests
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("DataMember is an internal, non-abstract class.")]
        public void DataMember_Is_An_Internal_Non_Abstract_Class()
        {
            Type type = typeof(DataMember);
            Assert.IsTrue(type.IsNotPublic, "DataContract should not be public.");
            Assert.IsFalse(type.IsVisible, "DataContract should not be visible.");
            Assert.IsFalse(type.IsAbstract, "DataContract should not be abstract.");
            Assert.IsTrue(type.IsClass, "DataContract should be a class.");
        }

        #endregion Type Tests

        #region EmitDefaultValue Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] has an EmitDefaultValue of true.")]
        public void SimpleDataContractClass_DataMember_0_EmitDefaultValue_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsTrue(member.EmitDefaultValue, "The data member name should have had an EmitDefaultValue of true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("EmitDefaultValueDataContractClass DataMember[0] has an EmitDefaultValue of true.")]
        public void EmitDefaultValueDataContractClass_DataMember_0_EmitDefaultValue_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(EmitDefaultValueDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsTrue(member.EmitDefaultValue, "The data member name should have had an EmitDefaultValue of true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("EmitDefaultValueDataContractClass DataMember[1] has an EmitDefaultValue of false.")]
        public void EmitDefaultValueDataContractClass_DataMember_1_EmitDefaultValue_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(EmitDefaultValueDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsFalse(member.EmitDefaultValue, "The data member name should have had an EmitDefaultValue of false.");
        }

        #endregion EmitDefaultValue Property Tests

        #region HasConflictingNameAndType Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] has an HasConflictingNameAndType of false.")]
        public void SimpleDataContractClass_DataMember_0_HasConflictingNameAndType_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsFalse(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[1] has an HasConflictingNameAndType of false.")]
        public void SimpleDataContractClass_DataMember_1_HasConflictingNameAndType_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsFalse(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ConflictingMembersDataContractClass DataMember[0] has an HasConflictingNameAndType of true.")]
        public void ConflictingMembersDataContractClass_DataMember_0_HasConflictingNameAndType_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsTrue(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ConflictingMembersDataContractClass DataMember[1] has an HasConflictingNameAndType of false.")]
        public void ConflictingMembersDataContractClass_DataMember_1_HasConflictingNameAndType_Is_False()
        {
            // This is the current behavior--but it is technically a bug in the original DataMember code.  This should return true.  Opened bug #207613
            ClassDataContract contract = DataContract.GetDataContract(typeof(ConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsFalse(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("NameOnlyConflictingMembersDataContractClass DataMember[0] has an HasConflictingNameAndType of false.")]
        public void NameOnlyConflictingMembersDataContractClass_DataMember_0_HasConflictingNameAndType_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NameOnlyConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsFalse(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("NameOnlyConflictingMembersDataContractClass DataMember[1] has an HasConflictingNameAndType of false.")]
        public void NameOnlyConflictingMembersDataContractClass_DataMember_1_HasConflictingNameAndType_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NameOnlyConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsFalse(member.HasConflictingNameAndType, "The data member name should have had an HasConflictingNameAndType of false.");
        }

        #endregion HasConflictingNameAndType Property Tests

        #region ConflictingMember Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] has a null ConflictingMember value.")]
        public void SimpleDataContractClass_DataMember_0_ConflictingMember_Is_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsNull(member.ConflictingMember, "The data member name should have had a null ConflictingMember.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[1] has a null ConflictingMember value.")]
        public void SimpleDataContractClass_DataMember_1_ConflictingMember_Is_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsNull(member.ConflictingMember, "The data member name should have had a null ConflictingMember.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ConflictingMembersDataContractClass DataMember[0] has a non-null ConflictingMember value.")]
        public void ConflictingMembersDataContractClass_DataMember_0_ConflictingMember_Is_Non_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            ClassDataContract baseContract = DataContract.GetDataContract(typeof(SimpleMembersDataContractClass)) as ClassDataContract;
            Assert.AreSame(baseContract.Members[0], member.ConflictingMember, "The data member name should have had a ConflictingMember from the base data contract.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ConflictingMembersDataContractClass DataMember[1] has a null ConflictingMember value.")]
        public void ConflictingMembersDataContractClass_DataMember_1_ConflictingMember_Is_Null()
        {
            // This is the current behavior--but it is technically a bug in the original DataMember code.  This should return non-null.  Opened bug #207613
            ClassDataContract contract = DataContract.GetDataContract(typeof(ConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsNull(member.ConflictingMember, "The data member name should have had a null ConflictingMember.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("NameOnlyConflictingMembersDataContractClass DataMember[0] has a null ConflictingMember value.")]
        public void NameOnlyConflictingMembersDataContractClass_DataMember_0_ConflictingMember_Is_Null()
        {
            // This is the current behavior--but it is technically a bug in the original DataMember code.  This should return non-null.  Opened bug #207613
            ClassDataContract contract = DataContract.GetDataContract(typeof(NameOnlyConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsNull(member.ConflictingMember, "The data member name should have had a null ConflictingMember.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("NameOnlyConflictingMembersDataContractClass DataMember[1] has a non-null ConflictingMember value.")]
        public void NameOnlyConflictingMembersDataContractClass_DataMember_1_ConflictingMember_Is_Non_Null()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(NameOnlyConflictingMembersDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            ClassDataContract baseContract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            Assert.AreSame(baseContract.Members[0], member.ConflictingMember, "The data member name should have had a ConflictingMember from the base data contract.");
        }

        #endregion IsGetOnlyCollection Property Tests

        #region IsGetOnlyCollection Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] IsGetOnlyCollection value is false.")]
        public void SimpleDataContractClass_DataMember_0_IsGetOnlyCollection_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsFalse(member.IsGetOnlyCollection, "The data member name should have had a IsGetOnlyCollection value of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ReadOnlyCollectionDataContractClass DataMember[0] IsGetOnlyCollection value is true.")]
        public void ReadOnlyCollectionDataContractClass_DataMember_0_IsGetOnlyCollection_Is_True()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ReadOnlyCollectionDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.IsTrue(member.IsGetOnlyCollection, "The data member name should have had a IsGetOnlyCollection value of true.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ReadOnlyCollectionDataContractClass DataMember[1] IsGetOnlyCollection value is false.")]
        public void ReadOnlyCollectionDataContractClass_DataMember_1_IsGetOnlyCollection_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ReadOnlyCollectionDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.IsFalse(member.IsGetOnlyCollection, "The data member name should have had a IsGetOnlyCollection value of false.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("ReadOnlyCollectionDataContractClass DataMember[2] IsGetOnlyCollection value is false.")]
        public void ReadOnlyCollectionDataContractClass_DataMember_2_IsGetOnlyCollection_Is_False()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(ReadOnlyCollectionDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[2];
            Assert.IsFalse(member.IsGetOnlyCollection, "The data member name should have had a IsGetOnlyCollection value of false.");
        }

        #endregion IsGetOnlyCollection Property Tests

        #region Name Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] has a name of 'DataMember1.")]
        public void SimpleDataContractClass_DataMember_0_Name_Is_DataMember1()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.AreEqual("DataMember1", member.Name, "The data member name should have had a name of 'DataMember1.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[1] has a name of 'RenamedDataMember.")]
        public void SimpleDataContractClass_DataMember_1_Name_Is_RenamedDataMember()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.AreEqual("RenamedDataMember", member.Name, "The data member name should have had a name of 'RenamedDataMember.");
        }

        #endregion Name Property Tests

        #region Order Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[0] has an Order of -1.")]
        public void SimpleDataContractClass_DataMember_0_Order_Is_Negative_1()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.AreEqual(-1, member.Order, "The data member name should have had an Order of -1.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("SimpleDataContractClass DataMember[1] has an Order of -1.")]
        public void SimpleDataContractClass_DataMember_1_Order_Is_Negative_1()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(SimpleDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.AreEqual(-1, member.Order, "The data member name should have had an Order of -1.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("OrderedDataContractClass DataMember[0] has an Order of -1.")]
        public void OrderedDataContractClass_DataMember_0_Order_Is_Negative_1()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(OrderedDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[0];
            Assert.AreEqual(-1, member.Order, "The data member name should have had an Order of -1.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("OrderedDataContractClass DataMember[1] has an Order of 1.")]
        public void OrderedDataContractClass_DataMember_1_Order_Is_1()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(OrderedDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[1];
            Assert.AreEqual(1, member.Order, "The data member name should have had an Order of 1.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.DataMember)]
        [Description("OrderedDataContractClass DataMember[1] has an Order of 2.")]
        public void OrderedDataContractClass_DataMember_1_Order_Is_2()
        {
            ClassDataContract contract = DataContract.GetDataContract(typeof(OrderedDataContractClass)) as ClassDataContract;
            DataMember member = contract.Members[2];
            Assert.AreEqual(2, member.Order, "The data member name should have had an Order of 2.");
        }

        #endregion Order Property Tests
    }
}
