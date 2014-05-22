// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// Class for serializing collection types
    /// </summary>
    internal class ODataCollectionSerializer : IODataSerializer
    {
        private readonly CollectionDataContract collectionDataContract;

        private object createPropertyBagDelegateLock = new object();
        private Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>> createPropertyBagDelegate;

        /// <summary>
        /// Initializes an <see cref="ODataCollectionSerializer"/>.
        /// </summary>
        /// <param name="collectionDataContract"><see cref="CollectionDataContract"/> associated with the type.</param>
        public ODataCollectionSerializer(CollectionDataContract collectionDataContract)
        {
            this.collectionDataContract = collectionDataContract;
        }

        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        public Type UnderlyingType
        {
            get
            {
                return this.collectionDataContract.UnderlyingType;
            }
        }

        /// <summary>
        /// Gets the serializer for the type.
        /// </summary>
        public virtual SerializerType SerializerType
        {
            get
            {
                return SerializerType.Collection;
            }
        }

        /// <summary>
        /// Gets the <see cref="ODataPayloadKind"/> associated with the type.
        /// </summary>
        public virtual ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Collection;
            }
        }

        /// <summary>
        /// Gets the property bag delegate that is il generated
        /// </summary>
        public Func<object, ODataSerializerWriteContext, CollectionDataContract, bool, IEnumerable<ODataProperty>> CreatePropertyBagDelegate
        {
            get
            {
                if (this.createPropertyBagDelegate == null)
                {
                    lock (this.createPropertyBagDelegateLock)
                    {
                        if (this.createPropertyBagDelegate == null)
                        {
                            this.createPropertyBagDelegate = ODataWriterGenerator.GeneratePropertyBagDelegate(this.collectionDataContract);
                        }
                    }
                }

                return this.createPropertyBagDelegate;
            }
        }

        /// <summary>
        /// Gets the CollectionDataContract for the type being serialized.
        /// </summary>
        protected CollectionDataContract CollectionDataContract
        {
            get
            {
                return this.collectionDataContract;
            }
        }

        /// <summary>
        /// Implements logic for writing collection type.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <param name="messageWriter"><see cref="ODataMessageWriter"/> used for writing the property.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public virtual void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
        {
            Task<ODataCollectionWriter> writerTask = messageWriter.CreateODataCollectionWriterAsync();
            writerTask.Wait();

            ODataCollectionWriter writer = writerTask.Result;
            writer.WriteStartAsync(new ODataCollectionResult { Name = writeContext.ResponseContext.ServiceOperationName }).Wait();
            
            ODataProperty property = this.CreateProperty(graph, writeContext.ResponseContext.ServiceOperationName, writeContext);
            ODataMultiValue multiValue = property.Value as ODataMultiValue;
            
            foreach (object item in multiValue.Items)
            {
                writer.WriteItemAsync(item).Wait();
                writer.FlushAsync().Wait();
            }

            writer.WriteEndAsync().Wait();
            writer.FlushAsync().Wait();
        }

        /// <summary>
        /// This method is not supported for the collection type.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="writer"><see cref="ODataWriter"/> used for writing out the type.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public virtual void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext)
        {            
            throw new NotSupportedException(SR.WriteObjectInlineNotSupported(this.GetType().Name));
        }

        /// <summary>
        /// Creates an <see cref="ODataProperty"/> for the object graph.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="elementName">Name of the element represented by the <paramref name="graph"/>. </param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        /// <returns><see cref="ODataProperty"/> for the object graph.</returns>
        public ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
        {
            IEnumerable<ODataProperty> propertyCollection = null;

            if (graph != null)
            {                
                propertyCollection = this.CreatePropertyBagDelegate(graph, writeContext, this.collectionDataContract, true);             
            }

            List<object> valueCollection = new List<object>();
            foreach (ODataProperty property in propertyCollection)
            {
                valueCollection.Add(property.Value);
            }

            // MutliValue is only a V3 property, arrays inside Complex Types or Entity types are only supported in V3
            // if a V1 or V2 Client requests a type that has a collection within it ODataLIb will throw.
            return new ODataProperty() { Name = elementName, Value = new ODataMultiValue { Items = valueCollection, TypeName = "MultiValue(" + ODataGeneratorUtil.GetFormatedStableName(this.collectionDataContract.StableName) + ")" } };
        }
    }
}
