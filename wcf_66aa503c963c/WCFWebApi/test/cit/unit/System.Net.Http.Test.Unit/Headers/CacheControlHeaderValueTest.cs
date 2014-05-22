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
    public class CacheControlHeaderValueTest
    {
        [TestMethod]
        public void Properties_SetAndGetAllProperties_SetValueReturnedInGetter()
        {
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();

            // Bool properties
            cacheControl.NoCache = true;
            Assert.IsTrue(cacheControl.NoCache, "NoCache");
            cacheControl.NoStore = true;
            Assert.IsTrue(cacheControl.NoStore, "NoStore");
            cacheControl.MaxStale = true;
            Assert.IsTrue(cacheControl.MaxStale, "MaxStale");
            cacheControl.NoTransform = true;
            Assert.IsTrue(cacheControl.NoTransform, "NoTransform");
            cacheControl.OnlyIfCached = true;
            Assert.IsTrue(cacheControl.OnlyIfCached, "OnlyIfCached");
            cacheControl.Public = true;
            Assert.IsTrue(cacheControl.Public, "Public");
            cacheControl.Private = true;
            Assert.IsTrue(cacheControl.Private, "Private");
            cacheControl.MustRevalidate = true;
            Assert.IsTrue(cacheControl.MustRevalidate, "MustRevalidate");
            cacheControl.ProxyRevalidate = true;
            Assert.IsTrue(cacheControl.ProxyRevalidate, "ProxyRevalidate");

            // TimeSpan properties
            TimeSpan timeSpan = new TimeSpan(1, 2, 3);
            cacheControl.MaxAge = timeSpan;
            Assert.AreEqual(timeSpan, cacheControl.MaxAge, "MaxAge");
            cacheControl.SharedMaxAge = timeSpan;
            Assert.AreEqual(timeSpan, cacheControl.SharedMaxAge, "SharedMaxAge");
            cacheControl.MaxStaleLimit = timeSpan;
            Assert.AreEqual(timeSpan, cacheControl.MaxStaleLimit, "MaxStaleLimit");
            cacheControl.MinFresh = timeSpan;
            Assert.AreEqual(timeSpan, cacheControl.MinFresh, "MinFresh");

            // String collection properties
            Assert.IsNotNull(cacheControl.NoCacheHeaders, "NoCacheHeaders");
            ExceptionAssert.Throws<ArgumentException>(() => cacheControl.NoCacheHeaders.Add(null),
                "NoCacheHeaders.Add(null)");
            ExceptionAssert.Throws<FormatException>(() => cacheControl.NoCacheHeaders.Add("invalid token"),
                "NoCacheHeaders.Add(\"invalid token\")");
            cacheControl.NoCacheHeaders.Add("token");
            Assert.AreEqual(1, cacheControl.NoCacheHeaders.Count, "NoCacheHeaders.Count");
            Assert.AreEqual("token", cacheControl.NoCacheHeaders.First(), "NoCacheHeaders[0]");

            Assert.IsNotNull(cacheControl.PrivateHeaders, "PrivateHeaders");
            ExceptionAssert.Throws<ArgumentException>(() => cacheControl.PrivateHeaders.Add(null),
                "PrivateHeaders.Add(null)");
            ExceptionAssert.Throws<FormatException>(() => cacheControl.PrivateHeaders.Add("invalid token"),
                "PrivateHeaders.Add(\"invalid token\")");
            cacheControl.PrivateHeaders.Add("token");
            Assert.AreEqual(1, cacheControl.PrivateHeaders.Count, "PrivateHeaders.Count");
            Assert.AreEqual("token", cacheControl.PrivateHeaders.First(), "PrivateHeaders[0]");

            // NameValueHeaderValue collection property
            Assert.IsNotNull(cacheControl.Extensions, "Extensions");
            ExceptionAssert.Throws<ArgumentNullException>(() => cacheControl.Extensions.Add(null),
                "Extensions.Add(null)");
            cacheControl.Extensions.Add(new NameValueHeaderValue("name", "value"));
            Assert.AreEqual(1, cacheControl.Extensions.Count, "Extensions.Count");
            Assert.AreEqual(new NameValueHeaderValue("name", "value"), cacheControl.Extensions.First(), "Extensions[0]");
        }

        [TestMethod]
        public void ToString_UseRequestDirectiveValues_AllSerializedCorrectly()
        {
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            Assert.AreEqual("", cacheControl.ToString());

            // Note that we allow all combinations of all properties even though the RFC specifies rules what value
            // can be used together.
            // Also for property pairs (bool property + collection property) like 'NoCache' and 'NoCacheHeaders' the
            // caller needs to set the bool property in order for the collection to be populated as string.

            // Cache Request Directive sample
            cacheControl.NoStore = true;
            Assert.AreEqual("no-store", cacheControl.ToString());
            cacheControl.NoCache = true;
            Assert.AreEqual("no-store, no-cache", cacheControl.ToString());
            cacheControl.MaxAge = new TimeSpan(0, 1, 10);
            Assert.AreEqual("no-store, no-cache, max-age=70", cacheControl.ToString());
            cacheControl.MaxStale = true;
            Assert.AreEqual("no-store, no-cache, max-age=70, max-stale", cacheControl.ToString());
            cacheControl.MaxStaleLimit = new TimeSpan(0, 2, 5);
            Assert.AreEqual("no-store, no-cache, max-age=70, max-stale=125", cacheControl.ToString());
            cacheControl.MinFresh = new TimeSpan(0, 3, 0);
            Assert.AreEqual("no-store, no-cache, max-age=70, max-stale=125, min-fresh=180", cacheControl.ToString());

            cacheControl = new CacheControlHeaderValue();
            cacheControl.NoTransform = true;
            Assert.AreEqual("no-transform", cacheControl.ToString());
            cacheControl.OnlyIfCached = true;
            Assert.AreEqual("no-transform, only-if-cached", cacheControl.ToString());
            cacheControl.Extensions.Add(new NameValueHeaderValue("custom"));
            cacheControl.Extensions.Add(new NameValueHeaderValue("customName", "customValue"));
            Assert.AreEqual("no-transform, only-if-cached, custom, customName=customValue", cacheControl.ToString());

            cacheControl = new CacheControlHeaderValue();
            cacheControl.Extensions.Add(new NameValueHeaderValue("custom"));
            Assert.AreEqual("custom", cacheControl.ToString());
        }

        [TestMethod]
        public void ToString_UseResponseDirectiveValues_AllSerializedCorrectly()
        {
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            Assert.AreEqual("", cacheControl.ToString());

            cacheControl.NoCache = true;
            Assert.AreEqual("no-cache", cacheControl.ToString());
            cacheControl.NoCacheHeaders.Add("token1");
            Assert.AreEqual("no-cache=\"token1\"", cacheControl.ToString());
            cacheControl.Public = true;
            Assert.AreEqual("public, no-cache=\"token1\"", cacheControl.ToString());

            cacheControl = new CacheControlHeaderValue();
            cacheControl.Private = true;
            Assert.AreEqual("private", cacheControl.ToString());
            cacheControl.PrivateHeaders.Add("token2");
            cacheControl.PrivateHeaders.Add("token3");
            Assert.AreEqual("private=\"token2, token3\"", cacheControl.ToString());
            cacheControl.MustRevalidate = true;
            Assert.AreEqual("must-revalidate, private=\"token2, token3\"", cacheControl.ToString());
            cacheControl.ProxyRevalidate = true;
            Assert.AreEqual("must-revalidate, proxy-revalidate, private=\"token2, token3\"", cacheControl.ToString());            
        }

        [TestMethod]
        public void GetHashCode_CompareValuesWithBoolFieldsSet_MatchExpectation()
        {
            // Verify that different bool fields return different hash values.
            CacheControlHeaderValue[] values = new CacheControlHeaderValue[9];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new CacheControlHeaderValue();
            }

            values[0].ProxyRevalidate = true;
            values[1].NoCache = true;
            values[2].NoStore = true;
            values[3].MaxStale = true;
            values[4].NoTransform = true;
            values[5].OnlyIfCached = true;
            values[6].Public = true;
            values[7].Private = true;
            values[8].MustRevalidate = true;

            // Only one bool field set. All hash codes should differ
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i != j)
                    {
                        CompareHashCodes(values[i], values[j], false);                        
                    }
                }
            }

            // Validate that two instances with the same bool fields set are equal.
            values[0].NoCache = true;
            CompareHashCodes(values[0], values[1], false);
            values[1].ProxyRevalidate = true;
            CompareHashCodes(values[0], values[1], true);            
        }

        [TestMethod]
        public void GetHashCode_CompareValuesWithTimeSpanFieldsSet_MatchExpectation()
        {
            // Verify that different timespan fields return different hash values.
            CacheControlHeaderValue[] values = new CacheControlHeaderValue[4];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new CacheControlHeaderValue();
            }

            values[0].MaxAge = new TimeSpan(0, 1, 1);
            values[1].MaxStaleLimit = new TimeSpan(0, 1, 1);
            values[2].MinFresh = new TimeSpan(0, 1, 1);
            values[3].SharedMaxAge = new TimeSpan(0, 1, 1);

            // Only one timespan field set. All hash codes should differ
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i != j)
                    {
                        CompareHashCodes(values[i], values[j], false);
                    }
                }
            }

            values[0].MaxStaleLimit = new TimeSpan(0, 1, 2);
            CompareHashCodes(values[0], values[1], false);
            
            values[1].MaxAge = new TimeSpan(0, 1, 1);
            values[1].MaxStaleLimit = new TimeSpan(0, 1, 2);
            CompareHashCodes(values[0], values[1], true);
        }

        [TestMethod]
        public void GetHashCode_CompareCollectionFieldsSet_MatchExpectation()
        {
            CacheControlHeaderValue cacheControl1 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl2 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl3 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl4 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl5 = new CacheControlHeaderValue();

            cacheControl1.NoCache = true;
            cacheControl1.NoCacheHeaders.Add("token2");
            
            cacheControl2.NoCache = true;
            cacheControl2.NoCacheHeaders.Add("token1");
            cacheControl2.NoCacheHeaders.Add("token2");

            CompareHashCodes(cacheControl1, cacheControl2, false);

            cacheControl1.NoCacheHeaders.Add("token1");
            CompareHashCodes(cacheControl1, cacheControl2, true);

            // Since NoCache and Private generate different hash codes, even if NoCacheHeaders and PrivateHeaders 
            // have the same values, the hash code will be different.
            cacheControl3.Private = true;
            cacheControl3.PrivateHeaders.Add("token2");
            CompareHashCodes(cacheControl1, cacheControl3, false);


            cacheControl4.Extensions.Add(new NameValueHeaderValue("custom"));
            CompareHashCodes(cacheControl1, cacheControl4, false);

            cacheControl5.Extensions.Add(new NameValueHeaderValue("customN", "customV"));
            cacheControl5.Extensions.Add(new NameValueHeaderValue("custom"));
            CompareHashCodes(cacheControl4, cacheControl5, false);

            cacheControl4.Extensions.Add(new NameValueHeaderValue("customN", "customV"));
            CompareHashCodes(cacheControl4, cacheControl5, true);
        }

        [TestMethod]
        public void Equals_CompareValuesWithBoolFieldsSet_MatchExpectation()
        {
            // Verify that different bool fields return different hash values.
            CacheControlHeaderValue[] values = new CacheControlHeaderValue[9];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new CacheControlHeaderValue();
            }

            values[0].ProxyRevalidate = true;
            values[1].NoCache = true;
            values[2].NoStore = true;
            values[3].MaxStale = true;
            values[4].NoTransform = true;
            values[5].OnlyIfCached = true;
            values[6].Public = true;
            values[7].Private = true;
            values[8].MustRevalidate = true;

            // Only one bool field set. All hash codes should differ
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i != j)
                    {
                        CompareValues(values[i], values[j], false);
                    }
                }
            }

            // Validate that two instances with the same bool fields set are equal.
            values[0].NoCache = true;
            CompareValues(values[0], values[1], false);
            values[1].ProxyRevalidate = true;
            CompareValues(values[0], values[1], true);
        }

        [TestMethod]
        public void Equals_CompareValuesWithTimeSpanFieldsSet_MatchExpectation()
        {
            // Verify that different timespan fields return different hash values.
            CacheControlHeaderValue[] values = new CacheControlHeaderValue[4];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new CacheControlHeaderValue();
            }

            values[0].MaxAge = new TimeSpan(0, 1, 1);
            values[1].MaxStaleLimit = new TimeSpan(0, 1, 1);
            values[2].MinFresh = new TimeSpan(0, 1, 1);
            values[3].SharedMaxAge = new TimeSpan(0, 1, 1);

            // Only one timespan field set. All hash codes should differ
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i != j)
                    {
                        CompareValues(values[i], values[j], false);
                    }
                }
            }

            values[0].MaxStaleLimit = new TimeSpan(0, 1, 2);
            CompareValues(values[0], values[1], false);

            values[1].MaxAge = new TimeSpan(0, 1, 1);
            values[1].MaxStaleLimit = new TimeSpan(0, 1, 2);
            CompareValues(values[0], values[1], true);

            CacheControlHeaderValue value1 = new CacheControlHeaderValue();
            value1.MaxStale = true;
            CacheControlHeaderValue value2 = new CacheControlHeaderValue();
            value2.MaxStale = true;
            CompareValues(value1, value2, true);

            value2.MaxStaleLimit = new TimeSpan(1, 2, 3);
            CompareValues(value1, value2, false);
        }

        [TestMethod]
        public void Equals_CompareCollectionFieldsSet_MatchExpectation()
        {
            CacheControlHeaderValue cacheControl1 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl2 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl3 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl4 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl5 = new CacheControlHeaderValue();
            CacheControlHeaderValue cacheControl6 = new CacheControlHeaderValue();

            cacheControl1.NoCache = true;
            cacheControl1.NoCacheHeaders.Add("token2");

            Assert.IsFalse(cacheControl1.Equals(null), "Compare with 'null'");
            
            cacheControl2.NoCache = true;
            cacheControl2.NoCacheHeaders.Add("token1");
            cacheControl2.NoCacheHeaders.Add("token2");

            CompareValues(cacheControl1, cacheControl2, false);

            cacheControl1.NoCacheHeaders.Add("token1");
            CompareValues(cacheControl1, cacheControl2, true);

            // Since NoCache and Private generate different hash codes, even if NoCacheHeaders and PrivateHeaders 
            // have the same values, the hash code will be different.
            cacheControl3.Private = true;
            cacheControl3.PrivateHeaders.Add("token2");
            CompareValues(cacheControl1, cacheControl3, false);

            cacheControl4.Private = true;
            cacheControl4.PrivateHeaders.Add("token3");
            CompareValues(cacheControl3, cacheControl4, false);

            cacheControl5.Extensions.Add(new NameValueHeaderValue("custom"));
            CompareValues(cacheControl1, cacheControl5, false);

            cacheControl6.Extensions.Add(new NameValueHeaderValue("customN", "customV"));
            cacheControl6.Extensions.Add(new NameValueHeaderValue("custom"));
            CompareValues(cacheControl5, cacheControl6, false);

            cacheControl5.Extensions.Add(new NameValueHeaderValue("customN", "customV"));
            CompareValues(cacheControl5, cacheControl6, true);
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            CacheControlHeaderValue source = new CacheControlHeaderValue();
            source.Extensions.Add(new NameValueHeaderValue("custom"));
            source.Extensions.Add(new NameValueHeaderValue("customN", "customV"));
            source.MaxAge = new TimeSpan(1, 1, 1);
            source.MaxStale = true;
            source.MaxStaleLimit = new TimeSpan(1, 1, 2);
            source.MinFresh = new TimeSpan(1, 1, 3);
            source.MustRevalidate = true;
            source.NoCache = true;
            source.NoCacheHeaders.Add("token1");
            source.NoStore = true;
            source.NoTransform = true;
            source.OnlyIfCached = true;
            source.Private = true;
            source.PrivateHeaders.Add("token2");
            source.ProxyRevalidate = true;
            source.Public = true;
            source.SharedMaxAge = new TimeSpan(1, 1, 4);
            CacheControlHeaderValue clone = (CacheControlHeaderValue)((ICloneable)source).Clone();
            
            Assert.AreEqual(source, clone);            
        }

        [TestMethod]
        public void GetCacheControlLength_DifferentValidScenariosAndNoExistingCacheControl_AllReturnNonZero()
        {
            CacheControlHeaderValue expected = new CacheControlHeaderValue();
            expected.NoCache = true;
            CheckGetCacheControlLength("X , , no-cache ,,", 1, null, 16, expected);

            expected = new CacheControlHeaderValue();
            expected.NoCache = true;
            expected.NoCacheHeaders.Add("token1");
            expected.NoCacheHeaders.Add("token2");
            CheckGetCacheControlLength("no-cache=\"token1, token2\"", 0, null, 25, expected);

            expected = new CacheControlHeaderValue();
            expected.NoStore = true;
            expected.MaxAge = new TimeSpan(0, 0, 125);
            expected.MaxStale = true;
            CheckGetCacheControlLength("X no-store , max-age = 125, max-stale,", 1, null, 37, expected);

            expected = new CacheControlHeaderValue();
            expected.MinFresh = new TimeSpan(0, 0, 123);
            expected.NoTransform = true;
            expected.OnlyIfCached = true;
            expected.Extensions.Add(new NameValueHeaderValue("custom"));            
            CheckGetCacheControlLength("min-fresh=123, no-transform, only-if-cached, custom", 0, null, 51, expected);

            expected = new CacheControlHeaderValue();
            expected.Public = true;
            expected.Private = true;
            expected.PrivateHeaders.Add("token1");
            expected.MustRevalidate = true;
            expected.ProxyRevalidate = true;
            expected.Extensions.Add(new NameValueHeaderValue("c", "d"));
            expected.Extensions.Add(new NameValueHeaderValue("a", "b"));
            CheckGetCacheControlLength(",public, , private=\"token1\", must-revalidate, c=d, proxy-revalidate, a=b", 0, 
                null, 72, expected);

            expected = new CacheControlHeaderValue();
            expected.Private = true;
            expected.SharedMaxAge = new TimeSpan(0, 0, 1234567890);
            expected.MaxAge = new TimeSpan(0, 0, 987654321);
            CheckGetCacheControlLength("s-maxage=1234567890, private, max-age = 987654321,", 0, null, 50, expected);
        }

        [TestMethod]
        public void GetCacheControlLength_DifferentValidScenariosAndExistingCacheControl_AllReturnNonZero()
        {
            CacheControlHeaderValue storeValue = new CacheControlHeaderValue();
            storeValue.NoStore = true;
            CacheControlHeaderValue expected = new CacheControlHeaderValue();
            expected.NoCache = true;
            expected.NoStore = true;
            CheckGetCacheControlLength("X no-cache", 1, storeValue, 9, expected);

            storeValue = new CacheControlHeaderValue();
            storeValue.Private = true;
            storeValue.PrivateHeaders.Add("token1");
            storeValue.NoCache = true;
            expected.NoCacheHeaders.Add("token1");
            expected.NoCacheHeaders.Clear(); // just make sure we have an assigned (empty) collection.
            expected = new CacheControlHeaderValue();
            expected.Private = true;
            expected.PrivateHeaders.Add("token1");
            expected.PrivateHeaders.Add("token2");
            expected.NoCache = true;
            expected.NoCacheHeaders.Add("token1");
            expected.NoCacheHeaders.Add("token2");
            CheckGetCacheControlLength("private=\"token2\", no-cache=\"token1, , token2,\"", 0, storeValue, 46, 
                expected);

            storeValue = new CacheControlHeaderValue();
            storeValue.Extensions.Add(new NameValueHeaderValue("x", "y"));
            storeValue.NoTransform = true;
            storeValue.OnlyIfCached = true;
            expected = new CacheControlHeaderValue();
            expected.Public = true;
            expected.Private = true;
            expected.PrivateHeaders.Add("token1");
            expected.MustRevalidate = true;
            expected.ProxyRevalidate = true;
            expected.NoTransform = true;
            expected.OnlyIfCached = true;
            expected.Extensions.Add(new NameValueHeaderValue("a", "\"b\""));
            expected.Extensions.Add(new NameValueHeaderValue("c", "d"));
            expected.Extensions.Add(new NameValueHeaderValue("x", "y")); // from store result
            CheckGetCacheControlLength(",public, , private=\"token1\", must-revalidate, c=d, proxy-revalidate, a=\"b\"",
                0, storeValue, 74, expected);

            storeValue = new CacheControlHeaderValue();
            storeValue.MaxStale = true;
            storeValue.MinFresh = new TimeSpan(1, 2, 3);
            expected = new CacheControlHeaderValue();
            expected.MaxStale = true;
            expected.MaxStaleLimit = new TimeSpan(0, 0, 5);
            expected.MinFresh = new TimeSpan(0, 0, 10); // note that the last header value overwrites existing ones
            CheckGetCacheControlLength("  ,,max-stale=5,,min-fresh = 10,,", 0, storeValue, 33, expected);

            storeValue = new CacheControlHeaderValue();
            storeValue.SharedMaxAge = new TimeSpan(1, 2, 3);
            storeValue.NoTransform = true;
            expected = new CacheControlHeaderValue();
            expected.SharedMaxAge = new TimeSpan(1, 2, 3);
            expected.NoTransform = true;
        }

        [TestMethod]
        public void GetCacheControlLength_DifferentInvalidScenarios_AllReturnZero()
        {
            // Token-only values
            CheckInvalidCacheControlLength("no-store=15", 0);
            CheckInvalidCacheControlLength("no-store=", 0);

            CheckInvalidCacheControlLength("no-transform=a", 0);
            CheckInvalidCacheControlLength("no-transform=", 0);

            CheckInvalidCacheControlLength("only-if-cached=\"x\"", 0);
            CheckInvalidCacheControlLength("only-if-cached=", 0);

            CheckInvalidCacheControlLength("public=\"x\"", 0);
            CheckInvalidCacheControlLength("public=", 0);

            CheckInvalidCacheControlLength("must-revalidate=\"1\"", 0);
            CheckInvalidCacheControlLength("must-revalidate=", 0);

            CheckInvalidCacheControlLength("proxy-revalidate=x", 0);
            CheckInvalidCacheControlLength("proxy-revalidate=", 0);
            
            // Token with optional field-name list
            CheckInvalidCacheControlLength("no-cache=", 0);
            CheckInvalidCacheControlLength("no-cache=token", 0);
            CheckInvalidCacheControlLength("no-cache=\"token", 0);
            CheckInvalidCacheControlLength("no-cache=\"\"", 0); // at least one token expected as value

            CheckInvalidCacheControlLength("private=", 0);
            CheckInvalidCacheControlLength("private=token", 0);
            CheckInvalidCacheControlLength("private=\"token", 0);
            CheckInvalidCacheControlLength("private=\",\"", 0); // at least one token expected as value
            CheckInvalidCacheControlLength("private=\"=\"", 0); 

            // Token with delta-seconds value
            CheckInvalidCacheControlLength("max-age", 0);
            CheckInvalidCacheControlLength("max-age=", 0);
            CheckInvalidCacheControlLength("max-age=a", 0);
            CheckInvalidCacheControlLength("max-age=\"1\"", 0);
            CheckInvalidCacheControlLength("max-age=1.5", 0);

            CheckInvalidCacheControlLength("max-stale=", 0);
            CheckInvalidCacheControlLength("max-stale=a", 0);
            CheckInvalidCacheControlLength("max-stale=\"1\"", 0);
            CheckInvalidCacheControlLength("max-stale=1.5", 0);

            CheckInvalidCacheControlLength("min-fresh", 0);
            CheckInvalidCacheControlLength("min-fresh=", 0);
            CheckInvalidCacheControlLength("min-fresh=a", 0);
            CheckInvalidCacheControlLength("min-fresh=\"1\"", 0);
            CheckInvalidCacheControlLength("min-fresh=1.5", 0);

            CheckInvalidCacheControlLength("s-maxage", 0);
            CheckInvalidCacheControlLength("s-maxage=", 0);
            CheckInvalidCacheControlLength("s-maxage=a", 0);
            CheckInvalidCacheControlLength("s-maxage=\"1\"", 0);
            CheckInvalidCacheControlLength("s-maxage=1.5", 0);

            // Invalid Extension values
            CheckInvalidCacheControlLength("custom=", 0);
            CheckInvalidCacheControlLength("custom value", 0);

            CheckInvalidCacheControlLength(null, 0);
            CheckInvalidCacheControlLength("", 0);
            CheckInvalidCacheControlLength("", 1);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Just verify parser is implemented correctly. Don't try to test syntax parsed by CacheControlHeaderValue.
            CacheControlHeaderValue expected = new CacheControlHeaderValue();
            expected.NoStore = true;
            expected.MinFresh = new TimeSpan(0, 2, 3);
            CheckValidParse(" , no-store, min-fresh=123", expected);

            expected = new CacheControlHeaderValue();
            expected.MaxStale = true;
            expected.NoCache = true;
            expected.NoCacheHeaders.Add("t");
            CheckValidParse("max-stale, no-cache=\"t\", ,,", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("no-cache,=", 0);
            CheckInvalidParse("max-age=123x", 0);
            CheckInvalidParse("=no-cache", 0);
            CheckInvalidParse("no-cache no-store", 0);
            CheckInvalidParse("invalid =", 0);
            CheckInvalidParse("会", 0);
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            // Just verify parser is implemented correctly. Don't try to test syntax parsed by CacheControlHeaderValue.
            CacheControlHeaderValue expected = new CacheControlHeaderValue();
            expected.NoStore = true;
            expected.MinFresh = new TimeSpan(0, 2, 3);
            CheckValidTryParse(" , no-store, min-fresh=123", expected);

            expected = new CacheControlHeaderValue();
            expected.MaxStale = true;
            expected.NoCache = true;
            expected.NoCacheHeaders.Add("t");
            CheckValidTryParse("max-stale, no-cache=\"t\", ,,", expected);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("no-cache,=", 0);
            CheckInvalidTryParse("max-age=123x", 0);
            CheckInvalidTryParse("=no-cache", 0);
            CheckInvalidTryParse("no-cache no-store", 0);
            CheckInvalidTryParse("invalid =", 0);
            CheckInvalidTryParse("会", 0);
        }

        #region Helper methods
        
        private void CompareHashCodes(CacheControlHeaderValue x, CacheControlHeaderValue y, bool areEqual)
        {
            if (areEqual)
            {
                Assert.AreEqual(x.GetHashCode(), y.GetHashCode(), "'{0}' vs. '{1}'", x.ToString(), y.ToString());
            }
            else
            {
                Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode(), "'{0}' vs. '{1}'", x.ToString(), y.ToString());
            }
        }

        private void CompareValues(CacheControlHeaderValue x, CacheControlHeaderValue y, bool areEqual)
        {
            Assert.AreEqual(areEqual, x.Equals(y), "'{0}' vs. '{1}'", x.ToString(), y.ToString());
            Assert.AreEqual(areEqual, y.Equals(x), "'{0}' vs. '{1}'", y.ToString(), x.ToString());
        }

        private static void CheckGetCacheControlLength(string input, int startIndex, CacheControlHeaderValue storeValue,
            int expectedLength, CacheControlHeaderValue expectedResult)
        {
            CacheControlHeaderValue result = null;
            Assert.AreEqual(expectedLength, CacheControlHeaderValue.GetCacheControlLength(input, startIndex, 
                storeValue, out result), "Input: '{0}', Start index: {1}", input, startIndex);

            if (storeValue == null)
            {
                Assert.AreEqual(expectedResult, result, "Input: '{0}', Start index: {1}", input, startIndex);
            }
            else
            {
                // If we provide a 'storeValue', then that instance will be updated and result will be 'null'
                Assert.IsNull(result);
                Assert.AreEqual(expectedResult, storeValue, "Input: '{0}', Start index: {1}", input, startIndex);
            }
        }

        private static void CheckInvalidCacheControlLength(string input, int startIndex)
        {
            CacheControlHeaderValue result = null;
            Assert.AreEqual(0, CacheControlHeaderValue.GetCacheControlLength(input, startIndex, null, out result),
                "Input: '{0}', Start index: {1}", input, startIndex);
            Assert.IsNull(result);
        }

        private void CheckValidParse(string input, CacheControlHeaderValue expectedResult)
        {
            CacheControlHeaderValue result = CacheControlHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input, int startIndex)
        {
            ExceptionAssert.Throws<FormatException>(() => CacheControlHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, CacheControlHeaderValue expectedResult)
        {
            CacheControlHeaderValue result = null;
            Assert.IsTrue(CacheControlHeaderValue.TryParse(input, out result));
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input, int startIndex)
        {
            CacheControlHeaderValue result = null;
            Assert.IsFalse(CacheControlHeaderValue.TryParse(input, out result));
            Assert.IsNull(result);
        }

        #endregion
    }
}
