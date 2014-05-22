// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Json;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Microsoft.Server.Common;

    /// <summary>
    /// Extension methods to allow strongly typed objects to be read from the query component of <see cref="Uri"/> instances.
    /// </summary>
    public static class UriExtensionMethods
    {
        /// <summary>
        /// Reads HTML form URL encoded data provided in the <see cref="Uri"/> query component as a <see cref="JsonValue"/> object.
        /// </summary>
        /// <param name="address">The <see cref="Uri"/> instance from which to read.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if the query component can be read as <see cref="JsonValue"/>; otherwise, false.</returns>
        public static bool TryReadQueryAsJson(this Uri address, out JsonObject value)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            IEnumerable<Tuple<string, string>> query = ParseQueryString(address.Query);
            return FormUrlEncodedJson.TryParse(query, out value);
        }

        /// <summary>
        /// Reads HTML form URL encoded data provided in the <see cref="Uri"/> query component as an <see cref="Object"/> of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="address">The <see cref="Uri"/> instance from which to read.</param>
        /// <param name="type">The type of the object to read.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if the query component can be read as the specified type; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "This is the non-generic version.")]
        public static bool TryReadQueryAs(this Uri address, Type type, out object value)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            IEnumerable<Tuple<string, string>> query = ParseQueryString(address.Query);
            JsonObject jsonObject;
            if (FormUrlEncodedJson.TryParse(query, out jsonObject))
            {
                return jsonObject.TryReadAsType(type, out value); 
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Reads HTML form URL encoded data provided in the <see cref="Uri"/> query component as an <see cref="Object"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="address">The <see cref="Uri"/> instance from which to read.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if the query component can be read as the specified type; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static bool TryReadQueryAs<T>(this Uri address, out T value)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            IEnumerable<Tuple<string, string>> query = ParseQueryString(address.Query);
            JsonObject jsonObject;
            if (FormUrlEncodedJson.TryParse(query, out jsonObject))
            {
                return jsonObject.TryReadAsType<T>(out value);
            }

            value = default(T);
            return false;
        }

        private static IEnumerable<Tuple<string, string>> ParseQueryString(string queryString)
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                if ((queryString.Length > 0) && (queryString[0] == '?'))
                {
                    queryString = queryString.Substring(1);
                }

                if (!string.IsNullOrEmpty(queryString))
                {
                    string[] pairs = queryString.Split('&');
                    foreach (string str in pairs)
                    {
                        string[] keyValue = str.Split('=');
                        if (keyValue.Length == 2)
                        {
                            yield return
                                keyValue[1].Equals(FormattingUtilities.JsonNullLiteral, StringComparison.Ordinal) ?
                                new Tuple<string, string>(UrlUtility.UrlDecode(keyValue[0], Encoding.UTF8), null) :
                                new Tuple<string, string>(UrlUtility.UrlDecode(keyValue[0], Encoding.UTF8), UrlUtility.UrlDecode(keyValue[1], Encoding.UTF8));
                        }
                        else if (keyValue.Length == 1)
                        {
                            yield return new Tuple<string, string>(null, UrlUtility.UrlDecode(keyValue[0], Encoding.UTF8));
                        }
                    }
                }
            }
        }
    }
}