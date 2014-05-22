// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{
    using System;

    /// <summary>
    /// This class is used to carry the result of various file uploads.
    /// </summary>
    public class FileResult
    {
        /// <summary>
        /// Gets or sets the file name provided as part of the Content-Disposition header field.
        /// </summary>
        /// <value>
        /// The local path.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the local path of the file saved on the server.
        /// </summary>
        /// <value>
        /// The local path.
        /// </value>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the length of the saved file.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the last access time of the saved file.
        /// </summary>
        /// <value>
        /// The last access time.
        /// </value>
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FileResult"/> represents a partial file upload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if partial; otherwise, <c>false</c>.
        /// </value>
        public bool Partial { get; set; }

        /// <summary>
        /// Gets or sets the submitter as indicated in the HTML form used to upload the data.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public string Submitter { get; set; }
    }
}