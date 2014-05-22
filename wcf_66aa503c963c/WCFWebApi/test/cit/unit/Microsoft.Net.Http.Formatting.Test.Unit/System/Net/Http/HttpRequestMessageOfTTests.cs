// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Net.Http.Formatting;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpRequestMessageOfTTests : UnitTest<HttpRequestMessage<object>>
    {
        private static readonly Type objectContentOfTType = typeof(ObjectContent<>);
        private static readonly Type httpRequestMessageOfTType = typeof(HttpRequestMessage<>);

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T> is public, concrete, unsealed and generic.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest, 
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsGenericType | TypeAssert.TypeProperties.IsDisposable,
                typeof(HttpRequestMessage));
        }

        #endregion Type

        #region Constructors

        #region HttpRequestMessage<T>()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>() works with all known value and reference types.")]
        public void Constructor()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                    httpRequestMessageOfTType, 
                                                    type);

                    Asserters.GenericType.IsCorrectGenericType<HttpRequestMessage>(request, type);
                    Assert.IsNotNull(request.Content, "default contructor should have set Content.");
                });
        }

        #endregion HttpRequestMessage<T>()

        #region HttpRequestMessage<T>(T)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>(T) sets Content property with all known value and reference types.")]
        public void Constructor1()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                    httpRequestMessageOfTType,
                                                    type,
                                                    new Type[] { type },
                                                    new object[] { obj });

                    Asserters.GenericType.IsCorrectGenericType<HttpRequestMessage>(request, type);
                    Asserters.ObjectContent.IsCorrectGenericType(request.Content as ObjectContent, type);
                });
        }

        #endregion HttpRequestMessage<T>(T)

        #region HttpRequestMessage<T>(T, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>(T, IEnumerable<MediaTypeFormatter>) sets Content property with all known value and reference types.")]
        public void Constructor2()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                    {
                        MediaTypeFormatter[] formatters = formatterCollection.ToArray();
                        HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                        httpRequestMessageOfTType,
                                                        type,
                                                        new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { obj, formatters });

                        Asserters.GenericType.IsCorrectGenericType<HttpRequestMessage>(request, type);
                        Asserters.ObjectContent.IsCorrectGenericType(request.Content as ObjectContent, type);
                        Asserters.ObjectContent.ContainsFormatters(request.Content as ObjectContent, formatters);
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>(T, IEnumerable<MediaTypeFormatter>) throws with null formatters parameter.")]
        public void Constructor2ThrowsWithNullFormatters()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Asserters.Exception.ThrowsArgumentNull<TargetInvocationException>(
                        "formatters",
                    () =>
                    {
                        HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                        httpRequestMessageOfTType,
                                                        type,
                                                        new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { obj, null });
                    });
                });
        }

        #endregion HttpRequestMessage<T>(T, IEnumerable<MediaTypeFormatter>)

        #region HttpRequestMessage<T>(T, HttpMethod, Uri, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>(T, HttpMethod, Uri, IEnumerable<MediaTypeFormatter>) sets Content, Method and Uri properties with all known value and reference types.")]
        public void Constructor3()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpMethod httpMethod in DataSets.Http.AllHttpMethods)
                    {
                        foreach (Uri uri in DataSets.WCF.Uris)
                        {
                            foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                            {
                                MediaTypeFormatter[] formatters = formatterCollection.ToArray();
                                HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                                httpRequestMessageOfTType,
                                                                type,
                                                                new Type[] { type, typeof(HttpMethod), typeof(Uri), typeof(IEnumerable<MediaTypeFormatter>) },
                                                                new object[] { obj, httpMethod, uri, formatters });

                                Asserters.GenericType.IsCorrectGenericType<HttpRequestMessage>(request, type);
                                Assert.AreEqual(uri, request.RequestUri, "Uri property was not set.");
                                Assert.AreEqual(httpMethod, request.Method, "Method property was not set.");
                                Asserters.ObjectContent.IsCorrectGenericType(request.Content as ObjectContent, type);
                                Asserters.ObjectContent.ContainsFormatters(request.Content as ObjectContent, formatters);
                            }
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage<T>(T, HttpMethod, Uri, IEnumerable<MediaTypeFormatter>) throws with null formatters.")]
        public void Constructor3ThrowsWithNullFormatters()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpMethod httpMethod in DataSets.Http.AllHttpMethods)
                    {
                        foreach (Uri uri in DataSets.WCF.Uris)
                        {
                            Asserters.Exception.ThrowsArgumentNull<TargetInvocationException>(
                                "formatters",
                                () =>
                                {
                                    HttpRequestMessage request = Asserters.GenericType.InvokeConstructor<HttpRequestMessage>(
                                                                    httpRequestMessageOfTType,
                                                                    type,
                                                                    new Type[] { type, typeof(HttpMethod), typeof(Uri), typeof(IEnumerable<MediaTypeFormatter>) },
                                                                    new object[] { obj, httpMethod, uri, null });
                                });
                        }
                    }
                });
        }

        #endregion HttpRequestMessage<T>(T, HttpMethod, Uri, IEnumerable<MediaTypeFormatter>)

        #endregion Constructors

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content is settable.")]
        public void ContentIsSettable()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpRequestMessage<string> request = new HttpRequestMessage<string>();
            request.Content = content;
            Assert.AreSame(content, request.Content, "Content was not set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content setter sets Content.HttpRequestMessage to maintain pairing.")]
        public void ContentSetterSetsContentHttpRequestMessage()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpRequestMessage<string> request = new HttpRequestMessage<string>();
            request.Content = content;
            Assert.AreSame(request, content.HttpRequestMessage, "Failed to set Content.HttpRequestMessage.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content getter sets Content.HttpRequestMessage if not set.")]
        public void ContentGetterSetsContentHttpRequestMessage()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpRequestMessage<string> request = new HttpRequestMessage<string>();

            // assign via base HttpRequestMessage to bypass our strongly typed setter
            HttpRequestMessage baseRequest = (HttpRequestMessage)request;
            baseRequest.Content = content;
            Assert.IsNull(content.HttpRequestMessage, "Content.HttpRequestMessage should be null before it is automatically repaired.");
            Assert.AreSame(request, request.Content.HttpRequestMessage, "Content.HttpRequestMessage should have been set via getter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content getter discovers non-ObjectContent<T> types and wraps them in ObjectContent<T>.")]
        public void ContentGetterSetsNewObjectContentOfTWithBaseHttpContent()
        {
            StringContent stringContent = new StringContent("data");
            HttpRequestMessage<string> request = new HttpRequestMessage<string>();

            // assign via base HttpRequestMessage to bypass our strongly typed setter
            HttpRequestMessage baseRequest = (HttpRequestMessage)request;
            baseRequest.Content = stringContent;

            ObjectContent<string> objectContent = request.Content;
            Assert.IsNotNull(objectContent, "Failed to create wrapper ObjectContent<T>");
            Assert.AreSame(request, objectContent.HttpRequestMessage, "Failed to pair new wrapper.");
        }

        #endregion Properties
    }
}
