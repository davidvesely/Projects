// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.ComponentModel;
    using Microsoft.Server.Common;

    /// <summary>
    /// Internal helper class to validate <see cref="HttpBindingSecurityMode"/> enum values.
    /// </summary>
    internal static class HttpBindingSecurityModeHelper
    {
        private static readonly Type httpBindingSecurityMode = typeof(HttpBindingSecurityMode);

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is defined by the <see cref="HttpBindingSecurityMode"/>
        /// enumeration.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is a valid <see cref="HttpBindingSecurityMode"/> value; otherwise<c> false</c>.</returns>
        public static bool IsDefined(HttpBindingSecurityMode value)
        {
            return value == HttpBindingSecurityMode.None ||
                value == HttpBindingSecurityMode.Transport ||
                value == HttpBindingSecurityMode.TransportCredentialOnly;
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and throws an <see cref="InvalidEnumArgumentException"/>
        /// exception if not valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="parameterName">Name of the parameter to use if throwing exception.</param>
        public static void Validate(HttpBindingSecurityMode value, string parameterName)
        {
            if (!IsDefined(value))
            {
                throw Fx.Exception.AsError(new InvalidEnumArgumentException(parameterName, (int)value, httpBindingSecurityMode));
            }
        }
    }
}
