using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using System.Net.Mail;
using System.Net.Test.Common.Logging;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class HttpRequestHeadersTest
    {
        private HttpRequestHeaders headers;

        [TestInitialize]
        public void TestInitialize()
        {
            headers = new HttpRequestHeaders();
        }

        #region Request headers

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Accept_AddInvalidValueUsingUnusualCasing_ParserRetrievedUsingCaseInsensitiveComparison()
        {
            // Use uppercase header name to make sure the parser gets retrieved using case-insensitive comparison.
            headers.Add("AcCePt", "this is invalid");
        }

        [TestMethod]
        public void Accept_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            MediaTypeWithQualityHeaderValue value1 = new MediaTypeWithQualityHeaderValue("text/plain");
            value1.CharSet = "utf-8";
            value1.Quality = 0.5;
            value1.Parameters.Add(new NameValueHeaderValue("custom", "value"));
            MediaTypeWithQualityHeaderValue value2 = new MediaTypeWithQualityHeaderValue("text/plain");
            value2.CharSet = "iso-8859-1";
            value2.Quality = 0.3868;

            Assert.AreEqual(0, headers.Accept.Count, "Collection expected to be empty on first call.");

            headers.Accept.Add(value1);
            headers.Accept.Add(value2);

            Assert.AreEqual(2, headers.Accept.Count, "Count");
            Assert.AreEqual(value1, headers.Accept.ElementAt(0), "Accept[0] expected to be value1.");
            Assert.AreEqual(value2, headers.Accept.ElementAt(1), "Accept[1] expected to be value2.");

            headers.Accept.Clear();
            Assert.AreEqual(0, headers.Accept.Count, "Count after Clear().");
        }
        
        [TestMethod]
        public void Accept_ReadEmptyProperty_EmptyCollection()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            Assert.AreEqual(0, request.Headers.Accept.Count);            
            // Copy to another list
            List<MediaTypeWithQualityHeaderValue> accepts = request.Headers.Accept.ToList();
            Assert.AreEqual(0, accepts.Count);
            accepts = new List<MediaTypeWithQualityHeaderValue>(request.Headers.Accept);
            Assert.AreEqual(0, accepts.Count);
        }

        [TestMethod]
        public void Accept_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Accept",
                ",, , ,,text/plain; charset=iso-8859-1; q=1.0,\r\n */xml; charset=utf-8; q=0.5,,,");

            MediaTypeWithQualityHeaderValue value1 = new MediaTypeWithQualityHeaderValue("text/plain");
            value1.CharSet = "iso-8859-1";
            value1.Quality = 1.0;

            MediaTypeWithQualityHeaderValue value2 = new MediaTypeWithQualityHeaderValue("*/xml");
            value2.CharSet = "utf-8";
            value2.Quality = 0.5;

            Assert.AreEqual(value1, headers.Accept.ElementAt(0), "Accept[0]");
            Assert.AreEqual(value2, headers.Accept.ElementAt(1), "Accept[1]");
        }

        [TestMethod]
        public void Accept_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            // Add a valid media-type with an invalid quality value
            headers.AddWithoutValidation("Accept", "text/plain; q=a"); // invalid quality
            Assert.IsNotNull(headers.Accept.First(), "Accept first value");
            Assert.IsNull(headers.Accept.First().Quality, "Accept.Quality"); // No quality value shown
            Assert.AreEqual("text/plain; q=a", headers.Accept.First().ToString(), "ToString()");
            
            headers.Clear();
            headers.AddWithoutValidation("Accept", "text/plain application/xml"); // no separator

            Assert.AreEqual(0, headers.Accept.Count, "Accept.Count");
            Assert.AreEqual(1, headers.GetValues("Accept").Count(), "Accept.Count");
            Assert.AreEqual("text/plain application/xml", headers.GetValues("Accept").First(), "Accept value");
        }

        [TestMethod]
        public void AcceptCharset_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.AcceptCharset.Count, "Collection expected to be empty on first call.");

            headers.AcceptCharset.Add(new StringWithQualityHeaderValue("iso-8859-5"));
            headers.AcceptCharset.Add(new StringWithQualityHeaderValue("unicode-1-1", 0.8));

            Assert.AreEqual(2, headers.AcceptCharset.Count, "Count");
            Assert.AreEqual(new StringWithQualityHeaderValue("iso-8859-5"), headers.AcceptCharset.ElementAt(0),
                "AcceptCharset[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("unicode-1-1", 0.8), headers.AcceptCharset.ElementAt(1),
                "AcceptCharset[1]");

            headers.AcceptCharset.Clear();
            Assert.AreEqual(0, headers.AcceptCharset.Count, "Count after Clear().");
        }

        [TestMethod]
        public void AcceptCharset_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Accept-Charset", ", ,,iso-8859-5 , \r\n utf-8 ; q=0.300 ,,,");

            Assert.AreEqual(new StringWithQualityHeaderValue("iso-8859-5"),
                headers.AcceptCharset.ElementAt(0), "AcceptCharset[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("utf-8", 0.3),
                headers.AcceptCharset.ElementAt(1), "AcceptCharset[1]");
        }

        [TestMethod]
        public void AcceptCharset_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Accept-Charset", "iso-8859-5 utf-8"); // no separator
            Assert.AreEqual(0, headers.AcceptCharset.Count, "Accept.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Charset").Count(), "Accept-Charset.Count");
            Assert.AreEqual("iso-8859-5 utf-8", headers.GetValues("Accept-Charset").First(), "Accept-Charset value");

            headers.Clear();
            headers.AddWithoutValidation("Accept-Charset", "utf-8; q=1; q=0.3");
            Assert.AreEqual(0, headers.AcceptCharset.Count, "AcceptCharset.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Charset").Count(), "Accept-Charset.Count");
            Assert.AreEqual("utf-8; q=1; q=0.3", headers.GetValues("Accept-Charset").First(), "Accept-Charset value");
        }

        [TestMethod]
        public void AcceptCharset_AddMultipleValuesAndGetValueString_AllValuesAddedUsingTheCorrectDelimiter()
        {
            headers.AddWithoutValidation("Accept-Charset", "invalid value");
            headers.Add("Accept-Charset", "utf-8");
            headers.AcceptCharset.Add(new StringWithQualityHeaderValue("iso-8859-5", 0.5));

            foreach (var header in headers.GetHeaderStrings())
            {
                Assert.AreEqual("Accept-Charset", header.Key);
                Assert.AreEqual("utf-8, iso-8859-5; q=0.5, invalid value", header.Value);
            }
        }

        [TestMethod]
        public void AcceptEncoding_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.AcceptEncoding.Count, "Collection expected to be empty on first call.");

            headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("compress", 0.9));
            headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            Assert.AreEqual(2, headers.AcceptEncoding.Count, "Count");
            Assert.AreEqual(new StringWithQualityHeaderValue("compress", 0.9), headers.AcceptEncoding.ElementAt(0),
                "AcceptEncoding[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("gzip"), headers.AcceptEncoding.ElementAt(1),
                "AcceptEncoding[1]");

            headers.AcceptEncoding.Clear();
            Assert.AreEqual(0, headers.AcceptEncoding.Count, "Count after Clear().");
        }

        [TestMethod]
        public void AcceptEncoding_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Accept-Encoding", ", gzip; q=1.0, identity; q=0.5, *;q=0, ");

            Assert.AreEqual(new StringWithQualityHeaderValue("gzip", 1),
                headers.AcceptEncoding.ElementAt(0), "AcceptEncoding[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("identity", 0.5),
                headers.AcceptEncoding.ElementAt(1), "AcceptEncoding[1]");
            Assert.AreEqual(new StringWithQualityHeaderValue("*", 0),
                headers.AcceptEncoding.ElementAt(2), "AcceptEncoding[2]");

            headers.AcceptEncoding.Clear();
            headers.AddWithoutValidation("Accept-Encoding", "");
            Assert.AreEqual(0, headers.AcceptEncoding.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Accept-Encoding"));
        }

        [TestMethod]
        public void AcceptEncoding_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Accept-Encoding", "gzip deflate"); // no separator
            Assert.AreEqual(0, headers.AcceptEncoding.Count, "AcceptEncoding.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Encoding").Count(), "Accept-Encoding.Count");
            Assert.AreEqual("gzip deflate", headers.GetValues("Accept-Encoding").First(), "Accept-Encoding value");

            headers.Clear();
            headers.AddWithoutValidation("Accept-Encoding", "compress; q=1; gzip");
            Assert.AreEqual(0, headers.AcceptEncoding.Count, "AcceptEncoding.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Encoding").Count(), "Accept-Encoding.Count");
            Assert.AreEqual("compress; q=1; gzip", headers.GetValues("Accept-Encoding").First(), "Accept-Encoding value");
        }

        [TestMethod]
        public void AcceptLanguage_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.AcceptLanguage.Count, "Collection expected to be empty on first call.");

            headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("da"));
            headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-GB", 0.8));
            headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.7));

            Assert.AreEqual(3, headers.AcceptLanguage.Count, "Count");
            Assert.AreEqual(new StringWithQualityHeaderValue("da"), headers.AcceptLanguage.ElementAt(0),
                "AcceptLanguage[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("en-GB", 0.8), headers.AcceptLanguage.ElementAt(1),
                "AcceptLanguage[1]");
            Assert.AreEqual(new StringWithQualityHeaderValue("en", 0.7), headers.AcceptLanguage.ElementAt(2),
                "AcceptLanguage[2]");

            headers.AcceptLanguage.Clear();
            Assert.AreEqual(0, headers.AcceptLanguage.Count, "Count after Clear().");
        }

        [TestMethod]
        public void AcceptLanguage_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Accept-Language", " , de-DE;q=0.9,de-AT;q=0.5,*;q=0.010 , ");

            Assert.AreEqual(new StringWithQualityHeaderValue("de-DE", 0.9),
                headers.AcceptLanguage.ElementAt(0), "AcceptLanguage[0]");
            Assert.AreEqual(new StringWithQualityHeaderValue("de-AT", 0.5),
                headers.AcceptLanguage.ElementAt(1), "AcceptLanguage[1]");
            Assert.AreEqual(new StringWithQualityHeaderValue("*", 0.01),
                headers.AcceptLanguage.ElementAt(2), "AcceptLanguage[2]");

            headers.AcceptLanguage.Clear();
            headers.AddWithoutValidation("Accept-Language", "");
            Assert.AreEqual(0, headers.AcceptLanguage.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Accept-Language"));
        }

        [TestMethod]
        public void AcceptLanguage_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Accept-Language", "de -DE"); // no separator
            Assert.AreEqual(0, headers.AcceptLanguage.Count, "AcceptLanguage.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Language").Count(), "Accept-Language.Count");
            Assert.AreEqual("de -DE", headers.GetValues("Accept-Language").First(), "Accept-Language value");

            headers.Clear();
            headers.AddWithoutValidation("Accept-Language", "en; q=0.4,[");
            Assert.AreEqual(0, headers.AcceptLanguage.Count, "AcceptLanguage.Count");
            Assert.AreEqual(1, headers.GetValues("Accept-Language").Count(), "Accept-Language.Count");
            Assert.AreEqual("en; q=0.4,[", headers.GetValues("Accept-Language").First(), "Accept-Language value");
        }

        [TestMethod]
        public void Expect_Add100Continue_Success()
        {
            // use non-default casing to make sure we do case-insensitive comparison.
            headers.Expect.Add(new NameValueWithParametersHeaderValue("100-CONTINUE"));
            Assert.IsTrue(headers.ExpectContinue == true);
            Assert.AreEqual(1, headers.Expect.Count);
        }

        [TestMethod]
        public void Expect_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Expect.Count, "Expect expected to be empty on first call.");
            Assert.IsNull(headers.ExpectContinue, "ExpectContinue should be null by default.");

            headers.Expect.Add(new NameValueWithParametersHeaderValue("custom1"));
            headers.Expect.Add(new NameValueWithParametersHeaderValue("custom2"));
            headers.ExpectContinue = true;

            // Connection collection has 2 values plus '100-Continue'
            Assert.AreEqual(3, headers.Expect.Count, "Expect.Count");
            Assert.AreEqual(3, headers.GetValues("Expect").Count(), "Expect header value count.");
            Assert.IsTrue(headers.ExpectContinue == true, "ExpectContinue == true");
            Assert.AreEqual(new NameValueWithParametersHeaderValue("custom1"), headers.Expect.ElementAt(0),
                "Expect[0]");
            Assert.AreEqual(new NameValueWithParametersHeaderValue("custom2"), headers.Expect.ElementAt(1),
                "Expect[1]");

            // Remove '100-continue' value from store. But leave other 'Expect' values.
            headers.ExpectContinue = false;
            Assert.IsTrue(headers.ExpectContinue == false, "ExpectContinue == false");
            Assert.AreEqual(2, headers.Expect.Count, "Expect.Count");
            Assert.AreEqual(new NameValueWithParametersHeaderValue("custom1"), headers.Expect.ElementAt(0),
                "Expect[0]");
            Assert.AreEqual(new NameValueWithParametersHeaderValue("custom2"), headers.Expect.ElementAt(1),
                "Expect[1]");

            headers.ExpectContinue = true;
            headers.Expect.Clear();
            Assert.IsTrue(headers.ExpectContinue == false, "ExpectContinue should be modified by Expect.Clear().");
            Assert.AreEqual(0, headers.Expect.Count, "Count after Clear().");
            IEnumerable<string> dummyArray;
            Assert.IsFalse(headers.TryGetValues("Expect", out dummyArray), "Expect header count after Expect.Clear().");

            // Remove '100-continue' value from store. Since there are no other 'Expect' values, remove whole header.
            headers.ExpectContinue = false;
            Assert.IsTrue(headers.ExpectContinue == false, "ExpectContinue == false");
            Assert.AreEqual(0, headers.Expect.Count, "Expect.Count");
            Assert.IsFalse(headers.Contains("Expect"));

            headers.ExpectContinue = null;
            Assert.IsNull(headers.ExpectContinue, "ExpectContinue");
        }

        [TestMethod]
        public void Expect_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Expect",
                ", , 100-continue, name1 = value1, name2; param2=paramValue2, name3=value3; param3 ,");

            // Connection collection has 3 values plus '100-continue'
            Assert.AreEqual(4, headers.Expect.Count, "Expect.Count");
            Assert.AreEqual(4, headers.GetValues("Expect").Count(), "Expect header value count.");
            Assert.IsTrue(headers.ExpectContinue == true, "ExpectContinue expected to be true.");

            Assert.AreEqual(new NameValueWithParametersHeaderValue("100-continue"),
                headers.Expect.ElementAt(0), "Expect[0]");

            Assert.AreEqual(new NameValueWithParametersHeaderValue("name1", "value1"),
                headers.Expect.ElementAt(1), "Expect[1]");

            NameValueWithParametersHeaderValue expected2 = new NameValueWithParametersHeaderValue("name2");
            expected2.Parameters.Add(new NameValueHeaderValue("param2", "paramValue2"));
            Assert.AreEqual(expected2, headers.Expect.ElementAt(2), "Expect[2]");

            NameValueWithParametersHeaderValue expected3 = new NameValueWithParametersHeaderValue("name3", "value3");
            expected3.Parameters.Add(new NameValueHeaderValue("param3"));
            Assert.AreEqual(expected3, headers.Expect.ElementAt(3), "Expect[3]");

            headers.Expect.Clear();
            Assert.IsNull(headers.ExpectContinue, "ExpectContinue should be modified by Expect.Clear().");
            Assert.AreEqual(0, headers.Expect.Count, "Count after Clear().");
            IEnumerable<string> dummyArray;
            Assert.IsFalse(headers.TryGetValues("Expect", out dummyArray), "Expect header count after Expect.Clear().");
        }

        [TestMethod]
        public void Expect_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Expect", "100-continue other"); // no separator

            Assert.AreEqual(0, headers.Expect.Count, "Expect.Count");
            Assert.AreEqual(1, headers.GetValues("Expect").Count(), "Expect.Count");
            Assert.AreEqual("100-continue other", headers.GetValues("Expect").First(), "Expect value");
        }

        [TestMethod]
        public void Host_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Host, "Host should be null by default.");

            headers.Host = "host";
            Assert.AreEqual("host", headers.Host);

            headers.Host = null;
            Assert.IsNull(headers.Host, "Host should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Host"),
                "Header store should not contain a header 'Host' after setting it to null.");

            ExceptionAssert.ThrowsFormat(() => headers.Host = "invalid host", "Setting invalid Host value should throw.");
        }

        [TestMethod]
        public void Host_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Host", "host:80");

            Assert.AreEqual("host:80", headers.Host);
        }

        [TestMethod]
        public void IfMatch_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.IfMatch.Count, "IfMatch expected to be empty on first call.");

            headers.IfMatch.Add(new EntityTagHeaderValue("\"custom1\""));
            headers.IfMatch.Add(new EntityTagHeaderValue("\"custom2\"", true));

            Assert.AreEqual(2, headers.IfMatch.Count, "IfMatch.Count");
            Assert.AreEqual(2, headers.GetValues("If-Match").Count(), "If-Match header value count.");
            Assert.AreEqual(new EntityTagHeaderValue("\"custom1\""), headers.IfMatch.ElementAt(0),
                "IfMatch[0]");
            Assert.AreEqual(new EntityTagHeaderValue("\"custom2\"", true), headers.IfMatch.ElementAt(1),
                "IfMatch[1]");

            headers.IfMatch.Clear();
            Assert.AreEqual(0, headers.IfMatch.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("If-Match"), "Header store should not contain 'If-Match'");
        }

        [TestMethod]
        public void IfMatch_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("If-Match", ", , W/\"tag1\", \"tag2\", W/\"tag3\" ,");

            Assert.AreEqual(3, headers.IfMatch.Count, "IfMatch.Count");
            Assert.AreEqual(3, headers.GetValues("If-Match").Count(), "If-Match header value count.");

            Assert.AreEqual(new EntityTagHeaderValue("\"tag1\"", true), headers.IfMatch.ElementAt(0),
                "IfMatch[0]");
            Assert.AreEqual(new EntityTagHeaderValue("\"tag2\"", false), headers.IfMatch.ElementAt(1),
                "IfMatch[1]");
            Assert.AreEqual(new EntityTagHeaderValue("\"tag3\"", true), headers.IfMatch.ElementAt(2),
                "IfMatch[2]");

            headers.IfMatch.Clear();
            headers.Add("If-Match", "*");
            Assert.AreEqual(1, headers.IfMatch.Count, "Count after Clear().");
            Assert.AreSame(EntityTagHeaderValue.Any, headers.IfMatch.ElementAt(0), "IfMatch[0]");
        }

        [TestMethod]
        public void IfMatch_UseAddMethodWithInvalidInput_PropertyNotUpdated()
        {

            headers.AddWithoutValidation("If-Match", "W/\"tag1\" \"tag2\""); // no separator
            Assert.AreEqual(0, headers.IfMatch.Count, "IfMatch.Count");
            Assert.AreEqual(1, headers.GetValues("If-Match").Count(), "If-Match header value count.");
        }

        [TestMethod]
        public void IfNoneMatch_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.IfNoneMatch.Count, "IfNoneMatch expected to be empty on first call.");

            headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"custom1\""));
            headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"custom2\"", true));

            Assert.AreEqual(2, headers.IfNoneMatch.Count, "IfNoneMatch.Count");
            Assert.AreEqual(2, headers.GetValues("If-None-Match").Count(), "If-None-Match header value count.");
            Assert.AreEqual(new EntityTagHeaderValue("\"custom1\""), headers.IfNoneMatch.ElementAt(0),
                "IfNoneMatch[0]");
            Assert.AreEqual(new EntityTagHeaderValue("\"custom2\"", true), headers.IfNoneMatch.ElementAt(1),
                "IfNoneMatch[1]");

            headers.IfNoneMatch.Clear();
            Assert.AreEqual(0, headers.IfNoneMatch.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("If-None-Match"), "Header store should not contain 'If-None-Match'");
        }

        [TestMethod]
        public void IfNoneMatch_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("If-None-Match", "W/\"tag1\", \"tag2\", W/\"tag3\"");

            Assert.AreEqual(3, headers.IfNoneMatch.Count, "IfNoneMatch.Count");
            Assert.AreEqual(3, headers.GetValues("If-None-Match").Count(), "If-None-Match header value count.");

            Assert.AreEqual(new EntityTagHeaderValue("\"tag1\"", true), headers.IfNoneMatch.ElementAt(0),
                "IfNoneMatch[0]");
            Assert.AreEqual(new EntityTagHeaderValue("\"tag2\"", false), headers.IfNoneMatch.ElementAt(1),
                "IfNoneMatch[1]");
            Assert.AreEqual(new EntityTagHeaderValue("\"tag3\"", true), headers.IfNoneMatch.ElementAt(2),
                "IfNoneMatch[2]");

            headers.IfNoneMatch.Clear();
            headers.Add("If-None-Match", "*");
            Assert.AreEqual(1, headers.IfNoneMatch.Count, "Count after Clear().");
            Assert.AreSame(EntityTagHeaderValue.Any, headers.IfNoneMatch.ElementAt(0), "IfNoneMatch[0]");
        }

        [TestMethod]
        public void TE_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            TransferCodingWithQualityHeaderValue value1 = new TransferCodingWithQualityHeaderValue("custom");
            value1.Quality = 0.5;
            value1.Parameters.Add(new NameValueHeaderValue("name", "value"));
            TransferCodingWithQualityHeaderValue value2 = new TransferCodingWithQualityHeaderValue("custom");
            value2.Quality = 0.3868;

            Assert.AreEqual(0, headers.TE.Count, "Collection expected to be empty on first call.");

            headers.TE.Add(value1);
            headers.TE.Add(value2);

            Assert.AreEqual(2, headers.TE.Count, "Count");
            Assert.AreEqual(value1, headers.TE.ElementAt(0), "TE[0] expected to be value1.");
            Assert.AreEqual(value2, headers.TE.ElementAt(1), "TE[1] expected to be value2.");

            headers.TE.Clear();
            Assert.AreEqual(0, headers.TE.Count, "Count after Clear().");
        }

        [TestMethod]
        public void TE_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("TE",
                ",custom1; param1=value1; q=1.0,,\r\n custom2; param2=value2; q=0.5  ,");

            TransferCodingWithQualityHeaderValue value1 = new TransferCodingWithQualityHeaderValue("custom1");
            value1.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            value1.Quality = 1.0;

            TransferCodingWithQualityHeaderValue value2 = new TransferCodingWithQualityHeaderValue("custom2");
            value2.Parameters.Add(new NameValueHeaderValue("param2", "value2"));
            value2.Quality = 0.5;

            Assert.AreEqual(value1, headers.TE.ElementAt(0), "TE[0]");
            Assert.AreEqual(value2, headers.TE.ElementAt(1), "TE[1]");

            headers.Clear();
            headers.AddWithoutValidation("TE", "");
            Assert.IsFalse(headers.Contains("TE"), "'TE' header should not be added if it just has empty values.");
        }

        [TestMethod]
        public void Range_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Range, "Uninitialized Range");
            RangeHeaderValue value = new RangeHeaderValue(1, 2);

            headers.Range = value;
            Assert.AreEqual(value, headers.Range, "Initialized Range");

            headers.Range = null;
            Assert.IsNull(headers.Range, "Range after setting it to null.");
        }

        [TestMethod]
        public void Range_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Range", "custom= , ,1-2, -4 , ");

            RangeHeaderValue value = new RangeHeaderValue();
            value.Unit = "custom";
            value.Ranges.Add(new RangeItemHeaderValue(1, 2));
            value.Ranges.Add(new RangeItemHeaderValue(null, 4));

            Assert.AreEqual(value, headers.Range, "Initialized ContentRange");
        }

        [TestMethod]
        public void Authorization_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Authorization, "Authorization should be null by default.");

            headers.Authorization = new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ=="), headers.Authorization);

            headers.Authorization = null;
            Assert.IsNull(headers.Authorization, "Authorization should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Authorization"),
                "Header store should not contain a header 'Authorization' after setting it to null.");
        }

        [TestMethod]
        public void Authorization_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Authorization", "NTLM blob");

            Assert.AreEqual(new AuthenticationHeaderValue("NTLM", "blob"), headers.Authorization);
        }

        [TestMethod]
        public void ProxyAuthorization_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.ProxyAuthorization, "ProxyAuthorization should be null by default.");

            headers.ProxyAuthorization = new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
            Assert.AreEqual(new AuthenticationHeaderValue("Basic", "QWxhZGRpbjpvcGVuIHNlc2FtZQ=="),
                headers.ProxyAuthorization);

            headers.ProxyAuthorization = null;
            Assert.IsNull(headers.ProxyAuthorization, "ProxyAuthorization should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("ProxyAuthorization"),
                "Header store should not contain a header 'ProxyAuthorization' after setting it to null.");
        }

        [TestMethod]
        public void ProxyAuthorization_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Proxy-Authorization", "NTLM blob");

            Assert.AreEqual(new AuthenticationHeaderValue("NTLM", "blob"), headers.ProxyAuthorization);
        }

        [TestMethod]
        public void UserAgent_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.UserAgent.Count, "UserAgent expected to be empty on first call.");

            headers.UserAgent.Add(new ProductInfoHeaderValue("(custom1)"));
            headers.UserAgent.Add(new ProductInfoHeaderValue("custom2", "1.1"));

            Assert.AreEqual(2, headers.UserAgent.Count, "UserAgent.Count");
            Assert.AreEqual(2, headers.GetValues("User-Agent").Count(), "User-Agent header value count.");
            Assert.AreEqual(new ProductInfoHeaderValue("(custom1)"), headers.UserAgent.ElementAt(0), "UserAgent[0]");
            Assert.AreEqual(new ProductInfoHeaderValue("custom2", "1.1"), headers.UserAgent.ElementAt(1), "UserAgent[1]");

            headers.UserAgent.Clear();
            Assert.AreEqual(0, headers.UserAgent.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("User-Agent"), "User-Agent header should be removed after calling Clear().");

            headers.UserAgent.Add(new ProductInfoHeaderValue("(comment)"));
            headers.UserAgent.Remove(new ProductInfoHeaderValue("(comment)"));
            Assert.AreEqual(0, headers.UserAgent.Count, "Count after Remove().");
            Assert.IsFalse(headers.Contains("User-Agent"), "User-Agent header should be removed after removing last value.");
        }

        [TestMethod]
        public void UserAgent_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("User-Agent", "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.6.30 Version/10.63");

            Assert.AreEqual(4, headers.UserAgent.Count, "UserAgent.Count");
            Assert.AreEqual(4, headers.GetValues("User-Agent").Count(), "User-Agent header value count.");

            Assert.AreEqual(new ProductInfoHeaderValue("Opera", "9.80"), headers.UserAgent.ElementAt(0), "UserAgent[0]");
            Assert.AreEqual(new ProductInfoHeaderValue("(Windows NT 6.1; U; en)"), headers.UserAgent.ElementAt(1), "UserAgent[1]");
            Assert.AreEqual(new ProductInfoHeaderValue("Presto", "2.6.30"), headers.UserAgent.ElementAt(2), "UserAgent[2]");
            Assert.AreEqual(new ProductInfoHeaderValue("Version", "10.63"), headers.UserAgent.ElementAt(3), "UserAgent[3]");

            headers.UserAgent.Clear();
            Assert.AreEqual(0, headers.UserAgent.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("User-Agent"), "User-Agent header should be removed after calling Clear().");
        }

        [TestMethod]
        public void UserAgent_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("User-Agent", "custom会");
            Assert.IsNull(headers.GetParsedValues("User-Agent"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("User-Agent").Count(), "Store value count");
            Assert.AreEqual("custom会", headers.GetValues("User-Agent").First(), "Store value");

            headers.Clear();
            // Note that "User-Agent" uses whitespaces as separators, so the following is an invalid value
            headers.AddWithoutValidation("User-Agent", "custom1, custom2"); 
            Assert.IsNull(headers.GetParsedValues("User-Agent"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("User-Agent").Count(), "Store value count");
            Assert.AreEqual("custom1, custom2", headers.GetValues("User-Agent").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("User-Agent", "custom1, ");
            Assert.IsNull(headers.GetParsedValues("User-Agent"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("User-Agent").Count(), "Store value count");
            Assert.AreEqual("custom1, ", headers.GetValues("User-Agent").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("User-Agent", ",custom1");
            Assert.IsNull(headers.GetParsedValues("User-Agent"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("User-Agent").Count(), "Store value count");
            Assert.AreEqual(",custom1", headers.GetValues("User-Agent").First(), "Store value");
        }

        [TestMethod]
        public void UserAgent_AddMultipleValuesAndGetValueString_AllValuesAddedUsingTheCorrectDelimiter()
        {
            headers.AddWithoutValidation("User-Agent", "custom会");
            headers.Add("User-Agent", "custom2/1.1");
            headers.UserAgent.Add(new ProductInfoHeaderValue("(comment)"));

            foreach (var header in headers.GetHeaderStrings())
            {
                Assert.AreEqual("User-Agent", header.Key);
                Assert.AreEqual("custom2/1.1 (comment) custom会", header.Value);
            }
        }

        [TestMethod]
        public void IfRange_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.IfRange, "IfRange expected to be null on first call.");

            headers.IfRange = new RangeConditionHeaderValue("\"x\"");

            Assert.AreEqual(1, headers.GetValues("If-Range").Count(), "If-Range header value count.");
            Assert.AreEqual(new RangeConditionHeaderValue("\"x\""), headers.IfRange, "IfRange value after setting it.");

            headers.IfRange = null;
            Assert.IsNull(headers.IfRange, "IfRange value after setting it to null.");
            Assert.IsFalse(headers.Contains("If-Range"), "If-Range header should be removed after calling Clear().");
        }

        [TestMethod]
        public void IfRange_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("If-Range", " W/\"tag\" ");

            Assert.AreEqual(new RangeConditionHeaderValue(new EntityTagHeaderValue("\"tag\"", true)),
                headers.IfRange, "IfRange");
            Assert.AreEqual(1, headers.GetValues("If-Range").Count(), "If-Range header value count.");

            headers.IfRange = null;
            Assert.IsNull(headers.IfRange, "IfRange value after setting it to null.");
            Assert.IsFalse(headers.Contains("If-Range"), "If-Range header should be removed after calling Clear().");
        }

        [TestMethod]
        public void IfRange_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("If-Range", "\"tag\"会");
            Assert.IsNull(headers.GetParsedValues("If-Range"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Range").Count(), "Store value count");
            Assert.AreEqual("\"tag\"会", headers.GetValues("If-Range").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("If-Range", " \"tag\", ");
            Assert.IsNull(headers.GetParsedValues("If-Range"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Range").Count(), "Store value count");
            Assert.AreEqual(" \"tag\", ", headers.GetValues("If-Range").First(), "Store value");
        }

        [TestMethod]
        public void From_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.From, "Host should be null by default.");

            headers.From = "info@example.com";
            Assert.AreEqual("info@example.com", headers.From);

            headers.From = null;
            Assert.IsNull(headers.From, "From should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("From"),
                "Header store should not contain a header 'From' after setting it to null.");

            ExceptionAssert.ThrowsFormat(() => headers.From = " ", "' '");
            ExceptionAssert.ThrowsFormat(() => headers.From = "invalid email address", "invalid email address");
        }

        [TestMethod]
        public void From_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("From", "  \"My Name\"   info@example.com  ");
            Assert.AreEqual("\"My Name\"   info@example.com  ", headers.From);

            // The following encoded string represents the character sequence "会员服务".
            headers.Clear();
            headers.AddWithoutValidation("From", "=?utf-8?Q?=E4=BC=9A=E5=91=98=E6=9C=8D=E5=8A=A1?= <info@example.com>");
            Assert.AreEqual("=?utf-8?Q?=E4=BC=9A=E5=91=98=E6=9C=8D=E5=8A=A1?= <info@example.com>", headers.From);
        }

        [TestMethod]
        public void From_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("From", " info@example.com ,");
            Assert.IsNull(headers.GetParsedValues("From"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("From").Count(), "Store value count");
            Assert.AreEqual(" info@example.com ,", headers.GetValues("From").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("From", "info@");
            Assert.IsNull(headers.GetParsedValues("From"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("From").Count(), "Store value count");
            Assert.AreEqual("info@", headers.GetValues("From").First(), "Store value");
        }

        [TestMethod]
        public void IfModifiedSince_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.IfModifiedSince, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.IfModifiedSince = expected;
            Assert.AreEqual(expected, headers.IfModifiedSince);

            headers.IfModifiedSince = null;
            Assert.IsNull(headers.IfModifiedSince, "IfModifiedSince should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("If-Modified-Since"),
                "Header store should not contain a header 'IfModifiedSince' after setting it to null.");
        }

        [TestMethod]
        public void IfModifiedSince_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("If-Modified-Since", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.IfModifiedSince);

            headers.Clear();
            headers.AddWithoutValidation("If-Modified-Since", "Sun, 06 Nov 1994 08:49:37 GMT");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.IfModifiedSince);
        }

        [TestMethod]
        public void IfModifiedSince_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("If-Modified-Since", " Sun, 06 Nov 1994 08:49:37 GMT ,");
            Assert.IsNull(headers.GetParsedValues("If-Modified-Since"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Modified-Since").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov 1994 08:49:37 GMT ,", headers.GetValues("If-Modified-Since").First(),
                "Store value");

            headers.Clear();
            headers.AddWithoutValidation("If-Modified-Since", " Sun, 06 Nov ");
            Assert.IsNull(headers.GetParsedValues("If-Modified-Since"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Modified-Since").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov ", headers.GetValues("If-Modified-Since").First(), "Store value");
        }

        [TestMethod]
        public void IfUnmodifiedSince_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.IfUnmodifiedSince, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.IfUnmodifiedSince = expected;
            Assert.AreEqual(expected, headers.IfUnmodifiedSince);

            headers.IfUnmodifiedSince = null;
            Assert.IsNull(headers.IfUnmodifiedSince, "IfUnmodifiedSince should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("If-Unmodified-Since"),
                "Header store should not contain a header 'IfUnmodifiedSince' after setting it to null.");
        }

        [TestMethod]
        public void IfUnmodifiedSince_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("If-Unmodified-Since", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.IfUnmodifiedSince);

            headers.Clear();
            headers.AddWithoutValidation("If-Unmodified-Since", "Sun, 06 Nov 1994 08:49:37 GMT");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.IfUnmodifiedSince);
        }

        [TestMethod]
        public void IfUnmodifiedSince_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("If-Unmodified-Since", " Sun, 06 Nov 1994 08:49:37 GMT ,");
            Assert.IsNull(headers.GetParsedValues("If-Unmodified-Since"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Unmodified-Since").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov 1994 08:49:37 GMT ,", headers.GetValues("If-Unmodified-Since").First(),
                "Store value");

            headers.Clear();
            headers.AddWithoutValidation("If-Unmodified-Since", " Sun, 06 Nov ");
            Assert.IsNull(headers.GetParsedValues("If-Unmodified-Since"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("If-Unmodified-Since").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov ", headers.GetValues("If-Unmodified-Since").First(), "Store value");
        }

        [TestMethod]
        public void Referrer_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Referrer, "Host should be null by default.");

            Uri expected = new Uri("http://example.com/path/");
            headers.Referrer = expected;
            Assert.AreEqual(expected, headers.Referrer);

            headers.Referrer = null;
            Assert.IsNull(headers.Referrer, "Referrer should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Referer"),
                "Header store should not contain a header 'Referrer' after setting it to null.");
        }

        [TestMethod]
        public void Referrer_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Referer", "  http://www.example.com/path/?q=v  ");
            Assert.AreEqual(new Uri("http://www.example.com/path/?q=v"), headers.Referrer);

            headers.Clear();
            headers.AddWithoutValidation("Referer", "/relative/uri/");
            Assert.AreEqual(new Uri("/relative/uri/", UriKind.Relative), headers.Referrer);
        }

        [TestMethod]
        public void Referrer_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Referer", " http://example.com http://other");
            Assert.IsNull(headers.GetParsedValues("Referer"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Referer").Count(), "Store value count");
            Assert.AreEqual(" http://example.com http://other", headers.GetValues("Referer").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Referer", "http://host /other");
            Assert.IsNull(headers.GetParsedValues("Referer"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Referer").Count(), "Store value count");
            Assert.AreEqual("http://host /other", headers.GetValues("Referer").First(), "Store value");
        }

        [TestMethod]
        public void MaxForwards_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.MaxForwards, "Host should be null by default.");

            headers.MaxForwards = 15;
            Assert.AreEqual(15, headers.MaxForwards);

            headers.MaxForwards = null;
            Assert.IsNull(headers.MaxForwards, "MaxForwards should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Max-Forwards"),
                "Header store should not contain a header 'MaxForwards' after setting it to null.");

            // Make sure the header gets serialized correctly
            headers.MaxForwards = 12345;
            Assert.AreEqual("12345", headers.GetValues("Max-Forwards").First(), "Serialized header");
        }

        [TestMethod]
        public void MaxForwards_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Max-Forwards", "  00123  ");
            Assert.AreEqual(123, headers.MaxForwards);

            headers.Clear();
            headers.AddWithoutValidation("Max-Forwards", "0");
            Assert.AreEqual(0, headers.MaxForwards);
        }

        [TestMethod]
        public void MaxForwards_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Max-Forwards", "15,");
            Assert.IsNull(headers.GetParsedValues("Max-Forwards"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Max-Forwards").Count(), "Store value count");
            Assert.AreEqual("15,", headers.GetValues("Max-Forwards").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Max-Forwards", "1.0");
            Assert.IsNull(headers.GetParsedValues("Max-Forwards"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Max-Forwards").Count(), "Store value count");
            Assert.AreEqual("1.0", headers.GetValues("Max-Forwards").First(), "Store value");
        }

        #endregion

        #region General headers

        [TestMethod]
        public void Connection_AddClose_Success()
        {
            headers.Connection.Add("CLOSE"); // use non-default casing to make sure we do case-insensitive comparison.
            Assert.IsTrue(headers.ConnectionClose == true);
            Assert.AreEqual(1, headers.Connection.Count);
        }

        [TestMethod]
        public void Connection_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Connection.Count, "Collection expected to be empty on first call.");
            Assert.IsNull(headers.ConnectionClose, "ConnectionClose should be null by default.");

            headers.Connection.Add("custom1");
            headers.Connection.Add("custom2");
            headers.ConnectionClose = true;

            // Connection collection has 2 values plus 'close'
            Assert.AreEqual(3, headers.Connection.Count, "Connection.Count");
            Assert.AreEqual(3, headers.GetValues("Connection").Count(), "Connection header value count.");
            Assert.IsTrue(headers.ConnectionClose == true, "ConnectionClose");

            Assert.AreEqual("custom1", headers.Connection.ElementAt(0), "Connection[0]");
            Assert.AreEqual("custom2", headers.Connection.ElementAt(1), "Connection[1]");

            // Remove 'close' value from store. But leave other 'Connection' values.
            headers.ConnectionClose = false;
            Assert.IsTrue(headers.ConnectionClose == false, "ConnectionClose == false");
            Assert.AreEqual(2, headers.Connection.Count, "Connection.Count");
            Assert.AreEqual("custom1", headers.Connection.ElementAt(0), "Connection[0]");
            Assert.AreEqual("custom2", headers.Connection.ElementAt(1), "Connection[1]");

            headers.ConnectionClose = true;
            headers.Connection.Clear();
            Assert.IsTrue(headers.ConnectionClose == false,
                "ConnectionClose should be modified by Connection.Clear().");
            Assert.AreEqual(0, headers.Connection.Count, "Count after Clear().");
            IEnumerable<string> dummyArray;
            Assert.IsFalse(headers.TryGetValues("Connection", out dummyArray),
                "Connection header count after Connection.Clear().");

            // Remove 'close' value from store. Since there are no other 'Connection' values, remove whole header.
            headers.ConnectionClose = false;
            Assert.IsTrue(headers.ConnectionClose == false, "ConnectionClose == false");
            Assert.AreEqual(0, headers.Connection.Count, "Connection.Count");
            Assert.IsFalse(headers.Contains("Connection"));

            headers.ConnectionClose = null;
            Assert.IsNull(headers.ConnectionClose, "ConnectionClose");
        }

        [TestMethod]
        public void Connection_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Connection", "custom1, close, custom2, custom3");

            // Connection collection has 3 values plus 'close'
            Assert.AreEqual(4, headers.Connection.Count, "Connection.Count");
            Assert.AreEqual(4, headers.GetValues("Connection").Count(), "Connection header value count.");
            Assert.IsTrue(headers.ConnectionClose == true, "ConnectionClose expected to be true.");

            Assert.AreEqual("custom1", headers.Connection.ElementAt(0), "Connection[0]");
            Assert.AreEqual("close", headers.Connection.ElementAt(1), "Connection[1]");
            Assert.AreEqual("custom2", headers.Connection.ElementAt(2), "Connection[2]");
            Assert.AreEqual("custom3", headers.Connection.ElementAt(3), "Connection[3]");

            headers.Connection.Clear();
            Assert.IsNull(headers.ConnectionClose, "ConnectionClose should be modified by Connection.Clear().");
            Assert.AreEqual(0, headers.Connection.Count, "Count after Clear().");
            IEnumerable<string> dummyArray;
            Assert.IsFalse(headers.TryGetValues("Connection", out dummyArray),
                "Connection header count after Connection.Clear().");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Connection_AddInvalidValue_Throw()
        {
            headers.Connection.Add("this is invalid");
        }

        [TestMethod]
        public void TransferEncoding_AddChunked_Success()
        {
            // use non-default casing to make sure we do case-insensitive comparison.
            headers.TransferEncoding.Add(new TransferCodingHeaderValue("CHUNKED"));
            Assert.IsTrue(headers.TransferEncodingChunked == true);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
        }

        [TestMethod]
        public void TransferEncoding_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.TransferEncoding.Count, "TransferEncoding expected to be empty on first call.");
            Assert.IsNull(headers.TransferEncodingChunked, "TransferEncodingChunked should be null by default.");

            headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom1"));
            headers.TransferEncoding.Add(new TransferCodingHeaderValue("custom2"));
            headers.TransferEncodingChunked = true;
            
            // Connection collection has 2 values plus 'chunked'
            Assert.AreEqual(3, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.AreEqual(3, headers.GetValues("Transfer-Encoding").Count(), "Transfer-Encoding header value count.");
            Assert.AreEqual(headers.TransferEncodingChunked, true, "TransferEncodingChunked == true");
            Assert.AreEqual(new TransferCodingHeaderValue("custom1"), headers.TransferEncoding.ElementAt(0),
                "TransferEncoding[0]");
            Assert.AreEqual(new TransferCodingHeaderValue("custom2"), headers.TransferEncoding.ElementAt(1),
                "TransferEncoding[1]");

            // Remove 'chunked' value from store. But leave other 'Transfer-Encoding' values. Note that according to
            // the RFC this is not valid, since 'chunked' must always be present. However this check is done
            // in the transport handler since the user can add invalid header values anyways.
            headers.TransferEncodingChunked = false;
            Assert.IsTrue(headers.TransferEncodingChunked == false, "TransferEncodingChunked == false");
            Assert.AreEqual(2, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.AreEqual(new TransferCodingHeaderValue("custom1"), headers.TransferEncoding.ElementAt(0),
                "TransferEncoding[0]");
            Assert.AreEqual(new TransferCodingHeaderValue("custom2"), headers.TransferEncoding.ElementAt(1),
                "TransferEncoding[1]");

            headers.TransferEncodingChunked = true;
            headers.TransferEncoding.Clear();
            Assert.IsTrue(headers.TransferEncodingChunked == false,
                "TransferEncodingChunked should be modified by TransferEncoding.Clear().");
            Assert.AreEqual(0, headers.TransferEncoding.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Transfer-Encoding"));

            // Remove 'chunked' value from store. Since there are no other 'Transfer-Encoding' values, remove whole
            // header.
            headers.TransferEncodingChunked = false;
            Assert.IsTrue(headers.TransferEncodingChunked == false, "TransferEncodingChunked == false");
            Assert.AreEqual(0, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.IsFalse(headers.Contains("Transfer-Encoding"));

            headers.TransferEncodingChunked = null;
            Assert.IsNull(headers.TransferEncodingChunked, "TransferEncodingChunked");
        }

        [TestMethod]
        public void TransferEncoding_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Transfer-Encoding", " , custom1, , custom2, custom3, chunked    ,");

            // Connection collection has 3 values plus 'chunked'
            Assert.AreEqual(4, headers.TransferEncoding.Count, "TransferEncoding.Count");
            Assert.AreEqual(4, headers.GetValues("Transfer-Encoding").Count(),
                "Transfer-Encoding header value count.");
            Assert.IsTrue(headers.TransferEncodingChunked == true, "TransferEncodingChunked expected to be true.");

            Assert.AreEqual(new TransferCodingHeaderValue("custom1"), headers.TransferEncoding.ElementAt(0),
                "TransferEncoding[0]");
            Assert.AreEqual(new TransferCodingHeaderValue("custom2"), headers.TransferEncoding.ElementAt(1),
                "TransferEncoding[1]");
            Assert.AreEqual(new TransferCodingHeaderValue("custom3"), headers.TransferEncoding.ElementAt(2),
                "TransferEncoding[2]");

            headers.TransferEncoding.Clear();
            Assert.IsNull(headers.TransferEncodingChunked,
                "TransferEncodingChunked should be modified by TransferEncoding.Clear().");
            Assert.AreEqual(0, headers.TransferEncoding.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Transfer-Encoding"),
                "Transfer-Encoding header after TransferEncoding.Clear().");
        }

        [TestMethod]
        public void TransferEncoding_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Transfer-Encoding", "custom会");
            Assert.IsNull(headers.GetParsedValues("Transfer-Encoding"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Transfer-Encoding").Count(), "Store value count");
            Assert.AreEqual("custom会", headers.GetValues("Transfer-Encoding").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Transfer-Encoding", "custom1 custom2");
            Assert.IsNull(headers.GetParsedValues("Transfer-Encoding"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Transfer-Encoding").Count(), "Store value count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Transfer-Encoding").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Transfer-Encoding", "");
            Assert.IsFalse(headers.Contains("Transfer-Encoding"), "'Transfer-Encoding' header should not be added if it just has empty values.");
        }

        [TestMethod]
        public void Upgrade_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Upgrade.Count, "Upgrade expected to be empty on first call.");

            headers.Upgrade.Add(new ProductHeaderValue("custom1"));
            headers.Upgrade.Add(new ProductHeaderValue("custom2", "1.1"));

            Assert.AreEqual(2, headers.Upgrade.Count, "Upgrade.Count");
            Assert.AreEqual(2, headers.GetValues("Upgrade").Count(), "Upgrade header value count.");
            Assert.AreEqual(new ProductHeaderValue("custom1"), headers.Upgrade.ElementAt(0), "Upgrade[0]");
            Assert.AreEqual(new ProductHeaderValue("custom2", "1.1"), headers.Upgrade.ElementAt(1), "Upgrade[1]");

            headers.Upgrade.Clear();
            Assert.AreEqual(0, headers.Upgrade.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Upgrade"), "Upgrade header should be removed after calling Clear().");
        }

        [TestMethod]
        public void Upgrade_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Upgrade", " , custom1 / 1.0, , custom2, custom3/2.0,");

            Assert.AreEqual(3, headers.Upgrade.Count, "Upgrade.Count");
            Assert.AreEqual(3, headers.GetValues("Upgrade").Count(),
                "Upgrade header value count.");

            Assert.AreEqual(new ProductHeaderValue("custom1", "1.0"), headers.Upgrade.ElementAt(0), "Upgrade[0]");
            Assert.AreEqual(new ProductHeaderValue("custom2"), headers.Upgrade.ElementAt(1), "Upgrade[1]");
            Assert.AreEqual(new ProductHeaderValue("custom3", "2.0"), headers.Upgrade.ElementAt(2), "Upgrade[2]");

            headers.Upgrade.Clear();
            Assert.AreEqual(0, headers.Upgrade.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Upgrade"), "Upgrade header should be removed after calling Clear().");
        }

        [TestMethod]
        public void Upgrade_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Upgrade", "custom会");
            Assert.IsNull(headers.GetParsedValues("Upgrade"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Upgrade").Count(), "Store value count");
            Assert.AreEqual("custom会", headers.GetValues("Upgrade").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Upgrade", "custom1 custom2");
            Assert.IsNull(headers.GetParsedValues("Upgrade"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Upgrade").Count(), "Store value count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Upgrade").First(), "Store value");
        }

        [TestMethod]
        public void Date_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.Date, "Host should be null by default.");

            DateTimeOffset expected = DateTimeOffset.Now;
            headers.Date = expected;
            Assert.AreEqual(expected, headers.Date);

            headers.Date = null;
            Assert.IsNull(headers.Date, "Date should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Date"),
                "Header store should not contain a header 'Date' after setting it to null.");

            // Make sure the header gets serialized correctly
            headers.Date = (new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero));
            Assert.AreEqual("Sun, 06 Nov 1994 08:49:37 GMT", headers.GetValues("Date").First(), "Serialized header");
        }

        [TestMethod]
        public void Date_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Date", "  Sun, 06 Nov 1994 08:49:37 GMT  ");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.Date);

            headers.Clear();
            headers.AddWithoutValidation("Date", "Sun, 06 Nov 1994 08:49:37 GMT");
            Assert.AreEqual(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero), headers.Date);
        }

        [TestMethod]
        public void Date_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Date", " Sun, 06 Nov 1994 08:49:37 GMT ,");
            Assert.IsNull(headers.GetParsedValues("Date"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Date").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov 1994 08:49:37 GMT ,", headers.GetValues("Date").First(), "Store value");

            headers.Clear();
            headers.AddWithoutValidation("Date", " Sun, 06 Nov ");
            Assert.IsNull(headers.GetParsedValues("Date"), "Parsed value.");
            Assert.AreEqual(1, headers.GetValues("Date").Count(), "Store value count");
            Assert.AreEqual(" Sun, 06 Nov ", headers.GetValues("Date").First(), "Store value");
        }
                
        [TestMethod]
        public void Via_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Via.Count, "Collection expected to be empty on first call.");

            headers.Via.Add(new ViaHeaderValue("x11", "host"));
            headers.Via.Add(new ViaHeaderValue("1.1", "example.com:8080", "HTTP", "(comment)"));

            Assert.AreEqual(2, headers.Via.Count, "Count");
            Assert.AreEqual(new ViaHeaderValue("x11", "host"), headers.Via.ElementAt(0), "Via[0]");
            Assert.AreEqual(new ViaHeaderValue("1.1", "example.com:8080", "HTTP", "(comment)"),
                headers.Via.ElementAt(1), "Via[1]");

            headers.Via.Clear();
            Assert.AreEqual(0, headers.Via.Count, "Count after Clear().");
        }

        [TestMethod]
        public void Via_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Via", ", 1.1 host, WS/1.0 [::1],X/11 192.168.0.1 (c(comment)) ");

            Assert.AreEqual(new ViaHeaderValue("1.1", "host"), headers.Via.ElementAt(0), "Via[0]");
            Assert.AreEqual(new ViaHeaderValue("1.0", "[::1]", "WS"), headers.Via.ElementAt(1), "Via[1]");
            Assert.AreEqual(new ViaHeaderValue("11", "192.168.0.1", "X", "(c(comment))"), headers.Via.ElementAt(2),
                "Via[2]");

            headers.Via.Clear();
            headers.AddWithoutValidation("Via", "");
            Assert.AreEqual(0, headers.Via.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Via"));
        }

        [TestMethod]
        public void Via_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Via", "1.1 host1 1.1 host2"); // no separator
            Assert.AreEqual(0, headers.Via.Count, "Via.Count");
            Assert.AreEqual(1, headers.GetValues("Via").Count(), "Via.Count");
            Assert.AreEqual("1.1 host1 1.1 host2", headers.GetValues("Via").First(), "Via value");

            headers.Clear();
            headers.AddWithoutValidation("Via", "X/11 host/1");
            Assert.AreEqual(0, headers.Via.Count, "Via.Count");
            Assert.AreEqual(1, headers.GetValues("Via").Count(), "Via.Count");
            Assert.AreEqual("X/11 host/1", headers.GetValues("Via").First(), "Via value");
        }

        [TestMethod]
        public void Warning_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Warning.Count, "Collection expected to be empty on first call.");

            headers.Warning.Add(new WarningHeaderValue(199, "microsoft.com", "\"Miscellaneous warning\""));
            headers.Warning.Add(new WarningHeaderValue(113, "example.com", "\"Heuristic expiration\""));

            Assert.AreEqual(2, headers.Warning.Count, "Count");
            Assert.AreEqual(new WarningHeaderValue(199, "microsoft.com", "\"Miscellaneous warning\""),
                headers.Warning.ElementAt(0), "Warning[0]");
            Assert.AreEqual(new WarningHeaderValue(113, "example.com", "\"Heuristic expiration\""),
                headers.Warning.ElementAt(1), "Warning[1]");

            headers.Warning.Clear();
            Assert.AreEqual(0, headers.Warning.Count, "Count after Clear().");
        }

        [TestMethod]
        public void Warning_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Warning",
                "112 example.com \"Disconnected operation\", 111 example.org \"Revalidation failed\"");

            Assert.AreEqual(new WarningHeaderValue(112, "example.com", "\"Disconnected operation\""),
                headers.Warning.ElementAt(0), "Warning[0]");
            Assert.AreEqual(new WarningHeaderValue(111, "example.org", "\"Revalidation failed\""),
                headers.Warning.ElementAt(1), "Warning[1]");

            headers.Warning.Clear();
            headers.AddWithoutValidation("Warning", "");
            Assert.AreEqual(0, headers.Warning.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Warning"));
        }

        [TestMethod]
        public void Warning_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Warning", "123 host1 \"\" 456 host2 \"\""); // no separator
            Assert.AreEqual(0, headers.Warning.Count, "Warning.Count");
            Assert.AreEqual(1, headers.GetValues("Warning").Count(), "Warning.Count");
            Assert.AreEqual("123 host1 \"\" 456 host2 \"\"", headers.GetValues("Warning").First(), "Warning value");

            headers.Clear();
            headers.AddWithoutValidation("Warning", "123 host1\"text\"");
            Assert.AreEqual(0, headers.Warning.Count, "Warning.Count");
            Assert.AreEqual(1, headers.GetValues("Warning").Count(), "Warning.Count");
            Assert.AreEqual("123 host1\"text\"", headers.GetValues("Warning").First(), "Warning value");
        }

        [TestMethod]
        public void CacheControl_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.IsNull(headers.CacheControl, "CacheControl should be null by default.");

            CacheControlHeaderValue value = new CacheControlHeaderValue();
            value.NoCache = true;
            value.NoCacheHeaders.Add("token1");
            value.NoCacheHeaders.Add("token2");
            value.MustRevalidate = true;
            value.SharedMaxAge = new TimeSpan(1, 2, 3);
            headers.CacheControl = value;
            Assert.AreEqual(value, headers.CacheControl);

            headers.CacheControl = null;
            Assert.IsNull(headers.CacheControl, "CacheControl should be null after setting it to null.");
            Assert.IsFalse(headers.Contains("Cache-Control"),
                "Header store should not contain a header 'Cache-Control' after setting it to null.");
        }

        [TestMethod]
        public void CacheControl_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Cache-Control", "no-cache=\"token1, token2\", must-revalidate, max-age=3");
            headers.Add("Cache-Control", "");
            headers.Add("Cache-Control", "public, s-maxage=15");
            headers.AddWithoutValidation("Cache-Control", "");

            CacheControlHeaderValue value = new CacheControlHeaderValue();
            value.NoCache = true;
            value.NoCacheHeaders.Add("token1");
            value.NoCacheHeaders.Add("token2");
            value.MustRevalidate = true;
            value.MaxAge = new TimeSpan(0, 0, 3);
            value.Public = true;
            value.SharedMaxAge = new TimeSpan(0, 0, 15);
            Assert.AreEqual(value, headers.CacheControl);
        }

        [TestMethod]
        public void Trailer_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Trailer.Count, "Collection expected to be empty on first call.");

            headers.Trailer.Add("custom1");
            headers.Trailer.Add("custom2");

            Assert.AreEqual(2, headers.Trailer.Count, "Trailer.Count");
            Assert.AreEqual(2, headers.GetValues("Trailer").Count(), "Trailer header value count.");

            Assert.AreEqual("custom1", headers.Trailer.ElementAt(0), "Trailer[0]");
            Assert.AreEqual("custom2", headers.Trailer.ElementAt(1), "Trailer[1]");

            headers.Trailer.Clear();
            Assert.AreEqual(0, headers.Trailer.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Trailer"),
                "There should be no Trailer header after calling Clear().");
        }

        [TestMethod]
        public void Trailer_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Trailer", ",custom1, custom2, custom3,");

            Assert.AreEqual(3, headers.Trailer.Count, "Trailer.Count");
            Assert.AreEqual(3, headers.GetValues("Trailer").Count(), "Trailer header value count.");

            Assert.AreEqual("custom1", headers.Trailer.ElementAt(0), "Trailer[0]");
            Assert.AreEqual("custom2", headers.Trailer.ElementAt(1), "Trailer[1]");
            Assert.AreEqual("custom3", headers.Trailer.ElementAt(2), "Trailer[2]");

            headers.Trailer.Clear();
            Assert.AreEqual(0, headers.Trailer.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Trailer"),
                "There should be no Trailer header after calling Clear().");
        }

        [TestMethod]
        public void Trailer_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Trailer", "custom1 custom2"); // no separator

            Assert.AreEqual(0, headers.Trailer.Count, "Trailer.Count");
            Assert.AreEqual(1, headers.GetValues("Trailer").Count(), "Trailer.Count");
            Assert.AreEqual("custom1 custom2", headers.GetValues("Trailer").First(), "Trailer value");
        }

        [TestMethod]
        public void Pragma_ReadAndWriteProperty_ValueMatchesPriorSetValue()
        {
            Assert.AreEqual(0, headers.Pragma.Count, "Collection expected to be empty on first call.");

            headers.Pragma.Add(new NameValueHeaderValue("custom1", "value1"));
            headers.Pragma.Add(new NameValueHeaderValue("custom2"));

            Assert.AreEqual(2, headers.Pragma.Count, "Pragma.Count");
            Assert.AreEqual(2, headers.GetValues("Pragma").Count(), "Pragma header value count.");

            Assert.AreEqual(new NameValueHeaderValue("custom1", "value1"), headers.Pragma.ElementAt(0), "Pragma[0]");
            Assert.AreEqual(new NameValueHeaderValue("custom2"), headers.Pragma.ElementAt(1), "Pragma[1]");

            headers.Pragma.Clear();
            Assert.AreEqual(0, headers.Pragma.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Pragma"),
                "There should be no Pragma header after calling Clear().");
        }

        [TestMethod]
        public void Pragma_UseAddMethod_AddedValueCanBeRetrievedUsingProperty()
        {
            headers.AddWithoutValidation("Pragma", ",custom1=value1, custom2, custom3=value3,");

            Assert.AreEqual(3, headers.Pragma.Count, "Pragma.Count");
            Assert.AreEqual(3, headers.GetValues("Pragma").Count(), "Pragma header value count.");

            Assert.AreEqual(new NameValueHeaderValue("custom1", "value1"), headers.Pragma.ElementAt(0), "Pragma[0]");
            Assert.AreEqual(new NameValueHeaderValue("custom2"), headers.Pragma.ElementAt(1), "Pragma[1]");
            Assert.AreEqual(new NameValueHeaderValue("custom3", "value3"), headers.Pragma.ElementAt(2), "Pragma[2]");

            headers.Pragma.Clear();
            Assert.AreEqual(0, headers.Pragma.Count, "Count after Clear().");
            Assert.IsFalse(headers.Contains("Pragma"),
                "There should be no Pragma header after calling Clear().");
        }

        [TestMethod]
        public void Pragma_UseAddMethodWithInvalidValue_InvalidValueRecognized()
        {
            headers.AddWithoutValidation("Pragma", "custom1, custom2="); 

            Assert.AreEqual(0, headers.Pragma.Count, "Pragma.Count");
            Assert.AreEqual(1, headers.GetValues("Pragma").Count(), "Pragma.Count");
            Assert.AreEqual("custom1, custom2=", headers.GetValues("Pragma").First(), "Pragma value");
        }

        #endregion
        
        [TestMethod]
        public void ToString_SeveralRequestHeaders_Success()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            string expected = string.Empty;
            
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/xml"));
            expected += HttpKnownHeaderNames.Accept + ": application/xml, */xml\r\n";

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic");
            expected += HttpKnownHeaderNames.Authorization + ": Basic\r\n";

            request.Headers.ExpectContinue = true;
            expected += HttpKnownHeaderNames.Expect + ": 100-continue\r\n";

            request.Headers.TransferEncodingChunked = true;
            expected += HttpKnownHeaderNames.TransferEncoding + ": chunked\r\n";
            
            Assert.AreEqual(expected, request.Headers.ToString());
        }

        [TestMethod]
        public void InvalidHeaders_AddContentAndResponseHeaders_Throw()
        {
            // Try adding response and content headers. Use different casing to make sure case-insensitive comparison
            // is used.
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Accept-Ranges", "v"), "Accept-Ranges");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.AddWithoutValidation("age", "v"), "age");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("ETag", "v"), "ETag");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Location", "v"), "Location");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Proxy-Authenticate", "v"), "Proxy-Authenticate");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Retry-After", "v"), "Retry-After");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Server", "v"), "Server");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("Vary", "v"), "Vary");
            ExceptionAssert.ThrowsInvalidOperation(() => headers.Add("WWW-Authenticate", "v"), "WWW-Authenticate");

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
