using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Betkeeper.Classes
{
    public class Http
    {
        /// <summary>
        /// Returns request body in specified format
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static T GetRequestBody<T>(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body);
            return JsonConvert.DeserializeObject<T>(reader.ReadToEndAsync().Result);
        }
    }
}
