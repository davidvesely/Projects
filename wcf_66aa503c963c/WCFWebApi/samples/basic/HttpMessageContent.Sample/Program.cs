// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpMessageContent.Sample
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// This sample illustrates several scenarios around encapsulating <see cref="HttpRequestMessage"/> and 
    /// <see cref="HttpResponseMessage"/> within an <see cref="HttpContent"/> including <see cref="MultipartContent"/>.
    /// </summary>
    public static class Program
    {
        public static string HttpMessageContentRequest()
        {
            // Create a sample HttpRequestMessage
            HttpRequestMessage httpRequest = 
                new HttpRequestMessage(HttpMethod.Post, "http://example.com/some/host");
            httpRequest.Content = new StringContent("This is a sample request body");

            // Create an HttpMessageContent encapsulating the sample HttpRequestMessage
            HttpMessageContent content = new HttpMessageContent(httpRequest);

            // We can now use this HttpContent in any and all contexts where HttpContent can be used
            // which means that we can use it as the HttpContent of another HttpRequestMessage etc.
            // The content will serialize itself as a wire-formatted HTTP request like this.
            string result = content.ReadAsStringAsync().Result;
            Console.WriteLine("\nHttp Request:\n{0}", result);
            return result;
        }

        public static string HttpMessageContentResponse()
        {
            // Create a sample HttpResponseMessage
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.Created);
            httpResponse.Content = new StringContent("This is a sample response body");

            // Create an HttpMessageContent encapsulating the sample HttpResponseMessage
            HttpMessageContent content = new HttpMessageContent(httpResponse);

            // We can now use this HttpContent in any and all contexts where HttpContent can be used
            // which means that we can use it as the HttpContent of another HttpResponseMessage etc.
            // The content will serialize itself as a wire-formatted HTTP Response like this.
            string result = content.ReadAsStringAsync().Result;
            Console.WriteLine("\nHttp Response:\n{0}", result);
            return result;
        }

        public static string HttpMessageObjectContentResponse()
        {
            // Create a sample HttpResponseMessage
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.Created);

            // Create an Contact instance
            Order order = new Order
            {
                Product = "aaabbbccc",
                Quantity = 1,
                Category = "AAABBBCCC",
                Id = 1,
            };

            // Create object content with the new contact
            httpResponse.Content = new ObjectContent<Order>(order);

            // Create an HttpMessageContent encapsulating the sample HttpResponseMessage
            HttpMessageContent content = new HttpMessageContent(httpResponse);

            // We can now use this HttpContent in any and all contexts where HttpContent can be used
            // which means that we can use it as the HttpContent of another HttpResponseMessage etc.
            // The content will serialize itself as a wire-formatted HTTP Response like this.
            string result = content.ReadAsStringAsync().Result;
            Console.WriteLine("\nHttp Response:\n{0}", result);
            return result;
        }

        public static string MultipartHttpMessageContent()
        {
            MultipartContent multipart = new MultipartContent("mixed", "12345");

            // Create a sample HttpRequestMessage
            HttpRequestMessage httpRequest =
                new HttpRequestMessage(HttpMethod.Post, "http://example.com/some/host");
            httpRequest.Content = new StringContent("This is a sample request body");

            // Create an HttpMessageContent encapsulating the sample HttpRequestMessage
            HttpMessageContent requestContent = new HttpMessageContent(httpRequest);

            // Add it to our MIME multipart message
            multipart.Add(requestContent);

            // Create a sample HttpResponseMessage
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.Created);
            httpResponse.Content = new StringContent("This is a sample response body");

            // Create an HttpMessageContent encapsulating the sample HttpResponseMessage
            HttpMessageContent responseContent = new HttpMessageContent(httpResponse);

            // Add it to our MIME multipart message
            multipart.Add(responseContent);

            // We can now see how the MIME multipart content serializes itself:
            string result = multipart.ReadAsStringAsync().Result;
            Console.WriteLine("\nMIME Multipart Message:\n{0}", result);
            return result;
        }

        public static HttpRequestMessage ReadContentAsHttpRequestMessage()
        {
            string message = HttpMessageContentRequest();
            StringContent content = new StringContent(message);
            MediaTypeHeaderValue contentType;
            MediaTypeHeaderValue.TryParse("application/http;msgtype=request", out contentType);
            content.Headers.ContentType = contentType;

            HttpRequestMessage request = content.ReadAsHttpRequestMessage();
            return request;
        }

        public static HttpResponseMessage ReadContentAsHttpResponseMessage()
        {
            string message = HttpMessageContentResponse();
            StringContent content = new StringContent(message);
            MediaTypeHeaderValue contentType;
            MediaTypeHeaderValue.TryParse("application/http;msgtype=response", out contentType);
            content.Headers.ContentType = contentType;

            HttpResponseMessage reponse = content.ReadAsHttpResponseMessage();
            return reponse;
        }

        public static void ReadMultipartHttpContent(out HttpRequestMessage request, out HttpResponseMessage response)
        {
            string message = MultipartHttpMessageContent();
            StringContent content = new StringContent(message);
            MediaTypeHeaderValue contentType;
            MediaTypeHeaderValue.TryParse("multipart/mixed;boundary=12345", out contentType);
            content.Headers.ContentType = contentType;

            IEnumerable<HttpContent> innerContent = content.ReadAsMultipart();

            HttpContent requestContent = innerContent.ElementAt(0);
            request = requestContent.ReadAsHttpRequestMessage();

            HttpContent responseContent = innerContent.ElementAt(1);
            response = responseContent.ReadAsHttpResponseMessage();
        }

        public static void Main(string[] args)
        {
            HttpMessageContentRequest();

            HttpMessageContentResponse();

            HttpMessageObjectContentResponse();

            MultipartHttpMessageContent();

            ReadContentAsHttpRequestMessage();

            ReadContentAsHttpResponseMessage();

            HttpRequestMessage request;
            HttpResponseMessage response;
            ReadMultipartHttpContent(out request, out response);

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}