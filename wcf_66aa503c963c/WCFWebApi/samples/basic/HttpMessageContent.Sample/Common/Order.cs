// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpMessageContent.Sample
{

    /// <summary>
    /// Sample class providing example order information.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets a unique identifier for this <see cref="Order"/> instance.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the product of the order.
        /// </summary>
        /// <value>
        /// The name of the order.
        /// </value>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the order.
        /// </summary>
        /// <value>
        /// The age of the order.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the order.
        /// </summary>
        /// <value>
        /// The avatar of the order.
        /// </value>
        public string Category { get; set; }
    }
}
