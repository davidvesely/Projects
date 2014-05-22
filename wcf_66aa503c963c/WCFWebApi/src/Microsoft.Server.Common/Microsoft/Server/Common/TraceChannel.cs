//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System.Diagnostics.CodeAnalysis;

    //Admin - End User/Admin/Support/Tools
    //Operational - Admin/Support/Tools
    //Analytic - Tools
    //Debug - Developers
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Preserving WCF compat")]
    public enum TraceChannel
    {
        Admin = 16,
        Operational = 17,
        Analytic = 18,
        Debug = 19,

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Preserving WCF compat")]
        Perf = 20,
        Application = 9, //This is reserved for Windows Event Log
    }
}
