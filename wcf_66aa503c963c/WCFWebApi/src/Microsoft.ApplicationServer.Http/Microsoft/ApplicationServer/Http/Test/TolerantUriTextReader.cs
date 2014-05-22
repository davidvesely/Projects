// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.Server.Common;
    
    // todo: escape text
    internal class TolerantUriTextReader : IDisposable, ITolerantTextReader
    {
        private static readonly Uri PseudoBaseUri = new Uri("http://host/");

        ////                                                      { *    name         }
        private static readonly Regex VarNameRegex = new Regex(@"\{\*?(?<name>[^\}]+)\}", RegexOptions.Compiled | RegexOptions.Singleline);

        public TolerantUriTextReader(string uriString, int stopAtPosition, Uri baseUri, UriTemplate uriTemplate, HttpOperationDescription operation)
        {
            this.UriString = uriString.TrimEnd();
            this.StopAtPosition = stopAtPosition >= 0 && stopAtPosition <= this.UriString.Length
                ? stopAtPosition
                : this.UriString.Length;
            this.Position = 0;
            this.TokenPosition = 0;

            this.UriString = this.UriString.Substring(0, this.StopAtPosition);

            this.BaseUri = baseUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
                ? baseUri
                : new Uri(baseUri.AbsoluteUri + "/");
            this.UriTemplate = uriTemplate;
            this.OpDesc = operation;

            this.UriTemplateSegments = null;
            this.IndexInUriTemplateSegments = -1;
            this.UriTemplateQueryVars = null;

            this.NodeName = string.Empty;
            this.NodeType = UriNodeType.BaseUri;
        }

        private enum UriNodeType
        {
            Other = 1000,
            BaseUri,                // e.g. http://xyz|
            EndOfBaseUri,
            StaticSegment,          // e.g. template = /abc, uri = /xyz|
            StaticSegment2,         // e.g. template = /abc{x}, uri = /ab|
            DynamicSegment,         // e.g. template = /{x}, uri = /xyz|
            EndOfSegments,
            QueryVarName,           // e.g. ?xyz|
            StaticQueryVarValue,    // e.g. template = ?abc=def, uri = ?abc=xyz|
            DynamicQueryVarValue,   // e.g. template = ?abc={x}, uri = ?abc=xyz|
            EndOfQueryString,
        }

        public Exception Exception { get; private set; }

        private string UriString { get; set; }

        private int StopAtPosition { get; set; }

        private int Position { get; set; }

        private int TokenPosition { get; set; } // where replacement should start, absolute index

        private bool EOF { get; set; }  // manually set, do not rely on Position vs. StopAtPosition

        private Uri BaseUri { get; set; }

        private UriTemplate UriTemplate { get; set; }

        private IList<string> UriTemplateSegments { get; set; }

        private int IndexInUriTemplateSegments { get; set; }

        private Dictionary<string, string> UriTemplateQueryVars { get; set; }

        private HttpOperationDescription OpDesc { get; set; }

        private string NodeName { get; set; }

        private UriNodeType NodeType { get; set; }

        public void Dispose()
        {
            // nop
            GC.SuppressFinalize(this);
        }

        public bool Read()
        {
            if (this.EOF)
            {
                return false;
            }

            int oldPosition = this.Position;
            int pos;
            string baseUriString = this.BaseUri.ToString();

            switch (this.NodeType)
            {
                case UriNodeType.BaseUri:
                    if (!this.UriString.StartsWith(baseUriString, StringComparison.OrdinalIgnoreCase))
                    {
                        this.EOF = true;

                        if (!baseUriString.StartsWith(this.UriString, StringComparison.OrdinalIgnoreCase))
                        {
                            this.Exception = Fx.Exception.AsError(new UriReaderException(SR.UriMustStartWithBaseUri)
                            {
                                Position = 0,
                            });

                            this.NodeType = UriNodeType.Other;
                            break;
                        }

                        // user is typing the uri from the beginning
                        this.TokenPosition = 0;
                        break;
                    }

                    pos = this.UriString.IndexOfAny(new char[] { '\r', '\n' });
                    if (pos >= 0)
                    {
                        this.EOF = true;
                        this.Exception = Fx.Exception.AsError(new UriReaderException(SR.UriCannotBeMultipleLines)
                        {
                            Position = pos,
                        });

                        this.NodeType = UriNodeType.Other;
                        break;
                    }

                    this.Position = baseUriString.Length;
                    this.TokenPosition = this.Position;

                    if (!baseUriString.EndsWith("/", StringComparison.Ordinal))
                    {
                        // expect '/'
                        if (this.Position >= this.StopAtPosition)
                        {
                            this.EOF = true;
                            this.NodeType = UriNodeType.Other;
                            break;
                        }

                        if (this.UriString[this.Position] != '/')
                        {
                            this.Exception = Fx.Exception.AsError(new UriReaderException(SR.UriMustStartWithBaseUri)
                            {
                                Position = 0,
                            });

                            this.NodeType = UriNodeType.Other;
                            break;
                        }

                        this.Position++;
                        this.TokenPosition = this.Position;
                    }

                    this.NodeType = UriNodeType.EndOfBaseUri;
                    break;

                case UriNodeType.EndOfBaseUri:
                    if (this.UriTemplateSegments == null)
                    {
                        this.InitUriTemplateSegments();
                        this.IndexInUriTemplateSegments = -1;
                    }

                    if (++this.IndexInUriTemplateSegments >= this.UriTemplateSegments.Count)
                    {
                        this.EOF = true;
                        this.Exception = Fx.Exception.AsError(new UriReaderException(SR.UriCannotHaveMoreSegmentsThanTemplate)
                        {
                            Position = this.Position,
                        });

                        this.NodeType = UriNodeType.Other;
                        break;
                    }

                    pos = this.UriString.IndexOfAny(new char[] { '?', '/', '#' }, this.Position);
                    if (pos < 0)
                    {
                        this.EOF = true;

                        this.NodeName = this.UriTemplateSegments[this.IndexInUriTemplateSegments];

                        this.ReadDynamicSegmentOrQueryVarValue(true);
                        break;
                    }

                    if (!this.MatchSegment(this.UriString.Substring(this.Position, pos - this.Position)))
                    {
                        this.EOF = true;
                        this.Exception = Fx.Exception.AsError(new UriReaderException(SR.UriSegmentMismatch)
                        {
                            Position = this.Position,
                        });

                        this.NodeType = UriNodeType.Other;
                        break;
                    }

                    this.Position = pos + 1;
                    this.TokenPosition = this.Position;

                    if (this.UriString[pos] == '?')
                    {
                        this.NodeType = UriNodeType.EndOfSegments;
                        break;
                    }
                    else if (this.UriString[pos] == '#')
                    {
                        this.NodeType = UriNodeType.EndOfQueryString;
                        break;
                    }

                    break;

                case UriNodeType.EndOfSegments:
                    if (this.UriTemplateQueryVars == null)
                    {
                        this.InitUriTemplateQueryVars();
                    }

                    pos = this.UriString.IndexOfAny(new char[] { '&', '#' }, this.Position);
                    if (pos < 0)
                    {
                        this.EOF = true;

                        pos = this.UriString.IndexOf('=', this.Position);
                        if (pos < 0)
                        {
                            this.NodeType = UriNodeType.QueryVarName;
                            break;
                        }

                        string varName = this.UriString.Substring(this.Position, pos - this.Position).Trim();
                        this.NodeName = this.UriTemplateQueryVars[varName];

                        this.Position = pos + 1;
                        this.TokenPosition = this.Position;

                        this.ReadDynamicSegmentOrQueryVarValue(false);
                        break;
                    }

                    this.Position = pos + 1;
                    this.TokenPosition = this.Position;
                    break;

                default:
                    this.EOF = true;
                    break;
            }

            Debug.WriteLine("{0} | {1}", this.NodeType, this.NodeName);
            Debug.WriteLine(this.UriString.Substring(oldPosition, this.Position - oldPosition));

            return true;
        }

        public IEnumerable<string> GetExpectedItems()
        {
            string displayName;
            string value;

            switch (this.NodeType)
            {
                case UriNodeType.BaseUri:
                    displayName = this.BaseUri.AbsoluteUri;
                    value = displayName;
                    yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                    break;

                case UriNodeType.StaticSegment:         // this.NodeName is the static segment
                case UriNodeType.StaticSegment2:        // this.NodeName is the static segment
                case UriNodeType.StaticQueryVarValue:   // this.NodeName is the static var value
                    displayName = this.NodeName;
                    value = displayName;    // do not escape!
                    if (this.NodeType == UriNodeType.StaticSegment2)
                    {
                        yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition, 0);  // force another autocomplete
                    }
                    else
                    {
                        yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                    }

                    break;

                case UriNodeType.DynamicSegment:        // this.NodeName is the var name
                case UriNodeType.DynamicQueryVarValue:  // this.NodeName is the var name                    
                    {
                        HttpParameter param = this.GetHttpParameterByName(this.NodeName);
                        if (param != null)
                        {
                            foreach (string valueI in TolerantUriTextReader.GetExpectedValuesByType(param.ParameterType))
                            {
                                displayName = valueI;
                                value = Uri.EscapeDataString(displayName);
                                yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                            }
                        }
                    }

                    break;

                case UriNodeType.QueryVarName:
                    foreach (string varName in this.UriTemplateQueryVars.Keys)
                    {
                        displayName = varName;
                        value = displayName;    // do not escape!
                        yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
                    }

                    break;

                default:
                    break;
            }
        }

        private static IEnumerable<string> GetExpectedValuesByType(Type type)
        {
            if (type.IsEnum)
            {
                return type.GetEnumNames();
            }
            else if (typeof(bool).Equals(type))
            {
                return new string[] { "true", "false" };
            }
            else
            {
                return TolerantTextReaderHelper.EmptyStringArray;
            }
        }

        private void ReadDynamicSegmentOrQueryVarValue(bool isSegment)
        {
            MatchCollection matches = TolerantUriTextReader.VarNameRegex.Matches(this.NodeName);
            if (matches.Count == 0)
            {
                this.NodeType = isSegment ? UriNodeType.StaticSegment : UriNodeType.StaticQueryVarValue;
                return;
            }
            else if (matches.Count == 1)
            {
                if (matches[0].Index == 0)
                {
                    // e.g. /{xyz}/, /12|
                    // e.g. /{xyz}def/, /12def|
                    this.NodeName = matches[0].Groups["name"].Value;
                    this.NodeType = isSegment ? UriNodeType.DynamicSegment : UriNodeType.DynamicQueryVarValue;
                    return;
                }
                else if (this.Position + matches[0].Index > this.StopAtPosition)
                {
                    // e.g. /abc{xyz}/, /ab|
                    Fx.Assert(isSegment, "query var cannot have compound structure");

                    this.NodeName = this.NodeName.Substring(0, matches[0].Index);
                    this.NodeType = UriNodeType.StaticSegment2;
                    return;
                }
                else
                {
                    // e.g. /abc{xyz}/, /abc12|
                    // e.g. /abc{xyz}/, /abc|
                    Fx.Assert(isSegment, "query var cannot have compound structure");

                    this.TokenPosition += matches[0].Index;
                    this.NodeName = matches[0].Groups["name"].Value;
                    this.NodeType = UriNodeType.DynamicSegment;
                    return;
                }
            }
            else
            {
                this.Exception = Fx.Exception.AsError(new UriReaderException(SR.MultipleVariablesInSingleSegmentOrQueryValue)
                {
                    Position = this.Position,
                });

                this.NodeType = UriNodeType.Other;
                return;
            }
        }

        private void InitUriTemplateQueryVars()
        {
            this.UriTemplateQueryVars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string uriTemplateString = this.UriTemplate.ToString();
            int head = uriTemplateString.IndexOf('?');
            if (head < 0)
            {
                return;
            }

            string uriTemplateQueryString;
            int tail = uriTemplateString.IndexOf('#', head + 1);
            if (tail >= 0)
            {
                uriTemplateQueryString = uriTemplateString.Substring(head + 1, tail - head - 1);
            }
            else
            {
                uriTemplateQueryString = uriTemplateString.Substring(head + 1);
            }

            string[] buf = uriTemplateQueryString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in buf)
            {
                string[] buf2 = item.Split(new char[] { '=' }, 2);
                string varName = buf2[0].Trim();
                string varValue = buf2.Length == 2 ? buf2[1] : string.Empty;
                this.UriTemplateQueryVars[varName] = varValue;
            }
        }

        private bool MatchSegment(string segment)
        {
            string uriTemplateSegment = this.UriTemplateSegments[this.IndexInUriTemplateSegments];
            UriTemplate pseudoUriTemplate = new UriTemplate(uriTemplateSegment);

            Uri pseudoUri = new Uri(TolerantUriTextReader.PseudoBaseUri, segment);
            UriTemplateMatch match = pseudoUriTemplate.Match(TolerantUriTextReader.PseudoBaseUri, pseudoUri);
            return match != null;
        }

        private void InitUriTemplateSegments()
        {
            string uriTemplateString = this.UriTemplate.ToString().TrimStart('/');
            string uriTemplateSegmentsString;   // cut off query string

            int tail = uriTemplateString.IndexOf('?');
            if (tail >= 0)
            {
                uriTemplateSegmentsString = uriTemplateString.Substring(0, tail);
            }
            else
            {
                uriTemplateSegmentsString = uriTemplateString;
            }

            this.UriTemplateSegments = uriTemplateSegmentsString.Split('/');
        }

        private HttpParameter GetHttpParameterByName(string name)
        {
            foreach (HttpParameter param in this.OpDesc.InputParameters)
            {
                if (param.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return param;
                }
            }

            return null;
        }

        [Serializable]
        internal class UriReaderException : UriFormatException
        {
            public UriReaderException()
            {
            }
    
            public UriReaderException(string textString) : base(textString)
            {
            }

            protected UriReaderException(SerializationInfo serializationInfo, StreamingContext streamingContext)
                : base(serializationInfo, streamingContext)
            {
            }
    
            public int Position { get; set; }
        }
    }
}
