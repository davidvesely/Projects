// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Json;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonValueIntegrationTests
    {
        // Various URI segments identifying operations
        private const string responseOnly = "/responseonly";
        private const string bigResponseOnly = "/bigresponseonly";
        private const string requestOnly = "/requestonly";
        private const string requestResponse = "/requestresponse";
        private const string bigRequestResponse = "/bigrequestresponse";
        private const string obj = "/object";
        private const string bigObj = "/bigobject";

        private const int maxSize = 4 * 1024 * 1024;
        private const string testServiceAddress = "http://localhost:8080/test";

        #region Helper Members

        private static JsonValue CreateSmallValue()
        {
            return new JsonPrimitive("hello world");
        }

        private static JsonValue CreateBigValue()
        {
            int size = 32 * 1024;
            JsonPrimitive[] hello = new JsonPrimitive[size];
            for (int index = 0; index < size; index++)
            {
                hello[index] = new JsonPrimitive("hello world hello world hello world hello world hello world hello world hello world");
            }

            return new JsonArray(hello);
        }

        private static HttpServiceHost OpenHost(bool streamed)
        {
            HttpConfiguration config = new HttpConfiguration();
            if (streamed)
            {
                config.TransferMode = TransferMode.Streamed;
                config.MaxBufferSize = 8 * 1024;
                config.MaxReceivedMessageSize = maxSize;
            }
            else
            {
                config.TransferMode = TransferMode.Buffered;
                config.MaxBufferSize = maxSize;
                config.MaxReceivedMessageSize = maxSize;
            }

            HttpServiceHost host = new HttpServiceHost(typeof(JsonValueIntegrationService), config, testServiceAddress);
            host.Open();
            return host;
        }

        private static void CloseHost(HttpServiceHost host)
        {
            if (host != null)
            {
                try
                {
                    host.Close();
                }
                catch
                {
                    host.Abort();
                }
            }
        }

        private static void ValidateResponse(HttpResponseMessage response, bool readContent)
        {
            Assert.IsTrue(response.IsSuccessStatusCode, "status not successful");
            Assert.IsNotNull(response.Content, "response content should not be null");
            Assert.IsNotNull(response.Content.Headers.ContentType, "response content type should not be null");
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType, "response content type should be application/json");
            if (readContent)
            {
                JsonValue content = response.Content.ReadAsAsync<JsonValue>().Result;
                Assert.IsNotNull(content, "content should not be null");
            }
        }

        private static void RunClient()
        {
            ObjectContent<JsonValue> value;

            WebRequestHandler handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = maxSize;
            HttpClient client = new HttpClient(handler);
            client.MaxResponseContentBufferSize = maxSize;

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + responseOnly).Result)
            {
                ValidateResponse(response, true);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + bigResponseOnly).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<JsonValue>(CreateSmallValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false);
            }

            value = new ObjectContent<JsonValue>(CreateBigValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + requestResponse).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<JsonValue>(CreateSmallValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<JsonValue>(CreateBigValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + bigRequestResponse).Result)
            {
                ValidateResponse(response, true);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + obj).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<JsonValue>(CreateSmallValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + obj, value).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<JsonValue>(CreateBigValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + obj, value).Result)
            {
                ValidateResponse(response, true);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + bigObj).Result)
            {
                ValidateResponse(response, true);
            }

            client.Dispose();
        }

        #endregion

        #region Test Service

        [ServiceContract]
        public class JsonValueIntegrationService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<JsonValue> GetResponseOfJsonValue()
            {
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateSmallValue());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<JsonValue>), "Content not of expected type.");
                return response;
            }

            [WebGet(UriTemplate = bigResponseOnly)]
            public HttpResponseMessage<JsonValue> GetBigResponseOfJsonValue()
            {
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateBigValue());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<JsonValue>), "Content not of expected type.");
                return response;
            }

            [WebInvoke(UriTemplate = requestOnly, Method = "POST")]
            public bool PostRequestOfJsonValue(HttpRequestMessage<JsonValue> request)
            {
                JsonValue input = request.Content.ReadAsAsync<JsonValue>().Result;
                Assert.IsNotNull(input, "unexpected ValueType");
                return true;
            }

            [WebGet(UriTemplate = requestResponse)]
            public HttpResponseMessage<JsonValue> GetRequestResponseOfJsonValue(HttpRequestMessage request)
            {
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateSmallValue());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<JsonValue>), "Content not of expected type.");
                response.RequestMessage = request;
                return response;
            }

            [WebInvoke(UriTemplate = requestResponse, Method = "POST")]
            public HttpResponseMessage<JsonValue> PostRequestResponseOfJsonValue(HttpRequestMessage<JsonValue> request)
            {
                Assert.IsNotNull(request, "request should not be null");
                JsonValue input = request.Content.ReadAsAsync<JsonValue>().Result;
                Assert.IsNotNull(input, "unexpected ValueType");
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(input);
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<JsonValue>), "Content not of expected type.");
                response.RequestMessage = request;
                return response;
            }

            [WebGet(UriTemplate = bigRequestResponse)]
            public HttpResponseMessage<JsonValue> GetRequestResponseOfBigJsonValue(HttpRequestMessage request)
            {
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateBigValue());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<JsonValue>), "Content not of expected type.");
                response.RequestMessage = request;
                return response;
            }

            [WebGet(UriTemplate = obj)]
            public JsonValue GetJsonValue()
            {
                return CreateSmallValue();
            }

            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public JsonValue PostJsonValue(JsonValue input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }

            [WebGet(UriTemplate = bigObj)]
            public JsonValue GetBigJsonValue()
            {
                return CreateBigValue();
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        public void BasicJsonValueIntegrationTestStreamed()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(true);
                RunClient();
            }
            finally
            {
                CloseHost(host);
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        public void BasicJsonValueIntegrationTestBuffered()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(false);
                RunClient();
            }
            finally
            {
                CloseHost(host);
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        public void SingleFormatterSelection()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.Clear();
            config.Formatters.Add(new JsonValueMediaTypeFormatter());
            HttpServiceHost host = new HttpServiceHost(typeof(JsonValueIntegrationService), config, testServiceAddress);

            try
            {
                host = OpenHost(false);
                HttpClient client = new HttpClient();
                using (HttpResponseMessage response = client.GetAsync(testServiceAddress + responseOnly).Result)
                {
                    ValidateResponse(response, true);
                }
            }
            finally
            {
                CloseHost(host);
            }
        }

        #endregion
    }
}