// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Formatting.Test.Sample
{
    using System.Net.Http;
    using HttpMessageContent.Sample;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.Base;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests for <see cref="HttpMessageContentSample"/> sample.
    /// </summary>
    [TestClass]
    public class HttpMessageContentSampleTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent request parsing scenario works correctly.")]
        public void HttpMessageContent_RequestTest()
        {
            string content = Program.HttpMessageContentRequest();
            UnitTest.Asserters.String.Contains(content, "This is a sample request body", "Request does not contain expected body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent response parsing scenario works correctly.")]
        public void HttpMessageContent_ResponseTest()
        {
            string content = Program.HttpMessageContentResponse();
            UnitTest.Asserters.String.Contains(content, "This is a sample response body", "Response does not contain expected body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent object response parsing scenario works correctly.")]
        public void HttpMessageContent_ObjectContent_ResponseTest()
        {
            string content = Program.HttpMessageObjectContentResponse();
            UnitTest.Asserters.String.Contains(content, "aaabbbccc", "Response does not contain expected body content.");
            UnitTest.Asserters.String.Contains(content, "AAABBBCCC", "Response does not contain expected body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent MIME multipart parsing scenario works correctly.")]
        public void HttpContentMessage_MultipartTest()
        {
            string content = Program.MultipartHttpMessageContent();
            UnitTest.Asserters.String.Contains(content, "This is a sample request body", "Multipart content does not contain expected request body content.");
            UnitTest.Asserters.String.Contains(content, "This is a sample response body", "Multipart content does not contain expected response body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent read as HttpRequestMessage scenario works correctly.")]
        public void HttpContentMessage_ReadContentAsHttpRequestMessageTest()
        {
            HttpRequestMessage request = Program.ReadContentAsHttpRequestMessage();
            Assert.IsNotNull(request, "request should not be null");
            string content = request.Content.ReadAsStringAsync().Result;
            UnitTest.Asserters.String.Contains(content, "This is a sample request body", "Request does not contain expected body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent read as HttpResponseMessage scenario works correctly.")]
        public void HttpContentMessage_ReadContentAsHttpResponseMessageTest()
        {
            HttpResponseMessage response = Program.ReadContentAsHttpResponseMessage();
            Assert.IsNotNull(response, "response should not be null");
            string content = response.Content.ReadAsStringAsync().Result;
            UnitTest.Asserters.String.Contains(content, "This is a sample response body", "Response does not contain expected body content.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that HttpMessageContent read as MIME multipart scenario works correctly.")]
        public void HttpContentMessage_ReadMultipartHttpContentTest()
        {
            HttpRequestMessage request;
            HttpResponseMessage response;
            Program.ReadMultipartHttpContent(out request, out response);
            Assert.IsNotNull(request, "request should not be null");
            Assert.IsNotNull(response, "response should not be null");

            string requestContent = request.Content.ReadAsStringAsync().Result;
            UnitTest.Asserters.String.Contains(requestContent, "This is a sample request body", "Request does not contain expected body content.");

            string responseContent = response.Content.ReadAsStringAsync().Result;
            UnitTest.Asserters.String.Contains(responseContent, "This is a sample response body", "Response does not contain expected body content.");
        }
    }
}