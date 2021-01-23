using Betkeeper.Page;
using Betkeeper.Page.Components;
using System;
using System.Collections.Generic;

namespace Betkeeper.Pages.HomePage
{
    public class HomePage : PageBase
    {
        public override PageResponse GetPage(string pageKey, int userId)
        {
            return new PageResponse(
                pageKey,
                new List<Component>
                {
                    new CardMenu(
                        new List<Card>
                        {
                            new Card("fas fa-chart-bar fa-4x", "Statistics", "See how your bets have gone", "/page/statistics"),
                            new Card("fas fa-pencil-alt fa-4x", "Bets", "Add and delete bets, update unresoved bets", "/page/bets"),
                            new Card("fas fa-folder fa-4x", "Folders", "Create and delete folders", "/page/folders"),
                            new Card("fas fa-volleyball-ball fa-4x", "Competitions", "Check your competitions", "/page/competitions")
                        }, "homePageMenu")
                },
                new Dictionary<string, object>());
        }
    }
}
