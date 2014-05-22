using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Collections;
using Microsoft.Net.Http.Test.Headers;
using System.Net.Test.Common;

namespace Microsoft.Net.Http.Test
{
    [TestClass]
    public class HttpHeadersTest
    {
        private const string knownHeader = "known-header";
        private const string customHeader = "custom-header";
        private const string rawPrefix = "raw";
        private const string parsedPrefix = "parsed";
        private const string invalidHeaderValue = "invalid";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWithoutValidation_UseNullHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(null, "value");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AddWithoutValidation_UseInvalidHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();

            // Spaces are not allowed in header names. This test is used to validate private method
            // HttpHeaders.CheckHeaders(). Since that helper method is used in all other public methods, this
            // test is not repeated for other public methods.
            headers.AddWithoutValidation("invalid header", "value");
        }

        [TestMethod]
        public void AddWithoutValidation_AddSingleValue_ValueParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            Assert.AreEqual(1, headers.Count(), "Expected 1 header.");
            Assert.AreEqual(1, headers.First().Value.Count(), "Expected 1 header value");
            Assert.AreEqual(parsedPrefix, headers.First().Value.First(), "Expected raw value to be parsed.");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddTwoSingleValues_BothValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddTwoValidValuesAsOneString_BothValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1," + rawPrefix + "2");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            // The parser gets called for each value in the raw string. I.e. if we have 1 raw string containing two
            // values, the parser gets called twice.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddTwoValuesOneValidOneInvalidAsOneString_RawStringAddedAsInvalid()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1," + invalidHeaderValue);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            // We expect the value to be returned without change since it couldn't be parsed in its entirety.
            Assert.AreEqual(rawPrefix + "1," + invalidHeaderValue, headers.First().Value.ElementAt(0),
                "Value at index 0.");

            // The parser gets called twice, but the second time it returns false, because it tries to parse
            // 'invalidHeaderValue'.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddTwoValueStringAndThirdValue_AllValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1," + rawPrefix + "2");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "3");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(3, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");

            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddInvalidAndValidValueString_BothValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix);
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix, headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(1), "Value at index 1.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddEmptyValueString_HeaderWithNoValueAfterParsing()
        {
            MockHeaders headers = new MockHeaders();

            // The parser returns 'true' to indicate that it could parse the value (empty values allowed) and an 
            // value of 'null'. HttpHeaders will remove the header from the collection since the known header doesn't
            // have a value.
            headers.AddWithoutValidation(knownHeader, string.Empty);
            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            headers.Clear();
            headers.AddWithoutValidation("custom", (string)null);
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(string.Empty, headers.GetValues("custom").First(), "Header value should be empty string.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddValidAndInvalidValueString_BothValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            // If you compare this test with the previous one: Note that we reversed the order of adding the invalid
            // string and the valid string. However, when enumerating header values the order is still the same as in
            // the previous test.
            // We don't keep track of the order if we have both invalid & valid values. This would add complexity
            // and additional memory to store the information. Given how rare this scenario is we consider this
            // by design. Note that this scenario is only an issue if:
            // - The header value has an invalid format (very rare for standard headers) AND
            // - There are multiple header values (some valid, some invalid) AND
            // - The order of the headers matters (e.g. Transfer-Encoding)
            Assert.AreEqual(parsedPrefix, headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(1), "Value at index 1.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            string expected = knownHeader + ": " + parsedPrefix + ", " + invalidHeaderValue + "\r\n";
            Assert.AreEqual(expected, headers.ToString());
        }

        [TestMethod]
        public void AddWithoutValidation_AddNullValueForKnownHeader_ParserRejectsNullEmptyStringAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, (string)null);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // MockParser is called with an empty string and decides that it is OK to have empty values but they
            // shouldn't be added to the list of header values. HttpHeaders will remove the header since it doesn't 
            // have values.
            Assert.AreEqual(0, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddNullValueForUnknownHeader_EmptyStringAddedAsValue()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(customHeader, (string)null);

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            // 'null' values are internally stored as string.Empty. Since we added a custom header, there is no
            // parser and the empty string is just added to the list of 'parsed values'.
            Assert.AreEqual(string.Empty, headers.First().Value.First(), "Value at index 0.");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddValueForUnknownHeader_ValueAddedToStore()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(customHeader, "custom value");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual("custom value", headers.First().Value.First(), "Value at index 0.");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddNullAndEmptyValuesToKnownHeader_HeaderRemovedFromCollection()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, (string)null);
            headers.AddWithoutValidation(knownHeader, string.Empty);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            // AddWithoutValidation() adds 'null' as string.empty to distinguish between an empty raw value and no raw
            // value. When the parser is called later, the parser can decide whether empty strings are valid or not.
            // In our case the MockParser returns 'success' with a parsed value of 'null' indicating that it is OK to
            // have empty values, but they should be ignored. 
            Assert.AreEqual(2, headers.Parser.EmptyValueCount, "EmptyValueCount");
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddNullAndEmptyValuesToUnknownHeader_TwoEmptyStringsAddedAsValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(customHeader, (string)null);
            headers.AddWithoutValidation(customHeader, string.Empty);

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            // AddWithoutValidation() adds 'null' as string.empty to distinguish between an empty raw value and no raw
            // value. For custom headers we just add what the user gives us. I.e. the result is a header with two empty
            // values.
            Assert.AreEqual(string.Empty, headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(string.Empty, headers.First().Value.ElementAt(1), "Value at index 1.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddMultipleValueToSingleValueHeaders_FirstHeaderAddedOthersAreInvalid()
        {
            MockHeaderParser parser = new MockHeaderParser(false); // doesn't support multiple values.
            MockHeaders headers = new MockHeaders(parser);
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            // Note that the first value was parsed and added to the 'parsed values' list. The second value however
            // was added to the 'invalid values' list since the header doesn't support multiple values.
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(rawPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            // The parser is only called once for the first value. HttpHeaders doesn't invoke the parser for
            // additional values if the parser only supports one value.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddMultipleValueStringToSingleValueHeaders_MultipleValueStringAddedAsInvalid()
        {
            MockHeaderParser parser = new MockHeaderParser(false); // doesn't support multiple values.
            MockHeaders headers = new MockHeaders(parser);
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1," + rawPrefix + "2");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Since parsing the header value fails because it is composed of 2 values, the original string is added
            // to the list of 'invalid values'. Therefore we only have 1 header value (the original string).
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(rawPrefix + "1," + rawPrefix + "2", headers.First().Value.First(), "Value at index 0.");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void AddWithoutValidation_AddValueContainingNewLine_NewLineFollowedByWhitespaceIsOKButNewLineFollowedByNonWhitespaceIsRejected()
        {
            MockHeaders headers = new MockHeaders();

            // The header parser rejects both of the following values. Both values contain new line chars. According
            // to the RFC, LWS supports newlines followed by whitespaces. I.e. the first value gets rejected by the
            // parser, but added to the list of invalid values.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\n other: value"); // OK, LWS is allowed
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(invalidHeaderValue + "\r\n other: value", headers.First().Value.First(), "Value at index 0.");
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // This value is considered invalid (newline char followed by non-whitepace). However, since
            // AddWithoutValidation() only causes the header value to be analyzed when it gets actually accessed, no
            // exception is thrown. Instead the value is discarded and a warning is logged.
            headers.Clear();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\nother:value");
            Assert.IsFalse(headers.Contains(knownHeader));
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            // Adding newline followed by whitespace to a custom header is OK.
            headers.Clear();
            headers.AddWithoutValidation("custom", "value\r\n other: value"); // OK, LWS is allowed
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual("value\r\n other: value", headers.First().Value.First(), "Value at index 0.");

            // Adding newline followed by non-whitespace chars is invalid. The value is discarded and a warning is
            // logged.
            headers.Clear();
            headers.AddWithoutValidation("custom", "value\r\nother: value");
            Assert.IsFalse(headers.Contains("custom"));
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            // Also ending a value with newline is invalid. Verify that valid values are added.
            headers.Clear();
            headers.Parser.TryParseValueCallCount = 0;
            headers.AddWithoutValidation(knownHeader, rawPrefix + "\rvalid");
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\n");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "\n," + invalidHeaderValue + "\r\nother");
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(parsedPrefix + "\rvalid", headers.First().Value.First(), "Value at index 0.");
            Assert.AreEqual(4, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.Clear();
            headers.AddWithoutValidation("custom", "value\r\ninvalid");
            headers.AddWithoutValidation("custom", "value\r\n valid");
            headers.AddWithoutValidation("custom", "validvalue, invalid\r\nvalue");
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual("value\r\n valid", headers.First().Value.First(), "Value at index 0.");
        }

        [TestMethod]
        public void AddWithoutValidation_MultipleAddInvalidValuesToNonExistingHeader_AddHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, new string[] { invalidHeaderValue });

            // Make sure the header did not get added since we just tried to add an invalid value.
            Assert.IsTrue(headers.Contains(knownHeader));
            Assert.AreEqual(1, headers.First().Value.Count());
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(0), "Expected invalid header value.");
        }

        [TestMethod]
        public void AddWithoutValidation_MultipleAddValidValueThenAddInvalidValuesToExistingHeader_AddValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, new string[] { rawPrefix + "2", invalidHeaderValue });

            Assert.IsTrue(headers.Contains(knownHeader));
            Assert.AreEqual(3, headers.First().Value.Count());
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Expected parsed header value.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Expected parsed header value.");
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(2), "Expected invalid header value.");
        }

        [TestMethod]
        public void AddWithoutValidation_MultipleAddValidValueThenAddInvalidValuesToNonExistingHeader_AddHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, new string[] { rawPrefix + "1", invalidHeaderValue });

            Assert.IsTrue(headers.Contains(knownHeader));
            Assert.AreEqual(2, headers.First().Value.Count());
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Expected parsed header value.");
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(1), "Expected invalid header value.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddWithoutValidation_MultipleAddNullValueCollection_Throws()
        {
            MockHeaders headers = new MockHeaders();
            string[] values = null;
            headers.AddWithoutValidation(knownHeader, values);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_SingleUseNullHeaderName_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(null, "value");
        }

        [TestMethod]
        public void Add_SingleUseStoreWithNoParserStore_AllHeadersConsideredCustom()
        {
            CustomTypeHeaders headers = new CustomTypeHeaders();
            headers.Add("custom", "value");

            Assert.AreEqual(1, headers.Count(), "No. of headers in store.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of headers values in store.");
            Assert.AreEqual("value", headers.First().Value.First(), "First header value.");
        }

        [TestMethod]
        public void Add_SingleAddValidValue_ValueParsedCorrectly()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix);

            // Add() should trigger parsing.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix, headers.First().Value.ElementAt(0), "Value at index 0.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleAddEmptyValueMultipleTimes_EmptyHeaderAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, (string)null);
            headers.Add(knownHeader, string.Empty);
            headers.Add(knownHeader, string.Empty);

            // Add() should trigger parsing.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(0, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        public void Add_SingleAddInvalidValueToNonExistingHeader_ThrowAndDontAddHeader()
        {
            // Since Add() immediately parses the value, it will throw an exception if the value is invalid.
            MockHeaders headers = new MockHeaders();
            ExceptionAssert.ThrowsFormat(() => headers.Add(knownHeader, invalidHeaderValue), "Invalid header value");

            // Make sure the header did not get added to the store.
            Assert.IsFalse(headers.Contains(knownHeader),
                "No header expected to be added since header value was invalid.");
        }

        [TestMethod]
        public void Add_SingleAddValidValueThenAddInvalidValue_ThrowAndHeaderContainsValidValue()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix);

            ExceptionAssert.ThrowsFormat(() => headers.Add(knownHeader, invalidHeaderValue), "Invalid header value");

            // Make sure the header did not get removed due to the failed add.
            Assert.IsTrue(headers.Contains(knownHeader), "Header was removed even if there is a valid header value.");
            Assert.AreEqual(1, headers.First().Value.Count(), "Expected one header value.");
            Assert.AreEqual(parsedPrefix, headers.First().Value.ElementAt(0), "Expected parsed header value.");
        }

        [TestMethod]
        public void Add_MultipleAddInvalidValuesToNonExistingHeader_ThrowAndDontAddHeader()
        {
            MockHeaders headers = new MockHeaders();

            ExceptionAssert.ThrowsFormat(() => headers.Add(knownHeader, new string[] { invalidHeaderValue }),
                "Invalid header header");

            // Make sure the header did not get added since we just tried to add an invalid value.
            Assert.IsFalse(headers.Contains(knownHeader), "Header was added even if we just added an invalid value.");
        }

        [TestMethod]
        public void Add_MultipleAddValidValueThenAddInvalidValuesToExistingHeader_ThrowAndDontAddHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");

            ExceptionAssert.ThrowsFormat(() =>
                headers.Add(knownHeader, new string[] { rawPrefix + "2", invalidHeaderValue }), "Invalid header value");

            // Make sure the header did not get removed due to the failed add. Note that the first value in the array
            // is valid, so it gets added. I.e. we have 2 values.
            Assert.IsTrue(headers.Contains(knownHeader), "Header was removed even if there is a valid header value.");
            Assert.AreEqual(2, headers.First().Value.Count(), "Expected one header value.");
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Expected parsed header value.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Expected parsed header value.");
        }

        [TestMethod]
        public void Add_MultipleAddValidValueThenAddInvalidValuesToNonExistingHeader_ThrowAndDontAddHeader()
        {
            MockHeaders headers = new MockHeaders();

            ExceptionAssert.ThrowsFormat(() =>
                headers.Add(knownHeader, new string[] { rawPrefix + "1", invalidHeaderValue }), "Invalid header value");

            // Make sure the header got added due to the valid add. Note that the first value in the array
            // is valid, so it gets added.
            Assert.IsTrue(headers.Contains(knownHeader), "Header was not added even though we added 1 valid value.");
            Assert.AreEqual(1, headers.First().Value.Count(), "Expected one header value.");
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Expected parsed header value.");
        }

        [TestMethod]
        public void Add_SingleAddThreeValidValues_ValuesParsedCorrectly()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.Add(knownHeader, rawPrefix + "2");
            headers.Add(knownHeader, rawPrefix + "3");

            // Add() should trigger parsing.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(3, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleAddTwoValidValuesToHeaderWithSingleValue_Throw()
        {
            MockHeaderParser parser = new MockHeaderParser(false); // doesn't support multiple values.
            MockHeaders headers = new MockHeaders(parser);

            try
            {
                headers.Add(knownHeader, rawPrefix + "1");
            }
            catch (Exception e)
            {
                Assert.Fail("Adding the first header already threw exception: {0}", e);
            }

            ExceptionAssert.ThrowsFormat(() => headers.Add(knownHeader, rawPrefix + "2"), "adding second value");

            // Verify that the first header value is still there.
            Assert.AreEqual(1, headers.First().Value.Count(), "Expected number of header values.");
        }

        [TestMethod]
        public void Add_SingleFirstAddWithoutValidationForValidValueThenAdd_TwoParsedValuesAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            headers.Add(knownHeader, rawPrefix + "2");

            // Add() should trigger parsing. AddWithoutValidation() doesn't trigger parsing, but Add() triggers
            // parsing of raw header values (TryParseValue() is called)
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleFirstAddWithoutValidationForInvalidValueThenAdd_TwoParsedValuesAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);
            headers.Add(knownHeader, rawPrefix + "1");

            // Add() should trigger parsing. AddWithoutValidation() doesn't trigger parsing, but Add() triggers
            // parsing of raw header values (TryParseValue() is called)
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(invalidHeaderValue, headers.First().Value.ElementAt(1), "Value at index 1.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleFirstAddWithoutValidationForEmptyValueThenAdd_OneParsedValueAddedEmptyIgnored()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, string.Empty);
            headers.Add(knownHeader, rawPrefix + "1");

            // Add() should trigger parsing. AddWithoutValidation() doesn't trigger parsing, but Add() triggers
            // parsing of raw header values (TryParseValue() is called)
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleFirstAddThenAddWithoutValidation_TwoParsedValuesAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            // Add() should trigger parsing. Since AddWithoutValidation() is called afterwards the second value is
            // not parsed yet.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleAddThenAddWithoutValidationThenAdd_ThreeParsedValuesAdded()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");
            headers.Add(knownHeader, rawPrefix + "3");

            // The second Add() triggers also parsing of the value added by AddWithoutValidation()
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(3, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Add_SingleFirstAddWithoutValidationThenAddToSingleValueHeader_AddThrows()
        {
            MockHeaderParser parser = new MockHeaderParser(false); // doesn't support multiple values.
            MockHeaders headers = new MockHeaders(parser);

            try
            {
                headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            }
            catch (Exception e)
            {
                Assert.Fail("Adding the first header already threw exception: {0}", e);
            }
            headers.Add(knownHeader, rawPrefix + "2");
        }

        [TestMethod]
        public void Add_SingleFirstAddThenAddWithoutValidationToSingleValueHeader_BothParsedAndInvalidValue()
        {
            MockHeaderParser parser = new MockHeaderParser(false); // doesn't support multiple values.
            MockHeaders headers = new MockHeaders(parser);
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Add() succeeds since we don't have a value added yet. AddWithoutValidation() also succeeds, however
            // the value is added to the 'invalid values' list when retrieved.
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(2, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(rawPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");

            // Note that TryParseValue() is not called because HttpHeaders sees that there is already a value
            // so it adds the raw value to 'invalid values'.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_MultipleAddThreeValidValuesWithOneCall_ValuesParsedCorrectly()
        {
            MockHeaders headers = new MockHeaders();
            string[] values = new string[] { rawPrefix + "1", rawPrefix + "2", rawPrefix + "3" };
            headers.Add(knownHeader, values);

            // Add() should trigger parsing.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(3, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");

            // Value is already parsed. There shouldn't be additional calls to the parser.
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_MultipleAddThreeValidValuesAsOneString_BothValuesParsed()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1," + rawPrefix + "2," + rawPrefix + "3");

            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(3, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_MultipleAddNullValueCollection_Throw()
        {
            MockHeaders headers = new MockHeaders();
            string[] values = null;
            headers.Add(knownHeader, values);
        }

        [TestMethod]
        public void Add_SingleAddCustomHeaderWithNullValue_HeaderIsAddedWithEmptyStringValue()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(customHeader, (string)null);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");

            Assert.AreEqual(string.Empty, headers.First().Value.ElementAt(0), "Value at index 0.");

            // We're using a custom header. No parsing should be triggered.
            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Add_SingleAddHeadersWithDifferentCasing_ConsideredTheSameHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom-header", "value1");
            headers.Add("Custom-Header", "value2");
            headers.Add("CUSTOM-HEADER", "value2");

            Assert.AreEqual(3, headers.GetValues("custom-header").Count(), "No. of values for header 'custom-header'.");
            Assert.AreEqual(3, headers.GetValues("Custom-Header").Count(), "No. of values for header 'Custom-Header'.");
            Assert.AreEqual(3, headers.GetValues("CUSTOM-HEADER").Count(), "No. of values for header 'CUSTOM-HEADER'.");
            Assert.AreEqual(3, headers.GetValues("CuStOm-HeAdEr").Count(), "No. of values for header 'CuStOm-HeAdEr'.");
        }

        [TestMethod]
        public void Add_AddValueContainingNewLine_NewLineFollowedByWhitespaceIsOKButNewLineFollowedByNonWhitespaceIsRejected()
        {
            MockHeaders headers = new MockHeaders();

            headers.Clear();
            headers.Add("custom", "value\r");
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual("value\r", headers.First().Value.First(), "Value at index 0.");

            headers.Clear();
            ExceptionAssert.ThrowsFormat(() =>
                headers.Add("custom", new string[] { "valid\n", "invalid\r\nother" }), "valid then invalid value");
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual("valid\n", headers.First().Value.First(), "Value at index 0.");
        }

        [TestMethod]
        public void RemoveParsedValue_AddValueAndRemoveIt_NoHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");

            // Remove the parsed value (note the original string 'raw1' was "parsed" to 'parsed1')
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, parsedPrefix + "1"));

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Remove the value again: It shouldn't be found in the store.
            Assert.IsFalse(headers.RemoveParsedValue(knownHeader, parsedPrefix + "1"));
        }

        [TestMethod]
        public void RemoveParsedValue_AddInvalidValueAndRemoveValidValue_InvalidValueRemains()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);

            // Remove a valid value which is not in the store.
            Assert.IsFalse(headers.RemoveParsedValue(knownHeader, parsedPrefix));

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(invalidHeaderValue, headers.GetValues(knownHeader).First(), "Store value");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Remove the value again: It shouldn't be found in the store.
            Assert.IsFalse(headers.RemoveParsedValue(knownHeader, parsedPrefix + "1"));
        }

        [TestMethod]
        public void RemoveParsedValue_ParserWithNoEqualityComparer_CaseSensitiveComparison()
        {
            CustomTypeHeaders headers = new CustomTypeHeaders("noComparerHeader", new NoComparerHeaderParser());
            headers.AddParsedValue("noComparerHeader", "lowercasevalue");

            // Since we don't provide a comparer, the default string.Equals() is called which is case-sensitive. So
            // the following call should return false.
            Assert.IsFalse(headers.RemoveParsedValue("noComparerHeader", "LOWERCASEVALUE"));

            // Now we try to remove the value using the correct casing. This should work.
            Assert.IsTrue(headers.RemoveParsedValue("noComparerHeader", "lowercasevalue"));

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(0, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        public void RemoveParsedValue_AddTwoValuesAndRemoveThem_NoHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.Add(knownHeader, rawPrefix + "2");

            // Remove the parsed value (note the original string 'raw1' was "parsed" to 'parsed1')
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, parsedPrefix + "1"));
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, parsedPrefix + "2"));

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void RemoveParsedValue_AddTwoValuesAndRemoveFirstOne_SecondValueRemains()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.Add(knownHeader, rawPrefix + "2");

            // Remove the parsed value (note the original string 'raw1' was "parsed" to 'parsed1')
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, parsedPrefix + "1"));

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(0), "Value at index 0.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void RemoveParsedValue_AddTwoValuesAndRemoveSecondOne_FirstValueRemains()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.Add(knownHeader, rawPrefix + "2");

            // Remove the parsed value (note the original string 'raw2' was "parsed" to 'parsed2')
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, parsedPrefix + "2"));

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Note that when the last value of a header gets removed, the whole header gets removed.
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
            Assert.AreEqual(1, headers.First().Value.Count(), "No. of header values.");
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");

            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void RemoveParsedValue_RemoveFromNonExistingHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix);

            // Header 'non-existing-header' can't be found, so false is returned.
            Assert.IsFalse(headers.RemoveParsedValue("non-existing-header", "doesntexist"));
        }

        [TestMethod]
        public void RemoveParsedValue_RemoveFromUninitializedHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();

            // If we never add a header value, the whole header (and also the header store) doesn't exist.
            // Make sure we considered this case.
            Assert.IsFalse(headers.RemoveParsedValue(knownHeader, "doesntexist"));
        }

        [TestMethod]
        public void RemoveParsedValue_AddOneValueToKnownHeaderAndCompareWithValueThatDiffersInCase_CustomComparerUsedForComparison()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddParsedValue(knownHeader, "value");

            // Our custom comparer (MockComparer) does case-insensitive value comparison. Verify that our custom
            // comparer is used to compare the header value.
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, "VALUE"));
            Assert.IsFalse(headers.Contains(knownHeader), "Header should be removed after removing value.");
            Assert.AreEqual(1, headers.Parser.MockComparer.EqualsCount, "Expected one call to Equals().");
        }

        [TestMethod]
        public void RemoveParsedValue_AddTwoValuesToKnownHeaderAndCompareWithValueThatDiffersInCase_CustomComparerUsedForComparison()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddParsedValue(knownHeader, "differentvalue");
            headers.AddParsedValue(knownHeader, "value");

            // Our custom comparer (MockComparer) does case-insensitive value comparison. Verify that our custom
            // comparer is used to compare the header value.
            // Note that since we added 2 values a different code path than in the previous test is used. In this
            // case we have stored the values as List<string> internally.
            Assert.IsTrue(headers.RemoveParsedValue(knownHeader, "VALUE"));
            Assert.AreEqual(1, headers.GetValues(knownHeader).Count(), "Expected one header value.");
            Assert.AreEqual(2, headers.Parser.MockComparer.EqualsCount, "Expected one call to Equals().");
        }

        [TestMethod]
        public void RemoveParsedValue_FirstAddInvalidNewlineCharsValueThenCallRemoveParsedValue_HeaderRemoved()
        {
            MockHeaders headers = new MockHeaders();

            // Add header value with invalid newline chars.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.RemoveParsedValue(knownHeader, "");

            Assert.IsFalse(headers.Contains(knownHeader), "Store should not have an entry for 'knownHeader'.");
        }

        [TestMethod]
        public void RemoveParsedValue_FirstAddInvalidNewlineCharsValueThenAddValidValueThenCallAddParsedValue_HeaderRemoved()
        {
            MockHeaders headers = new MockHeaders();

            // Add header value with invalid newline chars.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.RemoveParsedValue(knownHeader, parsedPrefix + "1");

            Assert.IsFalse(headers.Contains(knownHeader), "Store should not have an entry for 'knownHeader'.");
        }

        [TestMethod]
        public void Clear_AddMultipleHeadersAndThenClear_NoHeadersInCollection()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");
            headers.Add("custom1", "customValue1");
            headers.Add("custom2", "customValue2");
            headers.Add("custom3", "customValue3");

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // We added 4 different headers
            Assert.AreEqual(4, headers.Count(), "No. of headers.");

            headers.Clear();

            Assert.AreEqual(0, headers.Count(), "No. of headers.");

            // The call to Count() triggers a TryParseValue for the AddWithoutValidation() value. Clear() should
            // not cause any additional parsing operations.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Remove_UseNullHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.Remove(null);
        }

        [TestMethod]
        public void Remove_AddMultipleHeadersAndDeleteFirstAndLast_FirstAndLastHeaderRemoved()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");
            headers.Add("custom1", "customValue1");
            headers.Add("custom2", "customValue2");
            headers.Add("lastheader", "customValue3");

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // We added 4 different headers
            Assert.AreEqual(4, headers.Count(), "No. of headers.");

            // Remove first header
            Assert.IsTrue(headers.Remove(knownHeader));
            Assert.AreEqual(3, headers.Count(), "No. of headers.");

            // Remove last header
            Assert.IsTrue(headers.Remove("lastheader"));
            Assert.AreEqual(2, headers.Count(), "No. of headers.");

            // The call to Count() triggers a TryParseValue for the AddWithoutValidation() value. Clear() should
            // not cause any additional parsing operations.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Remove_RemoveHeaderFromUninitializedHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();

            // Remove header from uninitialized store (store collection is null)
            Assert.IsFalse(headers.Remove(knownHeader));
            Assert.AreEqual(0, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        public void Remove_RemoveNonExistingHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");

            Assert.AreEqual(1, headers.Count(), "No. of headers.");

            // Remove header from empty store
            Assert.IsFalse(headers.Remove("doesntexist"));
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TryGetValues_UseNullHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();

            IEnumerable<string> values = null;

            headers.TryGetValues(null, out values);
        }

        [TestMethod]
        public void TryGetValues_GetValuesFromUninitializedHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();

            IEnumerable<string> values = null;

            // Get header values from uninitialized store (store collection is null)
            Assert.IsFalse(headers.TryGetValues("doesntexist", out values));
            Assert.AreEqual(0, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        public void TryGetValues_GetValuesForNonExistingHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");

            IEnumerable<string> values = null;

            // Get header values from uninitialized store (store collection is null)
            Assert.IsFalse(headers.TryGetValues("doesntexist", out values));
            Assert.AreEqual(1, headers.Count(), "No. of headers.");
        }

        [TestMethod]
        public void TryGetValues_GetValuesForExistingHeader_ReturnsTrueAndListOfValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");
            headers.AddWithoutValidation(knownHeader, string.Empty);

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            IEnumerable<string> values = null;

            Assert.IsTrue(headers.TryGetValues(knownHeader, out values));
            Assert.IsNotNull(values, "Returned IEnumerable<> result is null.");

            // TryGetValues() should trigger parsing of values added with AddWithoutValidation()
            Assert.AreEqual(3, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(2, values.Count(), "No. of header values returned.");

            // Check returned values
            Assert.AreEqual(parsedPrefix + "1", values.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", values.ElementAt(1), "Value at index 1.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetValues_UseNullHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.GetValues(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValues_GetValuesFromUninitializedHeaderStore_Throw()
        {
            MockHeaders headers = new MockHeaders();

            // Get header values from uninitialized store (store collection is null). This will throw.
            headers.GetValues("doesntexist");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValues_GetValuesForNonExistingHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");

            // Get header values for non-existing header (but other headers exist in the store).
            headers.GetValues("doesntexist");
        }

        [TestMethod]
        public void GetValues_GetValuesForExistingHeader_ReturnsTrueAndListOfValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation("custom", rawPrefix + "0"); // this must not influence the result.
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            IEnumerable<string> values = headers.GetValues(knownHeader);
            Assert.IsNotNull(values, "Returned IEnumerable<> result is null.");

            // GetValues() should trigger parsing of values added with AddWithoutValidation()
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(2, values.Count(), "No. of header values returned.");

            // Check returned values
            Assert.AreEqual(parsedPrefix + "1", values.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", values.ElementAt(1), "Value at index 1.");
        }

        [TestMethod]
        public void GetValues_HeadersWithEmptyValues_ReturnsEmptyArray()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(customHeader, (string)null);
            headers.Add(knownHeader, string.Empty);

            // In the known header case, the MockParser accepts empty values but tells the store to not add the value.
            // Since no value is added for 'knownHeader', HttpHeaders removes the header from the store. This is only
            // done for known headers. Custom headers are allowed to have empty/null values as shown by 
            // 'valuesForCustomHeaders' below
            Assert.IsFalse(headers.Contains(knownHeader));

            // In the custom header case, we add whatever the users adds (besides that we add string.Empty if the
            // user adds null). So here we do have 1 value: string.Empty.
            IEnumerable<string> valuesForCustomHeader = headers.GetValues(customHeader);
            Assert.IsNotNull(valuesForCustomHeader, "Returned IEnumerable<> result is null.");
            Assert.AreEqual(1, valuesForCustomHeader.Count(), "No. of header values returned.");
            Assert.AreEqual(string.Empty, valuesForCustomHeader.First(), "Value at index 0 (custom header).");
        }

        [TestMethod]
        public void GetParsedValues_GetValuesFromUninitializedHeaderStore_ReturnsNull()
        {
            MockHeaders headers = new MockHeaders();

            // Get header values from uninitialized store (store collection is null).
            object storeValue = headers.GetParsedValues("doesntexist");
            Assert.IsNull(storeValue, "Expected no value in the store.");
        }

        [TestMethod]
        public void GetParsedValues_GetValuesForNonExistingHeader_ReturnsNull()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");

            // Get header values for non-existing header (but other headers exist in the store).
            object storeValue = headers.GetParsedValues("doesntexist");
            Assert.IsNull(storeValue, "Expected no value in the store.");
        }

        [TestMethod]
        public void GetParsedValues_GetSingleValueForExistingHeader_ReturnsAddedValue()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");

            // Get header values for non-existing header (but other headers exist in the store).
            object storeValue = headers.GetParsedValues("custom1");
            Assert.IsNotNull(storeValue, "Returned value is null.");

            // If we only have one value, then GetValues() should return just the value and not wrap it in a List<T>.
            Assert.AreEqual("customValue1", storeValue, "Expected provided value to be the result.");
        }

        [TestMethod]
        public void GetParsedValues_HeaderWithEmptyValues_ReturnsEmpty()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, string.Empty);

            object storeValue = headers.GetParsedValues(knownHeader);
            Assert.IsNull(storeValue, "Returned value is not null.");
        }

        [TestMethod]
        public void GetParsedValues_GetMultipleValuesForExistingHeader_ReturnsListOfValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation("custom", rawPrefix + "0"); // this must not influence the result.
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            object storeValue = headers.GetParsedValues(knownHeader);
            Assert.IsNotNull(storeValue, "Returned value is null.");

            // GetValues<T>() should trigger parsing of values added with AddWithoutValidation()
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Since we added 2 values to header 'knownHeader', we expect GetValues() to return a List<T> with
            // two values.
            List<object> storeValues = storeValue as List<object>;
            Assert.IsNotNull(storeValues, "Expected returned value to be a List<T>.");
            Assert.AreEqual(2, storeValues.Count, "Expected two values to be returned.");
            Assert.AreEqual(parsedPrefix + "1", storeValues[0], "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", storeValues[1], "Value at index 1.");
        }

        [TestMethod]
        public void GetParsedValues_GetValuesForExistingHeaderWithInvalidValues_ReturnsOnlyParsedValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix);

            // Here we add an invalid value. GetValues<T> only returns parsable values. So this value should get
            // parsed, however it will be added to the 'invalid values' list and thus is not part of the collection
            // returned by the enumerator.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);

            // Only 1 value should get parsed (call to Add() with known header value).
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            object storeValue = headers.GetParsedValues(knownHeader);
            Assert.IsNotNull(storeValue, "Returned value is null.");

            // GetValues<T>() should trigger parsing of values added with AddWithoutValidation()
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            // Since we added only one valid value to 'knownHeader', we expect GetValues() to return a that value.
            Assert.AreEqual(parsedPrefix, storeValue, "Expected provided value to be the result.");
        }

        [TestMethod]
        public void GetParsedValues_GetValuesForExistingHeaderWithOnlyInvalidValues_ReturnsEmptyEnumerator()
        {
            MockHeaders headers = new MockHeaders();

            // Here we add an invalid value. GetValues<T> only returns parsable values. So this value should get
            // parsed, however it will be added to the 'invalid values' list and thus is not part of the collection
            // returned by the enumerator.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            object storeValue = headers.GetParsedValues(knownHeader);
            Assert.IsNull(storeValue, "Expected no value in the store.");

            // GetValues<T>() should trigger parsing of values added with AddWithoutValidation()
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void GetParsedValues_AddInvalidValueToHeader_HeaderGetsRemovedAndNullReturned()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            object storeValue = headers.GetParsedValues(knownHeader);
            Assert.IsNull(storeValue, "Expected no value in the store.");
            Assert.IsFalse(headers.Contains(knownHeader), "'knownHeader' should be removed from store.");
        }

        [TestMethod]
        public void GetParsedValues_GetParsedValuesForKnownHeaderWithInvalidNewlineChars_ReturnsNull()
        {
            MockHeaders headers = new MockHeaders();

            // Add header value with invalid newline chars.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
            Assert.IsNull(headers.GetParsedValues(knownHeader), "Expected no parsed values.");
            Assert.AreEqual(0, headers.Count(), "Header value should be removed since it contains invalid newline chars.");
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void GetHeaderStrings_SetValidAndInvalidHeaderValues_AllHeaderValuesReturned()
        {
            MockHeaderParser parser = new MockHeaderParser("---");
            MockHeaders headers = new MockHeaders(parser);

            // Add header value with invalid newline chars.
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, "value2,value3");
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);

            foreach (var header in headers.GetHeaderStrings())
            {
                Assert.AreEqual(knownHeader, header.Key);
                // Note that raw values don't get parsed but just added to the result.
                Assert.AreEqual("value2,value3---" + invalidHeaderValue + "---" + parsedPrefix + "1", header.Value);
            }
        }

        [TestMethod]
        public void GetHeaderStrings_SetMultipleHeaders_AllHeaderValuesReturned()
        {
            MockHeaderParser parser = new MockHeaderParser(true);
            MockHeaders headers = new MockHeaders(parser);

            // Add header value with invalid newline chars.
            headers.Add(knownHeader, rawPrefix + "1");
            headers.Add("header2", "value2");
            headers.Add("header3", (string)null);
            headers.Add("header4", "value41");
            headers.Add("header4", "value42");

            string[] expectedHeaderNames = { knownHeader, "header2", "header3", "header4" };
            string[] expectedHeaderValues = { parsedPrefix + "1", "value2", "", "value41, value42" };
            int i = 0;

            foreach (var header in headers.GetHeaderStrings())
            {
                Assert.AreNotEqual(expectedHeaderNames.Length, i, "More headers than expected returned.");
                Assert.AreEqual(expectedHeaderNames[i], header.Key);
                Assert.AreEqual(expectedHeaderValues[i], header.Value);
                i++;
            }
        }

        [TestMethod]
        public void GetHeaderStrings_SetMultipleValuesOnSingleValueHeader_AllHeaderValuesReturned()
        {
            MockHeaderParser parser = new MockHeaderParser(false);
            MockHeaders headers = new MockHeaders(parser);
            
            headers.AddWithoutValidation(knownHeader, "value1");
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            foreach (var header in headers.GetHeaderStrings())
            {
                Assert.AreEqual(knownHeader, header.Key);
                // Note that the added rawPrefix did not get parsed
                Assert.AreEqual("value1, " + rawPrefix, header.Value); 
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Contains_UseNullHeader_Throw()
        {
            MockHeaders headers = new MockHeaders();
            headers.Contains(null);
        }

        [TestMethod]
        public void Contains_CallContainsFromUninitializedHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            Assert.IsFalse(headers.Contains("doesntexist"));
        }

        [TestMethod]
        public void Contains_CallContainsForNonExistingHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix);
            Assert.IsFalse(headers.Contains("doesntexist"));
        }

        [TestMethod]
        public void Contains_CallContainsForEmptyHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, string.Empty);
            Assert.IsFalse(headers.Contains(knownHeader));
        }

        [TestMethod]
        public void Contains_CallContainsForExistingHeader_ReturnsTrue()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");
            headers.Add("custom2", "customValue2");
            headers.Add("custom3", "customValue3");
            headers.Add("custom4", "customValue4");
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            // Nothing got parsed so far since we just added custom headers and for the known header we called
            // AddWithoutValidation().
            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsTrue(headers.Contains(knownHeader));

            // Contains() should trigger parsing of values added with AddWithoutValidation(): If the value was invalid,
            // i.e. contains invalid newline chars, then the header will be removed from the collection.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void Contains_AddValuesWithInvalidNewlineChars_HeadersGetRemovedWhenCallingContains()
        {
            MockHeaders headers = new MockHeaders();

            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");
            headers.AddWithoutValidation("custom", "invalid\r\nvalue");

            Assert.IsFalse(headers.Contains(knownHeader), "Store should not have an entry for 'knownHeader'.");
            Assert.IsFalse(headers.Contains("custom"), "Store should not have an entry for 'custom'.");
        }

        [TestMethod]
        public void GetEnumerator_GetEnumeratorFromUninitializedHeaderStore_ReturnsEmptyEnumerator()
        {
            MockHeaders headers = new MockHeaders();

            var enumerator = headers.GetEnumerator();
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void GetEnumerator_FirstHeaderWithOneValueSecondHeaderWithTwoValues_EnumeratorReturnsTwoHeaders()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(customHeader, "custom0");
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            // The value added with AddWithoutValidation() wasn't parsed yet.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            var enumerator = headers.GetEnumerator();

            // Getting the enumerator doesn't trigger parsing.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(customHeader, enumerator.Current.Key, "First header name.");
            Assert.AreEqual(1, enumerator.Current.Value.Count(), "No. of values for first header.");
            Assert.AreEqual("custom0", enumerator.Current.Value.ElementAt(0), "Value at index 0 (first header).");

            // Starting using the enumerator will trigger parsing of raw values. The first header is not a known
            // header, so there shouldn't be any parsing.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(knownHeader, enumerator.Current.Key, "Second header name.");
            Assert.AreEqual(2, enumerator.Current.Value.Count(), "No. of values for second header.");
            Assert.AreEqual(parsedPrefix + "1", enumerator.Current.Value.ElementAt(0), "Value at index 0 (second header).");
            Assert.AreEqual(parsedPrefix + "2", enumerator.Current.Value.ElementAt(1), "Value at index 1 (second header).");

            // The second header is a known header, so parsing raw values should get executed.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsFalse(enumerator.MoveNext(), "Only 2 values expected, but enumerator returns a third one.");
        }

        [TestMethod]
        public void GetEnumerator_FirstCustomHeaderWithEmptyValueSecondKnownHeaderWithEmptyValue_EnumeratorReturnsOneHeader()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(customHeader, string.Empty);
            headers.Add(knownHeader, string.Empty);

            var enumerator = headers.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(customHeader, enumerator.Current.Key, "First header name.");
            Assert.AreEqual(1, enumerator.Current.Value.Count(), "No. of values for first header.");
            Assert.AreEqual(string.Empty, enumerator.Current.Value.ElementAt(0), "Value at index 0 (first header).");

            Assert.IsFalse(enumerator.MoveNext(), "Only the (empty) custom value should be returned.");
        }

        [TestMethod]
        public void GetEnumerator_UseExplicitInterfaceImplementation_EnumeratorReturnsNoOfHeaders()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add("custom1", "customValue1");
            headers.Add("custom2", "customValue2");
            headers.Add("custom3", "customValue3");
            headers.Add("custom4", "customValue4");

            System.Collections.IEnumerable headersAsIEnumerable = headers;

            var enumerator = headersAsIEnumerable.GetEnumerator();

            KeyValuePair<string, IEnumerable<string>> currentValue;

            for (int i = 1; i <= 4; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                currentValue = (KeyValuePair<string, IEnumerable<string>>)enumerator.Current;
                Assert.AreEqual("custom" + i, currentValue.Key, "Header no. {0}.", i);
                Assert.AreEqual(1, currentValue.Value.Count(), "No. of header values for header no. {0}.", i);
            }

            Assert.IsFalse(enumerator.MoveNext(), "Only 2 values expected, but enumerator returns a third one.");
        }

        [TestMethod]
        public void AddParsedValue_AddSingleValueToNonExistingHeader_HeaderGetsCreatedAndValueAdded()
        {
            Uri headerValue = new Uri("http://example.org/");

            CustomTypeHeaders headers = new CustomTypeHeaders("myheader", new CustomTypeHeaderParser());
            headers.AddParsedValue("myheader", headerValue);

            Assert.IsTrue(headers.Contains("myheader"), "Store doesn't have the header after adding a value to it.");

            Assert.AreEqual(headerValue, headers.First().Value.ElementAt(0), "Value at index 0.");
        }

        [TestMethod]
        public void AddParsedValue_AddValueTypeValueToNonExistingHeader_HeaderGetsCreatedAndBoxedValueAdded()
        {
            int headerValue = 5;

            CustomTypeHeaders headers = new CustomTypeHeaders("myheader", new CustomTypeHeaderParser());
            headers.AddParsedValue("myheader", headerValue);

            Assert.IsTrue(headers.Contains("myheader"), "Store doesn't have the header after adding a value to it.");

            Assert.AreEqual(headerValue.ToString(), headers.First().Value.ElementAt(0), "Value at index 0.");
        }

        [TestMethod]
        public void AddParsedValue_AddTwoValuesToNonExistingHeader_HeaderGetsCreatedAndValuesAdded()
        {
            Uri headerValue1 = new Uri("http://example.org/1/");
            Uri headerValue2 = new Uri("http://example.org/2/");

            CustomTypeHeaders headers = new CustomTypeHeaders("myheader", new CustomTypeHeaderParser());
            headers.AddParsedValue("myheader", headerValue1);

            // Adding a second value will cause a List<T> to be created in order to store values. If we just add
            // one value, no List<T> is created, but the header is just added as store value.
            headers.AddParsedValue("myheader", headerValue2);

            Assert.IsTrue(headers.Contains("myheader"), "Store doesn't have the header after adding a value to it.");
            Assert.AreEqual(2, headers.GetValues("myheader").Count(), "Expected header value count.");

            Assert.AreEqual(headerValue1, headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(headerValue2, headers.First().Value.ElementAt(1), "Value at index 1.");
        }

        [TestMethod]
        public void AddParsedValue_UseDifferentAddMethods_AllValuesAddedCorrectly()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.AddParsedValue(knownHeader, parsedPrefix + "3");

            // Adding a parsed value, will trigger all raw values to be parsed.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(3, headers.GetValues(knownHeader).Count(), "Expected header value count.");
            Assert.AreEqual(parsedPrefix + "1", headers.First().Value.ElementAt(0), "Value at index 0.");
            Assert.AreEqual(parsedPrefix + "2", headers.First().Value.ElementAt(1), "Value at index 1.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(2), "Value at index 2.");
        }

        [TestMethod]
        public void AddParsedValue_FirstAddInvalidNewlineCharsValueThenCallAddParsedValue_ParsedValueAdded()
        {
            MockHeaders headers = new MockHeaders();

            // Add header value with invalid newline chars.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.AddParsedValue(knownHeader, parsedPrefix + "1");

            Assert.IsTrue(headers.Contains(knownHeader), "Store should have an entry for 'knownHeader'.");
            Assert.AreEqual(1, headers.GetValues(knownHeader).Count(), "'knownHeader' value count.");
            Assert.AreEqual(parsedPrefix + "1", headers.GetValues(knownHeader).First(), "First 'knownHeader' value.");
        }

        [TestMethod]
        public void AddParsedValue_FirstAddInvalidNewlineCharsValueThenAddValidValueThenCallAddParsedValue_ParsedValueAdded()
        {
            MockHeaders headers = new MockHeaders();

            // Add header value with invalid newline chars.
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue + "\r\ninvalid");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "0");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.AddParsedValue(knownHeader, parsedPrefix + "1");

            Assert.IsTrue(headers.Contains(knownHeader), "Store should have an entry for 'knownHeader'.");
            Assert.AreEqual(2, headers.GetValues(knownHeader).Count(), "'knownHeader' value count.");
            Assert.AreEqual(parsedPrefix + "0", headers.GetValues(knownHeader).ElementAt(0), "'knownHeader' value[0]");
            Assert.AreEqual(parsedPrefix + "1", headers.GetValues(knownHeader).ElementAt(1), "'knownHeader' value[1]");
        }

        [TestMethod]
        public void SetParsedValue_AddSingleValueToNonExistingHeader_HeaderGetsCreatedAndValueAdded()
        {
            Uri headerValue = new Uri("http://example.org/");

            CustomTypeHeaders headers = new CustomTypeHeaders("myheader", new CustomTypeHeaderParser());
            headers.SetParsedValue("myheader", headerValue);

            Assert.IsTrue(headers.Contains("myheader"), "Store doesn't have the header after adding a value to it.");

            Assert.AreEqual(headerValue, headers.First().Value.ElementAt(0), "Value at index 0.");
        }

        [TestMethod]
        public void SetParsedValue_SetTwoValuesToNonExistingHeader_HeaderGetsCreatedAndLastValueAdded()
        {
            Uri headerValue1 = new Uri("http://example.org/1/");
            Uri headerValue2 = new Uri("http://example.org/2/");

            CustomTypeHeaders headers = new CustomTypeHeaders("myheader", new CustomTypeHeaderParser());
            headers.SetParsedValue("myheader", headerValue1);

            // The following line will remove the previously added values and replace them with the provided value.
            headers.SetParsedValue("myheader", headerValue2);

            Assert.IsTrue(headers.Contains("myheader"), "Store doesn't have the header after adding a value to it.");
            Assert.AreEqual(1, headers.GetValues("myheader").Count(), "Expected header value count.");

            // The second value replaces the first value.
            Assert.AreEqual(headerValue2, headers.First().Value.ElementAt(0), "Value at index 0.");
        }

        [TestMethod]
        public void SetParsedValue_SetValueAfterAddingMultipleValues_SetValueReplacesOtherValues()
        {
            MockHeaders headers = new MockHeaders();
            headers.Add(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");

            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            headers.SetParsedValue(knownHeader, parsedPrefix + "3");

            // Adding a parsed value, will trigger all raw values to be parsed.
            Assert.AreEqual(2, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.AreEqual(1, headers.GetValues(knownHeader).Count(), "Expected header value count.");
            Assert.AreEqual(parsedPrefix + "3", headers.First().Value.ElementAt(0), "Value at index 0.");
        }

        [TestMethod]
        public void ContainsParsedValue_ContainsParsedValueFromUninitializedHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            Assert.IsFalse(headers.ContainsParsedValue(customHeader, "custom1"));
        }

        [TestMethod]
        public void ContainsParsedValue_ContainsParsedValueForNonExistingHeader_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsFalse(headers.ContainsParsedValue(customHeader, "custom1"));

            // ContainsParsedValue() must not trigger raw value parsing for headers other than the requested one.
            // In this case we expect ContainsParsedValue(customeHeader) not to trigger raw value parsing for
            // 'knownHeader'.
            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void ContainsParsedValue_ContainsParsedValueForNonExistingHeaderValue_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddParsedValue(knownHeader, "value1");
            headers.AddParsedValue(knownHeader, "value2");

            // After adding two values to header 'knownHeader' we ask for a non-existing value.
            Assert.IsFalse(headers.ContainsParsedValue(knownHeader, "doesntexist"));
        }

        [TestMethod]
        public void ContainsParsedValue_ContainsParsedValueForExistingHeaderButNonAvailableValue_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix);

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsFalse(headers.ContainsParsedValue(knownHeader, "custom1"));

            // ContainsParsedValue() must trigger raw value parsing for the header it was asked for.
            Assert.AreEqual(1, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void ContainsParsedValue_ContainsParsedValueForExistingHeaderWithAvailableValue_ReturnsTrue()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddWithoutValidation(knownHeader, rawPrefix + "1");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "2");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "3");
            headers.AddWithoutValidation(knownHeader, rawPrefix + "4");

            Assert.AreEqual(0, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");

            Assert.IsTrue(headers.ContainsParsedValue(knownHeader, parsedPrefix + "3"));

            // ContainsParsedValue() must trigger raw value parsing for the header it was asked for.
            Assert.AreEqual(4, headers.Parser.TryParseValueCallCount, "No. of times TryParseValueCallCount was called.");
        }

        [TestMethod]
        public void ContainsParsedValue_AddOneValueToKnownHeaderAndCompareWithValueThatDiffersInCase_CustomComparerUsedForComparison()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddParsedValue(knownHeader, "value");

            // Our custom comparer (MockComparer) does case-insensitive value comparison. Verify that our custom
            // comparer is used to compare the header value.
            Assert.IsTrue(headers.ContainsParsedValue(knownHeader, "VALUE"));
            Assert.AreEqual(1, headers.Parser.MockComparer.EqualsCount, "Expected one call to Equals().");

            headers.Clear();
            headers.AddWithoutValidation(knownHeader, invalidHeaderValue);
            Assert.IsFalse(headers.ContainsParsedValue(knownHeader, invalidHeaderValue));
        }

        [TestMethod]
        public void ContainsParsedValue_AddTwoValuesToKnownHeaderAndCompareWithValueThatDiffersInCase_CustomComparerUsedForComparison()
        {
            MockHeaders headers = new MockHeaders();
            headers.AddParsedValue(knownHeader, "differentvalue");
            headers.AddParsedValue(knownHeader, "value");

            // Our custom comparer (MockComparer) does case-insensitive value comparison. Verify that our custom
            // comparer is used to compare the header value.
            // Note that since we added 2 values a different code path than in the previous test is used. In this
            // case we have stored the values as List<string> internally.
            Assert.IsTrue(headers.ContainsParsedValue(knownHeader, "VALUE"));
            Assert.AreEqual(2, headers.Parser.MockComparer.EqualsCount, "Expected one call to Equals().");
        }

        [TestMethod]
        public void ContainsParsedValue_ParserWithNoEqualityComparer_CaseSensitiveComparison()
        {
            CustomTypeHeaders headers = new CustomTypeHeaders("noComparerHeader", new NoComparerHeaderParser());
            headers.AddParsedValue("noComparerHeader", "lowercasevalue");

            // Since we don't provide a comparer, the default string.Equals() is called which is case-sensitive. So
            // the following call should return false.
            Assert.IsFalse(headers.ContainsParsedValue("noComparerHeader", "LOWERCASEVALUE"));

            // Now we try to use the correct casing. This should return true.
            Assert.IsTrue(headers.ContainsParsedValue("noComparerHeader", "lowercasevalue"));
        }

        [TestMethod]
        public void ContainsParsedValue_CallFromEmptyHeaderStore_ReturnsFalse()
        {
            MockHeaders headers = new MockHeaders();

            // This will create a header entry with no value.
            headers.Add(knownHeader, string.Empty);

            Assert.IsFalse(headers.Contains(knownHeader), "Expected known header to be in the store.");

            // This will just return fals and not touch the header.
            Assert.IsFalse(headers.ContainsParsedValue(knownHeader, "x"),
                "Expected 'ContainsParsedValue' to return false.");
        }

        [TestMethod]
        public void AddHeaders_SourceAndDestinationStoreHaveMultipleHeaders_OnlyHeadersNotInDestinationAreCopiedFromSource()
        {
            Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
            parserStore.Add("known1", new MockHeaderParser());
            parserStore.Add("known2", new MockHeaderParser());
            parserStore.Add("known3", new MockHeaderParser());
            parserStore.Add("known4", new CustomTypeHeaderParser());

            // Add header values to the source store.
            MockHeaders source = new MockHeaders(parserStore);
            source.Add("custom1", "source10");
            source.Add("custom1", "source11");

            source.AddWithoutValidation("custom2", "source2");

            source.Add("known1", rawPrefix + "3");
            source.AddWithoutValidation("known1", rawPrefix + "4");

            source.AddWithoutValidation("known2", rawPrefix + "5");
            source.AddWithoutValidation("known2", invalidHeaderValue);
            source.AddWithoutValidation("known2", rawPrefix + "7");

            // this header value gets removed when it gets parsed.
            source.AddWithoutValidation("known3", (string)null);
            source.Add("known3", string.Empty);

            DateTimeOffset known4Value1 = new DateTimeOffset(2010, 6, 15, 18, 31, 34, TimeSpan.Zero);
            DateTimeOffset known4Value2 = new DateTimeOffset(2010, 4, 8, 11, 21, 04, TimeSpan.Zero);
            source.AddParsedValue("known4", known4Value1);
            source.AddParsedValue("known4", known4Value2);

            source.Add("custom5", "source5");
            source.AddWithoutValidation("custom6", (string)null);

            // This header value gets added even though it doesn't have values. But since this is a custom header we
            // assume it supports empty values.
            source.AddWithoutValidation("custom7", (string)null);
            source.Add("custom7", string.Empty);

            // Add header values to the destination store.
            MockHeaders destination = new MockHeaders(parserStore);
            destination.Add("custom2", "destination1");
            destination.Add("known1", rawPrefix + "9");

            // Now add all headers that are in source but not destination to destination.
            destination.AddHeaders(source);

            Assert.AreEqual(8, destination.Count(), "Expected headers.");

            Assert.AreEqual(2, destination.GetValues("custom1").Count(), "'custom1' header value count.");
            Assert.AreEqual("source10", destination.GetValues("custom1").ElementAt(0), "'custom1' header value[0].");
            Assert.AreEqual("source11", destination.GetValues("custom1").ElementAt(1), "'custom1' header value[1].");

            // This value was set in destination. The header in source was ignored.
            Assert.AreEqual(1, destination.GetValues("custom2").Count(), "'custom2' header value count.");
            Assert.AreEqual("destination1", destination.GetValues("custom2").First(), "'custom2' header value.");

            // This value was set in destination. The header in source was ignored.
            Assert.AreEqual(1, destination.GetValues("known1").Count(), "'known1' header value count.");
            Assert.AreEqual(parsedPrefix + "9", destination.GetValues("known1").First(), "'known1' header value.");

            // The header in source gets first parsed and then copied to destination. Note that here we have one
            // invalid value.
            Assert.AreEqual(3, destination.GetValues("known2").Count(), "'known2' header value count.");
            Assert.AreEqual(parsedPrefix + "5", destination.GetValues("known2").ElementAt(0), "'known2' header value[0].");
            Assert.AreEqual(parsedPrefix + "7", destination.GetValues("known2").ElementAt(1), "'known2' header value[1].");
            Assert.AreEqual(invalidHeaderValue, destination.GetValues("known2").ElementAt(2), "'known2' header value[2].");

            // Header 'known3' should not be copied, since it doesn't contain any values.
            Assert.IsFalse(destination.Contains("known3"), "'known3' header value count.");

            Assert.AreEqual(2, destination.GetValues("known4").Count(), "'known4' header value count.");
            Assert.AreEqual(known4Value1.ToString(), destination.GetValues("known4").ElementAt(0),
                "'known4' header value[0].");
            Assert.AreEqual(known4Value2.ToString(), destination.GetValues("known4").ElementAt(1),
                "'known4' header value[1].");

            Assert.AreEqual("source5", destination.GetValues("custom5").First(), "'custom5' header value.");

            Assert.AreEqual(string.Empty, destination.GetValues("custom6").First(), "'custom6' header value.");

            // Unlike 'known3', 'custom7' was added even though it only had empty values. The reason is that 'custom7'
            // is a custom header so we just add whatever value we get passed in.
            Assert.AreEqual(2, destination.GetValues("custom7").Count(), "'custom7' header value count.");
            Assert.AreEqual("", destination.GetValues("custom7").ElementAt(0), "'custom7' header value[0].");
            Assert.AreEqual("", destination.GetValues("custom7").ElementAt(1), "'custom7' header value[1].");
        }

        [TestMethod]
        public void AddHeaders_SourceHasEmptyHeaderStore_DestinationRemainsUnchanged()
        {
            Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
            parserStore.Add("known1", new MockHeaderParser());

            MockHeaders source = new MockHeaders(parserStore);

            MockHeaders destination = new MockHeaders(parserStore);
            destination.Add("known1", rawPrefix);

            destination.AddHeaders(source);

            Assert.AreEqual(1, destination.Count(), "Number of headers in destination.");
        }

        [TestMethod]
        public void AddHeaders_DestinationHasEmptyHeaderStore_DestinationHeaderStoreGetsCreatedAndValuesAdded()
        {
            Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
            parserStore.Add("known1", new MockHeaderParser());

            MockHeaders source = new MockHeaders(parserStore);
            source.Add("known1", rawPrefix);

            MockHeaders destination = new MockHeaders(parserStore);

            destination.AddHeaders(source);

            Assert.AreEqual(1, destination.Count(), "Number of headers in destination.");
        }

        [TestMethod]
        public void AddHeaders_SourceHasInvalidHeaderValues_InvalidHeadersRemovedFromSourceAndNotCopiedToDestination()
        {
            Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
            parserStore.Add("known", new MockHeaderParser());

            MockHeaders source = new MockHeaders(parserStore);
            source.AddWithoutValidation("known", invalidHeaderValue + "\r\ninvalid");
            source.AddWithoutValidation("custom", "invalid\r\nvalue");

            MockHeaders destination = new MockHeaders(parserStore);
            destination.AddHeaders(source);

            Assert.AreEqual(0, source.Count(), "No headers expected in source.");
            Assert.IsFalse(source.Contains("known"), "source contains 'known' header.");
            Assert.IsFalse(source.Contains("custom"), "source contains 'custom' header.");
            Assert.AreEqual(0, destination.Count(), "No headers expected in destination.");
            Assert.IsFalse(destination.Contains("known"), "destination contains 'known' header.");
            Assert.IsFalse(destination.Contains("custom"), "destination contains 'custom' header.");
        }

        #region Helper methods

        private class MockHeaders : HttpHeaders
        {
            private MockHeaderParser parser;

            public MockHeaderParser Parser
            {
                get { return parser; }
            }

            public MockHeaders(MockHeaderParser parser)
                : base()
            {
                Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
                this.parser = parser;
                parserStore.Add(knownHeader, this.parser);
                SetConfiguration(parserStore, new HashSet<string>());
            }

            public MockHeaders()
                : this(new MockHeaderParser())
            {
            }

            public MockHeaders(Dictionary<string, HttpHeaderParser> parserStore)
            {
                SetConfiguration(parserStore, new HashSet<string>());
            }
        }

        private class MockHeaderParser : HttpHeaderParser
        {
            public int TryParseValueCallCount { get; set; }
            public int EmptyValueCount { get; private set; }
            public MockComparer MockComparer { get; private set; }

            public MockHeaderParser()
                : this(true)
            {
            }

            public MockHeaderParser(bool supportsMultipleValues)
                : base(supportsMultipleValues)
            {
                this.MockComparer = new MockComparer();
            }

            public MockHeaderParser(string separator)
                : base(true, separator)
            {
                this.MockComparer = new MockComparer();
            }

            #region IHeaderParser Members

            public override IEqualityComparer Comparer
            {
                get { return MockComparer; }
            }

            public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
            {
                TryParseValueCallCount++;
                return TryParseValueCore(value, ref index, out parsedValue);
            }

            private bool TryParseValueCore(string value, ref int index, out object parsedValue)
            {
                parsedValue = null;

                if (value == null)
                {
                    parsedValue = null;
                    return true;
                }

                if (value == string.Empty)
                {
                    EmptyValueCount++;
                    parsedValue = null;
                    return true;
                }

                int separatorIndex = value.IndexOf(',', index);

                // Just fail if we don't support multiple values and the value is actually a list of values.
                if ((!SupportsMultipleValues) && (separatorIndex >= 0))
                {
                    return false;
                }

                if (separatorIndex == -1)
                {
                    // If the raw string just contains one value, then use the whole string.
                    separatorIndex = value.Length;
                }

                string tempValue = value.Substring(index, separatorIndex - index);

                if (tempValue.StartsWith(rawPrefix, StringComparison.Ordinal))
                {
                    index = Math.Min(separatorIndex + 1, value.Length);

                    // We "parse" the value by replacing 'rawPrefix' strings with 'parsedPrefix' string.
                    parsedValue = parsedPrefix + tempValue.Substring(rawPrefix.Length,
                        tempValue.Length - rawPrefix.Length);
                }
                else if (tempValue.StartsWith(invalidHeaderValue, StringComparison.Ordinal))
                {
                    return false;
                }
                else
                {
                    Assert.Fail("Unknown mock value: {0}", tempValue);
                }

                return true;
            }

            #endregion
        }

        private class MockComparer : IEqualityComparer
        {
            public int GetHashCodeCount { get; private set; }
            public int EqualsCount { get; private set; }

            #region IEqualityComparer Members

            public new bool Equals(object x, object y)
            {
                Assert.IsNotNull(x);
                Assert.IsNotNull(y);

                EqualsCount++;

                string xs = x as string;
                string ys = y as string;

                if ((xs != null) && (ys != null))
                {
                    return string.Compare(xs, ys, StringComparison.OrdinalIgnoreCase) == 0;
                }

                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                GetHashCodeCount++;
                return obj.GetHashCode();
            }

            #endregion
        }

        private class CustomTypeHeaders : HttpHeaders
        {
            public CustomTypeHeaders()
            {
            }

            public CustomTypeHeaders(string headerName, HttpHeaderParser parser)
            {
                Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
                parserStore.Add(headerName, parser);
                SetConfiguration(parserStore, new HashSet<string>());
            }
        }

        private class CustomTypeHeaderParser : HttpHeaderParser
        {
            private static CustomTypeComparer comparer = new CustomTypeComparer();

            public override IEqualityComparer Comparer
            {
                get { return comparer; }
            }

            public CustomTypeHeaderParser()
                : base(true)
            {
            }

            public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
            {
                throw new NotImplementedException();
            }
        }

        private class CustomTypeComparer : IEqualityComparer
        {
            #region IEqualityComparer Members

            public new bool Equals(object x, object y)
            {
                Assert.IsNotNull(x);
                Assert.IsNotNull(y);
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                Assert.IsNotNull(obj);
                return obj.GetHashCode();
            }

            #endregion
        }

        private class NoComparerHeaderParser : HttpHeaderParser
        {
            public NoComparerHeaderParser()
                : base(true)
            {
            }

            public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
            {
                throw new NotImplementedException();
            }
        }


        #endregion
    }
}

