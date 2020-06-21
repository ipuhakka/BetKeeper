﻿using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TokenController : ApiController
    {
        protected UserRepository UserRepository { get; set; }

        public TokenController()
        {
            UserRepository = new UserRepository();
        }

        protected override void Dispose(bool disposing)
        {
            UserRepository.Dispose();
            base.Dispose(disposing);
        }

        // GET: api/Token/5
        /// <summary>
        /// Checks if user authentication token is still valid.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("api/token/{userId}")]
        public HttpResponseMessage Get(int userId)
        {
            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!TokenLog.ContainsToken(tokenString))
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            if (TokenLog.GetTokenOwner(tokenString) == userId)
            {
                return Http.CreateResponse(HttpStatusCode.OK);
            }

            return Http.CreateResponse(HttpStatusCode.Unauthorized);
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
            var password = Request.Headers.Authorization?.ToString();

            if (password == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var username = Http.GetHttpContent(Request)["username"].ToString();

            var userId = UserRepository.GetUserId(username.ToString());

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (!UserRepository.Authenticate(userId, password))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var token = TokenLog.GetExistingToken(userId)
                ?? TokenLog.CreateToken(userId);

            return Http.CreateResponse(HttpStatusCode.OK, token);
        }

        // DELETE: api/Token/5
        [Route("api/token/{userId}")]
        public HttpResponseMessage Delete(int userId)
        {
            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (TokenLog.GetTokenOwner(tokenString) == userId)
            {
                TokenLog.DeleteToken(userId, tokenString);

                return Http.CreateResponse(HttpStatusCode.NoContent);
            }

            return Http.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}
