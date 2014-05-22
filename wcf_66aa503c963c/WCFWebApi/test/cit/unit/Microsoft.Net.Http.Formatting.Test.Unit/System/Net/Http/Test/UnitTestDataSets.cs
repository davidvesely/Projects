// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Test
{

    public class UnitTestDataSets
    {
        private static readonly Microsoft.TestCommon.UnitTestDataSets commonDataSets = new Microsoft.TestCommon.UnitTestDataSets();
        private static readonly Microsoft.TestCommon.WCF.UnitTestDataSets wcfDataSets = new Microsoft.TestCommon.WCF.UnitTestDataSets();
        private static readonly Microsoft.TestCommon.WCF.Http.UnitTestDataSets httpDataSets = new Microsoft.TestCommon.WCF.Http.UnitTestDataSets();

        public Microsoft.TestCommon.UnitTestDataSets Common { get { return commonDataSets; } }

        public Microsoft.TestCommon.WCF.UnitTestDataSets WCF { get { return wcfDataSets; } }

        public Microsoft.TestCommon.WCF.Http.UnitTestDataSets Http { get { return httpDataSets; } }
    }
}
