//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common.Interop
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security;
    using Microsoft.Win32.SafeHandles;

    [Fx.Tag.SecurityNote(Critical = "Usage of SafeHandleZeroOrMinusOneIsInvalid, which is protected by a LinkDemand and InheritanceDemand")]
    [SecurityCritical]
    sealed class SafeEventLogWriteHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Note: RegisterEventSource returns 0 on failure
        [Fx.Tag.SecurityNote(Critical = "Usage of SafeHandleZeroOrMinusOneIsInvalid, which is protected by a LinkDemand and InheritanceDemand")]
        [SecurityCritical]
        SafeEventLogWriteHandle() : base(true) { }

        [ResourceConsumption(ResourceScope.Machine)]
        [Fx.Tag.SecurityNote(Critical = "Usage of SafeHandleZeroOrMinusOneIsInvalid, which is protected by a LinkDemand and InheritanceDemand")]
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Preserve WCF compat")]
        [SecurityCritical]
        public static SafeEventLogWriteHandle RegisterEventSource(string uncServerName, string sourceName)
        {
            SafeEventLogWriteHandle retval = UnsafeNativeMethods.RegisterEventSource(uncServerName, sourceName);
            int error = Marshal.GetLastWin32Error();
            if (retval.IsInvalid)
            {
                Debug.Print("SafeEventLogWriteHandle::RegisterEventSource[" + uncServerName + ", " + sourceName + "] Failed. Last Error: " +
                    error.ToString(CultureInfo.InvariantCulture));
            }

            return retval;
        }

        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Ported from WCF")]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Ported from WCF")]
        [SuppressMessage("Microsoft.Interoperability", "CA1414:MarkBooleanPInvokeArgumentsWithMarshalAs", Justification = "Ported from WCF")]
        [DllImport("advapi32", SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        static extern bool DeregisterEventSource(IntPtr hEventLog);

        [Fx.Tag.SecurityNote(Critical = "Usage of SafeHandleZeroOrMinusOneIsInvalid, which is protected by a LinkDemand and InheritanceDemand")]
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return DeregisterEventSource(this.handle);
        }
    }
}