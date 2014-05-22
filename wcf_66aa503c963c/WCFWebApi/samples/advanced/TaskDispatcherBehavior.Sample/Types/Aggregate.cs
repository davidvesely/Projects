// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskDispatcherBehavior.Sample
{
    using System.Collections.Generic;

    /// <summary>
    /// Sample class providing an aggregation of <see cref="Contact"/> and <see cref="Order"/>
    /// instances.
    /// </summary>
    public class Aggregate
    {
        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>
        /// The contacts.
        /// </value>
        public List<Contact> Contacts { get; set; }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public List<Order> Orders { get; set; }
    }
}
