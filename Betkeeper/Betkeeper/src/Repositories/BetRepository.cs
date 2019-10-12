using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Betkeeper.Data;
using Betkeeper.Exceptions;
using Betkeeper.Models;

namespace Betkeeper.Repositories
{
    public class BetRepository : IBetRepository
    {
        /// <summary>
        /// Inserts a new bet to table bets.
        /// </summary>
        /// <returns></returns>
        public int CreateBet(
            Enums.BetResult betResult,
            string name,
            double odd,
            double stake,
            DateTime playedDate,
            int userId)
        {
            if (playedDate == null)
            {
                throw new InvalidOperationException(
                    "DateTime cannot be null when creating a new bet");
            }

            if (!new UserRepository().UserIdExists((int)userId))
            {
                throw new NotFoundException("UserId not found");
            }

            string query = "INSERT INTO bets " +
                "(bet_won, name, odd, bet, date_time, owner) " +
                "VALUES (@betWon, @name, @odd, @bet, @dateTime, @owner);";

            return Database.ExecuteCommand(
                query,
                new Dictionary<string, object>
                {
                    {"betWon", (int)betResult },
                    {"name", name },
                    {"odd", odd },
                    {"bet", stake },
                    {"dateTime", playedDate },
                    {"owner", userId }
                },
                returnLastInsertedRowId: true);
        }

        /// <summary>
        /// Adds bet to folders.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="folders"></param>
        /// <returns>List of folders to which bet was added</returns>
        public List<string> AddBetToFolders(int betId, int userId, List<string> folders)
        {
            var addedToFoldersList = new List<string>();
            var queryParameters = new Dictionary<string, object>();
            queryParameters.Add("betId", betId);
            queryParameters.Add("userId", userId);

            var folderModel = new FolderRepository();

            foreach(var folder in folders)
            {
                if (!folderModel.UserHasFolder(userId, folder)
                    || folderModel.FolderHasBet(userId, folder, betId))
                {
                    continue;
                }

                var query = "INSERT INTO bet_in_bet_folder VALUES (@folder, @userId, @betId)";
                queryParameters["folder"] = folder;

                var modifiedRowCount = Database.ExecuteCommand(
                    query,
                    queryParameters);

                if (modifiedRowCount == 1)
                {
                    addedToFoldersList.Add(folder);
                }
            }

            return addedToFoldersList;
        }

        /// <summary>
        /// Deletes a bet from database.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        public int DeleteBet(int betId, int userId)
        {
            if (GetBet(betId, userId) == null)
            {
                throw new NotFoundException("Bet trying to be deleted was not found");
            }

            var query = "DELETE FROM bets WHERE bet_id = @betId";

            return Database.ExecuteCommand(
                query,
                new Dictionary<string, object>
                {
                    { "betId", betId }
                });
        }

        /// <summary>
        /// Deletes a bet from specified folders.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <param name="folders"></param>
        /// <returns>List of folders from which bet was deleted</returns>
        public List<string> DeleteBetFromFolders(int betId, int userId, List<string> folders)
        {
            var deletedFromFoldersList = new List<string>();
            var queryParameters = new Dictionary<string, object>();
            queryParameters.Add("betId", betId);
            queryParameters.Add("userId", userId);

            var folderModel = new FolderRepository();

            foreach(var folder in folders)
            {
                if (!folderModel.UserHasFolder(userId, folder)
                    || !folderModel.FolderHasBet(userId, folder, betId))
                {
                    continue;
                }

                var query = "DELETE FROM bet_in_bet_folder " +
                    "WHERE bet_id = @betId AND folder = @folder";
                queryParameters["folder"] = folder;

                var modifiedRowCount = Database.ExecuteCommand(
                    query,
                    queryParameters);

                if (modifiedRowCount == 1)
                {
                    deletedFromFoldersList.Add(folder);
                }
            }

            return deletedFromFoldersList;
        }

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
        public int ModifyBet(
            int betId,
            int userId,
            Enums.BetResult betResult = Enums.BetResult.Unresolved,
            double? stake = null,
            double? odd = null,
            string name = null)
        {
            var bet = GetBet(betId, userId);

            if (bet == null || bet.Owner != userId)
            {
                throw new NotFoundException("Bet not found");
            }

            var queryParameters = new Dictionary<string, object>();
            var query = "UPDATE bets SET bet_won = @betResult";

            queryParameters.Add("betResult", betResult);                     

            if (stake != null)
            {
                query += ", bet = @stake";
                queryParameters.Add("stake", stake);
            }

            if (odd != null)
            {
                query += ", odd = @odd";
                queryParameters.Add("odd", odd);
            }

            if (name != null)
            {
                query += ", name = @name";
                queryParameters.Add("name", name);
            }

            query += " WHERE bet_id = @betId";
            queryParameters.Add("betId", betId);

            return Database.ExecuteCommand(
                query,
                queryParameters);
        }

        /// <summary>
        /// Gets a bet.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public Bet GetBet(int betId, int userId)
        {
            var query = "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId";

            var betDatatable = Database.ExecuteQuery(
                query,
                new Dictionary<string, object>
                {
                    {"betId", betId },
                    {"userId", userId }
                });

            var betList = DatatableToBetList(betDatatable);

            return betList.Count > 0
                ? betList[0]
                : null;
        }

        /// <summary>
        /// Gets a list of bets matching parameters
        /// </summary>
        /// <param name="betFinished"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Bet> GetBets(
            int? userId = null, 
            bool? betFinished = null,
            string folder = null)
        {
            if (folder != null && userId != null)
            {
                return GetBetsFromFolder((int)userId, folder, betFinished);
            }            

            var query = "SELECT * FROM bets";
            var queryParameters = new Dictionary<string, object>();
            var whereConditions = new List<string>();

            if (userId != null)
            {
                whereConditions.Add("owner=@userId");

                queryParameters.Add("userId", userId);
            }

            if (betFinished != null)
            {
                whereConditions.Add((bool)betFinished
                    ? "bet_won != @betFinished"
                    : "bet_won = @betFinished");

                queryParameters.Add("betFinished", -1);
            }

            if (queryParameters.Count > 0)
            {
                query += " WHERE ";
                query += string.Join(" AND ", whereConditions);           
            }

            return DatatableToBetList(
                Database.ExecuteQuery(
                    query,
                    queryParameters));
        }

        private List<Bet> GetBetsFromFolder(int userId, string folder, bool? betFinished)
        {
            var queryParameters = new Dictionary<string, object>();

            var query = "SELECT * " +
                    "FROM bet_in_bet_folder bf " +
                    "INNER JOIN bets b ON b.bet_id = bf.bet_id " +
                    "WHERE bf.owner = @userId and bf.folder = @folder";

            queryParameters.Add("userId", userId);
            queryParameters.Add("folder", folder);

            if (betFinished != null)
            {
                query += string.Format(" AND {0}", (bool)betFinished
                ? "bet_won != @betFinished"
                : "bet_won = @betFinished");

                queryParameters.Add("betFinished", -1);
            }      

            return DatatableToBetList(
                Database.ExecuteQuery(
                    query,
                    queryParameters));
    }

        private static List<Bet> DatatableToBetList(DataTable datatable)
        {
            var dataRows = datatable.Rows.Cast<DataRow>();

            return dataRows.Select(row =>
                new Bet(row))
                .ToList();
        }
    }
}
