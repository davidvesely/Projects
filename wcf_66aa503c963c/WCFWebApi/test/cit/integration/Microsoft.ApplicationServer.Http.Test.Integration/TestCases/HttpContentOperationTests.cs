// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml.Serialization;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpContentOperationTests
    {
        private static int portNumber = 2000;
        private const int timeout = 30 * 1000;

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("Demonstrates a service operation returning an HttpContent.")]
        public void GetHttpContent()
        {
            string baseAddress = string.Format("http://localhost:{0}", GetNextPortNumber());

            using (HttpServiceHost host = new HttpServiceHost(typeof(HttpContentService), baseAddress))
            {
                host.AddDefaultEndpoints();
                host.Open();

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(baseAddress + "/GetHttpContent/5").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string value = response.Content.ReadAsStringAsync().Result;
                        Assert.AreEqual("5", value, "Value was not correct.");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "status code not ok");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("Demonstrates a service operation returning an ObjectContent.")]
        public void GetObjectContent()
        {
            string baseAddress = string.Format("http://localhost:{0}", GetNextPortNumber());

            using (HttpServiceHost host = new HttpServiceHost(typeof(HttpContentService), baseAddress))
            {
                host.AddDefaultEndpoints();
                host.Open();

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(baseAddress + "/GetObjectContent/5").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        int value = response.Content.ReadAsAsync<int>().Result;
                        Assert.AreEqual(5, value, "Value was not correct.");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "status code not ok");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("Demonstrates a service operation returning an ObjectContent<>.")]
        public void GetObjectContentOfT()
        {
            string baseAddress = string.Format("http://localhost:{0}", GetNextPortNumber());

            using (HttpServiceHost host = new HttpServiceHost(typeof(HttpContentService), baseAddress))
            {
                host.AddDefaultEndpoints();
                host.Open();

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(baseAddress + "/GetObjectContentOfT/5").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        int value = response.Content.ReadAsAsync<int>().Result;
                        Assert.AreEqual(5, value, "Value was not correct.");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "status code not ok");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("Demonstrates a service operation posting and returning an HttpContent.")]
        public void PostHttpContent()
        {
            string baseAddress = string.Format("http://localhost:{0}", GetNextPortNumber());

            using (HttpServiceHost host = new HttpServiceHost(typeof(HttpContentService), baseAddress))
            {
                host.AddDefaultEndpoints();
                host.Open();

                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage<string> request = new HttpRequestMessage<string>("hello")
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(baseAddress + "/PostHttpContent")
                    })
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                        using (HttpResponseMessage response = client.SendAsync(request).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            string result = response.Content.ReadAsAsync<string>().Result;
                            Assert.AreEqual("hello", result, "Value was not correct.");
                            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "status code not ok");
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("Demonstrates a service operation posting and returning an ObjectContent<T>.")]
        public void PostObjectContentOfT()
        {
            string baseAddress = string.Format("http://localhost:{0}", GetNextPortNumber());

            using (HttpServiceHost host = new HttpServiceHost(typeof(HttpContentService), baseAddress))
            {
                host.AddDefaultEndpoints();
                host.Open();

                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage<int> request = new HttpRequestMessage<int>(5)
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(baseAddress + "/PostObjectContentOfT")
                    })
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                        using (HttpResponseMessage response = client.SendAsync(request).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            int result = response.Content.ReadAsAsync<int>().Result;
                            Assert.AreEqual(5, result, "Value was not correct.");
                            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "status code not ok");
                        }
                    }
                }
            }
        }

        private static int GetNextPortNumber()
        {
            return Interlocked.Increment(ref portNumber);
        }
    }
}
