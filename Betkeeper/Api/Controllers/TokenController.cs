using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Classes;
using Betkeeper.Models;
using Betkeeper.Data;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TokenController : ApiController
    {
        // GET: api/Token
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Token/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Token
        /// <summary>
        /// Post login-info.
        /// </summary>
        /// <returns>Token used for accessing the system.
        /// </returns>
        public HttpResponseMessage Post()
        {
            // TODO: Yksikkötestit
            var password = Request.Headers.Authorization.ToString();

            var username = Http.GetRequestBody(Request)["username"].ToString();

            var userModel = new Betkeeper.Models.UserModel();

            var userId = userModel.GetUserId(username);

            if (!userModel.Authenticate(userId, password))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var token = TokenLog.CreateToken(userId);

            return Http.CreateResponse(HttpStatusCode.OK, token);
        }

        // PUT: api/Token/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Token/5
        public void Delete(int id)
        {
        }
    }
}
