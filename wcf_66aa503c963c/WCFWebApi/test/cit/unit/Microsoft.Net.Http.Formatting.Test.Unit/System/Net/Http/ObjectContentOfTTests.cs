// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class ObjectContentOfTTests : UnitTest<ObjectContent<object>>
    {
        private static readonly Type objectContentOfTType = typeof(ObjectContent<>);
        private static readonly string readAsAsyncMethodName = "ReadAsAsync";
        private static readonly string readAsOrDefaultAsyncMethodName = "ReadAsOrDefaultAsync";

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T> is public, concrete, unsealed and generic.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest, 
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsGenericType | TypeAssert.TypeProperties.IsDisposable,
                typeof(ObjectContent));
        }

        #endregion Type

        #region Constructors

        #region ObjectContent<T>(T)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T) sets Type and (private) ObjectInstance properties with all known value and reference types.  ContentType defaults to null.")]
        public void Constructor()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(objectContentOfTType, type, new Type[] { type }, new object[] { obj });
                    Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                    Assert.IsNull(content.Headers.ContentType, "ContentType should default to null.");
                    Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set Value");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void ConstructorConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(objectContentOfTType, type, new Type[] { type }, new object[] { null });
                    Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                    Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T) throws with HttpContent as T.")]
        public void ConstructorThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                    SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                    () => Asserters.GenericType.InvokeConstructor(objectContentOfTType, httpContent.GetType(), new Type[] { httpContent.GetType() }, new object[] { httpContent }),
                    (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
            }
        }

        #endregion ObjectContent<T>(T)

        #region ObjectContent<T>(T, string)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string) sets Type, content header's media type and (private) ObjectInstance properties with all known value and reference types.")]
        public void Constructor1()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(string) },
                                                    new object[] { obj, mediaType });
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                        Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set Value.");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void Constructor1ConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(string) },
                                                    new object[] { null, mediaType });
                        Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                        Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string) throws with HttpContent as T.")]
        public void Constructor1ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                        SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                        () => Asserters.GenericType.InvokeConstructor(
                                objectContentOfTType, 
                                httpContent.GetType(), 
                                new Type[] { httpContent.GetType(), typeof(string) }, 
                                new object[] { httpContent, mediaType }),
                        (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string) throws for an empty media type.")]
        public void Constructor1ThrowsWithEmptyMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in TestData.EmptyStrings)
                    {
                        Asserters.Exception.Throws<TargetInvocationException, ArgumentNullException>(
                            null,
                            () => Asserters.GenericType.InvokeConstructor(
                                    objectContentOfTType,
                                    type,
                                    new Type[] { type, typeof(string) },
                                    new object[] { obj, mediaType }),
                            (ae) => Assert.AreEqual("mediaType", ae.ParamName, "ParamName in exception was incorrect."));

                    };
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string) throws with an illegal media type.")]
        public void Constructor1ThrowsWithIllegalMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.IllegalMediaTypeStrings)
                    {
                        Asserters.Exception.ThrowsArgument<TargetInvocationException>(
                            "mediaType",
                            SR.InvalidMediaType(mediaType, typeof(MediaTypeHeaderValue).Name),
                            () => Asserters.GenericType.InvokeConstructor(
                                    objectContentOfTType,
                                    type,
                                    new Type[] { type, typeof(string) },
                                    new object[] { obj, mediaType }));

                    };
            });
        }

        #endregion ObjectContent<T>(T, string)

        #region ObjectContent<T>(T, MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue) sets Type, content header's media type and (private) ObjectInstance properties with all known value and reference types.")]
        public void Constructor2()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(MediaTypeHeaderValue) },
                                                    new object[] { obj, mediaType });
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                        Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set Value.");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void Constructor2ConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(MediaTypeHeaderValue) },
                                                    new object[] { null, mediaType });
                        Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                        Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue) throws with HttpContent as T.")]
        public void Constructor2ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                        SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                        () => Asserters.GenericType.InvokeConstructor(
                                objectContentOfTType,
                                httpContent.GetType(),
                                new Type[] { httpContent.GetType(), typeof(MediaTypeHeaderValue) },
                                new object[] { httpContent, mediaType }),
                        (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue) throws for a null media type.")]
        public void Constructor2ThrowsWithNullMediaType()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "mediaType",
                () => new ObjectContent<int>(5, (MediaTypeHeaderValue)null));
        }

        #endregion ObjectContent<T>(T, MediaTypeHeaderValue)

        #region ObjectContent<T>(HttpContent)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent) sets Type, MediaType property for all known value and reference types and all standard HttpContent types.")]
        public void Constructor3()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { typeof(HttpContent) },
                                                    new object[] { httpContent });
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                        Assert.AreEqual(httpContent, MockObjectContent.GetHttpContentProperty(content), "Failed to set HttpContent");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, httpContent.Headers.ContentType, "MediaType was not set.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent) sets ContentHeaders with input HttpContent.")]
        public void Constructor3SetsContentHeadersWithHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        httpContent.Headers.Add("CIT-Name", "CIT-Value");
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { typeof(HttpContent) },
                                                    new object[] { httpContent });
                        Asserters.Http.Contains(content.Headers, "CIT-Name", "CIT-Value");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent) throws with HttpContent as T.")]
        public void Constructor3ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                    SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                    () => Asserters.GenericType.InvokeConstructor(
                            objectContentOfTType, 
                            httpContent.GetType(), 
                            new Type[] { typeof(HttpContent) }, 
                            new object[] { httpContent }),
                    (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent) throws with a null HttpContent.")]
        public void Constructor3ThrowsWithNullHttpContent()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "content",
                () => new ObjectContent<int>((HttpContent)null));
        }

        #endregion ObjectContent<T>(HttpContent)

        #region ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>) sets Type, Formatters and (private)  ObjectInstance properties with all known value and reference types.  ContentType defaults to null.")]
        public void Constructor4()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                    {
                        // eval to force stable instances
                        MediaTypeFormatter[] formatters = formatterCollection.ToArray();

                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                                    new object[] { obj, formatters });
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                        Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set value.");
                        Assert.IsNotNull(content.Formatters, "Failed to set Formatters.");
                        CollectionAssert.IsSubsetOf(formatters.ToList(), content.Formatters, "Formatters did not include all input formatters.");
                        Assert.IsNull(content.Headers.ContentType, "ContentType should default to null.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void Constructor4ConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
                    {
                        // eval to force stable instances
                        MediaTypeFormatter[] formatters = formatterCollection.ToArray();
                        ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                    objectContentOfTType,
                                                    type,
                                                    new Type[] { type, typeof(IEnumerable<MediaTypeFormatter>) },
                                                    new object[] { null, formatters });
                        Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                        Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                        Assert.IsNotNull(content.Formatters, "Failed to set Formatters.");
                        CollectionAssert.IsSubsetOf(formatters.ToList(), content.Formatters, "Formatters did not include all input formatters.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>) throws if formatters parameter is null.")]
        public void Constructor4ThrowsWithNullFormatters()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "formatters",
                () => new ObjectContent<int>(5, (IEnumerable<MediaTypeFormatter>)null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>) throws with HttpContent as T.")]
        public void Constructor4ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                    SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                    () => Asserters.GenericType.InvokeConstructor(
                            objectContentOfTType, 
                            httpContent.GetType(), 
                            new Type[] { httpContent.GetType() }, 
                            new object[] { httpContent }),
                    (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect"));
            }
        }

        #endregion ObjectContent<T>(T, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) sets Type, content header's media type, Formatters and (private) ObjectInstance properties with all known value and reference types.")]
        public void Constructor5()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();

                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { type, typeof(string), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { obj, mediaType, formatters });
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set Value.");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                            CollectionAssert.IsSubsetOf(formatters, content.Formatters, "Failed to use all formatters specified.");
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void Constructor5ConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();

                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { type, typeof(string), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { null, mediaType, formatters });
                            Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                            Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) throws with HttpContent as T.")]
        public void Constructor5ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                    {
                        Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                            SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                            () => Asserters.GenericType.InvokeConstructor(
                                    objectContentOfTType,
                                    httpContent.GetType(),
                                    new Type[] { httpContent.GetType(), typeof(string), typeof(IEnumerable<MediaTypeFormatter>) },
                                    new object[] { httpContent, mediaType, formatters }),
                            (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) throws for an empty media type.")]
        public void Constructor5ThrowsWithEmptyMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in TestData.EmptyStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            Asserters.Exception.Throws<TargetInvocationException, ArgumentNullException>(
                                null,
                                () => Asserters.GenericType.InvokeConstructor(
                                        objectContentOfTType,
                                        type,
                                        new Type[] { type, typeof(string), typeof(IEnumerable<MediaTypeFormatter>) },
                                        new object[] { obj, mediaType, formatters }),
                                (ae) => Assert.AreEqual("mediaType", ae.ParamName, "ParamName in exception was incorrect."));
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) throws with an illegal media type.")]
        public void Constructor5ThrowsWithIllegalMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.IllegalMediaTypeStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            Asserters.Exception.ThrowsArgument<TargetInvocationException>(
                                "mediaType",
                                SR.InvalidMediaType(mediaType, typeof(MediaTypeHeaderValue).Name),
                                () => Asserters.GenericType.InvokeConstructor(
                                        objectContentOfTType,
                                        type,
                                        new Type[] { type, typeof(string), typeof(IEnumerable<MediaTypeFormatter>) },
                                        new object[] { obj, mediaType, formatters }));
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>) throws if formatters parameter is null.")]
        public void Constructor5ThrowsWithNullFormatters()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent<int>(5, mediaType, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        #endregion ObjectContent<T>(T, string, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) sets Type, content header's media type, Formatters and (private) ObjectInstance properties with all known value and reference types.")]
        public void Constructor6()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();

                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { type, typeof(MediaTypeHeaderValue), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { obj, mediaType, formatters });
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreEqual(obj, MockObjectContent.GetValueProperty(content), "Failed to set ObjectInstance.");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                            CollectionAssert.IsSubsetOf(formatters, content.Formatters, "Failed to use all formatters specified.");
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) converts null value types to that type's default value and sets (private) ObjectInstance.")]
        public void Constructor6ConvertsNullValueTypeToDefault()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();

                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { type, typeof(MediaTypeHeaderValue), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { null, mediaType, formatters });
                            Assert.IsNotNull(MockObjectContent.GetValueProperty(content), "Setting null value type should have converted to default.");
                            Assert.AreEqual(type, MockObjectContent.GetValueProperty(content).GetType(), "Expected default object to be of generic parameter's type.");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws with HttpContent as T.")]
        public void Constructor6ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                    {
                        Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                            SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                            () => Asserters.GenericType.InvokeConstructor(
                                    objectContentOfTType,
                                    httpContent.GetType(),
                                    new Type[] { httpContent.GetType(), typeof(MediaTypeHeaderValue), typeof(IEnumerable<MediaTypeFormatter>) },
                                    new object[] { httpContent, mediaType, formatters }),
                            (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws for a null media type.")]
        public void Constructor6ThrowsWithNullMediaType()
        {
            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "mediaType",
                    () => new ObjectContent<int>(5, (MediaTypeHeaderValue)null, formatters));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws if formatters parameter is null.")]
        public void Constructor6ThrowsWithNullFormatters()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent<int>(5, mediaType, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        #endregion ObjectContent<T>(T, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent<T>(HttpContent,IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent, IEnumerable<MediaTypeFormatter>) sets HttpContent and Formatter properties for all known value and reference types and all standard HttpContent types.")]
        public void Constructor7()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();
                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { typeof(HttpContent), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { httpContent, formatters });
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreEqual(httpContent, MockObjectContent.GetHttpContentProperty(content), "Failed to set HttpContent.");
                            CollectionAssert.IsSubsetOf(formatters, content.Formatters, "Failed to use all input formatters.");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, httpContent.Headers.ContentType, "MediaType was not set.");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent, IEnumerable<MediaTypeFormatter>) sets ContentHeaders with input HttpContent.")]
        public void Constructor7SetsContentHeadersWithHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        httpContent.Headers.Add("CIT-Name", "CIT-Value");
                        foreach (IEnumerable<MediaTypeFormatter> formatterCollections in DataSets.Http.AllFormatterCollections)
                        {
                            MediaTypeFormatter[] formatters = formatterCollections.ToArray();
                            ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                        objectContentOfTType,
                                                        type,
                                                        new Type[] { typeof(HttpContent), typeof(IEnumerable<MediaTypeFormatter>) },
                                                        new object[] { httpContent, formatters });
                            Asserters.Http.Contains(content.Headers, "CIT-Name", "CIT-Value");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent, IEnumerable<MediaTypeFormatter>) throws with HttpContent as T.")]
        public void Constructor7ThrowsWithHttpContentAsT()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                    Asserters.Exception.Throws<TargetInvocationException, ArgumentException>(
                        SR.CannotUseThisParameterType(typeof(HttpContent).Name, typeof(ObjectContent).Name),
                        () => Asserters.GenericType.InvokeConstructor(
                                objectContentOfTType,
                                httpContent.GetType(),
                                new Type[] { typeof(HttpContent), typeof(IEnumerable<MediaTypeFormatter>) },
                                new object[] { httpContent, formatters }),
                        (ae) => Assert.AreEqual("type", ae.ParamName, "ParamName in exception was incorrect."));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent, IEnumerable<MediaTypeFormatter>) throws with a null HttpContent.")]
        public void Constructor7ThrowsWithNullHttpContent()
        {
            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "content",
                    () => new ObjectContent<int>((HttpContent)null, formatters));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent<T>(HttpContent, IEnumerable<MediaTypeFormatter>) throws with a null Formatters.")]
        public void Constructor7ThrowsWithNullFormatters()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent<int>(httpContent, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        #endregion ObjectContent<T>(HttpContent,IEnumerable<MediaTypeFormatter>)

        #endregion Constructors

        #region Methods

        #region ReadAsAsync

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() gets a Task<object> returning object instance provided to the constructor for all value and reference types.")]
        public void ReadAsAsyncGetsTaskOfObject()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(objectContentOfTType, type, new Type[] { type }, new object[] { obj });
                    Task task = Asserters.GenericType.InvokeMethod(content, readAsAsyncMethodName) as Task;
                    Asserters.Task.ResultEquals(task, obj);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() calls the registered MediaTypeFormatter for SupportedMediaTypes and ReadFromStream.")]
        public void ReadAsAsyncCallsFormatter()
        {
            MockHttpContent content = new MockHttpContent(new StringContent("data"));
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(content.Headers.ContentType);

            formatter.CanReadTypeCallback = (type) => true;
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) => Task.Factory.StartNew<object>(() => "mole data");

            ObjectContent<string> objectContent = new ObjectContent<string>(content);
            MediaTypeFormatterCollection formatterCollection = objectContent.Formatters;
            formatterCollection.Clear();
            formatterCollection.Add(formatter);

            // This statement should call CanReadType, get SupportedMediaTypes, discover the formatter and call it
            Task<string> readTask = objectContent.ReadAsAsync();
            string readObj = Asserters.Task.SucceedsWithResult<string>(readTask);

            Assert.AreEqual("mole data", readObj, "ReadAs did not return what the formatter returned.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() calls the registered MediaTypeFormatter for SupportedMediaTypes and ReadFromStream only once, and then uses the cached value.")]
        public void ReadAsAsyncCallsFormatterOnceOnly()
        {
            MockHttpContent content = new MockHttpContent(new StringContent("data"));
            bool contentWasDisposed = false;
            content.DisposeCallback = (bool disposing) => contentWasDisposed = true;
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(content.Headers.ContentType);

            formatter.CanReadTypeCallback = (type) => true;
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) => Task.Factory.StartNew<object>(() => "mole data");

            ObjectContent<string> objectContent = new ObjectContent<string>(content);
            MediaTypeFormatterCollection formatterCollection = objectContent.Formatters;
            formatterCollection.Clear();
            formatterCollection.Add(formatter);

            // This statement should call CanReadType, get SupportedMediaTypes, discover the formatter and call it
            Task<string> readTask = objectContent.ReadAsAsync();
            string readObj = Asserters.Task.SucceedsWithResult<string>(readTask);

            Assert.AreEqual("mole data", readObj, "ReadAs did not return what the formatter returned.");

            // 1st ReadAs should have cached the Value and disposed the wrapped HttpContent
            Assert.IsTrue(contentWasDisposed, "1st ReadAs should have disposed the wrapped HttpContent.");

            // --- 2nd ReadAs should use cached value and not interact with formatter ---
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) =>
            {
                Assert.Fail("2nd read should not call formatter.ReadFromStreamAsync.");
                return null;
            };

            readTask = objectContent.ReadAsAsync();
            readObj = Asserters.Task.SucceedsWithResult<string>(readTask);
            Assert.AreEqual("mole data", readObj, "ReadAs did not return the cached value.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() throws for all value and reference types if no formatter is available.")]
        public void ReadAsAsyncThrowsWithNoFormatter()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (!Asserters.Http.IsKnownUnserializable(type, obj) && Asserters.Http.CanRoundTrip(type))
                    {
                       Asserters.Serializer.UsingXmlSerializer(
                           type,
                           obj,
                           (stream) =>
                           {
                               StreamContent streamContent = new StreamContent(stream);
                               streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/unknownMediaType");

                               // ObjectContent contentWrappingStream = new ObjectContent<T>(streamContent);
                               ObjectContent wrappingContent = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                                   objectContentOfTType,
                                                                   type,
                                                                   streamContent);

                               string errorMessage = SR.NoReadSerializerAvailable(typeof(MediaTypeFormatter).Name, type.Name, "application/unknownMediaType");
                               Asserters.Exception.Throws<TargetInvocationException, InvalidOperationException>(
                                   errorMessage,
                                   // wrappingContent.ReadAsObj()
                                   () => Asserters.GenericType.InvokeMethod(wrappingContent, readAsAsyncMethodName));
                           });
                    }
                });
        }

        #endregion ReadAsAsync

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsOrDefaultAsync() gets a Task<object> returning object instance provided to the constructor for all value and reference types.")]
        public void ReadAsOrDefaultAsyncGetsTaskOfObject()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = Asserters.GenericType.InvokeConstructor<ObjectContent>(objectContentOfTType, type, new Type[] { type }, new object[] { obj });
                    Task task = Asserters.GenericType.InvokeMethod(content, readAsOrDefaultAsyncMethodName) as Task;
                    Asserters.Task.ResultEquals(task, obj);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsOrDefaultAsync() returns Task yielding default value if no formatters are available.")]
        public void ReadAsOrDefaultAsyncReturnsDefaultWithNoFormatter()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (!Asserters.Http.IsKnownUnserializable(type, obj) && Asserters.Http.CanRoundTrip(type))
                    {
                       Asserters.Serializer.UsingXmlSerializer(
                            type, 
                            obj,
                            (stream) =>
                            {
                                StreamContent streamContent = new StreamContent(stream);
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/unknownMediaType");

                                // ObjectContent contentWrappingStream = new ObjectContent<T>(streamContent);
                                ObjectContent wrappingContent = Asserters.GenericType.InvokeConstructor<ObjectContent>(
                                                                    objectContentOfTType,
                                                                    type,
                                                                    streamContent);

                                Task readTask = Asserters.GenericType.InvokeMethod(wrappingContent, readAsOrDefaultAsyncMethodName) as Task;
                                Assert.IsNotNull(readTask, "Should have returned a Task.");
                                object readObj = Asserters.Task.SucceedsWithResult(readTask);
                                object defaultObj = DefaultValue(type);
                                Asserters.Data.AreEqual(defaultObj, readObj, "Did not read default value.");
                            });
                    }
                });
        }

        #endregion Methods

        #region Test helpers

        private static object DefaultValue(Type type)
        {
            Assert.IsNotNull(type, "type cannot be null.");
            if (!type.IsValueType)
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }

        #endregion TestHelpers
    }
}
