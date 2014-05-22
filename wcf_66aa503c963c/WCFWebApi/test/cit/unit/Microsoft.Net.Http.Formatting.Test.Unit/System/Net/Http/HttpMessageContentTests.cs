// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.IO;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Text;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpMessageContentTests : UnitTest<HttpMessageContent>
    {
        private static readonly int iterations = 5;

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageContent is public, non-sealed type.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsDisposable,
                typeof(HttpContent));
        }
        #endregion

        #region Helpers
        private static void AddMessageHeaders(HttpHeaders headers)
        {
            headers.Add("N1", new string[] { "V1a", "V1b", "V1c", "V1d", "V1e" });
            headers.Add("N2", "V2");
        }

        private static HttpRequestMessage CreateRequest(bool content)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage();
            httpRequest.Method = new HttpMethod(ParserData.HttpMethod);
            httpRequest.RequestUri = ParserData.HttpRequestUri;
            httpRequest.Version = ParserData.Versions[2];
            AddMessageHeaders(httpRequest.Headers);
            if (content)
            {
                httpRequest.Content = new StringContent(ParserData.HttpMessageEntity);
            }

            return httpRequest;
        }

        private static HttpResponseMessage CreateResponse(bool content)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = ParserData.HttpStatus;
            httpResponse.ReasonPhrase = ParserData.HttpReasonPhrase;
            httpResponse.Version = ParserData.Versions[2];
            AddMessageHeaders(httpResponse.Headers);
            if (content)
            {
                httpResponse.Content = new StringContent(ParserData.HttpMessageEntity);
            }

            return httpResponse;
        }

        private static string ReadContentAsync(HttpContent message)
        {
            Task task = message.LoadIntoBufferAsync();
            task.Wait(TimeoutConstant.DefaultTimeout);
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status, "Task should have run to completion.");
            return message.ReadAsStringAsync().Result;
        }

        private static void ValidateRequest(HttpContent message, bool containsEntity)
        {
            Assert.AreEqual(ParserData.HttpRequestMediaType, message.Headers.ContentType, "Media type did not match expected value");
            long? length = message.Headers.ContentLength;
            Assert.IsNotNull(length, "Content length should not be null");

            string content = ReadContentAsync(message);

            if (containsEntity)
            {
                Assert.AreEqual(ParserData.HttpRequestWithEntity.Length, length, "Content length did not match expected value");
                Assert.AreEqual(ParserData.HttpRequestWithEntity, content, "Serialized request did not match expected value");
            }
            else
            {
                Assert.AreEqual(ParserData.HttpRequest.Length, length, "Content length did not match expected value");
                Assert.AreEqual(ParserData.HttpRequest, content, "Serialized request did not match expected value");
            }
        }

        private static void ValidateResponse(HttpContent message, bool containsEntity)
        {
            Assert.AreEqual(ParserData.HttpResponseMediaType, message.Headers.ContentType, "Media type did not match expected value");
            long? length = message.Headers.ContentLength;
            Assert.IsNotNull(length, "Content length should not be null");

            string content = ReadContentAsync(message);

            if (containsEntity)
            {
                Assert.AreEqual(ParserData.HttpResponseWithEntity.Length, length, "Content length did not match expected value");
                Assert.AreEqual(ParserData.HttpResponseWithEntity, content, "Serialized response did not match expected value");
            }
            else
            {
                Assert.AreEqual(ParserData.HttpResponse.Length, length, "Content length did not match expected value");
                Assert.AreEqual(ParserData.HttpResponse, content, "Serialized response did not match expected value");
            }
        }

        #endregion

        #region Constructors
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageContent(HttpRequestMessage) ctor.")]
        public void RequestConstructor()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            HttpMessageContent instance = new HttpMessageContent(request);
            Assert.IsNotNull(instance, "instance should not be null.");
            Assert.AreSame(request, instance.HttpRequestMessage, "Content did not coantain expected request instance.");
            Assert.IsNull(instance.HttpResponseMessage, "Response should be null");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageContent(HttpRequestMessage) throws on null.")]
        public void RequestConstructorThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull("httpRequest", () => { new HttpMessageContent((HttpRequestMessage)null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageContent(HttpResponseMessage) ctor.")]
        public void ResponseConstructor()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            HttpMessageContent instance = new HttpMessageContent(response);
            Assert.IsNotNull(instance, "instance should not be null.");
            Assert.AreSame(response, instance.HttpResponseMessage, "Content did not coantain expected response instance.");
            Assert.IsNull(instance.HttpRequestMessage, "Request should be null");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageContent(HttpResponseMessage) throws on null.")]
        public void ResponseConstructorThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull("httpResponse", () => { new HttpMessageContent((HttpResponseMessage)null); });
        }
        #endregion

        #region Members
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequest()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpRequestMessage request = CreateRequest(false);
                HttpMessageContent instance = new HttpMessageContent(request);
                ValidateRequest(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequestMultipleTimes()
        {
            HttpRequestMessage request = CreateRequest(false);
            HttpMessageContent instance = new HttpMessageContent(request);
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                ValidateRequest(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponse()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpResponseMessage response = CreateResponse(false);
                HttpMessageContent instance = new HttpMessageContent(response);
                ValidateResponse(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponseMultipleTimes()
        {
            HttpResponseMessage response = CreateResponse(false);
            HttpMessageContent instance = new HttpMessageContent(response);
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                ValidateResponse(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequestWithEntity()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpRequestMessage request = CreateRequest(true);
                HttpMessageContent instance = new HttpMessageContent(request);
                ValidateRequest(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequestWithEntityMultipleTimes()
        {
            HttpRequestMessage request = CreateRequest(true);
            HttpMessageContent instance = new HttpMessageContent(request);
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                ValidateRequest(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponseWithEntity()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpResponseMessage response = CreateResponse(true);
                HttpMessageContent instance = new HttpMessageContent(response);
                ValidateResponse(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponseWithEntityMultipleTimes()
        {
            HttpResponseMessage response = CreateResponse(true);
            HttpMessageContent instance = new HttpMessageContent(response);
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                ValidateResponse(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequestAsync()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpRequestMessage request = CreateRequest(false);
                HttpMessageContent instance = new HttpMessageContent(request);
                ValidateRequest(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponseAsync()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpResponseMessage response = CreateResponse(false);
                HttpMessageContent instance = new HttpMessageContent(response);
                ValidateResponse(instance, false);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeRequestWithEntityAsync()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpRequestMessage request = CreateRequest(true);
                HttpMessageContent instance = new HttpMessageContent(request);
                ValidateRequest(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsStringAsync should return message value.")]
        public void SerializeResponseWithEntityAsync()
        {
            for (int cnt = 0; cnt < iterations; cnt++)
            {
                HttpResponseMessage response = CreateResponse(true);
                HttpMessageContent instance = new HttpMessageContent(response);
                ValidateResponse(instance, true);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Dispose should dispose inner HttpRequestMessage.")]
        public void DisposeInnerHttpRequestMessage()
        {
            HttpRequestMessage request = CreateRequest(false);
            HttpMessageContent instance = new HttpMessageContent(request);
            instance.Dispose();
            Asserters.Exception.ThrowsObjectDisposed(typeof(HttpRequestMessage).FullName, () => { request.Method = HttpMethod.Get; });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Dispose should dispose inner HttpResponseMessage.")]
        public void DisposeInnerHttpResponseMessage()
        {
            HttpResponseMessage response = CreateResponse(false);
            HttpMessageContent instance = new HttpMessageContent(response);
            instance.Dispose();
            Asserters.Exception.ThrowsObjectDisposed(typeof(HttpResponseMessage).FullName, () => { response.StatusCode = HttpStatusCode.OK; });
        }

        #endregion
    }
}
