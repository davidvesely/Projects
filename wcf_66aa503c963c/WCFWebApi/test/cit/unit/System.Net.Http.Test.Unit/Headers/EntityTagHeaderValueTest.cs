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
    public class EntityTagHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_ETagNull_Throw()
        {
            new EntityTagHeaderValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_ETagEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            new EntityTagHeaderValue(string.Empty);
        }

        [TestMethod]
        public void Ctor_ETagInvalidFormat_ThrowFormatException()
        {
            // When adding values using strongly typed objects, no leading/trailing LWS (whitespaces) are allowed.
            AssertFormatException("tag");
            AssertFormatException("*");
            AssertFormatException(" tag ");
            AssertFormatException("\"tag\" invalid");
            AssertFormatException("\"tag");
            AssertFormatException("tag\"");
            AssertFormatException("\"tag\"\"");
            AssertFormatException("\"\"tag\"\"");
            AssertFormatException("\"\"tag\"");
            AssertFormatException("W/\"tag\""); // tag value must not contain 'W/'
        }

        [TestMethod]
        public void Ctor_ETagValidFormat_SuccessfullyCreated()
        {
            EntityTagHeaderValue etag = new EntityTagHeaderValue("\"tag\"");
            Assert.AreEqual("\"tag\"", etag.Tag, "Tag");
            Assert.IsFalse(etag.IsWeak, "IsWeak");
        }

        [TestMethod]
        public void Ctor_ETagValidFormatAndIsWeak_SuccessfullyCreated()
        {
            EntityTagHeaderValue etag = new EntityTagHeaderValue("\"e tag\"", true);
            Assert.AreEqual("\"e tag\"", etag.Tag, "Tag");
            Assert.IsTrue(etag.IsWeak, "IsWeak");
        }

        [TestMethod]
        public void ToString_UseDifferentETags_AllSerializedCorrectly()
        {
            EntityTagHeaderValue etag = new EntityTagHeaderValue("\"e tag\"");
            Assert.AreEqual("\"e tag\"", etag.ToString());

            etag = new EntityTagHeaderValue("\"e tag\"", true);
            Assert.AreEqual("W/\"e tag\"", etag.ToString());

            etag = new EntityTagHeaderValue("\"\"", false);
            Assert.AreEqual("\"\"", etag.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseSameAndDifferentETags_SameOrDifferentHashCodes()
        {
            EntityTagHeaderValue etag1 = new EntityTagHeaderValue("\"tag\"");
            EntityTagHeaderValue etag2 = new EntityTagHeaderValue("\"TAG\"");
            EntityTagHeaderValue etag3 = new EntityTagHeaderValue("\"tag\"", true);
            EntityTagHeaderValue etag4 = new EntityTagHeaderValue("\"tag1\"");
            EntityTagHeaderValue etag5 = new EntityTagHeaderValue("\"tag\"");
            EntityTagHeaderValue etag6 = EntityTagHeaderValue.Any;

            Assert.AreNotEqual(etag1.GetHashCode(), etag2.GetHashCode(), "Different casing.");
            Assert.AreNotEqual(etag1.GetHashCode(), etag3.GetHashCode(), "strong vs. weak.");
            Assert.AreNotEqual(etag1.GetHashCode(), etag4.GetHashCode(), "tag vs. tag1.");
            Assert.AreNotEqual(etag1.GetHashCode(), etag6.GetHashCode(), "tag vs. *.");
            Assert.AreEqual(etag1.GetHashCode(), etag5.GetHashCode(), "tag vs. tag.");
        }

        [TestMethod]
        public void Equals_UseSameAndDifferentETags_EqualOrNotEqualNoExceptions()
        {
            EntityTagHeaderValue etag1 = new EntityTagHeaderValue("\"tag\"");
            EntityTagHeaderValue etag2 = new EntityTagHeaderValue("\"TAG\"");
            EntityTagHeaderValue etag3 = new EntityTagHeaderValue("\"tag\"", true);
            EntityTagHeaderValue etag4 = new EntityTagHeaderValue("\"tag1\"");
            EntityTagHeaderValue etag5 = new EntityTagHeaderValue("\"tag\"");
            EntityTagHeaderValue etag6 = EntityTagHeaderValue.Any;

            Assert.IsFalse(etag1.Equals(etag2), "Different casing.");
            Assert.IsFalse(etag2.Equals(etag1), "Different casing.");
            Assert.IsFalse(etag1.Equals(null), "tag vs. <null>.");
            Assert.IsFalse(etag1.Equals(etag3), "strong vs. weak.");
            Assert.IsFalse(etag3.Equals(etag1), "weak vs. strong.");
            Assert.IsFalse(etag1.Equals(etag4), "tag vs. tag1.");
            Assert.IsFalse(etag1.Equals(etag6), "tag vs. *.");
            Assert.IsTrue(etag1.Equals(etag5), "tag vs. tag..");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            EntityTagHeaderValue source = new EntityTagHeaderValue("\"tag\"");
            EntityTagHeaderValue clone = (EntityTagHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Tag, clone.Tag, "Tag");
            Assert.AreEqual(source.IsWeak, clone.IsWeak, "IsWeak");

            source = new EntityTagHeaderValue("\"tag\"", true);
            clone = (EntityTagHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Tag, clone.Tag, "Tag");
            Assert.AreEqual(source.IsWeak, clone.IsWeak, "IsWeak");

            Assert.AreSame(EntityTagHeaderValue.Any, ((ICloneable)EntityTagHeaderValue.Any).Clone(), 
                "'Any' entity tag must not be cloned.");
        }

        [TestMethod]
        public void GetEntityTagLength_DifferentValidScenarios_AllReturnNonZero()
        {
            EntityTagHeaderValue result = null;

            Assert.AreEqual(6, EntityTagHeaderValue.GetEntityTagLength("\"ta会g\"", 0, out result), "\"ta会g\"");
            Assert.AreEqual("\"ta会g\"", result.Tag, "etag");
            Assert.IsFalse(result.IsWeak, "IsWeak");

            Assert.AreEqual(9, EntityTagHeaderValue.GetEntityTagLength("W/\"tag\"  ", 0, out result), "W/\"tag\"  ");
            Assert.AreEqual("\"tag\"", result.Tag, "etag");
            Assert.IsTrue(result.IsWeak, "IsWeak");

            // Note that even if after a valid tag & whitespaces there are invalid characters, GetEntityTagLength()
            // will return the length of the valid tag and ignore the invalid characters at the end. It is the callers
            // responsibility to consider the whole string invalid if after a valid ETag there are invalid chars.
            Assert.AreEqual(11, EntityTagHeaderValue.GetEntityTagLength("\"tag\"  \r\n  !!", 0, out result),
                "\"tag\"  \r\n  !!");
            Assert.AreEqual("\"tag\"", result.Tag, "etag");
            Assert.IsFalse(result.IsWeak, "IsWeak");

            Assert.AreEqual(7, EntityTagHeaderValue.GetEntityTagLength("\"W/tag\"", 0, out result), "\"W/tag\"");
            Assert.AreEqual("\"W/tag\"", result.Tag, "etag");
            Assert.IsFalse(result.IsWeak, "IsWeak");

            Assert.AreEqual(9, EntityTagHeaderValue.GetEntityTagLength("W/  \"tag\"", 0, out result), "W/  \"tag\"");
            Assert.AreEqual("\"tag\"", result.Tag, "etag");
            Assert.IsTrue(result.IsWeak, "IsWeak");

            // We also accept lower-case 'w': e.g. 'w/"tag"' rather than 'W/"tag"'
            Assert.AreEqual(4, EntityTagHeaderValue.GetEntityTagLength("w/\"\"", 0, out result), "w/\"\"");
            Assert.AreEqual("\"\"", result.Tag, "etag");
            Assert.IsTrue(result.IsWeak, "IsWeak");

            Assert.AreEqual(2, EntityTagHeaderValue.GetEntityTagLength("\"\"", 0, out result), "\"\"");
            Assert.AreEqual("\"\"", result.Tag, "etag");
            Assert.IsFalse(result.IsWeak, "IsWeak");

            Assert.AreEqual(2, EntityTagHeaderValue.GetEntityTagLength(",* , ", 1, out result), ",* , ");
            Assert.AreSame(EntityTagHeaderValue.Any, result, "Expected 'Any' to be returned.");
        }

        [TestMethod]
        public void GetEntityTagLength_DifferentInvalidScenarios_AllReturnZero()
        {
            EntityTagHeaderValue result = null;

            // no leading spaces allowed.
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength(" \"tag\"", 0, out result), " \"tag\""); 
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("\"tag", 0, out result), "\"tag");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("tag\"", 0, out result), "tag\"");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("a/\"tag\"", 0, out result), "a/\"tag\"");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("W//\"tag\"", 0, out result), "W//\"tag\"");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("W", 0, out result), "W");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("W/", 0, out result), "W/");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength("W/\"", 0, out result), "W/\"");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength(null, 0, out result), "<null>");
            Assert.IsNull(result);
            Assert.AreEqual(0, EntityTagHeaderValue.GetEntityTagLength(string.Empty, 0, out result), "String.Empty");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("\"tag\"", new EntityTagHeaderValue("\"tag\""));
            CheckValidParse(" \"tag\" ", new EntityTagHeaderValue("\"tag\""));
            CheckValidParse("\r\n \"tag\"\r\n ", new EntityTagHeaderValue("\"tag\""));
            CheckValidParse("\"tag\"", new EntityTagHeaderValue("\"tag\""));
            CheckValidParse("\"tag会\"", new EntityTagHeaderValue("\"tag会\""));
            CheckValidParse("W/\"tag\"", new EntityTagHeaderValue("\"tag\"", true));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  !");
            CheckInvalidParse("tag\"  !");
            CheckInvalidParse("!\"tag\"");
            CheckInvalidParse("\"tag\",");
            CheckInvalidParse("\"tag\" \"tag2\"");
            CheckInvalidParse("/\"tag\"");
            CheckInvalidParse("*"); // "any" is not allowed as ETag value.
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("\"tag\"", new EntityTagHeaderValue("\"tag\""));
            CheckValidTryParse(" \"tag\" ", new EntityTagHeaderValue("\"tag\""));
            CheckValidTryParse("\r\n \"tag\"\r\n ", new EntityTagHeaderValue("\"tag\""));
            CheckValidTryParse("\"tag\"", new EntityTagHeaderValue("\"tag\""));
            CheckValidTryParse("\"tag会\"", new EntityTagHeaderValue("\"tag会\""));
            CheckValidTryParse("W/\"tag\"", new EntityTagHeaderValue("\"tag\"", true));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  !");
            CheckInvalidTryParse("tag\"  !");
            CheckInvalidTryParse("!\"tag\"");
            CheckInvalidTryParse("\"tag\",");
            CheckInvalidTryParse("\"tag\" \"tag2\"");
            CheckInvalidTryParse("/\"tag\"");
            CheckInvalidTryParse("*"); // "any" is not allowed as ETag value.
        }

        #region Helper methods

        private void CheckValidParse(string input, EntityTagHeaderValue expectedResult)
        {
            EntityTagHeaderValue result = EntityTagHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => EntityTagHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, EntityTagHeaderValue expectedResult)
        {
            EntityTagHeaderValue result = null;
            Assert.IsTrue(EntityTagHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            EntityTagHeaderValue result = null;
            Assert.IsFalse(EntityTagHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void AssertFormatException(string tag)
        {
            ExceptionAssert.ThrowsFormat(() => { new EntityTagHeaderValue(tag); },
                "name: '" + (tag == null ? "<null>" : tag) + "'");
        }

        #endregion
    }
}
