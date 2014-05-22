// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpServiceHostTests : UnitTest<HttpServiceHost>
    {
        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost is public, concrete and disposable.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, 
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsDisposable,
                typeof(ServiceHost));
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(object, string[]) ctor")]
        public void ServiceHost_SingletonStringCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address string",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(new CustomerService(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(new CustomerServiceWithAspNetRequiredMode(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(new CustomerService(), address));
                });
            
            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("singletonInstance", () => { new HttpServiceHost((object)null, "http://somehost"); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(new CustomerService(), (string[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(new CustomerService(), "path"); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(object, HttpConfiguration, string[]) ctor")]
        public void ServiceHost_SingletonConfigStringCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address string",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(new CustomerService(), new HttpConfiguration(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(new CustomerServiceWithAspNetRequiredMode(), new HttpConfiguration(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(new CustomerService(), new HttpConfiguration(), address));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("singletonInstance", () => { new HttpServiceHost((object)null, new HttpConfiguration(), "http://somehost"); });

            // Throws on null config
            Asserters.Exception.ThrowsArgumentNull("configuration", () => { new HttpServiceHost(new CustomerService(), (HttpConfiguration)null, "http://somehost"); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(new CustomerService(), new HttpConfiguration(), (string[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(new CustomerService(), new HttpConfiguration(), "path"); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(object, Uri[]) ctor")]
        public void ServiceHost_SingletonUriCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address URI",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(new CustomerService(), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(new CustomerServiceWithAspNetRequiredMode(), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(new CustomerService(), new Uri(address)));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("singletonInstance", () => { new HttpServiceHost((object)null, new Uri("http://somehost")); });

            // Throws on null baseAddresses URIs
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(new CustomerService(), (Uri[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(new CustomerService(), new Uri("path", UriKind.Relative)); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(object, HttpConfiguration, Uri[]) ctor")]
        public void ServiceHost_SingletonConfigUriCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address string",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(new CustomerService(), new HttpConfiguration(), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(new CustomerServiceWithAspNetRequiredMode(), new HttpConfiguration(), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(new CustomerService(), new HttpConfiguration(), new Uri(address)));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("singletonInstance", () => { new HttpServiceHost((object)null, new HttpConfiguration(), new Uri("http://somehost")); });

            // Throws on null config
            Asserters.Exception.ThrowsArgumentNull("configuration", () => { new HttpServiceHost(new CustomerService(), (HttpConfiguration)null, new Uri("http://somehost")); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(new CustomerService(), new HttpConfiguration(), (Uri[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(new CustomerService(), new HttpConfiguration(), new Uri("path", UriKind.Relative)); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(Type, string[]) ctor")]
        public void ServiceHost_TypeStringCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address URI",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(typeof(CustomerServiceWithAspNetRequiredMode), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(typeof(CustomerService), address));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("serviceType", () => { new HttpServiceHost((Type)null, "http://somehost"); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), (string[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), "path"); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(Type, HttpConfiguration, string[]) ctor")]
        public void ServiceHost_TypeConfigStringCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address string",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(typeof(CustomerServiceWithAspNetRequiredMode), new HttpConfiguration(), address);
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), address));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("serviceType", () => { new HttpServiceHost((Type)null, new HttpConfiguration(), "http://somehost"); });

            // Throws on null config
            Asserters.Exception.ThrowsArgumentNull("configuration", () => { new HttpServiceHost(typeof(CustomerService), (HttpConfiguration)null, "http://somehost"); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), (string[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), "path"); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(Type, Uri[]) ctor")]
        public void ServiceHost_TypeUriCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address URI",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(typeof(CustomerServiceWithAspNetRequiredMode), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(typeof(CustomerService), new Uri(address)));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("serviceType", () => { new HttpServiceHost((Type)null, new Uri("http://somehost")); });

            // Throws on null baseAddresses URIs
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), (Uri[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), new Uri("path", UriKind.Relative)); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost(Type, HttpConfiguration, Uri[]) ctor")]
        public void ServiceHost_TypeConfigUriCtors()
        {
            // Singleton object ctor works
            Asserters.Data.Execute(DataSets.Http.LegalHttpAddresses, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to function with legal base address string",
                (type, obj) =>
                {
                    string address = (string)obj;
                    HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Allowed);

                    host = new HttpServiceHost(typeof(CustomerServiceWithAspNetRequiredMode), new HttpConfiguration(),new Uri(address));
                    VerifyAspNetCompatibilityRequirements(host, AspNetCompatibilityRequirementsMode.Required);
                });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "HttpServiceHost ctors failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("baseAddresses", () => new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), new Uri(address)));
                });

            // Throws on null singleton instance
            Asserters.Exception.ThrowsArgumentNull("serviceType", () => { new HttpServiceHost((Type)null, new HttpConfiguration(), new Uri("http://somehost")); });

            // Throws on null config
            Asserters.Exception.ThrowsArgumentNull("configuration", () => { new HttpServiceHost(typeof(CustomerService), (HttpConfiguration)null, new Uri("http://somehost")); });

            // Throws on null baseAddresses strings
            Asserters.Exception.ThrowsArgumentNull("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), (Uri[])null); });

            // Throws on relative URI
            Asserters.Exception.ThrowsArgument("baseAddresses", () => { new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), new Uri("path", UriKind.Relative)); });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddDefaultEndpoints() adds one endpoint.")]
        public void AddDefaultEndpoints()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), "http://somehost");
            Assert.AreEqual(0, host.Description.Endpoints.Count, "Expected 0 endpoints");

            host.AddDefaultEndpoints();
            Assert.AreEqual(1, host.Description.Endpoints.Count, "Expected 1 default endpoints");
            ServiceEndpoint endpoint = host.Description.Endpoints.ElementAt(0);

            DispatcherSynchronizationBehavior dispatcherSynchronizationBehavior = endpoint.Behaviors.Find<DispatcherSynchronizationBehavior>();
            Assert.IsNotNull(dispatcherSynchronizationBehavior, "DispatcherSynchronizationBehavior should be present.");
            Assert.IsTrue(dispatcherSynchronizationBehavior.AsynchronousSendEnabled, "DispatcherSynchronizationBehavior should have AsynchronousSendEnabled = true");

            ServiceDebugBehavior servicedebugBehavior = endpoint.Behaviors.Find<ServiceDebugBehavior>();
            Assert.IsNotNull(dispatcherSynchronizationBehavior, "ServiceDebugBehavior should be present.");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddDefaultEndpoints() throws if existing endpoints.")]
        public void AddDefaultEndpointsThrowsIfExistingEndpoints()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), "http://somehost");
            host.AddServiceEndpoint(new HttpEndpoint(ContractDescription.GetContract(typeof(CustomerService)), new EndpointAddress("http://somehost")));
            Assert.AreEqual(1, host.Description.Endpoints.Count, "Expected 1 endpoints");
            Asserters.Exception.Throws<InvalidOperationException>(SR.DefaultEndpointsMustBeAddedFirst,
                () =>
                {
                    host.AddDefaultEndpoints();
                });
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpEndpoint(Type, String) works as expected")]
        public void AddHttpEndpointString()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), "http://somehost/");

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpEndpoint(null, "http://somehost/"); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpEndpoint(typeof(CustomerService), (string)null); });

            HttpEndpoint httpEndpoint1 = host.AddHttpEndpoint(typeof(CustomerService), "");
            Assert.AreEqual("http://somehost/", httpEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpEndpoint httpEndpoint2 = host.AddHttpEndpoint(typeof(CustomerService), "/path");
            Assert.AreEqual("http://somehost/path", httpEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpEndpoint httpEndpoint3 = host.AddHttpEndpoint(typeof(CustomerService), "http://otherhost/");
            Assert.AreEqual("http://otherhost/", httpEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpEndpoint(Type, Uri) works as expected")]
        public void AddHttpEndpointUri()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new HttpConfiguration(), new Uri("http://somehost/"));

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpEndpoint(null, new Uri("http://somehost/")); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpEndpoint(typeof(CustomerService), (Uri)null); });

            HttpEndpoint httpEndpoint1 = host.AddHttpEndpoint(typeof(CustomerService), new Uri("", UriKind.Relative));
            Assert.AreEqual("http://somehost/", httpEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpEndpoint httpEndpoint2 = host.AddHttpEndpoint(typeof(CustomerService), new Uri("/path", UriKind.Relative));
            Assert.AreEqual("http://somehost/path", httpEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpEndpoint httpEndpoint3 = host.AddHttpEndpoint(typeof(CustomerService), new Uri("http://otherhost/"));
            Assert.AreEqual("http://otherhost/", httpEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpMemoryEndpoint(Type, String) works as expected")]
        public void AddHttpMemoryEndpointString()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), "http://somehost/");

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpMemoryEndpoint(null, "http://somehost/"); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpMemoryEndpoint(typeof(CustomerService), (string)null); });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "AddHttpMemoryEndpoint failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("address", () => host.AddHttpMemoryEndpoint(typeof(CustomerService), address));
                });

            HttpMemoryEndpoint HttpMemoryEndpoint1 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "");
            Assert.AreEqual("http://somehost/", HttpMemoryEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint2 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "/path");
            Assert.AreEqual("http://somehost/path", HttpMemoryEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint3 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "http://otherhost/");
            Assert.AreEqual("http://otherhost/", HttpMemoryEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpMemoryEndpoint(Type, String, HttpMemoryConfiguration) works as expected")]
        public void AddHttpMemoryEndpointStringConfig()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), "http://somehost/");

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpMemoryEndpoint(null, "http://somehost/", new HttpMemoryConfiguration()); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpMemoryEndpoint(typeof(CustomerService), (string)null, new HttpMemoryConfiguration()); });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "AddHttpMemoryEndpoint failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("address", () => host.AddHttpMemoryEndpoint(typeof(CustomerService), address, new HttpMemoryConfiguration()));
                });

            host.AddHttpMemoryEndpoint(typeof(CustomerService), "http://somehost/", null);

            HttpMemoryEndpoint HttpMemoryEndpoint1 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "", new HttpMemoryConfiguration());
            Assert.AreEqual("http://somehost/", HttpMemoryEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint2 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "/path", new HttpMemoryConfiguration());
            Assert.AreEqual("http://somehost/path", HttpMemoryEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint3 = host.AddHttpMemoryEndpoint(typeof(CustomerService), "http://otherhost/", new HttpMemoryConfiguration());
            Assert.AreEqual("http://otherhost/", HttpMemoryEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpMemoryEndpoint(Type, Uri) works as expected")]
        public void AddHttpMemoryEndpointUri()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new Uri("http://somehost/"));

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpMemoryEndpoint(null, new Uri("http://somehost/")); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpMemoryEndpoint(typeof(CustomerService), (Uri)null); });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "AddHttpMemoryEndpoint failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("address", () => host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri(address)));
                });

            HttpMemoryEndpoint HttpMemoryEndpoint1 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("", UriKind.Relative));
            Assert.AreEqual("http://somehost/", HttpMemoryEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint2 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("/path", UriKind.Relative));
            Assert.AreEqual("http://somehost/path", HttpMemoryEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint3 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("http://otherhost/"));
            Assert.AreEqual("http://otherhost/", HttpMemoryEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("AddHttpMemoryEndpoint(Type, Uri, HttpMemoryConfiguration) works as expected")]
        public void AddHttpMemoryEndpointUriConfig()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(CustomerService), new Uri("http://somehost/"));

            Asserters.Exception.ThrowsArgumentNull("implementedContract", () => { host.AddHttpMemoryEndpoint(null, new Uri("http://somehost/"), new HttpMemoryConfiguration()); });
            Asserters.Exception.ThrowsArgumentNull("address", () => { host.AddHttpMemoryEndpoint(typeof(CustomerService), (Uri)null, new HttpMemoryConfiguration()); });

            Asserters.Data.Execute(DataSets.Http.AddressesWithIllegalSchemes, TestDataVariations.AsInstance,
                "AddHttpMemoryEndpoint failed to throw with illegal base address schemes",
                (type, obj) =>
                {
                    string address = (string)obj;
                    UnitTest.Asserters.Exception.ThrowsArgument("address", () => host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri(address), new HttpMemoryConfiguration()));
                });

            host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("http://somehost/"), null);

            HttpMemoryEndpoint HttpMemoryEndpoint1 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("", UriKind.Relative), new HttpMemoryConfiguration());
            Assert.AreEqual("http://somehost/", HttpMemoryEndpoint1.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint2 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("/path", UriKind.Relative), new HttpMemoryConfiguration());
            Assert.AreEqual("http://somehost/path", HttpMemoryEndpoint2.Address.Uri.AbsoluteUri, "Unexpected endpoint address");

            HttpMemoryEndpoint HttpMemoryEndpoint3 = host.AddHttpMemoryEndpoint(typeof(CustomerService), new Uri("http://otherhost/"), new HttpMemoryConfiguration());
            Assert.AreEqual("http://otherhost/", HttpMemoryEndpoint3.Address.Uri.AbsoluteUri, "Unexpected endpoint address");
        }

        [TestMethod, TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpServiceHost OnOpening adds behaviors to endpoints that don't have them")]
        public void ServiceHost_Explicit_Add_Endpoint_Without_Behavior()
        {
            ContractDescription cd = ContractDescription.GetContract(typeof(LocalCustomerService));
            HttpServiceHost host = new HttpServiceHost(typeof(LocalCustomerService), new Uri("http://localhost:8080"));
            HttpEndpoint endpoint = new HttpEndpoint(cd, new EndpointAddress("http://somehost"));
            endpoint.Behaviors.Clear();
            Assert.AreEqual(0, endpoint.Behaviors.Count, "Expected no behaviors by default");
            host.Description.Endpoints.Add(endpoint);
            try
            {
                host.Open();
                Assert.AreEqual(1, endpoint.Behaviors.OfType<HttpBehavior>().Count(), "Expected open to add behavior");
            }
            finally
            {
                host.Close();
            }
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("HttpServiceHost works for one-way operation")]
        public void ServiceHost_Works_With_OneWay_Operation()
        {
            ContractDescription cd = ContractDescription.GetContract(typeof(OneWayService));
            HttpServiceHost host = new HttpServiceHost(typeof(OneWayService), new Uri("http://localhost/onewayservice"));
            host.Open();

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage actualResponse = client.GetAsync("http://localhost/onewayservice/name").Result)
                {
                    Assert.AreEqual(actualResponse.StatusCode, HttpStatusCode.Accepted, "Response status code should be Accepted(202) for one-way operation");
                }
            }

            host.Close();
        }

        #region CreateDescription

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("CreateDescription(out IDictionary<string, ContractDescription>) creates ContractDescription for type without [ServieContract.")]
        public void CreateDescriptionWorksWithoutServiceContract()
        {
            HttpServiceHost host = new HttpServiceHost(typeof(PocoServiceContract), new HttpConfiguration(), new Uri("http://somehost"));
            host.AddDefaultEndpoints();

            ServiceDescription sd = host.Description;
            Assert.IsNotNull(sd, "ServiceDescription was null");
            Assert.AreEqual(typeof(PocoServiceContract), sd.ServiceType, "ServiceType was not PocoServiceContract.");
            Assert.AreEqual(typeof(PocoServiceContract).Name, sd.Name, "Name was not PocoServiceContract.");

            ServiceEndpointCollection endPoints = sd.Endpoints;
            Assert.AreEqual(1, endPoints.Count, "Expected one endpoint.");
            ContractDescription cd = endPoints[0].Contract;
            Assert.IsNotNull(cd, "ContractDescription was null.");
            Assert.AreEqual(typeof(PocoServiceContract), cd.ContractType, "ContractType was incorrect.");

            OperationDescriptionCollection operations = cd.Operations;
            MethodInfo[] methods = typeof(PocoServiceContract).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Assert.AreEqual(methods.Length, operations.Count, "Incorrect number of operations for the methods.");
            foreach (MethodInfo method in methods)
            {
                Assert.AreEqual(1, operations.Count((oc) => oc.Name.Equals(method.Name)), string.Format("Did not find operation for method '{0}'.", method.Name));
            }
        }

        #endregion CreateDescription

        private void VerifyAspNetCompatibilityRequirements(HttpServiceHost httpHost, AspNetCompatibilityRequirementsMode requirementMode)
        {
            var aspNetCompatibilityBehavior = httpHost.Description.Behaviors.Find<AspNetCompatibilityRequirementsAttribute>();
            Assert.IsNotNull(aspNetCompatibilityBehavior, "HttpServiceHost description should contain AspNetCompatibilityRequirementsAttribute.");
            Assert.IsTrue(aspNetCompatibilityBehavior.RequirementsMode == requirementMode, "AspNetCompatibilityRequirementsMode should be '{0}'.", requirementMode.ToString());
        }
    }
}