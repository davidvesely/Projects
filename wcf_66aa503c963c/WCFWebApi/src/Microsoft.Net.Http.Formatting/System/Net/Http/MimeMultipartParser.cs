// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    /// Buffer-oriented MIME multipart parser.
    /// </summary>
    internal class MimeMultipartParser
    {
        internal const int MinMessageSize = 10;
        
        private static readonly ArraySegment<byte> EmptyBodyPart = new ArraySegment<byte>(new byte[0]);

        private const byte HTAB = 0x09;
        private const byte SP = 0x20;
        private const byte CR = 0x0D;
        private const byte LF = 0x0A;
        private const byte Dash = 0x2D;

        private long totalBytesConsumed;
        private long maxMessageSize;

        private BodyPartState bodyPartState;
        private string boundary;
        private CurrentBodyPartStore currentBoundary;

        /// <summary>
        /// Initializes a new instance of the <see cref="MimeMultipartParser"/> class.
        /// </summary>
        /// <param name="boundary">Message boundary</param>
        /// <param name="maxMessageSize">Maximum length of entire MIME multipart message.</param>
        public MimeMultipartParser(string boundary, long maxMessageSize)
        {
            // The minimum length which would be an empty message terminated by CRLF
            if (maxMessageSize < MimeMultipartParser.MinMessageSize)
            {
                throw new ArgumentException(SR.MinParameterSize(MimeMultipartParser.MinMessageSize), "maxMessageSize");
            }

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new ArgumentNullException("boundary");
            }

            if (boundary.EndsWith(" ", StringComparison.Ordinal))
            {
                throw new ArgumentException(SR.MimeMultipartParserBadBoundary, "boundary");
            }

            this.maxMessageSize = maxMessageSize;
            this.boundary = boundary;
            this.currentBoundary = new CurrentBodyPartStore(this.boundary);
            this.bodyPartState = BodyPartState.AfterFirstLineFeed;
        }

        /// <summary>
        /// Represents the overall state of the <see cref="MimeMultipartParser"/>.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Need more data
            /// </summary>
            NeedMoreData = 0,

            /// <summary>
            /// Parsing of a complete body part succeeded.
            /// </summary>
            BodyPartCompleted,

            /// <summary>
            /// Bad data format
            /// </summary>
            Invalid,

            /// <summary>
            /// Data exceeds the allowed size
            /// </summary>
            DataTooBig,
        }

        private enum MessageState
        {
            Boundary = 0,           // about to parse boundary
            BodyPart,               // about to parse body-part
            CloseDelimiter          // about to read close-delimiter
        }

        private enum BodyPartState
        {
            BodyPart = 0,
            AfterFirstCarriageReturn,
            AfterFirstLineFeed,
            AfterFirstDash,
            Boundary,
            AfterSecondCarriageReturn
        }

        /// <summary>
        /// Parse a MIME multipart message. Bytes are parsed in a consuming
        /// manner from the beginning of the request buffer meaning that the same bytes can not be 
        /// present in the request buffer.
        /// </summary>
        /// <param name="buffer">Request buffer from where request is read</param>
        /// <param name="bytesReady">Size of request buffer</param>
        /// <param name="bytesConsumed">Offset into request buffer</param>
        /// <param name="remainingBodyPart">Any body part that was considered as a potential MIME multipart boundary but which was in fact part of the body.</param>
        /// <param name="bodyPart">The bulk of the body part.</param>
        /// <param name="isFinalBodyPart">Indicates whether the final body part has been found.</param>
        /// <remarks>In order to get the complete body part, the caller is responsible for concatenating the contents of the 
        /// <paramref name="remainingBodyPart"/> and <paramref name="bodyPart"/> out parameters.</remarks>
        /// <returns>State of the parser.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is translated to parse state.")]
        public State ParseBuffer(
            byte[] buffer,
            int bytesReady,
            ref int bytesConsumed,
            out ArraySegment<byte> remainingBodyPart,
            out ArraySegment<byte> bodyPart,
            out bool isFinalBodyPart)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            State parseStatus = State.NeedMoreData;
            remainingBodyPart = MimeMultipartParser.EmptyBodyPart;
            bodyPart = MimeMultipartParser.EmptyBodyPart;
            isFinalBodyPart = false;

            if (bytesConsumed >= bytesReady)
            {
                // we already can tell we need more data
                return parseStatus;
            }

            try
            {
                parseStatus = MimeMultipartParser.ParseBodyPart(
                    buffer,
                    bytesReady,
                    ref bytesConsumed,
                    ref this.bodyPartState,
                    this.maxMessageSize,
                    ref this.totalBytesConsumed,
                    this.currentBoundary);
            }
            catch (Exception)
            {
                parseStatus = State.Invalid;
            }

            remainingBodyPart = this.currentBoundary.GetDiscardedBoundary();
            bodyPart = this.currentBoundary.BodyPart;
            if (parseStatus == State.BodyPartCompleted)
            {
                isFinalBodyPart = this.currentBoundary.IsFinal;
                this.currentBoundary.ClearAll();
            }
            else
            {
                this.currentBoundary.ClearBodyPart();
            }

            return parseStatus;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is a parser which cannot be split up for performance reasons.")]
        private static unsafe State ParseBodyPart(
            byte[] buffer,
            int bytesReady,
            ref int bytesConsumed,
            ref BodyPartState bodyPartState,
            long maximumMessageLength,
            ref long totalBytesConsumed,
            CurrentBodyPartStore currentBodyPart)
        {
            Contract.Assert((bytesReady - bytesConsumed) >= 0, "ParseBodyPart()|(bytesReady - bytesConsumed) < 0");
            Contract.Assert(maximumMessageLength <= 0 || totalBytesConsumed <= maximumMessageLength, "ParseBodyPart()|Message already read exceeds limit.");

            // Remember where we started.
            int segmentStart;            
            int initialBytesParsed = bytesConsumed;

            // Set up parsing status with what will happen if we exceed the buffer.
            State parseStatus = State.DataTooBig;
            long effectiveMax = maximumMessageLength <= 0 ? long.MaxValue : (maximumMessageLength - totalBytesConsumed + bytesConsumed);
            if (bytesReady < effectiveMax)
            {
                parseStatus = State.NeedMoreData;
                effectiveMax = bytesReady;
            }

            currentBodyPart.ResetBoundaryOffset();

            Contract.Assert(bytesConsumed < effectiveMax, "We have already consumed more than the max header length.");

            fixed (byte* inputPtr = buffer)
            {
                switch (bodyPartState)
                {
                    case BodyPartState.BodyPart:
                        while (inputPtr[bytesConsumed] != MimeMultipartParser.CR)
                        {
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.CR);

                        // Move past the CR
                        bodyPartState = BodyPartState.AfterFirstCarriageReturn;
                        if (++bytesConsumed == effectiveMax)
                        {
                            goto quit;
                        }

                        goto case BodyPartState.AfterFirstCarriageReturn;

                    case BodyPartState.AfterFirstCarriageReturn:
                        if (inputPtr[bytesConsumed] != MimeMultipartParser.LF)
                        {
                            currentBodyPart.ResetBoundary();
                            bodyPartState = BodyPartState.BodyPart;
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.BodyPart;
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.LF);

                        // Move past the CR
                        bodyPartState = BodyPartState.AfterFirstLineFeed;
                        if (++bytesConsumed == effectiveMax)
                        {
                            goto quit;
                        }

                        goto case BodyPartState.AfterFirstLineFeed;

                    case BodyPartState.AfterFirstLineFeed:
                        if (inputPtr[bytesConsumed] == MimeMultipartParser.CR)
                        {
                            // Remember potential boundary
                            currentBodyPart.ResetBoundary();
                            currentBodyPart.AppendBoundary(MimeMultipartParser.CR);

                            // Move past the CR
                            bodyPartState = BodyPartState.AfterFirstCarriageReturn;
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.AfterFirstCarriageReturn;
                        }

                        if (inputPtr[bytesConsumed] != MimeMultipartParser.Dash)
                        {
                            currentBodyPart.ResetBoundary();
                            bodyPartState = BodyPartState.BodyPart;
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.BodyPart;
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.Dash);

                        // Move past the Dash
                        bodyPartState = BodyPartState.AfterFirstDash;
                        if (++bytesConsumed == effectiveMax)
                        {
                            goto quit;
                        }

                        goto case BodyPartState.AfterFirstDash;

                    case BodyPartState.AfterFirstDash:
                        if (inputPtr[bytesConsumed] != MimeMultipartParser.Dash)
                        {
                            currentBodyPart.ResetBoundary();
                            bodyPartState = BodyPartState.BodyPart;
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.BodyPart;
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.Dash);

                        // Move past the Dash
                        bodyPartState = BodyPartState.Boundary;
                        if (++bytesConsumed == effectiveMax)
                        {
                            goto quit;
                        }

                        goto case BodyPartState.Boundary;

                    case BodyPartState.Boundary:
                        segmentStart = bytesConsumed;
                        while (inputPtr[bytesConsumed] != MimeMultipartParser.CR)
                        {
                            if (++bytesConsumed == effectiveMax)
                            {
                                currentBodyPart.AppendBoundary(buffer, segmentStart, bytesConsumed - segmentStart);
                                goto quit;
                            }
                        }

                        if (bytesConsumed > segmentStart)
                        {
                            currentBodyPart.AppendBoundary(buffer, segmentStart, bytesConsumed - segmentStart);
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.CR);

                        // Move past the CR
                        bodyPartState = BodyPartState.AfterSecondCarriageReturn;
                        if (++bytesConsumed == effectiveMax)
                        {
                            goto quit;
                        }

                        goto case BodyPartState.AfterSecondCarriageReturn;

                    case BodyPartState.AfterSecondCarriageReturn:
                        if (inputPtr[bytesConsumed] != MimeMultipartParser.LF)
                        {
                            currentBodyPart.ResetBoundary();
                            bodyPartState = BodyPartState.BodyPart;
                            if (++bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.BodyPart;
                        }

                        // Remember potential boundary
                        currentBodyPart.AppendBoundary(MimeMultipartParser.LF);

                        // Move past the LF
                        bytesConsumed++;

                        bodyPartState = BodyPartState.BodyPart;
                        if (currentBodyPart.IsBoundaryValid())
                        {
                            parseStatus = State.BodyPartCompleted;
                        }
                        else
                        {
                            currentBodyPart.ResetBoundary();
                            if (bytesConsumed == effectiveMax)
                            {
                                goto quit;
                            }

                            goto case BodyPartState.BodyPart;
                        }

                        goto quit;
                }
            }

        quit:
            if (initialBytesParsed < bytesConsumed)
            {
                int boundaryLength = currentBodyPart.BoundaryDelta;
                if (boundaryLength > 0 && parseStatus != State.BodyPartCompleted)
                {
                    currentBodyPart.HasPotentialBoundaryLeftOver = true;
                }

                int bodyPartEnd = bytesConsumed - initialBytesParsed - boundaryLength;

                currentBodyPart.BodyPart = new ArraySegment<byte>(buffer, initialBytesParsed, bodyPartEnd);
            }

            totalBytesConsumed += bytesConsumed - initialBytesParsed;
            return parseStatus;
        }

        /// <summary>
        /// Maintains information about the current body part being parsed.
        /// </summary>
        private class CurrentBodyPartStore
        {
            private const int MaxBoundarySize = 256;
            private const int InitialOffset = 2;

            private byte[] boundaryStore = new byte[CurrentBodyPartStore.MaxBoundarySize];
            private int boundaryStoreLength;

            private byte[] referenceBoundary = new byte[CurrentBodyPartStore.MaxBoundarySize];
            private int referenceBoundaryLength;

            private byte[] boundary = new byte[CurrentBodyPartStore.MaxBoundarySize];
            private int boundaryLength = 0;

            private ArraySegment<byte> bodyPart = MimeMultipartParser.EmptyBodyPart;
            private bool isFinal;
            private bool isFirst = true;
            private bool releaseDiscardedBoundary;
            private int boundaryOffset;

            /// <summary>
            /// Initializes a new instance of the <see cref="CurrentBodyPartStore"/> class.
            /// </summary>
            /// <param name="referenceBoundary">The reference boundary.</param>
            public CurrentBodyPartStore(string referenceBoundary)
            {
                this.referenceBoundary[0] = MimeMultipartParser.CR;
                this.referenceBoundary[1] = MimeMultipartParser.LF;
                this.referenceBoundary[2] = MimeMultipartParser.Dash;
                this.referenceBoundary[3] = MimeMultipartParser.Dash;
                this.referenceBoundaryLength = 4 + Encoding.UTF8.GetBytes(referenceBoundary, 0, referenceBoundary.Length, this.referenceBoundary, 4);

                this.boundary[0] = MimeMultipartParser.CR;
                this.boundary[1] = MimeMultipartParser.LF;
                this.boundaryLength = CurrentBodyPartStore.InitialOffset;
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance has potential boundary left over.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has potential boundary left over; otherwise, <c>false</c>.
            /// </value>
            public bool HasPotentialBoundaryLeftOver { get; set; }

            /// <summary>
            /// Gets the boundary delta.
            /// </summary>
            public int BoundaryDelta
            {
                get
                {
                    return (this.boundaryLength - this.boundaryOffset > 0) ? this.boundaryLength - this.boundaryOffset : this.boundaryLength;
                }
            }

            /// <summary>
            /// Gets or sets the body part.
            /// </summary>
            /// <value>
            /// The body part.
            /// </value>
            public ArraySegment<byte> BodyPart
            {
                get
                {
                    return this.bodyPart;
                }

                set
                {
                    this.bodyPart = value;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this body part instance is final.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this body part instance is final; otherwise, <c>false</c>.
            /// </value>
            public bool IsFinal
            {
                get
                {
                    return this.isFinal;
                }
            }

            /// <summary>
            /// Resets the boundary offset.
            /// </summary>
            public void ResetBoundaryOffset()
            {
                this.boundaryOffset = this.boundaryLength;
            }

            /// <summary>
            /// Resets the boundary.
            /// </summary>
            public void ResetBoundary()
            {
                // If we had a potential boundary left over then store it so that we don't loose it
                if (this.HasPotentialBoundaryLeftOver)
                {
                    Buffer.BlockCopy(this.boundary, 0, this.boundaryStore, 0, this.boundaryOffset);
                    this.boundaryStoreLength = this.boundaryOffset;
                    this.HasPotentialBoundaryLeftOver = false;
                    this.releaseDiscardedBoundary = true;
                }

                this.boundaryLength = 0;
                this.boundaryOffset = 0;
            }

            /// <summary>
            /// Appends byte to the current boundary.
            /// </summary>
            /// <param name="data">The data to append to the boundary.</param>
            public void AppendBoundary(byte data)
            {
                this.boundary[this.boundaryLength++] = data;
            }

            /// <summary>
            /// Appends array of bytes to the current boundary.
            /// </summary>
            /// <param name="data">The data to append to the boundary.</param>
            /// <param name="offset">The offset into the data.</param>
            /// <param name="count">The number of bytes to append.</param>
            public void AppendBoundary(byte[] data, int offset, int count)
            {
                Buffer.BlockCopy(data, offset, this.boundary, this.boundaryLength, count);
                this.boundaryLength += count;
            }

            /// <summary>
            /// Gets the discarded boundary.
            /// </summary>
            /// <returns>An <see cref="ArraySegment{T}"/> containing the discarded boundary.</returns>
            public ArraySegment<byte> GetDiscardedBoundary()
            {
                if (this.boundaryStoreLength > 0 && this.releaseDiscardedBoundary)
                {
                    ArraySegment<byte> discarded = new ArraySegment<byte>(this.boundaryStore, 0, this.boundaryStoreLength);
                    this.boundaryStoreLength = 0;
                    return discarded;
                }

                return MimeMultipartParser.EmptyBodyPart;
            }

            /// <summary>
            /// Determines whether current boundary is valid.
            /// </summary>
            /// <returns>
            ///   <c>true</c> if curent boundary is valid; otherwise, <c>false</c>.
            /// </returns>
            public bool IsBoundaryValid()
            {
                int offset = 0;
                if (this.isFirst)
                {
                    offset = CurrentBodyPartStore.InitialOffset;
                }

                int cnt = offset;
                for (; cnt < this.referenceBoundaryLength; cnt++)
                {
                    if (this.boundary[cnt] != this.referenceBoundary[cnt])
                    {
                        return false;
                    }
                }

                // Check for final
                bool boundaryIsFinal = false;
                if (this.boundary[cnt] == MimeMultipartParser.Dash &&
                    this.boundary[cnt + 1] == MimeMultipartParser.Dash)
                {
                    boundaryIsFinal = true;
                    cnt += 2;
                }

                // Rest of boundary must LWS in order for it to match
                for (; cnt < this.boundaryLength - 2; cnt++)
                {
                    if (this.boundary[cnt] != MimeMultipartParser.SP && this.boundary[cnt] != MimeMultipartParser.HTAB)
                    {
                        return false;
                    }
                }

                // We have a valid boundary so whatever we stored in the boundary story is no longer needed
                this.isFinal = boundaryIsFinal;
                this.isFirst = false;

                return true;
            }

            /// <summary>
            /// Clears the body part.
            /// </summary>
            public void ClearBodyPart()
            {
                this.BodyPart = MimeMultipartParser.EmptyBodyPart;
            }

            /// <summary>
            /// Clears all.
            /// </summary>
            public void ClearAll()
            {
                this.releaseDiscardedBoundary = false;
                this.HasPotentialBoundaryLeftOver = false;
                this.boundaryLength = 0;
                this.boundaryOffset = 0;
                this.boundaryStoreLength = 0;
                this.isFinal = false;
                this.ClearBodyPart();
            }
        }
    }
}