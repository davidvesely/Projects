// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Formatting.OData.Test.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Http;

    public class Util
    {
        public static void  VerifyResponse(string response, string expected)
        {
            Regex updatedRegEx = new Regex("<updated>*.*</updated>");
            response = updatedRegEx.Replace(response, "<updated>UpdatedTime</updated>");
            Assert.AreEqual(expected, response);
        }

        public static void VerifyJsonResponse(string response, string expected)
        {   
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
    }
}
