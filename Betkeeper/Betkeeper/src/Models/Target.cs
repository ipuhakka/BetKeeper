using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Column("Competition")]
        public int CompetitionId { get; set; }

        public List<Scoring> Scoring { get; set; }

        public string Bet { get; set; }

        public TargetType Type { get; set; }

        public string Result { get; set; }

        public List<string> Selections { get; set; }

        /// <summary>
        /// Converts JArray into target list
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        public static List<Target> JArrayToTargets(JArray jArray)
        {
            var targets = new List<Target>();

            for (var i = 0; i < jArray.Count; i++)
            {
                targets.Add(FromJObject((JObject)jArray[i], i));
            }

            return targets;
        }

        /// <summary>
        /// Gets target from jObject.
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="i">Index used in keys</param>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public static Target FromJObject(JObject jObject, int i, int competitionId = 0)
        {
            var targetObject = jObject[$"bet-target-{i}"] as JObject;

            var type = EnumHelper.FromString<TargetType>(targetObject[$"bet-type-{i}"].ToString());

            targetObject.TryGetValue($"question-{i}", out JToken questionJToken);
            targetObject.TryGetValue($"scoring-{i}", out JToken scoringJToken);

            List<Scoring> scorings = new List<Scoring>();

            if (!scoringJToken.IsNullOrWhiteSpace())
            {
                scorings.Add(new Scoring
                {
                    Score = TargetScore.CorrectResult,
                    Points = scoringJToken.GetDoubleInvariantCulture()
                });
            }

            if (type == TargetType.Result)
            {
                targetObject.TryGetValue($"winner-{i}", out JToken winnerJToken);

                if (!winnerJToken.IsNullOrWhiteSpace())
                {
                    scorings.Add(new Scoring
                    {
                        Score = TargetScore.CorrectWinner,
                        Points = winnerJToken.GetDoubleInvariantCulture()
                    });
                }
            }

            List<string> selections = null;
            if (type == TargetType.Selection)
            {
                targetObject.TryGetValue($"selection-{i}", out JToken selectionsToken);

                if (selectionsToken != null)
                {
                    selections = selectionsToken.ToObject<List<string>>();
                }
            }

            return new Target
            {
                Type = type,
                Bet = questionJToken?.ToString(),
                CompetitionId = competitionId,
                Scoring = scorings,
                Selections = selections
            };
        }

        /// <summary>
        /// Helper method to check if target has a specific scoring type with points set
        /// </summary>
        /// <param name="scoring"></param>
        /// <returns></returns>
        public bool HasScoringType(TargetScore scoring)
        {
            return Scoring.Any(score => score.Score == scoring && score.Points != null);
        }
    }

    public class Scoring
    {
        /// <summary>
        /// How many points scoring provides
        /// </summary>
        public double? Points { get; set; }

        /// <summary>
        /// Score type
        /// </summary>
        public TargetScore Score { get; set; }
    }

    public class TargetRepository : BaseRepository, IDisposable
    {
        private readonly BetkeeperDataContext _context;

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

        /// <summary>
        /// Deletes targets from competition.
        /// </summary>
        /// <param name="competitionId"></param>
        public void ClearTargets(int competitionId)
        {
            var competitionTargets = _context.Target
                .AsQueryable()
                .Where(target => target.CompetitionId == competitionId);

            _context.Target.RemoveRange(competitionTargets);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes specified target
        /// </summary>
        /// <param name="targetId"></param>
        public void RemoveTarget(int targetId)
        {
            var targetToRemove = _context.Target.FirstOrDefault(target => target.TargetId == targetId);

            if (targetToRemove == null)
            {
                return;
            }

            _context.Target.Remove(targetToRemove);
            _context.SaveChanges();
        }
    }
}
