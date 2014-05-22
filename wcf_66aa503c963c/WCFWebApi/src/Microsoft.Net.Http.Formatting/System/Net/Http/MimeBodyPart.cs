// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http.Headers;

    /// <summary>
    /// Maintains information about MIME body parts parsed by <see cref="MimeMultipartBodyPartParser"/>.
    /// </summary>
    internal class MimeBodyPart : IDisposable
    {
        private static readonly Type streamType = typeof(Stream);
        private Stream outputStream;
        private IMultipartStreamProvider streamProvider;
        private HttpContentHeaders headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MimeBodyPart"/> class.
        /// </summary>
        /// <param name="streamProvider">The stream provider.</param>
        /// <param name="maxBodyPartHeaderSize">The max length of the MIME header within each MIME body part.</param>
        public MimeBodyPart(IMultipartStreamProvider streamProvider, int maxBodyPartHeaderSize)
        {
            Contract.Assert(streamProvider != null, "Stream provider cannot be null.");
            this.streamProvider = streamProvider;
            this.Segments = new ArrayList(2);
            this.headers = FormattingUtilities.CreateEmptyContentHeaders();
            this.HeaderParser = new InternetMessageFormatHeaderParser(this.headers, maxBodyPartHeaderSize);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MimeBodyPart"/> is reclaimed by garbage collection.
        /// </summary>
        ~MimeBodyPart()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the header parser.
        /// </summary>
        /// <value>
        /// The header parser.
        /// </value>
        public InternetMessageFormatHeaderParser HeaderParser { get; private set; }

        /// <summary>
        /// Gets the content of the HTTP.
        /// </summary>
        /// <value>
        /// The content of the HTTP.
        /// </value>
        public HttpContent HttpContent { get; private set; }

        /// <summary>
        /// Gets the set of <see cref="ArraySegment{T}"/> pointing to the read buffer with
        /// contents of this body part.
        /// </summary>
        /// <remarks>We deliberately use <see cref="ArrayList"/> as List{ArraySegment{byte}} and Collection{ArraySegment{byte}} trip FxCop rule CA908
        /// which states the following: "Assemblies that are precompiled (using ngen.exe) should only instantiate generic types that will not cause JIT 
        /// compilation at runtime. Generic types with value type type parameters (outside of a special set of supported runtime generic types) will 
        /// always cause JIT compilation, even if the encapsulating assembly has been precompiled". As we don't want to force JIT'ing we use the old
        /// non-generic form which doesn't have this problem (ArraySegment{byte} of course is a value type).</remarks>
        public ArrayList Segments { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the body part has been completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the final body part.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <returns>The output stream to write the body part to.</returns>
        public Stream GetOutputStream()
        {
            if (this.outputStream == null)
            {
                try
                {
                    this.outputStream = this.streamProvider.GetStream(this.headers);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(SR.ReadAsMimeMultipartStreamProviderException(this.streamProvider.GetType().Name), e);
                }

                if (this.outputStream == null)
                {
                    throw new InvalidOperationException(
                            SR.ReadAsMimeMultipartStreamProviderNull(
                                this.streamProvider.GetType().Name,
                                streamType.Name));
                }

                if (!this.outputStream.CanWrite)
                {
                    throw new InvalidOperationException(
                            SR.ReadAsMimeMultipartStreamProviderReadOnly(
                                this.streamProvider.GetType().Name,
                                streamType.Name));
                }

                this.HttpContent = new StreamContent(this.outputStream);
                foreach (KeyValuePair<string, IEnumerable<string>> header in this.headers)
                {
                    this.HttpContent.Headers.AddWithoutValidation(header.Key, header.Value);
                }
            }

            return this.outputStream;
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CleanupOutputStream();
                this.HttpContent = null;
                this.HeaderParser = null;
                this.Segments.Clear();
            }
        }

        /// <summary>
        /// Resets the output stream by either closing it or, in the case of a <see cref="MemoryStream"/> resetting
        /// position to 0 so that it can be read by the caller.
        /// </summary>
        private void CleanupOutputStream()
        {
            if (this.outputStream != null)
            {
                MemoryStream output = this.outputStream as MemoryStream;
                if (output != null)
                {
                    output.Position = 0;
                }
                else
                {
                    this.outputStream.Close();
                }

                this.outputStream = null;
            }
        }
    }
}
