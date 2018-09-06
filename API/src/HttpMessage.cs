using System.Text;
using System.Net;
using System.Net.Http;

namespace API
{
    public class HttpMessage
    {
        /// <summary>
        /// Returns a HttpResponseMessage object.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="status">Statuscode response returns.</param>
        /// <param name="contentType">Optional parameter describing content type that the response is returning.</param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseMessage(string data, HttpStatusCode status, string contentType = null)
        {
            var response = new HttpResponseMessage(status);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (contentType == null)
                response.Content = new StringContent(data);
            else
                response.Content = new StringContent(data, Encoding.UTF8, contentType);
            return response;
        }
    }
}
