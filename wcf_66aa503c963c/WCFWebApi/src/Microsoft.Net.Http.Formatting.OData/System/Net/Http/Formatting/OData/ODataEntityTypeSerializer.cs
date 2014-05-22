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
    /// Class for serializing entity types
    /// </summary>
    internal class ODataEntityTypeSerializer : ODataComplexTypeSerializer
    {
        private readonly DataMember[] memberKeys;
        
        private object writeEntryDelegateLock = new object();        
        private Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool> writeEntryDelegate;

        /// <summary>
        /// Initializes an <see cref="ODataEntityTypeSerializer"/>.
        /// </summary>
        /// <param name="classDataContract"><see cref="ClassDataContract"/> associated with the type.</param>
        /// <param name="memberKeys">Keys associated with the entity type.</param>
        public ODataEntityTypeSerializer(ClassDataContract classDataContract, DataMember[] memberKeys)
            : base(classDataContract)
        {
            this.memberKeys = memberKeys;
        }

        /// <summary>
        /// Gets the delegate for writing entry, this is il generated 
        /// </summary>
        public Action<object, IEnumerable<ODataProperty>, ODataWriter, ODataSerializerWriteContext, ClassDataContract, bool> WriteEntryDelegate
        {
            get
            {
                if (this.writeEntryDelegate == null)
                {
                    lock (this.writeEntryDelegateLock)
                    {
                        if (this.writeEntryDelegate == null)
                        {                            
                            this.writeEntryDelegate = ODataWriterGenerator.GenerateWriteEntryDelegate(this.ClassDataContract, this.memberKeys);
                        }
                    }
                }

                return this.writeEntryDelegate;
            }
        }

        /// <summary>
        /// Gets the <see cref="ODataPayloadKind"/> associated with the type.
        /// </summary>
        public override ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Entry;
            }
        }

        /// <summary>
        /// Gets the serializer for the type.
        /// </summary>
        public override SerializerType SerializerType
        {
            get
            {
                return SerializerType.Entity;
            }
        }

        /// <summary>
        /// Implements logic for writing entity type.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <param name="messageWriter"><see cref="ODataMessageWriter"/> used for writing the property.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public override void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
        {
            Task<ODataWriter> writerTask = messageWriter.CreateODataEntryWriterAsync();
            writerTask.Wait();

            ODataWriter writer = writerTask.Result;
            this.WriteObjectInline(graph, writer, writeContext);
            writer.FlushAsync().Wait();
        }

        /// <summary>
        /// This method is supported for feeds and entry types and is called by WriteObject.
        /// </summary>
        /// <param name="graph">Object to be serialized.</param>
        /// <param name="writer"><see cref="ODataWriter"/> used for writing out the type.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public override void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext)
        {
            if (graph != null)
            {
                bool inlineLinks = writeContext.IncrementCurrentReferenceDepth();
                IEnumerable<ODataProperty> propertyBag = this.CreatePropertyBagDelegate(graph, writeContext, this.ClassDataContract, false /*includeAnnotatedTypes*/);
                this.WriteEntryDelegate(graph, propertyBag, writer, writeContext, this.ClassDataContract, inlineLinks);
                writeContext.DecrementCurrentReferenceDepth();
            }
            else
            {
                throw new SerializationException(SR.CannotSerializerNull(ODataConstants.Entry));
            }
        }      
    }
}
