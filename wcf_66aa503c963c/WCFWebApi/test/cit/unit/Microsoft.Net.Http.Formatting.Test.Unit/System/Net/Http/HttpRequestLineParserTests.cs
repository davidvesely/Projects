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
    public class HttpRequestLineParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser is internal class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<HttpRequestLineParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers
        internal static byte[] CreateBuffer(string method, string address, string version)
        {
            return CreateBuffer(method, address, version, false);
        }

        private static byte[] CreateBuffer(string method, string address, string version, bool withLws)
        {
            const string SP = " ";
            const string HTAB = "\t";
            const string CRLF = "\r\n";

            string lws = SP;
            if (withLws)
            {
                lws = SP + SP + HTAB + SP;
            }

            string requestLine = string.Format("{0}{1}{2}{3}{4}{5}", method, lws, address, lws, version, CRLF);
            return Encoding.UTF8.GetBytes(requestLine);
        }

        private static ParserState ParseBufferInSteps(HttpRequestLineParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
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

        private static void ValidateResult(HttpUnsortedRequest requestLine, string method, string requestUri, Version version)
        {
            Assert.AreEqual(new HttpMethod(method), requestLine.Method, "Parsed method did not match the expected value.");
            Assert.AreEqual(requestUri, requestLine.RequestUri, "Parsed request URI did not match expected value");
            Assert.AreEqual(version, requestLine.Version, "Parsed version did not match expected value");
        }

        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser constructor throws on invalid arguments")]
        public void HttpRequestLineParserConstructorTest()
        {
            HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
            Assert.IsNotNull(requestLine);

            UnitTest.Asserters.Exception.ThrowsArgument("maxRequestLineSize", () => { new HttpRequestLineParser(requestLine, ParserData.MinRequestLineSize - 1); });

            HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, ParserData.MinRequestLineSize);
            Assert.IsNotNull(parser);

            UnitTest.Asserters.Exception.ThrowsArgumentNull("httpRequest", () => { new HttpRequestLineParser(null, ParserData.MinRequestLineSize); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer throws on null buffer.")]
        public void RequestLineParserNullBuffer()
        {
            HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
            HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, ParserData.MinRequestLineSize);
            Assert.IsNotNull(parser);
            int bytesConsumed = 0;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer parses minimum requestline.")]
        public void RequestLineParserMinimumBuffer()
        {
            byte[] data = CreateBuffer("G", "/", "HTTP/1.1");
            HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
            HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, ParserData.MinRequestLineSize);
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed);
            Assert.AreEqual(ParserState.Done, state);
            Assert.AreEqual(data.Length, bytesConsumed);

            ValidateResult(requestLine, "G", "/", ParserData.Versions[1]);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer rejects LWS requestline.")]
        public void RequestLineParserRejectsLws()
        {
            byte[] data = CreateBuffer("GET", "/", "HTTP/1.1", true);

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, data.Length);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Invalid, state);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer parses standard methods.")]
        public void RequestLineParserAcceptsStandardMethods()
        {
            foreach (HttpMethod method in UnitTest.DataSets.Http.AllHttpMethods)
            {
                byte[] data = CreateBuffer(method.ToString(), "/", "HTTP/1.1");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, data.Length);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(requestLine, method.ToString(), "/", ParserData.Versions[1]);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer parses custom methods.")]
        public void RequestLineParserAcceptsCustomMethods()
        {
            foreach (HttpMethod method in UnitTest.DataSets.Http.CustomHttpMethods)
            {
                byte[] data = CreateBuffer(method.ToString(), "/", "HTTP/1.1");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, data.Length);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(requestLine, method.ToString(), "/", ParserData.Versions[1]);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer rejects invalid method")]
        public void RequestLineParserRejectsInvalidMethod()
        {
            foreach (string invalidMethod in ParserData.InvalidMethods)
            {
                byte[] data = CreateBuffer(invalidMethod, "/", "HTTP/1.1");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer rejects invalid URI.")]
        public void RequestLineParserRejectsInvalidUri()
        {
            foreach (string invalidRequestUri in ParserData.InvalidRequestUris)
            {
                byte[] data = CreateBuffer("GET", invalidRequestUri, "HTTP/1.1");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer accepts valid versions.")]
        public void RequestLineParserAcceptsValidVersion()
        {
            foreach (Version version in ParserData.Versions)
            {
                byte[] data = CreateBuffer("GET", "/", string.Format("HTTP/{0}", version.ToString(2)));

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(requestLine, "GET", "/", version);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestLineParser.ParseBuffer rejects invalid protocol version.")]
        public void RequestLineParserRejectsInvalidVersion()
        {
            foreach (string invalidVersion in ParserData.InvalidVersions)
            {
                byte[] data = CreateBuffer("GET", "/", invalidVersion);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedRequest requestLine = new HttpUnsortedRequest();
                    HttpRequestLineParser parser = new HttpRequestLineParser(requestLine, 256);
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