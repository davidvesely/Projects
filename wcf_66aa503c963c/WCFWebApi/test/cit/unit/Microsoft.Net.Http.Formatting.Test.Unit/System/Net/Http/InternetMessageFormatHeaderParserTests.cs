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
    public class InternetMessageFormatHeaderParserTests
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser is internal class")]
        public void TypeIsCorrect()
        {
            UnitTest.Asserters.Type.HasProperties<InternetMessageFormatHeaderParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers
        private static IEnumerable<HttpHeaders> CreateHttpHeaders()
        {
            return new HttpHeaders[]
            {
                new HttpRequestMessage().Headers,
                new HttpResponseMessage().Headers,
                new StringContent(string.Empty).Headers,
            };
        }

        private static InternetMessageFormatHeaderParser CreateHeaderParser(int maximumHeaderLength, out HttpHeaders headers)
        {
            headers = new HttpRequestMessage().Headers;
            return new InternetMessageFormatHeaderParser(headers, maximumHeaderLength);
        }

        internal static byte[] CreateBuffer(params string[] headers)
        {
            const string CRLF = "\r\n";
            StringBuilder header = new StringBuilder();
            foreach (var h in headers)
            {
                header.Append(h + CRLF);
            }

            header.Append(CRLF);
            return Encoding.UTF8.GetBytes(header.ToString());
        }

        private static void RunRfc5322SampleTest(string[] testHeaders, Action<HttpHeaders> validation)
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer(testHeaders);
            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                validation(headers);
            }
        }

        private static ParserState ParseBufferInSteps(InternetMessageFormatHeaderParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
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
        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser constructor throws on invalid arguments")]
        public void HeaderParserConstructorTest()
        {
            IEnumerable<HttpHeaders> headers = InternetMessageFormatHeaderParserTests.CreateHttpHeaders();
            foreach (var header in headers)
            {
                InternetMessageFormatHeaderParser parser = new InternetMessageFormatHeaderParser(header, ParserData.MinHeaderSize);
                Assert.IsNotNull(parser);
            }

            UnitTest.Asserters.Exception.ThrowsArgument("maxHeaderSize", () => { new InternetMessageFormatHeaderParser(headers.ElementAt(0), -1); });
            UnitTest.Asserters.Exception.ThrowsArgument("maxHeaderSize", () => { new InternetMessageFormatHeaderParser(headers.ElementAt(0), 0); });
            UnitTest.Asserters.Exception.ThrowsArgument("maxHeaderSize", () => { new InternetMessageFormatHeaderParser(headers.ElementAt(0), 1); });

            UnitTest.Asserters.Exception.ThrowsArgumentNull("headers", () => { new InternetMessageFormatHeaderParser(null, ParserData.MinHeaderSize); });
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer throws on null buffer.")]
        public void HeaderParserNullBuffer()
        {
            HttpHeaders headers;
            InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(128, out headers);
            Assert.IsNotNull(parser);
            int bytesConsumed = 0;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses empty header.")]
        public void HeaderParserEmptyBuffer()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer();
            HttpHeaders headers;
            InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
            Assert.IsNotNull(parser);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed);
            Assert.AreEqual(ParserState.Done, state);
            Assert.AreEqual(data.Length, bytesConsumed);

            Assert.AreEqual(0, headers.Count());
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses single header field.")]
        public void HeaderParserSingleNameValueHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N:V");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(1, headers.Count());
                IEnumerable<string> parsedValues = headers.GetValues("N");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual(parsedValues.ElementAt(0), "V");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses single header field with name only.")]
        public void HeaderParserSingleNameHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N:");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(1, headers.Count());
                IEnumerable<string> parsedValues = headers.GetValues("N");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual("", parsedValues.ElementAt(0));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses multiple header fields.")]
        public void HeaderParserMultipleNameHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N:V1", "N:V2");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(1, headers.Count());
                IEnumerable<string> parsedValues = headers.GetValues("N");
                Assert.AreEqual(2, parsedValues.Count());
                Assert.AreEqual("V1", parsedValues.ElementAt(0));
                Assert.AreEqual("V2", parsedValues.ElementAt(1));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses multiple header fields with linear white space.")]
        public void HeaderParserLwsHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N1:V1", "N2: V2", "N3:\tV3");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(3, headers.Count());

                IEnumerable<string> parsedValues = headers.GetValues("N1");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual("V1", parsedValues.ElementAt(0));

                parsedValues = headers.GetValues("N2");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual("V2", parsedValues.ElementAt(0));

                parsedValues = headers.GetValues("N3");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual("V3", parsedValues.ElementAt(0));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses invalid header field.")]
        public void HeaderParserInvalidHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N1 :V1");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Invalid, state);
                Assert.AreEqual(data.Length - 2, totalBytesConsumed);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses various specialized header fields including JSON, P3P, etc.")]
        public void HeaderParserSpecializedHeaders()
        {
            Dictionary<string, string> headerData = new Dictionary<string, string>
            {
                { @"JsonProperties0", @"{ ""SessionId"": ""{27729E1-B37B-4D29-AA0A-E367906C206E}"", ""MessageId"": ""{701332E1-B37B-4D29-AA0A-E367906C206E}"", ""TimeToLive"" : 90, ""CorrelationId"": ""{701332F3-B37B-4D29-AA0A-E367906C206E}"", ""SequenceNumber"" : 12345, ""DeliveryCount"" : 2, ""To"" : ""http://contoso.com/path1"", ""ReplyTo"" : ""http://fabrikam.com/path1"",  ""SentTimeUtc"" : ""Sun, 06 Nov 1994 08:49:37 GMT"", ""ScheduledEnqueueTimeUtc"" : ""Sun, 06 Nov 1994 08:49:37 GMT""}" },
                { @"JsonProperties1", @"{ ""SessionId"": ""{2813D4D2-46A9-4F4D-8904-E9BDE3712B70}"", ""MessageId"": ""{24AE31D6-63B8-46F3-9975-A3DAF1B6D3F4}"", ""TimeToLive"" : 80, ""CorrelationId"": ""{896DD5BD-1645-44D7-9E7C-D7F70958ECD6}"", ""SequenceNumber"" : 54321, ""DeliveryCount"" : 4, ""To"" : ""http://contoso.com/path2"", ""ReplyTo"" : ""http://fabrikam.com/path2"",  ""SentTimeUtc"" : ""Sun, 06 Nov 1994 10:49:37 GMT"", ""ScheduledEnqueueTimeUtc"" : ""Sun, 06 Nov 1994 10:49:37 GMT""}" },
                { @"P3P", @"CP=""ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI""" },
                { @"Cookie", @"omniID=1297715979621_9f45_1519_3f8a_f22f85346ac6; WT_FPC=id=65.55.227.138-2323234032.30136233:lv=1309374389020:ss=1309374389020; A=I&I=AxUFAAAAAACNCAAADYEZ7CFPss7Swnujy4PXZA!!&M=1&CS=126mAa0002ZB51a02gZB51a; MC1=GUID=568428660ad44d4ab8f46133f4b03738&HASH=6628&LV=20113&V=3; WT_NVR_RU=0=msdn:1=:2=; MUID=A44DE185EA1B4E8088CCF7B348C5D65F; MSID=Microsoft.CreationDate=03/04/2011 23:38:15&Microsoft.LastVisitDate=06/20/2011 04:15:08&Microsoft.VisitStartDate=06/20/2011 04:15:08&Microsoft.CookieId=f658f3f2-e6d6-42ab-b86b-96791b942b6f&Microsoft.TokenId=ffffffff-ffff-ffff-ffff-ffffffffffff&Microsoft.NumberOfVisits=106&Microsoft.CookieFirstVisit=1&Microsoft.IdentityToken=AA==&Microsoft.MicrosoftId=0441-6141-1523-9969; msresearch=%7B%22version%22%3A%224.6%22%2C%22state%22%3A%7B%22name%22%3A%22IDLE%22%2C%22url%22%3Aundefined%2C%22timestamp%22%3A1299281911415%7D%2C%22lastinvited%22%3A1299281911415%2C%22userid%22%3A%2212992819114151265672533023080%22%2C%22vendorid%22%3A1%2C%22surveys%22%3A%5Bundefined%5D%7D; CodeSnippetContainerLang=C#; msdn=L=1033; ADS=SN=175A21EF; s_cc=true; s_sq=%5B%5BB%5D%5D; TocHashCookie=ms310241(n)/aa187916(n)/aa187917(n)/dd273952(n)/dd295083(n)/ff472634(n)/ee667046(n)/ee667070(n)/gg259047(n)/gg618436(n)/; WT_NVR=0=/:1=query|library|en-us:2=en-us/vcsharp|en-us/library" },
                { @"Set-Cookie", @"A=I&I=AxUFAAAAAADsBgAA1sWZz4FGun/kOeyV4LGZVg!!&M=1; domain=.microsoft.com; expires=Sun, 30-Jun-2041 00:14:40 GMT; path=/" },
            };

            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer(headerData.Select((kv) => { return string.Format("{0}: {1}", kv.Key, kv.Value); }).ToArray());
            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(headerData.Count, headers.Count());
                for (int hCnt = 0; hCnt < headerData.Count; hCnt++)
                {
                    Assert.AreEqual(headerData.Keys.ElementAt(hCnt), headers.ElementAt(hCnt).Key);
                    Assert.AreEqual(headerData.Values.ElementAt(hCnt), headers.ElementAt(hCnt).Value.ElementAt(0));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses multi-line header field.")]
        public void HeaderParserSplitHeader()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N:V1,", " V2,", "\tV3,", "      V4,", " \tV5");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(data.Length, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.Done, state);
                Assert.AreEqual(data.Length, totalBytesConsumed);

                Assert.AreEqual(1, headers.Count());
                IEnumerable<string> parsedValues = headers.GetValues("N");
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual("V1, V2, V3,      V4, \tV5", parsedValues.ElementAt(0));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses too big header with single header field.")]
        public void HeaderParserDataTooBigSingle()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N:V");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(ParserData.MinHeaderSize, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.DataTooBig, state, "HeaderParser not in expected state (Count: {0}, Data: {1})", cnt, Encoding.UTF8.GetString(data));
                Assert.AreEqual(ParserData.MinHeaderSize, totalBytesConsumed, "HeaderParser did not consume expected number of bytes (Count: {0}, Data: {1})", cnt, Encoding.UTF8.GetString(data));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer parses too big header with multiple header field.")]
        public void HeaderParserTestDataTooBigMulti()
        {
            byte[] data = InternetMessageFormatHeaderParserTests.CreateBuffer("N1:V1", "N2:V2", "N3:V3");

            for (var cnt = 1; cnt <= data.Length; cnt++)
            {
                HttpHeaders headers;
                InternetMessageFormatHeaderParser parser = InternetMessageFormatHeaderParserTests.CreateHeaderParser(10, out headers);
                Assert.IsNotNull(parser);

                int totalBytesConsumed = 0;
                ParserState state = InternetMessageFormatHeaderParserTests.ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                Assert.AreEqual(ParserState.DataTooBig, state, "HeaderParser not in expected state (Count: {0}, Data: {1})", cnt, Encoding.UTF8.GetString(data));
                Assert.AreEqual(10, totalBytesConsumed, "HeaderParser did not consume expected number of bytes (Count: {0}, Data: {1})", cnt, Encoding.UTF8.GetString(data));
            }
        }
        #endregion

        #region RFC 5322 Samples
        // Set of samples from RFC 5322 with times adjusted to GMT following HTTP style for date time format. 

        static readonly string[] Rfc5322Sample1 = new string[] {
            @"From: John Doe <jdoe@machine.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample2 = new string[] {
            @"From: John Doe <jdoe@machine.example>",
            @"Sender: Michael Jones <mjones@machine.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample3 = new string[] {
            @"From: ""Joe Q. Public"" <john.q.public@example.com>",
            @"To: Mary Smith <mary@x.test>, jdoe@example.org, Who? <one@y.test>",
            @"Cc: <boss@nil.test>, ""Giant; \""Big\"" Box"" <sysservices@example.net>",
            @"Date: Tue, 01 Jul 2003 10:52:37 GMT",
            @"Message-ID: <5678.21-Nov-1997@example.com>",
        };

        static readonly string[] Rfc5322Sample4 = new string[] {
            @"From: Pete <pete@silly.example>",
            @"To: A Group:Ed Jones <c@a.test>,joe@where.test,John <jdoe@one.test>;",
            @"Cc: Undisclosed recipients:;",
            @"Date: Thu, 13 Feb 1969 23:32:54 GMT",
            @"Message-ID: <testabcd.1234@silly.example>",
        };

        static readonly string[] Rfc5322Sample5 = new string[] {
            @"From: John Doe <jdoe@machine.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample6 = new string[] {
            @"From: Mary Smith <mary@example.net>",
            @"To: John Doe <jdoe@machine.example>",
            @"Reply-To: ""Mary Smith: Personal Account"" <smith@home.example>",
            @"Subject: Re: Saying Hello",
            @"Date: Fri, 21 Nov 1997 10:01:10 GMT",
            @"Message-ID: <3456@example.net>",
            @"In-Reply-To: <1234@local.machine.example>",
            @"References: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample7 = new string[] {
            @"To: ""Mary Smith: Personal Account"" <smith@home.example>",
            @"From: John Doe <jdoe@machine.example>",
            @"Subject: Re: Saying Hello",
            @"Date: Fri, 21 Nov 1997 11:00:00 GMT",
            @"Message-ID: <abcd.1234@local.machine.test>",
            @"In-Reply-To: <3456@example.net>",
            @"References: <1234@local.machine.example> <3456@example.net>",
        };

        static readonly string[] Rfc5322Sample8 = new string[] {
            @"From: John Doe <jdoe@machine.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample9 = new string[] {
            @"Resent-From: Mary Smith <mary@example.net>",
            @"Resent-To: Jane Brown <j-brown@other.example>",
            @"Resent-Date: Mon, 24 Nov 1997 14:22:01 GMT",
            @"Resent-Message-ID: <78910@example.net>",
            @"From: John Doe <jdoe@machine.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.machine.example>",
        };

        static readonly string[] Rfc5322Sample10 = new string[] {
            @"Received: from x.y.test",
            @"   by example.net",
            @"   via TCP",
            @"   with ESMTP",
            @"   id ABC12345",
            @"   for <mary@example.net>;  21 Nov 1997 10:05:43 GMT",
            @"Received: from node.example by x.y.test; 21 Nov 1997 10:01:22 GMT",
            @"From: John Doe <jdoe@node.example>",
            @"To: Mary Smith <mary@example.net>",
            @"Subject: Saying Hello",
            @"Date: Fri, 21 Nov 1997 09:55:06 GMT",
            @"Message-ID: <1234@local.node.example>",
        };

        static readonly string[] Rfc5322Sample11 = new string[] {
            @"From: Pete(A nice \) chap) <pete(his account)@silly.test(his host)>",
            @"To:A Group(Some people)",
            @"     :Chris Jones <c@(Chris's host.)public.example>,",
            @"         joe@example.org,",
            @"  John <jdoe@one.test> (my dear friend); (the end of the group)",
            @"Cc:(Empty list)(start)Hidden recipients  :(nobody(that I know))  ;",
            @"Date: Thu,",
            @"      13",
            @"        Feb",
            @"          1969",
            @"      23:32:00",
            @"               GMT",
            @"Message-ID:              <testabcd.1234@silly.test>",
        };

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample1 header.")]
        public void Rfc5322Sample1Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample1,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample2 header.")]
        public void Rfc5322Sample2Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample2,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("sender"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample3 header.")]
        public void Rfc5322Sample3Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample3,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("cc"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample4 header.")]
        public void Rfc5322Sample4Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample4,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("cc"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample5 header.")]
        public void Rfc5322Sample5Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample5,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample6 header.")]
        public void Rfc5322Sample6Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample6,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("reply-to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                    Assert.IsTrue(headers.Contains("in-reply-to"));
                    Assert.IsTrue(headers.Contains("references"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample7 header.")]
        public void Rfc5322Sample7Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample7,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                    Assert.IsTrue(headers.Contains("in-reply-to"));
                    Assert.IsTrue(headers.Contains("references"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample8 header.")]
        public void Rfc5322Sample8Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample8,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample9 header.")]
        public void Rfc5322Sample9Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample9,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("resent-from"));
                    Assert.IsTrue(headers.Contains("resent-to"));
                    Assert.IsTrue(headers.Contains("resent-date"));
                    Assert.IsTrue(headers.Contains("resent-message-id"));
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample10 header.")]
        public void Rfc5322Sample10Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample10,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("received"));
                    Assert.AreEqual(2, headers.GetValues("received").Count());
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("subject"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HeaderParser.ParseBuffer Rfc5322Sample11 header.")]
        public void Rfc5322Sample11Test()
        {
            RunRfc5322SampleTest(Rfc5322Sample11,
                (headers) =>
                {
                    Assert.IsNotNull(headers);
                    Assert.IsTrue(headers.Contains("from"));
                    Assert.IsTrue(headers.Contains("to"));
                    Assert.IsTrue(headers.Contains("cc"));
                    Assert.IsTrue(headers.Contains("date"));
                    Assert.IsTrue(headers.Contains("message-id"));
                });
        }
        #endregion
    }
}