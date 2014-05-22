// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class SampleItemRequestValidationHandler : HttpOperationHandler<SampleItem, SampleItem>
    {
        public SampleItemRequestValidationHandler(string outputParameterName) : base(outputParameterName)
        {
        }

        protected override SampleItem OnHandle(SampleItem input)
        {
            if (input.Id < 0)
            {
                throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("the Id must be positive")
                    });
            }

            return input;
        }
    }
}
