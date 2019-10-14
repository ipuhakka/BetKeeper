using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.CSharp.RuntimeBinder;
using Betkeeper.Extensions;
using Betkeeper.Exceptions;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Models
{
    public class Bet
    {
        public Enums.BetResult BetResult { get; set; }

        public string Name { get; set; }

        public double? Odd { get; set; }

        public double? Stake { get; set; }

        public DateTime PlayedDate { get; set; }

        public int Owner { get; set; }

        public int BetId { get; }

        public List<string> Folders { get; }

        public Bet(
            bool? betResult,
            string name,
            double odd,
            double stake,
            DateTime playedDate,
            int userId)
        {
            BetResult = GetBetResult(betResult);
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
        /// <exception cref="ParsingException"></exception>
        public Bet(dynamic bet, int userId, DateTime playedDate)
        {
            try
            {
                BetResult = bet.betResult;
                Name = bet.name;
                Odd = bet.odd;

                Stake = bet.stake;
                PlayedDate = playedDate;
                Owner = userId;

                if (bet.folders is JArray)
                {
                    Folders = bet.folders.ToObject<List<string>>();
                }
            }
            catch (FormatException)
            {
                throw new ParsingException("Parsing dynamic bet content failed");
            }
            catch (RuntimeBinderException)
            {
                throw new ParsingException("Parsing dynamic bet content failed");
            }

            if (Stake == null || Odd == null)
            {
                throw new ParsingException("Empty parameters");
            }
        }

        /// <summary>
        /// Constructor for bet to be modified
        /// </summary>
        /// <param name="bet"></param>
        /// <param name="userId"></param>
        /// <exception cref="ParsingException"></exception>
        public Bet(dynamic bet, int userId)
        {
            try
            {
                if (!(bet.betResult is null))
                {
                    BetResult = bet.betResult;
                }
                else
                {
                    BetResult = Enums.BetResult.Unresolved;
                }

                if (!(bet.name is null))
                {
                    Name = bet.name;
                }

                if (!(bet.odd is null))
                {
                    Odd = bet.odd;
                }

                if (!(bet.stake is null))
                {
                    Stake = bet.stake;
                }

                Owner = userId;

                if (bet.folders is JArray)
                {
                    Folders = bet.folders.ToObject<List<string>>();
                }
            }
            catch (FormatException)
            {
                throw new ParsingException("Parsing dynamic bet content failed");
            }
            catch (RuntimeBinderException)
            {
                throw new ParsingException("Parsing dynamic bet content failed");
            }
        }

        /// <summary>
        /// Converts nullable boolean to BetResult.
        /// </summary>
        /// <param name="betResult"></param>
        /// <returns></returns>
        public static Enums.BetResult GetBetResult(bool? betResult)
        {
            return betResult == null
                ? Enums.BetResult.Unresolved
                : (Enums.BetResult)Convert.ToInt32(betResult);
        }
    }
}
