// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Threading;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpMemoryChannel)), UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpMemoryChannelTests : UnitTest
    {
        #region Type Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryChannel is internal non-abstract class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<HttpMemoryChannel, IReplyChannel>(
                TypeAssert.TypeProperties.IsClass |
                TypeAssert.TypeProperties.IsDisposable);
        }

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryHandler constructor.")]
        public void HttpMemoryHandlerConstructor()
        {
            HttpMemoryChannelListener listener = HttpMemoryChannelListenerTests.CreateHttpMemoryChannelListener();
            HttpMemoryChannel channel = new HttpMemoryChannel(listener);
            Assert.IsNotNull(channel, "channel should not be null");
            Assert.AreEqual(listener.Uri, channel.LocalAddress.Uri, "Local address did not match expected value.");
            Assert.IsNotNull(channel.HttpMemoryHandler, "Handler should not be null");
        }
        #endregion Constructor Tests

        #region Members

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMemoryChannel)]
        [Description("Dispose should dispose memory handler.")]
        public void HttpMemoryChannelShouldDisposeMemoryHandler()
        {
            HttpMemoryChannelListener listener = HttpMemoryChannelListenerTests.CreateHttpMemoryChannelListener();
            HttpMemoryChannel channel = new HttpMemoryChannel(listener);
            HttpMemoryHandler handler = channel.HttpMemoryHandler;
            channel.Dispose();

            Asserters.Exception.ThrowsObjectDisposed(typeof(HttpMemoryHandler).FullName,
                () =>
                {
                    handler.SubmitRequestAsync(new HttpRequestMessage(), CancellationToken.None);
                });
        }

        #endregion
    }
}