// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(RequestHeadersInvoker))]
    public class RequestHeadersInvokerTests : UnitTest
    {
        public static readonly ReadOnlyCollection<TestData> RepresentativeHeadersAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<HeadersAndExpectedOptions>(() => new List<HeadersAndExpectedOptions>()
            {
                new HeadersAndExpectedOptions() 
                {
                    Headers = "a",
                    CursorPos = 1,
                    ExpectedOptions = GetAllHeaders(0, 0),
                    StatusCode = HttpStatusCode.OK
                },
                new HeadersAndExpectedOptions() 
                {
                    Headers = "Accept: application/json\r\n  a",
                    CursorPos = 29,
                    ExpectedOptions = GetAllHeaders(25, 0),
                    StatusCode = HttpStatusCode.OK
                },
                new HeadersAndExpectedOptions() 
                {
                    Headers = "Accept: \ta",
                    CursorPos = 10,
                    ExpectedOptions = new string[] 
                    {
                        "*/*\n*/*\n7\n\n", "application/*\napplication/*\n7\n\n", "application/xml\napplication/xml\n7\n\n", "application/json\napplication/json\n7\n\n", "text/*\ntext/*\n7\n\n", "text/xml\ntext/xml\n7\n\n", "text/json\ntext/json\n7\n\n", "text/plain\ntext/plain\n7\n\n"
                    },
                    StatusCode = HttpStatusCode.OK
                },
                new HeadersAndExpectedOptions() 
                {
                    Headers = "Accept: application/json\r\n",
                    CursorPos = 100, // out of range
                    ExpectedOptions = null,
                    StatusCode = HttpStatusCode.BadRequest
                }
            })}
        );

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("RequestUriIntellisenseInvoker is internal and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("Invoke(object, object[], out object[]) returns available http header intellisense options.")]
        public void InvokeReturnsIntellisenseOptions()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            RequestHeadersInvoker invoker = new RequestHeadersInvoker();
            object[] dummy;

            Asserters.Data.Execute(
                RepresentativeHeadersAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    HeadersAndExpectedOptions headersAndExpectedOptions = (HeadersAndExpectedOptions)obj;
                    request.Content = new ActionOfStreamContent(stream =>
                    {
                        Encoding encoding = new UTF8Encoding();
                        byte[] preamble = encoding.GetPreamble();
                        stream.Write(preamble, 0, preamble.Length);
                        byte[] bytes = encoding.GetBytes(headersAndExpectedOptions.Headers);
                        stream.Write(bytes, 0, bytes.Length);
                    });

                    HttpResponseMessage response = (HttpResponseMessage)invoker.Invoke(
                        null, 
                        new object[] { request, headersAndExpectedOptions.CursorPos }, 
                        out dummy);

                    this.VerifyAutocompleteResponse(response, headersAndExpectedOptions);
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

        private void VerifyAutocompleteResponse(HttpResponseMessage response, HeadersAndExpectedOptions expectedOptions)
        {
            Assert.AreEqual(expectedOptions.StatusCode, response.StatusCode);

            if (response.Content == null)
            {
                Assert.AreEqual(expectedOptions.ExpectedOptions, null);
            }
            else
            {
                byte[] body = response.Content.ReadAsByteArrayAsync().Result;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TolerantTextReaderResult));
                TolerantTextReaderResult result;
                using (MemoryStream stream = new MemoryStream(body))
                {
                    result = serializer.ReadObject(stream) as TolerantTextReaderResult;
                }

                Assert.AreEqual<int>(expectedOptions.ExpectedOptions.Length, result.AutoCompleteList.Count<string>());
                for (int i = 0; i < expectedOptions.ExpectedOptions.Length; ++i)
                {
                    Assert.AreEqual<string>(expectedOptions.ExpectedOptions[i], result.AutoCompleteList.ElementAt<string>(i));
                }
            }
        }

        #endregion Methods

        public class HeadersAndExpectedOptions
        {
            public string Headers { get; set; }
            public int CursorPos { get; set; }
            public string[] ExpectedOptions { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }
    }
}
