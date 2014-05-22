// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Net.Http.Test;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class ObjectContentTests: UnitTest<ObjectContent>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsDisposable,
                typeof(HttpContent));
        }

        #endregion Type

        #region Constructors

        #region ObjectContent(Type, object)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) sets Type (private) ObjectInstance properties.  ContentType defaults to null.")]
        public void Constructor()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MockObjectContent content = new MockObjectContent(type, obj);
                    Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                    Assert.IsNull(content.Headers.ContentType, "Content type should default to null.");
                    Assert.AreEqual(obj, content.ValueProperty, "Failed to set ObjectInstance");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) sets HttpContent property with object parameter==HttpContent.")]
        public void ConstructorSetsHttpContentWithHttpContentAsObject()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                MockObjectContent content = new MockObjectContent(typeof(string), (object)httpContent);
                Assert.AreEqual(httpContent, content.HttpContentProperty, "Failed to set HttpContent");
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) sets HttpContent property to a StreamContent when Object parameter is a Stream.")]
        public void ConstructorSetsHttpContentWithStreamAsObject()
        {
           Asserters.Serializer.UsingXmlSerializer<int>(
                5,
                (stream) =>
                {
                    MockObjectContent content = new MockObjectContent(typeof(int), stream);
                    StreamContent streamContent = content.HttpContentProperty as StreamContent;
                    Assert.IsNotNull(streamContent, "Stream was not wrapped in StreamContent.");
                    XmlSerializer serializer = new XmlSerializer(typeof(int));
                    int result = (int) serializer.Deserialize(streamContent.ReadAsStreamAsync().Result);
                    Assert.AreEqual(5, result, "Expected stream to deserialize to this value.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) throws for a null Type parameter.")]
        public void ConstructorThrowsWithNullType()
        {
            Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) accepts (typeof(void), (object)null) as a special case.")]
        public void ConstructorAllowsTypeVoidWithNullValue()
        {
            MockObjectContent content = new MockObjectContent(typeof(void), (object)null);
            Assert.AreSame(typeof(void), content.ObjectType, "Failed to set ObjectType");
            Assert.IsNull(content.Headers.ContentType, "Content type should default to null.");
            Assert.AreEqual(null, content.ValueProperty, "Failed to set ObjectInstance");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) throws for a null value type.")]
        public void ConstructorThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    Asserters.Exception.Throws<InvalidOperationException>(
                        SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                        () => new ObjectContent(type, (object)null));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object) throws if object is not assignable to Type.")]
        public void ConstructorThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                        Asserters.Exception.ThrowsArgument(
                            "value",
                            SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                            () => new ObjectContent(mismatchingType, obj));
                    }
                });
        }

        #endregion ObjectContent(Type, object)

        #region ObjectContent(Type, object, string)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) sets Type, content header's media type, and (private) ObjectInstance properties.")]
        public void Constructor1()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        MockObjectContent content = new MockObjectContent(type, obj, mediaType);
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                        Assert.AreEqual(obj, content.ValueProperty, "Failed to set ObjectInstance.");
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) sets HttpContent property with object parameter==HttpContent.")]
        public void Constructor1SetsHttpContentWithHttpContentAsObject()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MockObjectContent content = new MockObjectContent(typeof(string), (object)httpContent, mediaType);
                    Assert.AreEqual(httpContent, content.HttpContentProperty, "Failed to set HttpContent.");
                };
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) sets HttpContent property to a StreamContent when Object parameter is a Stream.")]
        public void Constructor1SetsHttpContentWithStreamAsObject()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
               Asserters.Serializer.UsingXmlSerializer<int>(
                    5,
                    (stream) =>
                    {
                        MockObjectContent content = new MockObjectContent(typeof(int), stream, mediaType);
                        StreamContent streamContent = content.HttpContentProperty as StreamContent;
                        Assert.IsNotNull(streamContent, "Stream was not wrapped in StreamContent.");
                        XmlSerializer serializer = new XmlSerializer(typeof(int));
                        int result = (int)serializer.Deserialize(streamContent.ReadAsStreamAsync().Result);
                        Assert.AreEqual(5, result, "Expected stream to deserialize to this value.");
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) throws for a null Type parameter.")]
        public void Constructor1ThrowsWithNullType()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5, mediaType));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) throws for a null value type.")]
        public void Constructor1ThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    Asserters.Exception.Throws<InvalidOperationException>(
                        SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                        () => new ObjectContent(type, (object)null, "application/xml"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) throws for an empty media type.")]
        public void Constructor1ThrowsWithEmptyMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in TestData.EmptyStrings)
                    {
                        Asserters.Exception.ThrowsArgumentNull(
                            "mediaType",
                            () => new ObjectContent(type, obj, mediaType));
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) throws for an empty media type.")]
        public void Constructor1ThrowsWithIllegalMediaType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.IllegalMediaTypeStrings)
                    {
                        Asserters.Exception.ThrowsArgument(
                            "mediaType",
                            SR.InvalidMediaType(mediaType, typeof(MediaTypeHeaderValue).Name),
                            () => new ObjectContent(type, obj, mediaType));
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string) throws if object is not assignable to Type.")]
        public void Constructor1ThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                        {
                            Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                            Asserters.Exception.ThrowsArgument(
                                "value",
                                SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                                () => new ObjectContent(mismatchingType, obj, mediaType));
                        }
                    }
                });
        }

        #endregion ObjectContent(Type, object, string)

        #region ObjectContent(Type, object, MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue) sets Type, content header's media type and (private) ObjectInstance properties.")]
        public void Constructor2()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        MockObjectContent content = new MockObjectContent(type, obj, mediaType);
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                        Assert.AreEqual(obj, content.ValueProperty, "Failed to set ObjectInstance");
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue) throws for a null Type parameter.")]
        public void Constructor2ThrowsWithNullType()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5, mediaType));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue) throws for a null value type.")]
        public void Constructor2ThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    Asserters.Exception.Throws<InvalidOperationException>(
                        SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                        () => new ObjectContent(type, (object)null, new MediaTypeHeaderValue("application/xml")));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue) throws for a null media type.")]
        public void Constructor2ThrowsWithNullMediaType()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "mediaType",
                () => new ObjectContent(typeof(int), 5, (MediaTypeHeaderValue)null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue) throws if object is not assignable to Type.")]
        public void Constructor2ThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                        {
                            Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                            Asserters.Exception.ThrowsArgument(
                                "value",
                                SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                                () => new ObjectContent(mismatchingType, obj, mediaType));
                        }
                    }
                });
        }

        #endregion ObjectContent(Type, object, MediaTypeHeaderValue)

        #region ObjectContent(Type, HttpContent)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent) sets Type and HttpContent properties and sets MediaType from HttpContent.")]
        public void Constructor3()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        MockObjectContent content = new MockObjectContent(type, httpContent);
                        Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                        Assert.AreSame(httpContent, content.HttpContentProperty, "Failed to set HttpContent");
                        Asserters.MediaType.AreEqual(content.Headers.ContentType, httpContent.Headers.ContentType, "MediaType was not set.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent) sets ContentHeaders from the HttpContent.")]
        public void Constructor3SetsContentHeadersWithHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        httpContent.Headers.Add("CIT-Header", "CIT-Value");
                        ObjectContent content = new ObjectContent(type, httpContent);
                        Asserters.Http.Contains(content.Headers, "CIT-Header", "CIT-Value");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent) throws for a null Type parameter.")]
        public void Constructor3ThrowsWithNullType()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, httpContent));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent) throws for a null HttpContent.")]
        public void Constructor3ThrowsWithNullHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Asserters.Exception.ThrowsArgumentNull(
                        "content",
                        () => new ObjectContent(type, (HttpContent)null));
                });
        }

        #endregion ObjectContent(Type, HttpContent)

        #region ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>) sets Type, Formatters and (private) ObjectInstance properties.  ContentType defaults to null.")]
        public void Constructor4()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MediaTypeFormatter[] formatters = DataSets.Http.StandardFormatters.ToArray();
                    MockObjectContent content = new MockObjectContent(type, obj, formatters);
                    Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType");
                    Assert.IsNull(content.Headers.ContentType, "Content type should default to null.");
                    Assert.AreEqual(obj, content.ValueProperty, "Failed to set Value");
                    CollectionAssert.AreEqual(formatters, content.Formatters, "Formatters should have been same as passed in to ctor.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>) does not add defaults to empty formatter list.")]
        public void Constructor4UsesEmptyFormatterCollection()
        {
            IEnumerable<Type> standardFormatterTypes = DataSets.Http.StandardFormatters.Select<MediaTypeFormatter,Type>((m) => m.GetType());

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = new ObjectContent(type, obj, new MediaTypeFormatter[0]);
                    Assert.AreEqual(0, content.Formatters.Count, "Default formatters were added to empty list.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>) throws for a null Type parameter.")]
        public void Constructor4ThrowsWithNullType()
        {
            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
            {
                Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5, formatters));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>) throws for null value type.")]
        public void Constructor4ThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    Asserters.Exception.Throws<InvalidOperationException>(
                        SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                        () => new ObjectContent(type, (object)null, new MediaTypeHeaderValue("application/xml")));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>) throws for a null formatter list.")]
        public void Constructor4ThrowsWithNullFormatters()
        {
            Asserters.Exception.ThrowsArgumentNull(
                "formatters",
                () => new ObjectContent(typeof(int), 5, (IEnumerable<MediaTypeFormatter>)null));
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object,  IEnumerable<MediaTypeFormatter>) throws if object is not assignable to Type.")]
        public void Constructor4ThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                            Asserters.Exception.ThrowsArgument(
                                "value",
                                SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                                () => new ObjectContent(mismatchingType, obj, formatters));
                        }
                    }
                });
        }

        #endregion ObjectContent(Type, object, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) sets Type, content header's media type, Formatters and (private) ObjectInstance properties.")]
        public void Constructor5()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            MockObjectContent content = new MockObjectContent(type, obj, mediaType, formatters);
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreEqual(obj, content.ValueProperty, "Failed to set Value.");
                            Assert.IsNotNull(content.Formatters, "Failed to set Formatters");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) sets HttpContent property with object parameter==HttpContent.")]
        public void Constructor5SetsHttpContentWithHttpContentAsObject()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                    {
                        MockObjectContent content = new MockObjectContent(typeof(string), (object)httpContent, mediaType, formatters);
                        Assert.AreEqual(httpContent, content.HttpContentProperty, "Failed to set HttpContent.");
                    }
                };
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) sets HttpContent property to a StreamContent when Object parameter is a Stream.")]
        public void Constructor5SetsHttpContentWithStreamAsObject()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                   Asserters.Serializer.UsingXmlSerializer<int>(
                        5,
                        (stream) =>
                        {
                            MockObjectContent content = new MockObjectContent(typeof(int), stream, mediaType, formatters);
                            StreamContent streamContent = content.HttpContentProperty as StreamContent;
                            Assert.IsNotNull(streamContent, "Stream was not wrapped in StreamContent.");
                            XmlSerializer serializer = new XmlSerializer(typeof(int));
                            int result = (int)serializer.Deserialize(streamContent.ReadAsStreamAsync().Result);
                            Assert.AreEqual(5, result, "Expected stream to deserialize to this value.");
                        });
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) throws for a null Type parameter.")]
        public void Constructor5ThrowsWithNullType()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                    Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5, mediaType, formatters));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) throws for a null value type object.")]
        public void Constructor5ThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            Asserters.Exception.Throws<InvalidOperationException>(
                                SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                                () => new ObjectContent(type, (object)null, mediaType, formatters));
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) throws with an empty media type.")]
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
                            Asserters.Exception.ThrowsArgumentNull(
                                "mediaType",
                                () => new ObjectContent(type, obj, mediaType, formatters));
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string,  IEnumerable<MediaTypeFormatter>) throws for an illegal media type.")]
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
                        Asserters.Exception.ThrowsArgument(
                            "mediaType",
                            SR.InvalidMediaType(mediaType, typeof(MediaTypeHeaderValue).Name),
                            () => new ObjectContent(type, obj, mediaType, formatters));
                    }
                };
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) throws with a null formatters.")]
        public void Constructor5ThrowsWithNullFormatters()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent(typeof(int), 5, mediaType, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>) throws if object is not assignable to Type.")]
        public void Constructor5ThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                        {
                            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                            {
                                Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                                Asserters.Exception.ThrowsArgument(
                                    "value",
                                    SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                                    () => new ObjectContent(mismatchingType, obj, mediaType, formatters));
                            }
                        }
                    }
                });
        }

        #endregion ObjectContent(Type, object, string, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) sets Type, content header's media type, Formatters and (private) ObjectInstance properties.")]
        public void Constructor6()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            MockObjectContent content = new MockObjectContent(type, obj, mediaType, formatters);
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreEqual(obj, content.ValueProperty, "Failed to set Value.");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, mediaType, "MediaType was not set.");
                            Assert.IsNotNull(content.Formatters, "Failed to set Formatters");
                        }
                    };
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) sets HttpContent property with object parameter==HttpContent.")]
        public void Constructor6SetsHttpContentWithHttpContentAsObject()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                    {
                        MockObjectContent content = new MockObjectContent(typeof(string), (object)httpContent, mediaType, formatters);
                        Assert.AreEqual(httpContent, content.HttpContentProperty, "Failed to set HttpContent.");
                    }
                };
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) sets HttpContent property to a StreamContent when Object parameter is a Stream.")]
        public void Constructor6SetsHttpContentWithStreamAsObject()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                   Asserters.Serializer.UsingXmlSerializer<int>(
                        5,
                        (stream) =>
                        {
                            MockObjectContent content = new MockObjectContent(typeof(int), stream, mediaType, formatters);
                            StreamContent streamContent = content.HttpContentProperty as StreamContent;
                            Assert.IsNotNull(streamContent, "Stream was not wrapped in StreamContent.");
                            XmlSerializer serializer = new XmlSerializer(typeof(int));
                            int result = (int)serializer.Deserialize(streamContent.ReadAsStreamAsync().Result);
                            Assert.AreEqual(5, result, "Expected stream to deserialize to this value.");
                        });
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws for a null Type parameter.")]
        public void Constructor6ThrowsWithNullType()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                    Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, 5, mediaType, formatters));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws for a null value type object.")]
        public void Constructor6ThrowsWithNullValueTypeObject()
        {
            Asserters.Data.Execute(
                TestData.ValueTypeTestDataCollection,
                TestDataVariations.AsInstance,
                "Null value types should throw",
                (type, obj) =>
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            Asserters.Exception.Throws<InvalidOperationException>(
                                SR.CannotUseNullValueType(typeof(ObjectContent).Name, type.Name),
                                () => new ObjectContent(type, (object)null, mediaType, formatters));
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws with an null media type.")]
        public void Constructor6ThrowsWithNullMediaType()
        {
            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "mediaType",
                    () => new ObjectContent(typeof(int), 5, (MediaTypeHeaderValue)null, formatters));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws with a null formatters.")]
        public void Constructor6ThrowsWithNullFormatters()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent(typeof(int), 5, mediaType, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>) throws if object is not assignable to Type.")]
        public void Constructor6ThrowsWithObjectNotAssignableToType()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (obj != null)
                    {
                        foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                        {
                            foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                            {
                                Type mismatchingType = (type == typeof(string)) ? typeof(int) : typeof(string);
                                Asserters.Exception.ThrowsArgument(
                                    "value",
                                    SR.ObjectAndTypeDisagree(obj.GetType().Name, mismatchingType.Name),
                                    () => new ObjectContent(mismatchingType, obj, mediaType, formatters));
                            }
                        }
                    }
                });
        }

        #endregion ObjectContent(Type, object, MediaTypeHeaderValue, IEnumerable<MediaTypeFormatter>)

        #region ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>) sets Type, HttpContent and Formatter properties, and sets MediaType from HttpContent.")]
        public void Constructor7()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            MockObjectContent content = new MockObjectContent(type, httpContent);
                            Assert.AreSame(type, content.ObjectType, "Failed to set ObjectType.");
                            Assert.AreSame(httpContent, content.HttpContentProperty, "Failed to set HttpContent.");
                            Assert.IsNotNull(content.Formatters, "Failed to set Formatters.");
                            Asserters.MediaType.AreEqual(content.Headers.ContentType, httpContent.Headers.ContentType, "MediaType was not set.");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>) sets content headers from input HttpContent.")]
        public void Constructor7SetsContentHeadersWithHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
                    {
                        foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                        {
                            httpContent.Headers.Add("CIT-Name", "CIT-Value");
                            ObjectContent content = new ObjectContent(type, httpContent);
                            Asserters.Http.Contains(content.Headers, "CIT-Name", "CIT-Value");
                        }
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>) throws for a null Type parameter.")]
        public void Constructor7ThrowsWithNullType()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                {
                    Asserters.Exception.ThrowsArgumentNull("type", () => new ObjectContent((Type)null, httpContent, formatters));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>) throws for a null HttpContent.")]
        public void Constructor7ThrowsWithNullHttpContent()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (IEnumerable<MediaTypeFormatter> formatters in DataSets.Http.AllFormatterCollections)
                    {
                        Asserters.Exception.ThrowsArgumentNull(
                            "content",
                            () => new ObjectContent(type, (HttpContent)null, formatters));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>) throws for a null formatters.")]
        public void Constructor7ThrowsWithNullFormatters()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                Asserters.Exception.ThrowsArgumentNull(
                    "formatters",
                    () => new ObjectContent(typeof(int), httpContent, (IEnumerable<MediaTypeFormatter>)null));
            }
        }

        #endregion ObjectContent(Type, HttpContent, IEnumerable<MediaTypeFormatter>)

        #endregion Constructors

        #region Properties

        #region Formatters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Formatters property returns mutable MediaTypeFormatter collection.")]
        public void FormattersReturnsMutableCollection()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            MediaTypeFormatterCollection collection = content.Formatters;
            Assert.IsNotNull(collection, "Formatters cannot be null.");

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            collection.Add(formatter);
            CollectionAssert.Contains(collection, formatter, "Collection should contain formatter we added.");
        }

        #endregion Formatters

        #region HttpRequestMessage (internal)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage internal property returns HttpRequestMessage if they are pairs.")]
        public void HttpRequestMessageGetsValueWithPairing()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = content;
            content.HttpRequestMessage = request;
            Assert.AreSame(request, content.HttpRequestMessage, "HttpRequestMessage property was not set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage internal property returns null if not paired with HttpRequestMessage.")]
        public void HttpRequestMessageGetsNullWithNoPairing()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpRequestMessage request = new HttpRequestMessage();
            content.HttpRequestMessage = request;
            Assert.IsNull(content.HttpRequestMessage, "HttpRequestMessage should be null if not paired.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage internal property returns a newly paired HttpRequestMessage.")]
        public void HttpRequestMessageGetsAlteredPairing()
        {
            ObjectContent content1 = new ObjectContent(typeof(string), "data");
            ObjectContent content2 = new ObjectContent(typeof(int), 5);
            HttpRequestMessage request = new HttpRequestMessage();

            // Pair and verify
            request.Content = content1;
            content1.HttpRequestMessage = request;
            Assert.AreSame(request, content1.HttpRequestMessage, "HttpRequestMessage property should have been its pair.");

            // Alter half of the pairing and verify the original pairing is gone
            request.Content = content2;
            Assert.IsNull(content1.HttpRequestMessage, "HttpRequestMessage should be null if altered pairing.");

            // Complete the other half of the pairing and verify it is intact
            content2.HttpRequestMessage = request;
            Assert.AreSame(request, content2.HttpRequestMessage, "HttpRequestMessage property should have been its pair.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpRequestMessage property setter clears the HttpResponseMessage property.")]
        public void HttpRequestMessageSetsHttpResponseMessageToNull()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpResponseMessage response = new HttpResponseMessage();
            content.HttpResponseMessage = response;
            response.Content = content;
            Assert.AreSame(response, content.HttpResponseMessage, "HttpResponseMessage should have been paired with content.");

            // Now repair the content with a requst
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http:/somehost");
            content.HttpRequestMessage = request;
            request.Content = content;

            Assert.AreSame(request, content.HttpRequestMessage, "HttpRequestMessage should have re-paired with content.");
            Assert.IsNull(content.HttpResponseMessage, "HttpResponseMessage should be null after setting HttpRequestMessage.");
        }

        #endregion HttpRequestMessage (internal)

        #region HttpResponseMessage (internal)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage property returns HttpResponseMessage if they are pairs.")]
        public void HttpResponseMessageGetsValueWithPairing()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = content;
            content.HttpResponseMessage = response;
            Assert.AreSame(response, content.HttpResponseMessage, "HttpResponseMessage property was not set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage property returns null if not paired with HttpResponseMessage.")]
        public void HttpResponseMessageGetsNullWithNoPairing()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpResponseMessage response = new HttpResponseMessage();
            content.HttpResponseMessage = response;
            Assert.IsNull(content.HttpResponseMessage, "HttpResponseMessage should be null if not paired.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage property returns a newly paired HttpResponseMessage.")]
        public void HttpResponseMessageGetsAlteredPairing()
        {
            ObjectContent content1 = new ObjectContent(typeof(string), "data");
            ObjectContent content2 = new ObjectContent(typeof(int), 5);
            HttpResponseMessage response = new HttpResponseMessage();

            // Pair and verify
            response.Content = content1;
            content1.HttpResponseMessage = response;
            Assert.AreSame(response, content1.HttpResponseMessage, "HttpResponseMessage property should have been its pair.");

            // Alter half of the pairing and verify the original pairing is gone
            response.Content = content2;
            Assert.IsNull(content1.HttpResponseMessage, "HttpResponseMessage should be null if altered pairing.");

            // Complete the other half of the pairing and verify it is intact
            content2.HttpResponseMessage = response;
            Assert.AreSame(response, content2.HttpResponseMessage, "HttpResponseMessage property should have been its pair.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpResponseMessage property setter clears the HttpRequestMessage property.")]
        public void HttpResponseMessageSetsHttpRequestMessageToNull()
        {
            ObjectContent content = new ObjectContent(typeof(string), "data");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http:/somehost");

            content.HttpRequestMessage = request;
            request.Content = content;
            Assert.AreSame(request, content.HttpRequestMessage, "HttpRequestMessage should have been paired with content.");

            // Now repair the content with a response
            HttpResponseMessage response = new HttpResponseMessage();
            content.HttpResponseMessage = response;
            response.Content = content;
            Assert.AreSame(response, content.HttpResponseMessage, "HttpResponseMessage should have re-paired with content.");
            Assert.IsNull(content.HttpRequestMessage, "HttpRequestMessage should be null after setting HttpResponseMessage.");
        }

        #endregion HttpResponseMessage (internal)

        #endregion Properties

        #region Methods

        #region DetermineWriteSerializerAndContentType()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("DetermineWriteSerializerAndContentType() internal method selects formatter and content type.")]
        public void DetermineWriteSerializerAndContentType()
        {
            ObjectContent content = new ObjectContent(typeof(int), 5);
            Assert.IsNull(content.Headers.ContentType, "ContentType should have initialized to null.");
            content.DetermineWriteSerializerAndContentType();
            Asserters.MediaType.AreEqual(XmlMediaTypeFormatter.DefaultMediaType, content.Headers.ContentType, "Should have selected XmlMediaTypeFormatter's content type.");
        }

        #endregion DetermineWriteSerializerAndContentType()

        #region CopyToAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CopyToAsync(Stream) public method calls SerializeToStreamAsync protected method.")]
        public void CopyToAsyncCallsSerializeToStreamAsync()
        {
            MockObjectContent content = new MockObjectContent(typeof(int), 5) { CallBase = true };
            bool serializeToStreamCalled = false;
            bool taskRan = false;

            content.SerializeToStreamAsyncCallback = (stream, transportContext) =>
            {
                serializeToStreamCalled = true;
                return Task.Factory.StartNew(() => { taskRan = true;  });
            };

            using (MemoryStream stream = new MemoryStream())
            {
                Task readTask = content.CopyToAsync(stream);
                Asserters.Task.Succeeds(readTask);
            }

            Assert.IsTrue(serializeToStreamCalled, "CopyToAsync did not call SerializeToStreamAsync.");
            Assert.IsTrue(taskRan, "CopyToAsync ran the Task returned.");
        }

        #endregion CopyToAsync()

        #region SerializeToStreamAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SerializeToStreamAsync(Stream, TransportContext) uses the XmlMediaTypeFormatter if no matching formatters are available.")]
        public void SerializeToStreamAsyncUsesXmlMediaTypeFormatterWithNoMatchingFormatters()
        {
            ObjectContent content = new ObjectContent(typeof(int), 5, new MediaTypeHeaderValue("application/unknown"));
            int returnedValue = Asserters.Stream.WriteAndReadResult<int>(
                (stream) => Asserters.Task.Succeeds(content.CopyToAsync(stream)),
                (stream) =>
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(int));
                    object result = xmlSerializer.Deserialize(stream);
                    Assert.IsNotNull(result, "XmlSerializer returned null result.");
                    return (int)result;
                });

            Assert.AreEqual(5, returnedValue, "XmlSerializer returned wrong value");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SerializeToStreamAsync(Stream, TransportContext) calls the registered MediaTypeFormatter for SupportedMediaTypes and OnWriteToStream.")]
        public void SerializeToStreamAsyncCallsFormatter()
        {
            MediaTypeHeaderValue customMediaType = new MediaTypeHeaderValue("application/mine");
            bool onWriteToStreamCalled = false;
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(customMediaType);
            formatter.CanWriteTypeCallback = (type) => true;
            formatter.OnWriteToStreamAsyncCallback =
                (type, obj, stream, headers, context) =>
                {
                    onWriteToStreamCalled = true;
                    return Task.Factory.StartNew(() => { });
                };
            formatter.OnGetResponseHeadersCallback = (type, mediaType, responseMessage) => null;

            ObjectContent objectContent = new ObjectContent(typeof(string), "data");
            objectContent.Headers.ContentType = customMediaType;
            MediaTypeFormatterCollection formatterCollection = objectContent.Formatters;
            formatterCollection.Clear();
            formatterCollection.Add(formatter);

            // This statement should call CanWriteType, get SupportedMediaTypes, discover the formatter and call it
            Task writeTask = null;

            // Wait for task to complete before StreamAssert disposes it
            Asserters.Stream.WriteAndRead(
                (stream) => writeTask = objectContent.CopyToAsync(stream),
                (stream) => { Asserters.Task.Succeeds(writeTask); }
                );

            Assert.IsTrue(onWriteToStreamCalled, "SerializeToStream did not call our formatter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SerializeToStreamAsync(Stream, TransportContext) with a Stream object does not call the formatter to serialize.")]
        public void SerializeToStreamAsyncDoesNotCallFormatter()
        {
            StringContent content = new StringContent("data");
            Collection<MediaTypeHeaderValue> mediaTypeCollection = new Collection<MediaTypeHeaderValue>() { content.Headers.ContentType };

            bool onWriteToStreamCalled = false;
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(content.Headers.ContentType);
            formatter.CanWriteTypeCallback = (type) => true;
            formatter.OnWriteToStreamAsyncCallback =
                (type, obj, stream, headers, context) =>
                {
                    onWriteToStreamCalled = true;
                    return Task.Factory.StartNew(() => { });
                };

            ObjectContent objectContent = new ObjectContent(typeof(string), content, new MediaTypeFormatter[] { formatter });

            Task writeTask = null;

            // Wait for task to complete before StreamAssert disposes it
            Asserters.Stream.WriteAndRead(
                (stream) => writeTask = objectContent.CopyToAsync(stream),
                (stream) => { Asserters.Task.Succeeds(writeTask); }
                );

            Assert.IsFalse(onWriteToStreamCalled, "SerializeToStream for a stream should not call the formatter.");
        }

        #endregion SerializeToStreamAsync()

        #region ReadAsAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() gets a Task<object> returning object instance provided to the constructor for all value and reference types.")]
        public void ReadAsAsyncGetsTaskOfObject()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = new ObjectContent(type, obj);
                    Task<object> task = content.ReadAsAsync();
                    Asserters.Task.ResultEquals(task, obj);
                });
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
                                ObjectContent contentWrappingStream = new ObjectContent(type, streamContent);
                                string errorMessage = SR.NoReadSerializerAvailable(typeof(MediaTypeFormatter).Name, type.Name, "application/unknownMediaType");
                                Asserters.Exception.Throws<InvalidOperationException>(
                                    "No formatters should throw.",
                                    errorMessage,
                                    () => contentWrappingStream.ReadAsAsync());
                            });
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() from an Xml Stream produced by ObjectContent.CopyToAsync() round-trips correctly.")]
        public void ReadAsAsyncFromStreamGetsRoundTrippedObject()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (!Asserters.Http.IsKnownUnserializable(type, obj) && Asserters.Http.CanRoundTrip(type))
                    {
                        ObjectContent content = new ObjectContent(type, obj, XmlMediaTypeFormatter.DefaultMediaType);
                        Asserters.Stream.WriteAndRead(
                            (stream) => Asserters.Task.Succeeds(content.CopyToAsync(stream)),
                            (stream) =>
                            {
                                StreamContent streamContent = new StreamContent(stream);
                                streamContent.Headers.ContentType = content.Headers.ContentType;
                                ObjectContent contentWrappingStream = new ObjectContent(type, streamContent);
                                object readObj = Asserters.Task.SucceedsWithResult(contentWrappingStream.ReadAsAsync());
                                Asserters.Data.AreEqual(obj, readObj, "Failed to round trip.");
                            });
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() calls the registered MediaTypeFormatter for SupportedMediaTypes and ReadFromStream.")]
        public void ReadAsAsyncCallsFormatter()
        {
            StringContent content = new StringContent("data");
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(content.Headers.ContentType);
            formatter.CanReadTypeCallback = (type) => true;
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) => Task.Factory.StartNew<object>(() => "mole data");

            ObjectContent objectContent = new ObjectContent(typeof(string), content);
            MediaTypeFormatterCollection formatterCollection = objectContent.Formatters;
            formatterCollection.Clear();
            formatterCollection.Add(formatter);

            // This statement should call CanReadType, get SupportedMediaTypes, discover the formatter and call it
            Task<object> readTask = objectContent.ReadAsAsync();
            object readObj = Asserters.Task.SucceedsWithResult(readTask);

            Assert.AreEqual("mole data", readObj, "ReadAsAsync did not return what the formatter returned.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsAsync() calls the registered MediaTypeFormatter for SupportedMediaTypes and ReadFromStream only once and then uses the cached value.")]
        public void ReadAsAsyncCallsFormatterOnceOnly()
        {
            MockHttpContent content = new MockHttpContent(new StringContent("data"));
            bool contentWasDisposed = false;
            content.DisposeCallback = (bool disposing) => contentWasDisposed = true;
            content.TryComputeLengthCallback = (out long length) => { length = "mole data".Length; return true; };

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(content.Headers.ContentType);
            formatter.CanReadTypeCallback = (type) => true;
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) => Task.Factory.StartNew<object>(() => "mole data");

            ObjectContent objectContent = new ObjectContent(typeof(string), content);
            MediaTypeFormatterCollection formatterCollection = objectContent.Formatters;
            formatterCollection.Clear();
            formatterCollection.Add(formatter);

            // This statement should call CanReadType, get SupportedMediaTypes, discover the formatter and call it
            Task<object> readTask = objectContent.ReadAsAsync();
            object readObj = Asserters.Task.SucceedsWithResult(readTask);
            Assert.AreEqual("mole data", readObj, "ReadAsAsync did not return what the formatter returned.");

            // 1st ReadAs should have cached the Value and disposed the wrapped HttpContent
            Assert.IsTrue(contentWasDisposed, "1st ReadAsAsync should have disposed the wrapped HttpContent.");

            // --- A 2nd call to ReadAsAsync should no longer interact with the stream ---

            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) =>
            {
                Assert.Fail("2nd read should not call formatter.ReadFromStreamAsync.");
                return null;
            };

            readTask = objectContent.ReadAsAsync();
            readObj = Asserters.Task.SucceedsWithResult(readTask);
            Assert.AreEqual("mole data", readObj, "2nd ReadAsAsync did not return cached value.");
        }

        #endregion ReadAsAsync()

        #region ReadAsOrDefaultAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsOrDefaultAsync() gets a Task<object> returning object instance provided to the constructor for all value and reference types.")]
        public void ReadAsOrDefaultAsyncGetsTaskOfObject()
        {
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    ObjectContent content = new ObjectContent(type, obj);
                    Task<object> task = content.ReadAsOrDefaultAsync();
                    Asserters.Task.ResultEquals(task, obj);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadAsOrDefaultAsync() gets Task yielding default value for all value and reference types if no formatter is available.")]
        public void ReadAsOrDefaultAsyncGetsDefaultWithNoFormatter()
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
                                ObjectContent contentWrappingStream = new ObjectContent(type, streamContent);
                                Task<object> readTask = contentWrappingStream.ReadAsOrDefaultAsync();
                                object readObj = Asserters.Task.SucceedsWithResult(readTask);
                                object defaultObj = DefaultValue(type);
                                Asserters.Data.AreEqual(defaultObj, readObj, "Failed to get default value.");
                            });
                    }
                });
        }

        #endregion ReadAsOrDefaultAsync()

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
