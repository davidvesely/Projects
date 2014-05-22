// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test
{
    public class ReferenceDepthContext
    {
        int maxRefDepth;
        int currentRefDepth = -1;

        public ReferenceDepthContext(int maxRefDepth)
        {
            this.maxRefDepth = maxRefDepth;
        }

        public bool IncreamentCounter()
        {
            if (++currentRefDepth > this.maxRefDepth)
            {
                return false;
            }

            return true;
        }

        public void DecrementCounter()
        {
            --currentRefDepth;
        }
    }
}
