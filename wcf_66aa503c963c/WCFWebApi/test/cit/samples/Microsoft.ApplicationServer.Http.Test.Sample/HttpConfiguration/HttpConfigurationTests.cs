// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HttpConfigurationSample = global::HttpConfiguration.Sample;

    /// <summary>
    /// Tests for <see cref="Microsoft.ApplicationServer.Http.Configuration.Program"/> sample.
    /// </summary>
    [TestClass]
    public class HttpConfigurationSampleTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddFormatter run without exceptions.")]
        public void HttpConfiguration_AddFormatter()
        {
            HttpConfigurationSample.AddFormatter.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.EnableHelpPage run without exceptions.")]
        public void HttpConfiguration_EnableHelpPage()
        {
            HttpConfigurationSample.EnableHelpPage.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddMessageHandler run without exceptions.")]
        public void HttpConfiguration_AddMessageHandler()
        {
            HttpConfigurationSample.AddMessageHandler.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddMessageHandlerByType run without exceptions.")]
        public void HttpConfiguration_AddMessageHandler_ByType()
        {
            HttpConfigurationSample.AddMessageHandlerByType.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetInstanceProvider run without exceptions.")]
        public void HttpConfiguration_SetInstanceProvider()
        {
            HttpConfigurationSample.SetInstanceProvider.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetInstanceProviderMultipleHost run without exceptions.")]
        public void HttpConfiguration_SetInstanceProvider_MultipleHost()
        {
            HttpConfigurationSample.SetInstanceProviderMultipleHost.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddRequestHandler run without exceptions.")]
        public void HttpConfiguration_AddRequestHandler()
        {
            HttpConfigurationSample.AddRequestHandler.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetTrailingSlashMode run without exceptions.")]
        public void HttpConfiguration_SetTrailingSlashMode()
        {
            HttpConfigurationSample.SetTrailingSlashMode.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddResponseHandler run without exceptions.")]
        public void HttpConfiguration_AddResponseHandler()
        {
            HttpConfigurationSample.AddResponseHandler.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.AddErrorHandler run without exceptions.")]
        public void HttpConfiguration_AddErrorHandler()
        {
            HttpConfigurationSample.AddErrorHandler.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.IncludeExceptionDetail run without exceptions.")]
        public void HttpConfiguration_IncludeExceptionDetail()
        {
            HttpConfigurationSample.IncludeExceptionDetail.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetTransferMode run without exceptions.")]
        public void HttpConfiguration_SetTransferMode()
        {
            HttpConfigurationSample.SetTransferMode.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetMaxBufferSizeAndMaxReceivedMessageSize run without exceptions.")]
        public void HttpConfiguration_SetMaxBufferSize_MaxReceivedMessageSize()
        {
            HttpConfigurationSample.SetMaxBufferSizeAndMaxReceivedMessageSize.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetSecurity run without exceptions.")]
        public void HttpConfiguration_SetSecurity()
        {
            HttpConfigurationSample.SetSecurity.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.UseDataContractSerializer run without exceptions.")]
        public void HttpConfiguration_UseDataContractSerializer()
        {
            HttpConfigurationSample.UseDataContractSerializer.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetSerializerOnXmlFormatter run without exceptions.")]
        public void HttpConfiguration_SetSerializerOnXmlFormatter()
        {
            HttpConfigurationSample.SetSerializerOnXmlFormatter.Run();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Verify that HttpConfiguration.SetSerializerOnJsonFormatter run without exceptions.")]
        public void HttpConfiguration_SetSerializerOnJsonFormatter()
        {
            HttpConfigurationSample.SetSerializerOnJsonFormatter.Run();
        }
    }
}