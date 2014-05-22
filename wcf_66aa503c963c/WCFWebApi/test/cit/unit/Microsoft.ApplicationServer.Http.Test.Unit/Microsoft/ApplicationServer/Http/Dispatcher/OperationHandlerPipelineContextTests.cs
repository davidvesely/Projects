// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.ApplicationServer.Common.Test.Mocks;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(OperationHandlerPipelineContext))]
    public class OperationHandlerPipelineContextTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipelineContext is internal and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipelineContext(OperationHandlerPipelineInfo, HttpRequestMessage) initializes OperationHandlerPipelineInfo.")]
        public void Constructor()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            operation.InputParameters.Add(HttpParameter.RequestMessage);
            OperationHandlerPipelineInfo pipelineInfo = new OperationHandlerPipelineInfo(handlers, handlers, operation);
            HttpRequestMessage request = new HttpRequestMessage();

            OperationHandlerPipelineContext context = new OperationHandlerPipelineContext(pipelineInfo, request);
            object[] inputValues = context.GetInputValues();
            Assert.AreEqual(1, inputValues.Length, "Input values should be this size.");
            Assert.IsTrue(inputValues[0] is HttpRequestMessage, "1st input parameter should be the request message.");
            Asserters.Http.AreEqual(request, (HttpRequestMessage) inputValues[0]);
        }

        #endregion Constructors

        #region Methods

        #region GetInputValues()

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("GetInputValues() calls PipelineInfo.")]
        ////public void GetInputValuesCallsPipelineInfo()
        ////{
        ////    HttpOperationHandler[] handlers = new HttpOperationHandler[0];
        ////    HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
        ////    OperationHandlerPipelineInfo pipelineInfo = new OperationHandlerPipelineInfo(handlers, handlers, operation);
        ////    MockOperationHandlerPipelineInfo molePipelineInfo = new MockOperationHandlerPipelineInfo(pipelineInfo);
        ////    molePipelineInfo.GetEmptyPipelineValuesArray = () => new object[0];
        ////    molePipelineInfo.SetHttpRequestMessageHttpRequestMessageObjectArray = (req, values) => { };

        ////    bool calledForValues = false;
        ////    int handlerIndexAtCall = -1;
        ////    object[] valuesAtCall = null;
        ////    molePipelineInfo.GetInputValuesForHandlerInt32ObjectArray = (index, values) => 
        ////    { 
        ////        calledForValues = true;
        ////        handlerIndexAtCall = index;
        ////        valuesAtCall = values;
        ////        return new object[0]; 
        ////    };

        ////    HttpRequestMessage request = new HttpRequestMessage();
        ////    OperationHandlerPipelineContext context = new OperationHandlerPipelineContext(pipelineInfo, request);

        ////    object[] returnedValues = context.GetInputValues();

        ////    Assert.IsTrue(calledForValues, "PipelineInfo was not called for its values.");
        ////    Assert.AreEqual(1, handlerIndexAtCall, "Handler index should have been 0.");
        ////    Assert.IsNotNull(valuesAtCall, "Values at call should have not been null.");
        ////    Assert.IsNotNull(returnedValues, "Returned values were null.");
        ////    Assert.AreEqual(0, returnedValues.Length, "Returned values were incorrect length.");
        ////}

        #endregion GetInputValues()

        #region GetHttpResponseMessage()

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("GetHttpResponseMessage() calls PipelineInfo.")]
        ////public void GetHttpResponseMessageCallsPipelineInfo()
        ////{
        ////    HttpRequestMessage request = new HttpRequestMessage();
        ////    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
        ////    HttpOperationHandler[] handlers = new HttpOperationHandler[0];
        ////    HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
        ////    OperationHandlerPipelineInfo pipelineInfo = new OperationHandlerPipelineInfo(handlers, handlers, operation);
        ////    MockOperationHandlerPipelineInfo molePipelineInfo = new MockOperationHandlerPipelineInfo(pipelineInfo);
        ////    molePipelineInfo.GetEmptyPipelineValuesArray = () => new object[0];
        ////    molePipelineInfo.SetHttpRequestMessageHttpRequestMessageObjectArray = (req, values) => { };
        ////    molePipelineInfo.GetHttpResponseMessageObjectArray = (values) => response;
        ////    OperationHandlerPipelineContext context = new OperationHandlerPipelineContext(pipelineInfo, request);

        ////    HttpResponseMessage responseReturned = context.GetHttpResponseMessage();

        ////    Assert.IsNotNull(responseReturned, "HttpResponseMessage was not returned.");
        ////    Asserters.Http.AreEqual(response, responseReturned);
        ////}

        #endregion GetHttpResponseMessage()

        #region SetOutputValuesAndAdvance()

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("SetOutputValuesAndAdvance(object[]) calls PipelineInfo.")]
        ////public void SetOutputValuesAndAdvanceCallsPipelineInfo()
        ////{
        ////    HttpRequestMessage request = new HttpRequestMessage();
        ////    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
        ////    HttpOperationHandler[] handlers = new HttpOperationHandler[0];
        ////    HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
        ////    OperationHandlerPipelineInfo pipelineInfo = new OperationHandlerPipelineInfo(handlers, handlers, operation);

        ////    int indexAtCall = -1;
        ////    object[] valuesAtCall = null;
        ////    object[] pipelineValuesAtCall = null;

        ////    MockOperationHandlerPipelineInfo molePipelineInfo = new MockOperationHandlerPipelineInfo(pipelineInfo);
        ////    molePipelineInfo.GetEmptyPipelineValuesArray = () => new object[0];
        ////    molePipelineInfo.SetHttpRequestMessageHttpRequestMessageObjectArray = (req, values) => { };
        ////    molePipelineInfo.SetOutputValuesFromHandlerInt32ObjectArrayObjectArray = (index, values, pipelineValues) =>
        ////        {
        ////            indexAtCall = index;
        ////            valuesAtCall = values;
        ////            pipelineValuesAtCall = pipelineValues;
        ////        };

        ////    OperationHandlerPipelineContext context = new OperationHandlerPipelineContext(pipelineInfo, request);

        ////    context.SetOutputValuesAndAdvance(new object[0]);

        ////    Assert.AreEqual(1, indexAtCall, "Handler index was not set.");
        ////    Assert.IsNotNull(valuesAtCall, "Values were not set.");
        ////    Assert.IsNotNull(pipelineValuesAtCall, "Pipeline values were not set.");
        ////}

        #endregion SetOutputValuesAndAdvance()

        #endregion Methods
    }
}
