using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Net.Test.Common
{
    public sealed class HttpServerContextData
    {
        private HttpListenerContext context;
        private HttpServerData serverData;

        public HttpServerContextData(HttpListenerContext context, HttpServerData serverData)
        {
            Debug.Assert(context != null);
            Debug.Assert(serverData != null);

            this.context = context;
            this.serverData = serverData;
        }

        public HttpListenerContext Context
        {
            get { return context; }
        }

        public HttpServerData ServerData
        {
            get { return serverData; }
        }
    }
}
