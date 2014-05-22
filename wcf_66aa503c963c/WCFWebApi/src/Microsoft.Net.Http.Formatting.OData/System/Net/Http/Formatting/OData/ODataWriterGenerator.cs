// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading.Tasks;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>  
    /// Helper class that generates il code for generating OData properties and writing OData entries, feeds and links
    /// </summary>
    internal class ODataWriterGenerator
    {        
        private static readonly Type[] ArgTypeCreatePropertyFromCollection = new Type[] { typeof(object), typeof(ODataSerializerWriteContext), typeof(CollectionDataContract), typeof(bool) };
        private static readonly Type[] ArgTypeCreatePropertyFromClass = new Type[] { typeof(object), typeof(ODataSerializerWriteContext), typeof(ClassDataContract), typeof(bool) };
        private static readonly Type[] ArgTypeWriteEntry = new Type[] { typeof(object), typeof(IEnumerable<ODataProperty>), typeof(ODataWriter), typeof(ODataSerializerWriteContext), typeof(ClassDataContract), typeof(bool) };
        private static readonly Type[] ArgTypeWriteFeed = new Type[] { typeof(object), typeof(ODataWriter), typeof(ODataSerializerWriteContext), typeof(CollectionDataContract) };

        private static Module serializationModule;

        private CodeGenerator ilg;

        private ArgBuilder writeContextArg;
        private ArgBuilder dataContractArg;
        private ArgBuilder includeAnnotateTypeArg;

        private LocalBuilder objectGraphLocal;
        private LocalBuilder propertyCollectionLocal;
        private LocalBuilder serializerProviderLocal;

        // For writing an entry
        private ArgBuilder propertyCollectionArg;
        private ArgBuilder writerArg;
        private ArgBuilder inlineLinksArg;
        private LocalBuilder entryLocal;
        private LocalBuilder taskLocal;
      
        private static Module ODataSerializationModule
        {
            get
            {
                if (ODataWriterGenerator.serializationModule == null)
                {
                    ODataWriterGenerator.serializationModule = typeof(ODataWriterGenerator).Module;
                }

                return ODataWriterGenerator.serializationModule;
            }
        }

        internal static Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>> GeneratePropertyBagDelegate(ClassDataContract classDataContract)
        {
            ODataWriterGenerator writerGenerator = new ODataWriterGenerator();
            return writerGenerator.CreatePropertyBagDelegate(classDataContract);
        }

        internal static Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>> GeneratePropertyBagDelegate(CollectionDataContract collectionDataContract)
        {
            ODataWriterGenerator writerGenerator = new ODataWriterGenerator();
            return writerGenerator.CreatePropertyBagDelegate(collectionDataContract);
        }

        internal static Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool> GenerateWriteEntryDelegate(ClassDataContract classDataContract, DataMember[] memberKeys)
        {
            ODataWriterGenerator writerGenerator = new ODataWriterGenerator();
            return writerGenerator.CreateWriteEntryDelegate(classDataContract, memberKeys);
        }

        internal static Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract> GenerateWriteFeedDelegate(CollectionDataContract collectionDataContract)
        {
            ODataWriterGenerator writerGenerator = new ODataWriterGenerator();
            return writerGenerator.CreateWriteFeedDelegate(collectionDataContract);
        }

        private static bool ShouldGenerateSerializerDuringRuntime(IODataSerializer serializer, Type memberType)
        {
            return ((serializer.SerializerType == SerializerType.Primitive && serializer.UnderlyingType != Globals.TypeOfObject) || memberType.IsSealed) ? false : true;
        }

        private Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>> CreatePropertyBagDelegate(ClassDataContract classDataContract)
        {         
            this.ilg = new CodeGenerator();

            string methodName = "CreatePropertyBagFrom" + classDataContract.StableName.Name;
            bool memberAccessFlag = classDataContract.RequiresMemberAccessForWrite(null);

            try
            {
#if USE_REFEMIT
                this.ilg.BeginMethod(
                    methodName,
                    typeof(Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>>), 
                    memberAccessFlag);
#else
                DynamicMethod dynamicMethod = new DynamicMethod(
                    methodName,
                    typeof(IEnumerable<ODataProperty>),
                    ODataWriterGenerator.ArgTypeCreatePropertyFromClass,
                    ODataWriterGenerator.ODataSerializationModule,
                    memberAccessFlag);

                this.ilg.BeginMethod(
                    dynamicMethod,
                    typeof(Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>>),
                    methodName,
                    ODataWriterGenerator.ArgTypeCreatePropertyFromClass,
                    memberAccessFlag);
#endif
            }
            catch (SecurityException securityException)
            {
                if (memberAccessFlag && securityException.PermissionType.Equals(typeof(ReflectionPermission)))
                {
                    classDataContract.RequiresMemberAccessForWrite(securityException);
                }
                else
                {
                    throw;
                }
            }

            this.writeContextArg = this.ilg.GetArg(1);
            this.dataContractArg = this.ilg.GetArg(2);
            this.includeAnnotateTypeArg = this.ilg.GetArg(3);            

            this.objectGraphLocal = this.ilg.DeclareLocal(classDataContract.UnderlyingType, "objSerialized");
            ArgBuilder objectArg = this.ilg.GetArg(0);
            this.ilg.Load(objectArg);
            this.ilg.ConvertValue(objectArg.ArgType, classDataContract.UnderlyingType);
            this.ilg.Stloc(this.objectGraphLocal);

            this.propertyCollectionLocal = this.ilg.DeclareLocal(typeof(List<ODataProperty>), "propertyCollection");
            this.ilg.New(typeof(List<ODataProperty>).GetConstructor(new Type[] { }));
            this.ilg.Store(this.propertyCollectionLocal);

            this.serializerProviderLocal = this.ilg.DeclareLocal(typeof(DefaultODataSerializerProvider), "serializerProvider");
            this.ilg.LoadMember(ODataGeneratorStatics.DefaultODataSerializerProviderInstanceField);
            this.ilg.Store(this.serializerProviderLocal);

            this.GeneratePropertyBagFromMembers(classDataContract);

            this.ilg.Load(this.propertyCollectionLocal);

            return (Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>>)this.ilg.EndMethod();
        }

        private Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>> CreatePropertyBagDelegate(CollectionDataContract collectionDataContract)
        {
            this.ilg = new CodeGenerator();

            string methodName = "CreatePropertyBagFrom" + collectionDataContract.StableName.Name;
            bool memberAccessFlag = collectionDataContract.RequiresMemberAccessForWrite(null);
            
            try
            {
#if USE_REFEMIT
                this.ilg.BeginMethod(
                    methodName,
                    typeof(Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>>),
                    memberAccessFlag);                
#else
                DynamicMethod dynamicMethod = new DynamicMethod(
                    methodName,
                    typeof(IEnumerable<ODataProperty>),
                    ODataWriterGenerator.ArgTypeCreatePropertyFromCollection,
                    ODataWriterGenerator.ODataSerializationModule,
                    memberAccessFlag);

                this.ilg.BeginMethod(
                    dynamicMethod,
                    typeof(Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>>),
                    methodName,
                    ODataWriterGenerator.ArgTypeCreatePropertyFromCollection,
                    memberAccessFlag);
#endif
            }
            catch (SecurityException securityException)
            {
                if (memberAccessFlag && securityException.PermissionType.Equals(typeof(ReflectionPermission)))
                {
                    collectionDataContract.RequiresMemberAccessForWrite(securityException);
                }
                else
                {
                    throw;
                }
            }

            this.writeContextArg = this.ilg.GetArg(1);
            this.dataContractArg = this.ilg.GetArg(2);
            this.includeAnnotateTypeArg = this.ilg.GetArg(3);

            this.objectGraphLocal = this.ilg.DeclareLocal(collectionDataContract.UnderlyingType, "objSerialized");

            ArgBuilder objectArg = this.ilg.GetArg(0);
            this.ilg.Load(objectArg);
            this.ilg.ConvertValue(objectArg.ArgType, collectionDataContract.UnderlyingType);
            this.ilg.Stloc(this.objectGraphLocal);

            this.propertyCollectionLocal = this.ilg.DeclareLocal(typeof(List<ODataProperty>), "propertyCollection");
            this.ilg.New(typeof(List<ODataProperty>).GetConstructor(new Type[] { }));
            this.ilg.Store(this.propertyCollectionLocal);

            this.serializerProviderLocal = this.ilg.DeclareLocal(typeof(DefaultODataSerializerProvider), "serializerProvider");
            this.ilg.LoadMember(ODataGeneratorStatics.DefaultODataSerializerProviderInstanceField);
            this.ilg.Store(this.serializerProviderLocal);

            this.GenerateFromCollection(collectionDataContract, false);

            this.ilg.Load(this.propertyCollectionLocal);

            return (Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>>)this.ilg.EndMethod();
        }

        private Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool> CreateWriteEntryDelegate(ClassDataContract classDataContract, DataMember[] memberKeys)
        {            
            this.ilg = new CodeGenerator();

            string methodName = "Write" + classDataContract.StableName.Name + "AsODataEntry";
            bool memberAccessFlag = classDataContract.RequiresMemberAccessForWrite(null);
            
            try
            {
#if USE_REFEMIT
                this.ilg.BeginMethod(
                    methodName,
                    typeof(Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool>),
                    memberAccessFlag);
#else
                DynamicMethod dynamicMethod = new DynamicMethod(
                    methodName,
                    null,
                    ODataWriterGenerator.ArgTypeWriteEntry,
                    ODataWriterGenerator.ODataSerializationModule,
                    memberAccessFlag);

                this.ilg.BeginMethod(
                    dynamicMethod,
                    typeof(Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool>),
                    methodName,
                    ODataWriterGenerator.ArgTypeWriteEntry,
                    memberAccessFlag);
#endif
            }
            catch (SecurityException securityException)
            {
                if (memberAccessFlag && securityException.PermissionType.Equals(typeof(ReflectionPermission)))
                {
                    classDataContract.RequiresMemberAccessForWrite(securityException);
                }
                else
                {
                    throw;
                }
            }

            this.propertyCollectionArg = this.ilg.GetArg(1);
            this.writerArg = this.ilg.GetArg(2);
            this.writeContextArg = this.ilg.GetArg(3);
            this.dataContractArg = this.ilg.GetArg(4);
            this.inlineLinksArg = this.ilg.GetArg(5);

            this.objectGraphLocal = this.ilg.DeclareLocal(classDataContract.UnderlyingType, "objSerialized");
            ArgBuilder objectArg = this.ilg.GetArg(0);
            this.ilg.Load(objectArg);
            this.ilg.ConvertValue(objectArg.ArgType, classDataContract.UnderlyingType);
            this.ilg.Stloc(this.objectGraphLocal);

            this.serializerProviderLocal = this.ilg.DeclareLocal(typeof(DefaultODataSerializerProvider), "serializerProvider");
            this.ilg.LoadMember(ODataGeneratorStatics.DefaultODataSerializerProviderInstanceField);
            this.ilg.Store(this.serializerProviderLocal);

            this.entryLocal = this.InitializeEntry(memberKeys);

            this.taskLocal = this.ilg.DeclareLocal(typeof(Task), "task");
            this.ilg.Call(
                this.writerArg,
                ODataGeneratorStatics.WriteBeginEntryMethod,
                this.entryLocal);
            this.ilg.Store(this.taskLocal);
            this.ilg.Call(this.taskLocal, ODataGeneratorStatics.TaskWaitMethod);
            
            this.WriteLinks(classDataContract);            

            this.ilg.Call(this.writerArg, ODataGeneratorStatics.WriteEndMethod);
            this.ilg.Store(this.taskLocal);
            this.ilg.Call(this.taskLocal, ODataGeneratorStatics.TaskWaitMethod);
            
            return (Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool>)this.ilg.EndMethod();
        }

        private Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract> CreateWriteFeedDelegate(CollectionDataContract collectionDataContract)
        {
            this.ilg = new CodeGenerator();

            bool memberAccessFlag = collectionDataContract.RequiresMemberAccessForWrite(null);
            string methodName = "Write" + collectionDataContract.StableName.Name + "AsODataFeed";

            try
            {
#if USE_REFEMIT
                this.ilg.BeginMethod(
                    methodName,
                    typeof(Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract>),
                    memberAccessFlag);
#else
                DynamicMethod dynamicMethod = new DynamicMethod(
                    methodName,
                    null,
                    ODataWriterGenerator.ArgTypeWriteFeed,
                    ODataWriterGenerator.ODataSerializationModule,
                    memberAccessFlag);

                this.ilg.BeginMethod(
                    dynamicMethod,
                    typeof(Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract>),
                    methodName,
                    ODataWriterGenerator.ArgTypeWriteFeed,
                    memberAccessFlag);
#endif
            }
            catch (SecurityException securityException)
            {
                if (memberAccessFlag && securityException.PermissionType.Equals(typeof(ReflectionPermission)))
                {
                    collectionDataContract.RequiresMemberAccessForWrite(securityException);
                }
                else
                {
                    throw;
                }
            }

            this.writerArg = this.ilg.GetArg(1);
            this.writeContextArg = this.ilg.GetArg(2);
            this.dataContractArg = this.ilg.GetArg(3);

            this.objectGraphLocal = this.ilg.DeclareLocal(collectionDataContract.UnderlyingType, "objSerialized");
            ArgBuilder objectArg = this.ilg.GetArg(0);
            this.ilg.Load(objectArg);
            this.ilg.ConvertValue(objectArg.ArgType, collectionDataContract.UnderlyingType);
            this.ilg.Stloc(this.objectGraphLocal);

            this.serializerProviderLocal = this.ilg.DeclareLocal(typeof(DefaultODataSerializerProvider), "serializerProvider");
            this.ilg.LoadMember(ODataGeneratorStatics.DefaultODataSerializerProviderInstanceField);
            this.ilg.Store(this.serializerProviderLocal);

            LocalBuilder feedLocal = this.InitializeFeed();

            this.taskLocal = this.ilg.DeclareLocal(typeof(Task), "task");
            this.ilg.Call(
                this.writerArg,
                ODataGeneratorStatics.WriteBeginFeedMethod,
                feedLocal);
            this.ilg.Store(this.taskLocal);
            this.ilg.Call(this.taskLocal, ODataGeneratorStatics.TaskWaitMethod);

            this.GenerateFromCollection(collectionDataContract, true);

            this.ilg.Call(this.writerArg, ODataGeneratorStatics.WriteEndMethod);
            this.ilg.Store(this.taskLocal);
            this.ilg.Call(this.taskLocal, ODataGeneratorStatics.TaskWaitMethod);

            return (Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract>)this.ilg.EndMethod();
        }

        private LocalBuilder InitializeEntry(DataMember[] memberKeys)
        {
            LocalBuilder keyValueDictionaryLocal = this.ilg.DeclareLocal(typeof(Dictionary<string, object>), "keyValueDictionary");
            this.ilg.New(typeof(Dictionary<string, object>).GetConstructor(new Type[] { }));
            this.ilg.Store(keyValueDictionaryLocal);

            foreach (DataMember member in memberKeys)
            {
                this.ilg.Load(keyValueDictionaryLocal);
                this.ilg.Ldstr(member.Name);
                this.ilg.LdlocAddress(this.objectGraphLocal);
                this.ilg.LoadMember(member.MemberInfo);
                this.ilg.ConvertValue(member.MemberType, Globals.TypeOfObject);
                this.ilg.Call(ODataGeneratorStatics.AddMemberKeyValueToDictionaryMethod);
            }

            LocalBuilder entryLocalVar = this.ilg.DeclareLocal(typeof(ODataEntry), "entry");
            this.ilg.Load(this.dataContractArg);
            this.ilg.Load(this.propertyCollectionArg);
            this.ilg.Load(keyValueDictionaryLocal);
            this.ilg.Call(ODataGeneratorStatics.CreateEntryMethod);
            this.ilg.Store(entryLocalVar);

            return entryLocalVar;
        }

        private LocalBuilder InitializeFeed()
        {
            LocalBuilder feedLocal = this.ilg.DeclareLocal(typeof(ODataFeed), "feed");
            this.ilg.Load(this.dataContractArg);
            this.ilg.Call(ODataGeneratorStatics.CreateFeedMethod);
            this.ilg.Store(feedLocal);

            return feedLocal;
        }

        private void GeneratePropertyBagFromMembers(ClassDataContract currentClassDataContract)
        {
            if (currentClassDataContract.BaseContract != null)
            {
                this.GeneratePropertyBagFromMembers(currentClassDataContract.BaseContract);
            }

            if (currentClassDataContract.Members != null)
            {
                foreach (DataMember member in currentClassDataContract.Members)
                {
                    LocalBuilder memberValue = null;
                    Type memberType = member.MemberType;

                    IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(memberType);
                    bool resolveSerializerDuringRuntime = ODataWriterGenerator.ShouldGenerateSerializerDuringRuntime(serializer, memberType);

                    if (!member.EmitDefaultValue)
                    {
                        memberValue = this.GetMemberValue(member);
                        this.ilg.IfNotDefaultValue(memberValue);
                    }

                    if (serializer.SerializerType == SerializerType.Entity || serializer.SerializerType == SerializerType.Feed)
                    {
                        this.ilg.If(this.includeAnnotateTypeArg, Cmp.EqualTo, true);
                    }

                    this.AddMemberToPropertyCollection(member, memberValue, resolveSerializerDuringRuntime);

                    if (serializer.SerializerType == SerializerType.Entity || serializer.SerializerType == SerializerType.Feed)
                    {
                        this.ilg.EndIf();
                    }

                    if (!member.EmitDefaultValue)
                    {
                        if (member.IsRequired)
                        {
                            this.ilg.Else();
                            this.ilg.Call(null, XmlFormatGeneratorStatics.ThrowRequiredMemberMustBeEmittedMethod, member.Name, currentClassDataContract.UnderlyingType);
                        }

                        this.ilg.EndIf();
                    }
                }
            }
        }

        private void GenerateFromCollection(CollectionDataContract collectionContract, bool isFeed)
        {
            Type itemType = collectionContract.ItemType;
            IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(itemType);
            bool resolveSerializerDuringRuntime = ODataWriterGenerator.ShouldGenerateSerializerDuringRuntime(serializer, itemType);

            LocalBuilder serializerLocal = null;
            if (!resolveSerializerDuringRuntime)
            {
                serializerLocal = this.GetSerializerForSealedType(itemType);
            }

            if (collectionContract.Kind == CollectionKind.Array)
            {
                LocalBuilder i = this.ilg.DeclareLocal(Globals.TypeOfInt, "i");
                this.ilg.For(i, 0, this.objectGraphLocal);
                LocalBuilder memberValue = this.GetArrayElementValue(i, itemType);
               
                if (resolveSerializerDuringRuntime)
                {
                    serializerLocal = this.GetSerializerForInheritableType(memberValue, itemType);
                }

                if (isFeed)
                {                    
                    this.WriteEntryFragment(serializerLocal, memberValue, itemType);                    
                }
                else
                {
                    this.AddItemToPropertyCollection(serializerLocal, memberValue, itemType);
                }
                
                this.ilg.EndFor();
            }
            else
            {
                bool isDictionary = false, isGenericDictionary = false;
                Type enumeratorType = null;
                Type[] keyValueTypes = null;

                if (collectionContract.Kind == CollectionKind.GenericDictionary)
                {
                    isGenericDictionary = true;
                    keyValueTypes = collectionContract.ItemType.GetGenericArguments();
                    enumeratorType = Globals.TypeOfGenericDictionaryEnumerator.MakeGenericType(keyValueTypes);
                }
                else if (collectionContract.Kind == CollectionKind.Dictionary)
                {
                    isDictionary = true;
                    keyValueTypes = new Type[] { Globals.TypeOfObject, Globals.TypeOfObject };
                    enumeratorType = Globals.TypeOfDictionaryEnumerator;
                }
                else
                {
                    enumeratorType = collectionContract.GetEnumeratorMethod.ReturnType;
                }

                MethodInfo moveNextMethod = enumeratorType.GetMethod(Globals.MoveNextMethodName, BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
                MethodInfo getCurrentMethod = enumeratorType.GetMethod(Globals.GetCurrentMethodName, BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);

                if (moveNextMethod == null || getCurrentMethod == null)
                {
                    if (enumeratorType.IsInterface)
                    {
                        if (moveNextMethod == null)
                        {
                            moveNextMethod = XmlFormatGeneratorStatics.MoveNextMethod;
                        }

                        if (getCurrentMethod == null)
                        {
                            getCurrentMethod = XmlFormatGeneratorStatics.GetCurrentMethod;
                        }
                    }
                    else
                    {
                        Type ienumeratorInterface = Globals.TypeOfIEnumerator;
                        CollectionKind kind = collectionContract.Kind;
                        if (kind == CollectionKind.GenericDictionary || kind == CollectionKind.GenericCollection || kind == CollectionKind.GenericEnumerable)
                        {
                            Type[] interfaceTypes = enumeratorType.GetInterfaces();
                            foreach (Type interfaceType in interfaceTypes)
                            {
                                if (interfaceType.IsGenericType
                                    && interfaceType.GetGenericTypeDefinition() == Globals.TypeOfIEnumeratorGeneric
                                    && interfaceType.GetGenericArguments()[0] == collectionContract.ItemType)
                                {
                                    ienumeratorInterface = interfaceType;
                                    break;
                                }
                            }
                        }

                        if (moveNextMethod == null)
                        {
                            moveNextMethod = CollectionDataContract.GetTargetMethodWithName(Globals.MoveNextMethodName, enumeratorType, ienumeratorInterface);
                        }

                        if (getCurrentMethod == null)
                        {
                            getCurrentMethod = CollectionDataContract.GetTargetMethodWithName(Globals.GetCurrentMethodName, enumeratorType, ienumeratorInterface);
                        }
                    }
                }

                Type elementType = getCurrentMethod.ReturnType;
                LocalBuilder currentValue = this.ilg.DeclareLocal(elementType, "currentValue");

                LocalBuilder enumerator = this.ilg.DeclareLocal(enumeratorType, "enumerator");
                this.ilg.Call(this.objectGraphLocal, collectionContract.GetEnumeratorMethod);

                if (isDictionary)
                {
                    this.ilg.ConvertValue(collectionContract.GetEnumeratorMethod.ReturnType, Globals.TypeOfIDictionaryEnumerator);
                    this.ilg.New(XmlFormatGeneratorStatics.DictionaryEnumeratorCtor);
                }
                else if (isGenericDictionary)
                {
                    Type ctorParam = Globals.TypeOfIEnumeratorGeneric.MakeGenericType(Globals.TypeOfKeyValuePair.MakeGenericType(keyValueTypes));
                    ConstructorInfo dictEnumCtor = enumeratorType.GetConstructor(Globals.ScanAllMembers, null, new Type[] { ctorParam }, null);
                    this.ilg.ConvertValue(collectionContract.GetEnumeratorMethod.ReturnType, ctorParam);
                    this.ilg.New(dictEnumCtor);
                }

                this.ilg.Stloc(enumerator);

                this.ilg.ForEach(currentValue, elementType, enumeratorType, enumerator, getCurrentMethod);                

                if (resolveSerializerDuringRuntime)
                {
                    serializerLocal = this.GetSerializerForInheritableType(currentValue, elementType);
                }

                if (isFeed)
                {                    
                    this.WriteEntryFragment(serializerLocal, currentValue, itemType);                    
                }
                else
                {
                    this.AddItemToPropertyCollection(serializerLocal, currentValue, itemType);
                }                

                this.ilg.EndForEach(moveNextMethod);
            }
        }

        private void WriteLinks(ClassDataContract currentClassDataContract)
        {
            if (currentClassDataContract.BaseContract != null)
            {
                this.WriteLinks(currentClassDataContract.BaseContract);
            }

            foreach (DataMember member in currentClassDataContract.Members)
            {                
                Type memberType = member.MemberType;

                IODataSerializer serializer = DefaultODataSerializerProvider.Instance.GetODataSerializer(memberType);
                if (serializer.SerializerType == SerializerType.Entity || serializer.SerializerType == SerializerType.Feed)
                {
                    this.WriteLink(member, memberType.IsSealed);
                }
            }            
        }

        private void WriteLink(DataMember member, bool isSealed)
        {
            LocalBuilder memberValue = null;

            if (!member.EmitDefaultValue)
            {
                memberValue = this.GetMemberValue(member);
                this.ilg.IfNotDefaultValue(memberValue);
            }

            if (memberValue == null)
            {
                memberValue = this.GetMemberValue(member);
                this.ilg.Load(memberValue);
            }
            else
            {
                this.ilg.Load(memberValue);
            }

            this.ilg.ConvertValue(member.MemberType, Globals.TypeOfObject);
            this.ilg.Ldstr(member.Name);
            this.ilg.Load(this.entryLocal);

            if (isSealed)
            {
                this.LoadSerializerForSealedType(member.MemberType);
            }
            else
            {
                this.LoadSerializerForInheritableType(memberValue, member.MemberType);
            }

            this.ilg.Load(this.writerArg);
            this.ilg.Load(this.writeContextArg);
            this.ilg.Load(this.inlineLinksArg);

            this.ilg.Call(ODataGeneratorStatics.WriteLinkMethod);

            if (!member.EmitDefaultValue)
            {
                if (member.IsRequired)
                {
                    this.ilg.Else();
                    this.ilg.Call(null, XmlFormatGeneratorStatics.ThrowRequiredMemberMustBeEmittedMethod, member.Name, member.MemberInfo.DeclaringType);
                }

                this.ilg.EndIf();
            }
        }

        private void WriteEntryFragment(LocalBuilder serializerLocal, LocalBuilder itemValue, Type itemType)
        {
            this.ilg.If(itemValue, Cmp.NotEqualTo, null);
            this.ilg.Load(serializerLocal);
            this.ilg.Load(itemValue);
            this.ilg.ConvertValue(itemType, Globals.TypeOfObject);
            this.ilg.Load(this.writerArg);
            this.ilg.Load(this.writeContextArg);
            this.ilg.Call(ODataGeneratorStatics.WriteObjectInlineMethod);
            this.ilg.EndIf();
        }

        private void AddItemToPropertyCollection(LocalBuilder serializerLocal, LocalBuilder itemValue, Type itemType)
        {
            this.ilg.Load(this.propertyCollectionLocal);
            this.ilg.Load(serializerLocal);
            this.ilg.Load(itemValue);
            this.ilg.ConvertValue(itemType, Globals.TypeOfObject);
            this.ilg.Ldstr(ODataConstants.Element);
            this.ilg.Load(this.writeContextArg);
            this.ilg.Call(ODataGeneratorStatics.CreatePropertyMethod);
            this.ilg.Call(ODataGeneratorStatics.AddPropertyToCollectionMethod);
        }

        private void AddMemberToPropertyCollection(DataMember member, LocalBuilder memberValue, bool resolveSerializerDuringRuntime)
        {
            this.ilg.Load(this.propertyCollectionLocal);

            if (resolveSerializerDuringRuntime)
            {
                memberValue = memberValue ?? this.GetMemberValue(member);
                this.LoadSerializerForInheritableType(memberValue, member.MemberType);
            }
            else
            {
                this.LoadSerializerForSealedType(member.MemberType);
            }

            if (memberValue != null)
            {
                this.ilg.Load(memberValue);
            }
            else
            {
                this.ilg.LoadAddress(this.objectGraphLocal);
                this.ilg.LoadMember(member.MemberInfo);
            }

            this.ilg.ConvertValue(member.MemberType, Globals.TypeOfObject);
            this.ilg.Ldstr(member.Name);
            this.ilg.Load(this.writeContextArg);
            this.ilg.Call(ODataGeneratorStatics.CreatePropertyMethod);
            this.ilg.Call(ODataGeneratorStatics.AddPropertyToCollectionMethod);
        }

        private void LoadSerializerForInheritableType(LocalBuilder memberValue, Type memberType)
        {
            this.ilg.Load(this.serializerProviderLocal);
            this.ilg.If(memberValue, Cmp.EqualTo, null);
            this.ilg.Ldtoken(memberType);
            this.ilg.Call(ODataGeneratorStatics.GetTypeFromHandleMethod);
            this.ilg.Else();
            this.ilg.Call(memberValue, Globals.TypeOfObject.GetMethod("GetType"));
            this.ilg.EndIf();
            this.ilg.Call(ODataGeneratorStatics.GetODataSerializerMethod);
        }

        private void LoadSerializerForSealedType(Type memberType)
        {
            this.ilg.Load(this.serializerProviderLocal);
            this.ilg.Ldtoken(memberType);
            this.ilg.Call(ODataGeneratorStatics.GetTypeFromHandleMethod);
            this.ilg.Call(ODataGeneratorStatics.GetODataSerializerMethod);
        }

        private LocalBuilder GetSerializerForInheritableType(LocalBuilder memberValue, Type memberType)
        {
            LocalBuilder serializerLocal = this.ilg.DeclareLocal(typeof(IODataSerializer), "serializer");
            this.LoadSerializerForInheritableType(memberValue, memberType);
            this.ilg.Store(serializerLocal);
            return serializerLocal;
        }

        private LocalBuilder GetSerializerForSealedType(Type memberType)
        {
            LocalBuilder serializerLocal = this.ilg.DeclareLocal(typeof(IODataSerializer), "serializer");
            this.LoadSerializerForSealedType(memberType);
            this.ilg.Store(serializerLocal);
            return serializerLocal;
        }

        private LocalBuilder GetMemberValue(DataMember member)
        {
            this.ilg.LoadAddress(this.objectGraphLocal);
            this.ilg.LoadMember(member.MemberInfo);
            LocalBuilder memberValue = this.ilg.DeclareLocal(member.MemberType, member.Name + "Value");
            this.ilg.Stloc(memberValue);
            return memberValue;
        }

        private LocalBuilder GetArrayElementValue(LocalBuilder index, Type itemType)
        {
            this.ilg.LoadArrayElement(this.objectGraphLocal, index);
            LocalBuilder itemValue = this.ilg.DeclareLocal(itemType, "Value");
            this.ilg.Stloc(itemValue);
            return itemValue;
        }        
    }
}
