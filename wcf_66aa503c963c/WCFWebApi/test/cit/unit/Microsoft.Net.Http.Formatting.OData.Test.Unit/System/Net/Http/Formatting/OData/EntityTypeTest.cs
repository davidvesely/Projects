//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Net.Http.Formatting.OData
{
    using System.Net.Http.Formatting.OData.Test;
    using System.Net.Http.Formatting.OData.Test.EntityTypes;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntityTypeTest
    {
        Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();      

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter serailizes an entity type in valid ODataMessageFormat")]
        public void EntityTypeSerializesAsODataEntry()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();
            
            Employee employee = (Employee)TypeInitializer.GetInstance(SupportedTypes.Employee);
            ObjectContent<Employee> content = new ObjectContent<Employee>(employee);
            formatter.MaxReferenceDepth = 2;
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            RegexReplacement replaceUpdateTime = new RegexReplacement("<updated>*.*</updated>", "<updated>UpdatedTime</updated>");
            Asserters.String.AreEqual(BaselineResource.TestEntityTypeBasic, content.ReadAsStringAsync().Result, true, replaceUpdateTime); 
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter serailizes an entity type with multiple keys in valid ODataMessageFormat")]
        public void EntityTypeWithMultipleKeysSerializesAsODataEntry()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            MultipleKeyEmployee multipleKeyEmployee = (MultipleKeyEmployee)TypeInitializer.GetInstance(SupportedTypes.MultipleKeyEmployee);
            ObjectContent<MultipleKeyEmployee> content = new ObjectContent<MultipleKeyEmployee>(multipleKeyEmployee);
            formatter.MaxReferenceDepth = 1;
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            RegexReplacement replaceUpdateTime = new RegexReplacement("<updated>*.*</updated>", "<updated>UpdatedTime</updated>");
            Asserters.String.AreEqual(BaselineResource.TestEntityTypeWithMultipleKeys, content.ReadAsStringAsync().Result, true, replaceUpdateTime);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for an entity type when serialized as XML.")]
        public void ContentHeadersAreAddedForXmlMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<Employee> content = new ObjectContent<Employee>(new Employee(0, new ReferenceDepthContext(7)));
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/atom+xml; type=entry");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for an entity type when serialized as JSON.")]
        public void ContentHeadersAreAddedForJsonMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            HttpResponseMessage<Employee> response = new HttpResponseMessage<Employee>(new Employee(0, new ReferenceDepthContext(7)));
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ObjectContent<Employee> content = response.Content;
            content.Formatters.Clear();
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/json; charset=utf-8");
        }
    }
}
