// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ODataFormatter.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;


    public class Util
    {
        public const string BaseAddress = "http://localhost:8080/TestService/";
        public const string Version1NumberString = "1.0;";
        public const string Version2NumberString = "2.0;";
        public const string Version3NumberString = "3.0;";

        public static Uri BaseAddressUri = new Uri(BaseAddress);
        public static MediaTypeHeaderValue ApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");
        public static MediaTypeHeaderValue ApplicationAtomMediaType = new MediaTypeHeaderValue("application/atom+xml");
        public static MediaTypeWithQualityHeaderValue ApplicationJsonMediaTypeWithQuality = new MediaTypeWithQualityHeaderValue("application/json");
        public static MediaTypeWithQualityHeaderValue ApplicationAtomMediaTypeWithQuality = new MediaTypeWithQualityHeaderValue("application/atom+xml");

        public static HttpServiceHost CreateServiceHost(HttpConfiguration configuration)
        {
            Uri baseAddressUri = new Uri(BaseAddress);
            HttpServiceHost host = new HttpServiceHost(typeof(SampleService), configuration, BaseAddress);
            host.AddDefaultEndpoints();
            return host;
        }

        public static HttpRequestMessage GenerateRequestMessage(bool isAtom, Uri address)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, address);
            MediaTypeWithQualityHeaderValue mediaType = isAtom ? ApplicationAtomMediaTypeWithQuality : ApplicationJsonMediaTypeWithQuality;
            requestMessage.Headers.Accept.Add(mediaType);
            requestMessage.Headers.Add("DataServiceVersion", "2.0");
            requestMessage.Headers.Add("MaxDataServiceVersion", "3.0");
            return requestMessage;
        }
    }
}
