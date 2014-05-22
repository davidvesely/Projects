// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class HttpOperationHandlerOfTTests : UnitTest<HttpOperationHandler<object, object>>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpOperationHandlerOfT is public and abstract.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(
                this.TypeUnderTest, 
                TypeAssert.TypeProperties.IsPublicVisibleClass | TypeAssert.TypeProperties.IsGenericType | TypeAssert.TypeProperties.IsAbstract);
        }

        #endregion Type

        #region Constructors

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpOperationHandler() throws if the outputParameterName parameter is null.")]
        public void ConstructorThrowsWithNullOutputParameterName()
        {
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(null));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("HttpOperationHandler() throws if the outputParameterName parameter is an empty or whitespace string.")]
        public void ConstructorThrowsWithEmptyOutputParameterName()
        {
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(" "));
            Asserters.Exception.ThrowsArgumentNull("outputParameterName", () => new MockHttpOperationHandler<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(" "));
        }

        #endregion Constructors

        #region Properties

        #region InputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("InputParameters returns the HttpParameters determined by reflecting over the generic HttpOperationHandlerOfT.")]
        public void InputParametersReturnsReflectedHttpParameters()
        {
            List<Type> types = new List<Type>();

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (!types.Contains(type))
                    {
                        types.Add(type);

                        for (int i = 2; i <= 17; i++)
                        {
                            if (types.Count - i < 0)
                            {
                                break;
                            }

                            Type[] typeArray = types.Skip(types.Count - i).ToArray();
                            HttpOperationHandler genericHandler = GetGenericHandlerForTypes(typeArray);

                            for (int j = 0; j < genericHandler.InputParameters.Count; j++)
                            {
                                HttpParameter parameter = genericHandler.InputParameters[j];
                                Assert.AreEqual(typeArray[j], parameter.ParameterType, "The HttpParameter.ParameterType should have been the same type as from the array.");
                                if (i == 2)
                                {
                                    Assert.AreEqual("input", parameter.Name, "The HttpParameter.Name should have been 'input'.");
                                }
                                else
                                {
                                    string expectedName = "input" + (j + 1).ToString();
                                    Assert.AreEqual(expectedName, parameter.Name, string.Format("The HttpParameter.Name should have been '{0}'.", expectedName));
                                }
                            }
                        }
                    }
                });
        }

        #endregion InputParameters

        #region OutputParameters

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("OutputParameters returns the HttpParameters determined by reflecting over the generic HttpOperationHandlerOfT.")]
        public void OutputParametersReturnsReflectedHttpParameters()
        {
            List<Type> types = new List<Type>();

            Asserters.Data.Execute(
                TestData.RepresentativeValueAndRefTypeTestDataCollection,
                (type, obj) =>
                {
                    if (!types.Contains(type))
                    {
                        types.Add(type);

                        for (int i = 2; i <= 17; i++)
                        {
                            if (types.Count - i < 0)
                            {
                                break;
                            }

                            Type[] typeArray = types.Skip(types.Count - i).ToArray();
                            HttpOperationHandler genericHandler = GetGenericHandlerForTypes(typeArray);

                            for (int j = 0; j < genericHandler.OutputParameters.Count; j++)
                            {
                                HttpParameter parameter = genericHandler.OutputParameters[j];
                                Assert.AreEqual("output", parameter.Name, "The HttpParameter.Name should have been 'input'.");
                                Assert.AreEqual(typeArray.Last(), parameter.ParameterType, "The HttpParameter.ParameterType should have been the last type from the array.");
                            }
                        }
                    }
                });
        }

        #endregion OutputParameters

        #endregion Properties

        #region Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        public void HandleCallsOnHandleOfGeneric1()
        {
            MockHttpOperationHandler<int, int> handler = new MockHttpOperationHandler<int, int>("output");
            handler.OnHandleCallback = (in1) => in1;

            object[] output = handler.Handle(new object[]{ 1 });
            Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
            Assert.AreEqual(1, (int)output[0], "The Handle method should have returned the sum of the input values.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        public void HandleCallsOnHandleOfGeneric02()
        {
            MockHttpOperationHandler<int, int, int> handler = new MockHttpOperationHandler<int, int, int>("output");
            handler.OnHandleCallback = (in1, in2) => in1 + in2;

            object[] output = handler.Handle(new object[] { 1, 2 });
            Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
            Assert.AreEqual(3, (int)output[0], "The Handle method should have returned the sum of the input values.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        public void HandleCallsOnHandleOfGeneric03()
        {
            MockHttpOperationHandler<int, int, int, int> handler =
                new MockHttpOperationHandler<int, int, int, int>("output");
            handler.OnHandleCallback = (in1, in2, in3) => in1 + in2 + in3;

            object[] output = handler.Handle(new object[] { 1, 2, 3 });
            Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
            Assert.AreEqual(6, (int)output[0], "The Handle method should have returned the sum of the input values.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        public void HandleCallsOnHandleOfGeneric04()
        {
            MockHttpOperationHandler<int, int, int, int, int> handler =
                new MockHttpOperationHandler<int, int, int, int, int>("output");
            handler.OnHandleCallback = (in1, in2, in3, in4) =>
                in1 + in2 + in3 + in4;

            object[] output = handler.Handle(new object[] { 1, 2, 3, 4 });
            Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
            Assert.AreEqual(10, (int)output[0], "The Handle method should have returned the sum of the input values.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        public void HandleCallsOnHandleOfGeneric05()
        {
            MockHttpOperationHandler<int, int, int, int, int, int> handler =
                new MockHttpOperationHandler<int, int, int, int, int, int>("output");
            handler.OnHandleCallback = (in1, in2, in3, in4, in5) =>
                in1 + in2 + in3 + in4 + in5;

            object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
            Assert.AreEqual(15, (int)output[0], "The Handle method should have returned the sum of the input values.");
        }

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric06()
        ////{
        ////    MockHttpOperationHandler06<int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler06<int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6 = (in1, in2, in3, in4, in5, in6) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(21, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric07()
        ////{
        ////    MockHttpOperationHandler07<int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler07<int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7 = (in1, in2, in3, in4, in5, in6, in7) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(28, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric08()
        ////{
        ////    MockHttpOperationHandler08<int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler08<int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8 = (in1, in2, in3, in4, in5, in6, in7, in8) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(36, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric09()
        ////{
        ////    MockHttpOperationHandler09<int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler09<int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9 = (in1, in2, in3, in4, in5, in6, in7, in8, in9) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(45, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric10()
        ////{
        ////    MockHttpOperationHandler10<int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler10<int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(55, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric11()
        ////{
        ////    MockHttpOperationHandler11<int, int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler11<int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(66, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric12()
        ////{
        ////    MockHttpOperationHandler12<int, int, int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler12<int, int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11T12 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11 + in12;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(78, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric13()
        ////{
        ////    MockHttpOperationHandler13<int, int, int, int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler13<int, int, int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11T12T13 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11 + in12 + in13;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(91, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric14()
        ////{
        ////    MockHttpOperationHandler14<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler14<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11T12T13T14 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11 + in12 + in13 + in14;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(105, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric15()
        ////{
        ////    MockHttpOperationHandler15<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> handler =
        ////        new MockHttpOperationHandler15<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11T12T13T14T15 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11 + in12 + in13 + in14 + in15;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(120, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        ////[TestMethod]
        ////[TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        ////[Description("Handle(object[]) calls OnHandle for the generic HttpOperationHandlerOfT.")]
        ////public void HandleCallsOnHandleOfGeneric16()
        ////{
        ////    MockHttpOperationHandler16<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> handler = 
        ////        new MockHttpOperationHandler16<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>("output");
        ////    handler.OnHandleT1T2T3T4T5T6T7T8T9T10T11T12T13T14T15T16 = (in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15, in16) =>
        ////        in1 + in2 + in3 + in4 + in5 + in6 + in7 + in8 + in9 + in10 + in11 + in12 + in13 + in14 + in15 + in16;

        ////    object[] output = handler.Handle(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16});
        ////    Assert.AreEqual(1, output.Length, "The Handle method should have returned an array of length 1.");
        ////    Assert.AreEqual(136, (int)output[0], "The Handle method should have returned the sum of the input values.");
        ////}

        #endregion Methods

        #region Test Helpers

        public static HttpOperationHandler GetGenericHandlerForTypes(Type[] parameterTypes)
        {
            Type handlerType = null;
            switch (parameterTypes.Length) 
            {
                case 2: handlerType = typeof(MockHttpOperationHandler<,>); break;
                case 3: handlerType = typeof(MockHttpOperationHandler<,,>); break;
                case 4: handlerType = typeof(MockHttpOperationHandler<,,,>); break;
                case 5: handlerType = typeof(MockHttpOperationHandler<,,,,>); break;
                case 6: handlerType = typeof(MockHttpOperationHandler<,,,,,>); break;
                case 7: handlerType = typeof(MockHttpOperationHandler<,,,,,,>); break;
                case 8: handlerType = typeof(MockHttpOperationHandler<,,,,,,,>); break;
                case 9: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,>); break;
                case 10: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,>); break;
                case 11: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,>); break;
                case 12: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,>); break;
                case 13: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,,>); break;
                case 14: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,,,>); break;
                case 15: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,,,,>); break;
                case 16: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,,,,,>); break;
                case 17: handlerType = typeof(MockHttpOperationHandler<,,,,,,,,,,,,,,,,>); break;
                default:
                    Assert.Fail("Test Error: The type array can not be used to create a generic HttpOperationHandler");
                    break;
            }

            return Asserters.GenericType.InvokeConstructor<HttpOperationHandler>(handlerType, parameterTypes, new object[] { "output" });
        }

        #endregion Test Helpers
    }
}
