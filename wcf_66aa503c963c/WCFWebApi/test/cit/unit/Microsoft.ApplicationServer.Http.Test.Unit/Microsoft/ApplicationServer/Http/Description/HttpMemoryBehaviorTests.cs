// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Threading;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpMemoryBehavior)), UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMemoryBehaviorTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpMemoryBehavior is public.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region Constructor Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpMemoryBehavior ctor sets defaults")]
        public void HttpMemoryBehavior_Ctor_Defaults()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            Assert.IsNotNull(behavior, "Behavior should not be null");
        }

        #endregion

        #region AddBindingParameters Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddBindingParameters(ServiceEndpoint, BindingParameterCollection) with null endpoint throws.")]
        public void HttpMemoryBehavior_AddBindingParameters_Throws_For_Null_Endpoint()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            BindingParameterCollection bindingParameterCollection = new BindingParameterCollection();
            Asserters.Exception.ThrowsArgumentNull("endpoint", () => behavior.AddBindingParameters(null, bindingParameterCollection));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddBindingParameters(ServiceEndpoint, BindingParameterCollection) with null bindingParameterCollection parameter throws.")]
        public void HttpMemoryBehavior_ApplyDispatchBehavior_Throws_For_Null_BindingParameterCollection()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            ContractDescription cd = ContractDescription.GetContract(typeof(CustomerService));
            ServiceEndpoint endpoint = new ServiceEndpoint(cd);
            Asserters.Exception.ThrowsArgumentNull("bindingParameters", () => behavior.AddBindingParameters(endpoint, null));
        }

        #endregion AddBindingParameters Tests

        #region ApplyClientBehavior Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ApplyClientBehavior(ServiceEndpoint, ClientRuntime) throws")]
        public void HttpMemoryBehavior_ApplyClientBehavior_Throws()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            ContractDescription cd = ContractDescription.GetContract(typeof(CustomerService));
            ServiceEndpoint endpoint = new ServiceEndpoint(cd);
            EndpointDispatcher dispatcher = new EndpointDispatcher(new EndpointAddress("http://someuri"), cd.Name, cd.Namespace);
            Asserters.Exception.Throws<NotSupportedException>(
                "ApplyClientBehavior throws always",
                Http.SR.ApplyClientBehaviorNotSupportedByBehavior(typeof(HttpMemoryBehavior).Name),
                () => ((IEndpointBehavior)behavior).ApplyClientBehavior(null, null));
        }

        #endregion ApplyClientBehavior Tests

        #region ApplyDispatchBehavior Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ApplyDispatchBehavior(ServiceEndpoint, EndpointDispatcher) with null endpoint throws.")]
        public void HttpMemoryBehavior_ApplyDispatchBehavior_Throws_For_Null_Endpoint()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            ContractDescription cd = ContractDescription.GetContract(typeof(CustomerService));
            ServiceEndpoint endpoint = new ServiceEndpoint(cd);
            EndpointDispatcher dispatcher = new EndpointDispatcher(new EndpointAddress("http://someuri"), cd.Name, cd.Namespace);
            Asserters.Exception.ThrowsArgumentNull("endpoint", () => behavior.ApplyDispatchBehavior(null, dispatcher));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ApplyDispatchBehavior(ServiceEndpoint, EndpointDispatcher) with null endpointDispatcher parameter throws.")]
        public void HttpMemoryBehavior_ApplyDispatchBehavior_Throws_For_Null_EndpointDispatcher()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            ContractDescription cd = ContractDescription.GetContract(typeof(CustomerService));
            ServiceEndpoint endpoint = new ServiceEndpoint(cd);
            EndpointDispatcher dispatcher = new EndpointDispatcher(new EndpointAddress("http://someuri"), cd.Name, cd.Namespace);
            Asserters.Exception.ThrowsArgumentNull("endpointDispatcher", () => behavior.ApplyDispatchBehavior(endpoint, null));
        }

        #endregion

        #region Validate Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Validate(ServiceEndpoint) does not throw for a valid HttpBinding.")]
        public void Validate_Does_Not_Throw()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(CustomerService)));
            endpoint.Binding = new HttpBinding();
            endpoint.Address = new EndpointAddress("http://somehost");
            behavior.Validate(endpoint);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Validate(ServiceEndpoint) throws if the 'endpoint' parameter is null.")]
        public void Validate_Throws_With_Null_Endpoint()
        {
            HttpMemoryBehavior behavior = new HttpMemoryBehavior();
            Asserters.Exception.ThrowsArgumentNull("endpoint", () => behavior.Validate(null));
        }

        #endregion Validate Tests
    }
}
