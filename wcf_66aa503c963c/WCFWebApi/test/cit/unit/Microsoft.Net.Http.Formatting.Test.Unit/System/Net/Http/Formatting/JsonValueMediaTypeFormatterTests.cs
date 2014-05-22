// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class JsonValueMediaTypeFormatterTests : UnitTest<JsonValueMediaTypeFormatter>
    {
        public static readonly List<Type> JsonValueTypes = new List<Type>
        {
            typeof(JsonValue),
            typeof(JsonPrimitive),
            typeof(JsonArray),
            typeof(JsonObject)
        };

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonValueMediaTypeFormatter is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonValueMediaTypeFormatter() constructor sets standard JSON media types in SupportedMediaTypes.")]
        public void Constructor()
        {
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.StandardJsonMediaTypes)
            {
                Assert.IsTrue(formatter.SupportedMediaTypes.Contains(mediaType), string.Format("SupportedMediaTypes should have included {0}.", mediaType.ToString()));
            }
        }

        #endregion Constructors

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("DefaultMediaType property returns application/json.")]
        public void DefaultMediaTypeReturnsApplicationJson()
        {
            MediaTypeHeaderValue mediaType = JsonValueMediaTypeFormatter.DefaultMediaType;
            Assert.IsNotNull(mediaType, "DefaultMediaType cannot be null.");
            Assert.AreEqual("application/json", mediaType.MediaType);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonWriteOptions throws on invalid enum value.")]
        public void JsonWriteOptionsReturnsCorrectValue()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Assert.AreEqual(JsonSaveOptions.None, formatter.JsonWriteOptions, "Unexpected initial value.");
            formatter.JsonWriteOptions = JsonSaveOptions.EnableIndent;
            Assert.AreEqual(JsonSaveOptions.EnableIndent, formatter.JsonWriteOptions, "Unexpected value.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonWriteOptions throws on invalid enum value.")]
        public void JsonWriteOptionsThrowsOnInvalidEnum()
        {
            int invalidValue = 999;
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Exception.ThrowsInvalidEnumArgument("value", invalidValue, typeof(JsonSaveOptions),
                () =>
                {
                    formatter.JsonWriteOptions = (JsonSaveOptions)invalidValue;
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects changes in json write options.")]
        public void IsModifiedDetectsOptionChanges()
        {
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            JsonSaveOptions options = formatter.JsonWriteOptions;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");
            
            formatter.JsonWriteOptions = JsonSaveOptions.None;
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after write.");

            TestJsonValueMediaTypeFormatter subClass = new TestJsonValueMediaTypeFormatter();
            Assert.IsTrue(subClass.IsModified, "formatter should have been modified in subclass.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects clearing default mappings.")]
        public void IsModifiedDetectsClearingMappings()
        {
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.MediaTypeMappings.Clear();
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after clearing.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects adding mapping.")]
        public void IsModifiedDetectsAddingMapping()
        {
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.AddRequestHeaderMapping("name", "value", StringComparison.OrdinalIgnoreCase, false, "application/octet-stream");
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after addition.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() throws on null.")]
        public void CanReadTypeThrowsOnNull()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.CanReadType(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() returns only true for JsonValue and derived types (JP, JO, JA).")]
        public void CanReadTypeReturnsOnlyTrueForJsonValue()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Assert.IsFalse(formatter.CanReadType(type), "formatter should have returned false.");

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    Assert.IsFalse(formatter.CanReadType(type), "formatter should have returned false on 2nd try as well.");
                });

            foreach (Type type in JsonValueTypes)
            {
                Assert.IsTrue(formatter.CanReadType(type), "formatter should have returned true.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() throws on null.")]
        public void CanWriteTypeThrowsOnNull()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.CanWriteType(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() returns only true for JsonValue.")]
        public void CanWriteTypeReturnsOnlyTrueForJsonValue()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Assert.IsFalse(formatter.CanWriteType(type), "formatter should have returned false.");

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    Assert.IsFalse(formatter.CanWriteType(type), "formatter should have returned false on 2nd try as well.");
                });

            foreach (Type type in JsonValueTypes)
            {
                Assert.IsTrue(formatter.CanWriteType(type), "formatter should have returned true.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("ReadFromStream() throws on null.")]
        public void ReadFromStreamThrowsOnNull()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.OnReadFromStream(null, Stream.Null, null); });
            Asserters.Exception.ThrowsArgumentNull("stream", () => { formatter.OnReadFromStream(typeof(object), null, null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("ReadFromStream() roundtrips JsonValue.")]
        public void ReadFromStreamRoundTripsJsonValue()
        {
            string beforeMessage = "Hello World";
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            JsonValue before = beforeMessage;
            MemoryStream memStream = new MemoryStream();
            before.Save(memStream, JsonSaveOptions.None);
            memStream.Position = 0;

            JsonValue after = formatter.OnReadFromStream(typeof(JsonValue), memStream, null) as JsonValue;
            Assert.IsNotNull(after, "Expected JsonValue object");
            string afterMessage = after.ReadAs<string>();

            Assert.AreEqual(beforeMessage, afterMessage, "Round-tripping did not work.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("WriteToStream() throws on null.")]
        public void WriteToStreamThrowsOnNull()
        {
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.OnWriteToStream(null, new object(), Stream.Null, null, null); });
            Asserters.Exception.ThrowsArgumentNull("stream", () => { formatter.OnWriteToStream(typeof(object), new object(), null, null, null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("WriteToStream() roundtrips JsonValue.")]
        public void WriteToStreamRoundTripsJsonValue()
        {
            string beforeMessage = "Hello World";
            TestJsonValueMediaTypeFormatter formatter = new TestJsonValueMediaTypeFormatter();
            JsonValue before = new JsonPrimitive(beforeMessage);
            MemoryStream memStream = new MemoryStream();

            formatter.OnWriteToStream(typeof(JsonValue), before, memStream, null, null);
            memStream.Position = 0;
            JsonValue after = JsonValue.Load(memStream);
            string afterMessage = after.ReadAs<string>();

            Assert.AreEqual(beforeMessage, afterMessage, "Round-tripping did not work.");
        }

        #endregion Members

        #region Test types

        public class TestJsonValueMediaTypeFormatter : JsonValueMediaTypeFormatter
        {
            public new bool CanReadType(Type type)
            {
                return base.CanReadType(type);
            }

            public new bool CanWriteType(Type type)
            {
                return base.CanWriteType(type);
            }

            public new object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
            {
                return base.OnReadFromStream(type, stream, contentHeaders);
            }

            public new void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                base.OnWriteToStream(type, value, stream, contentHeaders, context);
            }
        }

        #endregion Test types
    }
}
