using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using API.Exceptions;

namespace API.Controllers
{
    //api/token
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TokenController : ApiController
    {
        /// <summary>
        ///  Creates a token for valid user which they can use to use the API, or sends an existing one if user still has valid
        ///  token.
        ///  
        /// responses:
        /// 200 OK,
        /// 400 Bad request,
        /// 401 Unauthorized
        /// </summary>
        public HttpResponseMessage Post([FromBody] TokenModel model)
        {
            if (model == null)
            {
                return HttpMessage.ResponseMessage("Invalid request body", HttpStatusCode.BadRequest);
            }
            string data = "";
            data = model.GetToken(model.username, model.password);

            if (data == null)
            {
                return HttpMessage.ResponseMessage("Username or password was incorrect", HttpStatusCode.Unauthorized);
            }
            else
            {
                return HttpMessage.ResponseMessage(data, HttpStatusCode.OK, "application/json");
            }
        }

        /// <summary>
        /// api/token?token={token}
        /// Checks if token is still in use. 
        /// 
        /// responses:
        /// 200 OK if token is still valid,
        /// 404 Not found if token is not found
        /// </summary>
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (token == null)
                return HttpMessage.ResponseMessage("Invalid request", HttpStatusCode.BadRequest);

            if (TokenLog.ContainsToken(token))
                return HttpMessage.ResponseMessage("", HttpStatusCode.OK);
            else
                return HttpMessage.ResponseMessage("", HttpStatusCode.NotFound);
        }
    }
}
