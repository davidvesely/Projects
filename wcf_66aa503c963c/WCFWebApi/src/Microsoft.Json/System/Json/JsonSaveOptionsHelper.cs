// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.ComponentModel;
    using System.Json;
    using Microsoft.Server.Common;

    /// <summary>
    /// Helper class for validating <see cref="JsonSaveOptions"/> values.
    /// </summary>
    public static class JsonSaveOptionsHelper
    {
        private static readonly Type jsonSaveOptionsType = typeof(JsonSaveOptions);

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is defined by the <see cref="JsonSaveOptions"/>
        /// enumeration.
        /// </summary>
        /// <param name="value">The value to verify.</param>
        /// <returns>
        /// <c>true</c> if the specified options is defined; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefined(JsonSaveOptions value)
        {
            return value == JsonSaveOptions.None ||
                   value == JsonSaveOptions.EnableIndent;
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and throws an <see cref="InvalidEnumArgumentException"/>
        /// exception if not valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="parameterName">Name of the parameter to use if throwing exception.</param>
        public static void Validate(JsonSaveOptions value, string parameterName)
        {
            if (!IsDefined(value))
            {
                throw Fx.Exception.AsError(new InvalidEnumArgumentException(parameterName, (int)value, jsonSaveOptionsType));
            }
        }
    }
}
