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
    using System.Net.Http.Test;
    using System.Threading.Tasks;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class MediaTypeFormatterTests : UnitTest<MediaTypeFormatter>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatter is public, abstract, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatter() constructor (via derived class) sets SupportedMediaTypes and MediaTypeMappings.")]
        public void Constructor()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;
            Assert.IsNotNull(supportedMediaTypes, "SupportedMediaTypes was not initialized.");
            Assert.AreEqual(0, supportedMediaTypes.Count, "SupportedMediaTypes should be empty by default.");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsNotNull(mappings, "MediaTypeMappings was not initialized.");
            Assert.AreEqual(0, mappings.Count, "MediaTypeMappings should be empty by default.");
        }

        #endregion Constructors

        #region Properties

        #region SupportedMediaTypes

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SupportedMediaTypes is a mutable collection.")]
        public void SupportedMediaTypesIsMutable()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;
            MediaTypeHeaderValue[] mediaTypes = DataSets.Http.LegalMediaTypeHeaderValues.ToArray();
            foreach (MediaTypeHeaderValue mediaType in mediaTypes)
            {
                supportedMediaTypes.Add(mediaType);
            }

            CollectionAssert.AreEqual(mediaTypes, formatter.SupportedMediaTypes, "SupportedMediaTypes does not contain expected set of media types.");
        }

        #region SupportedMediaTypes.Add()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SupportedMediaTypes Add throws with a null media type.")]
        public void SupportedMediaTypesAddThrowsWithNullMediaType()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;

            Asserters.Exception.ThrowsArgumentNull(
                "item",
                () => supportedMediaTypes.Add(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SupportedMediaTypes Add throws with a media range.")]
        public void SupportedMediaTypesAddThrowsWithMediaRange()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;
            MediaTypeHeaderValue[] mediaRanges = DataSets.Http.LegalMediaRangeValues.ToArray();
            foreach (MediaTypeHeaderValue mediaType in mediaRanges)
            {
                Asserters.Exception.ThrowsArgument(
                    "item",
                    SR.CannotUseMediaRangeForSupportedMediaType(typeof(MediaTypeHeaderValue).Name, mediaType.MediaType),
                    () => supportedMediaTypes.Add(mediaType));
            }
        }

        #endregion SupportedMediaTypes.Add()

        #region SupportedMediaTypes.Insert()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SupportedMediaTypes Insert throws with a null media type.")]
        public void SupportedMediaTypesInsertThrowsWithNullMediaType()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;

            Asserters.Exception.ThrowsArgumentNull(
                "item",
                () => supportedMediaTypes.Insert(0, null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SupportedMediaTypes Insert throws with a media range.")]
        public void SupportedMediaTypesInsertThrowsWithMediaRange()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeHeaderValue> supportedMediaTypes = formatter.SupportedMediaTypes;
            MediaTypeHeaderValue[] mediaRanges = DataSets.Http.LegalMediaRangeValues.ToArray();
            foreach (MediaTypeHeaderValue mediaType in mediaRanges)
            {
                Asserters.Exception.ThrowsArgument(
                    "item",
                    SR.CannotUseMediaRangeForSupportedMediaType(typeof(MediaTypeHeaderValue).Name, mediaType.MediaType),
                    () => supportedMediaTypes.Insert(0, mediaType));
            }
        }

        #endregion SupportedMediaTypes.Add()

        #endregion SupportedMediaTypes

        #region MediaTypeMappings

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeMappings is a mutable collection.")]
        public void MediaTypeMappingsIsMutable()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            MediaTypeMapping[] standardMappings = DataSets.Http.StandardMediaTypeMappings.ToArray();
            foreach (MediaTypeMapping mapping in standardMappings)
            {
                mappings.Add(mapping);
            }

            CollectionAssert.AreEqual(standardMappings, formatter.MediaTypeMappings, "MediaTypeMappings does not contain expected set of MediaTypeMapping elements.");
        }

        #endregion MediaTypeMappings

        #endregion Properties

        #region Methods

        #region TryMatchSupportedMediaType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchSupportedMediaType(MediaTypeHeaderValue, out MediaTypeMatch) returns media type and quality.")]
        public void TryMatchSupportedMediaTypeWithQuality()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaTypeWithQuality in DataSets.Http.StandardMediaTypesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
                MediaTypeHeaderValue mediaTypeWithoutQuality = new MediaTypeHeaderValue(mediaTypeWithQuality.MediaType);
                formatter.SupportedMediaTypes.Add(mediaTypeWithoutQuality);
                MediaTypeMatch match;
                bool result = formatter.TryMatchSupportedMediaType(mediaTypeWithQuality, out match);
                Assert.IsTrue(result, string.Format("TryMatchSupportedMediaType should have succeeded for '{0}'.", mediaTypeWithQuality));
                Assert.IsNotNull(match, string.Format("TryMatchSupportedMediaType returned null for '{0}'.", mediaTypeWithQuality));
                double quality = mediaTypeWithQuality.Quality.Value;
                Assert.AreEqual(quality, match.Quality, string.Format("TryMatchSupportedMediaType returned the wrong quality for '{0}'.", mediaTypeWithQuality));
                Assert.IsNotNull(match.MediaType, string.Format("TryMatchSupportedMediaType returned null media type for '{0}'.", mediaTypeWithQuality));
                Assert.AreEqual(mediaTypeWithoutQuality.MediaType, match.MediaType.MediaType, string.Format("TryMatchSupportedMediaType returned wrong media type for '{0}'.", mediaTypeWithQuality));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchSupportedMediaType(MediaTypeHeaderValue, out MediaTypeMatch) returns cloned media type, not original.")]
        public void TryMatchSupportedMediaTypeReturnsClone()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaTypeWithQuality in DataSets.Http.StandardMediaTypesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
                MediaTypeHeaderValue mediaTypeWithoutQuality = new MediaTypeHeaderValue(mediaTypeWithQuality.MediaType);
                formatter.SupportedMediaTypes.Add(mediaTypeWithoutQuality);
                MediaTypeMatch match;
                bool result = formatter.TryMatchSupportedMediaType(mediaTypeWithQuality, out match);
                Assert.IsNotNull(match, string.Format("TryMatchSupportedMediaType returned null for '{0}'.", mediaTypeWithQuality));
                Assert.IsNotNull(match.MediaType, "MediaType was null.");
                Assert.IsFalse(object.ReferenceEquals(mediaTypeWithoutQuality, match.MediaType), "Original object was leaked.");
            }
        }

        #endregion TryMatchSupportedMediaType

        #region TryMatchMediaTypeMapping(HttpResponseMessage, out MediaTypeMatch)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaTypeMapping(HttpResponseMessage, out MediaTypeMatch) returns media type and quality from media range with quality.")]
        public void TryMatchMediaTypeMappingWithQuality()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaRangeWithQuality in DataSets.Http.MediaRangeValuesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
                MediaTypeHeaderValue mediaRangeWithoutQuality = new MediaTypeHeaderValue(mediaRangeWithQuality.MediaType);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/xml");
                MediaRangeMapping mapping = new MediaRangeMapping(mediaRangeWithoutQuality, mediaType);
                formatter.MediaTypeMappings.Add(mapping);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Accept.Add(mediaRangeWithQuality);
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                MediaTypeMatch match;
                bool result = formatter.TryMatchMediaTypeMapping(response, out match);
                Assert.IsTrue(result, string.Format("TryMatchMediaTypeMapping should have succeeded for '{0}'.", mediaRangeWithQuality));
                Assert.IsNotNull(match, string.Format("TryMatchMediaTypeMapping returned null for '{0}'.", mediaRangeWithQuality));
                double quality = mediaRangeWithQuality.Quality.Value;
                Assert.AreEqual(quality, match.Quality, string.Format("TryMatchMediaTypeMapping returned the wrong quality for '{0}'.", mediaRangeWithQuality));
                Assert.IsNotNull(match.MediaType, string.Format("TryMatchMediaTypeMapping returned null media type for '{0}'.", mediaRangeWithQuality));
                Assert.AreEqual(mediaType.MediaType, match.MediaType.MediaType, string.Format("TryMatchMediaTypeMapping returned wrong media type for '{0}'.", mediaRangeWithQuality));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaTypeMapping(HttpResponseMessage, out MediaTypeMatch) returns a clone of the original media type.")]
        public void TryMatchMediaTypeMappingClonesMediaType()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaRangeWithQuality in DataSets.Http.MediaRangeValuesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
                MediaTypeHeaderValue mediaRangeWithoutQuality = new MediaTypeHeaderValue(mediaRangeWithQuality.MediaType);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/xml");
                MediaRangeMapping mapping = new MediaRangeMapping(mediaRangeWithoutQuality, mediaType);
                formatter.MediaTypeMappings.Add(mapping);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Accept.Add(mediaRangeWithQuality);
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                MediaTypeMatch match;
                formatter.TryMatchMediaTypeMapping(response, out match);
                Assert.IsNotNull(match, string.Format("TryMatchMediaTypeMapping returned null for '{0}'.", mediaRangeWithQuality));
                Assert.IsNotNull(match.MediaType, string.Format("TryMatchMediaTypeMapping returned null media type for '{0}'.", mediaRangeWithQuality));
                Assert.IsFalse(object.ReferenceEquals(mediaType, match.MediaType), string.Format("TryMatchMediaTypeMapping should hot return the identical instance media type for '{0}'.", mediaRangeWithQuality));
            }
        }

        #endregion TryMatchMediaTypeMapping(HttpResponseMessage, out MediaTypeMatch)

        #region SelectResponseMediaType(Type, HttpResponseMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) matches based only on type.")]
        public void SelectResponseMediaTypeMatchesType()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
            ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(typeof(string), response);

            Assert.IsNotNull(match, "SelectResponseMediaType returned null");
            Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnCanWriteType, match.ResponseFormatterSelectionResult, "SelectResponseMediaType returned the wrong enum");
            Assert.IsNull(match.MediaTypeMatch.MediaType, "SelectResponseMediaType returned non null media type.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) matches media type from request content type.")]
        public void SelectResponseMediaTypeMatchesRequestContentType()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
                formatter.SupportedMediaTypes.Add(mediaType);
                HttpRequestMessage request = new HttpRequestMessage() { Content = new StringContent("fred") };
                request.Content.Headers.ContentType = mediaType;
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(typeof(string), response);

                Assert.IsNotNull(match, string.Format("SelectResponseMediaType returned null for '{0}'.", mediaType));
                Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnRequestContentType, match.ResponseFormatterSelectionResult, string.Format("SelectResponseMediaType returned the wrong enum for '{0}'.", mediaType));
                Assert.IsNotNull(match.MediaTypeMatch.MediaType, string.Format("SelectResponseMediaType returned null media type for '{0}'.", mediaType));
                Assert.AreEqual(mediaType.MediaType, match.MediaTypeMatch.MediaType.MediaType, string.Format("SelectResponseMediaType returned wrong media type for '{0}'.", mediaType));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) matches media type from response content type.")]
        public void SelectResponseMediaTypeMatchesResponseContentType()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
                formatter.SupportedMediaTypes.Add(mediaType);
                HttpRequestMessage request = new HttpRequestMessage();
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request, Content = new StringContent("fred") };
                response.Content.Headers.ContentType = mediaType;
                ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(typeof(string), response);

                Assert.IsNotNull(match, string.Format("SelectResponseMediaType returned null for '{0}'.", mediaType));
                Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnResponseContentType, match.ResponseFormatterSelectionResult, string.Format("SelectResponseMediaType returned the wrong enum for '{0}'.", mediaType));
                Assert.IsNotNull(match.MediaTypeMatch.MediaType, string.Format("SelectResponseMediaType returned null media type for '{0}'.", mediaType));
                Assert.AreEqual(mediaType.MediaType, match.MediaTypeMatch.MediaType.MediaType, string.Format("SelectResponseMediaType returned wrong media type for '{0}'.", mediaType));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) matches supported media type from accept headers.")]
        public void SelectResponseMediaTypeMatchesAcceptHeaderToSupportedMediaTypes()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaTypeWithQuality in DataSets.Http.StandardMediaTypesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
                MediaTypeHeaderValue mediaTypeWithoutQuality = new MediaTypeHeaderValue(mediaTypeWithQuality.MediaType);
                formatter.SupportedMediaTypes.Add(mediaTypeWithoutQuality);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Accept.Add(mediaTypeWithQuality);
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(typeof(string), response);

                Assert.IsNotNull(match, string.Format("SelectResponseMediaType returned null for '{0}'.", mediaTypeWithQuality));
                Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnRequestAcceptHeader, match.ResponseFormatterSelectionResult, string.Format("SelectResponseMediaType returned the wrong enum for '{0}'.", mediaTypeWithQuality));
                double quality = mediaTypeWithQuality.Quality.Value;
                Assert.AreEqual(quality, match.MediaTypeMatch.Quality, string.Format("SelectResponseMediaType returned the wrong quality for '{0}'.", mediaTypeWithQuality));
                Assert.IsNotNull(match.MediaTypeMatch.MediaType, string.Format("SelectResponseMediaType returned null media type for '{0}'.", mediaTypeWithQuality));
                Assert.AreEqual(mediaTypeWithoutQuality.MediaType, match.MediaTypeMatch.MediaType.MediaType, string.Format("SelectResponseMediaType returned wrong media type for '{0}'.", mediaTypeWithQuality));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) matches media type with quality from media type mapping.")]
        public void SelectResponseMediaTypeMatchesAcceptHeaderWithMediaTypeMapping()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaRangeWithQuality in DataSets.Http.MediaRangeValuesWithQuality)
            {
                MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
                MediaTypeHeaderValue mediaRangeWithoutQuality = new MediaTypeHeaderValue(mediaRangeWithQuality.MediaType);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/xml");
                MediaRangeMapping mapping = new MediaRangeMapping(mediaRangeWithoutQuality, mediaType);
                formatter.MediaTypeMappings.Add(mapping);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Accept.Add(mediaRangeWithQuality);
                HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                ResponseMediaTypeMatch match = formatter.SelectResponseMediaType(typeof(string), response);

                Assert.IsNotNull(match, string.Format("SelectResponseMediaType returned null for '{0}'.", mediaRangeWithQuality));
                Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnRequestAcceptHeaderWithMediaTypeMapping, match.ResponseFormatterSelectionResult, string.Format("SelectResponseMediaType returned the wrong enum for '{0}'.", mediaRangeWithQuality));
                double quality = mediaRangeWithQuality.Quality.Value;
                Assert.AreEqual(quality, match.MediaTypeMatch.Quality, string.Format("SelectResponseMediaType returned the wrong quality for '{0}'.", mediaRangeWithQuality));
                Assert.IsNotNull(match.MediaTypeMatch.MediaType, string.Format("SelectResponseMediaType returned null media type for '{0}'.", mediaRangeWithQuality));
                Assert.AreEqual(mediaType.MediaType, match.MediaTypeMatch.MediaType.MediaType, string.Format("SelectResponseMediaType returned wrong media type for '{0}'.", mediaRangeWithQuality));
            }
        }

        #endregion SelectResponseMediaType(Type, HttpResponseMessage)

        #region CanReadAs(Type, HttpContent)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpContent) returns true for all standard media types.")]
        public void CanReadAsReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            string[] legalMediaTypeStrings = DataSets.Http.LegalMediaTypeStrings.ToArray();
            foreach (string mediaType in legalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in legalMediaTypeStrings)
                    {
                        StringContent content = new StringContent("data");
                        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                        Assert.IsTrue(formatter.CanReadAs(type, content), string.Format("CanReadAs should have returned true for '{0}'.", type));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpContent) throws with null type.")]
        public void CanReadAsThrowsWithNullType()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            StringContent content = new StringContent("data");
            Asserters.Exception.ThrowsArgumentNull("type", () => formatter.CanReadAs(null, content));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpContent) throws with null content.")]
        public void CanReadAsThrowsWithNullContent()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("content", () => formatter.CanReadAs(typeof(int), (HttpContent)null));
        }

        #endregion CanReadAs(Type, HttpContent)

        #region CanReadAs(Type, HttpRequestMessage, out MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpRequestMessage) returns true for all standard media types.")]
        public void CanReadAs1ReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            string[] legalMediaTypeStrings = DataSets.Http.LegalMediaTypeStrings.ToArray();
            foreach (string mediaType in legalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in legalMediaTypeStrings)
                    {
                        MockObjectContent objectContent = new MockObjectContent(type, obj);
                        objectContent.Headers.ContentType = formatter.SupportedMediaTypes[0];
                        Assert.IsTrue(formatter.CanReadAs(type, new HttpRequestMessage() { Content = objectContent }), string.Format("CanReadAs should have returned true for '{0}'.", type));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpRequestMessage) throws with null ObjectContent.")]
        public void CanReadAs1ThrowsWithNullContent()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => formatter.CanReadAs((Type)null, new HttpRequestMessage()));
        }

        #endregion CanReadAs(Type, HttpRequestMessage, out MediaTypeHeaderValue)

        #region CanReadAs(Type, HttpResponseMessage, out MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpResponseMessage) returns true for all standard media types.")]
        public void CanReadAs2ReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            string[] legalMediaTypeStrings = DataSets.Http.LegalMediaTypeStrings.ToArray();
            foreach (string mediaType in legalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in legalMediaTypeStrings)
                    {
                        MockObjectContent objectContent = new MockObjectContent(type, obj);
                        objectContent.Headers.ContentType = formatter.SupportedMediaTypes[0];
                        Assert.IsTrue(formatter.CanReadAs(type, new HttpResponseMessage() { Content = objectContent }), string.Format("CanReadAs should have returned true for '{0}'.", type));
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadAs(Type, HttpResponseMessage) throws with null Type.")]
        public void CanReadAs2ThrowsWithNullContent()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => formatter.CanReadAs((Type)null, new HttpResponseMessage()));
        }

        #endregion CanReadAs(Type, HttpResponseMessage, out MediaTypeHeaderValue)

        #region CanWriteAsAs(Type, HttpContent, out MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanWriteAs(Type, HttpContent, out MediaTypeHeaderValue) returns true always for supported media types.")]
        public void CanWriteAsReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MediaTypeHeaderValue matchedMediaType = null;
                    MockObjectContent objectContent = new MockObjectContent(type, obj);
                    objectContent.Headers.ContentType = formatter.SupportedMediaTypes[0];
                    Assert.IsTrue(formatter.CanWriteAs(type, objectContent, out matchedMediaType), string.Format("CanWriteAs should have returned true for '{0}'.", type));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanWriteAs(Type, HttpContent, out MediaTypeHeaderValue) throws with null content.")]
        public void CanWriteAsThrowsWithNullContent()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            MediaTypeHeaderValue mediaType = null;
            Asserters.Exception.ThrowsArgumentNull("content", () => formatter.CanWriteAs(typeof(int), (HttpContent)null, out mediaType));
        }

        #endregion CanWriteAs(Type, HttpContent, out MediaTypeHeaderValue)

        #region CanWriteAsAs(Type, HttpRequestMessage, out MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanWriteAs(Type, HttpRequestMessage, out MediaTypeHeaderValue) returns true always for supported media types.")]
        public void CanWriteAs1ReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MediaTypeHeaderValue matchedMediaType = null;
                    MockObjectContent objectContent = new MockObjectContent(type, obj);
                    objectContent.Headers.ContentType = formatter.SupportedMediaTypes[0];
                    Assert.IsTrue(formatter.CanWriteAs(type, new HttpRequestMessage() { Content = objectContent }, out matchedMediaType), string.Format("CanWriteAs should have returned true for '{0}'.", type));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanWriteAs(Type, HttpRequestMessage, out MediaTypeHeaderValue) throws with null type.")]
        public void CanWriteAs1ThrowsWithNullType()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            MediaTypeHeaderValue mediaType = null;
            Asserters.Exception.ThrowsArgumentNull("type", () => formatter.CanWriteAs(null, new HttpRequestMessage(), out mediaType));
        }

        #endregion CanWriteAs(Type, HttpRequestMessage, out MediaTypeHeaderValue)

        #region CanWriteAsAs(Type, HttpResponseMessage, out MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) returns match always for supported media types.")]
        public void CanWriteAs2ReturnsMatch()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MockObjectContent objectContent = new MockObjectContent(type, obj);
                    objectContent.Headers.ContentType = formatter.SupportedMediaTypes[0];
                    ResponseMediaTypeMatch mediaTypeMatch = formatter.SelectResponseMediaType(type, new HttpResponseMessage() { Content = objectContent });
                    Assert.AreEqual(ResponseFormatterSelectionResult.MatchOnResponseContentType, mediaTypeMatch.ResponseFormatterSelectionResult, string.Format("SelectResponseMediaType should have returned MatchOnResponseContentType for '{0}'.", type));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("SelectResponseMediaType(Type, HttpResponseMessage) throws with null type.")]
        public void CanWriteAs2ThrowsWithNullContent()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => formatter.SelectResponseMediaType(null, new HttpResponseMessage()));
        }

        #endregion CanWriteAs(Type, HttpResponseMessage, out MediaTypeHeaderValue)

        #region CanReadType()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanReadType() base implementation returns true always.")]
        public void CanReadTypeReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            string[] legalMediaTypeStrings = DataSets.Http.LegalMediaTypeStrings.ToArray();
            foreach (string mediaType in legalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    foreach (string mediaType in legalMediaTypeStrings)
                    {
                        StringContent content = new StringContent("data");
                        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                        Assert.IsTrue(formatter.CanReadAs(type, content), string.Format("CanReadType should have returned true for '{0}'.", type));
                    }
                });
        }

        #endregion CanReadType()

        #region CanWriteType()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("CanWriteType() base implementation returns true always.")]
        public void CanWriteTypeReturnsTrue()
        {
            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    MediaTypeHeaderValue matchedMediaType = null;
                    MockObjectContent objectContent = new MockObjectContent(type, obj, "application/xml");
                    Assert.IsTrue(formatter.CanWriteAs(type, objectContent, out matchedMediaType), string.Format("CanWriteType should have returned true for '{0}'.", type));
                });
        }

        #endregion CanWriteType()

        #region ReadFromStream()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStream() calls protected OnReadFromStream().")]
        public void ReadFromStreamCallsOnReadFromStream()
        {
            Type calledType = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.OnReadFromStreamCallback = (type, stream, headers) =>
                {
                    calledType = type;
                    calledStream = stream;
                    calledHeaders = headers;
                    return null;
                };
            MockHttpContent content = new MockHttpContent();
            HttpContentHeaders contentHeaders = content.Headers;

            Asserters.Stream.WriteAndRead(
                (stream) => { },
                (stream) => formatter.ReadFromStream(typeof(int), stream, contentHeaders));

            Assert.AreEqual(typeof(int), calledType, "OnReadFromStream was not called or did not pass Type.");
            Assert.IsNotNull(calledStream, "OnReadFromStream was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnReadFromStream was not called or did not pass ContentHeaders.");
        }

        #endregion ReadFromStream()

        #region ReadFromStreamAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStreamAsync() calls protected OnReadFromStreamAsync().")]
        public void ReadFromStreamAsyncCallsOnReadFromStreamAsync()
        {
            Type calledType = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.OnReadFromStreamAsyncCallback = (type, stream, headers) =>
            {
                calledType = type;
                calledStream = stream;
                calledHeaders = headers;
                return null;
            };
            MockHttpContent content = new MockHttpContent();
            HttpContentHeaders contentHeaders = content.Headers;

            Asserters.Stream.WriteAndRead(
                (stream) => { },
                (stream) => formatter.ReadFromStreamAsync(typeof(int), stream, contentHeaders));

            Assert.AreEqual(typeof(int), calledType, "OnReadFromStreamAsync was not called or did not pass Type.");
            Assert.IsNotNull(calledStream, "OnReadFromStreamAsync was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnReadFromStreamAsync was not called or did not pass ContentHeaders.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadFromStreamAsync() calls protected OnReadFromStream and wraps a Task around it().")]
        public void ReadFromStreamAsyncCallsOnReadFromStreamInTask()
        {
            Type calledType = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            formatter.OnReadFromStreamCallback = (type, stream, headers) =>
            {
                calledType = type;
                calledStream = stream;
                calledHeaders = headers;
                return 5;
            };

            MockHttpContent content = new MockHttpContent(new StringContent("data"));
            HttpContentHeaders contentHeaders = content.Headers;

            Task<object> createdTask =
                Asserters.Stream.WriteAndReadResult<Task<object>>(
                    (stream) => { },
                    (stream) => formatter.ReadFromStreamAsync(typeof(int), stream, contentHeaders));

            object readObject = Asserters.Task.SucceedsWithResult(createdTask);
            Assert.AreEqual(5, readObject, "ReadFromStreamAsync should have returned this value from stub.");
            Assert.AreEqual(typeof(int), calledType, "OnReadFromStreamAsync was not called or did not pass Type.");
            Assert.IsNotNull(calledStream, "OnReadFromStreamAsync was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnReadFromStreamAsync was not called or did not pass ContentHeaders.");
        }

        #endregion ReadFromStreamAsync()

        #region WriteToStream()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("WriteToStream() calls protected OnWriteToStream().")]
        public void WriteToStreamCallsOnWriteToStream()
        {
            Type calledType = null;
            object calledObj = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.OnWriteToStreamCallback = (type, obj, stream, headers, context) =>
            {
                calledType = type;
                calledObj = obj;
                calledStream = stream;
                calledHeaders = headers;
            };
            MockHttpContent content = new MockHttpContent();
            HttpContentHeaders contentHeaders = content.Headers;

            Asserters.Stream.WriteAndRead(
                (stream) => formatter.WriteToStream(typeof(int), 5, stream, contentHeaders, /*transportContext*/ null),
                (stream) => { });

            Assert.AreEqual(typeof(int), calledType, "OnWriteToStream was not called or did not pass Type.");
            Assert.AreEqual(5, calledObj, "OnWriteToStream was not called or did not pass the object value.");
            Assert.IsNotNull(calledStream, "OnWriteToStream was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnWriteToStream was not called or did not pass ContentHeaders.");
        }

        #endregion WriteToStream()

        #region WriteToStreamAsync()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("WriteToStreamAsync() calls protected OnWriteToStreamAsync().")]
        public void WriteToStreamAsyncCallsOnWriteToStreamAsync()
        {
            Type calledType = null;
            object calledObj = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;
            Task calledTask = null;
            Task createdTask = new Task(() => { });

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter();
            formatter.OnWriteToStreamAsyncCallback = (type, obj, stream, headers, context) =>
            {
                calledType = type;
                calledObj = obj;
                calledStream = stream;
                calledHeaders = headers;
                return createdTask;
            };

            MockHttpContent content = new MockHttpContent();
            HttpContentHeaders contentHeaders = content.Headers;

            Asserters.Stream.WriteAndRead(
                (stream) => calledTask = formatter.WriteToStreamAsync(typeof(int), 5, stream, contentHeaders, /*transportContext*/ null),
                (stream) => { });

            Assert.AreEqual(typeof(int), calledType, "OnWriteToStreamAsync was not called or did not pass Type.");
            Assert.AreEqual(5, calledObj, "OnWriteToStreamAsync was not called or did not pass the object value.");
            Assert.IsNotNull(calledStream, "OnWriteToStreamAsync was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnWriteToStreamAsync was not called or did not pass ContentHeaders.");
            Assert.AreSame(createdTask, calledTask, "OnWriteToStreamAsync was not called or did not return the Task result.");
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("WriteToStreamAsync() calls protected OnWriteToStream() and wraps a Task around it.")]
        public void WriteToStreamAsyncCallsOnWriteToStreamInTask()
        {
            Type calledType = null;
            object calledObj = null;
            Stream calledStream = null;
            HttpContentHeaders calledHeaders = null;
            Task createdTask = null;

            MockMediaTypeFormatter formatter = new MockMediaTypeFormatter() { CallBase = true };
            formatter.OnWriteToStreamCallback = (type, obj, stream, headers, context) =>
            {
                calledType = type;
                calledObj = obj;
                calledStream = stream;
                calledHeaders = headers;
            };

            MockHttpContent content = new MockHttpContent(new StringContent("data"));
            HttpContentHeaders contentHeaders = content.Headers;

            Asserters.Stream.WriteAndRead(
                (stream) => createdTask = formatter.WriteToStreamAsync(typeof(int), 5, stream, contentHeaders, /*transportContext*/ null),
                (stream) => { });

            Asserters.Task.Succeeds(createdTask);
            Assert.AreEqual(typeof(int), calledType, "OnWriteToStream was not called or did not pass Type.");
            Assert.AreEqual(5, calledObj, "OnWriteToStream was not called or did not pass the object value.");
            Assert.IsNotNull(calledStream, "OnWriteToStream was not called or did not pass Type.");
            Assert.AreSame(contentHeaders, calledHeaders, "OnWriteToStream was not called or did not pass ContentHeaders.");
        }

        #endregion WriteToStreamAsync()

        #endregion Methods
    }
}
