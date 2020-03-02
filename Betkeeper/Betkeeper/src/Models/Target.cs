﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Betkeeper.Data;
using Betkeeper.Enums;

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

        public int Type { get; set; }

        public DateTime StartTime { get; set; }

        public string Result { get; set; }
    }

    public class Scoring
    {
        public int Points { get; set; }

        public TargetScore Score { get; set; }
    }

    public class TargetRepository : BaseRepository
    {
        public List<Target> GetTargets(int? competitionId = null)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                var query = context.Target.AsQueryable();

                if (competitionId != null)
                {
                    query = query.Where(competition => competition.CompetitionId == competitionId);
                }

                return query.ToList();
            }
        }
    }
}