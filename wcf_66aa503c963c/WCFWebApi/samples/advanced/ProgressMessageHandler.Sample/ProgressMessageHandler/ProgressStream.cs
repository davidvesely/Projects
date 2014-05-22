// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http;

    internal class ProgressStream : Stream
    {
        private Stream innerStream;
        private ProgressMessageHandler handler;
        private HttpRequestMessage request;

        private long bytesReceived;
        private long? totalBytesToReceive;
        private long bytesSent;
        private long? totalBytesToSend;

        public ProgressStream(Stream innerStream, ProgressMessageHandler handler, HttpRequestMessage request)
            : this(innerStream, handler, request, null)
        {
        }

        public ProgressStream(Stream innerStream, ProgressMessageHandler handler, HttpRequestMessage request, HttpResponseMessage response)
        {
            Contract.Assert(innerStream != null, "inner stream cannot be null");
            Contract.Assert(handler != null, "handler cannot be null");
            Contract.Assert(request != null, "request cannot be null");

            if (request.Content != null)
            {
                this.totalBytesToSend = request.Content.Headers.ContentLength;
            }

            if (response != null && response.Content != null)
            {
                this.totalBytesToReceive = response.Content.Headers.ContentLength;
            }

            this.innerStream = innerStream;
            this.handler = handler;
            this.request = request;
        }

        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.innerStream.CanWrite; }
        }

        public override void Flush()
        {
            this.innerStream.Flush();
        }

        public override long Length
        {
            get { return this.innerStream.Length; }
        }

        public override long Position
        {
            get { return this.innerStream.Position; }
            set { this.innerStream.Position = value; }
        }

        public override int ReadTimeout
        {
            get { return this.innerStream.ReadTimeout; }
            set { this.innerStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return this.innerStream.WriteTimeout; }
            set { this.innerStream.WriteTimeout = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = this.innerStream.Read(buffer, offset, count);
            if (bytesRead > 0)
            {
                this.ReportBytesReceived(bytesRead);
            }

            return bytesRead;
        }

        public override int ReadByte()
        {
            int byteRead = this.innerStream.ReadByte();
            if (byteRead != -1)
            {
                this.ReportBytesReceived(1);
            }

            return byteRead;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int bytesRead = this.innerStream.EndRead(asyncResult);
            this.ReportBytesReceived(bytesRead);
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
            this.ReportBytesSent(count);
        }

        public override void WriteByte(byte value)
        {
            this.innerStream.WriteByte(value);
            this.ReportBytesSent(1);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return new ProgressWriteAsyncResult(this.innerStream, this, this.request, buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            ProgressWriteAsyncResult.End(asyncResult);
        }

        public override void Close()
        {
            this.innerStream.Close();
        }

        public override bool CanTimeout
        {
            get { return this.innerStream.CanTimeout; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerStream.Close();
            }
        }

        internal void ReportBytesSent(int bytesSent)
        {
            if (bytesSent > 0)
            {
                this.bytesSent += bytesSent;
                int percentage = 0;
                if (this.totalBytesToSend.HasValue)
                {
                    percentage = (int)((100L * this.bytesSent) / this.totalBytesToSend.Value);
                }

                this.handler.OnHttpUploadProgress(this.request, new HttpProgressEventArgs(percentage, null, this.bytesSent, this.totalBytesToSend.HasValue ? this.totalBytesToSend.Value : -1));
            }
        }

        private void ReportBytesReceived(int bytesReceived)
        {
            if (bytesReceived > 0)
            {
                this.bytesReceived += bytesReceived;
                int percentage = 0;
                if (this.totalBytesToReceive.HasValue)
                {
                    percentage = (int)((100L * this.bytesReceived) / this.totalBytesToReceive.Value);
                }

                this.handler.OnHttpDownloadProgress(this.request, new HttpProgressEventArgs(percentage, null, this.bytesReceived, this.totalBytesToReceive.HasValue ? this.totalBytesToReceive.Value : -1));
            }
        }
    }
}