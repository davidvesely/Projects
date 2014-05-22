// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
#if USE_REFEMIT
    public enum SerializerType
#else
    internal enum SerializerType
#endif
    {
        Primitive,
        ComplexType,
        Entity,
        Collection,
        Feed
    } 
}
