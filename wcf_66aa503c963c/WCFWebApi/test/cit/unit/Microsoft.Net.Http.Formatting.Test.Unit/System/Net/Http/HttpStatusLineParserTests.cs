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
    public class HttpStatusLineParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser is internal class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<HttpStatusLineParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers
        internal static byte[] CreateBuffer(string version, string statusCode, string reasonPhrase)
        {
            return CreateBuffer(version, statusCode, reasonPhrase, false);
        }

        private static byte[] CreateBuffer(string version, string statusCode, string reasonPhrase, bool withLws)
        {
            const string SP = " ";
            const string HTAB = "\t";
            const string CRLF = "\r\n";

            string lws = SP;
            if (withLws)
            {
                lws = SP + SP + HTAB + SP;
            }

            string statusLine = string.Format("{0}{1}{2}{3}{4}{5}", version, lws, statusCode, lws, reasonPhrase, CRLF);
            return Encoding.UTF8.GetBytes(statusLine);
        }

        private static ParserState ParseBufferInSteps(HttpStatusLineParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
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

        private static void ValidateResult(HttpUnsortedResponse statusLine, Version version, HttpStatusCode statusCode, string reasonPhrase)
        {
            Assert.AreEqual(version, statusLine.Version, "Parsed version did not match expected value");
            Assert.AreEqual(statusCode, statusLine.StatusCode, "Parsed status code did not match the expected value.");
            Assert.AreEqual(reasonPhrase, statusLine.ReasonPhrase, "Parsed reason phrase did not match expected value");
        }

        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser constructor throws on invalid arguments")]
        public void HttpStatusLineParserConstructorTest()
        {
            HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
            Assert.IsNotNull(statusLine);

            UnitTest.Asserters.Exception.ThrowsArgument("maxStatusLineSize", () => { new HttpStatusLineParser(statusLine, ParserData.MinStatusLineSize - 1); });

            HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, ParserData.MinStatusLineSize);
            Assert.IsNotNull(parser);

            UnitTest.Asserters.Exception.ThrowsArgumentNull("httpResponse", () => { new HttpStatusLineParser(null, ParserData.MinStatusLineSize); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer throws on null buffer.")]
        public void StatusLineParserNullBuffer()
        {
            HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
            HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, ParserData.MinStatusLineSize);
            Assert.IsNotNull(parser);
            int bytesConsumed = 0;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer parses minimum requestline.")]
        public void StatusLineParserMinimumBuffer()
        {
            byte[] data = CreateBuffer("HTTP/1.1", "200", "");
            HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
            HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, ParserData.MinStatusLineSize);
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed);
            Assert.AreEqual(ParserState.Done, state);
            Assert.AreEqual(data.Length, bytesConsumed);

            ValidateResult(statusLine, ParserData.Versions[1], HttpStatusCode.OK, "");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer rejects LWS requestline.")]
        public void StatusLineParserRejectsLws()
        {
            byte[] data = CreateBuffer("HTTP/1.1", "200", "Reason", true);
            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, data.Length);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Invalid, state);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer parses standard status codes.")]
        public void StatusLineParserAcceptsStandardStatusCodes()
        {
            foreach (HttpStatusCode status in UnitTest.DataSets.Http.AllHttpStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", ((int)status).ToString(), "Reason");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, data.Length);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(statusLine, ParserData.Versions[1], status, "Reason");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer parses custom status codes.")]
        public void StatusLineParserAcceptsCustomStatusCodes()
        {
            foreach (HttpStatusCode status in UnitTest.DataSets.Http.CustomHttpStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", ((int)status).ToString(), "Reason");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, data.Length);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(statusLine, ParserData.Versions[1], status, "Reason");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer rejects invalid status codes")]
        public void StatusLineParserRejectsInvalidStatusCodes()
        {
            foreach (string invalidStatus in ParserData.InvalidStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", invalidStatus, "Reason");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer accepts valid reason phrase.")]
        public void StatusLineParserAcceptsValidReasonPhrase()
        {
            foreach (string validReasonPhrase in ParserData.ValidReasonPhrases)
            {
                byte[] data = CreateBuffer("HTTP/1.1", "200", validReasonPhrase);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);

                    ValidateResult(statusLine, ParserData.Versions[1], HttpStatusCode.OK, validReasonPhrase);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer accepts valid versions.")]
        public void StatusLineParserAcceptsValidVersion()
        {
            foreach (Version version in ParserData.Versions)
            {
                byte[] data = CreateBuffer(string.Format("HTTP/{0}", version.ToString(2)), "200", "Reason");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, 256);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(statusLine, version, HttpStatusCode.OK, "Reason");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpStatusLineParser.ParseBuffer rejects invalid protocol version.")]
        public void StatusLineParserRejectsInvalidVersion()
        {
            foreach (string invalidVersion in ParserData.InvalidVersions)
            {
                byte[] data = CreateBuffer(invalidVersion, "200", "Reason");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse statusLine = new HttpUnsortedResponse();
                    HttpStatusLineParser parser = new HttpStatusLineParser(statusLine, 256);
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