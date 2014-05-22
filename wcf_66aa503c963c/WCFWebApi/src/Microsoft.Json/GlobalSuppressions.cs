// <copyright file="GlobalSuppressions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Json.SR.Culture", Justification = "Suppressing a generated file property")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Json.SR.#OperationNotSupportedOnJsonType(System.Object)", Justification = "Suppressing a generated file property")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Json.SR.#OperatorNotAllowedOnOperands(System.Object,System.Object,System.Object)", Justification = "Suppressing a generated file property")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Json.SR.#OperatorNotDefinedForJsonType(System.Object,System.Object)", Justification = "Suppressing a generated file property")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "These assemblies are delay-signed.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Classes are grouped logically for user clarity.", Scope = "namespace", Target = "System.Runtime.Serialization.Json")]
