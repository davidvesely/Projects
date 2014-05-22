//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Net.Http.Formatting.OData
{
    using System.Net.Http.Formatting.OData.Test;
    using System.Net.Http.Formatting.OData.Test.ComplexTypes;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexTypeTest
    {
        Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();
       
        [TestMethod]
        [TestCategory("CIT"), Timeout(Microsoft.TestCommon.TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter writes out complex  types in valid ODataMessageFormat")]
        public void ComplexTypeSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<Person> content = new ObjectContent<Person>(new Person(0, new ReferenceDepthContext(7)));
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestComplexTypePerson, content.ReadAsStringAsync().Result, true);            
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for a complex type when serialized as XML.")]
        public void ContentHeadersAreAddedForXmlMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<Person> content = new ObjectContent<Person>(new Person(0, new ReferenceDepthContext(7)));
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/xml");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for a complex type when serialized as JSON.")]
        public void ContentHeadersAreAddedForJsonMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            HttpResponseMessage<Person> response = new HttpResponseMessage<Person>(new Person(0, new ReferenceDepthContext(7)));
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ObjectContent<Person> content = response.Content;
            content.Formatters.Clear();
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/json; charset=utf-8");
        }
    }
}
