// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Linq;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress), UnitTestType(typeof(HttpParameterExtensionMethods))]
    public class HttpParameterExtensionMethodsTests : UnitTest
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpParameterExtensionMethods is a class that is public, abstract, sealed (static).")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsStatic);
        }

        #endregion Type

        #region Methods

        #region HttpParameter

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("ToHttpParameter creates an HttpParameter from from MessagePartDescription.")]
        public void ToHttpParameterCreatesHttpParameter()
        {
            OperationDescription od = GetOperationDescription(typeof(SimpleOperationsService), "OneInputAndReturnValue");
            MessagePartDescription mpd = od.Messages[1].Body.ReturnValue;
            HttpParameter hpd = mpd.ToHttpParameter();

            Assert.AreEqual("OneInputAndReturnValueResult", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(string), hpd.ParameterType, "ParameterType was not set correctly");
            Assert.AreSame(mpd, hpd.MessagePartDescription, "Internal messagePartDescription should be what we passed to ctor");
        }

        [TestMethod]
        [TestCategory("CIT")]
        [Timeout(TimeoutConstant.DefaultTimeout)]
        [Owner("maying")]
        [Description("ToHttpParameter throws with a null MessagePartDescription.")]
        public void ToHttpParameterThrowsWithNullMessagePartDescription()
        {
            MessagePartDescription mpd = null;
            Asserters.Exception.ThrowsArgumentNull(
                "description",
                () => mpd.ToHttpParameter()
            );
        }

        #endregion HttpParameter

        #endregion Methods

        #region Test helpers

        public static OperationDescription GetOperationDescription(Type contractType, string methodName)
        {
            ContractDescription cd = ContractDescription.GetContract(contractType);
            OperationDescription od = cd.Operations.FirstOrDefault(o => o.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(od, "Failed to get operation description for " + methodName);
            return od;
        }

        #endregion Test helpers

    }
}
