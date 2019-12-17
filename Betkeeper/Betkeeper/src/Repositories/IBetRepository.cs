using System;
using System.Collections.Generic;
using Betkeeper.Models;

namespace Betkeeper.Repositories
{
    public interface IBetRepository
    {

        /// <summary>
        /// Inserts a new bet to table bets.
        /// </summary>
        /// <returns></returns>
        int CreateBet(
            Enums.BetResult betResult,
            string name,
            double odd,
            double stake,
            DateTime playedDate,
            int userId);

        /// <summary>
        /// Adds bet to folders.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="folders"></param>
        /// <returns>List of folders to which bet was added</returns>
        List<string> AddBetToFolders(int betId, int userId, List<string> folders);

        /// <summary>
        /// Deletes a bet from database.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        int DeleteBet(int betId, int userId);

        /// <summary>
        /// Deletes a bet from specified folders.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="folders"></param>
        /// <returns>List of folders from which bet was deleted</returns>
        List<string> DeleteBetFromFolders(int betId, int userId, List<string> folders);

        /// <summary>
        /// Modifies users existing bet.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="betResult"></param>
        /// <param name="stake"></param>
        /// <param name="odd"></param>
        /// <param name="name"></param>
        /// <exception cref="NotFoundException"></exception>
        int ModifyBet(
            int betId,
            int userId,
            Enums.BetResult betResult = Enums.BetResult.Unresolved,
            double? stake = null,
            double? odd = null,
            string name = null);

        /// <summary>
        /// Mass updates users bets.
        /// </summary>
        /// <param name="betIds"></param>
        /// <param name="userId"></param>
        /// <param name="betResult"></param>
        /// <param name="stake"></param>
        /// <param name="odd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int ModifyBets(
            List<int> betIds,
            int userId,
            Enums.BetResult betResult = Enums.BetResult.Unresolved,
            double? stake = null,
            double? odd = null,
            string name = null);

        /// <summary>
        /// Gets a bet.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        Bet GetBet(int betId, int userId);

        /// <summary>
        /// Gets a list of bets matching parameters
        /// </summary>
        /// <param name="betFinished"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Bet> GetBets(
            int? userId = null,
            bool? betFinished = null,
            string folder = null);
    }
}
