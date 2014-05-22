// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.OData;

    /// <summary>
    /// Class for serializing primitive types
    /// </summary>
    internal class ODataPrimitiveSerializer : IODataSerializer
    {
        private static readonly ODataPrimitiveSerializer[] primitiveSerializers = new ODataPrimitiveSerializer[] 
            {
                new GenericODataPrimitiveSerializer<string>(),
                new GenericODataPrimitiveSerializer<bool>(),
                new GenericODataPrimitiveSerializer<byte>(),
                new GenericODataPrimitiveSerializer<DateTime>(),
                new DateTimeOffsetODataPrimitiveSerializer(),
                new GenericODataPrimitiveSerializer<decimal>(),
                new GenericODataPrimitiveSerializer<double>(),
                new GenericODataPrimitiveSerializer<Guid>(),
                new GenericODataPrimitiveSerializer<short>(),
                new GenericODataPrimitiveSerializer<int>(),
                new GenericODataPrimitiveSerializer<long>(),
                new GenericODataPrimitiveSerializer<sbyte>(),
                new GenericODataPrimitiveSerializer<float>(),
                new GenericODataPrimitiveSerializer<byte[]>(),
                new CharODataPrimitiveSerializer(),
                new UShortODataPrimitiveSerializer(),
                new UIntODataPrimitiveSerializer(),
                new ULongODataPrimitiveSerializer(),
                new ToStringODataPrimitiveSerializer<Uri>(),
                new TimeSpanODataPrimitiveSerializer(),
                new ToStringODataPrimitiveSerializer<XmlQualifiedName>(),
                new ObjectODataPrimitiveSerializer(),
            };

        private static readonly HashSet<Type> primitiveTypes = new HashSet<Type>(primitiveSerializers.Select(s => s.UnderlyingType));
 
        private ODataPrimitiveSerializer(Type type)
        {
            this.UnderlyingType = type;
        }

        public static IEnumerable<ODataPrimitiveSerializer> PrimitiveSerializers
        {
            get
            {
                return primitiveSerializers;
            }
        }

        public Type UnderlyingType { get; private set; }

        public ODataPayloadKind ODataPayloadKind
        {
            get
            {
                return ODataPayloadKind.Property;
            }
        }

        public SerializerType SerializerType
        {
            get 
            {
                return SerializerType.Primitive;
            }
        }

        public static bool IsODataPrimitiveType(Type type)
        {
            return primitiveTypes.Contains(type);
        }

        public void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerWriteContext writeContext)
        {
            messageWriter.WritePropertyAsync(
                this.CreateProperty(graph, writeContext.ResponseContext.ServiceOperationName, writeContext)).Wait();  
        }

        public virtual ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
        {
            return new ODataProperty() { Value = graph, Name = elementName };
        }

        public void WriteObjectInline(object graph, ODataWriter writer, ODataSerializerWriteContext writeContext)
        {
            throw new NotSupportedException(SR.WriteObjectInlineNotSupported(this.GetType().Name));
        }

        private class GenericODataPrimitiveSerializer<TPrimitive> : ODataPrimitiveSerializer
        {
            public GenericODataPrimitiveSerializer()
                : base(typeof(TPrimitive))
            {
            }
        }

        private class CharODataPrimitiveSerializer : GenericODataPrimitiveSerializer<char>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((char)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class UShortODataPrimitiveSerializer : GenericODataPrimitiveSerializer<ushort>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((ushort)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class UIntODataPrimitiveSerializer : GenericODataPrimitiveSerializer<uint>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((uint)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class ULongODataPrimitiveSerializer : GenericODataPrimitiveSerializer<ulong>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((ulong)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class ObjectODataPrimitiveSerializer : GenericODataPrimitiveSerializer<object>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = string.Empty;
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class TimeSpanODataPrimitiveSerializer : GenericODataPrimitiveSerializer<TimeSpan>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((TimeSpan)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class DateTimeOffsetODataPrimitiveSerializer : GenericODataPrimitiveSerializer<DateTimeOffset>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = XmlConvert.ToString((DateTimeOffset)graph);
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }

        private class ToStringODataPrimitiveSerializer<TPrimitive> : GenericODataPrimitiveSerializer<TPrimitive>
        {
            public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerWriteContext writeContext)
            {
                if (graph != null)
                {
                    graph = graph.ToString();
                }

                return base.CreateProperty(graph, elementName, writeContext);
            }
        }
    }
}
