// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using Microsoft.TestCommon.Rules;

    /// <summary>
    /// Custom attribute used to configure a <see cref="UnitTestSuite"/> class.
    /// </summary>
    /// <remarks>
    /// This custom attribute may appear only on a <see cref="UnitTestSuite"/> class and
    /// sets the quality bars for the suite and the default quality bars for classes
    /// within the suite.
    /// </remarks>
    [Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnitTestSuiteConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestSuiteConfigurationAttribute"/> class.
        /// </summary>
        public UnitTestSuiteConfigurationAttribute()
        {
            // Set all defaults
            this.ValidTestCategories = new string[] { "CIT" };
            this.RuleSetTypes = new Type[] { typeof(StandardRuleSet), typeof(EtcmRuleSet), typeof(NamingRuleSet) };
            this.MaxTimeoutMs = TimeoutConstant.ExtendedTimeout;
            this.PublicTypeMinimumCoverage = 90;
            this.PublicMemberMinimumCoverage = 90;
            this.UnitTestLevel = UnitTestLevel.InProgress;
            this.DefaultUnitTestClassLevel = UnitTestLevel.InProgress;
        }

        /// <summary>
        /// Gets or sets the minimum percentage of public types this unit test suite must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicTypeMinimumCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum percentage of non-public types this unit test suite must test
        /// before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicTypeMinimumCoverage { get; set; }

        /// <summary>
        /// Gets or sets the default minimum percentage of public members each unit test class
        /// must test before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicMemberMinimumCoverage { get; set; }

        /// <summary>
        /// Gets or sets the default minimum percentage of non-public members each unit test class
        /// must test before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicMemberMinimumCoverage { get; set; }

        /// <summary>
        /// Gets or sets the maximum timeout (milliseconds) any unit test in this
        /// suite may declare.
        /// </summary>
        public int MaxTimeoutMs { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UnitTestLevel"/> of the entire suite of unit tests.
        /// </summary>
        public UnitTestLevel UnitTestLevel { get; set; }

        /// <summary>
        /// Gets or sets the default<see cref="UnitTestLevel"/> for every unit test class
        /// which does not supply its own.
        /// </summary>
        public UnitTestLevel DefaultUnitTestClassLevel { get; set; }

        /// <summary>
        /// Gets or sets the rule sets used by this unit test suite.
        /// </summary>
        /// <remarks>
        /// The types specified here will be scanned for any static methods
        /// marked with <see cref="UnitTestRuleAttribute"/> to extract the rules.
        /// </remarks>
        public Type[] RuleSetTypes { get; set; }

        /// <summary>
        /// Gets or sets the legal categories for the unit test classes within this
        /// suite.
        /// </summary>
        /// <remarks>
        /// Unit tests declare their category in a <see cref="CategoryAttribute"/>, and
        /// the value they provide must be one from this list.
        /// </remarks>
        public string[] ValidTestCategories { get; set; }
    }
}
