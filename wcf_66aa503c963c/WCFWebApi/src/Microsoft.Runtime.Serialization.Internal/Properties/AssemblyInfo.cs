// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microsoft.Runtime.Serialization.Internal")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Microsoft.Runtime.Serialization.Internal")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("da1c6253-ee41-4815-9824-427b44c49204")]

// Friend assemblies - product
[assembly: InternalsVisibleTo("Microsoft.ApplicationServer.Http")]
[assembly: InternalsVisibleTo("Microsoft.ApplicationServer.ServiceModel.Tools")]
[assembly: InternalsVisibleTo("Microsoft.Net.Http.Formatting.OData")]

// Friend assemblies - CIT
[assembly: InternalsVisibleTo("Microsoft.Runtime.Serialization.Internal.Test.Unit")]

[assembly: NeutralResourcesLanguageAttribute("en-US")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "These assemblies are delay-signed.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Classes are grouped logically for user clarity.", Scope = "namespace", Target = "Microsoft.Runtime.Serialization")]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
