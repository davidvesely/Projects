// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Channels.Mocks;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMessageHandlerFactoryTests : UnitTest<HttpMessageHandlerFactory>
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory is public non-abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<HttpMessageHandlerFactory>(TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws on null.")]
        public void HttpMessageHandlerFactoryTypeThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgument(
                "handlers[0]",
                SR.HttpMessageHandlerTypeNotSupported("null", typeof(DelegatingHandler).Name),
                () => { new HttpMessageHandlerFactory((Type)null); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws on empty.")]
        public void HttpMessageHandlerFactoryTypeThrowsOnEmpty()
        {
            Asserters.Exception.ThrowsArgument(
                "handlers",
                SR.InputTypeListEmptyError,
                () => { new HttpMessageHandlerFactory(); });

            Asserters.Exception.ThrowsArgument(
                "handlers",
                SR.InputTypeListEmptyError,
                () => { new HttpMessageHandlerFactory(Type.EmptyTypes); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws on array with null item.")]
        public void HttpMessageHandlerFactoryTypeThrowsOnArrayWithNullItem()
        {
            Type[] types = new Type[2] { (Type)null, typeof(MockValidMessageHandler) };
            Asserters.Exception.ThrowsArgument(
                "handlers[0]",
                SR.HttpMessageHandlerTypeNotSupported("null", typeof(DelegatingHandler).Name),
                () => { HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(types); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws with non DelegatingHandler-derived types.")]
        public void HttpMessageHandlerFactoryTypeThrowsWithNonDelegatingHandlerDerivedType()
        {
            Asserters.Exception.ThrowsArgument(
                "handlers[0]",
                SR.HttpMessageHandlerTypeNotSupported(typeof(int).Name, typeof(DelegatingHandler).Name), 
                () => 
                {
                    new HttpMessageHandlerFactory(typeof(int));
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws with abstract types.")]
        public void HttpMessageHandlerFactoryConstructorThrowsWithAbstractType()
        {
            Asserters.Exception.ThrowsArgument(
                "handlers[0]",
                SR.HttpMessageHandlerTypeNotSupported(typeof(DelegatingHandler).Name, typeof(DelegatingHandler).Name),
                () =>
                {
                    new HttpMessageHandlerFactory(typeof(DelegatingHandler));
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Type[]) throws with types that don't have public default constructor.")]
        public void HttpMessageHandlerFactoryTypeThrowsOnNoDefaultCtor()
        {
            Asserters.Exception.ThrowsArgument(
                "handlers[0]",
                SR.HttpMessageHandlerTypeNotSupported(typeof(MockInvalidMessageHandler).Name, typeof(DelegatingHandler).Name),
                () =>
                {
                    new HttpMessageHandlerFactory(typeof(MockInvalidMessageHandler));
                });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("HttpMessageHandlerFactory(Func<IEnumerable<DelegatingHandler>>) throws on null.")]
        public void HttpMessageHandlerFactoryFuncThrowsOnNull()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "handlers",
                () =>
                {
                    new HttpMessageHandlerFactory((Func<IEnumerable<DelegatingHandler>>)null);
                });
        }

        #endregion Constructor Tests

        #region Member Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) throws on null.")]
        public void CreateThrowsOnNull()
        {
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(typeof(MockValidMessageHandler));
            Asserters.Exception.ThrowsArgumentNull("innerChannel", () => factory.Create(null));
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) where Func returns null should return inner channel.")]
        public void CreateWithFuncReturningNull()
        {
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(() => { return null; });
            MockValidMessageHandler innerChannel = new MockValidMessageHandler();
            HttpMessageHandler pipeline = factory.Create(innerChannel);
            Assert.AreSame(innerChannel, pipeline, "Expected pipeline to be same as inner channel");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) where Func returns array with null item.")]
        public void CreateWithFuncReturningArrayWithNullItem()
        {
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(() => { return new List<DelegatingHandler> { null }; });
            MockValidMessageHandler innerChannel = new MockValidMessageHandler();
            Asserters.Exception.ThrowsArgument(
                "handlers", 
                () => { factory.Create(innerChannel); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) where Func returns item with inner channel set.")]
        public void CreateWithFuncReturningItemWithInnerChannelSet()
        {
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(() => 
            { 
                List<DelegatingHandler> handlers = new List<DelegatingHandler>();
                MockValidMessageHandler handler = new MockValidMessageHandler();
                handler.InnerHandler = handler;
                handlers.Add(handler);
                return handlers;
            });
            MockValidMessageHandler innerChannel = new MockValidMessageHandler();
            Asserters.Exception.ThrowsArgument(
                "handlers",
                SR.DelegatingHandlerArrayHasNonNullInnerHandler(
                    typeof(DelegatingHandler).Name,
                    "InnerHandler",
                    typeof(MockValidMessageHandler).Name),
                () => { factory.Create(innerChannel); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) handles ordering correctly.")]
        public void CreateHandlesOrdering()
        {
            MockValidMessageHandler handler0 = new MockValidMessageHandler { Index = 0 };
            MockValidMessageHandler handler1 = new MockValidMessageHandler { Index = 1 };
            MockValidMessageHandler handler2 = new MockValidMessageHandler { Index = 2 };
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(() =>
            {
                return new List<DelegatingHandler> { handler1, handler2 };
            });

            MockValidMessageHandler actualHandler2 = factory.Create(handler0) as MockValidMessageHandler;
            Assert.AreEqual(2, actualHandler2.Index, "Expected pipeline to start with handler2.");

            MockValidMessageHandler actualHandler1 = ((MockValidMessageHandler)actualHandler2).InnerHandler as MockValidMessageHandler;
            Assert.AreEqual(1, actualHandler1.Index, "Expected pipeline to have handler1 in the middle");

            MockValidMessageHandler actualHandler0 = ((MockValidMessageHandler)actualHandler1).InnerHandler as MockValidMessageHandler;
            Assert.AreEqual(0, actualHandler0.Index, "Expected pipeline to end with handler0.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageHandler)]
        [Description("Create(HttpMessageHandler) where Type returns item with inner channel set.")]
        public void CreateWithTypeReturningItemWithInnerChannelSet()
        {
            HttpMessageHandlerFactory factory = new HttpMessageHandlerFactory(typeof(MockSelfMessageHandler));
            MockValidMessageHandler innerChannel = new MockValidMessageHandler();
            Asserters.Exception.ThrowsArgument(
                "handlers",
                SR.DelegatingHandlerArrayHasNonNullInnerHandler(
                    typeof(DelegatingHandler).Name, 
                    "InnerHandler", 
                    typeof(MockSelfMessageHandler).Name),
                () => { factory.Create(innerChannel); });
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("derik")]
        [Description("Create(HttpMessageHandler) on a derived factory which uses the default ctor.")]
        public void CreateWithDerivedFactoryUsingDefaultCtor()
        {
            HttpMessageHandlerFactory factory = new DerivedHttpMessageHandlerFactory();
            MockValidMessageHandler innerChannel = new MockValidMessageHandler();
            HttpMessageHandler pipeline = factory.Create(innerChannel);
            Assert.AreSame(innerChannel, pipeline, "Expected pipeline to be same as inner channel"); 
        }

        #endregion Member Tests

        class DerivedHttpMessageHandlerFactory : HttpMessageHandlerFactory
        {
            public DerivedHttpMessageHandlerFactory()
            {
            }
        }
    }
}