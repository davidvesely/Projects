using System.Linq;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class HttpResponseHeadersTest
    {
        private HttpResponseHeaders headers;

        [TestInitialize]
        public void TestInitialize()
        {
            headers = new HttpResponseHeaders();
        }
        
        #region Response headers

        [TestMethod]
        public void Location_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.Location, "Host should be null by default.");

            Uri expected = new Uri("http://example.com/path/");
            headers.Location = expected;
            Assert.AreEqual(expected, headers.Location);

            headers.Location = null;
            Assert.IsNull(headers.Location, "Location should be null after setting it to null.");             
            Assert.IsFalse(headers.Contains("Location"),
                "Header store should not contain a header 'Location' after setting it to null.");
        }

        [TestMethod]
        public void Location_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            // just verify header names are compared using case-insensitive comparison.
            headers.AddWithoutValidation("LoCaTiOn", "  http://www.example.com/path/?q=v  ");
            Assert.AreEqual(new Uri("http://www.example.com/path/?q=v"), headers.Location);

            headers.Clear();
            headers.AddWithoutValidation("Location", "http://host");
            Assert.AreEqual(new Uri("http://host"), headers.Location);

            // This violates the RFCs, the Location header should be absolute.  However,
            // IIS and HttpListener do not enforce this requirement.
            headers.Clear();
            headers.Add("LoCaTiOn", "/relative/");
            Assert.AreEqual<Uri>(new Uri("/relative/", UriKind.Relative), headers.Location);
        }

        [TestMethod]
        public void Location_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Location", " http://example.com http://other");
            Assert.IsNull(headers.GetParsedValues("Location"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Location").Count(), "Store value count");
            Assert.AreEqual(" http://example.com http://other", headers.GetValues("Location").First(), "Store value");
        }

        [TestMethod]
        public void ETag_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.ETag, "ETag should be null by default.");

            EntityTagHeaderValue etag = new EntityTagHeaderValue("\"tag\"", true);
            headers.ETag = etag;
            Assert.AreSame(etag, headers.ETag);

            headers.ETag = null;
            Assert.IsNull(headers.ETag, "ETag should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("ETag"),
                "Header store should not contain a header 'ETag' after setting it to null.");
        }

        [TestMethod]
        public void ETag_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("ETag", "W/\"tag\"");
            Assert.AreEqual(new EntityTagHeaderValue("\"tag\"", true), headers.ETag);
        }

        [TestMethod]
        public void ETag_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("ETag", ",\"tag\""); // leading separator
            Assert.IsNull(headers.ETag, "ETag");
            Assert.AreEqual(1, headers.GetValues("ETag").Count(), "ETag.Count");
            Assert.AreEqual(",\"tag\"", headers.GetValues("ETag").First(), "ETag value");

            headers.Clear();
            headers.AddWithoutValidation("ETag", "\"tag\","); // trailing separator
            Assert.IsNull(headers.ETag, "ETag");
            Assert.AreEqual(1, headers.GetValues("ETag").Count(), "ETag.Count");
            Assert.AreEqual("\"tag\",", headers.GetValues("ETag").First(), "ETag value");
        }

        [TestMethod]
        public void AcceptRanges_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.AcceptRanges.Count, "Collection expected to be empty on first call.");

            headers.AcceptRanges.Add("custom1");
            headers.AcceptRanges.Add("custom2");

            Assert.AreEqual(2, headers.AcceptRanges.Count, "AcceptRanges.Count");
            Assert.AreEqual(2, headers.GetValues("Accept-Ranges").Count(), "Accept-Ranges header value count.");

            Assert.AreEqual("custom1", headers.AcceptRanges.ElementAt(0), "AcceptRanges[0]");
            Assert.AreEqual("custom2", headers.AcceptRanges.ElementAt(1), "AcceptRanges[1]");

            headers.AcceptRanges.Clear();
            Assert.AreEqual(0, headers.AcceptRanges.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Accept-Ranges"), 
                "There should be no Accept-Ranges header after calling Clear().");
        }

        [TestMethod]
        public void AcceptRanges_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Accept-Ranges", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.AcceptRanges.Count, "AcceptRanges.Count");
            Assert.AreEqual(3, headers.GetValues("Accept-Ranges").Count(), "Accept-Ranges header value count.");

            Assert.AreEqual("custom1", headers.AcceptRanges.ElementAt(0), "AcceptRanges[0]");
            Assert.AreEqual("custom2", headers.AcceptRanges.ElementAt(1), "AcceptRanges[1]");
            Assert.AreEqual("custom3", headers.AcceptRanges.ElementAt(2), "AcceptRanges[2]");

            headers.AcceptRanges.Clear();
            Assert.AreEqual(0, headers.AcceptRanges.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Accept-Ranges"),
                "There should be no Accept-Ranges header after calling Clear().");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AcceptRanges_AddInvalidValue_Throw()
        {
            headers.AcceptRanges.Add("this is invalid");
        }

        [TestMethod]
        public void AcceptRanges_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Accept-Ranges", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.AcceptRanges.Count, "AcceptRanges.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Ranges").Count(), "Accept-Ranges.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Accept-Ranges").First(), "Accept-Ranges value");
        }

        [TestMethod]
        public void WwwAuthenticate_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.WwwAuthenticate.Count, "Collection expected to be empty on first call.");

            headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("NTLM"));
            headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""));

            Assert.AreEqual(2, headers.WwwAuthenticate.Count, "WwwAuthenticate.Count");
            Assert.AreEqual(2, headers.GetValues("WWW-Authenticate").Count(), "WWW-Authenticate header value count.");

            Assert.AreEqual(new AuthenticationHeaderValue("NTLM"), 
                headers.WwwAuthenticate.ElementAt(0), "WwwAuthenticate[0]");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""), 
                headers.WwwAuthenticate.ElementAt(1), "WwwAuthenticate[1]");

            headers.WwwAuthenticate.Clear();
            Assert.AreEqual(0, headers.WwwAuthenticate.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("WWW-Authenticate"),
                "There should be no WWW-Authenticate header after calling Clear().");
        }

        [TestMethod]
        public void WwwAuthenticate_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.Add("WWW-Authenticate", "Negotiate");
            headers.AddWithoutValidation("WWW-Authenticate", "Basic realm=\"alexandc-tst7\", Digest a=b, c=d, NTLM");
            headers.AddWithoutValidation("WWW-Authenticate", "Kerberos");

            Assert.AreEqual(5, headers.WwwAuthenticate.Count, "WwwAuthenticate.Count");
            Assert.AreEqual(5, headers.GetValues("WWW-Authenticate").Count(), "WWW-Authenticate header value count.");

            Assert.AreEqual(new AuthenticationHeaderValue("Negotiate"), 
                headers.WwwAuthenticate.ElementAt(0), "WwwAuthenticate[0]");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""), 
                headers.WwwAuthenticate.ElementAt(1), "WwwAuthenticate[1]");
            Assert.AreEqual(new AuthenticationHeaderValue("Digest", "a=b, c=d"),
                headers.WwwAuthenticate.ElementAt(2), "WwwAuthenticate[2]");
            Assert.AreEqual(new AuthenticationHeaderValue("NTLM"), 
                headers.WwwAuthenticate.ElementAt(3), "WwwAuthenticate[3]");
            Assert.AreEqual(new AuthenticationHeaderValue("Kerberos"),
                headers.WwwAuthenticate.ElementAt(4), "WwwAuthenticate[4]");

            headers.WwwAuthenticate.Clear();
            Assert.AreEqual(0, headers.WwwAuthenticate.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("WWW-Authenticate"),
                "There should be no WWW-Authenticate header after calling Clear().");
        }

        [TestMethod]
        public void ProxyAuthenticate_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.ProxyAuthenticate.Count, "Collection expected to be empty on first call.");

            headers.ProxyAuthenticate.Add(new AuthenticationHeaderValue("NTLM"));
            headers.ProxyAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""));

            Assert.AreEqual(2, headers.ProxyAuthenticate.Count, "ProxyAuthenticate.Count");
            Assert.AreEqual(2, headers.GetValues("Proxy-Authenticate").Count(), "Proxy-Authenticate header value count.");

            Assert.AreEqual(new AuthenticationHeaderValue("NTLM"),
                headers.ProxyAuthenticate.ElementAt(0), "ProxyAuthenticate[0]");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""),
                headers.ProxyAuthenticate.ElementAt(1), "ProxyAuthenticate[1]");

            headers.ProxyAuthenticate.Clear();
            Assert.AreEqual(0, headers.ProxyAuthenticate.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Proxy-Authenticate"),
                "There should be no Proxy-Authenticate header after calling Clear().");
        }

        [TestMethod]
        public void ProxyAuthenticate_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.Add("Proxy-Authenticate", "Negotiate");
            headers.AddWithoutValidation("Proxy-Authenticate", "Basic realm=\"alexandc-tst7\"");
            headers.AddWithoutValidation("Proxy-Authenticate", "NTLM");

            Assert.AreEqual(3, headers.ProxyAuthenticate.Count, "ProxyAuthenticate.Count");
            Assert.AreEqual(3, headers.GetValues("Proxy-Authenticate").Count(), "Proxy-Authenticate header value count.");

            Assert.AreEqual(new AuthenticationHeaderValue("Negotiate"),
                headers.ProxyAuthenticate.ElementAt(0), "ProxyAuthenticate[0]");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "realm=\"alexandc-tst7\""),
                headers.ProxyAuthenticate.ElementAt(1), "ProxyAuthenticate[1]");
            Assert.AreEqual(new AuthenticationHeaderValue("NTLM"),
                headers.ProxyAuthenticate.ElementAt(2), "ProxyAuthenticate[2]");

            headers.ProxyAuthenticate.Clear();
            Assert.AreEqual(0, headers.ProxyAuthenticate.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Proxy-Authenticate"),
                "There should be no Proxy-Authenticate header after calling Clear().");
        }

        [TestMethod]
        public void Server_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Server.Count, "Server expected to be empty on first call.");

            headers.Server.Add(new ProductInfoHeaderValue("(custom1)"));
            headers.Server.Add(new ProductInfoHeaderValue("custom2", "1.1"));

            Assert.AreEqual(2, headers.Server.Count, "Server.Count");
            Assert.AreEqual(2, headers.GetValues("Server").Count(), "Server header value count.");
            Assert.AreEqual(new ProductInfoHeaderValue("(custom1)"), headers.Server.ElementAt(0), "Server[0]");
            Assert.AreEqual(new ProductInfoHeaderValue("custom2", "1.1"), headers.Server.ElementAt(1), "Server[1]");

            headers.Server.Clear();
            Assert.AreEqual(0, headers.Server.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Server"), "Server header should be removed after calling Clear().");

            headers.Server.Add(new ProductInfoHeaderValue("(comment)"));
            headers.Server.Remove(new ProductInfoHeaderValue("(comment)"));
            Assert.AreEqual(0, headers.Server.Count, "Count after Remove().");
            Assert.IsFalse(headers.Contains("Server"), "Server header should be removed after removing last value.");
        }

        [TestMethod]
        public void Server_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Server", "CERN/3.0 libwww/2.17 (mycomment)");

            Assert.AreEqual(3, headers.Server.Count, "Server.Count");
            Assert.AreEqual(3, headers.GetValues("Server").Count(), "Server header value count.");

            Assert.AreEqual(new ProductInfoHeaderValue("CERN", "3.0"), headers.Server.ElementAt(0), "Server[0]");
            Assert.AreEqual(new ProductInfoHeaderValue("libwww", "2.17"), headers.Server.ElementAt(1), "Server[1]");
            Assert.AreEqual(new ProductInfoHeaderValue("(mycomment)"), headers.Server.ElementAt(2), "Server[2]");

            headers.Server.Clear();
            Assert.AreEqual(0, headers.Server.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Server"), "Server header should be removed after calling Clear().");
        }

        [TestMethod]
        public void Server_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Server", "custom会");
            Assert.IsNull(headers.GetParsedValues("Server"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Server").Count(), "Store value count");
            Assert.AreEqual("custom会", headers.GetValues("Server").First(), "Store value");

            headers.Clear();
            // Note that "Server" uses whitespaces as separators, so the following is an invalid value
            headers.AddWithoutValidation("Server", "custom1, custom2");
            Assert.IsNull(headers.GetParsedValues("Server"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Server").Count(), "Store value count");
            Assert.AreEqual("custom1, custom2", headers.GetValues("Server").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Server", "custom1, ");
            Assert.IsNull(headers.GetParsedValues("Server"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Server").Count(), "Store value count");
            Assert.AreEqual("custom1, ", headers.GetValues("Server").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Server", ",custom1");
            Assert.IsNull(headers.GetParsedValues("Server"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Server").Count(), "Store value count");
            Assert.AreEqual(",custom1", headers.GetValues("Server").First(), "Store value");
        }

        [TestMethod]
        public void RetryAfter_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.RetryAfter, "RetryAfter should be null by default.");

            RetryConditionHeaderValue retry = new RetryConditionHeaderValue(new TimeSpan(0, 1, 10));
            headers.RetryAfter = retry;
            Assert.AreSame(retry, headers.RetryAfter);

            headers.RetryAfter = null;
            Assert.IsNull(headers.RetryAfter, "RetryAfter should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("RetryAfter"),
                "Header store should not contain a header 'ETag' after setting it to null.");
        }

        [TestMethod]
        public void RetryAfter_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Retry-After", " 2100000 ");
            Assert.AreEqual(new RetryConditionHeaderValue(new TimeSpan(0, 0, 2100000)), headers.RetryAfter);
        }

        [TestMethod]
        public void RetryAfter_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Retry-After", "123,"); // trailing separator
            Assert.IsNull(headers.RetryAfter, "Retry-After");
            Assert.AreEqual(1, headers.GetValues("Retry-After").Count(), "Count");
            Assert.AreEqual("123,", headers.GetValues("Retry-After").First(), "Retry-After value");

            headers.Clear();
            headers.AddWithoutValidation("Retry-After", ",Sun, 06 Nov 1994 08:49:37 GMT"); // leading separator
            Assert.IsNull(headers.RetryAfter, "RetryAfter");
            Assert.AreEqual(1, headers.GetValues("Retry-After").Count(), "Retry-After.Count");
            Assert.AreEqual(",Sun, 06 Nov 1994 08:49:37 GMT", headers.GetValues("Retry-After").First(), 
                "Retry-After value");
        }

        [TestMethod]
        public void Vary_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Vary.Count, "Collection expected to be empty on first call.");

            headers.Vary.Add("custom1");
            headers.Vary.Add("custom2");

            Assert.AreEqual(2, headers.Vary.Count, "Vary.Count");
            Assert.AreEqual(2, headers.GetValues("Vary").Count(), "Vary header value count.");

            Assert.AreEqual("custom1", headers.Vary.ElementAt(0), "Vary[0]");
            Assert.AreEqual("custom2", headers.Vary.ElementAt(1), "Vary[1]");

            headers.Vary.Clear();
            Assert.AreEqual(0, headers.Vary.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Vary"),
                "There should be no Vary header after calling Clear().");
        }

        [TestMethod]
        public void Vary_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Vary", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.Vary.Count, "Vary.Count");
            Assert.AreEqual(3, headers.GetValues("Vary").Count(), "Vary header value count.");

            Assert.AreEqual("custom1", headers.Vary.ElementAt(0), "Vary[0]");
            Assert.AreEqual("custom2", headers.Vary.ElementAt(1), "Vary[1]");
            Assert.AreEqual("custom3", headers.Vary.ElementAt(2), "Vary[2]");

            headers.Vary.Clear();
            Assert.AreEqual(0, headers.Vary.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Vary"),
                "There should be no Vary header after calling Clear().");
        }

        [TestMethod]
        public void Vary_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Vary", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.Vary.Count, "Vary.Count");
            Assert.AreEqual(1, headers.GetValues("Vary").Count(), "Vary.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Vary").First(), "Vary value");
        }

        [TestMethod]
        public void Age_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.Age, "Host should be null by default.");

            TimeSpan expected = new TimeSpan(0, 1, 2);
            headers.Age = expected;
            Assert.AreEqual(expected, headers.Age);

            headers.Age = null;
            Assert.IsNull(headers.Age, "Age should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Age"),
                "Header store should not contain a header 'Age' after setting it to null.");

            // Make sure the header gets serialized correctly
            headers.Age = new TimeSpan(0, 1, 2);
            Assert.AreEqual("62", headers.GetValues("Age").First(), "Serialized header");
        }

        [TestMethod]
        public void Age_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Age", "  15  ");
            Assert.AreEqual(new TimeSpan(0, 0, 15), headers.Age);

            headers.Clear();
            headers.AddWithoutValidation("Age", "0");
            Assert.AreEqual(new TimeSpan(0, 0, 0), headers.Age);
        }

        [TestMethod]
        public void Age_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Age", "10,");
            Assert.IsNull(headers.GetParsedValues("Age"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Age").Count(), "Store value count");
            Assert.AreEqual("10,", headers.GetValues("Age").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Age", "1.1");
            Assert.IsNull(headers.GetParsedValues("Age"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Age").Count(), "Store value count");
            Assert.AreEqual("1.1", headers.GetValues("Age").First(), "Store value");
        }

        #endregion

        // General headers are tested in more detail in HttpRequestHeadersTest. This file only makes sure
        // HttpResponseHeaders correctly forwards calls to HttpGeneralHeaders.
        #region General headers

        [TestMethod]
        public void Connection_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Connection.Count, "Collection expected to be empty on first call.");
            Assert.IsNull(headers.ConnectionClose, "ConnectionClose should be null by default.");

            headers.Connection.Add("custom1");
            headers.ConnectionClose = true;

            // Connection collection has 1 values plus 'close'
            Assert.AreEqual(2, headers.Connection.Count, "Connection.Count");
            Assert.AreEqual(2, headers.GetValues("Connection").Count(), "Connection header value count.");
            Assert.IsTrue(headers.ConnectionClose == true, "ConnectionClose");

            headers.AddWithoutValidation("Connection", "custom2");
            Assert.AreEqual(3, headers.Connection.Count, "Connection.Count");
            Assert.AreEqual(3, headers.GetValues("Connection").Count(), "Connection header value count.");
        }

        [TestMethod]
        public void TransferEncoding_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.TransferEncoding.Count, "TransferEncoding expected to be empty on first call.");
            Assert.IsNull(headers.TransferEncodingChunked, "TransferEncodingChunked should be null by default.");

            headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom1"));
            headers.TransferEncodingChunked = true;

            // Connection collection has 1 value plus 'chunked'
            Assert.AreEqual(2, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.AreEqual(2, headers.GetValues("Transfer-Encoding").Count(), "Transfer-Encoding header value count.");
            Assert.AreEqual(headers.TransferEncodingChunked, true, "TransferEncodingChunked == true");

            // Note that 'chunked' is already in the collection, we add 'chunked' again here. Therefore the total 
            // number of headers is 4 (2x customm, 2x 'chunked').
            headers.AddWithoutValidation("Transfer-Encoding", " , custom2, chunked ,");
            Assert.AreEqual(4, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.AreEqual(4, headers.GetValues("Transfer-Encoding").Count(), "Transfer-Encoding header value count.");
        }
        
        [TestMethod]
        public void Upgrade_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Upgrade.Count, "Upgrade expected to be empty on first call.");

            headers.Upgrade.Add(new ProductHeaderValue("custom1"));

            Assert.AreEqual(1, headers.Upgrade.Count, "Upgrade.Count");
            Assert.AreEqual(1, headers.GetValues("Upgrade").Count(), "Upgrade header value count.");

            headers.AddWithoutValidation("Upgrade", " , custom1 / 1.0, ");
            Assert.AreEqual(2, headers.Upgrade.Count, "Upgrade.Count");
            Assert.AreEqual(2, headers.GetValues("Upgrade").Count(), "Upgrade header value count.");
        }

        [TestMethod]
        public void Date_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.Date, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.Date = expected;
            Assert.AreEqual(expected, headers.Date);

            headers.Clear();
            headers.AddWithoutValidation("Date", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.Date);
        }

        [TestMethod]
        public void Via_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Via.Count, "Collection expected to be empty on first call.");

            headers.Via.Add(new ViaHeaderValue("x11", "host"));

            Assert.AreEqual(1, headers.Via.Count, "Count");

            headers.AddWithoutValidation("Via", ", 1.1 host2");
            Assert.AreEqual(2, headers.Via.Count, "Count");
        }

        [TestMethod]
        public void Warning_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Warning.Count, "Collection expected to be empty on first call.");

            headers.Warning.Add(new WarningHeaderValue(199, "microsoft.com", "\"Miscellaneous warning\""));

            Assert.AreEqual(1, headers.Warning.Count, "Count");

            headers.AddWithoutValidation("Warning", "112 example.com \"Disconnected operation\"");
            Assert.AreEqual(2, headers.Warning.Count, "Count");
        }

        [TestMethod]
        public void CacheControl_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.IsNull(headers.CacheControl, "CacheControl should be null by default.");

            CacheControlHeaderValue value = new CacheControlHeaderValue();
            value.NoCache = true;
            headers.CacheControl = value;
            Assert.AreEqual(value, headers.CacheControl);

            headers.AddWithoutValidation("Cache-Control", "must-revalidate");
            value = new CacheControlHeaderValue();
            value.NoCache = true;
            value.MustRevalidate = true;
            Assert.AreEqual(value, headers.CacheControl);
        }

        [TestMethod]
        public void Trailer_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Trailer.Count, "Collection expected to be empty on first call.");

            headers.Trailer.Add("custom1");

            Assert.AreEqual(1, headers.Trailer.Count, "Trailer.Count");

            headers.AddWithoutValidation("Trailer", ",custom2, ,");
            Assert.AreEqual(2, headers.Trailer.Count, "Trailer.Count");
        }

        [TestMethod]
        public void Pragma_ReadAndWriteProperty_CallsForwardedToHttpGeneralHeaders()
        {
            Assert.AreEqual(0, headers.Pragma.Count, "Collection expected to be empty on first call.");

            headers.Pragma.Add(new NameValueHeaderValue("custom1", "value1"));

            Assert.AreEqual(1, headers.Pragma.Count, "Pragma.Count");

            headers.AddWithoutValidation("Pragma", "custom2");
            Assert.AreEqual(2, headers.Pragma.Count, "Pragma.Count");
        }

        #endregion

        [TestMethod]
        public void InvalidHeaders_AddContentAndRequestHeaders_Throw()
        {
            // Try adding request and content headers. Use different casing to make sure case-insensitive comparison
            // is used.
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept", "v"), "Accept");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept-Charset", "v"), "Accept-Charset");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept-Encoding", "v"), "Accept-Encoding");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept-Language", "v"), "Accept-Language");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Authorization", "v"), "Authorization");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Expect", "v"), "Expect");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.AddWithoutValidation("From", "v"), "From");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Host", "v"), "Host");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("If-Match", "v"), "If-Match");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("If-Modified-Since", "v"), "If-Modified-Since");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("If-None-Match", "v"), "If-None-Match");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("If-Range", "v"), "If-Range");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("If-Unmodified-Since", "v"), "If-Unmodified-Since");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Max-Forwards", "v"), "Max-Forwards");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Proxy-Authorization", "v"), "Proxy-Authorization");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Range", "v"), "Range");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Referer", "v"), "Referer");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("TE", "v"), "TE");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("User-Agent", "v"), "User-Agent");

            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Allow", "v"), "Allow");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Content-Encoding", "v"), "Content-Encoding");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Content-Language", "v"), "Content-Language");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("content-length", "v"), "content-length");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Content-Location", "v"), "Content-Location");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Content-MD5", "v"), "Content-MD5");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Content-Range", "v"), "Content-Range");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("CONTENT-TYPE", "v"), "CONTENT-TYPE");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Expires", "v"), "Expires");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Last-Modified", "v"), "Last-Modified");
        }
    }
}
