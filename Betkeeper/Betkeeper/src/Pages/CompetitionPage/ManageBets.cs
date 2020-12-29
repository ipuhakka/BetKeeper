using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Extensions;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Adds a new target to bet container.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse AddBetContainer(PageAction action)
        {
            var betTargetContainer = Component.GetComponentFromAction<Container>(action, "betTargets");

            var betTargetData = action.Parameters.ContainsKey("betTargets")
                ? action.Parameters?["betTargets"] as JArray
                : null;

            if (betTargetData == null || betTargetData.Count == 0)
            {
                var defaultBetTarget = CreateTargetContainer(0, TargetType.OpenQuestion);

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

            return new PageActionResponse(betTargetContainer);
        }

        /// <summary>
        /// Clears new changes to bet targets. Does not remove already saved data.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse GetTargetsFromDatabase(PageAction action)
        {
            var targets = TargetAction.GetTargets((int)action.PageId);

            var betTargetsContainer = GetBetTargetsContainer(targets);

            return new PageActionResponse(betTargetsContainer)
            {
                Data = new Dictionary<string, object>
                {
                    // Clear bet targets data
                    {"betTargets", TargetsToJObject(targets) }
                }
            };
        }

        /// <summary>
        /// Save bet targets.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse SaveBetTargets(PageAction action)
        {
            var competitionId = (int)action.PageId;

            var competition = CompetitionAction.GetCompetition(competitionId);

            if (competition.State != CompetitionState.Open)
            {
                return new PageActionResponse(ActionResultType.Conflict, "Competition has started, no new bets can be created");
            }

            List<Target> targets = new List<Target>();
            var targetData = action.Parameters["betTargets"] as JArray;
            // Create targets
            for (int i = 0; i < targetData.Count; i++)
            {
                var targetObject = targetData[i]as JObject;
                var target = Target.FromJObject(targetObject, i, competitionId);
                targets.Add(target);
            }

            TargetAction.HandleTargetsUpdate(action.UserId, competitionId, targets);

            return new PageActionResponse(
                ActionResultType.OK,
                "Targets added successfully",
                refresh: true);
        }

        /// <summary>
        /// Deletes a target.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse DeleteTarget(PageAction action)
        {
            /*
             * 1: Fetch target-id- value. 
             *  - If > 0, remove matching bet target.
             * 2: Get bet-target-key.
             * 3: Remove match from data and components.
             */
            var deleteTargetId = action.Parameters.GetInt(
                action.Parameters
                    .FirstOrDefault(kvp => kvp.Key.StartsWith("target-id"))
                    .Key) ?? 0;

            if (deleteTargetId > 0)
            {
                TargetAction.RemoveTarget(deleteTargetId);
            }

            var deleteDataIndex = Convert.ToInt32(action.Parameters
                .First(kvp => kvp.Key.StartsWith("bet-target-"))
                .Key
                .Split('-')
                .Last());

            var betTargetData = action.Parameters?["betTargets"] as JArray;

            var targets = Target.JArrayToTargets(betTargetData);

            // Remove deleted one
            targets.RemoveAt(deleteDataIndex);

            var betTargetContainer = GetBetTargetsContainer(targets);
            return new PageActionResponse(betTargetContainer)
            {
                Data = new Dictionary<string, object>
                    {
                        { "betTargets", TargetsToJObject(targets) }
                    },
                Refresh = true
            };
        }

        /// <summary>
        /// Gets bet targets container.
        /// </summary>
        /// <param name="competitionTargets"></param>
        private Container GetBetTargetsContainer(List<Target> competitionTargets)
        {
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

            var targets = new List<Component>();
            for (var i = 0; i < competitionTargets.Count; i++)
            {
                targets.Add(CreateTargetContainer(
                    i,
                    competitionTargets[i].Type));
            }

            components.AddRange(targets);

            return new Container(
                components,
                componentKey: "betTargets",
                storeDataAsArray: true);
        }

        /// <summary>
        /// Creates a target.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="targetType"></param>
        /// <param name="includeDeleteButton"></param>
        private Container CreateTargetContainer(
            int index,
            TargetType targetType)
        {
            var options = new List<Option>
            {
                new Option("result", "Result", initialValue: targetType == TargetType.Result),
                new Option("selection", "Selection", initialValue: targetType == TargetType.Selection),
                new Option("openQuestion", "Open question", initialValue: targetType == TargetType.OpenQuestion)
            };

            var components = new List<Component>
            {
                new Dropdown(
                $"bet-type-{index}",
                "Bet type",
                options,
                new List<string> { $"bet-target-{index}" })
            };

            switch (targetType)
            {
                default:
                    throw new NotImplementedException($"{targetType} container not implemented");

                case TargetType.OpenQuestion:
                    components.AddRange(
                        new List<Component>
                        {
                            new Field($"question-{index}", "Bet", FieldType.TextArea),
                            new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                        });
                    break;

                case TargetType.Result:
                    components.AddRange(
                        new List<Component>
                        {
                            new Field($"question-{index}", "Bet", FieldType.TextBox, dataKey: $"question-{index}"),
                            new Field($"scoring-{index}", "Points for correct result", FieldType.Double),
                            new Field($"winner-{index}", "Points for correct winner", FieldType.Double),
                        });
                    break;

                case TargetType.Selection:
                    components.AddRange(
                        new List<Component>
                        {
                            new Field($"question-{index}", "Bet", FieldType.TextBox),
                            new InputDropdown($"selection-{index}", "Selections"),
                            new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                        });
                    break;
            }

            components.Add(new PageActionButton(
                "DeleteTarget",
                new List<string> { $"target-id-{index}", $"bet-target-{index}", "betTargets" },
                "Delete",
                componentsToInclude: new List<string> { "betTargets" },
                style: "outline-danger",
                requireConfirm: true,
                displayType: DisplayType.Icon)
                {
                    IconName = "far fa-trash-alt"
                });

            return new Container(
                components,
                $"bet-target-{index}")
            {
                CustomCssClass = $"manageBets {targetType.ToString().ToCamelCase()}"
            };
        }
    }
}
