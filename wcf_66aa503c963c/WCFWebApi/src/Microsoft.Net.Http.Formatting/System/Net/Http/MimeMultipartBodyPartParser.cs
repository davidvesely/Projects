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
    using System.Net.Http.Headers;

    /// <summary>
    /// Complete MIME multipart parser that combines <see cref="MimeMultipartParser"/> for parsing the MIME message into individual body parts 
    /// and <see cref="InternetMessageFormatHeaderParser"/> for parsing each body part into a MIME header and a MIME body. The caller of the parser is returned
    /// the resulting MIME bodies which can then be written to some output.
    /// </summary>
    internal class MimeMultipartBodyPartParser : IDisposable
    {
        private const long DefaultMaxMessageSize = long.MaxValue;
        private const int DefaultMaxBodyPartHeaderSize = 4 * 1024;

        // MIME parser
        private MimeMultipartParser mimeParser;
        private MimeMultipartParser.State mimeStatus = MimeMultipartParser.State.NeedMoreData;
        private ArraySegment<byte>[] parsedBodyPart = new ArraySegment<byte>[2];
        private MimeBodyPart currentBodyPart;
        private bool isFirst = true;

        // Header field parser
        private ParserState bodyPartHeaderStatus = ParserState.NeedMoreData;
        private int maxBodyPartHeaderSize;

        // Stream provider
        private IMultipartStreamProvider streamProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MimeMultipartBodyPartParser"/> class.
        /// </summary>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        /// <param name="streamProvider">A stream provider providing output streams for where to write body parts as they are parsed.</param>
        public MimeMultipartBodyPartParser(HttpContent content, IMultipartStreamProvider streamProvider)
            : this(content, streamProvider, DefaultMaxMessageSize, DefaultMaxBodyPartHeaderSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MimeMultipartBodyPartParser"/> class.
        /// </summary>
        /// <param name="content">An existing <see cref="HttpContent"/> instance to use for the object's content.</param>
        /// <param name="streamProvider">A stream provider providing output streams for where to write body parts as they are parsed.</param>
        /// <param name="maxMessageSize">The max length of the entire MIME multipart message.</param>
        /// <param name="maxBodyPartHeaderSize">The max length of the MIME header within each MIME body part.</param>
        public MimeMultipartBodyPartParser(
            HttpContent content,
            IMultipartStreamProvider streamProvider, 
            long maxMessageSize, 
            int maxBodyPartHeaderSize)
        {
            Contract.Assert(content != null, "content cannot be null.");
            Contract.Assert(streamProvider != null, "streamProvider cannot be null.");

            string boundary = ValidateArguments(content, maxMessageSize, true);

            this.mimeParser = new MimeMultipartParser(boundary, maxMessageSize);
            this.currentBodyPart = new MimeBodyPart(streamProvider, maxBodyPartHeaderSize);

            this.maxBodyPartHeaderSize = maxBodyPartHeaderSize;

            this.streamProvider = streamProvider;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MimeMultipartBodyPartParser"/> is reclaimed by garbage collection.
        /// </summary>
        ~MimeMultipartBodyPartParser()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Determines whether the specified content is MIME multipart content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>
        ///   <c>true</c> if the specified content is MIME multipart content; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is translated to false return.")]
        public static bool IsMimeMultipartContent(HttpContent content)
        {
            Contract.Assert(content != null, "content cannot be null.");
            try
            {
                string boundary = ValidateArguments(content, DefaultMaxMessageSize, false);
                return boundary != null ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Parses the data provided and generates parsed MIME body part bodies in the form of <see cref="ArraySegment{T}"/> which are ready to 
        /// write to the output stream.
        /// </summary>
        /// <param name="data">The data to parse</param>
        /// <param name="bytesRead">The number of bytes available in the input data</param>
        /// <returns>Parsed <see cref="MimeBodyPart"/> instances.</returns>
        public IEnumerable<MimeBodyPart> ParseBuffer(byte[] data, int bytesRead)
        {
            int bytesConsumed = 0;
            bool isFinal = false;

            if (bytesRead == 0)
            {
                this.CleanupCurrentBodyPart();
                throw new IOException(SR.ReadAsMimeMultipartUnexpectedTermination);
            }

            // Make sure we don't have old array segments hanging around.
            this.currentBodyPart.Segments.Clear();

            while (bytesConsumed < bytesRead)
            {
                this.mimeStatus = this.mimeParser.ParseBuffer(data, bytesRead, ref bytesConsumed, out this.parsedBodyPart[0], out this.parsedBodyPart[1], out isFinal);
                if (this.mimeStatus != MimeMultipartParser.State.BodyPartCompleted && this.mimeStatus != MimeMultipartParser.State.NeedMoreData)
                {
                    this.CleanupCurrentBodyPart();
                    throw new IOException(SR.ReadAsMimeMultipartParseError(bytesConsumed, data));
                }

                // First body is empty preamble which we just ignore
                if (this.isFirst)
                {
                    if (this.mimeStatus == MimeMultipartParser.State.BodyPartCompleted)
                    {
                        this.isFirst = false;
                    }

                    continue;
                }

                // Parse the two array segments containing parsed body parts that the MIME parser gave us
                foreach (ArraySegment<byte> part in this.parsedBodyPart)
                {
                    if (part.Count == 0)
                    {
                        continue;
                    }

                    if (this.bodyPartHeaderStatus != ParserState.Done)
                    {
                        int headerConsumed = part.Offset;
                        this.bodyPartHeaderStatus = this.currentBodyPart.HeaderParser.ParseBuffer(part.Array, part.Count + part.Offset, ref headerConsumed);
                        if (this.bodyPartHeaderStatus == ParserState.Done)
                        {
                            // Add the remainder as body part content
                            this.currentBodyPart.Segments.Add(new ArraySegment<byte>(part.Array, headerConsumed, part.Count + part.Offset - headerConsumed));
                        }
                        else if (this.bodyPartHeaderStatus != ParserState.NeedMoreData)
                        {
                            this.CleanupCurrentBodyPart();
                            throw new IOException(SR.ReadAsMimeMultipartHeaderParseError(headerConsumed, part.Array));
                        }
                    }
                    else
                    {
                        // Add the data as body part content
                        this.currentBodyPart.Segments.Add(part);
                    }
                }

                if (this.mimeStatus == MimeMultipartParser.State.BodyPartCompleted)
                {
                    // If body is completed then swap current body part
                    MimeBodyPart completed = this.currentBodyPart;
                    completed.IsComplete = true;
                    completed.IsFinal = isFinal;

                    this.currentBodyPart = new MimeBodyPart(this.streamProvider, this.maxBodyPartHeaderSize);
                    this.mimeStatus = MimeMultipartParser.State.NeedMoreData;
                    this.bodyPartHeaderStatus = ParserState.NeedMoreData;
                    yield return completed;
                }
                else
                {
                    // Otherwise return what we have 
                    yield return this.currentBodyPart;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.mimeParser = null;
                this.CleanupCurrentBodyPart();
            }
        }

        private static string ValidateArguments(HttpContent content, long maxMessageSize, bool throwOnError)
        {
            Contract.Assert(content != null, "content cannot be null.");
            if (maxMessageSize < MimeMultipartParser.MinMessageSize)
            {
                if (throwOnError)
                {
                    throw new ArgumentException(SR.MinParameterSize(MimeMultipartParser.MinMessageSize), "maxMessageSize");
                }
                else
                {
                    return null;
                }
            }

            MediaTypeHeaderValue contentType = content.Headers.ContentType;
            if (contentType == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentException(SR.ReadAsMimeMultipartArgumentNoContentType(typeof(HttpContent).Name, "multipart/"), "content");
                }
                else
                {
                    return null;
                }
            }

            if (!contentType.MediaType.StartsWith("multipart", StringComparison.OrdinalIgnoreCase))
            {
                if (throwOnError)
                {
                    throw new ArgumentException(SR.ReadAsMimeMultipartArgumentNoMultipart(typeof(HttpContent).Name, "multipart/"), "content");
                }
                else
                {
                    return null;
                }
            }

            string boundary = null;
            foreach (NameValueHeaderValue p in contentType.Parameters)
            {
                if (p.Name.Equals("boundary", StringComparison.OrdinalIgnoreCase))
                {
                    boundary = FormattingUtilities.UnquoteToken(p.Value);
                    break;
                }
            }

            if (boundary == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentException(SR.ReadAsMimeMultipartArgumentNoBoundary(typeof(HttpContent).Name, "multipart", "boundary"), "content");
                }
                else
                {
                    return null;
                }
            }

            return boundary;
        }

        private void CleanupCurrentBodyPart()
        {
            if (this.currentBodyPart != null)
            {
                this.currentBodyPart.Dispose();
                this.currentBodyPart = null;
            }
        }
    }
}