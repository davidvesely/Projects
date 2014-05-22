// <copyright file="JsonValueExtensions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Runtime.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.IO;
    using System.Json;
    using System.Linq.Expressions;
    using System.Text;
    using System.Xml;
    using Microsoft.Server.Common;

    /// <summary>
    /// This class extends the functionality of the <see cref="JsonValue"/> type. 
    /// </summary>
    public static class JsonValueExtensions
    {
        /// <summary>
        /// Creates a <see cref="System.Json.JsonValue"/> object based on an arbitrary CLR object.
        /// </summary>
        /// <param name="value">The object to be converted to <see cref="System.Json.JsonValue"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> which represents the given object.</returns>
        /// <remarks>The conversion is done through the <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>;
        /// the object is first serialized into JSON using the serializer, then parsed into a <see cref="System.Json.JsonValue"/>
        /// object.</remarks>
        public static JsonValue CreateFrom(object value)
        {
            JsonValue jsonValue = null;

            if (value != null)
            {
                jsonValue = value as JsonValue;

                if (jsonValue == null)
                {
                    jsonValue = JsonValueExtensions.CreatePrimitive(value);

                    if (jsonValue == null)
                    {
                        jsonValue = JsonValueExtensions.CreateFromDynamic(value);

                        if (jsonValue == null)
                        {
                            jsonValue = JsonValueExtensions.CreateFromComplex(value);
                        }
                    }
                }
            }

            return jsonValue;
        }

        /// <summary>
        /// Deserializes JSON from a XML reader which implements the
        /// <a href="http://msdn.microsoft.com/en-us/library/bb924435.aspx">mapping between JSON and XML</a>.
        /// </summary>
        /// <param name="jsonReader">The <see cref="System.Xml.XmlDictionaryReader"/> which
        /// exposes JSON as XML.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> that represents the parsed
        /// JSON/XML as a CLR type.</returns>
        public static JsonValue Load(XmlDictionaryReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw Fx.Exception.ArgumentNull("jsonReader");
            }

            return JXmlToJsonValueConverter.JXMLToJsonValue(jsonReader);
        }

        /// <summary>
        /// Serializes this <see cref="System.Json.JsonValue"/> CLR type into a JSON/XML writer using the
        /// <a href="http://msdn.microsoft.com/en-us/library/bb924435.aspx">mapping between JSON and XML</a>.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="jsonWriter">The JSON/XML writer used to serialize this instance.</param>
        public static void Save(this JsonValue jsonValue, XmlDictionaryWriter jsonWriter)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            if (jsonWriter == null)
            {
                throw Fx.Exception.ArgumentNull("jsonWriter");
            }

            JXmlToJsonValueConverter.JsonValueToJXML(jsonWriter, jsonValue);
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="valueOfT">An instance of T initialized with this instance, or the default
        /// value of T, if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as type T; otherwise, false.</returns>
        public static bool TryReadAsType<T>(this JsonValue jsonValue, out T valueOfT)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            object value;
            if (JsonValueExtensions.TryReadAsType(jsonValue, typeof(T), out value))
            {
                valueOfT = (T)value;
                return true;
            }

            valueOfT = default(T);
            return false;
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <returns>An instance of T initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The generic parameter is used to specify the output type")]
        public static T ReadAsType<T>(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            return (T)JsonValueExtensions.ReadAsType(jsonValue, typeof(T));
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T, returning a fallback value
        /// if the conversion fails.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="fallback">A fallback value to be retuned in case the conversion cannot be performed.</param>
        /// <returns>An instance of T initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion succeeds or the specified fallback value if it fails.</returns>
        public static T ReadAsType<T>(this JsonValue jsonValue, T fallback)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            T outVal;
            if (JsonValueExtensions.TryReadAsType<T>(jsonValue, out outVal))
            {
                return outVal;
            }

            return fallback;
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <returns>An object instance initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        public static object ReadAsType(this JsonValue jsonValue, Type type)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            if (type == null)
            {
                throw Fx.Exception.ArgumentNull("type");
            }

            object result;
            if (JsonValueExtensions.TryReadAsType(jsonValue, type, out result))
            {
                return result;
            }

            throw Fx.Exception.AsError(new NotSupportedException(SR.CannotReadAsType(jsonValue.GetType().FullName, type.FullName)));
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as the specified type; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate",
                    Justification = "This is the non-generic version of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception translates to fail.")]
        public static bool TryReadAsType(this JsonValue jsonValue, Type type, out object value)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            if (type == null)
            {
                throw Fx.Exception.ArgumentNull("type");
            }

            if (type == typeof(JsonValue) || type == typeof(object))
            {
                value = jsonValue;
                return true;
            }

            if (type == typeof(object[]) || type == typeof(Dictionary<string, object>))
            {
                if (!JsonValueExtensions.CanConvertToClrCollection(jsonValue, type))
                {
                    value = null;
                    return false;
                }
#if CODEPLEX
                else
                {
                    value = JsonValueExtensions.ToClrCollection(jsonValue, type);
                    return true;
                }
#endif
            }

            if (jsonValue.TryReadAs(type, out value))
            {
                return true;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonValue.Save(ms, JsonSaveOptions.None);
                    ms.Position = 0;
                    DataContractJsonSerializer dcjs = new DataContractJsonSerializer(type);
                    value = dcjs.ReadObject(ms);
                }

                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Formats the <see cref="JsonValue"/> in order to make it easier to read.
        /// </summary>
        /// <param name="jv">The value to format.</param>
        /// <returns>The re-formatted representation.</returns>
        public static string PrettyPrint(JsonValue jv)
        {
            StringBuilder sb = new StringBuilder();
            PrettyPrint(sb, jv, 0, true);
            return sb.ToString();
        }

#if CODEPLEX
        /// <summary>
        /// Extension method for converting a <see cref="JsonValue"/> collection into an array of objects.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance to be converted to an object array.</param>
        /// <returns>An array of objects represented by the specified <see cref="JsonValue"/> instance.</returns>
        /// <remarks>The <see cref="JsonType"/> value of the specified <see cref="JsonValue"/> instance must be <see cref="JsonType.Array"/>.</remarks>
        public static object[] ToObjectArray(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            if (jsonValue.JsonType != JsonType.Array)
            {
                throw Fx.Exception.AsError(new NotSupportedException(SR.OperationNotSupportedOnJsonType(jsonValue.JsonType)));
            }

            return ToClrCollection<object[]>(jsonValue);
        }

        /// <summary>
        /// Extension method for converting a <see cref="JsonValue"/> collection into a dictionary of <see cref="string"/>/<see cref="object"/> key/value pairs.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance to be converted to a dictionary of <see cref="string"/>/<see cref="object"/> key/value pairs.</param>
        /// <returns>An <see cref="IDictionary{T1,T2}"/> of <see cref="string"/>/<see cref="object"/> key/value pairs represented by the specified <see cref="JsonValue"/> instance.</returns>
        /// <remarks>The <see cref="JsonType"/> value of the specified <see cref="JsonValue"/> instance must be <see cref="JsonType.Object"/>.</remarks>
        public static IDictionary<string, object> ToDictionary(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw Fx.Exception.ArgumentNull("jsonValue");
            }

            if (jsonValue.JsonType != JsonType.Object)
            {
                throw Fx.Exception.AsError(new NotSupportedException(SR.OperationNotSupportedOnJsonType(jsonValue.JsonType)));
            }

            return ToClrCollection<Dictionary<string, object>>(jsonValue);
        }
#endif

        private static void PrettyPrint(StringBuilder sb, JsonValue jv, int indent, bool first)
        {
            if (jv == null)
            {
                AddIndent(sb, indent);
                sb.Append("null");
            }
            else
            {
                int i;
                switch (jv.JsonType)
                {
                    case JsonType.Array:
                        if (!first)
                        {
                            AddIndent(sb, indent);
                        }

                        sb.AppendLine("[");
                        for (i = 0; i < jv.Count; i++)
                        {
                            PrettyPrint(sb, jv[i], indent + 1, false);
                            if (i < jv.Count - 1)
                            {
                                sb.AppendLine(",");
                            }
                            else
                            {
                                sb.AppendLine();
                            }
                        }

                        AddIndent(sb, indent);
                        sb.Append(']');
                        break;
                    case JsonType.Object:
                        if (!first)
                        {
                            AddIndent(sb, indent);
                        }
                        else
                        {
                            first = false;
                        }

                        sb.AppendLine("{");
                        i = 0;
                        foreach (string key in ((JsonObject)jv).Keys)
                        {
                            PrettyPrint(sb, new JsonPrimitive(key), indent + 1, true);
                            sb.Append(": ");
                            JsonValue value = jv[key];
                            if (value == null || value.JsonType == JsonType.Boolean || value.JsonType == JsonType.Number || value.JsonType == JsonType.String)
                            {
                                PrettyPrint(sb, value, 0, true);
                            }
                            else
                            {
                                PrettyPrint(sb, value, indent + 1, true);
                            }

                            i++;
                            if (i < jv.Count)
                            {
                                sb.AppendLine(",");
                            }
                            else
                            {
                                sb.AppendLine();
                            }
                        }

                        AddIndent(sb, indent);
                        sb.Append('}');
                        break;
                    case JsonType.Boolean:
                    case JsonType.Number:
                    case JsonType.String:
                        AddIndent(sb, indent);
                        sb.Append(jv.ToString(JsonSaveOptions.None));
                        break;
                }
            }
        }

        private static void AddIndent(StringBuilder sb, int indent)
        {
            for (int i = 0; i < indent * 2; i++)
            {
                sb.Append(" ");
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="JsonValue"/> instance can be converted to the specified collection <see cref="Type"/>.
        /// </summary>
        /// <param name="jsonValue">The instance to be converted.</param>
        /// <param name="collectionType">The collection type to convert the instance to.</param>
        /// <returns>true if the instance can be converted, false otherwise</returns>
        private static bool CanConvertToClrCollection(JsonValue jsonValue, Type collectionType)
        {
            if (jsonValue != null)
            {
                return (jsonValue.JsonType == JsonType.Object && collectionType == typeof(Dictionary<string, object>)) ||
                       (jsonValue.JsonType == JsonType.Array && collectionType == typeof(object[]));
            }

            return false;
        }
#if CODEPLEX
        /// <summary>
        /// Converts this instance to a collection containing <see cref="object"/> type instances corresponding to the underlying
        /// elements of this instance.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance to convert to the object collection.</param>
        /// <typeparam name="T">The type of the collection to convert this instance to.</typeparam>
        /// <returns>An object representing a CLR collection depending on the <see cref="JsonType"/> value of this instance and the specified type value.</returns>
        private static T ToClrCollection<T>(JsonValue jsonValue)
        {
            return (T)ToClrCollection(jsonValue, typeof(T));
        }

        /// <summary>
        /// Converts this instance to a collection containing <see cref="object"/> type instances corresponding to the underlying
        /// elements of this instance.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance to convert to the object collection.</param>
        /// <param name="type">The <see cref="Type"/> of the collection to convert this instance to.</param>
        /// <returns>An object representing a CLR collection depending on the <see cref="JsonType"/> value of this instance and the specified type value.</returns>
        private static object ToClrCollection(JsonValue jsonValue, Type type)
        {
            object collection = null;

            if (CanConvertToClrCollection(jsonValue, type))
            {
                JsonValue parentValue = jsonValue;
                Queue<KeyValuePair<string, JsonValue>> childValues = null;
                Stack<ToClrCollectionStackInfo> stackInfo = new Stack<ToClrCollectionStackInfo>();
                int currentIndex = 0;

                collection = JsonValueExtensions.CreateClrCollection(parentValue);

                do
                {
                    if (childValues == null)
                    {
                        childValues = new Queue<KeyValuePair<string, JsonValue>>(parentValue);
                    }

                    while (childValues != null && childValues.Count > 0)
                    {
                        KeyValuePair<string, JsonValue> item = childValues.Dequeue();
                        JsonValue childValue = item.Value;

                        switch (childValue.JsonType)
                        {
                            case JsonType.Array:
                            case JsonType.Object:
                                object childCollection = JsonValueExtensions.CreateClrCollection(childValue);

                                InsertClrItem(collection, ref currentIndex, item.Key, childCollection);

                                stackInfo.Push(new ToClrCollectionStackInfo(parentValue, collection, currentIndex, childValues));
                                parentValue = item.Value;
                                childValues = null;
                                collection = childCollection;
                                currentIndex = 0;
                                break;

                            default:
                                InsertClrItem(collection, ref currentIndex, item.Key, item.Value.Read());
                                break;
                        }
                    }

                    if (childValues != null && stackInfo.Count > 0)
                    {
                        ToClrCollectionStackInfo info = stackInfo.Pop();
                        collection = info.Collection;
                        childValues = info.JsonValueChildren;
                        parentValue = info.ParentJsonValue;
                        currentIndex = info.CurrentIndex;
                    }
                }
                while (stackInfo.Count > 0 || childValues == null || childValues.Count > 0);
            }

            return collection;
        }

        private static object CreateClrCollection(JsonValue jsonValue)
        {
            if (jsonValue.JsonType == JsonType.Object)
            {
                return new Dictionary<string, object>(jsonValue.Count);
            }

            return new object[jsonValue.Count];
        }

        private static void InsertClrItem(object collection, ref int index, string key, object value)
        {
            Dictionary<string, object> dictionary = collection as Dictionary<string, object>;
            if (dictionary != null)
            {
                dictionary.Add(key, value);
                return;
            }

            object[] array = collection as object[];
            array[index] = value;
            index++;
        }

#endif

        private static JsonValue CreatePrimitive(object value)
        {
            JsonPrimitive jsonPrimitive;

            if (JsonPrimitive.TryCreate(value, out jsonPrimitive))
            {
                return jsonPrimitive;
            }

            return null;
        }

        private static JsonValue CreateFromComplex(object value)
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(value.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                dcjs.WriteObject(ms, value);
                ms.Position = 0;
                return JsonValue.Load(ms);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "value is not the same")]
        private static JsonValue CreateFromDynamic(object value)
        {
            JsonObject parent = null;
            DynamicObject dynObj = value as DynamicObject;

            if (dynObj != null)
            {
                parent = new JsonObject();
                Stack<CreateFromTypeStackInfo> infoStack = new Stack<CreateFromTypeStackInfo>();
                IEnumerator<string> keys = null;

                do
                {
                    if (keys == null)
                    {
                        keys = dynObj.GetDynamicMemberNames().GetEnumerator();
                    }

                    while (keys.MoveNext())
                    {
                        JsonValue child = null;
                        string key = keys.Current;
                        SimpleGetMemberBinder binder = new SimpleGetMemberBinder(key);

                        if (dynObj.TryGetMember(binder, out value))
                        {
                            DynamicObject childDynObj = value as DynamicObject;

                            if (childDynObj != null)
                            {
                                child = new JsonObject();
                                parent.Add(key, child);

                                infoStack.Push(new CreateFromTypeStackInfo(parent, dynObj, keys));

                                parent = child as JsonObject;
                                dynObj = childDynObj;
                                keys = null;

                                break;
                            }
                            else
                            {
                                if (value != null)
                                {
                                    child = value as JsonValue;

                                    if (child == null)
                                    {
                                        child = JsonValueExtensions.CreatePrimitive(value);

                                        if (child == null)
                                        {
                                            child = JsonValueExtensions.CreateFromComplex(value);
                                        }
                                    }
                                }

                                parent.Add(key, child);
                            }
                        }
                    }

                    if (infoStack.Count > 0 && keys != null)
                    {
                        CreateFromTypeStackInfo info = infoStack.Pop();

                        parent = info.JsonObject;
                        dynObj = info.DynamicObject;
                        keys = info.Keys;
                    }
                }
                while (infoStack.Count > 0);
            }

            return parent;
        }

        private class CreateFromTypeStackInfo
        {
            public CreateFromTypeStackInfo(JsonObject jsonObject, DynamicObject dynamicObject, IEnumerator<string> keyEnumerator)
            {
                this.JsonObject = jsonObject;
                this.DynamicObject = dynamicObject;
                this.Keys = keyEnumerator;
            }

            /// <summary>
            /// Gets of sets
            /// </summary>
            public JsonObject JsonObject { get; set; }

            /// <summary>
            /// Gets of sets
            /// </summary>
            public DynamicObject DynamicObject { get; set; }

            /// <summary>
            /// Gets of sets
            /// </summary>
            public IEnumerator<string> Keys { get; set; }
        }
#if CODEPLEX
        private class ToClrCollectionStackInfo
        {
            public ToClrCollectionStackInfo(JsonValue jsonValue, object collection, int currentIndex, Queue<KeyValuePair<string, JsonValue>> iterator)
            {
                this.ParentJsonValue = jsonValue;
                this.CurrentIndex = currentIndex;
                this.Collection = collection;
                this.JsonValueChildren = iterator;
            }

            public JsonValue ParentJsonValue { get; set; }

            public object Collection { get; set; }

            public int CurrentIndex { get; set; }

            public Queue<KeyValuePair<string, JsonValue>> JsonValueChildren { get; set; }
        }
#endif

        private class SimpleGetMemberBinder : GetMemberBinder
        {
            public SimpleGetMemberBinder(string name)
                : base(name, false)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                if (target != null && errorSuggestion == null)
                {
                    string exceptionMessage = SR.DynamicPropertyNotDefined(target.LimitType, this.Name);
                    Expression throwExpression = Expression.Throw(Expression.Constant(new InvalidOperationException(exceptionMessage)), typeof(object));

                    errorSuggestion = new DynamicMetaObject(throwExpression, target.Restrictions);
                }

                return errorSuggestion;
            }
        }
    }
}
