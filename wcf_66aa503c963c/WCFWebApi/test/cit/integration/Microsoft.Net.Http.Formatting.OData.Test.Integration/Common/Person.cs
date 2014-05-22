// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.Integration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class Person
    {
        int age;
        Order order;

        [Key]
        public int PerId;

        public string Name;

        public Guid MyGuid;

        public int Age
        {
            get { return this.age; }
            set { this.age = value; }
        }

        public Order Order
        {
            get { return order; }
            set { this.order = (Order)value; }
        }
    }

    public class Order
    {
        public string OrderName { get; set; }

        public int OrderAmount { get; set; }
    }

}
