// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System.ComponentModel;

    /// <summary>
    /// Provides data for the <see cref="HttpProgressEventHandler"/>
    /// </summary>
    public class HttpProgressEventArgs : ProgressChangedEventArgs
    {
        internal HttpProgressEventArgs(int progressPercentage, object userToken, long bytesExchanged, long totalBytesExchanged)
            : base(progressPercentage, userToken)
        {
            this.BytesExchanged = bytesExchanged;
            this.TotalBytesExchanged = totalBytesExchanged;
        }

        /// <summary>
        /// Gets the bytes exchanged since the last event notification.
        /// </summary>
        public long BytesExchanged { get; private set; }

        /// <summary>
        /// Gets the expected total bytes exchanged as part of this <see cref="T:System.Net.Http.HttpRequestMessage"/> or
        /// <see cref="T:System.Net.Http.HttpResponseMessage"/>.
        /// </summary>
        public long TotalBytesExchanged { get; private set; }
    }
}
