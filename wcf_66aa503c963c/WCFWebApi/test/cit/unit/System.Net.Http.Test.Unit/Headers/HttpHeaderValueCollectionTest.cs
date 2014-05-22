using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Collections;
using System.Net.Test.Common;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class HttpHeaderValueCollectionTest
    {
        private const string knownHeader = "known-header";
        private static readonly Uri specialValue = new Uri("http://special/");
        private static readonly Uri invalidValue = new Uri("http://invalid/");
        private static readonly TransferCodingHeaderValue specialChunked = new TransferCodingHeaderValue("chunked");

        // Note that this type just forwards calls to HttpHeaders. So this test method focusses on making sure 
        // the correct calls to HttpHeaders are made. This test suite will not test HttpHeaders functionality.

        [TestMethod]
        public void IsReadOnly_CallProperty_AlwaysFalse()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(string)));
            HttpHeaderValueCollection<string> collection = new HttpHeaderValueCollection<string>(knownHeader, headers);

            Assert.IsFalse(collection.IsReadOnly);
        }

        [TestMethod]
        public void Count_AddSingleValueThenQueryCount_ReturnsValueCountWithSpecialValues()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(string)));
            HttpHeaderValueCollection<string> collection = new HttpHeaderValueCollection<string>(knownHeader, headers,
                "special");

            Assert.AreEqual(0, collection.Count);

            headers.Add(knownHeader, "value2");
            Assert.AreEqual(1, collection.Count);

            headers.Clear();
            headers.Add(knownHeader, "special");
            Assert.AreEqual(1, collection.Count, "No value expected when adding just special values");
            headers.Add(knownHeader, "special");
            headers.Add(knownHeader, "special");
            Assert.AreEqual(3, collection.Count, "No value expected when adding just special values");
        }

        [TestMethod]
        public void Count_AddMultipleValuesThenQueryCount_ReturnsValueCountWithSpecialValues()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(string)));
            HttpHeaderValueCollection<string> collection = new HttpHeaderValueCollection<string>(knownHeader, headers,
                "special");

            Assert.AreEqual(0, collection.Count, "New collection was not empty.");

            collection.Add("value1");
            headers.Add(knownHeader, "special");
            Assert.AreEqual(2, collection.Count);

            headers.Add(knownHeader, "special");
            headers.Add(knownHeader, "value2");
            headers.Add(knownHeader, "special");
            Assert.AreEqual(5, collection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_CallWithNullValue_Throw()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(null);
        }

        [TestMethod]
        public void Add_AddValues_AllValuesAdded()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Assert.AreEqual(3, collection.Count, "Number of items in the collection.");
        }

        [TestMethod]
        public void Add_UseSpecialValue_Success()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();
            Assert.IsNull(headers.TransferEncodingChunked);
            Assert.AreEqual(0, headers.TransferEncoding.Count);
            Assert.AreEqual(String.Empty, headers.TransferEncoding.ToString());

            headers.TransferEncoding.Add(specialChunked);

            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.AreEqual(specialChunked, headers.TransferEncoding.First());
            Assert.AreEqual(specialChunked.ToString(), headers.TransferEncoding.ToString());
        }

        [TestMethod]
        public void Add_UseSpecialValueWithSpecialAlreadyPresent_AddsDuplicate()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();
            headers.TransferEncodingChunked = true;

            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.AreEqual(specialChunked.ToString(), headers.TransferEncoding.ToString());

            headers.TransferEncoding.Add(specialChunked);

            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(2, headers.TransferEncoding.Count);
            Assert.AreEqual("chunked, chunked", headers.TransferEncoding.ToString());

            // removes first instance of
            headers.TransferEncodingChunked = false;

            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.AreEqual(specialChunked.ToString(), headers.TransferEncoding.ToString());

            // does not add duplicate
            headers.TransferEncodingChunked = true;

            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.AreEqual(specialChunked.ToString(), headers.TransferEncoding.ToString());
        }

        [TestMethod]
        public void ParseAdd_CallWithNullValue_NothingAdded()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.ParseAdd(null);
            Assert.IsFalse(collection.IsSpecialValueSet);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(String.Empty, collection.ToString());
        }

        [TestMethod]
        public void ParseAdd_AddValues_AllValuesAdded()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.ParseAdd("http://www.example.org/1/");
            collection.ParseAdd("http://www.example.org/2/");
            collection.ParseAdd("http://www.example.org/3/");

            Assert.AreEqual(3, collection.Count, "Number of items in the collection.");
        }
        
        [TestMethod]
        public void ParseAdd_UseSpecialValue_Added()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.ParseAdd(specialValue.AbsoluteUri);

            Assert.IsTrue(collection.IsSpecialValueSet);
            Assert.AreEqual(specialValue, collection.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseAdd_AddBadValue_Throws()
        {
            HttpResponseHeaders headers = new HttpResponseHeaders();
            string input = "Basic, D\rigest qop=\"auth\",algorithm=MD5-sess";
            headers.WwwAuthenticate.ParseAdd(input);
        }

        [TestMethod]
        public void TryParseAdd_CallWithNullValue_NothingAdded()
        {
            HttpResponseHeaders headers = new HttpResponseHeaders();

            Assert.IsTrue(headers.WwwAuthenticate.TryParseAdd(null));

            Assert.IsFalse(headers.WwwAuthenticate.IsSpecialValueSet);
            Assert.AreEqual(0, headers.WwwAuthenticate.Count);
            Assert.AreEqual(String.Empty, headers.WwwAuthenticate.ToString());
        }

        [TestMethod]
        public void TryParseAdd_AddValues_AllAdded()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            Assert.IsTrue(collection.TryParseAdd("http://www.example.org/1/"));
            Assert.IsTrue(collection.TryParseAdd("http://www.example.org/2/"));
            Assert.IsTrue(collection.TryParseAdd("http://www.example.org/3/"));

            Assert.AreEqual(3, collection.Count, "Number of items in the collection.");
        }

        [TestMethod]
        public void TryParseAdd_UseSpecialValue_Added()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            Assert.IsTrue(collection.TryParseAdd(specialValue.AbsoluteUri));

            Assert.IsTrue(collection.IsSpecialValueSet);
            Assert.AreEqual(specialValue, collection.ToString());
        }

        [TestMethod]
        public void TryParseAdd_AddBadValue_False()
        {
            HttpResponseHeaders headers = new HttpResponseHeaders();
            string input = "Basic, D\rigest qop=\"auth\",algorithm=MD5-sess";
            Assert.IsFalse(headers.WwwAuthenticate.TryParseAdd(input));
            Assert.AreEqual(string.Empty, headers.WwwAuthenticate.ToString());
            Assert.AreEqual(string.Empty, headers.ToString());
        }

        [TestMethod]
        public void TryParseAdd_AddBadAfterGoodValue_False()
        {
            HttpResponseHeaders headers = new HttpResponseHeaders();
            headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Negotiate"));
            string input = "Basic, D\rigest qop=\"auth\",algorithm=MD5-sess";
            Assert.IsFalse(headers.WwwAuthenticate.TryParseAdd(input));
            Assert.AreEqual("Negotiate", headers.WwwAuthenticate.ToString());
            Assert.AreEqual("WWW-Authenticate: Negotiate\r\n", headers.ToString());
        }

        [TestMethod]
        public void Clear_AddValuesThenClear_NoElementsInCollection()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Assert.AreEqual(3, collection.Count, "Number of items in the collection.");

            collection.Clear();

            Assert.AreEqual(0, collection.Count, "Expected no items in collection after calling Clear().");
        }

        [TestMethod]
        public void Clear_AddValuesAndSpecialValueThenClear_EverythingCleared()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.SetSpecialValue();
            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Assert.AreEqual(4, collection.Count, "Number of items in the collection.");
            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");

            collection.Clear();

            Assert.AreEqual(0, collection.Count, "Expected no items in collection after calling Clear().");
            Assert.IsFalse(collection.IsSpecialValueSet, "Special value was removed by Clear().");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contains_CallWithNullValue_Throw()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Contains(null);
        }

        [TestMethod]
        public void Contains_AddValuesThenCallContains_ReturnsTrueForExistingItemsFalseOtherwise()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Assert.IsTrue(collection.Contains(new Uri("http://www.example.org/2/")), "Expected true for existing item.");
            Assert.IsFalse(collection.Contains(new Uri("http://www.example.org/4/")),
                "Expected false for non-existing item.");
        }

        [TestMethod]
        public void Contains_UseSpecialValueWhenEmpty_False()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();

            Assert.IsFalse(headers.TransferEncoding.Contains(specialChunked));
        }

        [TestMethod]
        public void Contains_UseSpecialValueWithProperty_True()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();

            headers.TransferEncodingChunked = true;
            Assert.IsTrue(headers.TransferEncoding.Contains(specialChunked));

            headers.TransferEncodingChunked = false;
            Assert.IsFalse(headers.TransferEncoding.Contains(specialChunked));
        }

        [TestMethod]
        public void Contains_UseSpecialValueWhenSpecilValueIsPresent_True()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();

            headers.TransferEncoding.Add(specialChunked);
            Assert.IsTrue(headers.TransferEncoding.Contains(specialChunked));

            headers.TransferEncoding.Remove(specialChunked);
            Assert.IsFalse(headers.TransferEncoding.Contains(specialChunked));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_CallWithStartIndexPlusElementCountGreaterArrayLength_Throw()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));

            Uri[] array = new Uri[2];
            collection.CopyTo(array, 1); // startIndex + Count = 1 + 2 > array.Length
        }

        [TestMethod]
        public void CopyTo_EmptyToEmpty_Success()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            Uri[] array = new Uri[0];
            collection.CopyTo(array, 0);
        }

        [TestMethod]
        public void CopyTo_NoValues_DoesNotChangeArray()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            Uri[] array = new Uri[4];
            collection.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                Assert.IsNull(array[i], "No value expected.");
            }
        }

        [TestMethod]
        public void CopyTo_AddSingleValue_ContainsSingleValue()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/"));

            Uri[] array = new Uri[1];
            collection.CopyTo(array, 0);
            Assert.AreEqual(new Uri("http://www.example.org/"), array[0], "Array value at index 0");

            // Now only set the special value: nothing should be added to the array.
            headers.Clear();
            headers.Add(knownHeader, specialValue.ToString());
            array[0] = null;
            collection.CopyTo(array, 0);
            Assert.AreEqual(specialValue, array[0]);
        }

        [TestMethod]
        public void CopyTo_AddMultipleValues_ContainsAllValuesInTheRightOrder()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Uri[] array = new Uri[5];
            collection.CopyTo(array, 1);

            Assert.IsNull(array[0], "First element should not be modified.");
            Assert.AreEqual(new Uri("http://www.example.org/1/"), array[1], "Array value at index 1");
            Assert.AreEqual(new Uri("http://www.example.org/2/"), array[2], "Array value at index 2");
            Assert.AreEqual(new Uri("http://www.example.org/3/"), array[3], "Array value at index 3");
            Assert.IsNull(array[4], "Last element should not be modified.");
        }

        [TestMethod]
        public void CopyTo_AddValuesAndSpecialValue_AllValuesCopied()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.SetSpecialValue();
            collection.Add(new Uri("http://www.example.org/3/"));

            Uri[] array = new Uri[5];
            collection.CopyTo(array, 1);

            Assert.IsNull(array[0], "First element should not be modified.");
            Assert.AreEqual(new Uri("http://www.example.org/1/"), array[1], "Array value at index 1");
            Assert.AreEqual(new Uri("http://www.example.org/2/"), array[2], "Array value at index 2");
            Assert.AreEqual(specialValue, array[3], "Special value at index 3");
            Assert.AreEqual(new Uri("http://www.example.org/3/"), array[4], "Array value at index 4");

            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
        }

        [TestMethod]
        public void CopyTo_OnlySpecialValue_Copied()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.SetSpecialValue();
            headers.Add(knownHeader, specialValue.ToString());
            headers.Add(knownHeader, specialValue.ToString());
            headers.Add(knownHeader, specialValue.ToString());

            Uri[] array = new Uri[4];
            array[0] = null;
            collection.CopyTo(array, 0);

            Assert.AreEqual(specialValue, array[0]);
            Assert.AreEqual(specialValue, array[1]);
            Assert.AreEqual(specialValue, array[2]);
            Assert.AreEqual(specialValue, array[3]);

            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
        }

        [TestMethod]
        public void CopyTo_OnlySpecialValueEmptyDestination_Copied()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.SetSpecialValue();
            headers.Add(knownHeader, specialValue.ToString());

            Uri[] array = new Uri[2];
            collection.CopyTo(array, 0);

            Assert.AreEqual(specialValue, array[0]);
            Assert.AreEqual(specialValue, array[1]);

            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
        }

        [TestMethod]
        public void CopyTo_ArrayTooSmall_Throw()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(string)));
            HttpHeaderValueCollection<string> collection = new HttpHeaderValueCollection<string>(knownHeader, headers,
                "special");

            string[] array = new string[1];
            array[0] = null;

            collection.CopyTo(array, 0); // no exception
            Assert.IsNull(array[0], "Nothing should be copied.");
            
            ExceptionAssert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0), "null");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(array, -1), "-1");
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(array, 2), "2");

            headers.Add(knownHeader, "special");
            array = new string[0];
            ExceptionAssert.Throws<ArgumentException>(() => collection.CopyTo(array, 0), "array too small (0), Idx: 0");

            headers.Add(knownHeader, "special");
            headers.Add(knownHeader, "special");
            array = new string[1];
            ExceptionAssert.Throws<ArgumentException>(() => collection.CopyTo(array, 0), "array too small (0), Idx: 0");

            headers.Add(knownHeader, "value1");            
            array = new string[0];
            ExceptionAssert.Throws<ArgumentException>(() => collection.CopyTo(array, 0), "array too small (0), Idx: 0");

            headers.Add(knownHeader, "value2");
            array = new string[1];
            ExceptionAssert.Throws<ArgumentException>(() => collection.CopyTo(array, 0), "array too small (1), Idx: 0");

            array = new string[2];
            ExceptionAssert.Throws<ArgumentException>(() => collection.CopyTo(array, 1), "array too small (1), Idx: 1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_CallWithNullValue_Throw()
        {
            MockHeaders headers = new MockHeaders();
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Remove(null);
        }

        [TestMethod]
        public void Remove_AddValuesThenCallRemove_ReturnsTrueWhenRemovingExistingValuesFalseOtherwise()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            Assert.IsTrue(collection.Remove(new Uri("http://www.example.org/2/")), "Expected true for existing item.");
            Assert.IsFalse(collection.Remove(new Uri("http://www.example.org/4/")),
                "Expected false for non-existing item.");
        }

        [TestMethod]
        public void Remove_UseSpecialValue_FalseWhenEmpty()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();
            Assert.IsNull(headers.TransferEncodingChunked);
            Assert.AreEqual(0, headers.TransferEncoding.Count);

            Assert.IsFalse(headers.TransferEncoding.Remove(specialChunked));

            Assert.IsNull(headers.TransferEncodingChunked);
            Assert.AreEqual(0, headers.TransferEncoding.Count);
        }

        [TestMethod]
        public void Remove_UseSpecialValueWhenSetWithProperty_True()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();
            headers.TransferEncodingChunked = true;
            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            // Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.IsTrue(headers.TransferEncoding.Contains(specialChunked));

            Assert.IsTrue(headers.TransferEncoding.Remove(specialChunked));

            Assert.IsFalse((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(0, headers.TransferEncoding.Count);
            Assert.IsFalse(headers.TransferEncoding.Contains(specialChunked));
        }

        [TestMethod]
        public void Remove_UseSpecialValueWhenAdded_True()
        {
            HttpRequestHeaders headers = new HttpRequestHeaders();
            headers.TransferEncoding.Add(specialChunked);
            Assert.IsTrue((bool)headers.TransferEncodingChunked);
            Assert.AreEqual(1, headers.TransferEncoding.Count);
            Assert.IsTrue(headers.TransferEncoding.Contains(specialChunked));

            Assert.IsTrue(headers.TransferEncoding.Remove(specialChunked));

            Assert.IsNull(headers.TransferEncodingChunked);
            Assert.AreEqual(0, headers.TransferEncoding.Count);
            Assert.IsFalse(headers.TransferEncoding.Contains(specialChunked));
        }

        [TestMethod]
        public void GetEnumerator_AddSingleValueAndGetEnumerator_AllValuesReturned()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/"));

            bool started = false;
            foreach (var item in collection)
            {
                Assert.IsFalse(started, "We have more than one element returned by the enumerator.");
                Assert.AreEqual(new Uri("http://www.example.org/"), item, "Item at position 0");
            }
        }

        [TestMethod]
        public void GetEnumerator_AddValuesAndGetEnumerator_AllValuesReturned()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            int i = 1;
            foreach (var item in collection)
            {
                Assert.AreEqual(new Uri("http://www.example.org/" + i + "/"), item, "Item at position {0}", i);
                i++;
            }
        }

        [TestMethod]
        public void GetEnumerator_NoValues_EmptyEnumerator()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            IEnumerator<Uri> enumerator = collection.GetEnumerator();

            Assert.IsFalse(enumerator.MoveNext(), "No items expected in enumerator.");
        }
        
        [TestMethod]
        public void GetEnumerator_AddValuesAndGetEnumeratorFromInterface_AllValuesReturned()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));

            System.Collections.IEnumerable enumerable = collection;

            int i = 1;
            foreach (var item in enumerable)
            {
                Assert.AreEqual(new Uri("http://www.example.org/" + i + "/"), item, "Item at position {0}", i);
                i++;
            }
        }

        [TestMethod]
        public void GetEnumerator_AddValuesAndSpecialValueAndGetEnumeratorFromInterface_AllValuesReturned()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.Add(new Uri("http://www.example.org/3/"));
            collection.SetSpecialValue();

            System.Collections.IEnumerable enumerable = collection;

            // The "special value" should be ignored and not part of the resulting collection.
            int i = 1;
            bool specialFound = false;
            foreach (var item in enumerable)
            {
                if (item.Equals(specialValue))
                {
                    specialFound = true;
                }
                else
                {
                    Assert.AreEqual(new Uri("http://www.example.org/" + i + "/"), item, "Item at position {0}", i);
                    i++;
                }
            }

            Assert.IsTrue(specialFound);

            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
        }
        
        [TestMethod]
        public void GetEnumerator_AddValuesAndSpecialValue_AllValuesReturned()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.SetSpecialValue();
            collection.Add(new Uri("http://www.example.org/3/"));

            System.Collections.IEnumerable enumerable = collection;

            // The special value we added above, must be part of the collection returned by GetEnumerator().
            int i = 1;
            bool specialFound = false;
            foreach (var item in enumerable)
            {
                if (item.Equals(specialValue))
                {
                    specialFound = true;
                }
                else
                {
                    Assert.AreEqual(new Uri("http://www.example.org/" + i + "/"), item, "Item at position {0}", i);
                    i++;
                }
            }

            Assert.IsTrue(specialFound);
            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
        }

        [TestMethod]
        public void IsSpecialValueSet_NoSpecialValueUsed_ReturnsFalse()
        {
            // Create a new collection _without_ specifying a special value.
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                null, null);

            Assert.IsFalse(collection.IsSpecialValueSet, 
                "Special value is set even though collection doesn't define a special value.");
        }

        [TestMethod]
        public void RemoveSpecialValue_AddRemoveSpecialValue_SpecialValueGetsRemoved()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.SetSpecialValue();
            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
            Assert.AreEqual(1, headers.GetValues(knownHeader).Count(), "No. of values in HttpHeaders.");

            collection.RemoveSpecialValue();
            Assert.IsFalse(collection.IsSpecialValueSet, "Special value is set.");

            // Since the only header value was the "special value", removing it will remove the whole header
            // from the collection.
            Assert.IsFalse(headers.Contains(knownHeader));
        }

        [TestMethod]
        public void RemoveSpecialValue_AddValueAndSpecialValueThenRemoveSpecialValue_SpecialValueGetsRemoved()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/"));
            collection.SetSpecialValue();
            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
            Assert.AreEqual(2, headers.GetValues(knownHeader).Count(), "No. of values in HttpHeaders.");

            collection.RemoveSpecialValue();
            Assert.IsFalse(collection.IsSpecialValueSet, "Special value is set.");
            Assert.AreEqual(1, headers.GetValues(knownHeader).Count(), "No. of values in HttpHeaders.");
        }

        [TestMethod]
        public void RemoveSpecialValue_AddTwoValuesAndSpecialValueThenRemoveSpecialValue_SpecialValueGetsRemoved()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue);

            collection.Add(new Uri("http://www.example.org/1/"));
            collection.Add(new Uri("http://www.example.org/2/"));
            collection.SetSpecialValue();
            Assert.IsTrue(collection.IsSpecialValueSet, "Special value not set.");
            Assert.AreEqual(3, headers.GetValues(knownHeader).Count(), "No. of values in HttpHeaders.");

            // The difference between this test and the previous one is that HttpHeaders in this case will use
            // a List<T> to store the two remaining values, whereas in the previous case it will just store
            // the remaining value (no list).
            collection.RemoveSpecialValue();
            Assert.IsFalse(collection.IsSpecialValueSet, "Special value is set.");
            Assert.AreEqual(2, headers.GetValues(knownHeader).Count(), "No. of values in HttpHeaders.");
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void Ctor_ProvideValidator_ValidatorIsUsedWhenAddingValues()
        {
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                MockValidator);

            try
            {
                // Adding an arbitrary Uri should not throw.
                collection.Add(new Uri("http://some/"));
            }
            catch (Exception e)
            {
                Assert.Fail("No exception expected: {0}", e.ToString());
            }

            // When we add 'invalidValue' our MockValidator will throw.
            collection.Add(invalidValue);
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void Ctor_ProvideValidator_ValidatorIsUsedWhenRemovingValues()
        {
            // Use different ctor overload than in previous test to make sure all ctor overloads work correctly.
            MockHeaders headers = new MockHeaders(knownHeader, new MockHeaderParser(typeof(Uri)));
            HttpHeaderValueCollection<Uri> collection = new HttpHeaderValueCollection<Uri>(knownHeader, headers,
                specialValue, MockValidator);

            // When we remove 'invalidValue' our MockValidator will throw.
            collection.Remove(invalidValue);
        }

        [TestMethod]
        public void ToString_SpecialValues_Success()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            request.Headers.TransferEncodingChunked = true;
            string result = request.Headers.TransferEncoding.ToString();
            Assert.AreEqual("chunked", result);

            request.Headers.ExpectContinue = true;
             result = request.Headers.Expect.ToString();
            Assert.AreEqual("100-continue", result);

            request.Headers.ConnectionClose = true;
             result = request.Headers.Connection.ToString();
            Assert.AreEqual("close", result);
        }

        [TestMethod]
        public void ToString_SpecialValueAndExtra_Success()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            request.Headers.Add(HttpKnownHeaderNames.TransferEncoding, "bla1");
            request.Headers.TransferEncodingChunked = true;
            request.Headers.Add(HttpKnownHeaderNames.TransferEncoding, "bla2");
            string result = request.Headers.TransferEncoding.ToString();
            Assert.AreEqual("bla1, chunked, bla2", result);
        }

        [TestMethod]
        public void ToString_SingleValue_Success()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string input = "Basic";
            response.Headers.Add(HttpKnownHeaderNames.WWWAuthenticate, input);
            string result = response.Headers.WwwAuthenticate.ToString();
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ToString_MultipleValue_Success()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string input = "Basic, NTLM, Negotiate, Custom";
            response.Headers.Add(HttpKnownHeaderNames.WWWAuthenticate, input);
            string result = response.Headers.WwwAuthenticate.ToString();
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ToString_EmptyValue_Success()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string result = response.Headers.WwwAuthenticate.ToString();
            Assert.AreEqual(string.Empty, result);
        }

        #region Helper methods

        private static void MockValidator(HttpHeaderValueCollection<Uri> collection, Uri value)
        {
            if (value == invalidValue)
            {
                throw new MockException();
            }
        }

        public class MockException : Exception
        {
            public MockException() { }
            public MockException(string message) : base(message) { }
            public MockException(string message, Exception inner) : base(message, inner) { }
        }

        private class MockHeaders : HttpHeaders
        {
            public MockHeaders()
            {
            }

            public MockHeaders(string headerName, HttpHeaderParser parser)
            {
                Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>();
                parserStore.Add(headerName, parser);
                SetConfiguration(parserStore, new HashSet<string>());
            }
        }

        private class MockHeaderParser : HttpHeaderParser
        {
            private static MockComparer comparer = new MockComparer();
            private Type valueType;

            public override IEqualityComparer Comparer
            {
                get { return comparer; }
            }

            public MockHeaderParser(Type valueType)
                : base(true)
            {
                this.valueType = valueType;
            }

            public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
            {
                parsedValue = null;
                if (value == null)
                {
                    return true;
                }

                index = value.Length;

                // Just return the raw string (as string or Uri depending on the value type)
                if (valueType == typeof(string))
                {
                    parsedValue = value;
                }
                else if (valueType == typeof(Uri))
                {
                    parsedValue = new Uri(value);
                }
                else
                {
                    Assert.Fail("Parser: Unknown value type '{0}'", valueType.ToString());
                }

                return true;
            }
        }
        private class MockComparer : IEqualityComparer
        {
            public int EqualsCount { get; private set; }
            public int GetHashCodeCount { get; private set; }

            #region IEqualityComparer Members

            public new bool Equals(object x, object y)
            {
                EqualsCount++;
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                GetHashCodeCount++;
                return obj.GetHashCode();
            }

            #endregion
        }

        #endregion
    }
}
