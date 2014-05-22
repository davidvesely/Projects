// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [Flags]
    public enum TestHeaderOptions
    {
        None = 0x00,
        InsertRequest = 0x01,
        ValidateResponse = 0x02
    }

    public static class TestServiceClient
    {
        public static HttpClient CreateClient()
        {
            return TestServiceClient.CreateClient(true);
        }

        public static HttpClient CreateClient(bool allowAutoRedirect)
        {
            return CreateClient(allowAutoRedirect, TestServiceCommon.ServiceAddress, null);
        }

        public static HttpClient CreateClient(bool allowAutoRedirect, Uri serviceAddress)
        {
            return CreateClient(allowAutoRedirect, serviceAddress, null);
        }

        public static HttpClient CreateClient(bool allowAutoRedirect, Uri serviceAddress, ICredentials credentials)
        {
            WebRequestHandler webRequestHandler = new WebRequestHandler { AllowAutoRedirect = allowAutoRedirect };
            if (credentials != null)
            {
                webRequestHandler.Credentials = credentials;
            }

            var client = new HttpClient(webRequestHandler);
            client.BaseAddress = serviceAddress;
            return client;
        }

        public static ICollection<HttpResponseMessage> RunClient(HttpClient client, TestHeaderOptions options)
        {
            return TestServiceClient.RunClient(client, options, HttpMethod.Get);
        }
        
        public static ICollection<HttpResponseMessage> RunClient(HttpClient client, TestHeaderOptions options, HttpMethod method)
        {
            var result = new HttpResponseMessage[TestServiceCommon.Iterations];
            for (var cnt = 0; cnt < TestServiceCommon.Iterations; cnt++)
            {
                var httpRequest = new HttpRequestMessage(method, "");
                if ((options & TestHeaderOptions.InsertRequest) > 0)
                {
                    TestServiceCommon.AddRequestHeader(httpRequest, cnt);
                }

                try
                {
                    result[cnt] = client.SendAsync(httpRequest).Result;
                    Assert.IsNotNull(result[cnt]);
                }
                catch (HttpException he)
                {
                    var we = he.InnerException as WebException;
                    Assert.IsNull(we.Response, "Response should not be null.");
                    continue;
                }

                if ((options & TestHeaderOptions.ValidateResponse) > 0)
                {
                    TestServiceCommon.ValidateResponseTestHeader(result[cnt], cnt);
                }
            }

            Assert.AreEqual(TestServiceCommon.Iterations, result.Length);
            return result;
        }

        public static ICollection<HttpResponseMessage> RunClient(HttpClient client, TestHeaderOptions options, TimeSpan timeout)
        {
            var result = new HttpResponseMessage[TestServiceCommon.Iterations];
            using (var timer = new Timer(TestServiceClient.TimeoutHandler, client, timeout, timeout))
            {
                for (var cnt = 0; cnt < TestServiceCommon.Iterations; cnt++)
                {
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "");
                    if ((options & TestHeaderOptions.InsertRequest) > 0)
                    {
                        TestServiceCommon.AddRequestHeader(httpRequest, cnt);
                    }

                    try
                    {
                        result[cnt] = client.SendAsync(httpRequest).Result;
                        Assert.IsNotNull(result[cnt]);
                    }
                    catch (OperationCanceledException)
                    {
                        continue;
                    }
                    catch (AggregateException aggregateException)
                    {
                        if (aggregateException.GetBaseException() is OperationCanceledException)
                        {
                            continue;
                        }
                    }

                    if ((options & TestHeaderOptions.ValidateResponse) > 0)
                    {
                        TestServiceCommon.ValidateResponseTestHeader(result[cnt], cnt);
                    }
                }
            }

            Assert.AreEqual(TestServiceCommon.Iterations, result.Length);
            return result;
        }

        private static void TimeoutHandler(object state)
        {
            var client = state as HttpClient;
            try
            {
                client.CancelPendingRequests();
            }
            catch { }
        }
    }
}
