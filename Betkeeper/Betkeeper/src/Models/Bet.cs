using Betkeeper.Exceptions;
using Betkeeper.Extensions;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Betkeeper.Data;
using Betkeeper.Enums;
using System.ComponentModel.DataAnnotations;

namespace Betkeeper.Models
{
    [Table("bets")]
    public class Bet
    {
        [Column("bet_won")]
        public Enums.BetResult BetResult { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("odd")]
        public double? Odd { get; set; }

        [Column("bet")]
        public double? Stake { get; set; }

        [Column("date_time")]
        public DateTime PlayedDate { get; set; }

        [Column("owner")]
        public int Owner { get; set; }

        [Key]
        [Column("bet_id")]
        public int BetId { get; set; }

        public List<string> Folders { get; }

        public Bet()
        {

        }

        public Bet(
            BetResult betResult,
            string name,
            double odd,
            double stake,
            DateTime playedDate,
            int userId)
        {
            BetResult = betResult;
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
        /// Constructor for creating a bet with modify data.
        /// Returns null if bet cannot be created.
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

        public static BetResult GetBetResult(bool? betResult)
        {
            if (betResult == null)
            {
                return BetResult.Unresolved;
            }

            return betResult.Value
                ? BetResult.Won
                : BetResult.Lost;
        }
    }

    public class BetRepository
    {
        private BetkeeperDataContext _context { get; set; }

        public BetRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public List<Bet> GetBets(
            int userId,
            bool? betFinished = null,
            string folder = null)
        {
            var query = _context.Bet.Where(bet => bet.Owner == userId);

            if (betFinished == true)
            {
                query = query.Where(bet => bet.BetResult == BetResult.Lost
                    || bet.BetResult == BetResult.Won);
            }
            else if (betFinished == false)
            {
                query = query.Where(bet => bet.BetResult == BetResult.Unresolved);
            }

            if (folder != null)
            {
                var foldersBetIds = new FolderRepository().GetBetIdsFromFolder(userId, folder);

                query = query.Where(bet => foldersBetIds.Contains(bet.BetId));
            }

            return query.ToList();
        }

        public Bet GetBet(int betId, int userId)
        {
            return _context.Bet.SingleOrDefault(bet => bet.BetId == betId
                && bet.Owner == userId);
        }

        public int CreateBet(Bet bet)
        {
            _context.Bet.Add(bet);
            _context.SaveChanges();
            return bet.BetId;
        }

        public void ModifyBet(Bet bet)
        {
            _context.Bet.Update(bet);
            _context.SaveChanges();
        }

        public void DeleteBet(int betId, int userId)
        {
            _context.Bet.RemoveRange(
                _context.Bet.Where(bet =>
                    bet.BetId == betId && bet.Owner == userId));

            _context.SaveChanges();
        }
    }
}
