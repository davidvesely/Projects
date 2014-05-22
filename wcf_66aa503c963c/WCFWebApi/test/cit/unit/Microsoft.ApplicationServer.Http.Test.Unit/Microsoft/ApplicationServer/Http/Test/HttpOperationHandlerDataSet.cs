// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Http;

    /// <summary>
    /// C: concrete
    /// A: abstract
    /// I: interface
    /// B: base
    /// D: derived
    /// M: implements an interface
    /// S: struct
    /// E: enum
    /// Z: serializable
    /// N: nested
    /// G: Generic
    /// Class names are composed of these terms
    /// </summary>
    public class HttpOperationHandlerDataSet
    {
        public interface I1 { }

        public interface I2 { }

        public class CB { public class CN { } }

        public class CBG<T> { }

        [Serializable]
        public class CZ { }

        public class CD : CB { }

        public class CM : I1 { }

        public abstract class AB { }

        public abstract class AD : CB { }

        public abstract class AM : I1 { }

        public struct S { }

        [FlagsAttribute]
        public enum E1 { }

        public enum E2 : long { }

        [System.Runtime.Serialization.DataContract]
        class Data { }

        public static readonly TestData<Type> LegalHttpParameterTypes = new RefTypeTestData<Type>(() => new List<Type>(HttpTestData.LegalHttpParameterTypes.GetTestDataAsList())
        { 
            typeof(I1),
            typeof(I2),
            typeof(CB),
            typeof(CB.CN),
            typeof(CBG<object>),
            typeof(CZ),
            typeof(CD),
            typeof(CM),
            typeof(AB),
            typeof(AD),
            typeof(AM),
            typeof(E1),
            typeof(E2),
            typeof(S),
            typeof(Action<CD>),
            typeof(Func<CB>),
            typeof(Data)
        });
    }
}
