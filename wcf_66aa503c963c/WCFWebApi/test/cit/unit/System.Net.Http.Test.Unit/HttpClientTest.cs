using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using System.Net.Test.Common.Logging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class HttpClientTest
    {
        [TestMethod]
        public void Ctor_NullHandler_CreateDefault()
        {
            HttpClient client = new HttpClient(null);
        }

        [TestMethod]
        public void BaseAddress_ValidBaseAddressUris_SetCorrectly()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://example.com/path/");
            Assert.AreEqual(new Uri("http://example.com/path/"), client.BaseAddress);

            client.BaseAddress = new Uri("http://example.com/path2/");
            Assert.AreEqual(new Uri("http://example.com/path2/"), client.BaseAddress);

            client.BaseAddress = null;
            Assert.IsNull(client.BaseAddress);
        }

        [TestMethod]
        public void BaseAddress_InvalidBaseAddressUris_SetCorrectly()
        {
            HttpClient client = new HttpClient();
            ExceptionAssert.Throws<ArgumentException>(() => client.BaseAddress = new Uri("ftp://example.com"),
                "Invalid scheme.");
            ExceptionAssert.Throws<ArgumentException>(() => client.BaseAddress = new Uri("/path", UriKind.Relative),
                "Relative Uri");
        }

        [TestMethod]
        public void DefaultRequestHeaders_GetAndSetValues_UpdatedCorrectly()
        {
            HttpClient client = new HttpClient();
            Assert.IsNotNull(client.DefaultRequestHeaders, "DefaultRequestHeaders must not be null.");
            Assert.AreEqual(0, client.DefaultRequestHeaders.Count(), "DefaultRequestHeaders.Count");
            Assert.IsInstanceOfType(client.DefaultRequestHeaders, typeof(HttpRequestHeaders), "DefaultRequestHeaders");
        }

        [TestMethod]
        public void GetString_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            // All async operations complete sync. No need to wait for the task to complete.
            CheckConvencienceMethod(c => { Task t = c.GetStringAsync("/get"); t.Wait(); }, "/get", null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetStringAsync((string)null); t.Wait(); }, null, null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetStringAsync((Uri)null); t.Wait(); }, null, null, HttpMethod.Get);
        }

        [TestMethod]
        public void GetByteArray_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            // All async operations complete sync. No need to wait for the task to complete.
            CheckConvencienceMethod(c => { Task t = c.GetByteArrayAsync("/get"); t.Wait(); }, "/get", null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetByteArrayAsync((string)null); t.Wait(); }, null, null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetByteArrayAsync((Uri)null); t.Wait(); }, null, null, HttpMethod.Get);
        }

        [TestMethod]
        public void GetStream_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            // All async operations complete sync. No need to wait for the task to complete.
            CheckConvencienceMethod(c => { Task t = c.GetStreamAsync("/get"); t.Wait(); }, "/get", null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetStreamAsync((string)null); t.Wait(); }, null, null, HttpMethod.Get);
            CheckConvencienceMethod(c => { Task t = c.GetStreamAsync((Uri)null); t.Wait(); }, null, null, HttpMethod.Get);
        }

        [TestMethod]
        public void Get_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.Cancel();
            string testUriString = "/get/";
            Uri testUri = new Uri(testUriString, UriKind.Relative);

            // All async operations complete sync. No need to wait for the task to complete.
            CheckConvencienceMethod(c => c.GetAsync(testUriString), testUriString, null, HttpMethod.Get);
            CheckConvencienceMethod(c => c.GetAsync((string)null), null, null, HttpMethod.Get);
            CheckConvencienceMethod(c => c.GetAsync((Uri)null), null, null, HttpMethod.Get);

            CheckConvencienceMethod(c => c.GetAsync(testUriString, HttpCompletionOption.ResponseHeadersRead),
                testUriString, null, HttpMethod.Get);
            CheckConvencienceMethod(c => c.GetAsync(testUriString, token), testUriString, null, HttpMethod.Get, true);
            CheckConvencienceMethod(c => c.GetAsync(testUriString, HttpCompletionOption.ResponseHeadersRead, token),
                testUriString, null, HttpMethod.Get, true);

            CheckConvencienceMethod(c => c.GetAsync(testUri, HttpCompletionOption.ResponseHeadersRead),
                testUriString, null, HttpMethod.Get);
            CheckConvencienceMethod(c => c.GetAsync(testUri, token), testUriString, null, HttpMethod.Get, true);
            CheckConvencienceMethod(c => c.GetAsync(testUri, HttpCompletionOption.ResponseHeadersRead, token),
                testUriString, null, HttpMethod.Get, true);
        }

        [TestMethod]
        public void Put_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.Cancel();
            Uri testUri = new Uri("/put", UriKind.Relative);

            // All async operations complete sync. No need to wait for the task to complete.
            HttpContent content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync("/put", content); t.Wait(); }, "/put", content, 
                HttpMethod.Put);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync((string)null, content); t.Wait(); }, null, content, 
                HttpMethod.Put);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync((Uri)null, content); t.Wait(); }, null, content,
                HttpMethod.Put);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync("/put", null); t.Wait(); }, "/put", null, 
                HttpMethod.Put);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync((string)null, null); t.Wait(); }, null, null, 
                HttpMethod.Put);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PutAsync((Uri)null, null); t.Wait(); }, null, null, 
                HttpMethod.Put);

            content = new StringContent("content");
            CheckConvencienceMethod(c => c.PutAsync("/put", content, token), testUri.ToString(), content,
                HttpMethod.Put, true);
            content = new StringContent("content");
            CheckConvencienceMethod(c => c.PutAsync(testUri, content, token), testUri.ToString(), content, 
                HttpMethod.Put, true);
        }

        [TestMethod]
        public void Post_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.Cancel();
            Uri testUri = new Uri("/post", UriKind.Relative);

            // All async operations complete sync. No need to wait for the task to complete.
            HttpContent content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync("/post", content); t.Wait(); }, "/post", content, 
                HttpMethod.Post);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync((string)null, content); t.Wait(); }, null, content, 
                HttpMethod.Post);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync((Uri)null, content); t.Wait(); }, null, content, 
                HttpMethod.Post);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync("/post", null); t.Wait(); }, "/post", null, 
                HttpMethod.Post);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync((string)null, null); t.Wait(); }, null, null, 
                HttpMethod.Post);
            content = new StringContent("content");
            CheckConvencienceMethod(c => { Task t = c.PostAsync((Uri)null, null); t.Wait(); }, null, null,
                HttpMethod.Post);

            content = new StringContent("content");
            CheckConvencienceMethod(c => c.PostAsync("/post", content, token), testUri.ToString(), content,
                HttpMethod.Post, true);
            content = new StringContent("content");
            CheckConvencienceMethod(c => c.PostAsync(testUri, content, token), testUri.ToString(), content,
                HttpMethod.Post, true);
        }

        [TestMethod]
        public void Delete_CallAndCheckMessagePassedToHandler_RequestMessageContainsExpectedValues()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.Cancel();
            Uri testUri = new Uri("/delete", UriKind.Relative);

            // All async operations complete sync. No need to wait for the task to complete.
            CheckConvencienceMethod(c => { Task t = c.DeleteAsync("/delete"); t.Wait(); }, "/delete", null, 
                HttpMethod.Delete);
            CheckConvencienceMethod(c => { Task t = c.DeleteAsync((string)null); t.Wait(); }, null, null,
                HttpMethod.Delete);
            CheckConvencienceMethod(c => { Task t = c.DeleteAsync((Uri)null); t.Wait(); }, null, null,
                HttpMethod.Delete);

            CheckConvencienceMethod(c => c.DeleteAsync("/delete", token), testUri.ToString(), null, HttpMethod.Delete,
                true);
            CheckConvencienceMethod(c => c.DeleteAsync(testUri, token), testUri.ToString(), null, HttpMethod.Delete, 
                true);
        }

        [TestMethod]
        public void Dispose_DisposeClient_HandlerAlsoDisposed()
        {
            MockHandler handler = new MockHandler(null);
            HttpClient client = new HttpClient(handler);

            Assert.AreEqual(0, handler.DisposeCount, "DisposeCount before Dispose().");

            client.Dispose();

            Assert.AreEqual(1, handler.DisposeCount, "DisposeCount after Dispose().");
        }

        [TestMethod]
        public void Dispose_DisposeClientThenAccessMembers_AllThrow()
        {
            HttpClient client = new HttpClient();
            client.Dispose();

            // Property getters don't throw
            Assert.IsNull(client.BaseAddress, "get_BaseAddress");
            Assert.IsNotNull(client.DefaultRequestHeaders, "get_DefaultRequestHeaders");
            Assert.AreEqual(65536, client.MaxResponseContentBufferSize, "get_MaxResponseContentBufferSize");
            Assert.AreEqual(new TimeSpan(0, 1, 40), client.Timeout, "get_Timeout");

            // Property setter throw
            ExceptionAssert.ThrowsObjectDisposed(() => client.BaseAddress = null, "set_BaseAddress");
            ExceptionAssert.ThrowsObjectDisposed(() => client.MaxResponseContentBufferSize = 5, 
                "set_MaxResponseContentBufferSize");
            ExceptionAssert.ThrowsObjectDisposed(() => client.Timeout = new TimeSpan(1, 0, 0), "set_Timeout");

            // Methods throw
            ExceptionAssert.ThrowsObjectDisposed(() => { Task t = client.SendAsync(new HttpRequestMessage()); t.Wait(); }, 
                "SendAsync()");
            ExceptionAssert.ThrowsObjectDisposed(() => client.CancelPendingRequests(), "CancelPendingRequests()");
        }

        [TestMethod]
        public void Properties_SetPropertiesAfterFirstRequestSync_Throw()
        {
            MockHandler handler = new MockHandler((request, ct) => { return new HttpResponseMessage(); });
            HttpClient client = new HttpClient(handler);

            // Before sending the first request, all properties can be set.
            client.BaseAddress = new Uri("http://example.com");
            client.MaxResponseContentBufferSize = 10;
            client.Timeout = new TimeSpan(0, 1, 0);

            client.SendAsync(new HttpRequestMessage()).Wait();

            // After sending the first request, no property can be set.
            ExceptionAssert.ThrowsInvalidOperation(() => client.BaseAddress = new Uri("http://base/"),
                "BaseAddress");
            ExceptionAssert.ThrowsInvalidOperation(() => client.MaxResponseContentBufferSize = 20,
                "MaxResponseContentBufferSize");
            ExceptionAssert.ThrowsInvalidOperation(() => client.Timeout = new TimeSpan(0, 2, 0), "Timeout");
        }

        [TestMethod]
        public void Properties_SetAndGetProperties_CorrectValuesStoredExceptionForInvalidValues()
        {
            MockHandler handler = new MockHandler((request, ct) => { return new HttpResponseMessage(); });
            HttpClient client = new HttpClient(handler);

            client.BaseAddress = new Uri("http://example.com");
            client.MaxResponseContentBufferSize = 10;
            client.Timeout = new TimeSpan(0, 1, 0);

            Assert.AreEqual(new Uri("http://example.com"), client.BaseAddress, "BaseAddress");
            Assert.AreEqual(10, client.MaxResponseContentBufferSize, "MaxResponseContentBufferSize");
            Assert.AreEqual(new TimeSpan(0, 1, 0), client.Timeout, "Timeout");

            // We're expecting a new Timeout.InfiniteTimeSpan constant in M3S.
            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            Assert.AreEqual(Timeout.Infinite, client.Timeout.TotalMilliseconds, "Infinite Timeout");            

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => client.MaxResponseContentBufferSize = 0,
                "MaxResponseContentBufferSize");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => client.Timeout = new TimeSpan(0,0,0,0,-2),
                "Invalid Timeout");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => client.Timeout = TimeSpan.Zero,
                "Invalid Timeout");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => client.Timeout 
                = new TimeSpan(0, 0, 0, 1, Int32.MaxValue), "Invalid Timeout");
        }

        [TestMethod]
        public void Properties_SetPropertiesAfterFirstRequestAsync_Throw()
        {
            MockHandler handler = new MockHandler((request, ct) => { return new HttpResponseMessage(); });
            HttpClient client = new HttpClient(handler);

            // Before sending the first request, all properties can be set.
            client.BaseAddress = new Uri("http://example.com");
            client.MaxResponseContentBufferSize = 10;
            client.Timeout = new TimeSpan(0, 1, 0);

            Task t = client.SendAsync(new HttpRequestMessage());
            t.Wait();

            // After sending the first request, no property can be set.
            ExceptionAssert.ThrowsInvalidOperation(() => client.BaseAddress = new Uri("http://base/"),
                "BaseAddress");
            ExceptionAssert.ThrowsInvalidOperation(() => client.MaxResponseContentBufferSize = 20,
                "MaxResponseContentBufferSize");
            ExceptionAssert.ThrowsInvalidOperation(() => client.Timeout = new TimeSpan(0, 2, 0), "Timeout");
        }

        [TestMethod]
        public void PropertiesProperty_PopulatedAndSent_CarriedOver()
        {
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.RequestMessage = request;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            // State completely unrelated to the request
            NetworkInformation.PingOptions stateObject = new NetworkInformation.PingOptions(5, true);

            HttpRequestMessage initialRequest = new HttpRequestMessage();
            initialRequest.Properties.Add("state", stateObject);

            HttpResponseMessage finalResponse = client.SendAsync(initialRequest).Result;
            Assert.AreEqual(initialRequest, finalResponse.RequestMessage);
            Assert.AreEqual(stateObject, finalResponse.RequestMessage.Properties["state"]);
        }

        [TestMethod]
        public void PropertiesProperty_PopulatedAndSentAsync_CarriedOver()
        {
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.RequestMessage = request;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            // State completely unrelated to the request
            NetworkInformation.PingOptions stateObject = new NetworkInformation.PingOptions(5, true);

            HttpRequestMessage initialRequest = new HttpRequestMessage();
            initialRequest.Properties.Add("state", stateObject);

            Task t = client.SendAsync(initialRequest).ContinueWith(task =>
            {
                Assert.AreEqual(initialRequest, task.Result.RequestMessage);
                Assert.AreEqual(stateObject, task.Result.RequestMessage.Properties["state"]);
            });
            t.Wait();
        }

        [TestMethod]
        public void DefaultRequestHeaders_SetHeadersAndSendRequest_DefaultHeadersMergedWithMessageHeaders()
        {
            MediaTypeWithQualityHeaderValue acceptValue = new MediaTypeWithQualityHeaderValue("text/xml");

            MockHandler handler = new MockHandler((request, ct) =>
            {
                Assert.AreEqual(4, request.Headers.Count(), "Headers.Count");
                Assert.AreEqual("message1", request.Headers.GetValues("custom1").First(), "custom1 value");
                Assert.AreEqual("message2", request.Headers.GetValues("custom2").First(), "custom2 value");
                Assert.AreEqual("default3", request.Headers.GetValues("custom3").First(), "custom3 value");

                // Make sure the Accept header is copied from DefaultRequestHeaders to the message. However, it must
                // be a clone and not the original header value object.
                Assert.AreEqual(acceptValue, request.Headers.Accept.First(), "Accept value");
                Assert.AreNotSame(acceptValue, request.Headers.Accept.First(), "Accept value");

                Assert.AreEqual("default3", request.Headers.GetValues("custom3").First(), "custom3 value");
                return new HttpResponseMessage();
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            client.DefaultRequestHeaders.Add("custom1", "default1");
            client.DefaultRequestHeaders.Add("custom3", "default3");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            HttpRequestMessage rm2 = new HttpRequestMessage();
            rm2.Headers.Add("custom1", "message1");
            rm2.Headers.Add("custom2", "message2");

            Task t = client.SendAsync(rm2);
            t.Wait();
        }

        [TestMethod]
        public void MaxResponseContentBufferSize_TryGetContentWithSizeExceedingMaxResponseContentBufferSizeAsync_Throw()
        {
            StringContent content = new StringContent("This is the content.");
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            // Set MaxResponseContentBufferSize to a value that is less than the actual content size.
            client.MaxResponseContentBufferSize = 5;

            Task t = client.SendAsync(new HttpRequestMessage()).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Expected task to be faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException), 
                    "Expected HttpRequestException");
            });
            t.Wait();
        }

        [TestMethod]
        public void CancelPendingRequests_CancelWhileHandlerIsWorking_Thrown()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockContent content = new MockContent();
            AutoResetEvent started = new AutoResetEvent(false);

            // We simulate the CTS being canceled while the handler is processing the request.
            MockHandler handler = new MockHandler((request, ct) =>
            {
                // Signal main thread that we started. It's OK if CancelPendingRequests() is called before we register 
                // the cancellation callback.
                started.Set();

                AutoResetEvent ev = new AutoResetEvent(false);
                ct.Register(() => ev.Set());
                if (!ev.WaitOne(5000)) // wait for CTS.Cancel() to be called to unblock us.
                {
                    Log.Error("Cancellation token was never canceled.");
                }
                ct.ThrowIfCancellationRequested();
                return null;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            // Send one request and call CancelPendingRequests()
            Task firstRequest = Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(), cts.Token).Wait())
                .ContinueWith(task =>
                {
                    Assert.IsNotNull(task.Exception, "First request should fail due to cancellation.");
                    Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                        "First request should throw OperationCanceledException");
                });

            Assert.IsTrue(started.WaitOne(5000), "First request was never started.");
            client.CancelPendingRequests();
            Assert.IsTrue(firstRequest.Wait(5000), "First request did not complete.");

            // Send a second request to make sure CancelPendingRequests() works when called multiple times.
            Task secondRequest = Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(), cts.Token).Wait())
                .ContinueWith(task =>
                {
                    Assert.IsNotNull(task.Exception, "Second request should fail due to cancellation.");
                    Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                        "Second request should throw OperationCanceledException");
                });

            Assert.IsTrue(started.WaitOne(5000), "Second request was never started.");
            client.CancelPendingRequests();
            Assert.IsTrue(secondRequest.Wait(5000), "Second request did not complete.");

            cts.Dispose(); // this should not throw                        
        }

        [TestMethod]
        public void Timeout_ARequestTimesOut_ThrowsOperationCanceledException()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockContent content = new MockContent();
            AutoResetEvent started = new AutoResetEvent(false);

            // We simulate the timeout happening while the handler is processing the request.
            MockHandler handler = new MockHandler((request, ct) =>
            {
                // Signal main thread that we started. It's OK if the timeout happens before we register 
                // the cancellation callback.
                started.Set();

                AutoResetEvent ev = new AutoResetEvent(false);
                ct.Register(() => ev.Set());
                if (!ev.WaitOne(5000)) // wait for the timeout to be called to unblock us.
                {
                    Log.Error("Cancellation token was never canceled.");
                }
                ct.ThrowIfCancellationRequested();
                return null;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");
            client.Timeout = new TimeSpan(0, 0, 1); // 1s

            // Send one request and wait for the timeout
            Task firstRequest = Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(), cts.Token).Wait())
                .ContinueWith(task =>
                {
                    Assert.IsNotNull(task.Exception, "First request should fail due to cancellation.");
                    Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                        "First request should throw OperationCanceledException");
                });

            Assert.IsTrue(started.WaitOne(5000), "First request was never started.");
            Assert.IsTrue(firstRequest.Wait(5000), "First request did not complete.");

            // Send a second request to make sure Timeout works when called multiple times.
            Task secondRequest = Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(), cts.Token).Wait())
                .ContinueWith(task =>
                {
                    Assert.IsNotNull(task.Exception, "Second request should fail due to cancellation.");
                    Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(OperationCanceledException),
                       "Second request should throw OperationCanceledException");
                });

            Assert.IsTrue(started.WaitOne(5000), "Second request was never started.");
            Assert.IsTrue(secondRequest.Wait(5000), "Second request did not complete.");

            cts.Dispose(); // this should not throw                        
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_NoBaseAddressAndRelativeMessageRequestUri_Throw()
        {
            HttpClient client = new HttpClient();
            MockHandler handler = new MockHandler((request, ct) => { return new HttpResponseMessage(); });
            client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/path")).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_NoBaseAddressAndNoMessageRequestUri_Throw()
        {
            HttpClient client = new HttpClient();
            MockHandler handler = new MockHandler((request, ct) => { return new HttpResponseMessage(); });
            client.SendAsync(new HttpRequestMessage(HttpMethod.Get, (Uri)null)).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendAsync_NullRequestMessage_Throw()
        {
            HttpClient client = new HttpClient();
            Task t = client.SendAsync(null);
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_HandlerReturnsNullResponseMessage_Throw()
        {
            MockHandler handler = new MockHandler((request, ct) => { return null; });
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");
            Task t = client.SendAsync(new HttpRequestMessage()).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(InvalidOperationException));
            }, TaskContinuationOptions.ExecuteSynchronously);
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_HandlerThrowsException_Throw()
        {
            // Note that HttpClient doesn't catch exceptions, wrap them in a HttpRequestException and re-throw them. 
            // Developers are responsible for throwing HttpRequestException instances if the error is related to the request
            // execution. 
            MockHandler handler = new MockHandler((request, ct) => { throw new MockException(); });
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");
            Task t = client.SendAsync(new HttpRequestMessage()).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException));
            }, TaskContinuationOptions.ExecuteSynchronously); // So we don't have to wait for the task to complete.
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_ReadingMockContentThrows_Throw()
        {
            MockContent content = new MockContent(true);
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage()).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(MockException));
            }, TaskContinuationOptions.ExecuteSynchronously);
            t.Wait();

            Assert.AreEqual(1, content.DisposeCount, "Response content leaked");
        }

        [TestMethod]
        public void SendAsync_ResponseContentTooLargeToBuffer_ContentDisposed()
        {
            MockContent content = new MockContent(stream => stream.Write(new byte[100], 0, 100), false);
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.MaxResponseContentBufferSize = 99;
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage()).ContinueWith(task =>
            {
                Assert.IsNotNull(task.Exception, "Task not faulted.");
                Assert.IsInstanceOfType(task.Exception.GetBaseException(), typeof(HttpRequestException));
            }, TaskContinuationOptions.ExecuteSynchronously);
            t.Wait();

            Assert.AreEqual(1, content.DisposeCount, "Response content leaked");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAsync_SendMessageTwice_Throw()
        {
            MockHandler handler = new MockHandler((request, ct) => new HttpResponseMessage());
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            HttpRequestMessage rm = new HttpRequestMessage();

            Task t = null;
            try
            {
                t = client.SendAsync(rm);
                t.Wait();
            }
            catch (Exception)
            {
                Assert.Fail("First call to SendAsync() should not throw.");
            }

            t = client.SendAsync(rm);
            t.Wait();
        }

        [TestMethod]
        public void SendAsync_CheckOverloadMessage_CallForwardedCorrectly()
        {
            MockContent content = new MockContent();
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage());
            t.Wait();
            Assert.IsTrue(handler.CancellationTokenSet, "Cancellation token was not set.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "Content was not buffered.");
        }

        [TestMethod]
        public void SendAsync_CheckOverloadCancellationToken_CallForwardedCorrectly()
        {
            MockContent content = new MockContent();
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task t = client.SendAsync(new HttpRequestMessage(), cts.Token);
            t.Wait();
            Assert.IsTrue(handler.CancellationTokenSet, "Cancellation token was not set.");
            Assert.AreEqual(1, content.SerializeToStreamAsyncCount, "Content was not buffered.");
        }

        [TestMethod]
        public void SendAsync_CheckOverloadCompletionOption_CallForwardedCorrectly()
        {
            MockContent content = new MockContent();
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage(), HttpCompletionOption.ResponseHeadersRead);
            t.Wait();
            Assert.IsTrue(handler.CancellationTokenSet, "Cancellation token was not set.");
            Assert.AreEqual(0, content.SerializeToStreamAsyncCount, "Content was buffered even though it wasn't supposed to.");
        }

        [TestMethod]
        public void SendAsync_CancelWhileHandlerIsWorkingOnBackgroundThread_Thrown()
        {
            CancelWhileHandlerIsWorking(false);
        }

        [TestMethod]
        public void SendAsync_CancelWhileHandlerIsWorkingOnCurrentThread_Thrown()
        {
            CancelWhileHandlerIsWorking(true);
        }

        private static void CancelWhileHandlerIsWorking(bool completeSync)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockContent content = new MockContent();

            // We simulate the CTS being canceled while the handler is processing the request.
            MockHandler handler = new MockHandler((request, ct) =>
            {
                cts.Cancel();
                ct.ThrowIfCancellationRequested();
                Assert.Fail("Expected operation to be canceled.");
                return null;
            }, completeSync); // complete on background thread
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "Expected task to be canceled.");

            }, TaskContinuationOptions.ExecuteSynchronously);
            t.Wait();

            cts.Dispose(); // this should not throw
        }

        [TestMethod]
        public void SendAsync_CancelWhileBufferingContentOnBackgroundThread_Thrown()
        {
            CancelWhileBufferingContent(false);
        }

        [TestMethod]
        public void SendAsync_CancelWhileBufferingContentOnCurrentThread_Thrown()
        {
            CancelWhileBufferingContent(true);
        }

        private static void CancelWhileBufferingContent(bool completeSync)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            MockContent content = new MockContent(s =>
            {
                cts.Cancel();
                // This exception is thrown as consequence of the cancellation. Like a WebException would be 
                // thrown when reading from the response stream of an aborted HttpWebRequest.
                throw new HttpRequestException();
            }, completeSync);

            // We simulate the CTS being canceled while we're buffering the response content.
            MockHandler handler = new MockHandler((request, ct) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = content;
                return response;
            }, completeSync);

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            Task t = client.SendAsync(new HttpRequestMessage(), cts.Token).ContinueWith(task =>
            {
                Assert.IsTrue(task.IsCanceled, "Expected task to be canceled.");

            }, TaskContinuationOptions.ExecuteSynchronously);
            t.Wait();

            Assert.AreEqual(1, content.DisposeCount);
            cts.Dispose(); // this should not throw
        }

        [TestMethod]
        public void SendAsync_UploadContent_RequestContentGetsDisposed()
        {
            MockContent uploadContent = new MockContent();
            MockHandler handler = new MockHandler((r, cts) =>
            {
                Assert.AreEqual(0, uploadContent.DisposeCount, "Request content must not be disposed before the upload.");
                return new HttpResponseMessage();
            });

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");

            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = uploadContent;

            Task t = client.SendAsync(request);
            t.Wait();
            Assert.AreEqual(1, uploadContent.DisposeCount,
                "Expected request content to be disposed after sending request.");
        }

        #region Helper methods

        private static void CheckConvencienceMethod(Action<HttpClient> convenienceMethodCall, string requestUri,
            HttpContent content, HttpMethod expectedMethod, bool isCancelled = false)
        {
            MockHandler handler = new MockHandler((request, ct) =>
            {
                Assert.AreSame(content, request.Content, "Content");
                Assert.AreEqual(expectedMethod, request.Method, "Method");
                Assert.AreEqual("http://example.com" + (requestUri == null ? "/" : requestUri),
                    request.RequestUri.ToString(), "RequestUri");
                Assert.AreEqual(isCancelled, ct.IsCancellationRequested);
                return new HttpResponseMessage();
            });
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://example.com");
            convenienceMethodCall(client);
        }

        [Serializable]
        private class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        private class MockHandler : HttpMessageHandler
        {
            private Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> mockResponseDelegate;
            private bool completeSync;

            public int DisposeCount { get; private set; }
            public bool CancellationTokenSet { get; private set; }

            public MockHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> mockResponseDelegate)
                : this(mockResponseDelegate, true)
            {
            }

            public MockHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> mockResponseDelegate,
                bool completeSync)
            {
                this.mockResponseDelegate = mockResponseDelegate;
                this.completeSync = completeSync;
            }

            protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                CancellationTokenSet = cancellationToken != CancellationToken.None;

                TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();

                if (completeSync)
                {
                    DoSend(request, cancellationToken, tcs);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(delegate { DoSend(request, cancellationToken, tcs); });
                }

                return tcs.Task;
            }

            private void DoSend(HttpRequestMessage request, CancellationToken cancellationToken,
                TaskCompletionSource<HttpResponseMessage> tcs)
            {
                if (mockResponseDelegate == null)
                {
                    tcs.TrySetResult(new HttpResponseMessage());
                }
                else
                {
                    try
                    {
                        tcs.TrySetResult(mockResponseDelegate(request, cancellationToken));
                    }
                    catch (OperationCanceledException e)
                    {
                        if (cancellationToken.IsCancellationRequested && (cancellationToken == e.CancellationToken))
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetException(e);
                        }
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

        private class MockContent : HttpContent
        {
            private bool alwaysThrow;
            private Action<Stream> serializeToStreamDelegate;
            private bool completeSync;

            public int SerializeToStreamAsyncCount { get; private set; }
            public int DisposeCount { get; private set; }

            protected internal override bool TryComputeLength(out long length)
            {
                length = 0;
                return true;
            }

            public MockContent()
            {
            }

            public MockContent(bool alwaysThrow)
            {
                this.alwaysThrow = alwaysThrow;
            }

            public MockContent(Action<Stream> serializeToStreamDelegate, bool completeSync)
            {
                this.completeSync = completeSync;
                this.serializeToStreamDelegate = serializeToStreamDelegate;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                SerializeToStreamAsyncCount++;
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

                if (alwaysThrow)
                {
                    tcs.TrySetException(new MockException());
                }

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

        #endregion
    }
}
