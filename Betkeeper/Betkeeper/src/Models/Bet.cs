using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Betkeeper.Data;
using Betkeeper.Extensions;
using Betkeeper.Exceptions;

namespace Betkeeper.Models
{
    public class Bet
    {
        public Enums.BetResult BetResult { get; set; }

        public string Name { get; set; }

        public double? Odd { get; set; }

        public double? Stake { get; set; }

        public DateTime? PlayedDate { get; set; }

        public int? Owner { get; set; }

        public int BetId { get; }

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

        /// <summary>
        /// Inserts a new bet to table bets.
        /// </summary>
        /// <returns></returns>
        public int CreateBet()
        {
            if (Odd == null
                || Stake == null
                || Owner == null
                || PlayedDate == null)
            {
                throw new InvalidOperationException("Missing required parameters in bet");
            }

            // TODO: Tarkista onko owneria?

            // TODO: Luo veto

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a bet from database.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        public static int DeleteBet(int betId, int userId)
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
        /// Gets a bet.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public static Bet GetBet(int betId, int userId)
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
