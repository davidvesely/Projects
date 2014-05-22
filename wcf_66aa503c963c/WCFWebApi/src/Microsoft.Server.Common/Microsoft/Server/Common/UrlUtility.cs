//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Text;

    // copied from System.Web.HttpUtility code (renamed here) to remove dependency on System.Web.dll
    public static class UrlUtility
    {
        // Query string parsing support
        public static NameValueCollection ParseQueryString(string query)
        {
            return ParseQueryString(query, Encoding.UTF8);
        }

        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
            {
                throw Fx.Exception.ArgumentNull("query");
            }

            if (encoding == null)
            {
                throw Fx.Exception.ArgumentNull("encoding");
            }

            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }

            return new HttpValueCollection(query, encoding);
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlEncode(string value)
        {
            if (value == null)
            {
                return null;
            }

            return UrlEncode(value, Encoding.UTF8);
        }

        // URL encodes a path portion of a URL string and returns the encoded string.
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlPathEncode(string value)
        {
            if (value == null)
            {
                return null;
            }

            // recurse in case there is a query string
            int i = value.IndexOf('?');
            if (i >= 0)
            {
                return UrlPathEncode(value.Substring(0, i)) + value.Substring(i);
            }

            // encode DBCS characters and spaces only
            return UrlEncodeSpaces(UrlEncodeNonAscii(value, Encoding.UTF8));
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlEncode(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }

            return Encoding.ASCII.GetString(UrlEncodeToBytes(value, encoding));
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlEncodeUnicode(string value)
        {
            if (value == null)
            {
                return null;
            }

            return UrlEncodeUnicodeStringToStringInternal(value, false);
        }

        // Helper to encode the non-ASCII url characters only
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlEncodeNonAscii(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = encoding.GetBytes(value);
            bytes = UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
            return Encoding.ASCII.GetString(bytes);
        }

        // Helper to encode spaces only
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlEncodeSpaces(string value)
        {
            if (value != null && value.IndexOf(' ') >= 0)
            {
                value = value.Replace(" ", "%20");
            }

            return value;
        }

        public static byte[] UrlEncodeToBytes(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }

            byte[] bytes = encoding.GetBytes(value);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Ported from WCF")]
        public static string UrlDecode(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }

            return UrlDecodeStringFromStringInternal(value, encoding);
        }

        // Implementation for encoding
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "Ported from WCF")]
        public static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int countSpaces = 0;
            int countUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];

                if (ch == ' ')
                {
                    countSpaces++;
                }
                else if (!IsSafe(ch))
                {
                    countUnsafe++;
                }
            }

            // nothing to expand?
            if (!alwaysCreateReturnValue && countSpaces == 0 && countUnsafe == 0)
            {
                return bytes;
            }

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + (countUnsafe * 2)];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                char ch = (char)b;

                if (IsSafe(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }

            return expandedBytes;
        }

        public static bool IsNonAsciiByte(byte value)
        {
            return value >= 0x7F || value < 0x20;
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "Ported from WCF")]
        public static byte[] UrlEncodeBytesToBytesInternalNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int countNonAscii = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                if (IsNonAsciiByte(bytes[offset + i]))
                {
                    countNonAscii++;
                }
            }

            // nothing to expand?
            if (!alwaysCreateReturnValue && countNonAscii == 0)
            {
                return bytes;
            }

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + (countNonAscii * 2)];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];

                if (IsNonAsciiByte(b))
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
                else
                {
                    expandedBytes[pos++] = b;
                }
            }

            return expandedBytes;
        }

        private static string UrlEncodeUnicodeStringToStringInternal(string s, bool ignoreAscii)
        {
            int l = s.Length;
            StringBuilder sb = new StringBuilder(l);

            for (int i = 0; i < l; i++)
            {
                char ch = s[i];

                if ((ch & 0xff80) == 0)
                {  // 7 bit?
                    if (ignoreAscii || IsSafe(ch))
                    {
                        sb.Append(ch);
                    }
                    else if (ch == ' ')
                    {
                        sb.Append('+');
                    }
                    else
                    {
                        sb.Append('%');
                        sb.Append(IntToHex((ch >> 4) & 0xf));
                        sb.Append(IntToHex(ch & 0xf));
                    }
                }
                else
                { // arbitrary Unicode?
                    sb.Append("%u");
                    sb.Append(IntToHex((ch >> 12) & 0xf));
                    sb.Append(IntToHex((ch >> 8) & 0xf));
                    sb.Append(IntToHex((ch >> 4) & 0xf));
                    sb.Append(IntToHex(ch & 0xf));
                }
            }

            return sb.ToString();
        }

        private static string UrlDecodeStringFromStringInternal(string s, Encoding e)
        {
            int count = s.Length;
            UrlDecoder helper = new UrlDecoder(count, e);

            // go through the string's chars collapsing %XX and %uXXXX and
            // appending each char as char, with exception of %XX constructs
            // that are appended as bytes
            for (int pos = 0; pos < count; pos++)
            {
                char ch = s[pos];

                if (ch == '+')
                {
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    if (s[pos + 1] == 'u' && pos < count - 5)
                    {
                        int h1 = HexToInt(s[pos + 2]);
                        int h2 = HexToInt(s[pos + 3]);
                        int h3 = HexToInt(s[pos + 4]);
                        int h4 = HexToInt(s[pos + 5]);

                        if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
                        {   // valid 4 hex chars
                            ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
                            pos += 5;

                            // only add as char
                            helper.AddChar(ch);
                            continue;
                        }
                    }
                    else
                    {
                        int h1 = HexToInt(s[pos + 1]);
                        int h2 = HexToInt(s[pos + 2]);

                        if (h1 >= 0 && h2 >= 0)
                        {     // valid 2 hex chars
                            byte b = (byte)((h1 << 4) | h2);
                            pos += 2;

                            // don't add as char
                            helper.AddByte(b);
                            continue;
                        }
                    }
                }

                if ((ch & 0xFF80) == 0)
                {
                    helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                }
                else
                {
                    helper.AddChar(ch);
                }
            }

            return helper.GetString();
        }

        // Private helpers for URL encoding/decoding
        private static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }

        private static char IntToHex(int n)
        {
            // WCF CHANGE: CHANGED FROM Debug.Assert() to Fx.Assert()
            Fx.Assert(n < 0x10, "n < 0x10");

            if (n <= 9)
            {
                return (char)(n + (int)'0');
            }
            else
            {
                return (char)(n - 10 + (int)'a');
            }
        }

        // Set of safe chars, from RFC 1738.4 minus '+'
        private static bool IsSafe(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '\'':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        // Internal class to facilitate URL decoding -- keeps char buffer and byte buffer, allows appending of either chars or bytes
        internal class UrlDecoder
        {
            int charBufferSize;

            // Accumulate characters in a special array
            int numChars;
            char[] charBuffer;

            // Accumulate bytes for decoding into characters in a special array
            int numBytes;
            byte[] byteBuffer;

            // Encoding to convert chars to bytes
            Encoding charEncoding;

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                this.charBufferSize = bufferSize;
                this.charEncoding = encoding;

                // byte buffer created on demand
                this.charBuffer = new char[bufferSize];
            }

            internal void FlushBytes()
            {
                if (this.numBytes > 0)
                {
                    this.numChars += this.charEncoding.GetChars(this.byteBuffer, 0, this.numBytes, this.charBuffer, this.numChars);
                    this.numBytes = 0;
                }
            }

            internal void AddChar(char ch)
            {
                if (this.numBytes > 0)
                {
                    this.FlushBytes();
                }

                this.charBuffer[this.numChars++] = ch;
            }

            internal void AddByte(byte b)
            {
                //// if there are no pending bytes treat 7 bit bytes as characters
                //// this optimization is temp disable as it doesn't work for some encodings

                ////if (_numBytes == 0 && ((b & 0x80) == 0)) {
                ////    AddChar((char)b);
                ////}
                ////else
                ////
                ////{

                if (this.byteBuffer == null)
                {
                    this.byteBuffer = new byte[this.charBufferSize];
                }

                this.byteBuffer[this.numBytes++] = b;

                ////}
            }

            internal string GetString()
            {
                if (this.numBytes > 0)
                {
                    this.FlushBytes();
                }

                if (this.numChars > 0)
                {
                    return new string(this.charBuffer, 0, this.numChars);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [Serializable]
        internal class HttpValueCollection : NameValueCollection
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Ported from WCF")]
            internal HttpValueCollection(string str, Encoding encoding)
                : base(StringComparer.OrdinalIgnoreCase)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    this.FillFromString(str, true, encoding);
                }

                IsReadOnly = false;
            }

            protected HttpValueCollection(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            public override string ToString()
            {
                return this.ToString(true, null);
            }

            internal void FillFromString(string s, bool urlencoded, Encoding encoding)
            {
                int l = (s != null) ? s.Length : 0;
                int i = 0;

                while (i < l)
                {
                    // find next & while noting first = on the way (and if there are more)
                    int si = i;
                    int ti = -1;

                    while (i < l)
                    {
                        char ch = s[i];

                        if (ch == '=')
                        {
                            if (ti < 0)
                            {
                                ti = i;
                            }
                        }
                        else if (ch == '&')
                        {
                            break;
                        }

                        i++;
                    }

                    // extract the name / value pair
                    string name = null;
                    string value = null;

                    if (ti >= 0)
                    {
                        name = s.Substring(si, ti - si);
                        value = s.Substring(ti + 1, i - ti - 1);
                    }
                    else
                    {
                        value = s.Substring(si, i - si);
                    }

                    // add name / value pair to the collection
                    if (urlencoded)
                    {
                        this.Add(
                           UrlUtility.UrlDecode(name, encoding),
                           UrlUtility.UrlDecode(value, encoding));
                    }
                    else
                    {
                        this.Add(name, value);
                    }

                    // trailing '&'
                    if (i == l - 1 && s[i] == '&')
                    {
                        this.Add(null, string.Empty);
                    }

                    i++;
                }
            }

            string ToString(bool urlencoded, IDictionary excludeKeys)
            {
                int n = Count;
                if (n == 0)
                {
                    return string.Empty;
                }

                StringBuilder s = new StringBuilder();
                string key, keyPrefix, item;

                for (int i = 0; i < n; i++)
                {
                    key = GetKey(i);

                    if (excludeKeys != null && key != null && excludeKeys[key] != null)
                    {
                        continue;
                    }

                    if (urlencoded)
                    {
                        key = UrlUtility.UrlEncodeUnicode(key);
                    }

                    keyPrefix = (!string.IsNullOrEmpty(key)) ? (key + "=") : string.Empty;

                    ArrayList values = (ArrayList)BaseGet(i);
                    int numValues = (values != null) ? values.Count : 0;

                    if (s.Length > 0)
                    {
                        s.Append('&');
                    }

                    if (numValues == 1)
                    {
                        s.Append(keyPrefix);
                        item = (string)values[0];
                        if (urlencoded)
                        {
                            item = UrlUtility.UrlEncodeUnicode(item);
                        }

                        s.Append(item);
                    }
                    else if (numValues == 0)
                    {
                        s.Append(keyPrefix);
                    }
                    else
                    {
                        for (int j = 0; j < numValues; j++)
                        {
                            if (j > 0)
                            {
                                s.Append('&');
                            }

                            s.Append(keyPrefix);
                            item = (string)values[j];
                            if (urlencoded)
                            {
                                item = UrlUtility.UrlEncodeUnicode(item);
                            }

                            s.Append(item);
                        }
                    }
                }

                return s.ToString();
            }
        }
    }
}
