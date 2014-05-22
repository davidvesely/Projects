// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;
    using System.Xml;
    using Microsoft.Runtime.Serialization;
    using Microsoft.Server.Common;

    internal class TolerantJsonTextReader : IDisposable, ITolerantTextReader
    {   
        // pattern                                                          [
        private static readonly Regex ArrayBeginSignRegex = new Regex(@"\s*\[", RegexOptions.Compiled | RegexOptions.Singleline);

        // pattern                                                           {
        private static readonly Regex ObjectBeginSignRegex = new Regex(@"\s*\{", RegexOptions.Compiled | RegexOptions.Singleline);

        // pattern                                                        ]
        private static readonly Regex ArrayEndSignRegex = new Regex(@"\s*\]", RegexOptions.Compiled | RegexOptions.Singleline);

        // pattern                                                         }
        private static readonly Regex ObjectEndSignRegex = new Regex(@"\s*\}", RegexOptions.Compiled | RegexOptions.Singleline);

        // pattern                                                       ,
        private static readonly Regex SiblingSignRegex = new Regex(@"\s*\,", RegexOptions.Compiled | RegexOptions.Singleline);

        // pattern                                                      "           "
        private static readonly Regex QuotedTextRegex = new Regex(@"\s*\x22[^\x22]*\x22", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex NonQuotedTextRegex = new Regex(@"\s*(?!\x22)[^\,\]\}]*", RegexOptions.Compiled | RegexOptions.Singleline);

        // for autocomplete
        public TolerantJsonTextReader(string text, int stopAtPosition, Type schema)
        {
            Fx.Assert(schema != null, "schema should not be null");
            this.Initialize(text, stopAtPosition, schema);
        }

        // For formatting
        public TolerantJsonTextReader(string text)
        {
            this.Initialize(text, -1, null);
        }

        private enum JsonNodeTypeEx
        {
            Other = 1000,
            Name,
            Value,
        }

        public bool EOF
        {
            get
            {
                return this.Position >= this.StopAtPosition;
            }
        }

        public string RemainingText
        {
            get
            {
                return this.Text.Substring(this.Position, this.StopAtPosition - this.Position);
            }
        }

        public Exception Exception { get; private set; }    // unable to autocomplete / format
              
        public Exception Warning { get; set; }      // unable to format

        public bool IsFormattable
        {
            get { return this.Exception == null && this.Warning == null; }
        }

        private string Text { get; set; }

        private int StopAtPosition { get; set; }

        private int Position { get; set; }

        private int TokenPosition { get; set; } // where replacement should start, absolute index

        private Type Schema { get; set; }

        private MemoryStream MemoryStream { get; set; }

        private JsonTextReader JsonReader { get; set; }

        private XmlDocument Document { get; set; }

        private XmlNode Node { get; set; }

        private JsonNodeTypeEx NodeTypeEx { get; set; }

        private Dictionary<XmlNode, JsonNodeExtendedInfo> JsonNodeExtendedInfos { get; set; }

        public bool Read()
        {
            if (this.JsonReader != null)
            {
                try
                {
                    return this.ReadWithJsonTextReader();
                }
                catch (XmlException e)
                {
                    if (!this.OnXmlJsonReaderException(e))
                    {
                        throw;
                    }

                    // fall through
                }
                catch (FormatException e)
                {
                    if (!this.OnXmlJsonReaderException(e))
                    {
                        throw;
                    }

                    // fall through
                }
            }

            return false;
        }

        public IEnumerable<string> GetExpectedItems()
        {
            Fx.Assert(this.Schema != null, "Cannot get expected items when schema is null");

            JsonTypeSchema schema = new JsonTypeSchema(this.Schema);
            JsonSchemaValidator validator = new JsonSchemaValidator(schema, this.TokenPosition);

            LinkedList<XmlElement> elements = new LinkedList<XmlElement>();
            for (XmlElement element = this.Node.ParentNode as XmlElement; element != null; element = element.ParentNode as XmlElement)
            {
                elements.AddFirst(element);
            }

            switch (this.NodeTypeEx)
            {
                case JsonNodeTypeEx.Name:
                    // this.Node is dummy element
                    foreach (XmlElement element in elements)
                    {
                        validator.ValidateElement(element);
                    }

                    foreach (XmlNode child in this.Node.ParentNode.ChildNodes)
                    {
                        if (child == this.Node)
                        {
                            break;
                        }

                        if (child.NodeType != XmlNodeType.Element)
                        {
                            continue;
                        }

                        XmlElement element = child as XmlElement;
                        validator.ValidateElement(element);
                        validator.ValidateEndElement();
                    }

                    return validator.GetExpectedNodeNames();

                case JsonNodeTypeEx.Value:
                    // this.Node is current element
                    foreach (XmlElement element in elements)
                    {
                        validator.ValidateElement(element);
                    }

                    validator.ValidateElement(this.Node as XmlElement);
                    return validator.GetExpectedNodeValues();

                default:
                    return new string[0];
            }
        }

        public void Dispose()
        {
            if (this.MemoryStream != null)
            {
                this.MemoryStream.Dispose();
                this.MemoryStream = null;
            }

            if (this.JsonReader != null)
            {
                this.JsonReader.Dispose();
                this.JsonReader = null;
            }

            GC.SuppressFinalize(this);
        }

        public FormattedJsonTextWriter CreateWriter(TextWriter writer)
        {
            Fx.Assert(this.EOF, "Cannot create writer when the reader is still reading the json document.");
            return new FormattedJsonTextWriter(writer, this, this.Document);
        }

        internal bool IsArrayElement(XmlNode node)
        {
            return this.GetExtendedInfo(node).IsArrayElement;
        }

        internal bool IsElementTextQuoted(XmlNode node)
        {
            return this.GetExtendedInfo(node).IsElementTextQuoted;
        }

        internal bool IsObjectElement(XmlNode node)
        {
            return this.GetExtendedInfo(node).IsObjectElement;
        }

        internal bool IsNameLess(XmlNode node)
        {
            return this.GetExtendedInfo(node).IsNameless;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design")]
        private bool ReadWithJsonTextReader()
        {
            if (this.EOF)
            {
                this.JsonReader.Dispose();
                this.JsonReader = null;

                if (this.Node != this.Document && this.Exception == null && this.Position >= this.Text.Length)
                {
                    this.Warning = Fx.Exception.AsError(new XmlException(Http.SR.UnexpectedEOF)) as XmlException;
                }

                return false;
            }

            if (!this.JsonReader.Read())
            {
                this.JsonReader.Dispose();
                this.JsonReader = null;

                return false;
            }

            int oldPosition = this.Position;

            this.NodeTypeEx = JsonNodeTypeEx.Other;

            switch (this.JsonReader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        XmlNode newNode = this.Document.CreateElement(this.JsonReader.Name);
                        this.Node.AppendChild(newNode);
                        this.Node = newNode;

                        JsonNodeExtendedInfo info = this.GetExtendedInfo(newNode);
                        if (this.Node != this.Document.DocumentElement && !this.GetExtendedInfo(newNode.ParentNode).IsArrayElement)
                        {
                            info.IsNameless = false;

                            int pos = this.Text.IndexOf('\"', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                break;
                            }

                            this.Position = pos + 1;
                            this.TokenPosition = this.Position;
                            pos = this.Text.IndexOf('\"', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                this.Position = this.StopAtPosition;

                                this.NodeTypeEx = JsonNodeTypeEx.Name;
                                break;
                            }

                            this.Position = pos + 1;
                            pos = this.Text.IndexOf(':', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                break;
                            }

                            this.Position = pos + 1;
                            this.TokenPosition = this.Position;
                        }

                        Match match = TolerantJsonTextReader.ObjectBeginSignRegex.Match(this.Text, this.Position);
                        if (match.Success && match.Index == this.Position)
                        {
                            this.Position = match.Index + match.Length;

                            info.IsObjectElement = true;
                        }
                        else
                        {
                            this.NodeTypeEx = JsonNodeTypeEx.Value;

                            match = TolerantJsonTextReader.ArrayBeginSignRegex.Match(this.Text, this.Position);
                            if (match.Success && match.Index == this.Position)
                            {
                                this.Position = match.Index + match.Length;

                                info.IsArrayElement = true;
                            }
                        }
                    }

                    break;

                case XmlNodeType.EndElement:
                    {
                        JsonNodeExtendedInfo info = this.GetExtendedInfo(this.Node);
                        Match match;
                        if (info.IsObjectElement)
                        {
                            match = TolerantJsonTextReader.ObjectEndSignRegex.Match(this.Text, this.Position);
                            if (!match.Success || match.Index != this.Position)
                            {
                                // XmlJsonReader reported a non-existent EndElement!
                                throw Fx.Exception.AsError(new XmlException(Http.SR.UnexpectedEOF));
                            }

                            this.Position = match.Index + match.Length;
                        }
                        else if (info.IsArrayElement)
                        {
                            match = TolerantJsonTextReader.ArrayEndSignRegex.Match(this.Text, this.Position);
                            if (!match.Success || match.Index != this.Position)
                            {
                                // XmlJsonReader reported a non-existent EndElement!
                                throw Fx.Exception.AsError(new XmlException(Http.SR.UnexpectedEOF));
                            }

                            this.Position = match.Index + match.Length;
                        }
                        else if (this.Node.ChildNodes.Count < 1)
                        {
                            int pos = this.Text.IndexOf("null", this.Position, this.StopAtPosition - this.Position, StringComparison.Ordinal);
                            if (pos < 0)
                            {
                                // XmlJsonReader reported a non-existent EndElement!
                                throw Fx.Exception.AsError(new XmlException(Http.SR.UnexpectedEOF));
                            }

                            this.Position = pos + "null".Length;
                        }

                        this.Node = this.Node.ParentNode;

                        match = TolerantJsonTextReader.SiblingSignRegex.Match(this.Text, this.Position);
                        if (match.Success && match.Index == this.Position)
                        {
                            this.Position = match.Index + match.Length;
                            this.TokenPosition = this.Position;

                            info = this.GetExtendedInfo(this.Node);
                            if (info.IsObjectElement)
                            {
                                this.NodeTypeEx = JsonNodeTypeEx.Name;
                            }
                            else if (info.IsArrayElement)
                            {
                                this.NodeTypeEx = JsonNodeTypeEx.Value;
                            }
                        }
                    }

                    break;

                case XmlNodeType.Text:
                    {
                        XmlNode newNode = this.Document.CreateTextNode(this.JsonReader.Value);
                        this.Node.AppendChild(newNode);

                        this.NodeTypeEx = JsonNodeTypeEx.Value;

                        JsonNodeExtendedInfo info = this.GetExtendedInfo(this.Node);

                        Match match = TolerantJsonTextReader.QuotedTextRegex.Match(this.Text, this.Position);
                        if (match.Success && match.Index == this.Position)
                        {
                            this.Position = match.Index + match.Length;

                            info.IsElementTextQuoted = true;
                        }
                        else
                        {
                            match = TolerantJsonTextReader.NonQuotedTextRegex.Match(this.Text, this.Position);
                            if (match.Success && match.Index == this.Position)
                            {
                                this.Position = match.Index + match.Length;
                            }
                            else
                            {
                                throw Fx.Exception.AsError(new XmlException(Http.SR.UnexpectedEOF));
                            }
                        }
                    }

                    break;
            }

            Debug.WriteLine(this.Text.Substring(oldPosition, this.Position - oldPosition));

            return true;
        }

        private bool OnXmlJsonReaderException(Exception e)
        {
            this.Warning = e;

            bool handled = false;

            switch (this.JsonReader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        XmlNode newNode;

                        JsonNodeExtendedInfo info = this.GetExtendedInfo(this.Node);
                        if (this.Node != this.Document && !info.IsArrayElement)
                        {
                            info.IsNameless = false;

                            int pos = this.Text.IndexOf('\"', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                break;
                            }

                            this.Position = pos + 1;
                            this.TokenPosition = this.Position;
                            pos = this.Text.IndexOf('\"', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                this.Position = this.StopAtPosition;

                                newNode = this.Document.CreateElement("dummy");
                                this.Node.AppendChild(newNode);

                                this.Node = newNode;
                                this.NodeTypeEx = JsonNodeTypeEx.Name;

                                handled = true;
                                break;
                            }

                            this.Position = pos + 1;
                            pos = this.Text.IndexOf(':', this.Position, this.StopAtPosition - this.Position);
                            if (pos < 0)
                            {
                                break;
                            }

                            this.Position = pos + 1;
                            this.TokenPosition = this.Position;
                        }

                        newNode = this.Document.CreateElement(this.JsonReader.Name);
                        this.Node.AppendChild(newNode);

                        this.Node = newNode;
                        this.NodeTypeEx = JsonNodeTypeEx.Value;

                        handled = true;
                    }

                    break;

                case XmlNodeType.Text:
                    {
                        XmlNode newNode = this.Document.CreateTextNode("dummy");
                        this.Node.AppendChild(newNode);

                        this.NodeTypeEx = JsonNodeTypeEx.Value;
                    }

                    handled = true;
                    break;

                default:
                    this.Exception = e;
                    break;
            }

            this.JsonReader.Dispose();
            this.JsonReader = null;
            
            return handled;
        }

        private JsonNodeExtendedInfo GetExtendedInfo(XmlNode node)
        {
            JsonNodeExtendedInfo info;
            if (!this.JsonNodeExtendedInfos.TryGetValue(node, out info))
            {
                info = new JsonNodeExtendedInfo();
                this.JsonNodeExtendedInfos.Add(node, info);
            }

            return info;
        }

        private void Initialize(string text, int stopAtPosition, Type schema)
        {
            this.Text = text.TrimEnd();
            this.StopAtPosition = stopAtPosition >= 0 && stopAtPosition <= this.Text.Length
                ? stopAtPosition
                : this.Text.Length;
            this.Position = 0;
            this.TokenPosition = 0;

            this.Schema = schema;

            this.MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            this.JsonReader = new JsonTextReader(this.MemoryStream);

            this.Document = new XmlDocument();
            this.Node = this.Document;
            this.NodeTypeEx = JsonNodeTypeEx.Other;

            this.JsonNodeExtendedInfos = new Dictionary<XmlNode, JsonNodeExtendedInfo>();
        }

        private class JsonTextReader : IDisposable
        {
            private static Assembly asmSystemRuntimeSerialization = Assembly.Load("System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL");

            private static Type typeXmlJsonReader = JsonTextReader.asmSystemRuntimeSerialization.GetType("System.Runtime.Serialization.Json.XmlJsonReader");

            private static MethodInfo methodSetInput = JsonTextReader.typeXmlJsonReader.GetMethod(
                    "SetInput",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(Stream), typeof(Encoding), typeof(XmlDictionaryReaderQuotas), typeof(OnXmlDictionaryReaderClose) },
                    null);

            public JsonTextReader(Stream stream)
            {
                this.Reader = Activator.CreateInstance(JsonTextReader.typeXmlJsonReader);

                JsonTextReader.methodSetInput.Invoke(this.Reader, new object[] { stream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, (OnXmlDictionaryReaderClose)null });
            }

            public string Name
            {
                get
                {
                    return this.Reader.LocalName;
                }
            }

            public XmlNodeType NodeType
            {
                get
                {
                    return this.Reader.NodeType;
                }
            }

            public string Value
            {
                get
                {
                    return this.Reader.Value;
                }
            }

            private dynamic Reader { get; set; }    // System.Runtime.Serialization.Json.XmlJsonReader

            public bool Read()
            {
                bool result = this.Reader.Read();

                Debug.WriteLine("{0} | {1} | {2}", this.NodeType, this.Name, this.Value);

                return result;
            }

            public void Dispose()
            {
                ((IDisposable)this.Reader).Dispose();
                this.Reader = null;
                GC.SuppressFinalize(this);
            }
        }

        private class JsonSchemaValidator
        {
            public JsonSchemaValidator(JsonTypeSchema schema, int tokenPosition)
            {
                this.RootSchema = schema;
                this.ValidationStack = new LinkedList<ValidationState>();
                this.TokenPosition = tokenPosition;
            }

            private JsonTypeSchema RootSchema { get; set; }

            private LinkedList<ValidationState> ValidationStack { get; set; }

            private int TokenPosition { get; set; }

            public void ValidateElement(XmlElement element)
            {
                ValidationState elementState;
                if (this.ValidationStack.Count < 1)
                {
                    elementState = new ValidationState(this.RootSchema, element);
                    this.ValidationStack.AddLast(elementState);
                    return;
                }

                ValidationState containerState = this.ValidationStack.Last.Value;
                JsonTypeSchema containerSchema = containerState.Schema;
                JsonTypeSchema elementSchema = null;

                if (containerSchema != null)
                {
                    if (containerSchema.IsArray)
                    {
                        elementSchema = containerSchema.ElementSchema;
                    }
                    else
                    {
                        DataMember member = containerSchema.GetMember(element.Name);
                        if (member == null)
                        {
                            // invalid element
                            Debug.WriteLine("unrecognized element {0}", element.Name);
                        }
                        else
                        {
                            elementSchema = new JsonTypeSchema(member.MemberTypeContract);
                            containerState.PresentChildren.Add(element.Name);
                        }
                    }
                }

                elementState = new ValidationState(elementSchema, element);
                this.ValidationStack.AddLast(elementState);
            }

            public void ValidateEndElement()
            {
                if (this.ValidationStack.Count < 1)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(Http.SR.EmptyValidationStack));
                }

                this.ValidationStack.RemoveLast();
            }

            public IEnumerable<string> GetExpectedNodeNames()
            {
                if (this.ValidationStack.Count < 1)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(Http.SR.EmptyValidationStack));
                }

                ValidationState state = this.ValidationStack.Last.Value;
                foreach (DataMember member in state.Schema.Members)
                {
                    if (!state.PresentChildren.Contains(member.MemberInfo.Name))
                    {
                        string displayName = member.MemberInfo.Name;
                        string value = displayName;
                        yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                    }
                }
            }

            public IEnumerable<string> GetExpectedNodeValues()
            {
                if (this.ValidationStack.Count < 1)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(Http.SR.EmptyValidationStack));
                }

                ValidationState state = this.ValidationStack.Last.Value;
                JsonTypeSchema schema = state.Schema;

                if (schema.IsArray)
                {
                    schema = schema.ElementSchema;
                }

                if (schema.IsEnum)
                {
                    foreach (DataMember member in schema.Members)
                    {
                        string displayName = member.MemberInfo.Name;
                        object enumValue = (member.MemberInfo as FieldInfo).GetValue(null);
                        string value = ((int)enumValue).ToString();
                        yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                    }
                }
                else if (schema.IsBool)
                {
                    yield return TolerantTextReaderHelper.GetExpectedValue("true", "true", this.TokenPosition);
                    yield return TolerantTextReaderHelper.GetExpectedValue("false", "false", this.TokenPosition);
                }
                else if (schema.IsDateTime)
                {
                    DateTime now = DateTime.Now;
                    string displayName = XmlConvert.ToString(now, XmlDateTimeSerializationMode.RoundtripKind);

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string value = serializer.Serialize(now);   // keep leading and trailing "
                  
                    yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                }
            }

            private class ValidationState
            {
                public ValidationState(JsonTypeSchema schema, XmlElement element)
                {
                    this.Schema = schema;
                    this.Element = element;
                    this.PresentChildren = new HashSet<string>();
                }

                public JsonTypeSchema Schema { get; set; }

                [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Let's keep it for debugging")]
                public XmlElement Element { get; set; }

                public HashSet<string> PresentChildren { get; set; }
            }
        }

        private class JsonTypeSchema
        {
            // <member name, member>
            private Dictionary<string, DataMember> members;

            public JsonTypeSchema(Type type)
            {
                this.DataContract = DataContract.GetDataContract(type);
            }

            public JsonTypeSchema(DataContract dataContract)
            {
                this.DataContract = dataContract;
            }

            public IEnumerable<DataMember> Members
            {
                get
                {
                    if (this.members == null)
                    {
                        this.GetMembers();
                    }

                    return this.members.Values;
                }
            }

            public JsonTypeSchema ElementSchema
            {
                get
                {
                    CollectionDataContract collectionDataContract = this.DataContract as CollectionDataContract;
                    Fx.Assert(collectionDataContract != null, "current DataContract type does not have property 'ItemContract'");
                    DataContract itemContract = collectionDataContract.ItemContract;
                    return new JsonTypeSchema(itemContract);
                }
            }

            public bool IsArray
            {
                get
                {
                    return this.DataContract is CollectionDataContract;
                }
            }

            public bool IsEnum
            {
                get
                {
                    return this.DataContract is EnumDataContract;
                }
            }

            public bool IsBool
            {
                get
                {
                    return this.DataContract is BooleanDataContract;
                }
            }

            public bool IsDateTime
            {
                get
                {
                    return this.DataContract is DateTimeDataContract;
                }
            }

            private DataContract DataContract { get; set; }

            public DataMember GetMember(string name)
            {
                if (this.members == null)
                {
                    this.GetMembers();
                }

                DataMember member = null;
                this.members.TryGetValue(name, out member);
                return member;
            }

            private void GetMembers()
            {
                List<DataMember> list = null;

                ClassDataContract classDataContract = this.DataContract as ClassDataContract;
                if (classDataContract != null)
                {
                    list = classDataContract.Members;
                }
                else
                {
                    EnumDataContract enumDataContract = this.DataContract as EnumDataContract;
                    if (enumDataContract != null)
                    {
                        list = enumDataContract.Members;
                    }
                    else
                    {
                        list = new List<DataMember>(0);
                    }
                }

                this.members = new Dictionary<string, DataMember>(list.Count);
                foreach (DataMember member in list)
                {
                    this.members.Add(member.MemberInfo.Name, member);
                }
            }
        }

        private class JsonNodeExtendedInfo
        {
            public JsonNodeExtendedInfo()
            {
                this.IsNameless = true;
                this.IsArrayElement = false;
                this.IsElementTextQuoted = false;
                this.IsObjectElement = false;
            }

            public bool IsNameless { get; set; }

            public bool IsArrayElement { get; set; }

            public bool IsElementTextQuoted { get; set; }

            public bool IsObjectElement { get; set; }
        }
    }
}
