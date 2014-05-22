// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Server.Common;

    /// <summary>
    /// A static class that provides Queryable related types and functionality
    /// to perform checks related on them.
    /// </summary>
    internal static class QueryTypeHelper
    {
        internal static readonly Type EnumerableInterfaceGenericType = typeof(IEnumerable<>);
        internal static readonly Type QueryableInterfaceGenericType = typeof(IQueryable<>);

        internal static bool IsEnumerableInterfaceGenericTypeOrImplementation(Type type)
        {
            return type != null
                && (IsEnumerableInterfaceGenericType(type) || ImplementsEnumerableInterfaceGenericType(type));
        }

        internal static bool IsQueryableInterfaceGenericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            type = HttpTypeHelper.GetHttpResponseOrContentInnerTypeOrNull(type) ?? type;
            return type.IsInterface
                && type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(QueryableInterfaceGenericType);
        }

        internal static Type GetEnumerableInterfaceInnerTypeOrNull(Type type)
        {
            if (type == null)
            {
                return type;
            }

            if (IsEnumerableInterfaceGenericType(type))
            {
                return type.GetGenericArguments()[0];
            }
            else if (ImplementsEnumerableInterfaceGenericType(type))
            {
                return type.GetInterface(EnumerableInterfaceGenericType.FullName).GetGenericArguments()[0];
            }

            return null;
        }

        private static bool IsEnumerableInterfaceGenericType(Type type)
        {
            // This private method is called from others that ensured type is not null beforehand.
            Fx.Assert(type != null, "The 'type' parameter should not be null.");

            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(EnumerableInterfaceGenericType);
        }

        private static bool ImplementsEnumerableInterfaceGenericType(Type type)
        {
            // This private method is called from others that ensured type is not null beforehand.
            Fx.Assert(type != null, "The 'type' parameter should not be null.");

            return type.GetInterface(EnumerableInterfaceGenericType.FullName) != null;
        }
    }
}
