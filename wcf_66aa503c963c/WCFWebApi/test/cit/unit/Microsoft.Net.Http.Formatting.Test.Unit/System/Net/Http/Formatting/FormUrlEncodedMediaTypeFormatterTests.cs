// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class FormUrlEncodedMediaTypeFormatterTests : UnitTest<FormUrlEncodedMediaTypeFormatter>
    {
        private const int MinBufferSize = 256;
        private const int DefaultBufferSize = 32 * 1024;

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedMediaTypeFormatter is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedMediaTypeFormatter() constructor sets standard form URL encoded media types in SupportedMediaTypes.")]
        public void Constructor()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.StandardFormUrlEncodedMediaTypes)
            {
                Assert.IsTrue(formatter.SupportedMediaTypes.Contains(mediaType), string.Format("SupportedMediaTypes should have included {0}.", mediaType.ToString()));
            }
        }

        #endregion Constructors

        #region Members

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("DefaultMediaType property returns application/x-www-form-urlencoded.")]
        public void DefaultMediaTypeReturnsApplicationJson()
        {
            MediaTypeHeaderValue mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType;
            Assert.IsNotNull(mediaType, "DefaultMediaType cannot be null.");
            Assert.AreEqual("application/x-www-form-urlencoded", mediaType.MediaType);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadBufferSize return correct value.")]
        public void ReadBufferSizeReturnsCorrectValue()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            Assert.AreEqual(DefaultBufferSize, formatter.ReadBufferSize, "Unexpected initial value.");
            formatter.ReadBufferSize = MinBufferSize;
            Assert.AreEqual(MinBufferSize, formatter.ReadBufferSize, "Unexpected value.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ReadBufferSize throws on invalid value.")]
        public void ReadBufferSizeThrowsOnInvalidValue()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            Asserters.Exception.ThrowsArgument("value", () => { formatter.ReadBufferSize = -1; });
            Asserters.Exception.ThrowsArgument("value", () => { formatter.ReadBufferSize = 0; });
            Asserters.Exception.ThrowsArgument("value", () => { formatter.ReadBufferSize = MinBufferSize - 1; });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects changes in buffer size.")]
        public void IsModifiedDetectsOptionChanges()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            int bufferSize = formatter.ReadBufferSize;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");
            
            formatter.ReadBufferSize = DefaultBufferSize;
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after write.");

            TestFormUrlEncodedMediaTypeFormatter subClass = new TestFormUrlEncodedMediaTypeFormatter();
            Assert.IsTrue(subClass.IsModified, "formatter should have been modified in subclass.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsModified detects adding mapping.")]
        public void IsModifiedDetectsAddingMapping()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified");

            Collection<MediaTypeMapping> mappings = formatter.MediaTypeMappings;
            Assert.IsFalse(formatter.IsModified, "formatter should not have been modified after read.");

            formatter.AddRequestHeaderMapping("name", "value", StringComparison.OrdinalIgnoreCase, false, "application/octet-stream");
            Assert.IsTrue(formatter.IsModified, "formatter should have been modified after addition.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() throws on null.")]
        public void CanReadTypeThrowsOnNull()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.CanReadType(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanReadType() returns true.")]
        public void CanReadTypeReturnsTrue()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Assert.IsTrue(formatter.CanReadType(type), "formatter should have returned true.");

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    Assert.IsTrue(formatter.CanReadType(type), "formatter should have returned true on 2nd try as well.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() throws on null.")]
        public void CanWriteTypeThrowsOnNull()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.CanWriteType(null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("CanWriteType() returns false.")]
        public void CanWriteTypeReturnsFalse()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    Assert.IsFalse(formatter.CanWriteType(type), "formatter should have returned false.");

                    // Ask a 2nd time to probe whether the cached result is treated the same
                    Assert.IsFalse(formatter.CanWriteType(type), "formatter should have returned false on 2nd try as well.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("ReadFromStream() throws on null.")]
        public void ReadFromStreamThrowsOnNull()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Exception.ThrowsArgumentNull("type", () => { formatter.OnReadFromStream(null, Stream.Null, null); });
            Asserters.Exception.ThrowsArgumentNull("stream", () => { formatter.OnReadFromStream(typeof(object), null, null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.ExtendedTimeout), Owner("dravva")]
        [Description("WriteToStream() throws not implemented.")]
        public void WriteToStreamThrowsNotImplemented()
        {
            TestFormUrlEncodedMediaTypeFormatter formatter = new TestFormUrlEncodedMediaTypeFormatter();
            Asserters.Exception.Throws<NotImplementedException>(
                () => { formatter.OnWriteToStream(null, new object(), Stream.Null, null, null); },
                (exception) => { });
        }

        #endregion Members

        #region Test types

        public class TestFormUrlEncodedMediaTypeFormatter : FormUrlEncodedMediaTypeFormatter
        {
            public new bool CanReadType(Type type)
            {
                return base.CanReadType(type);
            }

            public new bool CanWriteType(Type type)
            {
                return base.CanWriteType(type);
            }

            public new object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
            {
                return base.OnReadFromStream(type, stream, contentHeaders);
            }

            public new void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                base.OnWriteToStream(type, value, stream, contentHeaders, context);
            }
        }

        #endregion Test types
    }
}
