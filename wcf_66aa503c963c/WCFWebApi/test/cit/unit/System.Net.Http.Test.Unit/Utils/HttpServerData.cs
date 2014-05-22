using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Net.Test.Common
{
    public sealed class HttpServerData
    {
        private string baseAddress;
        private string basePath;
        private HttpListener server;

        public string BaseAddress
        {
            get { return baseAddress; }
        }

        public string BasePath
        {
            get { return basePath; }
        }

        public HttpListener Server
        {
            get { return server; }
        }

        public HttpServerData(string baseAddress, string basePath, HttpListener server)
        {
            Debug.Assert(baseAddress != null);
            Debug.Assert(basePath != null);
            Debug.Assert(server != null);

            this.baseAddress = baseAddress;
            this.basePath = basePath;
            this.server = server;
        }
    }
}
