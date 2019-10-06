using System;
using System.Data;
using Microsoft.CSharp.RuntimeBinder;
using Betkeeper.Extensions;
using System.Globalization;

namespace Betkeeper.Models
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

        /// <summary>
        /// Constructor for creating a new bet from dynamic content.
        /// </summary>
        /// <param name="bet"></param>
        /// <param name="userId"></param>
        /// <exception cref="RuntimeBinderException"></exception>
        public Bet(dynamic bet, int userId, DateTime playedDate)
        {
            BetResult = bet.betWon;
            Name = bet.name;
            Odd = bet.odd;
            Stake = bet.stake;
            PlayedDate = playedDate;
            Owner = userId;
        }

        /// <summary>
        /// Converts nullable boolean to BetResult.
        /// </summary>
        /// <param name="betWon"></param>
        /// <returns></returns>
        public static Enums.BetResult GetBetResult(bool? betWon)
        {
            return betWon == null
                ? Enums.BetResult.Unresolved
                : (Enums.BetResult)Convert.ToInt32(betWon);
        }
    }
}
