// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.Rules
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnitTestRuleAttribute : Attribute
    {
        public UnitTestRuleAttribute(UnitTestLevel unitTestLevel)
        {
            this.UnitTestLevel = unitTestLevel;
        }

        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UnitTestLevel UnitTestLevel { get; private set; }
    }
}
