// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System.Net.Http.Headers;
    using System.Net.Http.Formatting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MediaTypeAssert
    {
        private static readonly MediaTypeAssert singleton = new MediaTypeAssert();

        public static MediaTypeAssert Singleton { get { return singleton; } }

        public void AreEqual(MediaTypeHeaderValue expected, MediaTypeHeaderValue actual, string errorMessage)
        {
            if (expected != null || actual != null)
            {
                Assert.IsNotNull(expected, string.Format("{0}  Expected media type should not be null.", errorMessage));
                Assert.IsTrue(MediaTypeHeaderValueComparer.Equals(expected, actual), string.Format("{0}  Expected media type '{1}' but found media type '{2}'.", errorMessage, expected, actual));
            }
        }

        public void AreEqual(MediaTypeHeaderValue expected, string actual, string errorMessage)
        {
            if (expected != null || !string.IsNullOrEmpty(actual))
            {
                MediaTypeHeaderValue actualMediaType = new MediaTypeHeaderValue(actual);
                Assert.IsNotNull(expected, string.Format("{0}: expected media type should not be null.", errorMessage));
                Assert.IsTrue(MediaTypeHeaderValueComparer.Equals(expected, actualMediaType), string.Format("{0}  Expected media type '{1}' but found media type '{2}'.", errorMessage, expected, actual));
            }
        }

        public void AreEqual(string expected, string actual, string errorMessage)
        {
            if (!string.IsNullOrEmpty(expected) || !string.IsNullOrEmpty(actual))
            {
                Assert.IsNotNull(expected, string.Format("{0}: expected media type should not be null.", errorMessage));
                MediaTypeHeaderValue expectedMediaType = new MediaTypeHeaderValue(expected);
                MediaTypeHeaderValue actualMediaType = new MediaTypeHeaderValue(actual);
                Assert.IsTrue(MediaTypeHeaderValueComparer.Equals(expectedMediaType, actualMediaType), string.Format("{0}  Expected media type '{1}' but found media type '{2}'.", errorMessage, expected, actual));
            }
        }

        public void AreEqual(string expected, MediaTypeHeaderValue actual, string errorMessage)
        {
            if (!string.IsNullOrEmpty(expected) || actual != null)
            {
                Assert.IsNotNull(expected, string.Format("{0}: expected media type should not be null.", errorMessage));
                MediaTypeHeaderValue expectedMediaType = new MediaTypeHeaderValue(expected);;
                Assert.IsTrue(MediaTypeHeaderValueComparer.Equals(expectedMediaType, actual), string.Format("{0}  Expected media type '{1}' but found media type '{2}'.", errorMessage, expected, actual));
            }
        }
    }
}
