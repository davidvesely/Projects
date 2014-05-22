// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.ComponentModel;
    using Microsoft.Server.Common;

    /// <summary>
    /// Helper class for validating <see cref="TrailingSlashMode"/> values.
    /// </summary>
    internal static class TrailingSlashModeHelper
    {
        private static readonly Type trailingSlashModeType = typeof(TrailingSlashMode);

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is defined by the <see cref="TrailingSlashMode"/>
        /// enumeration.
        /// </summary>
        /// <param name="value">The value to verify.</param>
        /// <returns>
        /// <c>true</c> if the specified options is defined; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefined(TrailingSlashMode value)
        {
            return value == TrailingSlashMode.AutoRedirect ||
                   value == TrailingSlashMode.Ignore;
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and throws an <see cref="InvalidEnumArgumentException"/>
        /// exception if not valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="parameterName">Name of the parameter to use if throwing exception.</param>
        public static void Validate(TrailingSlashMode value, string parameterName)
        {
            if (!IsDefined(value))
            {
                throw Fx.Exception.AsError(new InvalidEnumArgumentException(parameterName, (int)value, trailingSlashModeType));
            }
        }
    }
}
