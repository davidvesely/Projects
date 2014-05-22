// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System.Json;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete), UnitTestType(typeof(UriExtensionMethods))]
    public class UriExtensionMethodsTests : UnitTest
    {
        private static readonly Uri TestAddress = new Uri("http://www.example.com");
        private static readonly Type TestType = typeof(string);

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("UriExtensionMethods is public and static.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAsJson(Uri, out JsonObject) throws with null 'this'.")]
        public void TryReadQueryAsJsonThrowsWithNull()
        {
            JsonObject value;
            Asserters.Exception.ThrowsArgumentNull("address", () => ((Uri)null).TryReadQueryAsJson(out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAsJson(Uri, out JsonObject) succeeds with valid URIs.")]
        public void TryReadQueryAsJsonSucceeds()
        {
            foreach (Uri address in DataSets.Http.Uris)
            {
                JsonObject value;
                Assert.IsTrue(address.TryReadQueryAsJson(out value), "Expected 'true' as result");
                Assert.IsNotNull(value, "value should not be null");
                Assert.IsInstanceOfType(value, typeof(JsonObject), "Unexpected type returned.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAs(Uri, Type, out object) throws with null 'this'.")]
        public void TryReadQueryAsThrowsWithNull()
        {
            object value;
            Asserters.Exception.ThrowsArgumentNull("address", () => ((Uri)null).TryReadQueryAs(TestType, out value));
            Asserters.Exception.ThrowsArgumentNull("type", () => TestAddress.TryReadQueryAs(null, out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAs(Uri, Type, out object) succeeds with valid URIs.")]
        public void TryReadQueryAsSucceeds()
        {
            object value;
            UriBuilder address = new UriBuilder("http://some.host");

            address.Query = "a=2";
            Assert.IsTrue(address.Uri.TryReadQueryAs(typeof(SimpleObject1), out value), "Expected 'true' reading valid data");
            SimpleObject1 so1 = (SimpleObject1)value;
            Assert.IsNotNull(so1, "Object should not be null");
            Assert.AreEqual(2, so1.a, "value should have been 2");

            address.Query = "b=true";
            Assert.IsTrue(address.Uri.TryReadQueryAs(typeof(SimpleObject2), out value), "Expected 'true' reading valid data");
            SimpleObject2 so2 = (SimpleObject2)value;
            Assert.IsNotNull(so2, "Object should not be null");
            Assert.IsTrue(so2.b, "Value should have been true");

            address.Query = "c=hello";
            Assert.IsTrue(address.Uri.TryReadQueryAs(typeof(SimpleObject3), out value), "Expected 'true' reading valid data");
            SimpleObject3 so3 = (SimpleObject3)value;
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.AreEqual("hello", so3.c, "Value should have been 'hello'");

            address.Query = "c=";
            Assert.IsTrue(address.Uri.TryReadQueryAs(typeof(SimpleObject3), out value), "Expected 'true' reading valid data");
            so3 = (SimpleObject3)value;
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.AreEqual("", so3.c, "Value should have been ''");

            address.Query = "c=null";
            Assert.IsTrue(address.Uri.TryReadQueryAs(typeof(SimpleObject3), out value), "Expected 'true' reading valid data");
            so3 = (SimpleObject3)value;
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.IsNull(so3.c, "Value should have been null");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAs<T>(Uri, out T) throws with null 'this'.")]
        public void TryReadQueryAsTThrowsWithNull()
        {
            object value;
            Asserters.Exception.ThrowsArgumentNull("address", () => ((Uri)null).TryReadQueryAs<object>(out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("TryReadQueryAs<T>(Uri, out T) succeeds with valid URIs.")]
        public void TryReadQueryAsTSucceeds()
        {
            UriBuilder address = new UriBuilder("http://some.host");
            address.Query = "a=2";
            SimpleObject1 so1;
            Assert.IsTrue(address.Uri.TryReadQueryAs<SimpleObject1>(out so1), "Expected 'true' reading valid data");
            Assert.IsNotNull(so1, "Object should not be null");
            Assert.AreEqual(2, so1.a, "value should have been 2");

            address.Query = "b=true";
            SimpleObject2 so2;
            Assert.IsTrue(address.Uri.TryReadQueryAs<SimpleObject2>(out so2), "Expected 'true' reading valid data");
            Assert.IsNotNull(so2, "Object should not be null");
            Assert.IsTrue(so2.b, "Value should have been true");

            address.Query = "c=hello";
            SimpleObject3 so3;
            Assert.IsTrue(address.Uri.TryReadQueryAs<SimpleObject3>(out so3), "Expected 'true' reading valid data");
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.AreEqual("hello", so3.c, "Value should have been 'hello'");

            address.Query = "c=";
            Assert.IsTrue(address.Uri.TryReadQueryAs<SimpleObject3>(out so3), "Expected 'true' reading valid data");
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.AreEqual("", so3.c, "Value should have been ''");

            address.Query = "c=null";
            Assert.IsTrue(address.Uri.TryReadQueryAs<SimpleObject3>(out so3), "Expected 'true' reading valid data");
            Assert.IsNotNull(so3, "Object should not be null");
            Assert.IsNull(so3.c, "Value should have been null");
        }

        #endregion Methods

        #region Helpers

        public class SimpleObject1
        {
            public int a { get; set; }
        }

        public class SimpleObject2
        {
            public bool b { get; set; }
        }

        public class SimpleObject3
        {
            public string c { get; set; }
        }

        #endregion
    }
}