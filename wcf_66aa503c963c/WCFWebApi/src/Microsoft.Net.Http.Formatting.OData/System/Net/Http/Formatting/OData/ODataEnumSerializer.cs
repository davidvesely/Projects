// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// Class for serializing enum types
    /// </summary>
    internal class ODataEnumSerializer : IODataSerializer
    {
        private EnumDataContract enumDataContract;

        /// <summary>
        /// Initializes an <see cref="ODataEnumSerializer"/>.
        /// </summary>
        /// <param name="enumDataContract"><see cref="EnumDataContract"/> associated with the enumeration type.</param>
        public ODataEnumSerializer(EnumDataContract enumDataContract)
        {
            Contract.Assert(enumDataContract != null, "enumDataContract should be a non-null value");
            this.enumDataContract = enumDataContract;
        }

        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        public Type UnderlyingType
        {
            get 
            {
                return this.enumDataContract.UnderlyingType;
            }
        }

        /// <summary>
        /// Gets the <see cref="ODataPayloadKind"/> associated with the type.
        /// </summary>
        public ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Property;
            }
        }

        /// <summary>
        /// Gets the serializer for the type.
        /// </summary>
        public SerializerType SerializerType
        {
            get 
            {
                return SerializerType.ComplexType;
            }
        }

        /// <summary>
        /// Implements logic for writing enum type.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <param name="messageWriter"><see cref="ODataMessageWriter"/> used for writing the property.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
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
        public ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
        {
            string value = null;
            
            if (graph != null)
            {
                value = Enum.GetName(this.enumDataContract.UnderlyingType, graph);
            }

            ODataProperty property = new ODataProperty()
            {
                Name = elementName,
                Value = value
            };

            return property;
        }

        /// <summary>
        /// This method is not supported for the enum type.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="writer"><see cref="ODataWriter"/> used for writing out the type.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext)
        {
            throw new NotSupportedException(SR.WriteObjectInlineNotSupported(this.GetType().Name));
        }        
    }
}
