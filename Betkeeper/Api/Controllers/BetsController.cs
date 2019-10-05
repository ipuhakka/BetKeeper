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
        public IBetRepository _BetRepository { get; set; }

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
            _BetRepository = _BetRepository ?? new BetRepository();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            { 
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var bets = _BetRepository.GetBets(userId, finished, folder);

            return Http.CreateResponse(HttpStatusCode.OK, bets);
        }

        // POST: api/Bets
        public HttpResponseMessage Post()
        {
            // Authenticate
            _BetRepository = _BetRepository ?? new BetRepository();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            // Read and validate request

            // Käännä tulos?

            // Lisää veto, palauta 201

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
