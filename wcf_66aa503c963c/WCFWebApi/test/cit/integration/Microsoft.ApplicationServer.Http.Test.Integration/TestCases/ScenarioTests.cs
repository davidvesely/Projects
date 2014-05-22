// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.ApplicationServer.Http.TestCases;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ScenarioTests
    {
        public const string nullString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><int xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"/>";

        #region GET tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a GET request for all customers that already exist by default.")]
        public void Get_All_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Customers");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    string[] responseContent = response.Content.ReadAsStringAsync().Result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    Assert.AreEqual("Id = 1; Name = Customer1", responseContent[0], "The response content should have been 'Id = 1; Name = Customer1'.");
                    Assert.AreEqual("Id = 2; Name = Customer2", responseContent[1], "The response content should have been 'Id = 2; Name = Customer2'.");
                    Assert.AreEqual("Id = 3; Name = Customer3", responseContent[2], "The response content should have been 'Id = 3; Name = Customer3'.");
                }
            }
        }

        
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a GET request for a single customer that already exists by default.")]
        public void Get_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Customers?id=1");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual("Id = 1; Name = Customer1", response.Content.ReadAsStringAsync().Result, "The response content should have been 'Id = 1; Name = Customer1'.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Sends a GET request with json in accept header, expecting json in the response.")]
        public void Get_Existing_Customer_InJson()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(MyHelloService), baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Hello");
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    response.EnsureSuccessStatusCode();
                    Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a GET request for a customer that doesn't exist by default.")]
        public void Get_Non_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Customers?id=5");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status code should have been 'NotFound'.");
                    Assert.AreEqual("There is no customer with id '5'.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'There is no customer with id '5'.'");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a GET request with an id that can't be parsed as an integer.")]
        public void Get_With_Non_Integer_Id()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Customers?id=foo");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status code should have been 'BadRequest'.");
                    Assert.AreEqual("An 'id' with a integer value must be provided in the query string.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'An 'id' with a integer value must be provided in the query string.'");
                }
            }
        }

        #endregion GET Tests

        #region GET void Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a void service operation with Get.")]
        public void OneWay_Get_Succeeds()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Nothing");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a void service operation with Get.")]
        public void OneWay_Explicit_Get_Succeeds()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Nothing/Explicit");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode, "The status code should have been 'Accepted'.");
                }
            }
        }

        #endregion GET void Tests

        #region nullable Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a method which returns a nullable type.")]
        public void GetNullable()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(FooService), baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "nullableTest/5");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    response.EnsureSuccessStatusCode();
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(5, response.Content.ReadAsAsync<Nullable<int>>().Result);
                }

                request = new HttpRequestMessage(HttpMethod.Get, "nullableTest/0");
                response = client.SendAsync(request).Result;
                using (response)
                {
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(null, response.Content.ReadAsAsync<Nullable<int>>().Result);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a method which takes a nullable type.")]
        public void PostNullable()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(FooService), baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "nullableTest/5");
                
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    response.EnsureSuccessStatusCode();
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(5, response.Content.ReadAsAsync<int>().Result);
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a method which takes a null nullable type.")]
        public void PostNullableWithNull()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(FooService), baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "nullableNullTest");
                request.Content = new StringContent(nullString, new System.Text.UTF8Encoding(), "application/xml");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    response.EnsureSuccessStatusCode();
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(-1, response.Content.ReadAsAsync<int>().Result);
                }
            }
        }

        #endregion nullable Tests

        #region PUT Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a PUT request to update a single customer that already exists by default.")]
        public void Update_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "Customers?id=1");
                request.Content = new StringContent("Id = 1; Name = NewCustomerName1");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual("Id = 1; Name = NewCustomerName1", response.Content.ReadAsStringAsync().Result, "The response content should have been 'Id = 1; Name = NewCustomerName1'.");
                }

                // Put server back in original state
                request = new HttpRequestMessage(HttpMethod.Put, "Customers?id=1");
                request.Content = new StringContent("Id = 1; Name = Customer1");
                client.SendAsync(request).Wait();
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a PUT request to update a single customer that doesn't exist by default.")]
        public void Update_Non_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "Customers?id=5");
                request.Content = new StringContent("Id = 5; Name = NewCustomerName1");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status code should have been 'NotFound'.");
                    Assert.AreEqual("There is no customer with id '5'.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'There is no customer with id '5'.'");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)] 
        [Description("Put a strongly typed object using HttpClient")]
        public void PutStronglyTypedObjectInSelfHost() {

            var serviceUri = new Uri("http://localhost:8080/");

            using (HttpServiceHost host = new HttpServiceHost(typeof(FooService), serviceUri))
            {
                //Arrange
                host.AddDefaultEndpoints();
                host.Open();

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = serviceUri;

                //Act, 
                //TODO, CSDMain 235028, we need an fix from NCL to fix this bug, why do i need to set the media type at all?
                var body = new ObjectContent<foo>(new foo() { Bar = "Blah" }, "application/xml");
                body.Formatters.XmlFormatter.UseDataContractSerializer = true;
                var response = client.PutAsync(new Uri(serviceUri, "foo3"), body).Result;
                response.EnsureSuccessStatusCode();

                var respObject = response.Content.ReadAsAsync<foo>(new List<MediaTypeFormatter>() { new XmlMediaTypeFormatter() { UseDataContractSerializer = true } }).Result;

                // Assert
                Assert.AreEqual("Blah", respObject.Bar);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)]
        [Description("Put a http request message using HttpClient")]
        public void PutInSelfHost() {

            var serviceUri = new Uri("http://localhost:8080/");

            using (HttpServiceHost host = new HttpServiceHost(typeof(FooService), serviceUri))
            {
                //Arrange
                host.AddDefaultEndpoints();
                host.Open();

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = serviceUri;

                //Act
                MemoryStream stream = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(foo));
                serializer.WriteObject(stream, new foo() { Bar = "Blah" });
                stream.Position = 0;
                StreamContent content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                var response = client.PutAsync(new Uri(serviceUri, "foo3"), content).Result;
                response.EnsureSuccessStatusCode();
                var respObject = response.Content.ReadAsAsync<foo>(new List<MediaTypeFormatter>() { new XmlMediaTypeFormatter(){ UseDataContractSerializer = true }}).Result;

                // Assert
                Assert.AreEqual("Blah", respObject.Bar);
            }
        }

        #endregion PUT Tests

        #region POST Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a POST request to create a new customer.")]
        public void Create_New_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "Customers");
                request.Content = new StringContent("Id = 7; Name = NewCustomer7");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "The status code should have been 'Created'.");
                    Assert.IsNotNull(response.Headers.Location, "The location header should not have been null.");
                    Assert.AreEqual(new Uri("http://localhost:8080/Customers?id=7"), response.Headers.Location, "The location header should have been 'http://localhost:8080/Customers?id=7'.");
                    Assert.AreEqual("Id = 7; Name = NewCustomer7", response.Content.ReadAsStringAsync().Result, "The response content should have been 'Id = 7; Name = NewCustomer7'.");
                }

                // Put server back in original state
                request = new HttpRequestMessage(HttpMethod.Delete, "Customers?id=7");
                client.SendAsync(request).Wait();
            }
        }

        [TestMethod]
        [TestCategory("CIT"),Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Sends a POST request to trigger the formatter to throw exception on Read.")]
        public void Formatter_Negative_Read()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpConfiguration httpConfig = new HttpConfiguration();
            httpConfig.Formatters.Insert(0, new CustomThrowingFormatter());

            HttpServiceHost host = new HttpServiceHost(typeof(MyHelloService), httpConfig, baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "Hello3");
                request.Content = new StringContent("hongmei");
                request.Content.Headers.ContentType = CustomThrowingFormatter.FooMediaType;
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.Unused, response.StatusCode, "The status code should have been 'Unused'.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a POST request to create a customer that already exists.")]
        public void Create_Customer_That_Already_Exists()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "Customers?id=2");
                request.Content = new StringContent("Id = 2; Name = AlreadyCustomer2");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode, "The status code should have been 'Conflict'.");
                    Assert.AreEqual("There already a customer with id '2'.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'There already a customer with id '2'.'");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.ScenarioTests)]
        [Description("Invokes a void service operation with Get.")]
        public void OneWay_Void_POST_Succeeds()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            HttpServiceHost host = new HttpServiceHost(typeof(FooService), baseAddress);
            using (host)
            {
                host.AddDefaultEndpoints();
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "voidTest/1");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    response.EnsureSuccessStatusCode();
                    Assert.IsNotNull(response, "The response should not have been null.");
                }
            }
        }

        #endregion POST Tests

        #region DELETE Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a DELETE request to remove a single customer that already exists by default.")]
        public void Delete_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "Customers?id=3");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual(string.Empty, response.Content.ReadAsStringAsync().Result, "The response content should have been an empty string.");
                }

                // Put server back in the original state
                request = new HttpRequestMessage(HttpMethod.Post, "Customers");
                request.Content = new StringContent("Id = 3; Name = Customer3");
                client.SendAsync(request).Wait();
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a DELETE request to remove a single customer that doesn't exist.")]
        public void Delete_Non_Existing_Customer()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "Customers?id=4");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status code should have been 'NotFound'.");
                    Assert.AreEqual("There is no customer with id '4'.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'There is no customer with id '4'.'");
                }
            }
        }

        #endregion DELETE Tests

        #region CatchAll Tests

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a request that will get handled by the catch all operation because of the HTTP method used.")]
        public void Send_Request_With_Unknown_Method()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("UNKNOWN"), "Customers");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status code should have been 'NotFound'.");
                    Assert.AreEqual("The uri and/or method is not valid for any customer resource.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'The uri and/or method is not valid for any customer resource.'.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a request that will get handled by the catch all operation because of the URI used.")]
        public void Send_Request_With_Unknown_Uri()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "UnknownUri");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "The status code should have been 'NotFound'.");
                    Assert.AreEqual("The uri and/or method is not valid for any customer resource.", response.Content.ReadAsStringAsync().Result, "The response content should have been 'The uri and/or method is not valid for any customer resource.'.");
                }
            }
        }

        #endregion CatchAll Tests

        #region Message Inspector Tests
        
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner(TestOwner.ScenarioTests)]
        [Description("Sends a GET request for all customers and uses the 'NamesOnly' custom header.")]
        public void Get_With_Custom_Header_For_Customer_Names_Only()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            CustomServiceHost host = new CustomServiceHost(typeof(CustomerService), baseAddress);
            using (host)
            {
                host.Open();

                HttpClient client = new HttpClient();
                client.BaseAddress = baseAddress;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Customers");
                request.Headers.Add("NamesOnly", "Ok");
                HttpResponseMessage response = client.SendAsync(request).Result;
                using (response)
                {
                    Assert.IsNotNull(response, "The response should not have been null.");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The status code should have been 'OK'.");
                    Assert.AreEqual("Customer1, Customer2, Customer3", response.Content.ReadAsStringAsync().Result, "The response content should have been 'Customer1, Customer2, Customer3'.");
                }
            }
        }

        #endregion Message Inspector Tests
    }

    public class CustomThrowingFormatter : MediaTypeFormatter
    {
        internal static readonly MediaTypeHeaderValue FooMediaType = new MediaTypeHeaderValue("application/foo");

        public CustomThrowingFormatter()
        {
            this.SupportedMediaTypes.Add(FooMediaType);
        }

        protected override object OnReadFromStream(Type type, System.IO.Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders)
        {
            throw new HttpResponseException(HttpStatusCode.Unused);
        }

        protected override void OnWriteToStream(Type type, object value, System.IO.Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            throw new NotImplementedException();
        }
    }

   

}
