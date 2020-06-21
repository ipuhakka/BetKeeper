using Betkeeper.Classes;
using Betkeeper.Repositories;
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
        public IUserRepository _UserRepository;


        /// <summary>
        ///  Creates a new user.
        /// </summary>
        /// <returns></returns>
        // POST: api/users
        public HttpResponseMessage Post()
        {
            _UserRepository = _UserRepository ?? new UserRepository();

            string username = null;

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
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (_UserRepository.UsernameInUse(username))
            {
                return Http.CreateResponse(HttpStatusCode.Conflict);
            }

            _UserRepository.AddUser(username, password);

            return Http.CreateResponse(HttpStatusCode.Created);
        }
    }
}