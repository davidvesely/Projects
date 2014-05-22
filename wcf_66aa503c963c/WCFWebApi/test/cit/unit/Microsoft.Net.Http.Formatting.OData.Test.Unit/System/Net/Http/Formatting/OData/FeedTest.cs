//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Net.Http.Formatting.OData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net.Http.Formatting.OData.Test;
    using System.Net.Http.Formatting.OData.Test.EntityTypes;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class FeedTest
    {
        Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter serailizes a feed in valid ODataMessageFormat")]
        public void IEnumerableOfEntityTypeSerializesAsODataFeed()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            IEnumerable<Employee> collectionOfPerson = new Collection<Employee>() 
            {
                (Employee)TypeInitializer.GetInstance(SupportedTypes.Employee, 0),
                (Employee)TypeInitializer.GetInstance(SupportedTypes.Employee, 1),                
            };

            ObjectContent<IEnumerable<Employee>> content = new ObjectContent<IEnumerable<Employee>>(collectionOfPerson);
            formatter.MaxReferenceDepth = 2;
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            RegexReplacement replaceUpdateTime = new RegexReplacement("<updated>*.*</updated>", "<updated>UpdatedTime</updated>");
            Asserters.String.AreEqual(BaselineResource.TestFeedOfEmployee, content.ReadAsStringAsync().Result, true, replaceUpdateTime);  
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for a feed when serialized as ATOM.")]
        public void ContentHeadersAreAddedForXmlMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<IEnumerable<Employee>> content = new ObjectContent<IEnumerable<Employee>>(new Employee[] { new Employee(0, new ReferenceDepthContext(7)) });
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/atom+xml; type=feed");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for a feed when serialized as JSON.")]
        public void ContentHeadersAreAddedForJsonMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            HttpResponseMessage<IEnumerable<Employee>> response = new HttpResponseMessage<IEnumerable<Employee>>(new Employee[] { new Employee(0, new ReferenceDepthContext(7)) });
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ObjectContent<IEnumerable<Employee>> content = response.Content;
            content.Formatters.Clear();
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/json; charset=utf-8");
        }
    }
}
