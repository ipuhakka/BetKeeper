using Betkeeper.Data;
using Betkeeper.Enums;
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

    public class TargetRepository : BaseRepository
    {
        public void AddTarget(Target target)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                context.Target.Add(target);
                context.SaveChanges();
            }
        }

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
