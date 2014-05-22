// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// Class for serializing complex types
    /// </summary>
    internal class ODataComplexTypeSerializer : IODataSerializer
    {
        private readonly ClassDataContract classDataContract;

        private object createPropertyBagDelegateLock = new object();
        private Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>> createPropertyBagDelegate;
        
        /// <summary>
        /// Initializes an <see cref="ODataComplexTypeSerializer"/>.
        /// </summary>
        /// <param name="classDataContract"><see cref="ClassDataContract"/> associated with the type.</param>
        public ODataComplexTypeSerializer(ClassDataContract classDataContract)
        {
            this.classDataContract = classDataContract;            
        }

        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        public Type UnderlyingType
        {
            get { return this.classDataContract.UnderlyingType; }    
        }

        /// <summary>
        /// Gets the <see cref="ODataPayloadKind"/> associated with the type.
        /// </summary>
        public virtual ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Property;
            }
        }

        /// <summary>
        /// Gets the serializer for the type.
        /// </summary>
        public virtual SerializerType SerializerType
        {
            get { return SerializerType.ComplexType; }
        }

        /// <summary>
        /// Gets the property bag delegate that is il generated
        /// </summary>
        public Func<object, ODataSerializerWriteContext, ClassDataContract, bool, IEnumerable<ODataProperty>> CreatePropertyBagDelegate
        {
            get
            {
                if (this.createPropertyBagDelegate == null)
                {
                    lock (this.createPropertyBagDelegateLock)
                    {
                        if (this.createPropertyBagDelegate == null)
                        {                            
                            this.createPropertyBagDelegate = ODataWriterGenerator.GeneratePropertyBagDelegate(this.classDataContract);
                        }
                    }
                }

                return this.createPropertyBagDelegate;
            }
        }

        /// <summary>
        /// Gets the ClassDataContract for the type being serialized.
        /// </summary>
        protected ClassDataContract ClassDataContract
        {
            get
            {
                return this.classDataContract;
            }
        }

        /// <summary>
        /// Implements logic for writing the complex type.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <param name="messageWriter"><see cref="ODataMessageWriter"/> used for writing the property.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public virtual void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
        {
            ODataProperty property = this.CreateProperty(graph, writeContext.ResponseContext.ServiceOperationName, writeContext);
            messageWriter.WritePropertyAsync(property).Wait();            
        }

        /// <summary>
        /// Creates an <see cref="ODataProperty"/> for the object graph.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="elementName">Name of the element represented by the <paramref name="graph"/> </param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        /// <returns><see cref="ODataProperty"/> for the object graph.</returns>
        public virtual ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
        {
            IEnumerable<ODataProperty> propertyCollection = null;

            if (graph != null)
            {
                if (writeContext.IncrementCurrentReferenceDepth())
                {
                    propertyCollection = this.CreatePropertyBagDelegate(graph, writeContext, this.classDataContract, true);
                }
                                    
                writeContext.DecrementCurrentReferenceDepth();
            }

            return new ODataProperty()
            {
                Name = elementName,
                Value = new ODataComplexValue()
                {
                    Properties = propertyCollection,
                    TypeName = ODataGeneratorUtil.GetFormatedStableName(this.classDataContract.StableName)
                }
            };
        }

        /// <summary>
        /// This method is not supported for the complex type.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="writer"><see cref="ODataWriter"/> used for writing out the type.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public virtual void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext)
        {
            throw new NotSupportedException(SR.WriteObjectInlineNotSupported(this.GetType().Name));
        }
    }
}
