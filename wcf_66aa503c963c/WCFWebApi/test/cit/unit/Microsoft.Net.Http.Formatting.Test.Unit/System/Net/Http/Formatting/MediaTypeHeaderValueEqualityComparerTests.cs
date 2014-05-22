// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [UnitTestType(typeof(MediaTypeHeaderValueEqualityComparer)), UnitTestLevel(UnitTestLevel.Complete)]
    public class MediaTypeHeaderValueEqualityComparerTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("MediaTypeHeaderValueEqualityComparer is internal, concrete, and not sealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass);
        }

        #endregion Type

        #region EqualityComparer Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("EqualityComparer returns same MediaTypeHeadeValueEqualityComparer instance each time.")]
        public void EqualityComparerReturnsMediaTypeHeadeValueEqualityComparer()
        {
            MediaTypeHeaderValueEqualityComparer comparer1 = MediaTypeHeaderValueEqualityComparer.EqualityComparer;
            MediaTypeHeaderValueEqualityComparer comparer2 = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            Assert.IsNotNull(comparer1, "MediaTypeHeaderValueEqualityComparer.EqualityComparer should not have returned null.");
            Assert.AreSame(comparer1, comparer2, "MediaTypeHeaderValueEqualityComparer.EqualityComparer should have returned the same instance both times.");
        }

        #endregion EqualityComparer Tests

        #region GetHashCode Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetHashCode(MediaTypeHeaderValue) returns the same hash code for media types that differe only be case.")]
        public void GetHashCodeReturnsSameHashCodeRegardlessOfCase()
        {
            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/xml");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEXT/xml");
            Assert.AreEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned the same hash codes.");

            mediaType1 = new MediaTypeHeaderValue("text/*");
            mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            Assert.AreEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned the same hash codes.");

            mediaType1 = new MediaTypeHeaderValue("*/*");
            mediaType2 = new MediaTypeHeaderValue("*/*");
            Assert.AreEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned the same hash codes.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("GetHashCode(MediaTypeHeaderValue) returns different hash codes if the media types are different.")]
        public void GetHashCodeReturnsDifferentHashCodeForDifferentMediaType()
        {
            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/*");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEXT/xml");
            Assert.AreNotEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned different hash codes.");

            mediaType1 = new MediaTypeHeaderValue("application/*");
            mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            Assert.AreNotEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned different hash codes.");

            mediaType1 = new MediaTypeHeaderValue("application/*");
            mediaType2 = new MediaTypeHeaderValue("*/*");
            Assert.AreNotEqual(comparer.GetHashCode(mediaType1), comparer.GetHashCode(mediaType2), "GetHashCode should have returned different hash codes.");
        }

        #endregion GetHashCode Tests

        #region Equals Tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Equals(MediaTypeHeaderValue, MediaTypeHeaderValue) returns true if media type 1 is a subset of 2.")]
        public void EqualsReturnsTrueIfMediaType1IsSubset()
        {
            string[] parameters = new string[]
            {
                ";name=value",
                ";q=1.0",
                ";version=1",
            };

            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/*");
            mediaType1.CharSet = "someCharset";
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            mediaType2.CharSet = "SOMECHARSET";
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            mediaType1 = new MediaTypeHeaderValue("application/*");
            mediaType1.CharSet = "";
            mediaType2 = new MediaTypeHeaderValue("application/*");
            mediaType2.CharSet = null;
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            foreach (string parameter in parameters)
            {
                mediaType1 = new MediaTypeHeaderValue("text/xml");
                mediaType2 = MediaTypeHeaderValue.Parse("TEXT/xml" + parameter);
                Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

                mediaType1 = new MediaTypeHeaderValue("text/*");
                mediaType2 = MediaTypeHeaderValue.Parse("TEXT/*" + parameter);
                Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

                mediaType1 = new MediaTypeHeaderValue("*/*");
                mediaType2 = MediaTypeHeaderValue.Parse("*/*" + parameter);
                Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Equals(MediaTypeHeaderValue, MediaTypeHeaderValue) returns false if media type 1 is a superset of 2.")]
        public void EqualsReturnsFalseIfMediaType1IsSuperset()
        {
            string[] parameters = new string[]
            {
                ";name=value",
                ";q=1.0",
                ";version=1",
            };

            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            foreach (string parameter in parameters)
            {
                MediaTypeHeaderValue mediaType1 = MediaTypeHeaderValue.Parse("text/xml" + parameter);
                MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEXT/xml");
                Assert.IsFalse(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'false'.");

                mediaType1 = MediaTypeHeaderValue.Parse("text/*" + parameter);
                mediaType2 = new MediaTypeHeaderValue("TEXT/*");
                Assert.IsFalse(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'false'.");

                mediaType1 = MediaTypeHeaderValue.Parse("*/*" + parameter);
                mediaType2 = new MediaTypeHeaderValue("*/*");
                Assert.IsFalse(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'false'.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Equals(MediaTypeHeaderValue, MediaTypeHeaderValue) returns true if media types and charsets differ only by case.")]
        public void Equals1ReturnsTrueIfMediaTypesDifferOnlyByCase()
        {
            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/xml");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEXT/xml");
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            mediaType1 = new MediaTypeHeaderValue("text/*");
            mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            mediaType1 = new MediaTypeHeaderValue("*/*");
            mediaType2 = new MediaTypeHeaderValue("*/*");
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            mediaType1 = new MediaTypeHeaderValue("text/*");
            mediaType1.CharSet = "someCharset";
            mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            mediaType2.CharSet = "SOMECHARSET";
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");

            mediaType1 = new MediaTypeHeaderValue("application/*");
            mediaType1.CharSet = "";
            mediaType2 = new MediaTypeHeaderValue("application/*");
            mediaType2.CharSet = null;
            Assert.IsTrue(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'true'.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Equals(MediaTypeHeaderValue, MediaTypeHeaderValue) returns false if media types and charsets differ by more than case.")]
        public void EqualsReturnsFalseIfMediaTypesDifferByMoreThanCase()
        {
            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/xml");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("TEST/xml");
            Assert.IsFalse(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'false'.");

            mediaType1 = new MediaTypeHeaderValue("text/*");
            mediaType1.CharSet = "someCharset";
            mediaType2 = new MediaTypeHeaderValue("TEXT/*");
            mediaType2.CharSet = "SOMEOTHERCHARSET";
            Assert.IsFalse(comparer.Equals(mediaType1, mediaType2), "Equals should have returned 'false'.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Equals(MediaTypeHeaderValue, MediaTypeHeaderValue) returns true if media types differ only in quality.")]
        public void EqualsReturnsTrueIfMediaTypesDifferOnlyByQuality()
        {
            MediaTypeHeaderValueEqualityComparer comparer = MediaTypeHeaderValueEqualityComparer.EqualityComparer;

            foreach (MediaTypeWithQualityHeaderValue mediaType1 in DataSets.Http.StandardMediaTypesWithQuality)
            {
                MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue(mediaType1.MediaType);
                Assert.IsTrue(comparer.Equals(mediaType2, mediaType1), string.Format("Equals should have returned 'true' for '{0}'.", mediaType1));
            }
        }

        #endregion Equals Tests
    }
}
