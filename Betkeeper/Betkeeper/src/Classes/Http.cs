using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Betkeeper.Classes
{
    public class Http
    {
        public enum ContentType
        {
            Json,
            Text
        }

        /// <summary>
        /// Creates a HttpResponseMessage.
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(
            HttpStatusCode httpStatusCode,
            object data = null,
            ContentType contentType = ContentType.Json)
        {
            var response = new HttpResponseMessage(httpStatusCode)
            {
                Content = SetHttpContent(data, contentType)
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

        /// <summary>
        /// Returns HttpContent as specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T GetHttpContent<T>(HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Returns content as a object dictionary.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetContentAsDictionary(HttpRequestMessage request)
        {
            string jsonContent = request.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
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

        private static HttpContent SetHttpContent(object data, ContentType contentType = ContentType.Json)
        {
            if (data == null)
            {
                return null;
            }

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var contentTypeHeader = contentType == ContentType.Json
                ? "application/json"
                : "application/text";

            return data.GetType() == typeof(string)
                ? new StringContent(data.ToString())
                : new StringContent(JsonConvert.SerializeObject(
                    data,
                    serializerSettings),
                    Encoding.UTF8,
                    contentTypeHeader);
        }
    }
}
