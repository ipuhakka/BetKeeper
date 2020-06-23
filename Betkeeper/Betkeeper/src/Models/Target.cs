﻿using Betkeeper.Data;
using Betkeeper.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Betkeeper.Models
{
    /// <summary>
    /// Competition bet target.
    /// </summary>
    public class Target
    {
        [Key]
        public int TargetId { get; set; }

        public int CompetitionId { get; set; }

        public List<Scoring> Scoring { get; set; }

        public string Bet { get; set; }

        public TargetType Type { get; set; }

        public string Result { get; set; }
    }

    public class Scoring
    {
        public int Points { get; set; }

        public TargetScore Score { get; set; }
    }

    public class TargetRepository : BaseRepository, IDisposable
    {
        private BetkeeperDataContext _context;

        public TargetRepository()
        {
            _context = new BetkeeperDataContext(OptionsBuilder);
        }

        public TargetRepository(BetkeeperDataContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void AddTarget(Target target)
        {
            _context.Target.Add(target);
            _context.SaveChanges();
        }

        public void AddTargets(List<Target> targets)
        {
            _context.Target.AddRange(targets);
            _context.SaveChanges();
        }

        public List<Target> GetTargets(int? competitionId = null)
        {
            var query = _context.Target.AsQueryable();

            if (competitionId != null)
            {
                query = query.Where(competition => competition.CompetitionId == competitionId);
            }

            return query.ToList();
        }
    }
}
