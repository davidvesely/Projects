//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Server.Common
{

    public static class WaitCallbackActionItem
    {
        public static bool ShouldUseActivity { get; set; }

        public static bool EventTraceActivityEnabled 
        { 
            get 
            { 
                return Fx.Trace.IsEtwProviderEnabled; 
            } 
        }
    }
}
