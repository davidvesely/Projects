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
    public class RequestHeaderMappingIntegrationTests
    {
        // Various URI segments identifying operations
        private const string responseOnly = "/responseonly";
        private const string requestOnly = "/requestonly";
        private const string requestResponse = "/requestresponse";
        private const string obj = "/object";

        private const string testJsonValueServiceAddress = "http://localhost:8080/jsonvalue";
        private const string testJsonServiceAddress = "http://localhost:8080/json";

        #region Helper Members

        private static JsonValue CreateSampleJsonValue()
        {
            return new JsonPrimitive("hello world");
        }

        private static SampleData CreateSampleObject()
        {
            return new SampleData { Age = 1, Name = "1" };
        }

        private static HttpServiceHost OpenHost<TService>(string serviceAddress, bool streamed) where TService : class
        {
            HttpConfiguration config = new HttpConfiguration();
            if (streamed)
            {
                config.TransferMode = TransferMode.Streamed;
            }
            else
            {
                config.TransferMode = TransferMode.Buffered;
            }

            HttpServiceHost host = new HttpServiceHost(typeof(TService), config, serviceAddress);
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

        private static void ValidateResponse(HttpResponseMessage response, bool readContent, bool isContentObject)
        {
            Assert.IsTrue(response.IsSuccessStatusCode, "Expected status code 2xx but received " + response.StatusCode);
            Assert.IsNotNull(response.Content, "response content should not be null");
            Assert.IsNotNull(response.Content.Headers.ContentType, "response content type should not be null");
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType, "response content type should be application/json");
            if (readContent)
            {
                if (isContentObject)
                {
                    SampleData content = response.Content.ReadAsAsync<SampleData>().Result;
                    Assert.IsNotNull(content, "content should not be null");
                }
                else
                {
                    JsonValue content = response.Content.ReadAsAsync<JsonValue>().Result;
                    Assert.IsNotNull(content, "content should not be null");
                }
            }
        }

        private static void RunJsonValueClient(string serviceAddress)
        {
            ObjectContent<JsonValue> value;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Requested-With", "XmlHttpRequest");

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + responseOnly).Result)
            {
                ValidateResponse(response, true, false);
            }

            value = new ObjectContent<JsonValue>(CreateSampleJsonValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false, false);
            }

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + requestResponse).Result)
            {
                ValidateResponse(response, true, false);
            }

            value = new ObjectContent<JsonValue>(CreateSampleJsonValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true, false);
            }

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + obj).Result)
            {
                ValidateResponse(response, true, false);
            }

            value = new ObjectContent<JsonValue>(CreateSampleJsonValue(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + obj, value).Result)
            {
                ValidateResponse(response, true, false);
            }

            client.Dispose();
        }

        private static void RunJsonClient(string serviceAddress)
        {
            ObjectContent<SampleData> value;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Requested-With", "XmlHttpRequest");

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + responseOnly).Result)
            {
                ValidateResponse(response, true, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleObject(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false, true);
            }

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + requestResponse).Result)
            {
                ValidateResponse(response, true, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleObject(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true, true);
            }

            using (HttpResponseMessage response = client.GetAsync(serviceAddress + obj).Result)
            {
                ValidateResponse(response, true, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleObject(), "application/json");
            using (HttpResponseMessage response = client.PostAsync(serviceAddress + obj, value).Result)
            {
                ValidateResponse(response, true, true);
            }

            client.Dispose();
        }

        #endregion

        #region Test Services

        [ServiceContract]
        public class JsonValueSampleService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<JsonValue> GetResponseOfJsonValue()
            {
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateSampleJsonValue());
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
                HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(CreateSampleJsonValue());
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

            [WebGet(UriTemplate = obj)]
            public JsonValue GetJsonValue()
            {
                return CreateSampleJsonValue();
            }

            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public JsonValue PostJsonValue(JsonValue input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }
        }

        [ServiceContract]
        public class JsonSampleService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<SampleData> GetResponseOfSampleData()
            {
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(CreateSampleObject());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<SampleData>), "Content not of expected type.");
                return response;
            }

            [WebInvoke(UriTemplate = requestOnly, Method = "POST")]
            public bool PostRequestOfSampleData(HttpRequestMessage<SampleData> request)
            {
                SampleData input = request.Content.ReadAsAsync<SampleData>().Result;
                Assert.IsNotNull(input, "unexpected ValueType");
                return true;
            }

            [WebGet(UriTemplate = requestResponse)]
            public HttpResponseMessage<SampleData> GetRequestResponseOfSampleData(HttpRequestMessage request)
            {
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(CreateSampleObject());
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<SampleData>), "Content not of expected type.");
                response.RequestMessage = request;
                return response;
            }

            [WebInvoke(UriTemplate = requestResponse, Method = "POST")]
            public HttpResponseMessage<SampleData> PostRequestResponseOfSampleData(HttpRequestMessage<SampleData> request)
            {
                Assert.IsNotNull(request, "request should not be null");
                SampleData input = request.Content.ReadAsAsync<SampleData>().Result;
                Assert.IsNotNull(input, "unexpected ValueType");
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(input);
                Assert.IsNotNull(response, "response should not be null");
                Assert.IsNotNull(response.Content, "content should not be null");
                Assert.IsInstanceOfType(response.Content, typeof(ObjectContent<SampleData>), "Content not of expected type.");
                response.RequestMessage = request;
                return response;
            }

            [WebGet(UriTemplate = obj)]
            public SampleData GetSampleData()
            {
                return CreateSampleObject();
            }

            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public SampleData PostSampleData(SampleData input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }
        }

        public class SampleData
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        #endregion

        #region Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        public void RequestHeaderMappingJsonValueIntegrationTestStreamed()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost<JsonValueSampleService>(testJsonValueServiceAddress, true);
                RunJsonValueClient(testJsonValueServiceAddress);
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
        public void RequestHeaderMappingJsonValueIntegrationTestBuffered()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost<JsonValueSampleService>(testJsonValueServiceAddress, false);
                RunJsonValueClient(testJsonValueServiceAddress);
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
        public void RequestHeaderMappingJsonIntegrationTestStreamed()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost<JsonSampleService>(testJsonServiceAddress, true);
                RunJsonClient(testJsonServiceAddress);
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
        public void RequestHeaderMappingJsonIntegrationTestBuffered()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost<JsonSampleService>(testJsonServiceAddress, false);
                RunJsonClient(testJsonServiceAddress);
            }
            finally
            {
                CloseHost(host);
            }
        }

        #endregion
    }
}