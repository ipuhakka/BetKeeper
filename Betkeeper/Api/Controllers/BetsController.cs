using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Betkeeper.Models;
using Betkeeper.Repositories;
using Betkeeper.Classes;
using Betkeeper.Exceptions;
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
        /// <summary>
        ///  Creates a new bet and adds it to folders if specified.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Post()
        {
            _BetRepository = _BetRepository ?? new BetRepository();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            Bet bet;
            var content = Http.GetHttpContent(Request);

            try
            {
                bet = new Bet(
                    content,
                    (int)userId,
                    DateTime.Now
                    );                        
            }
            catch (RuntimeBinderException)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var betId = _BetRepository.CreateBet(
                bet.BetResult,
                bet.Name,
                bet.Odd,
                bet.Stake,
                bet.PlayedDate,
                bet.Owner);

            if (bet.Folders != null && bet.Folders.Count > 0)
            {
                _BetRepository.AddBetToFolders(betId, (int)userId, bet.Folders);
            }

            return Http.CreateResponse(HttpStatusCode.Created);
        }

        // PUT: api/Bets/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Bets/5
        public HttpResponseMessage Delete(
            int id, [FromUri]
            List<string> folders = null)
        {
            _BetRepository = _BetRepository ?? new BetRepository();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (folders == null || folders.Count == 0)
            {
                return DeleteCompletely(betId: id, userId: (int)userId);
            }
            else
            {
                return DeleteFromFolders(
                    betId: id,
                    userId: (int)userId,
                    folders: folders);
            }
        }

        private HttpResponseMessage DeleteCompletely(int betId, int userId)
        {
            try
            {
                _BetRepository.DeleteBet(betId: betId, userId: userId);
            }
            catch (NotFoundException)
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            return Http.CreateResponse(HttpStatusCode.NoContent);
        }

        private HttpResponseMessage DeleteFromFolders(
            int betId, 
            int userId, 
            List<string> folders)
        {
            var deletedFromFolders = _BetRepository.DeleteBetFromFolders(
                    betId: betId,
                    userId: userId,
                    folders: folders);

            if (deletedFromFolders.Count == 0)
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            return Http.CreateResponse(HttpStatusCode.OK);
        }
    }
}
