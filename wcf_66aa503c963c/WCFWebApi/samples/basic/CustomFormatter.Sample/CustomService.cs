// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace CustomFormatter.Sample
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    /// <summary>
    /// This customer service is used to model a Customer resource. It support GET, POST and Delete.
    /// There are various methods supporting GET, POST and DELETE, some of those showing how to modify the content type header dynamically
    /// in the service operation at runtime. 
    /// </summary>
    internal class CustomerService
    {
        private static ConcurrentDictionary<int, Customer> customers;

        static CustomerService()
        {
            customers = new ConcurrentDictionary<int, Customer>();
            customers.TryAdd(1, new Customer() { Id = 1, Name = "Customer1" });
            customers.TryAdd(2, new Customer() { Id = 2, Name = "Customer2" });
            customers.TryAdd(3, new Customer() { Id = 3, Name = "Customer3" });
        }

        [WebGet(UriTemplate="/{id}")]
        public Customer GetCustomer(int id)
        {
            return GetCustomerFromId(id);
        }

        [WebInvoke(Method = "POST", UriTemplate = "/Customers")]
        public HttpResponseMessage<Customer> PostCustomers(HttpRequestMessage request)
        {
            Customer newCustomer = GetCustomerFromRequest(request);
            AddCustomer(newCustomer);
            HttpResponseMessage<Customer> response = new HttpResponseMessage<Customer>(newCustomer);
            response.StatusCode = HttpStatusCode.Created;
            return response;
        }

        [WebInvoke(Method = "POST", UriTemplate = "/CustomersWithCustomContent")]
        public HttpResponseMessage<Customer> PostCustomersWithCustomContent(HttpRequestMessage request)
        {
            Customer newCustomer = GetCustomerFromRequest(request);
            AddCustomer(newCustomer);
            HttpResponseMessage<Customer> response = new HttpResponseMessage<Customer>(newCustomer);
            response.Headers.Location = GetLocationHeader(request, newCustomer.Id);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/foo");
            response.StatusCode = HttpStatusCode.Created;
            return response;
        }

        [WebInvoke(Method = "POST", UriTemplate = "/CustomersWithDynamicCustomContent")]
        public HttpResponseMessage<Customer> PostCustomersWithDynamicCustomContent(HttpRequestMessage request)
        {
            Customer newCustomer = GetCustomerFromRequest(request);
            AddCustomer(newCustomer);
            HttpResponseMessage<Customer> response = new HttpResponseMessage<Customer>(newCustomer);
            MediaTypeHeaderValue header = new MediaTypeHeaderValue("application/bar");

            // dynamically configuring the formatter inside service operation, this should override the one configured on the 
            // host level 
            response.Content.Formatters.JsonFormatter.SupportedMediaTypes.Add(header); 

            response.Headers.Location = GetLocationHeader(request, newCustomer.Id);
            response.Content.Headers.ContentType = header;
            response.StatusCode = HttpStatusCode.Created;
            return response;
        }

        [WebInvoke(Method = "DELETE", UriTemplate = "*")]
        public void DeleteCustomers(HttpRequestMessage request)
        {
            int id = GetIdFromQueryString(request);
            RemoveCustomer(id);
        }

        private static int GetIdFromQueryString(HttpRequestMessage request)
        {
            var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);

            int id;
            if (!int.TryParse(queryString["id"], out id))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("An 'id' with a integer value must be provided in the query string.");
                throw new HttpResponseException(response);
            }

            return id;
        }

        private static Customer GetCustomerFromId(int id)
        {
            Customer customer;
            if (!customers.TryGetValue(id, out customer))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.NotFound;
                response.Content = new StringContent(string.Format("There is no customer with id '{0}'.", id));
                throw new HttpResponseException(response);
            }

            return customer;
        }

        private static void RemoveCustomer(int id)
        {
            Customer customer;
            if (!customers.TryRemove(id, out customer))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.NotFound;
                response.Content = new StringContent(string.Format("There is no customer with id '{0}'.", id));
                throw new HttpResponseException(response);
            }
        }

        private static Customer GetCustomerFromRequest(HttpRequestMessage request)
        {
            if (request.Content == null)
            {
                HttpResponseMessage noContentResponse = new HttpResponseMessage();
                noContentResponse.StatusCode = HttpStatusCode.BadRequest;
                noContentResponse.Content = new StringContent("Expected an entity body with customer data but no content was found.");
                throw new HttpResponseException(noContentResponse);
            }

            string content = request.Content.ReadAsStringAsync().Result;
            string[] contentSplit = content.Split('=', ';');

            if (contentSplit.Length == 4)
            {
                if (string.Equals(contentSplit[0].Trim(), "Id", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(contentSplit[2].Trim(), "Name", StringComparison.OrdinalIgnoreCase))
                {
                    Customer customer = new Customer();
                    int id;
                    if (int.TryParse(contentSplit[1].Trim(), out id))
                    {
                        customer.Id = id;
                        customer.Name = contentSplit[3].Trim();
                        return customer;
                    }
                }
            }

            HttpResponseMessage faileParseResponse = new HttpResponseMessage();
            faileParseResponse.StatusCode = HttpStatusCode.BadRequest;
            faileParseResponse.Content = new StringContent("Parsing the entity body as a customer failed.");
            throw new HttpResponseException(faileParseResponse);
        }

        private static void AddCustomer(Customer customer)
        {
            if (!customers.TryAdd(customer.Id, customer))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Conflict;
                response.Content = new StringContent(string.Format("There already a customer with id '{0}'.", customer.Id));
                throw new HttpResponseException(response);
            }
        }

        private static Uri GetLocationHeader(HttpRequestMessage request, int id)
        {
            UriBuilder builder = new UriBuilder(request.RequestUri);
            builder.Query += string.Format("id={0}", id);
            return builder.Uri;
        }
    }
}
