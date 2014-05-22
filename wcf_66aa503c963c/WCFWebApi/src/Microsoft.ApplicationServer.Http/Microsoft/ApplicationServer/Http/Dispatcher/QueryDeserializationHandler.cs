// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Query;
    using Microsoft.Server.Common;
    using SR = Microsoft.ApplicationServer.Http.SR;

    // TODO: 233859 - Provide an easy way to customize the query deserialization / composition

    /// <summary>
    /// A <see cref="HttpOperationHandler"/> that deserialize the URI query for a given
    /// request and inserts it as a property on the <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class QueryDeserializationHandler : HttpOperationHandler<HttpRequestMessage, object>
    {
        /// <summary>
        /// The string key for the Property Name on the message.
        /// </summary>
        public const string QueryPropertyName = "queryToCompose";

        private QueryResolver queryResolver;
        private Type queryElementType;

        /// <summary>
        /// Initialize a new instance of <see cref="QueryDeserializationHandler"/> with the given
        /// <see cref="IQueryable"/> return type for the service operation.
        /// </summary>
        /// <param name="returnType">The return type of the service operation</param>
        public QueryDeserializationHandler(Type returnType)
            : this(returnType, queryResolver: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="QueryDeserializationHandler"/> with the given
        /// <see cref="IQueryable"/> return type for the service operation.
        /// </summary>
        /// <param name="returnType">The return type of the service operation</param>
        /// <param name="queryResolver">A <see cref="QueryResolver"/> that participates in the deserialization process.</param>
        internal QueryDeserializationHandler(Type returnType, QueryResolver queryResolver)
            : base("emptyDummy")
        {
            if (returnType == null)
            {
                throw Fx.Exception.ArgumentNull("returnType");
            }

            returnType = HttpTypeHelper.GetHttpResponseOrContentInnerTypeOrNull(returnType) ?? returnType;
            if (!QueryTypeHelper.IsEnumerableInterfaceGenericTypeOrImplementation(returnType))
            {
                throw Fx.Exception.Argument("returnType", SR.RequiresQueryableType);
            }

            this.queryElementType = QueryTypeHelper.GetEnumerableInterfaceInnerTypeOrNull(returnType);
            this.queryResolver = queryResolver;
        }

        /// <summary>
        /// Processes the URI query string from the request and deserializes it as a
        /// <see cref="IQueryable"/> ready for composition, added to the <see cref="HttpRequestMessage"/> as a Property.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> associated with the current request.</param>
        /// <returns>Return value is ignored.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "Better Intellisense experience for developers.")]
        protected override object OnHandle(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw Fx.Exception.ArgumentNull("request");
            }

            Uri requestUri = request.RequestUri;
            if (requestUri != null && !string.IsNullOrWhiteSpace(requestUri.Query))
            {
                try
                {
                    ServiceQuery serviceQuery = ODataQueryDeserializer.GetServiceQuery(requestUri);
                    if (serviceQuery.QueryParts.Count() > 0)
                    {
                        IQueryable baseQuery = Array.CreateInstance(this.queryElementType, 0).AsQueryable();
                        IQueryable deserializedQuery = ODataQueryDeserializer.Deserialize(baseQuery, serviceQuery.QueryParts, this.queryResolver);
                        if (!request.Properties.ContainsKey(QueryDeserializationHandler.QueryPropertyName))
                        {
                            request.Properties.Add(QueryDeserializationHandler.QueryPropertyName, deserializedQuery);
                        }
                        else
                        {
                            request.Properties[QueryDeserializationHandler.QueryPropertyName] = deserializedQuery;
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    throw new HttpRequestException(SR.UriQueryStringInvalid, e);
                }
            }

            return request;
        }
    }
}
