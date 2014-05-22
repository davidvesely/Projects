// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpBatching.Sample
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// This sample WCF service reads the contents of an HTML file upload and 
    /// deserializes the contents into a local object which is then returned 
    /// in the response. 
    /// </summary>
    public class SampleService
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

        [WebInvoke(UriTemplate = BatchingMessageHandler.BatchingUriPostfix, Method = "POST")]
        public bool Batching()
        {
            return false;
        }

        [WebGet(UriTemplate = "/contacts")]
        public List<Contact> Contacts()
        {
            List<Contact> result = new List<Contact>(contacts.ToArray());
            return result;
        }

        [WebInvoke(UriTemplate = "/contacts", Method = "POST")]
        public Contact AddContact(Contact contact)
        {
            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            contacts.Add(contact);
            return contact;
        }

        [WebGet(UriTemplate = "/orders")]
        public List<Order> Orders()
        {
            List<Order> result = new List<Order>(orders.ToArray());
            return result;
        }

        [WebInvoke(UriTemplate = "/orders", Method = "POST")]
        public Order AddOrder(Order order)
        {
            if (order == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            orders.Add(order);
            return order;
        }
    }
}