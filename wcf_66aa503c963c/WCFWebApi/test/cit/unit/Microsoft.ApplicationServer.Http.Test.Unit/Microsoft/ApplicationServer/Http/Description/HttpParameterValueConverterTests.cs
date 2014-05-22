// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Net.Http;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.ApplicationServer.Common.Test.Mocks;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(HttpParameterValueConverter))]
    public class HttpParameterValueConverterTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpParameverValueConverter is internal, and abstract.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpParameterValueConverter() sets Type for all value types.")]
        public void Constructor()
        {
            foreach (TestData testData in DataSets.Http.ConvertableValueTypes)
            {
                MockHttpParameterValueConverter converter = new MockHttpParameterValueConverter(testData.Type);
                Assert.IsNotNull(converter.Type, "Converter failed to set Type.");
                Assert.IsTrue(converter.Type.IsAssignableFrom(testData.Type), string.Format("Converter type {0} was not assignable from test data type {1}", converter.Type.Name, testData.Type.Name));
            }
        }

        #endregion Constructors

        #region Properties

        #region CanConvertFromString

        #region CanConvertFromString for value types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromString returns true for all value types converters.")]
        public void CanConvertFromStringReturnsTrueForT()
        {
            foreach (TestData testData in DataSets.Http.ConvertableValueTypes)
            {
                HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(testData.Type);
                if (!converter.CanConvertFromString)
                {
                    Assert.Fail(string.Format("CanConvertFromString was wrong for {0}.", testData.Type.Name));
                }
            }
        }

        #endregion CanConvertFromString for value types

        #region CanConvertFromString for reference types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromString returns false for ObjectContent converters.")]
        public void CanConvertFromStringReturnsFalseForObjectContent()
        {
            ObjectContent objectContent = new ObjectContent<int>(5);
            HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
            if (converter.CanConvertFromString)
            {
                Assert.Fail(string.Format("CanConvertFromString was wrong for ObjectContent."));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromString returns false for HttpRequestMessage converters.")]
        public void CanConvertFromStringReturnsFalseForHttpRequestMessage()
        {
            HttpRequestMessage request = new HttpRequestMessage<int>();
            HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(request.GetType());
            if (converter.CanConvertFromString)
            {
                Assert.Fail(string.Format("CanConvertFromString was wrong for HttpRequestMessage."));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromString returns false for HttpResponseMessage converters.")]
        public void CanConvertFromStringReturnsFalseForHttpResponseMessage()
        {
            HttpResponseMessage response = new HttpResponseMessage<int>(5);
            HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(response.GetType());
            if (converter.CanConvertFromString)
            {
                Assert.Fail(string.Format("CanConvertFromString was wrong for HttpResponseMessage."));
            }
        }

        #endregion CanConvertFromString for reference types

        #endregion CanConvertFromString

        #endregion Properties

        #region Methods

        #region GetValueConverter()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) throws with null type.")]
        public void GetValueConverterThrowsWithNullType()
        {
            Asserters.Exception.ThrowsArgumentNull("type", () => HttpParameterValueConverter.GetValueConverter(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) returns a converter for all values type.")]
        public void GetValueConverterReturnsConverterForAllValueTypes()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances | TestDataVariations.AsNullable,
                "GetValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                    Assert.IsNotNull(converter, "GetValueConverter returned null.");
                    Assert.AreEqual(convertType, converter.Type, "Converter Type was not correct.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) returns a converter for all HttpContent types.")]
        public void GetValueConverterHttpContent()
        {
            foreach (HttpContent httpContent in DataSets.Http.StandardHttpContents)
            {
                HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(httpContent.GetType());
                Assert.IsNotNull(converter, "GetValueConverter returned null.");
                Assert.AreEqual(httpContent.GetType(), converter.Type, "Converter Type was not correct.");
            }
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) returns a converter for all ObjectContent<T> types.")]
        public void GetValueConverterObjectContentOfT()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                    Assert.AreEqual(objectContent.GetType(), converter.Type, "Converter Type is wrong.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) returns a converter for all HttpRequestMessage<T> types.")]
        public void GetValueConverterHttpRequestMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(request.GetType());
                    Assert.AreEqual(request.GetType(), converter.Type, "Converter Type is wrong.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("GetValueConverter(Type) returns a converter for all HttpResponseMessage<T> types.")]
        public void GetValueConverterHttpResponseMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(response.GetType());
                    Assert.AreEqual(response.GetType(), converter.Type, "Converter Type is wrong.");
                });
        }

        #endregion GetValueConverter()

        #region CanConvertFromType()

        #region CanConvertFromType(Type) for value types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) true for all value types' own types.")]
        public void CanConvertFromTypeReturnsTrueForTtoT()
        {
            foreach (TestData testData in DataSets.Http.ConvertableValueTypes)
            {
                HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(testData.Type);
                if (!converter.CanConvertFromType(testData.Type))
                {
                    Assert.Fail(string.Format("CanConvertFromType was wrong for {0}.", testData.Type.Name));
                }
            }
        }

        #endregion CanConvertFromType(Type) for value types

        #region CanConvertFrom(Type) using ObjectContent<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns false for T to ObjectContent<T>.")]
        public void CanConvertFromTypeReturnsFalseForTtoObjectContentOfT()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                    if (converter.CanConvertFromType(objectContent.ObjectType))
                    {
                        Assert.Fail(string.Format("CanConvertFromType failed for {0}.", type));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) throws for null Type using ObjectContent<T>.")]
        public void CanConvertFromTypeThrowsWithNullTypeForTtoObjectContentOfT()
        {
            ObjectContent objectContent = new ObjectContent<int>(5);
            HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
            Asserters.Exception.ThrowsArgumentNull("type", () => converter.CanConvertFromType(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns true for ObjectContent<T> to T.")]
        public void CanConvertFromTypeReturnsTrueForObjectContentOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.ObjectType);
                    if (!converter.CanConvertFromType(objectContent.GetType()))
                    {
                        Assert.Fail(string.Format("CanConvertFromType failed for {0}.", objectContent.ObjectType));
                    }
                });
        }

        #endregion CanConvertFrom(Type) using ObjectContent<T>

        #region CanConvertFromType(Type) using HttpRequestMessage<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns false for T to HttpRequestMessage<T>.")]
        public void CanConvertFromTypeReturnsFalseForTtoHttpRequestMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(request.GetType());
                    if (converter.CanConvertFromType(convertType))
                    {
                        Assert.Fail(string.Format("CanConvertFromType failed for {0}.", convertType));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns true for HttpRequesteMessage<T> to T.")]
        public void CanConvertFromTypeReturnsTrueForHttpRequestMessageOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                    if (!converter.CanConvertFromType(request.GetType()))
                    {
                        Assert.Fail(string.Format("CanConvertFromType failed for {0}.", convertType));
                    }
                });
        }

        #endregion CanConvertFromType(Type) using HttpRequestMessage<T>

        #region CanConvertFromType(Type) using HttpResponseMessage<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns false for T to HttpResponseMessage<T>.")]
        public void CanConvertFromTypeReturnsFalseForTtoHttpResponseMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
               DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
               TestDataVariations.All,
               (response, type, obj) =>
               {
                   Type convertType = obj == null ? type : obj.GetType();
                   HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(response.GetType());
                    if (converter.CanConvertFromType(convertType))
                    {
                        Assert.Fail(string.Format("CanConvertFromType failed for {0}.", convertType));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanConvertFromType(Type) returns true for HttpResponseMessage<T> to T.")]
        public void CanConvertFromTypeReturnsTrueForHttpResponseMessageOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
             DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
             TestDataVariations.All,
             (response, type, obj) =>
             {
                 Type convertType = obj == null ? type : obj.GetType();
                 HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                 if (!converter.CanConvertFromType(response.GetType()))
                 {
                     Assert.Fail(string.Format("CanConvertFromType failed for {0}.", convertType));
                 }
             });
        }

        #endregion CanConvertFromType(Type) using HttpResponseMessage<T>

        #endregion CanConvertFromType()

        #region Convert(object)

        #region Convert(object) for value types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) T to T.")]
        public void ConvertTtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType); 
                    object actualObj = converter.Convert(obj);
                    Asserters.Data.AreEqual(obj, actualObj, string.Format("Conversion failed for {0}.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) Nullable<T> to T.")]
        public void ConvertNullableOfTtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "Nullable<T> to T failed.",
                (type, obj) =>
                {
                    Type nonNullableType = obj.GetType();
                    Assert.IsNull(Nullable.GetUnderlyingType(nonNullableType), "Test error: did not expect nullable object instance.");
                    Assert.AreEqual(nonNullableType, Nullable.GetUnderlyingType(type), "Test error: expected only nullable types.");
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                    object actualValue = converter.Convert(obj);
                    Asserters.Data.AreEqual(obj, actualValue, "Convert failed on Nullable<T> to T.");
                });
        }

        #endregion Convert(object) for value types

        #region Convert(object) using ObjectContent<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) throws converting T to ObjectContent<T>.")]
        public void ConvertThrowsWithTtoObjectContentOfT()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    if (obj != null)
                    {
                        Type convertType = obj.GetType();
                        HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                        string errorMessage = SR.ValueConversionFailed(convertType.FullName, converter.Type.FullName);
                        Asserters.Exception.Throws<InvalidOperationException>(
                            errorMessage,
                            () => converter.Convert(obj));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) ObjectContent<T> to T.")]
        public void ConvertObjectContentOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                    object actualValue = converter.Convert(objectContent);
                    Asserters.Data.AreEqual(obj, actualValue, "Convert failed to return T from ObjectContent<T>.");

                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) ObjectContent<Nullable<T>> to T.")]
        public void ConvertObjectContentOfNullableOfTtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "ObjectContent<Nullable<T>> failed.",
                (type, obj) =>
                {
                    Type nonNullableType = obj.GetType();
                    Assert.IsNull(Nullable.GetUnderlyingType(nonNullableType), "Test error: did not expect nullable object instance.");
                    Assert.AreEqual(nonNullableType, Nullable.GetUnderlyingType(type), "Test error: expected only nullable types.");

                    ObjectContent objectContent =
                        (ObjectContent)Asserters.GenericType.InvokeConstructor(
                            typeof(ObjectContent<>),
                            type,
                            new Type[] { type },
                            new object[] { obj });

                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(nonNullableType);
                    object actualValue = converter.Convert(objectContent);
                    Asserters.Data.AreEqual(obj, actualValue, "Convert failed to return T from ObjectContent<T>.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) ObjectContent<Nullable<T>> to T.")]
        public void ConvertObjectContentOfNullableOfTFromStringtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "ObjectContent<Nullable<T>> failed.",
                (type, obj) =>
                {  
                    if (!ShouldSkip(obj))
                    {
                        HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                        object actualValue = converter.Convert(obj.ToString());
                        Asserters.Data.AreEqual(obj, actualValue, "Convert failed to return T from string.");
                    }
                });
        }

        private bool ShouldSkip(object value)
        {
            Type type = value.GetType();

            // Min/Max floating point values cannot
            return ((type == typeof(Double) && (((Double)value) == Double.MinValue || ((Double)value) == Double.MaxValue)) ||
                    (type == typeof(Single) && (((Single)value) == Single.MinValue || ((Single)value) == Single.MaxValue)) ||
                    type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(DateTimeOffset) )
                        ? true
                        : false;
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) converts null to null for Nullable value types.")]
        public void ConvertObjectContentOfNullableOfTtoTWithNull()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "Converting Nullable<T> with null value failed.",
                (type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                    object actualValue = converter.Convert(null);
                    Asserters.Data.AreEqual(null, actualValue, "Convert failed to return null from ObjectContent<T>.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) converts null to null for NonNullable value types.")]
        public void ConvertObjectContentOftoTWithNull()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsInstance,
                "Converting T with null value failed.",
                (type, obj) =>
                {
                    object expected = Activator.CreateInstance(type);
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                    object actualValue = converter.Convert(null);
                    Asserters.Data.AreEqual(expected, actualValue, "Convert failed to return null from ObjectContent<T>.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) converts null to null for Nullable enum value types.")]
        public void ConvertObjectContentOfEnumNullableOfTtoTWithNull()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableEnumTypes,
                TestDataVariations.AsNullable,
                "Converting Nullable<T> with null value failed.",
                (type, obj) =>
                {
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                    object actualValue = converter.Convert(null);
                    Asserters.Data.AreEqual(null, actualValue, "Convert failed to return null from ObjectContent<T>.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) converts null to null for NonNullable enum value types.")]
        public void ConvertObjectContentOfEnumtoTWithNull()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableEnumTypes,
                TestDataVariations.AsInstance,
                "Converting T with null value failed.",
                (type, obj) =>
                {
                    object expected = Activator.CreateInstance(type);
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(type);
                    object actualValue = converter.Convert(null);
                    Asserters.Data.AreEqual(expected, actualValue, "Convert failed to return null from ObjectContent<T>.");
                });
        }

        #endregion Convert(object) using ObjectContent<T>

        #region Convert(object) using HttpRequestMessage<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) throws converting T to HttpRequestMessage<T>).")]
        public void ConvertThrowsWithTtoHttpRequestMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    if (obj != null)
                    {
                    Type convertType = obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(request.GetType());
                    string errorMessage = SR.ValueConversionFailed(convertType.FullName, converter.Type.FullName);

                        Asserters.Exception.Throws<InvalidOperationException>(
                            errorMessage,
                            () => converter.Convert(obj));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpRequestMessage<T> to T.")]
        public void ConvertHttpRequestMessageOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                    object actualValue = converter.Convert(request);
                    Asserters.Data.AreEqual(obj, actualValue, string.Format("Convert from HttpRequestMessage<T> to T failed for {0}.", convertType));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpRequestMessage<T> to ObjectContent<T>.")]
        public void ConvertHttpRequestMessageOfTtoObjectContentOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    ObjectContent objectContent = (ObjectContent)Asserters.GenericType.InvokeConstructor(typeof(ObjectContent<>), convertType, new Type[] { convertType }, new object[] { obj });
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                    ObjectContent convertedContent = converter.Convert(request) as ObjectContent;
                    Assert.IsNotNull(convertedContent, "Failed to convert to ObjectContent.");
                    Assert.AreEqual(((ObjectContent)request.Content).ReadAsAsync().Result, convertedContent.ReadAsAsync().Result, "Incorrect value.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpRequestMessage<Nullable<T>> to T.")]
        public void ConvertHttpRequestMessageOfNullableOfTtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "HttpRequestMessage<Nullable<T>> failied.",
                (type, obj) =>
                {
                    Type nonNullableType = obj.GetType();
                    Assert.IsNull(Nullable.GetUnderlyingType(nonNullableType), "Test error: did not expect nullable object instance.");
                    Assert.AreEqual(nonNullableType, Nullable.GetUnderlyingType(type), "Test error: expected only nullable types.");

                    HttpRequestMessage request =
                        (HttpRequestMessage)Asserters.GenericType.InvokeConstructor(
                            typeof(HttpRequestMessage<>),
                            type,
                            new Type[] { type },
                            new object[] { obj });

                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(nonNullableType);
                    object actualValue = converter.Convert(request);
                    Asserters.Data.AreEqual(obj, actualValue, "Convert failed to return T from HttpRequestMessage<T>.");
                });
        }

        #endregion Convert(object) using HttpRequestMessage<T>

        #region Convert(object) using HttpResponseMessage<T>

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) throws converting from T to HttpResponseMessage<T>).")]
        public void ConvertThrowsWithTtoHttpResponseMessageOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    if (obj != null)
                    {
                        Type convertType = obj.GetType();
                        HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(response.GetType());
                        string errorMessage = SR.ValueConversionFailed(convertType.FullName, converter.Type.FullName);
                        Asserters.Exception.Throws<InvalidOperationException>(
                            errorMessage,
                            () => converter.Convert(obj));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpResponseMessage<T> to T.")]
        public void ConvertHttpResponseMessageOfTtoT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                    object actualValue = converter.Convert(response);
                    Asserters.Data.AreEqual(obj, actualValue, string.Format("Convert from HttpResponseMessage<T> to T failed for {0}.", convertType));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpResponseMessage<T> to ObjectContent<T>.")]
        public void ConvertHttpResponseMessageOfTtoObjectContentOfT()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.RepresentativeValueAndRefTypeTestDataCollection,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    ObjectContent objectContent = (ObjectContent)Asserters.GenericType.InvokeConstructor(typeof(ObjectContent<>), convertType, new Type[] { convertType }, new object[] { obj });
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                    ObjectContent convertedContent = converter.Convert(response) as ObjectContent;
                    Assert.IsNotNull(convertedContent, "Failed to convert to ObjectContent.");
                    Assert.AreEqual(((ObjectContent)response.Content).ReadAsAsync().Result, convertedContent.ReadAsAsync().Result, "Incorrect value.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) HttpResponseMessage<Nullable<T>> to T.")]
        public void ConvertHttpResponseMessageOfNullableTtoT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AsNullable,
                "HttpResponseMessage<Nullable<T>> failied.",
                (type, obj) =>
                {
                    Type nonNullableType = obj.GetType();
                    Assert.IsNull(Nullable.GetUnderlyingType(nonNullableType), "Test error: did not expect nullable object instance.");
                    Assert.AreEqual(nonNullableType, Nullable.GetUnderlyingType(type), "Test error: expected only nullable types.");

                    HttpResponseMessage request =
                        (HttpResponseMessage)Asserters.GenericType.InvokeConstructor(
                            typeof(HttpResponseMessage<>),
                            type,
                            new Type[] { type },
                            new object[] { obj });

                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(nonNullableType);
                    object actualValue = converter.Convert(request);
                    Asserters.Data.AreEqual(obj, actualValue, "Convert failed to return T from HttpReesponseMessage<T>.");
                });
        }


        #endregion Convert(object) using HttpResponseMessage<T>

        #endregion Convert(object)

        #region Convert(string)

        #region Convert(string) for value types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) to T.")]
        public void ConvertStringToT()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                "Convert(string) failed",
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();

                    if (Asserters.HttpParameter.CanConvertToStringAndBack(obj))
                    {
                        HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(convertType);
                        string objAsString = obj.ToString();
                        object actualObj = converter.Convert(objAsString);
                        Assert.IsNotNull(actualObj, "Convert from string returned null.");
                        Assert.AreEqual(obj.GetType(), actualObj.GetType(), "Convert from string returned wrong type.");
                        string actualObjAsString = actualObj.ToString();
                        Assert.AreEqual(objAsString, actualObjAsString, string.Format("Conversion failed for {0}.", convertType.Name));
                    }
                });
        }

        #endregion Convert(string) for value types

        #region Convert(string) for reference types

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) with string to ObjectContent<T> throws.")]
        public void ConvertStringToObjectContentOfTThrows()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                (objectContent, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(objectContent.GetType());
                    string errorMessage = SR.ValueConversionFailed(typeof(string).FullName, converter.Type.FullName);
                    Asserters.Exception.Throws<InvalidOperationException>(
                        errorMessage,
                        () => converter.Convert("random string"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) to HttpRequestMessage<T> throws.")]
        public void ConvertStringToHttpRequestMessageOfTThrows()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                (request, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(request.GetType());
                    string errorMessage = SR.ValueConversionFailed(typeof(string).FullName, converter.Type.FullName);
                    Asserters.Exception.Throws<InvalidOperationException>(
                        errorMessage,
                        () => converter.Convert("random string"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Convert(object) to HttpResponseMessage<T> throws.")]
        public void ConvertStringToHttpResponseMessageOfTThrows()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                (response, type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpParameterValueConverter converter = HttpParameterValueConverter.GetValueConverter(response.GetType());
                    string errorMessage = SR.ValueConversionFailed(typeof(string).FullName, converter.Type.FullName);
                    Asserters.Exception.Throws<InvalidOperationException>(
                        errorMessage,
                        () => converter.Convert("random string"));
                });
        }

        #endregion Convert(string) for reference types

        #endregion Convert(string)

        #endregion Methods
    }
}
