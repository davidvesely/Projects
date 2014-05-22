// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Test;
    ////using Microsoft.Infrastructure.Runtime;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.TestCommon.WCF.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.InProgress)]
    public class HttpOperationHandlerTests : UnitTest<HttpOperationHandler>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpOperationHandler is public and abstract.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region Constructors
        #endregion Constructors

        #region Properties

        #region InputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters invokes virtual OnGetInputParmeters.")]
        public void InputParametersCallsOnGetInputParmeters()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            bool wereCalled = false;
            handler.OnGetInputParametersCallback = () => { wereCalled = true; return new HttpParameter[0]; };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.IsTrue(wereCalled, "OnGetInputParameters was not called.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters does not invoke virtual OnGetInputParmeters more than once.")]
        public void InputParametersDoesNotCallOnGetInputParmetersTwice()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            int callCount = 0;
            handler.OnGetInputParametersCallback = () => { ++callCount; return new HttpParameter[0]; };
            ReadOnlyCollection<HttpParameter> arguments1 = handler.InputParameters;
            ReadOnlyCollection<HttpParameter> arguments2 = handler.InputParameters;
            Assert.AreEqual(1, callCount, "OnGetInputParameters was called more than once.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters returns a ReadOnlyCollection<HttpParameter>.")]
        public void InputParametersReturnsReadOnlyCollection()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", typeof(string)) };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.IsNotNull(arguments, "InputParameters should never be null.");
            Assert.AreEqual(1, arguments.Count, "InputParameters.Count should have been 1.");
            HttpParameter hpd = arguments[0];
            Assert.AreEqual("arg1", hpd.Name, "Did not set inputParameters[0] corectly.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters invokes virtual OnGetInputParmeters, accepts a null return and produces an empty collection.")]
        public void InputParametersAcceptsNullFromOnGetInputParmeters()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            bool wereCalled = false;
            handler.OnGetInputParametersCallback = () => { wereCalled = true; return null; };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.IsTrue(wereCalled, "OnGetInputParameters was not called.");
            Assert.AreEqual(0, arguments.Count, "Collection should have been empty.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters preserves the order given by OnGetInputParameters.")]
        public void InputParametersPreservesOrderFromOnGetInputParameters()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
                new HttpParameter("arg2", typeof(int))
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Asserters.HttpParameter.AreEqual(parameters, arguments, "Order was not preserved.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("InputParameters clones the parameters given by OnGetInputParameters.")]
        public void InputParametersClonesParametersFromOnGetInputParameters()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };
            
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            bool isContentParameterOriginal = parameters[0].IsContentParameter;
            bool isContentParameterCloned = arguments[0].IsContentParameter;
            Assert.AreEqual(isContentParameterOriginal, isContentParameterCloned, "IsContentParameter property was not properly cloned.");
            parameters[0].IsContentParameter = !isContentParameterOriginal;
            Assert.AreEqual(isContentParameterOriginal, isContentParameterCloned, "IsContentParameter property on original should not have affected clone.");
        }

        #endregion InputParameters

        #region OutputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters invokes virtual OnGetOutputParmeters.")]
        public void OutputParametersCallsOnGetInputParmeters()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            bool wereCalled = false;
            handler.OnGetOutputParametersCallback = () => { wereCalled = true; return new HttpParameter[0]; };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.IsTrue(wereCalled, "OnGetOutputParameters was not called.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters does not invoke virtual OnGetOutputParmeters more than once.")]
        public void OutputParametersDoesNotCallOnGetOutputParmetersTwice()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            int callCount = 0;
            handler.OnGetOutputParametersCallback = () => { ++callCount; return new HttpParameter[0]; };
            ReadOnlyCollection<HttpParameter> arguments1 = handler.OutputParameters;
            ReadOnlyCollection<HttpParameter> arguments2 = handler.OutputParameters;
            Assert.AreEqual(1, callCount, "OnGetOutputParameters was called more than once.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters returns a ReadOnlyCollection<HttpParameter>.")]
        public void OutputParametersReturnsReadOnlyCollection()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", typeof(string)) };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.IsNotNull(arguments, "OutputParameters should never be null.");
            Assert.AreEqual(1, arguments.Count, "OutputParameters.Count should have been 1.");
            HttpParameter hpd = arguments[0];
            Assert.AreEqual("arg1", hpd.Name, "Did not set OutputParameters[0] corectly.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters invokes virtual OnGetOutputParmeters, accepts a null return and produces an empty collection.")]
        public void OutputParametersAcceptsNullFromOnGetOutputParmeters()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            bool wereCalled = false;
            handler.OnGetOutputParametersCallback = () => { wereCalled = true; return null; };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.IsTrue(wereCalled, "OnGetOutputParameters was not called.");
            Assert.AreEqual(0, arguments.Count, "Collection should have been empty.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters preserves the order given by OnGetInputParameters.")]
        public void OutputParametersPreservesOrderFromOnGetOutputParameters()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
                new HttpParameter("arg2", typeof(int))
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => parameters;
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Asserters.HttpParameter.AreEqual(parameters, arguments, "Order was not preserved.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("OutputParameters clones the parameters given by OnGetOutputParameters.")]
        public void OutputParametersClonesParametersFromOnGetInputParameters()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => parameters;
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            bool isContentParameterOriginal = parameters[0].IsContentParameter;
            bool isContentParameterCloned = arguments[0].IsContentParameter;
            Assert.AreEqual(isContentParameterOriginal, isContentParameterCloned, "IsContentParameter property was not properly cloned.");
            parameters[0].IsContentParameter = !isContentParameterOriginal;
            Assert.AreEqual(isContentParameterOriginal, isContentParameterCloned, "IsContentParameter property on original should not have affected clone.");
        }

        #endregion OutputParameters

        #endregion Properties

        #region Methods

        #region OnGetOutputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() can work without any output parameters.")]
        public void OnGetOutputParametersWithNull()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => { return null; };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.AreEqual<int>(0, arguments.Count);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() can return an empty set of output parameters.")]
        public void OnGetOutputParametersWithEmptySet()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => new HttpParameter[] { };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.AreEqual<int>(0, arguments.Count);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() can return output parameters in ReadOnlyCollection<HttpParameter>.")]
        public void OnGetOutputParametersWithReadOnlyCollection()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", typeof(string)) };
            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
            Assert.IsInstanceOfType(arguments, typeof(ReadOnlyCollection<HttpParameter>));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() throws when returning HttpParameter with null type.")]
        public void OnGetOutputParametersWithNullArgumentType()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetOutputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", null) };
            Asserters.Exception.ThrowsArgumentNull("type", () => { ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters; });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() throws for output parameter with empty name.")]
        public void OnGetOutputParametersWithNullArgumentName()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();

            foreach (string name in TestData.EmptyStrings)
            {
                handler.OnGetOutputParametersCallback = () => new HttpParameter[] { new HttpParameter(name, typeof(string)) };
                Asserters.Exception.ThrowsArgumentNull("name", () => { ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters; });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() can return an output parameter with a single character parameter name.")]
        public void OnGetOutputParametersWithSingleCharArgumentName()
        {
            foreach (char name in TestData.CharTestData)
            {
                MockHttpOperationHandler handler = new MockHttpOperationHandler();
                HttpParameter[] parameters = { new HttpParameter(name.ToString(), typeof(string)) };
                handler.OnGetOutputParametersCallback = () => parameters;
                ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
                Asserters.HttpParameter.AreEqual(parameters, arguments, "Name was not matched");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetOutputParameters() can return output parameters with any combination between legal parameter names and legal parameter types.")]
        public void OnGetOutputParametersWithLegalParameterNames()
        {
            foreach (string name1 in HttpTestData.LegalHttpParameterNames)
            {
                foreach (string name2 in HttpTestData.LegalHttpParameterNames)
                {
                    foreach (var type1 in HttpOperationHandlerDataSet.LegalHttpParameterTypes)
                    {
                        foreach (var type2 in HttpOperationHandlerDataSet.LegalHttpParameterTypes)
                        {
                            MockHttpOperationHandler handler = new MockHttpOperationHandler();
                            HttpParameter[] parameters = { new HttpParameter(name1, type1), new HttpParameter(name2, type2) };
                            handler.OnGetOutputParametersCallback = () => parameters;
                            ReadOnlyCollection<HttpParameter> arguments = handler.OutputParameters;
                            Asserters.HttpParameter.AreEqual(parameters, arguments, "Name, type or both don't match");
                        }
                    }
                }
            }
        }

        #endregion

        #region OnGetInputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() can work without any input parameters.")]
        public void OnGetInputParametersWithNull()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => { return null; };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.AreEqual<int>(0, arguments.Count);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() can return an empty set of input parameters.")]
        public void OnGetInputParametersWithEmptySet()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => new HttpParameter[] { };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.AreEqual<int>(0, arguments.Count);
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() can return input parameters in ReadOnlyCollection<HttpParameter>.")]
        public void OnGetInputParametersWithReadOnlyCollection()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", typeof(string)) };
            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
            Assert.IsInstanceOfType(arguments, typeof(ReadOnlyCollection<HttpParameter>));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() throws when returning HttpParameter with null type.")]
        public void OnGetInputParametersWithNullArgumentType()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => new HttpParameter[] { new HttpParameter("arg1", null) };
            Asserters.Exception.ThrowsArgumentNull("type", () => { ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters; });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() throws for input parameter with empty name.")]
        public void OnGetInputParametersWithNullArgumentName()
        {
            MockHttpOperationHandler handler = new MockHttpOperationHandler();

            foreach (string name in TestData.EmptyStrings)
            {
                handler.OnGetInputParametersCallback = () => new HttpParameter[] { new HttpParameter(name, typeof(string)) };
                Asserters.Exception.ThrowsArgumentNull("name", () => { ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters; });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() can return an input parameter with a single character parameter name.")]
        public void OnGetInputParametersWithSingleCharArgumentName()
        {
            foreach (char name in TestData.CharTestData)
            {
                MockHttpOperationHandler handler = new MockHttpOperationHandler();
                HttpParameter[] parameters = { new HttpParameter(name.ToString(), typeof(string)) };
                handler.OnGetInputParametersCallback = () => parameters;
                ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
                Asserters.HttpParameter.AreEqual(parameters, arguments, "Name was not matched");
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("maying")]
        [Description("OnGetInputParameters() can return input parameters with any combination between legal parameter names and legal parameter types.")]
        public void OnGetInputParametersWithLegalParameterNames()
        {
            foreach (string name1 in HttpTestData.LegalHttpParameterNames)
            {
                foreach (string name2 in HttpTestData.LegalHttpParameterNames)
                {
                    foreach (var type1 in HttpOperationHandlerDataSet.LegalHttpParameterTypes)
                    {
                        foreach (var type2 in HttpOperationHandlerDataSet.LegalHttpParameterTypes)
                        {
                            MockHttpOperationHandler handler = new MockHttpOperationHandler();
                            HttpParameter[] parameters = { new HttpParameter(name1, type1), new HttpParameter(name2, type2) };
                            handler.OnGetInputParametersCallback = () => parameters;
                            ReadOnlyCollection<HttpParameter> arguments = handler.InputParameters;
                            Asserters.HttpParameter.AreEqual(parameters, arguments, "Name, type or both don't match");
                        }
                    }
                }
            }
        }

        #endregion

        #region Handle(object[])

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws with null Input.")]
        public void HandleThrowsWithNullInput()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;

            Asserters.Exception.ThrowsArgumentNull("input", () => handler.Handle(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws with Input shorter than expected.")]
        public void HandleThrowsWithTooSmallInput()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;

            string errorMessage = SR.HttpOperationHandlerReceivedWrongNumberOfValues(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    parameters.Length,
                                    0);

            Asserters.Exception.Throws<InvalidOperationException>(errorMessage, () => handler.Handle(new object[0]));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws with Input longer than expected.")]
        public void HandleThrowsWithTooLargeInput()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;

            string errorMessage = SR.HttpOperationHandlerReceivedWrongNumberOfValues(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    parameters.Length,
                                    2);

            Asserters.Exception.Throws<InvalidOperationException>(errorMessage, () => handler.Handle(new object[2]));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws with Input that cannot be converted to expected types.")]
        public void HandleThrowsWitUnconvertableInput()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters; 

            string errorMessage = SR.HttpOperationHandlerReceivedWrongType(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    parameters[0].ParameterType.Name,
                                    parameters[0].Name,
                                    typeof(WcfPocoType).Name);

            Asserters.Exception.Throws<InvalidOperationException>(errorMessage, () => handler.Handle(new object[] { new WcfPocoType() }));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) converts string Input to required type.")]
        public void HandleConvertsStringInput()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                "Handle string input failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    if (Asserters.HttpParameter.CanConvertToStringAndBack(convertType))
                    {
                        HttpParameter hpd = new HttpParameter("aName", convertType);
                        HttpParameter[] parameters = new HttpParameter[] { hpd };

                        MockHttpOperationHandler handler = new MockHttpOperationHandler();
                        handler.OnGetInputParametersCallback = () => parameters;
                        handler.OnGetOutputParametersCallback = () => parameters;
                        handler.OnHandleCallback = (oArray) => oArray;

                        object[] result = handler.Handle(new object[] { obj.ToString() });
                        Assert.IsNotNull(result, "Null result returned from Handle.");
                        Assert.AreEqual(1, result.Length, "Handle returned wrong length array.");
                        Assert.AreEqual(convertType, result[0].GetType(), "Value did not convert to right type.");
                        Assert.AreEqual(obj.ToString(), result[0].ToString(), "Object did not convert to the right value.");
                    }
                });
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) calls OnHandle().")]
        public void HandleCallsOnHandle()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            bool called = false;
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => { called = true; return oArray; };

            handler.Handle(new object[] { "fred" });
            Assert.IsTrue(called, "Handle did not call OnHandle.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) returns empty array of the correct size when OnHandle() returns null.")]
        public void HandleReturnsEmptyArrayIfOnHandleReturnsNull()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => null;

            object[] result = handler.Handle(new object[] { "fred" });
            Assert.IsNotNull(result, "Handle returned null.");
            Assert.AreEqual(1, result.Length, "Handle returned wrong length array.");
            Assert.IsNull(result[0], "Handle did not return empty array.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws when OnHandle() returns an array smaller than promised.")]
        public void HandleThrowsIfOnHandleReturnsTooSmallArray()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => new object[0];

            string errorMessage = SR.HttpOperationHandlerProducedWrongNumberOfValues(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    1,
                                    0);

            Asserters.Exception.Throws<InvalidOperationException>(
                errorMessage,
                () => handler.Handle(new object[] { "fred" })
                );
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws when OnHandle() returns an array larger than promised.")]
        public void HandleThrowsIfOnHandleReturnsTooLargeArray()
        {
            HttpParameter[] parameters = new HttpParameter[] {
                new HttpParameter("arg1", typeof(string)),
            };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => new object[2];

            string errorMessage = SR.HttpOperationHandlerProducedWrongNumberOfValues(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    1,
                                    2);

            Asserters.Exception.Throws<InvalidOperationException>(
                errorMessage,
                () => handler.Handle(new object[] { "fred" })
                );
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) throws when OnHandle() returns an array containing types that cannot be converted to what it promised.")]
        public void HandleThrowsIfOnHandleReturnsArrayContainingNonconvertableTypes()
        {
            HttpParameter hpd = new HttpParameter("arg1", typeof(WcfPocoType));
            HttpParameter[] parameters = new HttpParameter[] { hpd };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => new object[] { "notAPocoType" };

            string errorMessage = SR.HttpOperationHandlerReceivedWrongType(
                                    typeof(HttpOperationHandler).Name,
                                    handler.ToString(),
                                    handler.OperationName,
                                    hpd.ParameterType.Name,
                                    hpd.Name,
                                    typeof(string).Name);

            Asserters.Exception.Throws<InvalidOperationException>(
                errorMessage,
                () => handler.Handle(new object[] { "fred" })
                );
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) converts string values returned by OnHandle.")]
        public void HandleConvertsStringValuesReturnedFromOnHandle()
        {
            Asserters.Data.Execute(
                DataSets.Http.ConvertableValueTypes,
                TestDataVariations.AllSingleInstances,
                "Handle failed",
                (type, obj) =>
                {
                    Type convertType = obj.GetType();
                    if (Asserters.HttpParameter.CanConvertToStringAndBack(convertType))
                    {
                        HttpParameter hpd = new HttpParameter("aName", convertType);
                        HttpParameter[] parameters = new HttpParameter[] { hpd };

                        MockHttpOperationHandler handler = new MockHttpOperationHandler();
                        handler.OnGetInputParametersCallback = () => parameters;
                        handler.OnGetOutputParametersCallback = () => parameters;
                        handler.OnHandleCallback = (oArray) => new object[] { obj.ToString() };

                        object[] result = handler.Handle(new object[] { obj });
                        Assert.IsNotNull(result, "Null result returned from Handle.");
                        Assert.AreEqual(1, result.Length, "Handle returned wrong length array.");
                        Assert.AreEqual(convertType, result[0].GetType(), "Value did not convert to right type.");
                        Assert.AreEqual(obj.ToString(), result[0].ToString(), "Object did not convert to the right value.");
                    }
                });
        }


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("vinelap")]
        [Description("Handle(object[]) propagates any exception thrown from OnHandle.")]
        public void HandlePropagatesExceptionFromOnHandle()
        {
            HttpParameter hpd = new HttpParameter("arg1", typeof(int));
            HttpParameter[] parameters = new HttpParameter[] { hpd };

            MockHttpOperationHandler handler = new MockHttpOperationHandler();
            handler.OnGetInputParametersCallback = () => parameters;
            handler.OnGetOutputParametersCallback = () => parameters;
            handler.OnHandleCallback = (oArray) => { throw new NotSupportedException("myMessage"); };

            Asserters.Exception.Throws<NotSupportedException>(
                "myMessage",
                () => handler.Handle(new object[] { 5 })
                );
        }

        #endregion Handle(object[])

        #endregion Methods
    }
}