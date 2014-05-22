// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(HttpOperationDescriptionExtensionMethods))]
    public class HttpOperationDescriptionExtensionMethodsTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpOperationDescriptionExtensionMethods is a class that is public, abstract, sealed (static).")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Methods

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("GetHttpMethod returns the correct HttpMethod for the operation.")]
        public void GetHttpMethod_Returns_HttpMethod()
        {
            Asserters.Exception.ThrowsArgumentNull("operation", () => ((HttpOperationDescription)null).GetHttpMethod());

            ContractDescription contract = ContractDescription.GetContract(typeof(WebMethodService));
            
            OperationDescription operationDescription = contract.Operations.Where(od => od.Name == "NoAttributeOperation").FirstOrDefault();
            HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Post, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should return 'POST' for operations with no WebGet or WebInvoke attribute.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Post, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should return 'POST' for operations with WebInvoke attribute but no Method set explicitly.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebGetOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Get, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should return 'GET' for operations with WebGet.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeGetOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Get, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'GET'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeGetLowerCaseOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(new HttpMethod("Get"), httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'Get'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokePutOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Put, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'PUT'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokePostOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Post, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'POST'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeDeleteOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(HttpMethod.Delete, httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'DELETE'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeCustomOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Assert.AreEqual(new HttpMethod("Custom"), httpOperationDescription.GetHttpMethod(), "HttpOperationDescription.GetHttpMethod should have return 'Custom'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("GetUriTemplate(HttpOperationDescription) returns the correct UriTemplate for the operation.")]
        public void GetUriTemplate_Returns_UriTemplate()
        {
            Asserters.Exception.ThrowsArgumentNull("operation", () => ((HttpOperationDescription)null).GetUriTemplate());

            ContractDescription contract = ContractDescription.GetContract(typeof(UriTemplateService));

            OperationDescription operationDescription = contract.Operations.Where(od => od.Name == "NoAttributeOperation").FirstOrDefault();
            HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();
            UriTemplate template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count , "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeSansTemplateStringOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebGetSansTemplateStringOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeWithParametersOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebGetWithParametersOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(2, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with two query variables.");
            Assert.AreEqual("IN1", template.QueryValueVariableNames[0], "HttpOperationDescription.GetUriTemplate should return a UriTemplate with query variable 'IN1'.");
            Assert.AreEqual("IN2", template.QueryValueVariableNames[1], "HttpOperationDescription.GetUriTemplate should return a UriTemplate with query variable 'IN2'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeWithEmptyTemplateStringOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(0, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebInvokeWithTemplateStringOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(1, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with one path variables.");
            Assert.AreEqual(1, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with one query variables.");
            Assert.AreEqual("VARIABLE1", template.PathSegmentVariableNames[0], "HttpOperationDescription.GetUriTemplate should return a UriTemplate with query variable 'VARIABLE1'.");
            Assert.AreEqual("VARIABLE2", template.QueryValueVariableNames[0], "HttpOperationDescription.GetUriTemplate should return a UriTemplate with query variable 'VARIABLE2'.");

            operationDescription = contract.Operations.Where(od => od.Name == "WebGetWithTemplateStringOperation").FirstOrDefault();
            httpOperationDescription = operationDescription.ToHttpOperationDescription();
            template = httpOperationDescription.GetUriTemplate();
            Assert.AreEqual(1, template.PathSegmentVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with one path variables.");
            Assert.AreEqual(0, template.QueryValueVariableNames.Count, "HttpOperationDescription.GetUriTemplate should return a UriTemplate with zero query variables.");
            Assert.AreEqual("VARIABLE1", template.PathSegmentVariableNames[0], "HttpOperationDescription.GetUriTemplate should return a UriTemplate with query variable 'VARIABLE1'.");
        }

        [TestMethod]
        [TestCategory("CIT"),Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetUriTemplate(HttpOperationDescription) returns the a UriTemplate with IgnoreTrailingSlashMode==false by default.")]
        public void GetUriTemplateReturnsDefaultTrailingSlashMode()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(UriTemplateService));

            OperationDescription operationDescription = contract.Operations.Where(od => od.Name == "NoAttributeOperation").FirstOrDefault();
            HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();
            UriTemplate template = httpOperationDescription.GetUriTemplate();
            Assert.IsFalse(template.IgnoreTrailingSlash, "Default UriTemplate should not ignore trailing slashes.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetUriTemplate(HttpOperationDescription, TrailingSlashMode) returns the a UriTemplate with IgnoreTrailingSlashMode as specified.")]
        public void GetUriTemplateReturnsSpecifiedTrailingSlashMode()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(UriTemplateService));

            OperationDescription operationDescription = contract.Operations.Where(od => od.Name == "NoAttributeOperation").FirstOrDefault();
            HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();

            // Asking for AutoRedirect will not ignore trailing slash
            UriTemplate template = httpOperationDescription.GetUriTemplate(TrailingSlashMode.AutoRedirect);
            Assert.IsFalse(template.IgnoreTrailingSlash, "Default UriTemplate should have been set not to ignore trailing slashes.");

            // Asking for Ignore should ignore trailing slash
            template = httpOperationDescription.GetUriTemplate(TrailingSlashMode.Ignore);
            Assert.IsTrue(template.IgnoreTrailingSlash, "Default UriTemplate should have been set to ignore trailing slashes.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetUriTemplate(HttpOperationDescription, TrailingSlashMode) throws with an illegal trailing slash mode.")]
        public void GetUriTemplateThrowsWithIllegalTrailingSlashMode()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(UriTemplateService));

            OperationDescription operationDescription = contract.Operations.Where(od => od.Name == "NoAttributeOperation").FirstOrDefault();
            HttpOperationDescription httpOperationDescription = operationDescription.ToHttpOperationDescription();
            Asserters.Exception.Throws<System.ComponentModel.InvalidEnumArgumentException>(
                () => { httpOperationDescription.GetUriTemplate((TrailingSlashMode)99); },
                (ex) =>
                {
                    Assert.AreEqual("trailingSlashMode", ex.ParamName, "ParamName should have been 'trailingSlashMode'");
                });
        }

        #endregion Methods

        #region Test helpers

        public static OperationDescription GetOperationDescription(Type contractType, string methodName)
        {
            ContractDescription cd = ContractDescription.GetContract(contractType);
            OperationDescription od = cd.Operations.FirstOrDefault(o => o.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(od, "Failed to get operation description for " + methodName);
            return od;
        }

        #endregion Test helpers
    }
}
