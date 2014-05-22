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
    public class RequestHeaderMappingTests : UnitTest<RequestHeaderMapping>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping is public, and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest,
                TypeAssert.TypeProperties.IsPublicVisibleClass,
                typeof(MediaTypeMapping));
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, MediaTypeHeaderValue) sets properties.")]
        public void Constructor()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, true, mediaType);
                        Assert.AreEqual(headerName, mapping.HeaderName, "HeaderName failed to set.");
                        Assert.AreEqual(headerValue, mapping.HeaderValue, "HeaderValue failed to set.");
                        Assert.AreEqual(StringComparison.CurrentCulture, mapping.HeaderValueComparison, "HeaderValueComparison failed to set.");
                        Assert.AreEqual(true, mapping.IsValueSubstring, "IsValueSubstring failed to set.");
                        Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, MediaTypeHeaderValue) throws with empty headerName.")]
        public void ConstructorThrowsWithEmptyHeaderName()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (string headerName in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("headerName", () => new RequestHeaderMapping(headerName, "value", StringComparison.CurrentCulture, false, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, MediaTypeHeaderValue) throws with empty headerValue.")]
        public void ConstructorThrowsWithEmptyHeaderValue()
        {
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
            {
                foreach (string headerValue in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("headerValue", () => new RequestHeaderMapping("name", headerValue, StringComparison.CurrentCulture, false, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, MediaTypeHeaderValue) throws with null MediaTypeHeaderValue.")]
        public void ConstructorThrowsWithNullMediaTypeHeaderValue()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    Asserters.Exception.ThrowsArgumentNull("mediaType", () => new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, false, (MediaTypeHeaderValue)null));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, MediaTypeHeaderValue) throws with invalid StringComparison.")]
        public void ConstructorThrowsWithInvalidStringComparison()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (MediaTypeHeaderValue mediaType in DataSets.Http.LegalMediaTypeHeaderValues)
                    {
                        int invalidValue = 999;
                        Asserters.Exception.ThrowsInvalidEnumArgument("valueComparison", invalidValue, typeof(StringComparison),
                            () => new RequestHeaderMapping(headerName, headerValue, (StringComparison)invalidValue, false, mediaType));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, string) sets properties.")]
        public void Constructor1()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, true, mediaType);
                        Assert.AreEqual(headerName, mapping.HeaderName, "HeaderName failed to set.");
                        Assert.AreEqual(headerValue, mapping.HeaderValue, "HeaderValue failed to set.");
                        Assert.AreEqual(StringComparison.CurrentCulture, mapping.HeaderValueComparison, "HeaderValueComparison failed to set.");
                        Assert.AreEqual(true, mapping.IsValueSubstring, "IsValueSubstring failed to set.");
                        Asserters.MediaType.AreEqual(mediaType, mapping.MediaType, "MediaType failed to set.");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, string) throws with empty headerName.")]
        public void Constructor1ThrowsWithEmptyHeaderName()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (string headerName in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("headerName", () => new RequestHeaderMapping(headerName, "value", StringComparison.CurrentCulture, false, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, string) throws with empty headerValue.")]
        public void Constructor1ThrowsWithEmptyHeaderValue()
        {
            foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
            {
                foreach (string headerValue in TestData.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("headerValue", () => new RequestHeaderMapping("name", headerValue, StringComparison.CurrentCulture, false, mediaType));
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, string) throws with empty MediaTypeHeaderValue.")]
        public void Constructor1ThrowsWithEmptyMediaType()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in TestData.EmptyStrings)
                    {
                        Asserters.Exception.ThrowsArgumentNull("mediaType", () => new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, false, mediaType));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("RequestHeaderMapping(string, string, StringComparison, bool, string) throws with invalid StringComparison.")]
        public void Constructor1ThrowsWithInvalidStringComparison()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        int invalidValue = 999;
                        Asserters.Exception.ThrowsInvalidEnumArgument("valueComparison", invalidValue, typeof(StringComparison),
                            () => new RequestHeaderMapping(headerName, headerValue, (StringComparison)invalidValue, false, mediaType));
                    }
                }
            }
        }

        #endregion  Constructors

        #region Members

        #region TryMatchMediaType(HttpRequestMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns true when the HeaderName and HeaderValue are in the request.")]
        public void TryMatchMediaTypeReturnsTrueWithNameAndValueInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        foreach (bool subset in DataSets.Common.Bools)
                        {
                            RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, subset, mediaType);
                            HttpRequestMessage request = new HttpRequestMessage();
                            request.Headers.Add(headerName, headerValue);
                            Assert.AreEqual(1.0, mapping.TryMatchMediaType(request), 
                                string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns true when the HeaderName and a HeaderValue subset are in the request.")]
        public void TryMatchMediaTypeReturnsTrueWithNameAndValueSubsetInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, true, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue);
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, headerValue + "postfix");
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue + "postfix");
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns false when HeaderName is not in the request.")]
        public void TryMatchMediaTypeReturnsFalseWithNameNotInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, false, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add("prefix" + headerName, headerValue);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName + "postfix", headerValue);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add("prefix" + headerName + "postfix", headerValue);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpRequestMessage) returns false when HeaderValue is not in the request.")]
        public void TryMatchMediaTypeReturnsFalseWithValueNotInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, false, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue);
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, headerValue + "postfix");
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue + "postfix");
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(request), 
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null HttpRequestMessage.")]
        public void TryMatchMediaTypeThrowsWithNullHttpRequestMessage()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, true, mediaType);
                        Asserters.Exception.ThrowsArgumentNull("request", () => mapping.TryMatchMediaType((HttpRequestMessage)null));
                    }
                }
            }
        }

        #endregion TryMatchMediaType(HttpRequestMessage)

        #region TryMatchMediaType(HttpResponseMessage)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns true when the headerName and headerValue are in the request.")]
        public void TryMatchMediaType1ReturnsTrueWithNameAndValueSubsetInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, true, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue);
                        HttpResponseMessage response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, headerValue + "postfix");
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue + "postfix");
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(1.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned true for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns false when HeaderName is not in the request.")]
        public void TryMatchMediaType1ReturnsFalseWithQueryStringParameterNameNotInUri()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, false, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add("prefix" + headerName, headerValue);
                        HttpResponseMessage response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName + "postfix", headerValue);
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add("prefix" + headerName + "postfix", headerValue);
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) returns false when HeaderValue is not in the request.")]
        public void TryMatchMediaType1ReturnsFalseWithValueNotInRequest()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.Ordinal, false, mediaType);
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue);
                        HttpResponseMessage response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, headerValue + "postfix");
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));

                        request = new HttpRequestMessage();
                        request.Headers.Add(headerName, "prefix" + headerValue + "postfix");
                        response = new HttpResponseMessage { RequestMessage = request };
                        Assert.AreEqual(0.0, mapping.TryMatchMediaType(response),
                            string.Format("TryMatchMediaType should have returned false for '{0}: {1}'.", headerName, headerValue));
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryMatchMediaType(HttpResponseMessage) throws with a null HttpResponseMessage.")]
        public void TryMatchMediaType1ThrowsWithNullHttpResponseMessage()
        {
            foreach (string headerName in DataSets.Http.LegalHttpHeaderNames)
            {
                foreach (string headerValue in DataSets.Http.LegalHttpHeaderValues)
                {
                    foreach (string mediaType in DataSets.Http.LegalMediaTypeStrings)
                    {
                        RequestHeaderMapping mapping = new RequestHeaderMapping(headerName, headerValue, StringComparison.CurrentCulture, true, mediaType);
                        Asserters.Exception.ThrowsArgumentNull("response", () => mapping.TryMatchMediaType((HttpResponseMessage)null));
                    }
                }
            }
        }

        #endregion 

        #endregion
    }
}
