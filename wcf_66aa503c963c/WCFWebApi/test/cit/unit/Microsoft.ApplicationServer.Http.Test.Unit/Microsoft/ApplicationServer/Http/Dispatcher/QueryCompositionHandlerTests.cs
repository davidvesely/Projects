// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.ApplicationServer.Query;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class QueryCompositionHandlerTests : UnitTest<QueryCompositionHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryCompositionHandler is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryCompositionHandler(Type) constructor takes IEnumerable<T>, IQueryable<T> and types implementing IEnumerable<T>.")]
        public void ConstructorTakesIEnumerableType()
        {
            Asserters.Data.Execute(
                 DataSets.WCF.RepresentativeValueAndRefTypes,
                 TestDataVariations.AllCollections,
                 "",
                 (type, obj) =>
                 {
                     QueryCompositionHandler handler = new QueryCompositionHandler(type);

                     Assert.AreEqual(HttpOperationDescription.UnknownName, handler.OperationName, "QueryCompositionHandler(Type) constructor should have an OperationName '{0}'.", HttpOperationDescription.UnknownName);
                 });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryCompositionHandler(Type) constructor takes wrapped IEnumerable<T>, IQueryable<T> and types implementing IEnumerable<T>.")]
        public void ConstructorTakesWrappedIEnumerableType()
        {
            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                     DataSets.WCF.RepresentativeValueAndRefTypes,
                     TestDataVariations.AllCollections,
                     "",
                     (type, obj) =>
                     {
                         QueryCompositionHandler handler = new QueryCompositionHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(HttpOperationDescription.UnknownName, handler.OperationName, "QueryCompositionHandler(Type) constructor should have an OperationName '{0}'.", HttpOperationDescription.UnknownName);
                     });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryCompositionHandler(Type) constructor throws on null Type.")]
        public void ConstructorThrowsOnNullType()
        {
            Asserters.Exception.ThrowsArgumentNull("returnType", () => { new QueryCompositionHandler(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryCompositionHandler(Type) constructor throws when the specified type is not IEnumerable<T>, IQueryable<T>, a type implementing IEnumerable<T> or a wrapped version of these.")]
        public void ConstructorThrowsOnInvalidType()
        {
            Asserters.Data.Execute(
                DataSets.WCF.ValueAndRefTypes,
                TestDataVariations.AllSingleInstances,
                "",
                (type, obj) =>
                {
                    if (type.Equals(typeof(string)))
                    {
                        // string is IEnumerable<char>
                        return;
                    }

                    Asserters.Exception.ThrowsArgument("returnType", () => { new QueryCompositionHandler(type); });

                    foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
                    {
                        Asserters.Exception.ThrowsArgument("returnType", () => { new QueryCompositionHandler(wrapperType.MakeGenericType(type)); });
                    }
                });
        }

        #endregion

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("InputParameters contains two arguments. The first is the HttpRequestMessage, the second is the HttpResponseMessage containing the result of the service operation.")]
        public void InputParametersContainsTwoArguments_HttpRequestMessage_And_ReturnType()
        {
            Asserters.Data.Execute(
                DataSets.WCF.RepresentativeValueAndRefTypes,
                TestDataVariations.AllCollections,
                "",
                (type, obj) =>
                {
                    QueryCompositionHandler handler = new QueryCompositionHandler(type);

                    Assert.AreEqual(2, handler.InputParameters.Count, "QueryCompositionHandler.InputParameters.Count should have returned '2'.");
                    Assert.AreEqual("request", handler.InputParameters[0].Name, "QueryCompositionHandler.InputParameters[0].Name should have returned '{0}'.", "request");
                    Assert.AreEqual(HttpParameter.RequestMessage.ParameterType, handler.InputParameters[0].ParameterType, "QueryCompositionHandler.InputParameters[0].ParameterType should have returned 'HttpRequestMessage'.");
                    Assert.AreEqual("response", handler.InputParameters[1].Name, "QueryCompositionHandler.InputParameters[1].Name should have returned '{0}'.", "response");
                    Assert.AreEqual(HttpParameter.ResponseMessage.ParameterType, handler.InputParameters[1].ParameterType, "QueryCompositionHandler.InputParameters[1].ParameterType should be {0}.", HttpParameter.ResponseMessage.ParameterType);
                });

            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                     DataSets.WCF.RepresentativeValueAndRefTypes,
                     TestDataVariations.AllCollections,
                     "",
                     (type, obj) =>
                     {
                         QueryCompositionHandler handler = new QueryCompositionHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(2, handler.InputParameters.Count, "QueryCompositionHandler.InputParameters.Count should have returned '2'.");
                         Assert.AreEqual("request", handler.InputParameters[0].Name, "QueryCompositionHandler.InputParameters[0].Name should have returned '{0}'.", "request");
                         Assert.AreEqual(HttpParameter.RequestMessage.ParameterType, handler.InputParameters[0].ParameterType, "QueryCompositionHandler.InputParameters[0].ParameterType should have returned 'HttpRequestMessage'.");
                         Assert.AreEqual("response", handler.InputParameters[1].Name, "QueryCompositionHandler.InputParameters[1].Name should have returned '{0}'.", "response");
                         Assert.AreEqual(HttpParameter.ResponseMessage.ParameterType, handler.InputParameters[1].ParameterType, "QueryCompositionHandler.InputParameters[1].ParameterType should be {0}.", HttpParameter.ResponseMessage.ParameterType);
                     });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("OutputParameters contains one argument. It is the IEnumerable version of the return type of the service operation.")]
        public void OutputParametersContainsOneArgument_ReturnType()
        {
            Asserters.Data.Execute(
                DataSets.WCF.RepresentativeValueAndRefTypes,
                TestDataVariations.AllCollections,
                "",
                (type, obj) =>
                {
                    QueryCompositionHandler handler = new QueryCompositionHandler(type);

                    Assert.AreEqual(1, handler.OutputParameters.Count, "QueryCompositionHandler.OutputParameters.Count should have returned '1'.");
                    Assert.AreEqual("emptyDummy", handler.OutputParameters[0].Name, "QueryCompositionHandler.OutputParameters[0].Name should have returned 'output'.");
                    Assert.AreEqual(HttpTypeHelper.HttpResponseMessageType, handler.OutputParameters[0].ParameterType, "QueryCompositionHandler.OutputParameters[1].ParameterType should be {0}.", HttpTypeHelper.HttpResponseMessageType);
                });

            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                     DataSets.WCF.RepresentativeValueAndRefTypes,
                     TestDataVariations.AllCollections,
                     "",
                     (type, obj) =>
                     {
                         QueryCompositionHandler handler = new QueryCompositionHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(1, handler.OutputParameters.Count, "QueryCompositionHandler.OutputParameters.Count should have returned '1'.");
                         Assert.AreEqual("emptyDummy", handler.OutputParameters[0].Name, "QueryCompositionHandler.OutputParameters[0].Name should have returned 'output'.");
                         Assert.AreEqual(HttpTypeHelper.HttpResponseMessageType, handler.OutputParameters[0].ParameterType, "QueryCompositionHandler.OutputParameters[1].ParameterType should be {0}.", HttpTypeHelper.HttpResponseMessageType);
                     });
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle returns the composed query, overriding the service operation result.")]
        public void HandleReturnsTheComposedQuery()
        {
            int skipCount = 2;
            int takeCount = 3;
            string queryUri = String.Format("http://someHost/someEndpoint?$skip={0}&$top={1}", skipCount, takeCount);

            Asserters.Data.Execute(
                DataSets.WCF.ValueAndRefTypes,
                TestDataVariations.AllCollections,
                "",
                (type, obj) =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUri);
                    this.AddQueryOnMessage(type, ref request);

                    QueryCompositionHandler handler = new QueryCompositionHandler(type);
                    Type httpResponseMessageQueryType = HttpTypeHelper.MakeHttpResponseMessageOf(type);
                    object instance = Activator.CreateInstance(httpResponseMessageQueryType, new object[] { obj });
                    object[] output = handler.Handle(new object[] { request, instance });

                    Assert.AreEqual(1, output.Length, "There should be exactly one argument in the output.");

                    this.CheckQueryResult(
                        ((IEnumerable)obj).GetEnumerator(),
                        ((IEnumerable)((ObjectContent)((HttpResponseMessage)output[0]).Content).ReadAsAsync().Result).GetEnumerator(),
                        skipCount,
                        takeCount);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle returns a HttpResponseMessage that has its properties set as the input one.")]
        public void HandleReturnsResponseMessageWithCorrectProperties()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, QueryCompositionHandlerTests.SomeUriWithQuery);
            request.Properties.Add(QueryDeserializationHandler.QueryPropertyName, ODataQueryDeserializer.Deserialize(typeof(DataContractType), request.RequestUri));
            QueryCompositionHandler handler = new QueryCompositionHandler(typeof(IQueryable<DataContractType>));

            HttpResponseMessage<IQueryable<DataContractType>> customInput = new HttpResponseMessage<IQueryable<DataContractType>>(this.queryableList.AsQueryable());
            customInput.Headers.Add("myCustomHeaderResponse", "myCustomHeaderResponseValue");
            customInput.ReasonPhrase = "myReasonPhrase";
            customInput.RequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://myCustomUri"));
            customInput.StatusCode = System.Net.HttpStatusCode.MethodNotAllowed;
            customInput.Version = new Version(0, 9);
            customInput.Content.Headers.Add("myCustomHeadersContent", "myCustomerHeadersContentValue");

            object[] output = handler.Handle(new object[] { request, customInput });

            Assert.AreEqual(1, output.Length);
            HttpResponseMessage outputResult = (HttpResponseMessage)output[0];
            Assert.AreEqual(customInput.Headers.First(k => k.Key == "myCustomHeaderResponse").Value.ElementAt(0),
                            outputResult.Headers.First(k => k.Key == "myCustomHeaderResponse").Value.ElementAt(0),
                            "The Headers property was not copied.");
            Assert.AreEqual(customInput.ReasonPhrase, outputResult.ReasonPhrase, "The ReasonPhrase property was not copied.");
            Assert.AreEqual(customInput.RequestMessage.RequestUri, outputResult.RequestMessage.RequestUri, "The Uri property was not copied.");
            Assert.AreEqual(customInput.StatusCode, outputResult.StatusCode, "The StatusCode property was not copied.");
            Assert.AreEqual(customInput.Version, outputResult.Version, "The Version property was not copied.");
            Assert.AreEqual(customInput.Content.Headers.First(k => k.Key == "myCustomHeadersContent").Value.ElementAt(0),
                            ((ObjectContent)outputResult.Content).Headers.First(k => k.Key == "myCustomHeadersContent").Value.ElementAt(0),
                            "The Headers on the Content was not copied.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle returns the original input if no 'serviceQuery' property is set on the request message.")]
        public void HandleReturnsSourceWhenNoServiceQuerySetOnMessage()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, QueryCompositionHandlerTests.SomeUriWithQuery);
            QueryCompositionHandler handler = new QueryCompositionHandler(typeof(IQueryable<DataContractType>));
            object[] output = handler.Handle(new object[] { request, new HttpResponseMessage<IQueryable<DataContractType>>(this.queryableList.AsQueryable()) });

            Assert.AreEqual(1, output.Length);
            CollectionAssert.AreEqual(
                this.queryableList,
                ((HttpResponseMessage<IQueryable<DataContractType>>)output[0]).Content.ReadAsAsync().Result.ToList(),
                "The composed query resulting from the handle is not the same as the expected one.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle returns null if the service operation input is null.")]
        public void HandleReturnsNullWhenSourceInputIsNull()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, QueryCompositionHandlerTests.SomeUriWithQuery);
            request.Properties.Add(QueryDeserializationHandler.QueryPropertyName, ODataQueryDeserializer.Deserialize(typeof(DataContractType), request.RequestUri));
            QueryCompositionHandler handler = new QueryCompositionHandler(typeof(IQueryable<DataContractType>));
            object[] output = handler.Handle(new object[] { request, null });

            Assert.AreEqual(1, output.Length);
            Assert.IsNull(output[0], "The output should be null when the source input is null.");
        }

        #endregion

        #region TestTypeAndHelpers

        private void AddQueryOnMessage(Type type, ref HttpRequestMessage requestMessage)
        {
            requestMessage.Properties.Add(QueryDeserializationHandler.QueryPropertyName, GenerateQuery(type, requestMessage.RequestUri));
        }

        private IQueryable GenerateQuery(Type type, Uri requestUri)
        {
            if (type.IsArray)
            {
                return ODataQueryDeserializer.Deserialize(type.GetElementType(), requestUri);
            }
            else
            {
                return ODataQueryDeserializer.Deserialize(type.GetGenericArguments()[0], requestUri);
            }
        }

        private void CheckQueryResult(IEnumerator sourceEnumerator, IEnumerator resultEnumerator, int skipCount, int takeCount)
        {
            // Skip in original set.
            for (int i = 0; i < skipCount; i++)
            {
                sourceEnumerator.MoveNext();
            }

            // Check each entry in result correspond to the expected original item.
            for (int i = 0; i < takeCount; i++)
            {
                bool origNext = sourceEnumerator.MoveNext();
                bool resultNext = resultEnumerator.MoveNext();
                Assert.AreEqual(origNext, resultNext, "Both list should have the same amount of content. We encountered the end of one of the list.");
                if (origNext)
                {
                    Assert.AreEqual(
                        sourceEnumerator.Current,
                        resultEnumerator.Current,
                        "The content is not the same. Expected '{0}' but encountered '{1}'",
                        sourceEnumerator.Current.ToString(),
                        resultEnumerator.Current.ToString());
                }
            }

            // Confirms no more item in result.
            Assert.IsFalse(resultEnumerator.MoveNext(), "There should be no more item in the result list.");
        }

        private const string SomeUriWithQuery = "http://someHost/someEndpoint?$skip=1&$top=2";

        private List<DataContractType> queryableList = new List<DataContractType>
        {
            new DataContractType(0, "Entry0"),
            new DataContractType(1, "Entry1"),
            new DataContractType(2, "Entry2"),
            new DataContractType(3, "Entry3 (duplicates)"),
            new DataContractType(3, "Entry3 (duplicates)"),
            new DataContractType(4, "Entry4")
        };

        #endregion
    }
}
