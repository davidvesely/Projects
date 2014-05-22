// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Xml;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryHandlersScenarioTests
    {
        public static List<string> SampleQueryableList = new List<string>
        {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
        };

        static string[] ContentTypes = new string[]
        {
            "application/xml",
            "text/xml",
            "application/json",
            "text/json",
            "text/html",
            "application/atom+xml",
        };

        static Tuple<Type, string[], bool>[] QueryHandlerTestParams = new Tuple<Type, string[], bool>[]
        {
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceIQueryable), new string[] {"WebGetReturnsIQueryableFromList"}, false),
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceIEnumerable), new string[] {"WebGetReturnsIEnumerableFromList"}, false),
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceIList), new string[] {"WebGetReturnsIListFromList"}, false),
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceConcreteList), new string[] {"WebGetReturnsConcreteList"}, false),
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceWrapped), new string[] {"WebGetReturnsHttpResponseMessageWrappedIQueryable", "WebGetReturnsHttpResponseMessageWrappedIQueryable"}, false),
            new Tuple<Type, string[], bool>(typeof(QueryOperationsServiceDataContract), new string[] {"WebGetReturnsIQueryableFromListDataContract", "WebGetReturnsIEnumerableFromListDataContract", "WebGetReturnsIListFromListDataContract", "WebGetReturnsConcreteListDataContract"}, true),
        };

        static KeyValuePair<string, List<string>>[] QuerysAndExpectedResults = new KeyValuePair<string, List<string>>[]
        {
            new KeyValuePair<string, List<string>>("?$skip=2&$top=3", SampleQueryableList.AsQueryable().Skip(2).Take(3).ToList()),
            new KeyValuePair<string, List<string>>("", SampleQueryableList),
        };

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Test functionality of the Query Composition feature end-to-end for supported scenarios.")]
        public void TestSimpleQueryableServiceOperations()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(Test), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = baseAddress;
                HttpRequestMessage request;
                HttpResponseMessage response; 
                
                // test the default query
                request = new HttpRequestMessage(HttpMethod.Get, "test?$top=1");
                response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(1, response.Content.ReadAsAsync<string[]>().Result.Count<string>());
                }

                // test json with query
                request = new HttpRequestMessage(HttpMethod.Get, "test?$top=1");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(1, response.Content.ReadAsAsync<string[]>().Result.Count<string>());
                }

                // test json without query
                request = new HttpRequestMessage(HttpMethod.Get, "test");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(2, response.Content.ReadAsAsync<string[]>().Result.Count<string>());
                }
            }

        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Test functionality of the Query Composition feature end-to-end for supported scenarios.")]
        public void TestQueryableServiceOperations()
        {
            foreach (Tuple<Type, string[], bool> testParams in QueryHandlerTestParams)
            {
                this.RunTestWithParams(testParams.Item1, testParams.Item2, testParams.Item3);
            }
        }

        private void RunTestWithParams(Type serviceType, string[] operationsCollection, bool useDC)
        {
            var baseAddress = new Uri("http://localhost:8080/");
            var config = this.CreateConfig(useDC);
            var host = new HttpServiceHost(serviceType, config, new Uri[] { baseAddress });

            using (host)
            {
                host.Open();

                foreach (string contentType in ContentTypes)
                {
                    Console.WriteLine("Testing with a client requesting [Content-Type: {0}]", contentType);
                    var client = new HttpClient();
                    client.BaseAddress = baseAddress;
                    client.Timeout = TimeSpan.FromHours(2);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                    foreach (string serviceOp in operationsCollection)
                    {
                        Console.WriteLine("Service Operation Uri {0}/{1}", baseAddress, serviceOp);

                        foreach (KeyValuePair<string, List<string>> queryAndExpectedResult in QuerysAndExpectedResults)
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, serviceOp + queryAndExpectedResult.Key);

                            try
                            {
                                Console.WriteLine("Issued client call with query {0}.", queryAndExpectedResult.Key);
                                var response = client.SendAsync(request).Result;
                                using (response)
                                {
                                    Console.WriteLine("Completed client call with query {0}.", queryAndExpectedResult.Key);
                                    if (response.StatusCode == HttpStatusCode.OK)
                                    {
                                        string expectedContentType = contentType == "text/html" ? "application/xml" : contentType;
                                        // Following line accounts for a bug in ODataMediaFormatter that does not set the correct MediaType on the headers.
                                        expectedContentType = contentType == "application/atom+xml" ? "application/xml" : expectedContentType;
                                        Assert.AreEqual(
                                            expectedContentType,
                                            response.Content.Headers.ContentType.MediaType,
                                            "Expected content-type '{0}', but received '{1}'",
                                            expectedContentType,
                                            response.Content.Headers.ContentType.MediaType);

                                        if (contentType != "application/atom+xml")
                                        {
                                            List<string> expected = queryAndExpectedResult.Value;
                                            List<string> actual = ((List<string>)response.Content.ReadAsAsync(
                                                typeof(List<string>),
                                                new List<MediaTypeFormatter> 
                                                { 
                                                    new XmlMediaTypeFormatter { UseDataContractSerializer = useDC },
                                                    new JsonMediaTypeFormatter()
                                                }).Result);

                                            Assert.AreEqual(expected.Count, actual.Count, "Returned value does not have the same number of item as the expected value.");
                                            CollectionAssert.AreEqual(expected, actual, "The collection received is not the one expected.");
                                            Console.WriteLine("Success.");
                                        }
                                        else
                                        {
                                            // Can't deserialize OData Feeds, so count the number of nodes expected in the xml as validation.
                                            XmlDocument xmlDoc = new XmlDocument();
                                            xmlDoc.LoadXml(new StreamReader(response.Content.ReadAsStreamAsync().Result).ReadToEnd());
                                            int receivedCount = xmlDoc.GetElementsByTagName("element").Count;
                                            Assert.AreEqual(queryAndExpectedResult.Value.Count, receivedCount, "Expected {0} results, received {1}.", queryAndExpectedResult.Value.Count, receivedCount);
                                        }
                                    }
                                    else
                                    {
                                        Assert.Fail("Call failed. Reason: {0}", response.ReasonPhrase);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Assert.Fail("Failed. Exception: {0}", ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private HttpConfiguration CreateConfig(bool useDC)
        {
            var config = new HttpConfiguration();
            config.Formatters.XmlFormatter.UseDataContractSerializer = useDC;
            config.Formatters.Add(new ODataMediaTypeFormatter());

            config.RequestHandlers = (handlers, endpoint, operation) =>
            {
                if (handlers.Where(a => a.GetType().Equals(typeof(QueryDeserializationHandler))).ToArray().Length == 0)
                {
                    try
                    {
                        if (IsGenericIEnumerable(UnwrapType(operation.ReturnValue.ParameterType)))
                        {
                            handlers.Add(new QueryDeserializationHandler(operation.ReturnValue.ParameterType));
                            Console.WriteLine("Added QueryDeserializationHandler on service operation {0} that returns {1}.", operation.Name, operation.ReturnValue.ParameterType);
                        }
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Could not add QueryDeserializationHandler on service operation {0} that returns {1}. Error Message is {2}", operation.Name, operation.ReturnValue.ParameterType, ex.Message);
                    }
                }
            };

            config.ResponseHandlers = (handlers, endpoint, operation) =>
            {
                if (handlers.Where(a => a.GetType().Equals(typeof(QueryCompositionHandler))).ToArray().Length == 0)
                {
                    try
                    {
                        if (IsGenericIEnumerable(UnwrapType(operation.ReturnValue.ParameterType)))
                        {
                            handlers.Add(new QueryCompositionHandler(operation.ReturnValue.ParameterType));
                            Console.WriteLine("Added QueryCompositionHandler on service operation {0} that returns {1}.", operation.Name, operation.ReturnValue.ParameterType);
                        }
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Could not add QueryCompositionHandler on service operation {0} that returns {1}. Error Message is {2}", operation.Name, operation.ReturnValue.ParameterType, ex.Message);
                    }
                }
            };

            return config;
        }

        private static Type UnwrapType(Type typeToUnwrap)
        {
            if (typeToUnwrap == null)
            {
                return null;
            }

            if (typeToUnwrap.IsGenericType
                && (typeToUnwrap.GetGenericTypeDefinition().Equals(typeof(HttpResponseMessage<>))
                    || typeToUnwrap.GetGenericTypeDefinition().Equals(typeof(ObjectContent<>))))
            {
                return typeToUnwrap.GetGenericArguments()[0];
            }
            else
            {
                return typeToUnwrap;
            }
        }

        private static bool IsGenericIEnumerable(Type type)
        {
            if (type == null)
            {
                return false;
            }

            // Type must be IEnumerable<T> or implement IEnumerable<T>
            if ((type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                || type.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                return true;
            }

            return false;
        }
    }

    [ServiceContract]
    public class QueryOperationsServiceIQueryable
    {
        [WebGet]
        public IQueryable<string> WebGetReturnsIQueryableFromList()
        {
            return QueryHandlersScenarioTests.SampleQueryableList.AsQueryable();
        }
    }

    [ServiceContract]
    public class QueryOperationsServiceIEnumerable
    {
        [WebGet]
        public IEnumerable<string> WebGetReturnsIEnumerableFromList()
        {
            return QueryHandlersScenarioTests.SampleQueryableList.AsEnumerable();
        }
    }

    [ServiceContract]
    public class QueryOperationsServiceIList
    {
        [WebGet]
        public IList<string> WebGetReturnsIListFromList()
        {
            return QueryHandlersScenarioTests.SampleQueryableList;
        }
    }

    [ServiceContract]
    public class QueryOperationsServiceConcreteList
    {
        [WebGet]
        public List<string> WebGetReturnsConcreteList()
        {
            return QueryHandlersScenarioTests.SampleQueryableList;
        }
    }

    [ServiceContract]
    public class QueryOperationsServiceWrapped
    {
        [WebGet]
        public HttpResponseMessage<IQueryable<string>> WebGetReturnsHttpResponseMessageWrappedIQueryable()
        {
            return new HttpResponseMessage<IQueryable<string>>(QueryHandlersScenarioTests.SampleQueryableList.AsQueryable());
        }

        [WebGet]
        public ObjectContent<IEnumerable<string>> WebGetReturnsObjectContentWrappedIEnumerable()
        {
            return new ObjectContent<IEnumerable<string>>(QueryHandlersScenarioTests.SampleQueryableList.AsEnumerable());
        }
    }

    [ServiceContract]
    [DataContractFormat]
    public class QueryOperationsServiceDataContract
    {
        [WebGet]
        [DataContractFormat]
        public IQueryable<string> WebGetReturnsIQueryableFromListDataContract()
        {
            return QueryHandlersScenarioTests.SampleQueryableList.AsQueryable();
        }

        [WebGet]
        [DataContractFormat]
        public IEnumerable<string> WebGetReturnsIEnumerableFromListDataContract()
        {
            return QueryHandlersScenarioTests.SampleQueryableList.AsEnumerable();
        }

        [WebGet]
        [DataContractFormat]
        public IList<string> WebGetReturnsIListFromListDataContract()
        {
            return QueryHandlersScenarioTests.SampleQueryableList;
        }

        [WebGet]
        [DataContractFormat]
        public List<string> WebGetReturnsConcreteListDataContract()
        {
            return QueryHandlersScenarioTests.SampleQueryableList;
        }
    }

    [ServiceContract]
    public class Test
    {
        [WebGet(UriTemplate = "test")]
        public IQueryable<string> GetStuff()
        {
            return new string[] { "test1", "test2" }.AsQueryable();
        }
    } 


}
