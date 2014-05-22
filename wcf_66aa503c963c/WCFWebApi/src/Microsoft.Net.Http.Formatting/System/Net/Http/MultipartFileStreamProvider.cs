// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net.Http.Headers;

    /// <summary>
    /// An <see cref="IMultipartStreamProvider"/> suited for writing each MIME body parts of the MIME multipart
    /// message to a file using a <see cref="FileStream"/>.
    /// </summary>
    public class MultipartFileStreamProvider : IMultipartStreamProvider
    {
        private const int DefaultBufferSize = 0x1000;

        private List<string> bodyPartFileNames = new List<string>();
        private object thisLock = new object();
        private string rootPath;
        private int bufferSize = DefaultBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFileStreamProvider"/> class.
        /// </summary>
        public MultipartFileStreamProvider()
            : this(Path.GetTempPath(), DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFileStreamProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path where the content of MIME multipart body parts are written to.</param>
        public MultipartFileStreamProvider(string rootPath)
            : this(rootPath, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFileStreamProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path where the content of MIME multipart body parts are written to.</param>
        /// <param name="bufferSize">The number of bytes buffered for writes to the file.</param>
        public MultipartFileStreamProvider(string rootPath, int bufferSize)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentNullException("rootPath");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize", bufferSize, SR.NonZeroParameterSize);
            }

            this.rootPath = Path.GetFullPath(rootPath);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing the files names of MIME 
        /// body part written to file.
        /// </summary>
        public IEnumerable<string> BodyPartFileNames
        {
            get
            {
                lock (this.thisLock)
                {
                    return this.bodyPartFileNames != null ?
                        new List<string>(this.bodyPartFileNames) : new List<string>();
                }
            }
        }

        /// <summary>
        /// This body part stream provider examines the headers provided by the MIME multipart parser
        /// and decides which <see cref="FileStream"/> to write the body part to.
        /// </summary>
        /// <param name="headers">Header fields describing the body part</param>
        /// <returns>The <see cref="Stream"/> instance where the message body part is written to.</returns>
        public Stream GetStream(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            return this.OnGetStream(headers);
        }

        /// <summary>
        /// Override this method in a derived class to examine the headers provided by the MIME multipart parser
        /// and decides which <see cref="FileStream"/> to write the body part to.
        /// </summary>
        /// <param name="headers">Header fields describing the body part</param>
        /// <returns>The <see cref="Stream"/> instance where the message body part is written to.</returns>
        protected virtual Stream OnGetStream(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string localFilePath;
            try
            {
                string filename = this.GetLocalFileName(headers);
                localFilePath = Path.Combine(this.rootPath, Path.GetFileName(filename));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(SR.MultipartStreamProviderInvalidLocalFileName, e);
            }

            if (!Directory.Exists(this.rootPath))
            {
                Directory.CreateDirectory(this.rootPath);
            }

            // Add local file name 
            lock (this.thisLock)
            {
                this.bodyPartFileNames.Add(localFilePath);
            }

            return File.Create(localFilePath, this.bufferSize, FileOptions.Asynchronous);
        }

        /// <summary>
        /// Gets the name of the local file which will be combined with the root path to
        /// create an absolute file name where the contents of the current MIME body part
        /// will be stored.
        /// </summary>
        /// <param name="headers">The headers for the current MIME body part.</param>
        /// <returns>A relative filename with no path component.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        protected virtual string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string filename = null;
            try
            {
                ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    filename = contentDisposition.ExtractLocalFileName();
                }
            }
            catch (Exception)
            {
                //// TODO: CSDMain 232171 -- review and fix swallowed exception
            }

            if (filename == null)
            {
                filename = string.Format(CultureInfo.InvariantCulture, "BodyPart_{0}", Guid.NewGuid());
            }

            return filename;
        }
    }
}