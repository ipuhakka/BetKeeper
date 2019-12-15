﻿using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Betkeeper.Classes
{
    public class Http
    {

        /// <summary>
        /// Creates a HttpResponseMessage.
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(
            HttpStatusCode httpStatusCode, 
            object data = null, 
            bool serializeAsCamelCase = true)
        {
            var response = new HttpResponseMessage(httpStatusCode)
            {
                Content = SetHttpContent(data, serializeAsCamelCase)
            };

            return response;
        }

        /// <summary>
        /// Returns request body as dynamic.
        /// </summary>
        /// <param name="request"></param>
        public static dynamic GetHttpContent(HttpRequestMessage request)
        {
            HttpContent requestContent = request.Content;

            return DeserializeHttpContent(requestContent);
        }

        /// <summary>
        /// Returns response body as dynamic.
        /// </summary>
        /// <param name="response"></param>
        public static dynamic GetHttpContent(HttpResponseMessage response)
        {
            HttpContent requestContent = response.Content;

            return DeserializeHttpContent(requestContent);
        }

        private static dynamic DeserializeHttpContent(HttpContent content)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            string jsonContent = content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject(jsonContent, serializerSettings);
        }

        private static HttpContent SetHttpContent(object data, bool serializeAsCamelCase)
        {
            var serializerSettings = serializeAsCamelCase
                ? new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                }
                : new JsonSerializerSettings();

            return data?.GetType() == typeof(string)
                ? new StringContent(data.ToString())
                : new StringContent(JsonConvert.SerializeObject(
                    data,
                    serializerSettings),
                    Encoding.UTF8,
                    "application/json");
        }
    }
}
