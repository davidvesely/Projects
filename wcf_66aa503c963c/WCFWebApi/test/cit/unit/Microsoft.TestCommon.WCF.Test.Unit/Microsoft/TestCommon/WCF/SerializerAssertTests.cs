// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Reflection;

    [TestClass]
    public class SerializerAssertTests : UnitTest<SerializerAssert>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("SerializerAssert type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                typeof(SerializerAssert),
                TypeAssert.TypeProperties.IsPublic | TypeAssert.TypeProperties.IsClass | TypeAssert.TypeProperties.IsVisible);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
