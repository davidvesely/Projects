// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FormUrlEncodedIntegrationTests
    {
        private const string urlFormEncodedMediaType = "application/x-www-form-urlencoded";

        // Various URI segments identifying operations
        private const string obj = "/object";
        private const string jsonValue = "/jsonValue";
        
        private const string testServiceAddress = "http://localhost:8080/test";

        #region Helper Members

        private static string CreateValue()
        {
            return "Name=Some+Name&Age=21&Phone=123-456-7890&Address=1234+Main+St.";
        }

        private static HttpServiceHost OpenHost(bool streamed)
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

        private static void ValidateResponse(HttpResponseMessage response)
        {
            Assert.IsTrue(response.IsSuccessStatusCode, "status not successful");
            Assert.IsNotNull(response.Content, "response content should not be null");
            Assert.IsNotNull(response.Content.Headers.ContentType, "response content type should not be null");
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType, "response content type should be application/json");

            Person person = response.Content.ReadAsAsync<Person>().Result;
            Assert.AreEqual("Some Name", person.Name);
            Assert.AreEqual(21, person.Age);
            Assert.AreEqual("123-456-7890", person.Phone);
            Assert.AreEqual("1234 Main St.", person.Address);
        }

        private static void RunClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent content;

            content = new StringContent(CreateValue(), Encoding.UTF8, urlFormEncodedMediaType);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + obj, content).Result)
            {
                ValidateResponse(response);
            }

            content = new StringContent(CreateValue(), Encoding.UTF8, urlFormEncodedMediaType);
            using (HttpResponseMessage response = client.PostAsync(testServiceAddress + obj, content).Result)
            {
                ValidateResponse(response);
            }

            client.Dispose();
        }

        #endregion

        #region Test Service

        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Phone { get; set; }

            public string Address { get; set; }
        }

        [ServiceContract]
        public class JsonValueIntegrationService
        {
            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public Person PostObject(Person input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }

            [WebInvoke(UriTemplate = jsonValue, Method = "POST")]
            public Person PostJsonValue(JsonValue input)
            {
                Person person = input.ReadAsType<Person>();
                Assert.IsNotNull(input, "input should not be null");
                return person;
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TestServiceCommon.DefaultTimeout), Owner("derik")]
        public void BasicUrlFormEncodedTestStreamed()
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
        [TestCategory("CIT"), Timeout(TestServiceCommon.DefaultTimeout), Owner("derik")]
        public void BasicUrlFormEncodedTestBuffered()
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

        #endregion
    }
}