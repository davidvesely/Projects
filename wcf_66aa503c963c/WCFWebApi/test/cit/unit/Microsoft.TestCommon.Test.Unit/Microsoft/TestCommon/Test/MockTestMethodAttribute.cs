// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MockTestMethodAttribute : Attribute
    {
        public MockTestMethodAttribute()
        {
        }
    }
}
