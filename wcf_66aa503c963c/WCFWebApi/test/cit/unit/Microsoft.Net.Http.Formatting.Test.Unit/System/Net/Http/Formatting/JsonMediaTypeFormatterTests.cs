// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class JsonMediaTypeFormatterTests : UnitTest<JsonMediaTypeFormatter>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonMediaTypeFormatter is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass, typeof(MediaTypeFormatter));
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonMediaTypeFormatter() constructor sets standard Json media types in SupportedMediaTypes.")]
        public void Constructor()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.StandardJsonMediaTypes)
            {
                Assert.IsTrue(formatter.SupportedMediaTypes.Contains(mediaType), string.Format("SupportedMediaTypes should have included {0}.", mediaType.ToString()));
            }
        }

        #endregion Constructors

        #region Properties

        #region DefaultMediaType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("DefaultMediaType property returns application/json.")]
        public void DefaultMediaTypeReturnsApplicationJson()
        {
            MediaTypeHeaderValue mediaType = JsonMediaTypeFormatter.DefaultMediaType;
            Assert.IsNotNull(mediaType, "DefaultMediaType cannot be null.");
            Assert.AreEqual("application/json", mediaType.MediaType);
        }

        #endregion DefaultMediaType

        #region CharacterEncoding

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CharacterEncoding property handles Get/Set correctly.")]
        public void CharacterEncodingGetSet()
        {
            JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            Assert.IsInstanceOfType(jsonFormatter.CharacterEncoding, typeof(UTF8Encoding), "Unexpected default character encoding");
            jsonFormatter.CharacterEncoding = Encoding.Unicode;
            Assert.AreSame(Encoding.Unicode, jsonFormatter.CharacterEncoding, "Unexpected character encoding after set.");
            jsonFormatter.CharacterEncoding = Encoding.UTF8;
            Assert.AreSame(Encoding.UTF8, jsonFormatter.CharacterEncoding, "Unexpected character encoding after set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CharacterEncoding property throws on invalid arguments")]
        public void CharacterEncodingSetThrows()
        {
            JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("value", () => { jsonFormatter.CharacterEncoding = null; });
            Asserters.Exception.ThrowsArgument("value", () => { jsonFormatter.CharacterEncoding = Encoding.UTF32; });
        }

        #endregion CharacterEncoding

        #region IsModified

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects clearing default mappings.")]
        public void IsModifiedDetectsClearingMappings()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
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
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.AddRequestHeaderMapping("name", "value", StringComparison.OrdinalIgnoreCase, false, "application/octet-stream");
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after addition.");
        }

        #endregion IsModified

        #endregion Properties

        #region Methods

        #region CanReadType()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadType() returns the expected results for all known value and reference types.")]
        public void CanReadTypeReturnsExpectedValues()
        {
            TestJsonMediaTypeFormatter formatter = new TestJsonMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool isSerializable = this.IsTypeSerializableWithJsonSerializer(type, obj);
                    bool canSupport = formatter.CanReadTypeProxy(type);

                    // If we don't agree, we assert only if the DCJ serializer says it cannot support something we think it should
                    if (isSerializable != canSupport && isSerializable)
                    {
                        Assert.Fail(string.Format("CanReadType returned wrong value for '{0}'.", type));
                    }

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    canSupport = formatter.CanReadTypeProxy(type);
                    if (isSerializable != canSupport && isSerializable)
                    {
                        Assert.Fail(string.Format("2nd CanReadType returned wrong value for '{0}'.", type));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() returns false on JsonValue.")]
        public void CanReadTypeReturnsFalseOnJsonValue()
        {
            TestJsonMediaTypeFormatter formatter = new TestJsonMediaTypeFormatter();
            foreach (Type type in JsonValueMediaTypeFormatterTests.JsonValueTypes)
            {
                Assert.IsFalse(formatter.CanReadTypeProxy(type), "formatter should have returned false.");
            }
        }

        #endregion CanReadType

        #region CanWriteType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() returns false on JsonValue.")]
        public void CanWriteTypeReturnsFalseOnJsonValue()
        {
            TestJsonMediaTypeFormatter formatter = new TestJsonMediaTypeFormatter();
            foreach (Type type in JsonValueMediaTypeFormatterTests.JsonValueTypes)
            {
                Assert.IsFalse(formatter.CanWriteTypeProxy(type), "formatter should have returned false.");
            }
        }

        #endregion

        #region SetSerializer()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, DataContractJsonSerializer) throws with null type.")]
        public void SetSerializerThrowsWithNullType()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.SetSerializer(null, serializer); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, DataContractJsonSerializer) throws with null serializer.")]
        public void SetSerializerThrowsWithNullSerializer()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer(typeof(string), null); });
        }

        #endregion SetSerializer()

        #region SetSerializer<T>()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer<T>(DataContractJsonSerializer) throws with null serializer.")]
        public void SetSerializer1ThrowsWithNullSerializer()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer<string>(null); });
        }

        #endregion SetSerializer<T>()

        #region RemoveSerializer()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RemoveSerializer() throws with null type.")]
        public void RemoveSerializerThrowsWithNullType()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.RemoveSerializer(null); });
        }

        #endregion RemoveSerializer()

        #region ReadFromStream()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStream() returns all value and reference types serialized via WriteToStream.")]
        public void ReadFromStreamRoundTripsWriteToStream()
        {
            TestJsonMediaTypeFormatter formatter = new TestJsonMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            Asserters.Data.Execute(
                TestData.ValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool canSerialize = this.IsTypeSerializableWithJsonSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);
                    if (canSerialize)
                    {
                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => formatter.WriteToStream(type, obj, stream, contentHeaders, /*transportContext*/ null),
                            (stream) => readObj = formatter.ReadFromStream(type, stream, contentHeaders));
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object.");
                    }
                });
        }

        #endregion ReadFromStream()

        #region ReadFromStreamAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStreamAsync() returns all value and reference types serialized via WriteToStreamAsync.")]
        public void ReadFromStreamAsyncRoundTripsWriteToStreamAsync()
        {
            TestJsonMediaTypeFormatter formatter = new TestJsonMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool canSerialize = this.IsTypeSerializableWithJsonSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);
                    if (canSerialize)
                    {
                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => Asserters.Task.Succeeds(formatter.WriteToStreamAsync(type, obj, stream, contentHeaders, /*transportContext*/ null)),
                            (stream) => readObj = Asserters.Task.SucceedsWithResult(formatter.ReadFromStreamAsync(type, stream, contentHeaders)));
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object.");
                    }
                });
        }

        #endregion ReadFromStreamAsync()

        #endregion Methods

        #region Test types

        public class TestJsonMediaTypeFormatter : JsonMediaTypeFormatter
        {
            public bool CanReadTypeProxy(Type type)
            {
                return this.CanReadType(type);
            }

            public bool CanWriteTypeProxy(Type type)
            {
                return this.CanWriteType(type);
            }
        }

        #endregion Test types

        #region Test helpers

        private bool IsTypeSerializableWithJsonSerializer(Type type, object obj)
        {
            try
            {
                new DataContractJsonSerializer(type);
                if (obj != null && obj.GetType() != type)
                {
                    new DataContractJsonSerializer(obj.GetType());
                }
            }
            catch
            {
                return false;
            }

            return !Asserters.Http.IsKnownUnserializable(type, obj, (t) => typeof(INotJsonSerializable).IsAssignableFrom(t));
        }

        #endregion Test helpers

    }
}
