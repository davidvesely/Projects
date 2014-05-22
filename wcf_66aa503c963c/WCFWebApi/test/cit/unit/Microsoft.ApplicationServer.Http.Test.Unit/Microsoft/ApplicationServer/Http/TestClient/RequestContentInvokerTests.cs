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

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(RequestContentInvoker))]
    public class RequestContentInvokerTests : UnitTest
    {
        public static readonly ReadOnlyCollection<TestData> RepresentativeContentAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<ContentAndExpectedOptions>(() => new List<ContentAndExpectedOptions>()
            {
                new ContentAndExpectedOptions() 
                {
                    ResourceUriTemplate = "http://localhost:8080/books",
                    HttpMethod = "POST",
                    Content = "<b",
                    CursorPos = 2,
                    ExpectedOptions = new string[] { "Book\nBook></Book>\n1\n-7\n" },
                    Format0 = "xml",
                    StatusCode = HttpStatusCode.OK
                },

                new ContentAndExpectedOptions() 
                {
                    ResourceUriTemplate = "http://localhost:8080/books",
                    HttpMethod = "POST",
                    Content = "<Book><a",
                    CursorPos = 8,
                    ExpectedOptions = new string[] { "IsAvailable\nIsAvailable></IsAvailable>\n7\n-14\n", "Name\nName></Name>\n7\n-7\n", "Category\nCategory></Category>\n7\n-11\n", "Id\nId></Id>\n7\n-5\n" },
                    Format0 = "xml",
                    StatusCode = HttpStatusCode.OK
                },

                new ContentAndExpectedOptions() 
                {
                    ResourceUriTemplate = "http://localhost:8080/books",
                    HttpMethod = "POST",
                    Content = "<Book><Name>Abc</Name><Category>a",
                    CursorPos = 33,
                    ExpectedOptions = new string[] { "Adventure\nAdventure\n32\n\n", "Love\nLove\n32\n\n", "History\nHistory\n32\n\n", "Technology\nTechnology\n32\n\n" },
                    Format0 = "xml",
                    StatusCode = HttpStatusCode.OK
                },

                new ContentAndExpectedOptions() 
                {
                    ResourceUriTemplate = "http://localhost:8080/books",
                    HttpMethod = "POST",
                    Content = "<Book><Name>Abc</Name><Category>Adventure</Category><a",
                    CursorPos = 54,
                    ExpectedOptions = new string[] { "IsAvailable\nIsAvailable></IsAvailable>\n53\n-14\n", "Id\nId></Id>\n53\n-5\n" },
                    Format0 = "xml",
                    StatusCode = HttpStatusCode.OK
                }
            })});

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
            const string BaseAddress = "http://localhost:8080/books";

            HttpEndpoint endpoint = new HttpEndpoint(new ContractDescription(typeof(BookService).Name), new EndpointAddress(BaseAddress));
            endpoint.Contract = ContractDescription.GetContract(typeof(BookService));

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            RequestContentInvoker invoker = new RequestContentInvoker(endpoint);
            object[] dummy;

            Asserters.Data.Execute(
                RepresentativeContentAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    ContentAndExpectedOptions contentAndExpectedOptions = (ContentAndExpectedOptions)obj;
                    request.Content = new ActionOfStreamContent(stream =>
                    {
                        Encoding encoding = new UTF8Encoding();
                        byte[] preamble = encoding.GetPreamble();
                        stream.Write(preamble, 0, preamble.Length);
                        byte[] bytes = encoding.GetBytes(contentAndExpectedOptions.Content);
                        stream.Write(bytes, 0, bytes.Length);
                    });

                    HttpResponseMessage response = (HttpResponseMessage)invoker.Invoke(
                        null, 
                        new object[] 
                        { 
                            request,
                            contentAndExpectedOptions.ResourceUriTemplate, 
                            contentAndExpectedOptions.HttpMethod,
                            "autocomplete",
                            contentAndExpectedOptions.CursorPos,
                            contentAndExpectedOptions.Format0,
                            contentAndExpectedOptions.Format1
                        }, 
                        out dummy);

                    this.VerifyAutocompleteResponse(response, contentAndExpectedOptions);
                });
        }


        private void VerifyAutocompleteResponse(HttpResponseMessage response, ContentAndExpectedOptions expectedOptions)
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

        public class ContentAndExpectedOptions
        {
            public string ResourceUriTemplate { get; set; }
            public string Content { get; set; }
            public string HttpMethod { get; set; }
            public int CursorPos { get; set; }
            public string Format0 { get; set; }
            public string Format1 { get; set; }
            public string[] ExpectedOptions { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }
    }
}
