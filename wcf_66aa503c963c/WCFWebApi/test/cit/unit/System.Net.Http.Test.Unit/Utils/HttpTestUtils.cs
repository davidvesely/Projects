using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Net.Test.Common.Logging;

namespace System.Net.Test.Common
{
    public delegate void HttpListenerCallback(HttpServerContextData contextData);

    public static class HttpTestUtils
    {
        // returns a path unique to the current test. This (relative) path can be used e.g. when
        // using a HttpListener instance for listening on a specific path.
        public static string GetTestPath(string fullClassName, string testName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fullClassName), "fullClassName is null");
            Debug.Assert(!string.IsNullOrEmpty(testName), "testName is null");

            return VersioningHelper.MakeVersionSafeName("Http", ResourceScope.Machine,
                ResourceScope.AppDomain) + "/" + fullClassName + "/" + testName + "/";
        }

        public static HttpServerData CreateHttpServer(string fullClassName, string testName, int port, bool useTls)
        {
            StringBuilder rootUri = new StringBuilder();
            StringBuilder baseAddress = new StringBuilder();

            if (useTls)
            {
                rootUri.Append("https");
                baseAddress.Append("https");
            }
            else
            {
                rootUri.Append("http");
                baseAddress.Append("http");
            }

            rootUri.Append("://*:").Append(port).Append("/");
            baseAddress.Append("://localhost:").Append(port).Append("/");

            HttpListener server = new HttpListener();
            string path = HttpTestUtils.GetTestPath(fullClassName, testName);
            try
            {
                baseAddress.Append(path);
                rootUri.Append(path);
                server.Prefixes.Add(rootUri.ToString());
            }
            catch (HttpListenerException e)
            {
                LogListenerFailureAndThrow(e);
            }

            return new HttpServerData(baseAddress.ToString(), path, server);
        }

        public static void StartHttpServer(HttpServerData data)
        {
            Debug.Assert(data != null, "'data' must be assigned");
            Debug.Assert(data.Server != null, "'data.Server' must be assigned");

            try
            {
                data.Server.Start();
            }
            catch (HttpListenerException e)
            {
                LogListenerFailureAndThrow(e);
            }
        }

        public static void CloseHttpServer(HttpServerData data)
        {
            Debug.Assert(data != null, "'data' must be assigned");
            Debug.Assert(data.Server != null, "'data.Server' must be assigned");

            // This method exists just for convenience + to provide a consistent way of using HttpListener in
            // tests: start, listen and close using HttpTestUtils.* methods.
            data.Server.Close();
        }

        public static void StartListening(HttpServerData data, HttpListenerCallback callback)
        {
            StartListening(data, 1, callback);
        }

        public static void StartListening(HttpServerData data, int noOfListeners, HttpListenerCallback callback)
        {
            Debug.Assert(data != null, "'data' must be assigned");
            Debug.Assert(data.Server != null, "'data.Server' must be assigned");
            Debug.Assert(noOfListeners > 0, "At least one listener must be created.");
            Debug.Assert(callback != null, "Callback can't be null.");
            try
            {
                for (int i = 0; i < noOfListeners; i++)
                {
                    data.Server.BeginGetContext(HandleRequest, new CallbackState(callback, data));
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                throw new TestException(TestUtils.Format(
                    "StartListening: Error while calling BeginGetContext(): {0}", e.Message));
            }
        }

        private static void HandleRequest(IAsyncResult ar)
        {            
            try
            {
                CallbackState state = ar.AsyncState as CallbackState;
                Debug.Assert(state != null, "'ar.AsyncState' was not of type 'CallbackState'");

                HttpListenerContext context = state.ServerData.Server.EndGetContext(ar);
                state.TestCallback(new HttpServerContextData(context, state.ServerData));
            }
            catch (ObjectDisposedException)
            {
                // This may occur if the test fails, and the listener gets cleaned up. Just ignore the exception
                // and continue with next test.
                Log.Warn("HttpListener: Listener is already disposed");
            }
            catch (Exception e)
            {
                Log.Error("HttpListener: Unexpected exception while handling the request.");
                Log.Exception(e);
            }
        }

        private static string LogListenerFailureAndThrow(HttpListenerException exception)
        {
            Log.Exception(exception);
            Log.Error("ErrorCode: {0}", exception.ErrorCode);
            Log.Info("Verify that there is a URL reservation for all prefixes by executing " +
                "'netsh http show urlacl'.");
            Log.Info("For HTTPS verify if there is a certificate binding by using 'netsh http show sslcert'");

            throw new TestException(TestUtils.Format("Can't start HttpListener. " +
                "Make sure to execute 'SetupProject.bat' located in the project directory, " +
                "before running unit tests. Error message from HttpListener: {0}", exception.Message));
        }

        private sealed class CallbackState
        {
            private HttpListenerCallback testCallback;
            private HttpServerData serverData;

            public CallbackState(HttpListenerCallback testCallback, HttpServerData serverData)
            {
                this.testCallback = testCallback;
                this.serverData = serverData;
            }

            public HttpListenerCallback TestCallback
            {
                get { return testCallback; }
            }

            public HttpServerData ServerData
            {
                get { return serverData; }
            }
        }
    }
}
