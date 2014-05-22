// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class HttpParameterAssert
    {
        private static readonly HttpParameterAssert singleton = new HttpParameterAssert();

        public static HttpParameterAssert Singleton { get { return singleton; } }

        public void AreEqual(HttpParameter expected, HttpParameter actual, string errorMessage)
        {
            Assert.IsNotNull(errorMessage, "Test error: errorMessage is required.");

            if (expected == null)
            {
                Assert.IsNull(actual, string.Format("{0} were expecting null but found non-null.", errorMessage));
            }

            Assert.IsNotNull(actual, string.Format("{0} were expecting non-null but found null.", errorMessage));

            Assert.AreEqual(expected.Name, actual.Name, string.Format("{0} Name mismatch.", errorMessage));
            Assert.AreEqual(expected.ParameterType, actual.ParameterType, string.Format("{0} ParameterType mismatch.", errorMessage));
        }

        public void AreEqual(IEnumerable<HttpParameter> expected, IEnumerable<HttpParameter> actual, string errorMessage)
        {
            Assert.IsNotNull(errorMessage, "Test error: errorMessage is required.");
            Assert.IsNotNull(expected, "Test error: expected cannot be null.");

            Assert.IsNotNull(actual, string.Format("{0} were expecting non-null but found null.", errorMessage));

            HttpParameter[] expectedArray = expected.ToArray();
            HttpParameter[] actualArray = actual.ToArray();

            Assert.AreEqual(expectedArray.Length, actualArray.Length, string.Format("{0} the collections are not the same size.", errorMessage));

            for (int i = 0; i < expectedArray.Length; ++i)
            {
                AreEqual(expectedArray[i], actualArray[i], errorMessage);
            }
        }

        public void Contains(IEnumerable<HttpParameter> collection, HttpParameter single, string errorMessage)
        {
            Assert.IsNotNull(errorMessage, "Test error: errorMessage is required.");
            Assert.IsNotNull(collection, "Test error: collection cannot be null.");
            Assert.IsNotNull(single, "Test error: single cannot be null.");
            bool isContained = collection.Any((hpd) => string.Equals(hpd.Name, single.Name, StringComparison.OrdinalIgnoreCase) && hpd.ParameterType == single.ParameterType);
            Assert.IsTrue(isContained, errorMessage);
        }

        public void ContainsOnly(IEnumerable<HttpParameter> collection, HttpParameter single, string errorMessage)
        {
            Assert.IsNotNull(errorMessage, "Test error: errorMessage is required.");
            Assert.IsNotNull(collection, "Test error: collection cannot be null.");
            Assert.IsNotNull(single, "Test error: single cannot be null.");
            int count = collection.Where((hpd) => string.Equals(hpd.Name, single.Name, StringComparison.OrdinalIgnoreCase) && hpd.ParameterType == single.ParameterType).Count();
            Assert.AreEqual(1, count, errorMessage);
        }


        /// <summary>
        /// Determines if the given type can be converted to a string and converted back to its original value.
        /// </summary>
        /// <param name="value">The value to check.  It cannot be <c>null</c>.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> can be converted to a string and back again.</returns>
        public bool CanConvertToStringAndBack(object value)
        {
            Assert.IsNotNull(value, "Test error: Value cannot be null.");
            Type type = value.GetType();

            // Reference types cannot
            if (!type.IsValueType)
            {
                return false;
            }

            // Min/Max floating point values cannot
            return ((type == typeof(Double) && (((Double)value) == Double.MinValue || ((Double)value) == Double.MaxValue)) ||
                    (type == typeof(Single) && (((Single)value) == Single.MinValue || ((Single)value) == Single.MaxValue)))
                        ? false
                        : true;
        }
    }
}
