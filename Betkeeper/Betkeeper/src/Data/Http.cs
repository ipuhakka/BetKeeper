using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Betkeeper.Data
{
    public class Http
    {

        /// <summary>
        /// Creates a HttpResponseMessage.
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(HttpStatusCode httpStatusCode, object data = null)
        {
            var response = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data))
            };

            return response;
        }

        /// <summary>
        /// Returns request body as dynamic.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static dynamic GetRequestBody(HttpRequestMessage request)
        {
            // TODO: Yksikkötestit
            HttpContent requestContent = request.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;

            return (dynamic)JsonConvert.DeserializeObject(jsonContent);
        }
    }
}
