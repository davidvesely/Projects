// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(OperationHandlerFormatter)), UnitTestLevel(UnitTestLevel.InProgress)]
    public class OperationHandlerFormatterTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerFormatter is internal, concrete, and unsealed, and it implements HttpMessageFormatter.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass, typeof(HttpMessageFormatter));
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerFormatter() initializes using empty pipeline.")]
        public void Constructor()
        {
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            IEnumerable<HttpOperationHandler> emptyHandlers = Enumerable.Empty<HttpOperationHandler>();
            OperationHandlerPipeline pipeline = new OperationHandlerPipeline(emptyHandlers, emptyHandlers, operation);
            OperationHandlerFormatter formatter = new OperationHandlerFormatter(pipeline);
        }

        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        #region OnDeserializeRequest

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("OnDeserializeRequest() executes request pipeline.")]
        ////public void OnDeserializeRequestExecutesRequestPipeline()
        ////{
        ////    HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
        ////    IEnumerable<HttpOperationHandler> emptyHandlers = Enumerable.Empty<HttpOperationHandler>();
        ////    OperationHandlerPipeline pipeline = new OperationHandlerPipeline(emptyHandlers, emptyHandlers, operation);
        ////    MOperationHandlerPipeline molePipeline = new MOperationHandlerPipeline(pipeline);
        ////    molePipeline.BehaveAsDefaultValue();

        ////    MOperationHandlerPipelineContext moleContext = new MOperationHandlerPipelineContext();

        ////    HttpRequestMessage setRequest = null;
        ////    object[] setValues = null;
        ////    OperationHandlerPipelineContext setContext = null;
        ////    molePipeline.ExecuteRequestPipelineHttpRequestMessageObjectArray = (request, values) =>
        ////        {
        ////            setRequest = request;
        ////            setValues = values;
        ////            return setContext = moleContext;
        ////        };

        ////    OperationHandlerFormatter formatter = new OperationHandlerFormatter(molePipeline);
        ////    IDispatchMessageFormatter dispatchMessageFormatter = (IDispatchMessageFormatter)formatter;

        ////    Uri uri = new Uri("http://somehost/Fred");
        ////    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        ////    httpRequest.Content = new StringContent("");

        ////    Message message = httpRequest.ToMessage();
        ////    object[] parameters = new object[0];
        ////    dispatchMessageFormatter.DeserializeRequest(message, parameters);

        ////    Asserters.Http.AreEqual(httpRequest, setRequest);
        ////    Assert.IsNotNull(setValues, "Input values were not passed to the pipeline.");
        ////    Assert.AreEqual(0, ((object[])setValues).Length, "Incorrect number of values.");
        ////    Assert.IsNotNull(setContext, "Context was not set.");

        ////}

        #endregion OnDeserializeRequest

        #region OnSerializeReply

        ////[Ignore]    // TODO: CSDMian #211411
        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("OnSerializeReply() executes response pipeline.")]
        ////public void OnSerializeReplyExecutesResponsePipeline()
        ////{
        ////    HttpOperationDescription operation = new HttpOperationDescription() { CallBase = true, ReturnValue = HttpParameter.ResponseMessage };
        ////    IEnumerable<HttpOperationHandler> emptyHandlers = Enumerable.Empty<HttpOperationHandler>();
        ////    OperationHandlerPipeline pipeline = new OperationHandlerPipeline(emptyHandlers, emptyHandlers, operation);
        ////    MOperationHandlerPipeline molePipeline = new MOperationHandlerPipeline(pipeline);
        ////    molePipeline.BehaveAsDefaultValue();

        ////    MOperationHandlerPipelineContext moleContext = new MOperationHandlerPipelineContext();
             
        ////    HttpResponseMessage response = new HttpResponseMessage();
        ////    OperationHandlerPipelineContext setContext = null;
        ////    object[] setValues = null;
        ////    object setResult = null;
        ////    molePipeline.ExecuteResponsePipelineOperationHandlerPipelineContextObjectArrayObject = (context, values, result) =>
        ////    {
        ////        setContext = context;
        ////        setValues = values;
        ////        setResult = result;
        ////        return response;
        ////    };

        ////    OperationHandlerFormatter formatter = new OperationHandlerFormatter(molePipeline);
        ////    IDispatchMessageFormatter dispatchMessageFormatter = (IDispatchMessageFormatter)formatter;

        ////    object[] parameters = new object[] { 1, "text" };
        ////    Message message = dispatchMessageFormatter.SerializeReply(MessageVersion.None, parameters, "theResult");

        ////    Assert.IsNotNull(setValues, "Input values were not passed to the pipeline.");
            
        ////    CollectionAssert.AreEqual(new List<object>(parameters), new List<object>(setValues), "Parameters were not passed correctly.");
        ////    Assert.AreEqual("theResult", setResult, "Result was not passed correctly.");

        ////    Assert.IsNotNull(setContext, "Context was not set.");
        ////}

        #endregion OnSerializeReply

        #endregion Methods
    }
}
