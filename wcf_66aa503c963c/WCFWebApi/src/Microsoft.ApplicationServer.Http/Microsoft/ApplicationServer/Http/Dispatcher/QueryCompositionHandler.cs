// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Reflection;
    using Microsoft.ApplicationServer.Query;
    using Microsoft.Server.Common;
    using SR = Microsoft.ApplicationServer.Http.SR;

    /// <summary>
    /// A <see cref="HttpOperationHandler"/> that composes a <see cref="IQueryable"/> with the result of the service operation.
    /// </summary>
    public class QueryCompositionHandler : HttpOperationHandler<HttpRequestMessage, HttpResponseMessage, HttpResponseMessage>
    {
        private Type returnResponseType;
        private ConstructorInfo returnResponseTypeConstructor;

        /// <summary>
        /// Initialize a new instance of <see cref="QueryCompositionHandler"/> with the
        /// given <paramref name="returnType"/>.
        /// </summary>
        /// <param name="returnType">The return type of the service operation</param>
        public QueryCompositionHandler(Type returnType)
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

            Type queryElementType = QueryTypeHelper.GetEnumerableInterfaceInnerTypeOrNull(returnType);
            this.returnResponseType = HttpTypeHelper.HttpResponseMessageGenericType.MakeGenericType(QueryTypeHelper.EnumerableInterfaceGenericType.MakeGenericType(queryElementType));
            this.returnResponseTypeConstructor = this.returnResponseType.GetConstructor(
                new Type[]
                {
                    QueryTypeHelper.EnumerableInterfaceGenericType.MakeGenericType(queryElementType),
                    typeof(HttpStatusCode),
                    typeof(MediaTypeFormatterCollection)
                });
        }

        /// <summary>
        /// Composes the service query with the result of the service operation and returns a new <see cref="HttpResponseMessage"/>, overriding the previous result.
        /// </summary>
        /// <param name="request">The original <see cref="HttpRequestMessage"/> for this request.</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> produced by the <see cref="ResponseContentHandler"/> from the service operation result.</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/> resulting from the composition of the <paramref name="response"/> and
        /// the serviceQuery Property set on the <see cref="HttpRequestMessage"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "Better Intellisense experience for developers.")]
        protected override HttpResponseMessage OnHandle(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request == null)
            {
                throw Fx.Exception.ArgumentNull("request");
            }

            IQueryable query = null;
            if (request.Properties.ContainsKey(QueryDeserializationHandler.QueryPropertyName))
            {
                query = request.Properties[QueryDeserializationHandler.QueryPropertyName] as IQueryable;
            }
            else
            {
                // No Query to compose, return unmodified HttpResponseMessage
                return response;
            }

            IQueryable source = null;
            if (response != null && QueryTypeHelper.IsEnumerableInterfaceGenericTypeOrImplementation(HttpTypeHelper.GetHttpResponseInnerTypeOrNull(response.GetType())))
            {
                source = ((IEnumerable)((ObjectContent)response.Content).ReadAsAsync().Result).AsQueryable();
            }
            else
            {
                // No Query to compose, return unmodified HttpResponseMessage
                return response;
            }

            try
            {
                IEnumerable composedQuery = QueryComposer.Compose(source, query);

                HttpResponseMessage newResponse = (HttpResponseMessage)this.returnResponseTypeConstructor.Invoke(
                    new object[]
                    {
                        composedQuery,
                        response.StatusCode,
                        ((ObjectContent)response.Content).Formatters
                    });
                newResponse.Headers.Clear();
                response.Headers.CopyTo(newResponse.Headers);
                response.Content.Headers.CopyTo(newResponse.Content.Headers);
                newResponse.ReasonPhrase = response.ReasonPhrase;
                newResponse.RequestMessage = response.RequestMessage;
                newResponse.Version = response.Version;
                ((ObjectContent)newResponse.Content).Headers.Clear();
                ((ObjectContent)response.Content).Headers.CopyTo(((ObjectContent)newResponse.Content).Headers);

                return newResponse;
            }
            catch (ParseException)
            {
                throw new HttpResponseException(SR.QueryCompositionFailed);
            }
        }
    }
}
