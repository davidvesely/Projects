using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Net.Test.Common;
using System.Net.Test.Common.Logging;

namespace System.Net.Http.Test
{
    [TestClass]
    public class HttpResponseMessageTest
    {
        [TestMethod]
        public void Ctor_Default_CorrectDefaults()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            
            Assert.AreEqual(HttpStatusCode.OK, rm.StatusCode, "Status Code doesn't match the default value.");
            Assert.AreEqual("OK", rm.ReasonPhrase, "Reason Phrase doesn't match the default value.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(null, rm.RequestMessage, "RequestMessage doesn't match the default value.");
        }

        [TestMethod]
        public void Ctor_SpecifiedValues_CorrectValues()
        {
            HttpResponseMessage rm = new HttpResponseMessage(HttpStatusCode.Accepted);

            Assert.AreEqual(HttpStatusCode.Accepted, rm.StatusCode, "Status Code doesn't match the provided value.");
            Assert.AreEqual("Accepted", rm.ReasonPhrase, "Reason Phrase doesn't match the expected default.");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version doesn't match the default value.");
            Assert.AreEqual(null, rm.Content, "Content doesn't match the default value.");
            Assert.AreEqual(null, rm.RequestMessage, "RequestMessage doesn't match the default value.");
        }

        [TestMethod]
        public void Ctor_InvalidStatusCodeRange_Throw()
        {
            int x = -1;
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new HttpResponseMessage((HttpStatusCode)x), "-1");
            x = 1000;
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => new HttpResponseMessage((HttpStatusCode)x), "1000");
        }

        [TestMethod]
        public void Dispose_DisposeObject_ContentGetsDisposedAndSettersWillThrowButGettersStillWork()
        {
            HttpResponseMessage rm = new HttpResponseMessage(HttpStatusCode.OK);
            MockContent content = new MockContent();
            rm.Content = content;
            Assert.IsFalse(content.IsDisposed, "Content should not be disposed before calling Dispose().");

            rm.Dispose();
            rm.Dispose(); // Multiple calls don't throw.

            Assert.IsTrue(content.IsDisposed, "Content not disposed.");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.StatusCode = HttpStatusCode.BadRequest, "StatusCode");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.ReasonPhrase = "Bad Request", "ReasonPhrase");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.Version = new Version(1, 0), "Version");
            ExceptionAssert.ThrowsObjectDisposed(() => rm.Content = null, "Content");

            // Property getters should still work after disposing.
            Assert.AreEqual(HttpStatusCode.OK, rm.StatusCode, "StatusCode");
            Assert.AreEqual("OK", rm.ReasonPhrase, "ReasonPhrase");
            Assert.AreEqual(new Version(1, 1), rm.Version, "Version");
            Assert.AreEqual(content, rm.Content, "Content");
        }

        [TestMethod]
        public void Headers_ReadProperty_HeaderCollectionInitialized()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            Assert.IsNotNull(rm.Headers, "Headers property should be initialized automatically.");
        }

        [TestMethod]
        public void IsSuccessStatusCode_VariousStatusCodes_ReturnTrueFor2xxFalseOtherwise()
        {
            Assert.IsTrue((new HttpResponseMessage()).IsSuccessStatusCode, "Default status code.");
            Assert.IsTrue((new HttpResponseMessage(HttpStatusCode.OK)).IsSuccessStatusCode, "200");
            Assert.IsTrue((new HttpResponseMessage(HttpStatusCode.PartialContent)).IsSuccessStatusCode, "200");

            Assert.IsFalse((new HttpResponseMessage(HttpStatusCode.MultipleChoices)).IsSuccessStatusCode, "300");
            Assert.IsFalse((new HttpResponseMessage(HttpStatusCode.Continue)).IsSuccessStatusCode, "100");
            Assert.IsFalse((new HttpResponseMessage(HttpStatusCode.BadRequest)).IsSuccessStatusCode, "400");
            Assert.IsFalse((new HttpResponseMessage(HttpStatusCode.BadGateway)).IsSuccessStatusCode, "502");
        }

        [TestMethod]
        public void EnsureSuccessStatusCode_VariousStatusCodes_ThrowIfNot2xx()
        {
            ExceptionAssert.Throws<HttpRequestException>(() =>
                (new HttpResponseMessage(HttpStatusCode.MultipleChoices)).EnsureSuccessStatusCode(),
                "Expected exception for 300");
            ExceptionAssert.Throws<HttpRequestException>(() =>
                (new HttpResponseMessage(HttpStatusCode.BadGateway)).EnsureSuccessStatusCode(),
                "Expected exception for 502");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Assert.AreSame(response, response.EnsureSuccessStatusCode(), "Expected same HttpResponseMessage object");
        }

        [TestMethod]
        public void EnsureSuccessStatusCode_AddContentToMessage_ContentIsDisposed()
        {
            HttpResponseMessage response404 = new HttpResponseMessage(HttpStatusCode.NotFound);
            response404.Content = new MockContent();
            ExceptionAssert.Throws<HttpRequestException>(() => response404.EnsureSuccessStatusCode(), 
                "Expected exception for 404");
            Assert.IsTrue((response404.Content as MockContent).IsDisposed, 
                "Expected content to get disposed if EnsureSuccessStatusCode() throws.");

            HttpResponseMessage response200 = new HttpResponseMessage(HttpStatusCode.OK);
            response200.Content = new MockContent();
            response200.EnsureSuccessStatusCode(); // no exception
            Assert.IsFalse((response200.Content as MockContent).IsDisposed,
                "Expected content to NOT be disposed if EnsureSuccessStatusCode() doesn't throw.");
        }

        [TestMethod]
        public void Properties_SetPropertiesAndGetTheirValue_MatchingValues()
        {
            HttpResponseMessage rm = new HttpResponseMessage();

            MockContent content = new MockContent();
            HttpStatusCode statusCode = HttpStatusCode.LengthRequired;
            string reasonPhrase = "Length Required";
            Version version = new Version(1, 0);
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            
            rm.Content = content;
            rm.ReasonPhrase = reasonPhrase;
            rm.RequestMessage = requestMessage;
            rm.StatusCode = statusCode;
            rm.Version = version;

            Assert.AreEqual(content, rm.Content, "Content");
            Assert.AreEqual(reasonPhrase, rm.ReasonPhrase, "ReasonPhrase");
            Assert.AreEqual(requestMessage, rm.RequestMessage, "RequestMessage");
            Assert.AreEqual(statusCode, rm.StatusCode, "StatusCode");
            Assert.AreEqual(version, rm.Version, "StatusCode");
            
            Assert.IsNotNull(rm.Headers, "Headers property should be initialized automatically.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Version_SetToNull_Exception()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.Version = null;
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReasonPhrase_ContainsCRChar_Exception()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.ReasonPhrase = "text\rtext";
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReasonPhrase_ContainsLFChar_Exception()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.ReasonPhrase = "text\ntext";
        }

        [TestMethod]
        public void ReasonPhrase_SetToNull_Accepted()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.ReasonPhrase = null;
            Assert.AreEqual("OK", rm.ReasonPhrase); // Default provided
        }

        [TestMethod]
        public void ReasonPhrase_UnknownStatusCode_Null()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.StatusCode = (HttpStatusCode)150; // Default reason unknown
            Assert.IsNull(rm.ReasonPhrase); // No default provided
        }

        [TestMethod]
        public void ReasonPhrase_SetToEmpty_Accepted()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.ReasonPhrase = String.Empty;
            Assert.AreEqual(String.Empty, rm.ReasonPhrase);
        }

        [TestMethod]
        public void Content_SetToNull_Accepted()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            rm.Content = null;
            Assert.IsNull(rm.Content);
        }

        [TestMethod]
        public void StatusCode_InvalidStatusCodeRange_Throw()
        {
            HttpResponseMessage rm = new HttpResponseMessage();

            int x = -1;
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => { rm.StatusCode = (HttpStatusCode)x; }, "-1");
            x = 1000;
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => { rm.StatusCode = (HttpStatusCode)x; }, "1000");
        }

        [TestMethod]
        public void ToString_DefaultAndNonDefaultInstance_DumpAllFields()
        {
            HttpResponseMessage rm = new HttpResponseMessage();
            Assert.AreEqual("StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: <null>, Headers:\r\n{\r\n}",
                rm.ToString());

            rm.StatusCode = HttpStatusCode.BadRequest;
            rm.ReasonPhrase = null;
            rm.Version = new Version(1, 0);
            rm.Content = new StringContent("content");

            // Note that there is no Content-Length header: The reason is that the value for Content-Length header
            // doesn't get set by StringContent..ctor, but ony if someone actually accesses the ContentLength property.
            Assert.AreEqual(
                "StatusCode: 400, ReasonPhrase: 'Bad Request', Version: 1.0, Content: System.Net.Http.StringContent, Headers:\r\n" +
                "{\r\n" +
                "  Content-Type: text/plain; charset=utf-8\r\n" +
                "}", rm.ToString());

            rm.Headers.AcceptRanges.Add("bytes");
            rm.Headers.AcceptRanges.Add("pages");
            rm.Headers.Add("Custom-Response-Header", "value1");
            rm.Content.Headers.Add("Custom-Content-Header", "value2");

            Assert.AreEqual(
                "StatusCode: 400, ReasonPhrase: 'Bad Request', Version: 1.0, Content: System.Net.Http.StringContent, Headers:\r\n" +
                "{\r\n" +
                "  Accept-Ranges: bytes\r\n" +
                "  Accept-Ranges: pages\r\n" +
                "  Custom-Response-Header: value1\r\n" +
                "  Content-Type: text/plain; charset=utf-8\r\n" +
                "  Custom-Content-Header: value2\r\n" +
                "}", rm.ToString());
        }

        #region Helper methods

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
