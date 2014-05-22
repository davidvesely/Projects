using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Net.Test.Common;

namespace Microsoft.Net.Http.Test.Headers
{
    [TestClass]
    public class MediaTypeHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_MediaTypeNull_Throw()
        {
            new MediaTypeHeaderValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_MediaTypeEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            new MediaTypeHeaderValue(string.Empty);
        }

        [TestMethod]
        public void Ctor_MediaTypeInvalidFormat_ThrowFormatException()
        {
            // When adding values using strongly typed objects, no leading/trailing LWS (whitespaces) are allowed.
            AssertFormatException(" text/plain ");
            AssertFormatException("text / plain");
            AssertFormatException("text/ plain");
            AssertFormatException("text /plain");
            AssertFormatException("text/plain ");
            AssertFormatException(" text/plain");
            AssertFormatException("te xt/plain");
            AssertFormatException("te=xt/plain");
            AssertFormatException("teäxt/plain");
            AssertFormatException("text/pläin");
            AssertFormatException("text");
            AssertFormatException("\"text/plain\"");
            AssertFormatException("text/plain; charset=utf-8; ");
            AssertFormatException("text/plain;");
            AssertFormatException("text/plain;charset=utf-8"); // ctor takes only media-type name, no parameters
        }

        [TestMethod]
        public void Ctor_MediaTypeValidFormat_SuccessfullyCreated()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.MediaType, "MediaType");
            Assert.AreEqual(0, mediaType.Parameters.Count, "Parameters.Count");
            Assert.IsNull(mediaType.CharSet, "CharSet");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parameters_AddNull_Throw()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            mediaType.Parameters.Add(null);
        }

        [TestMethod]
        public void MediaType_SetAndGetMediaType_MatchExpectations()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.MediaType, "After ctor");

            mediaType.MediaType = "application/xml";
            Assert.AreEqual("application/xml", mediaType.MediaType, "After setting the property");
        }

        [TestMethod]
        public void CharSet_SetCharSetAndValidateObject_ParametersEntryForCharSetAdded()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            mediaType.CharSet = "mycharset";
            Assert.AreEqual("mycharset", mediaType.CharSet, "CharSet");
            Assert.AreEqual(1, mediaType.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("charset", mediaType.Parameters.First().Name, "Parameters.First().Name");

            mediaType.CharSet = null;
            Assert.IsNull(mediaType.CharSet, "CharSet");
            Assert.AreEqual(0, mediaType.Parameters.Count, "Parameters.Count");
            mediaType.CharSet = null; // It's OK to set it again to null; no exception.
        }

        [TestMethod]
        public void CharSet_AddCharSetParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            
            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue charset = new NameValueHeaderValue("CHARSET", "old_charset");
            mediaType.Parameters.Add(charset);
            Assert.AreEqual(1, mediaType.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("CHARSET", mediaType.Parameters.First().Name, "Parameters.First().Name");

            mediaType.CharSet = "new_charset";
            Assert.AreEqual("new_charset", mediaType.CharSet, "CharSet");
            Assert.AreEqual(1, mediaType.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("CHARSET", mediaType.Parameters.First().Name, "Parameters.First().Name");

            mediaType.Parameters.Remove(charset);
            Assert.IsNull(mediaType.CharSet, "CharSet");
        }

        [TestMethod]
        public void ToString_UseDifferentMediaTypes_AllSerializedCorrectly()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.ToString());

            mediaType.CharSet = "utf-8";
            Assert.AreEqual("text/plain; charset=utf-8", mediaType.ToString());

            mediaType.Parameters.Add(new NameValueHeaderValue("custom", "\"custom value\""));
            Assert.AreEqual("text/plain; charset=utf-8; custom=\"custom value\"", mediaType.ToString());

            mediaType.CharSet = null;
            Assert.AreEqual("text/plain; custom=\"custom value\"", mediaType.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseMediaTypeWithAndWithoutParameters_SameOrDifferentHashCodes()
        {
            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/plain");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("text/plain");
            mediaType2.CharSet = "utf-8";
            MediaTypeHeaderValue mediaType3 = new MediaTypeHeaderValue("text/plain");
            mediaType3.Parameters.Add(new NameValueHeaderValue("name", "value"));
            MediaTypeHeaderValue mediaType4 = new MediaTypeHeaderValue("TEXT/plain");
            MediaTypeHeaderValue mediaType5 = new MediaTypeHeaderValue("TEXT/plain");
            mediaType5.Parameters.Add(new NameValueHeaderValue("CHARSET", "UTF-8"));

            Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode(), "No params vs. charset.");
            Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType3.GetHashCode(), "No params vs. custom param.");
            Assert.AreNotEqual(mediaType2.GetHashCode(), mediaType3.GetHashCode(), "charset vs. custom param.");
            Assert.AreEqual(mediaType1.GetHashCode(), mediaType4.GetHashCode(), "Different casing.");
            Assert.AreEqual(mediaType2.GetHashCode(), mediaType5.GetHashCode(), "Different casing in charset.");
        }

        [TestMethod]
        public void Equals_UseMediaTypeWithAndWithoutParameters_EqualOrNotEqualNoExceptions()
        {
            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/plain");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("text/plain");
            mediaType2.CharSet = "utf-8";
            MediaTypeHeaderValue mediaType3 = new MediaTypeHeaderValue("text/plain");
            mediaType3.Parameters.Add(new NameValueHeaderValue("name", "value"));
            MediaTypeHeaderValue mediaType4 = new MediaTypeHeaderValue("TEXT/plain");
            MediaTypeHeaderValue mediaType5 = new MediaTypeHeaderValue("TEXT/plain");
            mediaType5.Parameters.Add(new NameValueHeaderValue("CHARSET", "UTF-8"));
            MediaTypeHeaderValue mediaType6 = new MediaTypeHeaderValue("TEXT/plain");
            mediaType6.Parameters.Add(new NameValueHeaderValue("CHARSET", "UTF-8"));
            mediaType6.Parameters.Add(new NameValueHeaderValue("custom", "value"));
            MediaTypeHeaderValue mediaType7 = new MediaTypeHeaderValue("text/other");

            Assert.IsFalse(mediaType1.Equals(mediaType2), "No params vs. charset.");
            Assert.IsFalse(mediaType2.Equals(mediaType1), "charset vs. no params.");
            Assert.IsFalse(mediaType1.Equals(null), "No params vs. <null>.");
            Assert.IsFalse(mediaType1.Equals(mediaType3), "No params vs. custom param.");
            Assert.IsFalse(mediaType2.Equals(mediaType3), "charset vs. custom param.");
            Assert.IsTrue(mediaType1.Equals(mediaType4), "Different casing.");
            Assert.IsTrue(mediaType2.Equals(mediaType5), "Different casing in charset.");
            Assert.IsFalse(mediaType5.Equals(mediaType6), "charset vs. custom param.");
            Assert.IsFalse(mediaType1.Equals(mediaType7), "text/plain vs. text/other.");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            MediaTypeHeaderValue source = new MediaTypeHeaderValue("application/xml");
            MediaTypeHeaderValue clone = (MediaTypeHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.MediaType, clone.MediaType, "MediaType");
            Assert.AreEqual(0, clone.Parameters.Count, "Parameters.Count");

            source.CharSet = "utf-8";
            clone = (MediaTypeHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.MediaType, clone.MediaType, "MediaType");
            Assert.AreEqual("utf-8", clone.CharSet, "CharSet");
            Assert.AreEqual(1, clone.Parameters.Count, "Parameters.Count");

            source.Parameters.Add(new NameValueHeaderValue("custom", "customValue"));
            clone = (MediaTypeHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.MediaType, clone.MediaType, "MediaType");
            Assert.AreEqual("utf-8", clone.CharSet, "CharSet");
            Assert.AreEqual(2, clone.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", clone.Parameters.ElementAt(1).Name, "Custom parameter name.");
            Assert.AreEqual("customValue", clone.Parameters.ElementAt(1).Value, "Custom parameter value.");
        }

        [TestMethod]
        public void GetMediaTypeLength_DifferentValidScenarios_AllReturnNonZero()
        {
            MediaTypeHeaderValue result = null;

            Assert.AreEqual(11, MediaTypeHeaderValue.GetMediaTypeLength("text/plain , other/charset", 0,
                DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual(0, result.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(10, MediaTypeHeaderValue.GetMediaTypeLength("text/plain", 0, DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual(0, result.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(30, MediaTypeHeaderValue.GetMediaTypeLength("text/plain; charset=iso-8859-1", 0,
                DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual("iso-8859-1", result.CharSet, "CharSet");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(38, MediaTypeHeaderValue.GetMediaTypeLength(" text/plain; custom=value;charset=utf-8",
                1, DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual("utf-8", result.CharSet, "CharSet");
            Assert.AreEqual(2, result.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(18, MediaTypeHeaderValue.GetMediaTypeLength(" text/plain; custom, next/mediatype",
                1, DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.IsNull(result.CharSet, "CharSet");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", result.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.IsNull(result.Parameters.ElementAt(0).Value, "Parameters[0].Value");

            Assert.AreEqual(48, MediaTypeHeaderValue.GetMediaTypeLength(
                "text / plain ; custom =\r\n \"x\" ; charset = utf-8 , next/mediatype", 0, DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual("utf-8", result.CharSet, "CharSet");
            Assert.AreEqual(2, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", result.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.AreEqual("\"x\"", result.Parameters.ElementAt(0).Value, "Parameters[0].Value");
            Assert.AreEqual("charset", result.Parameters.ElementAt(1).Name, "Parameters[1].Name");
            Assert.AreEqual("utf-8", result.Parameters.ElementAt(1).Value, "Parameters[1].Value");

            Assert.AreEqual(35, MediaTypeHeaderValue.GetMediaTypeLength(
                "text/plain;custom=\"x\";charset=utf-8,next/mediatype", 0, DummyCreator, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual("utf-8", result.CharSet, "CharSet");
            Assert.AreEqual(2, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", result.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.AreEqual("\"x\"", result.Parameters.ElementAt(0).Value, "Parameters[0].Value");
            Assert.AreEqual("charset", result.Parameters.ElementAt(1).Name, "Parameters[1].Name");
            Assert.AreEqual("utf-8", result.Parameters.ElementAt(1).Value, "Parameters[1].Value");
        }

        [TestMethod]
        public void GetMediaTypeLength_UseCustomCreator_CustomCreatorUsedToCreateMediaTypeInstance()
        {
            MediaTypeHeaderValue result = null;

            // Path: media-type only
            Assert.AreEqual(10, MediaTypeHeaderValue.GetMediaTypeLength("text/plain", 0,
                () => { return new MediaTypeWithQualityHeaderValue(); }, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual(0, result.Parameters.Count, "Parameters.Count");
            Assert.IsInstanceOfType(result, typeof(MediaTypeWithQualityHeaderValue), 
                "Expected custom creator delegate to be used.");

            // Path: media-type and parameters
            Assert.AreEqual(25, MediaTypeHeaderValue.GetMediaTypeLength("text/plain; charset=utf-8", 0,
                () => { return new MediaTypeWithQualityHeaderValue(); }, out result));
            Assert.AreEqual("text/plain", result.MediaType, "MediaType");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("utf-8", result.CharSet, "CharSet");
            Assert.IsInstanceOfType(result, typeof(MediaTypeWithQualityHeaderValue),
                "Expected custom creator delegate to be used.");
        }

        [TestMethod]
        public void GetMediaTypeLength_DifferentInvalidScenarios_AllReturnZero()
        {
            MediaTypeHeaderValue result = null;

            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength(" text/plain", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength("text/plain;", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength("text/plain;name=", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength("text/plain;name=value;", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength("text/plain;", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength(null, 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, MediaTypeHeaderValue.GetMediaTypeLength(string.Empty, 0, DummyCreator, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            MediaTypeHeaderValue expected = new MediaTypeHeaderValue("text/plain");
            CheckValidParse("\r\n text/plain  ", expected);
            CheckValidParse("text/plain", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // media-type parser.
            expected.CharSet = "utf-8";
            CheckValidParse("\r\n text   /  plain ;  charset =   utf-8 ", expected);
            CheckValidParse("  text/plain;charset=utf-8", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("");
            CheckInvalidParse("  ");
            CheckInvalidParse(null);
            CheckInvalidParse("text/plain会");
            CheckInvalidParse("text/plain ,");
            CheckInvalidParse("text/plain,");
            CheckInvalidParse("text/plain; charset=utf-8 ,");
            CheckInvalidParse("text/plain; charset=utf-8,");
            CheckInvalidParse("textplain");
            CheckInvalidParse("text/");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            MediaTypeHeaderValue expected = new MediaTypeHeaderValue("text/plain");
            CheckValidTryParse("\r\n text/plain  ", expected);
            CheckValidTryParse("text/plain", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // media-type parser.
            expected.CharSet = "utf-8";
            CheckValidTryParse("\r\n text   /  plain ;  charset =   utf-8 ", expected);
            CheckValidTryParse("  text/plain;charset=utf-8", expected);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("");
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse("text/plain会");
            CheckInvalidTryParse("text/plain ,");
            CheckInvalidTryParse("text/plain,");
            CheckInvalidTryParse("text/plain; charset=utf-8 ,");
            CheckInvalidTryParse("text/plain; charset=utf-8,");
            CheckInvalidTryParse("textplain");
            CheckInvalidTryParse("text/");
        }

        #region Helper methods

        private void CheckValidParse(string input, MediaTypeHeaderValue expectedResult)
        {
            MediaTypeHeaderValue result = MediaTypeHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => MediaTypeHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, MediaTypeHeaderValue expectedResult)
        {
            MediaTypeHeaderValue result = null;
            Assert.IsTrue(MediaTypeHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            MediaTypeHeaderValue result = null;
            Assert.IsFalse(MediaTypeHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }
        
        private static void AssertFormatException(string mediaType)
        {
            ExceptionAssert.ThrowsFormat(() => { new MediaTypeHeaderValue(mediaType); },
                "name: '" + (mediaType == null ? "<null>" : mediaType) + "'");
        }

        private static MediaTypeHeaderValue DummyCreator()
        {
            return new MediaTypeHeaderValue();
        }

        #endregion
    }
}
