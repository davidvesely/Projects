// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http;
    using Microsoft.Server.Common;

    internal class ProgressWriteAsyncResult : AsyncResult
    {
        static private AsyncCallback writeCompletedCallback = new AsyncCallback(WriteCompletedCallback);
        private Stream innerStream;
        private ProgressStream progressStream;
        private HttpRequestMessage request;
        private int count;

        public ProgressWriteAsyncResult(Stream innerStream, ProgressStream progressStream, HttpRequestMessage request, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            : base(callback, state)
        {
            Contract.Assert(innerStream != null, "stream cannot be null");
            Contract.Assert(progressStream != null, "progressStream cannot be null");
            Contract.Assert(request != null, "request cannot be null");

            this.innerStream = innerStream;
            this.progressStream = progressStream;
            this.request = request;
            this.count = count;

            try
            {
                IAsyncResult result = innerStream.BeginWrite(buffer, offset, count, writeCompletedCallback, this);
                if (result.CompletedSynchronously)
                {
                    this.WriteCompleted(result);
                }
            }
            catch (Exception e)
            {
                this.Complete(true, e);
            }
        }

        private static void WriteCompletedCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            ProgressWriteAsyncResult thisPtr = (ProgressWriteAsyncResult)result.AsyncState;
            try
            {
                thisPtr.WriteCompleted(result);
            }
            catch (Exception e)
            {
                thisPtr.Complete(false, e);
            }
        }

        private void WriteCompleted(IAsyncResult result)
        {
            this.innerStream.EndWrite(result);
            this.progressStream.ReportBytesSent(this.count);
            this.Complete(result.CompletedSynchronously);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ProgressWriteAsyncResult>(result);
        }
    }
}