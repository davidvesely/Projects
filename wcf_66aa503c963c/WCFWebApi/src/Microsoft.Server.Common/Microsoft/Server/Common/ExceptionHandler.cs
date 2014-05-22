//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;

    public abstract class ExceptionHandler
    {
        [Fx.Tag.SecurityNote(Miscellaneous = "Must not call into PT code as it is called within a CER.")]
        public abstract bool HandleException(Exception exception);
    }
}