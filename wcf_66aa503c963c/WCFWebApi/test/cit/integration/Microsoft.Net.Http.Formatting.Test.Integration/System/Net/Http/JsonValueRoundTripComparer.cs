// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Collections.Generic;
    using System.Json;
    using System.Text;
    using Microsoft.Server.Common;

    internal class JsonValueRoundTripComparer
    {
        public static bool Compare(JsonValue initValue, JsonValue newValue)
        {
            if (initValue == null && newValue == null)
            {
                return true;
            }

            if (initValue == null || newValue == null)
            {
                return false;
            }

            if (initValue is JsonPrimitive)
            {
                string initStr;
                if (initValue.JsonType == JsonType.String)
                {
                    initStr = initValue.ToString(JsonSaveOptions.None);
                }
                else
                {
                    initStr = string.Format("\"{0}\"", ((JsonPrimitive)initValue).Value.ToString());
                }

                string newStr;
                if (newValue is JsonPrimitive)
                {
                    newStr = newValue.ToString(JsonSaveOptions.None);
                    initStr = UrlUtility.UrlDecode(UrlUtility.UrlEncode(initStr), UTF8Encoding.UTF8);
                    return initStr.Equals(newStr);
                }
                else if (newValue is JsonObject && newValue.Count == 1)
                {
                    initStr = string.Format("{0}", initValue.ToString(JsonSaveOptions.None));
                    return ((JsonObject)newValue).Keys.Contains(initStr);
                }

                return false;
            }

            if (initValue.Count != newValue.Count)
            {
                return false;
            }

            if (initValue is JsonObject && newValue is JsonObject)
            {
                foreach (KeyValuePair<string, JsonValue> item in initValue)
                {
                    if (!Compare(item.Value, newValue[item.Key]))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (initValue is JsonArray && newValue is JsonArray)
            {
                for (int i = 0; i < initValue.Count; i++)
                {
                    if (!Compare(initValue[i], newValue[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}