// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;

    /// <summary>
    /// The <see cref="HttpRequestHeaderParser"/> combines <see cref="HttpRequestLineParser"/> for parsing the HTTP Request Line  
    /// and <see cref="InternetMessageFormatHeaderParser"/> for parsing each header field. 
    /// </summary>
    internal class HttpRequestHeaderParser
    {
        private const int DefaultMaxRequestLineSize = 2 * 1024;
        private const int DefaultMaxHeaderSize = 16 * 1024;     // Same default size as IIS has for regular requests

        private HttpUnsortedRequest httpRequest;
        private HttpRequestState requestStatus = HttpRequestState.RequestLine;

        private HttpRequestLineParser requestLineParser;
        private InternetMessageFormatHeaderParser headerParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestHeaderParser"/> class.
        /// </summary>
        /// <param name="httpRequest">The parsed HTTP request without any header sorting.</param>
        public HttpRequestHeaderParser(HttpUnsortedRequest httpRequest)
            : this(httpRequest, DefaultMaxRequestLineSize, DefaultMaxHeaderSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestHeaderParser"/> class.
        /// </summary>
        /// <param name="httpRequest">The parsed HTTP request without any header sorting.</param>
        /// <param name="maxRequestLineSize">The max length of the HTTP request line.</param>
        /// <param name="maxHeaderSize">The max length of the HTTP header.</param>
        public HttpRequestHeaderParser(HttpUnsortedRequest httpRequest, int maxRequestLineSize, int maxHeaderSize)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            this.httpRequest = httpRequest;

            // Create request line parser
            this.requestLineParser = new HttpRequestLineParser(this.httpRequest, maxRequestLineSize);

            // Create header parser
            this.headerParser = new InternetMessageFormatHeaderParser(this.httpRequest.HttpHeaders, maxHeaderSize);
        }

        private enum HttpRequestState
        {
            RequestLine = 0,        // parsing request line
            RequestHeaders          // reading headers
        }

        /// <summary>
        /// Parse an HTTP request header and fill in the <see cref="HttpRequestMessage"/> instance.
        /// </summary>
        /// <param name="buffer">Request buffer from where request is read</param>
        /// <param name="bytesReady">Size of request buffer</param>
        /// <param name="bytesConsumed">Offset into request buffer</param>
        /// <returns>State of the parser.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        public ParserState ParseBuffer(
            byte[] buffer,
            int bytesReady,
            ref int bytesConsumed)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            ParserState parseStatus = ParserState.NeedMoreData;
            ParserState subParseStatus = ParserState.NeedMoreData;

            switch (this.requestStatus)
            {
                case HttpRequestState.RequestLine:
                    try
                    {
                        subParseStatus = this.requestLineParser.ParseBuffer(buffer, bytesReady, ref bytesConsumed);
                    }
                    catch (Exception)
                    {
                        subParseStatus = ParserState.Invalid;
                    }

                    if (subParseStatus == ParserState.Done)
                    {
                        this.requestStatus = HttpRequestState.RequestHeaders;
                        subParseStatus = ParserState.NeedMoreData;
                        goto case HttpRequestState.RequestHeaders;
                    }
                    else if (subParseStatus != ParserState.NeedMoreData)
                    {
                        // Report error - either Invalid or DataTooBig
                        parseStatus = subParseStatus;
                        break;
                    }

                    break; // read more data

                case HttpRequestState.RequestHeaders:
                    if (bytesConsumed >= bytesReady)
                    {
                        // we already can tell we need more data
                        break;
                    }

                    try
                    {
                        subParseStatus = this.headerParser.ParseBuffer(buffer, bytesReady, ref bytesConsumed);
                    }
                    catch (Exception)
                    {
                        subParseStatus = ParserState.Invalid;
                    }

                    if (subParseStatus == ParserState.Done)
                    {
                        parseStatus = subParseStatus;
                    }
                    else if (subParseStatus != ParserState.NeedMoreData)
                    {
                        parseStatus = subParseStatus;
                        break;
                    }

                    break; // need more data
            }

            return parseStatus;
        }
    }
}
