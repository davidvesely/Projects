// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.Server.Common;

    /// <summary>
    /// Class that can convert from values of one type to values of another type based on 
    /// the rules of binding <see cref="HttpParameter"/> instances.
    /// </summary>
    internal abstract class HttpParameterValueConverter
    {
        private static readonly Type[] booleanType = new Type[] { typeof(bool) };
        private static readonly HttpParameterValueConverter uriValueConverter = new UriValueConverter();

        private static readonly HttpParameterValueConverters NullableValueConverters = new HttpParameterValueConverters()
        {
            new BooleanValueConverter(true),
            new CharValueConverter(true),
            new SByteValueConverter(true),
            new ByteValueConverter(true),
            new Int16ValueConverter(true),
            new UInt16ValueConverter(true),
            new Int32ValueConverter(true),
            new UInt32ValueConverter(true),
            new Int64ValueConverter(true),
            new UInt64ValueConverter(true),
            new SingleValueConverter(true),
            new DoubleValueConverter(true),
            new DecimalValueConverter(true),
            new DateTimeValueConverter(true),
            new TimeSpanValueConverter(true),
            new GuidValueConverter(true),
            new DateTimeOffsetValueConverter(true)
        };

        private static readonly HttpParameterValueConverters ValueConverters = new HttpParameterValueConverters()
        {
            new BooleanValueConverter(false),
            new CharValueConverter(false),
            new SByteValueConverter(false),
            new ByteValueConverter(false),
            new Int16ValueConverter(false),
            new UInt16ValueConverter(false),
            new Int32ValueConverter(false),
            new UInt32ValueConverter(false),
            new Int64ValueConverter(false),
            new UInt64ValueConverter(false),
            new SingleValueConverter(false),
            new DoubleValueConverter(false),
            new DecimalValueConverter(false),
            new DateTimeValueConverter(false),
            new TimeSpanValueConverter(false),
            new GuidValueConverter(false),
            new DateTimeOffsetValueConverter(false)
        };

        private static readonly Type enumValueConverterGenericType = typeof(EnumValueConverter<>);
        private static readonly Type nonNullableValueConverterGenericType = typeof(DefaultValueConverter<>);
        private static readonly Type refValueConverterGenericType = typeof(NoOpValueConverter<>);
        private static readonly Type objectContentValueConverterGenericType = typeof(ObjectContentValueConverter<>);

        protected HttpParameterValueConverter(Type type)
        {
            Fx.Assert(type != null, "The 'type' parameter should not be null.");

            this.Type = type;
        }

        public Type Type { get; private set; }

        public bool CanConvertFromString { get; protected set; }

        public static HttpParameterValueConverter GetValueConverter(Type type)
        {
            if (type == null)
            {
                throw Fx.Exception.ArgumentNull("type");
            }

            Type objectContentTypeArg = HttpTypeHelper.GetHttpContentInnerTypeOrNull(type);

            if (objectContentTypeArg != null)
            {
                Type closedConverterType = objectContentValueConverterGenericType.MakeGenericType(new Type[] { objectContentTypeArg });
                ConstructorInfo constructor = closedConverterType.GetConstructor(Type.EmptyTypes);
                return constructor.Invoke(null) as HttpParameterValueConverter;
            }

            if (HttpTypeHelper.IsHttp(type))
            {
                Type closedConverterType = refValueConverterGenericType.MakeGenericType(new Type[] { type });
                ConstructorInfo constructor = closedConverterType.GetConstructor(Type.EmptyTypes);
                return constructor.Invoke(null) as HttpParameterValueConverter;
            }

            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            bool typeIsNullable = nullableUnderlyingType != null;
            Type actualType = typeIsNullable ? nullableUnderlyingType : type;

            if (actualType.IsEnum)
            {
                Type closedConverterType = enumValueConverterGenericType.MakeGenericType(new Type[] { actualType });
                ConstructorInfo constructor = closedConverterType.GetConstructor(booleanType);
                return constructor.Invoke(new object[] { typeIsNullable }) as HttpParameterValueConverter;
            }

            if (typeIsNullable && NullableValueConverters.Contains(actualType))
            {
                return NullableValueConverters[actualType];
            }
            else if (ValueConverters.Contains(actualType))
            {
                return ValueConverters[actualType];
            }

            if (actualType == typeof(Uri))
            {
                return uriValueConverter;
            }

            Type closedValueConverterType = nonNullableValueConverterGenericType.MakeGenericType(new Type[] { actualType });
            ConstructorInfo valueConverterConstructor = closedValueConverterType.GetConstructor(Type.EmptyTypes);
            return valueConverterConstructor.Invoke(null) as HttpParameterValueConverter;
        }

        public abstract object Convert(object value);

        public abstract bool IsInstanceOf(object value);

        public abstract bool CanConvertFromType(Type type);

        protected virtual object ConvertFromString(string value)
        {
            return null;
        }

        private class HttpParameterValueConverters : KeyedCollection<Type, HttpParameterValueConverter>
        {
            protected override Type GetKeyForItem(HttpParameterValueConverter item)
            {
                return item.Type;
            }
        }

        private class NoOpValueConverter<T> : HttpParameterValueConverter
        {
            public NoOpValueConverter()
                : base(typeof(T))
            {
            }

            public override object Convert(object value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is T)
                {
                    return value;
                }

                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Http.SR.ValueConversionFailed(value.GetType().FullName, this.Type.FullName)));
            }

            public override bool IsInstanceOf(object value)
            {
                return value == null ?
                    true :
                    value is T;
            }

            public override bool CanConvertFromType(Type type)
            {
                if (type == null)
                {
                    throw Fx.Exception.ArgumentNull("type");
                }

                return this.Type.IsAssignableFrom(type);
            }
        }

        private class ObjectContentValueConverter<T> : HttpParameterValueConverter
        {
            public ObjectContentValueConverter()
                : base(typeof(ObjectContent<T>))
            {
            }

            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Casting is limited to two times due to the if statements.")]
            public override object Convert(object value)
            {
                if (value == null)
                {
                    return null;
                }
                else if (value is ObjectContent<T>)
                {
                    return value;
                }
                else if (value is HttpRequestMessage<T>)
                {
                    return ((HttpRequestMessage<T>)value).Content;
                }
                else if (value is HttpResponseMessage<T>)
                {
                    return ((HttpResponseMessage<T>)value).Content;
                }

                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Http.SR.ValueConversionFailed(value.GetType().FullName, this.Type.FullName)));
            }

            public override bool IsInstanceOf(object value)
            {
                return value == null ?
                    true :
                    value is ObjectContent<T>;
            }

            public override bool CanConvertFromType(Type type)
            {
                if (type == null)
                {
                    throw Fx.Exception.ArgumentNull("type");
                }

                return this.Type.IsAssignableFrom(type) ||
                       typeof(HttpRequestMessage<T>).IsAssignableFrom(type) ||
                       typeof(HttpResponseMessage<T>).IsAssignableFrom(type);
            }
        }

        private class DefaultValueConverter<T> : HttpParameterValueConverter
        {
            private object defaultValue;

            public DefaultValueConverter() 
                : base(typeof(T))
            {
                this.defaultValue = default(T);
            }

            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Casting is limited to two times due to the if statements.")]
            public override object Convert(object value)
            {
                if (value == null)
                {
                    return this.defaultValue;
                }
                else if (value is T)
                {
                    return value;
                }
                else if (this.CanConvertFromString && value is string)
                {
                    string valueAsString = (string)value;
                    if (string.IsNullOrWhiteSpace(valueAsString))
                    {
                        return null;
                    }

                    return this.ConvertFromString((string)value);
                }
                else if (value is HttpRequestMessage<T>)
                {
                    HttpRequestMessage<T> valueAsRequest = (HttpRequestMessage<T>)value;
                    return valueAsRequest.Content.ReadAsAsync().Result;
                }
                else if (value is HttpResponseMessage<T>)
                {
                    HttpResponseMessage<T> valueAsResponse = (HttpResponseMessage<T>)value;
                    return valueAsResponse.Content.ReadAsAsync().Result;
                }
                else if (value is ObjectContent<T>)
                {
                    ObjectContent<T> valueAsContent = (ObjectContent<T>)value;
                    return valueAsContent.ReadAsOrDefaultAsync().Result;
                }

                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Http.SR.ValueConversionFailed(value.GetType().FullName, this.Type.FullName)));
            }

            public override bool IsInstanceOf(object value)
            {
                return value == null ?
                    true :
                    value is T;
            }

            public sealed override bool CanConvertFromType(Type type)
            {
                if (type == null)
                {
                    throw Fx.Exception.ArgumentNull("type");
                }

                if (this.Type.IsAssignableFrom(type)) 
                {
                    return true;
                }

                if (type == TypeHelper.StringType && this.CanConvertFromString)
                {
                    return true;
                }

                if (typeof(HttpRequestMessage<T>).IsAssignableFrom(type) ||
                    typeof(HttpResponseMessage<T>).IsAssignableFrom(type) ||
                    typeof(ObjectContent<T>).IsAssignableFrom(type))
                {
                    return true;
                }

                return false;
            }
        }

        private class ValueConverter<T> : HttpParameterValueConverter where T : struct
        {
            private object defaultValue;

            public ValueConverter(bool isNullable)
                : base(typeof(T))
            {
                if (!isNullable)
                {
                    this.defaultValue = default(T);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Casting is limited to two times due to the if statements.")]
            public override object Convert(object value)
            {
                if (value == null)
                {
                    return this.defaultValue;
                }
                else if (value is T)
                {
                    return value;
                }
                else if (this.CanConvertFromString && value is string)
                {
                    string valueAsString = (string)value;
                    if (string.IsNullOrWhiteSpace(valueAsString))
                    {
                        return null;
                    }

                    return this.ConvertFromString((string)value);
                }
                else if (value is HttpRequestMessage<T>)
                {
                    HttpRequestMessage<T> valueAsRequest = (HttpRequestMessage<T>)value;
                    return valueAsRequest.Content.ReadAsAsync().Result;
                }
                else if (value is HttpRequestMessage<T?>)
                {
                    HttpRequestMessage<T?> valueAsRequest = (HttpRequestMessage<T?>)value;
                    return valueAsRequest.Content.ReadAsAsync().Result;
                }
                else if (value is HttpResponseMessage<T>)
                {
                    HttpResponseMessage<T> valueAsResponse = (HttpResponseMessage<T>)value;
                    return valueAsResponse.Content.ReadAsAsync().Result;
                }
                else if (value is HttpResponseMessage<T?>)
                {
                    HttpResponseMessage<T?> valueAsResponse = (HttpResponseMessage<T?>)value;
                    return valueAsResponse.Content.ReadAsAsync().Result;
                }
                else if (value is ObjectContent<T>)
                {
                    ObjectContent<T> valueAsContent = (ObjectContent<T>)value;
                    return valueAsContent.ReadAsOrDefaultAsync().Result;
                }
                else if (value is ObjectContent<T?>)
                {
                    ObjectContent<T?> valueAsContent = (ObjectContent<T?>)value;
                    return valueAsContent.ReadAsOrDefaultAsync().Result;
                }

                throw Fx.Exception.AsError(
                    new InvalidOperationException(
                        Http.SR.ValueConversionFailed(value.GetType().FullName, this.Type.FullName)));
            }

            public override bool IsInstanceOf(object value)
            {
                return value == null ?
                    true :
                    value is T;
            }

            public sealed override bool CanConvertFromType(Type type)
            {
                if (type == null)
                {
                    throw Fx.Exception.ArgumentNull("type");
                }

                Type underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null)
                {
                    type = underlyingType;
                }

                if (this.Type.IsAssignableFrom(type))
                {
                    return true;
                }

                if (type == TypeHelper.StringType && this.CanConvertFromString)
                {
                    return true;
                }

                if (typeof(HttpRequestMessage<T>).IsAssignableFrom(type) ||
                    typeof(HttpRequestMessage<T?>).IsAssignableFrom(type) ||
                    typeof(HttpResponseMessage<T>).IsAssignableFrom(type) ||
                    typeof(HttpResponseMessage<T?>).IsAssignableFrom(type) ||
                    typeof(ObjectContent<T>).IsAssignableFrom(type) ||
                    typeof(ObjectContent<T?>).IsAssignableFrom(type))
                {
                    return true;
                }

                return false;
            }
        }

        private class BooleanValueConverter : ValueConverter<bool>
        {
            public BooleanValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return bool.Parse(value);
            }
        }

        private class CharValueConverter : ValueConverter<char>
        {
            public CharValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                Fx.Assert(value.Length > 0, "The 'value' string parameter should not be empty.");
                return value[0];
            }
        }

        private class SByteValueConverter : ValueConverter<sbyte>
        {
            public SByteValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return sbyte.Parse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }
        
        private class ByteValueConverter : ValueConverter<byte>
        {
            public ByteValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return byte.Parse(value, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);
            }
        }
        
        private class Int16ValueConverter : ValueConverter<short>
        {
            public Int16ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return short.Parse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }
        
        private class UInt16ValueConverter : ValueConverter<ushort>
        {
            public UInt16ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return ushort.Parse(value, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);
            }
        }

        private class Int32ValueConverter : ValueConverter<int>
        {
            public Int32ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return int.Parse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        private class UInt32ValueConverter : ValueConverter<uint>
        {
            public UInt32ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return uint.Parse(value, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);
            }
        }

        private class Int64ValueConverter : ValueConverter<long>
        {
            public Int64ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return long.Parse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        private class UInt64ValueConverter : ValueConverter<ulong>
        {
            public UInt64ValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return ulong.Parse(value, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);
            }
        }

        private class SingleValueConverter : ValueConverter<float>
        {
            public SingleValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return float.Parse(value, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
            }
        }

        private class DoubleValueConverter : ValueConverter<double>
        {
            public DoubleValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return double.Parse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            }
        }

        private class DecimalValueConverter : ValueConverter<decimal>
        {
            public DecimalValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return decimal.Parse(value, NumberStyles.AllowDecimalPoint | NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        private class DateTimeValueConverter : ValueConverter<DateTime>
        {
            public DateTimeValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }

        private class TimeSpanValueConverter : ValueConverter<TimeSpan>
        {
            public TimeSpanValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        private class GuidValueConverter : ValueConverter<Guid>
        {
            public GuidValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return new Guid(value);
            }
        }

        private class DateTimeOffsetValueConverter : ValueConverter<DateTimeOffset>
        {
            public DateTimeOffsetValueConverter(bool isNullable) : base(isNullable)
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces);
            }
        }

        private class UriValueConverter : DefaultValueConverter<Uri>
        {
            public UriValueConverter()
            {
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return new Uri(value, UriKind.RelativeOrAbsolute);
            }
        }

        private class EnumValueConverter<T> : ValueConverter<T> where T : struct
        {
            public EnumValueConverter(bool isNullable) : base(isNullable)
            {
                Fx.Assert(this.Type.IsEnum == true, "The EnumValueConverter should only be used with enum types.");
                this.CanConvertFromString = true;
            }

            protected override object ConvertFromString(string value)
            {
                return Enum.Parse(this.Type, value, true);
            }
        }
    }
}
