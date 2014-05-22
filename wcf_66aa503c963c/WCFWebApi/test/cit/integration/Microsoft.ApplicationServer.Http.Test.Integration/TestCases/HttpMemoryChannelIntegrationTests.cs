// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon.Base;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="HttpMemory"/>exchanging data between client and server using default encoding (UTF-8), custom UTF-8 encoding, and UTF-16 encodings in all combinations.
    /// </summary>
    [TestClass]
    public class HttpMemoryChannelIntegrationTests
    {
        // Various URI segments identifying operations
        private const string responseOnly = "/responseonly";
        private const string requestOnly = "/requestonly";
        private const string requestResponse = "/requestresponse";
        private const string obj = "/object";

        // Number of requests we send to the server
        private const int iterations = 64;

        private const string localServiceAddress = "http://localhost:8080/test";
        private const string memoryServiceAddress = "http://memoryhost/test";

        private static ConcurrentBag<SampleData> sampleDataCollection = null;

        #region Helper Members

        private enum EndpointConfiguration
        {
            MemoryOnly = 0,
            DefaultEndpointsBeforeMemoryEndpoint,
            DefaultEndpointsAfterMemoryEndpoint,
        }

        private static SampleData CreateSampleData()
        {
            return new SampleData
            {
                Name = "name",
                Age = 100,
            };
        }

        private static void RunDualHostTest<TService>()
        {
            HttpServiceHost host = null;
            try
            {
                sampleDataCollection = new ConcurrentBag<SampleData>();

                HttpMemoryHandler memoryHandler;
                host = OpenEndpointHost<TService>(EndpointConfiguration.DefaultEndpointsBeforeMemoryEndpoint, out memoryHandler);
                RunMemoryHostClient(memoryHandler);

                // Verify that sample data produced by service has *NOT* been disposed
                foreach (SampleData sampleData in sampleDataCollection)
                {
                    Assert.IsFalse(sampleData.IsDisposed, "content should not have been disposed.");
                }

                sampleDataCollection = new ConcurrentBag<SampleData>();

                RunLocalHostClient();

                // Verify that sample data produced by service *HAS* been disposed
                foreach (SampleData sampleData in sampleDataCollection)
                {
                    Assert.IsTrue(sampleData.IsDisposed, "content should have been disposed.");
                }
            }
            finally
            {
                CloseHost(host);
            }
        }

        private static void RunSingleHostTest<TService>()
        {
            HttpServiceHost host = null;
            try
            {
                sampleDataCollection = new ConcurrentBag<SampleData>();

                HttpMemoryHandler memoryHandler;
                host = OpenEndpointHost<TService>(EndpointConfiguration.MemoryOnly, out memoryHandler);
                RunMemoryHostClient(memoryHandler);

                // Verify that sample data produced by service has *NOT* been disposed
                foreach (SampleData sampleData in sampleDataCollection)
                {
                    Assert.IsFalse(sampleData.IsDisposed, "content should not have been disposed.");
                }
            }
            finally
            {
                CloseHost(host);
            }
        }

        private static HttpServiceHost OpenEndpointHost<TService>(EndpointConfiguration endpointConfig, out HttpMemoryHandler memoryHandler)
        {
            HttpServiceHost host = new HttpServiceHost(typeof(TService), localServiceAddress);

            if (endpointConfig == EndpointConfiguration.DefaultEndpointsBeforeMemoryEndpoint)
            {
                // Add default endpoints before memory endpoint
                host.AddDefaultEndpoints();
            }

            // Create memory configuration
            HttpMemoryConfiguration memoryConfiguration = new HttpMemoryConfiguration();
            memoryConfiguration.MessageHandlers.Add(typeof(SampleMessageHandler));

            // Create memory endpoint
            HttpMemoryEndpoint memoryEndpoint = host.AddHttpMemoryEndpoint(typeof(TService), memoryServiceAddress, memoryConfiguration);

            if (endpointConfig == EndpointConfiguration.DefaultEndpointsAfterMemoryEndpoint)
            {
                // Add default endpoints after memory endpoint
                host.AddDefaultEndpoints();
            }

            // Open host
            host.Open();

            // Verify that we have the right number of endpoints
            if (endpointConfig == EndpointConfiguration.MemoryOnly)
            {
                Assert.AreEqual(1, host.Description.Endpoints.Count, "Host should only have one endpoint -- no default endpoints should have been added");
            }
            else
            {
                Assert.AreEqual(2, host.Description.Endpoints.Count, "Host should have two endpoints one of which is the default endpoint");
            }

            // Get HttpMemoryHandler from HttpMemoryEndpoint
            memoryHandler = memoryEndpoint.GetHttpMemoryHandler();

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

            if (readContent)
            {
                SampleData content = response.Content.ReadAsAsync<SampleData>().Result;
                Assert.IsNotNull(content, "content should not be null");
                Assert.AreEqual("name", content.Name, "Unexpected name");
                Assert.AreEqual(100, content.Age, "Unexpected age");
            }
        }

        private static void RunMemoryHostClient(HttpMemoryHandler memoryHandler)
        {
            HttpClient client = new HttpClient(memoryHandler);
            Task<HttpResponseMessage>[] tasks = new Task<HttpResponseMessage>[iterations];

            // Send GET requests against 'responseOnly'
            for (int index = 0; index < iterations; index++)
            {
                tasks[index] = client.GetAsync(memoryServiceAddress + responseOnly);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, true);
            }

            // TODO: Pending CSDMain: 236230: PUT and POST requests using ObjectContent<T> on HttpMemoryEndpoint doesn't work 
#if false
            ObjectContent<SampleData> value;

            // Send POST requests against 'requestOnly'
            for (int index = 0; index < iterations; index++)
            {
                value = new ObjectContent<SampleData>(CreateSampleData());
                tasks[index] = client.PostAsync(memoryServiceAddress + requestOnly, value);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, false);
            }

            value = new ObjectContent<SampleData>(CreateSampleData());
            using (HttpResponseMessage response = client.PostAsync(memoryServiceAddress + requestOnly, value).Result)
            {
                ValidateResponse(response, false);
            }

            using (HttpResponseMessage response = client.GetAsync(memoryServiceAddress + requestResponse).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleData());
            using (HttpResponseMessage response = client.PostAsync(memoryServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleData());
            using (HttpResponseMessage response = client.PostAsync(memoryServiceAddress + requestResponse, value).Result)
            {
                ValidateResponse(response, true);
            }
#endif

            // Send GET requests against 'obj'
            for (int index = 0; index < iterations; index++)
            {
                tasks[index] = client.GetAsync(memoryServiceAddress + obj);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, true);
            }

            // TODO: Pending CSDMain: 236230: PUT and POST requests using ObjectContent<T> on HttpMemoryEndpoint doesn't work 
#if false
            // Send POST requests against 'obj'
            for (int index = 0; index < iterations; index++)
            {
                value = new ObjectContent<SampleData>(CreateSampleData());
                tasks[index] = client.PostAsync(memoryServiceAddress + obj, value);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, true);
            }
#endif

            client.Dispose();
        }

        private static void RunLocalHostClient()
        {
            HttpClient client = new HttpClient();
            ObjectContent<SampleData> value;
            Task<HttpResponseMessage>[] tasks = new Task<HttpResponseMessage>[iterations];

            // TODO: Pending CSDMain: 236228: Disposable content carried in ObjectContent is not disposed when sending HTTP responses on server side
#if false
            // Send GET requests against 'responseOnly'
            for (int index = 0; index < iterations; index++)
            {
                tasks[index] = client.GetAsync(localServiceAddress + responseOnly);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateLocalResponse(task.Result, true);
            }

            // Send POST requests against 'requestOnly'
            for (int index = 0; index < iterations; index++)
            {
                value = new ObjectContent<SampleData>(CreateSampleData(), "application/xml");
                tasks[index] = client.PostAsync(localServiceAddress + requestOnly, value);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateLocalResponse(task.Result, false);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), "application/xml");
            using (HttpResponseMessage response = client.PostAsync(localServiceAddress + requestOnly, value).Result)
            {
                ValidateLocalResponse(response, false);
            }

            using (HttpResponseMessage response = client.GetAsync(localServiceAddress + requestResponse).Result)
            {
                ValidateLocalResponse(response, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), "application/xml");
            using (HttpResponseMessage response = client.PostAsync(localServiceAddress + requestResponse, value).Result)
            {
                ValidateLocalResponse(response, true);
            }

            value = new ObjectContent<SampleData>(CreateSampleData(), "application/xml");
            using (HttpResponseMessage response = client.PostAsync(localServiceAddress + requestResponse, value).Result)
            {
                ValidateLocalResponse(response, true);
            }
#endif

            // Send GET requests against 'obj'
            for (int index = 0; index < iterations; index++)
            {
                tasks[index] = client.GetAsync(localServiceAddress + obj);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, true);
            }

            // Send POST requests against 'obj'
            for (int index = 0; index < iterations; index++)
            {
                value = new ObjectContent<SampleData>(CreateSampleData(), "application/xml");
                tasks[index] = client.PostAsync(localServiceAddress + obj, value);
            }

            Task.WaitAll(tasks);
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                ValidateResponse(task.Result, true);
            }

            client.Dispose();
        }

        #endregion

        #region Test Services

        public class PocoSampleService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<SampleData> GetResponseOfSampleData()
            {
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(sampleData);
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
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(sampleData);
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
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                return sampleData;
            }

            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public SampleData PostSampleData(SampleData input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }
        }

        [ServiceContract]
        public class SampleService
        {
            [WebGet(UriTemplate = responseOnly)]
            public HttpResponseMessage<SampleData> GetResponseOfSampleData()
            {
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(sampleData);
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
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                HttpResponseMessage<SampleData> response = new HttpResponseMessage<SampleData>(sampleData);
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
                SampleData sampleData = CreateSampleData();
                sampleDataCollection.Add(sampleData);
                return sampleData;
            }

            [WebInvoke(UriTemplate = obj, Method = "POST")]
            public SampleData PostSampleData(SampleData input)
            {
                Assert.IsNotNull(input, "input should not be null");
                return input;
            }
        }

        public class SampleMessageHandler : DelegatingHandler
        {
            private static int offset;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var requestHeaderValue = Interlocked.Increment(ref offset);
                request.Headers.Add("RequestHeader", requestHeaderValue.ToString());

                return base.SendAsync(request, cancellationToken).ContinueWith(
                    (task) =>
                    {
                        var responseHeaderValue = Interlocked.Increment(ref offset);
                        HttpResponseMessage response = task.Result;
                        response.Headers.Add("ResponseHeader", responseHeaderValue.ToString());
                        return response;
                    });
            }
        }

        public class SampleData : IDisposable
        {
            private bool isDisposed;
            private string name;
            private int age;

            public string Name
            {
                get
                {
                    CheckDisposed();
                    return this.name;
                }

                set
                {
                    CheckDisposed();
                    this.name = value;
                }
            }

            public int Age
            {
                get
                {
                    CheckDisposed();
                    return this.age;
                }

                set
                {
                    CheckDisposed();
                    this.age = value;
                }
            }

            public void Dispose()
            {
                this.isDisposed = true;
            }

            public bool IsDisposed
            {
                get
                {
                    return this.isDisposed;
                }
            }

            private void CheckDisposed()
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("contact");
                }
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        [Description("Runs client against server using host with memory endpoint and regular endpoint.")]
        public void HttpMemoryChannelDualEndpointIntegrationTest()
        {
            RunDualHostTest<PocoSampleService>();

            RunDualHostTest<SampleService>();
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        [Description("Runs client against server using host with memory endpoint and regular endpoint in the wrong order.")]
        public void HttpMemoryChannelDualEndpointInvalidOrderIntegrationTest()
        {
            UnitTest.Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    HttpMemoryHandler memoryHandler;
                    OpenEndpointHost<PocoSampleService>(EndpointConfiguration.DefaultEndpointsAfterMemoryEndpoint, out memoryHandler);
                },
                (exception) =>
                {
                });

            UnitTest.Asserters.Exception.Throws<InvalidOperationException>(
            () =>
            {
                HttpMemoryHandler memoryHandler;
                OpenEndpointHost<SampleService>(EndpointConfiguration.DefaultEndpointsAfterMemoryEndpoint, out memoryHandler);
            },
            (exception) =>
            {
            });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TestServiceCommon.DefaultTimeout)]
        [Owner("derik")]
        [Description("Runs client against server using host with only memory endpoint.")]
        public void HttpMemoryChannelSingleEndpointIntegrationTest()
        {
            RunSingleHostTest<PocoSampleService>();

            RunSingleHostTest<SampleService>();
        }

        #endregion Tests
    }
}