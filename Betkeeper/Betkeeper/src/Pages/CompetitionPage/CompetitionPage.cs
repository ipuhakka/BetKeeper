using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Extensions;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Pages.CompetitionPage
{
    /// <summary>
    /// A competition page structure.
    /// </summary>
    public partial class CompetitionPage : PageBase
    {
        private CompetitionAction CompetitionAction { get; }

        private TargetAction TargetAction { get; }

        private TargetBetAction TargetBetAction { get; }

        private Dictionary<string, object> Data { get; set; }

        public CompetitionPage()
        {
            CompetitionAction = new CompetitionAction();
            TargetAction = new TargetAction();
            TargetBetAction = new TargetBetAction();
        }

        public CompetitionPage(
            CompetitionAction competitionAction = null, 
            TargetAction targetAction = null,
            TargetBetAction targetBetAction = null)
        {
            CompetitionAction = competitionAction;
            TargetAction = targetAction;
            TargetBetAction = targetBetAction;
        }

        public override PageResponse GetPage(string pageKey, int userId)
        {
            Data = new Dictionary<string, object>();

            var competitionId = int.Parse(pageKey);

            var participator = CompetitionAction.GetParticipator(userId, competitionId);
            var competition = CompetitionAction.GetCompetition(competitionId);

            // User does not have rights to competition, redirect
            if (participator == null)
            {
                return new PageResponse(redirectTo: "../competitions");
            }

            var competitionTargets = TargetAction.GetTargets(competitionId);

            var tabs = new List<Component>
            {
                // General view
                new Tab(
                "home",
                "Home",
                new List<Component>
                {
                    new Field("name", "Name", true, FieldType.TextBox, "competition.name"),
                    new Field("joinCode", "Join code", true, FieldType.TextBox, "competition.joinCode"),
                    new Field("description", "Description", true, FieldType.TextArea, "competition.description"),
                    new DateTimeInput("startTime", "Start", readOnly: true, dataKey: "competition.startTime"),
                    new Field("state", "State", true, FieldType.TextBox, "competition.state")
                })
            };

            if (competition.State == CompetitionState.Open)
            {
                tabs.Add(GetBetsTab(competitionTargets));
            }
            else
            {
                // Results
                tabs.Add(GetResultsTab(competitionId));
            }

            if (participator.Role == CompetitionRole.Host)
            {
                var targetBets = new List<TargetBet>();
                if (competition.State == CompetitionState.Open)
                {
                    tabs.Add(GetManageBetsTab(competitionTargets));
                }
                if (competition.State != CompetitionState.Open)
                {
                    // Setting bet results
                    targetBets = TargetBetAction.GetCompetitionsTargetBets(competition.CompetitionId);

                    var betDict = new Dictionary<Target, List<TargetBet>>();

                    // Add dictionary mapping for target and its target bets
                    competitionTargets.ForEach(target =>
                    {
                        betDict.Add(target, targetBets
                            .Where(targetBet =>
                                targetBet.Target == target.TargetId)
                            .ToList());
                    });
                    Data.Add("setResultsContainer", TargetResultsToJObject(competitionTargets, targetBets));

                    tabs.Add(GetSetResultsTab(betDict));
                }

                tabs.Add(new Tab(
                    "settings",
                    "Settings",
                    new List<Component>
                    {
                        new PageActionButton(
                            "DeleteCompetition",
                            new List<string> { "competitionId" },
                            "Delete competition",
                            "outline-danger",
                            requireConfirm: true,
                            navigateTo: "../competitions")
                    }));
            }

            var usersBets = TargetBetAction.GetParticipatorsBets(participator.ParticipatorId);

            Data.Add("CompetitionId", competitionId);
            Data.Add("Competition", competition);
            Data.Add("betTargets", TargetsToJObject(competitionTargets));
            Data.Add("betsContainer", TargetBetsToJObject(usersBets));

            return new PageResponse($"competitions/{pageKey}", tabs, Data);
        }

        public override PageActionResponse HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                default:
                    throw new NotImplementedException($"{action.ActionName} not implemented");

                case "DeleteCompetition":
                    return DeleteCompetition(action);

                case "AddBetContainer":
                    return AddBetContainer(action);

                case "CancelBetTargetsUpdate":
                    //Clears new changes to bet targets. Does not remove already save data.
                    return GetTargetsFromDatabase(action);

                case "SaveBetTargets":
                    return SaveBetTargets(action);

                case "DeleteTarget":
                    return DeleteTarget(action);

                case "SaveUserBets":
                    return SaveUserBets(action);

                case "CancelUserBetsUpdate":
                    return new PageActionResponse(ActionResultType.OK)
                        {
                            Refresh = true
                        };

                case "SaveBetResults":
                    return SaveBetResults(action);
            }
        }

        public override HttpResponseMessage HandleDropdownUpdate(
            Dictionary<string, object> data,
            int? pageId = null)
        {
            var componentKey = data.GetString("key");
            var value = data.GetString("value");

            if (data.ContainsKey("components"))
            {
                if (componentKey.Contains("bet-type-"))
                {
                    var newTargetType = EnumHelper.FromString<TargetType>(value);

                    // Get index, format of component key is bet-type-{index}
                    var index = int.Parse(componentKey.Split('-').Last());

                    return Http.CreateResponse(
                        HttpStatusCode.OK,
                        new PageResponse(
                            new List<Component>
                            {
                                CreateTargetContainer(index, newTargetType)
                            })
                        );
                }
            }

            throw new ArgumentException($"{componentKey} options update not implemented");
        }

        private PageActionResponse DeleteCompetition(PageAction action)
        {
            var competitionId = action.Parameters.GetInt("competitionId");

            if (competitionId == null)
            {
                return new PageActionResponse(ActionResultType.InvalidInput, "Missing CompetitionId parameter");
            }

            try
            {
                CompetitionAction.DeleteCompetition(action.UserId, (int)competitionId);

                return new PageActionResponse(ActionResultType.OK, "Competition deleted successfully");
            }
            catch (InvalidOperationException)
            {
                return new PageActionResponse(ActionResultType.Unauthorized, "User not allowed to delete competition");
            }
        }
    }
}
