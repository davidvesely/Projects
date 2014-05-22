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
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using System.Xml.Schema;
    using Microsoft.Server.Common;

    internal class TolerantXmlTextReader : IDisposable, ITolerantTextReader
    {
        private static readonly Regex regexNonSpaceChar = new Regex(@"\S", RegexOptions.Compiled | RegexOptions.Multiline);

        public TolerantXmlTextReader(string text, int stopAtPosition, XmlSchemaSet schemaSet)
        {
            this.Text = text;
            this.LineInfo = new LineInformation(text);

            this.StopAtPosition = stopAtPosition >= 0
                ? stopAtPosition
                : text.Length;
            this.Position = 0;
            this.TokenPosition = 0;

            this.StringReader = new StringReader(text);
            this.XmlTextReader = new XmlTextReader(this.StringReader);

            this.NameTable = this.XmlTextReader.NameTable;

            this.SchemaSet = schemaSet;
            if (!this.SchemaSet.IsCompiled)
            {
                this.SchemaSet.Compile();
            }

            this.NamespaceManager = new XmlNamespaceManager(this.NameTable);

            this.Document = new XmlDocument(this.NameTable);
            this.Node = this.Document;
            this.NodeTypeEx = XmlNodeTypeEx.Other;
        }

        private enum XmlNodeTypeEx
        {
            Other = 1000,
            AttributeName,
            AttributeValue,
            BeginElement,
            EndElement,
            Text,
        }

        public Exception Exception { get; private set; }

        private string Text { get; set; }

        private LineInformation LineInfo { get; set; }

        private int StopAtPosition { get; set; }

        private int Position { get; set; }

        private int TokenPosition { get; set; } // where replacement should start, absolute index

        private StringReader StringReader { get; set; }

        private XmlTextReader XmlTextReader { get; set; }

        private MiniXmlReader MyXmlReader { get; set; }

        private XmlNameTable NameTable { get; set; }

        private XmlSchemaSet SchemaSet { get; set; }

        private XmlNamespaceManager NamespaceManager { get; set; }

        private XmlDocument Document { get; set; }

        private XmlNode Node { get; set; }

        private XmlNode ParentNode
        {
            get
            {
                return this.Node.NodeType == XmlNodeType.Attribute
                    ? ((XmlAttribute)this.Node).OwnerElement
                    : this.Node.ParentNode;
            }
        }

        private XmlNodeTypeEx NodeTypeEx { get; set; }

        public void Dispose()
        {
            if (this.StringReader != null)
            {
                this.StringReader.Dispose();
                this.StringReader = null;
            }

            this.DisposeXmlTextReader(false);

            if (this.MyXmlReader != null)
            {
                this.MyXmlReader.Dispose();
                this.MyXmlReader = null;
            }

            GC.SuppressFinalize(this);
        }

        public void SchemaValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Debug.WriteLine("\nError: {0}", e.Message);
                    break;

                case XmlSeverityType.Warning:
                    Debug.WriteLine("\nWarning: {0}", e.Message);
                    break;
            }
        }

        public bool Read()
        {
            if (this.XmlTextReader != null)
            {
                try
                {
                    return this.ReadWithXmlTextReader();
                }
                catch (XmlException e)
                {
                    this.DisposeXmlTextReader(true);

                    string text = this.StopAtPosition <= this.Text.Length
                        ? this.Text.Substring(0, this.StopAtPosition)
                        : this.Text;
                    this.MyXmlReader = new MiniXmlReader(text, this.Position, this.Document, this.NamespaceManager, e);

                    // fall through
                }
            }

            if (this.MyXmlReader != null)
            {
                try
                {
                    return this.ReadWithMiniXmlReader();
                }
                catch (XmlException e)
                {
                    this.Exception = e;

                    this.MyXmlReader.Dispose();
                    this.MyXmlReader = null;

                    // fall through
                }
            }

            return false;
        }

        public IEnumerable<string> GetExpectedItems()
        {
            XmlSchemaValidator validator = new XmlSchemaValidator(this.NameTable, this.SchemaSet, this.NamespaceManager, XmlSchemaValidationFlags.None);
            validator.ValidationEventHandler += new ValidationEventHandler(this.SchemaValidationEventHandler);
            validator.Initialize();

            // get a list of all ancestor elements
            LinkedList<XmlElement> elements = new LinkedList<XmlElement>();
            for (XmlElement element = this.ParentNode as XmlElement; element != null; element = element.ParentNode as XmlElement)
            {
                elements.AddFirst(element);
            }

            switch (this.NodeTypeEx)
            {
                case XmlNodeTypeEx.BeginElement:
                    return this.GetExpectedElementTags(validator, elements);

                case XmlNodeTypeEx.EndElement:
                    return this.GetExpectedEndTags();

                case XmlNodeTypeEx.AttributeName:
                    return this.GetExpectedAttribNames(validator, elements);

                case XmlNodeTypeEx.AttributeValue:
                    return this.GetExpectedAttribValues(validator, elements);

                case XmlNodeTypeEx.Text:
                    return this.GetExpectedElementTexts(validator, elements);

                default:
                    return TolerantTextReaderHelper.EmptyStringArray;
            }
        }

        private static void ValidateElementChain(XmlSchemaValidator validator, LinkedList<XmlElement> elements)
        {
            XmlSchemaInfo schemaInfo = new XmlSchemaInfo();

            for (LinkedListNode<XmlElement> node = elements.First, next = node != null ? node.Next : null;
                next != null;
                node = next, next = node.Next)
            {
                XmlElement element = node.Value;
                validator.ValidateElement(element.LocalName, element.NamespaceURI, schemaInfo);
                validator.ValidateEndOfAttributes(schemaInfo);

                foreach (XmlElement child in element.ChildNodes)
                {
                    if (child == next.Value)
                    {
                        break;
                    }

                    if (child.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    validator.ValidateElement(child.LocalName, child.NamespaceURI, schemaInfo);
                    validator.ValidateEndOfAttributes(schemaInfo);
                    validator.ValidateEndElement(schemaInfo);
                }
            }
        }

        private string GetExpectedNamespaceUri(HashSet<string> namespaceUris, string namespaceUri)
        {
            if (string.IsNullOrEmpty(namespaceUri) || namespaceUris.Contains(namespaceUri))
            {
                return null;
            }

            namespaceUris.Add(namespaceUri);

            string displayName = namespaceUri;
            string value = HttpUtility.HtmlEncode(namespaceUri);
            return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
        }

        private IEnumerable<string> GetExpectedElementTags(XmlSchemaValidator validator, LinkedList<XmlElement> elements)
        {
            // this.Node is current element
            elements.AddLast((XmlElement)this.Node);
            TolerantXmlTextReader.ValidateElementChain(validator, elements);

            foreach (XmlSchemaParticle particle in validator.GetExpectedParticles())
            {
                XmlSchemaElement schemaElement = particle as XmlSchemaElement;
                if (schemaElement != null)
                {
                    string namespaceUri = schemaElement.QualifiedName.Namespace;

                    // remove namespace from namespace manager, in case it is defined by xmlns attribs of current element
                    foreach (XmlAttribute attrib in ((XmlElement)this.Node).Attributes)
                    {
                        if (string.IsNullOrEmpty(attrib.Prefix) && "xmlns".Equals(attrib.LocalName, StringComparison.Ordinal))
                        {
                            if (string.Equals(attrib.Value, namespaceUri, StringComparison.Ordinal))
                            {
                                this.NamespaceManager.RemoveNamespace(string.Empty, namespaceUri);
                            }
                        }
                        else if ("xmlns".Equals(attrib.Prefix, StringComparison.Ordinal))
                        {
                            if (string.Equals(attrib.Value, namespaceUri, StringComparison.Ordinal))
                            {
                                this.NamespaceManager.RemoveNamespace(attrib.LocalName, namespaceUri);
                            }
                        }
                    }

                    string displayName;
                    string value;
                    if (string.Equals(this.NamespaceManager.DefaultNamespace, namespaceUri, StringComparison.Ordinal))
                    {
                        // prefix not needed
                        displayName = schemaElement.Name;
                        value = string.Format(CultureInfo.InvariantCulture, "{0}></{0}>", displayName);
                    }
                    else
                    {
                        string prefix = this.NamespaceManager.LookupPrefix(namespaceUri);
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            // prefix defined
                            PrefixedName prefixedName = new PrefixedName(prefix, schemaElement.Name);
                            displayName = prefixedName.ToString();
                            value = string.Format(CultureInfo.InvariantCulture, "{0}></{0}>", displayName);
                        }
                        else
                        {
                            // need new xmlns attrib
                            displayName = schemaElement.Name;
                            value = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} xmlns=\"{1}\"></{0}>",
                                displayName,
                                HttpUtility.HtmlEncode(namespaceUri));
                        }
                    }

                    yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition, -displayName.Length - 3);
                }
            }
        }

        private IEnumerable<string> GetExpectedEndTags()
        {
            // this.Node is parent element
            XmlElement lastElement = null;
            foreach (XmlNode child in this.Node.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (child != null)
                {
                    lastElement = element;
                }
            }

            Fx.Assert(lastElement != null, "end element not found, impossible");

            string displayName = lastElement.Name; // including prefix
            string value = string.Format(CultureInfo.InvariantCulture, "{0}>", displayName);
            yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
        }

        private IEnumerable<string> GetExpectedAttribNames(XmlSchemaValidator validator, LinkedList<XmlElement> elements)
        {
            // this.Node is current element
            elements.AddLast((XmlElement)this.Node);
            TolerantXmlTextReader.ValidateElementChain(validator, elements);

            XmlSchemaInfo schemaInfo = new XmlSchemaInfo();
            validator.ValidateElement(this.Node.LocalName, this.Node.NamespaceURI, schemaInfo);

            foreach (XmlSchemaAttribute schemaAttrib in validator.GetExpectedAttributes())
            {
                string namespaceUri = schemaAttrib.QualifiedName.Namespace;

                string displayName;
                string value;
                if (string.IsNullOrEmpty(namespaceUri))
                {
                    // prefix not needed
                    displayName = schemaAttrib.Name;
                    value = string.Format(CultureInfo.InvariantCulture, "{0}=\"\"", displayName);
                }
                else
                {
                    string prefix = this.NamespaceManager.LookupPrefix(namespaceUri);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        // prefix defined
                        PrefixedName prefixedName = new PrefixedName(prefix, schemaAttrib.QualifiedName.Name);
                        displayName = prefixedName.ToString();
                        value = string.Format(CultureInfo.InvariantCulture, "{0}=\"\"", displayName);
                    }
                    else
                    {
                        // prefix not found, omit this attrib
                        continue;
                    }
                }

                yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition, -1);
            }
        }

        private IEnumerable<string> GetExpectedAttribValues(XmlSchemaValidator validator, LinkedList<XmlElement> elements)
        {
            // this.Node is current attribute
            TolerantXmlTextReader.ValidateElementChain(validator, elements);

            XmlElement lastElement = ((XmlAttribute)this.Node).OwnerElement;
            XmlSchemaInfo schemaInfo = new XmlSchemaInfo();
            validator.ValidateElement(lastElement.LocalName, lastElement.NamespaceURI, schemaInfo);

            bool isXmlns = string.IsNullOrEmpty(this.Node.Prefix) && "xmlns".Equals(this.Node.Name, StringComparison.Ordinal);
            bool isXmlnsX = "xmlns".Equals(this.Node.Prefix, StringComparison.Ordinal);

            if (isXmlns || isXmlnsX)
            {
                // xmlns attribute

                // avoid dup
                HashSet<string> namespaceUris = new HashSet<string>();

                // return namespaces of last element
                XmlQualifiedName qualifiedName = new XmlQualifiedName(lastElement.LocalName, lastElement.NamespaceURI);
                List<XmlSchemaElement> schemaElements = new List<XmlSchemaElement>();
                foreach (XmlSchema schema in this.SchemaSet.Schemas())
                {
                    XmlSchemaElement schemaElement = schema.Elements[qualifiedName] as XmlSchemaElement;
                    if (schemaElement != null)
                    {
                        // last element is recognized in schema
                        schemaElements.Add(schemaElement);
                    }
                    else
                    {
                        // let's find all matching schemaElements with the same local name
                        schemaElements.AddRange(
                            from schemaElement2 in schema.Elements.Values.Cast<XmlSchemaElement>()
                            where string.Equals(lastElement.LocalName, schemaElement2.Name, StringComparison.Ordinal)
                            select schemaElement2);
                    }
                }

                foreach (XmlSchemaElement schemaElement in schemaElements)
                {
                    string namespaceUri = schemaElement.QualifiedName.Namespace;
                    string result = this.GetExpectedNamespaceUri(namespaceUris, namespaceUri);
                    if (result != null)
                    {
                        yield return result;
                    }

                    if (isXmlnsX)
                    {
                        foreach (XmlQualifiedName name in schemaElement.Namespaces.ToArray())
                        {
                            result = this.GetExpectedNamespaceUri(namespaceUris, name.Namespace);
                            if (result != null)
                            {
                                yield return result;
                            }
                        }
                    }
                }

                if (isXmlnsX || namespaceUris.Count < 1)
                {
                    // ok to show other namespace uris

                    // return target namespaces
                    foreach (XmlSchema schema in this.SchemaSet.Schemas())
                    {
                        string result = this.GetExpectedNamespaceUri(namespaceUris, schema.TargetNamespace);
                        if (result != null)
                        {
                            yield return result;
                        }
                    }

                    // return always applicable default namespaces 
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                    foreach (string prefix in nsmgr)
                    {
                        string namespaceUri = nsmgr.LookupNamespace(prefix);
                        string result = this.GetExpectedNamespaceUri(namespaceUris, namespaceUri);
                        if (result != null)
                        {
                            yield return result;
                        }
                    }
                }
            }
            else
            {
                // normal attribute
                foreach (XmlAttribute attrib in lastElement.Attributes)
                {
                    validator.ValidateAttribute(attrib.LocalName, attrib.NamespaceURI, attrib.Value, schemaInfo);

                    if (attrib == this.Node)
                    {
                        break;
                    }
                }

                foreach (string item in this.GetSimpleTypeValues(schemaInfo.SchemaType))
                {
                    string displayName = item;
                    string value = HttpUtility.HtmlEncode(item);
                    yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                }
            }
        }

        private IEnumerable<string> GetExpectedElementTexts(XmlSchemaValidator validator, LinkedList<XmlElement> elements)
        {
            // this.Node is current element
            elements.AddLast((XmlElement)this.Node);
            TolerantXmlTextReader.ValidateElementChain(validator, elements);

            XmlSchemaInfo schemaInfo = new XmlSchemaInfo();
            validator.ValidateElement(this.Node.LocalName, this.Node.NamespaceURI, schemaInfo);
            validator.ValidateEndOfAttributes(schemaInfo);

            foreach (string item in this.GetSimpleTypeValues(schemaInfo.SchemaType))
            {
                string displayName = item;
                string value = HttpUtility.HtmlEncode(item);
                yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
            }
        }

        private XmlAttribute CreateXmlAttrib(string prefix, string localName, string namespaceUri)
        {
            return string.IsNullOrEmpty(prefix)
                ? this.Document.CreateAttribute(localName)
                : this.Document.CreateAttribute(prefix, localName, namespaceUri);
        }

        private void DisposeXmlTextReader(bool saveNamespaces)
        {
            if (this.XmlTextReader != null)
            {
                if (saveNamespaces)
                {
                    IDictionary<string, string> namespaces = this.XmlTextReader.GetNamespacesInScope(XmlNamespaceScope.All);
                    foreach (string prefix in namespaces.Keys)
                    {
                        string namespaceURI = namespaces[prefix];
                        this.NamespaceManager.AddNamespace(prefix, namespaceURI);
                    }
                }

                this.XmlTextReader.Close();
                this.XmlTextReader = null;
            }
        }

        private IEnumerable<string> GetSimpleTypeValues(XmlSchemaType schemaType)
        {
            XmlSchemaSimpleType simpleType = schemaType as XmlSchemaSimpleType;
            if (simpleType == null)
            {
                yield break;
            }

            XmlSchemaSimpleTypeRestriction restriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
            if (restriction != null)
            {
                bool hasEnum = false;
                bool hasPattern = false;

                foreach (XmlSchemaObject schemaObject in restriction.Facets)
                {
                    if (schemaObject is XmlSchemaPatternFacet)
                    {
                        hasPattern = true;
                        continue;
                    }

                    XmlSchemaEnumerationFacet enumFacet = schemaObject as XmlSchemaEnumerationFacet;
                    if (enumFacet != null)
                    {
                        hasEnum = true;
                        yield return enumFacet.Value;
                    }
                }

                if (hasEnum)
                {
                    yield break;
                }

                if (hasPattern)
                {
                    // <simpleType ...><restriction ...><pattern ...>
                    // do not provide intellisense for primitive types
                    yield break;
                }

                // fall through to primitive types
            }

            switch (simpleType.TypeCode)
            {
                case XmlTypeCode.Boolean:
                    yield return "true";
                    yield return "false";
                    break;

                case XmlTypeCode.DateTime:
                    // yyyy-MM-ddTHH:mm:ss.ssszzz
                    yield return XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.RoundtripKind);
                    break;

                case XmlTypeCode.Date:
                    yield return XmlConvert.ToString(DateTime.Now, "yyyy-MM-ddzzz");
                    break;

                case XmlTypeCode.Time:
                    yield return XmlConvert.ToString(DateTime.Now, "HH:mm:sszzz");
                    break;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design")]
        private bool ReadWithXmlTextReader()
        {
            if (this.Position >= this.StopAtPosition)
            {
                this.DisposeXmlTextReader(true);

                return false;
            }

            if (!this.XmlTextReader.Read())
            {
                this.DisposeXmlTextReader(true);

                return false;
            }

            this.NodeTypeEx = XmlNodeTypeEx.Other;

            switch (this.XmlTextReader.NodeType)
            {
                case XmlNodeType.CDATA: // <![CDATA[my escaped text]]>
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf("]]>", this.Position, StringComparison.Ordinal) + "]]>".Length;
                    break;

                case XmlNodeType.Comment:   // <!-- my comment -->
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf("-->", this.Position, StringComparison.Ordinal) + "-->".Length;
                    break;

                case XmlNodeType.DocumentType:  // <!DOCTYPE...>
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf('>', this.Position) + 1;
                    break;

                case XmlNodeType.Element:
                    this.ReadElementWithXmlTextReader();
                    break;

                case XmlNodeType.EndElement:
                    this.ReadEndElementWithXmlTextReader(false);
                    break;

                case XmlNodeType.Entity:    // <!ENTITY name "text">
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf('>', this.Position) + 1;
                    break;

                case XmlNodeType.EntityReference:   // &name;
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf(';', this.Position) + 1;
                    break;

                case XmlNodeType.Notation:  // <!NOTATION...>
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf('>', this.Position) + 1;
                    break;

                case XmlNodeType.ProcessingInstruction: // <?pi test?>
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf("?>", this.Position, StringComparison.Ordinal) + "?>".Length;
                    break;

                case XmlNodeType.SignificantWhitespace:
                    this.ReadTextWithXmlTextReader();
                    break;

                case XmlNodeType.Text:
                    this.ReadTextWithXmlTextReader();
                    break;

                case XmlNodeType.Whitespace:
                    this.ReadTextWithXmlTextReader();
                    break;

                case XmlNodeType.XmlDeclaration:    // <?xml version='1.0'?>
                    this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                    this.Position = this.Text.IndexOf("?>", this.Position, StringComparison.Ordinal) + "?>".Length;
                    break;
            }

            return true;
        }

        private void ReadElementWithXmlTextReader()
        {
            // XmlTextReader.NodeType is never XmlNodeType.Attribute
            // attributes are read together with Element
            bool isEmptyElement = this.XmlTextReader.IsEmptyElement;

            XmlElement element = this.Document.CreateElement(this.XmlTextReader.Prefix, this.XmlTextReader.LocalName, this.XmlTextReader.NamespaceURI);
            this.Node.AppendChild(element);
            this.Node = element;

            this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
            this.TokenPosition = this.Position;
            this.Position += element.Name.Length;   // including prefix
            if (this.Position >= this.StopAtPosition)
            {
                this.NodeTypeEx = XmlNodeTypeEx.BeginElement;

                // save xmlns attribs
                for (bool result = this.XmlTextReader.MoveToFirstAttribute(); result; result = this.XmlTextReader.MoveToNextAttribute())
                {
                    XmlAttribute attrib = this.CreateXmlAttrib(this.XmlTextReader.Prefix, this.XmlTextReader.LocalName, this.XmlTextReader.NamespaceURI);
                    attrib.Value = this.XmlTextReader.Value;
                    element.Attributes.Append(attrib);
                }

                return;
            }

            for (bool result = this.XmlTextReader.MoveToFirstAttribute(); result; result = this.XmlTextReader.MoveToNextAttribute())
            {
                XmlAttribute attrib = this.CreateXmlAttrib(this.XmlTextReader.Prefix, this.XmlTextReader.LocalName, this.XmlTextReader.NamespaceURI);
                attrib.Value = this.XmlTextReader.Value;
                element.Attributes.Append(attrib);

                this.TokenPosition = TolerantXmlTextReader.regexNonSpaceChar.Match(this.Text, this.Position).Index;
                this.Position = this.Text.IndexOf('=', this.Position);
                if (this.Position >= this.StopAtPosition)
                {
                    this.NodeTypeEx = XmlNodeTypeEx.AttributeName;
                    return;
                }

                this.Position++;

                this.Position = this.Text.IndexOf('"', this.Position) + 1;
                this.TokenPosition = this.Position;
                this.Position = this.Text.IndexOf('"', this.Position);
                if (this.Position >= this.StopAtPosition)
                {
                    this.Node = attrib;
                    this.NodeTypeEx = XmlNodeTypeEx.AttributeValue;
                    return;
                }

                this.Position++;
            }

            if (isEmptyElement)
            {
                this.ReadEndElementWithXmlTextReader(true);
                return;
            }

            this.Position = this.Text.IndexOf('>', this.Position);
            this.TokenPosition = this.Position;
            if (this.Position >= this.StopAtPosition)
            {
                if (char.IsWhiteSpace(this.Text[this.TokenPosition - 1]))
                {
                    this.NodeTypeEx = XmlNodeTypeEx.AttributeName;
                }

                return;
            }

            this.Position++;
            this.TokenPosition = this.Position;
            if (this.Position >= this.StopAtPosition)
            {
                this.NodeTypeEx = XmlNodeTypeEx.Text;
                return;
            }
        }

        private void ReadEndElementWithXmlTextReader(bool isEmptyElement)
        {
            this.Node = this.ParentNode;

            if (!isEmptyElement)
            {
                this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
                if (this.Position > this.StopAtPosition)
                {
                    // <|/abc>
                    return;
                }
            }

            this.TokenPosition = this.Position;
            this.Position = this.Text.IndexOf('>', this.Position);

            if (!isEmptyElement)
            {
                if (this.Position >= this.StopAtPosition)
                {
                    this.NodeTypeEx = XmlNodeTypeEx.EndElement;
                    return;
                }
            }

            this.Position++;
        }

        private void ReadTextWithXmlTextReader()
        {
            if (this.XmlTextReader.NodeType == XmlNodeType.Text)
            {
                XmlText text = this.Document.CreateTextNode(this.XmlTextReader.Value);
                this.Node.AppendChild(text);
            }
            else
            {
                // nop for whitespace and significant whitespace
            }

            this.Position = this.LineInfo.GetPosition(this.XmlTextReader.LineNumber, this.XmlTextReader.LinePosition);
            this.TokenPosition = this.Position;

            // do not use this.Position += text.Length;
            // because text might be encoded
            int pos = this.Text.IndexOf('<', this.Position);
            this.Position = pos >= 0 ? pos : this.StopAtPosition;
            if (this.Position >= this.StopAtPosition)
            {
                this.NodeTypeEx = XmlNodeTypeEx.Text;
                return;
            }
        }

        private bool ReadWithMiniXmlReader()
        {
            if (!this.MyXmlReader.Read())
            {
                this.MyXmlReader.Dispose();
                this.MyXmlReader = null;

                return false;
            }

            this.NodeTypeEx = this.MyXmlReader.NodeTypeEx;
            this.TokenPosition = this.MyXmlReader.TokenPosition;

            XmlElement element;
            switch (this.NodeTypeEx)
            {
                case XmlNodeTypeEx.BeginElement:
                    // this.MyXmlReader.Node is current element
                    this.Node.AppendChild(this.MyXmlReader.Node);
                    this.Node = this.MyXmlReader.Node;
                    break;

                case XmlNodeTypeEx.EndElement:
                    this.Node = this.ParentNode;
                    break;

                case XmlNodeTypeEx.AttributeName:
                    // this.MyXmlReader.Node is current attrib
                    element = ((XmlAttribute)this.MyXmlReader.Node).OwnerElement;
                    this.Node.AppendChild(element);
                    this.Node = element;
                    break;

                case XmlNodeTypeEx.AttributeValue:
                    // this.MyXmlReader.Node is current attrib
                    element = ((XmlAttribute)this.MyXmlReader.Node).OwnerElement;
                    this.Node.AppendChild(element);
                    this.Node = this.MyXmlReader.Node;
                    break;
            }

            return true;
        }

        private class MiniXmlReader : IDisposable
        {
            ////                                           name
            private const string PartialNamePattern = @"(?<name>[^\<\>\=\x22\/\s]*)$";

            private static readonly Regex PartialNameRegex = new Regex(MiniXmlReader.PartialNamePattern, RegexOptions.Compiled | RegexOptions.Singleline);

            ////                                             prefix            :     localname
            private const string PrefixedNamePattern = @"((?<prefix>[\w\.\-]+)\:)?(?<localname>[\w\.\-]+)";

            private static readonly Regex PrefixedNameRegex = new Regex(MiniXmlReader.PrefixedNamePattern, RegexOptions.Compiled | RegexOptions.Singleline);

            ////                                                            < /
            private static readonly Regex EndTagSignRegex = new Regex(@"\s*\<\/", RegexOptions.Compiled | RegexOptions.Singleline);

            ////                                                              <
            private static readonly Regex BeginTagSignRegex = new Regex(@"\s*\<(?!\!)", RegexOptions.Compiled | RegexOptions.Singleline);

            ////                                                                         prefixedname                =   "      value          "
            private static readonly Regex AttribRegex = new Regex(@"\s+" + MiniXmlReader.PrefixedNamePattern + @"\s*\=\s*\x22(?<value>[^\x22]*)\x22", RegexOptions.Compiled | RegexOptions.Singleline);

            private static readonly Regex PartialAttribNameRegex = new Regex(@"\s+(" + MiniXmlReader.PartialNamePattern + @")", RegexOptions.Compiled | RegexOptions.Singleline);

            ////                                                                                      prefixedname                 =   "      value
            private static readonly Regex PartialAttribValueRegex = new Regex(@"\s+(" + MiniXmlReader.PrefixedNamePattern + @")\s*\=\s*\x22(?<value>[^\x22]*)$", RegexOptions.Compiled | RegexOptions.Singleline);

            public MiniXmlReader(string text, int startPosition, XmlDocument doc, XmlNamespaceManager namespaceManager, XmlException e)
            {
                this.Text = text;
                this.Position = startPosition;
                this.Document = doc;
                this.NamespaceManager = namespaceManager;
                this.Exception = e;
            }

            public XmlNode Node { get; private set; }

            public XmlNodeTypeEx NodeTypeEx { get; private set; }

            public int TokenPosition { get; set; }

            private string Text { get; set; }

            private int Position { get; set; }

            private XmlDocument Document { get; set; }

            private XmlNamespaceManager NamespaceManager { get; set; }

            private XmlException Exception { get; set; }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool Read()
            {
                this.Node = null;
                this.NodeTypeEx = XmlNodeTypeEx.Other;

                if (this.Position >= this.Text.Length)
                {
                    return false;
                }

                Match match = MiniXmlReader.EndTagSignRegex.Match(this.Text, this.Position);
                if (match.Success && match.Index == this.Position)
                {
                    this.Position = match.Index + match.Length;

                    match = MiniXmlReader.PartialNameRegex.Match(this.Text, this.Position);
                    if (match.Success && match.Index == this.Position)
                    {
                        this.TokenPosition = this.Position;
                        this.Position = match.Index + match.Length;

                        this.NodeTypeEx = XmlNodeTypeEx.EndElement;
                        return true;
                    }

                    throw this.Exception;
                }

                match = MiniXmlReader.BeginTagSignRegex.Match(this.Text, this.Position);
                if (match.Success && match.Index == this.Position)
                {
                    this.Position = match.Index + match.Length;

                    return this.ReadBeginTag();
                }

                // the last node is likely text, which should be already read by the caller
                return false;
            }

            private bool ReadBeginTag()
            {
                XmlElement element;

                // try to match partial name
                Match match = MiniXmlReader.PartialNameRegex.Match(this.Text, this.Position);
                if (match.Success && match.Index == this.Position)
                {
                    this.TokenPosition = this.Position;
                    this.Position = match.Index + match.Length;

                    element = this.Document.CreateElement("dummy");

                    this.Node = element;
                    this.NodeTypeEx = XmlNodeTypeEx.BeginElement;
                    return true;
                }

                // try to match prefixed name
                match = MiniXmlReader.PrefixedNameRegex.Match(this.Text, this.Position);
                if (!match.Success || match.Index != this.Position)
                {
                    throw this.Exception;
                }

                this.Position = match.Index + match.Length;

                PrefixedName name = PrefixedName.FromRegexMatch(match);

                this.NamespaceManager.PushScope();

                // try to match attribs
                MatchCollection matches = MiniXmlReader.AttribRegex.Matches(this.Text, this.Position);

                // pass One -- lookup xmlns
                foreach (Match match2 in matches)
                {
                    PrefixedName name2 = PrefixedName.FromRegexMatch(match2);
                    string value2 = match2.Groups["value"].Value;
                    value2 = HttpUtility.HtmlDecode(value2);

                    if (string.IsNullOrEmpty(name2.Prefix) && "xmlns".Equals(name2.LocalName, StringComparison.Ordinal))
                    {
                        this.NamespaceManager.AddNamespace(string.Empty, value2);
                    }
                    else if ("xmlns".Equals(name2.Prefix, StringComparison.Ordinal))
                    {
                        this.NamespaceManager.AddNamespace(name2.LocalName, value2);
                    }
                }

                // create element within current namespace scope
                string namespaceURI = this.NamespaceManager.LookupNamespace(name.Prefix);
                element = this.Document.CreateElement(name.Prefix, name.LocalName, namespaceURI);
                this.Node = element;

                // pass Two -- add attribs
                foreach (Match match2 in matches)
                {
                    this.Position = match2.Index + match2.Length;

                    PrefixedName name2 = PrefixedName.FromRegexMatch(match2);
                    string value2 = match2.Groups["value"].Value;
                    value2 = HttpUtility.HtmlDecode(value2);
                    string namespaceURI2 = this.NamespaceManager.LookupNamespace(name2.Prefix);

                    XmlAttribute attrib = this.CreateXmlAttrib(name2.Prefix, name2.LocalName, namespaceURI2);
                    attrib.Value = match2.Groups["value"].Value;
                    element.Attributes.Append(attrib);
                }

                // try to match partial attrib name
                match = MiniXmlReader.PartialAttribNameRegex.Match(this.Text, this.Position);
                if (match.Success && match.Index == this.Position)
                {
                    this.TokenPosition = match.Groups["name"].Index;
                    this.Position = match.Index + match.Length;

                    XmlAttribute attrib = this.Document.CreateAttribute("dummy");
                    element.Attributes.Append(attrib);

                    this.Node = attrib;
                    this.NodeTypeEx = XmlNodeTypeEx.AttributeName;
                    return true;
                }

                // try to match partial attrib value
                match = MiniXmlReader.PartialAttribValueRegex.Match(this.Text, this.Position);
                if (match.Success && match.Index == this.Position)
                {
                    this.TokenPosition = match.Groups["value"].Index;
                    this.Position = match.Index + match.Length;

                    PrefixedName name2 = PrefixedName.FromRegexMatch(match);
                    string namespaceURI2 = this.NamespaceManager.LookupNamespace(name2.Prefix);

                    XmlAttribute attrib = this.CreateXmlAttrib(name2.Prefix, name2.LocalName, namespaceURI2);
                    attrib.Value = "dummy";
                    element.Attributes.Append(attrib);

                    this.Node = attrib;
                    this.NodeTypeEx = XmlNodeTypeEx.AttributeValue;
                    return true;
                }

                throw this.Exception;
            }

            private XmlAttribute CreateXmlAttrib(string prefix, string localName, string namespaceUri)
            {
                return string.IsNullOrEmpty(prefix)
                    ? this.Document.CreateAttribute(localName)
                    : this.Document.CreateAttribute(prefix, localName, namespaceUri);
            }
        }

        private class PrefixedName
        {
            public PrefixedName(string prefix, string localName)
            {
                this.Prefix = prefix;
                this.LocalName = localName;
            }

            protected PrefixedName()
            {
            }

            public string Prefix { get; set; }

            public string LocalName { get; set; }

            public static PrefixedName FromRegexMatch(Match match)
            {
                Fx.Assert(match.Success, "match failed");

                PrefixedName name = new PrefixedName
                {
                    Prefix = match.Groups["prefix"].Value,
                    LocalName = match.Groups["localname"].Value,
                };

                return name;
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(this.Prefix)
                    ? this.LocalName
                    : string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Prefix, this.LocalName);
            }
        }

        private class LineInformation
        {
            public LineInformation(string text)
            {
                this.LineEndPositions = new List<int>();

                using (StringReader reader = new StringReader(text))
                {
                    int sum = 0;
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        sum += line.Length + 1;
                        this.LineEndPositions.Add(sum);
                    }

                    if (sum == 0)
                    {
                        // totally empty
                        this.LineEndPositions.Add(0);
                    }
                }
            }

            private List<int> LineEndPositions { get; set; }    // both dimension based from 0, counting \n

            public int GetPosition(int lineNumber, int linePosition)
            {
                int lastLineEndPosition = lineNumber >= 2
                    ? this.LineEndPositions[lineNumber - 2]
                    : 0;

                return lastLineEndPosition + linePosition - 1;  // based from 0, counting \n
            }
        }
    }
}