// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using Microsoft.ApplicationServer.Http.Configuration;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(TestCommon.UnitTestLevel.Complete)]
    public class HttpMemoryBindingCollectionElementTests : UnitTest<HttpMemoryBindingCollectionElement>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBindingCollectionElement is public, concrete.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner(TestOwner.HttpMemoryChannel)]
        [Description("HttpMemoryBindingCollectionElement() default ctor.")]
        public void DefaultCtor()
        {
            HttpMemoryBindingCollectionElement element = new HttpMemoryBindingCollectionElement();
            Assert.IsNotNull(element, "instance should not be null");
        }

        #endregion

        #region Members

        #endregion
    }
}