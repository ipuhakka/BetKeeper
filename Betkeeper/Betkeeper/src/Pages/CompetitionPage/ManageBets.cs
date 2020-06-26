using Betkeeper.Classes;
using Betkeeper.Extensions;
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
        private Tab GetManageBetsTab()
        {
            // TODO: Hae jo olemassa olevat targetit ja 
            // luo niille databoundeilla fieldiellä varustetu containerit
            return new Tab(
                "manageBets",
                "Manage bets",
                new List<Component>
                {
                    new Container(
                        new List<Component>(),
                        componentKey: "betTargets",
                        storeDataAsArray: true),
                    new PageActionButton(
                        "AddBetContainer",
                        new List<string>{ "betTargets" },
                        "Add bet",
                        componentsToInclude: new List<string>{ "betTargets" })
                });
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
        /// Creates a target. Called from HandleDropdownUpdate.
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
                        new Field($"question-{index}", "Bet", FieldType.TextArea),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                     },
                     $"bet-target-{index}");

                case TargetType.Result:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox),
                        new Field($"scoring-{index}", "Points for correct result", FieldType.Double),
                        new Field($"winner-{index}", "Points for correct winner", FieldType.Double)
                     },
                     $"bet-target-{index}");

                case TargetType.Selection:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox),
                        new InputDropdown($"selection-{index}", "Selections"),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
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
