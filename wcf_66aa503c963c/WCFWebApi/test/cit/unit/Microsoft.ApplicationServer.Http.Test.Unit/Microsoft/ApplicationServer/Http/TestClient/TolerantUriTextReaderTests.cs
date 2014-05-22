// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(TolerantUriTextReader))]
    public class TolerantUriTextReaderTests : UnitTest
    {
        public static readonly ReadOnlyCollection<TestData> RepresentativeUriAndExpectedOptions = new ReadOnlyCollection<TestData>(
            new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
            {
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg3?var1=xxx&var2=yyy",
                    ExpectedOptions = new string[] { "Y1\nY1\n62\n\n", "Y2\nY2\n62\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg3?var1=xxx&var2=",
                    ExpectedOptions = new string[] { "Y1\nY1\n62\n\n", "Y2\nY2\n62\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg3?var1=xxx&",
                    ExpectedOptions = new string[] { "var1\nvar1\n57\n\n", "var2\nvar2\n57\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg3?var1=x",
                    ExpectedOptions = new string[] { "X1\nX1\n53\n\n", "X2\nX2\n53\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg3?v",
                    ExpectedOptions = new string[] { "var1\nvar1\n48\n\n", "var2\nvar2\n48\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg",
                    ExpectedOptions = new string[] { "C1\nC1\n40\n\n", "C2\nC2\n40\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2bbb/c",
                    ExpectedOptions = new string[] { "C1\nC1\n40\n\n", "C2\nC2\n40\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2b",
                    ExpectedOptions = new string[] { "B1\nB1\n36\n\n", "B2\nB2\n36\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s%25eg2",
                    ExpectedOptions = new string[] { "B1\nB1\n36\n\n", "B2\nB2\n36\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/aaa/s",
                    ExpectedOptions = new string[] { "s%25eg2\ns%25eg2\n29\n0\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1/",
                    ExpectedOptions = new string[] { "true\ntrue\n25\n\n", "false\nfalse\n25\n\n" }
                },
                new TestClientTestHelper.TextAndExpectedOptions() 
                {
                    Text = @"http://host/service/seg1",
                    ExpectedOptions = new string[] { "seg1\nseg1\n20\n\n" }
                }
            })});
        
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("TolerantUriTextReader is internal, disposable and concrete.")]
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
            Uri baseUri = new Uri(@"http://host/service", UriKind.Absolute);
            UriTemplate uriTemplate1 = new UriTemplate(@"seg1/{a}/s%25eg2{b}/{c}seg3?var1={x}&var2={y}");
            UriTemplate uriTemplate2 = new UriTemplate(@"/seg1/{a}/s%25eg2{b}/{c}seg3?var1={x}&var2={y}");

            MockHttpOperationDescription description = new MockHttpOperationDescription( new HttpParameter[]
            {
                new HttpParameter("a", typeof(bool)),
                new HttpParameter("b", typeof(EnumB)),
                new HttpParameter("c", typeof(EnumC)),
                new HttpParameter("x", typeof(EnumX)),
                new HttpParameter("y", typeof(EnumY)),
            });

            Asserters.Data.Execute(
                RepresentativeUriAndExpectedOptions,
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    using (TolerantUriTextReader reader = new TolerantUriTextReader(textAndExpectedOptions.Text, textAndExpectedOptions.Text.Length, baseUri, uriTemplate1, description))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }

                    using (TolerantUriTextReader reader = new TolerantUriTextReader(textAndExpectedOptions.Text, textAndExpectedOptions.Text.Length, baseUri, uriTemplate2, description))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("GetExpectedItems() returns available options when there is wildcard segment")]
        public void GetExpectedItemsReturnsAvailableOptionsWithWildcardSegment()
        {
            Uri baseUri = new Uri(@"http://host/service", UriKind.Absolute);
            UriTemplate uriTemplate1 = new UriTemplate(@"seg1/{a}/s%25eg2{b}/{*c}");
            UriTemplate uriTemplate2 = new UriTemplate(@"/seg1/{a}/s%25eg2{b}/{*c}");

            MockHttpOperationDescription description = new MockHttpOperationDescription( new HttpParameter[]
            {
                new HttpParameter("a", typeof(bool)),
                new HttpParameter("b", typeof(EnumB)),
                new HttpParameter("c", typeof(EnumC)),
            });

            Asserters.Data.Execute(
                new ReadOnlyCollection<TestData>(
                    new TestData[] { new RefTypeTestData<TestClientTestHelper.TextAndExpectedOptions>(() => new List<TestClientTestHelper.TextAndExpectedOptions>()
                    {
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = @"http://host/service/seg1/aaa/s%25eg2bbb/cccseg",
                            ExpectedOptions = new string[] { "C1\nC1\n40\n\n", "C2\nC2\n40\n\n" }
                        },
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = @"http://host/service/seg1/aaa/s%25eg2bbb/c",
                            ExpectedOptions = new string[] { "C1\nC1\n40\n\n", "C2\nC2\n40\n\n" }
                        },
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = @"http://host/service/seg1/aaa/s%25eg2bb",
                            ExpectedOptions = new string[] { "B1\nB1\n36\n\n", "B2\nB2\n36\n\n" }
                        },
                        new TestClientTestHelper.TextAndExpectedOptions() 
                        {
                            Text = @"http://host/service/seg1/aaa/s%25e",
                            ExpectedOptions = new string[] { "s%25eg2\ns%25eg2\n29\n0\n" }
                        },
                    })}),
                TestDataVariations.AsInstance,
                "Returned intellisense options is unexpected",
                (type, obj) =>
                {
                    TestClientTestHelper.TextAndExpectedOptions textAndExpectedOptions = (TestClientTestHelper.TextAndExpectedOptions)obj;
                    using (TolerantUriTextReader reader = new TolerantUriTextReader(textAndExpectedOptions.Text, textAndExpectedOptions.Text.Length, baseUri, uriTemplate1, description))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }

                    using (TolerantUriTextReader reader = new TolerantUriTextReader(textAndExpectedOptions.Text, textAndExpectedOptions.Text.Length, baseUri, uriTemplate2, description))
                    {
                        TestClientTestHelper.TestTolerantTextReader(textAndExpectedOptions, reader);
                    }
                });
        }
        
        #endregion Methods

        enum EnumB
        {
            B1,
            B2,
        }

        enum EnumC
        {
            C1,
            C2,
        }

        enum EnumX
        {
            X1,
            X2,
        }

        enum EnumY
        {
            Y1,
            Y2,
        }
    }
}