using Betkeeper.Enums;
using Betkeeper.Models;
using System;
using System.Collections.Generic;

namespace Betkeeper.Actions
{
    public class BetAction
    {
        private BetRepository BetRepository { get; }

        private UserRepository UserRepository { get; }

        public BetAction()
        {
            BetRepository = new BetRepository();
            UserRepository = new UserRepository();
        }

        /// <summary>
        /// Get bets by user, result, and folder
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betFinished"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public List<Bet> GetBets(
            int userId,
            bool? betFinished = null,
            string folder = null)
        {
            return BetRepository.GetBets(
                userId,
                betFinished,
                folder);
        }

        /// <summary>
        /// Get a specific bet by user
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Bet GetBet(int betId, int userId)
        {
            return BetRepository.GetBet(betId, userId);
        }

        /// <summary>
        /// Create a new bet
        /// </summary>
        /// <param name="betResult"></param>
        /// <param name="name"></param>
        /// <param name="odd"></param>
        /// <param name="stake"></param>
        /// <param name="playedDate"></param>
        /// <param name="userId"></param>
        public int CreateBet(
            BetResult betResult,
            string name,
            double odd,
            double stake,
            DateTime playedDate,
            int userId)
        {
            if (UserRepository.GetUsersById(new List<int> { userId }).Count == 0)
            {
                throw new ActionException(
                    ActionExceptionType.Conflict,
                    "User not found");
            }

            return BetRepository.CreateBet(
                new Bet
                {
                    Owner = userId,
                    BetResult = betResult,
                    Stake = stake,
                    Odd = odd,
                    PlayedDate = playedDate,
                    Name = name
                });
        }

        public void DeleteBet(int betId, int userId)
        {
            if (GetBet(betId, userId) == null)
            {
                throw new ActionException(
                    ActionExceptionType.NotFound,
                    "Bet trying to be deleted was not found");
            }

            BetRepository.DeleteBet(betId, userId);
        }

        /// <summary>
        /// Modifies an existing bet
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="betResult"></param>
        /// <param name="stake"></param>
        /// <param name="odd"></param>
        /// <param name="name"></param>
        public void ModifyBet(
            int betId,
            int userId,
            BetResult? betResult = null,
            double? stake = null,
            double? odd = null,
            string name = null)
        {
            var bet = GetBet(betId, userId);

            if (bet == null)
            {
                throw new ActionException(
                    ActionExceptionType.NotFound,
                    "Bet not found");
            }

            if (betResult != null)
            {
                bet.BetResult = betResult.Value;
            }

            if (stake != null)
            {
                bet.Stake = stake.Value;
            }

            if (odd != null)
            {
                bet.Odd = odd.Value;
            }

            if (name != null)
            {
                bet.Name = name;
            }

            BetRepository.ModifyBet(bet);
        }
    }
}
