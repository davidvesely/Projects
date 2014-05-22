// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// MSTest utility for testing <see cref="HttpResponseMessage"/> instances.
    /// </summary>
    public class HttpAssert
    {
        private const string CommaSeperator = ", ";
        private static readonly HttpAssert singleton = new HttpAssert();

        public static HttpAssert Singleton { get { return singleton; } }

        /// <summary>
        /// Asserts that the expected <see cref="HttpRequestMessage"/> is equal to the actual <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="HttpRequestMessage"/>. Should not be <c>null</c>.</param>
        /// <param name="actual">The actual <see cref="HttpRequestMessage"/>. Should not be <c>null</c>.</param>
        public void AreEqual(HttpRequestMessage expected, HttpRequestMessage actual)
        {
            Assert.IsNotNull(expected, "The 'expected' parameter should not be null.");
            Assert.IsNotNull(actual, "The 'actual' parameter should not be null.");

            Assert.AreEqual(expected.Version, actual.Version, "The http version of the responses should have been the same.");
            AreEqual(expected.Headers, actual.Headers);

            if (expected.Content == null)
            {
                Assert.IsNull(actual.Content, "The response content should have been null.");
            }
            else
            {
                string expectedContent = CleanContentString(expected.Content.ReadAsStringAsync().Result);
                string actualContent = CleanContentString(actual.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(expectedContent, actualContent, "The content of the requests should have been the same.");
                AreEqual(expected.Content.Headers, actual.Content.Headers);
            }
        }

        /// <summary>
        /// Asserts that the expected <see cref="HttpResponseMessage"/> is equal to the actual <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="HttpResponseMessage"/>. Should not be <c>null</c>.</param>
        /// <param name="actual">The actual <see cref="HttpResponseMessage"/>. Should not be <c>null</c>.</param>
        public void AreEqual(HttpResponseMessage expected, HttpResponseMessage actual)
        {
            AreEqual(expected, actual, null);
        }

        /// <summary>
        /// Asserts that the expected <see cref="HttpResponseMessage"/> is equal to the actual <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="HttpResponseMessage"/>. Should not be <c>null</c>.</param>
        /// <param name="actual">The actual <see cref="HttpResponseMessage"/>. Should not be <c>null</c>.</param>
        /// <param name="verifyContentCallback">The callback to verify the Content string. If it is null, Assert.AreEqual will be used. </param>
        public void AreEqual(HttpResponseMessage expected, HttpResponseMessage actual, Action<string, string> verifyContentStringCallback)
        {
            Assert.IsNotNull(expected, "The 'expected' parameter should not be null.");
            Assert.IsNotNull(actual, "The 'actual' parameter should not be null.");

            Assert.AreEqual(expected.StatusCode, actual.StatusCode, "The status code of the responses should have been the same.");
            Assert.AreEqual(expected.ReasonPhrase, actual.ReasonPhrase, "The reason phrase of the responses should have been the same.");
            Assert.AreEqual(expected.Version, actual.Version, "The http version of the responses should have been the same.");
            AreEqual(expected.Headers, actual.Headers);

            if (expected.Content == null)
            {
                Assert.IsNull(actual.Content, "The response content should have been null.");
            }
            else
            {
                string expectedContent = CleanContentString(expected.Content.ReadAsStringAsync().Result);
                string actualContent = CleanContentString(actual.Content.ReadAsStringAsync().Result);
                if (verifyContentStringCallback != null)
                {
                    verifyContentStringCallback(expectedContent, actualContent);
                }
                else
                {
                    Assert.AreEqual(expectedContent, actualContent, "The content of the responses should have been the same.");
                }
                AreEqual(expected.Content.Headers, actual.Content.Headers);
            }
        }

        /// <summary>
        /// Asserts that the expected <see cref="HttpHeaders"/> instance is equal to the actual <see cref="actualHeaders"/> instance.
        /// </summary>
        /// <param name="expectedHeaders">The expected <see cref="HttpHeaders"/> instance. Should not be <c>null</c>.</param>
        /// <param name="actualHeaders">The actual <see cref="HttpHeaders"/> instance. Should not be <c>null</c>.</param>
        public void AreEqual(HttpHeaders expectedHeaders, HttpHeaders actualHeaders)
        {
            Assert.IsNotNull(expectedHeaders, "The 'expectedHeaders' parameter should not be null.");
            Assert.IsNotNull(actualHeaders, "The 'actualHeaders' parameter should not be null.");

            Assert.AreEqual(expectedHeaders.Count(), actualHeaders.Count(), "The number of headers should have been the same.");

            foreach (KeyValuePair<string, IEnumerable<string>> expectedHeader in expectedHeaders)
            {
                KeyValuePair<string, IEnumerable<string>> actualHeader = actualHeaders.FirstOrDefault(h => h.Key == expectedHeader.Key);
                Assert.IsNotNull(actualHeader, string.Format("The '{0}' header was expected but not found.", expectedHeader.Key));

                if (expectedHeader.Key == "Date")
                {
                    HandleDateHeader(expectedHeader.Value.ToArray(), actualHeader.Value.ToArray());
                }
                else
                {
                    string expectedHeaderStr = string.Join(CommaSeperator, expectedHeader.Value);
                    string actualHeaderStr = string.Join(CommaSeperator, actualHeader.Value);
                    Assert.AreEqual(expectedHeaderStr, actualHeaderStr, string.Format("The '{0}' header disagreed with the expected header value.", expectedHeader.Key));
                }
            }
        }

        /// <summary>
        /// Asserts the given <see cref="HttpHeaders"/> contain the given <paramref name="values"/>
        /// for the given <paramref name="name"/>.
        /// </summary>
        /// <param name="headers">The <see cref="HttpHeaders"/> to examine.  It cannot be <c>null</c>.</param>
        /// <param name="name">The name of the header.  It cannot be empty.</param>
        /// <param name="values">The values that must all be present.  It cannot be null.</param>
        public void Contains(HttpHeaders headers, string name, params string[] values)
        {
            Assert.IsNotNull(headers, "Test error: headers cannot be null.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(name), "Test error: name cannot be empty.");
            Assert.IsNotNull(values, "Test error: values cannot be null.");

            IEnumerable<string> headerValues = null;
            bool foundIt = headers.TryGetValues(name, out headerValues);
            Assert.IsTrue(foundIt, "Headers did not contain a header named " + name);
            CollectionAssert.IsSubsetOf(values.ToList(), headerValues.ToList(), "Headers did not contain any or all of the expected headers.");
        }

        public bool IsKnownUnserializableType(Type type, Func<Type, bool> isTypeUnserializableCallback)
        {
            if (isTypeUnserializableCallback != null && isTypeUnserializableCallback(type))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    if (type.GetMethod("Add") == null)
                    {
                        return true;
                    }
                }

                // Generic type -- recursively analyze generic arguments
                return IsKnownUnserializableType(type.GetGenericArguments()[0], isTypeUnserializableCallback);
            }

            if (type.HasElementType && IsKnownUnserializableType(type.GetElementType(), isTypeUnserializableCallback))
            {
                return true;
            }

            return false;
        }

        public bool IsKnownUnserializable(Type type, object obj, Func<Type, bool> isTypeUnserializableCallback)
        {
            if (IsKnownUnserializableType(type, isTypeUnserializableCallback))
            {
                return true;
            }

            return obj != null && IsKnownUnserializableType(obj.GetType(), isTypeUnserializableCallback);
        }

        public bool IsKnownUnserializable(Type type, object obj)
        {
            return IsKnownUnserializable(type, obj, null);
        }

        public bool CanRoundTrip(Type type)
        {
            if (typeof(TimeSpan).IsAssignableFrom(type))
            {
                return false;
            }

            if (typeof(DateTimeOffset).IsAssignableFrom(type))
            {
                return false;
            }

            if (type.IsGenericType)
            {
                foreach (Type genericParameterType in type.GetGenericArguments())
                {
                    if (!CanRoundTrip(genericParameterType))
                    {
                        return false;
                    }
                }
            }

            if (type.HasElementType)
            {
                return CanRoundTrip(type.GetElementType());
            }

            return true;
        }

        private static void HandleDateHeader(string[] expectedDateHeaderValues, string[] actualDateHeaderValues)
        {
            Assert.AreEqual(expectedDateHeaderValues.Length, actualDateHeaderValues.Length, "The 'Date' header value count disagreed with the expected 'Date' header value count.");

            for (int i = 0; i < expectedDateHeaderValues.Length; i++)
            {
                DateTime expectedDateTime = DateTime.Parse(expectedDateHeaderValues[i]);
                DateTime actualDateTime = DateTime.Parse(actualDateHeaderValues[i]);

                Assert.AreEqual(expectedDateTime.Year, actualDateTime.Year, "The 'Date' header year disagreed with the expected 'Date' header year.");
                Assert.AreEqual(expectedDateTime.Month, actualDateTime.Month, "The 'Date' header month disagreed with the expected 'Date' header month.");
                Assert.AreEqual(expectedDateTime.Day, actualDateTime.Day, "The 'Date' header day disagreed with the expected 'Date' header day.");

                int hourDifference = Math.Abs(actualDateTime.Hour - expectedDateTime.Hour);
                Assert.IsTrue(hourDifference <= 1, "The 'Date' header hours disagreed with the expected 'Date' header hours by more than a single hour.");

                int minuteDifference = Math.Abs(actualDateTime.Minute - expectedDateTime.Minute);
                Assert.IsTrue(minuteDifference <= 1, "The 'Date' header minutes disagreed with the expected 'Date' header minutes by more than a single minute.");
            }
        }

        private static string CleanContentString(string content)
        {
            Assert.IsNotNull(content, "The 'content' parameter should not be null.");
            
            string cleanedContent = null;

            // remove any port numbers from Uri's
            cleanedContent = Regex.Replace(content, ":\\d+", "");
            
            return cleanedContent;
        }
    }
}
