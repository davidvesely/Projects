// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System.IO;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(FormattedHttpHeaderTextWriter))]
    public class FormattedHttpHeaderTextWriterTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("ketian, zhedai")]
        [Description("FormattedHttpHeaderTextWriter is internal, disposable and concrete.")]
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
        [Description("WriteAllHeaders() output formatted headers")]
        public void WriteAllHeadersOutputFormattedHeaders()
        {
            string text = StringResources.headers_full_unformatted_txt.Replace("\r\n", "\n");
            using (TolerantHttpHeaderTextReader reader = new TolerantHttpHeaderTextReader(text, -1))
            {
                while (reader.Read()) ;
                Assert.IsNull(reader.Exception);
                StringBuilder sb = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(sb))
                {
                    using (FormattedHttpHeaderTextWriter writer = reader.CreateWriter(stringWriter))
                    {
                        writer.WriteAllHeaders();
                    }
                }

                Assert.AreEqual(StringResources.headers_full_txt, sb.ToString());
            }
        }

        #endregion Methods
    }
}