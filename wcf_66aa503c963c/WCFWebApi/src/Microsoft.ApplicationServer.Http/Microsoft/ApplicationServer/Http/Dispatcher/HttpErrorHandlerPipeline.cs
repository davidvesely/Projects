// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.Server.Common;

    /// <summary>
    /// This is an implementation of <see cref="IErrorHandler"/> used to execute a collection of <see cref="HttpErrorHandler"/>.
    /// </summary>
    internal class HttpErrorHandlerPipeline : IErrorHandler
    {
        private Collection<HttpErrorHandler> httpErrorHandlers;

        /// <summary>
        /// Gets the collection of <see cref="HttpErrorHandler"/>.
        /// </summary>
        public Collection<HttpErrorHandler> HttpErrorHandlers 
        {
            get
            {
                if (this.httpErrorHandlers == null)
                {
                    this.httpErrorHandlers = new Collection<HttpErrorHandler>();
                }

                return this.httpErrorHandlers;
            }
        }

        /// <summary>
        /// Enables the creation of a custom fault <see cref="Message"/>
        /// that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="Exception"/> object thrown in the course 
        /// of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="System.ServiceModel.Channels.Message"/> object 
        /// that is returned to the client, or service, in the duplex case.</param>
        void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null)
            {
                throw Fx.Exception.ArgumentNull("error");
            }

            HttpResponseMessage responseMessage = null;
            foreach (HttpErrorHandler handler in this.HttpErrorHandlers)
            {
                if (handler.TryProvideResponse(error, ref responseMessage))
                {
                    break;
                }
            }

            if (responseMessage != null)
            {
                fault = responseMessage.ToMessage();
            }
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>
        /// true if  should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single"/>; otherwise, false. The default is false.
        /// </returns>
        /// <remarks>
        /// This always return false which is the default and let other handlers decide whether to abort the session and/or instance context.
        /// </remarks>
        bool IErrorHandler.HandleError(Exception error)
        {
            return false;
        }
    }
}
