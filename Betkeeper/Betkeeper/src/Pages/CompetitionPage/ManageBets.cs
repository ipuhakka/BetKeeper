using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Pages
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
            var components = JObject.Parse(action.Parameters["components"].ToString());

            var betTargetsAsJObject = components["betTargets"];

            var betTargetContainer = ComponentParser
                .ParseComponent(betTargetsAsJObject.ToString()) as Container;

            var betTargetData = action.Parameters.ContainsKey("betTargets")
                ? action.Parameters?["betTargets"] as JArray
                : null;

            if (betTargetData == null || betTargetData.Count == 0)
            {
                var defaultBetTarget = CreateTargetContainer(0, TargetType.OpenQuestion);

                betTargetContainer.Children.Add(new Container(
                    new List<Component>
                    {
                        // TODO: DataKeyt
                        new PageActionButton(
                            "cancelBetTargetsUpdate",
                            new List<string>(),
                            "Cancel bet targets update",
                            style: "outline-danger",
                            requireConfirm: true),
                        new PageActionButton(
                            "saveBetTargets",
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
                        new Dropdown($"selection-{index}", "Selections", new List<Option>()),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                     },
                     $"bet-target-{index}");
            }
        }
    }
}
