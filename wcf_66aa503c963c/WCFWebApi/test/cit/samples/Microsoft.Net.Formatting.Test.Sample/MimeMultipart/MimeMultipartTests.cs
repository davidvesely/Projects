// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.CIT.Scenarios
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Ionic.Zlib;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MimeMultipart.Sample;

    /// <summary>
    /// Integration tests for <see cref="Program"/> sample.
    /// </summary>
    [TestClass]
    public class MimeMultipartTests
    {
        private const string FileData = "01234567890123456789012345678901234567890";
        private const string TypeData = "[{\"Age\":1,\"Avatar\":\"http:\\/\\/www.example.com\\/path\\/1\",\"Id\":1,\"Name\":\"1\"}]";
        private const string TypeServiceAddress = "http://localhost:8080/typeservice";
        private const string FileServiceAddress = "http://localhost:8081/fileservice";
        private const string DotNetZipServiceAddress = "http://localhost:8082/dotnetzipservice";
        private const string SplitFileServiceAddress = "http://localhost:8083/splitfileservice";

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify that multipart scenarios hosts open correctly.")]
        public void MimeMultipart_OpenCloseHosts()
        {
            Program.OpenHosts();
            Program.CloseHosts();
            Program.CloseHosts();
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify TypeService is running correctly.")]
        public void MimeMultipart_TypeServiceTestBasic()
        {
            try
            {
                Program.OpenHosts();
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent("submitter"), "submitter");
                    content.Add(new StringContent(TypeData, Encoding.UTF8, "application/json"), "data");
                    using (var response = client.PostAsync(TypeServiceAddress, content).Result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
                        List<Contact> contacts = response.Content.ReadAsAsync<List<Contact>>().Result;
                        Assert.IsNotNull(contacts);
                        Assert.AreEqual(1, contacts.Count());
                        Assert.AreEqual(1, contacts.ElementAt(0).Age);
                    }
                }
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify FileService is running correctly with a single file uploaded.")]
        public void MimeMultipart_FileServiceTestSingleFile()
        {
            try
            {
                Program.OpenHosts();
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent("submitter"), "submitter");
                    content.Add(new StringContent(FileData), "data", "filename");
                    using (var response = client.PostAsync(FileServiceAddress, content).Result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
                        List<FileResult> results = response.Content.ReadAsAsync<List<FileResult>>().Result;
                        Assert.IsNotNull(results);
                        Assert.AreEqual(1, results.Count());
                        Assert.AreEqual(FileData.Length, results.ElementAt(0).Length);
                    }
                }
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify FileService is running correctly with multiple files uploaded simultaneously.")]
        public void MimeMultipart_FileServiceTestMultipleFiles()
        {
            try
            {
                Program.OpenHosts();
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent("submitter"), "submitter");
                    content.Add(new StringContent(FileData), "data", "filename1");
                    content.Add(new StringContent(FileData), "data", "filename2");
                    content.Add(new StringContent(FileData), "data", "filename3");
                    using (var response = client.PostAsync(FileServiceAddress, content).Result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
                        List<FileResult> results = response.Content.ReadAsAsync<List<FileResult>>().Result;
                        Assert.IsNotNull(results);
                        Assert.AreEqual(3, results.Count());
                        Assert.AreEqual(FileData.Length, results.ElementAt(0).Length);
                    }
                }
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify DotNetZipService is running correctly.")]
        public void MimeMultipart_DotNetZipServiceBasic()
        {
            // Generate compressed data
            MemoryStream memStream = new MemoryStream();
            using (Stream deflate = new Ionic.Zlib.DeflateStream(memStream, CompressionMode.Compress, true))
            {
                var bytes = Encoding.UTF8.GetBytes(FileData);
                deflate.Write(bytes, 0, bytes.Length);
            }

            try
            {
                Program.OpenHosts();
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent("submitter"), "submitter");
                    content.Add(new ByteArrayContent(memStream.GetBuffer(), 0, (int)memStream.Length), "data", "filename.deflate");
                    using (var response = client.PostAsync(DotNetZipServiceAddress, content).Result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
                        List<FileResult> results = response.Content.ReadAsAsync<List<FileResult>>().Result;
                        Assert.IsNotNull(results);
                        Assert.AreEqual(1, results.Count());
                        Assert.AreEqual(FileData.Length, results.ElementAt(0).Length, "Server did not uncompress data to original size.");
                    }
                }
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify SplitFileService is running correctly.")]
        public void MimeMultipart_SplitFileServiceBasic()
        {
            try
            {
                Program.OpenHosts();
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent("partial"), "yes");
                    content.Add(new StringContent(FileData), "data", "filename");
                    using (var response = client.PostAsync(SplitFileServiceAddress, content).Result)
                    {
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
                        List<FileResult> results = response.Content.ReadAsAsync<List<FileResult>>().Result;
                        Assert.IsNotNull(results);
                        Assert.IsTrue(results.Count() > 0);
                    }
                }
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify client side reading MIME multipart into memory scenario.")]
        public void MimeMultipart_ClientReadMemory()
        {
            try
            {
                Program.OpenHosts();
                Program.ReadMultipartMemory();
            }
            finally
            {
                Program.CloseHosts();
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify client side reading MIME multipart to files scenario.")]
        public void MimeMultipart_ClientReadFiles()
        {
            try
            {
                Program.OpenHosts();
                Program.ReadMultipartFileAsync();
            }
            finally
            {
                Program.CloseHosts();
            }
        }
        
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Verify client side submitting MIME multipart with file scenario.")]
        [DeploymentItem("Web\\SampleData.random", "Web")]
        public void MimeMultipart_SubmitMultipartFormData()
        {
            try
            {
                Program.OpenHosts();
                Program.SubmitMultipartFormData();
            }
            finally
            {
                Program.CloseHosts();
            }
        }
    }
}