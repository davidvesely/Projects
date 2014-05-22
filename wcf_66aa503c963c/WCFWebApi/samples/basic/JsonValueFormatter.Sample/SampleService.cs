// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace JsonValueFormatter.Sample
{
    using System;
    using System.Json;
    using System.Net.Http;
    using System.ServiceModel.Web;

    /// <summary>
    /// This WCF Web API service accepts and returns <see cref="JsonValue"/> objects.
    /// </summary>
    public class SampleService
    {
        [WebGet(UriTemplate = "/message")]
        public HttpResponseMessage<JsonValue> GetResponseOfJsonValue(HttpRequestMessage request)
        {
            HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(new JsonPrimitive(DateTime.Now));
            response.RequestMessage = request;
            return response;
        }

        [WebInvoke(UriTemplate = "/message", Method = "POST")]
        public HttpResponseMessage<JsonValue> PostRequestOfJsonValue(HttpRequestMessage<JsonValue> request)
        {
            JsonValue input = request.Content.ReadAsAsync<JsonValue>().Result;
            HttpResponseMessage<JsonValue> response = new HttpResponseMessage<JsonValue>(input);
            response.RequestMessage = request;
            return response;
        }

        [WebGet(UriTemplate = "/object")]
        public JsonValue GetJsonValue()
        {
            return new JsonArray(new JsonPrimitive(1), new JsonPrimitive(2), new JsonPrimitive(3));
        }

        [WebInvoke(UriTemplate = "/object", Method = "POST")]
        public JsonValue PostJsonValue(JsonValue input)
        {
            return input;
        }
    }
}