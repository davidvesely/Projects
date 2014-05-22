// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting.OData;
    using System.Net.Http.Formatting.OData.Test;
    using System.Net.Http.Formatting.OData.Test.EntityTypes;
    using System.Net.Http.Headers;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class ODataMediaTypeFormatterTests : UnitTest<ODataMediaTypeFormatter>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass, typeof(MediaTypeFormatter));
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ODataMediaTypeFormatter() constructor sets standard Atom+xml/Json media types in SupportedMediaTypes.")]
        public void Constructor()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();
            foreach (MediaTypeHeaderValue mediaType in DataSets.Http.StandardODataMediaTypes)
            {
                Assert.IsTrue(formatter.SupportedMediaTypes.Contains(mediaType), string.Format("SupportedMediaTypes should have included {0}.", mediaType.ToString()));
            }
        }

        #endregion Constructors

        #region Properties

        #region DefaultMediaType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("DefaultMediaType property returns application/atom+xml.")]
        public void DefaultMediaTypeReturnsApplicationAtomXml()
        {
            MediaTypeHeaderValue mediaType = ODataMediaTypeFormatter.DefaultMediaType;
            Assert.IsNotNull(mediaType, "DefaultMediaType cannot be null.");
            Assert.AreEqual("application/atom+xml", mediaType.MediaType);
        }

        #endregion DefaultMediaType

        #endregion Properties

        #region Methods

        #region SetKeyMembers()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SetKeyMembers() throws for null 'type' parameter.")]
        public void SetKeyPropertiesThrowsForNullTypeParameter()
        {
            Asserters.Exception.ThrowsArgumentNull("type", () => ODataMediaTypeFormatter.SetKeyMembers(null, "key1", "key2"));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SetKeyMembers() throws for null 'memberNames' parameter.")]
        public void SetKeyPropertiesThrowsForNullMemberNamesParameter()
        {
            Asserters.Exception.ThrowsArgumentNull("memberNames", () => ODataMediaTypeFormatter.SetKeyMembers(typeof(DataContractType), (string[])null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SetKeyMembers() throws for types that are not class DataContracts; this includes primitives, enums, collections, etc.")]
        public void SetKeyPropertiesThrowsForNonClassDataContracts()
        {
            Asserters.Data.Execute(
                DataSets.Common.RepresentativeValueAndRefTypeTestDataCollection,
                (type, instance) =>
                {
                    Asserters.Exception.Throws<InvalidOperationException>(
                        OData.SR.TypeCannotHaveKeyMembers(type.Name),
                        () => ODataMediaTypeFormatter.SetKeyMembers(type, "Count"));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SetKeyMembers() throws if the type does not have a dataMember with the given name (case sensitive).")]
        public void SetKeyPropertiesThrowsForMissingOrNonPublicProperties()
        {
            string[] invalidMemberNames = new string[] { "Unknown", "id" /* wrong casing */ };
            foreach (string memberName in invalidMemberNames)
            {
                Asserters.Exception.Throws<InvalidOperationException>(
                    OData.SR.TypeDoesNotHaveMember(typeof(DataContractType).Name, memberName),
                    () => ODataMediaTypeFormatter.SetKeyMembers(typeof(DataContractType), memberName));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("SetKeyMembers() accepts one or more valid data member names (case sensitive).")]
        public void SetKeyPropertiesAcceptiesValidDataMembers()
        {
            string[][] invalidDataMemberNames = new string[][] { new string[] { "Id" }, new string[] { "Id", "Name" } };
            foreach (string[] propertyNames in invalidDataMemberNames)
            {
                ODataMediaTypeFormatter.SetKeyMembers(typeof(DataContractType), propertyNames);
            }
        }

        #endregion SetKeyMembers()

        #region WriteToStream()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("WriteToStream() returns the expected OData representation for the type.")]
        public void WriteToStreamReturnsODataRepresentation()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();

            ObjectContent<WorkItem> content = new ObjectContent<WorkItem>((WorkItem)TypeInitializer.GetInstance(SupportedTypes.WorkItem));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            RegexReplacement replaceUpdateTime = new RegexReplacement("<updated>*.*</updated>", "<updated>UpdatedTime</updated>");
            Asserters.String.AreEqual(BaselineResource.TestEntityWorkItem, content.ReadAsStringAsync().Result, true, replaceUpdateTime);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("WriteToStream() returns the expected OData representation for the type with explicitly set key members.")]
        public void WriteToStreamReturnsODataRepresentationWithExplicitlySetKeys()
        {
            ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter();
            ODataMediaTypeFormatter.SetKeyMembers(typeof(DerivedWorkItem), "EmployeeID", "WorkItemID");
            DerivedWorkItem workItem = new DerivedWorkItem() { EmployeeID = 0, IsCompleted = false, NumberOfHours = 100, WorkItemID = 25 };

            ObjectContent<DerivedWorkItem> content = new ObjectContent<DerivedWorkItem>(workItem);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            content.Formatters.Add(formatter);
            content.DefaultFormatter = formatter;

            RegexReplacement replaceUpdateTime = new RegexReplacement("<updated>*.*</updated>", "<updated>UpdatedTime</updated>");
            Asserters.String.AreEqual(BaselineResource.TestEntityWorkItemWithExplictKeys , content.ReadAsStringAsync().Result, true, replaceUpdateTime);
        }

        #endregion WriteToStream()

        #endregion Methods
    }
}
