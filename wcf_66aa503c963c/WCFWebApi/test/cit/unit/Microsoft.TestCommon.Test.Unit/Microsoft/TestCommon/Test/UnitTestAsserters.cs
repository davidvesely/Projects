// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.CIT.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class UnitTestAsserters : Microsoft.TestCommon.UnitTestAsserters
    {
        public ContextAssert Context { get { return ContextAssert.Singleton; } }

        public RuleAssert Rule { get { return RuleAssert.Singleton; } }
    }
}
