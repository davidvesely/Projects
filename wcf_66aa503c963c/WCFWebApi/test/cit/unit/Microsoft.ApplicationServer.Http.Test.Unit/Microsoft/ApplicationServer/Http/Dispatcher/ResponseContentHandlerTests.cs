// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class ResponseContentHandlerTests : UnitTest<ResponseContentHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler is public and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler() constructor initializes parameters and formatters.")]
        public void Constructor()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(int));
            HttpParameter expectedContentParameter = new HttpParameter("x", typeof(HttpResponseMessage<int>));
            MediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[] { formatter };
            ResponseContentHandler handler = new ResponseContentHandler(hpd, formatters);
            Asserters.HttpParameter.Contains(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters for RequestMessage.");
            Asserters.HttpParameter.Contains(handler.InputParameters, hpd, "Failed to initialize input parameter.");
            Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
            CollectionAssert.Contains(handler.Formatters, formatter, "Failed to accept mediaTypeFormatter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler() constructor throws for null Formatters.")]
        public void ConstructorThrowsWithNullHttpFormatters()
        {
            Asserters.Exception.ThrowsArgumentNull("formatters", () => new ResponseContentHandler(new HttpParameter("x", typeof(int)), null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler() constructor accepts parameter of HttpContent.")]
        public void ConstructorAcceptsHttpContent()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(HttpContent));
            HttpParameter expectedContentParameter = new HttpParameter("x", typeof(HttpResponseMessage));
            ResponseContentHandler handler = new ResponseContentHandler(hpd, new MediaTypeFormatter[0]);
            Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
            Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler() constructor accepts parameter of ObjectContent.")]
        public void ConstructorAcceptsObjectContent()
        {
            HttpParameter hpd = new HttpParameter("x", typeof(ObjectContent));
            HttpParameter expectedContentParameter = new HttpParameter("x", typeof(HttpResponseMessage));
            ResponseContentHandler handler = new ResponseContentHandler(hpd, new MediaTypeFormatter[0]);
            Asserters.HttpParameter.ContainsOnly(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters to HttpRequestMessage.");
            Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ResponseContentHandler() constructor accepts parameter of ObjectContentOfT.")]
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
                    Type httpResponseMessageOfTType = typeof(HttpResponseMessage<>).MakeGenericType(convertType);

                    HttpParameter hpd = new HttpParameter("x", objectContentOfTType);
                    HttpParameter expectedContentParameter = new HttpParameter("x", httpResponseMessageOfTType);
                    ResponseContentHandler handler = new ResponseContentHandler(hpd, new MediaTypeFormatter[0]);
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
            ResponseContentHandler handler = new ResponseContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
            MediaTypeFormatterCollection formatters = handler.Formatters;
            Assert.IsNotNull(formatters, "Formatters was null.");
            Assert.IsNotNull(formatters.XmlFormatter != null, "Xml formatter was not set.");
            Assert.IsNotNull(formatters.JsonFormatter != null, "Json formatter was not set.");
        }

        #endregion Formatters

        #region InputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("InputParameters are created a content parameter for all legal types for content.")]
        public void InputParameterAreCreatedAllValueAndReferenceTypes()
        {
            Asserters.Data.Execute(
            DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
            TestDataVariations.All,
            "InputParameters for all types failed.",
            (type, obj) =>
            {
                Type convertType = obj == null ? type : obj.GetType();
                HttpParameter hpd = new HttpParameter("x", convertType);
                Type expectedType = typeof(HttpResponseMessage<>).MakeGenericType(convertType);
                HttpParameter expectedContentParameter = new HttpParameter("x", expectedType);
                ResponseContentHandler handler = new ResponseContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
                Asserters.HttpParameter.Contains(handler.InputParameters, HttpParameter.RequestMessage, "Failed to initialize input parameters for RequestMessage.");
                Asserters.HttpParameter.Contains(handler.InputParameters, hpd, "Failed to initialize input parameter.");
            });
        }

        #endregion InputParameters

        #region OutputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OutputParameters are created a content parameter for all legal types for content.")]
        public void OutputParameterAreCreatedAllValueAndReferenceTypes()
        {
            Asserters.Data.Execute(
            DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
            TestDataVariations.All,
            "OutputParameters all types failed.",
            (type, obj) =>
            {
                Type convertType = obj == null ? type : obj.GetType();
                HttpParameter hpd = new HttpParameter("x", convertType);
                Type expectedType = typeof(HttpResponseMessage<>).MakeGenericType(convertType);
                HttpParameter expectedContentParameter = new HttpParameter("x", expectedType);
                ResponseContentHandler handler = new ResponseContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());
                Asserters.HttpParameter.ContainsOnly(handler.OutputParameters, expectedContentParameter, "Failed to initialize content parameter.");
            });
        }

        #endregion OutputParameters


        #endregion Properties

        #region Methods

        #region FormattersMergeLogic

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {<none>} + {my} => {my}")]
        public void MergeHandlerWithClearedHost()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection(new MediaTypeFormatter[] { new MyMediaFormatter() });

            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(1, collection.Count, "Expected only my formatter in merged collection");
            Assert.IsInstanceOfType(collection[0], typeof(MyMediaFormatter), "Element[0] should be MyMediaTypeFormatter.");
        }

        [TestMethod]
        [Ignore] // this deletion does not work, the MediaTypeFormatterCollection will always add the default back
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml, my} + {xml, json} => {xml, my}")]
        public void MergeHandlerWithDelete()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            hostLevelFormatters.RemoveAt(1); // remove the json one
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(2, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(XmlMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[1], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [Ignore]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml, json, my} + {xml } => {xml, my}")]
        public void MergeHandlerWithOperationLevelDelete()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(2, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(XmlMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[1], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml, json, my} + {my1, xml, json} => {my1, xml, json, my}")]
        public void MergeHandlerOperationLevelPrepend()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            operationLevelFormatters.Insert(0, new MyMediaFormatter1());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(6, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(MyMediaFormatter1));
            Assert.IsInstanceOfType(collection[1], typeof(XmlMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[2], typeof(JsonValueMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[3], typeof(JsonMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[4], typeof(FormUrlEncodedMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[5], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml1, json1, my} + {xml, json} => {xml1, json1, my}")]
        public void MergeHandlerModified()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            hostLevelFormatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/foo"));
            hostLevelFormatters.JsonValueFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            hostLevelFormatters.JsonFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            hostLevelFormatters.FormUrlEncodedFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(5, collection.Count);
            Assert.AreSame(collection[0], hostLevelFormatters.XmlFormatter);
            Assert.AreSame(collection[1], hostLevelFormatters.JsonValueFormatter);
            Assert.AreSame(collection[2], hostLevelFormatters.JsonFormatter);
            Assert.AreSame(collection[3], hostLevelFormatters.FormUrlEncodedFormatter);
            Assert.IsInstanceOfType(collection[4], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml, json, my} + {xml, json} => {xml, json, my}")]
        public void MergeHandlerCommon()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(5, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(XmlMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[1], typeof(JsonValueMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[2], typeof(JsonMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[3], typeof(FormUrlEncodedMediaTypeFormatter));
            Assert.IsInstanceOfType(collection[4], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {xml, json, my} + {xml1, json1, my1} => {xml1, json1, my1, my}")]
        public void MergeHandlerModifiedAtOperation()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.Add(new MyMediaFormatter());
            operationLevelFormatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/foo"));
            operationLevelFormatters.JsonFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            operationLevelFormatters.JsonValueFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            operationLevelFormatters.FormUrlEncodedFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            operationLevelFormatters.Add(new MyMediaFormatter1());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(6, collection.Count);
            Assert.AreSame(collection[0], operationLevelFormatters.XmlFormatter);
            Assert.AreSame(collection[1], operationLevelFormatters.JsonValueFormatter);
            Assert.AreSame(collection[2], operationLevelFormatters.JsonFormatter);
            Assert.AreSame(collection[3], operationLevelFormatters.FormUrlEncodedFormatter);
            Assert.IsInstanceOfType(collection[4], typeof(MyMediaFormatter1));
            Assert.IsInstanceOfType(collection[5], typeof(MyMediaFormatter));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle should merge formatter correctly: {my, xml1, json} + {xml, json1, my1} => {json1, my1, my, xml1}")]
        public void MergeHandlerModifiedAtOperationMixed()
        {
            MediaTypeFormatterCollection hostLevelFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatterCollection operationLevelFormatters = new MediaTypeFormatterCollection();

            hostLevelFormatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/foo"));
            hostLevelFormatters.Insert(0, new MyMediaFormatter());
            operationLevelFormatters.JsonFormatter.MediaTypeMappings.Add(new MyMediaTypeMapping());
            operationLevelFormatters.Add(new MyMediaFormatter1());
            MediaTypeFormatterCollection collection = VerifyFormatters(hostLevelFormatters, operationLevelFormatters);

            Assert.AreEqual(6, collection.Count);
            Assert.AreSame(collection[0], operationLevelFormatters.JsonFormatter);
            Assert.IsInstanceOfType(collection[1], typeof(MyMediaFormatter1));
            Assert.IsInstanceOfType(collection[2], typeof(MyMediaFormatter));
            Assert.AreSame(collection[3], hostLevelFormatters.XmlFormatter);
           
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle with void operation type returns valid HttpResponseMessage with no content.")]
        public void HandleWithVoidResponseTypeReturnsValidEmptyResponse()
        {
            HttpParameter hrd = new HttpParameter("RequestMessage", typeof(HttpRequestMessage));
            HttpParameter hpd = new HttpParameter("response", typeof(void));
            ResponseContentHandler handler = new ResponseContentHandler(hpd, Enumerable.Empty<MediaTypeFormatter>());

            object[] input = new object[1];
            input[0] = new HttpRequestMessage();

            object[] output = handler.Handle(input);

            Assert.IsNotNull(output, "ResponseContentHandler for void type returned a null result.");
            Assert.AreEqual(1, output.Length, "ResponseContentHandler for void type should return a single element array.");
            Assert.IsNotNull(output[0], "ResponseContentHandler for void type returned a null element[0].");
            HttpResponseMessage response = output[0] as HttpResponseMessage;
            Assert.IsNotNull(response, string.Format("ResponseContentHandler returned '{0}' but should have been HttpResponseMessage.", output[0].GetType()));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "ResponseContentHandler for void type should have returned OK.");
            if (response.Content != null)
            {
                Assert.Fail(string.Format("ResponseContentHandler for void type returned content of type '{0}' instead of null.", response.Content.GetType()));
            }

            Assert.AreSame(input[0], response.RequestMessage, "ResponseContentHandler for void type should contain the original request message.");
        }

        private MediaTypeFormatterCollection VerifyFormatters(IEnumerable<MediaTypeFormatter> hostLevelFormatters, IEnumerable<MediaTypeFormatter> operationLevelFormatters)
        {
            HttpParameter hrd = new HttpParameter("RequestMessage", typeof(HttpRequestMessage));
            HttpParameter hpd = new HttpParameter("response", typeof(HttpResponseMessage<int>));
            ResponseContentHandler handler = new ResponseContentHandler(hpd, hostLevelFormatters);

            HttpResponseMessage<int> response = new HttpResponseMessage<int>(1, operationLevelFormatters);
            object[] input = new Object[2];
            input[0] = new HttpRequestMessage();
            input[1] = response;

            object[] output = handler.Handle(input);

            HttpResponseMessage<int> result = output[0] as HttpResponseMessage<int>;

            return result.Content.Formatters;
        }

        public class MyMediaFormatter : MediaTypeFormatter
        {
            private MediaTypeHeaderValue fooMediaType = new MediaTypeHeaderValue("application/foo");

            public MyMediaFormatter()
            {
                this.SupportedMediaTypes.Add(fooMediaType);
            }

            protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
            {
                throw new NotImplementedException();
            }

            protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class MyMediaFormatter1 : MediaTypeFormatter
        {
            private MediaTypeHeaderValue fooMediaType = new MediaTypeHeaderValue("application/foo");

            public MyMediaFormatter1()
            {
                this.SupportedMediaTypes.Add(fooMediaType);
            }

            protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
            {
                throw new NotImplementedException();
            }

            protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class MyMediaTypeMapping : MediaTypeMapping
        {
            public MyMediaTypeMapping() : base(new MediaTypeHeaderValue("application/bar"))
            {
            }

            protected override double OnTryMatchMediaType(HttpRequestMessage request)
            {
                throw new NotImplementedException();
            }

            protected override double OnTryMatchMediaType(HttpResponseMessage response)
            {
                throw new NotImplementedException();
            }
        }

        #endregion FormattersMergeLogic

        #endregion Methods
    }
}
