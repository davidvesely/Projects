// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpContentMessageExtensionMethodsTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpContentMessageExtensionMethods is a public static class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties(
                typeof(HttpContentMessageExtensionMethods),
                TypeAssert.TypeProperties.IsPublicVisibleClass |
                TypeAssert.TypeProperties.IsStatic);
        }
        #endregion

        #region Helpers
        private static HttpContent CreateContent(bool isRequest, bool hasEntity)
        {
            string message;
            if (isRequest)
            {
                message = hasEntity ? ParserData.HttpRequestWithEntity : ParserData.HttpRequest;
            }
            else
            {
                message = hasEntity ? ParserData.HttpResponseWithEntity : ParserData.HttpResponse;
            }

            StringContent content = new StringContent(message);
            content.Headers.ContentType = isRequest ? ParserData.HttpRequestMediaType : ParserData.HttpResponseMediaType;
            return content;
        }

        private static HttpContent CreateContent(bool isRequest, string[] header, string body)
        {
            StringBuilder message = new StringBuilder();
            foreach (string h in header)
            {
                message.Append(h);
                message.Append("\r\n");
            }

            message.Append("\r\n");
            if (body != null)
            {
                message.Append(body);
            }

            StringContent content = new StringContent(message.ToString());
            content.Headers.ContentType = isRequest ? ParserData.HttpRequestMediaType : ParserData.HttpResponseMediaType;
            return content;
        }

        private static void ValidateEntity(HttpContent content)
        {
            Assert.IsNotNull(content, "message should have content");
            Assert.AreEqual(ParserData.TextContentType, content.Headers.ContentType.ToString(), "message did not have expected media type");
            string entity = content.ReadAsStringAsync().Result;
            Assert.AreEqual(ParserData.HttpMessageEntity, entity, "message did not have expected entity");
         }

        private static void ValidateRequestMessage(HttpRequestMessage request, bool hasEntity)
        {
            Assert.IsNotNull(request, "request should not be null");
            Assert.AreEqual(ParserData.Versions[2], request.Version, "request did not have expected version");
            Assert.AreEqual(ParserData.HttpMethod, request.Method.ToString(), "Request did not have the expected method");
            Assert.AreEqual(ParserData.HttpRequestUri, request.RequestUri, "Request did not have the expected request URI");
            Assert.AreEqual(ParserData.HttpHostName, request.Headers.Host, "Request did not have the expected host header");
            Assert.IsTrue(request.Headers.Contains("N1"), "request did not contain expected N1 header.");
            Assert.IsTrue(request.Headers.Contains("N2"), "request did not contain expected N2 header.");

            if (hasEntity)
            {
                ValidateEntity(request.Content);
            }
        }

        private static void ValidateResponseMessage(HttpResponseMessage response, bool hasEntity)
        {
            Assert.IsNotNull(response, "Response should not be null");
            Assert.AreEqual(ParserData.Versions[2], response.Version, "Response did not have expected version");
            Assert.AreEqual(ParserData.HttpStatus, response.StatusCode, "Response did not have the expected status");
            Assert.AreEqual(ParserData.HttpReasonPhrase, response.ReasonPhrase, "Response did not have the expected reason phrase");
            Assert.IsTrue(response.Headers.Contains("N1"), "Response did not contain expected N1 header.");
            Assert.IsTrue(response.Headers.Contains("N2"), "Response did not contain expected N2 header.");

            if (hasEntity)
            {
                ValidateEntity(response.Content);
            }
        }

        #endregion

        #region ArgumentValidation

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Checks extension method arguments.")]
        public void ReadAsHttpRequestMessageVerifyArguments()
        {
            UnitTest.Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMessageExtensionMethods.ReadAsHttpRequestMessage(content);
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () => 
            {
                HttpContent content = new ByteArrayContent(new byte[] { });
                content.ReadAsHttpRequestMessage();
            });
            
            UnitTest.Asserters.Exception.ThrowsArgument("content", () => 
            { 
                HttpContent content = new StringContent(string.Empty);
                content.ReadAsHttpRequestMessage();
            });
            
            UnitTest.Asserters.Exception.ThrowsArgument("content", () => 
            { 
                HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/http");
                content.ReadAsHttpRequestMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpResponseMediaType;
                content.ReadAsHttpRequestMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgumentNull("uriScheme", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpRequestMediaType;
                content.ReadAsHttpRequestMessage(null);
            });

            UnitTest.Asserters.Exception.ThrowsArgument("uriScheme", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpRequestMediaType;
                content.ReadAsHttpRequestMessage("i n v a l i d");
            });

            UnitTest.Asserters.Exception.ThrowsArgument("bufferSize", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpRequestMediaType;
                content.ReadAsHttpRequestMessage(Uri.UriSchemeHttp, ParserData.MinHeaderSize - 1);
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Checks extension method arguments.")]
        public void ReadAsHttpResponseMessageVerifyArguments()
        {
            UnitTest.Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMessageExtensionMethods.ReadAsHttpResponseMessage(content);
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new ByteArrayContent(new byte[] { });
                content.ReadAsHttpResponseMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.ReadAsHttpResponseMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/http");
                content.ReadAsHttpResponseMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpRequestMediaType;
                content.ReadAsHttpResponseMessage();
            });

            UnitTest.Asserters.Exception.ThrowsArgument("bufferSize", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.Headers.ContentType = ParserData.HttpResponseMediaType;
                content.ReadAsHttpResponseMessage(ParserData.MinHeaderSize - 1);
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Checks extension method arguments.")]
        public void IsHttpRequestMessageContentVerifyArguments()
        {
            UnitTest.Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMessageExtensionMethods.IsHttpRequestMessageContent(content);
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Checks extension method arguments.")]
        public void IsHttpResponseMessageContentVerifyArguments()
        {
            UnitTest.Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMessageExtensionMethods.IsHttpResponseMessageContent(content);
            });
        }

        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsHttpRequestMessageContent responds correctly to various content types")]
        public void IsHttpRequestMessageContent()
        {
            Assert.IsFalse(new ByteArrayContent(new byte[] { }).IsHttpRequestMessageContent(), "HttpContent should not be valid HTTP Request content");

            Assert.IsFalse(new StringContent(string.Empty).IsHttpRequestMessageContent(), "HttpContent should not be valid HTTP Request content");

            Assert.IsFalse(new StringContent(string.Empty, Encoding.UTF8, "application/http").IsMimeMultipartContent(), "HttpContent should not be valid HTTP Request content");

            HttpContent content = new StringContent(string.Empty);
            content.Headers.ContentType = ParserData.HttpRequestMediaType;
            Assert.IsTrue(content.IsHttpRequestMessageContent(), "Content should be HTTP request.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsHttpResponseMessageContent responds correctly to various content types")]
        public void IsHttpResponseMessageContent()
        {
            Assert.IsFalse(new ByteArrayContent(new byte[] { }).IsHttpResponseMessageContent(), "HttpContent should not be valid HTTP Response content");

            Assert.IsFalse(new StringContent(string.Empty).IsHttpResponseMessageContent(), "HttpContent should not be valid HTTP Response content");

            Assert.IsFalse(new StringContent(string.Empty, Encoding.UTF8, "application/http").IsMimeMultipartContent(), "HttpContent should not be valid HTTP Response content");

            HttpContent content = new StringContent(string.Empty);
            content.Headers.ContentType = ParserData.HttpResponseMediaType;
            Assert.IsTrue(content.IsHttpResponseMessageContent(), "Content should be HTTP response.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeRequest()
        {
            HttpContent content = CreateContent(true, false);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            ValidateRequestMessage(httpRequest, false);
        }

        // TODO, CSDMain 239978, enable this test once the original issue is resolved.
        [Ignore]
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeRequestWithEntity()
        {
            HttpContent content = CreateContent(true, true);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            ValidateRequestMessage(httpRequest, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage with https scheme.")]
        public void DeserializeRequestWithEntityHttpsScheme()
        {
            HttpContent content = CreateContent(true, true);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage(Uri.UriSchemeHttps);
            Assert.AreEqual(ParserData.HttpsRequestUri, httpRequest.RequestUri, "Request did not have the expected request URI");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpResponseMessage should return HttpResponseMessage.")]
        public void DeserializeResponse()
        {
            HttpContent content = CreateContent(false, false);
            HttpResponseMessage httpResponse = content.ReadAsHttpResponseMessage();
            ValidateResponseMessage(httpResponse, false);
        }

        // TODO, CSDMain 239978, enable this test once the original issue is resolved.
        [Ignore]
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpResponseMessage should return HttpResponseMessage.")]
        public void DeserializeResponseWithEntity()
        {
            HttpContent content = CreateContent(false, true);
            HttpResponseMessage httpResponse = content.ReadAsHttpResponseMessage();
            ValidateResponseMessage(httpResponse, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void ReadHttpRequestMessageNoHost()
        {
            string[] NoHostRequest = new string[] {
                @"GET / HTTP/1.1",
            };

            HttpContent content = CreateContent(true, NoHostRequest, null);
            UnitTest.Asserters.Exception.Throws<IOException>(
                () => { HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage(); },
                (exception) => { });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void ReadHttpRequestMessageMultipleHosts()
        {
            string[] TwoHostRequest = new string[] {
                @"GET / HTTP/1.1",
                @"Host: somehost.com",
                @"Host: otherhost.com",
            };

            HttpContent content = CreateContent(true, TwoHostRequest, null);
            UnitTest.Asserters.Exception.Throws<IOException>(
                () => { HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage(); },
                (exception) => { });
        }

        #endregion

        #region Browser and Server Samples

        public static readonly string[] BrowserRequestExplorer = new string[] {
            @"GET / HTTP/1.1",
            @"Accept: text/html, application/xhtml+xml, */*",
            @"Accept-Language: en-US",
            @"User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)",
            @"Accept-Encoding: gzip, deflate",
            @"Proxy-Connection: Keep-Alive",
            @"Host: msdn.microsoft.com",
            @"Cookie: omniID=1297715979621_9f45_1519_3f8a_f22f85346ac6; WT_FPC=id=65.55.227.138-2323234032.30136233:lv=1309374389020:ss=1309374389020; A=I&I=AxUFAAAAAACNCAAADYEZ7CFPss7Swnujy4PXZA!!&M=1&CS=126mAa0002ZB51a02gZB51a; MC1=GUID=568428660ad44d4ab8f46133f4b03738&HASH=6628&LV=20113&V=3; WT_NVR_RU=0=msdn:1=:2=; MUID=A44DE185EA1B4E8088CCF7B348C5D65F; MSID=Microsoft.CreationDate=03/04/2011 23:38:15&Microsoft.LastVisitDate=06/20/2011 04:15:08&Microsoft.VisitStartDate=06/20/2011 04:15:08&Microsoft.CookieId=f658f3f2-e6d6-42ab-b86b-96791b942b6f&Microsoft.TokenId=ffffffff-ffff-ffff-ffff-ffffffffffff&Microsoft.NumberOfVisits=106&Microsoft.CookieFirstVisit=1&Microsoft.IdentityToken=AA==&Microsoft.MicrosoftId=0441-6141-1523-9969; msresearch=%7B%22version%22%3A%224.6%22%2C%22state%22%3A%7B%22name%22%3A%22IDLE%22%2C%22url%22%3Aundefined%2C%22timestamp%22%3A1299281911415%7D%2C%22lastinvited%22%3A1299281911415%2C%22userid%22%3A%2212992819114151265672533023080%22%2C%22vendorid%22%3A1%2C%22surveys%22%3A%5Bundefined%5D%7D; CodeSnippetContainerLang=C#; msdn=L=1033; ADS=SN=175A21EF; s_cc=true; s_sq=%5B%5BB%5D%5D; TocHashCookie=ms310241(n)/aa187916(n)/aa187917(n)/dd273952(n)/dd295083(n)/ff472634(n)/ee667046(n)/ee667070(n)/gg259047(n)/gg618436(n)/; WT_NVR=0=/:1=query|library|en-us:2=en-us/vcsharp|en-us/library",
        };

        public static readonly string[] BrowserRequestFireFox = new string[] {
            @"GET / HTTP/1.1",
            @"Host: msdn.microsoft.com",
            @"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0",
            @"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            @"Accept-Language: en-us,en;q=0.5",
            @"Accept-Encoding: gzip, deflate",
            @"Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7",
            @"Proxy-Connection: keep-alive",
        };

        public static readonly string[] BrowserRequestChrome = new string[]
        {
            @"GET / HTTP/1.1",
            @"Host: msdn.microsoft.com",
            @"Proxy-Connection: keep-alive",
            @"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.100 Safari/534.30",
            @"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            @"Accept-Encoding: gzip,deflate,sdch",
            @"Accept-Language: en-US,en;q=0.8",
            @"Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3",        
        };

        public static readonly string[] BrowserRequestSafari = new string[]
        {
            @"GET / HTTP/1.1",
            @"Host: msdn.microsoft.com",
            @"User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.21.1 (KHTML, like Gecko) Version/5.0.5 Safari/533.21.1",
            @"Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5",
            @"Accept-Language: en-US",
            @"Accept-Encoding: gzip, deflate",
            @"Connection: keep-alive",
            @"Proxy-Connection: keep-alive",
        };

        public static readonly string[] BrowserRequestOpera = new string[]
        {
            @"GET / HTTP/1.0",
            @"User-Agent: Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
            @"Host: msdn.microsoft.com",
            @"Accept: text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/webp, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1",
            @"Accept-Language: en-US,en;q=0.9",
            @"Accept-Encoding: gzip, deflate",
            @"Proxy-Connection: Keep-Alive",
        };

        public static readonly string[] ServerResponseAsp = new string[] 
        { 
            @"HTTP/1.1 302 Found",
            @"Proxy-Connection: Keep-Alive",
            @"Connection: Keep-Alive",
            @"Content-Length: 124",
            @"Via: 1.1 RED-PRXY-23",
            @"Date: Thu, 30 Jun 2011 00:16:35 GMT",
            @"Location: /en-us/",
            @"Content-Type: text/html; charset=utf-8",
            @"Server: Microsoft-IIS/7.5",
            @"Cache-Control: private",
            @"P3P: CP=""ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI""",
            @"Set-Cookie: A=I&I=AxUFAAAAAAD7BwAA8Jx0njhGoW3MGASDmzeaGw!!&M=1; domain=.microsoft.com; expires=Sun, 30-Jun-2041 00:16:35 GMT; path=/",
            @"Set-Cookie: ADS=SN=175A21EF; domain=.microsoft.com; path=/",
            @"Set-Cookie: Sto.UserLocale=en-us; path=/",
            @"X-AspNetMvc-Version: 3.0",
            @"X-AspNet-Version: 4.0.30319",
            @"X-Powered-By: ASP.NET",
            @"Set-Cookie: A=I&I=AxUFAAAAAAD7BwAA8Jx0njhGoW3MGASDmzeaGw!!&M=1; domain=.microsoft.com; expires=Sun, 30-Jun-2041 00:16:35 GMT; path=/; path=/",
            @"Set-Cookie: ADS=SN=175A21EF; domain=.microsoft.com; path=/; path=/",
            @"P3P: CP=""ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI""",
            @"X-Powered-By: ASP.NET",
        };

        public static readonly string ServerResponseAspEntity = @"<html><head><title>Object moved</title></head><body><h2>Object moved to <a href=""/en-us/"">here</a>.</h2></body></html>";

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeBrowserRequestExplorer()
        {
            HttpContent content = CreateContent(true, BrowserRequestExplorer, null);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            Assert.IsTrue(httpRequest.Headers.Contains("cookie"), "request did not contain expected header");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeBrowserRequestFireFox()
        {
            HttpContent content = CreateContent(true, BrowserRequestFireFox, null);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            Assert.IsTrue(httpRequest.Headers.Contains("proxy-connection"), "request did not contain expected header");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeBrowserRequestChrome()
        {
            HttpContent content = CreateContent(true, BrowserRequestChrome, null);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            Assert.IsTrue(httpRequest.Headers.Contains("accept-charset"), "request did not contain expected header");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeBrowserRequestSafari()
        {
            HttpContent content = CreateContent(true, BrowserRequestSafari, null);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            Assert.IsTrue(httpRequest.Headers.Contains("proxy-connection"), "request did not contain expected header");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpRequestMessage should return HttpRequestMessage.")]
        public void DeserializeBrowserRequestOpera()
        {
            HttpContent content = CreateContent(true, BrowserRequestOpera, null);
            HttpRequestMessage httpRequest = content.ReadAsHttpRequestMessage();
            Assert.IsTrue(httpRequest.Headers.Contains("proxy-connection"), "request did not contain expected header");
        }

        // TODO, CSDMain 239978, enable this test once the original issue is resolved.
        [Ignore]
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsHttpResponseMessage should return HttpResponseMessage.")]
        public void DeserializeServerResponseAsp()
        {
            HttpContent content = CreateContent(false, ServerResponseAsp, ServerResponseAspEntity);
            HttpResponseMessage httpResponse = content.ReadAsHttpResponseMessage();
            Assert.IsTrue(httpResponse.Headers.Contains("x-powered-by"), "response did not contain expected header");
            string entity = httpResponse.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(ServerResponseAspEntity, entity, "response did not contain expected entity");
        }

        #endregion
    }
}