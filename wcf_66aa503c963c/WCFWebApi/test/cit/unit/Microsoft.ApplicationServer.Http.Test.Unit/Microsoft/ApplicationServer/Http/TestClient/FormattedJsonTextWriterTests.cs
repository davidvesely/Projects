// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(FormattedJsonTextWriter))]
    public class FormattedJsonTextWriterTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("FormattedJsonTextWriter is internal, disposable and concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsDisposable);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods
        
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("WriteDocument() output formatted json")]
        public void WriteDocumentOutputFormattedJson()
        {
            string text = StringResources.contosoBooks_json.Replace("\r\n", "\n");
            using (TolerantJsonTextReader reader = new TolerantJsonTextReader(text))
            {
                while (reader.Read()) ;
                Assert.IsNull(reader.Exception);
                StringBuilder sb = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(sb))
                {
                    using (FormattedJsonTextWriter writer = reader.CreateWriter(stringWriter))
                    {
                        writer.WriteDocument();
                    }
                }

                Assert.AreEqual(StringResources.contosoBooks_json, sb.ToString());
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("WriteDocument() output formatted json with an empty array")]
        public void WriteDocumentOutputFormattedJsonCsdmain232427()
        {
            string text = "{\"Authors\":[],\"Category\":0,\"Comments\":[],\"Id\":0,\"IsAvailable\":false,\"Name\":null,\"PublishTime\":\"\\/Date(-62135568000000-0800)\\/\",\"Weight\":0}";
            string expectedFormattedText = "{\r\n  \"Authors\":\r\n  [\r\n  ],\r\n  \"Category\":0,\r\n  \"Comments\":\r\n  [\r\n  ],\r\n  \"Id\":0,\r\n  \"IsAvailable\":false,\r\n  \"Name\":null,\r\n  \"PublishTime\":\"\\/Date(-62135568000000-0800)\\/\",\r\n  \"Weight\":0\r\n}";

            using (TolerantJsonTextReader reader = new TolerantJsonTextReader(text))
            {
                while (reader.Read()) ;
                Assert.IsNull(reader.Exception);
                StringBuilder sb = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(sb))
                {
                    using (FormattedJsonTextWriter writer = reader.CreateWriter(stringWriter))
                    {
                        writer.WriteDocument();
                    }
                }

                Assert.AreEqual(expectedFormattedText, sb.ToString());
            }
        }

        #endregion Methods
    }
}