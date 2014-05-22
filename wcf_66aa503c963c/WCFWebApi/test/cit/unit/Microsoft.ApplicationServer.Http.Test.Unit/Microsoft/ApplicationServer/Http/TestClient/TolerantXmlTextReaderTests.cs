// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(TolerantXmlTextReader))]
    public class TolerantXmlTextReaderTests : UnitTest
    {
         public static readonly ReadOnlyCollection<TestData> RepresentativeXmlAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
            {
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contoso_stop_at_root_element_xmlns_xml,
                    ExpectedOptions = new string[] { "http://www.contoso.com/books\nhttp://www.contoso.com/books\n58\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contoso_stop_at_root_element_xml,
                    ExpectedOptions = new string[] { "bookstore\nbookstore xmlns=\"http://www.contoso.com/books\"></bookstore>\n41\n-12\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_author_xml,
                    ExpectedOptions = new string[] { "author\nauthor></author>\n233\n-9\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_end_xml,
                    ExpectedOptions = new string[] { "book\nbook>\n610\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_genre_value_xml,
                    ExpectedOptions = new string[] { "autobiography\nautobiography\n104\n\n", "novel\nnovel\n104\n\n", "philosophy\nphilosophy\n104\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_genre_xml,
                    ExpectedOptions = new string[] { "genre\ngenre=\"\"\n97\n-1\n", "publicationdate\npublicationdate=\"\"\n97\n-1\n", "ISBN\nISBN=\"\"\n97\n-1\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_ISBN_xml,
                    ExpectedOptions = new string[] { "genre\ngenre=\"\"\n418\n-1\n", "publicationdate\npublicationdate=\"\"\n418\n-1\n", "ISBN\nISBN=\"\"\n418\n-1\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_publicationdate_value,
                    ExpectedOptionsGenerator = delegate() 
                    {
                        string displayName = DateTime.Now.ToString("yyyy-MM-ddzzz");
                        string value = displayName;
                        return new string[] { TolerantTextReaderHelper.GetExpectedValue(displayName, value, 136) };
                    },
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_title_text_xml,
                    ExpectedOptions = new string[] { "The Autobiography of Benjamin Franklin\nThe Autobiography of Benjamin Franklin\n181\n\n", "The Confidence Man\nThe Confidence Man\n181\n\n", "The Gorgias\nThe Gorgias\n181\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_title_xml,
                    ExpectedOptions = new string[] { "title\ntitle></title>\n175\n-8\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_book_xml,
                    ExpectedOptions = new string[] { "book\nbook></book>\n92\n-7\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_xml,
                    ExpectedOptions = new string[] { }
                }
            })});
        
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("TolerantXmlTextReader is internal, disposable, and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsDisposable);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("GetExpectedItems() returns available options")]
        public void GetExpectedItemsReturnsAvailableOptions()
        {
            XmlSchema schema;
            using (XmlReader reader = XmlReader.Create(new StringReader(StringResources.contosoBooks_xsd)))
            {
                schema = XmlSchema.Read(reader, null);
            }

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            Asserters.Data.Execute(
                RepresentativeXmlAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    string text = textAndExpectedOptions.Text.Replace("\r\n", "\n");
                    using (TolerantXmlTextReader reader = new TolerantXmlTextReader(text, -1, schemaSet))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }

                    int length = text.Length;
                    text = StringResources.contosoBooks_xml.Replace("\r\n", "\n");
                    using (TolerantXmlTextReader reader = new TolerantXmlTextReader(text, length, schemaSet))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("GetExpectedItems() returns available options with an empty tag")]
        public void GetExpectedItemsReturnsAvailableOptionsCsdmain229886()
        {
            XmlSchema schema;
            using (XmlReader reader = XmlReader.Create(new StringReader(StringResources.contosoBooks_xsd)))
            {
                schema = XmlSchema.Read(reader, null);
            }

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            Asserters.Data.Execute(
                new ReadOnlyCollection<TestData>(
                    new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
                    {
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = "<Book><Name/></b",
                            ExpectedOptions = new string[] { "Book\nBook>\n15\n\n" }
                        },
                    })}),
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    string text = textAndExpectedOptions.Text.Replace("\r\n", "\n");
                    using (TolerantXmlTextReader reader = new TolerantXmlTextReader(text, -1, schemaSet))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }
        
        #endregion Methods
    }
}