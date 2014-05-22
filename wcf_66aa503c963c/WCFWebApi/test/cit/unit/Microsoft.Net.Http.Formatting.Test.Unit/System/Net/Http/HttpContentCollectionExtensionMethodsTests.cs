// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpContentCollectionExtensionMethods)), UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpContentCollectionExtensionMethodsTests : UnitTest
    {
        private const string contentID = "content-id";
        private const string matchContentID = "matchID";
        private const string matchContentType = "text/plain";
        private const string matchDispositionName = "N1";
        private const string quotedMatchDispositionName = "\"" + matchDispositionName + "\"";
        private const string matchDispositionType = "form-data";
        private const string quotedMatchDispositionType = "\"" + matchDispositionType + "\"";

        private const string noMatchContentID = "nomatchID";
        private const string noMatchContentType = "text/nomatch";
        private const string noMatchDispositionName = "nomatchName";
        private const string noMatchDispositionType = "nomatchType";

        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IEnumerableHttpContentExtensionMethods is a public static class.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                typeof(HttpContentCollectionExtensionMethods),
                TypeAssert.TypeProperties.IsPublicVisibleClass |
                TypeAssert.TypeProperties.IsStatic);
        }
        #endregion

        #region Helpers
        private static IEnumerable<HttpContent> CreateContent()
        {
            MultipartFormDataContent multipart = new MultipartFormDataContent();

            multipart.Add(new StringContent("A", UTF8Encoding.UTF8, matchContentType), matchDispositionName);
            multipart.Add(new StringContent("B", UTF8Encoding.UTF8, matchContentType), "N2");
            multipart.Add(new StringContent("C", UTF8Encoding.UTF8, matchContentType), "N3");

            multipart.Add(new ByteArrayContent(new byte[] { 0x65 }), "N4");
            multipart.Add(new ByteArrayContent(new byte[] { 0x65 }), "N5");
            multipart.Add(new ByteArrayContent(new byte[] { 0x65 }), "N6");

            HttpContent cidContent = new StringContent("<html>A</html>", UTF8Encoding.UTF8, "text/html");
            cidContent.Headers.Add(contentID, matchContentID);
            multipart.Add(cidContent);

            return multipart;
        }

        private static void ClearHeaders(IEnumerable<HttpContent> contents)
        {
            foreach (var c in contents)
            {
                c.Headers.Clear();
            }
        }

        #endregion

        #region Member tests

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, string) throws on null.")]
        public void FindAllContentTypeString()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FindAllContentType(null, "text/plain"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("contentType", () => { HttpContentCollectionExtensionMethods.FindAllContentType(content, (string)null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("contentType", () => { HttpContentCollectionExtensionMethods.FindAllContentType(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, string) no match.")]
        public void FindAllContentTypeStringNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            IEnumerable<HttpContent> result = null;
            result = content.FindAllContentType(noMatchContentType);
            Assert.AreEqual(0, result.Count(), "Expected no matching content types based on string.");

            ClearHeaders(content);
            result = content.FindAllContentType(noMatchContentType);
            Assert.AreEqual(0, result.Count(), "Expected no matching content types based on string.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, string) match.")]
        public void FindAllContentTypeStringMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            IEnumerable<HttpContent> result = content.FindAllContentType(matchContentType);
            Assert.AreEqual(3, result.Count(), "Expected matching content types based on string.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, MediaTypeHeaderValue) throws on null.")]
        public void FindAllContentTypeMediaTypeThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FindAllContentType(null, new MediaTypeHeaderValue("text/plain")); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("contentType", () => { HttpContentCollectionExtensionMethods.FindAllContentType(content, (MediaTypeHeaderValue)null); });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, MediaTypeHeaderValue) no match.")]
        public void FindAllContentTypeMediaTypeNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            IEnumerable<HttpContent> result = null;

            result = content.FindAllContentType(new MediaTypeHeaderValue(noMatchContentType));
            Assert.AreEqual(0, result.Count(), "Expected no matching content types based on MediaTypeHeaderValue.");

            ClearHeaders(content);
            result = content.FindAllContentType(new MediaTypeHeaderValue(noMatchContentType));
            Assert.AreEqual(0, result.Count(), "Expected no matching content types based on MediaTypeHeaderValue.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FindAllContentType(IEnumerable<HttpContent>, string) match.")]
        public void FindAllContentTypeMediaTypeMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            IEnumerable<HttpContent> result = content.FindAllContentType(new MediaTypeHeaderValue(matchContentType));
            Assert.AreEqual(3, result.Count(), "Expected matching content types based on MediaTypeHeaderValue.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionName(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstDispositionNameThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstDispositionName(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.FirstDispositionName(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.FirstDispositionName(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionName(IEnumerable<HttpContent>, string) no match.")]
        public void FirstDispositionNameNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNull(content.FirstDispositionNameOrDefault(noMatchDispositionName), "Expected default value (null)");

            ClearHeaders(content);
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    content.FirstDispositionName(noMatchDispositionName);
                },
                (exception) =>
                {
                    Assert.IsNotNull(exception);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionName(IEnumerable<HttpContent>, string) match.")]
        public void FirstDispositionNameMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstDispositionName(matchDispositionName), "Expected match on disposition name.");
            Assert.IsNotNull(content.FirstDispositionName(quotedMatchDispositionName), "Expected match on quoted disposition name.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionNameOrDefault(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstDispositionNameOrDefaultThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstDispositionNameOrDefault(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.FirstDispositionNameOrDefault(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.FirstDispositionNameOrDefault(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionName(IEnumerable<HttpContent>, string) no match.")]
        public void FirstDispositionNameOrDefaultNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNull(content.FirstDispositionNameOrDefault(noMatchDispositionName), "Expected default value (null)");

            ClearHeaders(content);
            Assert.IsNull(content.FirstDispositionNameOrDefault(noMatchDispositionName), "Expected default value (null)");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionName(IEnumerable<HttpContent>, string) match.")]
        public void FirstDispositionNameOrDefaultMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstDispositionNameOrDefault(matchDispositionName), "Expected match based on disposition name.");
            Assert.IsNotNull(content.FirstDispositionNameOrDefault(quotedMatchDispositionName), "Expected match on quoted disposition name.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionType(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstDispositionTypeThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstDispositionType(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("dispositionType", () => { HttpContentCollectionExtensionMethods.FirstDispositionType(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("dispositionType", () => { HttpContentCollectionExtensionMethods.FirstDispositionType(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionType(IEnumerable<HttpContent>, string) no match.")]
        public void FirstDispositionTypeNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    content.FirstDispositionType(noMatchDispositionType);
                },
                (exception) =>
                {
                    Assert.IsNotNull(exception);
                });

            Assert.IsNull(content.FirstDispositionTypeOrDefault(noMatchDispositionType), "Expected default value (null)");

            ClearHeaders(content);
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    content.FirstDispositionType(noMatchDispositionType);
                },
                (exception) =>
                {
                    Assert.IsNotNull(exception);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionType(IEnumerable<HttpContent>, string) match.")]
        public void FirstDispositionTypeMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstDispositionType(matchDispositionType), "Expected match on disposition type.");
            Assert.IsNotNull(content.FirstDispositionType(quotedMatchDispositionType), "Expected match on quoted disposition type.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionTypeOrDefault(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstDispositionTypeOrDefaultThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstDispositionTypeOrDefault(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("dispositionType", () => { HttpContentCollectionExtensionMethods.FirstDispositionTypeOrDefault(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("dispositionType", () => { HttpContentCollectionExtensionMethods.FirstDispositionTypeOrDefault(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionTypeOrDefault(IEnumerable<HttpContent>, string) no match.")]
        public void FirstDispositionTypeOrDefaultNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNull(content.FirstDispositionTypeOrDefault(noMatchDispositionType), "Expected default value (null)");

            ClearHeaders(content);
            Assert.IsNull(content.FirstDispositionTypeOrDefault(noMatchDispositionType), "Expected default value (null)");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstDispositionTypeOrDefault(IEnumerable<HttpContent>, string) match.")]
        public void FirstDispositionTypeOrDefaultMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstDispositionTypeOrDefault(matchDispositionType), "Expected match on disposition type.");
            Assert.IsNotNull(content.FirstDispositionTypeOrDefault(quotedMatchDispositionType), "Expected match on quoted disposition type.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStart(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstStartThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstStart(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("start", () => { HttpContentCollectionExtensionMethods.FirstStart(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("start", () => { HttpContentCollectionExtensionMethods.FirstStart(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStart(IEnumerable<HttpContent>, string) no match.")]
        public void FirstStartNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.Throws<InvalidOperationException>(
                () =>
                {
                    content.FirstStart(noMatchContentID);
                },
                (exception) =>
                {
                    Assert.IsNotNull(exception);
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStart(IEnumerable<HttpContent>, string) match.")]
        public void FirstStartMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstStart(matchContentID), "Expected match based on start parameter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStartOrDefault(IEnumerable<HttpContent>, string) throws on null.")]
        public void FirstStartOrDefaultThrows()
        {
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.FirstStartOrDefault(null, "A"); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("start", () => { HttpContentCollectionExtensionMethods.FirstStartOrDefault(content, null); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("start", () => { HttpContentCollectionExtensionMethods.FirstStartOrDefault(content, empty); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStartOrDefault(IEnumerable<HttpContent>, string) no match.")]
        public void FirstStartOrDefaultNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNull(content.FirstStartOrDefault(noMatchContentID), "Expected default value (null)");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FirstStartOrDefault(IEnumerable<HttpContent>, string) match.")]
        public void FirstStartOrDefaultMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Assert.IsNotNull(content.FirstStartOrDefault(matchContentID), "Expected match based on start parameter.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryGetFormFieldValue(IEnumerable<HttpContent>, string, out string) throws on null")]
        public void TryGetFormFieldValueThrows()
        {
            string value;
            Asserters.Exception.ThrowsArgumentNull("contents", () => { HttpContentCollectionExtensionMethods.TryGetFormFieldValue(null, "A", out value); });

            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.TryGetFormFieldValue(content, null, out value); });
            foreach (string empty in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("dispositionName", () => { HttpContentCollectionExtensionMethods.TryGetFormFieldValue(content, empty, out value); });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryGetFormFieldValue(IEnumerable<HttpContent>, string, out string) no match")]
        public void TryGetFormFieldValueNoMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            string value;
            Assert.IsFalse(HttpContentCollectionExtensionMethods.TryGetFormFieldValue(content, noMatchDispositionName, out value));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("TryGetFormFieldValue(IEnumerable<HttpContent>, string, out string) match")]
        public void TryGetFormFieldValueMatch()
        {
            IEnumerable<HttpContent> content = HttpContentCollectionExtensionMethodsTests.CreateContent();
            string value;
            Assert.IsTrue(HttpContentCollectionExtensionMethods.TryGetFormFieldValue(content, matchDispositionName, out value));
            Assert.AreEqual("A", value, "Unexpected HttpContent value read");
        }


        #endregion
    }
}