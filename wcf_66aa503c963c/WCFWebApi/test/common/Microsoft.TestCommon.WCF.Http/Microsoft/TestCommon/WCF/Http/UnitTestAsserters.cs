// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class UnitTestAsserters : Microsoft.TestCommon.WCF.UnitTestAsserters
    {
        public HttpAssert Http { get { return HttpAssert.Singleton; } }

        public MediaTypeAssert MediaType { get { return MediaTypeAssert.Singleton; } }

        public ObjectContentAssert ObjectContent { get { return ObjectContentAssert.Singleton; } }

        public HttpOperationAssert HttpOperation { get { return HttpOperationAssert.Singleton; } }

        public HttpParameterAssert HttpParameter { get { return HttpParameterAssert.Singleton; } }

        public ServiceHostAssert ServiceHost { get { return ServiceHostAssert.Singleton; } }

        public HttpServiceHostAssert HttpServiceHost { get { return HttpServiceHostAssert.Singleton; } }

        public WebHttpServiceHostAssert WebHttpServiceHost { get { return WebHttpServiceHostAssert.Singleton; } }
    }
}
