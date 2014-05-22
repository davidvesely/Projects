// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpMemoryTransportBindingElement)), UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMemoryTransportBindingElementTests : UnitTest
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBindingElement is public non-abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<HttpMemoryTransportBindingElement, BindingElement>(TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryTransportBindingElement() ctor.")]
        public void DefaultConstructor()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Assert.IsTrue(typeof(BindingElement).IsAssignableFrom(bindingElement.GetType()), "HttpMemoryBindingElement should be a BindingElement");
        }

        #endregion

        #region Member Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Scheme is 'http'")]
        public void HttpMemoryUriSchemeIsHttp()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Assert.AreEqual(Uri.UriSchemeHttp, bindingElement.Scheme, "Unexpected URI scheme -- should be 'http'");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HostNameComparisonMode is 'StrongWildcard'")]
        public void HttpMemoryHostNameComparisonModeIsStrong()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Assert.AreEqual(HostNameComparisonMode.StrongWildcard, bindingElement.HostNameComparisonMode, "Unexpected hostname comparison mode -- should be 'strong'");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("BuildChannelFactory<T>(BindingContext) throws regardless of channel shape.")]
        public void BuildChannelFactoryThrows()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelFactoryNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IChannelFactory).Name),
                () =>
                {
                    bindingElement.BuildChannelFactory<IReplyChannel>(MockBindingContext.Create());
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("CanBuildChannelFactory<T>(BindingContext) always returns 'false' regardless of channel shape.")]
        public void CanBuildChannelFactory_Always_Returns_False()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Asserters.Exception.ThrowsArgumentNull("context", () =>
                {
                    bindingElement.CanBuildChannelFactory<IReplyChannel>(null);
                });

            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IReplyChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IReplySessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IRequestChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IRequestSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IOutputChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IOutputSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IInputChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IInputSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IDuplexChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
            Assert.IsFalse(bindingElement.CanBuildChannelFactory<IDuplexSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelFactory should always return 'false'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("BuildChannelListener<T>(BindingContext) returns an HttpMemoryChannelListener for IReplyChannel.")]
        public void BuildChannelListenerReturnsChannelListenerForIReplyChannel()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            BindingContext context = MockBindingContext.Create();
            context.ListenUriMode = ListenUriMode.Explicit;
            IChannelListener<IReplyChannel> listener = bindingElement.BuildChannelListener<IReplyChannel>(context);
            Assert.IsNotNull(listener, "HttpMemoryBindingElement.BuildChannelListener should have returned an instance.");
            Assert.IsInstanceOfType(listener, typeof(HttpMemoryChannelListener), "HttpMemoryBindingElement.BuildChannelListener should have returned an HttpMemoryChannelListener.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("BuildChannelListener<T>(BindingContext) throws for non-IReplyChannel.")]
        public void BuildChannelListenerThrowsForNonIReplyChannel()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();

            Asserters.Exception.ThrowsArgumentNull("context", () =>
                {
                    bindingElement.BuildChannelListener<IRequestChannel>(null);
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IRequestChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IRequestChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IRequestSessionChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IRequestSessionChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IOutputChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IOutputChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IOutputSessionChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IOutputSessionChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IInputChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IInputChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IInputSessionChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IInputSessionChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IDuplexChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IDuplexChannel>(MockBindingContext.Create());
                });

            Asserters.Exception.Throws<NotSupportedException>(
                SR.ChannelShapeNotSupported(typeof(HttpMemoryTransportBindingElement).Name, typeof(IDuplexSessionChannel).Name, typeof(IReplyChannel).Name),
                () =>
                {
                    bindingElement.BuildChannelListener<IDuplexSessionChannel>(MockBindingContext.Create());
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("CanBuildChannelListener<T>(BindingContext) only returns 'true' for IReplyChannel.")]
        public void CanBuildChannelListenerOnlyReturnsTrueForIReplyChannel()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();

            Asserters.Exception.ThrowsArgumentNull("context", () =>
            {
                bindingElement.CanBuildChannelListener<IReplyChannel>(null);
            });

            Assert.IsTrue(bindingElement.CanBuildChannelListener<IReplyChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should have returned 'true'.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IRequestChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IRequestSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IOutputChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IOutputSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IInputChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IInputSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IDuplexChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
            Assert.IsFalse(bindingElement.CanBuildChannelListener<IDuplexSessionChannel>(MockBindingContext.Create()), "HttpMemoryBindingElement.CanBuildChannelListener should always return 'false' for non IReplyChannel shapes.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Clone() creates a new BindingElement.")]
        public void CloneCreatesNewHttpMemoryBindingElement()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            HttpMemoryTransportBindingElement bindingElementClone = bindingElement.Clone() as HttpMemoryTransportBindingElement;
            Assert.IsNotNull(bindingElementClone, "Clone should have created a new HttpMemoryBindingElement instance.");
            Assert.AreEqual(bindingElement.ManualAddressing, bindingElementClone.ManualAddressing, "ManualAddressing should be identical to the original.");
            Assert.AreEqual(bindingElement.Scheme, bindingElementClone.Scheme, "Scheme should be identical to the original.");
            Assert.AreEqual(bindingElement.MaxBufferPoolSize, bindingElementClone.MaxBufferPoolSize, "MaxBufferPoolSize should be identical to the original.");
            Assert.AreEqual(bindingElement.MaxReceivedMessageSize, bindingElementClone.MaxReceivedMessageSize, "MaxReceivedMessageSize should be identical to the original.");
            Assert.AreNotSame(bindingElement, bindingElementClone, "The cloned instance should be different from the original HttpMemoryBindingElement instance.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetProperty<T> throws on null.")]
        public void GetPropertyThrowsOnNull()
        {
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Asserters.Exception.ThrowsArgumentNull("context",
                () =>
                {
                    bindingElement.GetProperty<MessageVersion>(null);
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetProperty<T> creates a new BindingElement.")]
        public void GetPropertyReturnsMessageVersionNone()
        {
            BindingContext context = MockBindingContext.Create();
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            MessageVersion version = bindingElement.GetProperty<MessageVersion>(context);
            Assert.AreEqual(MessageVersion.None, version, "Unexpected message version");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetProperty<T> returns property from base type.")]
        public void GetPropertyReturnsPropertyFromBasetype()
        {
            BindingContext context = MockBindingContext.Create();
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            XmlDictionaryReaderQuotas quotas = bindingElement.GetProperty<XmlDictionaryReaderQuotas>(context);
            Assert.IsNotNull(quotas, "Property from base class should not be null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("GetHttpMemoryHandler(TimeSpan) throws on timeout.")]
        public void GetHttpMemoryHandlerThrowsOnTimeout()
        {
            BindingContext context = MockBindingContext.Create();
            HttpMemoryTransportBindingElement bindingElement = new HttpMemoryTransportBindingElement();
            Asserters.Exception.Throws<TimeoutException>(
                () => { bindingElement.GetHttpMemoryHandler(TimeSpan.FromSeconds(1)); },
                (timeoutexception) => { });
        }

        #endregion
    }
}