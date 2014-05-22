// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// Util class used while generating CreateProperty and WriteObject
    /// </summary>
#if USE_REFEMIT
    public static class ODataGeneratorUtil
#else
    internal static class ODataGeneratorUtil
#endif
    {   
        /// <summary>
        /// Creates and adds an ODataProperty
        /// </summary>
        /// <param name="objValue">Value of the item</param>
        /// <param name="memberName">Name of the item</param>
        /// <param name="includeAnnotatedType">Indicates if the type should be included</param>
        /// <param name="serializer">Serializer for the type associated with the item</param>
        /// <param name="writeContext">WriteContext for the parent type</param>
        /// <param name="propertyBag">ODataProperty of the type</param>
        public static void CreateAndAddProperty(
            object objValue,
            string memberName,
            bool includeAnnotatedType,
            IODataSerializer serializer,
            ODataSerializerWriteContext writeContext,
            List<ODataProperty> propertyBag)
        {
            if (serializer.SerializerType == SerializerType.Primitive || 
                serializer.SerializerType == SerializerType.ComplexType ||
                includeAnnotatedType)
            {
                propertyBag.Add(serializer.CreateProperty(objValue, memberName, writeContext));
            }
        }

        /// <summary>
        /// Creates an ODataEntry for the Entity type associated with the <paramref name="classDataContract"/>
        /// </summary>
        /// <param name="classDataContract"><see cref="ClassDataContract"/> for the Entity type being serialized.</param>
        /// <param name="propertyBag">ODataProperty assoicated with the entry.</param>
        /// <param name="keyValues">Collection of keys for the entity type.</param>
        /// <returns><see cref="ODataEntry"/> corresponding to the <paramref name="classDataContract"/></returns>
        public static ODataEntry CreateEntry(ClassDataContract classDataContract, IEnumerable<ODataProperty> propertyBag, Dictionary<string, object> keyValues)
        {
            Contract.Assert(keyValues != null, "keyValues parameter should be a non-null Value");
            Contract.Assert(keyValues.Count() > 0, "keyValues parameter should should have atleast one key-value pair");

            int keyValuesCount = keyValues.Count();

            ODataEntry entry = new ODataEntry();
            StringBuilder idBuilder = new StringBuilder(ODataGeneratorUtil.GetFormatedStableName(classDataContract.StableName));
            
            idBuilder.Append('(');            
            
            int i = 0;            
            foreach (KeyValuePair<string, object> keyValuePair in keyValues)
            {
                string memberName = keyValuePair.Key;
                object memberValue = keyValuePair.Value;

                if (memberValue == null)
                {
                    throw new SerializationException(SR.CannotSerializerKeyValueNull(memberName, classDataContract.StableName.ToString()));
                }

                if (keyValuesCount > 1)
                {
                    idBuilder.Append(memberName + "=");
                }

                Type memberType = memberValue.GetType();

                if (memberType.IsValueType && 
                    memberType != Globals.TypeOfGuid && 
                    memberType != Globals.TypeOfTimeSpan && 
                    memberType != Globals.TypeOfDateTimeOffset)
                {
                    idBuilder.Append(memberValue);                    
                }
                else
                {
                    idBuilder.Append("'" + memberValue + "'");
                }

                if (++i < keyValuesCount)
                {
                    idBuilder.Append(",");
                }
            }

            idBuilder.Append(')');

            entry.Id = idBuilder.ToString();
            entry.ReadLink = new Uri(idBuilder.ToString());
            entry.TypeName = ODataGeneratorUtil.GetFormatedStableName(classDataContract.StableName);
            entry.Properties = propertyBag;            
            
            return entry;
        }

        /// <summary>
        /// Creates an ODataFeed for the <paramref name="collectionDataContract"/>
        /// </summary>
        /// <param name="collectionDataContract"><see cref="CollectionDataContract"/> for the feed type.</param>
        /// <returns><see cref="ODataFeed"/> corresponding to the <paramref name="collectionDataContract"/></returns>
        public static ODataFeed CreateFeed(CollectionDataContract collectionDataContract)
        {
            string stableName = ODataGeneratorUtil.GetFormatedStableName(collectionDataContract.ItemContract.StableName);
            ODataFeed feed = new ODataFeed();
            feed.Id = stableName;
            
            return feed;
        }

        /// <summary>
        /// Writes a link for the specified <paramref name="entry"/> type
        /// </summary>
        /// <param name="objValue">Referenced entity type for which the link is generated.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="entry">ODataEntry for which the link is generated.</param>
        /// <param name="serializer">Serializer used for inlining the link.</param>
        /// <param name="writer">ODataWriter for writing the link.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> for serializing the type.</param>
        /// <param name="inlineLink">Indicates if the link is inlined</param>
        public static void WriteLink(            
            object objValue,
            string memberName, 
            ODataEntry entry,
            IODataSerializer serializer, 
            ODataWriter writer, 
            ODataSerializerWriteContext writeContext,
            bool inlineLink) 
        {
            bool isExpanded = (objValue != null) && inlineLink;
            string linkUrl = entry.Id + "/" + memberName;
            bool isCollection = false;

            if (serializer.SerializerType == SerializerType.Entity)
            {
                isCollection = false;
            }
            else if (serializer.SerializerType == SerializerType.Feed)
            {
                isCollection = true;
            }            
            else
            {
                // This property will be in the propertyBag
                return;
            }

            ODataNavigationLink link = new ODataNavigationLink()
            {                
                IsCollection = isCollection,                
                Name = ODataConstants.DataServiceRelatedNamespace + memberName,
                Url = new Uri(linkUrl)
            };
            
            writer.WriteStartAsync(link).Wait();

            if (isExpanded)
            {
                serializer.WriteObjectInline(objValue, writer, writeContext);                
            }

            writer.WriteEndAsync().Wait();
        }

        internal static string GetFormatedStableName(XmlQualifiedName stableName)
        {
            StringBuilder formattedStableName = new StringBuilder();

            if (!string.IsNullOrEmpty(stableName.Namespace))
            {
                formattedStableName.Append(stableName.Namespace + "/");
            }

            formattedStableName.Append(stableName.Name);
            return formattedStableName.ToString();
        }
    }
}
