using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Net.Test.Common;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class HttpContentHeadersTest
    {
        private HttpContentHeaders headers;

        [TestInitialize]
        public void TestInitialize()
        {
            headers = new HttpContentHeaders(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ContentLength_AddInvalidValueUsingUnusualCasing_ParserRetrievedUsingCaseInsensitiveComparison()
        {
            headers = new HttpContentHeaders(() => { return 15; });

            // Use uppercase header name to make sure the parser gets retrieved using case-insensitive comparison.
            headers.Add("CoNtEnT-LeNgTh", "this is invalid");
        }

        [TestMethod]
        public void ContentLength_ReadValue_DelegateInvoked()
        {
            headers = new HttpContentHeaders(() => { return 15; });

            // The delegate is invoked to return the length.
            Assert.AreEqual(15, headers.ContentLength);
            Assert.AreEqual((long)15, headers.GetParsedValues(HttpKnownHeaderNames.ContentLength), "GetParsedValues()");

            // After getting the calculated content length, set it to null.
            headers.ContentLength = null;
            Assert.AreEqual(null, headers.ContentLength, "ContentLength property.");
            Assert.IsFalse(headers.Contains(HttpKnownHeaderNames.ContentLength),
                "Store should not contain 'Content-Length' header.");

            headers.ContentLength = 27;
            Assert.AreEqual((long)27, headers.ContentLength, "ContentLength property.");
            Assert.AreEqual((long)27, headers.GetParsedValues(HttpKnownHeaderNames.ContentLength), "GetParsedValues()");
        }

        [TestMethod]
        public void ContentLength_SetCustomValue_DelegateNotInvoked()
        {
            headers = new HttpContentHeaders(() => { Assert.Fail("Delegate called.");  return 0; });

            headers.ContentLength = 27;
            Assert.AreEqual((long)27, headers.ContentLength, "ContentLength property.");
            Assert.AreEqual((long)27, headers.GetParsedValues(HttpKnownHeaderNames.ContentLength), "GetParsedValues()");

            // After explicitly setting the content length, set it to null.
            headers.ContentLength = null;
            Assert.AreEqual(null, headers.ContentLength, "ContentLength property.");
            Assert.IsFalse(headers.Contains(HttpKnownHeaderNames.ContentLength),
                "Store should not contain 'Content-Length' header.");

            // Make sure the header gets serialized correctly
            headers.ContentLength = 12345;
            Assert.AreEqual("12345", headers.GetValues("Content-Length").First(), "Serialized header");
        }

        [TestMethod]
        public void ContentLength_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers = new HttpContentHeaders(() => { Assert.Fail("Delegate called."); return 0; });
            headers.AddWithoutValidation(HttpKnownHeaderNames.ContentLength, " 68 \r\n ");

            Assert.AreEqual(68, headers.ContentLength, "ContentLength property.");
        }

        [TestMethod]
        public void ContentType_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            MediaTypeHeaderValue value = new MediaTypeHeaderValue("text/plain");
            value.CharSet = "utf-8";
            value.Parameters.Add(new NameValueHeaderValue("custom", "value"));

            Assert.IsNull(headers.ContentType, "Uninitialized ContentType");

            headers.ContentType = value;
            Assert.AreSame(value, headers.ContentType, "Initialized ContentType");

            headers.ContentType = null;
            Assert.IsNull(headers.ContentType, "ContentType after setting it to null.");
        }

        [TestMethod]
        public void ContentType_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-Type", "text/plain; charset=utf-8; custom=value");

            MediaTypeHeaderValue value = new MediaTypeHeaderValue("text/plain");
            value.CharSet = "utf-8";
            value.Parameters.Add(new NameValueHeaderValue("custom", "value"));

            Assert.AreEqual(value, headers.ContentType, "ContentType");
        }

        [TestMethod]
        public void ContentType_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Content-Type", "text/plain; charset=utf-8; custom=value, other/type");
            Assert.IsNull(headers.ContentType, "ContentType");
            Assert.AreEqual(1, headers.GetValues("Content-Type").Count(), "Content-Type.Count");
            Assert.AreEqual("text/plain; charset=utf-8; custom=value, other/type",
                headers.GetValues("Content-Type").First(), "Content-Type value");

            headers.Clear();
            headers.AddWithoutValidation("Content-Type", ",text/plain"); // leading separator
            Assert.IsNull(headers.ContentType, "ContentType");
            Assert.AreEqual(1, headers.GetValues("Content-Type").Count(), "Content-Type.Count");
            Assert.AreEqual(",text/plain", headers.GetValues("Content-Type").First(), "Content-Type value");

            headers.Clear();
            headers.AddWithoutValidation("Content-Type", "text/plain,"); // trailing separator
            Assert.IsNull(headers.ContentType, "ContentType");
            Assert.AreEqual(1, headers.GetValues("Content-Type").Count(), "Content-Type.Count");
            Assert.AreEqual("text/plain,", headers.GetValues("Content-Type").First(), "Content-Type value");
        }

        [TestMethod]
        public void ContentRange_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.ContentRange, "Uninitialized ContentRange");
            ContentRangeHeaderValue value = new ContentRangeHeaderValue(1, 2, 3);

            headers.ContentRange = value;
            Assert.AreEqual(value, headers.ContentRange, "Initialized ContentRange");

            headers.ContentRange = null;
            Assert.IsNull(headers.ContentRange, "ContenRange after setting it to null.");
        }

        [TestMethod]
        public void ContentRange_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-Range", "custom 1-2/*");

            ContentRangeHeaderValue value = new ContentRangeHeaderValue(1, 2);
            value.Unit = "custom";

            Assert.AreEqual(value, headers.ContentRange, "Initialized ContentRange");
        }

        [TestMethod]
        public void ContentLocation_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.ContentLocation, "Host should be null by default.");

            Uri expected = new Uri("http://example.com/path/");
            headers.ContentLocation = expected;
            Assert.AreEqual(expected, headers.ContentLocation);

            headers.ContentLocation = null;
            Assert.IsNull(headers.ContentLocation, "ContentLocation should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Content-Location"),
                "Header store should not contain a header 'ContentLocation' after setting it to null.");
        }

        [TestMethod]
        public void ContentLocation_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-Location", "  http://www.example.com/path/?q=v  ");
            Assert.AreEqual(new Uri("http://www.example.com/path/?q=v"), headers.ContentLocation);

            headers.Clear();
            headers.AddWithoutValidation("Content-Location", "/relative/uri/");
            Assert.AreEqual(new Uri("/relative/uri/", UriKind.Relative), headers.ContentLocation);
        }

        [TestMethod]
        public void ContentLocation_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Content-Location", " http://example.com http://other");
            Assert.IsNull(headers.GetParsedValues("Content-Location"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Content-Location").Count(), "Store value count");
            Assert.AreEqual(" http://example.com http://other", headers.GetValues("Content-Location").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Content-Location", "http://host /other");
            Assert.IsNull(headers.GetParsedValues("Content-Location"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Content-Location").Count(), "Store value count");
            Assert.AreEqual("http://host /other", headers.GetValues("Content-Location").First(), "Store value");
        }

        [TestMethod]
        public void ContentEncoding_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.ContentEncoding.Count, "Collection expected to be empty on first call.");

            headers.ContentEncoding.Add("custom1");
            headers.ContentEncoding.Add("custom2");

            Assert.AreEqual(2, headers.ContentEncoding.Count, "ContentEncoding.Count");
            Assert.AreEqual(2, headers.GetValues("Content-Encoding").Count(), "Content-Encoding header value count.");

            Assert.AreEqual("custom1", headers.ContentEncoding.ElementAt(0), "ContentEncoding[0]");
            Assert.AreEqual("custom2", headers.ContentEncoding.ElementAt(1), "ContentEncoding[1]");

            headers.ContentEncoding.Clear();
            Assert.AreEqual(0, headers.ContentEncoding.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Content-Encoding"),
                "There should be no Content-Encoding header after calling Clear().");
        }

        [TestMethod]
        public void ContentEncoding_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-Encoding", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.ContentEncoding.Count, "ContentEncoding.Count");
            Assert.AreEqual(3, headers.GetValues("Content-Encoding").Count(), "Content-Encoding header value count.");

            Assert.AreEqual("custom1", headers.ContentEncoding.ElementAt(0), "ContentEncoding[0]");
            Assert.AreEqual("custom2", headers.ContentEncoding.ElementAt(1), "ContentEncoding[1]");
            Assert.AreEqual("custom3", headers.ContentEncoding.ElementAt(2), "ContentEncoding[2]");

            headers.ContentEncoding.Clear();
            Assert.AreEqual(0, headers.ContentEncoding.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Content-Encoding"),
                "There should be no Content-Encoding header after calling Clear().");
        }

        [TestMethod]
        public void ContentEncoding_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Content-Encoding", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.ContentEncoding.Count, "ContentEncoding.Count");
            Assert.AreEqual(1, headers.GetValues("Content-Encoding").Count(), "Content-Encoding.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Content-Encoding").First(), "Content-Encoding value");
        }

        [TestMethod]
        public void ContentLanguage_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.ContentLanguage.Count, "Collection expected to be empty on first call.");

            // Note that Content-Language for us is just a list of tokens. We don't verify if the format is a valid
            // language tag. Users will pass the language tag to other classes like Encoding.GetEncoding() to retrieve
            // an encoding. These classes will do not only syntax checking but also verify if the language tag exists.
            headers.ContentLanguage.Add("custom1");
            headers.ContentLanguage.Add("custom2");

            Assert.AreEqual(2, headers.ContentLanguage.Count, "ContentLanguage.Count");
            Assert.AreEqual(2, headers.GetValues("Content-Language").Count(), "Content-Language header value count.");

            Assert.AreEqual("custom1", headers.ContentLanguage.ElementAt(0), "ContentLanguage[0]");
            Assert.AreEqual("custom2", headers.ContentLanguage.ElementAt(1), "ContentLanguage[1]");

            headers.ContentLanguage.Clear();
            Assert.AreEqual(0, headers.ContentLanguage.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Content-Language"),
                "There should be no Content-Language header after calling Clear().");
        }

        [TestMethod]
        public void ContentLanguage_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-Language", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.ContentLanguage.Count, "ContentLanguage.Count");
            Assert.AreEqual(3, headers.GetValues("Content-Language").Count(), "Content-Language header value count.");

            Assert.AreEqual("custom1", headers.ContentLanguage.ElementAt(0), "ContentLanguage[0]");
            Assert.AreEqual("custom2", headers.ContentLanguage.ElementAt(1), "ContentLanguage[1]");
            Assert.AreEqual("custom3", headers.ContentLanguage.ElementAt(2), "ContentLanguage[2]");

            headers.ContentLanguage.Clear();
            Assert.AreEqual(0, headers.ContentLanguage.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Content-Language"),
                "There should be no Content-Language header after calling Clear().");
        }

        [TestMethod]
        public void ContentLanguage_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Content-Language", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.ContentLanguage.Count, "ContentLanguage.Count");
            Assert.AreEqual(1, headers.GetValues("Content-Language").Count(), "Content-Language.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Content-Language").First(), "Content-Language value");
        }

        [TestMethod]
        public void ContentMD5_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.ContentMD5, "Host should be null by default.");

            byte[] expected = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            headers.ContentMD5 = expected;
            Assert.AreEqual(expected, headers.ContentMD5); // must be the same object reference

            // Make sure the header gets serialized correctly
            Assert.AreEqual("AQIDBAUGBw==", headers.GetValues("Content-MD5").First(), "Serialized header");

            headers.ContentMD5 = null;
            Assert.IsNull(headers.ContentMD5, "ContentMD5 should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Content-MD5"),
                "Header store should not contain a header 'ContentMD5' after setting it to null.");
        }

        [TestMethod]
        public void ContentMD5_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Content-MD5", "  lvpAKQ==  ");            
            CollectionAssert.AreEqual(new byte[] { 150, 250, 64, 41 }, headers.ContentMD5);

            headers.Clear();
            headers.AddWithoutValidation("Content-MD5", "+dIkS/MnOP8=");
            CollectionAssert.AreEqual(new byte[] { 249, 210, 36, 75, 243, 39, 56, 255 }, headers.ContentMD5);
        }

        [TestMethod]
        public void ContentMD5_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Content-MD5", "AQ--");
            Assert.IsNull(headers.GetParsedValues("Content-MD5"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Content-MD5").Count(), "Store value count");
            Assert.AreEqual("AQ--", headers.GetValues("Content-MD5").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Content-MD5", "AQ==, CD");
            Assert.IsNull(headers.GetParsedValues("Content-MD5"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Content-MD5").Count(), "Store value count");
            Assert.AreEqual("AQ==, CD", headers.GetValues("Content-MD5").First(), "Store value");
        }

        [TestMethod]
        public void Allow_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Allow.Count, "Collection expected to be empty on first call.");

            headers.Allow.Add("custom1");
            headers.Allow.Add("custom2");

            Assert.AreEqual(2, headers.Allow.Count, "Allow.Count");
            Assert.AreEqual(2, headers.GetValues("Allow").Count(), "Allow header value count.");

            Assert.AreEqual("custom1", headers.Allow.ElementAt(0), "Allow[0]");
            Assert.AreEqual("custom2", headers.Allow.ElementAt(1), "Allow[1]");

            headers.Allow.Clear();
            Assert.AreEqual(0, headers.Allow.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Allow"),
                "There should be no Allow header after calling Clear().");
        }

        [TestMethod]
        public void Allow_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Allow", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.Allow.Count, "Allow.Count");
            Assert.AreEqual(3, headers.GetValues("Allow").Count(), "Allow header value count.");

            Assert.AreEqual("custom1", headers.Allow.ElementAt(0), "Allow[0]");
            Assert.AreEqual("custom2", headers.Allow.ElementAt(1), "Allow[1]");
            Assert.AreEqual("custom3", headers.Allow.ElementAt(2), "Allow[2]");

            headers.Allow.Clear();
            Assert.AreEqual(0, headers.Allow.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Allow"),
                "There should be no Allow header after calling Clear().");
        }

        [TestMethod]
        public void Allow_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Allow", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.Allow.Count, "Allow.Count");
            Assert.AreEqual(1, headers.GetValues("Allow").Count(), "Allow.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Allow").First(), "Allow value");
        }

        [TestMethod]
        public void Expires_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Expires, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.Expires = expected;
            Assert.AreEqual(expected, headers.Expires);

            headers.Expires = null;
            Assert.IsNull(headers.Expires, "Expires should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Expires"),
                "Header store should not contain a header 'Expires' after setting it to null.");
        }

        [TestMethod]
        public void Expires_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Expires", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.Expires);

            headers.Clear();
            headers.AddWithoutValidation("Expires", "Sun, 06 Nov 1994 08:49:37 GMT");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.Expires);
        }

        [TestMethod]
        public void Expires_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Expires", " Sun, 06 Nov 1994 08:49:37 GMT ,");
            Assert.IsNull(headers.GetParsedValues("Expires"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Expires").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov 1994 08:49:37 GMT ,", headers.GetValues("Expires").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Expires", " Sun, 06 Nov ");
            Assert.IsNull(headers.GetParsedValues("Expires"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Expires").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov ", headers.GetValues("Expires").First(), "Store value");
        }

        [TestMethod]
        public void LastModified_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.LastModified, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.LastModified = expected;
            Assert.AreEqual(expected, headers.LastModified);

            headers.LastModified = null;
            Assert.IsNull(headers.LastModified, "LastModified should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Last-Modified"),
                "Header store should not contain a header 'LastModified' after setting it to null.");
        }

        [TestMethod]
        public void LastModified_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Last-Modified", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.LastModified);

            headers.Clear();
            headers.AddWithoutValidation("Last-Modified", "Sun, 06 Nov 1994 08:49:37 GMT");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.LastModified);
        }

        [TestMethod]
        public void LastModified_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Last-Modified", " Sun, 06 Nov 1994 08:49:37 GMT ,");
            Assert.IsNull(headers.GetParsedValues("Last-Modified"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Last-Modified").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov 1994 08:49:37 GMT ,", headers.GetValues("Last-Modified").First(),
                "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Last-Modified", " Sun, 06 Nov ");
            Assert.IsNull(headers.GetParsedValues("Last-Modified"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Last-Modified").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov ", headers.GetValues("Last-Modified").First(), "Store value");
        }

        [TestMethod]
        public void InvalidHeaders_AddContentAndResponseHeaders_Throw()
        {
            // Try adding request, response, and general headers. Use different casing to make sure case-insensitive
            // comparison is used.
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept-Ranges", "v"), "Accept-Ranges");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.AddWithoutValidation("age", "v"), "age");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("ETag", "v"), "ETag");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Location", "v"), "Location");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Proxy-Authenticate", "v"), "Proxy-Authenticate");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Retry-After", "v"), "Retry-After");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Server", "v"), "Server");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Vary", "v"), "Vary");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("WWW-Authenticate", "v"), "WWW-Authenticate");

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

            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Cache-Control", "v"), "Cache-Control");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Connection", "v"), "Connection");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Date", "v"), "Date");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Pragma", "v"), "Pragma");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Trailer", "v"), "Trailer");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Transfer-Encoding", "v"), "Transfer-Encoding");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Upgrade", "v"), "Upgrade");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Via", "v"), "Via");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Warning", "v"), "Warning");
        }
    }
}
