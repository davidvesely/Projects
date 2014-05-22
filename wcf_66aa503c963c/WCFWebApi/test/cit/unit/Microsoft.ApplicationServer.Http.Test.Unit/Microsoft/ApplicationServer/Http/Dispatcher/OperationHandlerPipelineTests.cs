// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(OperationHandlerPipeline))]
    public class OperationHandlerPipelineTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipeline is internal and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipeline() initializes.")]
        public void Constructor()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            OperationHandlerPipeline pipeline = new OperationHandlerPipeline(handlers, handlers, operation);
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipeline() throws with null request handlers.")]
        public void ConstructorThrowsWithNullRequestHandlers()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            Asserters.Exception.ThrowsArgumentNull("requestHttpOperationHandlers", () => new OperationHandlerPipeline(null, handlers, operation));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipeline() throws with null response handlers.")]
        public void ConstructorThrowsWithNullResponseHandlers()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            Asserters.Exception.ThrowsArgumentNull("responseHttpOperationHandlers", () => new OperationHandlerPipeline(handlers, null, operation));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OperationHandlerPipeline() throws with null operation.")]
        public void ConstructorThrowsWithNullOperation()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            Asserters.Exception.ThrowsArgumentNull("operation", () => new OperationHandlerPipeline(handlers, handlers, null));
        }

        #endregion Constructors

        #region Methods

        #region ExecuteRequestPipeline

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExecuteRequestPipeline() executes with no handlers.")]
        public void ExecuteRequestPipeline()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            OperationHandlerPipeline pipeline = new OperationHandlerPipeline(handlers, handlers, operation);

            HttpRequestMessage request = new HttpRequestMessage();
            OperationHandlerPipelineContext context = pipeline.ExecuteRequestPipeline(request, new object[0]);

            Assert.IsNotNull(context, "Execute returned null context.");
        }

        #endregion ExecuteRequestPipeline

        #region ExecuteResponsePipeline

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ExecuteResponsePipeline() executes with no handlers.")]
        public void ExecuteResponsePipeline()
        {
            HttpOperationHandler[] handlers = new HttpOperationHandler[0];
            HttpOperationDescription operation = new HttpOperationDescription() { ReturnValue = HttpParameter.ResponseMessage };
            OperationHandlerPipeline pipeline = new OperationHandlerPipeline(handlers, handlers, operation);

            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };

            OperationHandlerPipelineContext context = pipeline.ExecuteRequestPipeline(request, new object[0]);

            //// TODO: what is the convention for returning an HttpResponse from the operation?
            HttpResponseMessage actualResponse = pipeline.ExecuteResponsePipeline(context, new object[0], response);

            Assert.IsNotNull(actualResponse, "Execute returned null response.");
        }

        #endregion ExecuteResponsePipeline

        #endregion Methods
    }
}
