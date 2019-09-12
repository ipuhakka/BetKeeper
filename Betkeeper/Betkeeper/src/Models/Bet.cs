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

        public double Odd { get; set; }

        public double Stake { get; set; }

        public DateTime? PlayedDate { get; set; }

        public int Owner { get; set; }

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

        /// <summary>
        /// Deletes a bet from database.
        /// </summary>
        /// <param name="betId"></param>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        public static int DeleteBet(int betId, int userId)
        {
            throw new NotImplementedException();
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
