// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class OperationCountResponseHandler : HttpOperationHandler<HttpResponseMessage, HttpResponseMessage>
    {
        private Counter counter;

        public OperationCountResponseHandler(string outputParameterName, Counter counter)
            : base(outputParameterName)
        {
            this.counter = counter;
        }

        protected override HttpResponseMessage OnHandle(HttpResponseMessage input)
        {
            this.counter.IncreaseCount();
            return input;
        }
    }
}
