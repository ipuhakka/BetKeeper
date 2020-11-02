using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using Betkeeper.Classes;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ErrorController : ApiController
    {
        /// <summary>
        /// Log a client error
        /// </summary>
        /// <param name="clientError"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(ClientError clientError)
        {
            clientError.LogError();

            return Http.CreateResponse(System.Net.HttpStatusCode.OK);
        }
    }
}
