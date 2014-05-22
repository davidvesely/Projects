// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Server.Common;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpMemoryChannelListener)), UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpMemoryChannelListenerTests : UnitTest
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener is public non-abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<HttpMemoryChannelListener, ChannelListenerBase<IReplyChannel>>(TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type Tests

        #region Helpers

        internal static HttpMemoryChannelListener CreateHttpMemoryChannelListener()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Explicit;
            return new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor.")]
        public void HttpMemoryChannelListenerConstructor()
        {
            HttpMemoryChannelListener channelListener = CreateHttpMemoryChannelListener();
            Assert.IsNotNull(channelListener, "channelListener should not be null");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor Throws On Invalid ListenUriMode.")]
        public void HttpMemoryChannelListenerConstructorThrowsOnInvalidListenUriMode()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Unique;
            Asserters.Exception.ThrowsArgument("context",
                () =>
                {
                    HttpMemoryChannelListener channelListener = new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor Throws On null base URI.")]
        public void HttpMemoryChannelListenerConstructorThrowsOnNullBaseUri()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Explicit;
            bindingContext.ListenUriBaseAddress = null;
            Asserters.Exception.ThrowsArgument("context",
                () =>
                {
                    HttpMemoryChannelListener channelListener = new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor Throws On null base URI.")]
        public void HttpMemoryChannelListenerConstructorThrowsOnRelativeUri()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Explicit;
            bindingContext.ListenUriBaseAddress = new Uri("relative", UriKind.Relative);
            Asserters.Exception.ThrowsArgument("context",
                () =>
                {
                    HttpMemoryChannelListener channelListener = new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor Throws On bad URI scheme.")]
        public void HttpMemoryChannelListenerConstructorThrowsOnBadUriScheme()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Explicit;
            bindingContext.ListenUriBaseAddress = new Uri("ftp://example.com");
            Asserters.Exception.ThrowsArgument("context",
                () =>
                {
                    HttpMemoryChannelListener channelListener = new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannelListener constructor Throws On null relative URI.")]
        public void HttpMemoryChannelListenerConstructorThrowsOnNullRelativeUri()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();
            BindingContext bindingContext = MockBindingContext.Create();
            bindingContext.ListenUriMode = ListenUriMode.Explicit;
            bindingContext.ListenUriBaseAddress = new Uri("http://example.com");
            bindingContext.ListenUriRelativeAddress = null;
            Asserters.Exception.ThrowsArgument("context",
                () =>
                {
                    HttpMemoryChannelListener channelListener = new HttpMemoryChannelListener(bindingElement, bindingContext, handlerQueue);
                });
        }

        #endregion Constructor Tests

        #region Property Tests
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Uri returns listening address.")]
        public void UriReturnsListeningAddress()
        {
            Uri expected = new Uri("http://somehost");
            HttpMemoryChannelListener channelListener = CreateHttpMemoryChannelListener();
            Assert.AreEqual(expected, channelListener.Uri, "Uri didn't return expected value");
        }

        #endregion

        #region Member Tests
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetProperty<T>() creates a new BindingElement.")]
        public void GetPropertyReturnsMessageVersionNone()
        {
            HttpMemoryChannelListener channelListener = CreateHttpMemoryChannelListener();
            MessageVersion version = channelListener.GetProperty<MessageVersion>();
            Assert.AreEqual(MessageVersion.None, version, "Unexpected message version");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetProperty<T>() returns property from base type.")]
        public void GetPropertyReturnsPropertyFromBasetype()
        {
            HttpMemoryChannelListener channelListener = CreateHttpMemoryChannelListener();
            IChannelListener listener = channelListener.GetProperty<IChannelListener>();
            Assert.IsNotNull(listener, "Property from base class should not be null.");
        }

        #endregion
    }
}