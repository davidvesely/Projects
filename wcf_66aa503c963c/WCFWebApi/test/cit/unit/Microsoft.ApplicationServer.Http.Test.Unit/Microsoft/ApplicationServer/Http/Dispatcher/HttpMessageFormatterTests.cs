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
  
    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpMessageFormatterTests : UnitTest<HttpMessageFormatter>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpMessageFormatter is public abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region DeserializeRequest Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("DeserializeRequest(Message, object[]) receives HttpRequestMessage and message parameters.")]
        public void DeserializeRequest_Receives_Message_And_Parameters()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            Message wcfMessage = httpRequestMessage.ToMessage();
            object[] messageParameters = new object[] { "hello", 5.0 };

            MockHttpMessageFormatter formatter = new MockHttpMessageFormatter();
            formatter.OnDeserializeRequestCallback =
                (msg, parameters) =>
                {
                    Assert.AreSame(httpRequestMessage, msg, "DeserializeRequest() did not receive the HttpRequestMessage we specified");
                    Assert.AreSame(messageParameters, parameters, "DeserializeRequest() did not receive the parameters we specified");
                };

            ((IDispatchMessageFormatter)formatter).DeserializeRequest(wcfMessage, messageParameters);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("DeserializeRequest(Message, object[]) throws ArgumentNullException for null WCF message.")]
        public void DeserializeRequest_Null_Message_Throws()
        {
            object[] parameters = new object[] { "hello", 5.0 };
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.ThrowsArgumentNull("message", () => formatter.DeserializeRequest(/*message*/ null, parameters));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("DeserializeRequest(Message, object[]) throws ArgumentNullException for null parameters.")]
        public void DeserializeRequest_Null_Parameters_Throws()
        {
            Message wcfMessage = new HttpRequestMessage().ToMessage();
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.ThrowsArgumentNull("parameters", () => formatter.DeserializeRequest(wcfMessage, parameters: null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("DeserializeRequest(Message, object[]) throws InvalidOperationException for null HttpRequestMessage.")]
        public void DeserializeRequest_Null_HttpRequestMessage_Throws()
        {
            Message wcfMessage = Message.CreateMessage(MessageVersion.None, "unused");
            object[] parameters = new object[] { "hello", 5.0 };
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.Throws<InvalidOperationException>(
                SR.HttpMessageFormatterNullMessage(typeof(MockHttpMessageFormatter).Name, typeof(HttpRequestMessage).Name, "DeserializeRequest"),
                () => formatter.DeserializeRequest(wcfMessage, parameters));
        }

        #endregion DeserializeRequest Tests

        #region SerializeReply Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) receives parameters and result.")]
        public void SerializeReply_Receives_Parameters_And_Result()
        {
            object[] messageParameters = new object[] { "hello", 5.0 };
            string messageResult = "hello";
            MockHttpMessageFormatter formatter = new MockHttpMessageFormatter();
            formatter.OnSerializeReplyCallback =
                (parameters, result) =>
                {
                    Assert.AreSame(messageParameters, parameters, "SerializeReply() did not receive the input parameters");
                    Assert.AreSame(messageResult, result, "SerializeReply() did not receive the input result");
                    return new HttpResponseMessage();
                };

            Message responseMessage = ((IDispatchMessageFormatter)formatter).SerializeReply(MessageVersion.None, messageParameters, messageResult);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) returns valid HttpResponseMessage.")]
        public void SerializeReply_Returns_HttpResponseMessage()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            MockHttpMessageFormatter formatter = new MockHttpMessageFormatter();
            formatter.OnSerializeReplyCallback = (parameters, result) => httpResponseMessage;

            Message wcfMessage = ((IDispatchMessageFormatter)formatter).SerializeReply(MessageVersion.None, parameters: new object[0], result: "result");
            Assert.IsNotNull(wcfMessage, "Returned WCF message cannot be null");
            HttpResponseMessage returnedHttpResponseMessage = wcfMessage.ToHttpResponseMessage();
            Assert.AreSame(httpResponseMessage, returnedHttpResponseMessage, "SerializeReply() response message was not the one we returned.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) throws for null parameters argument.")]
        public void SerializeReply_Null_Parameters_Throws()
        {
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.ThrowsArgumentNull("parameters", () => formatter.SerializeReply(MessageVersion.None, /*parameters*/ null, /*result*/ "hello"));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) throws for null parameters argument.")]
        public void SerializeReply_HttpMessageFormatter_Null_Parameters_Throws()
        {
            HttpMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.ThrowsArgumentNull("parameters", () => formatter.SerializeReply(/*parameters*/ null, /*result*/ "hello"));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) accepts a null result argument.")]
        public void SerializeReply_Null_Result_Allowed()
        {
            bool receivedNullResult = false;
            MockHttpMessageFormatter formatter = new MockHttpMessageFormatter();
            formatter.OnSerializeReplyCallback =
                (parameters, result) =>
                {
                    receivedNullResult = (result == null);
                    return new HttpResponseMessage();
                };

            ((IDispatchMessageFormatter)formatter).SerializeReply(MessageVersion.None, parameters: new object[0], result: null);
            Assert.IsTrue(receivedNullResult, "Null result did not make it through SerializeReply");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) throws NotSupportedException if MessageVersion is not MessageVersion.None.")]
        public void SerializeReply_MessageVersion_Not_None_Throws()
        {
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            Asserters.Exception.Throws<NotSupportedException>(
                SR.HttpMessageFormatterMessageVersion(typeof(MockHttpMessageFormatter), typeof(MessageVersion), "None"),
                () => formatter.SerializeReply(MessageVersion.Soap11, parameters: new object[0], result: "result"));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SerializeReply(MessageVersion, object[], object) throws InvalidOperationException for null returned HttpResponseMessage.")]
        public void SerializeReply_Null_HttpResponseMessage_Throws()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            MockHttpMessageFormatter formatter = new MockHttpMessageFormatter();
            formatter.OnSerializeReplyCallback = (parameters, result) => null;

            Asserters.Exception.Throws<InvalidOperationException>(
                SR.HttpMessageFormatterNullMessage(typeof(MockHttpMessageFormatter), typeof(HttpResponseMessage).Name, "SerializeReply"),
                () => ((IDispatchMessageFormatter)formatter).SerializeReply(MessageVersion.None, parameters: new object[0], result: "result"));
        }

        #endregion SerializeReply Tests
    }
}
