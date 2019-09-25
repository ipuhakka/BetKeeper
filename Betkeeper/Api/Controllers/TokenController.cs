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
        public IUserModel _UserModel { get; set; }

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
        /// <param name="userModel"></param>
        /// <returns>Token used for accessing the system.
        /// </returns>
        public HttpResponseMessage Post()
        {
            _UserModel = _UserModel ?? new UserModel();

            var password = Request.Headers.Authorization.ToString();

            var username = Http.GetRequestBody(Request)["username"].ToString();

            var userId = _UserModel.GetUserId(username);

            if (!_UserModel.Authenticate(userId, password))
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
