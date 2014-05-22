using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public abstract class HttpContent : IDisposable
    {
        private HttpContentHeaders headers;
        private MemoryStream bufferedContent;
        private bool disposed;
        private Stream contentReadStream;
        private bool canCalculateLength;

        internal const int DefaultMaxBufferSize = 65536; // 64KB
        internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;

        public HttpContentHeaders Headers
        {
            get
            {
                if (headers == null)
                {
                    headers = new HttpContentHeaders(GetComputedOrBufferLength);
                }
                return headers;
            }
        }

        private bool IsBuffered
        {
            get { return bufferedContent != null; }
        }

        protected HttpContent()
        {
            // Log to get an ID for the current content. This ID is used when the content gets associated to a message.
            if (Logging.On) Logging.Enter(Logging.Http, this, ".ctor", null);

            // We start with the assumption that we can calculate the content length.
            this.canCalculateLength = true;

            if (Logging.On) Logging.Exit(Logging.Http, this, ".ctor", null);
        }

        public Task<string> ReadAsStringAsync()
        {
            CheckDisposed();

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            LoadIntoBufferAsync().ContinueWith(task =>
            {
                if (HttpUtilities.HandleFaultsAndCancelation(task, tcs))
                {
                    return;
                }

                if (bufferedContent.Length == 0)
                {
                    tcs.TrySetResult(string.Empty);
                    return;
                }

                // We don't validate the Content-Encoding header: If the content was encoded, it's the caller's 
                // responsibility to make sure to only call ReadAsString() on already decoded content. E.g. if the 
                // Content-Encoding is 'gzip' the user should set WebRequestChannel.AutomaticDecompression to get a 
                // decoded response stream.

                // If we do have encoding information in the 'Content-Type' header, use that information to convert
                // the content to a string.
                Encoding encoding = DefaultStringEncoding;
                if ((Headers.ContentType != null) && (Headers.ContentType.CharSet != null))
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(Headers.ContentType.CharSet);
                    }
                    catch (ArgumentException e)
                    {
                        tcs.TrySetException(new InvalidOperationException(SR.net_http_content_invalid_charset, e));
                        return;
                    }
                }

                try
                {
                    string result = encoding.GetString(bufferedContent.GetBuffer(), 0, (int)bufferedContent.Length);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public Task<byte[]> ReadAsByteArrayAsync()
        {
            CheckDisposed();

            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();

            LoadIntoBufferAsync().ContinueWith(task =>
            {
                if (!HttpUtilities.HandleFaultsAndCancelation(task, tcs))
                {
                    tcs.TrySetResult(bufferedContent.ToArray());
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public Task<Stream> ReadAsStreamAsync()
        {
            CheckDisposed();

            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();

            if (contentReadStream == null && IsBuffered)
            {
                // We cast bufferedContent.Length to 'int': The framework doesn't support arrays > int.MaxValue,
                // so the length will always be in the 'int' range.
                contentReadStream = new MemoryStream(bufferedContent.GetBuffer(), 0,
                    (int)bufferedContent.Length, false, false);
            }

            if (contentReadStream != null)
            {
                tcs.TrySetResult(contentReadStream);
                return tcs.Task;
            }

            CreateContentReadStreamAsync().ContinueWith(task =>
            {
                if (!HttpUtilities.HandleFaultsAndCancelation(task, tcs))
                {
                    contentReadStream = task.Result;
                    tcs.TrySetResult(contentReadStream);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext context);

        public Task CopyToAsync(Stream stream, TransportContext context)
        {
            CheckDisposed();
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // The try..catch is used, since both FromAsync() and SerializeToStreamAsync() may throw: E.g. if a HWR
            // gets aborted after a request is complete, but before the response stream is read, trying to read from
            // the response stream will throw. Abort WebExceptions will be converted to IOExceptions by 
            // HttpClientHandler.WebExceptionWrapperStream.
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            try
            {
                Task task = null;
                if (IsBuffered)
                {
                    task = Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, bufferedContent.GetBuffer(), 0,
                        (int)bufferedContent.Length, null);
                }
                else
                {
                    task = SerializeToStreamAsync(stream, context);
                    CheckTaskNotNull(task);
                }

                // If the copy operation fails, wrap the exception in an HttpRequestException() if appropriate.
                task.ContinueWith(copyTask =>
                {
                    if (copyTask.IsFaulted)
                    {
                        tcs.TrySetException(GetStreamCopyException(copyTask.Exception.GetBaseException()));
                    }
                    else if (copyTask.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);

            }
            catch (IOException e)
            {
                tcs.TrySetException(GetStreamCopyException(e));
            }
            catch (ObjectDisposedException e)
            {
                tcs.TrySetException(GetStreamCopyException(e));
            }

            return tcs.Task;
        }

        public Task CopyToAsync(Stream stream)
        {
            return CopyToAsync(stream, null);
        }

        // Workaround for HttpWebRequest synchronous resubmit
        internal void CopyTo(Stream stream)
        {
            CopyToAsync(stream).Wait();
        }

        public Task LoadIntoBufferAsync()
        {
            return LoadIntoBufferAsync(DefaultMaxBufferSize);
        }

        // No "CancellationToken" parameter needed since canceling the CTS will close the connection, resulting
        // in an exception being thrown while we're buffering.
        // If buffering is used without a connection, it is supposed to be fast, thus no cancellation required.
        public Task LoadIntoBufferAsync(int maxBufferSize)
        {
            CheckDisposed();

            if (IsBuffered)
            {
                // If we already buffered the content, just return a completed task.
                return CreateCompletedTask();
            }

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            Exception error = null;
            MemoryStream tempBuffer = CreateMemoryStream(maxBufferSize, out error);

            if (tempBuffer == null)
            {
                // We don't throw in LoadIntoBufferAsync(): set the task as faulted and return the task.
                Contract.Assert(error != null);
                tcs.TrySetException(error);
            }
            else
            {
                // SerializeToStreamAsync() may throw, e.g. when trying to read from a ConnectStream where the HWR was
                // aborted. Make sure to catch these exceptions and let the task fail.
                try
                {
                    Task task = SerializeToStreamAsync(tempBuffer, null);
                    CheckTaskNotNull(task);

                    task.ContinueWith(copyTask =>
                    {
                        try
                        {
                            if (copyTask.IsFaulted)
                            {
                                tempBuffer.Dispose(); // Cleanup partially filled stream.
                                tcs.TrySetException(GetStreamCopyException(copyTask.Exception.GetBaseException()));
                                return;
                            }

                            if (copyTask.IsCanceled)
                            {
                                tempBuffer.Dispose(); // Cleanup partially filled stream.
                                tcs.TrySetCanceled();
                                return;
                            }

                            bufferedContent = tempBuffer;
                            tcs.TrySetResult(null);
                        }
                        catch (Exception e)
                        {
                            // Make sure we catch any exception, otherwise the task will catch it and throw in the finalizer.
                            tcs.TrySetException(e);
                            if (Logging.On) Logging.Exception(Logging.Http, this, "LoadIntoBufferAsync", e);
                        }

                        // Since we're not doing any CPU and/or I/O intensive operations, continue on the same thread.
                        // This results in better performance since the continuation task doesn't get scheduled by the
                        // scheduler and there are no context switches required.
                    }, TaskContinuationOptions.ExecuteSynchronously);

                }
                catch (IOException e)
                {
                    tcs.TrySetException(GetStreamCopyException(e));
                }
                catch (ObjectDisposedException e)
                {
                    tcs.TrySetException(GetStreamCopyException(e));
                }
            }

            return tcs.Task;
        }

        protected virtual Task<Stream> CreateContentReadStreamAsync()
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            // By default just buffer the content to a memory stream. Derived classes can override this behavior
            // if there is a better way to retrieve the content as stream (e.g. byte array/string use a more efficient
            // way, like wrapping a read-only MemoryStream around the bytes/string)
            LoadIntoBufferAsync().ContinueWith(task =>
            {
                if (!HttpUtilities.HandleFaultsAndCancelation(task, tcs))
                {
                    tcs.TrySetResult(bufferedContent);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        // Derived types return true if they're able to compute the length. It's OK if derived types return false to
        // indicate that they're not able to compute the length. The transport channel needs to decide what to do in
        // that case (send chunked, buffer first, etc.).
        protected internal abstract bool TryComputeLength(out long length);

        private long? GetComputedOrBufferLength()
        {
            CheckDisposed();

            if (IsBuffered)
            {
                return bufferedContent.Length;
            }

            // If we already tried to calculate the length, but the derived class returned 'false', then don't try
            // again; just return null.
            if (canCalculateLength)
            {
                long length = 0;
                if (TryComputeLength(out length))
                {
                    return length;
                }

                // Set flag to make sure next time we don't try to compute the length, since we know that we're unable
                // to do so.
                canCalculateLength = false;
            }
            return null;
        }

        private MemoryStream CreateMemoryStream(int maxBufferSize, out Exception error)
        {
            Contract.Ensures((Contract.Result<MemoryStream>() != null) ||
                (Contract.ValueAtReturn<Exception>(out error) != null));

            error = null;

            // If we have a Content-Length allocate the right amount of buffer up-front. Also check whether the
            // content length exceeds the max. buffer size.
            long? contentLength = Headers.ContentLength;

            if (contentLength != null)
            {
                Contract.Assert(contentLength >= 0);

                if (contentLength > maxBufferSize)
                {
                    error = new HttpRequestException(string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, maxBufferSize));
                    return null;
                }

                // We can safely cast contentLength to (int) since we just checked that it is <= maxBufferSize.
                return new LimitMemoryStream(maxBufferSize, (int)contentLength);
            }

            // We couldn't determine the length of the buffer. Create a memory stream with an empty buffer.
            return new LimitMemoryStream(maxBufferSize, 0);
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;

                if (contentReadStream != null)
                {
                    contentReadStream.Dispose();
                }

                if (IsBuffered)
                {
                    bufferedContent.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Helpers

        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void CheckTaskNotNull(Task task)
        {
            if (task == null)
            {
                if (Logging.On) Logging.PrintError(Logging.Http, string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_log_content_no_task_returned_copytoasync, this.GetType().FullName));
                throw new InvalidOperationException(SR.net_http_content_no_task_returned);
            }
        }

        private static Task CreateCompletedTask()
        {
            TaskCompletionSource<object> completed = new TaskCompletionSource<object>();
            bool resultSet = completed.TrySetResult(null);
            Contract.Assert(resultSet, "Can't set Task as completed.");
            return completed.Task;
        }

        private static Exception GetStreamCopyException(Exception originalException)
        {
            // HttpContent derived types should throw HttpRequestExceptions if there is an error. However, since the stream
            // provided by CopyTo() can also throw, we wrap such exceptions in HttpRequestException. This way custom content
            // types don't have to worry about it. The goal is that users of HttpContent don't have to catch multiple
            // exceptions (depending on the underlying transport), but just HttpRequestExceptions (like HWR users just catch
            // WebException).
            // Custom stream should throw either IOException or HttpRequestException.
            // We don't want to wrap other exceptions thrown by Stream (e.g. InvalidOperationException), since we
            // don't want to hide such "usage error" exceptions in HttpRequestException.
            // ObjectDisposedException is also wrapped, since aborting HWR after a request is complete will result in
            // the response stream being closed.
            Exception result = originalException;
            if ((result is IOException) || (result is ObjectDisposedException))
            {
                result = new HttpRequestException(SR.net_http_content_stream_copy_error, result);
            }
            return result;
        }

        #endregion Helpers

        private class LimitMemoryStream : MemoryStream
        {
            private int maxSize;

            public LimitMemoryStream(int maxSize, int capacity)
                : base(capacity)
            {
                this.maxSize = maxSize;
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                CheckSize(count);
                return base.BeginWrite(buffer, offset, count, callback, state);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                CheckSize(count);
                base.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                CheckSize(1);
                base.WriteByte(value);
            }

            private void CheckSize(int countToAdd)
            {
                if (maxSize - Length < countToAdd)
                {
                    throw new HttpRequestException(string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, maxSize));
                }
            }
        }
    }
}
