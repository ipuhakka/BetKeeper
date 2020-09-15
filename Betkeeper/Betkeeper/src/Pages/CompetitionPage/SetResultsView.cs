using Betkeeper.Classes;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

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
            var container = new Container(bets
                .Select(kvp => GetComponentsForTarget(kvp.Key, kvp.Value))
                .ToList(),
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
            var content = new List<Component>();

            content.Add(new Field(
                $"question-{target.TargetId}",
                "Bet",
                FieldType.TextBox,
                readOnly: true));

            switch (target.Type)
            {
                case Enums.TargetType.Result:
                    content.Add(new Field(
                        $"result-{target.TargetId}",
                        "Result",
                        FieldType.TextBox));
                    break;
                case Enums.TargetType.Selection:
                    content.Add(new Dropdown(
                        $"result-{target.TargetId}",
                        "Result",
                        target.Selections
                            .Select(selection =>
                                new Option(selection, selection))
                            .ToList()));
                    break;
                case Enums.TargetType.OpenQuestion:
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
                default:
                    throw new NotImplementedException();
            }

            return new Container(content, $"setResultsContainer-{target.TargetId}");
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
                            new Field($"answer-{target.TargetId}-{targetBet.TargetBetId}", "Answer", FieldType.TextBox, readOnly: true),
                            new Dropdown(
                                $"result-{target.TargetId}-{targetBet.TargetBetId}", 
                                "Result", 
                                new List<Option>
                                {
                                    new Option("Correct", "Correct"),
                                    new Option("Wrong", "Wrong"),
                                    new Option("Unresolved", "Unresolved")
                                })
                        }));
            });

            return new List<Component>
            {
                new Container(
                    targetBetContainerChildren,
                    componentKey: $"setResultsContainer-{target.TargetId}")
            };
        }

        private HttpResponseMessage SaveBetResults(PageAction action)
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

            return Http.CreateResponse(
                System.Net.HttpStatusCode.OK,
                new PageActionResponse("Results saved succesfully"));
        }
    }
}
