//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System.Diagnostics.CodeAnalysis;

    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/computer_name_format_str.asp
    public enum ComputerNameFormat
    {
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Preserving WCF compat")]
        NetBIOS,
        DnsHostName,
        Dns,
        DnsFullyQualified,

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Preserving WCF compat")]
        PhysicalNetBIOS,
        PhysicalDnsHostName,
        PhysicalDnsDomain,
        PhysicalDnsFullyQualified
    }
}
