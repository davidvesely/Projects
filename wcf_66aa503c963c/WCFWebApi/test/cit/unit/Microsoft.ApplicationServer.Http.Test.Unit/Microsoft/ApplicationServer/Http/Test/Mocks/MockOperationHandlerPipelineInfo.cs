// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    internal class MockOperationHandlerPipelineInfo : OperationHandlerPipelineInfo
    {
        internal MockOperationHandlerPipelineInfo(
                            IEnumerable<HttpOperationHandler> requestHandlers,
                            IEnumerable<HttpOperationHandler> responseHandlers,
                            HttpOperationDescription operation)
            : base(requestHandlers, responseHandlers, operation)
        {
        }
    }
}
