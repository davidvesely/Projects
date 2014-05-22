// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskAsync
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;

    public class AggregatorApi
    {
        static string backendAddress = "http://localhost:8400/backend";

        [WebGet]
        public Task<Aggregate> Aggregation()
        {
            // Create an HttpClient (we could also reuse an existing one)
            HttpClient client = new HttpClient();

            // Submit GET requests for contacts and orders
            Task<List<Contact>> contactsTask = client.GetAsync(backendAddress + "/contacts").ContinueWith<Task<List<Contact>>>((responseTask) =>
                {
                    return responseTask.Result.Content.ReadAsAsync<List<Contact>>();
                }).Unwrap();
            Task<List<Order>> ordersTask = client.GetAsync(backendAddress + "/orders").ContinueWith<Task<List<Order>>>((responseTask) =>
                {
                    return responseTask.Result.Content.ReadAsAsync<List<Order>>();
                }).Unwrap();
   
            // Wait for both requests to complete
            return Task.Factory.ContinueWhenAll(new Task[] { contactsTask, ordersTask },
                (completedTasks) =>
                {
                    client.Dispose();
                    Aggregate aggregate = new Aggregate() 
                    { 
                        Contacts = contactsTask.Result,
                        Orders = ordersTask.Result
                    };

                    return aggregate;
                });
        }

        [WebGet(UriTemplate = "contacts")]
        public Task<HttpResponseMessage> Contacts()
        {
            // Create an HttpClient (we could also reuse an existing one)
            HttpClient client = new HttpClient();

            // Submit GET requests for contacts and return task directly
            return client.GetAsync(backendAddress + "/contacts");
        }
    }
}