using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Models;
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
            //TODO: Yksikkötestit
            _BetRepository = _BetRepository ?? new BetRepository();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            // TODO: Pyöristä datetime sekunteihin

            var bet = new Bet(
                Http.GetHttpContent(Request), 
                (int)userId,
                DateTime.Now
                );

            // Lisää veto, palauta 201
            _BetRepository.CreateBet(
                bet.BetResult,
                bet.Name,
                bet.Odd,
                bet.Stake,
                bet.PlayedDate,
                bet.Owner);

            // TODO: Lisää kansioihin jos on.

            return Http.CreateResponse(HttpStatusCode.Created);
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
