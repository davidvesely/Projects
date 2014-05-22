//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Net.Http.Formatting.OData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net.Http.Formatting.OData.Test;
    using System.Net.Http.Formatting.OData.Test.ComplexTypes;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CollectionTest
    {
        Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out array of ints in valid ODataMessageFormat")]
        public void ArrayOfIntsSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<int[]> content = new ObjectContent<int[]>(new int[] { 10, 20, 30, 40, 50 });
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestArrayOfInts, content.ReadAsStringAsync().Result, true);  

        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out array of bool in valid ODataMessageFormat")]
        public void ArrayOfBoolsSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<bool[]> content = new ObjectContent<bool[]>(new bool[] { true, false, true, false });
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestArrayOfBools, content.ReadAsStringAsync().Result, true);  
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out List of strings in valid ODataMessageFormat")]
        public void ListOfStringsSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            List<string> listOfStrings = new List<string>();
            listOfStrings.Add("Frank");
            listOfStrings.Add("Steve");
            listOfStrings.Add("Tom");
            listOfStrings.Add("Chandler");

            ObjectContent<List<string>> content = new ObjectContent<List<string>>(listOfStrings);
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestListOfStrings, content.ReadAsStringAsync().Result, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out Collection of objects in valid ODataMessageFormat")]
        public void CollectionOfObjectsSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            Collection<object> collectionOfObjects = new Collection<object>();
            collectionOfObjects.Add(1);
            collectionOfObjects.Add("Frank");
            collectionOfObjects.Add(TypeInitializer.GetInstance(SupportedTypes.Person, 2));
            collectionOfObjects.Add(TypeInitializer.GetInstance(SupportedTypes.Employee, 3));

            ObjectContent<Collection<object>> content = new ObjectContent<Collection<object>>(collectionOfObjects);
            formatter.MaxReferenceDepth = 1;
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestCollectionOfObjects, content.ReadAsStringAsync().Result, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out Collection of complex types in valid ODataMessageFormat")]
        public void CollectionOfComplexTypeSerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            IEnumerable<Person> collectionOfPerson = new Collection<Person>() 
            {
                (Person)TypeInitializer.GetInstance(SupportedTypes.Person, 0),
                (Person)TypeInitializer.GetInstance(SupportedTypes.Person, 1),
                (Person)TypeInitializer.GetInstance(SupportedTypes.Person, 2)
            };

            ObjectContent<IEnumerable<Person>> content = new ObjectContent<IEnumerable<Person>>(collectionOfPerson);
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestCollectionOfPerson, content.ReadAsStringAsync().Result, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.OData)]
        [Description("ODataMediaTypeFormatter writes out Dictionary type in valid ODataMessageFormat")]
        public void DictionarySerializesAsOData()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "Frank");
            dictionary.Add(2, "Steve");
            dictionary.Add(3, "Tom");
            dictionary.Add(4, "Chandler");

            ObjectContent<Dictionary<int, string>> content = new ObjectContent<Dictionary<int, string>>(dictionary);
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            Asserters.String.AreEqual(BaselineResource.TestDictionary, content.ReadAsStringAsync().Result, true);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter sets required headers for a complex type when serialized as XML.")]
        public void ContentHeadersAreAddedForXmlMediaType()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<IEnumerable<Person>> content = new ObjectContent<IEnumerable<Person>>(new Person[] { new Person(0, new ReferenceDepthContext(7)) });
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

            HttpResponseMessage<IEnumerable<Person>> response = new HttpResponseMessage<IEnumerable<Person>>(new Person[] { new Person(0, new ReferenceDepthContext(7)) });
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ObjectContent<IEnumerable<Person>> content = response.Content;
            content.Formatters.Clear();
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;
            content.LoadIntoBufferAsync().Wait();

            Asserters.Http.Contains(content.Headers, "DataServiceVersion", "3.0;");
            Asserters.Http.Contains(content.Headers, "Content-Type", "application/json; charset=utf-8");
        }
    }
}
