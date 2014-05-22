// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;

    /// <summary>
    /// Custom attribute used to indicate the level of verification to
    /// apply to a unit test class.
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnitTestLevelAttribute : Attribute
    {
        public const UnitTestLevel DefaultUnitTestLevel = UnitTestLevel.InProgress;
        public const int DefaultPublicMemberMinimumCoverage = 90;
        public const int DefaultNonPublicMemberMinimumCoverage = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestLevelAttribute"/> class.
        /// </summary>
        /// <param name="unitTestLevel"></param>
        public UnitTestLevelAttribute(UnitTestLevel unitTestLevel)
        {
            this.UnitTestLevel = unitTestLevel;
            this.PublicMemberMinimumCoverage = DefaultPublicMemberMinimumCoverage;
            this.NonPublicMemberMinimumCoverage = DefaultNonPublicMemberMinimumCoverage;
        }

        internal UnitTestLevelAttribute(UnitTestSuiteConfigurationAttribute suiteLevelAttribute)
        {
            this.UnitTestLevel = suiteLevelAttribute.UnitTestLevel;
            this.PublicMemberMinimumCoverage = suiteLevelAttribute.PublicMemberMinimumCoverage;
            this.NonPublicMemberMinimumCoverage = suiteLevelAttribute.NonPublicMemberMinimumCoverage;
        }

        /// <summary>
        /// Gets the level of verification to apply to the unit test class
        /// marked with this <see cref="UnitTestLevelAttribute"/>.
        /// </summary>
        public UnitTestLevel UnitTestLevel { get; private set; }

        /// <summary>
        /// Gets or sets the minimum percentage of public members which must be tested
        /// by the unit test for this type before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int PublicMemberMinimumCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum percentage of non-public members which must be tested
        /// by the unit test for this type before it can be marked <see cref="UnitTestLevel.Complete"/>.
        /// </summary>
        public int NonPublicMemberMinimumCoverage { get; set; }
    }
}
