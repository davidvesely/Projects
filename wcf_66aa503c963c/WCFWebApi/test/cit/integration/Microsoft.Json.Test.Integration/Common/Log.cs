// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;

    internal static class Log
    {
        public static void Info(string text, params object[] args)
        {
            Console.WriteLine(text, args);
        }
    }
}
