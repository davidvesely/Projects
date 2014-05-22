// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.ServiceModel;
    using Microsoft.Server.Common;

    /// <summary>
    /// An exception that allows for a given <see cref="HttpResponseMessage"/> 
    /// to be returned to the client.
    /// </summary>
    [Serializable]
    public class HttpResponseException : Exception
    {
        private const string ResponsePropertyName = "Response";

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        public HttpResponseException()
            : this(HttpStatusCode.InternalServerError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public HttpResponseException(string message)
            : base(message)
        {
            this.InitializeResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public HttpResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.InitializeResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code to use with the <see cref="HttpResponseMessage"/>.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public HttpResponseException(HttpStatusCode statusCode)
            : this(new HttpResponseMessage(statusCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="response">The response message.</param>
        public HttpResponseException(HttpResponseMessage response)
            : base(Http.SR.HttpResponseExceptionMessage(ResponsePropertyName))
        {
            if (response == null)
            {
                throw Fx.Exception.ArgumentNull("response");
            }

            this.InitializeResponse(response);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="streamingContext">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        protected HttpResponseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            this.InitializeResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// Gets the <see cref="HttpResponseMessage"/> to return to the client.
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:ArgumentNullException">The <paramref name="info"/> parameter is a null reference.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        private void InitializeResponse(HttpResponseMessage response)
        {
            Fx.Assert(response != null, "Response cannot be null!");
            this.Response = response;
            this.Response.RequestMessage = OperationContext.Current.GetHttpRequestMessage();
        }
    }
}
