using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test
{
    [TestClass]
    public class StringContentTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullString_Throw()
        {
            StringContent content = new StringContent(null);
        }

        [TestMethod]
        public void Ctor_EmptyString_Accept()
        {
            // Consider empty strings like null strings (null and empty strings should be treated equally).
            StringContent content = new StringContent(string.Empty);
            Assert.AreEqual(0, content.ReadAsStreamAsync().Result.Length, "Length");
        }

        [TestMethod]
        public void Ctor_UseCustomEncoding_EncodingUsedAndContentTypeHeaderUpdated()
        {
            // Use content with turkish dot-less lower-case i and uppercase I with dot. Verify that the StringContent
            // not only serializes the string using the provided encoding, but also updates the 'Content-Type' header
            // value.
            string sourceString = "i\u0131I\u0130";
            Encoding encoding = Encoding.GetEncoding("iso-8859-9");

            StringContent content = new StringContent(sourceString, encoding);

            // We didn't provide a media type, so expect the default media type to be used.
            Assert.AreEqual("text/plain", content.Headers.ContentType.MediaType, "Expected media type.");
            Assert.AreEqual("iso-8859-9", content.Headers.ContentType.CharSet, "Expected charset.");

            MemoryStream destination = new MemoryStream(4);
            content.CopyTo(destination);
            byte[] serializedString = destination.GetBuffer();

            Assert.AreEqual(4, destination.Length, "Expected content length.");

            // The source string serialized using the turkish encoding results in the following byte array: 69 fd 49 dd
            Assert.AreEqual(0x69, serializedString[0], "First byte");
            Assert.AreEqual(0xfd, serializedString[1], "Second byte");
            Assert.AreEqual(0x49, serializedString[2], "Third byte");
            Assert.AreEqual(0xdd, serializedString[3], "Fourth byte");
        }

        [TestMethod]
        public void Ctor_UseCustomEncodingAndMediaType_EncodingUsedAndContentTypeHeaderUpdated()
        {
            // Use UTF-8 encoding to serialize a chinese string.
            string sourceString = "会员服务";

            StringContent content = new StringContent(sourceString, Encoding.UTF8, "application/custom");

            Assert.AreEqual("application/custom", content.Headers.ContentType.MediaType, "Expected media type.");
            Assert.AreEqual("utf-8", content.Headers.ContentType.CharSet, "Expected charset.");

            MemoryStream destination = new MemoryStream(12);
            content.CopyTo(destination);

            string destinationString = Encoding.UTF8.GetString(destination.GetBuffer(), 0, (int)destination.Length);

            Assert.AreEqual(sourceString, destinationString, "Expected source and destination strings to be equal.");
        }

        [TestMethod]
        public void Ctor_DefineNoEncoding_DefaultEncodingUsed()
        {
            string sourceString = "ÄäüÜ"; // c4 e4 fc dc
            StringContent content = new StringContent(sourceString);
            Encoding defaultStringEncoding = Encoding.GetEncoding("utf-8");

            // If no encoding is defined, the default encoding is used: utf-8
            Assert.AreEqual("text/plain", content.Headers.ContentType.MediaType, "Expected media type.");
            Assert.AreEqual(defaultStringEncoding.WebName, content.Headers.ContentType.CharSet, 
                "Expected default encoding.");

            // Make sure the default encoding is also used when serializing the content.
            MemoryStream destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(8, destination.Length, "Expected destination stream length.");

            destination.Seek(0, SeekOrigin.Begin);
            String roundTrip = new StreamReader(destination, defaultStringEncoding).ReadToEnd();
            Assert.AreEqual(sourceString, roundTrip, "Round trip encoding failure");
        }
    }
}
