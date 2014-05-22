// <copyright file="JsonSaveOptions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    /// <summary>
    /// Specifies serialization options.
    /// <remarks>This enumeration has a <see cref="FlagsAttribute"/> attribute that allows a bitwise combination of its member values.</remarks>
    /// </summary>
    [Flags]
    public enum JsonSaveOptions
    {
        /// <summary>
        /// No options specified.
        /// </summary>
        None            = 0x00,

        /// <summary>
        /// Includes indentation when formatting the JSON while serializing.
        /// </summary>
        EnableIndent    = 0x01
    }
}
