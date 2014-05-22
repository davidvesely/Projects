// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(HttpErrorHandlerPipeline))]
    public class HttpErrorHandlerPipelineTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpErrorHandlerPipeline is internal class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpErrorHandlerPipeline default constructor works as expected.")]
        public void HttpErrorHandlerPipeline_DefaultConstructor()
        {
            HttpErrorHandlerPipeline errorHandlerPipeline = new HttpErrorHandlerPipeline();
            Assert.IsNotNull(errorHandlerPipeline, "errorHandlerPipeline should not be null.");
        }

        #endregion

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpErrorHandlers should not be null by default.")]
        public void HttpErrorHandlers_Default_Value()
        {
            HttpErrorHandlerPipeline errorHandlerPipeline = new HttpErrorHandlerPipeline();
            Assert.IsNotNull(errorHandlerPipeline.HttpErrorHandlers, "ErrorHandlers property should not be null by default.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpErrorHandlers property works as expected.")]
        public void HttpErrorHandlers_Add_Clear()
        {
            HttpErrorHandlerPipeline errorHandlerPipeline = new HttpErrorHandlerPipeline();
            errorHandlerPipeline.HttpErrorHandlers.Add(new MockHttpErrorHandler());
            Assert.IsTrue(errorHandlerPipeline.HttpErrorHandlers.Count == 1, "ErrorHandlers should contain one HttpErrorHandler.");
            errorHandlerPipeline.HttpErrorHandlers.Clear();
            Assert.IsTrue(errorHandlerPipeline.HttpErrorHandlers.Count == 0, "ErrorHandlers should contain zero HttpErrorHandler.");
        }

        #endregion

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ProvideFault(Exception, MessageVersion, ref Message) throws ArgumentNullException for null error argument.")]
        public void ProvideFault_Null_Error_Argument_Throws()
        {
            IErrorHandler errorHandlerPipeline = new HttpErrorHandlerPipeline();
            Message message = null;
            Asserters.Exception.ThrowsArgumentNull("error", () => errorHandlerPipeline.ProvideFault(error:null, version:MessageVersion.None, fault:ref message));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ProvideFault(Exception, MessageVersion, ref Message) returns when the first HttpErrorHandler returns true on TryProvideResponse.")]
        public void ProvideFault_ErrorHandler_Selection_Logic()
        {
            HttpErrorHandlerPipeline errorHandlerPipeline = new HttpErrorHandlerPipeline();
            errorHandlerPipeline.HttpErrorHandlers.Add(new MockHttpErrorHandler
            {
                OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
                {
                    message = new HttpResponseMessage();
                    return false;
                }
            });

            errorHandlerPipeline.HttpErrorHandlers.Add(new MockHttpErrorHandler
            {
                OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
                {
                    message = new HttpResponseMessage(System.Net.HttpStatusCode.NotAcceptable);
                    return true;
                }
            });

            errorHandlerPipeline.HttpErrorHandlers.Add(new MockHttpErrorHandler
            {
                // This handler should not be called since the previous one returned true.
                OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
                {
                    message = null;
                    return true;
                }
            });

            Message responseMessage = null;
            ((IErrorHandler)errorHandlerPipeline).ProvideFault(new ApplicationException(), MessageVersion.None, ref responseMessage);
            Assert.IsNotNull(responseMessage, "the responseMessage should not be null.");
            Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, responseMessage.ToHttpResponseMessage().StatusCode, "the responseMessage status code should be NotAcceptable.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HandleError(Exception) always defaults to false.")]
        public void HandleError_Defaults_To_False()
        {
            IErrorHandler errorHandlerPipeline = new HttpErrorHandlerPipeline();
            Assert.IsFalse(errorHandlerPipeline.HandleError(null), "HandleError should return false.");
            Assert.IsFalse(errorHandlerPipeline.HandleError(new ApplicationException()), "HandleError should return false.");
        }

        #endregion
    }
}
