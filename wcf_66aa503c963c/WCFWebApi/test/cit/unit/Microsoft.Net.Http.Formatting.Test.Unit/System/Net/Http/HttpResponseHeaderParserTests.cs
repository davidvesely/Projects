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
    public class HttpResponseHeaderParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser is internal class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<HttpResponseHeaderParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers
        private static byte[] CreateBuffer(string version, string statusCode, string reasonPhrase, Dictionary<string, string> headers)
        {
            const string SP = " ";
            const string CRLF = "\r\n";
            string lws = SP;

            StringBuilder response = new StringBuilder();
            response.AppendFormat("{0}{1}{2}{3}{4}{5}", version, lws, statusCode, lws, reasonPhrase, CRLF);
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    response.AppendFormat("{0}: {1}{2}", h.Key, h.Value, CRLF);
                }
            }

            response.Append(CRLF);
            return Encoding.UTF8.GetBytes(response.ToString());
        }

        private static ParserState ParseBufferInSteps(HttpResponseHeaderParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
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
            HttpUnsortedResponse statusLine,
            Version version,
            HttpStatusCode statusCode, 
            string reasonPhrase, 
            Dictionary<string, string> headers)
        {
            Assert.AreEqual(version, statusLine.Version, "Parsed version did not match expected value");
            Assert.AreEqual(statusCode, statusLine.StatusCode, "Parsed status code did not match the expected value.");
            Assert.AreEqual(reasonPhrase, statusLine.ReasonPhrase, "Parsed reason phrase did not match expected value");

            if (headers != null)
            {
                Assert.AreEqual(headers.Count, statusLine.HttpHeaders.Count(), "Parsed header count did not match expected value.");
                foreach (var header in headers)
                {
                    Assert.IsTrue(statusLine.HttpHeaders.Contains(header.Key), "Parsed header did not contain expected key " + header.Key);
                    Assert.AreEqual(header.Value, statusLine.HttpHeaders.GetValues(header.Key).ElementAt(0), "Parsed header did not contain expected value.");
                }
            }
        }

        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser constructor throws on invalid arguments")]
        public void HttpResponseHeaderParserConstructorTest()
        {
            HttpUnsortedResponse result = new HttpUnsortedResponse();
            Assert.IsNotNull(result);

            UnitTest.Asserters.Exception.ThrowsArgument("maxStatusLineSize", () => { new HttpResponseHeaderParser(result, ParserData.MinStatusLineSize - 1, ParserData.MinHeaderSize); });

            UnitTest.Asserters.Exception.ThrowsArgument("maxHeaderSize", () => { new HttpResponseHeaderParser(result, ParserData.MinStatusLineSize, ParserData.MinHeaderSize - 1); });

            HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result, ParserData.MinStatusLineSize, ParserData.MinHeaderSize);
            Assert.IsNotNull(parser);

            UnitTest.Asserters.Exception.ThrowsArgumentNull("httpResponse", () => { new HttpResponseHeaderParser(null); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer throws on null buffer.")]
        public void ResponseHeaderParserNullBuffer()
        {
            HttpUnsortedResponse result = new HttpUnsortedResponse();
            HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result, ParserData.MinStatusLineSize, ParserData.MinHeaderSize);
            Assert.IsNotNull(parser);
            int bytesConsumed = 0;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer parses minimum statusLine.")]
        public void ResponseHeaderParserMinimumBuffer()
        {
            byte[] data = CreateBuffer("HTTP/1.1", "200", "", null);
            HttpUnsortedResponse result = new HttpUnsortedResponse();
            HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result, ParserData.MinStatusLineSize, ParserData.MinHeaderSize); 
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed);
            Assert.AreEqual(ParserState.Done, state);
            Assert.AreEqual(data.Length, bytesConsumed);

            ValidateResult(result, ParserData.Versions[1], HttpStatusCode.OK, "", null);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer parses standard status codes.")]
        public void ResponseHeaderParserAcceptsStandardStatusCodes()
        {
            foreach (HttpStatusCode status in UnitTest.DataSets.Http.AllHttpStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", ((int)status).ToString(), "Reason", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, ParserData.Versions[1], status, "Reason", ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer parses custom status codes.")]
        public void ResponseHeaderParserAcceptsCustomStatusCodes()
        {
            foreach (HttpStatusCode status in UnitTest.DataSets.Http.CustomHttpStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", ((int)status).ToString(), "Reason", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, ParserData.Versions[1], status, "Reason", ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer rejects invalid status codes")]
        public void ResponseHeaderParserRejectsInvalidStatusCodes()
        {
            foreach (string invalidStatus in ParserData.InvalidStatusCodes)
            {
                byte[] data = CreateBuffer("HTTP/1.1", invalidStatus, "Reason", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer rejects invalid reason phrase.")]
        public void ResponseHeaderParserRejectsInvalidReasonPhrase()
        {
            foreach (string invalidReason in ParserData.InvalidReasonPhrases)
            {
                byte[] data = CreateBuffer("HTTP/1.1", "200", invalidReason, ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Invalid, state);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer accepts valid versions.")]
        public void ResponseHeaderParserAcceptsValidVersion()
        {
            foreach (Version version in ParserData.Versions)
            {
                byte[] data = CreateBuffer(string.Format("HTTP/{0}", version.ToString(2)), "200", "Reason", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed = 0;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    ValidateResult(result, version, HttpStatusCode.OK, "Reason", ParserData.ValidHeaders);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseHeaderParser.ParseBuffer rejects invalid protocol version.")]
        public void ResponseHeaderParserRejectsInvalidVersion()
        {
            foreach (string invalid in ParserData.InvalidVersions)
            {
                byte[] data = CreateBuffer(invalid, "200", "Reason", ParserData.ValidHeaders);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    HttpUnsortedResponse result = new HttpUnsortedResponse();
                    HttpResponseHeaderParser parser = new HttpResponseHeaderParser(result);
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