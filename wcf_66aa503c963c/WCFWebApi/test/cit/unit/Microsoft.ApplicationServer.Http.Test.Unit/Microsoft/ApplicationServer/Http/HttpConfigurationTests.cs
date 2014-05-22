// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.ApplicationServer.Http.Channels.Mocks;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.ServiceModel.Channels;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpConfigurationTests : UnitTest<HttpConfiguration>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpConfiguration is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass, 
                typeof(HttpConfiguration));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpConfiguration is locked after passing it to HttpServiceHost.")]
        public void MembersLockAfterPassingToHttpServiceHost()
        {
            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpConfiguration configuration = new HttpConfiguration();
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.EnableHelpPage = true;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name, 
                            typeof(HttpServiceHost).Name), 
                        exception.Message,
                        "The EnableHelpPage property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.EnableTestClient = true;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                        "The EnableTestCient property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.IncludeExceptionDetail = true;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name), 
                        exception.Message,
                        "The IncludeExceptionDetail property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.MaxBufferSize = 200;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name, 
                            typeof(HttpServiceHost).Name), 
                        exception.Message,
                        "The MaxBufferSize property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.MaxReceivedMessageSize = 100;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                        "The MaxReceivedMessageSize property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    configuration.TrailingSlashMode = Http.TrailingSlashMode.Ignore;
                },
                exception =>
                {
                    Assert.AreEqual<string>(
                        SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                        "The TrailingSlashMode property should be locked after the configuration is passed to the HttpServiceHost.");
                });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.TransferMode = System.ServiceModel.TransferMode.StreamedRequest;
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The TransferMode property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.ErrorHandlers = ((a, b, c) => { });
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The ErrorHandlers property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.MessageHandlerFactory = () => { return new List<DelegatingHandler>(); };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The MessageHandlerDelegate property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.RequestHandlers = (a, b, c) => { };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The RequestHandlers property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.ResponseHandlers = (a, b, c) => { };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The ResponseHandlers property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.Security = (a, b) => { };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The Security property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.CreateInstance = (a, b, c) => { return null; };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The CreateServiceInstance property should be locked after the configuration is passed to the HttpServiceHost.");
               });

            Asserters.Exception.Throws<InvalidOperationException>(
               () =>
               {
                   configuration.ReleaseInstance = (a, b) => { };
               },
               exception =>
               {
                   Assert.AreEqual<string>(
                       SR.HttpConfigurationIsReadOnly(
                            typeof(HttpConfiguration).Name,
                            typeof(HttpServiceHost).Name),
                        exception.Message,
                       "The ReleaseServiceInstance property should be locked after the configuration is passed to the HttpServiceHost.");
               });
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("HttpConfiguration() works as expected.")]
        public void HttpConfigurationConstructor()
        {
            HttpConfiguration configuration = new HttpConfiguration();
            Assert.IsNotNull(configuration, "configuration should not be null.");
            Assert.IsFalse(configuration.EnableHelpPage, "configuration.EnableHelpPage should be false by default.");
            Assert.IsFalse(configuration.EnableTestClient, "configuration.EnableTestClient should be false by default.");
            Assert.IsNotNull(configuration.Formatters, "configuration.Formatters should not be null by default.");
            Assert.IsFalse(configuration.IncludeExceptionDetail, "configuration.IncludeExceptionDetail should be false by default.");
            Assert.AreEqual(TransportDefaults.MaxBufferSize, configuration.MaxBufferSize, "configuration.MaxBufferSize should be equal to TransportDefaults.MaxBufferSize by default.");
            Assert.AreEqual(TransportDefaults.MaxReceivedMessageSize, configuration.MaxReceivedMessageSize, "configuration.MaxReceivedMessageSize should be equal to TransportDefaults.MaxReceivedMessageSize by default.");
            Assert.AreEqual(Http.TrailingSlashMode.AutoRedirect, configuration.TrailingSlashMode, "configuration.TrailingSlashMode should be equal to AutoRedirect by default");
            Assert.AreEqual(System.ServiceModel.TransferMode.Buffered, configuration.TransferMode, "configuration.TransferMode should be equal to Buffered by default");
        }

        #endregion Constructors

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TrailingSlashMode is mutable.")]
        public void TrailingSlashModeIsMutable()
        {
            HttpConfiguration config = new HttpConfiguration();
            foreach (TrailingSlashMode enumValue in Enum.GetValues(typeof(TrailingSlashMode)).Cast<TrailingSlashMode>())
            {
                config.TrailingSlashMode = enumValue;
                Assert.AreEqual(enumValue, config.TrailingSlashMode, string.Format("The enum value '{0}' was not settable.", enumValue));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TrailingSlashMode throws with illegal enum value.")]
        public void TrailingSlashModeThrowsWithIllegalValue()
        {
            HttpConfiguration config = new HttpConfiguration();
            Asserters.Exception.Throws<System.ComponentModel.InvalidEnumArgumentException>(
                () => { config.TrailingSlashMode = (TrailingSlashMode)999; },
                (ex) =>
                {
                    Assert.AreEqual("value", ex.ParamName, "Invalid ParamName.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("EnableHelpPage works as expected.")]
        public void EnableHelpPage()
        {
            HttpConfiguration configuration = new HttpConfiguration { EnableHelpPage = true };
            Assert.IsTrue(configuration.EnableHelpPage, "EnableHelpPage should be set to true.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpBehavior httpBehavior = endpoint.Behaviors.Find<HttpBehavior>();
                Assert.IsNotNull(httpBehavior, "httpBehavior should not be null.");
                Assert.IsTrue(httpBehavior.HelpEnabled, "EnableHelpPage should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("EnableTestClient works as expected.")]
        public void EnableTestClient()
        {
            HttpConfiguration configuration = new HttpConfiguration { EnableTestClient = true };
            Assert.IsTrue(configuration.EnableTestClient, "EnableTestClient should be set to true.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpBehavior httpBehavior = endpoint.Behaviors.Find<HttpBehavior>();
                Assert.IsNotNull(httpBehavior, "httpBehavior should not be null.");
                Assert.IsTrue(httpBehavior.TestClientEnabled, "EnableTestClient should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Formatters works as expected.")]
        public void Formatters()
        {
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Formatters.Clear();
            Assert.IsTrue(configuration.Formatters.Count == 0, "configuration.Formatters should be cleared.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpEndpoint httpEndpoint = endpoint as HttpEndpoint;
                Assert.IsTrue(httpEndpoint.OperationHandlerFactory.Formatters.Count == 0, "configuration.Formatters should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("IncludeExceptionDetail works as expected.")]
        public void IncludeExceptionDetail()
        {
            HttpConfiguration configuration = new HttpConfiguration { IncludeExceptionDetail = true };
            Assert.IsTrue(configuration.IncludeExceptionDetail, "configuration.IncludeExceptionDetail should be set to true.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            ServiceDebugBehavior debugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            Assert.IsNotNull(debugBehavior, "debugBehavior should not be null.");
            Assert.IsTrue(debugBehavior.IncludeExceptionDetailInFaults, "configuration.IncludeExceptionDetail should be propagated to the endpoint.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MaxBufferSize works as expected.")]
        public void MaxBufferSize()
        {
            HttpConfiguration configuration = new HttpConfiguration { MaxBufferSize = 600 };
            Assert.AreEqual(600, configuration.MaxBufferSize, "configuration.MaxBufferSize should be set to 600.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpEndpoint httpEndpoint = endpoint as HttpEndpoint;
                HttpBinding httpBinding = endpoint.Binding as HttpBinding;
                Assert.IsNotNull(httpBinding, "httpBinding should not be null.");
                Assert.AreEqual(600, httpBinding.MaxBufferSize, "configuration.MaxBufferSize should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MaxReceivedMessageSize works as expected.")]
        public void MaxReceivedMessageSize()
        {
            HttpConfiguration configuration = new HttpConfiguration { MaxReceivedMessageSize = 600 };
            Assert.AreEqual(600, configuration.MaxReceivedMessageSize, "configuration.MaxReceivedMessageSize should be set to 600.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpEndpoint httpEndpoint = endpoint as HttpEndpoint;
                HttpBinding httpBinding = endpoint.Binding as HttpBinding;
                Assert.IsNotNull(httpBinding, "httpBinding should not be null.");
                Assert.AreEqual(600, httpBinding.MaxReceivedMessageSize, "configuration.MaxReceivedMessageSize should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TrailingSlashMode works as expected.")]
        public void TrailingSlashMode()
        {
            HttpConfiguration configuration = new HttpConfiguration { TrailingSlashMode = Http.TrailingSlashMode.Ignore };
            Assert.AreEqual(Http.TrailingSlashMode.Ignore, configuration.TrailingSlashMode, "configuration.TrailingSlashMode should be set to Ignore.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpBehavior httpBehavior = endpoint.Behaviors.Find<HttpBehavior>();
                Assert.IsNotNull(httpBehavior, "httpBehavior should not be null.");
                Assert.AreEqual(Http.TrailingSlashMode.Ignore, httpBehavior.TrailingSlashMode, "configuration.TrailingSlashMode should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TransferMode works as expected.")]
        public void TransferMode()
        {
            HttpConfiguration configuration = new HttpConfiguration { TransferMode = System.ServiceModel.TransferMode.StreamedRequest };
            Assert.AreEqual(System.ServiceModel.TransferMode.StreamedRequest, configuration.TransferMode, "configuration.TransferMode should be set to StreamedRequest.");

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
            foreach (var endpoint in host.AddDefaultEndpoints())
            {
                HttpEndpoint httpEndpoint = endpoint as HttpEndpoint;
                HttpBinding httpBinding = endpoint.Binding as HttpBinding;
                Assert.IsNotNull(httpBinding, "httpBinding should not be null.");
                Assert.AreEqual(System.ServiceModel.TransferMode.StreamedRequest, httpEndpoint.TransferMode, "configuration.TransferMode should be propagated to the endpoint.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("CreateInstance works as expected.")]
        public void CreateInstance()
        {
            ContactsService serviceInstance = new ContactsService();
            HttpConfiguration configuration = new HttpConfiguration 
            { 
                CreateInstance = (type, context, request) =>
                    {
                        return serviceInstance;
                    } 
            };

            Assert.IsNotNull(configuration.CreateInstance, "configuration.CreateInstance should not be null.");
            object createdInstance = configuration.CreateInstance(typeof(string), new System.ServiceModel.InstanceContext(new object()), new HttpRequestMessage());
            Assert.AreEqual(serviceInstance, createdInstance, "The object returned by configuration.CreateInstance should be the same as 'serviceInstance'.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ReleaseInstance cannot be set without setting CreateServiceInstance.")]
        public void ReleaseInstance_Throws_If_CreateServiceInstance_Null()
        {
            HttpConfiguration configuration = new HttpConfiguration
            {
                ReleaseInstance = (context, obj) => { }
            };

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
                    host.AddDefaultEndpoints();
                },
                exception => 
                {
                    Assert.AreEqual(SR.CannotSetOnlyOneProperty("ReleaseInstance", "CreateInstance", "HttpConfiguration"), exception.Message, "The exception message did not match.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ErrorHandlers works as expected.")]
        public void ErrorHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var errorHandlerInstance = new DummyErrorHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                ErrorHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Add(errorHandlerInstance);
                }
            };

            Collection<HttpErrorHandler> errorHandlers = new Collection<HttpErrorHandler>();
            Assert.IsNotNull(configuration.ErrorHandlers, "configuration.ErrorHandlers should not be null.");
            configuration.ErrorHandlers(errorHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new List<HttpOperationDescription>());
            Assert.AreEqual(errorHandlerInstance, errorHandlers[0], "The 'errorHandlers' should contain 'errorHandlerInstance' after calling configuration.ErrorHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ErrorHandlers should support insertion.")]
        public void InsertErrorHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var errorHandlerInstance = new DummyErrorHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                ErrorHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Insert(0, errorHandlerInstance);
                }
            };

            Collection<HttpErrorHandler> errorHandlers = new Collection<HttpErrorHandler>();
            Assert.IsNotNull(configuration.ErrorHandlers, "configuration.ErrorHandlers should not be null.");
            configuration.ErrorHandlers(errorHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new List<HttpOperationDescription>());
            Assert.AreEqual(errorHandlerInstance, errorHandlers[0], "The 'errorHandlers' should contain 'errorHandlerInstance' after calling configuration.ErrorHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MessageHandlerFactory works as expected.")]
        public void MessageHandlerFactory()
        {
            ContactsService serviceInstance = new ContactsService();
            var messageHandlerInstance = new MockValidMessageHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                MessageHandlerFactory = () =>
                {
                    return new List<DelegatingHandler> { messageHandlerInstance };
                }
            };

            Assert.IsNotNull(configuration.MessageHandlerFactory, "configuration.MessageHandlerFactory should not be null.");
            var result = configuration.MessageHandlerFactory();
            Assert.AreEqual(messageHandlerInstance, result.First(), "The object returned by configuration.MessageHandlerFactory should be the same as 'messageHandlerInstance'.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MessageHandlers works as expected.")]
        public void MessageHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            HttpConfiguration configuration = new HttpConfiguration();

            configuration.MessageHandlers.Add(typeof(MockValidMessageHandler));

            Assert.IsNotNull(configuration.MessageHandlers, "configuration.MessageHandlers should not be null.");
            Assert.AreEqual(1, configuration.MessageHandlers.Count, "configuration.MessageHandlers should have one handler.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MessageHandlers cannot be set when MessageHandlerFactory is set.")]
        public void MessageHandler_Throws_If_MessageHandlerFactory_Is_Set()
        {
            ContactsService serviceInstance = new ContactsService();
            HttpConfiguration configuration = new HttpConfiguration
            {
                MessageHandlerFactory = () =>
                {
                    return new List<DelegatingHandler>();
                }
            };
            configuration.MessageHandlers.Add(typeof(MockValidMessageHandler));

            Uri baseAddress = new Uri("http://localhost:8080/contactsservice");
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    HttpServiceHost host = new HttpServiceHost(typeof(ContactsService), configuration, baseAddress);
                    host.AddDefaultEndpoints();
                },
                exception =>
                {
                    Assert.AreEqual(SR.CannotSetBothProperties("MessageHandlerFactory", "MessageHandlers", "HttpConfiguration"), exception.Message, "The exception message did not match.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("RequestHandlers works as expected.")]
        public void RequestHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var requestHandler = new DummyOperationHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                RequestHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Add(requestHandler);
                }
            };

            Collection<HttpOperationHandler> requestHandlers = new Collection<HttpOperationHandler>();
            Assert.IsNotNull(configuration.RequestHandlers, "configuration.RequestHandlers should not be null.");
            configuration.RequestHandlers(requestHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new HttpOperationDescription());
            Assert.AreEqual(requestHandler, requestHandlers[0], "The 'requestHandlers' should contain 'requestHandler' after calling configuration.RequestHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("RequestHandlers should support insertion.")]
        public void InsertRequestHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var requestHandler = new DummyOperationHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                RequestHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Insert(0, requestHandler);
                }
            };

            Collection<HttpOperationHandler> requestHandlers = new Collection<HttpOperationHandler>();
            Assert.IsNotNull(configuration.RequestHandlers, "configuration.RequestHandlers should not be null.");
            configuration.RequestHandlers(requestHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new HttpOperationDescription());
            Assert.AreEqual(requestHandler, requestHandlers[0], "The 'requestHandlers' should contain 'requestHandler' after calling configuration.RequestHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ResponseHandlers works as expected.")]
        public void ResponseHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var responseHandler = new DummyOperationHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                ResponseHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Add(responseHandler);
                }
            };
            Collection<HttpOperationHandler> responseHandlers = new Collection<HttpOperationHandler>();
            Assert.IsNotNull(configuration.ResponseHandlers, "configuration.ResponseHandlers should not be null.");
            configuration.ResponseHandlers(responseHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new HttpOperationDescription());
            Assert.AreEqual(responseHandler, responseHandlers[0], "The 'responseHandlers' should contain 'responseHandler' after calling configuration.ResponseHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("ResponseHandlers should support insertion.")]
        public void InsertResponseHandlers()
        {
            ContactsService serviceInstance = new ContactsService();
            var responseHandler = new DummyOperationHandler();
            HttpConfiguration configuration = new HttpConfiguration
            {
                ResponseHandlers = (handlers, endpoint, operations) =>
                {
                    handlers.Insert(0, responseHandler);
                }
            };
            Collection<HttpOperationHandler> responseHandlers = new Collection<HttpOperationHandler>();
            Assert.IsNotNull(configuration.ResponseHandlers, "configuration.ResponseHandlers should not be null.");
            configuration.ResponseHandlers(responseHandlers, new ServiceEndpoint(new ContractDescription("myContract")), new HttpOperationDescription());
            Assert.AreEqual(responseHandler, responseHandlers[0], "The 'responseHandlers' should contain 'responseHandler' after calling configuration.ResponseHandlers.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Security works as expected.")]
        public void Security()
        {
            ContactsService serviceInstance = new ContactsService();
            HttpConfiguration configuration = new HttpConfiguration
            {
                Security = (uri, bindingSecurity) =>
                {
                    bindingSecurity.Mode = HttpBindingSecurityMode.Transport;
                }
            };

            HttpBindingSecurity security = new HttpBindingSecurity { Mode = HttpBindingSecurityMode.None };
            Assert.IsNotNull(configuration.Security, "configuration.Security should not be null.");
            configuration.Security(new Uri("http://tempuri.org"), security);
            Assert.AreEqual(HttpBindingSecurityMode.Transport, security.Mode, "The security mode should be set to transport.");
        }

        #endregion Properties

        #region Custom Handlers

        public class DummyErrorHandler : HttpErrorHandler
        {
            protected override bool OnTryProvideResponse(Exception exception, ref HttpResponseMessage message)
            {
                return true;
            }
        }

        public class DummyOperationHandler : HttpOperationHandler
        {
            protected override IEnumerable<HttpParameter> OnGetInputParameters()
            {
                return null;
            }

            protected override IEnumerable<HttpParameter> OnGetOutputParameters()
            {
                return null;
            }

            protected override object[] OnHandle(object[] input)
            {
                return null;
            }
        }

        #endregion
    }
}
