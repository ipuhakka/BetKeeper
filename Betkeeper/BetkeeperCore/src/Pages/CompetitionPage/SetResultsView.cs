using Betkeeper.Enums;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Pages.CompetitionPage
{
    public partial class CompetitionPage
    {
        /// <summary>
        /// Returns tab for setting bet results.
        /// </summary>
        /// <param name="bets"></param>
        /// <returns></returns>
        private Tab GetSetResultsTab(Dictionary<Target, List<TargetBet>> bets)
        {
            var groups = bets.GroupBy(kvp => kvp.Key.Grouping);

            var viewComponents = new List<Component>();
            foreach (var group in groups)
            {
                var groupComponents = group.Select(kvp => GetComponentsForTarget(kvp.Key, kvp.Value)).ToList();

                if (!string.IsNullOrEmpty(group.Key))
                {
                    var panel = new Panel(groupComponents, group.Key);
                    viewComponents.Add(panel);
                }
                else
                {
                    viewComponents.AddRange(groupComponents);
                }
            }

            var container = new Container(
                viewComponents,
                "setResultsContainer",
                storeDataAsArray: true);

            return new Tab(
                "setResultsTab", 
                "Set bet results", 
                new List<Component>
                {
                    container,
                    new PageActionButton(
                    "SaveBetResults",
                    new List<string>{ "setResultsContainer" },
                    "Save bet results",
                    requireConfirm: true)
                });
        }

        private Component GetComponentsForTarget(
            Target target, 
            List<TargetBet> targetBets)
        {
            var content = new List<Component>
            {
                new Field(
                $"question-{target.TargetId}",
                "Bet",
                target.Bet.Length > 35 
                    ? FieldType.TextArea
                    : FieldType.TextBox,
                readOnly: true)
            };

            switch (target.Type)
            {
                case TargetType.Result:
                    content.Add(new Field(
                        $"result-{target.TargetId}",
                        "Result",
                        FieldType.TextBox)
                    {
                        AutoFormatter = AutoFormatter.Result
                    });
                    break;
                case TargetType.Selection:
                    var options = new List<Option>
                    {
                        new Option("UNRESOLVED-BET", "Unresolved")
                    };

                    options.AddRange(target.Selections
                        .Select(selection => new Option(selection, selection)));
                    content.Add(new Dropdown(
                        $"result-{target.TargetId}",
                        "Result",
                        options.ToList()));
                    break;
                case TargetType.OpenQuestion:
                    content.Add(new ModalActionButton(
                        action: "",
                        components: GetSetOpenQuestionResultModal(
                            target,
                            targetBets
                                .Where(targetBet => targetBet.Target == target.TargetId)
                                .ToList()),
                        text: "Set result",
                        absoluteDataPath: "setResultsContainer"));
                    break;
                case TargetType.MultiSelection:
                    var selections = target.Selections
                        ?.Select(selection => new Option(selection, selection))
                        .ToList() ?? new List<Option>();

                    content.Add(new Dropdown(
                        $"result-{target.TargetId}",
                        "Correct answers",
                        selections,
                        multiple: true)
                    {
                        AllowedSelectionCount = target.AllowedSelectionCount
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }

            var customCssClass = "setResults ";

            switch (target.Type)
            {
                case Enums.TargetType.OpenQuestion:
                    customCssClass += "openQuestion";
                    break;
                case Enums.TargetType.Result:
                    customCssClass += "result";
                    break;
                case Enums.TargetType.Selection:
                    customCssClass += "selection";
                    break;
            }

            return new Container(content, $"setResultsContainer-{target.TargetId}")
            {
                CustomCssClass = customCssClass
            };
        }

        /// <summary>
        /// Gets modal
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetBets"></param>
        /// <returns></returns>
        private List<Component> GetSetOpenQuestionResultModal(Target target, List<TargetBet> targetBets)
        {
            var targetBetContainerChildren = new List<Component>
            {
                new Field($"question-{target.TargetId}", "Bet", fieldType: FieldType.TextBox, readOnly: true)
            };

            targetBets.ForEach(targetBet =>
            {
                targetBetContainerChildren.Add(
                    new Container(
                        children: new List<Component>
                        {
                            new Field($"answer-{target.TargetId}-{targetBet.TargetBetId}", "Answer", FieldType.TextArea, readOnly: true),
                            new Dropdown(
                                $"result-{target.TargetId}-{targetBet.TargetBetId}",
                                "Result",
                                new List<Option>
                                {
                                    new Option("Correct", "Correct"),
                                    new Option("Wrong", "Wrong"),
                                    new Option("Unresolved", "Unresolved")
                                })
                        })
                    {
                        CustomCssClass = "setBetResultsModalIndividualContainer"
                    });
            });

            return new List<Component>
            {
                new Container(
                    targetBetContainerChildren,
                    componentKey: $"setResultsContainer-{target.TargetId}")
            };
        }

        private PageActionResponse SaveBetResults(PageAction action)
        {
            var resultsJArray = action.Parameters["setResultsContainer"] as JArray;

            var targetResultItems = new List<TargetResultItem>();

            for (var i = 0; i < resultsJArray.Count; i++)
            {
                targetResultItems.Add(new TargetResultItem(resultsJArray[i] as JObject));
            }

            var targets = TargetAction.GetTargets((int)action.PageId);

            targets.ForEach(target =>
            {
                target.Result = targetResultItems.Single(targetResultItem => targetResultItem.TargetId == target.TargetId);
            });

            TargetAction.SetTargetResults((int)action.PageId, action.UserId, targets);

            return new PageActionResponse(
                ActionResultType.OK,
                "Results saved succesfully",
                refresh: true);
        }
    }
}
