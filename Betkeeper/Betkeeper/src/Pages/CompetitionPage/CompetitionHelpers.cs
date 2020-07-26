using Betkeeper.Enums;
using Betkeeper.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Betkeeper.Pages.CompetitionPage
{
    /// <summary>
    /// Helper methods for competition page.
    /// </summary>
    public partial class CompetitionPage
    {
        /// <summary>
        /// Converts a list of targets into betTargets JObject.
        /// 
        /// Model: 
        /// [
        ///     bet-target-0: 
        ///     {
        ///         question-0: "Question"
        ///         ...
        ///     },
        ///     bet-target-1:
        ///     ...
        /// ]
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static JArray TargetsToJArray(List<Target> targets)
        {
            JArray targetsJArray = new JArray();

            for (int i = 0; i < targets.Count; i++)
            {
                var outerObject = new JObject
                {
                    { $"bet-target-{i}", GetInnerTargetObject(i, targets[i]) }
                };

                targetsJArray.Add(outerObject);
            }

            return targetsJArray;
        }

        /// <summary>
        /// Converts targets listing into client structured JObject.
        /// 
        /// Model:
        /// 
        /// betTargets:
        ///     bet-target-0:
        ///         bet-type-0: "selection"
        ///     bet-target-1:
		///         bet-type-1: "selection"
        /// </summary>
        /// <param name="targets"></param>
        private static JObject TargetsToJObject(List<Target> targets)
        {
            JObject targetsObject = new JObject();

            for (int i = 0; i < targets.Count; i++)
            {
                var innerObject = GetInnerTargetObject(i, targets[i]);

                targetsObject.Add($"bet-target-{i}", innerObject);
            }

            return targetsObject;
        }

        /// <summary>
        /// Returns inner target object formed from specific target
        /// </summary>
        /// <param name="i"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static JObject GetInnerTargetObject(int i, Target target)
        {
            var valueDict = new Dictionary<string, object>
                {
                    { $"question-{i}", target.Bet },
                    { $"bet-type-{i}", target.Type.ToString() },
                    { $"target-id-{i}", target.TargetId }
                };

            target.Scoring?.ForEach(scoring =>
            {
                var key = scoring.Score == TargetScore.CorrectResult
                    ? $"scoring-{i}"
                    : $"winner-{i}";

                valueDict.Add(key, scoring.Points);
            });

            var innerObject = new JObject();

            foreach (var kvp in valueDict)
            {
                innerObject.Add(kvp.Key, new JValue(kvp.Value));
            }

            innerObject.Add(
                $"selection-{i}",
                target.Selections == null
                    ? (JToken)new JValue((object)null)
                    : new JArray(target.Selections));

            return innerObject;
        }
    }
}
