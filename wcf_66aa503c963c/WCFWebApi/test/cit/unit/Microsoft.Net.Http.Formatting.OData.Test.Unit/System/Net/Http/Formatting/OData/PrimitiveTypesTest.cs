//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Net.Http.Formatting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class PrimitiveTypesTest
    {
        Microsoft.TestCommon.WCF.Http.UnitTestAsserters Asserters = new Microsoft.TestCommon.WCF.Http.UnitTestAsserters();

        static readonly string String = "This is a Test String";
        static readonly bool Bool = true;
        static readonly byte Byte = 64;
        static readonly DateTime DateTime = new DateTime(2010, 1, 1);
        static readonly decimal Decimal = 12345.99999M;
        static readonly double Double = 99999.12345;
        static readonly Guid Guid = new Guid("f99080c0-2f9e-472e-8c72-1a8ecd9f902d");
        static readonly Int16 Int16 = Int16.MinValue;
        static readonly Int32 Int32 = Int32.MinValue;
        static readonly Int64 Int64 = Int64.MinValue;
        static readonly sbyte SByte = SByte.MinValue;
        static readonly Single Single = Single.PositiveInfinity;
        static readonly byte[] ByteArray = new byte[] { 0, 2, 32, 64, 128, 255 };
        static readonly char Char = 'A';
        static readonly UInt16 UInt16 = UInt16.MaxValue;
        static readonly UInt32 UInt32 = UInt32.MaxValue;
        static readonly UInt64 UInt64 = UInt64.MaxValue;
        static readonly Uri AbsoluteUri = new Uri("http://www.tempuri.org:8000/AbsoluteUri");
        static readonly Uri RelativeUri = new Uri("RelativeUri", UriKind.Relative);
        static readonly TimeSpan TimeSpan = TimeSpan.FromMinutes(60);
        static readonly XmlQualifiedName XmlQualifiedName = new XmlQualifiedName("QualifiedName", "http://www.tempuri.org");
        static readonly object Object = new object();
        static readonly bool? NullableBool = false;
        static readonly int? NullableInt = null;

        static Dictionary<string, object> PrimitiveTypesToTest;

        [ClassInitialize]
        public static void InitializeTest(TestContext context)
        {
            PrimitiveTypesToTest = new Dictionary<string, object>();            
            PrimitiveTypesToTest.Add("String", String);
            PrimitiveTypesToTest.Add("Bool", Bool);
            PrimitiveTypesToTest.Add("Byte", Byte);
            PrimitiveTypesToTest.Add("ByteArray", ByteArray);
            PrimitiveTypesToTest.Add("Char", Char);
            PrimitiveTypesToTest.Add("DateTime", DateTime);
            PrimitiveTypesToTest.Add("Decimal", Decimal);
            PrimitiveTypesToTest.Add("Double", Double);
            PrimitiveTypesToTest.Add("Guid", Guid);
            PrimitiveTypesToTest.Add("Int16", Int16);
            PrimitiveTypesToTest.Add("Int32", Int32);
            PrimitiveTypesToTest.Add("Int64", Int64);
            PrimitiveTypesToTest.Add("UInt16", UInt16);
            PrimitiveTypesToTest.Add("UInt32", UInt32);
            PrimitiveTypesToTest.Add("UInt64", UInt64);
            PrimitiveTypesToTest.Add("SByte", SByte);
            PrimitiveTypesToTest.Add("Single", Single);
            PrimitiveTypesToTest.Add("AbsoluteUri", AbsoluteUri);
            PrimitiveTypesToTest.Add("RelativeUri", RelativeUri);
            PrimitiveTypesToTest.Add("TimeSpan", TimeSpan);
            PrimitiveTypesToTest.Add("XmlQualifiedName", XmlQualifiedName);
            PrimitiveTypesToTest.Add("Object", Object);
            PrimitiveTypesToTest.Add("NullableBool", NullableBool);
            PrimitiveTypesToTest.Add("NullableInt", NullableInt);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter serializes primitive types in valid ODataMessageFormat")]
        public void PrimitiveTypesSerializeAsOData()
        {                                 
            foreach (KeyValuePair<string, object> primitiveType in PrimitiveTypesToTest)
            {
                string expected = BaselineResource.ResourceManager.GetString(primitiveType.Key);

                if (expected == null)
                {
                    Assert.Fail("Could not find a resource string associated with {0}. Please check the BaselineResource.resx file", primitiveType.Key);
                }

                ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

                object value = primitiveType.Value;
                Type type = (value != null) ? value.GetType() : typeof(Nullable<int>);

                ObjectContent content = new ObjectContent(type, value);
                content.Formatters.Add(formatter);
                content.DefaultFormatter = formatter;

                Asserters.String.AreEqual(content.ReadAsStringAsync().Result, expected, true);  
            }              
        }
    }
}
