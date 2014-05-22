using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Test.Common;

namespace System.Net.Http.Test
{
    [TestClass]
    public class MultipartContentTest
    {
        [TestMethod]
        public void Ctor_NullOrEmptySubType_Throw()
        {
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent(null); }, "<null> SubType");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent(""); }, "Empty SubType");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent(" "); }, "Blank SubType");
        }

        [TestMethod]
        public void Ctor_NullOrEmptyBoundary_Throw()
        {
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", null); }, "<null>");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", ""); }, "Empty");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", " "); }, "Blank");
        }

        [TestMethod]
        public void Ctor_BadBoundary_Throw()
        {
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "EndsInSpace "); }, "EndsInSpace ");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some",
                "LongerThan70CharactersLongerThan70CharactersLongerThan70CharactersLongerThan70CharactersLongerThan70Characters");
            }, "Too long Boundary");
            // Invalid chars CTLs HT < > @ ; \ " [ ] { } ! # $ % & ^ ~ `
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "a\t"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "<"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "@"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "["); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "{"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "!"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "#"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "$"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "%"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "&"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "^"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "~"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "`"); }, "Invalid Boundary");
            ExceptionAssert.Throws<ArgumentException>(() => { new MultipartContent("Some", "\"quoted\""); }, "Invalid Boundary");
        }

        [TestMethod]
        public void Ctor_GoodBoundary_Success()
        {
            // RFC 2046 Section 5.1.1
            // boundary := 0*69<bchars> bcharsnospace
            // bchars := bcharsnospace / " "
            // bcharsnospace := DIGIT / ALPHA / "'" / "(" / ")" / "+" / "_" / "," / "-" / "." / "/" / ":" / "=" / "?"
            new MultipartContent("some", "09");
            new MultipartContent("some", "az");
            new MultipartContent("some", "AZ");
            new MultipartContent("some", "'");
            new MultipartContent("some", "(");
            new MultipartContent("some", "+");
            new MultipartContent("some", "_");
            new MultipartContent("some", ",");
            new MultipartContent("some", "-");
            new MultipartContent("some", ".");
            new MultipartContent("some", "/");
            new MultipartContent("some", ":");
            new MultipartContent("some", "=");
            new MultipartContent("some", "?");
            new MultipartContent("some", "Contains Space");
            new MultipartContent("some", " StartsWithSpace");
            new MultipartContent("some", Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void Ctor_Headers_AutomaticallyCreated()
        {
            MultipartContent content = new MultipartContent("test_subtype", "test_boundary");
            Assert.AreEqual("multipart/test_subtype", content.Headers.ContentType.MediaType);
            Assert.AreEqual(1, content.Headers.ContentType.Parameters.Count);
        }

        [TestMethod]
        public void Serialize_EmptyList_Success()
        {
            MultipartContent content = new MultipartContent("mixed","test_boundary");
            MemoryStream output = new MemoryStream();
            content.CopyTo(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            long length;
            Assert.IsTrue(content.TryComputeLength(out length));
            Assert.AreEqual(result.Length, length);

            Assert.AreEqual("--test_boundary\r\n\r\n--test_boundary--\r\n", result);
        }

        [TestMethod]
        public void Serialize_StringContent_Success()
        {
            MultipartContent content = new MultipartContent("mixed", "test_boundary");
            content.Add(new StringContent("Hello World"));

            MemoryStream output = new MemoryStream();
            content.CopyTo(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            long length;
            Assert.IsTrue(content.TryComputeLength(out length));
            Assert.AreEqual(result.Length, length);

            Assert.AreEqual(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8"
                + "\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [TestMethod]
        public void Serialize_EmptyFormUrlEncodedContent_Success()
        {
            MultipartContent content = new MultipartContent("mixed", "test_boundary");
            content.Add(new FormUrlEncodedContent(new Dictionary<string, string>()));

            MemoryStream output = new MemoryStream();
            content.CopyTo(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            long length;
            Assert.IsTrue(content.TryComputeLength(out length));
            Assert.AreEqual(result.Length, length);

            Assert.AreEqual(
                "--test_boundary\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\n\r\n--test_boundary--\r\n",
                result);
        }

        [TestMethod]
        public void Serialize_StreamContent_Success()
        {
            MultipartContent content = new MultipartContent("mixed", "test_boundary");
            content.Add(new StreamContent(new MemoryStream(new byte[] {65,66,67,68,69,70})));

            MemoryStream output = new MemoryStream();
            content.CopyTo(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            long length;
            Assert.IsTrue(content.TryComputeLength(out length));
            Assert.AreEqual(result.Length, length);

            Assert.AreEqual("--test_boundary\r\n\r\nABCDEF\r\n--test_boundary--\r\n", result);
        }

        [TestMethod]
        public void Serialize_AddStringContentTwice_TwoCopiesSerialized()
        {
            MultipartContent content = new MultipartContent("mixed", "test_boundary");
            HttpContent stringContent = new StringContent("Hello World");
            content.Add(stringContent);
            content.Add(stringContent);

            MemoryStream output = new MemoryStream();
            content.CopyTo(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            long length;
            Assert.IsTrue(content.TryComputeLength(out length));
            Assert.AreEqual(result.Length, length);

            Assert.AreEqual(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n\r\nHello World"
                + "\r\n--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n\r\nHello World"
                + "\r\n--test_boundary--\r\n",
                result);
        }

        [TestMethod]
        public void Serialize_SyncMatchesAsync_Success()
        {
            MultipartContent content1 = new MultipartContent("mixed", "test_boundary");
            content1.Add(new StringContent("Hello World"));
            content1.Add(new FormUrlEncodedContent(new Dictionary<string, string>()));
            content1.Add(new StreamContent(new MemoryStream(new byte[] { 65, 66, 67, 68, 69, 70 })));

            MemoryStream output1 = new MemoryStream();
            content1.CopyTo(output1);
            output1.Seek(0, SeekOrigin.Begin);
            string result1 = new StreamReader(output1).ReadToEnd();

            MultipartContent content2 = new MultipartContent("mixed", "test_boundary");
            content2.Add(new StringContent("Hello World"));
            content2.Add(new FormUrlEncodedContent(new Dictionary<string, string>()));
            content2.Add(new StreamContent(new MemoryStream(new byte[] { 65, 66, 67, 68, 69, 70 })));

            MemoryStream output2 = new MemoryStream();
            Task task = content2.CopyToAsync(output2);
            Assert.IsTrue(task.Wait(1000), "Timed out");
            output2.Seek(0, SeekOrigin.Begin);
            string result2 = new StreamReader(output2).ReadToEnd();

            long length1, length2;
            Assert.IsTrue(content1.TryComputeLength(out length1));
            Assert.IsTrue(content2.TryComputeLength(out length2));
            Assert.AreEqual(length1, length2);
            Assert.AreEqual(length1, result1.Length);
            Assert.AreEqual(length2, result2.Length);

            Assert.AreEqual(result1, result2);
        }

        [TestMethod]
        public void SerializeAsync_MultipleTimes_Success()
        {
            MultipartContent content = new MultipartContent("mixed", "test_boundary");
            content.Add(new StringContent("Hello World"));
            content.Add(new FormUrlEncodedContent(new Dictionary<string, string>()));
            content.Add(new StreamContent(new MemoryStream(new byte[] { 65, 66, 67, 68, 69, 70 })));

            for (int i = 0; i < 10; i++)
            {
                MemoryStream output = new MemoryStream();
                Task task = content.CopyToAsync(output);
                Assert.IsTrue(task.Wait(1000), "Timed out");
                output.Seek(0, SeekOrigin.Begin);
                string result = new StreamReader(output).ReadToEnd();

                long length;
                Assert.IsTrue(content.TryComputeLength(out length));
                Assert.AreEqual(length, result.Length);
            }
        }

        [TestMethod]
        public void Dispose_Empty_Sucess()
        {
            MultipartContent content = new MultipartContent();
            content.Dispose();
        }

        [TestMethod]
        public void Dispose_InnerContent_InnerContentDisposed()
        {
            MultipartContent content = new MultipartContent();
            MockContent innerContent = new MockContent();
            content.Add(innerContent);
            content.Dispose();
            Assert.AreEqual(1, innerContent.DisposeCount);
            content.Dispose();
            // Inner content is discarded after first dispose
            Assert.AreEqual(1, innerContent.DisposeCount);
        }

        [TestMethod]
        public void Dispose_NestedContent_NestedContentDisposed()
        {
            MultipartContent outer = new MultipartContent();
            MultipartContent inner = new MultipartContent();
            outer.Add(inner);
            MockContent mock = new MockContent();
            inner.Add(mock);
            outer.Dispose();
            Assert.AreEqual(1, mock.DisposeCount);
            outer.Dispose();
            // Inner content is discarded after first dispose
            Assert.AreEqual(1, mock.DisposeCount);
        }

        #region Helpers

        private class MockContent : HttpContent
        {
            public int DisposeCount { get; private set; }

            public MockContent() { }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                throw new NotImplementedException();
            }

            protected internal override bool TryComputeLength(out long length)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Helpers
    }
}
