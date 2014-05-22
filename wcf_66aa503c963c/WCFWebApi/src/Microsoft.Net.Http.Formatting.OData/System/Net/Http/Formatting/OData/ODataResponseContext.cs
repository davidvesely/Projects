// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;
    using Microsoft.Data.OData;

    /// <summary>    
    /// This class contains information that the DataContractODataSerializer will use during
    /// serialization.
    /// </summary>
    internal class ODataResponseContext
    {
        private IODataResponseMessage responseMessage;
        private ODataFormat format;
        private ODataVersion version;
        private Uri baseAddress;
        private string serviceOperationName;

        /// <summary>        
        /// Initializes a new instance of ODataResponseContext with the specified responseMessage,
        /// baseAddress and serviceOperationName.
        /// </summary>
        /// <param name="responseMessage">An instance of the IODataResponseMessage.</param>
        /// <param name="format">ODataFormat to be used.</param>
        /// <param name="version">DataServiceversion to be used</param>
        /// <param name="baseAddress">The baseAddress to be used while serializing feed/entry.</param>
        /// <param name="serviceOperationName">The serviceOperationName to use while serializing primitives and complex types.</param>
        public ODataResponseContext(IODataResponseMessage responseMessage, ODataFormat format, ODataVersion version, Uri baseAddress, string serviceOperationName)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException("responseMessage");
            }

            if (baseAddress == null)
            {
                throw new ArgumentNullException("baseAddress");
            }

            if (string.IsNullOrEmpty(serviceOperationName))
            {
                throw new ArgumentException(SR.ArgumentNullOrEmpty("serviceOperationName"));
            }

            this.responseMessage = responseMessage;
            this.format = format;
            this.version = version;
            this.baseAddress = baseAddress;
            this.serviceOperationName = serviceOperationName;
            this.IsIndented = true;
        }

        /// <summary>        
        /// Gets or sets the instance of IODataResponseMessage which gives information
        /// such as contentType, ODataFormatVersion, stream etc.
        /// </summary>
        public IODataResponseMessage ODataResponseMessage
        {
            get
            {
                return this.responseMessage;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.responseMessage = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the ODataFormat that should be used for writing the content payload
        /// </summary>
        public ODataFormat ODataFormat
        {
            get
            {
                return this.format;
            }

            set
            {
                this.format = value;
            }
        }

        /// <summary>
        /// Gets or sets the DataServiceVersion that would be used by <see cref="ODataMessageWriter"/>.
        /// </summary>
        public ODataVersion ODataVersion
        {
            get
            {
                return this.version;
            }

            set
            {
                this.version = value;
            }
        }

        /// <summary>    
        /// Gets or sets the BaseAddress which is used when writing an entry or feed.
        /// </summary>
        public Uri BaseAddress
        {
            get
            {
                return this.baseAddress;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.baseAddress = value;
            }
        }

        /// <summary>      
        /// Gets or sets the ServiceOperationName which is used when writing primitive types
        /// and complex types.
        /// </summary>
        public string ServiceOperationName
        {
            get
            {
                return this.serviceOperationName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(SR.ArgumentNullOrEmpty("value"));
                }

                this.serviceOperationName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the serialized content should be indented.
        /// </summary>
        public bool IsIndented
        {
            get;
            set;
        }
    }
}
