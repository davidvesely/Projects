// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class TolerantHttpHeaderTextReader : IDisposable, ITolerantTextReader
    {
        private static readonly Dictionary<string, string[]> WellKnownHttpHeaders = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Accept",                         
                new string[] 
                {
                    "*/*", "application/*", "application/xml", "application/json", "text/*", "text/xml", "text/json", "text/plain" 
                }
            },
            {
                "Accept-Charset", 
                new string[] { "utf-8", "utf-16" }
            },
            {
                "Accept-Encoding", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Accept-Language", 
                new string[] { "en-US" }
            },
            {
                "Authorization", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Cache-Control", 
                new string[] 
                { 
                    "no-cache", "no-store", "max-age", "max-stale", "min-fresh", "no-transform", "only-if-cached" 
                }
            },
            {
                "Connection", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Cookie", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Content-Length", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Content-MD5", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Content-Type", 
                new string[] 
                { 
                    "application/xml", "application/json", "text/xml", "text/json", "text/plain" 
                }
            },
            {
                "Date", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "From", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Host", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "If-Match", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "If-Modified-Since", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "If-None-Match", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "If-Range", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "If-Unmodified-Since", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Max-Forwards", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Pragma", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Proxy-Authorization", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Range", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Referer", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "TE", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Upgrade", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "User-Agent", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Via", 
                TolerantTextReaderHelper.EmptyStringArray
            },
            {
                "Warning", 
                TolerantTextReaderHelper.EmptyStringArray
            },
        };

        public TolerantHttpHeaderTextReader(string text, int stopAtPosition)
        {
            this.Text = text;
            this.StopAtPosition = stopAtPosition >= 0 && stopAtPosition <= this.Text.Length
                ? stopAtPosition
                : this.Text.Length;
            this.Position = 0;
            this.TokenPosition = 0;

            this.StringReader = new StringReader(this.Text);

            this.Headers = new List<HttpTestUtils.HttpHeaderInfo>();
            this.NodeType = HttpHeaderNodeType.Other;

            this.Exception = null;
        }

        private enum HttpHeaderNodeType
        {
            Other,
            Name,
            Value,
        }

        public Exception Exception { get; private set; }

        private string Text { get; set; }

        private int StopAtPosition { get; set; }

        private int Position { get; set; }

        private int TokenPosition { get; set; } // where replacement should start, absolute index

        private StringReader StringReader { get; set; }

        private List<HttpTestUtils.HttpHeaderInfo> Headers { get; set; }

        private HttpTestUtils.HttpHeaderInfo Header { get; set; }

        private HttpHeaderNodeType NodeType { get; set; }

        private bool EOF
        {
            get
            {
                return this.Position >= this.StopAtPosition;
            }
        }

        public void Dispose()
        {
            if (this.StringReader != null)
            {
                this.StringReader.Dispose();
                this.StringReader = null;
            }

            GC.SuppressFinalize(this);
        }

        public bool Read()
        {
            if (this.EOF)
            {
                this.StringReader.Dispose();
                this.StringReader = null;

                if (this.NodeType == HttpHeaderNodeType.Other)
                {
                    this.NodeType = HttpHeaderNodeType.Name;
                }

                return false;
            }

            string line = this.StringReader.ReadLine();
            if (line == null)
            {
                this.StringReader.Dispose();
                this.StringReader = null;

                return false;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                return true;
            }

            string[] buf = line.Split(new char[] { ':' }, 2);

            string name = buf[0].Trim();
            if (string.IsNullOrEmpty(name))
            {
                name = "dummy";
            }

            this.Position += buf[0].Length;

            if (this.EOF)
            {
                this.Header = new HttpTestUtils.HttpHeaderInfo(name, null);
                this.Headers.Add(this.Header);

                this.NodeType = HttpHeaderNodeType.Name;
                return true;
            }

            if (buf.Length == 2)
            {
                this.Position++;
                this.TokenPosition = this.Position;

                this.Position += buf[1].Length;

                this.Header = new HttpTestUtils.HttpHeaderInfo(name, buf[1].Trim());
                this.Headers.Add(this.Header);

                if (this.EOF)
                {
                    this.NodeType = HttpHeaderNodeType.Value;
                    return true;
                }
            }

            this.Position++;    // count \n
            this.TokenPosition = this.Position;

            return true;
        }

        public IEnumerable<string> GetExpectedItems()
        {
            switch (this.NodeType)
            {
                case HttpHeaderNodeType.Name:
                    return this.GetExpectedHeaderNames();

                case HttpHeaderNodeType.Value:
                    return this.GetExpectedHeaderValues();

                default:
                    return TolerantTextReaderHelper.EmptyStringArray;
            }
        }

        public FormattedHttpHeaderTextWriter CreateWriter(TextWriter innerWriter)
        {
            return new FormattedHttpHeaderTextWriter(innerWriter, this.Headers);
        }

        private IEnumerable<string> GetExpectedHeaderNames()
        {
            foreach (string name in TolerantHttpHeaderTextReader.WellKnownHttpHeaders.Keys)
            {
                string displayName = name;
                string value = displayName + ":";
                yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition, 0);  // force another autocomplete
            }
        }

        private IEnumerable<string> GetExpectedHeaderValues()
        {
            string[] values = TolerantTextReaderHelper.EmptyStringArray;
            TolerantHttpHeaderTextReader.WellKnownHttpHeaders.TryGetValue(this.Header.Key, out values);
            for (int i = 0; i < values.Length; i++)
            {
                string displayName = values[i];
                string value = displayName;
                yield return TolerantTextReaderHelper.GetExpectedValue(displayName, value, this.TokenPosition);
            }
        }
    }
}
