using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Betkeeper.Data;
using Betkeeper.Extensions;
using Betkeeper.Exceptions;

namespace Betkeeper.Models
{
    public class BetModel
    {
        public class Bet
        {
            public Enums.BetResult BetResult { get; set; }

            public string Name { get; set; }

            public double Odd { get; set; }

            public double Stake { get; set; }

            public DateTime PlayedDate { get; set; }

            public int Owner { get; set; }

            public int BetId { get; }

            public Bet(
                bool? betWon,
                string name,
                double odd,
                double stake,
                DateTime playedDate,
                int userId)
            {
                BetResult = GetBetResult(betWon);
                Name = name;
                Odd = odd;
                Stake = stake;
                PlayedDate = playedDate;
                Owner = userId;
            }
            public Bet(DataRow row)
            {
                BetResult = row.ToBetResult("bet_won");
                Name = row["name"].ToString();
                Odd = row.ToDouble("odd");
                Stake = row.ToDouble("bet");
                PlayedDate = row.ToDateTime("date_time");
                BetId = row.ToInt32("bet_id");
                Owner = row.ToInt32("owner");
            }
        }

        /// <summary>
        /// Inserts a new bet to table bets.
        /// </summary>
        /// <returns></returns>
        public int CreateBet(
            bool? betWon,
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

            if (!new UserModel().UserIdExists((int)userId))
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
                    {"betWon", (int)GetBetResult(betWon) },
                    {"name", name },
                    {"odd", odd },
                    {"bet", stake },
                    {"dateTime", playedDate },
                    {"owner", userId }
                });
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

            var folderModel = new FolderModel();

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

            var folderModel = new FolderModel();

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
        /// <param name="betWon"></param>
        /// <param name="stake"></param>
        /// <param name="odd"></param>
        /// <param name="name"></param>
        /// <exception cref="NotFoundException"></exception>
        public int ModifyBet(
            int betId,
            int userId,
            bool? betWon = null,
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

            queryParameters.Add("betResult", GetBetResult(betWon));

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
        public List<Bet> GetBets(int? userId = null, bool? betFinished = null)
        {
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

        private static List<Bet> DatatableToBetList(DataTable datatable)
        {
            var dataRows = datatable.Rows.Cast<DataRow>();

            return dataRows.Select(row =>
                new Bet(row))
                .ToList();
        }

        /// <summary>
        /// Converts nullable boolean to BetResult.
        /// </summary>
        /// <param name="betWon"></param>
        /// <returns></returns>
        private static Enums.BetResult GetBetResult(bool? betWon)
        {
            return betWon == null
                ? Enums.BetResult.Unresolved
                : (Enums.BetResult)Convert.ToInt32(betWon);
        }
    }
}
