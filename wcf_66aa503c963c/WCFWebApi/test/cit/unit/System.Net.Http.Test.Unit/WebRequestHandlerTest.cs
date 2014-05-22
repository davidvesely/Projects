using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Test.Common;
using System.Net.Test.Common.Logging;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class WebRequestHandlerTest
    {
        private TestContext testContextInstance;
        private HttpServerData httpServerData;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            httpServerData = HttpClientTestUtils.CreateHttpServer(typeof(WebRequestHandlerTest).FullName,
                TestContext.TestName, false);
            HttpTestUtils.StartHttpServer(httpServerData);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            HttpTestUtils.CloseHttpServer(httpServerData);
        }

        [TestMethod]
        public void Dispose_MultipleCalls_NoException()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.Dispose();
            handler.Dispose();
            handler.Dispose();
        }

        [TestMethod]
        public void Dispose_DisposeHandlerThenAccessMembers_AllThrow()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.Dispose();

            // Property getter don't throw.
            Assert.IsTrue(handler.AllowAutoRedirect, "get_AllowAutoRedirect");
            Assert.IsTrue(handler.AllowPipelining, "get_AllowPipelining");
            Assert.AreEqual(AuthenticationLevel.MutualAuthRequested, handler.AuthenticationLevel,
                "get_AuthenticationLevel");
            Assert.AreEqual(DecompressionMethods.None, handler.AutomaticDecompression,
                "get_AutomaticDecompression");
            Assert.AreEqual(WebRequest.DefaultCachePolicy, handler.CachePolicy, "get_CachePolicy");
            Assert.AreEqual(0, handler.ClientCertificates.Count, "get_ClientCertificates");
            Assert.AreEqual(ClientCertificateOption.Manual, handler.ClientCertificateOptions,
                "get_ClientCertificateOptions");
            Assert.IsNotNull(handler.CookieContainer, "get_CookieContainer");
            Assert.IsNull(handler.Credentials, "get_Credentials");
            Assert.AreEqual(TokenImpersonationLevel.Delegation, handler.ImpersonationLevel, "get_ImpersonationLevel");
            Assert.AreEqual(50, handler.MaxAutomaticRedirections, "get_MaxAutomaticRedirections");
            Assert.AreEqual(HttpWebRequest.DefaultMaximumResponseHeadersLength, handler.MaxResponseHeadersLength,
                "get_MaxResponseHeadersLength");
            Assert.IsFalse(handler.PreAuthenticate, "get_PreAuthenticate");
            Assert.IsNull(handler.Proxy, "get_Proxy");
            Assert.IsFalse(handler.UnsafeAuthenticatedConnectionSharing, "get_UnsafeAuthenticatedConnectionSharing");
            Assert.IsTrue(handler.UseCookies, "get_UseCookies");
            Assert.IsFalse(handler.UseDefaultCredentials, "get_UseDefaultCredentials");
            Assert.IsTrue(handler.UseProxy, "get_UseProxy");

            // Property setter will throw.
            ExceptionAssert.ThrowsObjectDisposed(() => handler.AllowAutoRedirect = false, "set_AllowAutoRedirect");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.AllowPipelining = false, "set_AllowPipelining");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested,
                "set_AuthenticationLevel");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.AutomaticDecompression = DecompressionMethods.None,
                "set_AutomaticDecompression");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.CachePolicy = null, "set_CachePolicy");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.CookieContainer = new CookieContainer(),
                "set_CookieContainer");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.Credentials = null, "set_Credentials");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.ImpersonationLevel = TokenImpersonationLevel.Anonymous,
                "set_ImpersonationLevel");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.MaxAutomaticRedirections = 5,
                "set_MaxAutomaticRedirections");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.MaxResponseHeadersLength = 10,
                "set_MaxResponseHeadersLength");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.PreAuthenticate = true, "set_PreAuthenticate");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.Proxy = null, "set_Proxy");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.ReadWriteTimeout = 15, "set_ReadWriteTimeout");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.UnsafeAuthenticatedConnectionSharing = true,
                "set_UnsafeAuthenticatedConnectionSharing");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.UseCookies = false, "set_UseCookies");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.UseDefaultCredentials = false,
                "set_UseDefaultCredentials");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.UseProxy = false, "set_UseProxy");
            ExceptionAssert.ThrowsObjectDisposed(() => handler.ClientCertificateOptions
                = ClientCertificateOption.Manual, "set_ClientCertificateOptions");

            // Methods
            ExceptionAssert.ThrowsObjectDisposed(() =>
            {
                Task t = handler.SendAsync(new HttpRequestMessage(), CancellationToken.None);
                t.Wait();
            }, "SendAsync()");
        }

        [TestMethod]
        public void Properties_SendRequestsAndThenSetProperties_AllThrow()
        {
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            // "Use" the handler. I.e. send a request. After sending the first request, no property should be settable.
            handler.SendAsync(new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress), CancellationToken.None).Wait();

            ExceptionAssert.ThrowsInvalidOperation(() => handler.AllowAutoRedirect = false, "set_AllowAutoRedirect");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.AllowPipelining = false, "set_AllowPipelining");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested,
                "set_AuthenticationLevel");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.AutomaticDecompression = DecompressionMethods.None,
                "set_AutomaticDecompression");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.CachePolicy = null, "set_CachePolicy");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.CookieContainer = new CookieContainer(),
                "set_CookieContainer");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.Credentials = null, "set_Credentials");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.ImpersonationLevel = TokenImpersonationLevel.Anonymous,
                "set_ImpersonationLevel");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.MaxAutomaticRedirections = 5,
                "set_MaxAutomaticRedirections");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.MaxResponseHeadersLength = 10,
                "set_MaxResponseHeadersLength");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.PreAuthenticate = true, "set_PreAuthenticate");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.Proxy = null, "set_Proxy");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.ReadWriteTimeout = 15, "set_ReadWriteTimeout");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.UnsafeAuthenticatedConnectionSharing = true,
                "set_UnsafeAuthenticatedConnectionSharing");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.UseCookies = false, "set_UseCookies");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.UseDefaultCredentials = false,
                "set_UseDefaultCredentials");
            ExceptionAssert.ThrowsInvalidOperation(() => handler.UseProxy = false, "set_UseProxy");
        }

        [TestMethod]
        public void Properties_SetAndGetPropertyValues_MatchExpectation()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.AllowAutoRedirect = false;
            Assert.IsFalse(handler.AllowAutoRedirect, "AllowAutoRedirect");
            handler.AllowPipelining = false;
            Assert.IsFalse(handler.AllowPipelining, "AllowPipelining");
            handler.AuthenticationLevel = AuthenticationLevel.None;
            Assert.AreEqual(AuthenticationLevel.None, handler.AuthenticationLevel, "AuthenticationLevel");
            handler.AutomaticDecompression = DecompressionMethods.Deflate;
            Assert.AreEqual(DecompressionMethods.Deflate, handler.AutomaticDecompression, "AutomaticDecompression");
            RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Revalidate);
            handler.CachePolicy = policy;
            Assert.AreEqual(policy, handler.CachePolicy, "CachePolicy");
            Assert.IsNotNull(handler.CookieContainer, "CookieContainer should not be null accessed.");
            CookieContainer cookies = new CookieContainer();
            handler.CookieContainer = cookies;
            Assert.AreEqual(cookies, handler.CookieContainer, "CookieContainer");
            NetworkCredential creds = new NetworkCredential("user", "pass");
            handler.Credentials = creds;
            Assert.AreEqual(creds, handler.Credentials, "Credentials");
            handler.ImpersonationLevel = TokenImpersonationLevel.None;
            Assert.AreEqual(TokenImpersonationLevel.None, handler.ImpersonationLevel, "ImpersonationLevel");
            handler.MaxAutomaticRedirections = 15;
            Assert.AreEqual(15, handler.MaxAutomaticRedirections, "MaxAutomaticRedirections");
            handler.MaxRequestContentBufferSize = 16;
            Assert.AreEqual(16, handler.MaxRequestContentBufferSize, "MaxRequestContentBufferSize");
            handler.MaxResponseHeadersLength = 17;
            Assert.AreEqual(17, handler.MaxResponseHeadersLength, "MaxResponseHeadersLength");
            handler.PreAuthenticate = true;
            Assert.IsTrue(handler.PreAuthenticate, "PreAuthenticate");
            IWebProxy proxy = new WebProxy();
            handler.Proxy = proxy;
            Assert.AreEqual(proxy, handler.Proxy, "Proxy");
            handler.ReadWriteTimeout = 18;
            Assert.AreEqual(18, handler.ReadWriteTimeout, "ReadWriteTimeout");
            handler.UnsafeAuthenticatedConnectionSharing = true;
            Assert.IsTrue(handler.UnsafeAuthenticatedConnectionSharing, "UnsafeAuthenticatedConnectionSharing");
            handler.UseCookies = false;
            Assert.IsFalse(handler.UseCookies, "UseCookies");
            handler.UseDefaultCredentials = true;
            Assert.IsTrue(handler.UseDefaultCredentials, "UseDefaultCredentials");
            handler.UseProxy = false;
            Assert.IsFalse(handler.UseProxy, "UseProxy");
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            Assert.AreEqual(ClientCertificateOption.Automatic, handler.ClientCertificateOptions);
        }

        [TestMethod]
        public void Properties_SetInvalidValues_ArgumentException()
        {
            WebRequestHandler handler = new WebRequestHandler();
            ExceptionAssert.Throws<ArgumentNullException>(() => handler.CookieContainer = null, "CookieContainer");

            handler.UseCookies = false;
            ExceptionAssert.Throws<InvalidOperationException>(() => handler.CookieContainer
                = new CookieContainer(), "CookieContainer");

            handler.UseProxy = false;
            ExceptionAssert.Throws<InvalidOperationException>(() => handler.Proxy = WebRequest.DefaultWebProxy,
                "Proxy");

            handler.MaxRequestContentBufferSize = 0; // no exception. It's OK to set this to 0.
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.MaxRequestContentBufferSize = -1,
                "MaxRequestContentBufferSize");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.MaxAutomaticRedirections = 0,
                "MaxAutomaticRedirections = 0");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.MaxAutomaticRedirections = -1,
                "MaxAutomaticRedirections = -1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.ReadWriteTimeout = 0,
                "ReadWriteTimeout = 0");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.ReadWriteTimeout = -1,
                "ReadWriteTimeout = -1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.MaxResponseHeadersLength = 0,
                "MaxResponseHeadersLength = 0");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => handler.MaxResponseHeadersLength = -1,
                "MaxResponseHeadersLength = -1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
                handler.ContinueTimeout = new TimeSpan(int.MaxValue, 0, 0), "ContinueTimeout = int.MaxValue hours");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
                handler.ClientCertificateOptions = (ClientCertificateOption)3, "ClientCertificateOptions = custom");

            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            ExceptionAssert.Throws<InvalidOperationException>(() => GC.KeepAlive(handler.ClientCertificates.Count),
                "ClientCertificates");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void SendAsync_ContentLengthUndeterminable_Throw()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = 0;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
            message.Content = new MockContent(); // No length, can't set Content Length

            try
            {
                handler.SendAsync(message, CancellationToken.None).Wait();
            }
            catch (AggregateException ag)
            {
                throw ag.GetBaseException();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void SendAsync_ContentLengthLongerThanBuffer_Throw()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = 1;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
            // No length, can't set Content Length
            message.Content = new MockContent(new Action<Stream>(SeralizeHelloWorld), null);

            try
            {
                handler.SendAsync(message, CancellationToken.None).Wait();
            }
            catch (AggregateException ag)
            {
                throw ag.GetBaseException();
            }
        }

        private static void SeralizeHelloWorld(Stream stream)
        {
            byte[] output = Encoding.ASCII.GetBytes("Hello World");
            stream.Write(output, 0, output.Length);
        }

        [TestMethod]
        public void SendAsync_SetHostHeader_HostHeaderSetCorrectly()
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                string host = contextData.Context.Request.Headers["Host"];
                if ((host == null) || (host != "myhost:80"))
                {
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress);
            request.Headers.Host = "myhost:80";
            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result;

            Assert.IsTrue(response.IsSuccessStatusCode, "Host header not set correctly.");
        }

        [TestMethod]
        public void SendAsync_SetUserAgentHeader_ValuesAddedCorrectlyWithSpaceSeparator()
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                string userAgent = contextData.Context.Request.Headers["User-Agent"];
                if ((userAgent == null) ||
                    (userAgent != "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)"))
                {
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.AddHeader("Server", "myserver/1.0 (comment)");
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            Assert.AreEqual(2, request.Headers.UserAgent.Count);
            request.Headers.Add("Accept-Charset", "utf-8; q=0.1, iso-8859-5; q=0.5");
            Assert.AreEqual(2, request.Headers.AcceptCharset.Count);
            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result;
            Assert.AreEqual(3, response.Headers.Server.Count); // http.sys adds a Server header too
            int i = 0;
            foreach (var header in response.Headers.Server)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual("myserver/1.0", header.Product.ToString());
                        break;
                    case 1:
                        Assert.AreEqual("(comment)", header.Comment.ToString());
                        break;
                    case 2:
                        Assert.AreEqual("Microsoft-HTTPAPI", header.Product.Name);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
                i++;
            }

            Assert.IsTrue(response.IsSuccessStatusCode, "Host header not set correctly.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendAsync_NullRequest_Throw()
        {
            WebRequestHandler handler = new WebRequestHandler();
            Task t = handler.SendAsync(null, CancellationToken.None);
            t.Wait();
        }
#if DEBUG
        [TestMethod]
        public void SendAsync_HttpWebRequestThrowsInEndGetResponseUsingGetMethod_Throw()
        {
            ThrowInXGetResponse(Intercept.EndGetResponse, null);
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestThrowsInEndGetResponseUsingPostMethod_Throw()
        {
            ThrowInXGetResponse(Intercept.EndGetResponse, new StringContent("custom"));
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestThrowsInBeginGetResponseUsingGetMethod_Throw()
        {
            ThrowInXGetResponse(Intercept.BeginGetResponse, null);
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestThrowsInBeginGetResponseUsingPostMethod_Throw()
        {
            ThrowInXGetResponse(Intercept.BeginGetResponse, new StringContent("custom"));
        }

        private void ThrowInXGetResponse(Intercept intercept, HttpContent content)
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                using (StreamReader sr = new StreamReader(contextData.Context.Request.InputStream))
                {
                    sr.ReadToEnd();
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            HttpRequestMessage request = null;
            if (content == null)
            {
                request = new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
                request.Content = content;
            }

            WebRequestHandler handler = new WebRequestHandler();
            handler.WebRequestCreator = (req, connectionGroupName) =>
            {
                return new MockHttpWebRequest(req, connectionGroupName, hwr =>
                {
                    // Throw exception as soon as GetResponse() is called. The WebException exception is caught and 
                    // wrapped in an HttpRequestException.
                    throw new WebException("Custom exception.");
                }, intercept);
            };
            Task t = handler.SendAsync(request, CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsFaulted, "IsFaulted");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException),
                    "HttpRequestException expected.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(WebException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestGetCanceledAndThrowsInEndGetResponseUsingGetMethod_Throw()
        {
            CancelAndThrowInXGetResponse(Intercept.EndGetResponse, null);
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestGetCanceledAndThrowsInEndGetResponseUsingPostMethod_Throw()
        {
            CancelAndThrowInXGetResponse(Intercept.EndGetResponse, new StringContent("content"));
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestGetCanceledAndThrowsInBeginGetResponseUsingGetMethod_Throw()
        {
            CancelAndThrowInXGetResponse(Intercept.BeginGetResponse, null);
        }

        [TestMethod]
        public void SendAsync_HttpWebRequestGetCanceledAndThrowsInBeginGetResponseUsingPostMethod_Throw()
        {
            CancelAndThrowInXGetResponse(Intercept.BeginGetResponse, new StringContent("content"));
        }

        private void CancelAndThrowInXGetResponse(Intercept intercept, HttpContent content)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                using (StreamReader sr = new StreamReader(contextData.Context.Request.InputStream))
                {
                    sr.ReadToEnd();
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            HttpRequestMessage request = null;
            if (content == null)
            {
                request = new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
                request.Content = content;
            }

            WebRequestHandler handler = new WebRequestHandler();
            handler.WebRequestCreator = (req, connectionGroupName) =>
            {
                return new MockHttpWebRequest(req, connectionGroupName, hwr =>
                {
                    // Simulate cancel while HWR is processing the respons async: cancel the token in XGetResponse()
                    // but also throw a WebException: We pretend the WebException was thrown due to the cancellation.
                    // Note that since we cancel too late, HWR completes successfully (thus the WebException thrown below).
                    cts.Cancel();
                    throw new WebException("Custom exception.");
                }, intercept);
            };
            Task t = handler.SendAsync(request, cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "IsCanceled");
                Assert.IsFalse(task.IsFaulted, "IsFaulted");
            });
            t.Wait();
        }
#endif
        [TestMethod]
        public void SendAsync_PostStringAndReadResponse_Success()
        {
            HttpTestUtils.StartListening(httpServerData, 1, MirrorContent);

            WebRequestHandler handler = new WebRequestHandler();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = new StringContent("This is the content"); ;

            Task t = handler.SendAsync(request, CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsNull(task.Exception, "Task was faulted.");
                Assert.IsFalse(task.IsCanceled, "Task should not be canceled.");

                HttpResponseMessage response = task.Result;
                Assert.IsNotNull(response, "Response message must not be null.");
                Assert.IsNotNull(response.Content, "Response content should not be null.");
                Assert.AreEqual("This is the content", response.Content.ReadAsStringAsync().Result, "Content");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_CancelWhileServerIsProcessingResponse_Throw()
        {
            AutoResetEvent ev = new AutoResetEvent(false);
            CancellationTokenSource cts = new CancellationTokenSource();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                // Simulate cancellation while server is processing response.
                cts.Cancel();
                ev.WaitOne(5000);
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            try
            {
                Task t = handler.SendAsync(new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress),
                    cts.Token).ContinueWith(task =>
                    {
                        Assert.IsNull(task.Exception, "No exception expected");
                        Assert.IsTrue(task.IsCanceled, "Expected IsCanceled to be true.");
                    });
                t.Wait();
            }
            finally
            {
                ev.Set();
            }
        }

        [TestMethod]
        public void SendAsync_EndGetRequestStreamThrows_WrapExceptionAndFail()
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpContent content = new MockContent(stream => { throw new IOException("custom"); }, 10);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            Task t = handler.SendAsync(request, CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsFaulted, "IsFaulted");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException),
                    "HttpRequestException expected.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_EndGetRequestStreamThrowsAndGetsCanceled_WrapExceptionAndFail()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpContent content = new MockContent(stream => { cts.Cancel(); throw new IOException("custom"); }, 10);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            Task t = handler.SendAsync(request, cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "IsCanceled");
            });
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_PrepareWebRequestForContentUploadThrows_TaskIsFaultedButNoExceptionThrownBySendAsync()
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.StatusCode = 200;
                contextData.Context.Response.Close();
            });

            // We use a content that is not able to calculate the content length. We also _don't_ set chunked. I.e.
            // the handler will try to buffer the content. The mock content will throw when it gets buffered. I.e.
            // PrepareWebRequestForContentUpload will throw. SendAsync() must catch the exception and set the task to
            // faulted.
            WebRequestHandler handler = new WebRequestHandler();
            HttpContent content = new MockContent(stream => { throw new IOException("custom"); }, null);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            Task t = handler.SendAsync(request, CancellationToken.None).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsFaulted, "IsFaulted");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException),
                    "HttpRequestException expected.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException().InnerException, typeof(IOException),
                    "InnerException");
            });
            t.Wait();
        }

        [TestMethod]
        public void PrepareWebRequestForContentUpload_NoChunkedAndContentCantCalculateLength_ContentGetsBufferedToDetermineContentLength()
        {
            byte[] contentData = new byte[5];
            // The following mock content can't calculate content length.
            MockContent content = new MockContent(s => s.Write(contentData, 0, contentData.Length), null);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                if (contextData.Context.Request.Headers[HttpKnownHeaderNames.TransferEncoding] != null)
                {
                    Log.Error("Did not expect Transfer-Encoding header to be set. ");
                    contextData.Context.Response.StatusCode = 400;
                }
                else if (contextData.Context.Request.ContentLength64 != contentData.Length)
                {
                    // Make sure Content-Length is set to the length of the array.
                    Log.Error("Expected Content-Length <{0}>, but was <{1}> ",
                        contentData.Length, contextData.Context.Request.ContentLength64);
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            // The mock content can't calculate the content length and we didn't set chunked to true. The handler must
            // buffer the content to determine the content-length.
            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result.EnsureSuccessStatusCode();

            Assert.AreEqual(1, content.TryComputeLengthCount, "Expected no. of calls to TryComputeLength()");
        }

        [TestMethod]
        public void PrepareWebRequestForContentUpload_NoChunkedAndContentCanCalculateLength_ContentDoesNotGetBuffered()
        {
            byte[] contentData = new byte[5];
            // The following mock content can calculate content length.
            MockContent content = new MockContent(s => s.Write(contentData, 0, contentData.Length), contentData.Length);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                if (contextData.Context.Request.Headers[HttpKnownHeaderNames.TransferEncoding] != null)
                {
                    Log.Error("Did not expect Transfer-Encoding header to be set. ");
                    contextData.Context.Response.StatusCode = 400;
                }
                else if (contextData.Context.Request.ContentLength64 != contentData.Length)
                {
                    // Make sure Content-Length is set to the length of the array.
                    Log.Error("Expected Content-Length <{0}>, but was <{1}> ",
                        contentData.Length, contextData.Context.Request.ContentLength64);
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result.EnsureSuccessStatusCode();

            Assert.AreEqual(1, content.TryComputeLengthCount, "Expected no. of calls to TryComputeLength()");
        }

        [TestMethod]
        public void PrepareWebRequestForContentUpload_NoChunkedAndNoContent_ContentLengthSetTo0()
        {
            // Request has no content but uses POST
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                if (contextData.Context.Request.Headers[HttpKnownHeaderNames.TransferEncoding] != null)
                {
                    Log.Error("Did not expect Transfer-Encoding header to be set. ");
                    contextData.Context.Response.StatusCode = 400;
                }
                else if (contextData.Context.Request.ContentLength64 != 0)
                {
                    // Make sure Content-Length is set to the length of the array.
                    Log.Error("Expected Content-Length <{0}>, but was <{1}> ",
                        0, contextData.Context.Request.ContentLength64);
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public void PrepareWebRequestForContentUpload_ChunkedAndNoContent_ContentLengthNotSet()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);

            // Even though we set chunked, by not providing a content WebRequestHandler will just set Content-Length
            // to 0 rather than setting a chunked encoding.
            request.Headers.TransferEncodingChunked = true;
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                if (contextData.Context.Request.Headers[HttpKnownHeaderNames.TransferEncoding] != null)
                {
                    Log.Error("Did not expect Transfer-Encoding header to be set. ");
                    contextData.Context.Response.StatusCode = 400;
                }
                else if (contextData.Context.Request.ContentLength64 != 0)
                {
                    // Make sure Content-Length is set to the length of the array.
                    Log.Error("Expected Content-Length <{0}>, but was <{1}> ",
                        0, contextData.Context.Request.ContentLength64);
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public void PrepareWebRequestForContentUpload_ChunkedNoBuffered_ContentGetsNotBufferedSinceChunkedIsUsed()
        {
            byte[] contentData = new byte[5];
            MockContent content = new MockContent(s => s.Write(contentData, 0, contentData.Length), null);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Headers.TransferEncodingChunked = true;
            request.Content = content;
            WebRequestHandler handler = new WebRequestHandler();

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                contextData.Context.Response.StatusCode = 200;
                if (contextData.Context.Request.Headers[HttpKnownHeaderNames.TransferEncoding] == null)
                {
                    Log.Error("Expected Transfer-Encoding header to be set. ");
                    contextData.Context.Response.StatusCode = 400;
                }
                contextData.Context.Response.ContentLength64 = 0;
                contextData.Context.Response.Close();
            });

            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result.EnsureSuccessStatusCode();

            Assert.AreEqual(0, content.TryComputeLengthCount, "Expected no. of calls to TryComputeLength()");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void PrepareWebRequestForContentUpload_NoChunkedAndContentCantCalculateLengthAndZeroMaxRequestContentBufferLength_Throw()
        {
            byte[] contentData = new byte[5];
            // The following mock content can't calculate content length.
            MockContent content = new MockContent(s => s.Write(contentData, 0, contentData.Length), null);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpServerData.BaseAddress);
            request.Content = content;
            WebRequestHandler handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = 0; // don't do request content buffering.

            // The mock content can't calculate the content length and we didn't set chunked to true. Since we have a
            // MaxRequestContentBufferSize of 0, the handler can't buffer the content. 
            try
            {
                handler.SendAsync(request, CancellationToken.None).Wait();
            }
            catch (AggregateException ag)
            {
                throw ag.GetBaseException();
            }
        }
#if DEBUG
        [TestMethod]
        public void SetDefaultOptions_UseDefaultValues_AllHandlerDefaultsMatchHttpWebRequestDefaults()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsTrue(hwr.AllowAutoRedirect, "AllowAutoRedirect");
                Assert.AreEqual(AuthenticationLevel.MutualAuthRequested, hwr.AuthenticationLevel, "AuthenticationLevel");
                Assert.AreEqual(DecompressionMethods.None, hwr.AutomaticDecompression, "AutomaticDecompression");
                Assert.AreEqual(HttpWebRequest.DefaultCachePolicy, hwr.CachePolicy, "CachePolicy");
                Assert.AreEqual(0, hwr.ClientCertificates.Count, "ClientCertificates.Count");
                Assert.IsNotNull(hwr.ConnectionGroupName, "ConnectionGroupName");
                Assert.IsNotNull(hwr.CookieContainer, "CookieContainer");
                Assert.IsNull(hwr.Credentials, "Credentials");
                Assert.AreEqual(TokenImpersonationLevel.Delegation, hwr.ImpersonationLevel, "ImpersonationLevel");
                Assert.AreEqual(50, hwr.MaximumAutomaticRedirections, "MaximumAutomaticRedirections");
                Assert.AreEqual(HttpWebRequest.DefaultMaximumResponseHeadersLength, hwr.MaximumResponseHeadersLength,
                    "MaximumResponseHeadersLength");
                Assert.IsTrue(hwr.Pipelined, "Pipelined");
                Assert.IsFalse(hwr.PreAuthenticate, "PreAuthenticate");
                Assert.AreEqual(HttpWebRequest.DefaultWebProxy, hwr.Proxy, "Proxy");
                Assert.AreEqual(Timeout.Infinite, hwr.Timeout, "Timeout");
                Assert.IsFalse(hwr.UnsafeAuthenticatedConnectionSharing, "UnsafeAuthenticatedConnectionSharing");
                Assert.IsFalse(hwr.UseDefaultCredentials, "UseDefaultCredentials");
            }, null);
        }

        [TestMethod]
        public void SetDefaultOptions_SetCustomValuesForAllOptions_AllForwardedToHttpWebRequest()
        {
            RequestCachePolicy cachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            CookieContainer cookies = new CookieContainer();
            IWebProxy proxy = new WebProxy();

            PrepareHandlerAndSendMockRequest(hwr =>
            {
                // Not configurable settings
                Assert.IsNotNull(hwr.ConnectionGroupName, "ConnectionGroupName");
                Assert.AreEqual(Timeout.Infinite, hwr.Timeout, "Timeout");

                // Configurable settings
                Assert.IsFalse(hwr.AllowAutoRedirect, "AllowAutoRedirect");
                Assert.AreEqual(AuthenticationLevel.None, hwr.AuthenticationLevel, "AuthenticationLevel");
                Assert.AreEqual(DecompressionMethods.GZip, hwr.AutomaticDecompression, "AutomaticDecompression");
                Assert.AreEqual(cachePolicy, hwr.CachePolicy, "CachePolicy");
                Assert.AreEqual(1, hwr.ClientCertificates.Count, "ClientCertificates.Count");
                Assert.AreEqual(cookies, hwr.CookieContainer, "CookieContainer");
                Assert.AreEqual(TokenImpersonationLevel.None, hwr.ImpersonationLevel, "ImpersonationLevel");
                Assert.AreEqual(5000, hwr.MaximumResponseHeadersLength, "MaximumResponseHeadersLength");
                Assert.IsFalse(hwr.Pipelined, "Pipelined");
                Assert.IsTrue(hwr.PreAuthenticate, "PreAuthenticate");
                Assert.AreEqual(proxy, hwr.Proxy, "Proxy");
                Assert.AreEqual(12345, hwr.ReadWriteTimeout, "ReadWriteTimeout");
                Assert.IsTrue(hwr.UnsafeAuthenticatedConnectionSharing, "UnsafeAuthenticatedConnectionSharing");
                Assert.IsTrue(hwr.UseDefaultCredentials, "UseDefaultCredentials");

                Assert.AreEqual("CUSTOM", hwr.Method, "Method");
                Assert.AreEqual(new Version(1, 0), hwr.ProtocolVersion, "ProtocolVersion");

            }, (handler, message) =>
            {
                handler.AllowAutoRedirect = false;
                handler.AllowPipelining = false; // HWR.Pipelined
                handler.AuthenticationLevel = AuthenticationLevel.None;
                handler.AutomaticDecompression = DecompressionMethods.GZip;
                handler.CachePolicy = cachePolicy;
                handler.ClientCertificates.Add(new X509Certificate());
                handler.CookieContainer = cookies;
                handler.ImpersonationLevel = TokenImpersonationLevel.None;
                handler.MaxResponseHeadersLength = 5000;
                handler.PreAuthenticate = true;
                handler.Proxy = proxy;
                handler.ReadWriteTimeout = 12345;
                handler.UnsafeAuthenticatedConnectionSharing = true;
                handler.UseDefaultCredentials = true;

                message.Method = new HttpMethod("CUSTOM");
                message.Version = new Version(1, 0);
            });
        }

        [TestMethod]
        public void SetDefaultOptions_SetCustomCredentials_ValueForwardedToHttpWebRequest()
        {
            NetworkCredential creds = new NetworkCredential("user", "pass");

            PrepareHandlerAndSendMockRequest(hwr =>
            {
                // Not configurable settings
                Assert.IsNotNull(hwr.ConnectionGroupName, "ConnectionGroupName");
                Assert.AreEqual(Timeout.Infinite, hwr.Timeout, "Timeout");

                // Configurable settings
                Assert.AreEqual(creds, hwr.Credentials, "Credentials");

            }, (handler, message) => handler.Credentials = creds);
        }

        [TestMethod]
        public void SetDefaultOptions_SetCustomMaxAutomaticRedirects_ValueForwardedToHttpWebRequest()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                // Not configurable settings
                Assert.IsNotNull(hwr.ConnectionGroupName, "ConnectionGroupName");
                Assert.AreEqual(Timeout.Infinite, hwr.Timeout, "Timeout");

                // Configurable settings
                Assert.AreEqual(5, hwr.MaximumAutomaticRedirections, "MaximumAutomaticRedirections");

            }, (handler, message) => handler.MaxAutomaticRedirections = 5);
        }

        [TestMethod]
        public void SetDefaultOptions_SetNoProxy_ProxyOfHttpWebRequestSetToNull()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                // Not configurable settings
                Assert.IsNotNull(hwr.ConnectionGroupName, "ConnectionGroupName");
                Assert.AreEqual(Timeout.Infinite, hwr.Timeout, "Timeout");

                // Configurable settings
                Assert.IsNull(hwr.Proxy, "Proxy");

            }, (handler, message) => handler.UseProxy = false);
        }

        [TestMethod]
        public void SetConnectionOptions_V10MessageWithConnectionKeepAliveHeader_KeepAliveIsTrue()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsTrue(hwr.KeepAlive, "KeepAlive");
                Assert.AreEqual(new Version(1, 0), hwr.ProtocolVersion, "ProtocolVersion");
            }, (handler, message) =>
            {
                message.Version = new Version(1, 0);
                message.Headers.Connection.Add("keep-alive");
            });
        }

        [TestMethod]
        public void SetConnectionOptions_V10MessageWithoutConnectionKeepAliveHeader_KeepAliveIsFalse()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsFalse(hwr.KeepAlive, "KeepAlive");
                Assert.AreEqual(new Version(1, 0), hwr.ProtocolVersion, "ProtocolVersion");
            }, (handler, message) =>
            {
                // Without setting the 'Connection: keep-alive' header, 1.0 requests close connections after 
                // requests complete.
                message.Version = new Version(1, 0);
                message.Headers.Connection.Add("custom");
            });
        }

        [TestMethod]
        public void SetConnectionOptions_V11MessageWithConnectionCloseHeaderNotSet_KeepAliveIsTrue()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsTrue(hwr.KeepAlive, "KeepAlive");
            }, (handler, message) =>
            {
                // Don't set message.Headers.ConnectionClose to false. This should be the default.
                message.Version = new Version(1, 1);
            });
        }

        [TestMethod]
        public void SetConnectionOptions_V11MessageWithConnectionCloseHeaderSet_KeepAliveIsFalse()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsFalse(hwr.KeepAlive, "KeepAlive");
            }, (handler, message) =>
            {
                message.Version = new Version(1, 1);
                message.Headers.ConnectionClose = true;
            });
        }

        [TestMethod]
        public void SetServicePointOptions_SetExpectContinueToFalse_ServicePointUpdated()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsFalse(hwr.ServicePoint.Expect100Continue, "ServicePoint.Expect100Continue");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = new StringContent("This is the content.");
                message.Headers.ExpectContinue = false;
            });
        }

        [TestMethod]
        public void SetRequestHeaders_SetExpectContinueToTrueAndAddCustomExpectHeaderValues_ServicePointUpdatedAndHeadersSent()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsTrue(hwr.ServicePoint.Expect100Continue, "ServicePoint.Expect100Continue");
                // Note there is no space before '100-continue': This header value was added by HWR, whereas all other
                // values were added by the handler.
                Assert.AreEqual("custom1, custom2, custom3,100-continue", hwr.Headers["Expect"], "Expect header values");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = new StringContent("This is the content.");
                message.Headers.ExpectContinue = true;
                message.Headers.Expect.Add(new NameValueWithParametersHeaderValue("custom1"));
                message.Headers.Expect.Add(new NameValueWithParametersHeaderValue("custom2"));
                message.Headers.Expect.Add(new NameValueWithParametersHeaderValue("custom3"));
            });
        }

        [TestMethod]
        public void SetRequestHeaders_SetConnectionCloseToTrueAndAddCustomConnectionHeaderValues_KeepAliveIsFalseAndHeadersSent()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsFalse(hwr.KeepAlive, "KeepAlive");
                // Note there is no space before 'Close': This header value was added by HWR, whereas 'custom1'
                // was added by the handler.
                Assert.AreEqual("custom1,Close", hwr.Headers["Connection"],
                    "Connection header values");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = new StringContent("This is the content.");
                message.Headers.ConnectionClose = true;
                message.Headers.Connection.Add("custom1");
            });
        }

        [TestMethod]
        public void SetRequestHeaders_SetTransferEncodingChunkedToTrueAndAddCustomTransferEncodingHeaderValues_SendChunkedIsTrueAndHeadersSent()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.IsTrue(hwr.SendChunked, "SendChunked");
                // Note there is no space before 'chunked': This header value was added by HWR, whereas all other
                // values were added by the handler.
                Assert.AreEqual("custom1, custom2, custom3,chunked", hwr.Headers["Transfer-Encoding"],
                    "Transfer-Encoding header values");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = new StringContent("This is the content.");
                message.Headers.TransferEncodingChunked = true;
                message.Headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom1"));
                message.Headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom2"));
                message.Headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom3"));
            });
        }

        [TestMethod]
        public void SetRequestHeaders_SetHostHeader_HostPropertySet()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.AreEqual("myhost", hwr.Host, "Host");
                Assert.IsNull(hwr.Headers["Host"], "Host header value should not be added to collection.");
            }, (handler, message) =>
            {
                message.Headers.Host = "myhost";
            });
        }

        [TestMethod]
        public void SetRequestHeaders_RequestHeadersSetKnownAndUnknownHeaders_AllHeadersSentToServer()
        {
            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.AreEqual("\"123\", \"456\"", hwr.Headers["If-Match"], "If-Match");
                Assert.AreEqual("expect1", hwr.Headers["Expect"], "Expect");
                Assert.AreEqual("text/plain; q=0.4", hwr.Headers["Accept"], "Accept");
                Assert.AreEqual("customHeaderValue1", hwr.Headers["customHeader1"], "customHeader1");
                Assert.AreEqual("customHeaderValue21, customHeaderValue22", hwr.Headers["customHeader2"],
                    "customHeader2");
            }, (handler, message) =>
            {
                message.Headers.IfMatch.Add(new EntityTagHeaderValue("\"123\""));
                message.Headers.IfMatch.Add(new EntityTagHeaderValue("\"456\""));
                message.Headers.Expect.Add(new NameValueWithParametersHeaderValue("expect1"));
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 0.4));
                message.Headers.Add("customHeader1", "customHeaderValue1");
                message.Headers.Add("customHeader2", "customHeaderValue21");
                message.Headers.Add("customHeader2", "customHeaderValue22");
            });
        }

        [TestMethod]
        public void SetContentHeaders_RequestHeadersSetKnownAndUnknownHeadersWithContentLength_AllHeadersSentToServer()
        {
            HttpContent content = new StringContent("This is the content.");

            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.AreEqual(content.Headers.ContentLength, hwr.ContentLength, "ContentLength");
                Assert.AreEqual("text/plain", hwr.Headers["Content-Type"], "Content-Location");
                Assert.AreEqual("customHeaderValue1", hwr.Headers["customHeader1"], "customHeader1");

                // Note that there is no space between the two values. The reason is that the two values were combined
                // by HWR.
                Assert.AreEqual("customHeaderValue21,customHeaderValue22", hwr.Headers["customHeader2"],
                    "customHeader2");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = content;

                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                Assert.IsNotNull(content.Headers.ContentLength, "ContentLength should not be null for string content");
                content.Headers.Add("customHeader1", "customHeaderValue1");

                // Add the same custom header to both content and the message. Both values should get combined.
                message.Headers.Add("customHeader2", "customHeaderValue21");
                content.Headers.Add("customHeader2", "customHeaderValue22");
            });
        }

        [TestMethod]
        public void SetContentHeaders_RequestHeadersSetKnownAndUnknownHeadersWithoutContentLength_AllHeadersSentToServer()
        {
            HttpContent content = new StringContent("This is the content.");

            PrepareHandlerAndSendMockRequest(hwr =>
            {
                Assert.AreEqual(-1, hwr.ContentLength, "ContentLength");
                Assert.AreEqual("text/plain", hwr.Headers["Content-Type"], "Content-Location");
                Assert.AreEqual("customHeaderValue1", hwr.Headers["customHeader1"], "customHeader1");

                // Note that there is no space between the two values. The reason is that the two values were combined
                // by HWR.
                Assert.AreEqual("customHeaderValue21,customHeaderValue22", hwr.Headers["customHeader2"],
                    "customHeader2");
            }, (handler, message) =>
            {
                message.Method = HttpMethod.Post;
                message.Content = content;

                // Use chunked so that the handler doesn't set HWR.ContentLength.
                message.Headers.TransferEncodingChunked = true;

                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                content.Headers.Add("customHeader1", "customHeaderValue1");

                // Add the same custom header to both content and the message. Both values should get combined.
                message.Headers.Add("customHeader2", "customHeaderValue21");
                content.Headers.Add("customHeader2", "customHeaderValue22");
            });
        }
#endif
        [TestMethod]
        public void CreateResponseMessage_SendRequestAndCheckResponse_AllPropertiesSetCorrectly()
        {
            byte[] responseContent = Encoding.UTF8.GetBytes("This is the response.");

            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                HttpListenerResponse response = contextData.Context.Response;
                response.StatusDescription = "custom";
                response.StatusCode = 209;
                response.Headers.Add("customHeader", "customValue");
                response.Headers.Add("customHeaderList", "customValue1");
                response.Headers.Add("customHeaderList", "customValue2");
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.ContentLength64 = responseContent.Length;
                response.OutputStream.Write(responseContent, 0, responseContent.Length);
                response.OutputStream.Close();
                response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpResponseMessage responseMessage = handler.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                httpServerData.BaseAddress), CancellationToken.None).Result;

            Assert.AreEqual(209, (int)responseMessage.StatusCode, "StatusCode");
            Assert.AreEqual("custom", responseMessage.ReasonPhrase, "ReasonPhrase");
            Assert.AreEqual("customValue", responseMessage.Headers.GetValues("customHeader").First(), "customHeader");
            Assert.AreEqual("customValue1", responseMessage.Headers.GetValues("customHeaderList").ElementAt(0),
                "customHeader[0]");
            Assert.AreEqual("customValue2", responseMessage.Headers.GetValues("customHeaderList").ElementAt(1),
                "customHeader[1]");

            Assert.IsNotNull(responseMessage.Content, "Content should be assigned.");
            Assert.AreEqual("This is the response.", responseMessage.Content.ReadAsStringAsync().Result, "Content");
            Assert.AreEqual("text/plain", responseMessage.Content.Headers.ContentType.MediaType, "ContentType.MediaType");
            Assert.AreEqual("utf-8", responseMessage.Content.Headers.ContentType.CharSet, "ContentType.CharSet");
            Assert.AreEqual(responseContent.Length, responseMessage.Content.Headers.ContentLength, "ContentLength");
            responseMessage.Content.Dispose();
        }

        [TestMethod]
        public void CreateResponseMessage_SendRedirectedRequestAndCheckFinalRequestUri_LastUriReturned()
        {
            HttpTestUtils.StartListening(httpServerData, 2, contextData =>
            {
                HttpListenerResponse response = contextData.Context.Response;
                if (contextData.Context.Request.Url.AbsolutePath.EndsWith("redirected/"))
                {
                    response.StatusCode = 200;
                    response.StatusDescription = "OK";
                }
                else
                {
                    response.StatusCode = 307;
                    response.StatusDescription = "Temporary Redirect";
                    response.RedirectLocation = httpServerData.BaseAddress + "redirected/";
                }

                response.ContentLength64 = 0;
                response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpResponseMessage responseMessage = handler.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                httpServerData.BaseAddress), CancellationToken.None).Result;

            Assert.AreEqual(200, (int)responseMessage.StatusCode, "StatusCode");
            Assert.AreEqual(httpServerData.BaseAddress + "redirected/",
                responseMessage.RequestMessage.RequestUri.ToString(), "RequestUri");
            responseMessage.Content.Dispose();
        }
#if DEBUG
        [TestMethod]
        public void CreateAndPrepareWebRequest_SetAllRequestHeaders_AllHeadersSentToTheServer()
        {
            PrepareHandlerAndSendMockRequest(null, (handler, message) =>
            {
                StringContent content = new StringContent("content");
                message.Method = HttpMethod.Post;
                message.Content = content;

                HttpRequestHeaders headers = message.Headers;
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 1));
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0));
                headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8", 0.8));
                headers.AcceptCharset.Add(new StringWithQualityHeaderValue("*", 0.1));
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 0.7));
                headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("*", 0.3));
                headers.Authorization = new AuthenticationHeaderValue("Basic", "AQ==");
                headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };
                headers.Connection.Add("custom");
                headers.ConnectionClose = true;
                headers.Date = new DateTimeOffset(2010, 7, 16, 17, 11, 15, TimeSpan.Zero);
                headers.Expect.Add(new NameValueWithParametersHeaderValue("custom", "value"));
                headers.ExpectContinue = true;
                headers.From = "info@example.com";
                headers.Host = "customhost";
                headers.IfMatch.Add(new EntityTagHeaderValue("\"tag1\""));
                headers.IfModifiedSince = new DateTimeOffset(2010, 7, 17, 1, 2, 3, TimeSpan.Zero);
                headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"tag2\""));
                headers.IfUnmodifiedSince = new DateTimeOffset(2010, 7, 17, 1, 2, 4, TimeSpan.Zero);
                headers.MaxForwards = 5;
                headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
                headers.ProxyAuthorization = new AuthenticationHeaderValue("Basic", "AQX=");
                headers.Range = new RangeHeaderValue(1, 5);
                headers.Referrer = new Uri("http://referrer.com/");
                headers.TE.Add(new TransferCodingWithQualityHeaderValue("gzip", 0.1));
                headers.Trailer.Add("Range");
                //headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom")); // HttpListener only supports 'chunked'
                headers.TransferEncodingChunked = true;
                headers.Upgrade.Add(new ProductHeaderValue("HTTP", "1.1"));
                headers.UserAgent.Add(new ProductInfoHeaderValue("Test", "1.0"));
                headers.Via.Add(new ViaHeaderValue("1.1", "localhost"));
                headers.Warning.Add(new WarningHeaderValue(111, "localhost", "\"Revalidation failed\""));
                headers.Add("Custom", "custom_value");

            }, (context) =>
            {
                NameValueCollection headers = context.Request.Headers;

                Assert.AreEqual("text/plain; q=1.0, */*; q=0.0", headers["Accept"], "Accept");
                Assert.AreEqual("utf-8; q=0.8, *; q=0.1", headers["Accept-Charset"], "Accept-Charset");
                Assert.AreEqual("gzip", headers["Accept-Encoding"], "Accept-Encoding");
                Assert.AreEqual("de-DE; q=0.7, *; q=0.3", headers["Accept-Language"], "Accept-Language");
                Assert.AreEqual("Basic AQ==", headers["Authorization"], "Authorization");
                Assert.AreEqual("no-cache", headers["Cache-Control"], "Cache-Control");
                Assert.AreEqual("custom,Close", headers["Connection"], "Connection"); // Close added by HWR
                Assert.AreEqual("Fri, 16 Jul 2010 17:11:15 GMT", headers["Date"], "Date");
                Assert.AreEqual("custom=value,100-continue", headers["Expect"], "Expect");
                Assert.AreEqual("info@example.com", headers["From"], "From");
                Assert.AreEqual("customhost", headers["Host"], "Host");
                Assert.AreEqual("\"tag1\"", headers["If-Match"], "If-Match");
                Assert.AreEqual("Sat, 17 Jul 2010 01:02:03 GMT", headers["If-Modified-Since"], "If-Modified-Since");
                Assert.AreEqual("\"tag2\"", headers["If-None-Match"], "If-None-Match");
                Assert.AreEqual("Sat, 17 Jul 2010 01:02:03 GMT", headers["If-Modified-Since"], "If-Modified-Since");
                Assert.AreEqual("5", headers["Max-Forwards"], "Max-Forwards");
                Assert.AreEqual("no-cache", headers["Pragma"], "Pragma");
                Assert.AreEqual("Basic AQX=", headers["Proxy-Authorization"], "Proxy-Authorization");
                Assert.AreEqual("bytes=1-5", headers["Range"], "Range");
                Assert.AreEqual("http://referrer.com/", headers["Referer"], "Referer");
                Assert.AreEqual("gzip; q=0.1", headers["TE"], "TE");
                Assert.AreEqual("Range", headers["Trailer"], "Trailer");
                Assert.AreEqual("chunked", headers["Transfer-Encoding"], "Transfer-Encoding");
                Assert.AreEqual("HTTP/1.1", headers["Upgrade"], "Upgrade");
                Assert.AreEqual("Test/1.0", headers["User-Agent"], "User-Agent");
                Assert.AreEqual("1.1 localhost", headers["Via"], "Via");
                Assert.AreEqual("111 localhost \"Revalidation failed\"", headers["Warning"], "Warning");
                Assert.AreEqual("custom_value", headers["Custom"], "Custom");
            });
        }

        [TestMethod]
        public void CreateAndPrepareWebRequest_SetAllContentHeaders_AllHeadersSentToTheServer()
        {
            PrepareHandlerAndSendMockRequest(null, (handler, message) =>
            {
                byte[] contentBytes = Encoding.ASCII.GetBytes("content");
                StringContent content = new StringContent("content");
                message.Method = HttpMethod.Post;
                message.Content = content;

                HttpContentHeaders headers = content.Headers;
                headers.Allow.Add("POST");
                headers.ContentEncoding.Add("identity");
                headers.ContentLanguage.Add("en");
                headers.ContentLength = 7;
                headers.ContentLocation = new Uri("http://contentlocation/");
                headers.ContentMD5 = MD5.Create().ComputeHash(contentBytes, 0, contentBytes.Length);
                headers.ContentRange = new ContentRangeHeaderValue(1, 7, 7);
                headers.ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "iso-8859-1" };
                headers.Expires = new DateTimeOffset(2010, 7, 16, 17, 11, 15, TimeSpan.Zero);
                headers.LastModified = new DateTimeOffset(2010, 7, 16, 17, 11, 16, TimeSpan.Zero);
                headers.Add("custom", "custom_value");

            }, (context) =>
            {
                NameValueCollection headers = context.Request.Headers;

                Assert.AreEqual("POST", headers["Allow"], "Allow");
                Assert.AreEqual("identity", headers["Content-Encoding"], "Content-Encoding");
                Assert.AreEqual("en", headers["Content-Language"], "Content-Language");
                Assert.AreEqual("7", headers["Content-Length"], "Content-Length");
                Assert.AreEqual("http://contentlocation/", headers["Content-Location"], "Content-Location");
                Assert.AreEqual("mgNkuembtIDdJeHwKEyFVQ==", headers["Content-MD5"], "Content-MD5");
                Assert.AreEqual("bytes 1-7/7", headers["Content-Range"], "Content-Range");
                Assert.AreEqual("text/plain; charset=iso-8859-1", headers["Content-Type"], "Content-Type");
                Assert.AreEqual("Fri, 16 Jul 2010 17:11:15 GMT", headers["Expires"], "Expires");
                Assert.AreEqual("Fri, 16 Jul 2010 17:11:16 GMT", headers["Last-Modified"], "Last-Modified");
                Assert.AreEqual("custom_value", headers["Custom"], "Custom");
            });
        }
#endif
        #region Helper methods

        public bool AllowAnyServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            return true;  // allow everything
        }
#if DEBUG
        private void PrepareHandlerAndSendMockRequest(Action<HttpWebRequest> interceptor,
            Action<WebRequestHandler, HttpRequestMessage> setup)
        {
            PrepareHandlerAndSendMockRequest(interceptor, setup, null);
        }

        private void PrepareHandlerAndSendMockRequest(Action<HttpWebRequest> interceptor,
            Action<WebRequestHandler, HttpRequestMessage> setup, Action<HttpListenerContext> listenerInterceptor)
        {
            HttpTestUtils.StartListening(httpServerData, 1, contextData =>
            {
                HttpListenerContext context = contextData.Context;
                context.Response.StatusCode = 200;
                context.Response.ContentLength64 = 0;

                if (listenerInterceptor != null)
                {
                    // If the listener interceptor throws an exception, catch it and set the exception information as
                    // response content. Also set a status code of 400. If the client gets a status code of 400 the
                    // test will fail with the response content as error message.
                    try
                    {
                        listenerInterceptor(contextData.Context);
                    }
                    catch (Exception e)
                    {
                        byte[] content = Encoding.ASCII.GetBytes(e.Message);
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.ContentLength64 = content.Length;
                        context.Response.OutputStream.Write(content, 0, content.Length);
                        context.Response.OutputStream.Close();
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(context.Request.InputStream))
                    {
                        sr.ReadToEnd();
                    }
                }
                context.Response.Close();
            });

            WebRequestHandler handler = new WebRequestHandler();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpServerData.BaseAddress);
            if (setup != null)
            {
                setup(handler, request);
            }

            handler.WebRequestCreator = (req, connectionGroupName) =>
            {
                return new MockHttpWebRequest(req, connectionGroupName, interceptor,
                    Intercept.BeginGetResponse | Intercept.GetResponse);
            };

            HttpResponseMessage response = handler.SendAsync(request, CancellationToken.None).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Assert.Fail(response.Content.ReadAsStringAsync().Result);
            }
        }
#endif
        private void MirrorContent(HttpServerContextData contextData)
        {
            HttpListenerRequest request = contextData.Context.Request;
            Stream requestStream = request.InputStream;
            HttpListenerResponse response = contextData.Context.Response;
            response.ContentLength64 = request.ContentLength64;

            byte[] data = new byte[1024];
            int bytesRead = 0;
            do
            {
                bytesRead = requestStream.Read(data, 0, data.Length);
                if (bytesRead > 0)
                {
                    response.OutputStream.Write(data, 0, bytesRead);
                }
            } while (bytesRead > 0);

            response.OutputStream.Close();

            response.Close();
        }

        [Serializable]
        private class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        private class MockContent : HttpContent
        {
            private Action<Stream> serializeToStreamDelegate;
            private bool completeSync;
            private long? length;

            public int SerializeToStreamAsyncCount { get; private set; }
            public int DisposeCount { get; private set; }
            public int TryComputeLengthCount { get; private set; }

            protected internal override bool TryComputeLength(out long length)
            {
                TryComputeLengthCount++;

                length = 0;
                if (this.length == null)
                {
                    return false;
                }

                length = (long)this.length;
                return true;
            }

            public MockContent()
            {
            }

            public MockContent(Action<Stream> serializeToStreamDelegate, long? length)
                : this(serializeToStreamDelegate, length, false)
            {
            }

            public MockContent(Action<Stream> serializeToStreamDelegate, long? length, bool completeSync)
            {
                this.completeSync = completeSync;
                this.serializeToStreamDelegate = serializeToStreamDelegate;
                this.length = length;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                SerializeToStreamAsyncCount++;
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

                if (completeSync)
                {
                    DoSerializeToStream(stream, tcs);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(delegate { DoSerializeToStream(stream, tcs); });
                }

                return tcs.Task;
            }

            private void DoSerializeToStream(Stream stream, TaskCompletionSource<object> tcs)
            {
                if (serializeToStreamDelegate == null)
                {
                    tcs.TrySetResult(null);
                }
                else
                {
                    try
                    {
                        serializeToStreamDelegate(stream);
                        tcs.TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        // If the mockResponseDelegate() throws, set the task as faulted using the exception thrown by
                        // the delegate.
                        tcs.TrySetException(e);
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }
        }

        [Flags]
        private enum Intercept
        {
            None = 0x0,
            BeginGetResponse = 0x1,
            EndGetResponse = 0x2,
            GetResponse = 0x4,
            EndGetRequestStream = 0x8,
        }

        private class MockHttpWebRequest : HttpWebRequest
        {
            private Action<HttpWebRequest> interceptorCallback;
            private Intercept intercept;
#pragma warning disable 618 // Ignoring the obsolete API warning
            public MockHttpWebRequest(HttpRequestMessage request, string connectionGroupName,
                Action<HttpWebRequest> interceptorCallback, Intercept intercept)
                : base(GenerateHttpWebRequestSerInfo(request.RequestUri, connectionGroupName), new StreamingContext(StreamingContextStates.All))
            {
                typeof(HttpWebRequest).GetField("_Uri", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, request.RequestUri);
                this.interceptorCallback = interceptorCallback;
                this.intercept = intercept;
            }
#pragma warning restore 0618
            private static SerializationInfo GenerateHttpWebRequestSerInfo(Uri uri, string connectionGroupName)
            {
                SerializationInfo serInfo = new SerializationInfo(typeof(HttpWebRequest), new FormatterConverter());
                serInfo.AddValue("_OriginUri", uri, typeof(Uri));
                serInfo.AddValue("_ConnectionGroupName", connectionGroupName);
                serInfo.AddValue("_HttpRequestHeaders", new WebHeaderCollection(), typeof(WebHeaderCollection));
                serInfo.AddValue("_Proxy", typeof(WebRequest).GetMethod("get_InternalDefaultWebProxy", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null), typeof(IWebProxy));
                serInfo.AddValue("_KeepAlive", true);
                serInfo.AddValue("_Pipelined", true);
                serInfo.AddValue("_AllowAutoRedirect", true);
                serInfo.AddValue("_AllowWriteStreamBuffering", true);
                serInfo.AddValue("_HttpWriteMode", 0);
                serInfo.AddValue("_MaximumAllowedRedirections", 50);
                serInfo.AddValue("_AutoRedirects", 0);
                serInfo.AddValue("_Timeout", 0x186a0);
                serInfo.AddValue("_ReadWriteTimeout", 0x493e0);
                serInfo.AddValue("_MaximumResponseHeadersLength", int.MaxValue);
                serInfo.AddValue("_ContentLength", -1L);
                serInfo.AddValue("_MediaType", "");
                serInfo.AddValue("_OriginVerb", "Get");
                serInfo.AddValue("_Version", new Version(1, 1), typeof(Version));

                return serInfo;
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                if ((intercept & Intercept.BeginGetResponse) != 0)
                {
                    CallInterceptor();
                }
                return base.BeginGetResponse(callback, state);
            }

            public override WebResponse EndGetResponse(IAsyncResult asyncResult)
            {
                if ((intercept & Intercept.EndGetResponse) != 0)
                {
                    CallInterceptor();
                }
                return base.EndGetResponse(asyncResult);
            }

            public override WebResponse GetResponse()
            {
                if ((intercept & Intercept.GetResponse) != 0)
                {
                    CallInterceptor();
                }
                return base.GetResponse();
            }

            private void CallInterceptor()
            {
                if (interceptorCallback != null)
                {
                    interceptorCallback(this);
                }
            }
        }

        #endregion
    }
}
