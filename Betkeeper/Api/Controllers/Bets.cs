using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Exceptions;
using Betkeeper.Actions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Models;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BetsController : ApiController
    {
        private BetAction BetAction { get; }

        private FolderAction FolderAction { get; }

        public BetsController()
        {
            BetAction = new BetAction();
            FolderAction = new FolderAction();
        }

        // GET: api/Bets
        /// <summary>
        /// Gets bets.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="finished">Should only finished, not finished, or all bets be returned</param>
        /// <returns></returns>
        public HttpResponseMessage Get(
            [FromUri]bool? finished = null,
            [FromUri]string folder = null)
        {
            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var bets = BetAction.GetBets(userId.Value, finished, folder);

            return Http.CreateResponse(HttpStatusCode.OK, bets);
        }

        // POST: api/Bets
        /// <summary>
        ///  Creates a new bet and adds it to folders if specified.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Post()
        {
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
                    DateTime.UtcNow);
            }
            catch (ParsingException)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var betId = BetAction.CreateBet(
                bet.BetResult,
                bet.Name,
                (double)bet.Odd,
                (double)bet.Stake,
                bet.PlayedDate,
                bet.Owner);

            if (bet.Folders != null && bet.Folders.Count > 0)
            {
                FolderAction.AddBetToFolders((int)userId, betId, bet.Folders);
            }

            return Http.CreateResponse(HttpStatusCode.Created);
        }

        // PUT: api/Bets/5
        /// <summary>
        /// Updates a bet.
        /// </summary>
        /// <param name="id">betId</param>
        /// <returns></returns>
        public HttpResponseMessage Put(int id)
        {
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
                    (int)userId
                    );
            }
            catch (ParsingException)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                BetAction.ModifyBet(
                    id,
                    (int)userId,
                    bet.BetResult,
                    bet.Stake,
                    bet.Odd,
                    bet.Name);
            }
            catch (ActionException actionException)
            {
                return Http.CreateResponse(
                    (HttpStatusCode)actionException.ActionExceptionType,
                    actionException.ErrorMessage);
            }

            if (bet.Folders != null && bet.Folders.Count > 0)
            {
                FolderAction.AddBetToFolders(
                    (int)userId,
                    id,
                    bet.Folders);
            }

            return Http.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Mass update for bets.
        /// </summary>
        /// <param name="betIds"></param>
        /// <returns></returns>
        public HttpResponseMessage Put([FromUri]List<int> betIds)
        {
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
                    (int)userId
                    );
            }
            catch (ParsingException)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                betIds.ForEach(betId =>
                {
                    BetAction.ModifyBet(
                        betId,
                        bet.Owner,
                        bet.BetResult,
                        bet.Stake,
                        bet.Odd,
                        bet.Name);
                });
            }
            catch (ActionException actionException)
            {
                return Http.CreateResponse(
                    (HttpStatusCode)actionException.ActionExceptionType,
                    actionException.ErrorMessage);
            }

            // TODO: Kansioihin lisäys?

            return Http.CreateResponse(HttpStatusCode.OK, "Bets updated successfully");
        }

        // DELETE: api/Bets/5
        /// <summary>
        /// Deletes a bet
        /// </summary>
        /// <param name="id">Bet id</param>
        /// <param name="folders">List of folders from where bet is deleted.</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(
            int id, [FromUri]
            List<string> folders = null)
        {
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
                BetAction.DeleteBet(betId: betId, userId: userId);
            }
            catch (ActionException actionException)
            {
                return Http.CreateResponse(
                    (HttpStatusCode)actionException.ActionExceptionType,
                    actionException.ErrorMessage);
            }

            return Http.CreateResponse(HttpStatusCode.NoContent);
        }

        private HttpResponseMessage DeleteFromFolders(
            int betId,
            int userId,
            List<string> folders)
        {
            try
            {
                FolderAction.DeleteBetFromFolders(
                    betId: betId,
                    userId: userId,
                    folders: folders);
            }
            catch (ActionException actionException)
            {
                return Http.CreateResponse(
                    (HttpStatusCode)actionException.ActionExceptionType,
                    actionException.ErrorMessage);
            }

            return Http.CreateResponse(HttpStatusCode.OK);
        }
    }
}
