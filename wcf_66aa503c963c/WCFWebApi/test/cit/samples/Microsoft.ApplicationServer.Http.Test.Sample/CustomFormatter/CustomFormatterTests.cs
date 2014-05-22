// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using System.Net.Http;
    using CustomFormatter.Sample;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CustomFormatterTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinela")]
        [Description("Verify that CustomFormatter sample runs without exceptions.")]
        public void CustomFormatter_Basic()
        {
            HttpServiceHost host = null;
            try
            {
                host = Program.ConfigureAndOpenHost();

                HttpResponseMessage response = Program.SendRequest("CustomersWithCustomContent", 7);

                // we are expecting the "application/foo" in the response, and the response is actually processed by the MyMediaTypeFormatter user added
                Assert.AreEqual("application/foo", response.Content.Headers.ContentType.MediaType);
                Program.RemoveCustomer("CustomersWithCustomContent", 7);

                response = Program.SendRequest("CustomersWithDynamicCustomContent", 8);
                Assert.AreEqual("application/bar", response.Content.Headers.ContentType.MediaType);

                Program.RemoveCustomer("CustomersWithDynamicCustomContent", 8);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinela")]
        [Description("Verify that entire CustomFormatter sample runs without exceptions.")]
        public void CustomFormatter_TestAll()
        {
            Program.Main();
        }
    }
}
