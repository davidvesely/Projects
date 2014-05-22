using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Net.Test.Common.Logging;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Test.Common;

namespace System.Net.Http.Test
{
    [TestClass]
    public class HttpRequestMessageTest
    {
        [TestMethod]
        public void Ctor_Default_CorrectDefaults()
        {
            HttpRequestMessage rm = new HttpRequestMessage();

            Assert.AreEqual(HttpMethod.Get, rm.Method, "Method doesn't match the default value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(null, rm.RequestUri, "RequestUri doesn't match the default value.");
        }

        [TestMethod]
        public void Ctor_RelativeStringUri_CorrectValues()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Post, "/relative");

            Assert.AreEqual(HttpMethod.Post, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(new Uri("/relative", UriKind.Relative), rm.RequestUri,
                "RequestUri doesn't match the provided value.");
        }

        [TestMethod]
        public void Ctor_AbsoluteStringUri_CorrectValues()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Post, "http://host/absolute/");

            Assert.AreEqual(HttpMethod.Post, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(new Uri("http://host/absolute/"), rm.RequestUri,
                "RequestUri doesn't match the provided value.");
        }

        [TestMethod]
        public void Ctor_NullStringUri_Accepted()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Put, (string)null);

            Assert.AreEqual(null, rm.RequestUri, "Expected 'null' request Uri");
            Assert.AreEqual(HttpMethod.Put, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
        }

        [TestMethod]
        public void Ctor_RelativeUri_CorrectValues()
        {
            Uri uri = new Uri("/relative", UriKind.Relative);
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Post, uri);

            Assert.AreEqual(HttpMethod.Post, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(uri, rm.RequestUri, "RequestUri doesn't match the provided value.");
        }

        [TestMethod]
        public void Ctor_AbsoluteUri_CorrectValues()
        {
            Uri uri = new Uri("http://host/absolute/");
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Post, uri);

            Assert.AreEqual(HttpMethod.Post, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(uri, rm.RequestUri, "RequestUri doesn't match the provided value.");
        }

        [TestMethod]
        public void Ctor_NullUri_Accepted()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Put, (Uri)null);

            Assert.AreEqual(null, rm.RequestUri, "Expected 'null' request Uri");
            Assert.AreEqual(HttpMethod.Put, rm.Method, "Method doesn't match the provided value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullMethod_Throw()
        {
            HttpRequestMessage rm = new HttpRequestMessage(null, "http://example.com");            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NonHttpUri_Throw()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Put, "ftp://example.com");
        }

        [TestMethod]
        public void Dispose_DisposeObject_ContentGetsDisposedAndSettersWillThrowButGettersStillWork()
        {
            HttpRequestMessage rm = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
            MockContent content = new MockContent();
            rm.Content = content;
            Assert.IsFalse(content.IsDisposed, "Content should not be disposed before calling Dispose().");

            rm.Dispose();
            rm.Dispose(); // Multiple calls don't throw.
            
            Assert.IsTrue(content.IsDisposed, "Content not disposed.");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.Method = HttpMethod.Put, "Method");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.RequestUri = null, "RequestUri");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.Version = new Version(1, 0), "Version");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.Content = null, "Content");

            // Property getters should still work after disposing.
            Assert.AreEqual(HttpMethod.Get, rm.Method, "Method");
            Assert.AreEqual(new Uri("http://example.com"), rm.RequestUri, "RequestUri");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version");
            Assert.AreEqual(content, rm.Content, "Content");
        }

        [TestMethod]
        public void Properties_SetPropertiesAndGetTheirValue_MatchingValues()
        {
            HttpRequestMessage rm = new HttpRequestMessage();

            MockContent content = new MockContent();
            Uri uri = new Uri("https://example.com");
            Version version = new Version(1, 0);
            HttpMethod method = new HttpMethod("custom");

            rm.Content = content;
            rm.Method = method;
            rm.RequestUri = uri;
            rm.Version = version;

            Assert.AreEqual(content, rm.Content, "Content");
            Assert.AreEqual(uri, rm.RequestUri, "RequestUri");
            Assert.AreEqual(method, rm.Method, "Method");
            Assert.AreEqual(version, rm.Version, "Version");

            Assert.IsNotNull(rm.Headers, "Headers property should be initialized automatically.");
            Assert.IsNotNull(rm.Properties, "Properties property should be initialized automatically.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RequestUri_SetNonHttpUri_Throw()
        {
            HttpRequestMessage rm = new HttpRequestMessage();
            rm.RequestUri = new Uri("ftp://example.com");
        }

        [TestMethod]
        public void MarkAsSent_VerifyFlagIsSetAndMultipleCallsAreOK_MessageFlagSet()
        {
            HttpRequestMessage rm = new HttpRequestMessage();

            Assert.IsTrue(rm.MarkAsSent(), "Message is flagged as sent, even though it wasn't sent.");
            Assert.IsFalse(rm.MarkAsSent(), "Message is not flagged as sent, even though it was sent.");
            Assert.IsFalse(rm.MarkAsSent(), "Message is not flagged as sent after calling MarkAsSent() twice.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Version_SetToNull_Exception()
        {
            HttpRequestMessage rm = new HttpRequestMessage();
            rm.Version = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Method_SetToNull_Exception()
        {
            HttpRequestMessage rm = new HttpRequestMessage();
            rm.Method = null;
        }

        [TestMethod]
        public void ToString_DefaultAndNonDefaultInstance_DumpAllFields()
        {
            HttpRequestMessage rm = new HttpRequestMessage();
            Assert.AreEqual("Method: GET, RequestUri: '<null>', Version: 1.1, Content: <null>, Headers:\r\n{\r\n}", 
                rm.ToString());

            rm.Method = HttpMethod.Put;
            rm.RequestUri = new Uri("http://a.com/");
            rm.Version = new Version(1, 0);
            rm.Content = new StringContent("content");

            // Note that there is no Content-Length header: The reason is that the value for Content-Length header
            // doesn't get set by StringContent..ctor, but ony if someone actually accesses the ContentLength property.
            Assert.AreEqual(
                "Method: PUT, RequestUri: 'http://a.com/', Version: 1.0, Content: System.Net.Http.StringContent, Headers:\r\n" +
                "{\r\n" +
                "  Content-Type: text/plain; charset=utf-8\r\n" +
                "}", rm.ToString());

            rm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 0.2));
            rm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml", 0.1));
            rm.Headers.Add("Custom-Request-Header", "value1");            
            rm.Content.Headers.Add("Custom-Content-Header", "value2");

            Assert.AreEqual(
                "Method: PUT, RequestUri: 'http://a.com/', Version: 1.0, Content: System.Net.Http.StringContent, Headers:\r\n" +
                "{\r\n" +
                "  Accept: text/plain; q=0.2\r\n" +
                "  Accept: text/xml; q=0.1\r\n" +
                "  Custom-Request-Header: value1\r\n" +
                "  Content-Type: text/plain; charset=utf-8\r\n" +
                "  Custom-Content-Header: value2\r\n" +
                "}", rm.ToString());
        }

        #region Helper methods

        private void AssertObjectDisposedException(Action action, string failureText)
        {
            try
            {
                action();
                Assert.Fail("Expected ObjectDisposedException: {0}", failureText);
            }
            catch (ObjectDisposedException) { }
        }

        private class MockContent : HttpContent
        {
            public bool IsDisposed { get; private set; }

            protected internal override bool TryComputeLength(out long length)
            {
                throw new NotImplementedException();
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                throw new NotImplementedException();
            }

            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}
