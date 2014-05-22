//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InternalException : SystemException
    {
        public InternalException()
            : base()
        {
        }

        public InternalException(string description)
            : base(SR.ShipAssertExceptionMessage(description))
        {
        }

        public InternalException(string description, Exception exception)
            : base(SR.ShipAssertExceptionMessage(description), exception)
        {
        }

        protected InternalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
