using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Test.Common.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Test.Common;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http.Test
{
    /// <summary>
    /// Summary description for HttpAuthTest
    /// </summary>
    [TestClass]
    public class HttpAuthTest
    {
        #region Setup

        private HttpServerData httpServerData;
        private TestContext testContextInstance;
        private string serverAddress;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            httpServerData = HttpClientTestUtils.CreateHttpServer(typeof(HttpAuthTest).FullName,
                TestContext.TestName, false);

            serverAddress = httpServerData.BaseAddress;
        }

        #endregion Setup

        [TestMethod]
        public void Post_Anonymous_Success()
        {
            TestVariations(AuthenticationSchemes.Anonymous);
        }

        [TestMethod]
        public void Post_Basic_Success()
        {
            TestVariations(AuthenticationSchemes.Basic);
        }

        [TestMethod]
        public void Post_Digest_Success()
        {
            TestVariations(AuthenticationSchemes.Digest);
        }

        [TestMethod]
        public void Post_Ntlm_Success()
        {
            TestVariations(AuthenticationSchemes.Ntlm);
        }

        [TestMethod]
        public void Post_Negotiate_Success()
        {
            TestVariations(AuthenticationSchemes.Negotiate);
        }

        [TestMethod]
        public void Post_IntegratedWindowsAuthentication_Success()
        {
            TestVariations(AuthenticationSchemes.IntegratedWindowsAuthentication);
        }

        #region Helpers

        private void TestVariations(AuthenticationSchemes authSchemes)
        {
            try
            {
                httpServerData.Server.AuthenticationSchemes = authSchemes;
                HttpTestUtils.StartHttpServer(httpServerData);
                HttpTestUtils.StartListening(httpServerData, 20, c =>
                {
                    try
                    {
                        Log.Info("Server got request");

                        HttpListenerRequest request = c.Context.Request;
                        byte[] bytes;
                        int read;
                        if (request.ContentLength64 == -1) // Chunked
                        {
                            bytes = new byte[1000];
                            read = request.InputStream.Read(bytes, 0, bytes.Length);
                            Log.Info("Read chunked bytes: " + read);
                        }
                        else
                        {
                            bytes = new byte[(int)request.ContentLength64];
                            read = request.InputStream.Read(bytes, 0, bytes.Length);
                            Assert.AreEqual(request.ContentLength64, read);
                        }
                        Log.Info("Server read upload");

                        HttpListenerResponse response = c.Context.Response;
                        response.StatusCode = 200;
                        response.ContentLength64 = read;
                        response.OutputStream.Write(bytes, 0, read);
                        response.Close();
                        //request.Close();
                        Log.Info("Server sent response");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                });

                CredentialCache creds = new CredentialCache();
                if (authSchemes == AuthenticationSchemes.Basic)
                {
                    creds.Add(new Uri(serverAddress), GetSchemeName(authSchemes),
                        new NetworkCredential("user", "password"));
                }
                else if (authSchemes != AuthenticationSchemes.Anonymous)
                {
                    creds.Add(new Uri(serverAddress), GetSchemeName(authSchemes),
                        CredentialCache.DefaultNetworkCredentials);
                }

                WebRequestHandler handler = new WebRequestHandler();
                //handler.ContinueTimeout = new TimeSpan(0, 2, 0); // Longer than the request timeout
                handler.Credentials = creds;
                handler.UseProxy = false;

                HttpClient client = new HttpClient(handler);
                client.BaseAddress = new Uri(serverAddress);
                client.Timeout = new TimeSpan(0, 1, 0);

                // Buffered
                ExecuteRequest(client, true, true, true);
                ExecuteRequest(client, false, true, true);
                ExecuteRequest(client, true, false, true);
                ExecuteRequest(client, false, false, true);

                // Unbuffered
                ExecuteRequest(client, true, true, false);
                ExecuteRequest(client, false, true, false);
                ExecuteRequest(client, true, false, false);
                ExecuteRequest(client, false, false, false);
            }
            finally
            {
                if (httpServerData.Server.IsListening)
                {
                    HttpTestUtils.CloseHttpServer(httpServerData);
                }
            }
        }

        private void ExecuteRequest(HttpClient client, bool sync, bool chunked, bool buffer)
        {
            Log.Info("Test Variation; Sync:{0}, Chunked:{1}, Buffer:{2}", sync, chunked, buffer);

            ServicePoint servicePoint = ServicePointManager.FindServicePoint(client.BaseAddress);
            //if (!servicePoint.Understands100Continue)
            //{
            //    // Broken by another test
            //    // Assert.Inconclusive("ServicePoint does not understand 100Continue");
            //    servicePoint.Understands100Continue = true;
            //}

            string input = "Hello World! How are you today?";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StreamContent(new NonReserializableStream(input));

            if (buffer)
            {
                request.Content.LoadIntoBufferAsync().Wait();
            }

            if (chunked)
            {
                request.Headers.TransferEncodingChunked = true;
            }
            else
            {
                request.Content.Headers.ContentLength = input.Length;
            }

            HttpResponseMessage response;
            if (sync)
            {
                response = client.SendAsync(request).Result;
            }
            else
            {
                Task<HttpResponseMessage> task = client.SendAsync(request);
                task.Wait();
                response = task.Result;
            }
            string output = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(input, output);
        }

        private string GetSchemeName(AuthenticationSchemes authSchemes)
        {
            switch (authSchemes)
            {
                case AuthenticationSchemes.Basic:
                    return "Basic";
                case AuthenticationSchemes.Digest:
                    return "Digest";
                case AuthenticationSchemes.Ntlm:
                    return "NTLM";
                case AuthenticationSchemes.Negotiate:
                case AuthenticationSchemes.IntegratedWindowsAuthentication:
                    return "Negotiate";
            }
            return string.Empty;
        }

        internal class NonReserializableStream : MemoryStream
        {
            public NonReserializableStream(string content)
                : base(Encoding.ASCII.GetBytes(content))
            { }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }
        }

        #endregion Helpers
    }
}
