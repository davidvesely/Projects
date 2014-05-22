// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;

    internal static class HttpTestUtils
    {
        public const string HomePageMethodName = "GetTestHomePage";
        public const string HomePageUriTemplate = "test";
        
        public const string StaticResourceMethodName = "GetTestStaticResource";
        public const string StaticResourceUriTemplate = "test/{name}";

        public const string ServiceResourceMethodName = "GetServiceResources";
        public const string ServiceResourceUriTemplate = "test/resources";

        public const string RequestUriInvokerMethodName = "ProcessRequestUri";
        public const string RequestUriInvokerUriTemplate = "test/request/uri?uri={uri}&method={method}&op={op}&cursorpos={cursorpos}";

        public const string RequestHeadersInvokerMethodName = "ProcessRequestHeaders";
        public const string RequestHeadersInvokerUriTemplate = "test/request/headers?cursorpos={cursorpos}";

        public const string RequestContentInvokerMethodName = "ProcessRequestContent";
        public const string RequestContentInvokerUriTemplate = "test/request/content?uri={uri}&method={method}&op={op}&cursorpos={cursorpos}&format0={format0}&format1={format1}";

        public const string ResponseContentInvokerMethodName = "ProcessResponseContent";
        public const string ResponseContentInvokerUriTemplate = "test/response/content?uri={uri}&method={method}&format={format}";

        public static OperationDescription[] AddHttpTestOperations(HttpEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            Fx.Assert(endpoint != null, "The 'endpoint' parameter should not be null.");
            Fx.Assert(dispatchRuntime != null, "The 'dispatchRuntime' parameter should not be null.");

            HttpOperationDescription homePageOperation = new HttpOperationDescription(HttpTestUtils.HomePageMethodName, endpoint.Contract);
            homePageOperation.Behaviors.Add(new WebGetAttribute() { UriTemplate = HttpTestUtils.HomePageUriTemplate });
            homePageOperation.InputParameters.Add(HttpParameter.RequestMessage);
            homePageOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription staticResourceOperation = new HttpOperationDescription(HttpTestUtils.StaticResourceMethodName, endpoint.Contract);
            staticResourceOperation.Behaviors.Add(new WebGetAttribute() { UriTemplate = HttpTestUtils.StaticResourceUriTemplate });
            staticResourceOperation.InputParameters.Add(HttpParameter.RequestMessage);
            staticResourceOperation.InputParameters.Add(new HttpParameter("name", TypeHelper.StringType));
            staticResourceOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription resourcesOperation = new HttpOperationDescription(HttpTestUtils.ServiceResourceMethodName, endpoint.Contract);
            resourcesOperation.Behaviors.Add(new WebGetAttribute() { UriTemplate = HttpTestUtils.ServiceResourceUriTemplate });
            resourcesOperation.InputParameters.Add(HttpParameter.RequestMessage);
            resourcesOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription requestUriOperation = new HttpOperationDescription(HttpTestUtils.RequestUriInvokerMethodName, endpoint.Contract);
            requestUriOperation.Behaviors.Add(new WebInvokeAttribute() { UriTemplate = HttpTestUtils.RequestUriInvokerUriTemplate, Method = "POST" });
            requestUriOperation.InputParameters.Add(HttpParameter.RequestMessage);
            requestUriOperation.InputParameters.Add(new HttpParameter("uri", TypeHelper.StringType));
            requestUriOperation.InputParameters.Add(new HttpParameter("method", TypeHelper.StringType));
            requestUriOperation.InputParameters.Add(new HttpParameter("op", TypeHelper.StringType));
            requestUriOperation.InputParameters.Add(new HttpParameter("cursorpos", TypeHelper.IntType));
            requestUriOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription requestHeadersOperation = new HttpOperationDescription(HttpTestUtils.RequestHeadersInvokerMethodName, endpoint.Contract);
            requestHeadersOperation.Behaviors.Add(new WebInvokeAttribute() { UriTemplate = HttpTestUtils.RequestHeadersInvokerUriTemplate, Method = "POST" });
            requestHeadersOperation.InputParameters.Add(HttpParameter.RequestMessage);
            requestHeadersOperation.InputParameters.Add(new HttpParameter("cursorpos", TypeHelper.IntType));
            requestHeadersOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription requestContentOperation = new HttpOperationDescription(HttpTestUtils.RequestContentInvokerMethodName, endpoint.Contract);
            requestContentOperation.Behaviors.Add(new WebInvokeAttribute() { UriTemplate = HttpTestUtils.RequestContentInvokerUriTemplate, Method = "POST" });
            requestContentOperation.InputParameters.Add(HttpParameter.RequestMessage);
            requestContentOperation.InputParameters.Add(new HttpParameter("uri", TypeHelper.StringType));
            requestContentOperation.InputParameters.Add(new HttpParameter("method", TypeHelper.StringType));
            requestContentOperation.InputParameters.Add(new HttpParameter("op", TypeHelper.StringType));
            requestContentOperation.InputParameters.Add(new HttpParameter("cursorpos", TypeHelper.IntType));
            requestContentOperation.InputParameters.Add(new HttpParameter("format0", TypeHelper.StringType));
            requestContentOperation.InputParameters.Add(new HttpParameter("format1", TypeHelper.StringType));
            requestContentOperation.ReturnValue = HttpParameter.ResponseMessage;

            HttpOperationDescription responseContentOperation = new HttpOperationDescription(HttpTestUtils.ResponseContentInvokerMethodName, endpoint.Contract);
            responseContentOperation.Behaviors.Add(new WebInvokeAttribute() { UriTemplate = HttpTestUtils.ResponseContentInvokerUriTemplate, Method = "POST" });
            responseContentOperation.InputParameters.Add(HttpParameter.RequestMessage);
            responseContentOperation.InputParameters.Add(new HttpParameter("format", TypeHelper.StringType));
            responseContentOperation.ReturnValue = HttpParameter.ResponseMessage;

            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.HomePageMethodName, null, null) { Invoker = new StaticResourceInvoker(true) });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.StaticResourceMethodName, null, null) { Invoker = new StaticResourceInvoker(false) });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.ServiceResourceMethodName, null, null) { Invoker = new ServiceResourceInvoker(endpoint) });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.RequestUriInvokerMethodName, null, null) { Invoker = new RequestUriInvoker(endpoint) });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.RequestHeadersInvokerMethodName, null, null) { Invoker = new RequestHeadersInvoker() });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.RequestContentInvokerMethodName, null, null) { Invoker = new RequestContentInvoker(endpoint) });
            dispatchRuntime.Operations.Add(
                new DispatchOperation(dispatchRuntime, HttpTestUtils.ResponseContentInvokerMethodName, null, null) { Invoker = new ResponseContentInvoker() });

            return new OperationDescription[] 
            { 
                homePageOperation.ToOperationDescription(), 
                resourcesOperation.ToOperationDescription(), 
                staticResourceOperation.ToOperationDescription(),
                requestUriOperation.ToOperationDescription(),
                requestHeadersOperation.ToOperationDescription(),
                requestContentOperation.ToOperationDescription(),
                responseContentOperation.ToOperationDescription()
            };
        }

        internal static HttpResponseMessage CreateIntellisenseResponse(ITolerantTextReader reader, HttpRequestMessage request)
        {
            while (reader.Read())
            {
            }

            if (reader.Exception != null)
            {
                throw reader.Exception;
            }

            TolerantTextReaderResult result = new TolerantTextReaderResult
            {
                AutoCompleteList = reader.GetExpectedItems(),
            };

            string body;
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TolerantTextReaderResult));
                HttpTestUtils.TryWriteObject(serializer, stream, result);
                body = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }

            HttpResponseMessage response = HttpTestUtils.CreateStringResponse(request, body, MediaTypeConstants.ApplicationJsonMediaType);
            return response;
        }

        internal static HttpResponseMessage CreateFormattingResponse(HttpRequestMessage request, string format, string originalStr, bool htmlEncode)
        {
            string formattedStr = null;
            if (string.Equals(format, "xml", StringComparison.OrdinalIgnoreCase))
            {
                formattedStr = HttpTestUtils.FormatXml(originalStr);
            }
            else if (string.Equals(format, "json", StringComparison.OrdinalIgnoreCase))
            {
                formattedStr = HttpTestUtils.FormatJson(originalStr);
            }

            if (formattedStr == null)
            {
                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
            }

            if (htmlEncode)
            {
                formattedStr = HttpUtility.HtmlEncode(formattedStr).Replace("\r\n", "\n").Replace("\n", "<br/>").Replace(" ", "&nbsp;");
            }

            return HttpTestUtils.CreateStringResponse(request, formattedStr, new MediaTypeHeaderValue("text/plain"));
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed by wrapper.")]
        internal static string FormatXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    doc.Load(reader);
                }

                StringBuilder sb = new StringBuilder();
                using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = ' ';
                    writer.Indentation = 2;
                    doc.WriteTo(writer);
                }

                return sb.ToString();
            }
            catch (XmlException e)
            {
                Debug.WriteLine(e.ToString());
                return xml;
            }
        }

        internal static string FormatJson(string json)
        {
            try
            {
                using (TolerantJsonTextReader reader = new TolerantJsonTextReader(json))
                {
                    while (reader.Read())
                    {
                    }

                    if (!reader.IsFormattable)
                    {
                        Debug.WriteLine(reader.Exception);
                        Debug.WriteLine(reader.Warning);
                        return json;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
                        {
                            using (FormattedJsonTextWriter writer = reader.CreateWriter(stringWriter))
                            {
                                writer.WriteDocument();
                            }
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (XmlException e)
            {
                Debug.WriteLine(e.ToString());
                return json;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
                return json;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        internal static HttpResponseMessage CreateStringResponse(HttpRequestMessage request, string str, MediaTypeHeaderValue mediaType)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.RequestMessage = request;
            response.Content = new ActionOfStreamContent(stream =>
            {
                Encoding encoding = new UTF8Encoding();
                byte[] preamble = encoding.GetPreamble();
                stream.Write(preamble, 0, preamble.Length);
                byte[] bytes = encoding.GetBytes(str == null ? string.Empty : str);
                stream.Write(bytes, 0, bytes.Length);
            });

            response.Content.Headers.ContentType = mediaType;
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.WebName;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        internal static HttpResponseMessage CreateBoolResponse(HttpRequestMessage request, bool value)
        {
            HttpResponseMessage response = new HttpResponseMessage<bool>(value, HttpStatusCode.OK);
            response.RequestMessage = request;
            response.Content.Headers.ContentType = MediaTypeConstants.ApplicationJsonMediaType;
            return response;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        internal static HttpResponseMessage CreateImageResponse(HttpRequestMessage request, byte[] image, MediaTypeHeaderValue mediaType)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.RequestMessage = request;
            response.Content = new ActionOfStreamContent(stream =>
            {
                stream.Write(image, 0, image.Length);
            });

            response.Content.Headers.ContentType = mediaType;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        internal static HttpResponseMessage CreateErrorResponseMessage(HttpRequestMessage request, Exception exception)
        {
            HttpResponseMessage response = HttpTestUtils.CreateStringResponse(request, exception.ToString(), MediaTypeConstants.HtmlMediaType);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

        internal static ServiceInfo GetServiceInfo(HttpEndpoint endpoint)
        {
            ServiceInfo service = new ServiceInfo();

            service.HelpEnabled = endpoint.HelpEnabled;

            List<ResourceInfo> resources = null;
            if (HttpTestUtils.AnalyzeHttpEndpoint(endpoint, out resources))
            {
                service.Resources.AddRange(resources);
            }

            return service;
        }

        internal static string BuildFullUri(Uri baseUri, string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return baseUri.AbsoluteUri;
            }
            else
            {
                return baseUri.AbsoluteUri.TrimEnd('/') + "/" + uri.TrimStart('/');
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed by wrapper.")]
        internal static bool TryDeserializeXml(string xml, Type type, bool useXmlSerializer, out object instance)
        {
            instance = null;
            using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
            {
                if (useXmlSerializer)
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    return HttpTestUtils.TryDeserialize(serializer, reader, out instance);
                }
                else
                {
                    DataContractSerializer serializer = new DataContractSerializer(type);
                    return HttpTestUtils.TryReadObject(serializer, reader, out instance);
                }
            }
        }

        internal static bool TryDeserializeJson(string json, Type type, out object instance)
        {
              DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(type);
              using (MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
              {
                  return HttpTestUtils.TryReadObject(jsonSerializer, jsonStream, out instance);
              }
        }

        internal static string Xml2Json(string xml, Type type, bool useXmlSerializer)
        {
            object instance;
            bool succeeded = HttpTestUtils.TryDeserializeXml(xml, type, useXmlSerializer, out instance);
            if (succeeded)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                    HttpTestUtils.TryWriteObject(serializer, stream, instance);
                    return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }

            return null;
        }

        internal static string Json2Xml(string json, Type type, bool useXmlSerializer)
        {
            object instance;
            bool succeeded  = HttpTestUtils.TryDeserializeJson(json, type, out instance);
            if (succeeded)
            {
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    if (useXmlSerializer)
                    {
                        XmlSerializer serializer = new XmlSerializer(type);
                        HttpTestUtils.TrySerialize(serializer, xmlStream, instance);
                    }
                    else
                    {
                        DataContractSerializer serializer = new DataContractSerializer(type);
                        HttpTestUtils.TryWriteObject(serializer, xmlStream, instance);
                    }

                    return Encoding.UTF8.GetString(xmlStream.GetBuffer(), 0, (int)xmlStream.Length);
                }
            }
            
            return null;
        }

        internal static string CleanString(string str, ref int cursorPos)
        {
            string strBeforeCursor = str.Substring(0, cursorPos);
            strBeforeCursor = strBeforeCursor.Replace("\r\n", "\n");
            cursorPos = strBeforeCursor.Length;
            return str.Replace("\r\n", "\n");
        }

        private static void TrySerialize(XmlSerializer serializer, MemoryStream stream, object instance)
        {
            try
            {
                serializer.Serialize(stream, instance);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private static bool TryDeserialize(XmlSerializer serializer, XmlReader reader, out object instance)
        {
            instance = null;
            try
            {
                instance = serializer.Deserialize(reader);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private static void TryWriteObject(XmlObjectSerializer serializer, Stream stream, object instance)
        {
            try
            {
                serializer.WriteObject(stream, instance);
            }
            catch (InvalidDataContractException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (SerializationException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (QuotaExceededException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private static bool TryReadObject(XmlObjectSerializer serializer, Stream stream, out object instance)
        {
            instance = null;
            try
            {
                instance = serializer.ReadObject(stream);
            }
            catch (SerializationException e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private static bool TryReadObject(XmlObjectSerializer serializer, XmlReader reader, out object instance)
        {
            instance = null;
            try
            {
                instance = serializer.ReadObject(reader);
            }
            catch (SerializationException e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private static bool ValidateHttpEndpoint(ServiceEndpoint endpoint)
        {
            if (!(endpoint is HttpEndpoint))
            {
                return false;
            }

            return true;
        }

        private static bool AnalyzeHttpEndpoint(ServiceEndpoint endpoint, out List<ResourceInfo> resources)
        {
            Fx.Assert(endpoint != null, "endpoint is null");

            resources = null;

            if (!HttpTestUtils.ValidateHttpEndpoint(endpoint))
            {
                return false;
            }

            resources = new List<ResourceInfo>();

            Uri baseUri = endpoint.Address.Uri;

            Dictionary<string, ResourceInfo> resourceDict = new Dictionary<string, ResourceInfo>();
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                HttpOperationDescription description = operation.ToHttpOperationDescription();
                UriTemplate uriTemplate = description.GetUriTemplate();
                string uri = HttpTestUtils.BuildFullUri(baseUri, uriTemplate.ToString());

                ResourceInfo resource = null;
                if (!resourceDict.TryGetValue(uri, out resource))
                {
                    resource = new ResourceInfo() { Uri = uri, UriTemplate = description.GetUriTemplate(), Endpoint = (HttpEndpoint)endpoint };
                    resourceDict.Add(uri, resource);
                }

                string httpMethod = description.GetHttpMethod().Method.ToUpper();
                resource.OperationDictionary.Add(httpMethod, new OperationInfo() { Description = description, HttpMethod = httpMethod, ResourceInfo = resource });
            }

            resources.AddRange(resourceDict.Values);
            return true;
        }

        internal class ServiceInfo
        {
            public ServiceInfo()
            {
                this.Resources = new List<ResourceInfo>();
                this.HelpEnabled = false;
            }

            public bool HelpEnabled { get; set; }

            public List<ResourceInfo> Resources { get; private set; }

            public ResourceInfo GetResourceInfo(string uriTemplate)
            {
                UriTemplate targetUri = new UriTemplate(uriTemplate, true);
                foreach (HttpTestUtils.ResourceInfo resource in this.Resources)
                {
                    if (targetUri.IsEquivalentTo(new UriTemplate(resource.Uri, true)))
                    {
                        return resource;
                    }
                }

                return null;
            }

            public OperationInfo GetOperationInfo(string uriTemplate, string httpMethod)
            {
                ResourceInfo resource = this.GetResourceInfo(uriTemplate);
                if (resource != null && resource.OperationDictionary.ContainsKey(httpMethod))
                {
                    return resource.OperationDictionary[httpMethod];
                }

                return null;
            }

            public HttpOperationDescription GetOperationDescription(string uriTemplate, string httpMethod)
            {
                OperationInfo operation = this.GetOperationInfo(uriTemplate, httpMethod);
                if (operation != null)
                {
                    return operation.Description;
                }

                return null;
            }
        }

        internal class ResourceInfo
        {
            public ResourceInfo()
            {
                this.OperationDictionary = new Dictionary<string, OperationInfo>();
            }

            // Full Uri
            public string Uri { get; set; }

            public string BaseAddress
            {
                get
                {
                    return this.Endpoint.Address.Uri.AbsoluteUri;
                }
            }

            // Relative template
            [ScriptIgnore]
            public UriTemplate UriTemplate { get; set; }

            [ScriptIgnore]
            public HttpEndpoint Endpoint { get; set; }

            public IEnumerable<OperationInfo> Operations
            {
                get
                {
                    return this.OperationDictionary.Values;
                }
            }

            [ScriptIgnore]
            public Dictionary<string, OperationInfo> OperationDictionary { get; private set; }
        }

        internal class OperationInfo
        {               
            public string HttpMethod { get; set; }

            public string Name
            {
                get
                {
                    return this.Description.Name;
                }
            }

            [ScriptIgnore]
            public ResourceInfo ResourceInfo { get; set; }
            
            [ScriptIgnore]
            public HttpOperationDescription Description { get; set; }
        }

        internal class HttpHeaderInfo
        {
            public HttpHeaderInfo(string key, string value)
            {
                this.Key = key;
                this.Value = value;
            }

            public string Key { get; set; }

            public string Value { get; set; }
        }
    }
}
