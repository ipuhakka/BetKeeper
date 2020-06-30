using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Pages.CompetitionPage
{

    /// <summary>
    /// Code related to CompetitionPage's Manage Bets-tab.
    /// </summary>
    public partial class CompetitionPage
    {
        /// <summary>
        /// Returns manage bets structure.
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private Tab GetManageBetsTab(List<Target> targets)
        {
            return new Tab(
                "manageBets",
                "Manage bets",
                new List<Component>
                {
                    GetBetTargetsContainer(targets),
                    new PageActionButton(
                        "AddBetContainer",
                        new List<string>{ "betTargets" },
                        "Add bet",
                        componentsToInclude: new List<string>{ "betTargets" })
                });
        }

        /// <summary>
        /// Gets bet targets container.
        /// </summary>
        /// <param name="competitionTargets"></param>
        private Container GetBetTargetsContainer(List<Target> competitionTargets)
        {
            var targets = new List<Component>();

            for (var i = 0; i < competitionTargets.Count; i++)
            {
                targets.Add(CreateTargetContainer(i, competitionTargets[i].Type));
            }

            var components = new List<Component>
            {
                new PageActionButton(
                            "CancelBetTargetsUpdate",
                            new List<string>{ "betTargets" },
                            "Cancel bet targets update",
                            style: "outline-danger",
                            requireConfirm: true,
                            componentsToInclude: new List<string>{ "betTargets" }),
                        new PageActionButton(
                            "SaveBetTargets",
                            new List<string>{ "betTargets" },
                            "Save bet targets",
                            requireConfirm: true)
            };

            components.AddRange(targets);

            return new Container(
                components,
                componentKey: "betTargets",
                storeDataAsArray: true);
        }

        /// <summary>
        /// Adds a new target to bet container.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpResponseMessage AddBetContainer(PageAction action)
        {
            var betTargetContainer = ComponentTools<Container>.GetComponentFromAction(action, "betTargets");

            var betTargetData = action.Parameters.ContainsKey("betTargets")
                ? action.Parameters?["betTargets"] as JArray
                : null;

            if (betTargetData == null || betTargetData.Count == 0)
            {
                var defaultBetTarget = CreateTargetContainer(0, TargetType.OpenQuestion);

                betTargetContainer.Children.Add(new Container(
                    new List<Component>
                    {
                        new PageActionButton(
                            "CancelBetTargetsUpdate",
                            new List<string>{ "betTargets" },
                            "Cancel bet targets update",
                            style: "outline-danger",
                            requireConfirm: true,
                            componentsToInclude: new List<string>{ "betTargets" }),
                        new PageActionButton(
                            "SaveBetTargets",
                            new List<string>{ "betTargets" },
                            "Save bet targets",
                            requireConfirm: true)
                    }));

                betTargetContainer.Children.Add(defaultBetTarget);
            }
            else
            {
                var betTargetCount = betTargetData.Count;

                var previouslyCreatedTargetTypeAsString = betTargetData.Last()[$"bet-target-{betTargetCount - 1}"][$"bet-type-{betTargetCount - 1}"].ToObject<string>();

                var newTargetType = EnumHelper.FromString<TargetType>(previouslyCreatedTargetTypeAsString);
                var newBetTarget = CreateTargetContainer(betTargetCount, newTargetType);
                betTargetContainer.Children.Add(newBetTarget);
            }

            return Http.CreateResponse(HttpStatusCode.OK, new PageActionResponse(betTargetContainer));
        }

        /// <summary>
        /// Creates a target.
        /// </summary>
        /// <param name="index"></param>
        private Container CreateTargetContainer(int index, TargetType targetType)
        {
            var options = new List<Option>
                {
                    new Option("result", "Result", initialValue: targetType == TargetType.Result),
                    new Option("selection", "Selection", initialValue: targetType == TargetType.Selection),
                    new Option("openQuestion", "Open question", initialValue: targetType == TargetType.OpenQuestion)
                };

            var betTypeDropdown = new Dropdown(
                $"bet-type-{index}",
                "Bet type",
                options,
                new List<string> { $"bet-target-{index}" });

            switch (targetType)
            {
                default:
                    throw new NotImplementedException($"{targetType} container not implemented");

                case TargetType.OpenQuestion:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextArea, dataKey: $"question-{index}"),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double, dataKey: $"scoring-{index}")
                     },
                     $"bet-target-{index}");

                case TargetType.Result:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox, dataKey: $"question-{index}"),
                        new Field($"scoring-{index}", "Points for correct result", FieldType.Double, dataKey: $"scoring-{index}"),
                        new Field($"winner-{index}", "Points for correct winner", FieldType.Double, dataKey: $"winner-{index}")
                     },
                     $"bet-target-{index}");

                case TargetType.Selection:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox, dataKey: $"question-{index}"),
                        new InputDropdown($"selection-{index}", "Selections", dataKey: $"selection-{index}"),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double, dataKey: $"scoring-{index}")
                     },
                     $"bet-target-{index}");
            }
        }

        /// <summary>
        /// Clears new changes to bet targets. Does not remove already save data.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpResponseMessage CancelBetTargetsUpdate(PageAction action)
        {
            var betTargetsContainer = ComponentTools<Container>.GetComponentFromAction(action, "betTargets");

            betTargetsContainer.Clear();

            return Http.CreateResponse(HttpStatusCode.OK, new PageActionResponse(betTargetsContainer)
            {
                Data = new Dictionary<string, object>
                {
                    // Clear bet targets data
                    {"betTargets", new { } }
                }
            });
        }

        /// <summary>
        /// Save bet targets.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpResponseMessage SaveBetTargets(PageAction action)
        {
            var targetData = action.Parameters["betTargets"] as JArray;

            var competitionId = (int)action.PageId;

            var competition = CompetitionAction.GetCompetition(competitionId);

            if (competition.State != CompetitionState.Open)
            {
                return Http.CreateResponse(
                        HttpStatusCode.Conflict,
                        new PageActionResponse("Competition has started, no new bets can be created"));
            }

            List<Target> targets = new List<Target>();
            // Create targets
            for (int i = 0; i < targetData.Count; i++)
            {
                var targetObject = targetData[i]as JObject;

                var target = Target.FromJObject(targetObject, i, competitionId);

                if (string.IsNullOrWhiteSpace(target.Bet))
                {
                    return Http.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new PageActionResponse($"Row {i + 1}: No question given"));
                }

                if (!target.HasScoringType(TargetScore.CorrectResult))
                {
                    return Http.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new PageActionResponse($"Row {i + 1}: Missing points for correct result"));
                }

                if (target.Type == TargetType.Result && !target.HasScoringType(TargetScore.CorrectWinner))
                {
                    return Http.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new PageActionResponse($"Row {i + 1}: Missing points for correct winner"));
                }

                if (target.Type == TargetType.Selection && 
                   (target.Selections == null || target.Selections.Count == 0))
                {
                    return Http.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new PageActionResponse($"Row {i + 1}: No selections given for selection typed bet"));
                }

                targets.Add(target);
            }

            // Delete previous targets before adding new ones
            TargetAction.ClearTargets((int)action.PageId);

            TargetAction.AddTargets(action.UserId, competitionId, targets);

            return Http.CreateResponse(
                HttpStatusCode.OK, 
                new PageActionResponse("Targets added successfully", refresh: true));
        }
    }
}
