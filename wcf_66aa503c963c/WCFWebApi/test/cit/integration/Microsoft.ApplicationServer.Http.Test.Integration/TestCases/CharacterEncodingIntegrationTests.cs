// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests exchanging data between client and server using default encoding (UTF-8), custom UTF-8 encoding, and UTF-16 encodings in all combinations.
    /// </summary>
    [TestClass]
    public class CharacterEncodingIntegrationTests
    {
        // Various URI segments identifying operations
        private const string responseOnly = "/responseonly";
        private const string requestOnly = "/requestonly";
        private const string requestResponse = "/requestresponse";
        private const string obj = "/object";

        private const string testServiceAddress = "http://localhost:8080/test";

        #region Helper Members

        public enum CharacterSet
        {
            None = 0,
            Utf8,
            Utf16
        }

        private static SampleData CreateSampleData()
        {
            return new SampleData
            {
                Name = "name",
                Age = 100,
                Collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }
            };
        }

        private static HttpServiceHost OpenHost(CharacterSet charset)
        {
            HttpConfiguration config = new HttpConfiguration();

            if (charset == CharacterSet.Utf8)
            {
                config.Formatters.XmlFormatter.CharacterEncoding = new UTF8Encoding(false, true);
                config.Formatters.JsonFormatter.CharacterEncoding = new UTF8Encoding(false, true);
            }
            else if (charset == CharacterSet.Utf16)
            {
                config.Formatters.XmlFormatter.CharacterEncoding = new UnicodeEncoding(false, true);
                config.Formatters.JsonFormatter.CharacterEncoding = new UnicodeEncoding(false, true);
            }

            HttpServiceHost host = new HttpServiceHost(typeof(CharacterEncodingIntegrationService), config, testServiceAddress);
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

        private static MediaTypeHeaderValue GetMediaType(bool asXml, CharacterSet charset)
        {
            MediaTypeHeaderValue mediatype = asXml ? new MediaTypeHeaderValue("application/xml") : new MediaTypeHeaderValue("application/json");
            if (charset == CharacterSet.Utf8)
            {
                mediatype.CharSet = Encoding.UTF8.WebName;
            }
            else if (charset == CharacterSet.Utf16)
            {
                mediatype.CharSet = Encoding.Unicode.WebName;
            }

            return mediatype;
        }

        private static void ValidateResponse(HttpResponseMessage response, bool readContent, bool asXml, CharacterSet charset)
        {
            MediaTypeHeaderValue mediatype = GetMediaType(asXml, charset);
            Assert.IsTrue(response.IsSuccessStatusCode, "status not successful");
            Assert.IsNotNull(response.Content, "response content should not be null");
            Assert.IsNotNull(response.Content.Headers.ContentType, "response content type should not be null");
            Assert.AreEqual(mediatype.MediaType, response.Content.Headers.ContentType.MediaType, "response content type doesn't match.");

            Encoding encoding = null;
            switch (charset)
            {
                case CharacterSet.None:
                    Assert.AreEqual(Encoding.UTF8.WebName, response.Content.Headers.ContentType.CharSet, "response charset should have been utf8");
                    encoding = new UTF8Encoding(false, true);
                    break;

                case CharacterSet.Utf8:
                    Assert.AreEqual(Encoding.UTF8.WebName, response.Content.Headers.ContentType.CharSet, "response charset should have been utf8");
                    encoding = new UTF8Encoding(false, true);
                    break;

                case CharacterSet.Utf16:
                    Assert.AreEqual(Encoding.Unicode.WebName, response.Content.Headers.ContentType.CharSet, "response charset should have been utf16");
                    encoding = new UnicodeEncoding(false, true);
                    break;
            }

            if (readContent)
            {
                byte[] byteReponse = response.Content.ReadAsByteArrayAsync().Result;
                string strResult = encoding.GetString(byteReponse);
                Assert.IsNotNull(strResult, "String result should not be null");

                SampleData content = response.Content.ReadAsAsync<SampleData>().Result;
                Assert.IsNotNull(content, "content should not be null");
                Assert.AreEqual("name", content.Name, "Unexpected name");
                Assert.AreEqual(100, content.Age, "Unexpected age");
            }
        }

        private static void SetCharset(ObjectContent content, bool asXml, CharacterSet charset)
        {
            if (asXml)
            {
                if (charset == CharacterSet.Utf8)
                {
                    content.Formatters.XmlFormatter.CharacterEncoding = Encoding.UTF8;
                }
                else if (charset == CharacterSet.Utf16)
                {
                    content.Formatters.XmlFormatter.CharacterEncoding = Encoding.Unicode;
                }
            }
            else
            {
                if (charset == CharacterSet.Utf8)
                {
                    content.Formatters.JsonFormatter.CharacterEncoding = Encoding.UTF8;
                }
                else if (charset == CharacterSet.Utf16)
                {
                    content.Formatters.JsonFormatter.CharacterEncoding = Encoding.Unicode;
                }
            }
        }

        private static void RunClient(bool asXml, CharacterSet requestCharset, CharacterSet responseCharset)
        {
            HttpClient client = new HttpClient();
            ObjectContent<SampleData> value;
            MediaTypeHeaderValue mediatype = GetMediaType(asXml, requestCharset);
            MediaTypeWithQualityHeaderValue accept = asXml ? 
                new MediaTypeWithQualityHeaderValue("application/xml") : new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(accept);

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + responseOnly).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), mediatype);
            SetCharset(value, asXml, requestCharset);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false, asXml, responseCharset);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), mediatype);
            SetCharset(value, asXml, requestCharset);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false, asXml, responseCharset);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + requestResponse).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), mediatype);
            SetCharset(value, asXml, requestCharset);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), mediatype);
            SetCharset(value, asXml, requestCharset);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            using (HttpResponseMessage response = client.GetAsync(testServiceAddress + obj).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), mediatype);
            SetCharset(value, asXml, requestCharset);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + obj, value).Result)
            {
                ValidateResponse(response, true, asXml, responseCharset);
            }

            client.Dispose();
        }

        #endregion

        #region Test Service

        [ServiceContract]
        public class CharacterEncodingIntegrationService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<SampleData> GetResponseOfSampleData()
            {
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(CreateSampleData());
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
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(CreateSampleData());
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
                return CreateSampleData();
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

            public List<int> Collection { get; set; }
        }

        #endregion

        #region Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        [Description("Runs XML test client with default encoding against server with default encoding.")]
        public void XmlEncodingRequestDefaultResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(true, CharacterSet.None, CharacterSet.None);
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
        [Description("Runs XML test client with custom UTF8 encoding against server with default encoding.")]
        public void XmlEncodingRequestUtf8ResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(true, CharacterSet.Utf8, CharacterSet.None);
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
        [Description("Runs XML test client with UTF 16 encoding against server with default encoding.")]
        public void XmlEncodingRequestUtf16ResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(true, CharacterSet.Utf16, CharacterSet.None);
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
        [Description("Runs XML test client with default encoding against server with custom UTF8 encoding.")]
        public void XmlEncodingRequestDefaultResponseUtf8IntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.Utf8);
                RunClient(true, CharacterSet.None, CharacterSet.Utf8);
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
        [Description("Runs XML test client with custom UTF8 encoding against server with UTF16 encoding.")]
        public void XmlEncodingRequestDefaultResponseUtf16IntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.Utf16);
                RunClient(true, CharacterSet.Utf8, CharacterSet.Utf16);
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
        [Description("Runs JSON test client with default encoding against server with default encoding.")]
        public void JsonEncodingRequestDefaultResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(false, CharacterSet.None, CharacterSet.None);
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
        [Description("Runs JSON test client with custom UTF8 encoding against server with default encoding.")]
        public void JsonEncodingRequestUtf8ResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(false, CharacterSet.Utf8, CharacterSet.None);
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
        [Description("Runs JSON test client with UTF 16 encoding against server with default encoding.")]
        public void JsonEncodingRequestUtf16ResponseDefaultIntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.None);
                RunClient(false, CharacterSet.Utf16, CharacterSet.None);
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
        [Description("Runs JSON test client with default encoding against server with custom UTF8 encoding.")]
        public void JsonEncodingRequestDefaultResponseUtf8IntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.Utf8);
                RunClient(false, CharacterSet.None, CharacterSet.Utf8);
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
        [Description("Runs JSON test client with custom UTF8 encoding against server with UTF16 encoding.")]
        public void JsonEncodingRequestDefaultResponseUtf16IntegrationTest()
        {
            HttpServiceHost host = null;
            try
            {
                host = OpenHost(CharacterSet.Utf16);
                RunClient(false, CharacterSet.Utf8, CharacterSet.Utf16);
            }
            finally
            {
                CloseHost(host);
            }
        }

        #endregion
    }
}