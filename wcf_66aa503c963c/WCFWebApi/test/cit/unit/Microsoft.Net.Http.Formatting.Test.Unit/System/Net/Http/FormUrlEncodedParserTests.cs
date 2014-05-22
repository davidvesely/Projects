// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(FormUrlEncodedParser)), UnitTestLevel(Microsoft.TestCommon.UnitTestLevel.Complete)]
    public class FormUrlEncodedParserTests : UnitTest
    {
        private const int MinMessageSize = 1;
        private const int Iterations = 16;

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedParser is internal class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<FormUrlEncodedParser>(TypeAssert.TypeProperties.IsClass);
        }
        #endregion

        #region Helpers

        internal static Collection<Tuple<string, string>> CreateCollection()
        {
            return new Collection<Tuple<string, string>>();
        }

        internal static FormUrlEncodedParser CreateParser(int maxMessageSize, out ICollection<Tuple<string, string>> nameValuePairs)
        {
            nameValuePairs = CreateCollection();
            return new FormUrlEncodedParser(nameValuePairs, maxMessageSize);
        }

        internal static byte[] CreateBuffer(params string[] nameValuePairs)
        {
            StringBuilder buffer = new StringBuilder();
            bool first = true;
            foreach (var h in nameValuePairs)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    buffer.Append('&');
                }

                buffer.Append(h);
            }

            return Encoding.UTF8.GetBytes(buffer.ToString());
        }

        internal static ParserState ParseBufferInSteps(FormUrlEncodedParser parser, byte[] buffer, int readsize, out int totalBytesConsumed)
        {
            ParserState state = ParserState.Invalid;
            totalBytesConsumed = 0;
            while (totalBytesConsumed <= buffer.Length)
            {
                int size = Math.Min(buffer.Length - totalBytesConsumed, readsize);
                byte[] parseBuffer = new byte[size];
                Buffer.BlockCopy(buffer, totalBytesConsumed, parseBuffer, 0, size);

                int bytesConsumed = 0;
                state = parser.ParseBuffer(parseBuffer, parseBuffer.Length, ref bytesConsumed, totalBytesConsumed == buffer.Length - size);
                totalBytesConsumed += bytesConsumed;

                if (state != ParserState.NeedMoreData)
                {
                    return state;
                }
            }

            return state;
        }

        private static void RunTest(string segment, string name, string value)
        {
            for (int index = 1; index < Iterations; index++)
            {
                List<string> segments = new List<string>();
                for (int cnt = 0; cnt < index; cnt++)
                {
                    segments.Add(segment);
                }

                byte[] data = CreateBuffer(segments.ToArray());
                for (var cnt = 1; cnt <= data.Length; cnt++)
                {
                    ICollection<Tuple<string, string>> collection;
                    FormUrlEncodedParser parser = CreateParser(data.Length + 1, out collection);
                    Assert.IsNotNull(parser);

                    int totalBytesConsumed;
                    ParserState state = ParseBufferInSteps(parser, data, cnt, out totalBytesConsumed);
                    Assert.AreEqual(ParserState.Done, state);
                    Assert.AreEqual(data.Length, totalBytesConsumed);

                    Assert.AreEqual(index, collection.Count());
                    foreach (Tuple<string, string> element in collection)
                    {
                        Assert.AreEqual(name, element.Item1);
                        Assert.AreEqual(value, element.Item2);
                    }
                }
            }
        }

        #endregion

        #region Constructor
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedParser(ICollection<Tuple<string, string>>, long) throws on null")]
        public void FormUrlEncodedParserThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull("nameValuePairs", () => { new FormUrlEncodedParser(null, ParserData.MinHeaderSize); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedParser(ICollection<Tuple<string, string>>, long) throws on invalid size")]
        public void FormUrlEncodedParserThrowsOnInvalidSize()
        {
            UnitTest.Asserters.Exception.ThrowsArgument("maxMessageSize", () => { new FormUrlEncodedParser(CreateCollection(), MinMessageSize - 1); });

            FormUrlEncodedParser parser = new FormUrlEncodedParser(CreateCollection(), MinMessageSize);
            Assert.IsNotNull(parser, "parser should not be null");

            parser = new FormUrlEncodedParser(CreateCollection(), MinMessageSize + 1);
            Assert.IsNotNull(parser, "parser should not be null");
        }
        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) throws on null buffer.")]
        public void ParseBufferThrowsOnNullBuffer()
        {
            ICollection<Tuple<string, string>> collection;
            FormUrlEncodedParser parser = CreateParser(128, out collection);
            int bytesConsumed = 0;
            Asserters.Exception.ThrowsArgumentNull("buffer", () => { parser.ParseBuffer(null, 0, ref bytesConsumed, false); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses empty header.")]
        public void ParseBufferHandlesEmptyBuffer()
        {
            byte[] data = CreateBuffer();
            ICollection<Tuple<string,string>> collection;
            FormUrlEncodedParser parser = CreateParser(MinMessageSize, out collection);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed, true);
            Assert.AreEqual(ParserState.Done, state, "Unexpected parser state");
            Assert.AreEqual(data.Length, bytesConsumed, "Unexpected number of consumed bytes");
            Assert.AreEqual(0, collection.Count(), "Expected 0 entries in parsed collection.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses single name value pair.")]
        public void ParseBufferSingleNameValuePair()
        {
            RunTest("N=V", "N", "V");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses single name.")]
        public void ParseBufferSingleName()
        {
            RunTest("N", null, "N");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses equal-only tokens.")]
        public void ParseBufferEqualOnly()
        {
            RunTest("=", string.Empty, string.Empty);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses N= tokens.")]
        public void ParseBufferEqualRight()
        {
            RunTest("N=", "N", string.Empty);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses =N tokens.")]
        public void ParseBufferEqualLeft()
        {
            RunTest("=N", string.Empty, "N");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseBuffer(byte[], int, ref int, bool) parses too big header with single header field.")]
        public void HeaderParserDataTooBig()
        {
            byte[] data = CreateBuffer("N=V");
            ICollection<Tuple<string, string>> collection;
            FormUrlEncodedParser parser = CreateParser(MinMessageSize, out collection);

            int bytesConsumed = 0;
            ParserState state = parser.ParseBuffer(data, data.Length, ref bytesConsumed, true);
            Assert.AreEqual(ParserState.DataTooBig, state, "HeaderParser not in expected state");
            Assert.AreEqual(MinMessageSize, bytesConsumed, "HeaderParser did not consume expected number of bytes");
        }

        #endregion
    }
}