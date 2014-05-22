// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskDispatcherBehavior.Sample
{
    /// <summary>
    /// Sample class providing example order information.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the order category.
        /// </summary>
        /// <value>
        /// The order category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }
    }
}
