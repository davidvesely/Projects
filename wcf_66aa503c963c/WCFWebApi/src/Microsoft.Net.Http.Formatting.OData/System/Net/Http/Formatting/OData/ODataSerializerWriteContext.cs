// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData
{
    using System;

    /// <summary>
    /// Context information used by the serilizers while serializing objects in OData message format.
    /// </summary>
#if USE_REFEMIT
    public class ODataSerializerWriteContext
#else
    internal class ODataSerializerWriteContext
#endif
    {        
        private ODataResponseContext responseContext;

        private int maxReferenceDepth;
        private int currentReferenceDepth;

        /// <summary>
        ///  Initializes a new instance of <see cref="ODataSerializerWriteContext"/>.
        /// </summary>
        /// <param name="maxReferenceDepth">The maximum depth in the object graph to be written out while serializing a type.</param>
        /// <param name="responseContext">An instance of <see cref="ODataResponseContext"/> that has context information for serializing various types. </param>
        public ODataSerializerWriteContext(int maxReferenceDepth, ODataResponseContext responseContext)
        {
            this.maxReferenceDepth = maxReferenceDepth;
            this.responseContext = responseContext;
            this.currentReferenceDepth = -1;
        }

        /// <summary>
        /// Gets the maximum depth in the object graph to be written out while serializing a type. This is to avoid issues with cyclic references
        /// </summary>
        public int MaxReferenceDepth
        {
            get
            {
                return this.maxReferenceDepth;
            }
        }

        /// <summary>
        /// Gets the ResponseContext that has context information for serializing various types.
        /// </summary>
        public ODataResponseContext ResponseContext
        {
            get
            {
                return this.responseContext;
            }
        }

        /// <summary>
        /// Increments the currentReferenceDepth that is maintained by the <see cref="ODataSerializerWriteContext"/> instance.
        /// </summary>
        /// <returns>True if the currentDepth is less than the MaxReferenceDepth, else returns False.</returns>
        public bool IncrementCurrentReferenceDepth()
        {
            if (++this.currentReferenceDepth > this.maxReferenceDepth)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Decrements the CurrentReferenceDepth of the type being written
        /// </summary>
        public void DecrementCurrentReferenceDepth()
        {
            this.currentReferenceDepth--;
        }
    }
}
