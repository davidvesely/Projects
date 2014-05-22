using System.Linq;
using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class NameValueWithParametersHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NameNull_Throw()
        {
            NameValueWithParametersHeaderValue nameValue = new NameValueWithParametersHeaderValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NameEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            NameValueWithParametersHeaderValue nameValue = new NameValueWithParametersHeaderValue(string.Empty);
        }

        [TestMethod]
        public void Ctor_CallBaseCtor_Success()
        {
            // Just make sure the base ctor gets called correctly. Validation of input parameters is done in the base
            // class.
            NameValueWithParametersHeaderValue nameValue = new NameValueWithParametersHeaderValue("name");
            Assert.AreEqual("name", nameValue.Name, "Name");
            Assert.IsNull(nameValue.Value, "Value");

            nameValue = new NameValueWithParametersHeaderValue("name", "value");
            Assert.AreEqual("name", nameValue.Name, "Name");
            Assert.AreEqual("value", nameValue.Value, "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parameters_AddNull_Throw()
        {
            NameValueWithParametersHeaderValue nameValue = new NameValueWithParametersHeaderValue("name");
            nameValue.Parameters.Add(null);
        }

        [TestMethod]
        public void ToString_WithAndWithoutParameters_SerializedCorrectly()
        {
            NameValueWithParametersHeaderValue nameValue = new NameValueWithParametersHeaderValue("text", "token");
            Assert.AreEqual("text=token", nameValue.ToString());

            nameValue.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            nameValue.Parameters.Add(new NameValueHeaderValue("param2", "value2"));
            Assert.AreEqual("text=token; param1=value1; param2=value2", nameValue.ToString());
        }

        [TestMethod]
        public void GetHashCode_ValuesUseDifferentValues_HashDiffersAccordingToRfc()
        {
            NameValueWithParametersHeaderValue nameValue1 = new NameValueWithParametersHeaderValue("text");
            NameValueWithParametersHeaderValue nameValue2 = new NameValueWithParametersHeaderValue("text");

            // NameValueWithParametersHeaderValue just calls methods of the base class. Just verify Parameters is used.
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "No parameters.");

            nameValue1.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            nameValue2.Value = null;
            Assert.AreNotEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "none vs. 1 parameter.");

            nameValue2.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            Assert.AreEqual(nameValue1.GetHashCode(), nameValue2.GetHashCode(), "1 parameter vs. 1 parameter.");
        }

        [TestMethod]
        public void Equals_ValuesUseDifferentValues_ValuesAreEqualOrDifferentAccordingToRfc()
        {
            NameValueWithParametersHeaderValue nameValue1 = new NameValueWithParametersHeaderValue("text", "value");
            NameValueWithParametersHeaderValue nameValue2 = new NameValueWithParametersHeaderValue("text", "value");
            NameValueHeaderValue nameValue3 = new NameValueHeaderValue("text", "value");

            // NameValueWithParametersHeaderValue just calls methods of the base class. Just verify Parameters is used.
            Assert.IsTrue(nameValue1.Equals(nameValue2), "No parameters.");
            Assert.IsFalse(nameValue1.Equals(null), "Compare to null.");
            Assert.IsFalse(nameValue1.Equals(nameValue3), "Compare to base class instance.");

            nameValue1.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            Assert.IsFalse(nameValue1.Equals(nameValue2), "none vs. 1 parameter.");

            nameValue2.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            Assert.IsTrue(nameValue1.Equals(nameValue2), "1 parameter vs. 1 parameter.");
        }
        
        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            NameValueWithParametersHeaderValue source = new NameValueWithParametersHeaderValue("name", "value");
            source.Parameters.Add(new NameValueHeaderValue("param1", "value1"));
            NameValueWithParametersHeaderValue clone = (NameValueWithParametersHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.Name, clone.Name, "Name");
            Assert.AreEqual(source.Value, clone.Value, "Value");
            Assert.AreEqual(1, clone.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("param1", clone.Parameters.First().Name, "Parameters[0].Name");
            Assert.AreEqual("value1", clone.Parameters.First().Value, "Parameters[0].Value");
        }

        [TestMethod]
        public void GetNameValueLength_DifferentScenariosWithNoParameters_AllReturnNonZero()
        {
            NameValueWithParametersHeaderValue result = null;

            CallGetNameValueWithParametersLength("name=value", 0, 10, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");

            CallGetNameValueWithParametersLength(" name=value", 1, 10, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");

            CallGetNameValueWithParametersLength(" name", 1, 4, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.IsNull(result.Value, "result.Value");

            CallGetNameValueWithParametersLength("name=\"quoted str\"", 0, 17, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("\"quoted str\"", result.Value, "result.Value");

            CallGetNameValueWithParametersLength(" name=\"quoted str\"", 1, 17, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("\"quoted str\"", result.Value, "result.Value");

            CallGetNameValueWithParametersLength("name\t =va1ue\"", 0, 12, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("va1ue", result.Value, "result.Value");

            CallGetNameValueWithParametersLength(" name  ", 1, 6, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.IsNull(result.Value, "result.Value");
        }

        [TestMethod]
        public void GetNameValueLength_DifferentScenariosWithParameters_AllReturnNonZero()
        {
            NameValueWithParametersHeaderValue result = null;

            CallGetNameValueWithParametersLength(" name = value ; param1 = value1 ,", 1, 31, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("param1", result.Parameters.First().Name, "Parameters[0].Name");
            Assert.AreEqual("value1", result.Parameters.First().Value, "Parameters[0].Value");

            CallGetNameValueWithParametersLength(" name=value;param1=value1;param2=value2,next", 1, 38, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");
            Assert.AreEqual(2, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("param1", result.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.AreEqual("value1", result.Parameters.ElementAt(0).Value, "Parameters[0].Value");
            Assert.AreEqual("param2", result.Parameters.ElementAt(1).Name, "Parameters[1].Name");
            Assert.AreEqual("value2", result.Parameters.ElementAt(1).Value, "Parameters[1].Value");

            CallGetNameValueWithParametersLength(" name= value ;   param1 , next", 1, 23, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.AreEqual("value", result.Value, "result.Value");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("param1", result.Parameters.First().Name, "Parameters[0].Name");
            Assert.IsNull(result.Parameters.First().Value, "Parameters[0].Value");

            CallGetNameValueWithParametersLength(" name ;   param1 , next", 1, 16, out result);
            Assert.AreEqual("name", result.Name, "result.Name");
            Assert.IsNull(result.Value, "result.Value");
            Assert.AreEqual(1, result.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("param1", result.Parameters.First().Name, "Parameters[0].Name");
            Assert.IsNull(result.Parameters.First().Value, "Parameters[0].Value");
        }

        [TestMethod]
        public void GetNameValueLength_DifferentInvalidScenarios_AllReturnZero()
        {
            CheckInvalidGetNameValueWithParametersLength(" name=value", 0);
            CheckInvalidGetNameValueWithParametersLength(" name=", 1);
            CheckInvalidGetNameValueWithParametersLength(" name=value; param;", 1);
            CheckInvalidGetNameValueWithParametersLength(" name;", 1);
            CheckInvalidGetNameValueWithParametersLength(" ,name", 1);
            CheckInvalidGetNameValueWithParametersLength("name=value", 10);
            CheckInvalidGetNameValueWithParametersLength("", 0);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            NameValueWithParametersHeaderValue expected = new NameValueWithParametersHeaderValue("custom");
            CheckValidParse("\r\n custom  ", expected);
            CheckValidParse("custom", expected);
            
            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a
            // transfer-coding parser.
            expected.Parameters.Add(new NameValueHeaderValue("name", "value"));
            CheckValidParse("\r\n custom ;  name =   value ", expected);
            CheckValidParse("  custom;name=value",  expected);
            CheckValidParse("  custom ; name=value", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("custom会");
            CheckInvalidParse("custom; name=value;");
            CheckInvalidParse("custom; name1=value1; name2=value2;");
            CheckInvalidParse(null);
            CheckInvalidParse(string.Empty);
            CheckInvalidParse("  ");
            CheckInvalidParse("  ,,");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            NameValueWithParametersHeaderValue expected = new NameValueWithParametersHeaderValue("custom");
            CheckValidTryParse("\r\n custom  ", expected);
            CheckValidTryParse("custom", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a
            // transfer-coding parser.
            expected.Parameters.Add(new NameValueHeaderValue("name", "value"));
            CheckValidTryParse("\r\n custom ;  name =   value ", expected);
            CheckValidTryParse("  custom;name=value", expected);
            CheckValidTryParse("  custom ; name=value", expected);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("custom会");
            CheckInvalidTryParse("custom; name=value;");
            CheckInvalidTryParse("custom; name1=value1; name2=value2;");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse(string.Empty);
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse("  ,,");
        }

        #region Helper methods

        private void CheckValidParse(string input, NameValueWithParametersHeaderValue expectedResult)
        {
            NameValueWithParametersHeaderValue result = NameValueWithParametersHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => NameValueWithParametersHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, NameValueWithParametersHeaderValue expectedResult)
        {
            NameValueWithParametersHeaderValue result = null;
            Assert.IsTrue(NameValueWithParametersHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            NameValueWithParametersHeaderValue result = null;
            Assert.IsFalse(NameValueWithParametersHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        private static void CallGetNameValueWithParametersLength(string input, int startIndex, int expectedLength,
            out NameValueWithParametersHeaderValue result)
        {
            object temp = null;
            Assert.AreEqual(expectedLength, NameValueWithParametersHeaderValue.GetNameValueWithParametersLength(input, 
                startIndex, out temp), "Input: '{0}', Start index: {1}", input, startIndex);
            result = temp as NameValueWithParametersHeaderValue;
        }

        private static void CheckInvalidGetNameValueWithParametersLength(string input, int startIndex)
        {
            object result = null;
            Assert.AreEqual(0, NameValueWithParametersHeaderValue.GetNameValueWithParametersLength(input, startIndex,
                out result), "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        #endregion
    }
}
