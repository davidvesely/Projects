// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Runtime.Serialization;
    using Microsoft.Server.Common;

    internal class RequestContentInvoker : IOperationInvoker
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        private HttpEndpoint endpoint;
        private HttpTestUtils.ServiceInfo serviceInfo;
        private Dictionary<string, XmlSchemaSet> schemaDic = new Dictionary<string, XmlSchemaSet>(StringComparer.OrdinalIgnoreCase);

        public RequestContentInvoker(HttpEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        private HttpTestUtils.ServiceInfo ServiceInfo
        {
            get
            {
                if (this.serviceInfo == null)
                {
                    this.serviceInfo = HttpTestUtils.GetServiceInfo(this.endpoint);
                }

                return this.serviceInfo;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposed later.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design. Any uncaught exception will be turned into an internal server error response")]
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            HttpRequestMessage request = inputs[0] as HttpRequestMessage;
            try
            {
                string content = request.Content.ReadAsStringAsync().Result;
                string resourceUriTemplate = (string)inputs[1];
                string method = (string)inputs[2];
                string operation = (string)inputs[3];
                int cursorPos = (int)inputs[4];
                string format0 = (string)inputs[5];

                if (string.Equals(operation, "autocomplete", StringComparison.OrdinalIgnoreCase))
                {
                    if (cursorPos <= 0 || cursorPos > content.Length)
                    {
                        return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
                    }

                    if (string.Equals(format0, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        return this.CreateXmlContentIntellisenseResponse(request, content, resourceUriTemplate, method, cursorPos);
                    }
                    else if (string.Equals(format0, "json", StringComparison.OrdinalIgnoreCase))
                    {
                        return this.CreateJsonContentIntellisenseResponse(request, content, resourceUriTemplate, method, cursorPos);
                    }
                }
                else if (string.Equals(operation, "format", StringComparison.OrdinalIgnoreCase))
                {
                    return HttpTestUtils.CreateFormattingResponse(request, format0, content, false);
                }
                else if (string.Equals(operation, "validate", StringComparison.OrdinalIgnoreCase))
                {
                    HttpOperationDescription operationDescription = this.ServiceInfo.GetOperationDescription(resourceUriTemplate, method);
                    if (operationDescription != null)
                    {
                        bool useXmlSerializer = ShouldUseXmlSerializer(operationDescription);
                        Type type = RequestContentInvoker.GetRequestBodyType(operationDescription, resourceUriTemplate);
                        return CreateValidationResponse(request, content, useXmlSerializer, type, format0);
                    }
                }
                else if (string.Equals(operation, "convert", StringComparison.OrdinalIgnoreCase))
                {
                    string format1 = (string)inputs[6];
                    HttpOperationDescription operationDescription = this.ServiceInfo.GetOperationDescription(resourceUriTemplate, method);
                    if (operationDescription != null)
                    {
                        bool useXmlSerializer = ShouldUseXmlSerializer(operationDescription);
                        Type type = RequestContentInvoker.GetRequestBodyType(operationDescription, resourceUriTemplate);
                        if (string.Equals(format0, "xml", StringComparison.OrdinalIgnoreCase))
                        {
                            if (typeof(XObject).IsAssignableFrom(type) || typeof(XmlNode).IsAssignableFrom(type))
                            {
                                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
                            }
                        }

                        //// TODO: if format0 is json and type is JsonValue, create bad request response

                        return CreateConversionResponse(request, content, useXmlSerializer, type, format0, format1);
                    }
                }

                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
            }
            catch (Exception e)
            {
                return HttpTestUtils.CreateErrorResponseMessage(request, e);
            }
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
            return new object[7];
        }

        private static bool ShouldUseXmlSerializer(HttpOperationDescription operationDescription)
        {
            return HttpOperationHandlerFactory.DetermineSerializerFormat(operationDescription) != XmlMediaTypeSerializerFormat.DataContractSerializer;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private static object CreateConversionResponse(HttpRequestMessage request, string content, bool useXmlSerializer, Type type, string srcFormat, string destFormat)
        {
            if (string.Equals(srcFormat, "xml", StringComparison.OrdinalIgnoreCase) && string.Equals(destFormat, "json", StringComparison.OrdinalIgnoreCase))
            {
                return HttpTestUtils.CreateStringResponse(request, HttpTestUtils.FormatJson(HttpTestUtils.Xml2Json(content, type, useXmlSerializer)), new MediaTypeHeaderValue("text/plain"));
            }
            else if (string.Equals(srcFormat, "json", StringComparison.OrdinalIgnoreCase) && string.Equals(destFormat, "xml", StringComparison.OrdinalIgnoreCase))
            {
                return HttpTestUtils.CreateStringResponse(request, HttpTestUtils.FormatXml(HttpTestUtils.Json2Xml(content, type, useXmlSerializer)), new MediaTypeHeaderValue("text/plain"));
            }

            return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
        }

        private static HttpResponseMessage CreateValidationResponse(HttpRequestMessage request, string content, bool useXmlSerializer, Type type, string format)
        {
            object dummy;
            bool isValid = false;
            if (string.Equals(format, "xml", StringComparison.OrdinalIgnoreCase))
            {
                isValid = HttpTestUtils.TryDeserializeXml(content, type, useXmlSerializer, out dummy);
            }
            else if (string.Equals(format, "json", StringComparison.OrdinalIgnoreCase))
            {
                isValid = HttpTestUtils.TryDeserializeJson(content, type, out dummy);
            }
            else
            {
                return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
            }

            return HttpTestUtils.CreateBoolResponse(request, isValid);
        }

        private static Type GetRequestBodyType(HttpOperationDescription description, string uriTemplate)
        {
            if (description.Behaviors.Contains(typeof(WebGetAttribute)))
            {
                return typeof(void);
            }

            UriTemplate template = new UriTemplate(uriTemplate);
            IEnumerable<HttpParameter> parameterDescriptions =
                from parameterDescription in description.InputParameters
                where !template.PathSegmentVariableNames.Contains(parameterDescription.Name.ToUpperInvariant()) && !template.QueryValueVariableNames.Contains(parameterDescription.Name.ToUpperInvariant())
                select parameterDescription;

            HttpParameter[] matchingParameters = parameterDescriptions.ToArray();

            // No match == void
            // One match == the body type
            // Multiple matches == the request is wrapped and not supported
            Type type = matchingParameters.Length == 0
                    ? typeof(void)
                    : (matchingParameters.Length == 1)
                        ? matchingParameters[0].ParameterType
                        : null;

            if (type != null)
            {
                Type innerType = HttpTypeHelper.GetHttpRequestOrContentInnerTypeOrNull(type);
                if (innerType != null)
                {
                    return innerType;
                }
            }

            return type;
        }

        private static XmlSchema FixSchema(XmlSchema schema, XmlQualifiedName rootElementName)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                schema.Write(writer);
            }

            XDocument doc = XDocument.Parse(sb.ToString());

            // 1. The default generated schema for DataContractSerializer contains an 'xs:element'
            // for each type as the child of 'xs:schema'. That makes intellisense return multiple options
            // for the root element. So remove any 'xs:element' that is not the root element.
            if (rootElementName != null)
            {
                foreach (XElement element in doc.Root.Elements(XName.Get("element", RequestContentInvoker.XmlSchemaNamespace)).ToArray<XElement>())
                {
                    if (!string.Equals(schema.TargetNamespace, rootElementName.Namespace, StringComparison.Ordinal))
                    {
                        element.Remove();
                    }
                    else
                    {
                        XAttribute name = element.Attribute("name");
                        if (name == null || !string.Equals(rootElementName.Name, name.Value, StringComparison.Ordinal))
                        {
                            element.Remove();
                        }
                    }
                }
            }

            // 2. The default generated schema uses 'xs:sequence' for the members of complex types.
            // And the 'xs:sequence' element specifies that the child elements must appear in a sequence,
            // while the 'xs:all' element specifies that the child elements can appear in any order.
            // So replace 'xs:sequence' with 'xs:all' to make intellisense more friendly.
            foreach (XElement sequence in doc.Descendants(XName.Get("sequence", RequestContentInvoker.XmlSchemaNamespace)).ToArray<XElement>())
            {
                if (sequence.Elements().All<XElement>((e) =>
                {
                    XAttribute maxOccurs = e.Attribute("maxOccurs");
                    XAttribute minOccurs = e.Attribute("minOccurs");
                    return (maxOccurs == null || maxOccurs.Value == "1") &&
                        (minOccurs == null || minOccurs.Value == "0" || minOccurs.Value == "1");
                }))
                {
                    XElement all = new XElement(XName.Get("all", RequestContentInvoker.XmlSchemaNamespace));
                    all.Add(sequence.Elements());
                    sequence.ReplaceWith(all);
                }
            }

            using (XmlReader reader = doc.CreateReader())
            {
                return XmlSchema.Read(reader, null);
            }
        }

        private static Type GetSubstituteDataContractType(Type type, out bool isQueryable)
        {
            if (type == typeof(IQueryable))
            {
                isQueryable = true;
                return typeof(IEnumerable);
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                isQueryable = true;
                return typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments());
            }

            isQueryable = false;
            return type;
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "dispose is idempotent.")]
        private static string GetSchemaString(XmlSchema schema)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                CloseOutput = false,
                Indent = true,
            };

            StringBuilder sb = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    schema.Write(writer);
                }
            }

            return sb.ToString();
        }

        private static void DebugPrintSchema(XmlSchemaSet schemaSet)
        {
            StringBuilder schemaSb = new StringBuilder();
            foreach (XmlSchema s in schemaSet.Schemas())
            {
                schemaSb.AppendLine(RequestContentInvoker.GetSchemaString(s));
            }

            string schema = schemaSb.ToString();
            Debug.WriteLine(schema);
        }

        private XmlSchemaSet GetXmlSchemaSet(string method, string uriTemplate)
        {
            XmlSchemaSet schemaSet = null;
            lock (this.schemaDic)
            {         
                string key = method + uriTemplate;
                if (this.schemaDic.TryGetValue(key, out schemaSet))
                {
                    return schemaSet;
                }

                HttpOperationDescription description = this.ServiceInfo.GetOperationDescription(uriTemplate, method);
                if (description != null)
                {
                    Type type = RequestContentInvoker.GetRequestBodyType(description, uriTemplate);
                    bool usesXmlSerializer = ShouldUseXmlSerializer(description);
                    if (usesXmlSerializer)
                    {
                        schemaSet = new XmlSchemaSet();
                        XmlReflectionImporter importer = new XmlReflectionImporter();
                        XmlTypeMapping typeMapping = importer.ImportTypeMapping(type);
                        XmlSchemas schemas = new XmlSchemas();
                        XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);
                        exporter.ExportTypeMapping(typeMapping);
                        foreach (XmlSchema schema in schemas)
                        {
                            schemaSet.Add(RequestContentInvoker.FixSchema(schema, null));
                        }
                    }
                    else
                    {
                        XsdDataContractExporter exporter = new XsdDataContractExporter();
                        List<Type> listTypes = new List<Type>(description.KnownTypes);
                        bool isQueryable;
                        Type dataContractType = GetSubstituteDataContractType(type, out isQueryable);
                        if (exporter.CanExport(dataContractType))
                        {
                            schemaSet = new XmlSchemaSet();
                            listTypes.Add(dataContractType);
                            exporter.Export(listTypes);
                            XmlQualifiedName rootElementName = exporter.GetRootElementName(dataContractType);

                            foreach (XmlSchema schema in exporter.Schemas.Schemas())
                            {
                                schemaSet.Add(RequestContentInvoker.FixSchema(schema, rootElementName));
                            }
                        }
                    }
                }

                this.schemaDic.Add(key, schemaSet);
            }

            return schemaSet;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private HttpResponseMessage CreateXmlContentIntellisenseResponse(HttpRequestMessage request, string content, string resourceUriTemplate, string method, int cursorPos)
        {
            XmlSchemaSet schemaSet = this.GetXmlSchemaSet(method, resourceUriTemplate);
            if (schemaSet != null)
            {
#if DEBUG
                RequestContentInvoker.DebugPrintSchema(schemaSet);
#endif
                content = HttpTestUtils.CleanString(content, ref cursorPos);

                // get autocomplete list
                using (TolerantXmlTextReader reader = new TolerantXmlTextReader(content, cursorPos, schemaSet))
                {
                    try
                    {
                        return HttpTestUtils.CreateIntellisenseResponse(reader, request);
                    }
                    catch (XmlException e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private HttpResponseMessage CreateJsonContentIntellisenseResponse(HttpRequestMessage request, string content, string resourceUriTemplate, string method, int cursorPos)
        {
            HttpOperationDescription description = this.ServiceInfo.GetOperationDescription(resourceUriTemplate, method);
            if (description != null)
            {
                Type type = RequestContentInvoker.GetRequestBodyType(description, resourceUriTemplate);

                content = HttpTestUtils.CleanString(content, ref cursorPos);

                // get autocomplete list
                using (TolerantJsonTextReader reader = new TolerantJsonTextReader(content, cursorPos, type))
                {
                    try
                    {
                        return HttpTestUtils.CreateIntellisenseResponse(reader, request);
                    }
                    catch (XmlException e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            return StandardHttpResponseMessageBuilder.CreateBadRequestResponse(request);
        }
    }
}