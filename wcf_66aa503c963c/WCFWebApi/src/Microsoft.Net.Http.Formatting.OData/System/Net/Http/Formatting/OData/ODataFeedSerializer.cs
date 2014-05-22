// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.Data.OData;
    using Microsoft.Runtime.Serialization;

    /// <summary>
    /// Class for serializing feed types
    /// </summary>
    internal class ODataFeedSerializer : ODataCollectionSerializer
    {
        private object writeFeedDelegateLock = new object();
        private Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract> writeFeedDelegate;

        /// <summary>
        /// Initializes an <see cref="ODataFeedSerializer"/>.
        /// </summary>
        /// <param name="collectionDataContract"><see cref="CollectionDataContract"/> associated with the type.</param>
        public ODataFeedSerializer(CollectionDataContract collectionDataContract)
            : base(collectionDataContract)
        {
        }

        /// <summary>
        /// <see cref="ODataPayloadKind"/> associated with the type.
        /// </summary>
        public override ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Feed;
            }
        }

        /// <summary>
        /// Gets the serializer for the type.
        /// </summary>
        public override SerializerType SerializerType
        {
            get
            {
                return SerializerType.Feed;
            }
        }

        /// <summary>
        /// Gets the delegate for writing feed, this is il generated 
        /// </summary>
        public Action<object, ODataWriter, ODataSerializerWriteContext, CollectionDataContract> WriteFeedDelegate
        {
            get
            {
                if (this.writeFeedDelegate == null)
                {
                    lock (this.writeFeedDelegateLock)
                    {
                        if (this.writeFeedDelegate == null)
                        {
                            this.writeFeedDelegate = ODataWriterGenerator.GenerateWriteFeedDelegate(this.CollectionDataContract);
                        }
                    }
                }

                return this.writeFeedDelegate;
            }
        }

        /// <summary>
        /// Implements logic for writing feed type.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <param name="messageWriter"><see cref="ODataMessageWriter"/> used for writing the property.</param>
        /// <param name="writeContext"><see cref="ODataSerializerWriteContext"/> used while serializing the type.</param>
        public override void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
        {
            Task<ODataWriter> writerTask = messageWriter.CreateODataFeedWriterAsync();
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
                this.WriteFeedDelegate(graph, writer, writeContext, this.CollectionDataContract);             
            }
            else
            {
                throw new SerializationException(SR.CannotSerializerNull(ODataConstants.Feed));
            }
        }
    }
}
