// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal static class TestClientTestHelper
    {
        internal static void TestTolerantTextReader(TextAndExpectedOptions textAndExpectedOptions, ITolerantTextReader reader)
        {
            if (textAndExpectedOptions.ExpectedOptions == null)
            {
                textAndExpectedOptions.ExpectedOptions = textAndExpectedOptions.ExpectedOptionsGenerator();
            }

            while (reader.Read()) ;

            IEnumerable<string> expectedItems = new List<string>(reader.GetExpectedItems());
            Assert.AreEqual(textAndExpectedOptions.ExpectedOptions.Count<string>(), expectedItems.Count<string>());
            for (int i = 0; i < textAndExpectedOptions.ExpectedOptions.Count<string>(); ++i)
            {
                Assert.AreEqual(textAndExpectedOptions.ExpectedOptions.ElementAt<string>(i), expectedItems.ElementAt<string>(i));
            }
        }

        internal class TextAndExpectedOptions
        {
            public string Text { get; set; }
            public IEnumerable<string> ExpectedOptions { get; set; }
            public Func<IEnumerable<string>> ExpectedOptionsGenerator { get; set; }
        }
    }
}