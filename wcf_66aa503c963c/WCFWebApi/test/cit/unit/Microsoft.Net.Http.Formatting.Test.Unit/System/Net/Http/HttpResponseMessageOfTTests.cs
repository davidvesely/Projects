// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Net.Http.Formatting;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpResponseMessageOfTTests : UnitTest<HttpResponseMessage<object>>
    {
        private static readonly Type objectContentOfTType = typeof(ObjectContent<>);
        private static readonly Type httpResponseMessageOfTType = typeof(HttpResponseMessage<>);

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T> is public, concrete, unsealed and generic.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsGenericType | TypeAssert.TypeProperties.IsDisposable,
                typeof(HttpResponseMessage));
        }

        #endregion Type

        #region Constructors

        #region HttpResponseMessage<T>(HttpStatusCode)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(HttpStatusCode) sets StatusCode with all known value and reference types.")]
        public void Constructor()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpStatusCode statusCode in DataSets.Http.AllHttpStatusCodes)
                    {
                        HttpResponseMessage response = Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                                            httpResponseMessageOfTType,
                                                            type,
                                                            statusCode);

                        Asserters.GenericType.IsCorrectGenericType<HttpResponseMessage>(response, type);
                        Assert.IsNotNull(response.Content, "default contructor should have set Content.");
                        Assert.AreEqual(statusCode, response.StatusCode, "StatusCode was not set.");
                    }
                });
        }

        #endregion HttpResponseMessage<T>(HttpStatusCode)

        #region HttpResponseMessage<T>(T)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T) sets Content property with all known value and reference types.")]
        public void Constructor1()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    HttpResponseMessage response = Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                                        httpResponseMessageOfTType,
                                                        type,
                                                        new Type[] { type },
                                                        new object[] { obj });

                    Asserters.GenericType.IsCorrectGenericType<HttpResponseMessage>(response, type);
                    Asserters.ObjectContent.IsCorrectGenericType(response.Content as ObjectContent, type);
                });
        }

        #endregion HttpResponseMessage<T>(T)

        #region HttpResponseMessage<T>(T, HttpStatusCode)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T, HttpStatusCode) sets Content and StatusCode properties with all known value and reference types.")]
        public void Constructor2()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpStatusCode statusCode in DataSets.Http.AllHttpStatusCodes)
                    {
                        HttpResponseMessage response = Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                                            httpResponseMessageOfTType,
                                                            type,
                                                            new Type[] { type, typeof(HttpStatusCode) },
                                                            new object[] { obj, statusCode });

                        Asserters.GenericType.IsCorrectGenericType<HttpResponseMessage>(response, type);
                        Asserters.ObjectContent.IsCorrectGenericType(response.Content as ObjectContent, type);
                        Assert.AreEqual(statusCode, response.StatusCode, "StatusCode was not set.");
                    }
                });
        }

        #endregion HttpResponseMessage<T>(T, HttpStatusCode)

        #region HttpResponseMessage<T>(T, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T, IEnumerable<MediaTypeFormatter>) sets Content property with all known value and reference types.")]
        public void Constructor3()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                    {
                        MediaTypeFormatter[] formatters = formatterCollection.ToArray();
                        HttpResponseMessage response = Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                                            httpResponseMessageOfTType,
                                                            type,
                                                            new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                                            new object[] { obj, formatters });

                        Asserters.GenericType.IsCorrectGenericType<HttpResponseMessage>(response, type);
                        Asserters.ObjectContent.IsCorrectGenericType(response.Content as ObjectContent, type);
                        Asserters.ObjectContent.ContainsFormatters(response.Content as ObjectContent, formatters);
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T, IEnumerable<MediaTypeFormatter>) throws with null formatters parameter.")]
        public void Constructor3ThrowsWithNullFormatters()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                        Asserters.Exception.ThrowsArgumentNull<TargetInvocationException>(
                            "formatters",
                            () => Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                    httpResponseMessageOfTType,
                                    type,
                                    new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                    new object[] { obj, null }));
                });
        }

        #endregion HttpResponseMessage<T>(T, IEnumerable<MediaTypeFormatter>)

        #region  HttpResponseMessage<T>(T, HttpStatusCode, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T, HttpStatusCode, IEnumerable<MediaTypeFormatter>) sets Content and StatusCode properties with all known value and reference types.")]
        public void Constructor4()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpStatusCode statusCode in DataSets.Http.AllHttpStatusCodes)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollection.ToArray();
                            HttpResponseMessage response = Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                                            httpResponseMessageOfTType,
                                                            type,
                                                            new Type[] { type, typeof(HttpStatusCode), typeof(IEnumerable<MediaTypeFormatter>) },
                                                            new object[] { obj, statusCode, formatters });

                            Assert.AreEqual(statusCode, response.StatusCode, "StatusCode was not set.");
                            Asserters.GenericType.IsCorrectGenericType<HttpResponseMessage>(response, type);
                            Asserters.ObjectContent.IsCorrectGenericType(response.Content as ObjectContent, type);
                            Asserters.ObjectContent.ContainsFormatters(response.Content as ObjectContent, formatters);
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage<T>(T, HttpStatusCode, IEnumerable<MediaTypeFormatter>) throws with null formatters parameter.")]
        public void Constructor4ThrowsWithNullFormatters()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpStatusCode statusCode in DataSets.Http.AllHttpStatusCodes)
                    {
                        Asserters.Exception.ThrowsArgumentNull<TargetInvocationException>(
                            "formatters",
                            () => Asserters.GenericType.InvokeConstructor<HttpResponseMessage>(
                                    httpResponseMessageOfTType,
                                    type,
                                    new Type[] { type, typeof(HttpStatusCode), typeof(IEnumerable<MediaTypeFormatter>) },
                                    new object[] { obj, statusCode, null }));
                    }
                });
        }

        #endregion HttpResponseMessage<T>(T, HttpStatusCode, IEnumerable<MediaTypeFormatter>)

        #endregion Constructors

        #region Properties

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content is settable.")]
        public void ContentIsSettable()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpResponseMessage<string> response = new HttpResponseMessage<string>("data");
            response.Content = content;
            Assert.AreSame(content, response.Content, "Content was not set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content setter sets Content.HttpResponseMessage to maintain pairing.")]
        public void ContentSetterSetsContentHttpResponseMessage()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpResponseMessage<string> response = new HttpResponseMessage<string>("data");
            response.Content = content;
            Assert.AreSame(response, content.HttpResponseMessage, "Failed to set Content.HttpResponseMessage.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content getter sets Content.HttpResponseMessage if not set.")]
        public void ContentGetterSetsContentHttpResponseMessage()
        {
            ObjectContent<string> content = new ObjectContent<string>("data");
            HttpResponseMessage<string> response = new HttpResponseMessage<string>("data");

            // assign via base HttpResponseMessage to bypass our strongly typed setter
            HttpResponseMessage baseResponse = (HttpResponseMessage)response;
            baseResponse.Content = content;
            Assert.IsNull(content.HttpResponseMessage, "Content.HttpResponseMessage should be null before it is automatically repaired.");
            Assert.AreSame(response, response.Content.HttpResponseMessage, "Content.HttpResponseMessage should have been set via getter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Content getter discovers non-ObjectContent<T> types and wraps them in ObjectContent<T>.")]
        public void ContentGetterSetsNewObjectContentOfTWithBaseHttpContent()
        {
            StringContent stringContent = new StringContent("data");
            HttpResponseMessage<string> response = new HttpResponseMessage<string>("data");

            // assign via base HttpResponseMessage to bypass our strongly typed setter
            HttpResponseMessage baseResponse = (HttpResponseMessage)response;
            baseResponse.Content = stringContent;

            ObjectContent<string> objectContent = response.Content;
            Assert.IsNotNull(objectContent, "Failed to create wrapper ObjectContent<T>");
            Assert.AreSame(response, objectContent.HttpResponseMessage, "Failed to pair new wrapper.");
        }

        #endregion Properties
    }
}
