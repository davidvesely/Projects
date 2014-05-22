// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System.Collections.Generic;
    using Microsoft.ApplicationServer.Http.Description;

    public class MockHttpOperationDescription : HttpOperationDescription
    {
        public MockHttpOperationDescription(IEnumerable<HttpParameter> inputParameters) : base()
        {
            foreach (var parameter in inputParameters)
            {
                this.InputParameters.Add(parameter);
            }
        }
    }
}
