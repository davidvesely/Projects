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
    public class NameValueHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NameNull_Throw()
        {
            NameValueHeaderValue nameValue = new NameValueHeaderValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NameEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            NameValueHeaderValue nameValue = new NameValueHeaderValue(string.Empty);
        }

        [TestMethod]
        public void Ctor_NameInvalidFormat_ThrowFormatException()
        {
            // When adding values using strongly typed objects, no leading/trailing LWS (whitespaces) are allowed.
            AssertFormatException(" text ", null);
            AssertFormatException("text ", null);
            AssertFormatException(" text", null);
            AssertFormatException("te xt", null);
            AssertFormatException("te=xt", null); // The ctor takes a name which must not contain '='.
            AssertFormatException("teäxt", null);
        }

        [TestMethod]
        public void Ctor_NameValidFormat_SuccessfullyCreated()
        {
            NameValueHeaderValue nameValue = new NameValueHeaderValue("text", null);
            Assert.AreEqual("text", nameValue.Name);            
        }

        [TestMethod]
        public void Ctor_ValueInvalidFormat_ThrowFormatException()
        {
            // When adding values using strongly typed objects, no leading/trailing LWS (whitespaces) are allowed.
            AssertFormatException("text", " token ");
            AssertFormatException("text", "token ");
            AssertFormatException("text", " token");
            AssertFormatException("text", "token string");
            AssertFormatException("text", "\"quoted string with \" quotes\"");
            AssertFormatException("text", "\"quoted string with \"two\" quotes\"");
        }

        [TestMethod]
        public void Ctor_ValueValidFormat_SuccessfullyCreated()
        {
            CheckValue(null);
            CheckValue(string.Empty);
            CheckValue("token_string");
            CheckValue("\"quoted string\"");
            CheckValue("\"quoted string with quoted \\\" quote-pair\"");
        }

        [TestMethod]
        public void Value_CallSetterWithInvalidValues_Throw()
        {
            // Just verify that the setter calls the same validation the ctor invokes.
            ExceptionAssert.ThrowsFormat(() => { var x = new NameValueHeaderValue("name"); x.Value = " x "; }, " x ");
            ExceptionAssert.ThrowsFormat(() => { var x = new NameValueHeaderValue("name"); x.Value = "x y"; }, "x y");
        }

        [TestMethod]
        public void ToString_UseNoValueAndTokenAndQuotedStringValues_SerializedCorrectly()
        {
            NameValueHeaderValue nameValue = new NameValueHeaderValue("text", "token");
            Assert.AreEqual("text=token", nameValue.ToString());
            
            nameValue.Value = "\"quoted string\"";
            Assert.AreEqual("text=\"quoted string\"", nameValue.ToString());

            nameValue.Value = null;
            Assert.AreEqual("text", nameValue.ToString(), "Null value");

            nameValue.Value = string.Empty;
            Assert.AreEqual("text", nameValue.ToString(), "Empty string value");
        }

        [TestMethod]
        public void GetHashCode_ValuesUseDifferentValues_HashDiffersAccordingToRfc()
        {
            NameValueHeaderValue nameValue1 = new NameValueHeaderValue("text");
            NameValueHeaderValue nameValue2 = new NameValueHeaderValue("text");

            nameValue1.Value = null;
            nameValue2.Value = null;
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "<null> vs. <null>.");

            nameValue1.Value = "token";
            nameValue2.Value = null;
            Assert.AreNotEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "token vs. <null>.");

            nameValue1.Value = "token";
            nameValue2.Value = string.Empty;
            Assert.AreNotEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "token vs. string.Empty.");

            nameValue1.Value = null;
            nameValue2.Value = string.Empty;
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "<null> vs. string.Empty.");

            nameValue1.Value = "token";
            nameValue2.Value = "TOKEN";
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "token vs. TOKEN.");

            nameValue1.Value = "token";
            nameValue2.Value = "token";
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "token vs. token.");

            nameValue1.Value = "\"quoted string\"";
            nameValue2.Value = "\"QUOTED STRING\"";
            Assert.AreNotEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), 
                "\"quoted string\" vs. \"QUOTED STRING\".");

            nameValue1.Value = "\"quoted string\"";
            nameValue2.Value = "\"quoted string\"";
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(),
                "\"quoted string\" vs. \"quoted string\".");
        }

        [TestMethod]
        public void GetHashCode_NameUseDifferentCasing_HashDiffersAccordingToRfc()
        {
            NameValueHeaderValue nameValue1 = new NameValueHeaderValue("text");
            NameValueHeaderValue nameValue2 = new NameValueHeaderValue("TEXT");
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "text vs. TEXT.");
        }

        [TestMethod]
        public void Equals_ValuesUseDifferentValues_ValuesAreEqualOrDifferentAccordingToRfc()
        {
            NameValueHeaderValue nameValue1 = new NameValueHeaderValue("text");
            NameValueHeaderValue nameValue2 = new NameValueHeaderValue("text");

            nameValue1.Value = null;
            nameValue2.Value = null;
            Assert.IsTrue(nameValue1.Equals(nameValue2), "<null> vs. <null>.");

            nameValue1.Value = "token";
            nameValue2.Value = null;
            Assert.IsFalse(nameValue1.Equals(nameValue2), "token vs. <null>.");

            nameValue1.Value = null;
            nameValue2.Value = "token";
            Assert.IsFalse(nameValue1.Equals(nameValue2), "<null> vs. token.");

            nameValue1.Value = string.Empty;
            nameValue2.Value = "token";
            Assert.IsFalse(nameValue1.Equals(nameValue2), "string.Empty vs. token.");

            nameValue1.Value = null;
            nameValue2.Value = string.Empty;
            Assert.IsTrue(nameValue1.Equals(nameValue2), "<null> vs. string.Empty.");

            nameValue1.Value = "token";
            nameValue2.Value = "TOKEN";
            Assert.IsTrue(nameValue1.Equals(nameValue2), "token vs. TOKEN.");

            nameValue1.Value = "token";
            nameValue2.Value = "token";
            Assert.IsTrue(nameValue1.Equals(nameValue2), "token vs. token.");

            nameValue1.Value = "\"quoted string\"";
            nameValue2.Value = "\"QUOTED STRING\"";
            Assert.IsFalse(nameValue1.Equals(nameValue2), "\"quoted string\" vs. \"QUOTED STRING\".");

            nameValue1.Value = "\"quoted string\"";
            nameValue2.Value = "\"quoted string\"";
            Assert.IsTrue(nameValue1.Equals(nameValue2), "\"quoted string\" vs. \"quoted string\".");
                        
            Assert.IsFalse(nameValue1.Equals(null), "\"quoted string\" vs. <null>.");
        }

        [TestMethod]
        public void Equals_NameUseDifferentCasing_ConsideredEqual()
        {
            NameValueHeaderValue nameValue1 = new NameValueHeaderValue("text");
            NameValueHeaderValue nameValue2 = new NameValueHeaderValue("TEXT");
            Assert.IsTrue(nameValue1.Equals(nameValue2), "text vs. TEXT.");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            NameValueHeaderValue source = new NameValueHeaderValue("name", "value");
            NameValueHeaderValue clone = (NameValueHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Name, clone.Name, "Name");
            Assert.AreEqual(source.Value, clone.Value, "Value");
        }

        [TestMethod]
        public void GetNameValueLength_DifferentValidScenarios_AllReturnNonZero()
        {
            NameValueHeaderValue result = null;

            Assert.AreEqual(10, NameValueHeaderValue.GetNameValueLength("name=value", 0, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");

            Assert.AreEqual(10, NameValueHeaderValue.GetNameValueLength(" name=value", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");

            Assert.AreEqual(4, NameValueHeaderValue.GetNameValueLength(" name", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.IsNull(result.Value, "result.Value");

            Assert.AreEqual(17, NameValueHeaderValue.GetNameValueLength("name=\"quoted str\"", 0, DummyCreator, 
                out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("\"quoted str\"", result.Value, "result.Value");

            Assert.AreEqual(17, NameValueHeaderValue.GetNameValueLength(" name=\"quoted str\"", 1, DummyCreator, 
                out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("\"quoted str\"", result.Value, "result.Value");

            Assert.AreEqual(12, NameValueHeaderValue.GetNameValueLength("name\t =va1ue\"", 0, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("va1ue", result.Value, "result.Value");

            Assert.AreEqual(12, NameValueHeaderValue.GetNameValueLength(" name= va*ue ", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("va*ue", result.Value, "result.Value");

            Assert.AreEqual(6, NameValueHeaderValue.GetNameValueLength(" name  ", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.IsNull(result.Value, "result.Value");

            Assert.AreEqual(12, NameValueHeaderValue.GetNameValueLength(" name= va*ue ,", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("va*ue", result.Value, "result.Value");

            Assert.AreEqual(9, NameValueHeaderValue.GetNameValueLength(" name = va:ue", 1, DummyCreator, out result));
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("va", result.Value, "result.Value");
        }

        [TestMethod]
        public void GetNameValueLength_DifferentInvalidScenarios_AllReturnZero()
        {
            NameValueHeaderValue result = null;

            Assert.AreEqual(0, NameValueHeaderValue.GetNameValueLength(" name=value", 0, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, NameValueHeaderValue.GetNameValueLength(" name=", 1, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, NameValueHeaderValue.GetNameValueLength(" ,", 1, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, NameValueHeaderValue.GetNameValueLength("name=value", 10, DummyCreator, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, NameValueHeaderValue.GetNameValueLength("", 0, DummyCreator, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidParse("  name = value    ", new NameValueHeaderValue("name", "value"));
            CheckValidParse(" name", new NameValueHeaderValue("name"));
            CheckValidParse(" name=\"value\"", new NameValueHeaderValue("name", "\"value\""));
            CheckValidParse("name=value", new NameValueHeaderValue("name", "value"));
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("name[value");
            CheckInvalidParse("name=value=");
            CheckInvalidParse("name=会");
            CheckInvalidParse("name==value");
            CheckInvalidParse("=value");
            CheckInvalidParse("name value");
            CheckInvalidParse("name=,value");
            CheckInvalidParse("会");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
            CheckInvalidParse(" , , name = value  ,  ");
            CheckInvalidParse(" name,");
            CheckInvalidParse(" ,name=\"value\"");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            CheckValidTryParse("  name = value    ", new NameValueHeaderValue("name", "value"));
            CheckValidTryParse(" name", new NameValueHeaderValue("name"));
            CheckValidTryParse(" name=\"value\"", new NameValueHeaderValue("name", "\"value\""));
            CheckValidTryParse("name=value", new NameValueHeaderValue("name", "value"));
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("name[value");
            CheckInvalidTryParse("name=value=");
            CheckInvalidTryParse("name=会");
            CheckInvalidTryParse("name==value");
            CheckInvalidTryParse("=value");
            CheckInvalidTryParse("name value");
            CheckInvalidTryParse("name=,value");
            CheckInvalidTryParse("会");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
            CheckInvalidTryParse(" , , name = value  ,  ");
            CheckInvalidTryParse(" name,");
            CheckInvalidTryParse(" ,name=\"value\"");
        }

        #region Helper methods

        private void CheckValidParse(string input, NameValueHeaderValue expectedResult)
        {
            NameValueHeaderValue result = NameValueHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => NameValueHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, NameValueHeaderValue expectedResult)
        {
            NameValueHeaderValue result = null;
            Assert.IsTrue(NameValueHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            NameValueHeaderValue result = null;
            Assert.IsFalse(NameValueHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CheckValue(string value)
        {
            NameValueHeaderValue nameValue = new NameValueHeaderValue("text", value);
            Assert.AreEqual(value, nameValue.Value);
        }

        private static void AssertFormatException(string name, string value)
        {
            ExceptionAssert.ThrowsFormat(() => { new NameValueHeaderValue(name, value); }, 
                "name: '" + (name == null ? "<null>" : name) + "', value: '" +
                (value == null ? "<null>" : value) + "'");
        }

        private static NameValueHeaderValue DummyCreator()
        {
            return new NameValueHeaderValue();
        }

        #endregion
    }
}
