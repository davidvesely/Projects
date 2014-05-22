// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpBatching.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationServer.Http.Channels;
    using Microsoft.ApplicationServer.Http.Description;

    public class BatchingMessageHandler : DelegatingHandler
    {
        public const string BatchingUriPostfix = @"/batching";
        private HttpMemoryHandler httpMemoryHandler;

        /// <summary>
        /// Initializes this instance with the <see cref="HttpMemoryBinding"/> providing the 
        /// <see cref="HttpMemoryHandler"/> used for submitting the batched requests</param>
        public void Initialize(HttpMemoryEndpoint httpMemoryEndpoint)
        {
            if (httpMemoryEndpoint == null)
            {
                throw new ArgumentNullException("httpMemoryEndpoint");
            }

            if (this.httpMemoryHandler != null)
            {
                throw new InvalidOperationException("The handler cannot be initialized more than once.");
            }

            this.httpMemoryHandler = httpMemoryEndpoint.GetHttpMemoryHandler();
        }

        /// <summary>
        /// This is where the actual batching happens. First the message is checked to see if it is a MIME 
        /// multipart batching request. Then it is read and each individual HTTP request extracted. 
        /// Once all are extracted the batched HTTP requests are submitted to the memory binding that 
        /// pushes them up to the service. When all responses have completed they are put into a 
        /// MIME multipart batching response message and returned to the caller.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Check that the request is for a batching endpoint and if not then just pass it through
            if (request.Method == HttpMethod.Post &&
                request.Content != null && request.Content.IsMimeMultipartContent("batching") &&
                request.RequestUri.AbsolutePath.EndsWith(BatchingUriPostfix, StringComparison.OrdinalIgnoreCase))
            {
                // Check that we have been initialized and if not return a server error response 
                if (this.httpMemoryHandler == null)
                {
                    return Task.Factory.StartNew<HttpResponseMessage>(() => { return CreateHttpResponse(request, HttpStatusCode.InternalServerError); });
                }

                // Read the MIME multipart asynchronously
                return request.Content.ReadAsMultipartAsync().ContinueWith<HttpResponseMessage>(
                    (readTask) =>
                    {
                        // Read individual MIME parts and create corresponding HttpRequestMessage instances stopping on first failure
                        List<HttpRequestMessage> requests = new List<HttpRequestMessage>();
                        foreach (HttpContent content in readTask.Result)
                        {
                            try
                            {
                                HttpRequestMessage batchedRequest = content.ReadAsHttpRequestMessage();

                                // Note: Here we can add properties from the outer request if desired.
                                requests.Add(batchedRequest);
                            }
                            catch
                            {
                                return CreateHttpResponse(request, HttpStatusCode.BadRequest);
                            }
                        }

                        // Submit all batched requests and collect the corresponding tasks
                        List<Task<HttpResponseMessage>> requestTasks = new List<Task<HttpResponseMessage>>();
                        foreach (HttpRequestMessage batchedRequest in requests)
                        {
                            try
                            {
                                requestTasks.Add(this.httpMemoryHandler.SubmitRequestAsync(batchedRequest, cancellationToken));
                            }
                            catch
                            {
                                return CreateHttpResponse(request, HttpStatusCode.BadRequest);
                            }
                        }

                        // When all requests have completed then continue generating the batched response.
                        return Task.Factory.ContinueWhenAll(requestTasks.ToArray(),
                            (Task<HttpResponseMessage>[] batchedTasks) =>
                            {
                                MultipartContent responseContent = new MultipartContent("batching");
                                foreach (Task<HttpResponseMessage> batchedTask in batchedTasks)
                                {
                                    try
                                    {
                                        HttpMessageContent batchedResponseContent = new HttpMessageContent(batchedTask.Result);
                                        responseContent.Add(batchedResponseContent);
                                    }
                                    catch
                                    {
                                        return CreateHttpResponse(request, HttpStatusCode.BadRequest);
                                    }
                                }

                                HttpResponseMessage response = CreateHttpResponse(request, HttpStatusCode.OK);
                                response.Content = responseContent;
                                return response;
                            }).Result;

                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }

        private static HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, HttpStatusCode status)
        {
            Contract.Assert(request != null, "request cannot be null");
            return new HttpResponseMessage(status) { RequestMessage = request };
        }
    }
}
