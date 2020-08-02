using Betkeeper.Models;
using Betkeeper.Page.Components;
using Betkeeper.Enums;
using System.Linq;
using System.Collections.Generic;

namespace Betkeeper.Pages.CompetitionPage
{
    public partial class CompetitionPage
    {
        /// <summary>
        /// Returns bets tab components
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private Tab GetBetsTab(List<Target> targets)
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
        private Container GetBetsView(List<Target> targets)
        {
            var components = new List<Component>();

            if (targets.Count > 0)
            {
                components.Add(new PageActionButton(
                    "CancelUserBetsUpdate",
                    new List<string> { "betsContainer" },
                    "Cancel bet updates",
                    style: "outline-danger",
                    requireConfirm: true,
                    componentsToInclude: new List<string> { "betTargets" }));

                components.Add(new PageActionButton(
                    "SaveUserBets",
                    new List<string> { "betsContainer" },
                    "Save bets",
                    requireConfirm: true));
            }
            components.AddRange(targets
                .Select((target, i) => GetBetContainer(target, i)));

            return new Container(
                children: components,
                componentKey: "betsContainer");
        }

        /// <summary>
        /// Get bet container for a target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Container GetBetContainer(Target target, int index)
        {
            var children = new List<Component>();

            switch (target.Type)
            {
                case TargetType.Selection:
                    children.Add(
                        new Dropdown(
                            $"bet-selection-{index}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            target.Selections));
                    break;

                case TargetType.OpenQuestion:
                    children.Add(
                        new Field(
                            $"bet-answer-{index}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            FieldType.TextArea));
                    break;

                case TargetType.Result:
                    children.Add(
                        new Field(
                            $"bet-answer-{index}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            FieldType.TextBox));
                    break;
            }

            return new Container(children, "betsContainer", storeDataAsArray: true);
        }

        // TODO: Tallennusaction

        // TODO: Muutoksien peruutusaction
    }
}
