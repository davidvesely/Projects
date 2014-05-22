// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;

    /// <summary>
    /// Class that holds the result of executing a unit test rule.
    /// </summary>
    public class UnitTestRuleResult
    {
        public static readonly UnitTestRuleResult Success = null;

        public UnitTestRuleResult(string message, UnitTestIssueLevel level)
        {
            this.Message = message;
            this.Level = level;
        }

        public string Message { get; private set; }

        public UnitTestIssueLevel Level { get; private set; }
    }
}
