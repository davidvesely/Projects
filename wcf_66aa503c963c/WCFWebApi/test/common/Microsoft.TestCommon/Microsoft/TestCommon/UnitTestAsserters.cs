// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class that composes all the available asserter classes into a single
    /// place to make them available to Intellisense.
    /// </summary>
    /// <remarks>
    /// New asserters added to this assembly should be exposed by adding additional
    /// public properties to this class.
    /// </remarks>
    public class UnitTestAsserters
    {
        /// <summary>
        /// Gets the <see cref="TestDataAssert"/> asserter singleton instance.
        /// </summary>
        public TestDataAssert Data { get { return TestDataAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="ExceptionAssert"/> asserter singleton instance.
        /// </summary>
        public ExceptionAssert Exception { get { return ExceptionAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="GenericTypeAssert"/> asserter singleton instance.
        /// </summary>
        public GenericTypeAssert GenericType { get { return GenericTypeAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="StreamAssert"/> asserter singleton instance.
        /// </summary>
        public StreamAssert Stream { get { return StreamAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="TaskAssert"/> asserter singleton instance.
        /// </summary>
        public TaskAssert Task { get { return TaskAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="TypeAssert"/> asserter singleton instance.
        /// </summary>
        public TypeAssert Type { get { return TypeAssert.Singleton; } }

        /// <summary>
        /// Gets the <see cref="StringAssert"/> asserter singleton instance.
        /// </summary>
        public StringAssert String { get { return StringAssert.Singleton; } }
    }
}