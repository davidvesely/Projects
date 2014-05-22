
// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
namespace System.Net.Http.Formatting.OData.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Util
    {
        public const string Version1NumberString = "1.0;";
        public const string Version2NumberString = "2.0;";
        public const string Version3NumberString = "3.0;";
        public static MediaTypeHeaderValue ApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");
        public static MediaTypeHeaderValue ApplicationAtomMediaType = new MediaTypeHeaderValue("application/atom+xml");
        public static MediaTypeWithQualityHeaderValue ApplicationJsonMediaTypeWithQuality = new MediaTypeWithQualityHeaderValue("application/json");
        public static MediaTypeWithQualityHeaderValue ApplicationAtomMediaTypeWithQuality = new MediaTypeWithQualityHeaderValue("application/atom+xml");
       
        public static void  VerifyResponse(HttpContent responseContent, string expected)
        {
            string response = responseContent.ReadAsStringAsync().Result;
            Regex updatedRegEx = new Regex("<updated>*.*</updated>");
            response = updatedRegEx.Replace(response, "<updated>UpdatedTime</updated>");
            Assert.AreEqual(expected, response);
        }


        public static void VerifyJsonResponse(HttpContent responseContent, string expected)
        {
            string response = responseContent.ReadAsStringAsync().Result;
            
            // resource file complains if "{" is present in the value
            Regex updatedRegEx = new Regex("{");
            response = updatedRegEx.Replace(response, "%");
            expected = expected.Trim();
            response = response.Trim();

            // compare line by line since odata json typically differs from baseline by spaces
            string[] expectedLines = expected.Split('\n');
            string[] responseLines = response.Split('\n');
            Assert.AreEqual(expectedLines.Length, responseLines.Length);
            for(int i=0;i<expectedLines.Length;i++)
            {
                Assert.AreEqual(expectedLines[i].Trim(), responseLines[i].Trim());
            }
            
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

        public static string GetDataServiceVersion(HttpResponseHeaders headers)
        {
            string dataServiceVersion = null;
            IEnumerable<string> values;
            if (headers.TryGetValues("DataServiceVersion", out values))
            {
                foreach (string value in values)
                {
                    dataServiceVersion = value;
                    break;
                }
            }
            return dataServiceVersion;
        }
    }
}
