// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Derived <see cref="HttpContent"/> class which can encapsulate an <see cref="HttpResponseMessage"/>
    /// or an <see cref="HttpRequestMessage"/> as an entity with media type "application/http".
    /// </summary>
    public class HttpMessageContent : HttpContent
    {
        private static readonly AsyncCallback onWriteComplete = new AsyncCallback(OnWriteComplete);

        private const string SP = " ";
        private const string CRLF = "\r\n";

        private const int DefaultHeaderAllocation = 2 * 1024;

        private const string DefaultMediaType = "application/http";

        private const string MsgTypeParameter = "msgtype";
        private const string DefaultRequestMsgType = "request";
        private const string DefaultResponseMsgType = "response";

        private const string DefaultRequestMediaType = DefaultMediaType + "; " + MsgTypeParameter + "=" + DefaultRequestMsgType;
        private const string DefaultResponseMediaType = DefaultMediaType + "; " + MsgTypeParameter + "=" + DefaultResponseMsgType;

        private bool contentConsumed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageContent"/> class encapsulating an
        /// <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpResponseMessage"/> instance to encapsulate.</param>
        public HttpMessageContent(HttpRequestMessage httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            this.HttpRequestMessage = httpRequest;
            this.Headers.ContentType = new MediaTypeHeaderValue(DefaultMediaType);
            this.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(MsgTypeParameter, DefaultRequestMsgType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageContent"/> class encapsulating an
        /// <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponseMessage"/> instance to encapsulate.</param>
        public HttpMessageContent(HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException("httpResponse");
            }

            this.HttpResponseMessage = httpResponse;
            this.Headers.ContentType = new MediaTypeHeaderValue(DefaultMediaType);
            this.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(MsgTypeParameter, DefaultResponseMsgType));
        }

        /// <summary>
        /// Gets the HTTP request message.
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; private set; }

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        /// <summary>
        /// Validates whether the content contains an HTTP Request or an HTTP Response.
        /// </summary>
        /// <param name="content">The content to validate.</param>
        /// <param name="isRequest">if set to <c>true</c> if the content is either an HTTP Request or an HTTP Response.</param>
        /// <param name="throwOnError">Indicates whether validation failure should result in an <see cref="Exception"/> or not.</param>
        /// <returns><c>true</c> if content is either an HTTP Request or an HTTP Response</returns>
        internal static bool ValidateHttpMessageContent(HttpContent content, bool isRequest, bool throwOnError)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            MediaTypeHeaderValue contentType = content.Headers.ContentType;
            if (contentType != null)
            {
                if (!contentType.MediaType.Equals(DefaultMediaType, StringComparison.OrdinalIgnoreCase))
                {
                    if (throwOnError)
                    {
                        throw new ArgumentException(
                            SR.HttpMessageInvalidMediaType(
                                FormattingUtilities.HttpContentType.Name,
                                isRequest ? DefaultRequestMediaType : DefaultResponseMediaType),
                            "content");
                    }
                    else
                    {
                        return false;
                    }
                }

                foreach (NameValueHeaderValue parameter in contentType.Parameters)
                {
                    if (parameter.Name.Equals(MsgTypeParameter, StringComparison.OrdinalIgnoreCase))
                    {
                        string msgType = FormattingUtilities.UnquoteToken(parameter.Value);
                        if (!msgType.Equals(isRequest ? DefaultRequestMsgType : DefaultResponseMsgType, StringComparison.OrdinalIgnoreCase))
                        {
                            if (throwOnError)
                            {
                                throw new ArgumentException(
                                    SR.HttpMessageInvalidMediaType(
                                        FormattingUtilities.HttpContentType.Name,
                                        isRequest ? DefaultRequestMediaType : DefaultResponseMediaType),
                                    "content");
                            }
                            else
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            if (throwOnError)
            {
                throw new ArgumentException(
                    SR.HttpMessageInvalidMediaType(
                        FormattingUtilities.HttpContentType.Name,
                        isRequest ? DefaultRequestMediaType : DefaultResponseMediaType),
                    "content");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously serializes the object's content to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="context">The associated <see cref="TransportContext"/>.</param>
        /// <returns>A <see cref="Task"/> instance that is asynchronously serializing the object's content.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);

            // Serialize header
            byte[] header = this.SerializeHeader();

            TaskCompletionSource<object> writeTask = new TaskCompletionSource<object>();
            try
            {
                // We don't use TaskFactory.FromAsync as it generates an FxCop CA908 error
                Tuple<HttpMessageContent, Stream, TaskCompletionSource<object>> state =
                    new Tuple<HttpMessageContent, Stream, TaskCompletionSource<object>>(this, stream, writeTask);
                IAsyncResult result = stream.BeginWrite(header, 0, header.Length, onWriteComplete, state);
                if (result.CompletedSynchronously)
                {
                    WriteComplete(result, this, stream, writeTask);
                }
            }
            catch (Exception e)
            {
                writeTask.TrySetException(e);
            }

            return writeTask.Task;
        }

        /// <summary>
        /// Computes the length of the stream if possible.
        /// </summary>
        /// <param name="length">The computed length of the stream.</param>
        /// <returns><c>true</c> if the length has been computed; otherwise <c>false</c>.</returns>
        protected override bool TryComputeLength(out long length)
        {
            long contentLength = 0;
            HttpContent content = this.HttpRequestMessage != null ? this.HttpRequestMessage.Content : this.HttpResponseMessage.Content;
            if (content != null)
            {
                Stream readStream = content.ReadAsStreamAsync().Result;
                if (readStream != null && readStream.CanSeek)
                {
                    contentLength = readStream.Length;
                }
                else
                {
                    length = -1;
                    return false;
                }
            }

            // We serialize header to a StringBuilder so that we can determine the length
            // following the pattern for HttpContent to try and determine the message length.
            // The perf overhead is no larger than for the other HttpContent implementations.
            byte[] header = this.SerializeHeader();
            length = header.Length + contentLength;
            return true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.HttpRequestMessage != null)
                {
                    this.HttpRequestMessage.Dispose();
                    this.HttpRequestMessage = null;
                }

                if (this.HttpResponseMessage != null)
                {
                    this.HttpResponseMessage.Dispose();
                    this.HttpResponseMessage = null;
                }
            }

            base.Dispose(disposing);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        private static void OnWriteComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            Tuple<HttpMessageContent, Stream, TaskCompletionSource<object>> state =
                (Tuple<HttpMessageContent, Stream, TaskCompletionSource<object>>)result.AsyncState;
            Contract.Assert(state != null, "state cannot be null");
            try
            {
                WriteComplete(result, state.Item1, state.Item2, state.Item3);
            }
            catch (Exception e)
            {
                state.Item3.TrySetException(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        private static void WriteComplete(IAsyncResult result, HttpMessageContent thisPtr, Stream stream, TaskCompletionSource<object> writeTask)
        {
            Contract.Assert(result != null, "result cannot be null");
            Contract.Assert(thisPtr != null, "thisPtr cannot be null");
            Contract.Assert(stream != null, "stream cannot be null");
            Contract.Assert(writeTask != null, "writeTask cannot be null");

            try
            {
                stream.EndWrite(result);
            }
            catch (Exception e)
            {
                writeTask.TrySetException(e);
            }

            HttpContent content = thisPtr.PrepareContent();
            if (content != null)
            {
                content.CopyToAsync(stream).ContinueWith(
                (contentTask) =>
                {
                    if (contentTask.IsCanceled)
                    {
                        writeTask.TrySetCanceled();
                    }
                    else if (contentTask.IsFaulted)
                    {
                        writeTask.TrySetException(contentTask.Exception);
                    }
                    else
                    {
                        writeTask.TrySetResult(null);
                    }
                });
            }
            else
            {
                writeTask.TrySetResult(null);
            }
        }

        /// <summary>
        /// Serializes the HTTP request line.
        /// </summary>
        /// <param name="message">Where to write the request line.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        private static void SerializeRequestLine(StringBuilder message, HttpRequestMessage httpRequest)
        {
            Contract.Assert(message != null, "message cannot be null");
            message.Append(httpRequest.Method + SP);
            message.Append(httpRequest.RequestUri.AbsolutePath + SP);
            message.Append(FormattingUtilities.HttpVersionToken + "/" + (httpRequest.Version != null ? httpRequest.Version.ToString(2) : "1.1") + CRLF);
            message.Append(FormattingUtilities.HttpHostHeader + ":" + SP + httpRequest.RequestUri.DnsSafeHost + CRLF);
        }

        /// <summary>
        /// Serializes the HTTP status line.
        /// </summary>
        /// <param name="message">Where to write the status line.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        private static void SerializeStatusLine(StringBuilder message, HttpResponseMessage httpResponse)
        {
            Contract.Assert(message != null, "message cannot be null");
            message.Append(FormattingUtilities.HttpVersionToken + "/" + (httpResponse.Version != null ? httpResponse.Version.ToString(2) : "1.1") + SP);
            message.Append((int)httpResponse.StatusCode + SP);
            message.Append(httpResponse.ReasonPhrase + CRLF);
        }

        /// <summary>
        /// Serializes the header fields.
        /// </summary>
        /// <param name="message">Where to write the status line.</param>
        /// <param name="headers">The headers to write.</param>
        private static void SerializeHeaderFields(StringBuilder message, HttpHeaders headers)
        {
            Contract.Assert(message != null, "message cannot be null");
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    message.Append(header.Key + ":" + SP);
                    bool first = true;
                    foreach (string value in header.Value)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            message.Append("," + SP);
                        }

                        message.Append(value);
                    }

                    message.Append(CRLF);
                }
            }
        }

        private HttpContent PrepareContent()
        {
            HttpContent content = this.HttpRequestMessage != null ? this.HttpRequestMessage.Content : this.HttpResponseMessage.Content;
            if (content == null)
            {
                return content;
            }

            // If the content needs to be written to a target stream a 2nd time, then the stream must support
            // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
            // stream (e.g. a NetworkStream).
            if (this.contentConsumed)
            {
                Stream readStream = content.ReadAsStreamAsync().Result;
                if (readStream != null && readStream.CanRead)
                {
                    readStream.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException(
                        SR.HttpMessageContentAlreadyRead(
                            FormattingUtilities.HttpContentType.Name,
                            this.HttpRequestMessage != null ?
                                FormattingUtilities.HttpRequestMessageType.Name :
                                FormattingUtilities.HttpResponseMessageType.Name));
                }
            }

            this.contentConsumed = true;
            return content;
        }

        private byte[] SerializeHeader()
        {
            StringBuilder message = new StringBuilder(DefaultHeaderAllocation);
            HttpHeaders headers = null;
            HttpContent content = null;
            if (this.HttpRequestMessage != null)
            {
                SerializeRequestLine(message, this.HttpRequestMessage);
                headers = this.HttpRequestMessage.Headers;
                content = this.HttpRequestMessage.Content;
            }
            else
            {
                SerializeStatusLine(message, this.HttpResponseMessage);
                headers = this.HttpResponseMessage.Headers;
                content = this.HttpResponseMessage.Content;
            }

            SerializeHeaderFields(message, headers);
            if (content != null)
            {
                ObjectContent objectContent = content as ObjectContent;
                if (objectContent != null)
                {
                    objectContent.DetermineWriteSerializerAndContentType();
                }

                SerializeHeaderFields(message, content.Headers);
            }

            message.Append(CRLF);
            return Encoding.UTF8.GetBytes(message.ToString());
        }
    }
}