// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MimeMultipartParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MimeMultipartParserTypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<InternetMessageFormatHeaderParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        private static MimeMultipartParser CreateMimeMultipartParser(int maximumHeaderLength, string boundary)
        {
            return new MimeMultipartParser(boundary, maximumHeaderLength);
        }

        internal static byte[] CreateBuffer(string boundary, params string[] bodyparts)
        {
            return CreateBuffer(boundary, false, bodyparts);
        }

        internal static string CreateNestedBuffer(int count)
        {
            StringBuilder buffer = new StringBuilder("content");

            for (var cnt = 0; cnt < count; cnt++)
            {
                byte[] nested = CreateBuffer("N" + cnt.ToString(), buffer.ToString());
                var message = Encoding.UTF8.GetString(nested);
                buffer.Length = 0;
                buffer.AppendLine(message);
            }

            return buffer.ToString();
        }

        private static byte[] CreateBuffer(string boundary, bool withLws, params string[] bodyparts)
        {
            const string SP = " ";
            const string HTAB = "\t";
            const string CRLF = "\r\n";
            const string DashDash = "--";

            string lws = string.Empty;
            if (withLws)
            {
                lws = SP + SP + HTAB + SP;
            }

            StringBuilder message = new StringBuilder();
            message.Append(DashDash + boundary + lws + CRLF);
            for (var cnt = 0; cnt < bodyparts.Length; cnt++)
            {
                message.Append(bodyparts[cnt]);
                if (cnt < bodyparts.Length - 1)
                {
                    message.Append(CRLF + DashDash + boundary + lws + CRLF);
                }
            }

            // Note: We rely on a final CRLF even though it is not required by the BNF existing application do send it
            message.Append(CRLF + DashDash + boundary + DashDash + lws + CRLF);
            return Encoding.UTF8.GetBytes(message.ToString());
        }

        private static MimeMultipartParser.State ParseBufferInSteps(MimeMultipartParser parser, byte[] buffer, int readsize, out List<string> bodyParts, out int totalBytesConsumed)
        {
            MimeMultipartParser.State state = MimeMultipartParser.State.Invalid;
            totalBytesConsumed = 0;
            bodyParts = new List<string>();
            bool isFinal = false;
            byte[] currentBodyPart = new byte[32 * 1024];
            int currentBodyLength = 0;

            while (totalBytesConsumed <= buffer.Length)
            {
                int size = Math.Min(buffer.Length - totalBytesConsumed, readsize);
                byte[] parseBuffer = new byte[size];
                Buffer.BlockCopy(buffer, totalBytesConsumed, parseBuffer, 0, size);

                int bytesConsumed = 0;
                ArraySegment<byte> out1;
                ArraySegment<byte> out2;
                state = parser.ParseBuffer(parseBuffer, parseBuffer.Length, ref bytesConsumed, out out1, out out2, out isFinal);
                totalBytesConsumed += bytesConsumed;

                Buffer.BlockCopy(out1.Array, out1.Offset, currentBodyPart, currentBodyLength, out1.Count);
                currentBodyLength += out1.Count;

                Buffer.BlockCopy(out2.Array, out2.Offset, currentBodyPart, currentBodyLength, out2.Count);
                currentBodyLength += out2.Count;

                if (state == MimeMultipartParser.State.BodyPartCompleted)
                {
                    var bPart = new byte[currentBodyLength];
                    Buffer.BlockCopy(currentBodyPart, 0, bPart, 0, currentBodyLength);
                    bodyParts.Add(Encoding.UTF8.GetString(bPart));
                    currentBodyLength = 0;
                    if (isFinal)
                    {
                        break;
                    }
                }
                else if (state != MimeMultipartParser.State.NeedMoreData)
                {
                    return state;
                }
            }

            Assert.IsTrue(isFinal);
            return state;
        }

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MimeMultipartParserConstructorTest()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                MimeMultipartParser parser = new MimeMultipartParser(boundary, ParserData.MinMessageSize);
                Assert.IsNotNull(parser);
            }

            UnitTest.Asserters.Exception.ThrowsArgument("maxMessageSize", () => { new MimeMultipartParser("-", ParserData.MinMessageSize - 1); });

            foreach (string empty in TestData.EmptyStrings)
            {
                UnitTest.Asserters.Exception.ThrowsArgument("boundary", () => { new MimeMultipartParser(empty, ParserData.MinMessageSize); });
            }

            UnitTest.Asserters.Exception.ThrowsArgument("boundary", () => { new MimeMultipartParser("trailingspace ", ParserData.MinMessageSize); });

            UnitTest.Asserters.Exception.ThrowsArgumentNull("boundary", () => { new MimeMultipartParser(null, ParserData.MinMessageSize); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserNullBuffer()
        {
            MimeMultipartParser parser = CreateMimeMultipartParser(128, "-");
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ArraySegment<byte> out1;
            ArraySegment<byte> out2;
            bool isFinal;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed, out out1, out out2, out isFinal); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserEmptyBuffer()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(2, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);
                    Assert.AreEqual(0, bodyParts[1].Length);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserSingleShortBodyPart()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, "A");

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(2, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);
                    Assert.AreEqual(1, bodyParts[1].Length);
                    Assert.AreEqual("A", bodyParts[1]);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserMultipleShortBodyParts()
        {
            string[] text = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, text);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(text.Length + 1, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);

                    for (var check = 0; check < text.Length; check++)
                    {
                        Assert.AreEqual(1, bodyParts[check + 1].Length);
                        Assert.AreEqual(text[check], bodyParts[check + 1]);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserMultipleShortBodyPartsWithLws()
        {
            string[] text = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, true, text);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(text.Length + 1, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);

                    for (var check = 0; check < text.Length; check++)
                    {
                        Assert.AreEqual(1, bodyParts[check + 1].Length);
                        Assert.AreEqual(text[check], bodyParts[check + 1]);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserSingleLongBodyPart()
        {
            const string text = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, text);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(2, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);

                    Assert.AreEqual(text.Length, bodyParts[1].Length);
                    Assert.AreEqual(text, bodyParts[1]);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserMultipleLongBodyParts()
        {
            const string middleText = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            string[] text = new string[] { 
                "A" + middleText + "A", 
                "B" + middleText + "B", 
                "C" + middleText + "C", 
                "D" + middleText + "D", 
                "E" + middleText + "E", 
                "F" + middleText + "F", 
                "G" + middleText + "G", 
                "H" + middleText + "H", 
                "I" + middleText + "I", 
                "J" + middleText + "J", 
                "K" + middleText + "K", 
                "L" + middleText + "L", 
                "M" + middleText + "M", 
                "N" + middleText + "N", 
                "O" + middleText + "O", 
                "P" + middleText + "P", 
                "Q" + middleText + "Q", 
                "R" + middleText + "R", 
                "S" + middleText + "S", 
                "T" + middleText + "T", 
                "U" + middleText + "U", 
                "V" + middleText + "V", 
                "W" + middleText + "W", 
                "X" + middleText + "X", 
                "Y" + middleText + "Y", 
                "Z" + middleText + "Z"};

            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, text);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(text.Length + 1, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);

                    for (var check = 0; check < text.Length; check++)
                    {
                        Assert.AreEqual(text[check].Length, bodyParts[check + 1].Length);
                        Assert.AreEqual(text[check], bodyParts[check + 1]);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserNearMatches()
        {
            const string CR = "\r";
            const string CRLF = "\r\n";
            const string Dash = "-";
            const string DashDash = "--";

            string[] text = new string[] { 
                CR + Dash + "AAA",
                CRLF + Dash + "AAA",
                CRLF + DashDash + "AAA" + CR + "AAA",
                CRLF,
                "AAA",
                "AAA" + CRLF,
                CRLF + CRLF,
                CRLF + CRLF + CRLF,
                "AAA" + DashDash + "AAA",
                CRLF + "AAA" + DashDash + "AAA" + DashDash,
                CRLF + DashDash + "AAA" + CRLF, 
                CRLF + DashDash + "AAA" + CRLF + CRLF, 
                CRLF + DashDash + "AAA" + DashDash + CRLF, 
                CRLF + DashDash + "AAA" + DashDash + CRLF + CRLF
            };

            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary, text);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(text.Length + 1, bodyParts.Count);
                    Assert.AreEqual(0, bodyParts[0].Length);

                    for (var check = 0; check < text.Length; check++)
                    {
                        Assert.AreEqual(text[check].Length, bodyParts[check + 1].Length);
                        Assert.AreEqual(text[check], bodyParts[check + 1]);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultipartParserNesting()
        {
            for (var nesting = 0; nesting < 16; nesting++)
            {
                string nested = CreateNestedBuffer(nesting);

                foreach (var boundary in ParserData.Boundaries)
                {
                    byte[] data = CreateBuffer(boundary, nested);

                    for (var cnt = 1; cnt <= data.Length; cnt++)
                    {
                        MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                        Assert.IsNotNull(parser);

                        int totalBytesConsumed;
                        List<string> bodyParts;
                        MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                        Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state, "MimeMultipartParser not in expected state (Count: {0}, Nesting: {1}, Boundary: {2})", cnt, nesting, boundary);
                        Assert.AreEqual(data.Length, totalBytesConsumed, "MimeMultipartParser did not consume expected number of bytes (Count: {0}, Nesting: {1}, Boundary: {2})", cnt, nesting, boundary);

                        Assert.AreEqual(2, bodyParts.Count, "MimeMultipartParser did not produce expected number of bodyparts (Count: {0}, Nesting: {1}, Boundary: {2})", cnt, nesting, boundary);
                        Assert.AreEqual(0, bodyParts[0].Length, "MimeMultipartParser produced unexpected length of first bodypart (Count: {0}, Nesting: {1}, Boundary: {2})", cnt, nesting, boundary);
                        Assert.AreEqual(nested.Length, bodyParts[1].Length, "MimeMultipartParser did not produce the expected number of bodyparts (Count: {0}, Nesting: {1}, Boundary: {2})", cnt, nesting, boundary);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MimeMultipartParserTestDataTooBig()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                byte[] data = CreateBuffer(boundary);

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(ParserData.MinMessageSize, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.DataTooBig, state);
                    Assert.AreEqual(ParserData.MinMessageSize, totalBytesConsumed);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MimeMultipartParserTestMultipartContent()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                MultipartContent content = new MultipartContent("mixed", boundary);
                content.Add(new StringContent("A"));
                content.Add(new StringContent("B"));
                content.Add(new StringContent("C"));

                MemoryStream memStream = new MemoryStream();
                content.CopyToAsync(memStream).Wait();
                memStream.Position = 0;
                byte[] data = memStream.ToArray();

                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    MimeMultipartParser parser = CreateMimeMultipartParser(data.Length, boundary);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    List<string> bodyParts;
                    MimeMultipartParser.State state = ParseBufferInSteps(parser, data, cnt, out bodyParts, out totalBytesConsumed);
                    Assert.AreEqual(MimeMultipartParser.State.BodyPartCompleted, state, "MimeMultipartParser not in expected state (Count: {0}, Boundary: {1})", cnt, boundary);
                    Assert.AreEqual(data.Length, totalBytesConsumed, "MimeMultipartParser did not consume expected number of bytes (Count: {0}, Boundary: {1})", cnt, boundary);

                    Assert.AreEqual(4, bodyParts.Count, "MimeMultipartParser did not produce expected number of bodyparts (Count: {0}, Boundary: {1})", cnt, boundary);
                    Assert.AreEqual(0, bodyParts[0].Length, "MimeMultipartParser produced unexpected length of first bodypart (Count: {0}, Boundary: {1})", cnt, boundary);

                    UnitTest.Asserters.String.EndsWith(bodyParts[1], "A");
                    UnitTest.Asserters.String.EndsWith(bodyParts[2], "B");
                    UnitTest.Asserters.String.EndsWith(bodyParts[3], "C");
                }
            }
        }

        #endregion
    }
}