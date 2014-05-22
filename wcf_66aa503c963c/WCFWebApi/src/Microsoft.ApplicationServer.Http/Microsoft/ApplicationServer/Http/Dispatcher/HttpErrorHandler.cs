// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Server.Common;

    /// <summary>
    /// Abstract base class to provide an <see cref="IErrorHandler"/> for the
    /// <see cref="HttpBinding">HttpBinding</see>
    /// </summary>
    public abstract class HttpErrorHandler
    {
        /// <summary>
        /// Enables the creation of a custom response describing the specified <paramref name="error"/>.
        /// </summary>
        /// <param name="error">The error for which a response is required.</param>
        /// <param name="message">The <see cref="HttpResponseMessage"/> to return.</param>
        /// <returns>A value indicating whether the message is ready to be returned.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference",
            Justification = "The message needs to be passed by ref so that it can be modified/replaced within 'TryProvideResponse'.")]
        public bool TryProvideResponse(Exception error, ref HttpResponseMessage message)
        {
            if (error == null)
            {
                throw Fx.Exception.ArgumentNull("error");
            }

            bool responseProvided = this.OnTryProvideResponse(error, ref message);
            if (responseProvided && message == null)
            {
                string errorMessage = Http.SR.HttpErrorMessageNullResponse(this.GetType().Name, true, "TryProvideResponse", typeof(HttpResponseMessage).Name);
                throw Fx.Exception.AsError(new InvalidOperationException(errorMessage));
            }

            return responseProvided;
        }

        /// <summary>
        /// Called from <see cref="TryProvideResponse(Exception, ref HttpResponseMessage)"/>.
        /// Derived classes must implement this.
        /// </summary>
        /// <param name="exception">The error for which a response is required.</param>
        /// <param name="message">The <see cref="HttpResponseMessage"/> to return.</param>
        /// <returns>A value indicating whether the message is ready to be returned.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference",
            Justification = "The message needs to be passed by ref so that it can be modified/replaced within 'TryProvideResponse'.")]
        protected abstract bool OnTryProvideResponse(Exception exception, ref HttpResponseMessage message);
    }
}
