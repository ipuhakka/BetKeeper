using Betkeeper.Classes;
using Betkeeper.Models;
using Microsoft.CSharp.RuntimeBinder;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        private UserRepository UserRepository { get; set; }

        public UsersController()
        {
            UserRepository = new UserRepository();
        }

        /// <summary>
        ///  Creates a new user.
        /// </summary>
        /// <returns></returns>
        // POST: api/users
        public HttpResponseMessage Post()
        {
            string username;

            try
            {
                username = Http.GetHttpContent(Request).username;
            }
            catch (RuntimeBinderException)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var password = Request.Headers.Authorization?.ToString();

            if (string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password))
            {
                return Http.CreateResponse(
                    HttpStatusCode.BadRequest, 
                    "Empty username or password",
                    Http.ContentType.Text);
            }

            if (UserRepository.UsernameInUse(username))
            {
                return Http.CreateResponse(
                    HttpStatusCode.Conflict,
                    "Username already in use",
                    Http.ContentType.Text);
            }

            UserRepository.AddUser(username, password);

            return Http.CreateResponse(HttpStatusCode.Created);
        }
    }
}