// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.ServiceModel.Channels;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageEncoderFactoryTests
    {
        #region Type Tests
        
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageEncoder)]
        [Description("HttpMessageEncoderFactory is an internal, non-abstract class.")]
        public void HttpMessageEncoderFactory_Is_An_Internal_Non_Abstract_Class()
        {
            UnitTest.Asserters.Type.HasProperties<HttpMessageEncoderFactory>(TypeAssert.TypeProperties.IsClass);
        }

        #endregion  Type Tests

        #region Property Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageEncoder)]
        [Description("HttpMessageEncoderFactory.Encoder returns a MessageEncoder.")]
        public void MessageEncoderFactory_Encoder_Returns_A_MessageEncoder()
        {
            HttpMessageEncoderFactory factory = new HttpMessageEncoderFactory();
            MessageEncoder encoder = factory.Encoder;
            Assert.IsNotNull(encoder, "HttpMessageEncoderFactory.Encoder should have returned null.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageEncoder)]
        [Description("HttpMessageEncoderFactory.MessageVersion is always MessageVersion.None.")]
        public void MessageVersion_Is_None()
        {
            HttpMessageEncoderFactory factory = new HttpMessageEncoderFactory();
            Assert.AreEqual(MessageVersion.None, factory.MessageVersion, "HttpMessageEncoderFactory.MessageVersion should always be MessageVersion.None.");
        }

        #endregion Property Tests

        #region CreateSessionEncoder Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.HttpMessageEncoder)]
        [Description("HttpMessageEncoderFactory.CreateSessionEncoder always throws.")]
        public void CreateSessionEncoder_Throws()
        {
            HttpMessageEncoderFactory factory = new HttpMessageEncoderFactory();

            UnitTest.Asserters.Exception.Throws <NotSupportedException>(
                SR.HttpMessageEncoderFactoryDoesNotSupportSessionEncoder(typeof(HttpMessageEncoderFactory)),
                () =>
                {                   
                    factory.CreateSessionEncoder();
                });
        }

        #endregion CreateSessionEncoder Tests
    }
}
