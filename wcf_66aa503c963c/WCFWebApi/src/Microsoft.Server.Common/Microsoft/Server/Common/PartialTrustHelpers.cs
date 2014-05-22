//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;

    public static class PartialTrustHelpers
    {
        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        static Type aptca;

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        static volatile bool checkedForFullTrust;
        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        static bool inFullTrust;

        public static bool ShouldFlowSecurityContext
        {
            [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
            [SecurityCritical]
            get
            {
                if (SecurityManager.CurrentThreadRequiresSecurityContextCapture())
                {
                    // It's possible for the above check to return true even if we really don't
                    // need to capture the security context. Do a Demand for the current AppDomain's
                    // permission set. If that succeeds, then we don't need to capture the security
                    // context because there are no AppDomains on the stack with fewer privileges than
                    // the current domain. If the demand results in a SecurityException, then return true
                    // indicating that we should flow the security context. The caller will capture it.
                    try
                    {
                        AppDomain.CurrentDomain.PermissionSet.Demand();
                    }
                    catch (SecurityException)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        public static bool IsInFullTrust()
        {
            if (!SecurityManager.CurrentThreadRequiresSecurityContextCapture())
            {
                return true;
            }

            try
            {
                DemandForFullTrust();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Captures security context with identity flow suppressed, " +
            "this requires satisfying a LinkDemand for infrastructure.")]
        [SecurityCritical]
        public static SecurityContext CaptureSecurityContextNoIdentityFlow()
        {
            // capture the security context but never flow windows identity
            if (SecurityContext.IsWindowsIdentityFlowSuppressed())
            {
                return SecurityContext.Capture();
            }
            else
            {
                using (SecurityContext.SuppressFlowWindowsIdentity())
                {
                    return SecurityContext.Capture();
                }
            }
        }
                
        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        public static bool IsTypeAptca(Type type)
        {
            Assembly assembly = type.Assembly;
            return IsAssemblyAptca(assembly) || !IsAssemblySigned(assembly);
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void DemandForFullTrust()
        {
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        public static bool IsAssemblyAptca(Assembly assembly)
        {
            if (aptca == null)
            {
                aptca = typeof(AllowPartiallyTrustedCallersAttribute);
            }
            return assembly.GetCustomAttributes(aptca, false).Length > 0;
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "Ported from WCF")]
        internal static bool IsAssemblySigned(Assembly assembly)
        {
            byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
            return publicKeyToken != null & publicKeyToken.Length > 0;
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        public static bool CheckAppDomainPermissions(PermissionSet permissions)
        {
            return AppDomain.CurrentDomain.IsHomogenous &&
                   permissions.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision")]
        [SecurityCritical]
        public static bool HasEtwPermissions()
        {
            //Currently unrestricted permissions are required to create Etw provider. 
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
            return CheckAppDomainPermissions(permissions);
        }

        public static bool AppDomainFullyTrusted
        {
            [Fx.Tag.SecurityNote(Critical = "used in a security-sensitive decision",
                Safe = "Does not leak critical resources")]
            [SecuritySafeCritical]
            get
            {
                if (!checkedForFullTrust)
                {
                    inFullTrust = AppDomain.CurrentDomain.IsFullyTrusted;
                    checkedForFullTrust = true;
                }

                return inFullTrust;
            }
        }
    }
}
