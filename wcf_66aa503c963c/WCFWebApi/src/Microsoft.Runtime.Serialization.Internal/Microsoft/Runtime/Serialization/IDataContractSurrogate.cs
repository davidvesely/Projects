//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Runtime.Serialization
{
    using System;
    using System.CodeDom;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Microsoft.Server.Common;

    public interface IDataContractSurrogate
    {
        Type GetDataContractType(Type type);

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "This is cloned code.")]
        object GetObjectToSerialize(object obj, Type targetType);

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "This is cloned code.")]
        object GetDeserializedObject(object obj, Type targetType);
        object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType);
        object GetCustomDataToExport(Type clrType, Type dataContractType);
        void GetKnownCustomDataTypes(Collection<Type> customDataTypes);
        Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData);
        CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit);
    }

    static class DataContractSurrogateCaller
    {
        internal static Type GetDataContractType(IDataContractSurrogate surrogate, Type type)
        {
            if (DataContract.GetBuiltInDataContract(type) != null)
                return type;
            Type dcType = surrogate.GetDataContractType(type);
            if (dcType == null)
                return type;
            return dcType;
        }

        internal static object GetCustomDataToExport(IDataContractSurrogate surrogate, MemberInfo memberInfo, Type dataContractType)
        {
            return surrogate.GetCustomDataToExport(memberInfo, dataContractType);
        }

        internal static object GetCustomDataToExport(IDataContractSurrogate surrogate, Type clrType, Type dataContractType)
        {
            if (DataContract.GetBuiltInDataContract(clrType) != null)
                return null;
            return surrogate.GetCustomDataToExport(clrType, dataContractType);
        }
    }
}
