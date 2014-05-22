// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskDispatcherBehavior.Sample
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    public class BackendService
    {
        private static ConcurrentBag<Contact> contacts = new ConcurrentBag<Contact>
        { 
            new Contact{ Age = 1, Avatar = "http://www.example.com/1", Id = 1, Name = "1"},
            new Contact{ Age = 2, Avatar = "http://www.example.com/2", Id = 2, Name = "2"},
            new Contact{ Age = 3, Avatar = "http://www.example.com/3", Id = 3, Name = "3"},
            new Contact{ Age = 4, Avatar = "http://www.example.com/4", Id = 4, Name = "4"},
        };

        private static ConcurrentBag<Order> orders = new ConcurrentBag<Order>
        { 
            new Order { Product = "a", Category = "a", Quantity = 1},
            new Order { Product = "b", Category = "b", Quantity = 2},
            new Order { Product = "c", Category = "c", Quantity = 3},
            new Order { Product = "d", Category = "d", Quantity = 4},
        };

        [WebGet(UriTemplate = "/contacts")]
        public List<Contact> Contacts()
        {
            List<Contact> result = new List<Contact>(contacts.ToArray());
            return result;
        }

        [WebGet(UriTemplate = "/orders")]
        public List<Order> Orders()
        {
            List<Order> result = new List<Order>(orders.ToArray());
            return result;
        }
    }
}