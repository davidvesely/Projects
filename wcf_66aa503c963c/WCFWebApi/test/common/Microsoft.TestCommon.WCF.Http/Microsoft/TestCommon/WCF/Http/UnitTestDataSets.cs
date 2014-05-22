// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Formatting;
    using Microsoft.TestCommon.WCF;

    public class UnitTestDataSets : Microsoft.TestCommon.WCF.UnitTestDataSets
    {
        public TestData<HttpMethod> AllHttpMethods { get { return HttpTestData.AllHttpMethods; } }

        public TestData<HttpMethod> StandardHttpMethods { get { return HttpTestData.StandardHttpMethods; } }

        public TestData<HttpMethod> CustomHttpMethods { get { return HttpTestData.CustomHttpMethods; } }

        public TestData<HttpStatusCode> AllHttpStatusCodes { get { return HttpTestData.AllHttpStatusCodes; } }

        public TestData<HttpStatusCode> CustomHttpStatusCodes { get { return HttpTestData.CustomHttpStatusCodes; } }

        public ReadOnlyCollection<TestData> ConvertablePrimitiveValueTypes { get { return HttpTestData.ConvertablePrimitiveValueTypes; } }
        
        public ReadOnlyCollection<TestData> ConvertableEnumTypes { get { return HttpTestData.ConvertableEnumTypes; } }
        
        public ReadOnlyCollection<TestData> ConvertableValueTypes { get { return HttpTestData.ConvertableValueTypes; } }
        
        public TestData<MediaTypeHeaderValue> StandardJsonMediaTypes { get { return HttpTestData.StandardJsonMediaTypes; } }
        
        public TestData<MediaTypeHeaderValue> StandardXmlMediaTypes { get { return HttpTestData.StandardXmlMediaTypes; } }

        public TestData<MediaTypeHeaderValue> StandardODataMediaTypes { get { return HttpTestData.StandardODataMediaTypes; } }

        public TestData<MediaTypeHeaderValue> StandardFormUrlEncodedMediaTypes { get { return HttpTestData.StandardFormUrlEncodedMediaTypes; } }

        public TestData<MediaTypeWithQualityHeaderValue> StandardMediaTypesWithQuality { get { return HttpTestData.StandardMediaTypesWithQuality; } }

        public TestData<string> StandardJsonMediaTypeStrings { get { return HttpTestData.StandardXmlMediaTypeStrings; } }
        
        public TestData<string> StandardXmlMediaTypeStrings { get { return HttpTestData.StandardXmlMediaTypeStrings; } }
        
        public TestData<string> LegalMediaTypeStrings { get { return HttpTestData.LegalMediaTypeStrings; } }

        public TestData<string> IllegalMediaTypeStrings { get { return HttpTestData.IllegalMediaTypeStrings; } }

        public TestData<MediaTypeHeaderValue> LegalMediaTypeHeaderValues { get { return HttpTestData.LegalMediaTypeHeaderValues; } }
       
        public TestData<HttpContent> StandardHttpContents { get { return HttpTestData.StandardHttpContents; } }
       
        public TestData<MediaTypeMapping> StandardMediaTypeMappings { get { return HttpTestData.StandardMediaTypeMappings; } }

        public TestData<QueryStringMapping> QueryStringMappings { get { return HttpTestData.QueryStringMappings; } }
        
        public TestData<UriPathExtensionMapping> UriPathExtensionMappings { get { return HttpTestData.UriPathExtensionMappings; } }

        public TestData<MediaRangeMapping> MediaRangeMappings { get { return HttpTestData.MediaRangeMappings; } }

        public TestData<string> LegalUriPathExtensions { get { return HttpTestData.LegalUriPathExtensions; } }

        public TestData<string> LegalQueryStringParameterNames { get { return HttpTestData.LegalQueryStringParameterNames; } }

        public TestData<string> LegalQueryStringParameterValues { get { return HttpTestData.LegalQueryStringParameterValues; } }

        public TestData<string> LegalHttpHeaderNames { get { return HttpTestData.LegalHttpHeaderNames; } }

        public TestData<string> LegalHttpHeaderValues { get { return HttpTestData.LegalHttpHeaderValues; } }

        public TestData<string> LegalMediaRangeStrings { get { return HttpTestData.LegalMediaRangeStrings; } }

        public TestData<MediaTypeHeaderValue> LegalMediaRangeValues { get { return HttpTestData.LegalMediaRangeValues; } }

        public TestData<MediaTypeWithQualityHeaderValue> MediaRangeValuesWithQuality { get { return HttpTestData.MediaRangeValuesWithQuality; } }

        public TestData<string> IllegalMediaRangeStrings { get { return HttpTestData.IllegalMediaRangeStrings; } }

        public TestData<MediaTypeHeaderValue> IllegalMediaRangeValues { get { return HttpTestData.IllegalMediaRangeValues; } }

        public TestData<MediaTypeFormatter> StandardFormatters { get { return HttpTestData.StandardFormatters; } }

        public TestData<Type> StandardFormatterTypes { get { return HttpTestData.StandardFormatterTypes; } }

        public TestData<MediaTypeFormatter> DerivedFormatters { get { return HttpTestData.DerivedFormatters; } }

        public TestData<IEnumerable<MediaTypeFormatter>> AllFormatterCollections { get { return HttpTestData.AllFormatterCollections; } }

        public TestData<string> LegalHttpAddresses { get { return HttpTestData.LegalHttpAddresses; } }

        public TestData<string> AddressesWithIllegalSchemes { get { return HttpTestData.AddressesWithIllegalSchemes; } }

        public TestData<HttpRequestMessage> NullContentHttpRequestMessages { get { return HttpTestData.NullContentHttpRequestMessages; } }

        public ReadOnlyCollection<TestData> NonHttpRequestMessages { get { return HttpTestData.NonHttpRequestMessages; } }

        public ReadOnlyCollection<TestData> RepresentativeValueAndRefTypeTestDataCollection { get { return HttpTestData.RepresentativeValueAndRefTypeTestDataCollection; } }

        public TestData<string> LegalHttpParameterNames { get { return HttpTestData.LegalHttpParameterNames; } }

        public TestData<Type> LegalHttpParameterTypes { get { return HttpTestData.LegalHttpParameterTypes; } }
    }
}
