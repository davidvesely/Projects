// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.IO;
    using System.Web.Script.Serialization;
    using System.Xml;
    using Microsoft.Server.Common;

    // for data contract only
    internal class FormattedJsonTextWriter : IDisposable
    {
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        internal FormattedJsonTextWriter(TextWriter writer, TolerantJsonTextReader reader, XmlDocument doc)
        {
            this.InnerWriter = writer;
            this.Settings = new JsonTextWriterSettings();
            this.Reader = reader;
            this.Document = doc;
        }

        private JsonTextWriterSettings Settings { get; set; }

        private TextWriter InnerWriter { get; set; }

        private TolerantJsonTextReader Reader { get; set; }

        private XmlDocument Document { get; set; }

        public void WriteDocument()
        {
            this.WriteElement(this.Document.DocumentElement, 0);
        }

        public void Dispose()
        {
            // nop, do not dispose InnerWriter
            GC.SuppressFinalize(this);
        }

        private void WriteElement(XmlElement element, int level)
        {
            bool isArrayElement = this.Reader.IsArrayElement(element);
            bool isObjectElement = this.Reader.IsObjectElement(element);
            bool hasName = !this.Reader.IsNameLess(element);

            // write start element
            if (hasName)
            {
                this.WriteIndention(level);
                this.InnerWriter.Write("\"{0}\":", this.EscapeText(element.Name));

                if (isObjectElement)
                {
                    this.InnerWriter.WriteLine();
                }
            }

            if (isArrayElement)
            {
                this.InnerWriter.WriteLine();
                this.WriteIndention(level);
                this.InnerWriter.WriteLine('[');
            }
            else if (isObjectElement)
            {
                this.WriteIndention(level);
                this.InnerWriter.WriteLine('{');
            }
            else if (!hasName)
            {
                this.WriteIndention(level);
            }

            // write children
            if (isArrayElement || isObjectElement)
            {
                foreach (XmlNode child in element.ChildNodes)
                {
                    Fx.Assert(child.NodeType == XmlNodeType.Element, "child is not element");
                    this.WriteElement(child as XmlElement, level + 1);

                    if (child.NextSibling != null)
                    {
                        this.InnerWriter.WriteLine(",");
                    }
                    else
                    {
                        this.InnerWriter.WriteLine();
                    }
                }
            }
            else
            {
                this.WriteElementText(element);
            }

            // write end element
            if (isArrayElement)
            {
                this.WriteIndention(level);
                this.InnerWriter.Write(']');
            }
            else if (isObjectElement)
            {
                this.WriteIndention(level);
                this.InnerWriter.Write('}');
            }
        }

        private void WriteElementText(XmlElement element)
        {
            if (element.ChildNodes.Count == 0)
            {
                this.InnerWriter.Write("null");
                return;
            }

            bool isElementTextQuoted = this.Reader.IsElementTextQuoted(element);
            if (isElementTextQuoted)
            {
                this.InnerWriter.Write('\"');

                foreach (XmlNode child in element.ChildNodes)
                {
                    Fx.Assert(child.NodeType == XmlNodeType.Text, "child is not text");
                    this.InnerWriter.Write(this.EscapeText(child.Value));
                }

                this.InnerWriter.Write('\"');
            }
            else
            {
                foreach (XmlNode child in element.ChildNodes)
                {
                    Fx.Assert(child.NodeType == XmlNodeType.Text, "child is not text");
                    this.InnerWriter.Write(child.Value);    // do not escape!
                }
            }
        }

        private void WriteIndention(int level)
        {
            if (this.Settings.Indent)
            {
                for (int i = 0; i < level; i++)
                {
                    this.InnerWriter.Write(this.Settings.IndentChars);
                }
            }
        }

        private string EscapeText(string src)
        {
            string dst = this.serializer.Serialize(src);
            dst = dst.Substring(1, dst.Length - 2);    // Trim leading and trailing "
            
            if ("/".Equals(dst, StringComparison.Ordinal))
            {
                // TolerantJsonTextReader will receive 3 XmlNodeType.Text from XmlJsonReader.Read() on a JSON date string:
                // 1st /, 2nd Date(...), 3rd /
                // so let's convert / back to \/, which has no negative impact but helps to restore the JSON date string
                // see http://msdn.microsoft.com/en-us/library/bb299886.aspx#intro%5Fto%5Fjson%5Ftopic5
                dst = @"\/";   
            }

            return dst;
        }

        private class JsonTextWriterSettings
        {
            public JsonTextWriterSettings()
            {
                this.Indent = true;
                this.IndentChars = "  ";
            }

            public bool Indent { get; set; }

            public string IndentChars { get; set; }
        }
    }
}
