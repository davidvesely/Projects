// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF
{

    public class UnitTestAsserters : Microsoft.TestCommon.UnitTestAsserters
    {
        public new TestDataAssert Data { get { return TestDataAssert.Singleton; } }

        public new GenericTypeAssert GenericType { get { return GenericTypeAssert.Singleton; } }

        public SerializerAssert Serializer { get { return SerializerAssert.Singleton; } }

        public ConfigAssert Config { get { return ConfigAssert.Singleton; } }
    }
}