// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.OData;

    /// <summary>
    /// Defines the interface that all the internal serializers need to implement
    /// </summary>
#if USE_REFEMIT
    public interface IODataSerializer
#else
    internal interface IODataSerializer
#endif
    {
        Type UnderlyingType { get; }

        ODataPayloadKind ODataPayloadKind { get; }

        SerializerType SerializerType { get; }        

        void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext);

        void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext);                

        ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext);
    }          
}
