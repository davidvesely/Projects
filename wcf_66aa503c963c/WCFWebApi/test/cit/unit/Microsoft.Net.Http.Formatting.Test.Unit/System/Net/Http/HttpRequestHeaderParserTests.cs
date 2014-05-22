// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpRequestHeaderParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser is internal class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<HttpRequestHeaderParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers
        private static byte[] CreateBuffer(string method, string address, string version, Dictionary<string,string> headers)
        {
            const string SP = " ";
            const string CRLF = "\r\n";
            string lws = SP;

            StringBuilder request = new StringBuilder();
            request.AppendFormat("{0}{1}{2}{3}{4}{5}", method, lws, address, lws, version, CRLF);
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.AppendFormat("{0}: {1}{2}", h.Key, h.Value, CRLF);
                }
            }

            request.Append(CRLF);
            return Encoding.UTF8.GetBytes(request.ToString());
        }

        private static ParserState ParseBufferInSteps(HttpRequestHeaderParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
        {
            ParserState state = ParserState.Invalid;
            totalBytesConsumed = 0;
            while (totalBytesConsumed <= buffer.Length)
            {
                int size = Math.Min(buffer.Length - totalBytesConsumed, readsize);
                byte[] parseBuffer = new byte[size];
                Buffer.BlockCopy(buffer, totalBytesConsumed, parseBuffer, 0, size);

                int bytesConsumed = 0;
                state = parser.ParseBuffer(parseBuffer, parseBuffer.Length, ref bytesConsumed);
                totalBytesConsumed += bytesConsumed;

                if (state != ParserState.NeedMoreData)
                {
                    return state;
                }
            }

            return state;
        }

        private static void ValidateResult(
            HttpUnsortedRequest requestLine, 
            string method, 
            string requestUri, 
            Version version, 
            Dictionary<string, string> headers)
        {
            Assert.AreEqual(new HttpMethod(method), requestLine.Method, "Parsed method did not match the expected value.");
            Assert.AreEqual(requestUri, requestLine.RequestUri, "Parsed request URI did not match expected value");
            Assert.AreEqual(version, requestLine.Version, "Parsed version did not match expected value");

            if (headers != null)
            {
                Assert.AreEqual(headers.Count, requestLine.HttpHeaders.Count(), "Parsed header count did not match expected value.");
                foreach (var header in headers)
                {
                    Assert.IsTrue(requestLine.HttpHeaders.Contains(header.Key), "Parsed header did not contain expected key " + header.Key);
                    Assert.AreEqual(header.Value, requestLine.HttpHeaders.GetValues(header.Key).ElementAt(0), "Parsed header did not contain expected value.");
                }
            }
        }

        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser constructor throws on invalid arguments")]
        public void HttpRequestHeaderParserConstructorTest()
        {
            HttpUnsortedRequest result = new HttpUnsortedRequest();
            Assert.IsNotNull(result);

            UnitTest.Asserters.Exception.ThrowsArgument("maxRequestLineSize", () => { new HttpRequestHeaderParser(result, ParserData.MinRequestLineSize - 1, ParserData.MinHeaderSize); });

            UnitTest.Asserters.Exception.ThrowsArgument("maxHeaderSize", () => { new HttpRequestHeaderParser(result, ParserData.MinRequestLineSize, ParserData.MinHeaderSize - 1); });

            HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result, ParserData.MinRequestLineSize, ParserData.MinHeaderSize);
            Assert.IsNotNull(parser);

            UnitTest.Asserters.Exception.ThrowsArgumentNull("httpRequest", () => { new HttpRequestHeaderParser(null); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer throws on null buffer.")]
        public void RequestHeaderParserNullBuffer()
        {
            HttpUnsortedRequest result = new HttpUnsortedRequest();
            HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result, ParserData.MinRequestLineSize, ParserData.MinHeaderSize);
            Assert.IsNotNull(parser);
            int bytesConsumed = 0;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer parses minimum requestline.")]
        public void RequestHeaderParserMinimumBuffer()
        {
            byte[] data = CreateBuffer("G", "/", "HTTP/1.1", null);
            HttpUnsortedRequest result = new HttpUnsortedRequest();
            HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result, ParserData.MinRequestLineSize, ParserData.MinHeaderSize); 
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed);
            Assert.AreEqual(ParserState.Done, state);
            Assert.AreEqual(data.Length, bytesConsumed);

            ValidateResult(result, "G", "/", ParserData.Versions[1], null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer parses standard methods.")]
        public void RequestHeaderParserAcceptsStandardMethods()
        {
            foreach (HttpMethod method in UnitTest.DataSets.Http.AllHttpMethods)
            {
                byte[] data = CreateBuffer(method.ToString(), "/", "HTTP/1.1", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, method.ToString(), "/", ParserData.Versions[1], ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer parses custom methods.")]
        public void RequestHeaderParserAcceptsCustomMethods()
        {
            foreach (HttpMethod method in UnitTest.DataSets.Http.CustomHttpMethods)
            {
                byte[] data = CreateBuffer(method.ToString(), "/", "HTTP/1.1", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, method.ToString(), "/", ParserData.Versions[1], ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer rejects invalid method")]
        public void RequestHeaderParserRejectsInvalidMethod()
        {
            foreach (string invalidMethod in ParserData.InvalidMethods)
            {
                byte[] data = CreateBuffer(invalidMethod, "/", "HTTP/1.1", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer rejects invalid URI.")]
        public void RequestHeaderParserRejectsInvalidUri()
        {
            foreach (string invalidRequestUri in ParserData.InvalidRequestUris)
            {
                byte[] data = CreateBuffer("GET", invalidRequestUri, "HTTP/1.1", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer accepts valid versions.")]
        public void RequestHeaderParserAcceptsValidVersion()
        {
            foreach (Version version in ParserData.Versions)
            {
                byte[] data = CreateBuffer("GET", "/", string.Format("HTTP/{0}", version.ToString(2)), ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, "GET", "/", version, ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestHeaderParser.ParseBuffer rejects lower case protocol version.")]
        public void RequestHeaderParserRejectsInvalidVersion()
        {
            foreach (string invalid in ParserData.InvalidVersions)
            {
                byte[] data = CreateBuffer("GET", "/", invalid, ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest result = new HttpUnsortedRequest();
                    HttpRequestHeaderParser parser = new HttpRequestHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        #endregion
    }
}