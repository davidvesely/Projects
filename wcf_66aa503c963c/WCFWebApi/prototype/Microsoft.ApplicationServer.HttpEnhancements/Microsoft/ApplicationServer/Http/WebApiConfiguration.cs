// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    public class WebApiConfiguration : HttpConfiguration
    {
        private readonly bool useMethodPrefixForHttpMethod;

        private readonly string[] methods = new[] { "GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE" };

        private WebApiHttpOperationHandlerFactory operationHandlerFactory;

        public WebApiConfiguration(bool useMethodPrefixForHttpMethod = true)
        {
            this.useMethodPrefixForHttpMethod = useMethodPrefixForHttpMethod;
            this.Formatters.Remove(this.Formatters.JsonFormatter);
            this.Formatters.Remove(this.Formatters.JsonValueFormatter);
            this.Formatters.Add(new JsonpMediaTypeFormatter());
            this.Formatters.Add(new JsonpValueMediaTypeFormatter());
        }

        protected override void OnConfigureServiceHost(HttpServiceHost serviceHost)
        {
            base.OnConfigureServiceHost(serviceHost);
            this.ConfigureTasks(serviceHost.Description);
        }

        private void ConfigureTasks(ServiceDescription serviceDescription)
        {
            serviceDescription.Behaviors.Add(new TaskServiceAttribute());
        }

        protected override void OnConfigureEndpoint(Description.HttpEndpoint endpoint)
        {
            if (this.useMethodPrefixForHttpMethod)
            {
                this.ConfigureHttpMethod(endpoint.Contract);
            }

            base.OnConfigureEndpoint(endpoint);
            this.ConfigureTasks(endpoint.Contract);
            this.ConfigureJsonpHttpResponseHandler(endpoint);
        }

        private void ConfigureJsonpHttpResponseHandler(Description.HttpEndpoint endpoint)
        {
            endpoint.OperationHandlerFactory = this.OperationHandlerFactory;
        }

        private WebApiHttpOperationHandlerFactory OperationHandlerFactory
        {
            get
            {
                if (this.operationHandlerFactory == null)
                {
                    this.operationHandlerFactory = new WebApiHttpOperationHandlerFactory(this.Formatters)
                        {
                            RequestHandlerDelegate = this.RequestHandlers,
                            ResponseHandlerDelegate = this.ResponseHandlers
                        };
                }

                return this.operationHandlerFactory;
            }
        }

        private void ConfigureTasks(ContractDescription contract)
        {
            foreach (var operation in contract.Operations)
            {
                if (operation.SyncMethod != null)
                {
                    if (ServiceReflector.taskType.IsAssignableFrom(operation.SyncMethod.ReturnType))
                    {
                        if (!operation.Behaviors.Contains(typeof(TaskOperationAttribute)))
                        {
                            operation.Behaviors.Add(new TaskOperationAttribute());
                        }
                    }
                }
            }
        }

        // set uri template to empty if it is null and set method to the method prefix if conventional match is enabled
        private void ConfigureHttpMethod(ContractDescription contract)
        {
            foreach (var operation in contract.Operations)
            {
                var webGet = operation.Behaviors.Find<WebGetAttribute>();
                if (webGet != null)
                {
                    if (webGet.UriTemplate == null)
                    {
                        webGet.UriTemplate = string.Empty;
                    }
                }
                else
                {
                    var webInvoke = operation.Behaviors.Find<WebInvokeAttribute>();
                    if (webInvoke != null)
                    {
                        if (webInvoke.UriTemplate == null)
                        {
                            webInvoke.UriTemplate = string.Empty;
                        }

                        if (webInvoke.Method == null)
                        {
                            var compareName = operation.Name.ToUpper();
                            var method = this.methods.SingleOrDefault(compareName.StartsWith);
                            webInvoke.Method = method;
                        }
                    }
                }

                var operationBehavior = operation.Behaviors.Find<OperationBehaviorAttribute>();
            }
        }
    }
}
