// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class AlwaysOkErrorHandler : HttpErrorHandler
    {
        protected override bool OnTryProvideResponse(Exception exception, ref HttpResponseMessage message)
        {
            message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(exception.Message)
            };

            return true;
        }
    }
}
