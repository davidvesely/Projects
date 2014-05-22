// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.TestCommon.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class UnitTestDataSets
    {
        public ValueTypeTestData<char> Chars { get { return TestData.CharTestData; } }
        public ValueTypeTestData<int> Ints { get { return TestData.IntTestData; } }
        public ValueTypeTestData<uint> Uints { get { return TestData.UintTestData; } }
        public ValueTypeTestData<short> Shorts { get { return TestData.ShortTestData; } }
        public ValueTypeTestData<ushort> Ushorts { get { return TestData.UshortTestData; } }
        public ValueTypeTestData<long> Longs { get { return TestData.LongTestData; } }
        public ValueTypeTestData<ulong> Ulongs { get { return TestData.UlongTestData; } }
        public ValueTypeTestData<byte> Bytes { get { return TestData.ByteTestData; } }
        public ValueTypeTestData<sbyte> SBytes { get { return TestData.SByteTestData; } }
        public ValueTypeTestData<bool> Bools { get { return TestData.BoolTestData; } }
        public ValueTypeTestData<double> Doubles { get { return TestData.DoubleTestData; } }
        public ValueTypeTestData<float> Floats { get { return TestData.FloatTestData; } }
        public ValueTypeTestData<DateTime> DateTimes { get { return TestData.DateTimeTestData; } }
        public ValueTypeTestData<Decimal> Decimals { get { return TestData.DecimalTestData; } }
        public ValueTypeTestData<TimeSpan> TimeSpans { get { return TestData.TimeSpanTestData; } }
        public ValueTypeTestData<Guid> Guids { get { return TestData.GuidTestData; } }
        public ValueTypeTestData<DateTimeOffset> DateTimeOffsets { get { return TestData.DateTimeOffsetTestData; } }
        public ValueTypeTestData<SimpleEnum> SimpleEnums { get { return TestData.SimpleEnumTestData; } }
        public ValueTypeTestData<LongEnum> LongEnums { get { return TestData.LongEnumTestData; } }
        public ValueTypeTestData<FlagsEnum> FlagsEnums { get { return TestData.FlagsEnumTestData; } }
        public TestData<string> EmptyStrings { get { return TestData.EmptyStrings; } }
        public RefTypeTestData<string> Strings { get { return TestData.StringTestData; } }
        public RefTypeTestData<ISerializableType> ISerializableTypes { get { return TestData.ISerializableTypeTestData; } }
        public ReadOnlyCollection<TestData> ValueTypeTestDataCollection { get { return TestData.ValueTypeTestDataCollection; } }
        public ReadOnlyCollection<TestData> RefTypeTestDataCollection { get { return TestData.RefTypeTestDataCollection; } }
        public ReadOnlyCollection<TestData> ValueAndRefTypeTestDataCollection { get { return TestData.ValueTypeTestDataCollection; } }
        public ReadOnlyCollection<TestData> RepresentativeValueAndRefTypeTestDataCollection { get { return TestData.RepresentativeValueAndRefTypeTestDataCollection; } }
    }
}