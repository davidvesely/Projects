//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
// ***NOTE*** If this code is changed, make corresponding changes in System.Runtime.Serialization.XmlObjectSerializerWriteContext also.

namespace Microsoft.Runtime.Serialization
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Server.Common;


#if USE_REFEMIT
    public class XmlObjectSerializerWriteContext //: XmlObjectSerializerContext
#else
    internal class XmlObjectSerializerWriteContext //: XmlObjectSerializerContext
#endif
   {
        public static void ThrowRequiredMemberMustBeEmitted(string memberName, Type type)
        {
            throw Fx.Exception.AsError(new SerializationException(SR.RequiredMemberMustBeEmitted(memberName, type.FullName)));
        }
   }
}

