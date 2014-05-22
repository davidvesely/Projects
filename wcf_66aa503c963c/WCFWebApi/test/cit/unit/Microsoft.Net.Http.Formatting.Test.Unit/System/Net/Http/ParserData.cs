// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal static class ParserData
    {
        public const int MinHeaderSize = 2;

        public const int MinMessageSize = 10;

        public const int MinRequestLineSize = 14;

        public const int MinStatusLineSize = 15;

        public const int MinBufferSize = 256;

        public static readonly string[] Boundaries = new string[]
        { 
            "1",
            "a",
            "'",
            "(",
            ")",
            "+",
            "_",
            ",",
            "-",
            ".",
            "/",
            ":",
            "=",
            "?",
            "--",
            "--------------------01234567890123456789",
            "--------------------01234567890123456789--------------------",
            "--A--B--C--D--E--F--",
        };

        public static readonly string[] ValidReasonPhrases = new string[]
        {
            "",
            "Ok",
            "public Server Error",
            "r e a s o n",
            "reason ",
            " reason ",
        };

        public static readonly Version[] Versions = new Version[]
        {
            Version.Parse("1.0"),
            Version.Parse("1.1"),
            Version.Parse("1.2"),
            Version.Parse("2.0"),
            Version.Parse("10.0"),
            Version.Parse("1.15"),
        };

        public static readonly string[] InvalidVersions = new string[]
        {
            "",
            "http/1.1",
            "HTTP/a.1",
            "HTTP/1.a",
            "HTTP 1.1",
            "HTTP\t1.1",
            "HTTP 1 1",
            "\0",
            "HTTP\01.1",
            "HTTP/4294967295.4294967295",
        };

        public static readonly string[] InvalidMethods = new string[]
        {
            "",
            "G\tT",
            "G E T",
            "\0",
            "G\0T",
            "GET\n",
        };

        public static readonly string[] InvalidReasonPhrases = new string[]
        {
            "\0",
            "\t",
            "reason\n",
        };

        // This deliberately only checks for syntac boundaries of the URI, not its content
        public static readonly string[] InvalidRequestUris = new string[]
        {
            "",
            "p a t h",
            "path ",
            " path ",
        };

        public static readonly string[] InvalidStatusCodes = new string[]
        {
            "0",
            "99",
            "1a1",
            "abc",
            "1001",
            "2000",
            int.MinValue.ToString(),
            int.MaxValue.ToString(),
        };

        public static readonly Dictionary<string, string> ValidHeaders = new Dictionary<string, string>
        {
            { "N0", "V0"},
            { "N1", "V1"},
            { "N2", "V2"},
            { "N3", "V3"},
            { "N4", "V4"},
            { "N5", "V5"},
            { "N6", "V6"},
            { "N7", "V7"},
            { "N8", "V8"},
            { "N9", "V9"},
        };

        public static readonly string HttpMethod = "TEG";
        public static readonly HttpStatusCode HttpStatus = HttpStatusCode.Created;
        public static readonly string HttpReasonPhrase = "ReasonPhrase";
        public static readonly string HttpHostName = "example.com";
        public static readonly string HttpMessageEntity = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";

        public static readonly Uri HttpRequestUri = new Uri("http://" + HttpHostName + "/some/path");
        public static readonly Uri HttpsRequestUri = new Uri("https://" + HttpHostName + "/some/path");

        public static readonly string HttpRequest =
            HttpMethod +
            " /some/path HTTP/1.2\r\nHost: " +
            HttpHostName +
            "\r\nN1: V1a, V1b, V1c, V1d, V1e\r\nN2: V2\r\n\r\n";

        public static readonly string HttpResponse =
            "HTTP/1.2 " +
            ((int)HttpStatus).ToString() +
            " " +
            HttpReasonPhrase +
            "\r\nN1: V1a, V1b, V1c, V1d, V1e\r\nN2: V2\r\n\r\n";

        public static readonly string TextContentType = "text/plain; charset=utf-8";

        public static readonly string HttpRequestWithEntity =
            HttpMethod +
            " /some/path HTTP/1.2\r\nHost: " +
            HttpHostName +
            "\r\nN1: V1a, V1b, V1c, V1d, V1e\r\nN2: V2\r\nContent-Type: " +
            TextContentType +
            "\r\n\r\n" +
            HttpMessageEntity;

        public static readonly string HttpResponseWithEntity =
            "HTTP/1.2 " +
            ((int)HttpStatus).ToString() +
            " " +
            HttpReasonPhrase +
            "\r\nN1: V1a, V1b, V1c, V1d, V1e\r\nN2: V2\r\nContent-Type: " +
            TextContentType +
            "\r\n\r\n" +
            HttpMessageEntity;

        public static readonly MediaTypeHeaderValue HttpRequestMediaType;

        public static readonly MediaTypeHeaderValue HttpResponseMediaType;

        static ParserData()
        {
            MediaTypeHeaderValue.TryParse("application/http; msgtype=request", out HttpRequestMediaType);
            MediaTypeHeaderValue.TryParse("application/http; msgtype=response", out HttpResponseMediaType);
        }

}
}