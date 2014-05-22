// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using System.Net.Http.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class MediaRangeMappingTests : UnitTest<MediaRangeMapping>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping is public, concrete, and sealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest, 
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsSealed, 
                typeof(MediaTypeMapping));
        }

        #endregion Type

        #region Constructors

        #region MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue) sets public properties.")]
        public void Constructor()
        {
            foreach (MediaTypeHeaderValue mediaRange in DataSets.Http.LegalMediaRangeValues)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                   Asserters.MediaType.AreEqual(mediaRange, mapping.MediaRange, "MediaRange failed to set.");
                   Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue) throws if the MediaRange parameter is null.")]
        public void ConstructorThrowsWithNullMediaRange()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                Asserters.Exception.ThrowsArgumentNull("mediaRange", () => new MediaRangeMapping((MediaTypeHeaderValue)null, mediaType));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue) throws if the MediaType parameter is null.")]
        public void ConstructorThrowsWithNullMediaType()
        {
            foreach (MediaTypeHeaderValue mediaRange in DataSets.Http.LegalMediaRangeValues)
            {
                Asserters.Exception.ThrowsArgumentNull("mediaType", () => new MediaRangeMapping(mediaRange, (MediaTypeHeaderValue)null));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue) throws if the MediaRange parameter is not really a media range.")]
        public void ConstructorThrowsWithIllegalMediaRange()
        {
            foreach (MediaTypeHeaderValue mediaRange in DataSets.Http.IllegalMediaRangeValues)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    string errorMessage = SR.InvalidMediaRange(mediaRange.MediaType);
                    Asserters.Exception.Throws<InvalidOperationException>("Invalid media range should throw.", errorMessage, () => new MediaRangeMapping(mediaRange, mediaType));
                }
            }
        }

        #endregion MediaRangeMapping(MediaTypeHeaderValue, MediaTypeHeaderValue)


        #region MediaRangeMapping(string, string)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(string, string) sets public properties.")]
        public void Constructor1()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    Asserters.MediaType.AreEqual(mediaRange, mapping.MediaRange, "MediaRange failed to set.");
                    Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(string, string) throws if the MediaRange parameter is empty.")]
        public void Constructor1ThrowsWithEmptyMediaRange()
        {
            foreach (string mediaRange in TestData.EmptyStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("mediaRange", () => new MediaRangeMapping(mediaRange, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(string, string) throws if the MediaType parameter is empty.")]
        public void Constructor1ThrowsWithEmptyMediaType()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("mediaType", () => new MediaRangeMapping(mediaRange, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaRangeMapping(string, string) throws if the MediaRange parameter is not really a media range.")]
        public void Constructor1ThrowsWithIllegalMediaRange()
        {
            foreach (string mediaRange in DataSets.Http.IllegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    string errorMessage = SR.InvalidMediaRange(mediaRange);
                    Asserters.Exception.Throws<InvalidOperationException>("Invalid media range should throw.", errorMessage, () => new MediaRangeMapping(mediaRange, mediaType));
                }
            }
        }

        #endregion MediaRangeMapping(string, string)

        #endregion  Constructors

        #region Properties

        #endregion Properties

        #region Methods

        #region TryMatchMediaType(HttpRequestMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 0.0 unconditionally.")]
        public void TryMatchMediaTypeReturnsZeroAlways()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    HttpRequestMessage request = new HttpRequestMessage();
                    Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), "TryMatchMediaType should have returned no match.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) throws with null HttpRequestMessage.")]
        public void TryMatchMediaTypeThrowsWithNullHttpRequestMessage()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    HttpRequestMessage request = null;
                    Asserters.Exception.ThrowsArgumentNull(
                        "request",
                        () => mapping.TryMatchMediaType(request));
                }
            }
        }

        #endregion TryMatchMediaType(HttpRequestMessage)

        #region TryMatchMediaType(HttpResponseMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 1.0 when the MediaRange is in the accept headers.")]
        public void TryMatchMediaType1ReturnsOneWithMediaRangeInAcceptHeader()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaRange));
                    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                    Assert.AreEqual(1.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 1.0 for '{0}'.", mediaRange));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns quality factor when a MediaRange with quality is in the accept headers.")]
        public void TryMatchMediaType1ReturnsQualityWithMediaRangeWithQualityInAcceptHeader()
        {
            foreach (MediaTypeWithQualityHeaderValue mediaRangeWithQuality in DataSets.Http.MediaRangeValuesWithQuality)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    MediaTypeWithQualityHeaderValue mediaRangeWithNoQuality = new MediaTypeWithQualityHeaderValue(mediaRangeWithQuality.MediaType);
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRangeWithNoQuality, mediaType);
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Headers.Accept.Add(mediaRangeWithQuality);
                    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                    double quality = mediaRangeWithQuality.Quality.Value;
                    Assert.AreEqual(quality, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned quality for '{0}'.", mediaRangeWithQuality));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 0.0 when the MediaRange is not in the accept headers.")]
        public void TryMatchMediaType1ReturnsFalseWithMediaRangeNotInAcceptHeader()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    HttpRequestMessage request = new HttpRequestMessage();
                    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                    Assert.AreEqual(0.0, mapping.TryMatchMediaType(response), "TryMatchMediaType should have returned 0.0 for empty accept headers");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with null HttpResponseMessage.")]
        public void TryMatchMediaType1ThrowsWithNullHttpResponseMessage()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    HttpResponseMessage response = null;
                    Asserters.Exception.ThrowsArgumentNull(
                        "response",
                        () => mapping.TryMatchMediaType(response));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null HttpRequestMessage in HttpResponseMessage.")]
        public void TryMatchMediaType1ThrowsWithNullRequestInHttpResponseMessage()
        {
            foreach (string mediaRange in DataSets.Http.LegalMediaRangeStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    MediaRangeMapping mapping = new MediaRangeMapping(mediaRange, mediaType);
                    string errorMessage = SR.ResponseMustReferenceRequest(typeof(HttpResponseMessage).Name, "response", typeof(HttpRequestMessage).Name, "RequestMessage");
                    Asserters.Exception.Throws<InvalidOperationException>("Null request should throw", errorMessage, () => mapping.TryMatchMediaType(new HttpResponseMessage()));
                }
            }
        }

        #endregion TryMatchMediaType(HttpResponseMessage)

        #endregion Methods

    }
}
