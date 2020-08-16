using Betkeeper.Models;
using Betkeeper.Page.Components;
using Betkeeper.Enums;
using System.Linq;
using System.Collections.Generic;
using Betkeeper.Page;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Betkeeper.Classes;

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
                .Select(target => GetBetContainer(target)));

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
        private Container GetBetContainer(Target target)
        {
            var children = new List<Component>();

            switch (target.Type)
            {
                case TargetType.Selection:
                    children.Add(
                        new Dropdown(
                            $"bet-answer-{target.TargetId}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            target.Selections));
                    break;

                case TargetType.OpenQuestion:
                    children.Add(
                        new Field(
                            $"bet-answer-{target.TargetId}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            FieldType.TextArea));
                    break;

                case TargetType.Result:
                    children.Add(
                        new Field(
                            $"bet-answer-{target.TargetId}", 
                            $"{target.Bet} ({target.GetPointInformation()})", 
                            FieldType.TextBox));
                    break;
            }

            return new Container(children, $"target-{target.TargetId}");
        }

        /// <summary>
        /// Save users bets for competition.
        /// </summary>
        /// <param name="pageAction"></param>
        /// <returns></returns>
        private HttpResponseMessage SaveUserBets(PageAction pageAction)
        {
            var asJArray = pageAction.Parameters["betsContainer"] as JArray;

            var targetBets = new List<TargetBet>();
            foreach (var jToken in asJArray)
            {
                var targetBet = TargetBet.FromJObject(jToken as JObject);

                // Only manage bets which have a bet inputted
                if (!string.IsNullOrEmpty(targetBet.Bet))
                {
                    targetBets.Add(targetBet);
                }
            }

            TargetBetAction.SaveTargetBets((int)pageAction.PageId, pageAction.UserId, targetBets);

            return Http.CreateResponse(
                System.Net.HttpStatusCode.OK,
                new PageActionResponse("Bets saved succesfully"));
        }

        // TODO: Muutoksien peruutusaction
    }
}
