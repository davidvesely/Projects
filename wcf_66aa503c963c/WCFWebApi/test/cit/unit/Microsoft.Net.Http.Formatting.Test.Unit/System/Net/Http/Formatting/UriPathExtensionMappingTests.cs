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
    public class UriPathExtensionMappingTests : UnitTest<UriPathExtensionMapping>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping is public, concrete, and sealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsSealed,
                typeof(MediaTypeMapping));
        }

        #endregion Type

        #region Constructors

        #region UriPathExtensionMapping(string, string)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, string) sets UriPathExtension and MediaType.")]
        public void Constructor()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    Assert.AreEqual(uriPathExtension, mapping.UriPathExtension, "Failed to set UriPathExtension.");
                    Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "Failed to set MediaType.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, string) throws if the UriPathExtensions parameter is null.")]
        public void ConstructorThrowsWithEmptyUriPathExtension()
        {
            foreach (string uriPathExtension in TestData.EmptyStrings)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("uriPathExtension", () => new UriPathExtensionMapping(uriPathExtension, mediaType));
                }
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, string) throws if the MediaType (string) parameter is empty.")]
        public void ConstructorThrowsWithEmptyMediaType()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("mediaType", () => new UriPathExtensionMapping(uriPathExtension, mediaType));
                }
            };
        }

        #endregion UriPathExtensionMapping(string, string)

        #region UriPathExtensionMapping(string, MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, MediaTypeHeaderValue) sets UriPathExtension and MediaType.")]
        public void Constructor1()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    Assert.AreEqual(uriPathExtension, mapping.UriPathExtension, "Failed to set UriPathExtension.");
                    Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "Failed to set MediaType.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, MediaTypeHeaderValue) throws if the UriPathExtensions parameter is null.")]
        public void Constructor1ThrowsWithEmptyUriPathExtension()
        {
            foreach (string uriPathExtension in TestData.EmptyStrings)
            {
                foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                {
                    Asserters.Exception.ThrowsArgumentNull("uriPathExtension", () => new UriPathExtensionMapping(uriPathExtension, mediaType));
                }
            };
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("UriPathExtensionMapping(string, MediaTypeHeaderValue) constructor throws if the mediaType parameter is null.")]
        public void Constructor1ThrowsWithNullMediaType()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                Asserters.Exception.ThrowsArgumentNull("mediaType", () => new UriPathExtensionMapping(uriPathExtension, (MediaTypeHeaderValue)null));
            };
        }

        #endregion UriPathExtensionMapping(string, MediaTypeHeaderValue)

        #endregion  Constructors

        #region Properties
        #endregion Properties

        #region Methods

        #region TryMatchMediaType(HttpRequestMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 1.0 when the extension is in the Uri.")]
        public void TryMatchMediaTypeReturnsMatchWithExtensionInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x." + uriPathExtension);
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 1.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 0.0 when the extension is not in the Uri.")]
        public void TryMatchMediaTypeReturnsZeroWithExtensionNotInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x.");
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 0.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 0.0 when the uri contains the extension but does not end with it.")]
        public void TryMatchMediaTypeReturnsZeroWithExtensionNotLastInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x." + uriPathExtension + "z");
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 0.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) throws if the request is null.")]
        public void TryMatchMediaTypeThrowsWithNullHttpRequestMessage()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    Asserters.Exception.ThrowsArgumentNull("request", () => mapping.TryMatchMediaType((HttpRequestMessage)null));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) throws if the Uri in the request is null.")]
        public void TryMatchMediaTypeThrowsWithNullUriInHttpRequestMessage()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    string errorMessage = SR.NonNullUriRequiredForMediaTypeMapping(typeof(UriPathExtensionMapping).Name);
                    Asserters.Exception.Throws<InvalidOperationException>("Null Uri should throw.", errorMessage, () => mapping.TryMatchMediaType(new HttpRequestMessage()));
                }
            }
        }

        #endregion TryMatchMediaType(HttpRequestMessage)

        #region TryMatchMediaType(HttpResponseMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns match when the extension is in the Uri.")]
        public void TryMatchMediaType1ReturnsMatchWithExtensionInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x." + uriPathExtension);
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 1.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 0.0 when the extension is not in the Uri.")]
        public void TryMatchMediaType1ReturnsZeroWithExtensionNotInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x.");
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 0.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 0.0 when the uri contains the extension but does not end with it.")]
        public void TryMatchMediaType1ReturnsZeroWithExtensionNotLastInUri()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    foreach (string baseUriString in DataSets.WCF.UriStrings)
                    {
                        Uri baseUri = new Uri(baseUriString);
                        Uri uri = new Uri(baseUri, "x." + uriPathExtension + "z");
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                        HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 0.0 for '{0}' and '{1}'.", mediaType, uri));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws if the response is null.")]
        public void TryMatchMediaType1ThrowsWithNullHttpResponseMessage()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    Asserters.Exception.ThrowsArgumentNull("response", () => mapping.TryMatchMediaType((HttpResponseMessage)null));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws if the HttpRequestMessage in the HttpResponseMessage is null.")]
        public void TryMatchMediaType1ThrowsWithNullRequestInHttpResponseMessage()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    string errorMessage = SR.ResponseMustReferenceRequest(typeof(HttpResponseMessage).Name, "response", typeof(HttpRequestMessage).Name, "RequestMessage");
                    Asserters.Exception.Throws<InvalidOperationException>("Null request in response should throw.", errorMessage, () => mapping.TryMatchMediaType(new HttpResponseMessage()));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws if the Uri in the request is null.")]
        public void TryMatchMediaType1ThrowsWithNullUriInHttpRequestMessage()
        {
            foreach (string uriPathExtension in DataSets.Http.LegalUriPathExtensions)
            {
                foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                {
                    UriPathExtensionMapping mapping = new UriPathExtensionMapping(uriPathExtension, mediaType);
                    HttpRequestMessage request = new HttpRequestMessage();
                    HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                    string errorMessage = SR.NonNullUriRequiredForMediaTypeMapping(typeof(UriPathExtensionMapping).Name);
                    Asserters.Exception.Throws<InvalidOperationException>("Null Uri should throw.", errorMessage, () => mapping.TryMatchMediaType(response));
                }
            }
        }
        #endregion TryMatchMediaType(HttpResponseMessage)

        #endregion Methods
    }
}
