// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Mocks
{
    using System;
    using System.ServiceModel;
    using Microsoft.ApplicationServer.Http.Activation;

    public class MockHttpServiceHostFactory : HttpServiceHostFactory
    {
        public static MockHttpServiceHostFactory Create()
        {
            return new MockHttpServiceHostFactory();
        }

        internal MockHttpServiceHostFactory()
        {
        }

        new public ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return base.CreateServiceHost(serviceType, baseAddresses);
        }
    }
}