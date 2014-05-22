// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(TolerantJsonTextReader))]
    public class TolerantJsonTextReaderTests : UnitTest
    {
        public static readonly ReadOnlyCollection<TestData> RepresentativeJsonAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
            {
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_author_json,
                    ExpectedOptions = new string[] { "author\nauthor\n59\n\n", "genre\ngenre\n59\n\n", "price\nprice\n59\n\n", "publicationdate\npublicationdate\n59\n\n", "title\ntitle\n59\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_genre_value_json,
                    ExpectedOptions = new string[] { "autobiography\n0\n182\n\n", "novel\n1\n182\n\n", "philosophy\n2\n182\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_genres_enum_json,
                    ExpectedOptions = new string[] { "autobiography\n0\n822\n\n", "novel\n1\n822\n\n", "philosophy\n2\n822\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_ISBN_value_json,
                    ExpectedOptions = new string[] { }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_stop_at_ISBN_json,
                    ExpectedOptions = new string[] { "ISBN\nISBN\n29\n\n", "author\nauthor\n29\n\n", "genre\ngenre\n29\n\n", "price\nprice\n29\n\n", "publicationdate\npublicationdate\n29\n\n", "title\ntitle\n29\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = StringResources.contosoBooks_json,
                    ExpectedOptions = new string[] { }
                }
            })});
        
        
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("TolerantJsonTextReader is internal, disposable and concrete.")]
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
            Type schema = typeof(bookstore);

            Asserters.Data.Execute(
                RepresentativeJsonAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    string text = textAndExpectedOptions.Text.Replace("\r\n", "\n");
                    using (TolerantJsonTextReader reader = new TolerantJsonTextReader(text, -1, schema))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }

                    int length = text.Length;
                    text = StringResources.contosoBooks_json.Replace("\r\n", "\n");
                    using (TolerantJsonTextReader reader = new TolerantJsonTextReader(text, length, schema))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("GetExpectedItems() returns available options, with a property name missing closing quotation mark")]
        public void GetExpectedItemsReturnsAvailableOptionsCsdmain232168()
        {
            Type schema = typeof(bookstore);

            Asserters.Data.Execute(
                new ReadOnlyCollection<TestData>(
                    new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
                    {
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = "{\n\"book\n}",
                            ExpectedOptions = new string[] { "book\nbook\n3\n\n", "genres\ngenres\n3\n\n" }
                        },
                    })}),
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    string text = textAndExpectedOptions.Text.Replace("\r\n", "\n");
                    using (TolerantJsonTextReader reader = new TolerantJsonTextReader(text, -1, schema))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }

        #endregion Methods
    }
}