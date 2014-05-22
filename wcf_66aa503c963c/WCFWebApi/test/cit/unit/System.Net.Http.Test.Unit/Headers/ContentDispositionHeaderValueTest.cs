using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Headers;
using System.Net.Test.Common;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class ContentDispositionHeaderValueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_ContentDispositionNull_Throw()
        {
            new ContentDispositionHeaderValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_ContentDispositionEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            new ContentDispositionHeaderValue(string.Empty);
        }

        [TestMethod]
        public void Ctor_ContentDispositionInvalidFormat_ThrowFormatException()
        {
            // When adding values using strongly typed objects, no leading/trailing LWS (whitespaces) are allowed.
            AssertFormatException(" inline ");
            AssertFormatException(" inline"); 
            AssertFormatException("inline ");
            AssertFormatException("\"inline\"");
            AssertFormatException("te xt");
            AssertFormatException("te=xt");
            AssertFormatException("teäxt");
            AssertFormatException("text;");
            AssertFormatException("te/xt;");
            AssertFormatException("inline; name=someName; ");
            AssertFormatException("text;name=someName"); // ctor takes only disposition-type name, no parameters
        }

        [TestMethod]
        public void Ctor_ContentDispositionValidFormat_SuccessfullyCreated()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            Assert.AreEqual("inline", contentDisposition.DispositionType, "DispositionType");
            Assert.AreEqual(0, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.IsNull(contentDisposition.Name, "Name");
            Assert.IsNull(contentDisposition.FileName, "Name");
            Assert.IsNull(contentDisposition.CreationDate, "CreationDate");
            Assert.IsNull(contentDisposition.ModificationDate, "ModificationDate");
            Assert.IsNull(contentDisposition.ReadDate, "ReadDate");
            Assert.IsNull(contentDisposition.Size, "Size");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parameters_AddNull_Throw()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            contentDisposition.Parameters.Add(null);
        }

        [TestMethod]
        public void ContentDisposition_SetAndGetContentDisposition_MatchExpectations()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            Assert.AreEqual("inline", contentDisposition.DispositionType, "After ctor");

            contentDisposition.DispositionType = "attachment";
            Assert.AreEqual("attachment", contentDisposition.DispositionType, "After setting the property");
        }

        [TestMethod]
        public void Name_SetNameAndValidateObject_ParametersEntryForNameAdded()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            contentDisposition.Name = "myname";
            Assert.AreEqual("myname", contentDisposition.Name, "Name");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("name", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Name = null;
            Assert.IsNull(contentDisposition.Name, "Name");
            Assert.AreEqual(0, contentDisposition.Parameters.Count, "Parameters.Count");
            contentDisposition.Name = null; // It's OK to set it again to null; no exception.
        }

        [TestMethod]
        public void Name_AddNameParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            
            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue name = new NameValueHeaderValue("NAME", "old_name");
            contentDisposition.Parameters.Add(name);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("NAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Name = "new_name";
            Assert.AreEqual("new_name", contentDisposition.Name, "Name");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("NAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(name);
            Assert.IsNull(contentDisposition.Name, "Name");
        }

        [TestMethod]
        public void FileName_AddNameParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue fileName = new NameValueHeaderValue("FILENAME", "old_name");
            contentDisposition.Parameters.Add(fileName);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.FileName = "new_name";
            Assert.AreEqual("new_name", contentDisposition.FileName, "FileName");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(fileName);
            Assert.IsNull(contentDisposition.FileName, "FileName");
        }

        [TestMethod]
        public void FileName_NeedsEncoding_EncodedAndDecodedCorrectly()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            contentDisposition.FileName = "FileÃName.bat";
            Assert.AreEqual("FileÃName.bat", contentDisposition.FileName, "FileName");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("filename", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("\"=?utf-8?B?RmlsZcODTmFtZS5iYXQ=?=\"", contentDisposition.Parameters.First().Value, "Parameters.First().Value");

            contentDisposition.Parameters.Remove(contentDisposition.Parameters.First());
            Assert.IsNull(contentDisposition.FileName, "FileName");
        }

        [TestMethod]
        public void FileName_UnknownOrBadEncoding_PropertyFails()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue fileName = new NameValueHeaderValue("FILENAME", "\"=?utf-99?Q?R=mlsZcODTmFtZS5iYXQ=?=\"");
            contentDisposition.Parameters.Add(fileName);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("\"=?utf-99?Q?R=mlsZcODTmFtZS5iYXQ=?=\"", contentDisposition.Parameters.First().Value, "Parameters.First().Value");
            Assert.AreEqual("\"=?utf-99?Q?R=mlsZcODTmFtZS5iYXQ=?=\"", contentDisposition.FileName);

            contentDisposition.FileName = "new_name";
            Assert.AreEqual("new_name", contentDisposition.FileName, "FileName");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(fileName);
            Assert.IsNull(contentDisposition.FileName, "FileName");
        }

        [TestMethod]
        public void FileNameStar_AddNameParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue fileNameStar = new NameValueHeaderValue("FILENAME*", "old_name");
            contentDisposition.Parameters.Add(fileNameStar);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME*", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.IsNull(contentDisposition.FileNameStar); // Decode failure

            contentDisposition.FileNameStar = "new_name";
            Assert.AreEqual("new_name", contentDisposition.FileNameStar, "FileNameStar");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME*", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("utf-8\'\'new_name", contentDisposition.Parameters.First().Value, "Parameters.First().Value");

            contentDisposition.Parameters.Remove(fileNameStar);
            Assert.IsNull(contentDisposition.FileNameStar, "FileName");
        }

        [TestMethod]
        public void FileNameStar_NeedsEncoding_EncodedAndDecodedCorrectly()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            contentDisposition.FileNameStar = "FileÃName.bat";
            Assert.AreEqual("FileÃName.bat", contentDisposition.FileNameStar, "FileNameStar");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("filename*", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("utf-8\'\'File%C3%83Name.bat", contentDisposition.Parameters.First().Value, "Parameters.First().Value");

            contentDisposition.Parameters.Remove(contentDisposition.Parameters.First());
            Assert.IsNull(contentDisposition.FileNameStar, "FileNameStar");
        }

        [TestMethod]
        public void FileNameStar_UnknownOrBadEncoding_PropertyFails()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue fileNameStar = new NameValueHeaderValue("FILENAME*", "utf-99'lang'File%CZName.bat");
            contentDisposition.Parameters.Add(fileNameStar);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME*", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("utf-99'lang'File%CZName.bat", contentDisposition.Parameters.First().Value, "Parameters.First().Value");
            Assert.IsNull(contentDisposition.FileNameStar); // Decode failure

            contentDisposition.FileNameStar = "new_name";
            Assert.AreEqual("new_name", contentDisposition.FileNameStar, "FileNameStar");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("FILENAME*", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(fileNameStar);
            Assert.IsNull(contentDisposition.FileNameStar, "FileNameStar");
        }
        
        [TestMethod]
        public void Dates_AddDateParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            string validDateString = "\"Tue, 15 Nov 1994 08:12:31 GMT\"";
            DateTimeOffset validDate = DateTimeOffset.Parse("Tue, 15 Nov 1994 08:12:31 GMT");

            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue dateParameter = new NameValueHeaderValue("Creation-DATE", validDateString);
            contentDisposition.Parameters.Add(dateParameter);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("Creation-DATE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            Assert.AreEqual(validDate, contentDisposition.CreationDate);

            DateTimeOffset newDate = validDate.AddSeconds(1);
            contentDisposition.CreationDate = newDate;
            Assert.AreEqual(newDate, contentDisposition.CreationDate, "CreationDate");            
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("Creation-DATE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual("\"Tue, 15 Nov 1994 08:12:32 GMT\"", contentDisposition.Parameters.First().Value, 
                "Parameters.First().Value");

            contentDisposition.Parameters.Remove(dateParameter);
            Assert.IsNull(contentDisposition.CreationDate, "CreationDate");
        }

        [TestMethod]
        public void Dates_InvalidDates_PropertyFails()
        {
            string invalidDateString = "\"Tue, 15 Nov 94 08:12 GMT\"";

            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue dateParameter = new NameValueHeaderValue("read-DATE", invalidDateString);
            contentDisposition.Parameters.Add(dateParameter);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("read-DATE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            Assert.IsNull(contentDisposition.ReadDate);

            contentDisposition.ReadDate = null;
            Assert.IsNull(contentDisposition.ReadDate);
            Assert.AreEqual(0, contentDisposition.Parameters.Count, "Parameters.Count");
        }
        
        [TestMethod]
        public void Size_AddSizeParameterThenUseProperty_ParametersEntryIsOverwritten()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue sizeParameter = new NameValueHeaderValue("SIZE", "279172874239");
            contentDisposition.Parameters.Add(sizeParameter);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("SIZE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.AreEqual(279172874239, contentDisposition.Size);

            contentDisposition.Size = 279172874240;
            Assert.AreEqual(279172874240, contentDisposition.Size, "CreationDate");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("SIZE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(sizeParameter);
            Assert.IsNull(contentDisposition.Size, "Size");
        }

        [TestMethod]
        public void Size_InvalidSizes_PropertyFails()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");

            // Note that uppercase letters are used. Comparison should happen case-insensitive.
            NameValueHeaderValue sizeParameter = new NameValueHeaderValue("SIZE", "-279172874239");
            contentDisposition.Parameters.Add(sizeParameter);
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("SIZE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");
            Assert.IsNull(contentDisposition.Size);

            try
            {
                // Negatives not allowed
                contentDisposition.Size = -279172874240;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }
            Assert.IsNull(contentDisposition.Size, "Size");
            Assert.AreEqual(1, contentDisposition.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("SIZE", contentDisposition.Parameters.First().Name, "Parameters.First().Name");

            contentDisposition.Parameters.Remove(sizeParameter);
            Assert.IsNull(contentDisposition.Size, "Size");
        }
        
        [TestMethod]
        public void ToString_UseDifferentContentDispositions_AllSerializedCorrectly()
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("inline");
            Assert.AreEqual("inline", contentDisposition.ToString());

            contentDisposition.Name = "myname";
            Assert.AreEqual("inline; name=myname", contentDisposition.ToString());

            contentDisposition.FileName = "my File Name";
            Assert.AreEqual("inline; name=myname; filename=\"my File Name\"", contentDisposition.ToString());

            contentDisposition.CreationDate = new DateTimeOffset(new DateTime(2011, 2, 15));
            Assert.AreEqual("inline; name=myname; filename=\"my File Name\"; creation-date="
                + "\"Tue, 15 Feb 2011 08:00:00 GMT\"", contentDisposition.ToString());

            contentDisposition.Parameters.Add(new NameValueHeaderValue("custom", "\"custom value\""));
            Assert.AreEqual("inline; name=myname; filename=\"my File Name\"; creation-date="
                + "\"Tue, 15 Feb 2011 08:00:00 GMT\"; custom=\"custom value\"",  contentDisposition.ToString());

            contentDisposition.Name = null;
            Assert.AreEqual("inline; filename=\"my File Name\"; creation-date="
                + "\"Tue, 15 Feb 2011 08:00:00 GMT\"; custom=\"custom value\"", contentDisposition.ToString());

            contentDisposition.FileNameStar = "File%Name";
            Assert.AreEqual("inline; filename=\"my File Name\"; creation-date="
                + "\"Tue, 15 Feb 2011 08:00:00 GMT\"; custom=\"custom value\"; filename*=utf-8\'\'File%25Name", 
                contentDisposition.ToString());

            contentDisposition.FileName = null;
            Assert.AreEqual("inline; creation-date=\"Tue, 15 Feb 2011 08:00:00 GMT\"; custom=\"custom value\";"
                + " filename*=utf-8\'\'File%25Name", contentDisposition.ToString());

            contentDisposition.CreationDate = null;
            Assert.AreEqual("inline; custom=\"custom value\"; filename*=utf-8\'\'File%25Name", 
                contentDisposition.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseContentDispositionWithAndWithoutParameters_SameOrDifferentHashCodes()
        {
            ContentDispositionHeaderValue contentDisposition1 = new ContentDispositionHeaderValue("inline");
            ContentDispositionHeaderValue contentDisposition2 = new ContentDispositionHeaderValue("inline");
            contentDisposition2.Name = "myname";
            ContentDispositionHeaderValue contentDisposition3 = new ContentDispositionHeaderValue("inline");
            contentDisposition3.Parameters.Add(new NameValueHeaderValue("name", "value"));
            ContentDispositionHeaderValue contentDisposition4 = new ContentDispositionHeaderValue("INLINE");
            ContentDispositionHeaderValue contentDisposition5 = new ContentDispositionHeaderValue("INLINE");
            contentDisposition5.Parameters.Add(new NameValueHeaderValue("NAME", "MYNAME"));

            Assert.AreNotEqual(contentDisposition1.GetHashCode(), contentDisposition2.GetHashCode(), "No params vs. name.");
            Assert.AreNotEqual(contentDisposition1.GetHashCode(), contentDisposition3.GetHashCode(), "No params vs. custom param.");
            Assert.AreNotEqual(contentDisposition2.GetHashCode(), contentDisposition3.GetHashCode(), "name vs. custom param.");
            Assert.AreEqual(contentDisposition1.GetHashCode(), contentDisposition4.GetHashCode(), "Different casing.");
            Assert.AreEqual(contentDisposition2.GetHashCode(), contentDisposition5.GetHashCode(), "Different casing in name.");
        }

        [TestMethod]
        public void Equals_UseContentDispositionWithAndWithoutParameters_EqualOrNotEqualNoExceptions()
        {
            ContentDispositionHeaderValue contentDisposition1 = new ContentDispositionHeaderValue("inline");
            ContentDispositionHeaderValue contentDisposition2 = new ContentDispositionHeaderValue("inline");
            contentDisposition2.Name = "myName";
            ContentDispositionHeaderValue contentDisposition3 = new ContentDispositionHeaderValue("inline");
            contentDisposition3.Parameters.Add(new NameValueHeaderValue("name", "value"));
            ContentDispositionHeaderValue contentDisposition4 = new ContentDispositionHeaderValue("INLINE");
            ContentDispositionHeaderValue contentDisposition5 = new ContentDispositionHeaderValue("INLINE");
            contentDisposition5.Parameters.Add(new NameValueHeaderValue("NAME", "MYNAME"));
            ContentDispositionHeaderValue contentDisposition6 = new ContentDispositionHeaderValue("INLINE");
            contentDisposition6.Parameters.Add(new NameValueHeaderValue("NAME", "MYNAME"));
            contentDisposition6.Parameters.Add(new NameValueHeaderValue("custom", "value"));
            ContentDispositionHeaderValue contentDisposition7 = new ContentDispositionHeaderValue("attachment");

            Assert.IsFalse(contentDisposition1.Equals(contentDisposition2), "No params vs. name.");
            Assert.IsFalse(contentDisposition2.Equals(contentDisposition1), "name vs. no params.");
            Assert.IsFalse(contentDisposition1.Equals(null), "No params vs. <null>.");
            Assert.IsFalse(contentDisposition1.Equals(contentDisposition3), "No params vs. custom param.");
            Assert.IsFalse(contentDisposition2.Equals(contentDisposition3), "name vs. custom param.");
            Assert.IsTrue(contentDisposition1.Equals(contentDisposition4), "Different casing.");
            Assert.IsTrue(contentDisposition2.Equals(contentDisposition5), "Different casing in name.");
            Assert.IsFalse(contentDisposition5.Equals(contentDisposition6), "name vs. custom param.");
            Assert.IsFalse(contentDisposition1.Equals(contentDisposition7), "inline vs. text/other.");
        }

        [TestMethod]
        public void Clone_Call_CloneFieldsMatchSourceFields()
        {
            ContentDispositionHeaderValue source = new ContentDispositionHeaderValue("attachment");
            ContentDispositionHeaderValue clone = (ContentDispositionHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.DispositionType, clone.DispositionType, "DispositionType");
            Assert.AreEqual(0, clone.Parameters.Count, "Parameters.Count");

            source.Name = "myName";
            clone = (ContentDispositionHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.DispositionType, clone.DispositionType, "DispositionType");
            Assert.AreEqual("myName", clone.Name, "Name");
            Assert.AreEqual(1, clone.Parameters.Count, "Parameters.Count");

            source.Parameters.Add(new NameValueHeaderValue("custom", "customValue"));
            clone = (ContentDispositionHeaderValue)((ICloneable)source).Clone();
            Assert.AreEqual(source.DispositionType, clone.DispositionType, "DispositionType");
            Assert.AreEqual("myName", clone.Name, "Name");
            Assert.AreEqual(2, clone.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", clone.Parameters.ElementAt(1).Name, "Custom parameter name.");
            Assert.AreEqual("customValue", clone.Parameters.ElementAt(1).Value, "Custom parameter value.");
        }

        [TestMethod]
        public void GetDispositionTypeLength_DifferentValidScenarios_AllReturnNonZero()
        {
            object result = null;
            ContentDispositionHeaderValue value = null;

            Assert.AreEqual(7, ContentDispositionHeaderValue.GetDispositionTypeLength("inline , other/name", 0,
                out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual(0, value.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(6, ContentDispositionHeaderValue.GetDispositionTypeLength("inline", 0, out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual(0, value.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(19, ContentDispositionHeaderValue.GetDispositionTypeLength("inline; name=MyName", 0,
                out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual("MyName", value.Name, "Name");
            Assert.AreEqual(1, value.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(32, ContentDispositionHeaderValue.GetDispositionTypeLength(" inline; custom=value;name=myName",
                1, out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual("myName", value.Name, "Name");
            Assert.AreEqual(2, value.Parameters.Count, "Parameters.Count");

            Assert.AreEqual(14, ContentDispositionHeaderValue.GetDispositionTypeLength(" inline; custom, next",
                1, out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.IsNull(value.Name, "Name");
            Assert.AreEqual(1, value.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", value.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.IsNull(value.Parameters.ElementAt(0).Value, "Parameters[0].Value");

            Assert.AreEqual(40, ContentDispositionHeaderValue.GetDispositionTypeLength(
                "inline ; custom =\r\n \"x\" ; name = myName , next", 0, out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual("myName", value.Name, "Name");
            Assert.AreEqual(2, value.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", value.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.AreEqual("\"x\"", value.Parameters.ElementAt(0).Value, "Parameters[0].Value");
            Assert.AreEqual("name", value.Parameters.ElementAt(1).Name, "Parameters[1].Name");
            Assert.AreEqual("myName", value.Parameters.ElementAt(1).Value, "Parameters[1].Value");

            Assert.AreEqual(29, ContentDispositionHeaderValue.GetDispositionTypeLength(
                "inline;custom=\"x\";name=myName,next", 0, out result));
            value = (ContentDispositionHeaderValue)result;
            Assert.AreEqual("inline", value.DispositionType, "DispositionType");
            Assert.AreEqual("myName", value.Name, "Name");
            Assert.AreEqual(2, value.Parameters.Count, "Parameters.Count");
            Assert.AreEqual("custom", value.Parameters.ElementAt(0).Name, "Parameters[0].Name");
            Assert.AreEqual("\"x\"", value.Parameters.ElementAt(0).Value, "Parameters[0].Value");
            Assert.AreEqual("name", value.Parameters.ElementAt(1).Name, "Parameters[1].Name");
            Assert.AreEqual("myName", value.Parameters.ElementAt(1).Value, "Parameters[1].Value");
        }

        [TestMethod]
        public void GetDispositionTypeLength_DifferentInvalidScenarios_AllReturnZero()
        {
            object result = null;

            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength(" inline", 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength("inline;", 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength("inline;name=", 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength("inline;name=value;", 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength("inline;", 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength(null, 0, out result));
            Assert.IsNull(result);
            Assert.AreEqual(0, ContentDispositionHeaderValue.GetDispositionTypeLength(string.Empty, 0, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            ContentDispositionHeaderValue expected = new ContentDispositionHeaderValue("inline");
            CheckValidParse("\r\n inline  ", expected);
            CheckValidParse("inline", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // Content-Disposition parser.
            expected.Name = "myName";
            CheckValidParse("\r\n inline  ;  name =   myName ", expected);
            CheckValidParse("  inline;name=myName", expected);

            expected.Name = null;
            expected.DispositionType = "attachment";
            expected.FileName = "foo-ae.html";
            expected.Parameters.Add(new NameValueHeaderValue("filename*", "UTF-8''foo-%c3%a4.html"));
            CheckValidParse(@"attachment; filename*=UTF-8''foo-%c3%a4.html; filename=foo-ae.html", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("");
            CheckInvalidParse("  ");
            CheckInvalidParse(null);
            CheckInvalidParse("inline会");
            CheckInvalidParse("inline ,");
            CheckInvalidParse("inline,");
            CheckInvalidParse("inline; name=myName ,");
            CheckInvalidParse("inline; name=myName,");
            CheckInvalidParse("inline; name=my会Name");
            CheckInvalidParse("inline/");
        }

        [TestMethod]
        public void TryParse_SetOfValidValueStrings_ParsedCorrectly()
        {
            ContentDispositionHeaderValue expected = new ContentDispositionHeaderValue("inline");
            CheckValidTryParse("\r\n inline  ", expected);
            CheckValidTryParse("inline", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // Content-Disposition parser.
            expected.Name = "myName";
            CheckValidTryParse("\r\n inline  ;  name =   myName ", expected);
            CheckValidTryParse("  inline;name=myName", expected);
        }

        [TestMethod]
        public void TryParse_SetOfInvalidValueStrings_ReturnsFalse()
        {
            CheckInvalidTryParse("");
            CheckInvalidTryParse("  ");
            CheckInvalidTryParse(null);
            CheckInvalidTryParse("inline会");
            CheckInvalidTryParse("inline ,");
            CheckInvalidTryParse("inline,");
            CheckInvalidTryParse("inline; name=myName ,");
            CheckInvalidTryParse("inline; name=myName,");
            CheckInvalidTryParse("text/");
        }

        #region Tests from Henrik


        private static Dictionary<string, ContentDispositionValue> ContentDispositionTestCases = new Dictionary<string, ContentDispositionValue>()
        {
            // Valid values
            { "valid1", new ContentDispositionValue(@"inline", @"This should be equivalent to not including the header at all.", true) },
            { "valid2", new ContentDispositionValue(@"inline; filename=""foo.html""", @"'inline', specifying a filename of foo.html", true) },
            { "valid3", new ContentDispositionValue(@"inline; filename=""Not an attachment!""", @"'inline', specifying a filename of Not an attachment! - this checks for proper parsing for disposition types.", true) },
            { "valid4", new ContentDispositionValue(@"inline; filename=""foo.pdf""", @"'inline', specifying a filename of foo.pdf", true) },
            { "valid5", new ContentDispositionValue(@"attachment", @"'attachment' only", true) },
            { "valid6", new ContentDispositionValue(@"ATTACHMENT", @"'ATTACHMENT' only", true) },
            { "valid7", new ContentDispositionValue(@"attachment; filename=""foo.html""", @"'attachment', specifying a filename of foo.html", true) },
            { "valid8", new ContentDispositionValue(@"attachment; filename=""f\oo.html""", @"'attachment', specifying a filename of f\oo.html (the first 'o' being escaped)", true) },
            { "valid9", new ContentDispositionValue(@"attachment; filename=""\""quoting\"" tested.html""", @"'attachment', specifying a filename of \""quoting\"" tested.html (using double quotes around ""quoting"" to test... quoting)", true) },
            { "valid10", new ContentDispositionValue(@"attachment; filename=""Here's a semicolon;.html""", @"'attachment', specifying a filename of Here's a semicolon;.html - this checks for proper parsing for parameters. ", true) },
            { "valid11", new ContentDispositionValue(@"attachment; foo=""bar""; filename=""foo.html""", @"'attachment', specifying a filename of foo.html and an extension parameter ""foo"" which should be ignored (see <a href=""http://greenbytes.de/tech/webdav/rfc2183.html#rfc.section.2.8"">Section 2.8 of RFC 2183</a>.).", true) },
            { "valid12", new ContentDispositionValue(@"attachment; foo=""\""\\"";filename=""foo.html""", @"'attachment', specifying a filename of foo.html and an extension parameter ""foo"" which should be ignored (see <a href=""http://greenbytes.de/tech/webdav/rfc2183.html#rfc.section.2.8"">Section 2.8 of RFC 2183</a>.). The extension parameter actually uses backslash-escapes. This tests whether the UA properly skips the parameter.", true) },
            { "valid13", new ContentDispositionValue(@"attachment; FILENAME=""foo.html""", @"'attachment', specifying a filename of foo.html", true) },
            { "valid14", new ContentDispositionValue(@"attachment; filename=foo.html", @"'attachment', specifying a filename of foo.html using a token instead of a quoted-string.", true) },
            { "valid15", new ContentDispositionValue(@"attachment; filename='foo.bar'", @"'attachment', specifying a filename of 'foo.bar' using single quotes. ", true) },
            { "valid16", new ContentDispositionValue(@"attachment; filename=""foo-ä.html""", @"'attachment', specifying a filename of foo-ä.html, using plain ISO-8859-1", true) },
            { "valid17", new ContentDispositionValue(@"attachment; filename=""foo-&#xc3;&#xa4;.html""", @"'attachment', specifying a filename of foo-&#xc3;&#xa4;.html, which happens to be foo-ä.html using UTF-8 encoding.", true) },
            { "valid18", new ContentDispositionValue(@"attachment; filename=""foo-%41.html""", @"'attachment', specifying a filename of foo-%41.html", true) },
            { "valid19", new ContentDispositionValue(@"attachment; filename=""50%.html""", @"'attachment', specifying a filename of 50%.html", true) },
            { "valid20", new ContentDispositionValue(@"attachment; filename=""foo-%\41.html""", @"'attachment', specifying a filename of foo-%41.html, using an escape character (this tests whether adding an escape character inside a %xx sequence can be used to disable the non-conformant %xx-unescaping).", true) },
            { "valid21", new ContentDispositionValue(@"attachment; name=""foo-%41.html""", @"'attachment', specifying a <i>name</i> parameter of foo-%41.html. (this test was added to observe the behavior of the (unspecified) treatment of ""name"" as synonym for ""filename""; see <a href=""http://www.imc.org/ietf-smtp/mail-archive/msg05023.html"">Ned Freed's summary</a> where this comes from in MIME messages)", true) },
            { "valid22", new ContentDispositionValue(@"attachment; filename=""ä-%41.html""", @"'attachment', specifying a filename parameter of ä-%41.html. (this test was added to observe the behavior when non-ASCII characters and percent-hexdig sequences are combined)", true) },
            { "valid23", new ContentDispositionValue(@"attachment; filename=""foo-%c3%a4-%e2%82%ac.html""", @"'attachment', specifying a filename of foo-%c3%a4-%e2%82%ac.html, using raw percent encoded UTF-8 to represent foo-ä-&#x20ac;.html", true) },
            { "valid24", new ContentDispositionValue(@"attachment; filename =""foo.html""", @"'attachment', specifying a filename of foo.html, with one blank space <em>before</em> the equals character.", true) },
            { "valid25", new ContentDispositionValue(@"attachment; xfilename=foo.html", @"'attachment', specifying an ""xfilename"" parameter.", true) },
            { "valid26", new ContentDispositionValue(@"attachment; filename=""/foo.html""", @"'attachment', specifying an absolute filename in the filesystem root.", true) },
            { "valid27", new ContentDispositionValue(@"attachment; filename=""\\foo.html""", @"'attachment', specifying an absolute filename in the filesystem root.", true) },
            { "valid28", new ContentDispositionValue(@"attachment; creation-date=""Wed, 12 Feb 1997 16:29:51 -0500""", @"'attachment', plus creation-date (see <a href=""http://greenbytes.de/tech/webdav/rfc2183.html#rfc.section.2.4"">Section 2.4 of RFC 2183</a>)", true) },
            { "valid29", new ContentDispositionValue(@"attachment; modification-date=""Wed, 12 Feb 1997 16:29:51 -0500""", @"'attachment', plus modification-date (see <a href=""http://greenbytes.de/tech/webdav/rfc2183.html#rfc.section.2.5"">Section 2.5 of RFC 2183</a>)", true) },
            { "valid30", new ContentDispositionValue(@"foobar", @"This should be equivalent to using ""attachment"".", true) },
            { "valid31", new ContentDispositionValue(@"attachment; example=""filename=example.txt""", @"'attachment', with no filename parameter", true) },
            { "valid32", new ContentDispositionValue(@"attachment; filename*=iso-8859-1''foo-%E4.html", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded ISO-8859-1", true) },
            { "valid33", new ContentDispositionValue(@"attachment; filename*=UTF-8''foo-%c3%a4-%e2%82%ac.html", @"'attachment', specifying a filename of foo-ä-&#x20ac;.html, using RFC2231 encoded UTF-8", true) },
            { "valid34", new ContentDispositionValue(@"attachment; filename*=''foo-%c3%a4-%e2%82%ac.html", @"Behavior is undefined in RFC 2231, the charset part is missing, although UTF-8 was used.", true) },
            { "valid35", new ContentDispositionValue(@"attachment; filename*=UTF-8''foo-a%cc%88.html", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded UTF-8, but choosing the decomposed form (lowercase a plus COMBINING DIAERESIS) -- on a Windows target system, this should be translated to the preferred Unicode normal form (composed).", true) },
            { "valid36", new ContentDispositionValue(@"attachment; filename*= UTF-8''foo-%c3%a4.html", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded UTF-8, with whitespace after ""*=""", true) },
            { "valid37", new ContentDispositionValue(@"attachment; filename* =UTF-8''foo-%c3%a4.html", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded UTF-8, with whitespace inside ""* =""", true) },
            { "valid38", new ContentDispositionValue(@"attachment; filename*=UTF-8''A-%2541.html", @"'attachment', specifying a filename of A-%41.html, using RFC2231 encoded UTF-8.", true) },
            { "valid39", new ContentDispositionValue(@"attachment; filename*=UTF-8''%5cfoo.html", @"'attachment', specifying a filename of /foo.html, using RFC2231 encoded UTF-8.", true) },
            { "valid40", new ContentDispositionValue(@"attachment; filename*0=""foo.""; filename*1=""html""", @"'attachment', specifying a filename of foo.html, using RFC2231-style parameter continuations.", true) },
            { "valid41", new ContentDispositionValue(@"attachment; filename*0*=UTF-8''foo-%c3%a4; filename*1="".html""", @"'attachment', specifying a filename of foo-ä.html, using both RFC2231-style parameter continuations and UTF-8 encoding.", true) },
            { "valid42", new ContentDispositionValue(@"attachment; filename*0=""foo""; filename*01=""bar""", @"'attachment', specifying a filename of foo (the parameter filename*01 should be ignored because of the leading zero)", true) },
            { "valid43", new ContentDispositionValue(@"attachment; filename*0=""foo""; filename*2=""bar""", @"'attachment', specifying a filename of foo (the parameter filename*2 should be ignored because there's no filename*1 parameter)", true) },
            { "valid44", new ContentDispositionValue(@"attachment; filename*1=""foo.""; filename*2=""html""", @"'attachment' (the filename* parameters should be ignored because filename*0 is missing)", true) },
            { "valid45", new ContentDispositionValue(@"attachment; filename*1=""bar""; filename*0=""foo""", "'attachment', specifying a filename of foobar", true) },
            { "valid46", new ContentDispositionValue(@"attachment; filename=""foo-ae.html""; filename*=UTF-8''foo-%c3%a4.html", @"'attachment', specifying a filename of foo-ae.html in the traditional format, and foo-ä.html in RFC2231 format.", true) },
            { "valid47", new ContentDispositionValue(@"attachment; filename*=UTF-8''foo-%c3%a4.html; filename=""foo-ae.html""", @"'attachment', specifying a filename of foo-ae.html in the traditional format, and foo-ä.html in RFC2231 format.", true) },
            { "valid48", new ContentDispositionValue(@"attachment; foobar=x; filename=""foo.html""", @"'attachment', specifying a new parameter ""foobar"", plus a filename of foo.html in the traditional format.", true) },
            { "valid49", new ContentDispositionValue(@"attachment; filename=""=?ISO-8859-1?Q?foo-=E4.html?=""", @"attachment; filename=""=?ISO-8859-1?Q?foo-=E4.html?=""", true) },
            { "valid50", new ContentDispositionValue(@"attachment; filename=""=?utf-8?B?Zm9vLeQuaHRtbA==?=""", @"attachment; filename=""=?utf-8?B?Zm9vLeQuaHRtbA==?=""", true) },

            // Invalid values
            { "invalid1", new ContentDispositionValue(@"""inline""", @"'inline' only, using double quotes", false) },
            { "invalid2", new ContentDispositionValue(@"""attachment""", @"'attachment' only, using double quotes", false) },
            { "invalid3", new ContentDispositionValue(@"attachment; filename=foo.html ;", @"'attachment', specifying a filename of foo.html using a token instead of a quoted-string, and adding a trailing semicolon.", false) },
            { "invalid4", new ContentDispositionValue(@"attachment; filename=foo bar.html", @"'attachment', specifying a filename of foo bar.html without using quoting.", false) },
            // Duplicate file name paramiter
            // { "invalid5", new ContentDispositionValue(@"attachment; filename=""foo.html""; filename=""bar.html""", @"'attachment', specifying two filename parameters. This is invalid syntax.", false) },
            { "invalid6", new ContentDispositionValue(@"attachment; filename=foo[1](2).html", @"'attachment', specifying a filename of foo[1](2).html, but missing the quotes. Also, ""["", ""]"", ""("" and "")"" are not allowed in the HTTP <a href=""http://greenbytes.de/tech/webdav/draft-ietf-httpbis-p1-messaging-latest.html#rfc.section.1.2.2"">token</a> production.", false) },
            { "invalid7", new ContentDispositionValue(@"attachment; filename=foo-ä.html", @"'attachment', specifying a filename of foo-ä.html, but missing the quotes.", false) },
            // HTML escaping, not supported
            // { "invalid8", new ContentDispositionValue(@"attachment; filename=foo-&#xc3;&#xa4;.html", "'attachment', specifying a filename of foo-&#xc3;&#xa4;.html (which happens to be foo-ä.html using UTF-8 encoding) but missing the quotes.", false) },
            { "invalid9", new ContentDispositionValue(@"filename=foo.html", @"Disposition type missing, filename specified.", false) },
            { "invalid10", new ContentDispositionValue(@"x=y; filename=foo.html", @"Disposition type missing, filename specified after extension parameter.", false) },
            { "invalid11", new ContentDispositionValue(@"""foo; filename=bar;baz""; filename=qux", @"Disposition type missing, filename ""qux"". Can it be more broken? (Probably)", false) },
            { "invalid12", new ContentDispositionValue(@"filename=foo.html, filename=bar.html", @"Disposition type missing, two filenames specified separated by a comma (this is syntactically equivalent to have two instances of the header with one filename parameter each).", false) },
            { "invalid13", new ContentDispositionValue(@"; filename=foo.html", @"Disposition type missing (but delimiter present), filename specified.", false) },
            // This is permitted as a paramiter without a value
            // { "invalid14", new ContentDispositionValue(@"inline; attachment; filename=foo.html", @"Both disposition types specified.", false) },
            // This is permitted as a paramiter without a value
            // { "invalid15", new ContentDispositionValue(@"inline; attachment; filename=foo.html", @"Both disposition types specified.", false) },
            { "invalid16", new ContentDispositionValue(@"attachment; filename=""foo.html"".txt", @"'attachment', specifying a filename parameter that is broken (quoted-string followed by more characters). This is invalid syntax. ", false) },
            { "invalid17", new ContentDispositionValue(@"attachment; filename=""bar", @"'attachment', specifying a filename parameter that is broken (missing ending double quote). This is invalid syntax.", false) },
            { "invalid18", new ContentDispositionValue(@"attachment; filename=foo""bar;baz""qux", @"'attachment', specifying a filename parameter that is broken (disallowed characters in token syntax). This is invalid syntax.", false) },
            { "invalid19", new ContentDispositionValue(@"attachment; filename=foo.html, attachment; filename=bar.html", @"'attachment', two comma-separated instances of the header field. As Content-Disposition doesn't use a list-style syntax, this is invalid syntax and, according to <a href=""http://greenbytes.de/tech/webdav/rfc2616.html#rfc.section.4.2.p.5"">RFC 2616, Section 4.2</a>, roughly equivalent to having two separate header field instances.", false) },
            { "invalid20", new ContentDispositionValue(@"filename=foo.html; attachment", @"filename parameter and disposion type reversed.", false) },
            // Escaping is not verified
            // { "invalid21", new ContentDispositionValue(@"attachment; filename*=iso-8859-1''foo-%c3%a4-%e2%82%ac.html", @"'attachment', specifying a filename of foo-ä-&#x20ac;.html, using RFC2231 encoded UTF-8, but declaring ISO-8859-1", false) },
            // Escaping is not verified
            // { "invalid22", new ContentDispositionValue(@"attachment; filename *=UTF-8''foo-%c3%a4.html", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded UTF-8, with whitespace before ""*=""", false) },
            // Escaping is not verified
            // { "invalid23", new ContentDispositionValue(@"attachment; filename*=""UTF-8''foo-%c3%a4.html""", @"'attachment', specifying a filename of foo-ä.html, using RFC2231 encoded UTF-8, with double quotes around the parameter value.", false) },
            { "invalid24", new ContentDispositionValue(@"attachment; filename==?ISO-8859-1?Q?foo-=E4.html?=", @"Uses RFC 2047 style encoded word. ""="" is invalid inside the token production, so this is invalid.", false) },
            { "invalid25", new ContentDispositionValue(@"attachment; filename==?utf-8?B?Zm9vLeQuaHRtbA==?=", @"Uses RFC 2047 style encoded word. ""="" is invalid inside the token production, so this is invalid.", false) },
        };

        #region Parsing

        [TestMethod]
        public void ContentDispositionHeaderValue_Parse_ExpectedResult()
        {
            foreach (var cd in ContentDispositionTestCases.Values)
            {
                ContentDispositionHeaderValue header = Parse(cd);
            }
        }

        [TestMethod]
        public void ContentDispositionHeaderValue_TryParse_ExpectedResult()
        {
            foreach (var cd in ContentDispositionTestCases.Values)
            {
                ContentDispositionHeaderValue header = TryParse(cd);
            }
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid1_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid1"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "inline", null);
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid2_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid2"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "inline", @"""foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid3_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid3"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "inline", @"""Not an attachment!""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid4_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid4"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "inline", @"""foo.pdf""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid5_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid5"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid6_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid6"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "ATTACHMENT", null);
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid7_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid7"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid8_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid8"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""f\oo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid9_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid9"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""\""quoting\"" tested.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid10_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid10"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""Here's a semicolon;.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid11_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid11"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
            ValidateExtensionParameter(header, "foo", @"""bar""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid12_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid12"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
            ValidateExtensionParameter(header, "foo", @"""\""\\""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid13_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid13"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid14_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid14"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"foo.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid15_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid15"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"'foo.bar'");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid16_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid16"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-ä.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid17_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid17"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-&#xc3;&#xa4;.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid18_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid18"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-%41.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid19_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid19"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""50%.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid20_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid20"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-%\41.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid21_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid21"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "name", @"""foo-%41.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid22_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid22"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""ä-%41.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid23_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid23"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-%c3%a4-%e2%82%ac.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid24_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid24"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid25_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid25"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "xfilename", @"foo.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid26_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid26"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""/foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid27_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid27"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""\\foo.html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid28_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid28"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "creation-date", @"""Wed, 12 Feb 1997 16:29:51 -0500""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid29_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid29"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "modification-date", @"""Wed, 12 Feb 1997 16:29:51 -0500""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid30_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid30"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "foobar", null);
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid31_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid31"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "example", @"""filename=example.txt""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid32_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid32"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"iso-8859-1''foo-%E4.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid33_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid33"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-%c3%a4-%e2%82%ac.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid34_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid34"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"''foo-%c3%a4-%e2%82%ac.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid35_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid35"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-a%cc%88.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid36_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid36"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-%c3%a4.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid37_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid37"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-%c3%a4.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid38_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid38"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''A-%2541.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid39_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid39"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*", @"UTF-8''%5cfoo.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid40_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid40"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*0", @"""foo.""");
            ValidateExtensionParameter(header, "filename*1", @"""html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid41_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid41"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*0*", @"UTF-8''foo-%c3%a4");
            ValidateExtensionParameter(header, "filename*1", @""".html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid42_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid42"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*0", @"""foo""");
            ValidateExtensionParameter(header, "filename*01", @"""bar""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid43_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid43"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*0", @"""foo""");
            ValidateExtensionParameter(header, "filename*2", @"""bar""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid44_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid44"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*1", @"""foo.""");
            ValidateExtensionParameter(header, "filename*2", @"""html""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid45_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid45"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", null);
            ValidateExtensionParameter(header, "filename*0", @"""foo""");
            ValidateExtensionParameter(header, "filename*1", @"""bar""");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid46_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid46"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-ae.html""");
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-%c3%a4.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid47_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid47"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo-ae.html""");
            ValidateExtensionParameter(header, "filename*", @"UTF-8''foo-%c3%a4.html");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid48_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid48"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""foo.html""");
            ValidateExtensionParameter(header, "foobar", @"x");
        }

        [TestMethod]
        public void ContentDispositionHeader_Valid49_Success()
        {
            ContentDispositionValue cd = ContentDispositionTestCases["valid49"];
            ContentDispositionHeaderValue header = TryParse(cd);
            ValidateHeaderValues(header, "attachment", @"""=?ISO-8859-1?Q?foo-=E4.html?=""");
        }

        #endregion

        private static void ValidateHeaderValues(ContentDispositionHeaderValue header, string expectedDispositionType, string expectedFilename)
        {
            Assert.IsNotNull(header);
            Assert.AreEqual(expectedDispositionType, header.DispositionType,
                string.Format("ContentDispositionHeaderValue DipositionType '{0}' did not matched expected value '{1}'",
                header.DispositionType, expectedDispositionType));
            Assert.AreEqual(expectedFilename, header.FileName,
                string.Format("ContentDispositionHeaderValue Filename '{0}' did not matched expected value '{1}'",
                header.FileName, expectedFilename));
        }

        private static void ValidateExtensionParameter(ContentDispositionHeaderValue header, string name, string expectedValue)
        {
            Assert.IsNotNull(header);
            NameValueHeaderValue parameter = FindParameter(header.Parameters, name);
            Assert.IsNotNull(parameter, string.Format("Did not find extension parameter '{0}'", name));
            Assert.AreEqual(expectedValue, parameter.Value, string.Format("The value '{0}' of extension parameter '{1}' did not match expected calue '{2}'",
                parameter.Value, name, expectedValue));
        }

        private static NameValueHeaderValue FindParameter(ICollection<NameValueHeaderValue> values, string name)
        {
            if ((values == null) || (values.Count == 0))
            {
                return null;
            }

            foreach (var value in values)
            {
                if (string.Compare(value.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return value;
                }
            }
            return null;
        }
        
        private static ContentDispositionHeaderValue Parse(ContentDispositionValue cd)
        {
            Assert.IsNotNull(cd);
            ContentDispositionHeaderValue header = null;
            if (cd.Valid)
            {
                header = ContentDispositionHeaderValue.Parse(cd.Value);
                Assert.IsNotNull(header);
            }
            else
            {
                try
                {
                    header = ContentDispositionHeaderValue.Parse(cd.Value);
                    Assert.Fail(string.Format("ContentDispositionHeaderValue failed to detect INVALID value '{0}': {1}", cd.Value, cd.Description));
                }
                catch (FormatException) { }
            }

            return header;
        }

        private static ContentDispositionHeaderValue TryParse(ContentDispositionValue cd)
        {
            Assert.IsNotNull(cd);
            ContentDispositionHeaderValue header;
            if (cd.Valid)
            {
                Assert.IsTrue(
                    ContentDispositionHeaderValue.TryParse(cd.Value, out header),
                    string.Format("ContentDispositionHeaderValue failed to parse VALID value '{0}': {1}", cd.Value, cd.Description));
                Assert.IsNotNull(header);
            }
            else
            {
                Assert.IsFalse(ContentDispositionHeaderValue.TryParse(cd.Value, out header),
                    string.Format("ContentDispositionHeaderValue failed to detect INVALID value '{0}': {1}", cd.Value, cd.Description));
                Assert.IsNull(header);
            }

            return header;
        }

        public class ContentDispositionValue
        {
            public ContentDispositionValue(string value, string description, bool valid)
            {
                this.Value = value;
                this.Description = description;
                this.Valid = valid;
            }

            public string Value { get; private set; }

            public string Description { get; private set; }

            public bool Valid { get; private set; }
        }

        #endregion Tests from Henrik

        #region Helper methods

        private void CheckValidParse(string input, ContentDispositionHeaderValue expectedResult)
        {
            ContentDispositionHeaderValue result = ContentDispositionHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidParse(string input)
        {
            ExceptionAssert.Throws<FormatException>(() => ContentDispositionHeaderValue.Parse(input), "Parse");
        }

        private void CheckValidTryParse(string input, ContentDispositionHeaderValue expectedResult)
        {
            ContentDispositionHeaderValue result = null;
            Assert.IsTrue(ContentDispositionHeaderValue.TryParse(input, out result), input);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckInvalidTryParse(string input)
        {
            ContentDispositionHeaderValue result = null;
            Assert.IsFalse(ContentDispositionHeaderValue.TryParse(input, out result), input);
            Assert.IsNull(result);
        }
        
        private static void AssertFormatException(string contentDisposition)
        {
            ExceptionAssert.ThrowsFormat(() => { new ContentDispositionHeaderValue(contentDisposition); },
                "name: '" + (contentDisposition == null ? "<null>" : contentDisposition) + "'");
        }

        #endregion
    }
}
