using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Betkeeper.Data;
using Betkeeper.Enums;

namespace Betkeeper.Models
{
    public class TargetBet
    {
        [Key]
        public int TargetBetId { get; set; }

        public int Target { get; set; }

        public int Participator { get; set; }

        public string Bet { get; set; }
    }

    public class TargetBetRepository : BaseRepository, IDisposable
    {
        private readonly BetkeeperDataContext _context;

        public TargetBetRepository()
        {
            _context = new BetkeeperDataContext(OptionsBuilder);
        }

        public TargetBetRepository(BetkeeperDataContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Adds new target bets.
        /// </summary>
        /// <param name="targetBets"></param>
        public void AddTargetBets(List<TargetBet> targetBets)
        {
            _context.TargetBet.AddRange(targetBets);
            _context.SaveChanges();
        }
    }
}
