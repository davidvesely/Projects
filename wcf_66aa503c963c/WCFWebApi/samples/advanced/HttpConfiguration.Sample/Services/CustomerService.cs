// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Simple Customer Service
    /// </summary>
    [Export]
    public class CustomerService
    {
        private static List<Customer> customers;

        static CustomerService()
        {
            customers = new List<Customer>();
            customers.Add(new Customer() { Id = 1, Name = "Customer1" });
            customers.Add(new Customer() { Id = 2, Name = "Customer2" });
            customers.Add(new Customer() { Id = 3, Name = "Customer3" });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="customerList">The customer list.</param>
        /// <remarks>This is used by <see cref="SetInstanceProviderMultipleHost"/></remarks>
        [ImportingConstructor]
        public CustomerService(List<Customer> customerList)
        {
            customers = customerList;
        }

        [WebGet(UriTemplate="")]
        public List<Customer> GetCustomers()
        {
            return customers;
        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
