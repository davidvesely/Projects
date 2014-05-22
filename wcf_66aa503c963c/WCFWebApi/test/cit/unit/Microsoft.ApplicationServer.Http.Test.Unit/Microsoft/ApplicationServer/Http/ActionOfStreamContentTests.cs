// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.IO;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionOfStreamContentTests
    {
        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("ActionOfStreamContent.ReadAsStringAsync calls the inner action of stream.")]
        public void ReadAsString_Calls_The_ActionOfStream()
        {
            bool actionCalled = false;
            Action<Stream> actionOfStream = (stream) =>
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write("Hello");
                    writer.Flush();
                    actionCalled = true;
                };

            ActionOfStreamContent content = new ActionOfStreamContent(actionOfStream);
            Assert.IsFalse(actionCalled, "The actionOfStream should not have been called yet.");
            Assert.AreEqual("Hello", content.ReadAsStringAsync().Result, "The content should have been 'Hello'.");
            Assert.IsTrue(actionCalled, "The actionOfStream should have been called.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("ActionOfStreamContent.Headers.ContentLength returns null.")]
        public void ContentLength_Returns_Null()
        {
            bool actionCalled = false;
            Action<Stream> actionOfStream = (stream) =>
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write("Hello");
                writer.Flush();
                actionCalled = true;
            };

            ActionOfStreamContent content = new ActionOfStreamContent(actionOfStream);
            Assert.IsNull(content.Headers.ContentLength, "The content length should have been null.");
            Assert.IsFalse(actionCalled, "The actionOfStream should not have been called yet.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("ActionOfStreamContent.ReadAsStreamAsync calls the inner action of stream.")]
        public void ContentReadStream_Calls_The_ActionOfStream()
        {
            bool actionCalled = false;
            Action<Stream> actionOfStream = (stream) =>
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write("Hello");
                writer.Flush();
                actionCalled = true;
            };

            ActionOfStreamContent content = new ActionOfStreamContent(actionOfStream);
            Stream contentStream = content.ReadAsStreamAsync().Result;
            contentStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(contentStream);
            Assert.IsTrue(actionCalled, "The actionOfStream should have been called.");
            Assert.AreEqual("Hello", reader.ReadToEnd(), "The content should have been 'Hello'.");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("vinelap")]
        [Description("ActionOfStreamContent.CopyToAsync calls the inner action of stream.")]
        public void CopyTo_Calls_The_ActionOfStream()
        {
            bool actionCalled = false;
            Stream writtenToStream = null;
            Action<Stream> actionOfStream = (stream) =>
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write("Hello");
                writer.Flush();
                actionCalled = true;
                writtenToStream = stream;
            };

            ActionOfStreamContent content = new ActionOfStreamContent(actionOfStream);
            Assert.IsFalse(actionCalled, "The actionOfStream should not have been called yet.");

            MemoryStream memoryStream = new MemoryStream();
            content.CopyToAsync(memoryStream).Wait();
            Assert.IsTrue(actionCalled, "The actionOfStream should have been called.");

            memoryStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memoryStream);
            Assert.AreEqual("Hello", reader.ReadToEnd(), "The content should have been 'Hello'.");

            Assert.AreSame(writtenToStream, memoryStream, "The ActionOfStream should have written to the memory stream.");
        }
    }
}
