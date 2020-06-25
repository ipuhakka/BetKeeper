using Betkeeper.Models;
using Newtonsoft.Json.Linq;
using System;
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
                var valueDict = new Dictionary<string, object>
                {
                    { $"question-{i}", targets[i].Bet },
                    { $"bet-type-{i}", targets[i].Type.ToString() }
                };

                targets[i].Scoring?.ForEach(scoring =>
                {
                    var key = scoring.Score == Enums.TargetScore.CorrectResult
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
                    targets[i].Selections == null
                        ? (JToken)new JValue((object)null)
                        : new JArray(targets[i].Selections));

                var outerObject = new JObject
                {
                    { $"bet-target-{i}", innerObject }
                };

                targetsJArray.Add(outerObject);
            }

            return targetsJArray;
        }
    }
}
