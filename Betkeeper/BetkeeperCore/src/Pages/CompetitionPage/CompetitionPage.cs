﻿using Betkeeper.Actions;
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
    /// A competition page structure.
    /// </summary>
    public partial class CompetitionPage : PageBase
    {
        public override string PageKey => "competition";

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

            var competitionTargets = TargetAction.GetTargets(competitionId)
                .OrderBy(target => target.Grouping)
                .ThenBy(target => target.TargetId)
                .ToList();

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

                var settingsTab = new Tab(
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
                    });

                if (competition.State == CompetitionState.Open)
                {
                    settingsTab.TabContent.Add(new ModalActionButton(
                        "InviteUsers",
                        new List<Component>
                        {
                            new InputDropdown("usersToInvite", "Users to invite")
                        },
                        "Invite users",
                        requireConfirm: true));
                }

                tabs.Add(settingsTab);
            }

            var usersBets = TargetBetAction.GetParticipatorsBets(participator.ParticipatorId);

            Data.Add("competitionId", competitionId);
            Data.Add("competition", competition);
            Data.Add("betTargets", TargetsToJObject(competitionTargets));
            Data.Add("betsContainer", TargetBetsToJObject(usersBets));

            return new PageResponse($"competition/{pageKey}", tabs, Data);
        }

        public override PageActionResponse HandleAction(PageAction action)
        {
            return action.ActionName switch
            {
                "DeleteCompetition" => DeleteCompetition(action),
                "AddBetContainer" => AddBetContainer(action),
                "AddBetContainerToPanel" => AddBetContainerToPanel(action),
                // Clears new changes to bet targets. Does not remove already save data.
                "CancelBetTargetsUpdate" => GetTargetsFromDatabase(action),
                "SaveBetTargets" => SaveBetTargets(action),
                "DeleteTarget" => DeleteTarget(action),
                "SaveUserBets" => SaveUserBets(action),
                "CancelUserBetsUpdate" => new PageActionResponse(ActionResultType.OK)
                {
                    Refresh = true
                },
                "SaveBetResults" => SaveBetResults(action),
                "AddGroup" => AddGroup(action),
                "InviteUsers" => InviteUsers(action),
                _ => throw new NotImplementedException($"{action.ActionName} not implemented"),
            };
        }

        /// <summary>
        /// Invite users to competition
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PageActionResponse InviteUsers(PageAction action)
        {
            var usersToInvite = action.Parameters.ContainsKey("usersToInvite")
                ? ((JArray)action.Parameters["usersToInvite"]).ToObject<List<string>>()
                : null;

            if (usersToInvite == null || usersToInvite.Count == 0)
            {
                return new PageActionResponse(ActionResultType.InvalidInput, "No users selected");
            }

            CompetitionInvitationAction.InviteUsers((int)action.PageId, action.UserId, usersToInvite);
            return new PageActionResponse(ActionResultType.OK, "Invites sent to users with valid usernames");
        }

        public override PageResponse HandleDropdownUpdate(DropdownUpdateParameters parameters)
        {
            var data = parameters.Data;
            var componentKey = data.GetString("key");
            var value = data.GetString("value");

            if (data.ContainsKey("components"))
            {
                if (componentKey.Contains("bet-type-"))
                {
                    var newTargetType = EnumHelper.FromString<TargetType>(value);

                    // Get index, format of component key is bet-type-{index}
                    var index = int.Parse(componentKey.Split('-').Last());

                    return new PageResponse(
                            new List<Component>
                            {
                                CreateTargetContainer(index, newTargetType)
                            });
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
