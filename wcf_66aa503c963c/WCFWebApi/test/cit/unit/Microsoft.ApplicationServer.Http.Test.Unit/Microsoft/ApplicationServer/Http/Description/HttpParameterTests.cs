// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel.Description;
    using Microsoft.ApplicationServer.Common.Test.Services;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpParameterTests : UnitTest<HttpParameter>
    {
        private static readonly string isAssignableFromParameterOfTMethodName = "IsAssignableFromParameter";

        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpParameter is public, concrete and not sealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass);
        }

        #endregion Type

        #region Constructors

        #region HttpParameter(string, Type)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpParameter(string, Type) sets Name and Type.")]
        public void Constructor()
        {
            HttpParameter hpd = new HttpParameter("aName", typeof(int));
            Assert.AreEqual("aName", hpd.Name, "Name was not set.");
            Assert.AreEqual(typeof(int), hpd.ParameterType, "ParameterType was not set.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpParameter(string, Type) throws for empty name.")]
        public void ConstructorThrowsWithEmptyName()
        {
            foreach (string name in TestData.EmptyStrings)
            {
                Asserters.Exception.ThrowsArgumentNull("name", () => new HttpParameter(name, typeof(int)));
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpParameter(string, Type) throws for empty name.")]
        public void ConstructorThrowsWithNullType()
        {
            Asserters.Exception.ThrowsArgumentNull("type", () => new HttpParameter("aName", null));
        }

        #endregion HttpParameter(string, Type)

        #region HttpParameter(MessagePartDescription)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("HttpParameter(MessagePartDescription) (internal constructor) sets Name, Type and (internal) MessagePartDescription.")]
        public void Constructor1()
        {
            OperationDescription od = GetOperationDescription(typeof(SimpleOperationsService), "OneInputAndReturnValue");
            MessagePartDescription mpd = od.Messages[1].Body.ReturnValue;
            HttpParameter hpd = new HttpParameter(mpd);

            Assert.AreEqual("OneInputAndReturnValueResult", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(string), hpd.ParameterType, "ParameterType was not set correctly");
            Assert.AreSame(mpd, hpd.MessagePartDescription, "Internal messagePartDescription should be what we passed to ctor");
        }

        #endregion HttpParameter(MessagePartDescription)

        #endregion Constructors

        #region Properties

        #region Name

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("Name returns value from constructor.")]
        public void Name()
        {
            HttpParameter hpd = new HttpParameter("aName", typeof(char));
            Assert.AreEqual("aName", hpd.Name, "Name property was incorrect.");
        }

        #endregion Name

        #region ParameterType

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ParameterType returns value from constructor.")]
        public void ParameterType()
        {
            HttpParameter hpd = new HttpParameter("aName", typeof(char));
            Assert.AreEqual(typeof(char), hpd.ParameterType, "ParameterType property was incorrect.");
        }

        #endregion ParameterType

        #region IsContentParameter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("IsContentParameter returns false by default.")]
        public void IsContentParameterReturnsFalseByDefault()
        {
            HttpParameter hpd = new HttpParameter("aName", typeof(char));
            Assert.IsFalse(hpd.IsContentParameter, "IsContentParameter should have been false by default.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("IsContentParameter is mutable.")]
        public void IsContentParameterIsMutable()
        {
            HttpParameter hpd = new HttpParameter("aName", typeof(char));
            hpd.IsContentParameter = true;
            Assert.IsTrue(hpd.IsContentParameter, "IsContentParameter should have been set.");
        }

        #endregion IsContentParameter

        #region RequestMessage

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("RequestMessage returns HttpRequestMessage HttpParameter.")]
        public void RequestMessageReturnsHttpRequestMessageHttpParameter()
        {
            HttpParameter hpd = HttpParameter.RequestMessage;
            Assert.IsNotNull(hpd, "RequestMessage retured null.");
            Assert.AreEqual("RequestMessage", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpRequestMessage), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion RequestMessage

        #region RequestUri

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("RequestUri returns Uri HttpParameter.")]
        public void RequestUriReturnsUriHttpParameter()
        {
            HttpParameter hpd = HttpParameter.RequestUri;
            Assert.IsNotNull(hpd, "RequestUri retured null.");
            Assert.AreEqual("RequestUri", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(Uri), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion RequestUri

        #region RequestMethod

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("RequestMethod returns HttpMethod HttpParameter.")]
        public void RequestMethodReturnsHttpMethodHttpParameter()
        {
            HttpParameter hpd = HttpParameter.RequestMethod;
            Assert.IsNotNull(hpd, "RequestMethod retured null.");
            Assert.AreEqual("RequestMethod", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpMethod), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion RequestMethod

        #region RequestHeaders

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("RequestHeaders returns HttpRequestHeaders HttpParameter.")]
        public void RequestHeadersReturnsHeadersHttpParameter()
        {
            HttpParameter hpd = HttpParameter.RequestHeaders;
            Assert.IsNotNull(hpd, "RequestHeaders retured null.");
            Assert.AreEqual("RequestHeaders", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpRequestHeaders), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion RequestHeaders

        #region RequestContent

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("RequestHeaders returns HttpContent HttpParameter.")]
        public void RequestContentReturnsHttpContentHttpParameter()
        {
            HttpParameter hpd = HttpParameter.RequestContent;
            Assert.IsNotNull(hpd, "RequestContent retured null.");
            Assert.AreEqual("RequestContent", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpContent), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion RequestContent

        #region ResponseMessage

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ResponseMessage returns HttpResponseMessage HttpParameter.")]
        public void ResponseMessageReturnsHttpResponseMessageHttpParameter()
        {
            HttpParameter hpd = HttpParameter.ResponseMessage;
            Assert.IsNotNull(hpd, "ResponseMessage retured null.");
            Assert.AreEqual("ResponseMessage", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpResponseMessage), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion ResponseMessage

        #region ResponseStatusCode

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ResponseStatusCode returns HttpStatusCode HttpParameter.")]
        public void ResponseStatusCodeReturnsStatusCodeHttpParameter()
        {
            HttpParameter hpd = HttpParameter.ResponseStatusCode;
            Assert.IsNotNull(hpd, "ResponseStatusCode retured null.");
            Assert.AreEqual("ResponseStatusCode", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpStatusCode), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion ResponseStatusCode

        #region ResponseHeaders

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ResponseHeaders returns HttpResponseHeaders HttpParameter.")]
        public void ResponseHeadersReturnsHttpResponseHeadersHttpParameter()
        {
            HttpParameter hpd = HttpParameter.ResponseHeaders;
            Assert.IsNotNull(hpd, "ResponseHeaders retured null.");
            Assert.AreEqual("ResponseHeaders", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpResponseHeaders), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion ResponseHeaders

        #region ResponseContent

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ResponseContent returns HttpContent HttpParameter.")]
        public void ResponseContentReturnsHttpContentHttpParameter()
        {
            HttpParameter hpd = HttpParameter.ResponseContent;
            Assert.IsNotNull(hpd, "ResponseContent retured null.");
            Assert.AreEqual("ResponseContent", hpd.Name, "Name was incorrect.");
            Assert.AreEqual(typeof(HttpContent), hpd.ParameterType, "ParameterType was incorrect.");
        }

        #endregion ResponseContent

        #region ParameterType (Updated via MessagePartDescription)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("ParameterType returns value updated via MessagePartDescription.")]
        public void ParameterTypeReturnsTypeFromMessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(SimpleOperationsService), "TwoInputOneOutputAndReturnValue");
            MessagePartDescription mpd = od.Messages[0].Body.Parts[0];
            HttpParameter hpd = new HttpParameter(mpd);
            mpd.Type = typeof(float);
            Assert.AreEqual(typeof(float), hpd.ParameterType, "Setting type on messagePartDescription should update http parameter description");
        }

        #endregion ParameterType (Updated via MessagePartDescription)

        #region ValueConverter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ValueConverter returns a converter for all values type.")]
        public void ValueConverterReturnsConverterForAllValueTypes()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                "ValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    HttpParameterValueConverter converter = hpd.ValueConverter;
                    Assert.IsNotNull("ValueConverter returned null.");
                    Assert.AreEqual(convertType, converter.Type, "ValueConverter was made for wrong type.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ValueConverter returns a converter for all ObjectContent<T> types.")]
        public void ValueConverterReturnsConverterForAllObjectContentOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    Type convertType = objectContent.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    HttpParameterValueConverter converter = hpd.ValueConverter;
                    Assert.IsNotNull("ValueConverter returned null.");
                    Assert.AreEqual(convertType, converter.Type, "ValueConverter was made for wrong type.");
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ValueConverter returns a converter for all HttpRequestMessage<T> types.")]
        public void ValueConverterReturnsConverterForAllHttpRequestMessageOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = request.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    HttpParameterValueConverter converter = hpd.ValueConverter;
                    Assert.IsNotNull("ValueConverter returned null.");
                    Assert.AreEqual(convertType, converter.Type, "ValueConverter was made for wrong type.");
                });
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ValueConverter returns a converter for all HttpResponseMessage<T> types.")]
        public void ValueConverterReturnsConverterForAllHttpResponseMessageOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    Type convertType = response.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    HttpParameterValueConverter converter = hpd.ValueConverter;
                    Assert.IsNotNull("ValueConverter returned null.");
                    Assert.AreEqual(convertType, converter.Type, "ValueConverter was made for wrong type.");
                });
        }

        #endregion ValueConverter

        #endregion Properties

        #region Methods

        #region IsAssignableFromParameter<T>()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter<T>() returns true for all values type.")]
        public void IsAssignableFromParameterOfTReturnsTrueForAllValueTypes()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                "ValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    bool result = (bool)Asserters.GenericType.InvokeGenericMethod(hpd, isAssignableFromParameterOfTMethodName, convertType /*, new Type[0], new object[0]*/);
                    Assert.IsTrue(result, string.Format("IsAssignableFromParameter<{0}>() was false.", convertType.Name));
                });
        }

        #endregion IsAssignableFromParameter<T>()

        #region IsAssignableFromParameter(Type)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns true for all values type.")]
        public void IsAssignableFromParameterReturnsTrueForAllValueTypes()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                "ValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsTrue(hpd.IsAssignableFromParameter(convertType), string.Format("IsAssignableFrom({0}) was false.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns true for typeof(string) for all values type.")]
        public void IsAssignableFromParameterReturnsTrueForStringForAllValueTypes()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                "ValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsTrue(hpd.IsAssignableFromParameter(typeof(string)), string.Format("IsAssignableFrom({0}) was false.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns false for a reference type for all values type.")]
        public void IsAssignableFromParameterReturnsFalseForReferenceTypeToValueType()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                "ValueConverter failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsFalse(hpd.IsAssignableFromParameter(typeof(WcfPocoType)), string.Format("IsAssignableFrom({0}) was true.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns true for all ObjectContent<T> types.")]
        public void IsAssignableFromParameterReturnsTrueForAllObjectContentOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachObjectContent(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (objectContent, type, obj) =>
                {
                    Type convertType = objectContent.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsTrue(hpd.IsAssignableFromParameter(convertType), string.Format("IsAssignableFrom({0}) was false.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns true for all HttpRequestMessage<T> types.")]
        public void IsAssignableFromParameterReturnsTrueForAllHttpRequestMessageOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachHttpRequestMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (request, type, obj) =>
                {
                    Type convertType = request.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsTrue(hpd.IsAssignableFromParameter(convertType), string.Format("IsAssignableFrom({0}) was false.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) returns true for all HttpResponseMessage<T> types.")]
        public void IsAssignableFromParameterReturnsTrueForAllHttpResponseMessageOfTTypes()
        {
            Asserters.ObjectContent.ExecuteForEachHttpResponseMessage(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.All,
                (response, type, obj) =>
                {
                    Type convertType = response.GetType();
                    HttpParameter hpd = new HttpParameter("aName", convertType);
                    Assert.IsTrue(hpd.IsAssignableFromParameter(convertType), string.Format("IsAssignableFrom({0}) was false.", convertType.Name));
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("IsAssignableFromParameter(Type) throws with a null Type.")]
        public void IsAssignableFromParameterThrowsWithNullType()
        {
             HttpParameter hpd = new HttpParameter("aName", typeof(int));
             Asserters.Exception.ThrowsArgumentNull("type", () => hpd.IsAssignableFromParameter(null));
        }

        #endregion IsAssignableFromParameter(Type)

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
