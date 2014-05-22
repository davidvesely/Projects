using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net.Test.Common.Logging;
using System.Runtime.Versioning;
using System.Net.Test.Common;

namespace System.Net.Http.Test
{
    internal sealed class HttpClientTestUtils
    {
        // use this port when using HttpListener instances listening on HTTP. 
        public const int HttpListenerPort = 8686;

        // use this port when using HttpListener instances listening on HTTPS (SSL). 
        public const int HttpsListenerPort = 8787;

        public static HttpServerData CreateHttpServer(string fullClassName, string testName, bool useTls)
        {
            if (useTls)
            {
                return HttpTestUtils.CreateHttpServer(fullClassName, testName, HttpsListenerPort, useTls);
            }
            else
            {
                return HttpTestUtils.CreateHttpServer(fullClassName, testName, HttpListenerPort, useTls);
            }
        }
    }
}
