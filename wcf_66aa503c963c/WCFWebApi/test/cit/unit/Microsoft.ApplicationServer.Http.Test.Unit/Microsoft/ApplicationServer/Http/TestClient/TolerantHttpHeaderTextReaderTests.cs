// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(TolerantHttpHeaderTextReader))]
    public class TolerantHttpHeaderTextReaderTests : UnitTest
    {
        public static readonly ReadOnlyCollection<TestData> RepresentativeHeadersAndExpectedOptions = new ReadOnlyCollection<TestData>(
          new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
            {
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.headers_full_txt,
                    ExpectedOptions = GetAllHeaders(53, 0)
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.headers_stop_at_accept_value_txt,
                    ExpectedOptions = new string[] { "*/*\n*/*\n7\n\n", "application/*\napplication/*\n7\n\n", "application/xml\napplication/xml\n7\n\n", "application/json\napplication/json\n7\n\n", "text/*\ntext/*\n7\n\n", "text/xml\ntext/xml\n7\n\n", "text/json\ntext/json\n7\n\n", "text/plain\ntext/plain\n7\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.headers_stop_at_accept_txt,
                    ExpectedOptions = GetAllHeaders(0, 0)
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.headers_stop_at_contenttype_value_txt,
                    ExpectedOptions = new string[] { "application/xml\napplication/xml\n36\n\n", "application/json\napplication/json\n36\n\n", "text/xml\ntext/xml\n36\n\n", "text/json\ntext/json\n36\n\n", "text/plain\ntext/plain\n36\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.headers_stop_at_contenttype_txt,
                    ExpectedOptions = GetAllHeaders(23, 0)
                }
            })});
        
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("TolerantHttpHeaderTextReader is internal, disposable and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsDisposable);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("GetExpectedItems() returns available options")]
        public void GetExpectedItemsReturnsAvailableOptions()
        {
            Asserters.Data.Execute(
                RepresentativeHeadersAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    string text = textAndExpectedOptions.Text.Replace("\r\n", "\n");
                    using (TolerantHttpHeaderTextReader reader = new TolerantHttpHeaderTextReader(text, -1))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }

                    int length = text.Length;
                    text = StringResources.headers_full_txt.Replace("\r\n", "\n");
                    using (TolerantHttpHeaderTextReader reader = new TolerantHttpHeaderTextReader(text, length))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }

        private static string[] GetAllHeaders(int replaceStartPos, int finalCaretPos)
        {
            string[] array = new string[] 
            {
                "Accept", "Accept-Charset", "Accept-Encoding", "Accept-Language", "Authorization", "Cache-Control", 
                "Connection", "Cookie", "Content-Length", "Content-MD5", "Content-Type", "Date", "From", 
                "Host", "If-Match", "If-Modified-Since", "If-None-Match", "If-Range", "If-Unmodified-Since", 
                "Max-Forwards", "Pragma", "Proxy-Authorization", "Range", "Referer", "TE", "Upgrade", "User-Agent",
                "Via", "Warning"
            };

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = string.Format("{0}\n{0}:\n{1}\n{2}\n", array[i], replaceStartPos, finalCaretPos);
            }

            return array;
        }

        #endregion Methods
    }
}