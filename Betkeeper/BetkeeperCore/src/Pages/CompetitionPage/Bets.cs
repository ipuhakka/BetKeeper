using Betkeeper.Models;
using Betkeeper.Page.Components;
using Betkeeper.Enums;
using System.Linq;
using System.Collections.Generic;
using Betkeeper.Page;
using Newtonsoft.Json.Linq;
using System;

namespace Betkeeper.Pages.CompetitionPage
{
    public partial class CompetitionPage
    {
        /// <summary>
        /// Returns bets tab components
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private static Tab GetBetsTab(List<Target> targets)
        {
            return new Tab(
                "bets",
                "Bets",
                new List<Component> { GetBetsView(targets) }
            );
        }

        /// <summary>
        /// Get bets view components.
        /// </summary>
        /// <param name="targets"></param>
        private static Container GetBetsView(List<Target> targets)
        {
            var components = new List<Component>();

            if (targets.Count > 0)
            {
                components.Add(new PageActionButton(
                    "CancelUserBetsUpdate",
                    new List<string> { "betsContainer" },
                    "Cancel bet updates",
                    buttonStyle: "outline-danger",
                    requireConfirm: true,
                    componentsToInclude: new List<string> { "betTargets" }));

                components.Add(new PageActionButton(
                    "SaveUserBets",
                    new List<string> { "betsContainer" },
                    "Save bets",
                    requireConfirm: true));
            }

            var targetGroups = targets.GroupBy(target => target.Grouping);

            foreach (var group in targetGroups)
            {
                var groupTargets = group.ToList();
                var targetComponents = new List<Component>();

                // No grouping defined, don't add to panel
                if (string.IsNullOrEmpty(group.Key))
                {
                    components.AddRange(groupTargets
                        .Select(target => GetBetContainer(target)));
                }
                else
                {
                    foreach (var target in groupTargets)
                    {
                        targetComponents.Add(GetBetContainer(target));
                    }

                    components.Add(new Panel(
                        targetComponents,
                        group.Key ?? ""));
                }
            }

            return new Container(
                children: components,
                componentKey: "betsContainer",
                storeDataAsArray: true);
        }

        /// <summary>
        /// Get bet container for a target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static Container GetBetContainer(Target target)
        {
            var children = new List<Component>();
            var cssClass = "bets ";

            switch (target.Type)
            {
                case TargetType.Selection:
                    cssClass += "selection";
                    children.Add(
                        new Dropdown(
                            $"bet-answer-{target.TargetId}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            target.Selections));
                    break;

                case TargetType.OpenQuestion:
                    cssClass += "openQuestion";
                    children.Add(
                        new Field(
                            $"bet-answer-{target.TargetId}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            FieldType.TextArea));
                    break;

                case TargetType.Result:
                    cssClass += "result";
                    children.Add(
                        new Field(
                            $"bet-answer-{target.TargetId}",
                            $"{target.Bet} ({target.GetPointInformation()})",
                            FieldType.TextBox)
                        {
                            AutoFormatter = AutoFormatter.Result
                        });
                    break;

                case TargetType.MultiSelection:
                    cssClass += "selection";
                    children.Add(
                        new Dropdown(
                            $"bet-answer-{target.TargetId}",
                            $"{target.Bet} ({target.GetPointInformation()})",
                            target.Selections,
                            multiple: true)
                        {
                            AllowedSelectionCount = target.AllowedSelectionCount
                        });
                    break;

                default:
                    throw new NotImplementedException($"Unhandled target type {target.Type}");
            }

            return new Container(children, $"target-{target.TargetId}")
            {
                CustomCssClass = cssClass
            };
        }

        /// <summary>
        /// Save users bets for competition.
        /// </summary>
        /// <param name="pageAction"></param>
        /// <returns></returns>
        private PageActionResponse SaveUserBets(PageAction pageAction)
        {
            var asJArray = pageAction.Parameters["betsContainer"] as JArray;

            var targetBets = new List<TargetBet>();
            foreach (var jToken in asJArray)
            {
                targetBets.Add(TargetBet.FromJObject(jToken as JObject));
            }

            TargetBetAction.SaveTargetBets((int)pageAction.PageId, pageAction.UserId, targetBets);

            return new PageActionResponse(
                ActionResultType.OK,
                "Bets saved succesfully",
                refresh: true);
        }
    }
}
