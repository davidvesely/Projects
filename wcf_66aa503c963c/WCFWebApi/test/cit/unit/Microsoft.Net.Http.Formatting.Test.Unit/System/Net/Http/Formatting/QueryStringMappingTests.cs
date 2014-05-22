// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Test;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class QueryStringMappingTests : UnitTest<QueryStringMapping>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping is public, concrete, and sealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsSealed,
                typeof(MediaTypeMapping));
        }

        #endregion Type

        #region Constructors

        #region Constructor(string, string, MediaTypeHeaderValue)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, MediaTypeHeaderValue) sets properties.")]
        public void Constructor()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        Assert.AreEqual(queryStringParameterName, mapping.QueryStringParameterName, "QueryStringParameterName failed to set.");
                        Assert.AreEqual(queryStringParameterValue, mapping.QueryStringParameterValue, "QueryStringParameterValue failed to set.");
                        Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, MediaTypeHeaderValue) throws with empty QueryStringParameterName.")]
        public void ConstructorThrowsWithEmptyQueryParameterName()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (string queryStringParameterName in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("queryStringParameterName", () => new QueryStringMapping(queryStringParameterName, "json", mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, MediaTypeHeaderValue) throws with empty QueryStringParameterValue.")]
        public void ConstructorThrowsWithEmptyQueryParameterValue()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (string queryStringParameterValue in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("queryStringParameterValue", () => new QueryStringMapping("query", queryStringParameterValue, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, MediaTypeHeaderValue) throws with null MediaTypeHeaderValue.")]
        public void ConstructorThrowsWithNullMediaTypeHeaderValue()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    Asserters.Exception.ThrowsArgumentNull("mediaType", () => new QueryStringMapping(queryStringParameterName, queryStringParameterValue, (MediaTypeHeaderValue)null));
                }
            }
        }

        #endregion Constructor(string, string, MediaTypeHeaderValue)

        #region Constructor1(string, string, string)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, string) sets properties.")]
        public void Constructor1()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        Assert.AreEqual(queryStringParameterName, mapping.QueryStringParameterName, "QueryStringParameterName failed to set.");
                        Assert.AreEqual(queryStringParameterValue, mapping.QueryStringParameterValue, "QueryStringParameterValue failed to set.");
                        Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, string) throws with empty QueryStringParameterName.")]
        public void Constructor1ThrowsWithEmptyQueryParameterName()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (string queryStringParameterName in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("queryStringParameterName", () => new QueryStringMapping(queryStringParameterName, "json", mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, string) throws with empty QueryStringParameterValue.")]
        public void Constructor1ThrowsWithEmptyQueryParameterValue()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (string queryStringParameterValue in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("queryStringParameterValue", () => new QueryStringMapping("query", queryStringParameterValue, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("QueryStringMapping(string, string, string) throws with empty MediaType.")]
        public void Constructor1ThrowsWithEmptyMediaType()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in TestData.EmptyStrings)
                    {
                        Asserters.Exception.ThrowsArgumentNull("mediaType", () => new QueryStringMapping(queryStringParameterName, queryStringParameterValue, (MediaTypeHeaderValue)null));
                    }
                }
            }
        }

        #endregion Constructor(string, string, string)

        #endregion  Constructors

        #region Properties

        #endregion Properties

        #region Methods

        #region TryMatchMediaType(HttpRequestMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns match when the QueryStringParameterName and QueryStringParameterValue are in the Uri.")]
        public void TryMatchMediaTypeReturnsMatchWithQueryStringParameterNameAndValueInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + queryStringParameterName + "=" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            Assert.AreEqual(1.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 1.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 0.0 when the QueryStringParameterName is not in the Uri.")]
        public void TryMatchMediaTypeReturnsZeroWithQueryStringParameterNameNotInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + "not" + queryStringParameterName + "=" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 0.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns 0.0 when the QueryStringParameterValue is not in the Uri.")]
        public void TryMatchMediaTypeReturnsZeroWithQueryStringParameterValueNotInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + queryStringParameterName + "=" + "not" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), string.Format("TryMatchMediaType should have returned 0.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) throws with a null HttpRequestMessage.")]
        public void TryMatchMediaTypeThrowsWithNullHttpRequestMessage()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        Asserters.Exception.ThrowsArgumentNull("request", () => mapping.TryMatchMediaType((HttpRequestMessage)null));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) throws with a null Uri in HttpRequestMessage.")]
        public void TryMatchMediaTypeThrowsWithNullUriInHttpRequestMessage()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        string errorMessage = SR.NonNullUriRequiredForMediaTypeMapping(typeof(QueryStringMapping).Name);
                        Asserters.Exception.Throws<InvalidOperationException>("Null Uri should throw.", errorMessage, () => mapping.TryMatchMediaType(new HttpRequestMessage()));
                    }
                }
            }
        }

        #endregion TryMatchMediaType(HttpRequestMessage)

        #region TryMatchMediaType(HttpResponseMessage)


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 1.0 when the QueryStringParameterName and QueryStringParameterValue are in the Uri.")]
        public void TryMatchMediaType1ReturnsTrueWithQueryStringParameterNameAndValueInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + queryStringParameterName + "=" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                            Assert.AreEqual(1.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 1.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 0.0 when the QueryStringParameterName is not in the Uri.")]
        public void TryMatchMediaType1ReturnsZeroWithQueryStringParameterNameNotInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + "not" + queryStringParameterName + "=" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                            Assert.AreEqual(0.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 0.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns 0.0 when the QueryStringParameterValue is not in the Uri.")]
        public void TryMatchMediaType1ReturnsZeroWithQueryStringParameterValueNotInUri()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        foreach (string uriBase in DataSets.WCF.UriStrings.Where((s) => !s.Contains('?')))
                        {
                            string uri = uriBase + "?" + queryStringParameterName + "=" + "not" + queryStringParameterValue;
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                            HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                            Assert.AreEqual(0.0, mapping.TryMatchMediaType(response), string.Format("TryMatchMediaType should have returned 0.0 for '{0}'.", uri));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null HttpResponseMessage.")]
        public void TryMatchMediaType1ThrowsWithNullHttpResponseMessage()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        Asserters.Exception.ThrowsArgumentNull("response", () => mapping.TryMatchMediaType((HttpResponseMessage)null));
                    }
                }
            }
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null HttpRequestMessage in the HttpResponseMessage.")]
        public void TryMatchMediaType1ThrowsWithNullRequestInHttpResponseMessage()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        string errorMessage = SR.ResponseMustReferenceRequest(typeof(HttpResponseMessage).Name, "response", typeof(HttpRequestMessage).Name, "RequestMessage");
                        Asserters.Exception.Throws<InvalidOperationException>("Null request in response should throw.", errorMessage, () => mapping.TryMatchMediaType(new HttpResponseMessage()));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null Uri in HttpRequestMessage.")]
        public void TryMatchMediaType1ThrowsWithNullUriInHttpRequestMessage()
        {
            foreach (string queryStringParameterName in DataSets.Http.LegalQueryStringParameterNames)
            {
                foreach (string queryStringParameterValue in DataSets.Http.LegalQueryStringParameterValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        QueryStringMapping mapping = new QueryStringMapping(queryStringParameterName, queryStringParameterValue, mediaType);
                        string errorMessage = SR.NonNullUriRequiredForMediaTypeMapping(typeof(QueryStringMapping).Name);
                        HttpRequestMessage request = new HttpRequestMessage();
                        HttpResponseMessage response = new HttpResponseMessage() { RequestMessage = request };
                        Asserters.Exception.Throws<InvalidOperationException>("Null Uri should throw.", errorMessage, () => mapping.TryMatchMediaType(response));
                    }
                }
            }
        }

        #endregion TryMatchMediaType(HttpResponseMessage)

        #endregion Methods
    }
}
