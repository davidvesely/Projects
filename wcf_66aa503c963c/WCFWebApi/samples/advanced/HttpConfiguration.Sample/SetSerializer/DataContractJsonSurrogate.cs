// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Runtime.Serialization;

    public class DataContractJsonSurrogate : IDataContractSurrogate
    {
        public bool WasCalled { get; set; }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            this.WasCalled = true;
            return new object();
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            this.WasCalled = true;
            return new object();
        }

        public Type GetDataContractType(Type type)
        {
            this.WasCalled = true;
            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            this.WasCalled = true;
            return obj;
        }

        public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
        {
            this.WasCalled = true;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            this.WasCalled = true;
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            this.WasCalled = true;
            return typeof(string);
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            this.WasCalled = true;
            return typeDeclaration;
        }
    }
}
