using Betkeeper.Classes;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using Betkeeper.Extensions;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private UserRepository UserRepository { get; set; }

        public UsersController()
        {
            UserRepository = new UserRepository();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <returns></returns>
        // POST: api/users
        public IActionResult Post()
        {
            var body = Http.GetRequestBody<Dictionary<string, object>>(Request);
            var username = body?.GetString("username");

            Request.Headers.TryGetValue("Authorization", out var authorization);

            var password = authorization.ToString();

            if (string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password))
            {
                return BadRequest("Empty username or password");
            }

            if (UserRepository.UsernameInUse(username))
            {
                return Conflict("Username already in use");
            }

            UserRepository.AddUser(username, password);

            return Ok();
        }
    }
}