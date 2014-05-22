// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace TaskDispatcherBehavior.Sample
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;
    using TaskDispatcherBehavior;

    [ServiceContract, TaskService]
    public class AggregatorService
    {
        [WebGet(UriTemplate = ""), TaskOperation]
        public Task<Aggregate> Aggregation()
        {
            // Create an HttpClient (we could also reuse an existing one)
            HttpClient client = new HttpClient();

            // Submit GET requests for contacts and orders
            Task<HttpResponseMessage>[] requestTasks = new Task<HttpResponseMessage>[2];
            requestTasks[0] = client.GetAsync(Program.BackendServiceAddress + "/contacts");
            requestTasks[1] = client.GetAsync(Program.BackendServiceAddress + "/orders");

            // Wait for both requests to complete
            return Task.Factory.ContinueWhenAll(requestTasks,
                (completedTasks) =>
                {
                    Aggregate aggregate = new Aggregate();

                    // Extract contacts
                    HttpResponseMessage contactResponse = completedTasks[0].Result;
                    if (contactResponse.IsSuccessStatusCode)
                    {
                        aggregate.Contacts = contactResponse.Content.ReadAsAsync<List<Contact>>().Result;
                    }

                    // Extract orders
                    HttpResponseMessage orderResponse = completedTasks[1].Result;
                    if (orderResponse.IsSuccessStatusCode)
                    {
                        aggregate.Orders = orderResponse.Content.ReadAsAsync<List<Order>>().Result;
                    }

                    return aggregate;
                });
        }

        [WebGet(UriTemplate = "contacts"), TaskOperation]
        public Task<HttpResponseMessage> Contacts()
        {
            // Create an HttpClient (we could also reuse an existing one)
            HttpClient client = new HttpClient();

            // Submit GET requests for contacts and return task directly
            return client.GetAsync(Program.BackendServiceAddress + "/contacts");
        }

        /// <summary>
        /// This operation returns simply <see cref="Task"/> which is translated into an empty
        /// HTTP response with status code 200 OK.
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "empty"), TaskOperation]
        public Task EmptyResponse()
        {
            // Create an HttpClient (we could also reuse an existing one)
            HttpClient client = new HttpClient();

            // Submit GET requests for contacts and return task directly
            return client.GetAsync(Program.BackendServiceAddress + "/contacts");
        }
    }
}