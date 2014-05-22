// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace JsonValue.Sample
{
    using System;
    using System.Json;

    /// <summary>
    /// This sample illustrates several scenarios around <see cref="JsonValue"/>.
    /// </summary>
    public static class Program
    {
        static string colorObject = @"{ ""color"": ""red"",	""value"": ""#f00"" }";
        static string colorArray = @"[ { ""color"": ""red"", ""value"": ""#f00"" }, { ""color"": ""green"", ""value"": ""#0f0"" }, { ""color"": ""blue"", ""value"": ""#00f"" } ]";

        private static void BasicObject()
        {
            JsonValue color = JsonValue.Parse(colorObject);
            Console.WriteLine("Found color {0} with value {1}", color["color"], color["value"]);
        }

        private static void BasicArray()
        {
            JsonValue colors = JsonValue.Parse(colorArray);
            Console.WriteLine("Found color array of length {0}", colors.Count);
            foreach (var color in (JsonArray)colors)
            {
                Console.WriteLine("   color: {0} with value {1}", color["color"], color["value"]);
            }
        }

        public static void Main()
        {
            BasicObject();

            BasicArray();
        }
    }
}