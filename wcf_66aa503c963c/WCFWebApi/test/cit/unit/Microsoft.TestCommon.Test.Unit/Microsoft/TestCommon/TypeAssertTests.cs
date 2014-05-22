// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeAssertTests : UnitTest<TypeAssert>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Owner("roncain"), Timeout(TimeoutConstant.DefaultTimeout)]
        [Description("TypeAssert type is a public, concrete and not sealed class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
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
