using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        protected UserRepository UserRepository { get; set; }

        public TokenController()
        {
            UserRepository = new UserRepository();
        }

        // GET: api/Token/5
        /// <summary>
        /// Checks if user authentication token is still valid.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("api/token/{userId}")]
        public IActionResult Get(int userId)
        {
            Request.Headers.TryGetValue("Authorization", out var authorization);

            var tokenString = authorization.ToString();

            if (string.IsNullOrEmpty(tokenString))
            {
                return BadRequest();
            }

            if (!TokenLog.ContainsToken(tokenString))
            {
                return NotFound();
            }

            if (TokenLog.GetTokenOwner(tokenString) == userId)
            {
                return Ok();
            }

            return Unauthorized();
        }

        // POST: api/Token
        /// <summary>
        /// Post login-info.
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns>Token used for accessing the system.
        /// </returns>
        public IActionResult Post()
        {
            Request.Headers.TryGetValue("Authorization", out var authorization);

            var password = authorization.ToString();

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest();
            }

            var username = Http.GetRequestBody<Dictionary<string, object>>(Request)["username"];
            var userId = UserRepository.GetUserId(username.ToString());

            if (userId == null || !UserRepository.Authenticate((int)userId, password))
            {
                return Unauthorized();
            }

            var token = TokenLog.GetExistingToken((int)userId)
                ?? TokenLog.CreateToken((int)userId);

            return Ok(token);
        }

        // DELETE: api/Token/5
        [HttpGet("{id}")]
        public IActionResult Delete(int userId)
        {
            Request.Headers.TryGetValue("Authorization", out var authorization);
            var tokenString = authorization.ToString();

            if (string.IsNullOrEmpty(tokenString))
            {
                return BadRequest();
            }

            if (TokenLog.GetTokenOwner(tokenString) == userId)
            {
                TokenLog.DeleteToken(userId, tokenString);

                return NoContent();
            }

            return Unauthorized();
        }
    }
}
