// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TestCommon.Rules;

    /// <summary>
    /// Class that encapsulates the <see cref="UnitTestRuleResult"/> returned by a unit test rule
    /// together with the metadata describing that rule.
    /// </summary>
    public class UnitTestIssue
    {
        public UnitTestIssue(UnitTestRuleAttribute unitTestRuleAttribute, UnitTestRuleResult ruleResult)
        {
            this.UnitTestRuleAttribute = unitTestRuleAttribute;
            this.RuleResult = ruleResult;
        }

        public UnitTestRuleAttribute UnitTestRuleAttribute { get; private set; }
        public UnitTestRuleResult RuleResult { get; private set; }
    }
}
