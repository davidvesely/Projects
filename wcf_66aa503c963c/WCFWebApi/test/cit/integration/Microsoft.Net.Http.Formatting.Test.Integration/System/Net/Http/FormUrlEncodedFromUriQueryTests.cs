// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Json;
    using System.Text;
    using Microsoft.Server.Common;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FormUrlEncodedJsonFromUriQueryTests
    {
        #region Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void SimpleStringsTest()
        {
            ValidateFormUrlEncoded("abc", "{\"abc\":null}");
            ValidateFormUrlEncoded("%2eabc%2e", "{\".abc.\":null}");
            ValidateFormUrlEncoded("", "{}");
            ValidateFormUrlEncoded("a=1", "{\"a\":\"1\"}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void SimpleObjectsTest()
        {
            ValidateFormUrlEncoded("a=2", "{\"a\":\"2\"}");
            ValidateFormUrlEncoded("b=true", "{\"b\":\"true\"}");
            ValidateFormUrlEncoded("c=hello", "{\"c\":\"hello\"}");
            ValidateFormUrlEncoded("d=", "{\"d\":\"\"}");
            ValidateFormUrlEncoded("e=null", "{\"e\":null}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void LegacyArraysTest()
        {
            ValidateFormUrlEncoded("a=1&a=hello&a=333", "{\"a\":[\"1\",\"hello\",\"333\"]}");

            // Only valid in shallow serialization
            ParseInvalidFormUrlEncoded("a[z]=2&a[z]=3");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void ArraysTest()
        {
            ValidateFormUrlEncoded("a[]=1&a[]=hello&a[]=333", "{\"a\":[\"1\",\"hello\",\"333\"]}");
            ValidateFormUrlEncoded("a[b][]=1&a[b][]=hello&a[b][]=333", "{\"a\":{\"b\":[\"1\",\"hello\",\"333\"]}}");
            ValidateFormUrlEncoded("a[]=", "{\"a\":[\"\"]}");
            ValidateFormUrlEncoded("a%5B%5D=2", @"{""a"":[""2""]}");
            ValidateFormUrlEncoded("a[x][0]=1&a[x][]=2", @"{""a"":{""x"":[""1"",""2""]}}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void MultidimensionalArraysTest()
        {
            ValidateFormUrlEncoded("a[0][]=1&a[0][]=hello&a[1][]=333", "{\"a\":[[\"1\",\"hello\"],[\"333\"]]}");
            ValidateFormUrlEncoded("a[b][0][]=1&a[b][1][]=hello&a[b][1][]=333", "{\"a\":{\"b\":[[\"1\"],[\"hello\",\"333\"]]}}");
            ValidateFormUrlEncoded("a[0][0][0][]=1", "{\"a\":[[[[\"1\"]]]]}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void SparseArraysTest()
        {
            ValidateFormUrlEncoded("a[0][]=hello&a[2][]=333", "{\"a\":{\"0\":[\"hello\"],\"2\":[\"333\"]}}");
            ValidateFormUrlEncoded("a[0]=hello", "{\"a\":[\"hello\"]}");
            ValidateFormUrlEncoded("a[1][]=hello", "{\"a\":{\"1\":[\"hello\"]}}");
            ValidateFormUrlEncoded("a[1][0]=hello", "{\"a\":{\"1\":[\"hello\"]}}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void ArraysWithMixedMembers()
        {
            ValidateFormUrlEncoded("b[]=2&b[1][c]=d", "{\"b\":[\"2\",{\"c\":\"d\"}]}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void EmptyKeyTest()
        {
            ValidateFormUrlEncoded("=3", "{\"\":\"3\"}");
            ValidateFormUrlEncoded("a=1&=3", "{\"a\":\"1\",\"\":\"3\"}");
            ValidateFormUrlEncoded("=3&b=2", "{\"\":\"3\",\"b\":\"2\"}");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void InvalidObjectGraphsTest()
        {
            ParseInvalidFormUrlEncoded("a[b]=1&a=2");
            ParseInvalidFormUrlEncoded("a[b]=1&a[b][]=2");
            ParseInvalidFormUrlEncoded("a[x][]=1&a[x][0]=2");
            ParseInvalidFormUrlEncoded("a=2&a[b]=1");
            ParseInvalidFormUrlEncoded("[]=1");
            ParseInvalidFormUrlEncoded("a[][]=0");
            ParseInvalidFormUrlEncoded("a[][x]=0");
            ParseInvalidFormUrlEncoded("a&a[b]=1");
            ParseInvalidFormUrlEncoded("a&a=1");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void InvalidFormUrlEncodingTest()
        {
            ParseInvalidFormUrlEncoded("a[b=2");
            ParseInvalidFormUrlEncoded("a[[b]=2");
            ParseInvalidFormUrlEncoded("a[b]]=2");
        }

        /// <summary>
        /// Tests for parsing form-urlencoded data originated from JS primitives.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestJsonPrimitive()
        {
            ValidateFormUrlEncoded("abc", @"{""abc"":null}");
            ValidateFormUrlEncoded("123", @"{""123"":null}");
            ValidateFormUrlEncoded("true", @"{""true"":null}");
            ValidateFormUrlEncoded("", "{}");
            ValidateFormUrlEncoded("%2fabc%2f", @"{""\/abc\/"":null}");
        }

        /// <summary>
        /// Negative tests for parsing form-urlencoded data originated from JS primitives.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestJsonPrimitiveNegative()
        {
            ParseInvalidFormUrlEncoded("a[b]=1&a=2");
            ParseInvalidFormUrlEncoded("a=2&a[b]=1");
            ParseInvalidFormUrlEncoded("[]=1");
        }

        /// <summary>
        /// Tests for parsing form-urlencoded data originated from JS objects.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestObjects()
        {
            ValidateFormUrlEncoded("a=NaN", @"{""a"":""NaN""}");
            ValidateFormUrlEncoded("a=false", @"{""a"":""false""}");
            ValidateFormUrlEncoded("a=foo", @"{""a"":""foo""}");
            ValidateFormUrlEncoded("1=1", "{\"1\":\"1\"}");
        }

        /// <summary>
        /// Tests for parsing form-urlencoded data originated from JS arrays.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestArray()
        {
            ValidateFormUrlEncoded("a[]=2", @"{""a"":[""2""]}");
            ValidateFormUrlEncoded("a[]=", @"{""a"":[""""]}");
            ValidateFormUrlEncoded("a[0][0][]=1", @"{""a"":[[[""1""]]]}");
            ValidateFormUrlEncoded("z[]=9&z[]=true&z[]=undefined&z[]=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            ValidateFormUrlEncoded("z[]=9&z[]=true&z[]=undefined&z[]=null", @"{""z"":[""9"",""true"",""undefined"",null]}");
            ValidateFormUrlEncoded("z[0][]=9&z[0][]=true&z[1][]=undefined&z[1][]=null", @"{""z"":[[""9"",""true""],[""undefined"",null]]}");
            ValidateFormUrlEncoded("a[0][x]=2", @"{""a"":[{""x"":""2""}]}");
            ValidateFormUrlEncoded("a%5B%5D=2", @"{""a"":[""2""]}");
            ValidateFormUrlEncoded("a%5B%5D=", @"{""a"":[""""]}");
            ValidateFormUrlEncoded("z%5B%5D=9&z%5B%5D=true&z%5B%5D=undefined&z%5B%5D=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            ValidateFormUrlEncoded("z%5B%5D=9&z%5B%5D=true&z%5B%5D=undefined&z%5B%5D=null", @"{""z"":[""9"",""true"",""undefined"",null]}");
            ValidateFormUrlEncoded("z%5B0%5D%5B%5D=9&z%5B0%5D%5B%5D=true&z%5B1%5D%5B%5D=undefined&z%5B1%5D%5B%5D=null", @"{""z"":[[""9"",""true""],[""undefined"",null]]}");
        }

        /// <summary>
        /// Tests for parsing form-urlencoded data originated from JS arrays, using the jQuery 1.3 format (no []'s).
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestArrayCompat()
        {
            ValidateFormUrlEncoded("z=9&z=true&z=undefined&z=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            ValidateFormUrlEncoded("z=9&z=true&z=undefined&z=null", @"{""z"":[""9"",""true"",""undefined"",null]}");
            ValidateFormUrlEncoded("z=9&z=true&z=undefined&z=null&a=hello", @"{""z"":[""9"",""true"",""undefined"",null],""a"":""hello""}");
        }

        /// <summary>
        /// Negative tests for parsing form-urlencoded data originated from JS arrays.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestArrayCompatNegative()
        {
            ParseInvalidFormUrlEncoded("a[z]=2&a[z]=3");
        }

        /// <summary>
        /// Tests for form-urlencoded data originated from sparse JS arrays.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestArraySparse()
        {
            ValidateFormUrlEncoded("a[2]=hello", @"{""a"":{""2"":""hello""}}");
            ValidateFormUrlEncoded("a[x][0]=2", @"{""a"":{""x"":[""2""]}}");
            ValidateFormUrlEncoded("a[x][1]=2", @"{""a"":{""x"":{""1"":""2""}}}");
            ValidateFormUrlEncoded("a[x][0]=0&a[x][1]=1", @"{""a"":{""x"":[""0"",""1""]}}");
            ValidateFormUrlEncoded("a[0][0][0]=hello&a[1][0][0][0][]=hello", @"{""a"":[[[""hello""]],[[[[""hello""]]]]]}");
            ValidateFormUrlEncoded("a[0][0][0]=hello&a[1][0][0][0]=hello", @"{""a"":[[[""hello""]],[[[""hello""]]]]}");
            ValidateFormUrlEncoded("a[1][0][]=1", @"{""a"":{""1"":[[""1""]]}}");
            ValidateFormUrlEncoded("a[1][1][]=1", @"{""a"":{""1"":{""1"":[""1""]}}}");
            ValidateFormUrlEncoded("a[1][1][0]=1", @"{""a"":{""1"":{""1"":[""1""]}}}");
            ValidateFormUrlEncoded("a[0][]=2&a[0][]=3&a[2][]=1", "{\"a\":{\"0\":[\"2\",\"3\"],\"2\":[\"1\"]}}");
            ValidateFormUrlEncoded("a[x][]=1&a[x][1]=2", @"{""a"":{""x"":[""1"",""2""]}}");
            ValidateFormUrlEncoded("a[x][0]=1&a[x][]=2", @"{""a"":{""x"":[""1"",""2""]}}");
        }

        /// <summary>
        /// Negative tests for parsing form-urlencoded arrays.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestArrayIndexNegative()
        {
            ParseInvalidFormUrlEncoded("a[x]=2&a[x][]=3");
            ParseInvalidFormUrlEncoded("a[]=1&a[0][]=2");
            ParseInvalidFormUrlEncoded("a[]=1&a[0][0][]=2");
            ParseInvalidFormUrlEncoded("a[x][]=1&a[x][0]=2");
            ParseInvalidFormUrlEncoded("a[][]=0");
            ParseInvalidFormUrlEncoded("a[][x]=0");
        }

        /// <summary>
        /// Tests for parsing complex object graphs form-urlencoded.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestObject()
        {
            string encoded = "a[]=4&a[]=5&b[x][]=7&b[y]=8&b[z][]=9&b[z][]=true&b[z][]=undefined&b[z][]=&c=1&f=";
            string resultStr = @"{""a"":[""4"",""5""],""b"":{""x"":[""7""],""y"":""8"",""z"":[""9"",""true"",""undefined"",""""]},""c"":""1"",""f"":""""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "customer[Name]=Pete&customer[Address]=Redmond&customer[Age][0][]=23&customer[Age][0][]=24&customer[Age][1][]=25&" +
                "customer[Age][1][]=26&customer[Phones][]=425+888+1111&customer[Phones][]=425+345+7777&customer[Phones][]=425+888+4564&" +
                "customer[EnrolmentDate]=%22%5C%2FDate(1276562539537)%5C%2F%22&role=NewRole&changeDate=3&count=15";
            resultStr = @"{""customer"":{""Name"":""Pete"",""Address"":""Redmond"",""Age"":[[""23"",""24""],[""25"",""26""]]," +
                @"""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276562539537)\\\/\""""},""role"":""NewRole"",""changeDate"":""3"",""count"":""15""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "customers[0][Name]=Pete2&customers[0][Address]=Redmond2&customers[0][Age][0][]=23&customers[0][Age][0][]=24&" +
                "customers[0][Age][1][]=25&customers[0][Age][1][]=26&customers[0][Phones][]=425+888+1111&customers[0][Phones][]=425+345+7777&" +
                "customers[0][Phones][]=425+888+4564&customers[0][EnrolmentDate]=%22%5C%2FDate(1276634840700)%5C%2F%22&customers[1][Name]=Pete3&" +
                "customers[1][Address]=Redmond3&customers[1][Age][0][]=23&customers[1][Age][0][]=24&customers[1][Age][1][]=25&customers[1][Age][1][]=26&" +
                "customers[1][Phones][]=425+888+1111&customers[1][Phones][]=425+345+7777&customers[1][Phones][]=425+888+4564&customers[1][EnrolmentDate]=%22%5C%2FDate(1276634840700)%5C%2F%22";
            resultStr = @"{""customers"":[{""Name"":""Pete2"",""Address"":""Redmond2"",""Age"":[[""23"",""24""],[""25"",""26""]]," +
                @"""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276634840700)\\\/\""""}," +
                @"{""Name"":""Pete3"",""Address"":""Redmond3"",""Age"":[[""23"",""24""],[""25"",""26""]],""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276634840700)\\\/\""""}]}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "ab%5B%5D=hello";
            resultStr = @"{""ab"":[""hello""]}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "123=hello";
            resultStr = @"{""123"":""hello""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "a%5B%5D=1&a";
            resultStr = @"{""a"":[""1"",null]}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "a=1&a";
            resultStr = @"{""a"":[""1"",null]}";
            ValidateFormUrlEncoded(encoded, resultStr);
        }

        /// <summary>
        /// Tests for parsing form-urlencoded data with encoded names.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestEncodedName()
        {
            string encoded = "some+thing=10";
            string resultStr = @"{""some thing"":""10""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar";
            resultStr = @"{""带三个表"":""bar""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "some+thing=10&%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar";
            resultStr = @"{""some thing"":""10"",""带三个表"":""bar""}";
            ValidateFormUrlEncoded(encoded, resultStr);

            encoded = "a[0\r\n][b]=1";
            resultStr = "{\"a\":{\"0\\u000d\\u000a\":{\"b\":\"1\"}}}";
            ValidateFormUrlEncoded(encoded, resultStr);
            ValidateFormUrlEncoded(encoded.Replace("\r", "%0D").Replace("\n", "%0A"), resultStr);

            ValidateFormUrlEncoded("a[0\0]=1", "{\"a\":{\"0\\u0000\":\"1\"}}");
            ValidateFormUrlEncoded("a[0%00]=1", "{\"a\":{\"0\\u0000\":\"1\"}}");
            ValidateFormUrlEncoded("a[\00]=1", "{\"a\":{\"\\u00000\":\"1\"}}");
            ValidateFormUrlEncoded("a[%000]=1", "{\"a\":{\"\\u00000\":\"1\"}}");
        }

        /// <summary>
        /// Tests for malformed form-urlencoded data.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void TestNegative()
        {
            ParseInvalidFormUrlEncoded("a[b=2");
            ParseInvalidFormUrlEncoded("a[[b]=2");
            ParseInvalidFormUrlEncoded("a[b]]=2");
        }

        /// <summary>
        /// Tests for parsing generated form-urlencoded data.
        /// </summary>
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void GeneratedJsonValueTest()
        {
            Random rndGen = new Random(1);
            int oldMaxArray = CreatorSettings.MaxArrayLength;
            int oldMaxList = CreatorSettings.MaxListLength;
            int oldMaxStr = CreatorSettings.MaxStringLength;
            double oldNullProbability = CreatorSettings.NullValueProbability;
            bool oldCreateAscii = CreatorSettings.CreateOnlyAsciiChars;
            CreatorSettings.MaxArrayLength = 5;
            CreatorSettings.MaxListLength = 3;
            CreatorSettings.MaxStringLength = 3;
            CreatorSettings.NullValueProbability = 0;
            CreatorSettings.CreateOnlyAsciiChars = true;
            JsonValueCreatorSurrogate jsonValueCreator = new JsonValueCreatorSurrogate();
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    JsonValue jv = (JsonValue)jsonValueCreator.CreateInstanceOf(typeof(JsonValue), rndGen);
                    if (jv.JsonType == JsonType.Array || jv.JsonType == JsonType.Object)
                    {
                        string jaStr = FormUrlEncoding(jv);
                        JsonValue deserJv = ParseFormUrlEncoded(jaStr);
                        bool compare = true;
                        if (deserJv is JsonObject && ((JsonObject)deserJv).ContainsKey("JV"))
                        {
                            compare = JsonValueRoundTripComparer.Compare(jv, deserJv["JV"]);
                        }
                        else
                        {
                            compare = JsonValueRoundTripComparer.Compare(jv, deserJv);
                        }

                        Assert.IsTrue(compare, "Comparison failed for test instance " + i);
                    }
                }
            }
            finally
            {
                CreatorSettings.MaxArrayLength = oldMaxArray;
                CreatorSettings.MaxListLength = oldMaxList;
                CreatorSettings.MaxStringLength = oldMaxStr;
                CreatorSettings.NullValueProbability = oldNullProbability;
                CreatorSettings.CreateOnlyAsciiChars = oldCreateAscii;
            }
        }

        #endregion

        #region Helpers
        private static string FormUrlEncoding(JsonValue jsonValue)
        {
            List<string> results = new List<string>();
            if (jsonValue is JsonPrimitive)
            {
                return UrlUtility.UrlEncode(((JsonPrimitive)jsonValue).Value.ToString());
            }

            BuildParams("JV", jsonValue, results);
            StringBuilder strResult = new StringBuilder();
            foreach (var result in results)
            {
                strResult.Append("&" + result);
            }

            if (strResult.Length > 0)
            {
                return strResult.Remove(0, 1).ToString();
            }

            return strResult.ToString();
        }

        private static void BuildParams(string prefix, JsonValue jsonValue, List<string> results)
        {
            if (jsonValue is JsonPrimitive)
            {
                JsonPrimitive jsonPrimitive = jsonValue as JsonPrimitive;
                if (jsonPrimitive != null)
                {
                    if (jsonPrimitive.JsonType == JsonType.String && string.IsNullOrEmpty(jsonPrimitive.Value.ToString()))
                    {
                        results.Add(prefix + "=" + string.Empty);
                    }
                    else
                    {
                        if (jsonPrimitive.Value is DateTime || jsonPrimitive.Value is DateTimeOffset)
                        {
                            string dateStr = jsonPrimitive.ToString();
                            if (!string.IsNullOrEmpty(dateStr) && dateStr.StartsWith("\""))
                            {
                                dateStr = dateStr.Substring(1, dateStr.Length - 2);
                            }
                            results.Add(prefix + "=" + UrlUtility.UrlEncode(dateStr));
                        }
                        else
                        {
                            results.Add(prefix + "=" + UrlUtility.UrlEncode(jsonPrimitive.Value.ToString()));
                        }
                    }
                }
                else
                {
                    results.Add(prefix + "=" + string.Empty);
                }
            }
            else if (jsonValue is JsonArray)
            {
                for (int i = 0; i < jsonValue.Count; i++)
                {
                    if (jsonValue[i] is JsonArray || jsonValue[i] is JsonObject)
                    {
                        BuildParams(prefix + "[" + i + "]", jsonValue[i], results);
                    }
                    else
                    {
                        BuildParams(prefix + "[]", jsonValue[i], results);
                    }
                }
            }
            else //jsonValue is JsonObject
            {
                foreach (KeyValuePair<string, JsonValue> item in jsonValue)
                {
                    BuildParams(prefix + "[" + item.Key + "]", item.Value, results);
                }
            }
        }

        private static Uri GetQueryUri(string query)
        {
            UriBuilder uriBuilder = new UriBuilder("http://some.host");
            uriBuilder.Query = query;
            return uriBuilder.Uri;
        }

        private static JsonValue ParseFormUrlEncoded(string encoded)
        {
            Uri address = GetQueryUri(encoded);
            JsonObject result;
            Assert.IsTrue(UriExtensionMethods.TryReadQueryAsJson(address, out result), "Expected parsing to return true");
            return result;
        }

        private static void ParseInvalidFormUrlEncoded(string encoded)
        {
            Uri address = GetQueryUri(encoded);
            JsonObject result;
            Assert.IsFalse(UriExtensionMethods.TryReadQueryAsJson(address, out result), "Expected parsing to return false");
            Assert.IsNull(result, "expected null result");
        }

        private static void ValidateFormUrlEncoded(string encoded, string expectedResult)
        {
            Uri address = GetQueryUri(encoded);
            JsonObject result;
            Assert.IsTrue(UriExtensionMethods.TryReadQueryAsJson(address, out result), "Expected parsing to return true");
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.ToString(JsonSaveOptions.None));
        }

        #endregion
    }
}