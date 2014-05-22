// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Web;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(ServiceResourceInvoker))]
    public class ServiceResourceInvokerTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("RequestUriIntellisenseInvoker is internal and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("Invoke(object, object[], out object[]) returns json serialized service resources.")]
        public void InvokeReturnsServiceResources()
        {
            const string BaseAddress = "http://localhost:8080/books";

            HttpEndpoint endpoint = new HttpEndpoint(new ContractDescription(typeof(BookService).Name), new EndpointAddress(BaseAddress));
            endpoint.Contract = ContractDescription.GetContract(typeof(BookService));
            endpoint.HelpEnabled = true;

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;

            ServiceResourceInvoker invoker = new ServiceResourceInvoker(endpoint);
            object[] dummy;
            HttpResponseMessage response = (HttpResponseMessage)invoker.Invoke(null, new object[] { request }, out dummy);

            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.AreEqual("{\"HelpEnabled\":true,\"Resources\":[" + 
                "{\"Uri\":\"http://localhost:8080/books\",\"BaseAddress\":\"http://localhost:8080/books\",\"Operations\":[{\"HttpMethod\":\"GET\",\"Name\":\"Get\"},{\"HttpMethod\":\"POST\",\"Name\":\"Add\"}]}," +
                "{\"Uri\":\"" + HttpUtility.JavaScriptStringEncode("http://localhost:8080/books/{category}?id={id}&name={name}&isavailable  ={available}") + "\",\"BaseAddress\":\"http://localhost:8080/books\",\"Operations\":[{\"HttpMethod\":\"GET\",\"Name\":\"GetCategory\"}]}," +
                "{\"Uri\":\"http://localhost:8080/books/{i}\",\"BaseAddress\":\"http://localhost:8080/books\",\"Operations\":[{\"HttpMethod\":\"DELETE\",\"Name\":\"Remove\"},{\"HttpMethod\":\"PUT\",\"Name\":\"Modify\"}]}]}", 
                response.Content.ReadAsStringAsync().Result);
        }

        #endregion Methods
    }
}
