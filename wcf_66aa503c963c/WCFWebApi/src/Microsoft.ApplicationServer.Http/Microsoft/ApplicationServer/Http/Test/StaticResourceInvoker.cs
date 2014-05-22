// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Resources;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.Server.Common;

    internal class StaticResourceInvoker : IOperationInvoker
    {
        private const string HomePageUri = "home";

        private static Regex regexEmbeddedFile = new Regex(@"\s+(href|src)\=\x22[^\x22]+\x22", RegexOptions.Compiled);

        private static ResourceManager resourceManager;
        private bool isHomePageInvoker;

        public StaticResourceInvoker(bool homePageInvoker)
        {
            this.isHomePageInvoker = homePageInvoker;
        }

        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        private static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    resourceManager = new ResourceManager("Microsoft.ApplicationServer.Http.SR", typeof(Microsoft.ApplicationServer.Http.SR).Assembly);
                }

                return resourceManager;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            HttpResponseMessage response = null;
            if (this.isHomePageInvoker)
            {
                response = new HttpResponseMessage(HttpStatusCode.Redirect);
                response.Headers.Location = new Uri(HttpTestUtils.BuildFullUri(request.RequestUri.GetHostNormalizedUri(request), StaticResourceInvoker.HomePageUri));
            }
            else
            {
                string name = inputs[1] as string;

                if (string.Equals(name, HomePageUri, StringComparison.OrdinalIgnoreCase))
                {
                    return StaticResourceInvoker.CreateHtmlResponse(request, name + "_htm");
                }

                if (!string.IsNullOrEmpty(name) && (name.EndsWith("_htm", StringComparison.OrdinalIgnoreCase) || name.EndsWith("_html", StringComparison.OrdinalIgnoreCase)))
                {
                    response = StaticResourceInvoker.CreateHtmlResponse(request, name);
                }
                else if (!string.IsNullOrEmpty(name) && name.EndsWith("_css", StringComparison.OrdinalIgnoreCase))
                {
                    response = StaticResourceInvoker.CreateCSSResponse(request, name);
                }
                else if (!string.IsNullOrEmpty(name) && name.EndsWith("_js", StringComparison.OrdinalIgnoreCase))
                {
                    response = StaticResourceInvoker.CreateJavaScriptResponse(request, name);
                }
                else if (!string.IsNullOrEmpty(name) && name.EndsWith("_png", StringComparison.OrdinalIgnoreCase))
                {
                    response = StaticResourceInvoker.CreatePngImageResponse(request, name);
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }

            response.RequestMessage = request;
            return response;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw Fx.Exception.AsError(new NotSupportedException());
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw Fx.Exception.AsError(new NotSupportedException());
        }

        public object[] AllocateInputs()
        {
            return this.isHomePageInvoker ? new object[1] : new object[2];
        }

        private static HttpResponseMessage CreateHtmlResponse(HttpRequestMessage request, string name)
        {
            return CreateStringResponse(request, name, MediaTypeConstants.HtmlMediaType, true);
        }

        private static HttpResponseMessage CreateJavaScriptResponse(HttpRequestMessage request, string name)
        {
            return CreateStringResponse(request, name, new MediaTypeHeaderValue("text/javascript"));
        }

        private static HttpResponseMessage CreateCSSResponse(HttpRequestMessage request, string name)
        {
            return CreateStringResponse(request, name, new MediaTypeHeaderValue("text/css"));
        }

        private static HttpResponseMessage CreatePngImageResponse(HttpRequestMessage request, string name)
        {
            return CreateImageResponse(request, name, new MediaTypeHeaderValue("image/png"));
        }

        private static HttpResponseMessage CreateImageResponse(HttpRequestMessage request, string name, MediaTypeHeaderValue mediaType)
        {
            byte[] image = ResourceManager.GetObject(name, CultureInfo.InvariantCulture) as byte[];
            return HttpTestUtils.CreateImageResponse(request, image, mediaType);
        }

        private static HttpResponseMessage CreateStringResponse(HttpRequestMessage request, string name, MediaTypeHeaderValue mediaType, bool needFilterDot = false)
        {
            byte[] buf = ResourceManager.GetObject(name, CultureInfo.InvariantCulture) as byte[];
            if (buf == null)
            {
                // stub_js
                return HttpTestUtils.CreateStringResponse(request, string.Empty, mediaType);
            }

            string resource = Encoding.UTF8.GetString(buf);

            if (needFilterDot)
            {
                resource = regexEmbeddedFile.Replace(
                    resource,
                    delegate(Match match)
                    {
                        return match.Value.Replace('.', '_').Replace('-', '_');
                    });
            }

            return HttpTestUtils.CreateStringResponse(request, resource, mediaType);
        }
    }
}
