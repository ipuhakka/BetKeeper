using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Repositories;
using Betkeeper.Data;
using Api.Classes;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BetsController : ApiController
    {
        // GET: api/Bets
        /// <summary>
        /// Gets bets.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="finished"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(
            [FromUri]bool? finished = null,
            [FromUri]string folder = null)
        {
            // TODO: Testit, interfaceimplementointi,
            // refaktorointi (bet pois modelin sisältä?)
            // Implementoi kansiohaku

            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null
                || !TokenLog.ContainsToken(tokenString))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var userId = TokenLog.GetTokenOwner(tokenString);

            var bets = new BetRepository().GetBets(userId, finished);

            return Http.CreateResponse(HttpStatusCode.OK, bets);
        }

        // POST: api/Bets
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT: api/Bets/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Bets/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
