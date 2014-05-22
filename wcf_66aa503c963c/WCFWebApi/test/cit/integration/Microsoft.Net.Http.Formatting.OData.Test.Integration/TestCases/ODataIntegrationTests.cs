// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.Integration
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataIntegrationTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the response from an Http GET in OData atom format when the accept header is application/atom+xml")]
        public void Get_Entry_In_OData_Atom_Format()
        {
            string baseAddress = "http://localhost:8080/TestService/";
            Uri baseAddressUri = new Uri(baseAddress);
            HttpServiceHost host = new HttpServiceHost(typeof(TestService), baseAddress);
            host.AddDefaultEndpoints();
            HttpEndpoint endpoint = host.Description.Endpoints.OfType<HttpEndpoint>().Single();
            endpoint.OperationHandlerFactory.Formatters.Insert(0, new ODataMediaTypeFormatter());
            MediaTypeWithQualityHeaderValue atomMediaType = new MediaTypeWithQualityHeaderValue("application/atom+xml");
            host.Open();
            using (host)
            {  
                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddressUri;
                HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(baseAddress + "GetPerson"));
                requestMessage.Headers.Accept.Add(atomMediaType);
                requestMessage.Headers.Add("DataServiceVersion", "2.0");
                requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(response.Content.Headers.ContentType.MediaType, atomMediaType.MediaType);
                    Assert.AreEqual(Util.GetDataServiceVersion(response.Headers), Util.Version2NumberString);

                    Util.VerifyResponse(response.Content, BaselineResource.EntryTypePersonAtom);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the response from an Http GET in OData atom format when the accept header is application/json")]
        public void Get_Entry_In_OData_Json_Format()
        {
            string baseAddress = "http://localhost:8080/TestService/";
            Uri baseAddressUri = new Uri(baseAddress);
            HttpServiceHost host = new HttpServiceHost(typeof(TestService), baseAddress);
            host.AddDefaultEndpoints();
            HttpEndpoint endpoint = host.Description.Endpoints.OfType<HttpEndpoint>().Single();
            endpoint.OperationHandlerFactory.Formatters.Insert(0, new ODataMediaTypeFormatter());
            MediaTypeWithQualityHeaderValue jsonMediaType = new MediaTypeWithQualityHeaderValue(Util.ApplicationJsonMediaType.MediaType);
            host.Open();
            using (host)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddressUri;
                HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(baseAddress + "GetPerson"));
                requestMessage.Headers.Accept.Add(jsonMediaType);
                requestMessage.Headers.Add("DataServiceVersion", "2.0");
                requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(response.Content.Headers.ContentType.MediaType, jsonMediaType.MediaType);
                    Assert.AreEqual(Util.GetDataServiceVersion(response.Headers), Util.Version2NumberString);

                    // this request should be handled by OData Json
                    Util.VerifyJsonResponse(response.Content, BaselineResource.EntryTypePersonODataJson );
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the ODataMediaTypeFormatter to only support application/atom+xml")]
        public void Support_Only_OData_Atom_Format()
        {
            string baseAddress = "http://localhost:8080/TestService/";
            Uri baseAddressUri = new Uri(baseAddress);
            HttpServiceHost host = new HttpServiceHost(typeof(TestService), baseAddress);
            host.AddDefaultEndpoints();
            HttpEndpoint endpoint = host.Description.Endpoints.OfType<HttpEndpoint>().Single();
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter();
            odataFormatter.SupportedMediaTypes.Remove(Util.ApplicationJsonMediaType);
            endpoint.OperationHandlerFactory.Formatters.Insert(0, odataFormatter);
            MediaTypeWithQualityHeaderValue atomMediaType = new MediaTypeWithQualityHeaderValue("application/atom+xml");
            host.Open();
            using (host)
            {      
                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddressUri;

                HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(baseAddress + "GetPerson"));
                requestMessage.Headers.Accept.Add(atomMediaType);
                requestMessage.Headers.Add("DataServiceVersion", "2.0");
                requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(response.Content.Headers.ContentType.MediaType, atomMediaType.MediaType);
                    Assert.AreEqual(Util.GetDataServiceVersion(response.Headers), Util.Version2NumberString);

                    Util.VerifyResponse(response.Content, BaselineResource.EntryTypePersonAtom);
                }

                HttpRequestMessage messageWithJsonHeader = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(baseAddress + "GetPerson"));
                messageWithJsonHeader.Headers.Accept.Add(Util.ApplicationJsonMediaTypeWithQuality);
                messageWithJsonHeader.Headers.Add("DataServiceVersion", "2.0");
                messageWithJsonHeader.Headers.Add("MaxDataServiceVersion", "3.0");
                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(response.Content.Headers.ContentType.MediaType, Util.ApplicationJsonMediaTypeWithQuality.MediaType);
                    Assert.IsNull(Util.GetDataServiceVersion(response.Headers));

                    Util.VerifyJsonResponse(response.Content, BaselineResource.EntryTypePersonRegularJson);
                }
            }
        }

        [Ignore] // TODO: reactivate test when fix CSDMAIN 234391
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how ODataMediaTypeFormatter would conditionally support application/atom+xml and application/json only if format=odata is present in the QueryString")]
        public void Conditionally_Support_OData_If_Query_String_Present()
        {
            string baseAddress = "http://localhost:8080/TestService/";
            Uri baseAddressUri = new Uri(baseAddress);
            HttpServiceHost host = new HttpServiceHost(typeof(TestService), baseAddress);
            host.AddDefaultEndpoints();
            HttpEndpoint endpoint = host.Description.Endpoints.OfType<HttpEndpoint>().Single();
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter();
            odataFormatter.SupportedMediaTypes.Clear();
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationAtomMediaTypeWithQuality));
            odataFormatter.MediaTypeMappings.Add(new ODataMediaTypeMapping(Util.ApplicationJsonMediaTypeWithQuality));
            endpoint.OperationHandlerFactory.Formatters.Insert(0, odataFormatter);
            
            host.Open();
            using (host)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddressUri;

                // this request should return response in OData atom format
                HttpRequestMessage requestMessage = Util.GenerateRequestMessage(true, new Uri(baseAddress + "GetPerson?format=odata"));
                using (HttpResponseMessage response = client.SendAsync(requestMessage).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(Util.ApplicationAtomMediaTypeWithQuality.MediaType, response.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual(Util.GetDataServiceVersion(response.Headers), Util.Version2NumberString);
                    Util.VerifyResponse(response.Content, BaselineResource.EntryTypePersonAtom);
                }

                // this request should return response in OData json format
                HttpRequestMessage messageWithJsonHeader = Util.GenerateRequestMessage(false, new Uri(baseAddress + "GetPerson?format=odata"));
                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(Util.ApplicationJsonMediaType.MediaType, response.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual(Util.GetDataServiceVersion(response.Headers), Util.Version2NumberString);

                    // this request should be handled by OData Json
                    Util.VerifyJsonResponse(response.Content, BaselineResource.EntryTypePersonODataJson);
                }


                // when the query string is not present, request should be handled by the regular Json Formatter
                messageWithJsonHeader = Util.GenerateRequestMessage(false, new Uri(baseAddress + "GetPerson"));

                using (HttpResponseMessage response = client.SendAsync(messageWithJsonHeader).Result)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(response.Content.Headers.ContentType.MediaType, Util.ApplicationJsonMediaTypeWithQuality.MediaType);
                    Assert.IsNull(Util.GetDataServiceVersion(response.Headers));

                    Util.VerifyJsonResponse(response.Content, BaselineResource.EntryTypePersonRegularJson);
                }
            }
        }
    }
}
