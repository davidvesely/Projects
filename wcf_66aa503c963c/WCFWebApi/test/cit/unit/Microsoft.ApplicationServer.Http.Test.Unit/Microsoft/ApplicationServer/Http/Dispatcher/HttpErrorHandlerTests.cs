// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Net.Http;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpErrorHandlerTests : UnitTest<HttpErrorHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpErrorHandler is public abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region TryProvideResponse Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("TryProvideResponse throws for null response when returning true.")]
        public void TryProvideResponse_Null_Response_Throws_On_True()
        {
            Exception error = new InvalidOperationException("problem");
            HttpResponseMessage faultMessage = null;

            MockHttpErrorHandler errorHandler = new MockHttpErrorHandler();
            errorHandler.OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
            {
                message = null;
                return true;
            };

            Asserters.Exception.Throws<InvalidOperationException>(
                SR.HttpErrorMessageNullResponse(typeof(MockHttpErrorHandler).Name, true, "TryProvideResponse", typeof(HttpResponseMessage).Name),
                () => errorHandler.TryProvideResponse(error, ref faultMessage));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("TryProvideResponse doesn't throws for null response when returning false.")]
        public void TryProvideResponse_Null_Response_Not_Throw_On_False()
        {
            Exception error = new InvalidOperationException("problem");
            HttpResponseMessage faultMessage = null;

            MockHttpErrorHandler errorHandler = new MockHttpErrorHandler();
            errorHandler.OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
            {
                message = null;
                return false;
            };

            errorHandler.TryProvideResponse(error, ref faultMessage);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("TryProvideResponse can return custom response message.")]
        public void TryProvideResponse_Returns_Custom_Response_Message()
        {
            Exception error = new InvalidOperationException("problem");
            HttpResponseMessage customResponseMessage = new HttpResponseMessage();
            HttpResponseMessage faultMessage = null;

            MockHttpErrorHandler errorHandler = new MockHttpErrorHandler();
            errorHandler.OnTryProvideResponseCallback = delegate(Exception e, ref HttpResponseMessage message)
            {
                message = customResponseMessage;
                return true;
            };

            bool responseProvided = errorHandler.TryProvideResponse(error, ref faultMessage);

            Assert.IsTrue(responseProvided, "TryProvideResponse should return true");
            Assert.IsNotNull(faultMessage, "ProvideFault cannot yield null response");
            Assert.AreSame(customResponseMessage, faultMessage, "TryProvideResponse should return custom message");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("TryProvideResponse throws ArgumentNullException for null error argument.")]
        public void TryProvideResponse_Null_Error_Argument_Throws()
        {
            HttpErrorHandler errorHandler = new MockHttpErrorHandler();
            HttpResponseMessage message = null;
            Asserters.Exception.ThrowsArgumentNull("error", () => errorHandler.TryProvideResponse(/*error*/ null, ref message));
        }

        #endregion TryProvideResponse Tests
    }
}
