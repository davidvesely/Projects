// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Net.Http;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Channels;

    internal class CustomErrorHandler : IErrorHandler
    {
        protected bool OnHandleError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return error is HttpResponseMessageException;
        }

        protected HttpResponseMessage OnProvideResponse(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            HttpResponseMessageException httpError = error as HttpResponseMessageException;
            return httpError != null ? httpError.Response : new HttpResponseMessage();
        }

        bool IErrorHandler.HandleError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return this.OnHandleError(error);
        }

        void IErrorHandler.ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            HttpResponseMessage responseMessage = this.OnProvideResponse(error);
            fault = responseMessage.ToMessage();
        }
    }
}
