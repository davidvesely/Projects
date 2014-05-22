// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Formatting.OData.Test.Sample
{
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataFormatter.Sample;

    [TestClass]
    public class ODataScenarioTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the response from an Http GET in OData atom format when the accept header is application/atom+xml")]
        public void GetAtomXml()
        {
            string result = Program.GetAtomXml();
            Util.VerifyResponse(result, BaselineResource.EntryTypePersonAtom);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the response from an Http GET in OData atom format when the accept header is application/json")]
        public void GetApplicationJson()
        {
            string result = Program.GetApplicationJson();
            Util.VerifyJsonResponse(result, BaselineResource.EntryTypePersonODataJson);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how to get the ODataMediaTypeFormatter to only support application/atom+xml")]
        public void GetApplicationJsonFromJsonFormatter()
        {
            string result = Program.GetApplicationJsonFromJsonFormatter();
            Util.VerifyJsonResponse(result, BaselineResource.EntryTypePersonRegularJson);
        }

        [Ignore] // TODO: reactivate test when fix CSDMAIN 234391
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how ODataMediaTypeFormatter would conditionally support application/atom+xml and application/json only if format=odata is present in the QueryString")]
        public void GetApplicationJsonWithFormat()
        {
            string result = Program.GetApplicationJsonWithFormat();
            Util.VerifyJsonResponse(result, BaselineResource.EntryTypePersonODataJson);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Demonstrates how ODataMediaTypeFormatter would conditionally support application/atom+xml and application/json only if format=odata is present in the QueryString")]
        public void GetApplicationJsonWithoutFormat()
        {
            string result = Program.GetApplicationJsonWithoutFormat();
            Util.VerifyJsonResponse(result, BaselineResource.EntryTypePersonRegularJson);
        }
    }
}
