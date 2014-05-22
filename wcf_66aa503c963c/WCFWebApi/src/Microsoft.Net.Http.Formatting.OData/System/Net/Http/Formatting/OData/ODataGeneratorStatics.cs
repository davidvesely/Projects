// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>  
    /// Helper class with utility methods for creating and writing OData message format
    /// </summary>
    internal static class ODataGeneratorStatics
    {
        private static MethodInfo getODataSerializerMethod;
        private static MemberInfo defaultODataSerializerProviderInstanceProperty;
        private static MethodInfo createPropertyMethod;
        private static MethodInfo writeObjectInlineMethod;
        private static MethodInfo addPropertyToCollectionMethod;
        private static MethodInfo addObjectToCollectionMethod;
        private static MethodInfo getTypeFromHandleMethod;
        private static MethodInfo taskWaitMethod;
        private static MethodInfo writeLinkMethod;
        private static MethodInfo createEntryMethod;
        private static MethodInfo createFeedMethod;
        private static MethodInfo writeBeginEntryMethod;
        private static MethodInfo writeBeginFeedMethod;
        private static MethodInfo writeEndMethod;

        internal static MethodInfo GetODataSerializerMethod
        {
            get
            {
                if (getODataSerializerMethod == null)
                {
                    getODataSerializerMethod = typeof(ODataSerializerProvider).GetMethod("GetODataSerializer", Globals.ScanAllMembers);
                }

                return getODataSerializerMethod;
            }
        }
        
        internal static MemberInfo DefaultODataSerializerProviderInstanceField
        {
            get
            {
                if (defaultODataSerializerProviderInstanceProperty == null)
                {
                    defaultODataSerializerProviderInstanceProperty = typeof(DefaultODataSerializerProvider).GetField("Instance", Globals.ScanAllMembers);
                }

                return defaultODataSerializerProviderInstanceProperty;
            }
        }

        internal static MethodInfo CreatePropertyMethod
        {
            get
            {
                if (createPropertyMethod == null)
                {
                    createPropertyMethod = typeof(IODataSerializer).GetMethod("CreateProperty", Globals.ScanAllMembers);
                }

                return createPropertyMethod;
            }
        }
        
        internal static MethodInfo WriteObjectInlineMethod
        {
            get
            {
                if (writeObjectInlineMethod == null)
                {
                    writeObjectInlineMethod = typeof(IODataSerializer).GetMethod("WriteObjectInline", Globals.ScanAllMembers);
                }

                return writeObjectInlineMethod;
            }
        }
        
        internal static MethodInfo AddPropertyToCollectionMethod
        {
            get
            {
                if (addPropertyToCollectionMethod == null)
                {
                    addPropertyToCollectionMethod = typeof(ICollection<ODataProperty>).GetMethod("Add", Globals.ScanAllMembers);
                }

                return addPropertyToCollectionMethod;
            }
        }
        
        internal static MethodInfo AddMemberKeyValueToDictionaryMethod
        {
            get
            {
                if (addObjectToCollectionMethod == null)
                {
                    addObjectToCollectionMethod = typeof(IDictionary<string, object>).GetMethod("Add", Globals.ScanAllMembers);
                }

                return addObjectToCollectionMethod;
            }
        }
        
        internal static MethodInfo GetTypeFromHandleMethod
        {
            get
            {
                if (getTypeFromHandleMethod == null)
                {
                    getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", Globals.ScanAllMembers);
                }

                return getTypeFromHandleMethod;
            }
        }

        internal static MethodInfo TaskWaitMethod
        {
            get
            {
                if (taskWaitMethod == null)
                {
                    taskWaitMethod = typeof(Task).GetMethod("Wait", new Type[] { });
                }

                return taskWaitMethod;
            }
        }

        internal static MethodInfo WriteLinkMethod
        {
            get
            {
                if (writeLinkMethod == null)
                {
                    writeLinkMethod = typeof(ODataGeneratorUtil).GetMethod("WriteLink", Globals.ScanAllMembers);
                }

                return writeLinkMethod;
            }
        }
        
        internal static MethodInfo CreateEntryMethod
        {
            get
            {
                if (createEntryMethod == null)
                {
                    createEntryMethod = typeof(ODataGeneratorUtil).GetMethod("CreateEntry", Globals.ScanAllMembers);
                }

                return createEntryMethod;
            }
        }
        
        internal static MethodInfo CreateFeedMethod
        {
            get
            {
                if (createFeedMethod == null)
                {
                    createFeedMethod = typeof(ODataGeneratorUtil).GetMethod("CreateFeed", Globals.ScanAllMembers);
                }

                return createFeedMethod;
            }
        }
        
        internal static MethodInfo WriteBeginEntryMethod
        {
            get
            {
                if (writeBeginEntryMethod == null)
                {
                    writeBeginEntryMethod = typeof(ODataWriter).GetMethod("WriteStartAsync", new Type[] { typeof(ODataEntry) });
                }

                return writeBeginEntryMethod;
            }
        }
        
        internal static MethodInfo WriteBeginFeedMethod
        {
            get
            {
                if (writeBeginFeedMethod == null)
                {
                    writeBeginFeedMethod = typeof(ODataWriter).GetMethod("WriteStartAsync", new Type[] { typeof(ODataFeed) });
                }

                return writeBeginFeedMethod;
            }
        }
        
        internal static MethodInfo WriteEndMethod
        {
            get
            {
                if (writeEndMethod == null)
                {
                    writeEndMethod = typeof(ODataWriter).GetMethod("WriteEndAsync", Globals.ScanAllMembers);
                }

                return writeEndMethod;
            }
        }
    }
}
