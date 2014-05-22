//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security;
    using Microsoft.Server.Common;

    [Fx.Tag.SecurityNote(Critical = "Class holds static instances used for code generation during serialization."
        + " Static fields are marked SecurityCritical or readonly to prevent data from being modified or leaked to other components in appdomain.",
        Safe = "All get-only properties marked safe since they only need to be protected for write.")]
    static class XmlFormatGeneratorStatics
    {

        [SecurityCritical]
        static MethodInfo extensionDataSetExplicitMethodInfo;
        internal static MethodInfo ExtensionDataSetExplicitMethodInfo
        {
            [SecuritySafeCritical]
            get
            {
                if (extensionDataSetExplicitMethodInfo == null)
                    extensionDataSetExplicitMethodInfo = typeof(IExtensibleDataObject).GetMethod(Globals.ExtensionDataSetMethod);
                return extensionDataSetExplicitMethodInfo;
            }
        }

        [SecurityCritical]
        static ConstructorInfo dictionaryEnumeratorCtor;
        internal static ConstructorInfo DictionaryEnumeratorCtor
        {
            [SecuritySafeCritical]
            get
            {
                if (dictionaryEnumeratorCtor == null)
                    dictionaryEnumeratorCtor = Globals.TypeOfDictionaryEnumerator.GetConstructor(Globals.ScanAllMembers, null, new Type[] { Globals.TypeOfIDictionaryEnumerator }, null);
                return dictionaryEnumeratorCtor;
            }
        }

        [SecurityCritical]
        static MethodInfo ienumeratorGetCurrentMethod;
        internal static MethodInfo GetCurrentMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (ienumeratorGetCurrentMethod == null)
                    ienumeratorGetCurrentMethod = typeof(IEnumerator).GetProperty("Current").GetGetMethod();
                return ienumeratorGetCurrentMethod;
            }
        }

        [SecurityCritical]
        static MethodInfo ienumeratorMoveNextMethod;
        internal static MethodInfo MoveNextMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (ienumeratorMoveNextMethod == null)
                    ienumeratorMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
                return ienumeratorMoveNextMethod;
            }
        }

        [SecurityCritical]
        static MethodInfo throwRequiredMemberMustBeEmittedMethod;
        internal static MethodInfo ThrowRequiredMemberMustBeEmittedMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (throwRequiredMemberMustBeEmittedMethod == null)
                    throwRequiredMemberMustBeEmittedMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("ThrowRequiredMemberMustBeEmitted", Globals.ScanAllMembers);
                return throwRequiredMemberMustBeEmittedMethod;
            }
        }

        [SecurityCritical]
        static MethodInfo traceInstructionMethod;
        internal static MethodInfo TraceInstructionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (traceInstructionMethod == null)
                    traceInstructionMethod = typeof(SerializationTrace).GetMethod("TraceInstruction", Globals.ScanAllMembers);
                return traceInstructionMethod;
            }
        }
    }
}
