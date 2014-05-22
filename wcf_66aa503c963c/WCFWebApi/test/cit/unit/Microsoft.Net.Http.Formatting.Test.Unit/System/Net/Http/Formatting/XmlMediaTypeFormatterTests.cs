// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.Types;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete, PublicMemberMinimumCoverage = 100)]
    public class XmlMediaTypeFormatterTests : UnitTest<XmlMediaTypeFormatter>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("XmlMediaTypeFormatter is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("XmlMediaTypeFormatter() constructor sets standard Xml media types in SupportedMediaTypes.")]
        public void Constructor()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.StandardXmlMediaTypes)
            {
                Assert.IsTrue(formatter.SupportedMediaTypes.Contains(mediaType), string.Format("SupportedMediaTypes should have included {0}.", mediaType.ToString()));
            }
        }

        #endregion Constructors

        #region Properties

        #region DefaultMediaType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("DefaultMediaType property returns application/xml.")]
        public void DefaultMediaTypeReturnsApplicationXml()
        {
            MediaTypeHeaderValue mediaType = XmlMediaTypeFormatter.DefaultMediaType;
            Assert.IsNotNull(mediaType, "DefaultMediaType cannot be null.");
            Assert.AreEqual("application/xml", mediaType.MediaType);
        }

        #endregion DefaultMediaType

        #region CharacterEncoding

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CharacterEncoding property handles Get/Set correctly.")]
        public void CharacterEncodingGetSet()
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter();
            Assert.IsInstanceOfType(xmlFormatter.CharacterEncoding, typeof(UTF8Encoding), "Unexpected default character encoding");
            xmlFormatter.CharacterEncoding = Encoding.Unicode;
            Assert.AreSame(Encoding.Unicode, xmlFormatter.CharacterEncoding, "Unexpected character encoding after set.");
            xmlFormatter.CharacterEncoding = Encoding.UTF8;
            Assert.AreSame(Encoding.UTF8, xmlFormatter.CharacterEncoding, "Unexpected character encoding after set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CharacterEncoding property throws on invalid arguments")]
        public void CharacterEncodingSetThrows()
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("value", () => { xmlFormatter.CharacterEncoding = null; });
            Asserters.Exception.ThrowsArgument("value", () => { xmlFormatter.CharacterEncoding = Encoding.UTF32; });
        }

        #endregion CharacterEncoding

        #region IsModified

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects clearing default mappings.")]
        public void IsModifiedDetectsClearingMappings()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.MediaTypeMappings.Clear();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after clearing.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects adding mapping.")]
        public void IsModifiedDetectsAddingMapping()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.AddRequestHeaderMapping("name", "value", StringComparison.OrdinalIgnoreCase, false, "application/octet-stream");
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after addition.");
        }

        #endregion IsModified

        #region UseDataContractSerializer

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UseDataContractSerializer property should be false by default.")]
        public void UseDataContractSerializer_Default()
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter();
            Assert.IsFalse(xmlFormatter.UseDataContractSerializer, "The UseDataContractSerializer property should be false by default.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UseDataContractSerializer property works when set to true.")]
        public void UseDataContractSerializer_True()
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter { UseDataContractSerializer = true };
            MemoryStream memoryStream = new MemoryStream();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;
            xmlFormatter.WriteToStream(typeof(SampleType), new SampleType(), memoryStream, contentHeaders, null);
            memoryStream.Position = 0;
            string serializedString = new StreamReader(memoryStream).ReadToEnd();
            Assert.IsTrue(serializedString.Contains("DataContractSampleType"),
                "SampleType should be serialized with data contract name DataContractSampleType because UseDataContractSerializer is set to true.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UseDataContractSerializer property works when set to false.")]
        public void UseDataContractSerializer_False()
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter { UseDataContractSerializer = false };
            MemoryStream memoryStream = new MemoryStream();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;
            xmlFormatter.WriteToStream(typeof(SampleType), new SampleType(), memoryStream, contentHeaders, null);
            memoryStream.Position = 0;
            string serializedString = new StreamReader(memoryStream).ReadToEnd();
            Assert.IsFalse(serializedString.Contains("DataContractSampleType"),
                "SampleType should not be serialized with data contract name DataContractSampleType because UseDataContractSerializer is set to false.");
        }

        #endregion

        #endregion Properties

        #region Methods

        #region CanReadType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() returns the same result as the XmlSerializer constructor.")]
        public void CanReadTypeReturnsSameResultAsXmlSerializerConstructor()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool isSerializable = this.IsSerializableWithXmlSerializer(type, obj);
                    bool canSupport = formatter.CanReadTypeCaller(type);
                    if (isSerializable != canSupport)
                    {
                        Assert.AreEqual(isSerializable, canSupport, string.Format("CanReadType returned wrong value for '{0}'.", type));
                    }

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    canSupport = formatter.CanReadTypeCaller(type);
                    if (isSerializable != canSupport)
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
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            foreach (Type type in JsonValueMediaTypeFormatterTests.JsonValueTypes)
            {
                Assert.IsFalse(formatter.CanReadTypeCaller(type), "formatter should have returned false.");
            }
        }

        #endregion CanReadType

        #region CanWriteType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() returns false on JsonValue.")]
        public void CanWriteTypeReturnsFalseOnJsonValue()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            foreach (Type type in JsonValueMediaTypeFormatterTests.JsonValueTypes)
            {
                Assert.IsFalse(formatter.CanWriteTypeCaller(type), "formatter should have returned false.");
            }
        }

        #endregion

        #region SetSerializer(Type, XmlSerializer)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, XmlSerializer) throws with null type.")]
        public void SetSerializerThrowsWithNullType()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(string));
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.SetSerializer(null, xmlSerializer); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, XmlSerializer) throws with null serializer.")]
        public void SetSerializerThrowsWithNullSerializer()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer(typeof(string), (XmlSerializer)null); });
        }

        #endregion SetSerializer(Type, XmlSerializer)

        #region SetSerializer<T>(XmlSerializer)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer<T>(XmlSerializer) throws with null serializer.")]
        public void SetSerializer1ThrowsWithNullSerializer()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer<string>((XmlSerializer)null); });
        }

        #endregion SetSerializer<T>(XmlSerializer)

        #region SetSerializer(Type, XmlObjectSerializer)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, XmlObjectSerializer) throws with null type.")]
        public void SetSerializer2ThrowsWithNullType()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            XmlObjectSerializer xmlObjectSerializer = new DataContractSerializer(typeof(string));
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.SetSerializer(null, xmlObjectSerializer); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer(Type, XmlObjectSerializer) throws with null serializer.")]
        public void SetSerializer2ThrowsWithNullSerializer()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer(typeof(string), (XmlObjectSerializer)null); });
        }

        #endregion SetSerializer(Type, XmlObjectSerializer)

        #region SetSerializer<T>(XmlObjectSerializer)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SetSerializer<T>(XmlObjectSerializer) throws with null serializer.")]
        public void SetSerializer3ThrowsWithNullSerializer()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("serializer", () => { formatter.SetSerializer<string>((XmlSerializer)null); });
        }

        #endregion SetSerializer<T>(XmlObjectSerializer)

        #region RemoveSerializer()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RemoveSerializer throws with null type.")]
        public void RemoveSerializerThrowsWithNullType()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.RemoveSerializer(null); });
        }

        #endregion RemoveSerializer()

        #region ReadFromStream() using XmlSerializer

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("ReadFromStream() returns all value and reference types serialized via WriteToStream using XmlSerializer.")]
        public void ReadFromStreamRoundTripsWriteToStreamUsingXmlSerializer()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            // Excludes ReferenceDataContractType tests because XmlSerializer cannot handle circular references
            // TODO: Excludes ISerializableType because of bug CSDMain 239642 in XmlSerializer
            Asserters.Data.Execute(
                TestData.ValueAndRefTypeTestDataCollection.Where((td) => !(typeof(RefTypeTestData<ReferenceDataContractType>).IsAssignableFrom(td.GetType())) && !(typeof(RefTypeTestData<ISerializableType>).IsAssignableFrom(td.GetType()))),
                (type, obj) =>
                {
                    bool canSerialize = this.IsSerializableWithXmlSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);

                    if (canSerialize)
                    {
                        formatter.SetSerializer(type, new XmlSerializer(type));

                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => formatter.WriteToStream(type, obj, stream, contentHeaders, /*transportContext*/ null),
                            (stream) => readObj = formatter.ReadFromStream(type, stream, contentHeaders));
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object");
                    }
                });
        }

        #endregion ReadFromStream() using XmlSerializer

        #region ReadFromStream() using DataContractSerializer

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStream() returns all value and reference types serialized via WriteToStream using DataContractSerializer.")]
        public void ReadFromStreamRoundTripsWriteToStreamUsingDataContractSerializer()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            Asserters.Data.Execute(
                TestData.ValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool canSerialize = this.IsSerializableWithDataContractSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);
                    if (canSerialize)
                    {
                        formatter.SetSerializer(type, new DataContractSerializer(type));

                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => formatter.WriteToStream(type, obj, stream, contentHeaders, /*transportContext*/ null),
                            (stream) => readObj = formatter.ReadFromStream(type, stream, contentHeaders));
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object");
                    }
                });
        }

        #endregion ReadFromStream() using DataContractSerializer

        #region ReadFromStreamAsync() using XmlSerializer

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("ReadFromStreamAsync() returns all value and reference types serialized via WriteToStreamAsync using XmlSerializer.")]
        public void ReadFromStreamAsyncRoundTripsWriteToStreamAsyncUsingXmlSerializer()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool canSerialize = this.IsSerializableWithXmlSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);
                    if (canSerialize)
                    {
                        formatter.SetSerializer(type, new XmlSerializer(type));

                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => Asserters.Task.Succeeds(formatter.WriteToStreamAsync(type, obj, stream, contentHeaders, /*transportContext*/ null)),
                            (stream) => readObj = Asserters.Task.SucceedsWithResult(formatter.ReadFromStreamAsync(type, stream, contentHeaders))
                            );
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object");
                    }
                });
        }

        #endregion ReadFromStreamAsync() using XmlSerializer

        #region ReadFromStreamAsync() using DataContractSerializer

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStream() returns all value and reference types serialized via WriteToStream using DataContractSerializer.")]
        public void ReadFromStreamAsyncRoundTripsWriteToStreamUsingDataContractSerializer()
        {
            TestXmlMediaTypeFormatter formatter = new TestXmlMediaTypeFormatter();
            HttpContentHeaders contentHeaders = new StringContent(string.Empty).Headers;

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    bool canSerialize = this.IsSerializableWithDataContractSerializer(type, obj) && Asserters.Http.CanRoundTrip(type);
                    if (canSerialize)
                    {
                        formatter.SetSerializer(type, new DataContractSerializer(type));

                        object readObj = null;
                        Asserters.Stream.WriteAndRead(
                            (stream) => Asserters.Task.Succeeds(formatter.WriteToStreamAsync(type, obj, stream, contentHeaders, /*transportContext*/ null)),
                            (stream) => readObj = Asserters.Task.SucceedsWithResult(formatter.ReadFromStreamAsync(type, stream, contentHeaders))
                            );
                        Asserters.Data.AreEqual(obj, readObj, "Failed to round trip object.");
                    }
                });
        }

        #endregion ReadFromStreamAsync() using DataContractSerializer

        #endregion Methods

        #region Test types

        public class TestXmlMediaTypeFormatter : XmlMediaTypeFormatter
        {
            public bool CanReadTypeCaller(Type type)
            {
                return this.CanReadType(type);
            }

            public bool CanWriteTypeCaller(Type type)
            {
                return this.CanWriteType(type);
            }
        }

        [DataContract(Name = "DataContractSampleType")]
        public class SampleType
        {
            [DataMember]
            public int Number { get; set; }
        }

        #endregion Test types

        #region Test helpers

        private bool IsSerializableWithXmlSerializer(Type type, object obj)
        {
            if (Asserters.Http.IsKnownUnserializable(type, obj))
            {
                return false;
            }

            try
            {
                new XmlSerializer(type);
                if (obj != null && obj.GetType() != type)
                {
                    new XmlSerializer(obj.GetType());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool IsSerializableWithDataContractSerializer(Type type, object obj)
        {
            if (Asserters.Http.IsKnownUnserializable(type, obj))
            {
                return false;
            }

            try
            {
                new DataContractSerializer(type);
                if (obj != null && obj.GetType() != type)
                {
                    new DataContractSerializer(obj.GetType());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion Test helpers
    }
}
