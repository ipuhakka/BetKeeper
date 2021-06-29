using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Extensions;
using Newtonsoft.Json;
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

        public Scoring Scoring { get; set; }

        public string Bet { get; set; }

        public TargetType Type { get; set; }

        public TargetResultItem Result { get; set; }

        public List<string> Selections { get; set; }

        public string Grouping { get; set; }

        /// <summary>
        /// Number of selections possible to make for multiselection target
        /// </summary>
        public int? AllowedSelectionCount { get; set; }

        /// <summary>
        /// Returns points for a given target bet
        /// </summary>
        /// <param name="targetBet"></param>
        /// <returns></returns>
        public double GetPoints(TargetBet targetBet)
        {
            if (Type == TargetType.MultiSelection)
            {
                return GetMultiSelectionTargetPoints(targetBet);
            }

            var result = GetResult(targetBet);

            if (result == TargetResult.CorrectResult)
            {
                return (double)Scoring.PointsForCorrectResult;
            }

            if (result == TargetResult.CorrectWinner)
            {
                return (double)Scoring.PointsForCorrectWinner;
            }

            return 0;
        }

        /// <summary>
        /// Calculate target bet points for a multi selection bet
        /// </summary>
        /// <param name="bet"></param>
        /// <returns></returns>
        public double GetMultiSelectionTargetPoints(TargetBet bet)
        {
            var pointsPerCorrectBet = Scoring.PointsForCorrectResult;

            if (string.IsNullOrEmpty(bet.Bet))
            {
                return 0;
            }

            var selections = JsonConvert.DeserializeObject<List<string>>(bet.Bet);

            return (selections.Count(selection => Result?.MultiSelectionResult?.Contains(selection) ?? false) * pointsPerCorrectBet) ?? 0;
        }

        /// <summary>
        /// Returns a dictionary for bets and their correctness
        /// </summary>
        /// <param name="bet"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetMultiSelectionBetTargetResults(TargetBet bet)
        {
            if (string.IsNullOrEmpty(bet.Bet))
            {
                return new Dictionary<string, bool>();
            }

            var selections = JsonConvert.DeserializeObject<List<string>>(bet.Bet);

            return selections.ToDictionary(
                selection => selection,
                selection => Result?.MultiSelectionResult?.Contains(selection) ?? false);
        }

        /// <summary>
        /// Returns result for bet
        /// </summary>
        /// <param name="targetBet"></param>
        /// <returns></returns>
        public TargetResult GetResult(TargetBet targetBet)
        {
            // No results given
            if (!TargetResultSet())
            {
                return TargetResult.Unresolved;
            }

            if (Type == TargetType.OpenQuestion)
            {
                return Result.TargetBetResultDictionary.TryGetValue(targetBet.TargetBetId, out string result)
                    && result == "Correct"
                    ? TargetResult.CorrectResult
                    : TargetResult.Wrong;
            }

            if (Type == TargetType.Result)
            {
                var resultArray = Result.Result.Split('-');
                var homescore = int.Parse(resultArray[0]);
                var awayscore = int.Parse(resultArray[1]);

                var betResultsArray = targetBet.Bet.Split('-');
                var betHomescore = int.Parse(betResultsArray[0]);
                var betAwayscore = int.Parse(betResultsArray[1]);

                if (homescore == betHomescore && awayscore == betAwayscore)
                {
                    // Result correct
                    return TargetResult.CorrectResult;
                }
                else if (Math.Sign(homescore - awayscore) == Math.Sign(betHomescore - betAwayscore))
                {
                    // Winner correct
                    return TargetResult.CorrectWinner;
                }

                return TargetResult.Wrong;
            }

            // Selection
            if (Type == TargetType.Selection && targetBet.Bet == Result.Result)
            {
                return TargetResult.CorrectResult;
            }

            if (Type == TargetType.MultiSelection && !string.IsNullOrEmpty(targetBet.Bet))
            {
                var betList = JsonConvert.DeserializeObject<List<string>>(targetBet.Bet);

                var correctCount = betList.Count(bet => Result.MultiSelectionResult?.Contains(bet) ?? false);

                if (correctCount == AllowedSelectionCount)
                {
                    return TargetResult.CorrectResult;
                }

                if (correctCount > 0)
                {
                    return TargetResult.CorrectWinner;
                }
            }

            return TargetResult.Wrong;
        }

        /// <summary>
        /// Has target result been set
        /// </summary>
        /// <returns></returns>
        public bool TargetResultSet()
        {
            if (Result == null)
            {
                return false;
            }

            if (Type == TargetType.OpenQuestion)
            {
                var resolvedCount = Result.TargetBetResultDictionary?.Count(kvp => kvp.Value != "Unresolved") ?? 0;
                return resolvedCount > 0 && resolvedCount == Result.TargetBetResultDictionary.Count;
            }

            if (Type == TargetType.Selection)
            {
                return !string.IsNullOrEmpty(Result.Result) && Result.Result != "UNRESOLVED-BET";
            }

            if (Type == TargetType.MultiSelection)
            {
                return (Result.MultiSelectionResult?.Count ?? 0) == AllowedSelectionCount;
            }

            return !string.IsNullOrEmpty(Result.Result);
        }

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
            targetObject.TryGetValue($"grouping-{i}", out JToken groupingJToken);

            var scoring = new Scoring();

            if (!scoringJToken.IsNullOrWhiteSpace())
            {
                scoring.PointsForCorrectResult = scoringJToken.GetDoubleInvariantCulture();
            }

            if (type == TargetType.Result)
            {
                targetObject.TryGetValue($"winner-{i}", out JToken winnerJToken);

                if (!winnerJToken.IsNullOrWhiteSpace())
                {
                    scoring.PointsForCorrectWinner = winnerJToken.GetDoubleInvariantCulture();
                }
            }

            List<string> selections = null;
            if (type == TargetType.Selection || type == TargetType.MultiSelection)
            {
                targetObject.TryGetValue($"selection-{i}", out JToken selectionsToken);

                if (selectionsToken != null)
                {
                    selections = selectionsToken.ToObject<List<string>>();
                }
            }

            int? allowedSelectionCount = null;
            if (type == TargetType.MultiSelection)
            {
                targetObject.TryGetValue($"selection-count-{i}", out JToken selectionCountToken);

                if (selectionCountToken != null)
                {
                    allowedSelectionCount = int.TryParse(selectionCountToken.ToString(), out var selectionCount)
                        ? selectionCount
                        : null;
                }
            }

            int targetId = targetObject.TryGetValue(
                $"target-id-{i}", 
                out JToken targetIdToken)
                ? targetIdToken.Value<int>()
                : 0;
            
            return new Target
            {
                Type = type,
                Bet = questionJToken?.ToString(),
                CompetitionId = competitionId,
                Scoring = scoring,
                Selections = selections,
                TargetId = targetId,
                Grouping = groupingJToken?.ToString(),
                AllowedSelectionCount = allowedSelectionCount
            };
        }

        /// <summary>
        /// Returns point information as string for target.
        /// </summary>
        /// <returns></returns>
        public string GetPointInformation()
        {
            // Score term for CorrectResult typed score.
            var scoreTerm = Type != TargetType.Result
                ? "Correct"
                : "Result";

            if (Type == TargetType.MultiSelection)
            {
                return $"Points: {Scoring.PointsForCorrectResult} per correct answer";
            }

            if (Type == TargetType.Result)
            {
                return $"Result: {Scoring.PointsForCorrectResult} points, Winner: {Scoring.PointsForCorrectWinner} points";
            }

            return $"{scoreTerm}: {Scoring.PointsForCorrectResult} points";
        }
    }

    public class Scoring
    {
        public double? PointsForCorrectResult { get; set; }

        public double? PointsForCorrectWinner { get; set; }
    }

    public class TargetResultItem
    {
        /// <summary>
        /// Result property for targets typed selection and result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Stores target bets and their results.
        /// </summary>
        public Dictionary<int, string> TargetBetResultDictionary { get; set; }

        /// <summary>
        /// Correct result listing for multi selection targets
        /// </summary>
        public List<string> MultiSelectionResult { get; set; }

        [JsonIgnore]
        public int TargetId { get; }

        [JsonConstructor]
        public TargetResultItem()
        {

        }

        /// <summary>
        /// Create a target result item from JObject.
        /// </summary>
        /// <param name="jObject"></param>
        public TargetResultItem(JObject jObject)
        {
            TargetId = jObject.GetIdentifierValueFromKeyLike("setResultsContainer-");

            var innerObject = jObject[$"setResultsContainer-{TargetId}"] as JObject;

            var targetType = (TargetType)int.Parse(innerObject["type"].ToString());

            if (targetType == TargetType.OpenQuestion)
            {
                TargetBetResultDictionary = new Dictionary<int, string>();
                // Handle open questions
                innerObject
                .GetKeysLike("result-")
                    .ForEach(key =>
                    {
                        var targetBetId = innerObject.GetIdentifierValueFromKeyLike(key);
                        TargetBetResultDictionary.Add(targetBetId, innerObject[key].ToString());
                    });
            }
            else if (targetType == TargetType.MultiSelection)
            {
                MultiSelectionResult = innerObject[$"result-{TargetId}"].ToObject<List<string>>();
            }
            else
            {
                // Selections and result bets
                Result = innerObject[$"result-{TargetId}"].ToString();
            }
        }
    }

    public class TargetRepository
    {
        private readonly BetkeeperDataContext _context;

        public TargetRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
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

        /// <summary>
        /// Updates target bets.
        /// </summary>
        /// <param name="targets"></param>
        public void UpdateTargets(List<Target> targets)
        {
            _context.Target.UpdateRange(targets);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get targets.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="targetIds"></param>
        /// <returns></returns>
        public List<Target> GetTargets(int? competitionId = null, List<int> targetIds = null)
        {
            var query = _context.Target.AsQueryable();

            if (competitionId != null)
            {
                query = query.Where(target => target.CompetitionId == competitionId);
            }

            if (targetIds != null)
            {
                query = query.Where(target => targetIds.Contains(target.TargetId));
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
