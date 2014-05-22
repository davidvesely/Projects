// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System.Threading;

    public class Counter
    {
        int count;

        public void IncreaseCount()
        {
            Interlocked.Increment(ref count);
        }

        public int GetCount()
        {
            return count;
        }
    }
}
