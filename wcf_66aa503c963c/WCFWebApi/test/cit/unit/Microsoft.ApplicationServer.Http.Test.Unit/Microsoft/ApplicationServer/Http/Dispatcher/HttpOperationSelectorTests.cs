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

    [TestClass]
    public class HttpOperationSelectorTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("HttpOperationSelector.SelectOperation returns custom operation name")]
        public void SelectOperation_Returns_Custom_Operation_Name()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            Message message = httpRequestMessage.ToMessage();

            MockHttpOperationSelector selector = new MockHttpOperationSelector();
            selector.OnSelectOperationCallback =
                (localHttpRequestMessag) =>
                {
                    Assert.AreSame(httpRequestMessage, localHttpRequestMessag, "The 'OnSelectOperation' method should have been called with the same HttpRequestMessage instance.");
                    return "CustomOperation";
                };
            
            string returnedOperation = ((IDispatchOperationSelector)selector).SelectOperation(ref message);
            Assert.AreEqual("CustomOperation", returnedOperation, "SelectOperation should have returned the custom operation name.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("HttpOperationSelector.SelectOperation throws ArgumentNullException for null message")]
        public void SelectOperation_Null_Message_Throws()
        {
            IDispatchOperationSelector selector = new MockHttpOperationSelector();
            Message message = null;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("message", () => selector.SelectOperation(ref message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("HttpOperationSelector.SelectOperation(HttpRequestMessage) throws ArgumentNullException for null message")]
        public void SelectOperation_Null_HttpRequestMessage_Throws()
        {
            HttpOperationSelector selector = new MockHttpOperationSelector();
            HttpRequestMessage message = null;
            UnitTest.Asserters.Exception.ThrowsArgumentNull("message", () => selector.SelectOperation(message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("HttpOperationSelector.SelectOperation throws InvalidOperationException for non-http message")]
        public void SelectOperation_Non_Http_Message_Throws()
        {
            IDispatchOperationSelector selector = new MockHttpOperationSelector();
            Message message = Message.CreateMessage(MessageVersion.None, "notUsed");
            UnitTest.Asserters.Exception.Throws<InvalidOperationException>(
                SR.HttpOperationSelectorNullRequest(typeof(MockHttpOperationSelector).Name, typeof(HttpRequestMessage).Name, "SelectOperation"),
                () => selector.SelectOperation(ref message));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("HttpOperationSelector.SelectOperation throws if null operation is returned")]
        public void SelectOperation_Null_Return_Throws()
        {
            Message message = new HttpRequestMessage().ToMessage();

            MockHttpOperationSelector selector = new MockHttpOperationSelector();
            selector.OnSelectOperationCallback = (localMessage) => null;

            UnitTest.Asserters.Exception.Throws<InvalidOperationException>(
                SR.HttpOperationSelectorNullOperation(typeof(MockHttpOperationSelector).Name),
                () => ((IDispatchOperationSelector)selector).SelectOperation(ref message));
        }
    }
}
