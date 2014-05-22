// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestType(typeof(HttpContentMultipartExtensionMethods)), UnitTestLevel(Microsoft.TestCommon.UnitTestLevel.Complete)]
    public class HttpContentMultipartExtensionMethodsTests : UnitTest
    {
        const string DefaultContentType = "text/plain";
        const string DefaultContentDisposition = "form-data";
        const string ExceptionStreamProviderMessage = "Bad Stream Provider!";
        const string ExceptionSyncStreamMessage = "Bad Sync Stream!";
        const string ExceptionAsyncStreamMessage = "Bad Async Stream!";
        const string LongText = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("HttpContentMultipartExtensionMethods is a public static class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                typeof(HttpContentMultipartExtensionMethods),
                TypeAssert.TypeProperties.IsPublicVisibleClass |
                TypeAssert.TypeProperties.IsStatic);
        }

        #endregion

        #region Helpers

        private static HttpContent CreateContent(string boundary, params string[] bodyEntity)
        {
            List<string> entities = new List<string>();
            int cnt = 0;
            foreach (var body in bodyEntity)
            {
                byte[] header = InternetMessageFormatHeaderParserTests.CreateBuffer(
                    string.Format("N{0}: V{0}", cnt),
                    string.Format("Content-Type: {0}", DefaultContentType),
                    string.Format("Content-Disposition: {0}; FileName=\"N{1}\"", DefaultContentDisposition, cnt));
                entities.Add(Encoding.UTF8.GetString(header) + body);
                cnt++;
            }

            byte[] message = MimeMultipartParserTests.CreateBuffer(boundary, entities.ToArray());
            HttpContent result = new ByteArrayContent(message);
            var contentType = new MediaTypeHeaderValue("multipart/form-data");
            contentType.Parameters.Add(new NameValueHeaderValue("boundary", string.Format("\"{0}\"", boundary)));
            result.Headers.ContentType = contentType;
            return result;
        }

        private static void ValidateContents(IEnumerable<HttpContent> contents)
        {
            int cnt = 0;
            foreach (var content in contents)
            {
                Assert.IsNotNull(content);
                Assert.IsNotNull(content.Headers);
                Assert.AreEqual(4, content.Headers.Count());

                IEnumerable<string> parsedValues = content.Headers.GetValues(string.Format("N{0}", cnt));
                Assert.AreEqual(1, parsedValues.Count());
                Assert.AreEqual(string.Format("V{0}", cnt), parsedValues.ElementAt(0));

                Assert.AreEqual(DefaultContentType, content.Headers.ContentType.MediaType);

                Assert.AreEqual(DefaultContentDisposition, content.Headers.ContentDisposition.DispositionType);
                Assert.AreEqual(string.Format("\"N{0}\"", cnt), content.Headers.ContentDisposition.FileName);

                cnt++;
            }
        }

        #endregion

        #region ArgumentValidation

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) detects non-Multipart content.")]
        public void ReadAsMultipartDetectsNonMultipartContent()
        {
            Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMultipartExtensionMethods.ReadAsMultipart(content);
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new ByteArrayContent(new byte[] { });
                content.ReadAsMultipart();
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.ReadAsMultipart();
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "multipart/form-data");
                content.ReadAsMultipart();
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) throws on null provider.")]
        public void ReadAsMultipartStreamProviderThrowsOnNull()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);

                Asserters.Exception.ThrowsArgumentNull("streamProvider", () =>
                {
                    content.ReadAsMultipart(null);
                });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider, int) throws on buffersize.")]
        public void ReadAsMultipartStreamProviderThrowsOnBufferSize()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);

                Asserters.Exception.ThrowsArgument("bufferSize", () =>
                {
                    content.ReadAsMultipart(new MemoryStreamProvider(), ParserData.MinBufferSize - 1);
                });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) detects non-Multipart content.")]
        public void ReadAsMultipartAsyncDetectsNonMultipartContent()
        {
            Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMultipartExtensionMethods.ReadAsMultipartAsync(content);
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new ByteArrayContent(new byte[] { });
                content.ReadAsMultipartAsync();
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty);
                content.ReadAsMultipartAsync();
            });

            Asserters.Exception.ThrowsArgument("content", () =>
            {
                HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "multipart/form-data");
                content.ReadAsMultipartAsync();
            });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) throws on null provider.")]
        public void ReadAsMultipartAsyncStreamProviderThrowsOnNull()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);

                Asserters.Exception.ThrowsArgumentNull("streamProvider", () =>
                {
                    content.ReadAsMultipartAsync(null);
                });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider, int) throws on buffersize.")]
        public void ReadAsMultipartAsyncStreamProviderThrowsOnBufferSize()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);

                Asserters.Exception.ThrowsArgument("bufferSize", () =>
                {
                    content.ReadAsMultipartAsync(new MemoryStreamProvider(), ParserData.MinBufferSize - 1);
                });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("IsMimeMultipartContent(HttpContent) checks extension method arguments.")]
        public void IsMimeMultipartContentVerifyArguments()
        {
            Asserters.Exception.ThrowsArgumentNull("content", () =>
            {
                HttpContent content = null;
                HttpContentMultipartExtensionMethods.IsMimeMultipartContent(content);
            });
        }

        #endregion

        #region Parsing
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("IsMimeMultipartContent(HttpContent) responds correctly to MIME multipart and other content")]
        public void IsMimeMultipartContent()
        {
            Assert.IsFalse(new ByteArrayContent(new byte[] { }).IsMimeMultipartContent(), "HttpContent should not be valid MIME multipart content");

            Assert.IsFalse(new StringContent(string.Empty).IsMimeMultipartContent(), "HttpContent should not be valid MIME multipart content");

            Assert.IsFalse(new StringContent(string.Empty, Encoding.UTF8, "multipart/form-data").IsMimeMultipartContent(), "HttpContent should not be valid MIME multipart content");

            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);
                Assert.IsTrue(content.IsMimeMultipartContent());
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("IsMimeMultipartContent(HttpContent, string) throws on null string.")]
        public void IsMimeMultipartContentThrowsOnNullString()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Assert.IsNotNull(content);
                foreach (var subtype in DataSets.Common.EmptyStrings)
                {
                    Asserters.Exception.ThrowsArgumentNull("subtype",
                        () =>
                        {
                            content.IsMimeMultipartContent(subtype);
                        });
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) successful parses synchronously.")]
        public void ReadAsMultipartSuccess()
        {
            HttpContent successContent;
            IEnumerable<HttpContent> result;

            successContent = CreateContent("boundary", "A", "B", "C");
            result = successContent.ReadAsMultipart();
            Assert.AreEqual(3, result.Count());

            successContent = CreateContent("boundary", "A", "B", "C");
            result = successContent.ReadAsMultipart(new MemoryStreamProvider());
            Assert.AreEqual(3, result.Count());

            successContent = CreateContent("boundary", "A", "B", "C");
            result = successContent.ReadAsMultipart(new MemoryStreamProvider(), 1024);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) successful parses asynchronously.")]
        public void ReadAsMultipartSuccessAsync()
        {
            HttpContent successContent;
            Task<IEnumerable<HttpContent>> task;
            IEnumerable<HttpContent> result;

            successContent = CreateContent("boundary", "A", "B", "C");
            task = successContent.ReadAsMultipartAsync();
            task.Wait(TimeoutConstant.DefaultTimeout);
            result = task.Result;
            Assert.AreEqual(3, result.Count());

            successContent = CreateContent("boundary", "A", "B", "C");
            task = successContent.ReadAsMultipartAsync(new MemoryStreamProvider());
            task.Wait(TimeoutConstant.DefaultTimeout);
            result = task.Result;
            Assert.AreEqual(3, result.Count());

            successContent = CreateContent("boundary", "A", "B", "C");
            task = successContent.ReadAsMultipartAsync(new MemoryStreamProvider(), 1024);
            task.Wait(TimeoutConstant.DefaultTimeout);
            result = task.Result;
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parse empty content synchronously.")]
        public void ReadAsMultipartEmptyMessage()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                IEnumerable<HttpContent> parts = content.ReadAsMultipart();
                Assert.AreEqual(0, parts.Count());
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parse empty content asynchronously.")]
        public void ReadAsMultipartEmptyMessageAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary);
                Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                task.Wait(TimeoutConstant.DefaultTimeout);
                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with stream provider that throws on getstream synchronously.")]
        public void ReadAsMultipartBadStreamProvider()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.Throws<InvalidOperationException>(
                    () =>
                    {
                        content.ReadAsMultipart(new BadStreamProvider());
                    },
                    (exception) =>
                    {
                        Assert.IsNotNull(exception.InnerException);
                        Assert.AreEqual(ExceptionStreamProviderMessage, exception.InnerException.Message);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with stream provider that throws on getstream asynchronously.")]
        public void ReadAsMultipartBadStreamProviderAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.ThrowsAggregateException<InvalidOperationException, Exception>(
                    null,
                    ExceptionStreamProviderMessage,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync(new BadStreamProvider());
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with stream provider that returns null on getstream synchronously.")]
        public void ReadAsMultipartNullStreamProvider()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.Throws<InvalidOperationException>(
                    () =>
                    {
                        content.ReadAsMultipart(new NullStreamProvider());
                    },
                    (exception) =>
                    {
                        Assert.IsNull(exception.InnerException);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with stream provider that returns null on getstream asynchronously.")]
        public void ReadAsMultipartNullStreamProviderAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.ThrowsAggregateException<InvalidOperationException>(
                    null,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync(new NullStreamProvider());
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with stream provider that returns read-only stream on getstream synchronously.")]
        public void ReadAsMultipartReadOnlyStreamProvider()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.Throws<InvalidOperationException>(
                    () =>
                    {
                        content.ReadAsMultipart(new ReadOnlyStreamProvider());
                    },
                    (exception) =>
                    {
                        Assert.IsNull(exception.InnerException);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with stream provider that returns read-only stream on getstream asynchronously.")]
        public void ReadAsMultipartReadOnlyStreamProviderAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.ThrowsAggregateException<InvalidOperationException>(
                    null,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync(new ReadOnlyStreamProvider());
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with stream provider that returns end of stream prematurely synchronously.")]
        public void ReadAsMultipartPrematureEndOfReadStream()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = new StreamContent(Stream.Null);
                var contentType = new MediaTypeHeaderValue("multipart/form-data");
                contentType.Parameters.Add(new NameValueHeaderValue("boundary", string.Format("\"{0}\"", boundary)));
                content.Headers.ContentType = contentType;
                Asserters.Exception.Throws<IOException>(
                    () =>
                    {
                        content.ReadAsMultipart();
                    },
                    (exception) =>
                    {
                        Assert.IsNull(exception.InnerException);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with stream provider that returns end of stream prematurely asynchronously.")]
        public void ReadAsMultipartPrematureEndOfReadStreamAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = new StreamContent(Stream.Null);
                var contentType = new MediaTypeHeaderValue("multipart/form-data");
                contentType.Parameters.Add(new NameValueHeaderValue("boundary", string.Format("\"{0}\"", boundary)));
                content.Headers.ContentType = contentType;
                Asserters.Exception.ThrowsAggregateException<IOException>(
                    null,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with read stream error synchronously.")]
        public void ReadAsMultipartReadErrorStream()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = new StreamContent(new ReadErrorStream());
                var contentType = new MediaTypeHeaderValue("multipart/form-data");
                contentType.Parameters.Add(new NameValueHeaderValue("boundary", string.Format("\"{0}\"", boundary)));
                content.Headers.ContentType = contentType;
                Asserters.Exception.Throws<IOException>(
                    () =>
                    {
                        content.ReadAsMultipart();
                    },
                    (exception) =>
                    {
                        Assert.IsNotNull(exception.InnerException);
                        Assert.AreEqual(ExceptionSyncStreamMessage, exception.InnerException.Message);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with read stream error asynchronously.")]
        public void ReadAsMultipartReadErrorStreamAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = new StreamContent(new ReadErrorStream());
                var contentType = new MediaTypeHeaderValue("multipart/form-data");
                contentType.Parameters.Add(new NameValueHeaderValue("boundary", string.Format("\"{0}\"", boundary)));
                content.Headers.ContentType = contentType;
                Asserters.Exception.ThrowsAggregateException<IOException, Exception>(
                    null,
                    ExceptionAsyncStreamMessage,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent, IMultipartStreamProvider) parses with write stream error synchronously.")]
        public void ReadAsMultipartWriteErrorStream()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.Throws<IOException>(
                    () =>
                    {
                        content.ReadAsMultipart(new WriteErrorStreamProvider());
                    },
                    (exception) =>
                    {
                        Assert.IsNotNull(exception.InnerException);
                        Assert.AreEqual(ExceptionSyncStreamMessage, exception.InnerException.Message);
                    });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent, IMultipartStreamProvider) parses with write stream error synchronously.")]
        public void ReadAsMultipartWriteErrorStreamAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A", "B", "C");
                Asserters.Exception.ThrowsAggregateException<IOException, Exception>(
                    null,
                    ExceptionAsyncStreamMessage,
                    () =>
                    {
                        Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync(new WriteErrorStreamProvider());
                        task.Wait(TimeoutConstant.DefaultTimeout);
                    },
                    (aggregateException) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses with single short body synchronously.")]
        public void ReadAsMultipartSingleShortBodyPart()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A");
                IEnumerable<HttpContent> result = content.ReadAsMultipart();
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual("A", result.ElementAt(0).ReadAsStringAsync().Result);
                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses with single short body asynchronously.")]
        public void ReadAsMultipartSingleShortBodyPartAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, "A");
                Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                task.Wait(TimeoutConstant.DefaultTimeout);
                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual("A", result.ElementAt(0).ReadAsStringAsync().Result);
                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses with multiple short bodies synchronously.")]
        public void ReadAsMultipartMultipleShortBodyParts()
        {
            string[] text = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, text);
                IEnumerable<HttpContent> result = content.ReadAsMultipart();
                Assert.AreEqual(text.Length, result.Count());
                for (var check = 0; check < text.Length; check++)
                {
                    Assert.AreEqual(text[check], result.ElementAt(check).ReadAsStringAsync().Result);
                }

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses with multiple short bodies asynchronously.")]
        public void ReadAsMultipartMultipleShortBodyPartsAsync()
        {
            string[] text = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, text);
                Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                task.Wait(TimeoutConstant.DefaultTimeout);

                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(text.Length, result.Count());
                for (var check = 0; check < text.Length; check++)
                {
                    Assert.AreEqual(text[check], result.ElementAt(check).ReadAsStringAsync().Result);
                }

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses with single long body synchronously.")]
        public void ReadAsMultipartSingleLongBodyPart()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, LongText);
                IEnumerable<HttpContent> result = content.ReadAsMultipart();
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(LongText, result.ElementAt(0).ReadAsStringAsync().Result);

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses with single long body asynchronously.")]
        public void ReadAsMultipartSingleLongBodyPartAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, LongText);
                Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                task.Wait(TimeoutConstant.DefaultTimeout);
                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(LongText, result.ElementAt(0).ReadAsStringAsync().Result);

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses with multiple long bodies synchronously.")]
        public void ReadAsMultipartMultipleLongBodyParts()
        {
            string[] text = new string[] { 
                "A" + LongText + "A", 
                "B" + LongText + "B", 
                "C" + LongText + "C", 
                "D" + LongText + "D", 
                "E" + LongText + "E", 
                "F" + LongText + "F", 
                "G" + LongText + "G", 
                "H" + LongText + "H", 
                "I" + LongText + "I", 
                "J" + LongText + "J", 
                "K" + LongText + "K", 
                "L" + LongText + "L", 
                "M" + LongText + "M", 
                "N" + LongText + "N", 
                "O" + LongText + "O", 
                "P" + LongText + "P", 
                "Q" + LongText + "Q", 
                "R" + LongText + "R", 
                "S" + LongText + "S", 
                "T" + LongText + "T", 
                "U" + LongText + "U", 
                "V" + LongText + "V", 
                "W" + LongText + "W", 
                "X" + LongText + "X", 
                "Y" + LongText + "Y", 
                "Z" + LongText + "Z"};

            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, text);
                IEnumerable<HttpContent> result = content.ReadAsMultipart(new MemoryStreamProvider(), ParserData.MinBufferSize);
                Assert.AreEqual(text.Length, result.Count());
                for (var check = 0; check < text.Length; check++)
                {
                    Assert.AreEqual(text[check], result.ElementAt(check).ReadAsStringAsync().Result);
                }

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses with multiple long bodies asynchronously.")]
        public void ReadAsMultipartMultipleLongBodyPartsAsync()
        {
            string[] text = new string[] { 
                "A" + LongText + "A", 
                "B" + LongText + "B", 
                "C" + LongText + "C", 
                "D" + LongText + "D", 
                "E" + LongText + "E", 
                "F" + LongText + "F", 
                "G" + LongText + "G", 
                "H" + LongText + "H", 
                "I" + LongText + "I", 
                "J" + LongText + "J", 
                "K" + LongText + "K", 
                "L" + LongText + "L", 
                "M" + LongText + "M", 
                "N" + LongText + "N", 
                "O" + LongText + "O", 
                "P" + LongText + "P", 
                "Q" + LongText + "Q", 
                "R" + LongText + "R", 
                "S" + LongText + "S", 
                "T" + LongText + "T", 
                "U" + LongText + "U", 
                "V" + LongText + "V", 
                "W" + LongText + "W", 
                "X" + LongText + "X", 
                "Y" + LongText + "Y", 
                "Z" + LongText + "Z"};

            foreach (var boundary in ParserData.Boundaries)
            {
                HttpContent content = CreateContent(boundary, text);
                Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync(new MemoryStreamProvider(), ParserData.MinBufferSize);
                task.Wait(TimeoutConstant.DefaultTimeout);
                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(text.Length, result.Count());
                for (var check = 0; check < text.Length; check++)
                {
                    Assert.AreEqual(text[check], result.ElementAt(check).ReadAsStringAsync().Result);
                }

                ValidateContents(result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses content generated by MultipartContent synchronously.")]
        public void ReadAsMultipartUsingMultipartContent()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                MultipartContent content = new MultipartContent("mixed", boundary);
                content.Add(new StringContent("A"));
                content.Add(new StringContent("B"));
                content.Add(new StringContent("C"));

                MemoryStream memStream = new MemoryStream();
                content.CopyToAsync(memStream).Wait();
                memStream.Position = 0;
                byte[] data = memStream.ToArray();
                var byteContent = new ByteArrayContent(data);
                byteContent.Headers.ContentType = content.Headers.ContentType;

                IEnumerable<HttpContent> result = byteContent.ReadAsMultipart();
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual("A", result.ElementAt(0).ReadAsStringAsync().Result);
                Assert.AreEqual("B", result.ElementAt(1).ReadAsStringAsync().Result);
                Assert.AreEqual("C", result.ElementAt(2).ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses content generated by MultipartContent asynchronously.")]
        public void ReadAsMultipartUsingMultipartContentAsync()
        {
            foreach (var boundary in ParserData.Boundaries)
            {
                MultipartContent content = new MultipartContent("mixed", boundary);
                content.Add(new StringContent("A"));
                content.Add(new StringContent("B"));
                content.Add(new StringContent("C"));

                MemoryStream memStream = new MemoryStream();
                content.CopyToAsync(memStream).Wait();
                memStream.Position = 0;
                byte[] data = memStream.ToArray();
                var byteContent = new ByteArrayContent(data);
                byteContent.Headers.ContentType = content.Headers.ContentType;

                Task<IEnumerable<HttpContent>> task = byteContent.ReadAsMultipartAsync();
                task.Wait(TimeoutConstant.DefaultTimeout);
                IEnumerable<HttpContent> result = task.Result;
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual("A", result.ElementAt(0).ReadAsStringAsync().Result);
                Assert.AreEqual("B", result.ElementAt(1).ReadAsStringAsync().Result);
                Assert.AreEqual("C", result.ElementAt(2).ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipart(HttpContent) parses nested content generated by MultipartContent synchronously.")]
        public void ReadAsMultipartNestedMultipartContent()
        {
            const int nesting = 10;
            const string innerText = "Content";

            foreach (var boundary in ParserData.Boundaries)
            {
                MultipartContent innerContent = new MultipartContent("mixed", boundary);
                innerContent.Add(new StringContent(innerText));
                for (var cnt = 0; cnt < nesting; cnt++)
                {
                    string outerBoundary = string.Format("{0}_{1}", boundary, cnt);
                    MultipartContent outerContent = new MultipartContent("mixed", outerBoundary);
                    outerContent.Add(innerContent);
                    innerContent = outerContent;
                }

                MemoryStream memStream = new MemoryStream();
                innerContent.CopyToAsync(memStream).Wait();
                memStream.Position = 0;
                byte[] data = memStream.ToArray();
                HttpContent content = new ByteArrayContent(data);
                content.Headers.ContentType = innerContent.Headers.ContentType;

                for (var cnt = 0; cnt < nesting + 1; cnt++)
                {
                    IEnumerable<HttpContent> result = content.ReadAsMultipart();
                    Assert.AreEqual(1, result.Count(), "Expected one nested content.");
                    content = result.ElementAt(0);
                    Assert.IsNotNull(content, "Did not find nested content.");
                }

                string text = content.ReadAsStringAsync().Result;
                Assert.AreEqual(innerText, text, "Unexpected inner text.");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMimeMultipart)]
        [Description("ReadAsMultipartAsync(HttpContent) parses nested content generated by MultipartContent asynchronously.")]
        public void ReadAsMultipartNestedMultipartContentAsync()
        {
            const int nesting = 10;
            const string innerText = "Content";

            foreach (var boundary in ParserData.Boundaries)
            {
                MultipartContent innerContent = new MultipartContent("mixed", boundary);
                innerContent.Add(new StringContent(innerText));
                for (var cnt = 0; cnt < nesting; cnt++)
                {
                    string outerBoundary = string.Format("{0}_{1}", boundary, cnt);
                    MultipartContent outerContent = new MultipartContent("mixed", outerBoundary);
                    outerContent.Add(innerContent);
                    innerContent = outerContent;
                }

                MemoryStream memStream = new MemoryStream();
                innerContent.CopyToAsync(memStream).Wait();
                memStream.Position = 0;
                byte[] data = memStream.ToArray();
                HttpContent content = new ByteArrayContent(data);
                content.Headers.ContentType = innerContent.Headers.ContentType;

                for (var cnt = 0; cnt < nesting + 1; cnt++)
                {
                    Task<IEnumerable<HttpContent>> task = content.ReadAsMultipartAsync();
                    task.Wait(TimeoutConstant.DefaultTimeout);
                    IEnumerable<HttpContent> result = task.Result;
                    Assert.AreEqual(1, result.Count(), "Expected one nested content.");
                    content = result.ElementAt(0);
                    Assert.IsNotNull(content, "Did not find nested content.");
                }

                string text = content.ReadAsStringAsync().Result;
                Assert.AreEqual(innerText, text, "Unexpected inner text.");
            }
        }

        #endregion

        #region Mocks

        public class ReadOnlyStream : MemoryStream
        {
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }
        }

        public class ReadErrorStream : MemoryStream
        {
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new Exception(ExceptionSyncStreamMessage);
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                throw new Exception(ExceptionAsyncStreamMessage);
            }
        }

        public class WriteErrorStream : MemoryStream
        {
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new Exception(ExceptionSyncStreamMessage);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                throw new Exception(ExceptionAsyncStreamMessage);
            }
        }

        public class MemoryStreamProvider : IMultipartStreamProvider
        {
            public Stream GetStream(HttpContentHeaders headers)
            {
                return new MemoryStream();
            }
        }

        public class BadStreamProvider : IMultipartStreamProvider
        {
            public Stream GetStream(HttpContentHeaders headers)
            {
                throw new Exception(ExceptionStreamProviderMessage);
            }
        }

        public class NullStreamProvider : IMultipartStreamProvider
        {
            public Stream GetStream(HttpContentHeaders headers)
            {
                return null;
            }
        }

        public class ReadOnlyStreamProvider : IMultipartStreamProvider
        {
            public Stream GetStream(HttpContentHeaders headers)
            {
                return new ReadOnlyStream();
            }
        }

        public class WriteErrorStreamProvider : IMultipartStreamProvider
        {
            public Stream GetStream(HttpContentHeaders headers)
            {
                return new WriteErrorStream();
            }
        }

        #endregion
    }
}
