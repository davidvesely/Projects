// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// This class provides a default implementation for <see cref="ODataSerializerProvider"/>
    /// </summary>
#if USE_REFEMIT
    public class DefaultODataSerializerProvider : ODataSerializerProvider
#else
    internal class DefaultODataSerializerProvider : ODataSerializerProvider
#endif
    {
        /// <summary>
        /// Singleton instance of the DefaultODataSerializerProvider.
        /// </summary>
        public static readonly DefaultODataSerializerProvider Instance = new DefaultODataSerializerProvider();

        private static readonly Type keyAttributeType = typeof(KeyAttribute);
        private static readonly DataMember[] emptyDataMemberArray = new DataMember[0];

        private DefaultODataSerializerProvider()
        {
            Type openNullableType = typeof(Nullable<>);

            foreach (IODataSerializer primitiveSerializer in ODataPrimitiveSerializer.PrimitiveSerializers)
            {
                Type serializerType = primitiveSerializer.UnderlyingType;
                this.SetODataSerializer(serializerType, primitiveSerializer);

                if (serializerType.IsValueType)
                {
                    this.SetODataSerializer(openNullableType.MakeGenericType(serializerType), primitiveSerializer);
                }
            }
        }
        
        /// <summary>
        /// Creates a serializer based on the type and optional explicit key member names.
        /// </summary>
        /// <param name="type">The type for which an <see cref="IODataSerializer"/> should be created.</param>
        /// <param name="keyMemberNames">The names of the members of the <see cref="Type"/> that should be considered key members.  
        /// Maybe null if there are no explicit key memebers and the default key members should be used.</param>
        /// <returns>The <see cref="IODataSerializer"/> for the given <see cref="Type"/>.</returns>
        protected override IODataSerializer CreateSerializer(Type type, string[] keyMemberNames)
        {
            Contract.Assert(type != null, "The 'type' parameter should never be null.");

            DataContract dataContract = DataContract.GetDataContract(type);

            ClassDataContract classContract = dataContract as ClassDataContract;
            if (classContract != null)
            {
                return CreateSerializerForClass(type, keyMemberNames, classContract);
            }

            if (keyMemberNames != null)
            {
                throw new InvalidOperationException(OData.SR.TypeCannotHaveKeyMembers(type.Name));
            }
                
            CollectionDataContract collectionContract = dataContract as CollectionDataContract;           
            if (collectionContract != null)
            {
                ClassDataContract itemContract = collectionContract.ItemContract as ClassDataContract;
                return (itemContract != null && this.GetODataSerializer(itemContract.UnderlyingType) is ODataEntityTypeSerializer) ?
                    new ODataFeedSerializer(collectionContract) :
                    new ODataCollectionSerializer(collectionContract);
            }

            EnumDataContract enumContract = dataContract as EnumDataContract;
            if (enumContract != null)
            {
                return new ODataEnumSerializer(enumContract);
            }

            ObjectDataContract interfaceContract = dataContract as ObjectDataContract;
            if (interfaceContract != null)
            {
                return this.GetODataSerializer(Globals.TypeOfObject);
            }

            throw new InvalidOperationException(OData.SR.TypeCannotBeSerialized(type.Name));
        }

        private static IODataSerializer CreateSerializerForClass(Type type, string[] keyMemberNames, ClassDataContract classContract)
        {
            Contract.Assert(type != null, "The 'type' parameter should never be null.");
            Contract.Assert(classContract != null, "The 'classContract' parameter should never be null.");

            DataMember[] keyMembers;
            if (keyMemberNames != null)
            {
                keyMembers = GetMembersWithMemberNames(classContract, keyMemberNames).ToArray();
                if (keyMemberNames.Length > keyMembers.Length)
                {
                    string unknownMemberName = keyMemberNames.Except(keyMembers.Select(m => m.Name)).FirstOrDefault();
                    throw new InvalidOperationException(OData.SR.TypeDoesNotHaveMember(type.Name, unknownMemberName));
                }
            }
            else
            {
                keyMembers = GetDefaultKeyMembers(classContract).ToArray();
            }

            return keyMembers.Length > 0 ?
                new ODataEntityTypeSerializer(classContract, keyMembers) :
                new ODataComplexTypeSerializer(classContract);
        }

        private static IEnumerable<DataMember> GetKeyMembersByName(ClassDataContract classContract, string[] memberNames)
        {
            Contract.Assert(classContract != null, "The 'classContract' parameter should never be null.");
            Contract.Assert(memberNames != null, "The 'memberNames' parameter should never be null.");

            foreach (string memberName in memberNames)
            {
                DataMember matchingMember = null;
                foreach (DataMember member in GetAllDataMembers(classContract))
                {
                    if (string.Equals(member.Name, memberName, StringComparison.Ordinal))
                    {
                        matchingMember = member;
                        break;
                    }
                }

                if (matchingMember == null)
                {
                    throw new InvalidOperationException(OData.SR.TypeDoesNotHaveMember(classContract.UnderlyingType.Name, memberName));
                }

                EnsureValidKeyMember(matchingMember);

                yield return matchingMember;
            }
        }

        private static IEnumerable<DataMember> GetDefaultKeyMembers(ClassDataContract classContract)
        {
            Contract.Assert(classContract != null, "The 'classContract' parameter should never be null.");

            IEnumerable<DataMember> keyMembers = GetMembersWithKeyAttribute(classContract);

            return keyMembers.Any() ?
                keyMembers :
                GetKeyMembersOfPOCOType(classContract);
        }

        private static IEnumerable<DataMember> GetAllDataMembers(ClassDataContract classDataContract)
        {
            Contract.Assert(classDataContract != null, "The 'classDataContract' parameter should never be null.");

            IEnumerable<DataMember> allMembers = emptyDataMemberArray;

            if (classDataContract.BaseContract != null)
            {
                allMembers = GetAllDataMembers(classDataContract.BaseContract);
            }

            return classDataContract.Members != null ?
                allMembers.Concat(classDataContract.Members) :
                allMembers;
        }

        private static IEnumerable<DataMember> GetMembersWithKeyAttribute(ClassDataContract classDataContract)
        {
            Contract.Assert(classDataContract != null, "The 'classDataContract' parameter should never be null.");
            return GetAllDataMembers(classDataContract).Where(DataMemberHasKeyAttribute);
        }

        private static IEnumerable<DataMember> GetMembersWithMemberNames(ClassDataContract classDataContract, string[] memberNames)
        {
            Contract.Assert(classDataContract != null, "The 'classDataContract' parameter should never be null.");
            return GetAllDataMembers(classDataContract).Where(dm => memberNames.Contains(dm.Name, StringComparer.Ordinal));
        }

        private static IEnumerable<DataMember> GetKeyMembersOfPOCOType(ClassDataContract classDataContract)
        {
            Contract.Assert(classDataContract != null, "The 'classDataContract' parameter should never be null.");

            string contractName = classDataContract.Name.Value;
            DataMember[] possibleKeyMembers = GetAllDataMembers(classDataContract).Where(IsValidKeyMember).ToArray();

            IEnumerable<DataMember> keyMembers = possibleKeyMembers.Where(dm => IsContractNameAndIdMember(contractName, dm));

            if (!keyMembers.Any())
            {
                // Didn't find any members with the name '<ContractName>Id', so now just look for members with names 'Id'
                keyMembers = possibleKeyMembers.Where(IsIdMember);
            }

            return keyMembers;
        }

        private static bool IsContractNameAndIdMember(string contractName, DataMember dataMember)
        {
            Contract.Assert(contractName != null, "The 'contractName' parameter should never be null.");
            Contract.Assert(dataMember != null, "The 'dataMember' parameter should never be null.");
            Contract.Assert(ODataConstants.Id == "Id", "The ODataConstant.Id must be 'Id'");

            string dataMemberName = dataMember.Name;
            int dataMemberNameLength = dataMemberName.Length;

            return dataMemberName.StartsWith(contractName, StringComparison.Ordinal) &&
                   dataMemberNameLength == contractName.Length + ODataConstants.Id.Length &&
                   dataMemberName[dataMemberNameLength - 2] == ODataConstants.Id[0] &&
                   (dataMemberName[dataMemberNameLength - 1] == ODataConstants.Id[1] ||
                    dataMemberName[dataMemberNameLength - 1] == char.ToUpperInvariant(ODataConstants.Id[1]));
        }

        private static bool IsIdMember(DataMember dataMember)
        {
            Contract.Assert(dataMember != null, "The 'dataMember' parameter should never be null.");

            return dataMember.Name.Equals(ODataConstants.Id, StringComparison.OrdinalIgnoreCase);
        }

        private static bool DataMemberHasKeyAttribute(DataMember dataMember)
        {
            Contract.Assert(dataMember != null, "The 'dataMember' parameter should never be null.");

            if (dataMember.MemberInfo.GetCustomAttributes(keyAttributeType, false).Any())
            {
                EnsureValidKeyMember(dataMember);
                return true;
            }

            return false; 
        }

        private static void EnsureValidKeyMember(DataMember dataMember)
        {
            Contract.Assert(dataMember != null, "The 'dataMember' parameter should never be null.");
            if (!IsValidKeyMember(dataMember))
            {
                throw new InvalidOperationException(
                        SR.MemberCannotBeKey(dataMember.Name, dataMember.MemberInfo.DeclaringType));
            }
        }

        private static bool IsValidKeyMember(DataMember dataMember)
        {
            Contract.Assert(dataMember != null, "The 'dataMember' parameter should never be null.");
            return ODataPrimitiveSerializer.IsODataPrimitiveType(dataMember.MemberType);
        }
    }
}
