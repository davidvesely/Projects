// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ObjectContentAssert
    {
        private static readonly ObjectContentAssert singleton = new ObjectContentAssert();

        public static ObjectContentAssert Singleton { get { return singleton; } }

        public void IsCorrectGenericType(ObjectContent objectContent, Type genericTypeParameter)
        {
            GenericTypeAssert.Singleton.IsCorrectGenericType<ObjectContent>(objectContent, genericTypeParameter);
            Assert.AreEqual(genericTypeParameter, objectContent.ObjectType, "objectContent.ObjectType did not match its generic parameter.");
        }

        public void ContainsFormatters(ObjectContent objectContent, IEnumerable<MediaTypeFormatter> formatters)
        {
            Assert.IsNotNull(objectContent, "objectContent cannot be null.");
            Assert.IsNotNull(formatters, "Test error: formatters must be specified.");
            Assert.IsNotNull(objectContent.Formatters, "Formatters property cannot be null.");
            CollectionAssert.IsSubsetOf(formatters.ToList(), objectContent.Formatters, "Formatters did not include all expected formatters.");
        }

        /// <summary>
        /// Creates an instance of the generic <see cref="ObjectContent"/> for every value
        /// in the given <paramref name="testDataCollection"/> and invokes the <paramref name="codeUnderTest"/>.
        /// </summary>
        /// <param name="testDataCollection">The collection of test data.</param>
        /// <param name="flags">The test variations.</param>
        /// <param name="codeUnderTest">The code to invoke with each <see cref="ObjectContent"/>.</param>
        public void ExecuteForEachObjectContent(IEnumerable<TestData> testDataCollection, TestDataVariations flags, Action<ObjectContent, Type, object> codeUnderTest)
        {
            Assert.IsNotNull(testDataCollection, "testDataCollection cannot be null.");
            Assert.IsNotNull(codeUnderTest, "codeUnderTest cannot be null.");

            TestDataAssert.Singleton.Execute(
                testDataCollection,
                flags,
                "Failed in ExecuteForEachObjectContent.",
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    ObjectContent objectContent = 
                        (ObjectContent)GenericTypeAssert.Singleton.InvokeConstructor(
                            typeof(ObjectContent<>), 
                            convertType, 
                            new Type[] { convertType }, 
                            new object[] { obj });

                    codeUnderTest(objectContent, type, obj);
                });
        }

        /// <summary>
        /// Creates an instance of the generic <see cref="HttpRequestMessage"/> for every value
        /// in the given <paramref name="testDataCollection"/> and invokes the <paramref name="codeUnderTest"/>.
        /// </summary>
        /// <param name="testDataCollection">The collection of test data.</param>
        /// <param name="flags">The test variations.</param>
        /// <param name="codeUnderTest">The code to invoke with each <see cref="HttpRequestMessage"/>.</param>
        public void ExecuteForEachHttpRequestMessage(IEnumerable<TestData> testDataCollection, TestDataVariations flags, Action<HttpRequestMessage, Type, object> codeUnderTest)
        {
            Assert.IsNotNull(testDataCollection, "testDataCollection cannot be null.");
            Assert.IsNotNull(codeUnderTest, "codeUnderTest cannot be null.");

            TestDataAssert.Singleton.Execute(
                testDataCollection,
                flags,
                "Failed in ExecuteForEachHttpRequestMessage.",
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpRequestMessage request =
                        (HttpRequestMessage)GenericTypeAssert.Singleton.InvokeConstructor(
                            typeof(HttpRequestMessage<>),
                            convertType,
                            new Type[] { convertType },
                            new object[] { obj });

                    codeUnderTest(request, type, obj);
                });
        }

        /// <summary>
        /// Creates an instance of the generic <see cref="HttpResponseMessage"/> for every value
        /// in the given <paramref name="testDataCollection"/> and invokes the <paramref name="codeUnderTest"/>.
        /// </summary>
        /// <param name="testDataCollection">The collection of test data.</param>
        /// <param name="flags">The test variations.</param>
        /// <param name="codeUnderTest">The code to invoke with each <see cref="HttpResponseMessage"/>.</param>
        public void ExecuteForEachHttpResponseMessage(IEnumerable<TestData> testDataCollection, TestDataVariations flags, Action<HttpResponseMessage, Type, object> codeUnderTest)
        {
            Assert.IsNotNull(testDataCollection, "testDataCollection cannot be null.");
            Assert.IsNotNull(codeUnderTest, "codeUnderTest cannot be null.");

            TestDataAssert.Singleton.Execute(
                testDataCollection,
                flags,
                "Failed in ExecuteForEachHttpResponseMessage.",
                (type, obj) =>
                {
                    Type convertType = obj == null ? type : obj.GetType();
                    HttpResponseMessage response =
                        (HttpResponseMessage)GenericTypeAssert.Singleton.InvokeConstructor(
                            typeof(HttpResponseMessage<>),
                            convertType,
                            new Type[] { convertType },
                            new object[] { obj });

                    codeUnderTest(response, type, obj);
                });
        }
    }
}
