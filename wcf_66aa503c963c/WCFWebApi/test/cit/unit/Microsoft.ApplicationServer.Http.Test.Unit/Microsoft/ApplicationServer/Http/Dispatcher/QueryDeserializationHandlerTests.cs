// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.TestCommon.WCF.Types;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class QueryDeserializationHandlerTests : UnitTest<QueryDeserializationHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryDeserializationHandler is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryDeserializationHandler(Type)constructor takes IEnumerable<T>, IQueryable<T> and types implementing IEnumerable<T>. It assigns a dummy name to the operation.")]
        public void ConstructorTakesIEnumerableType()
        {
            Asserters.Data.Execute(
                 DataSets.WCF.RepresentativeValueAndRefTypes,
                 TestDataVariations.AllCollections,
                 "",
                 (type, obj) =>
                 {
                     QueryDeserializationHandler handler = new QueryDeserializationHandler(type);

                     Assert.AreEqual(HttpOperationDescription.UnknownName, handler.OperationName, "QueryDeserializationHandler() constructor should have an OperationName '{0}'.", HttpOperationDescription.UnknownName);
                 });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryDeserializationHandler(Type)constructor takes wrapped IEnumerable<T>, IQueryable<T> and types implementing IEnumerable<T>. It assigns a dummy name to the operation.")]
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
                         QueryDeserializationHandler handler = new QueryDeserializationHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(HttpOperationDescription.UnknownName, handler.OperationName, "QueryDeserializationHandler() constructor should have an OperationName '{0}'.", HttpOperationDescription.UnknownName);
                     });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryDeserializationHandler(Type) constructor throws when null is specified as the type.")]
        public void ConstructorThrowsOnNullType()
        {
            Asserters.Exception.ThrowsArgumentNull("returnType", () => { new QueryDeserializationHandler(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("QueryDeserializationHandler(Type) constructor throws when the specified type is not IEnumerable<T>, IQueryable<T>, a type implementing IEnumerable<T> or a wrapped version of these.")]
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

                    Asserters.Exception.ThrowsArgument("returnType", () => { new QueryDeserializationHandler(type); });
                    foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
                    {
                        Asserters.Exception.ThrowsArgument("returnType", () => { new QueryDeserializationHandler(wrapperType.MakeGenericType(type)); });
                    }
                });
        }

        #endregion

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("InputParameters contains one argument. It is the HttpRequestMessage")]
        public void InputParametersContainsOneArgument_HttpRequestMessage()
        {
            Asserters.Data.Execute(
                 DataSets.WCF.RepresentativeValueAndRefTypes,
                 TestDataVariations.AllCollections,
                 "",
                 (type, obj) =>
                 {
                     QueryDeserializationHandler handler = new QueryDeserializationHandler(type);

                     Assert.AreEqual(1, handler.InputParameters.Count, "QueryDeserializationHandler.InputParameters.Count should have returned '1'.");
                     Assert.AreEqual("request", handler.InputParameters[0].Name, "QueryDeserializationHandler.InputParameters[0].Name should have returned 'request'.");
                     Assert.AreEqual(HttpParameter.RequestMessage.ParameterType, handler.InputParameters[0].ParameterType, "QueryDeserializationHandler.InputParameters[0].ParameterType should have returned '{0}'.", HttpParameter.RequestMessage.ParameterType.ToString());
                 });

            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                     DataSets.WCF.RepresentativeValueAndRefTypes,
                     TestDataVariations.AllCollections,
                     "",
                     (type, obj) =>
                     {
                         QueryDeserializationHandler handler = new QueryDeserializationHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(1, handler.InputParameters.Count, "QueryDeserializationHandler.InputParameters.Count should have returned '1'.");
                         Assert.AreEqual("request", handler.InputParameters[0].Name, "QueryDeserializationHandler.InputParameters[0].Name should have returned 'request'.");
                         Assert.AreEqual(HttpParameter.RequestMessage.ParameterType, handler.InputParameters[0].ParameterType, "QueryDeserializationHandler.InputParameters[0].ParameterType should have returned '{0}'.", HttpParameter.RequestMessage.ParameterType.ToString());
                     });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("OutputParameters contains one argument. It is an HttpParameter of type Object.")]
        public void OutputParametersContainsOneArgument_Object()
        {
            Asserters.Data.Execute(
                 DataSets.WCF.RepresentativeValueAndRefTypes,
                 TestDataVariations.AllCollections,
                 "",
                 (type, obj) =>
                 {
                     QueryDeserializationHandler handler = new QueryDeserializationHandler(type);

                     Assert.AreEqual(1, handler.OutputParameters.Count, "QueryDeserializationHandler.OutputParameters.Count should have returned '1'.");
                     Assert.AreEqual("emptyDummy", handler.OutputParameters[0].Name, "QueryDeserializationHandler.OutputParameters[0].Name should have returned 'emptyDummy'.");
                     Assert.AreEqual(typeof(Object), handler.OutputParameters[0].ParameterType, "QueryDeserializationHandler.OutputParameters[0].ParameterType should have returned '{0}'.", typeof(Object).ToString());
                 });

            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                     DataSets.WCF.RepresentativeValueAndRefTypes,
                     TestDataVariations.AllCollections,
                     "",
                     (type, obj) =>
                     {
                         QueryDeserializationHandler handler = new QueryDeserializationHandler(wrapperType.MakeGenericType(type));

                         Assert.AreEqual(1, handler.OutputParameters.Count, "QueryDeserializationHandler.OutputParameters.Count should have returned '1'.");
                         Assert.AreEqual("emptyDummy", handler.OutputParameters[0].Name, "QueryDeserializationHandler.OutputParameters[0].Name should have returned 'emptyDummy'.");
                         Assert.AreEqual(typeof(Object), handler.OutputParameters[0].ParameterType, "QueryDeserializationHandler.OutputParameters[0].ParameterType should have returned '{0}'.", typeof(Object).ToString());
                     });
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle returns its input untouched.")]
        public void HandleReturnsInputHttpRequestMessageUnmodified()
        {
            Asserters.Data.Execute(
                 DataSets.WCF.RepresentativeValueAndRefTypes,
                 TestDataVariations.AllCollections,
                 "",
                 (type, obj) =>
                 {
                     QueryDeserializationHandler handler = new QueryDeserializationHandler(type);
                     HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "http://someHost/somePath/somePath/value1/?$skip=1&$top=1");
                     object[] output = handler.Handle(new object[] { message });

                     Assert.AreEqual(1, output.Length, "QueryDeserializationHandler.Handle output array should contain only one value.");
                     Assert.AreEqual(message, output[0], "QueryDeserializationHandler.Handle output[0] should be the same as the input as it is not used.");
                 });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle adds a property on the HttpRequestMessage of type System.Linq.EnumerableQuery<T>.")]
        public void HandleCreatesAnEnumerableQueryOfTheCorrectType()
        {
            Asserters.Data.Execute(
                DataSets.WCF.RepresentativeValueAndRefTypes,
                TestDataVariations.AllCollections,
                "",
                (type, obj) =>
                {
                    QueryDeserializationHandler handler = new QueryDeserializationHandler(type);
                    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "http://someHost/somePath/somePath/value1/?$skip=1&$top=1");
                    object[] output = handler.Handle(new object[] { message });

                    Assert.IsNotNull(message.Properties[QueryDeserializationHandler.QueryPropertyName], "The HttpRequestMessage 'serviceQuery' property should not be null.");
                    Assert.IsTrue(message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType().IsGenericType, "The HttpRequestMessage 'serviceQuery' property should be a generic type.");
                    Assert.AreEqual(
                        typeof(EnumerableQuery<>),
                        message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType().GetGenericTypeDefinition(),
                        "The HttpRequestMessage 'serviceQuery' property should be of generic type definition 'System.Linq.EnumerableQuery<>.");
                    Assert.AreEqual(
                        typeof(EnumerableQuery<>).MakeGenericType(type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0]),
                        message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType(),
                        "The HttpRequestMessage 'serviceQuery' property should be of type 'System.Linq.EnumerableQuery<{0}>.", type.FullName);
                });

            foreach (Type wrapperType in new Type[] { typeof(HttpResponseMessage<>), typeof(ObjectContent<>) })
            {
                Asserters.Data.Execute(
                DataSets.WCF.RepresentativeValueAndRefTypes,
                TestDataVariations.AllCollections,
                "",
                (type, obj) =>
                {
                    Type wrappedType = wrapperType.MakeGenericType(type);
                    QueryDeserializationHandler handler = new QueryDeserializationHandler(wrappedType);
                    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "http://someHost/somePath/somePath/value1/?$skip=1&$top=1");
                    object[] output = handler.Handle(new object[] { message });

                    Assert.IsNotNull(message.Properties[QueryDeserializationHandler.QueryPropertyName], "The HttpRequestMessage 'serviceQuery' property should not be null.");
                    Assert.IsTrue(message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType().IsGenericType, "The HttpRequestMessage 'serviceQuery' property should be a generic type.");
                    Assert.AreEqual(
                        typeof(EnumerableQuery<>),
                        message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType().GetGenericTypeDefinition(),
                        "The HttpRequestMessage 'serviceQuery' property should be of generic type definition 'System.Linq.EnumerableQuery<>.");
                    Assert.AreEqual(
                        typeof(EnumerableQuery<>).MakeGenericType(type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0]),
                        message.Properties[QueryDeserializationHandler.QueryPropertyName].GetType(),
                        "The HttpRequestMessage 'serviceQuery' property should be of type 'System.Linq.EnumerableQuery<{0}>.", type.FullName);
                });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle works if the query string specified in the URL contains parameter that are not starting with $.")]
        public void HandleProcessQueryStringThatContainsNonDollarParam()
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "http://someHost/somePath/?$skip=1&myOwnParam1=somevalue&$top=1&myownparam2");
            QueryDeserializationHandler handler = new QueryDeserializationHandler(typeof(IQueryable<string>));
            handler.Handle(new object[] { message });

            Assert.IsNotNull(message.Properties[QueryDeserializationHandler.QueryPropertyName], "The HttpRequestMessage 'serviceQuery' property should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("cscro")]
        [Description("Handle throws if the query string specified in the URL contains invalid $param.")]
        public void HandleThrowsIfQueryStringContainsInvalidDollarParam()
        {
            Asserters.Exception.Throws<HttpRequestException>(SR.UriQueryStringInvalid,
                () =>
                {
                    QueryDeserializationHandler handler = new QueryDeserializationHandler(typeof(IQueryable<string>));
                    handler.Handle(new object[] { new HttpRequestMessage(HttpMethod.Get, "http://someHost/somePath/?$invalidarg=1") });
                });
        }

        #endregion
    }
}
