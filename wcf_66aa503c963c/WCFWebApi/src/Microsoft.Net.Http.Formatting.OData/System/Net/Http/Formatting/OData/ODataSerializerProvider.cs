// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Abstract class that generates an <see cref="IODataSerializer" /> instance based on the type to be serialized./>
    /// </summary>
#if USE_REFEMIT
    public abstract class ODataSerializerProvider
#else
    internal abstract class ODataSerializerProvider
#endif
    {
        private ConcurrentDictionary<Type, IODataSerializer> serializerCache; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataSerializerProvider"/> class.
        /// </summary>
        protected ODataSerializerProvider()
        {
            this.serializerCache = new ConcurrentDictionary<Type, IODataSerializer>();
        }

        /// <summary>
        /// Gets a serializer based on <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to be serialized</param>
        /// <returns>Instance of <see cref="IODataSerializer"/> that can serialize the specified <paramref name="type"/></returns>
        public IODataSerializer GetODataSerializer(Type type)
        {
            Contract.Assert(type != null, "type must be a non-null value");

            return this.serializerCache.GetOrAdd(type, this.CreateSerializer);
        }

        /// <summary>
        /// Allows one or more members of a type to be specified as the key members for
        /// the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the key members are being set.</param>
        /// <param name="memberNames">Zero or more member names that should be set as the key members for the given <see name="Type"/>.</param>
        public void SetKeyMembers(Type type,  string[] memberNames)
        {
            Contract.Assert(type != null, "The 'type' parameter should never be null.");
            Contract.Assert(memberNames != null, "The 'memberNames' parameter should never be null.");

            IODataSerializer newSerializer = this.CreateSerializer(type, memberNames);
            this.serializerCache.AddOrUpdate(type, newSerializer, (t, s) => newSerializer);
        }

        /// <summary>
        /// Creates a serializer based on the type and optional explicit key member names.
        /// </summary>
        /// <param name="type">The type for which an <see cref="IODataSerializer"/> should be created.</param>
        /// <param name="keyMemberNames">The names of the members of the <see cref="Type"/> that should be considered key members.</param>
        /// <returns>The <see cref="IODataSerializer"/> for the given <see cref="Type"/>.</returns>
        protected abstract IODataSerializer CreateSerializer(Type type, string[] keyMemberNames);

        /// <summary>
        /// Allows derived classes to add serializers directly to the cache.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the serializer was created.</param>
        /// <param name="serializer">The <see cref="IODataSerializer"/> to add to the serializer cache.</param>
        protected void SetODataSerializer(Type type, IODataSerializer serializer)
        {
            Contract.Assert(type != null, "The 'type' parameter should never be null.");
            Contract.Assert(serializer != null, "The 'serializer' parameter should never be null.");
            this.serializerCache.AddOrUpdate(type, serializer, (t, s) => serializer);
        }

        private IODataSerializer CreateSerializer(Type type)
        {
            return this.CreateSerializer(type, null);
        }
    }    
}

