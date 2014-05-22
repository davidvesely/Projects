// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
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

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(RequestUriInvoker))]
    public class RequestUriInvokerTests : UnitTest
    {
        private const string TestUriTemplate = "http://localhost:8080/books/{category}?id={id}&name={name}&isavailable  ={available}";

        public static readonly ReadOnlyCollection<TestData> RepresentativeUriAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<UriAndExpectedOptions>(() => new List<UriAndExpectedOptions>()
            {
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/{category}?name=Harry Potter&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 28, // http://localhost:8080/books/|{category}?name=Harry Potter&Id=9999
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"Adventure\nAdventure\n28\n\n", "Love\nLove\n28\n\n", "History\nHistory\n28\n\n", "Technology\nTechnology\n28\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?name=Harry Potter&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 29, // http://localhost:8080/books/a|dventure?name=Harry Potter&Id=9999
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"Adventure\nAdventure\n28\n\n", "Love\nLove\n28\n\n", "History\nHistory\n28\n\n", "Technology\nTechnology\n28\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/detective?name=Harry Potter&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 36,  // http://localhost:8080/books/detectiv|e?name=Harry Potter&Id=9999
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"Adventure\nAdventure\n28\n\n", "Love\nLove\n28\n\n", "History\nHistory\n28\n\n", "Technology\nTechnology\n28\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/detective?name=Harry Potter&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 37,  // http://localhost:8080/books/detective|?name=Harry Potter&Id=9999
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"Adventure\nAdventure\n28\n\n", "Love\nLove\n28\n\n", "History\nHistory\n28\n\n", "Technology\nTechnology\n28\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable=true&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 50, // "http://localhost:8080/books/adventure?isavailable=|true&Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"true\ntrue\n50\n\n", "false\nfalse\n50\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable =true&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 51, // "http://localhost:8080/books/adventure?isavailable =|true&Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"true\ntrue\n51\n\n", "false\nfalse\n51\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 60, // "http://localhost:8080/books/adventure?isavailable={available|}&Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"true\ntrue\n50\n\n", "false\nfalse\n50\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 61, // "http://localhost:8080/books/adventure?isavailable={available}|&Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"true\ntrue\n50\n\n", "false\nfalse\n50\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 42, // "http://localhost:8080/books/adventure?isav|ailable={available}&Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"id\nid\n38\n\n", "name\nname\n38\n\n", "isavailable\nisavailable\n38\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 62, // "http://localhost:8080/books/adventure?isavailable={available}&|Id=9999"
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] {"id\nid\n62\n\n", "name\nname\n62\n\n", "isavailable\nisavailable\n62\n\n"},
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "http://localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    HttpMethod = "GET",
                    CursorPos = 5, // "http:|//localhost:8080/books/adventure?isavailable={available}&Id=9999",
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = new string[] { "http://localhost:8080/books/\nhttp://localhost:8080/books/\n0\n\n" },
                    StatusCode = HttpStatusCode.OK
                },
                new UriAndExpectedOptions() 
                {
                    Uri = "Bad Uri",
                    HttpMethod = "GET",
                    CursorPos = 5,
                    ResourceUriTemplate = TestUriTemplate,
                    ExpectedOptions = null,
                    StatusCode = HttpStatusCode.BadRequest
                },
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
        [Description("Invoke(object, object[], out object[]) returns available intellisense options.")]
        public void InvokeReturnsIntellisenseOptions()
        {
            const string BaseAddress = "http://localhost:8080/books";

            HttpEndpoint endpoint = new HttpEndpoint(new ContractDescription(typeof(BookService).Name), new EndpointAddress(BaseAddress));
            endpoint.Contract = ContractDescription.GetContract(typeof(BookService));

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            RequestUriInvoker invoker = new RequestUriInvoker(endpoint);
            object[] dummy;

            Asserters.Data.Execute(
                RepresentativeUriAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    UriAndExpectedOptions uriAndExpectedOptions = (UriAndExpectedOptions)obj;

                    Debug.WriteLine(string.Format("testing {0} @ {1}", uriAndExpectedOptions.Uri, uriAndExpectedOptions.CursorPos));

                    request.Content = new ActionOfStreamContent(stream =>
                    {
                        Encoding encoding = new UTF8Encoding();
                        byte[] preamble = encoding.GetPreamble();
                        stream.Write(preamble, 0, preamble.Length);
                        byte[] bytes = encoding.GetBytes(uriAndExpectedOptions.Uri);
                        stream.Write(bytes, 0, bytes.Length);
                    });

                    HttpResponseMessage response = (HttpResponseMessage)invoker.Invoke(
                        null,
                        new object[] 
                        { 
                            request, 
                            uriAndExpectedOptions.ResourceUriTemplate, 
                            uriAndExpectedOptions.HttpMethod, 
                            "autocomplete", 
                            uriAndExpectedOptions.CursorPos 
                        },
                        out dummy);

                    this.VerifyAutocompleteResponse(response, uriAndExpectedOptions);
                });
        }

        private void VerifyAutocompleteResponse(HttpResponseMessage response, UriAndExpectedOptions expectedOptions)
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

        public class UriAndExpectedOptions
        {
            public string Uri { get; set; }
            public string HttpMethod { get; set; }
            public string ResourceUriTemplate { get; set; }
            public int CursorPos { get; set; }
            public string[] ExpectedOptions { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }
    }
}
