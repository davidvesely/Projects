// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class RequestContentHandlerTests : UnitTest<RequestContentHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler is public and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler() constructor initializes parameters and formatters.")]
        public void Constructor()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(int));
            HttpParameter expectedContentParameter = new HttpParameter("x", typeof(HttpRequestMessage<int>));
            MediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[] { formatter };
            RequestContentHandler handler = new RequestContentHandler(hpd, formatters);
            Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
            Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
            CollectionAssert.Contains(handler.Formatters, formatter, "Failed to accept mediaTypeFormatter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler() constructor throws for null HttpParameter.")]
        public void ConstructorThrowsWithNullHttpParameter()
        {
            Asserters.Exception.ThrowsArgumentNull("requestContentParameter", () => new RequestContentHandler(null, Enumerable.Empty<MediaTypeFormatter>()));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler() constructor throws for null Formatters.")]
        public void ConstructorThrowsWithNullHttpFormatters()
        {
            Asserters.Exception.ThrowsArgumentNull("formatters", () => new RequestContentHandler(new HttpParameter("x", typeof(int)), null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler() constructor accepts parameter of HttpContent.")]
        public void ConstructorAcceptsHttpContent()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(HttpContent));
            HttpParameter expectedContentParameter = new HttpParameter("x", typeof(HttpContent));
            RequestContentHandler handler = new RequestContentHandler(hpd, new MediaTypeFormatter[0]);
            Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
            Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestContentHandler() constructor accepts parameter of ObjectContent<T>.")]
        public void ConstructorAcceptsObjectContentOfT()
        {
            Asserters.Data.Execute(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                "InputParameters for all types failed.",
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();

                    // typeof(ObjectContent<T>)
                    Type objectContentOfTType = typeof(ObjectContent<>).MakeGenericType(convertType);

                    // typeof(HttpRequestMessage<T>)
                    Type httpRequestMessageOfTType = typeof(HttpRequestMessage<>).MakeGenericType(convertType);

                    HttpParameter hpd = new HttpParameter("x", objectContentOfTType);
                    HttpParameter expectedContentParameter = new HttpParameter("x", httpRequestMessageOfTType);
                    RequestContentHandler handler = new RequestContentHandler(hpd, new MediaTypeFormatter[0]);
                    Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
                    Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
                });
        }

        #endregion Constructors

        #region Properties

        #region Formatters
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Formatters are initialized to the standard formatters if none are supplied.")]
        public void FormattersHaveStandardFormatters()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(int));
            RequestContentHandler handler = new RequestContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
            MediaTypeFormatterCollection formatters = handler.Formatters;
            Assert.IsNotNull(formatters, "Formatters was null.");
            Assert.IsNotNull(formatters.XmlFormatter != null, "Xml formatter was not set.");
            Assert.IsNotNull(formatters.JsonFormatter != null, "Json formatter was not set.");
        }

        #endregion Formatters

        #region InputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("InputParameters contains only HttpParameter of HttpRequestMessage for all legal types for content.")]
        public void InputParametersContainHttpRequestMessageWithAllValueAndReferenceTypes()
        {
            Asserters.Data.Execute(
            DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
            TestDataVariations.All,
            "InputParameters for all types failed.",
            (type, obj) =>
            {
                Type convertType = obj == null ? type : obj.GetType();
                HttpParameter hpd = new HttpParameter("x", convertType);
                Type expectedType = typeof(HttpRequestMessage<>).MakeGenericType(convertType);
                HttpParameter expectedContentParameter = new HttpParameter("x", expectedType);
                RequestContentHandler handler = new RequestContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
                Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
            });
        }

        #endregion InputParameters

        #region OutputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OutputParameters contains only HttpParameter of HttpRequestMessage<> for all legal types for content.")]
        public void OutputParametersContainHttpRequestMessageOfTWithAllValueAndReferenceTypes()
        {
            Asserters.Data.Execute(
            DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
            TestDataVariations.All,
            "OutputParameters all types failed.",
            (type, obj) =>
            {
                Type convertType = obj == null ? type : obj.GetType();
                HttpParameter hpd = new HttpParameter("x", convertType);
                Type expectedType = typeof(HttpRequestMessage<>).MakeGenericType(convertType);
                HttpParameter expectedContentParameter = new HttpParameter("x", expectedType);
                RequestContentHandler handler = new RequestContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
                Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, string.Format("Failed to initialize content parameter for {0}.", convertType.Name));
            });
        }

        #endregion OutputParameters

        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) returns HttpRequestMessage<T> with HttpRequestMessage for all representative types.")]
        public void HandleReturnsHttpRequestMessageOfTWithHttpRequestMessage()
        {
            MediaTypeFormatter formatter = new XmlMediaTypeFormatter();

            Asserters.Data.Execute(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                "Converting to HttpRequestMessage<T> for all types failed.",
                (type, obj) =>
                {
                    type = obj == null ? type : obj.GetType();
                    HttpRequestMessage requestMessage = new HttpRequestMessage();
                    requestMessage.Content = new ObjectContent(type, obj);
                    Type expectedRequestMessageType = typeof(HttpRequestMessage<>).MakeGenericType(type);
                    RequestContentHandler handler = new RequestContentHandler(new HttpParameter("x", type),
                        new MediaTypeFormatter[] { formatter });

                    object[] output = handler.Handle(new object[] { requestMessage });

                    Assert.AreEqual(1, output.Length, "Output length mismatch.");
                    Assert.AreEqual(expectedRequestMessageType, output[0].GetType(), "Output HttpRequestMessage type mismatch.");
                    CollectionAssert.Contains(handler.Formatters, formatter, "Output HttpRequestMessage formatter mismatch");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) returns HttpRequestMessage<T> with HttpRequestMessage<T> for all representative types.")]
        public void HandleReturnsHttpRequestMessageOfTWithHttpRequestMessageOfT()
        {
            MediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            
            Asserters.Data.Execute(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                "Converting to HttpRequestMessage<T> for all types failed.",
                (type, obj) =>
                {
                    type = obj == null ? type : obj.GetType();
                    Type genericRequestMessageType = typeof(HttpRequestMessage<>).MakeGenericType(type);
                    HttpRequestMessage requestMessage = (HttpRequestMessage)Activator.CreateInstance(genericRequestMessageType, obj);
                    RequestContentHandler handler = new RequestContentHandler(new HttpParameter("x", genericRequestMessageType),
                        new MediaTypeFormatter[] { formatter });

                    object[] output = handler.Handle(new object[] { requestMessage });

                    Assert.AreEqual(1, output.Length, "Output length mismatch.");
                    Assert.AreEqual(genericRequestMessageType, output[0].GetType(), "Output HttpRequestMessage type mismatch.");
                    CollectionAssert.Contains(handler.Formatters, formatter, "Output HttpRequestMessage formatter mismatch");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) throws for null content.")]
        public void HandleThrowsWithNullHttpContent()
        {
            RequestContentHandler handler = new RequestContentHandler(new HttpParameter("x", typeof(int)),
                new MediaTypeFormatter[] { new XmlMediaTypeFormatter() });

            Asserters.Data.Execute(
                DataSets.Http.NullContentHttpRequestMessages,
                TestDataVariations.AsInstance,
                "Handle(object[]) failed to throw for null content.",
                (type, obj) =>
                {
                    Asserters.Exception.ThrowsArgumentNull("content", () =>
                    {
                        handler.Handle(new object[] { obj });
                    });
                });  
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) throws for non-HttpRequestMessage.")]
        public void HandleThrowsWithNonHttpRequestMessage()
        {
            Asserters.Data.Execute(
               DataSets.Http.NonHttpRequestMessages,
               TestDataVariations.All,
               "Handle failed to throw correctly for non-HttpRequestMessage",
               (type, obj) =>
               {
                   type = obj == null ? type : obj.GetType();
                   RequestContentHandler handler = new RequestContentHandler(new HttpParameter("x", type),
                       new MediaTypeFormatter[] { new XmlMediaTypeFormatter() });
                   Asserters.Exception.Throws<InvalidOperationException>(
                       () =>
                       {
                           handler.Handle(new object[] { obj });
                       },
                       (e) =>
                       {
                           string expectedMessage = SR.HttpOperationHandlerReceivedWrongType(
                               typeof(HttpOperationHandler).Name, handler.ToString(), handler.OperationName, 
                               handler.InputParameters[0].ParameterType.Name, handler.InputParameters[0].Name, type.Name);
                           Assert.AreEqual(expectedMessage, e.Message);
                       });
               }
           );
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) throws for null request message.")]
        public void HandleThrowsWithNullHttpRequestMessage()
        {
            RequestContentHandler handler = new RequestContentHandler(new HttpParameter("x", typeof(int)),  
                new MediaTypeFormatter[] { new XmlMediaTypeFormatter() });
            
            Asserters.Exception.ThrowsArgumentNull(HttpParameter.RequestMessage.Name, () =>
                {
                    handler.Handle(new object[] { null });
                });
        }

        #endregion Methods
    }
}
