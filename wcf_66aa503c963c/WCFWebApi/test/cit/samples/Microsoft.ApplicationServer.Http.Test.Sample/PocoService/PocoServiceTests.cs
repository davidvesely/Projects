// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test.Sample
{
    using System.Net.Http;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PocoServiceSample = global::PocoService.Sample;

    [TestClass]
    public class PocoServiceTests
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinela")]
        [Description("Verify that PocoService returns the correct item.")]
        public void PocoService_GetItem()
        {
            HttpServiceHost host = null;
            try
            {
                host = PocoServiceSample.Program.ConfigureAndOpenHost();

                HttpResponseMessage response = PocoServiceSample.Program.SendRequest("Item/SomeName");

                PocoServiceSample.PocoServiceItem item = response.Content.ReadAsAsync<PocoServiceSample.PocoServiceItem>().Result;
                Assert.AreEqual("SomeName", item.Name, "PocoServiceItem should have had this name.");
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
        [Description("Verify that entire PocoService sample runs without exceptions.")]
        public void PocoService_TestAll()
        {
            PocoServiceSample.Program.Main();
        }
    }
}
