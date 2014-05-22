// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonTypeTest
    {
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void JsonTypeValues()
        {
            string[] allJsonTypeExpectedValues = new string[] { "Array", "Boolean", "Default", "Number", "Object", "String" };
            JsonType[] allJsonTypeActualValues = (JsonType[])Enum.GetValues(typeof(JsonType));

            Assert.AreEqual(allJsonTypeExpectedValues.Length, allJsonTypeActualValues.Length);

            List<string> allJsonTypeActualStringValues = new List<string>(allJsonTypeActualValues.Select((x) => x.ToString()));
            allJsonTypeActualStringValues.Sort(StringComparer.Ordinal);

            for (int i = 0; i < allJsonTypeExpectedValues.Length; i++)
            {
                Assert.AreEqual(allJsonTypeExpectedValues[i], allJsonTypeActualStringValues[i]);
            }
        }
    }
}
